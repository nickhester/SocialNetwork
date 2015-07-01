using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class LevelTesterState : MonoBehaviour {
	
//	public List<bool> myState = new List<bool>();
//	public bool isWinningState = false;
//	public int numStepsToReach;
//
//	// add equals operators
//	public static bool operator ==(levelTesterState lhs, levelTesterState rhs)
//	{
//		bool returnValue = true;
//		for (int i = 0; i < lhs.myState.Count; i++)
//		{
//			if (lhs.myState[i] == rhs.myState[i]) { returnValue = false; break; }
//		}
//		return returnValue;
//	}
//	public static bool operator !=(levelTesterState lhs, levelTesterState rhs)
//	{
//		bool returnValue = false;
//		for (int i = 0; i < lhs.myState.Count; i++)
//		{
//			if (lhs.myState[i] != rhs.myState[i]) { returnValue = true; break; }
//		}
//		return returnValue;
//	}
}

public struct levelTesterState_s
{
	public List<Mood> myState;
	public bool isWinningState;
	public int numStepsToReach;
	public bool isNull;
	public actionTrail pathOfActions;
	
	public levelTesterState_s(List<Mood> _m, bool _w, int _n)
	{
		myState = _m; isWinningState = _w; numStepsToReach = _n; isNull = false; pathOfActions = new actionTrail(true);
	}
	public void InitializeStart()
	{
		isWinningState = false; numStepsToReach = 0; isNull = false; pathOfActions = new actionTrail(true);
	}
	public void AddPath(actionTrail _path, int _newPerson, bool _newAction)
	{
		pathOfActions.trail.AddRange(_path.trail);

		actionTrail tempTrail = new actionTrail(_newPerson, _newAction);
		pathOfActions.trail.AddRange(tempTrail.trail);
	}

	// add equals operators
	public static bool operator ==(levelTesterState_s lhs, levelTesterState_s rhs)
	{
		bool returnValue = true;
		for (int i = 0; i < lhs.myState.Count; i++)
		{
			if (lhs.myState[i] != rhs.myState[i]) { returnValue = false; break; }
		}
		return returnValue;
	}
	public static bool operator !=(levelTesterState_s lhs, levelTesterState_s rhs)
	{
		bool returnValue = false;
		for (int i = 0; i < lhs.myState.Count; i++)
		{
			if (lhs.myState[i] != rhs.myState[i]) { returnValue = true; break; }
		}
		return returnValue;
	}
	public override string ToString()
	{
		string returnVal = "";
		foreach (var vals in myState)
		{
			returnVal += vals + ",";
		}
		return returnVal;
	}
}

public struct actionPossibility
{
	public bool isGoodAction;
	public int personIndex;
	public actionPossibility(bool _g, int _p)
	{
		isGoodAction = _g; personIndex = _p;
	}
}

public struct actionTrail
{
	public List<KeyValuePair<int, bool>> trail;
	public actionTrail(int person, bool action)
	{
		trail = new List<KeyValuePair<int, bool>>();
		AddItem(person, action);
	}
	public actionTrail(bool isNewTrail)
	{
		trail = new List<KeyValuePair<int, bool>>();
	}
	public void AddItem(int person, bool action)
	{
		KeyValuePair<int, bool> tempkvp = new KeyValuePair<int, bool>(person, action);
		trail.Add(tempkvp);
	}
	public override string ToString()
	{
		string returnValue = "";
		if (trail != null && trail.Count > 0)
		{
			foreach (KeyValuePair<int, bool> step in trail)
			{
				returnValue += step.Key;
				if (step.Value == true) { returnValue += "T"; }
				else { returnValue += "F"; }
				returnValue += "-";
			}
		}
		//if (returnValue.Length > 1) { returnValue = returnValue.Remove(returnValue.Length - 1); }
		return returnValue;
	}
//	public static actionTrail operator +(actionTrail lhs, actionTrail rhs)
//	{
//		actionTrail returnValue;
//		returnValue = lhs;
//		returnValue.trail.AddRange(rhs.trail);
//		return returnValue;
//	}
}
