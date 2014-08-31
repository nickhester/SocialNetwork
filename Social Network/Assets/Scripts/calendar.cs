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
			if (PlayerPrefs.GetInt("M1_D" + i + "_starCount") == 0)
			{  }
			else if (PlayerPrefs.GetInt("M1_D" + i + "_starCount") == 1)
			{ _newCalDayComponent.numStars = 1; }
			else if (PlayerPrefs.GetInt("M1_D" + i + "_starCount") == 2)
			{ _newCalDayComponent.numStars = 2; }
			else if (PlayerPrefs.GetInt("M1_D" + i + "_starCount") == 3)
			{ _newCalDayComponent.numStars = 3; }

			if (PlayerPrefs.GetInt("M1_D" + i + "_isPlayable") == 1)
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

					_newCalDayComponent.numAppointments = 		3;
					_newCalDayComponent.SetDifficulties			(100, 0, 0, 0);

					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 874353, false, false, false, false));
					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 278940, false, false, false, false));
					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 315229, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);

					break;
				case 1: // tuesday
					
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(50, 50, 0, 0);
					
					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 651607, false, false, false, false));
					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 116645, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 213762, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 85586, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					
					break;
				case 2:	// wednesday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(25, 75, 0, 0);
					
					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 990103, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 899196, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 890388, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 595357, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 101640, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					
					break;
				case 3:	// thursday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(25, 50, 25, 0);
					
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 653977, false, false, false, false));
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
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 666502, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 207756, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 2 --------------
				// This week, the levels are randomly generated, and the hardest levels are introduced

				case 5:	// monday
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(0, 25, 75, 0);
					break;
				case 6:	// tuesday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 20, 60, 20);
					break;
				case 7:	// wednesday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 20, 60, 20);
					break;
				case 8:	// thursday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 0, 60, 40);
					break;
				case 9:	// friday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 60, 40);
					break;

				// WEEK 3 --------------
				// This week, special levels are introduced with mostly easy levels.
				// Each appt starts with a chosen level before then being randomly chosen.
					
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
				// All levels are random now, special levels interspersed manually
					
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
				// same as before...
					
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
					// same as before...
					
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
		if (viewingWeek == 0)
		{
			// show score instructions the first time see the calendar
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructions(0);
		}
		else if (viewingWeek == 1)
		{
			// show score instructions when you start the second week
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructions(4);
		}
		else if (viewingWeek == 2)
		{
			// show score instructions when you start the third week
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructions(5);
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

	void OnGUI()
	{
		// make any day playable whether or not you've "unlocked" it
		if (!isDebugActive)
			GUI.color = new Color(0, 0, 0, 0);

		if (GUI.Button(new Rect(130, Screen.height - 40, 100, 40), "*debug mode*\nall levels open"))
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
