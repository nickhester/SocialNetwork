using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types;

public class LevelFactory : MonoBehaviour {

	private ValidSeedListFileParse fileParse;

	void Start () {
		GenerateValidLevelList();
	}

	#region GetALevel+overloads

	public ValidLevels GetALevel(Difficulty _difficulty, int _level, bool _fallToRed, bool _oneClick, bool _cantTouch, bool _noLines, int _seed, bool isCreatingNewLevel)
	{
		// if a seed is entered, then find that exact level
		if (_seed != -1)
		{
			if (isCreatingNewLevel)
			{
				ValidLevels returnLevel = new ValidLevels(_level, _difficulty, _seed, false, false, false, false);
				return returnLevel;
			}
			else
			{
				foreach (ValidLevels level in GenerateValidLevelList ())
				{
					if (level.seed == _seed && level.difficulty == _difficulty && level.level == _level)
					{
						// set special attributes, if any
						level.SetOnlySpecialAttributes(_fallToRed, _oneClick, _cantTouch, _noLines);
						return level;
					}
				}
				Debug.LogException(new System.Exception("A requested level was not found in the level list"));
			}
		}

		// otherwise, find all matching levels
		List<ValidLevels> LevelList = GenerateValidLevelList();
		var levelsToChooseFrom = (from level in LevelList
		                          where level.level == _level
		                          where level.difficulty == _difficulty
		                          where (_oneClick == true && level.oneClick) || (_oneClick == false)
		                          where (_cantTouch == true && level.cantTouch >= 0) || (_cantTouch == false)
		                          select level).ToList();

		if (levelsToChooseFrom.Count == 0)			// if there aren't any found for this level
		{
			Debug.LogException(new System.Exception("No seed found for this level"));
		}

		if (levelsToChooseFrom.Count == 0) { Debug.LogException(new System.Exception("warning: no level found to match request. returning null.")); return null; }	// if none were found to match, return null

		ValidLevels levelToReturn = levelsToChooseFrom[Random.Range(0, levelsToChooseFrom.Count)];	// choose a matching level at random
		levelToReturn.SetOnlySpecialAttributes(_fallToRed, _oneClick, _cantTouch, _noLines);	// explicitly set all special attributes on this level

		return levelToReturn;
	}

	public ValidLevels GetALevel(Difficulty _difficulty, int _level, bool _fallToRed, bool _oneClick, bool _cantTouch, bool _noLines)
	{
		return GetALevel(_difficulty, _level, _fallToRed, _oneClick, _cantTouch, _noLines, -1, false);
	}

	public ValidLevels GetALevel(Difficulty _difficulty, int _level)
	{
		return GetALevel(_difficulty, _level, false, false, false, false);
	}

	public ValidLevels GetALevel(Difficulty _difficulty, int _level, int _seed)
	{
		return GetALevel(_difficulty, _level, false, false, false, false, _seed, false);
	}

	public ValidLevels GetALevel(Difficulty _difficulty, int _level, int _seed, bool isCreatingNewLevel)
	{
		return GetALevel(_difficulty, _level, false, false, false, false, _seed, isCreatingNewLevel);
	}

	#endregion

	List<ValidLevels> GenerateValidLevelList()
	{
		if (fileParse == null) { fileParse = new ValidSeedListFileParse(); }
		return fileParse.DeseriealizeLevels();
	}
}
