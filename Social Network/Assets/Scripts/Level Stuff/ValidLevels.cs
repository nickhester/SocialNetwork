﻿using UnityEngine;
using System.Collections;
using Types;

public class ValidLevels {

	public int level;       // this value is equal to the number of people, so the minimum number should be 3
	public Difficulty difficulty;
	public int seed;
	public int cantTouch;
	public bool oneClick;
	public int numClicks;
	public int cantTouchNumClicks;

    public ActionTrail path;
    public ActionTrail cantTouchPath;

	public bool isFallToRed = false;
	public bool isOneClick = false;
	public bool isCantTouch = false;
	public bool isNoLines = false;

	void SetAttributes(int _level, Difficulty _difficulty, int _seed, int _cantTouch, bool _oneClick, int _numClicks, int _cantTouchNumClicks,
	                          bool _isFallToRed, bool _isOneClick, bool _isCantTouch, bool _isNoLines)
	{
		level = _level;
		difficulty = _difficulty;
		seed = _seed;
		cantTouch = _cantTouch;
		oneClick = _oneClick;
		numClicks = _numClicks;
		cantTouchNumClicks = _cantTouchNumClicks;
		isFallToRed = _isFallToRed;
		isOneClick = _isOneClick;
		isCantTouch = _isCantTouch;
		isNoLines = _isNoLines;
	}

	public ValidLevels(int _level, Difficulty _difficulty, int _seed, int _cantTouch, bool _oneClick, int _numClicks, int _cantTouchNumClicks)
	{
		SetAttributes(_level, _difficulty, _seed, _cantTouch, _oneClick, _numClicks, _cantTouchNumClicks);
	}

	public ValidLevels() { }

	public ValidLevels(int _level, Difficulty _difficulty, int _seed, bool _isFallToRed, bool _isOneClick, bool _isCantTouch, bool _isNoLines)
	{
		level = _level;
		difficulty = _difficulty;
		seed = _seed;
		SetOnlySpecialAttributes(_isFallToRed, _isOneClick, _isCantTouch, _isNoLines);
	}

	public ValidLevels(int _level, Difficulty _difficulty, int _seed, Types.SpecialLevel _special)
	{
		level = _level;
		difficulty = _difficulty;
		seed = _seed;
		if (_special == Types.SpecialLevel.None)
			SetOnlySpecialAttributes(false, false, false, false);
		else if (_special == Types.SpecialLevel.FallToRed)
			SetOnlySpecialAttributes(true, false, false, false);
		else if (_special == Types.SpecialLevel.OneClick)
			SetOnlySpecialAttributes(false, true, false, false);
		else if (_special == Types.SpecialLevel.CantTouch)
			SetOnlySpecialAttributes(false, false, true, false);
		else if (_special == Types.SpecialLevel.NoLines)
			SetOnlySpecialAttributes(false, false, false, true);
	}

	// overload...
	public void SetAttributes(int _level, Difficulty _difficulty, int _seed, int _cantTouch, bool _oneClick, int _numClicks, int _cantTouchNumClicks)
	{
		SetAttributes(_level, _difficulty, _seed, _cantTouch, _oneClick, _numClicks, _cantTouchNumClicks, false, false, false, false);
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
