using UnityEngine;
using System.Collections;
using Types;

public class validLevels {

	public int level;
	public Difficulty difficulty;
	public int seed;
	public int cantTouch;
	public bool oneClick;
	public int numClicks;

	public actionTrail path;

	public bool isFallToRed = false;
	public bool isOneClick = false;
	public bool isCantTouch = false;
	public bool isNoLines = false;

	public void SetAttributes(int _level, Difficulty _difficulty, int _seed, int _cantTouch, bool _oneClick, int _numClicks,
	                          bool _isFallToRed, bool _isOneClick, bool _isCantTouch, bool _isNoLines)
	{
		level = _level;
		difficulty = _difficulty;
		seed = _seed;
		cantTouch = _cantTouch;
		oneClick = _oneClick;
		numClicks = _numClicks;
		isFallToRed = _isFallToRed;
		isOneClick = _isOneClick;
		isCantTouch = _isCantTouch;
		isNoLines = _isNoLines;
	}

	public validLevels(int _level, Difficulty _difficulty, int _seed, int _cantTouch, bool _oneClick, int _numClicks)
	{
		SetAttributes(_level, _difficulty, _seed, _cantTouch, _oneClick, _numClicks);
	}

	public validLevels() { }

	public validLevels(int _level, Difficulty _difficulty, int _seed, bool _isFallToRed, bool _isOneClick, bool _isCantTouch, bool _isNoLines)
	{
		level = _level;
		difficulty = _difficulty;
		seed = _seed;
		SetOnlySpecialAttributes(_isFallToRed, _isOneClick, _isCantTouch, _isNoLines);
	}

	// overload...
	public void SetAttributes(int _level, Difficulty _difficulty, int _seed, int _cantTouch, bool _oneClick, int _numClicks)
	{
		SetAttributes(_level, _difficulty, _seed, _cantTouch, _oneClick, _numClicks, false, false, false, false);
	}

	public void SetOnlySpecialAttributes(bool _isFallToRed, bool _isOneClick, bool _isCantTouch, bool _isNoLines)
	{
		isFallToRed = _isFallToRed;
		isOneClick = _isOneClick;
		isCantTouch = _isCantTouch;
		isNoLines = _isNoLines;
	}

	public override string ToString()
	{
		string returnVal = level + ", " +
			difficulty + ", " +
				seed + ", " +
				cantTouch + ", " +
				oneClick + ", " +
				numClicks + ", " +
				isFallToRed + ", " +
				isOneClick + ", " +
				isCantTouch + ", " +
				isNoLines;
		return returnVal;
	}
}
