using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types;

public class LevelFactory : MonoBehaviour {

	public TextAsset validSeedList;
	
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
					// WARNING: setting special attributes on a specific seeded level does not check that those attributes will work!
					level.SetOnlySpecialAttributes(_fallToRed, _oneClick, _cantTouch, _noLines);
					return level;
				}
				else
				{
					//validLevels aNewRandomLevel = new validLevels();
					validLevels aNewRandomLevel = gameObject.AddComponent<validLevels>();
					aNewRandomLevel.SetAttributes(_level, _difficulty, _seed, 0, false, 0);
					print ("returning a new random level with seed " + aNewRandomLevel.seed);
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
		                          where (_cantTouch == true && level.cantTouch > 0) || (_cantTouch == false)
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
		List<validLevels> _list = new List<validLevels>();
		string listText = validSeedList.text;
		List<string> _eachLine = new List<string>();
		_eachLine.AddRange(listText.Split('\n'));
		if (_eachLine.Contains("")) { _eachLine.Remove(""); }   // remove possible trailing line
		foreach (var line in _eachLine)
		{
			if (!line.StartsWith("//"))
			{
				Difficulty thisDifficulty = Difficulty.Unknown;
				int thisSeed = 0;
				int thisLevel = 0;
				int thisCantTouch = 0;
				bool thisOneClick = false;
				int thisNumClick = 0;

				string[] tokens = line.Split(',');
				int a;
				bool b;
				if (int.TryParse((tokens[0].Split(':')[1]), out a)) { thisLevel = a; }      // set the level
				string _diff = tokens[1].Split(':')[1];     // set the difficulty
				if (_diff == "VeryEasy")
				{ thisDifficulty = Difficulty.VeryEasy; }
				else if (_diff == "Easy")
				{ thisDifficulty = Difficulty.Easy; }
				else if (_diff == "Medium")
				{ thisDifficulty = Difficulty.Medium; }
				else if (_diff == "Hard")
				{ thisDifficulty = Difficulty.Hard; }

				if (int.TryParse((tokens[2].Split(':')[1]), out a)) 
					{ thisSeed = a; }       							// set the seed
				if (int.TryParse((tokens[3].Split(':')[1]), out a))
					{ thisNumClick = a; }     							// set the numClicks
				if (bool.TryParse((tokens[4].Split(':')[1]), out b))	
				{ thisOneClick = b; }        							// set the oneClick possibility
				if (int.TryParse((tokens[5].Split(':')[1]), out a))	
				{ thisCantTouch = a; }        							// set who you cant touch, if possible

				validLevels lvl = gameObject.AddComponent<validLevels>();
				lvl.SetAttributes(thisLevel, thisDifficulty, thisSeed, thisCantTouch, thisOneClick, thisNumClick);
				_list.Add(lvl);
			}
		}
		return _list;
	}
}
