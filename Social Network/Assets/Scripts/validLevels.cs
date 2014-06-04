using UnityEngine;
using System.Collections;
using Types;

public class validLevels : MonoBehaviour {

	public int level;
	public Difficulty difficulty;
	public int seed;
	public int percentRelationship;
	public int cantTouch;
	public bool oneClick;
	public int numClicks;

	public bool isFallToRed = false;
	public bool isOneClick = false;
	public bool isCantTouch = false;
	public bool isNoLines = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetAttributes(int _level, Difficulty _difficulty, int _seed, int _percentRelationship, int _cantTouch, bool _oneClick, int _numClicks,
	                          bool _isFallToRed, bool _isOneClick, bool _isCantTouch, bool _isNoLines)
	{
		level = _level;
		difficulty = _difficulty;
		seed = _seed;
		percentRelationship = _percentRelationship;
		cantTouch = _cantTouch;
		oneClick = _oneClick;
		numClicks = _numClicks;
		isFallToRed = _isFallToRed;
		isOneClick = _isOneClick;
		isCantTouch = _isCantTouch;
		isNoLines = _isNoLines;
	}

	// overload...
	public void SetAttributes(int _level, Difficulty _difficulty, int _seed, int _percentRelationship, int _cantTouch, bool _oneClick, int _numClicks)
	{
		SetAttributes(_level, _difficulty, _seed, _percentRelationship, _cantTouch, _oneClick, _numClicks, false, false, false, false);
	}

	public void SetOnlySpecialAttributes(bool _isFallToRed, bool _isOneClick, bool _isCantTouch, bool _isNoLines)
	{
		isFallToRed = _isFallToRed;
		isOneClick = _isOneClick;
		isCantTouch = _isCantTouch;
		isNoLines = _isNoLines;
	}
}
