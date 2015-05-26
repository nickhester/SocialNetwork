using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Types;

public class levelTester : MonoBehaviour {

	#region variables
	
	FileParse fp;
	public bool runTestSeries = false;
	
	public int numLevelsToRun = 5;
	[HideInInspector]
	public int numLevelsLeftToRun;
	[HideInInspector]
	public int levelToLoad;
	[HideInInspector]
	public bool isStartingSingleton = false;
	private class_NetworkMgr networkMgr;

	private List<Person> failCaseCheckList = new List<Person>();

	private Person loopStart;
	private Person lastVisitedInLoop;
	private List<Person> loopMembers = new List<Person>();

	#endregion
	
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
		GetComponent<createAndDestroyAppointment>().MakeNewTestLevel(levelToLoad);
	}

	void FindNetworkManager()
	{
		networkMgr = GameObject.Find("networkMgr").GetComponent<class_NetworkMgr>();
	}

	#region FailCaseChecking

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

	#endregion

	bool RunOneLevelTrial()
	{
		FindNetworkManager();
		validLevels thisLevel = new validLevels();
		LevelValidator lv = new LevelValidator();

		levelTesterState_s bestWinState = lv.PathfindLevel_BreadthFirst();
		if (bestWinState.numStepsToReach == -1)
		{
			return false;
		}
		else
		{
			thisLevel.numClicks = bestWinState.numStepsToReach; 					// set number of clicks
		}

		thisLevel.level = networkMgr.allPeople.Count;		// set how many people on this level

		if (thisLevel.level == 3)
		{	thisLevel.difficulty = Difficulty.VeryEasy;	}			// set difficulty from number of clicks and level
		else if (thisLevel.level == 4 && thisLevel.numClicks < 3)
		{	thisLevel.difficulty = Difficulty.VeryEasy;	}
		else if (thisLevel.level == 4 && thisLevel.numClicks >= 3)
		{	thisLevel.difficulty = Difficulty.Easy;	}
		else if (thisLevel.level == 5 && thisLevel.numClicks < 4)
		{	thisLevel.difficulty = Difficulty.Easy;	}
		else if (thisLevel.level == 5 && thisLevel.numClicks >= 4)
		{	thisLevel.difficulty = Difficulty.Medium;	}
		else if (thisLevel.level == 6 && thisLevel.numClicks < 3)
		{	return false;	}		// this level is too easy
		else if (thisLevel.level == 6 && thisLevel.numClicks < 5)
		{	thisLevel.difficulty = Difficulty.Medium;	}
		else if (thisLevel.level == 6 && thisLevel.numClicks >= 5)
		{	thisLevel.difficulty = Difficulty.Hard;	}
		else if (thisLevel.level == 7 && thisLevel.numClicks < 3)
		{	return false;	}		// this level is too easy
		else if (thisLevel.level == 7 && thisLevel.numClicks < 5)
		{	thisLevel.difficulty = Difficulty.Medium;	}
		else if (thisLevel.level == 7 && thisLevel.numClicks >= 5)
		{	thisLevel.difficulty = Difficulty.Hard;	}
		else if (thisLevel.level == 8 && thisLevel.numClicks < 3)
		{	return false;	}		// this level is too easy
		else if (thisLevel.level == 8 && thisLevel.numClicks < 5)
		{	thisLevel.difficulty = Difficulty.Medium;	}
		else if (thisLevel.level == 8 && thisLevel.numClicks >= 5)
		{	thisLevel.difficulty = Difficulty.Hard;	}
		else
		{	Debug.LogException(new System.Exception("Level and number of clicks wasn't handled by levelTester class (" + thisLevel.level + ", " + thisLevel.numClicks + ")"));	}

		thisLevel.seed = networkMgr.usedSeed;						// set used seed
		thisLevel.path = bestWinState.pathOfActions;
        
		if (lv.CheckIfLevelCanBeOneClick(bestWinState))
		{
			thisLevel.oneClick = true;
		}

		// find a person for the cantTouch special level
		bool foundCantClickPerson = false;
		for (int i = 0; i < bestWinState.pathOfActions.trail.Count; i++)
		{
			int personToExclude = bestWinState.pathOfActions.trail[i].Key;
			networkMgr.ReloadStartingState();
			LevelValidator lv2 = new LevelValidator();
			levelTesterState_s bestWinState2 = lv2.PathfindLevel_BreadthFirst(personToExclude);
			if (bestWinState2.numStepsToReach != -1)
			{
				foundCantClickPerson = true;
				thisLevel.cantTouch = personToExclude;
				//print (bestWinState.numStepsToReach + ":" + bestWinState2.numStepsToReach);
				thisLevel.cantTouchPath = bestWinState2.pathOfActions;

			}
		}
		if (!foundCantClickPerson) { thisLevel.cantTouch = -1; }

		if (thisLevel.difficulty != Difficulty.Impossible)
		{
			lv.levelList.Add(thisLevel);

			if (fp == null) { fp = new FileParse(); }
			fp.SerializeALevel(thisLevel);
		}
		return true;
	}

	void SaveLevelToFile(validLevels level)
	{
		if (fp == null) { fp = new FileParse(); }
		fp.SerializeALevel(level);
	}
}
