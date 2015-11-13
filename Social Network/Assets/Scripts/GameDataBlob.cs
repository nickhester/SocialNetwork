using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class GameDataBlob
{
	public List<bool> seenInstructions;
	public List<List<int>> starCountDay;
	public float totalAppointmentTime;
	public bool hasUnlockedFullGame;

	public GameDataBlob()
	{
		//
	}

	public void Init()
	{
		seenInstructions = new List<bool>();
		starCountDay = new List<List<int>>();

		// find all instruction seen statuses
		for (int i = 0; i < 100; i++)
		{
			seenInstructions.Add(SaveGame.GetSeenInstruction(i));
		}

		totalAppointmentTime = SaveGame.GetTotalAppointmentTime();
		hasUnlockedFullGame = (SaveGame.GetCustomString("hasUpgraded") == "true");
	}

	public void AddDayStarCount(int day, int round, int stars)
	{
		if (day == starCountDay.Count)
		{
			starCountDay.Add(new List<int>());
		}
		else if (day > starCountDay.Count)
		{
			// this is fine, no need to add a new day yet
		}
		else if (day > starCountDay.Count + 1)
		{
			// throw exception, must have skipped one
			Debug.LogError("Blob: must have skipped a day");
		}

		if (round == starCountDay[day].Count)
		{
			starCountDay[day].Add(stars);
		}
		else if (round > starCountDay[day].Count)
		{
			// throw exception, must have skipped one
			Debug.LogError("Blob: must have skipped a level");
		}
	}

	public void ClearDayStarCount()
	{
		starCountDay = new List<List<int>>();
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
		starCountDay[day][round] = starCount;
	}

	// updated internally on data package
	public void UpdateHasUnlockedFullGame()
	{
		if (SaveGame.GetHasUpgraded())
		{
			hasUnlockedFullGame = true;
		}
	}

	public void UpdateToSend()
	{
		UpdateHasUnlockedFullGame();
		UpdateTotalAppointmentTime();

		//MonoBehaviour.print(this.ToString());
	}

	public override string ToString()
	{
		string retVal = "";
		retVal += "hasUnlockedFullGame = " + hasUnlockedFullGame + "\n";
		retVal += "totalAppointmentTime = " + totalAppointmentTime + "\n";
		retVal += "\nseenInstructions: \n";

		for (int i = 0; i < seenInstructions.Count; i++)
		{
			retVal += i + " = " + seenInstructions[i] + "\n";
		}

		retVal += "\nstarCountDay\n";

		for (int i = 0; i < starCountDay.Count; i++)
		{
			for (int j = 0; j < starCountDay[i].Count; j++)
			{
				retVal += i + "," + j + " = " + starCountDay[i][j] + "\n";
			}
		}

		return retVal;
	}
}
