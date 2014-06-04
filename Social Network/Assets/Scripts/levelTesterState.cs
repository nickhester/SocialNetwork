using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class levelTesterState : MonoBehaviour {
	
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

public struct levelTesterState_s {
	
	public List<bool> myState;
	public bool isWinningState;
	public int numStepsToReach;
	public bool isNull;

	//public void levelTesterState_s(myState _state, isWinningState _win, numStepsToReach _steps)
	public levelTesterState_s(List<bool> _m, bool _w, int _n)
	{
		myState = _m; isWinningState = _w; numStepsToReach = _n; isNull = false;
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
}

public struct actionPossibility {
	public bool isGoodAction;
	public int personIndex;
	public actionPossibility(bool _g, int _p)
	{
		isGoodAction = _g; personIndex = _p;
	}
}
