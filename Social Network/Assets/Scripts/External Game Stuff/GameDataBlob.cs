﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class GameDataBlob
{
	// HACK: these values are hard coded, and will need to be updated if they change
	const int numInstructions = 101;
	const int numDaysOnCalendar = 30;
	const int numAppointmentsPerDayMax = 5;

	public int saveDataFormatVersion = -1;
	public bool[] seenInstructions = new bool[numInstructions];
	public int[,] starCountDay = new int[30, numAppointmentsPerDayMax];
	public float totalAppointmentTime;
	public bool hasUnlockedFullGame;

	public GameDataBlob()
	{
		//
	}

	public void Init(int _saveDataFormatVersion)
	{
		// find all instruction seen statuses
		for (int i = 0; i < numInstructions; i++)
		{
			seenInstructions[i] = SaveGame.GetSeenInstruction(i);
		}

		totalAppointmentTime = SaveGame.GetTotalAppointmentTime();
		hasUnlockedFullGame = SaveGame.GetHasUpgraded();

		saveDataFormatVersion = _saveDataFormatVersion;
	}

	public void AddDayStarCount(int day, int round, int stars)
	{
		starCountDay[day, round] = stars;
	}

	public void ClearDayStarCount()
	{
		starCountDay = new int[30, numAppointmentsPerDayMax];
	}

	// updated internally on data package
	public void UpdateTotalAppointmentTime()
	{
		totalAppointmentTime = SaveGame.GetTotalAppointmentTime();
	}

	// updated externally on instruction save
	public void UpdateSeenInstructions(int instructionIndex)
	{
		seenInstructions[instructionIndex] = true;
	}

	// updated externally on level complete
	public void UpdateStarCountDay(int day, int round, int starCount)
	{
		starCountDay[day,round] = starCount;
	}

	// updated internally on data package
	public void UpdateHasUnlockedFullGame()
	{
		hasUnlockedFullGame = SaveGame.GetHasUpgraded();
	}

	public void UpdateToSend(int _version)
	{
		UpdateHasUnlockedFullGame();
		UpdateTotalAppointmentTime();
		saveDataFormatVersion = _version;
	}

	public override string ToString()
	{
		string retVal = "";
		retVal += "saveVersion = " + saveDataFormatVersion + "\n";
		retVal += "hasUnlockedFullGame = " + hasUnlockedFullGame + "\n";
		retVal += "totalAppointmentTime = " + totalAppointmentTime + "\n";
		retVal += "\nseenInstructions: \n";

		for (int i = 0; i < seenInstructions.GetLength(0); i++)
		{
			retVal += i + " = " + seenInstructions[i] + " -- ";
		}

		retVal += "\nstarCountDay\n";

		for (int i = 0; i < starCountDay.GetLength(0); i++)
		{
			for (int j = 0; j < starCountDay.GetLength(1); j++)
			{
				retVal += i + "," + j + " = " + starCountDay[i,j] + " -- ";
			}
		}
		retVal += "\n";

		return retVal;
	}
}
