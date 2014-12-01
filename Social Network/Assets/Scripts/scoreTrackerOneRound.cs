using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class scoreTrackerOneRound : MonoBehaviour {

	public int score;

	private const int bonusForVeryEasy = 1;
	private const int bonusForEasy = 2;
	private const int bonusForMedium = 4;
	private const int bonusForHard = 8;
	
	private const int bonusSpecialMultiplier = 2;
	private const int bonusPerfectMultiplier = 3;

	public int maximumPossibleScore;

	public void Reset ()
	{
		score = 0;
		maximumPossibleScore = 0;
	}

	// Use this for initialization
	void Start () {
		Reset();
	}
	
	// Update is called once per frame
	void Update () {

	}


	int CalculateNumPointsForDifficulty(Difficulty diff)
	{
		if (diff == Difficulty.VeryEasy) { return bonusForVeryEasy; }
		else if (diff == Difficulty.Easy) { return bonusForEasy; }
		else if (diff == Difficulty.Medium) { return bonusForMedium; }
		else if (diff == Difficulty.Hard) { return bonusForHard; }
		return -1;
	}


	public void UpdateMaxScore(Difficulty diff, bool isSpecial, int numActionsTaken)
	{
		int pointsFromLevel = 0;
		pointsFromLevel += CalculateNumPointsForDifficulty(diff);
		validLevels thisLevel = GameObject.Find("Clipboard").GetComponent<clipboard>().nextLevelUp.myLevel;

		// "perfect" bonus not possible on special levels, cause it's often impossible, and not fair to expect
		if (isSpecial)
		{
			pointsFromLevel *= bonusSpecialMultiplier;
		}
		else
		{
			pointsFromLevel *= bonusPerfectMultiplier;
		}
		maximumPossibleScore += pointsFromLevel;

		//print ("best score is now: " + maximumPossibleScore);
	}

	public int[] GetStarRequirements()
	{
		int[] returnList = new int[3];
		returnList[2] = maximumPossibleScore;
		returnList[1] = (int)(Mathf.Round(maximumPossibleScore * 0.66f));
		returnList[0] = (int)(Mathf.Round(maximumPossibleScore * 0.33f));
		if (returnList[1] >= returnList[2])
		{
			returnList[1] = returnList[2] - 1;
		}
		if (returnList[0] >= returnList[1])
		{
			returnList[0] = returnList[1] - 1;
		}
		return returnList;
	}

	public void UpdateScore(Difficulty diff, int numActionsTaken, bool isSpecial)
	{
		int scoreToAdd = 0;

		// add to score for difficulty
		scoreToAdd += CalculateNumPointsForDifficulty(diff);

		print ("score - w/ diff bonus: " + scoreToAdd);

		// add score for perfect number of actions
		validLevels thisLevel = GameObject.Find("Clipboard").GetComponent<clipboard>().nextLevelUp.myLevel;

		if (numActionsTaken <= thisLevel.numClicks && !isSpecial)
		{
			scoreToAdd *= bonusPerfectMultiplier;
			print ("score - w/ perfect bonus (" + numActionsTaken + "/" + thisLevel.numClicks + "):" + scoreToAdd);
		}
		else if (isSpecial)
		{
			scoreToAdd *= bonusSpecialMultiplier;
			print ("score - w/ special bonus: " + scoreToAdd);
		}

		score += scoreToAdd;
	}
}
