using UnityEngine;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public static class Achievements {

	public static void CheckForNewAchievements()
	{
		CalculateStarCount();
		for (int i = 0; i < SaveGame.numTotalDays/5; i++)
		{
			isWeekCompleted(i);
			isWeekPerfected(i);
		}


		isGameCompleted();
		isGamePerfected();
	}

	// "Add" type stat
	private static int CalculateStarCount()
	{
		int r = SaveGame.numStarsAcquired;
		if (r > 0)
		{
			KongregateAPI.ReportStarCount(r);
			GooglePlayAPI.ReportStarCount(r);
		}
		return r;
	}

	// Individual achievements per week
	private static bool isWeekCompleted(int week)
	{
		for (int i = 0; i < 5; i++)
		{
			if (SaveGame.GetHasCompletedAllRoundsInDay(i + (week * 5)) == false)
			{
				return false;
			}
		}
		// note: week is 0 based
		KongregateAPI.ReportWeekCompleted(week);
		GooglePlayAPI.ReportWeekCompleted(week);
		return true;
	}

	// Individual achievements per week
	private static bool isWeekPerfected(int week)
	{
		if (SaveGame.numStarsAcquiredPerWeek[week] == SaveGame.numStarsPossiblePerWeek[week])
		{
			// note: week is 0 based
			KongregateAPI.ReportWeekPerfected(week);
			GooglePlayAPI.ReportWeekPerfected(week);
			return true;
		}
		return false;
	}

	// Individual achievement
	private static bool isGameCompleted()
	{
		if (SaveGame.numDaysCompleted == SaveGame.numTotalDays)
		{
			KongregateAPI.ReportGameCompleted();
			GooglePlayAPI.ReportGameCompleted();
			return true;
		}
		return false;
	}

	// Individual achievement
	private static bool isGamePerfected()
	{
		for (int i = 0; i < SaveGame.numStarsPossiblePerWeek.Count; i++)
		{
			if (SaveGame.numStarsAcquiredPerWeek[i] != SaveGame.numStarsPossiblePerWeek[i])
				return false;
		}
		KongregateAPI.ReportGamePerfected();
		GooglePlayAPI.ReportGamePerfected();
		return true;
	}

}
