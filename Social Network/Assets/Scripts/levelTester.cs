using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Types;

public class levelTester : MonoBehaviour {

	#region variables

	private string filename = "validSeedList";
	FileParse fp;
	public bool runTestSeries = false;
	public bool overWriteExistingFile = false;
	private List<levelSeedList> seedList = new List<levelSeedList>();
	
	public int numLevelsToRun = 5;
	[HideInInspector]
	public int numLevelsLeftToRun;
	[HideInInspector]
	public int levelToLoad;
	[HideInInspector]
	public bool isStartingSingleton = false;
	private class_NetworkMgr networkMgr;

	private List<Person> failCaseCheckList = new List<Person>();

	private validLevels currentLevel;
	private List<levelTesterState_s> statesThatHaveBeenReached = new List<levelTesterState_s>();
	private levelTesterState_s winState;
	private List<levelTesterState_s> listWinStates = new List<levelTesterState_s>();
	private int currentStepsTaken;
	private levelTesterState_s bestWinState;
	private Person loopStart;
	private Person lastVisitedInLoop;
	private List<Person> loopMembers = new List<Person>();
	private bool bestWinStateHasBeenFound = false;

	#endregion

	void Start () {

	}
	
	void Update ()
	{
		if (numLevelsLeftToRun > 0)
		{
			runTestAndRecord();
			numLevelsLeftToRun--;
		}
	}

	public void runTestAndRecord()
	{
		if (!HasFailCase())
		{
			RunOneLevelTrial();
		}
		GetComponent<createAndDestroyLevel>().MakeNewTestLevel(levelToLoad);
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
			int chooseAPerson = Random.Range(0, networkMgr.allPeople.Count);
			networkMgr.TriggerRelationshipChange(networkMgr.allPeople[chooseAPerson].gameObject, true, true);
			//networkMgr.CalculateScore();	// TODO: remove with mood logic
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
		FindNetworkManager();
		bool hasAtLeastOnePersonWithAllGreenRelationships = false;
		foreach (Person _person in networkMgr.allPeople)					// for each person
		{
			if (_person.relationshipListNegative.Count > 0)	// if the person has at least 1 red relationship
			{
				failCaseCheckList.Clear();
				if (!Recursive_CheckForGreenNeighbors(_person))		// check to see if they connect to anything green, even their neighbors, etc.
				{ print ("fail case: red island"); return true; }

				else if (_person.relationshipListNegative.Count == 2 && _person.relationshipListPositive.Count == 0)	// if the person has just 2 red relationships
				{
					class_Relationship _greenRel = null;
					if (_person.GetMyNeighbor(_person.relationshipListNegative[0], _person).relationshipListPositive.Count == 1
					    && _person.GetMyNeighbor(_person.relationshipListNegative[0], _person).relationshipListNegative.Count == 1)		// if they have one green and one red relationship
					{
						_greenRel = _person.GetMyNeighbor(_person.relationshipListNegative[0], _person).relationshipListPositive[0];				// remember that relationship
					}
					if (_person.GetMyNeighbor(_person.relationshipListNegative[1], _person).relationshipListPositive.Count == 1
					    && _person.GetMyNeighbor(_person.relationshipListNegative[1], _person).relationshipListNegative.Count == 1)		// if they also have one green and one red relationship
					{
						if (_greenRel != null && _greenRel == _person.GetMyNeighbor(_person.relationshipListNegative[1], _person).relationshipListPositive[0])				// if that green relationship is the same as the last one
						{ print ("fail case: triangle of death"); return true; }
					}
				}
			}

			else if (_person.relationshipListNonZero.Count == 0) 		// if a person is not connected to anything
			{ print ("fail case: unconnected person"); return true; }

//				else if (_person.relationshipListNonZeroNoPlayer.Count == 2) 			// find loops (polygons) that have no other connections
//				{
//					loopStart = null;
//					loopMembers.Clear();
//					if (Recursive_isImpossibleLoop(_person))
//					{ print ("fail case: loop without 2 greens (not yet checking greens)"); return true; }
//				}

			if (!hasAtLeastOnePersonWithAllGreenRelationships && _person.relationshipListNegative.Count == 0)
			{
				hasAtLeastOnePersonWithAllGreenRelationships = true;
			}
		}

		if (!hasAtLeastOnePersonWithAllGreenRelationships)
		{ print ("fail case: no green only people"); return true; }

		return false;
	}

	// TODO: does not yet look for the double greens.
	bool Recursive_isImpossibleLoop(Person p)
	{
		loopMembers.Add(p);
		if (p.relationshipListNonZero.Count == 2)	// if a person has exactly 2 relationships
		{
			if (loopStart == null) 		// if a loop start person has not been set, this must be the first person, so set it
			{
				loopStart = p;
			} else if (loopStart == p) 	// if you've reached the loop start again, then you've successfully completed the loop
			{
				if (!CheckPeopleForTwoGeenRelationships(loopMembers))
				{ return true; }
			}
			
			foreach (class_Relationship _rel in p.relationshipListNonZero)
			{
				Person _neighbor = p.GetMyNeighbor(_rel);
				if (_neighbor != loopStart && _neighbor != lastVisitedInLoop)
				{
					lastVisitedInLoop = p;
					if (Recursive_isImpossibleLoop(_neighbor))
					{
						if (!CheckPeopleForTwoGeenRelationships(loopMembers))
						{ return true; }
					}
				}
			}
		}
		
		return false;		// if a loop was not found
	}

	bool CheckPeopleForTwoGeenRelationships(List<Person> _ppl)
	{
		foreach (Person _p in _ppl)
		{
			if (_p.relationshipListPositive.Count > 1)
			{
				return true;
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
			if (p.relationshipListNonZero.Count > p.relationshipListNegative.Count)		// if he has any positive relationships
			{
				return true;
			}
			else
			{
				foreach (class_Relationship _rel in p.relationshipListNegative)
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

	bool RunOneLevelTrial()
	{
		FindNetworkManager();
		levelSeedList thisLevelSeedList = gameObject.AddComponent<levelSeedList>();

		int _numTries = PathfindLevel_BreadthFirst();
		if (_numTries == -1)
		{
			return false;
		}
		else
		{
			thisLevelSeedList.numTimesAITook = _numTries; 					// set number of clicks
		}

		thisLevelSeedList.mySeed = networkMgr.usedSeed;						// set used seed

		thisLevelSeedList.numPeople = networkMgr.allPeople.Count();		// set how many people on this level
        
		if (thisLevelSeedList.myDifficulty != Difficulty.Impossible)
		{
			seedList.Add(thisLevelSeedList);

			if (fp == null) { fp = new FileParse(); }
			fp.SerializeALevel(ConvertLevelSeedListToValidLevel(thisLevelSeedList));
		}
		return true;
	}

	validLevels ConvertLevelSeedListToValidLevel(levelSeedList seedList)
	{
		validLevels returnLevel = new validLevels();
		returnLevel.level = seedList.numPeople;
		returnLevel.difficulty = seedList.myDifficulty;
		returnLevel.seed = seedList.mySeed;
		returnLevel.numClicks = seedList.numTimesAITook;
		returnLevel.oneClick = false;
		returnLevel.cantTouch = -2;

		return returnLevel;
	}

	void SaveLevelToFile(validLevels level)
	{
		if (fp == null) { fp = new FileParse(); }
		fp.SerializeALevel(level);
	}

	#region BreadthFirstBestPathSearch

	public int PathfindLevel_BreadthFirst()
	{
		FindNetworkManager();
		bestWinStateHasBeenFound = false;
		// create winning state for reference
		List<bool> allTrue = new List<bool>();
		for (int i = 0; i < networkMgr.allPeople.Count; i++)
		{
			allTrue.Add(true);
		}
		winState = new levelTesterState_s(allTrue, true, 0);

		levelTesterState_s gameStartingState = GetLevelState();
		gameStartingState.numStepsToReach = 0;
		statesThatHaveBeenReached.Clear();
		statesThatHaveBeenReached.Add(gameStartingState);

		// make a default "best win"
		bestWinState.numStepsToReach = 99;

		// create lists to hold the "levels" of depth for the search
		List<levelTesterState_s> parentList = new List<levelTesterState_s>();
		List<levelTesterState_s> childList = new List<levelTesterState_s>();

		parentList.Add(gameStartingState);		// to start, game start state is in the parent list...

		currentStepsTaken = 0;
		while (!bestWinStateHasBeenFound)
		{
			currentStepsTaken++;
			//print ("steps taken: " + currentStepsTaken);
			if (currentStepsTaken > 15) { print ("breaking out early, none found"); break; }
			if (parentList.Count == 0)
			{
				print ("TESTER FEEDBACK: this level is impossible");
				return -1;
			}
			foreach (levelTesterState_s _parentState in parentList)
			{
				childList.AddRange(SearchAllOptionsOneLevelDeep(_parentState));
				//print ("child count:" + childList.Count);
			}
			if (!bestWinStateHasBeenFound)
			{
				parentList.Clear();
				parentList.AddRange(childList);
				childList.Clear();
			}
			else
			{
				print ("TESTER FEEDBACK: requires " + bestWinState.numStepsToReach + " steps");
			}
		}
		return bestWinState.numStepsToReach;
	}

	List<levelTesterState_s> SearchAllOptionsOneLevelDeep(levelTesterState_s parentState)
	{
		List<levelTesterState_s> returnState = new List<levelTesterState_s>();
		foreach (actionPossibility _actionPossibility in FindAllPossibleActions())
		{
			PerformAnAction(_actionPossibility);
			levelTesterState_s currentState = GetLevelState();
			currentState.numStepsToReach = currentStepsTaken;
			if (currentState == winState) 				// if you find a win state...
			{
				bestWinStateHasBeenFound = true;
				returnState.Add(currentState);
				bestWinState = currentState;
				break;
			}
			else if (CheckIfMatchingStateExists(currentState).isNull) 		// if it's a new state...
			{
				statesThatHaveBeenReached.Add(currentState);
				//print ("state count: " + statesThatHaveBeenReached.Count);
				returnState.Add(currentState);
			}
			ReturnToPreviousLevelState(parentState);
		}
		return returnState;
	}

	void PerformAnAction(actionPossibility _action)
	{
		foreach (Person _per in networkMgr.allPeople)
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
		for (int i = 0; i < networkMgr.allPeople.Count; i++)
		{
			statesOfPeople.Add(false);		// create a place holder list, defaulting everyone to false
		}
		for (int i = 0; i < networkMgr.allPeople.Count; i++)
		{
			Person _people = networkMgr.allPeople[i];
			if (_people.m_Mood == Mood.Positive)
			{
				statesOfPeople[_people.personalIndex] = true;
			}
		}
		levelTesterState_s listToReturn = new levelTesterState_s();
		listToReturn.myState = statesOfPeople;
		return listToReturn;
	}

	void ReturnToPreviousLevelState(levelTesterState_s levelStateToReturnTo)
	{
		for (int i = 0; i < networkMgr.allPeople.Count; i++)
		{
			if (levelStateToReturnTo.myState[networkMgr.allPeople[i].personalIndex])
			{
				networkMgr.allPeople[i].m_Mood = Mood.Positive;
			}
			else
			{
				networkMgr.allPeople[i].m_Mood = Mood.Negative;
			}
		}
	}
	
	List<actionPossibility> FindAllPossibleActions()
	{
		List<actionPossibility> listToReturn = new List<actionPossibility>();
		for (int i = 0; i < networkMgr.allPeople.Count; i++) 		// for every person
		{
			actionPossibility possibility1 = new actionPossibility(true, networkMgr.allPeople[i].personalIndex);
			actionPossibility possibility2 = new actionPossibility(false, networkMgr.allPeople[i].personalIndex);
			listToReturn.Add(possibility1);
			listToReturn.Add(possibility2);
		}
		return listToReturn;
	}

	levelTesterState_s CheckIfMatchingStateExists(levelTesterState_s stateToCompare)
	{
		foreach (levelTesterState_s _state in statesThatHaveBeenReached)
		{
			if (stateToCompare == _state)
			{ return _state; }
		}
		levelTesterState_s nullState = new levelTesterState_s();
		nullState.isNull = true;
		return nullState;			// a new state has been found, return a "null" state
	}

	#endregion
}
