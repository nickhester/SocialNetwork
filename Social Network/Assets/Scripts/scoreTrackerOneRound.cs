using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class ScoreTrackerOneRound : MonoBehaviour {

	public int score;
	

	public void Reset ()
	{
		score = 0;
	}

	// Use this for initialization
	void Start () {
		Reset();
	}
	
	// Update is called once per frame
	void Update () {

	}


	public void UpdateScore(Difficulty diff, int numActionsTaken, bool isSpecial)
	{
		// add score for perfect number of actions
        ValidLevels thisLevel = GameObject.Find("Clipboard").GetComponent<Clipboard>().GetNextLevelUp().myLevel;

		if (!isSpecial)
		{
			if (numActionsTaken == thisLevel.numClicks)
			{
				score = 3;
			}
			else if (numActionsTaken == (thisLevel.numClicks + 1))
			{
				score = 2;
			}
			else
			{
				score = 1;
			}
		}
		else if (isSpecial)
		{
			score = 3;
		}
	}
}
