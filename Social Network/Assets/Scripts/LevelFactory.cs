using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types;

public class LevelFactory : MonoBehaviour {

	FileParse fp;

	void Start () {
		GenerateValidLevelList();
	}

	#region GetALevel+overloads

	public validLevels GetALevel(Difficulty _difficulty, int _level, bool _fallToRed, bool _oneClick, bool _cantTouch, bool _noLines, int _seed)
	{
		// if a seed is entered, then find that exact level
		if (_seed != -1)
		{
			foreach (validLevels level in GenerateValidLevelList ())
			{
				if (level.seed == _seed && level.difficulty == _difficulty && level.level == _level)
				{
					// set special attributes, if any
					level.SetOnlySpecialAttributes(_fallToRed, _oneClick, _cantTouch, _noLines);
					return level;
				}
				else
				{
					validLevels aNewRandomLevel = new validLevels(_level, _difficulty, _seed, 0, false, 0);
					return aNewRandomLevel;
				}
			}
		}

		// otherwise, find all matching levels
		List<validLevels> LevelList = GenerateValidLevelList();
		var levelsToChooseFrom = (from level in LevelList
		                          where level.level == _level
		                          where level.difficulty == _difficulty
		                          where (_oneClick == true && level.oneClick) || (_oneClick == false)
		                          where (_cantTouch == true && level.cantTouch >= 0) || (_cantTouch == false)
		                          select level).ToList();

		print ("choosing from " + levelsToChooseFrom.Count + " choices");

		if (levelsToChooseFrom.Count == 0)			// if there aren't any found for this level
		{
			print ("No seed found for this level");
		}

		if (levelsToChooseFrom.Count == 0) { print ("warning: no level found to match request. returning null."); return null; }	// if none were found to match, return null

		validLevels levelToReturn = levelsToChooseFrom[Random.Range(0, levelsToChooseFrom.Count)];	// choose a matching level at random
		levelToReturn.SetOnlySpecialAttributes(_fallToRed, _oneClick, _cantTouch, _noLines);	// explicitly set all special attributes on this level

		return levelToReturn;
	}

	public validLevels GetALevel(Difficulty _difficulty, int _level, bool _fallToRed, bool _oneClick, bool _cantTouch, bool _noLines)
	{
		return GetALevel(_difficulty, _level, _fallToRed, _oneClick, _cantTouch, _noLines, -1);
	}

	public validLevels GetALevel(Difficulty _difficulty, int _level)
	{
		return GetALevel(_difficulty, _level, false, false, false, false);
	}

	public validLevels GetALevel(Difficulty _difficulty, int _level, int _seed)
	{
		return GetALevel(_difficulty, _level, false, false, false, false, _seed);
	}

	#endregion

	List<validLevels> GenerateValidLevelList()
	{
		if (fp == null) { fp = new FileParse(); }
		return fp.DeseriealizeLevels();
	}
}
