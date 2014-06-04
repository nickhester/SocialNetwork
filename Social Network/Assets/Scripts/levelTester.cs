using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Types;

public class levelTester : MonoBehaviour {

	#region variables

	public string filename = "validSeedList.txt";
	public bool runTestSeries = false;
	public bool overWriteExistingFile = false;
	public TextAsset textFile;
	private List<levelSeedList> seedList = new List<levelSeedList>();
	private string textToWrite = "";

	public int numTriesPerLevel = 5;
	public int numLevelsToRun = 5;
	public int numClicksPerRun = 5000;
	private int numLevelsHaveBeenRun = 0;
	[HideInInspector]
	public bool isStartingSingleton = false;
	private bool hasRunTest = false;
	private class_NetworkMgr networkMgr;

	private List<Person> failCaseCheckList = new List<Person>();

	private validLevels currentLevel;
	private List<levelTesterState_s> statesThatHaveBeenReached = new List<levelTesterState_s>();
	private levelTesterState_s winState;
	private List<levelTesterState_s> listWinStates = new List<levelTesterState_s>();
	private int currentStepsTaken = 0;
	private levelTesterState_s bestWinState;
	private int stackOverflowPreventer = 0;

	#endregion

	void Awake ()
	{
		FindNetworkManager();
		List<GameObject> _temp = new List<GameObject>();
		_temp.AddRange(GameObject.FindGameObjectsWithTag("levelTester"));
		if (_temp.Count() > 1) 					// initiate singleton
		{
			foreach (GameObject GO in _temp)
			{
				if (!GO.GetComponent<levelTester>().isStartingSingleton)
				{
					Destroy(GO);
				}
			}
		}
		else { DontDestroyOnLoad(gameObject); isStartingSingleton = true; }


	}

	void Start () {

	}
	
	void Update ()
	{
		if (runTestSeries)
		{
			FindNetworkManager();
			if (!HasFailCase())
			{
				if (!hasRunTest)
				{
					Application.LoadLevel(Application.loadedLevel);
					RunOneLevelTrial();
					numLevelsHaveBeenRun++;
					AppendToFile(textToWrite);
					if (numLevelsHaveBeenRun == numLevelsToRun)
					{
						hasRunTest = true;
					}
				}
			}
		}
	}

	void FindNetworkManager()
	{
		networkMgr = GameObject.Find("networkMgr").GetComponent<class_NetworkMgr>();
	}

	// Let the computer try to find its best result to give a goal to the player
	public int ComputerPlay(int numClicks, int numTrials)
	{
		int numTries = 0;
		bool _foundSolution = false;
		
		if (HasFailCase())
		{
			networkMgr.ReloadStartingState();
			print ("---This level contains a Fail Case (definitely impossible)---");
			networkMgr.AIbutton = "F";
			return -1;
		}
		
		for (int i = 0; i < numClicks; i++)
		{
			numTries++;
			int chooseAPerson = Random.Range(0, networkMgr.allPeopleExceptPlayer.Count);
			networkMgr.TriggerRelationshipChange(networkMgr.allPeopleExceptPlayer[chooseAPerson].gameObject, true, true);
			networkMgr.CalculateScore();
			if (networkMgr.CheckWinningRequirements())
			{
				print ("---Success after " + numTries + " random clicks.---");
				_foundSolution = true;
				networkMgr.ReloadStartingState();
				networkMgr.AIbutton = numTries.ToString();
				networkMgr.AIScoreReturn = numTries;
				break;
			}
		}
		if (!_foundSolution)
		{
			networkMgr.ReloadStartingState();
			print ("---No solution found in " + numTries + " tries.---");
			networkMgr.AIbutton = "X";
			networkMgr.AIScoreReturn = -1;
		}
		return networkMgr.AIScoreReturn;
	}
	
	public bool HasFailCase()
	{	
		foreach (Person _person in networkMgr.allPeople)					// for each person
		{
			if (_person.relationshipListNegativeNoPlayer.Count > 0)	// if the person has at least 1 red relationship
			{
				failCaseCheckList.Clear();
				if (!Recursive_CheckForGreenNeighbors(_person))		// check to see if they connect to anything green, even their neighbors, etc.
				{ return true; }
				else if (_person.relationshipListNegativeNoPlayer.Count == 2 && _person.relationshipListPositiveNoPlayer.Count == 0)	// if the person has just 2 red relationships
				{
					class_Relationship _greenRel = null;
					if (_person.GetMyNeighbor(_person.relationshipListNegativeNoPlayer[0], _person).relationshipListPositiveNoPlayer.Count == 1
					    && _person.GetMyNeighbor(_person.relationshipListNegativeNoPlayer[0], _person).relationshipListNegativeNoPlayer.Count == 1)		// if they have one green and one red relationship
					{
						_greenRel = _person.GetMyNeighbor(_person.relationshipListNegativeNoPlayer[0], _person).relationshipListPositiveNoPlayer[0];				// remember that relationship
					}
					if (_person.GetMyNeighbor(_person.relationshipListNegativeNoPlayer[1], _person).relationshipListPositiveNoPlayer.Count == 1
					    && _person.GetMyNeighbor(_person.relationshipListNegativeNoPlayer[1], _person).relationshipListNegativeNoPlayer.Count == 1)		// if they also have one green and one red relationship
					{
						if (_greenRel != null && _greenRel == _person.GetMyNeighbor(_person.relationshipListNegativeNoPlayer[1], _person).relationshipListPositiveNoPlayer[0])				// if that green relationship is the same as the last one
						{ return true; }
					}
				}
			}
		}
		return false;
	}
	
	bool Recursive_CheckForGreenNeighbors(Person p)
	{
		if (failCaseCheckList.Contains(p))		// if you're already on the list, don't run again
		{
			return false;
		}
		else
		{
			failCaseCheckList.Add(p);			// add yourself to the list
			if (p.relationshipListNonZeroNoPlayer.Count > p.relationshipListNegativeNoPlayer.Count)		// if he has any positive relationships
			{
				return true;
			}
			else
			{
				foreach (class_Relationship _rel in p.relationshipListNegativeNoPlayer)
				{
					if (Recursive_CheckForGreenNeighbors(p.GetMyNeighbor(_rel, p)))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	void RunOneLevelTrial()
	{
		FindNetworkManager();
		
		for (int i = 0; i < 1; i++)
		{
			levelSeedList thisLevelSeedList = new levelSeedList();
			List<int> numTriesList = new List<int>();
			int _counter = 0;
			for (int i2 = 0; i2 < numTriesPerLevel; i2++)
			{
				int _numTries = ComputerPlay(numClicksPerRun, 1);
				if (_numTries == -1) { _counter++; }	// if it fails, increment the counter
				else
				{
					numTriesList.Add(_numTries);		// only add it to the list if it succeeds
					_counter += -99;					// if any answer is found, prevent the failsafe
				}
				if (_counter == 3) { break; }			// if it fails 3 time, fail out early
			}
			thisLevelSeedList.mySeed = networkMgr.usedSeed;						// set used seed
			if (numTriesList.Count() > 0)
			{
				thisLevelSeedList.numTimesAITook = (int)numTriesList.Average();		// set average number of clicks taken
			}
			else { thisLevelSeedList.numTimesAITook = -1; }
			thisLevelSeedList.numPeople = networkMgr.allPeople.Count() - 1;		// set how many people on this level
			thisLevelSeedList.myPercentRelationship = networkMgr.percentHasRelationship;
			string _results = string.Format("level:{0},difficulty:{1},seed:{2},clicks:{3},percentRelationship:{4}\n",
			                                thisLevelSeedList.numPeople,
			                                thisLevelSeedList.myDifficulty,
			                                thisLevelSeedList.mySeed,
			                                thisLevelSeedList.numTimesAITook,
			                                thisLevelSeedList.myPercentRelationship
			                                );
			print (_results);
			if (thisLevelSeedList.myDifficulty != Difficulty.NoSolutionFound)
			{
				textToWrite += _results;
				seedList.Add(thisLevelSeedList);
			}
		}
	}

	void AppendToFile(string _stringToAppend)
	{
		print (_stringToAppend);
		string _stringToAppendTrimmed = _stringToAppend.TrimEnd('\n');
		print (_stringToAppendTrimmed);
		if (!overWriteExistingFile)
		{
			_stringToAppendTrimmed = textFile.text + _stringToAppendTrimmed;
		}
		StreamWriter sw = new StreamWriter(Application.dataPath + "\\" + filename);
		sw.Write(_stringToAppendTrimmed);
		sw.Close();
	}

	#region RecursiveBestPathSearch

	public void PathfindLevel()
	{
		currentLevel = GameObject.Find("Clipboard").GetComponent<clipboard>().nextLevelUp.myLevel;

		// create winning state for reference
		List<bool> allTrue = new List<bool>();
		for (int i = 0; i < networkMgr.allPeopleExceptPlayer.Count; i++)
		{
			allTrue.Add(true);
		}
		winState = new levelTesterState_s(allTrue, true, 0);

		levelTesterState_s gameStartingState = GetLevelState();

		// make a default "best win"
		statesThatHaveBeenReached.Add(gameStartingState);
		bestWinState.numStepsToReach = 99;

		FindBestWin(0);

		if (bestWinState.numStepsToReach == 99)
		{
			print ("TESTER FEEDBACK: this level is impossible");
		}
		else
		{
			print ("TESTER FEEDBACK: requires " + bestWinState.numStepsToReach + " steps");
		}
	}

	// recursive function to find best pathway to win state
	void FindBestWin(int currentStepNumber)
	{
		stackOverflowPreventer++;
		//if (stackOverflowPreventer > 50) { print ("float back up... " + stackOverflowPreventer + "================="); return; }

		int myCurrentStepNumber = currentStepNumber += 1;
		if (myCurrentStepNumber > bestWinState.numStepsToReach) { return; }

		print ("### " + myCurrentStepNumber + " -- " + stackOverflowPreventer);

		levelTesterState_s thisLevelState = GetLevelState();
		foreach (actionPossibility possibleAction in FindAllPossibleActions())
		{
			//print ("loop...");

			ReturnToPreviousLevelState(thisLevelState);						// make sure you're starting in the right state
			PerformAnAction(possibleAction);								// perform the chosen action
			levelTesterState_s _newState = GetLevelState();					// find the state that this action takes you to
			_newState.numStepsToReach = myCurrentStepNumber;

			if (_newState == winState) { 										// if it's a win state
				if (_newState.numStepsToReach < bestWinState.numStepsToReach) 	// and if it's a win state that takes less steps to get to
				{
					bestWinState = _newState;
				}
				listWinStates.Add(_newState);	// this may not be needed. maybe remove this list
				//print ("TESTER: found a win state");
				continue;
			}

			// if you hit a state you've hit before
			levelTesterState_s match = CheckIfMatchingStateExists(_newState);

			if (!match.isNull && _newState.numStepsToReach < match.numStepsToReach)				// this state matches a previous state, but it got there faster, so keep going
			{
				match.numStepsToReach = _newState.numStepsToReach;
				//print ("TESTER: found faster way to get to previous state");
			}
			else if (match.isNull) 							// this state does not match any existing state
			{
				//print ("TESTER: new state found");
				statesThatHaveBeenReached.Add(_newState);		// so add it to the list of found states
			}
			else if (!match.isNull) 				// this state has already been reached, and you didn't get there any faster than before
			{
				//print ("TESTER: previous state found");
				continue;
			}

			FindBestWin(myCurrentStepNumber);
		}
	}

	void PerformAnAction(actionPossibility _action)
	{
		foreach (Person _per in networkMgr.allPeopleExceptPlayer)
		{
			if (_per.personalIndex == _action.personIndex)
			{
				networkMgr.TriggerRelationshipChange(_per.gameObject, _action.isGoodAction, true);
			}
		}
	}

	levelTesterState_s GetLevelState()
	{
		List<bool> statesOfPeople = new List<bool>();
		for (int i = 0; i < networkMgr.allPeopleExceptPlayer.Count; i++)
		{
			statesOfPeople.Add(false);		// create a place holder list, defaulting everyone to false
		}
		for (int i = 0; i < networkMgr.allPeopleExceptPlayer.Count; i++)
		{
			Person _people = networkMgr.allPeopleExceptPlayer[i];
			if (_people.relWithPlayer.relationshipValue == 100)
			{
				statesOfPeople[_people.personalIndex - 1] = true;		// -1 because this list excludes the player, so the player indexes are off by one
			}
		}
		levelTesterState_s listToReturn = new levelTesterState_s();
		listToReturn.myState = statesOfPeople;
		return listToReturn;
	}

	void ReturnToPreviousLevelState(levelTesterState_s levelStateToReturnTo)
	{
		for (int i = 0; i < networkMgr.allPeopleExceptPlayer.Count; i++)
		{
			if (levelStateToReturnTo.myState[networkMgr.allPeopleExceptPlayer[i].personalIndex - 1])	// -1 because this list excludes the player, so the player indexes are off by one
			{
				networkMgr.allPeopleExceptPlayer[i].relWithPlayer.relationshipValue = 100;
			}
			else
			{
				networkMgr.allPeopleExceptPlayer[i].relWithPlayer.relationshipValue = -100;
			}
		}
	}
	
	List<actionPossibility> FindAllPossibleActions()
	{
		List<actionPossibility> listToReturn = new List<actionPossibility>();
		for (int i = 0; i < networkMgr.allPeopleExceptPlayer.Count; i++) 		// for every person
		{
			actionPossibility possibility1 = new actionPossibility(true, networkMgr.allPeopleExceptPlayer[i].personalIndex);
			actionPossibility possibility2 = new actionPossibility(false, networkMgr.allPeopleExceptPlayer[i].personalIndex);
			listToReturn.Add(possibility1);
			listToReturn.Add(possibility2);
		}
		return listToReturn;
	}

	levelTesterState_s CheckIfMatchingStateExists(levelTesterState_s stateToCompare)
	{
		foreach (levelTesterState_s _state in statesThatHaveBeenReached)
		{
//			print (stateToCompare.myState[0] + "," + stateToCompare.myState[1] + "," + stateToCompare.myState[2] +
//			       " vs " + _state.myState[0] + "," + _state.myState[1] + "," + _state.myState[2]);
			if (stateToCompare == _state)
			{ return _state; }
		}
		levelTesterState_s nullState = new levelTesterState_s();
		nullState.isNull = true;
		return nullState;			// a new state has been found, return a "null" state
	}

	#endregion
}
