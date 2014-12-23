using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class calendar : MonoBehaviour {

	public GameObject calendarDayObject;
	private List<CalendarDay> dayList = new List<CalendarDay>();
	private const int daysToGenerate = 30;
	private GameObject dayParent;
	private const float distanceBetweenWeeks = 10.0f;
	private int viewingWeek;
	private bool isDebugActive = false;
	private int debugActivateClickCount = 0;

	// Use this for initialization
	void Start () {

		float calendarDaySeparationVertical = 2.0f;
		dayParent = new GameObject("dayParent");
		for (int i = 0; i < daysToGenerate; i++)
		{
			// create calendar days in order
			GameObject _newCalDay = Instantiate(calendarDayObject, new Vector3(
				Mathf.Floor((i)/5.0f) * distanceBetweenWeeks,
				(-((i % 5)*calendarDaySeparationVertical) + ((calendarDaySeparationVertical/2.0f) * 3.7f))
				), Quaternion.identity) as GameObject;
			_newCalDay.transform.parent = dayParent.transform;
			CalendarDay _newCalDayComponent = _newCalDay.GetComponent<CalendarDay>();
			_newCalDayComponent.dayIndex = i;
			dayList.Add(_newCalDayComponent);

			// set player pref data
			// TODO: show total star count for day (this should already be determined)
			if (SaveGame.GetDayStarCount(i) == 0)
			{  }
			else if (SaveGame.GetDayStarCount(i) == 1)
			{ _newCalDayComponent.numStars = 1; }
			else if (SaveGame.GetDayStarCount(i) == 2)
			{ _newCalDayComponent.numStars = 2; }
			else if (SaveGame.GetDayStarCount(i) == 3)
			{ _newCalDayComponent.numStars = 3; }

			if (SaveGame.GetDayIsPlayable(i))
			{
				_newCalDayComponent.isPlayable = true;
				viewingWeek = (int)Mathf.Floor(i / 5.0f);
			}

			// configure settings for each day ##################################################
			List<validLevels> reqList = new List<validLevels>();
			switch (i)
			{
				// WEEK 1 --------------
				// This week, each level is hand-chosen to gradually increase difficulty

				case 0:	// monday
					_newCalDayComponent.isPlayable = true;					// the first level is always playable by default
					SaveGame.SetDayIsPlayable(0, true);

					_newCalDayComponent.numAppointments = 		3;
					_newCalDayComponent.SetDifficulties			(100, 0, 0, 0);

					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 874353, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 894346, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 577185, false, false, false, false));

					_newCalDayComponent.SetSpecificLevels(reqList);

					break;
				case 1: // tuesday
					
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(50, 50, 0, 0);
					
					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 278940, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 85586, false, false, false, false));
					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 315229, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 651607, false, false, false, false));

					_newCalDayComponent.SetSpecificLevels(reqList);
					
					break;
				case 2:	// wednesday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(25, 75, 0, 0);
					
					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 990103, false, false, false, false));
					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 116645, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 272873, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 890388, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 101640, false, false, false, false));

					_newCalDayComponent.SetSpecificLevels(reqList);
					
					break;
				case 3:	// thursday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(25, 50, 25, 0);
					
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 595357, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 186873, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 950079, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 595034, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 773589, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					
					break;
				case 4:	// friday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 50, 50, 0);
					
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 674841, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 29864, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 131922, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 996259, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 362846, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 666502, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 2 --------------
				// This week "cantTouch" special levels are introduced

				case 5:	// monday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);

					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 358450, false, false, true, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 674841, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 563331, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 542022, false, false, true, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 673234, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 35521, false, false, true, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 6:	// tuesday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);

					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 563331, false, false, true, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 395761, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 273965, false, false, true, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 834762, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 186873, false, false, true, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 773589, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 7:	// wednesday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);

					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 471243, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 135371, false, false, true, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 33313, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 172740, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 734778, false, false, true, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 602047, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 8:	// thursday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);

					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 694082, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 405497, false, false, true, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 786434, false, false, true, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 408562, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 475095, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 748285, false, false, true, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 9:	// friday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);

					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 125373, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 639339, false, false, true, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 721178, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 602047, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 971095, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 109204, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 3 --------------
				// This week "oneClick" special levels are introduced
					
				case 10:	// monday
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(0, 75, 25, 0);
					_newCalDayComponent.SetSpecialAttributes	(3, 0, 0, 0);

					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 794668, true, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 11:	// tuesday
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(0, 50, 50, 0);
					_newCalDayComponent.SetSpecialAttributes	(0, 3, 0, 0);

					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 630288, false, true, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 12:	// wednesday
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(0, 50, 50, 0);
					_newCalDayComponent.SetSpecialAttributes	(0, 0, 3, 0);

					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 127899, false, false, true, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 13:	// thursday
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(0, 100, 0, 0);
					_newCalDayComponent.SetSpecialAttributes	(0, 0, 0, 3);

					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 306922, false, false, false, true));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 14:	// friday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 50, 50, 0);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;

				// WEEK 4 --------------
				// This week "fallToRed" special levels are introduced
					
				case 15:	// monday
					_newCalDayComponent.numAppointments = 		3;
					_newCalDayComponent.SetDifficulties			(0, 0, 0, 100);
					_newCalDayComponent.SetSpecialAttributes	(0, 0, 0, 0);
					break;
				case 16:	// tuesday
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(0, 25, 75, 0);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 0, 0);
					break;
				case 17:	// wednesday
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(0, 0, 75, 25);
					_newCalDayComponent.SetSpecialAttributes	(0, 1, 1, 0);
					break;
				case 18:	// thursday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 0, 60, 40);
					_newCalDayComponent.SetSpecialAttributes	(1, 0, 1, 1);
					break;
				case 19:	// friday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 0, 60, 40);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;

				// WEEK 5 --------------
				// This week "noLines" special levels are introduced
					
				case 20:	// monday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;
				case 21:	// tuesday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;
				case 22:	// wednesday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;
				case 23:	// thursday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;
				case 24:	// friday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;

				// WEEK 6 --------------
				// everything...
					
				case 25:	// monday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;
				case 26:	// tuesday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;
				case 27:	// wednesday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 20, 80);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;
				case 28:	// thursday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 20, 80);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;
				case 29:	// friday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 0, 100);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;
					
				default:
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(20, 20, 20, 20);
					_newCalDayComponent.SetSpecialAttributes	(1, 1, 1, 1);
					break;
			}

			_newCalDay.name = (_newCalDayComponent.dayIndex + 1).ToString() + " " + _newCalDayComponent.dayOfTheWeek.ToString();
			_newCalDayComponent.AddStatusOverlay();
		}

		// show instructions on specific weeks of the calendar view
		if (viewingWeek == 0)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructions(0);
		}
		else if (viewingWeek == 1)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructions(11);
		}
		else if (viewingWeek == 2)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructions(12);
		}
		else if (viewingWeek == 3)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructions(13);
		}
		else if (viewingWeek == 4)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructions(14);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown(0))		// when you left click
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 10.0f))
			{
				if (hit.transform.tag == "calendarDay" && hit.transform.gameObject.GetComponent<CalendarDay>().isPlayable)
				{
					GameObject.Find("LevelSelector").GetComponent<levelSelector>().StartDay(hit.transform.GetComponent<CalendarDay>());
				}
				else if (hit.transform.name == "MainMenu")
				{
					Destroy(GameObject.FindObjectOfType<levelSelector>().gameObject);
					Application.LoadLevel("Scene_MainMenu");
				}
				else if (hit.transform.name == "Next Week")
				{
					if (viewingWeek < (Mathf.Floor(daysToGenerate/5.0f)) - 1)
					{ viewingWeek++; }
				}
				else if (hit.transform.name == "Last Week")
				{
					if (viewingWeek != 0)
					{ viewingWeek--; }
				}
			}
		}

		Vector3 targetWeekPosition = new Vector3(-viewingWeek * distanceBetweenWeeks, dayParent.transform.position.y, 0);
		dayParent.transform.position = Vector3.Lerp(dayParent.transform.position, targetWeekPosition, 0.1f);
	}

	public int Get_daysToGenerate()
	{
		return daysToGenerate;
	}

	public int Get_dayNumAppointments(int dayIndex)
	{
		return dayList[dayIndex].numAppointments;
	}

	void OnGUI()
	{
		// make any day playable whether or not you've "unlocked" it
		if (!isDebugActive)
			GUI.color = new Color(0, 0, 0, 0);

		if (GUI.Button(new Rect((Screen.width/2) - 50, Screen.height - 40, 100, 40), "*debug mode*\nall levels open"))
		{
			debugActivateClickCount++;

			if (debugActivateClickCount >= 5)
			{
				GUI.color = new Color(1, 1, 1, 1);
				isDebugActive = true;
				foreach (CalendarDay day in dayList)
				{
					day.isPlayable = true;
				}
			}
		}
	}
}
