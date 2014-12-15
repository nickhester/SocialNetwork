using UnityEngine;
using System.Collections;

public class SaveGame {

	public static void SetSeenInstruction(int instructionIndex, bool hasSeen)
	{
		if (hasSeen)
			SaveData.SetInt("hasSeenInstruction" + instructionIndex, 1);
		else
			SaveData.SetInt("hasSeenInstruction" + instructionIndex, 0);
		SaveAllData();
	}

	public static bool GetSeenInstruction(int instructionIndex)
	{
		return (SaveData.GetInt("hasSeenInstruction" + instructionIndex) == 1);
	}

	public static void SetDayStarCount(int day, int count)
	{
		SaveData.SetInt(GetDayStarCount(day) + "_starCount", count);
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

	private static string FormatDayString(int day)
	{
		return "D" + day;
	}

	private static string FormatRoundString(int day, int round)
	{
		return FormatDayString(day) + "_R" + round;
	}

	public static void SetDayIsPlayable(int day, bool playable)
	{
		if (playable)
			SaveData.SetInt(FormatDayString(day) + "_isPlayable", 1);
		else
			SaveData.SetInt(FormatDayString(day) + "_isPlayable", 0);
		SaveAllData();
	}
	
	public static bool GetDayIsPlayable(int day)
	{
		return (SaveData.GetInt(FormatDayString(day) + "_isPlayable") == 1);
	}

	public static void SetAudioOn(bool isOn)
	{
		if (isOn)
			SaveData.SetInt("isAudioOn", 1);
		else
			SaveData.SetInt("isAudioOn", 0);
		SaveAllData();
	}

	public static bool GetAudioOn()
	{
		if (!SaveData.HasKey("isAudioOn"))	// if it has not been set, audio should be on
			return true;
		else if (SaveData.GetInt("isAudioOn") == 1)	// if it has been set to 1, audio should be on
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
	}
}
