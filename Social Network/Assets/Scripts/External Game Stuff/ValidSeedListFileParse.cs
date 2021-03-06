﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class ValidSeedListFileParse {

	FileIO fileIO;

	public ValidSeedListFileParse()
	{
		fileIO = new FileIO("validSeedList");
	}

	public void SerializeALevel(ValidLevels level)
	{
		string formattedString = string.Format("level:{0},difficulty:{1},seed:{2},clicks:{3},oneClick:{4},cantTouch:{5},path:{6},cantTouchPath:{7}\n",
		                                       level.level.ToString(),
		                                       level.difficulty.ToString(),
		                                       level.seed.ToString(),
		                                       level.numClicks.ToString(),
		                                       level.oneClick,
		                                       level.cantTouch.ToString(),
		                                       level.path,
		                                       level.cantTouchPath
		                     );

		fileIO.AppendToFile(formattedString);
	}

	public List<ValidLevels> DeseriealizeLevels()
	{
		List<ValidLevels> _list = new List<ValidLevels>();

		string listText = fileIO.GetFileText();
		
		List<string> _eachLine = new List<string>();
		_eachLine.AddRange(listText.Split('\n'));
		if (_eachLine.Contains("")) { _eachLine.Remove(""); }   // remove possible trailing line
		foreach (string line in _eachLine)
		{
			if (!line.StartsWith("//"))
			{
				Difficulty thisDifficulty = Difficulty.Unknown;
				int thisSeed = 0;
				int thisLevel = 0;
				int thisCantTouch = 0;
				bool thisOneClick = false;
				int thisNumClick = 0;

				string lineWithoutEnding = line.Split('\r')[0];
				string[] tokens = lineWithoutEnding.Split(',');
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

				ActionTrail thisPath = new ActionTrail(tokens[6]);
				ActionTrail thisCantTouchPath = new ActionTrail(tokens[7]);
				ValidLevels lvl = new ValidLevels(thisLevel, thisDifficulty, thisSeed, thisCantTouch, thisOneClick, thisNumClick, thisCantTouchPath.trail.Count);
				lvl.path = thisPath;
				lvl.cantTouchPath = thisCantTouchPath;
				_list.Add(lvl);
			}
		}
		return _list;

	}

}
