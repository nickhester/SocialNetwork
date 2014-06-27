using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class levelSeedList : MonoBehaviour {

	public int mySeed { get; set; }
	public int numPeople { get; set; }

	private int _numTimesAITook;
	public int numTimesAITook
	{
		get
		{
			return _numTimesAITook;
		}
		set
		{
			_numTimesAITook = value;
			if (value == -1) { myDifficulty = Difficulty.NoSolutionFound; }
			else if (value < 1000) { myDifficulty = Difficulty.VeryEasy; }
			else if (value < 50000) { myDifficulty = Difficulty.Easy; }
		}
	}
	
	public Difficulty myDifficulty { get; private set; }
}
