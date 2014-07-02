using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Types;

public class LevelValidator {

	public List<validLevels> levelList = new List<validLevels>();
	private class_NetworkMgr networkMgr;
	private List<levelTesterState_s> statesThatHaveBeenReached = new List<levelTesterState_s>();
	private levelTesterState_s winState;
	private int currentStepsTaken;
	private Person loopStart;
	private Person lastVisitedInLoop;
	private List<Person> loopMembers = new List<Person>();
	private bool bestWinStateHasBeenFound = false;

	void FindNetworkManager()
	{
		networkMgr = GameObject.Find("networkMgr").GetComponent<class_NetworkMgr>();
	}

	public bool CheckIfLevelCanBeOneClick(levelTesterState_s bestLevel)
	{
		List<int> tracker = new List<int>();
		foreach (var actions in bestLevel.pathOfActions.trail)	// for the path of actions to reach the best path...
		{
			if (tracker.Contains(actions.Key))		// if a person has been clicked twice
			{
				return false;
			}
			else
			{
				tracker.Add(actions.Key);
			}
		}
		return true;
	}

	#region BreadthFirstBestPathSearch

	public levelTesterState_s PathfindLevel_BreadthFirst()
	{
		return PathfindLevel_BreadthFirst(-1);
	}

	public levelTesterState_s PathfindLevel_BreadthFirst(int personToExclude)
	{
		FindNetworkManager();
		bestWinStateHasBeenFound = false;
		// create winning state for reference
		List<Mood> allPositive = new List<Mood>();
		for (int i = 0; i < networkMgr.allPeople.Count; i++)
		{
			allPositive.Add(Mood.Positive);
		}
		winState = new levelTesterState_s(allPositive, true, 0);
		levelTesterState_s bestWinState = new levelTesterState_s();
		
		levelTesterState_s gameStartingState = GetLevelState();
		gameStartingState.InitializeStart();
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
			if (currentStepsTaken > 10) { MonoBehaviour.print ("breaking out early, none found"); break; }
			if (parentList.Count == 0)
			{
				MonoBehaviour.print ("TESTER FEEDBACK: this level is impossible");
				bestWinState.numStepsToReach = -1;
				return bestWinState;
			}
			foreach (levelTesterState_s _parentState in parentList)
			{
				childList.AddRange(SearchAllOptionsOneLevelDeep(_parentState, ref bestWinState, personToExclude));
				if (bestWinStateHasBeenFound) { break; }
			}
			if (!bestWinStateHasBeenFound)
			{
				parentList.Clear();
				parentList.AddRange(childList);
				childList.Clear();
			}
			else
			{
				MonoBehaviour.print ("TESTER FEEDBACK: requires " + bestWinState.numStepsToReach + " steps");
			}
		}
		return bestWinState;
	}
	
	List<levelTesterState_s> SearchAllOptionsOneLevelDeep(levelTesterState_s parentState, ref levelTesterState_s bestState, int personToExclude)
	{
		List<levelTesterState_s> returnState = new List<levelTesterState_s>();
		foreach (actionPossibility _actionPossibility in FindAllPossibleActions())
		{
			if (_actionPossibility.personIndex == personToExclude)
			{
				continue;
			}

			if (parentState.pathOfActions.trail.Count > 1)
			{
				if (_actionPossibility.personIndex == 0 && _actionPossibility.isGoodAction == true)
				{
					int i = 1;
				}
			}

			else if (parentState.pathOfActions.trail.Count > 1)
			{
				if (parentState.pathOfActions.trail[0].Key == 1 && parentState.pathOfActions.trail[0].Value == true)
				{
					int i = 1;
				}
			}

			ReturnToPreviousLevelState(parentState);
			PerformAnAction(_actionPossibility);
			levelTesterState_s currentState = GetLevelState();
			currentState.InitializeStart();
			currentState.numStepsToReach = currentStepsTaken;
			currentState.AddPath(parentState.pathOfActions, _actionPossibility.personIndex, _actionPossibility.isGoodAction);
			//MonoBehaviour.print ("current and parent:" + currentState.pathOfActions + " from " + parentState.pathOfActions);
			
			if (currentState == winState) 				// if you find a win state...
			{
				bestWinStateHasBeenFound = true;
				returnState.Add(currentState);
				bestState = currentState;
				//MonoBehaviour.print ("the win state is: " + currentState.pathOfActions);
				break;
			}
			else if (CheckIfMatchingStateExists(currentState).isNull) 		// if it's a new state...
			{
				statesThatHaveBeenReached.Add(currentState);
				returnState.Add(currentState);
			}
			else
			{
				//MonoBehaviour.print ("matching state -- end branch");
			}
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
				return;
			}
		}
	}
	
	levelTesterState_s GetLevelState()
	{
		List<Mood> statesOfPeople = new List<Mood>();
		for (int i = 0; i < networkMgr.allPeople.Count; i++)
		{
			statesOfPeople.Add(Mood.Neutral);		// create a place holder list, defaulting everyone to neutral
		}
		for (int i = 0; i < networkMgr.allPeople.Count; i++)
		{
			Person _people = networkMgr.allPeople[i];
			statesOfPeople[_people.personalIndex] = _people.m_Mood;
//			if (_people.m_Mood == Mood.Positive)
//			{
//				statesOfPeople[_people.personalIndex] = true;
//			}
		}
		levelTesterState_s listToReturn = new levelTesterState_s();
		listToReturn.myState = statesOfPeople;
		return listToReturn;
	}
	
	void ReturnToPreviousLevelState(levelTesterState_s levelStateToReturnTo)
	{
		for (int i = 0; i < networkMgr.allPeople.Count; i++)
		{
			networkMgr.allPeople[i].m_Mood = levelStateToReturnTo.myState[networkMgr.allPeople[i].personalIndex];
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
