using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class DayScheduleFactory {

	private int numAppointmentsToCreate;
	private CalendarDay dayToCreate;
	private List<Difficulty> listOfLevelDifficulties;
	private List<int> listofLevelNumber;
	private List<SpecialLevelAttributes> listOfSpecialAttributes;
	private List<int> listLevel;

	public List<validLevels> GenerateAppointments(CalendarDay day)
	{
		numAppointmentsToCreate = day.numAppointments;
		dayToCreate = day;
		listOfLevelDifficulties = new List<Difficulty>();
		listofLevelNumber = new List<int>();
		listOfSpecialAttributes = new List<SpecialLevelAttributes>();
		listLevel = new List<int>();
		listLevel.Add(3); listLevel.Add(4); listLevel.Add(5); listLevel.Add(6); listLevel.Add(7); listLevel.Add(8);

		if (day.hasSpecificLevelRequests)
		{
			numAppointmentsToCreate -= day.specificLevelsRequested.Count;
		}
		GenerateListOfDifficultiesAndLevels(numAppointmentsToCreate);

		for (int i = 0; i < day.numAppointments; i++)
		{
			validLevels lev;

		}

		// just to compile
		List<validLevels> aList = new List<validLevels>();
		return aList;
	}

	void ShuffleAppointments(ref List<Appointment> listOfAppointments)
	{

	}

	void GenerateListOfDifficultiesAndLevels(int levelsToGenerate)
	{
		for (float i = 0; i < levelsToGenerate; i++)
		{
			if ((i / levelsToGenerate) * 100.0f <= dayToCreate.percentVeryEasy-1 
			    && dayToCreate.percentVeryEasy != 0)
			{
				listOfLevelDifficulties.Add(Difficulty.VeryEasy);
				listofLevelNumber.Add(Random.Range(0, 2) + 3);
				// levels 3 or 4
			}
			else if ((i / levelsToGenerate) * 100.0f <= dayToCreate.percentVeryEasy-1 
			         + dayToCreate.percentEasy 
			         && dayToCreate.percentEasy != 0)
			{
				listOfLevelDifficulties.Add(Difficulty.Easy);
				listofLevelNumber.Add(Random.Range(1, 3) + 3);
				// levels 4, 5
			}
			else if ((i / levelsToGenerate) * 100.0f <= dayToCreate.percentVeryEasy-1 
			         + dayToCreate.percentEasy 
			         + dayToCreate.percentMedium 
			         && dayToCreate.percentMedium != 0)
			{
				listOfLevelDifficulties.Add(Difficulty.Medium);
				listofLevelNumber.Add(Random.Range(2, listLevel.Count) + 3);
				// levels 5 and up
			}
			else
			{
				listOfLevelDifficulties.Add(Difficulty.Hard);
				listofLevelNumber.Add(Random.Range(3, listLevel.Count) + 3);
				// levels 6 and up
			}
			
			if (dayToCreate.special_CantTouch > 0)
			{
				listOfSpecialAttributes.Add(SpecialLevelAttributes.CantTouch);
				dayToCreate.special_CantTouch--;
			}
			else if (dayToCreate.special_FallToRed > 0)
			{
				listOfSpecialAttributes.Add(SpecialLevelAttributes.FallToRed);
				dayToCreate.special_FallToRed--;
			}
			else if (dayToCreate.special_NoLines > 0)
			{
				listOfSpecialAttributes.Add(SpecialLevelAttributes.NoLines);
				dayToCreate.special_NoLines--;
			}
			else if (dayToCreate.special_OneClick > 0)
			{
				listOfSpecialAttributes.Add(SpecialLevelAttributes.OneClick);
				dayToCreate.special_OneClick--;
			}
			else
			{
				listOfSpecialAttributes.Add(SpecialLevelAttributes.None);
			}
		}
	}
}
