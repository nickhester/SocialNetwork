using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class ScoreTrackerOneRound : MonoBehaviour {

	private int score;
	
	public void Reset ()
	{
		score = 0;
	}

	void Start () {
		Reset();
	}

	public int GetScore()
	{
		return score;
	}

	public void UpdateScore(Difficulty diff, int numActionsTaken, bool isSpecial)
	{
		// add score for perfect number of actions
        ValidLevels thisLevel = GameObject.Find("Clipboard").GetComponent<Clipboard>().GetNextLevelUp().myLevel;

		int requiredClicks = (thisLevel.isCantTouch ? thisLevel.cantTouchNumClicks : thisLevel.numClicks);

		if (numActionsTaken == requiredClicks)
		{
			score = 3;
		}
		else if (numActionsTaken == (requiredClicks + 1))
		{
			score = 2;
		}
		else
		{
			score = 1;
		}
	}
}
