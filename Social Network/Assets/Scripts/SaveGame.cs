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
	public static int lastCalendarDayClicked = -1;

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
				_numDaysCompleted = i + 1;										// count it as a completed day
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

	/*		// for now, I'll just keep sending the achievement each time it checks for it.
	public static void SetHasCompletedWeek(int week, bool hasCompleted)
	{
		SaveData.SetInt("Week" + week + "_hasCompleted", (hasCompleted ? 1 : 0));
		SaveAllData();
	}

	public static bool GetHasCompletedWeek(int week)
	{
		return (SaveData.GetInt("Week" + week + "_hasCompleted") == 1);
	}

	public static void SetHasPerfectedWeek(int week, bool hasPerfected)
	{
		SaveData.SetInt("Week" + week + "_hasPerfected", (hasPerfected ? 1 : 0));
		SaveAllData();
	}
	
	public static bool GetHasPerfectedWeek(int week)
	{
		return (SaveData.GetInt("Week" + week + "_hasPerfected") == 1);
	}
	*/

	public static void SetSeenInstruction(int instructionIndex, bool hasSeen)
	{
		SaveData.SetInt("hasSeenInstruction" + instructionIndex, (hasSeen ? 1 : 0));
		SaveAllData();

		GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().gameDataBlob.UpdateSeenInstructions(instructionIndex);
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

		GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().gameDataBlob.UpdateStarCountDay(day, round, count);
	}
	
	public static int GetRoundStarCount(int day, int round)
	{
		bool hasIt = SaveData.HasKey(FormatRoundString(day, round) + "_starCount");
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

	public static void SetTotalAppointmentTime(float t)
	{
		SaveData.SetFloat("accumulatedAppointmentTime", t);
		SaveAllData();
	}

	public static float GetTotalAppointmentTime()
	{
		if (SaveData.HasKey("accumulatedAppointmentTime"))
		{
			return SaveData.GetFloat("accumulatedAppointmentTime");
		}
		else
		{
			return 0.0f;
		}
	}

	public static void SetHasUpgraded(bool hasUpgraded)
	{
		if (hasUpgraded)
		{
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().gameDataBlob.hasUnlockedFullGame = true;
		}

		SaveData.SetInt("hasUpgraded", (hasUpgraded ? 1 : 0));
	}

	public static bool GetHasUpgraded()
	{
		return ((SaveData.GetInt("hasUpgraded") == 1) ? true : false);
	}

	// debug only
	public static void SetCustomString(string key, string value)
	{
		SaveData.SetString(key, value);
		SaveAllData();
	}

	public static string GetCustomString(string key)
	{
		return SaveData.GetString(key);
	}
	
	public static void LocalSaveToCloudSave(GameDataBlob blob)
	{
		
		// send to google saved game
	}
	
	public static void CloudSaveToLocalSave(GameDataBlob blob)
	{
		if (blob == null)
		{
			Debug.LogWarning("Incoming save data is null or unreadable. Save updated aborting.");
			return;
		}

		// update upgraded status if it's not already
		if (blob.hasUnlockedFullGame && !GetHasUpgraded())
		{
			SetHasUpgraded(true);
		}

		// update instruction status if it's less
		for (int i = 0; i < blob.seenInstructions.Length; i++)
		{
			if (GetSeenInstruction(i) == false && blob.seenInstructions[i] == true)
			{
				SetSeenInstruction(i, true);
			}
		}

		// update appointment time
		if (blob.totalAppointmentTime > GetTotalAppointmentTime())
		{
			SetTotalAppointmentTime(blob.totalAppointmentTime);
		}

		// update star statuses
		for (int i = 0; i < blob.starCountDay.GetLength(0); i++)
		{
			for (int j = 0; j < blob.starCountDay.GetLength(1); j++)
			{
				SetRoundStarCount(i, j, blob.starCountDay[i,j]);
			}
		}
	}
}
