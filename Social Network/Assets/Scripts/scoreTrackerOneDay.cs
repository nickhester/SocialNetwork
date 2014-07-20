using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class scoreTrackerOneDay : MonoBehaviour {

	public int score = 0;

	private int bonusForVeryEasy = 1;
	private int bonusForEasy = 2;
	private int bonusForMedium = 4;
	private int bonusForHard = 8;
	
	private int bonusSpecialMultiplier = 2;
	private int bonusPerfectMultiplier = 3;

	public int maximumPossibleScore = 0;

	// Use this for initialization
	void Start () {
		score = 0;
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
		pointsFromLevel *= bonusPerfectMultiplier;
		if (isSpecial)
		{
			pointsFromLevel *= bonusSpecialMultiplier;
		}
		maximumPossibleScore += pointsFromLevel;

		print ("best score is now: " + maximumPossibleScore);
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

		if (numActionsTaken <= thisLevel.numClicks)
		{
			scoreToAdd *= bonusPerfectMultiplier;
			ScorePopUp spu2 = gameObject.AddComponent<ScorePopUp>();
			spu2.DisplayScorePopUp("Perfect " + bonusPerfectMultiplier.ToString() + "x", -3, 3);
			print ("score - w/ perfect bonus (" + numActionsTaken + "/" + thisLevel.numClicks + "):" + scoreToAdd);
		}

		// add score for it being a special level
		if (isSpecial)
		{
			scoreToAdd *= bonusSpecialMultiplier;
			ScorePopUp spu3 = gameObject.AddComponent<ScorePopUp>();
			spu3.DisplayScorePopUp("Special " + bonusSpecialMultiplier.ToString() + "x", -3, 2);
			print ("score - w/ special bonus: " + scoreToAdd);
		}

		score += scoreToAdd;

		ScorePopUp spu = gameObject.AddComponent<ScorePopUp>();
		spu.DisplayScorePopUp(scoreToAdd.ToString());
	}
}
