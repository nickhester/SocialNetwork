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
				// WEEK 1 ----
				case 0:
					_newCalDayComponent.isPlayable = true;					// the first level is always playable by default

					_newCalDayComponent.numAppointments = 		3;
					_newCalDayComponent.SetDifficulties			(100, 0, 0, 0);
					//_newCalDayComponent.SetRequirementsForStars	(3, 6, 9);
					_newCalDayComponent.SetRequirementsForStars	(99, 99, 99);

					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 874353, false, false, false, false));
					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 278940, false, false, false, false));
					reqList.Add(new validLevels(3, Types.Difficulty.VeryEasy, 315229, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);

					break;
				case 1:
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(25, 75, 0, 0);
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 27);

					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 990103, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 899196, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 890388, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 595357, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 101640, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);

					break;
				case 2:
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(25, 50, 25, 0);
					_newCalDayComponent.SetRequirementsForStars	(14, 28, 42);

					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 653977, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 633214, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 950079, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 595034, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 773589, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);

					break;
				case 3:
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 40, 50, 0);
					_newCalDayComponent.SetRequirementsForStars	(40, 60, 80);
					break;
				case 4:
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 40, 50, 0);
					_newCalDayComponent.SetRequirementsForStars	(40, 60, 80);
					break;

				// WEEK 2 ----

					// these are for testing the "special" levels
				case 5:	// monday
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 30);
					_newCalDayComponent.SetSpecialAttributes	(8, 0, 0, 0);
					break;
				case 6:	// tuesday
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 30);
					_newCalDayComponent.SetSpecialAttributes	(0, 8, 0, 0);
					break;
				case 7:	// wednesday
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 30);
					_newCalDayComponent.SetSpecialAttributes	(0, 0, 8, 0);
					break;
				case 8:	// thursday
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 100);
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 30);
					_newCalDayComponent.SetSpecialAttributes	(0, 0, 0, 8);
					break;
				case 9:	// friday
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 30);
					_newCalDayComponent.SetSpecialAttributes	(2, 2, 2, 2);
					break;

					// this is a test level
//				case 10:
//					_newCalDayComponent.numAppointments = 		8;
//					_newCalDayComponent.SetDifficulties			(0, 0, 100, 0);
//					_newCalDayComponent.SetRequirementsForStars	(0, 0, 0);
//					break;

				default:
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 30);
					break;
			}

			_newCalDay.name = (_newCalDayComponent.dayIndex + 1).ToString() + " " + _newCalDayComponent.dayOfTheWeek.ToString();
			_newCalDayComponent.AddStatusOverlay();
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
		if (GUI.Button(new Rect(130, Screen.height - 40, 100, 20), "unlock days"))
		{
			foreach (CalendarDay day in dayList)
			{
				day.isPlayable = true;
			}
		}
	}
}
