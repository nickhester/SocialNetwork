using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class calendar : MonoBehaviour {

	public GameObject calendarDayObject;
	private List<CalendarDay> dayList = new List<CalendarDay>();

	// Use this for initialization
	void Start () {

		float calendarDaySeparationHorizontal = 2.0f;
		float calendarDaySeparationVertical = 2.0f;
		for (int i = 0; i < 20; i++)
		{
			// create calendar days in order, 5 days in each row, 4 rows
			GameObject _newCalDay = Instantiate(calendarDayObject, new Vector3(
				((i%5)*calendarDaySeparationHorizontal) - ((calendarDaySeparationHorizontal/2.0f) * 4),
				-((Mathf.Floor(i/5.0f))*calendarDaySeparationVertical) + ((calendarDaySeparationVertical/2.0f) * 3)
				), Quaternion.identity) as GameObject;
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
			{ _newCalDayComponent.isPlayable = true; }

			// configure settings for each day ##################################################

			switch (i)
			{
				// WEEK 1 ----
				case 0:
					_newCalDayComponent.isPlayable = true;					// the first level is always playable by default

					_newCalDayComponent.numAppointments = 		3;
					_newCalDayComponent.SetDifficulties			(100, 0, 0, 0);
					_newCalDayComponent.timeLimit = 			180;
					_newCalDayComponent.SetRequirementsForStars	(5, 15, 25);
					break;
				case 1:
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(25, 75, 0, 0);
					_newCalDayComponent.timeLimit = 			240;
					_newCalDayComponent.SetRequirementsForStars	(25, 35, 45);
					break;
				case 2:
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(25, 50, 25, 0);
					_newCalDayComponent.timeLimit = 			300;
					_newCalDayComponent.SetRequirementsForStars	(50, 70, 90);
					break;
				case 3:
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(10, 40, 50, 0);
					_newCalDayComponent.timeLimit = 			300;
					_newCalDayComponent.SetRequirementsForStars	(40, 60, 80);
					break;
				case 4:
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(10, 40, 50, 0);
					_newCalDayComponent.timeLimit = 			120;
					_newCalDayComponent.SetRequirementsForStars	(40, 60, 80);
					break;

				// WEEK 2 ----

					// these are for testing the "special" levels
				case 5:	// monday
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(100, 0, 0, 0);
					_newCalDayComponent.timeLimit = 			360;
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 30);
					_newCalDayComponent.SetSpecialAttributes	(8, 0, 0, 0);
					break;
				case 6:	// tuesday
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(0, 100, 0, 0);
					_newCalDayComponent.timeLimit = 			360;
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 30);
					_newCalDayComponent.SetSpecialAttributes	(0, 8, 0, 0);
					break;
				case 7:	// wednesday
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(0, 100, 0, 0);
					_newCalDayComponent.timeLimit = 			360;
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 30);
					_newCalDayComponent.SetSpecialAttributes	(0, 0, 8, 0);
					break;
				case 8:	// thursday
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(0, 0, 0, 100);
					_newCalDayComponent.timeLimit = 			360;
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 30);
					_newCalDayComponent.SetSpecialAttributes	(0, 0, 0, 8);
					break;
				case 9:	// friday
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);
					_newCalDayComponent.timeLimit = 			600;
					_newCalDayComponent.SetRequirementsForStars	(10, 20, 30);
					_newCalDayComponent.SetSpecialAttributes	(2, 2, 2, 2);
					break;

					// this is a test level
				case 10:
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(0, 0, 100, 0);
					_newCalDayComponent.timeLimit = 			1000;
					_newCalDayComponent.SetRequirementsForStars	(0, 0, 0);
					break;

				default:
					_newCalDayComponent.numAppointments = 		8;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);
					_newCalDayComponent.timeLimit = 			360;
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
			}
		}
	
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
