using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SaveGame {

	public static List<int> numStarsPossiblePerWeek = new List<int>();
	public static List<int> numStarsAcquiredPerWeek = new List<int>();
	public static List<int> numAppointmentsPerDay = new List<int>();
	public static int numTotalPossibleStars;
	public static int numStarsAcquired;
	public static int numTotalDays;
	public static int numDaysCompleted;

	public static void Initialize(List<int> _numStarsPossiblePerWeek, List<int> _numAppointmentsPerDay, int _numTotalPossibleStars, int _numTotalDays)
	{
		numStarsPossiblePerWeek = _numStarsPossiblePerWeek;
		numTotalPossibleStars = _numTotalPossibleStars;
		numTotalDays = _numTotalDays;
		numStarsAcquired = AddAllItemsInList(numStarsAcquiredPerWeek);
		numAppointmentsPerDay = _numAppointmentsPerDay;
	}

	public static void UpdateGameStats()
	{
		List<int> numAcquiredStarsByWeek = new List<int>();
		int _numDaysCompleted = 0;
		for (int i = 0; i < numTotalDays; i++)		// "i" represents the day
		{
			int weekIndex = i/5;
			for (int week = 0; week < numTotalDays/5; week++) { numAcquiredStarsByWeek.Add(0); }		// add a "0" to the list for each week
			
			if (SaveGame.GetHasCompletedAllRoundsInDay(i))	// if all rounds within have been completed...
			{
				_numDaysCompleted = i;										// count it as a completed day
			}
			// if it's the first day, if all rounds have been completed, or if it's one day after an allroundscompleted day...
			if (i == 0 || SaveGame.GetHasCompletedAllRoundsInDay(i) || SaveGame.GetHasCompletedAllRoundsInDay(i - 1))
			{
				for (int j = 0; j < numAppointmentsPerDay[i]; j++)
				{
					numAcquiredStarsByWeek[weekIndex] += SaveGame.GetRoundStarCount(i, j);		// add each star from each round individually
				}
			}
		}

		// save out updated globals
		numStarsAcquiredPerWeek = numAcquiredStarsByWeek;
		numStarsAcquired = AddAllItemsInList(numAcquiredStarsByWeek);
		numDaysCompleted = _numDaysCompleted;

		Achievements.CheckForNewAchievements();
	}

	private static int AddAllItemsInList(List<int> inputList)
	{
		int total = 0;
		for (int i = 0; i < inputList.Count; i++)
		{
			total += inputList[i];
		}
		return total;
	}

	public static void SetSeenInstruction(int instructionIndex, bool hasSeen)
	{
		SaveData.SetInt("hasSeenInstruction" + instructionIndex, (hasSeen ? 1 : 0));
		SaveAllData();
	}

	public static bool GetSeenInstruction(int instructionIndex)
	{
		return (SaveData.GetInt("hasSeenInstruction" + instructionIndex) == 1);
	}

	public static void SetDayStarCount(int day, int count)
	{
		SaveData.SetInt(FormatDayString(day) + "_starCount", count);
		SaveAllData();
	}

	public static int GetDayStarCount(int day)
	{
		return SaveData.GetInt(FormatDayString(day) + "_starCount");
	}

	public static void SetRoundStarCount(int day, int round, int count)
	{
		SaveData.SetInt(FormatRoundString(day, round) + "_starCount", count);
		SaveAllData();
	}
	
	public static int GetRoundStarCount(int day, int round)
	{
		return SaveData.GetInt(FormatRoundString(day, round) + "_starCount");
	}

	public static void SetHasCompletedAllRoundsInDay(int day, bool hasCompletedAllRounds)
	{
		SaveData.SetInt(FormatDayString(day) + "_hasCompletedAllRoundsInDay", (hasCompletedAllRounds ? 1 : 0));
		SaveAllData();
	}

	public static bool GetHasCompletedAllRoundsInDay(int day)
	{
		return (SaveData.GetInt(FormatDayString(day) + "_hasCompletedAllRoundsInDay") == 1);
	}

	private static string FormatDayString(int day)
	{
		return "D" + day;
	}

	private static string FormatRoundString(int day, int round)
	{
		return FormatDayString(day) + "_R" + round;
	}

	public static void SetAudioOn_sfx(bool isOn)
	{
		if (isOn)
			SaveData.SetInt("isAudioOn_sfx", 1);
		else
			SaveData.SetInt("isAudioOn_sfx", 0);
		SaveAllData();
	}

	public static bool GetAudioOn_sfx()
	{
		if (!SaveData.HasKey("isAudioOn_sfx"))	// if it has not been set, audio should be on
			return true;
		else if (SaveData.GetInt("isAudioOn_sfx") == 1)	// if it has been set to 1, audio should be on
			return true;
		else
			return false;
	}

	public static void SetAudioOn_music(bool isOn)
	{
		if (isOn)
			SaveData.SetInt("isAudioOn_music", 1);
		else
			SaveData.SetInt("isAudioOn_music", 0);
		SaveAllData();
	}
	
	public static bool GetAudioOn_music()
	{
		if (!SaveData.HasKey("isAudioOn_music"))	// if it has not been set, audio should be on
			return true;
		else if (SaveData.GetInt("isAudioOn_music") == 1)	// if it has been set to 1, audio should be on
			return true;
		else
			return false;
	}

	private static void SaveAllData()
	{
		SaveData.Save();
	}

	public static void DeleteAll()
	{
		SaveData.DeleteAll();
		SaveAllData();
	}
}
