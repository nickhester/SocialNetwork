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
    public ActionTrail pathOfActions;
	
	public levelTesterState_s(List<Mood> _m, bool _w, int _n)
	{
        myState = _m; isWinningState = _w; numStepsToReach = _n; isNull = false; pathOfActions = new ActionTrail(true);
	}
	public void InitializeStart()
	{
        isWinningState = false; numStepsToReach = 0; isNull = false; pathOfActions = new ActionTrail(true);
	}
    public void AddPath(ActionTrail _path, int _newPerson, bool _newAction)
	{
		pathOfActions.trail.AddRange(_path.trail);

        ActionTrail tempTrail = new ActionTrail(_newPerson, _newAction);
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
	public override bool Equals(object obj)
	{
		Debug.LogError("Equals() not implemented");
		return base.Equals(obj);
	}
	public override int GetHashCode()
	{
		Debug.LogError("GetHashCode() not implemented");
		return base.GetHashCode();
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
