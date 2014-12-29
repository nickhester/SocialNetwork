using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Calendar : MonoBehaviour {

	public GameObject calendarDayObject;
	private List<CalendarDay> dayList = new List<CalendarDay>();
	private const int daysToGenerate = 30;
	private GameObject dayParent;
	private const float distanceBetweenWeeks = 10.0f;
	private int viewingWeek;
	private bool isDebugActive = false;
	private int debugActivateClickCount = 0;
	private int furthestDayUnlocked = 0;

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
				furthestDayUnlocked = i;
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
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);

					reqList.Add(new validLevels(4, Types.Difficulty.VeryEasy, 358450, false, false, true, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 594364, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 542022, false, false, true, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 673234, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 35521, false, false, true, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 6:	// tuesday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(25, 25, 25, 25);

					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 531692, false, false, true, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 395761, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 273965, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 834762, false, false, false, false));
					reqList.Add(new validLevels(4, Types.Difficulty.Easy, 367595, false, false, true, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 549206, false, false, false, false));
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
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 555842, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 971095, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 109204, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 3 --------------
				// This week "fallToRed" special levels are introduced
					
				case 10:	// monday
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(0, 75, 25, 0);

					reqList.Add(new validLevels(5, Types.Difficulty.Easy, 497006, true, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 568529, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 923075, false, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 190513, true, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 11:	// tuesday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 50, 50, 0);

					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 996834, true, false, false, false));
					reqList.Add(new validLevels(5, Types.Difficulty.Medium, 563331, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 37622, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 399938, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 523313, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 12:	// wednesday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 50, 50, 0);

					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 393680, false, false, true, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 608186, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 290475, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 225281, true, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 87679, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 458246, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 13:	// thursday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 100, 0, 0);

					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 499828, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 530389, true, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 697705, true, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 941858, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 469668, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 226248, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 14:	// friday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 50, 50, 0);

					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 729545, true, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 578373, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 795167, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 498042, false, false, true, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 383528, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 373779, true, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 4 --------------
				// This week "oneClick" special levels are introduced
					
				case 15:	// monday
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(0, 0, 0, 100);

					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 226841, false, true, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 564213, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 823623, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 173226, false, true, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 16:	// tuesday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 25, 75, 0);

					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 299227, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 350236, false, true, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 495962, true, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 456126, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 878049, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 17:	// wednesday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 75, 25);

					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 596355, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 928784, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 831773, false, true, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 900183, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 479115, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 305434, false, true, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 18:	// thursday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 60, 40);

					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 80819, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 471503, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 255441, false, false, true, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 207756, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 425810, false, true, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 459147, true, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 19:	// friday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 60, 40);

					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 973499, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 192043, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 83437, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 371048, true, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 607871, false, true, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 22442, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 5 --------------
				// This week "noLines" special levels are introduced
					
				case 20:	// monday
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);

					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 927693, false, false, false, true));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 371048, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 452025, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 571617, false, false, false, true));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 21:	// tuesday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);

					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 460725, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 479409, false, true, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 102959, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 315767, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 780861, false, false, false, true));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 22:	// wednesday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);

					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 600791, false, false, false, true));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 748769, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 789758, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 903815, false, false, true, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 446865, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 811732, true, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 23:	// thursday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);

					reqList.Add(new validLevels(6, Types.Difficulty.Medium, 225978, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 384622, false, false, false, true));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 544317, false, false, true, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 125636, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 216198, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 875004, true, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 24:	// friday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);

					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 213290, false, true, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 186008, false, false, false, true));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 828931, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 542392, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 533452, true, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 304916, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 6 --------------
				// everything... go nuts
					
				case 25:	// monday
					_newCalDayComponent.numAppointments = 		4;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);

					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 143004, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 582033, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 610995, false, true, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 351956, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 26:	// tuesday
					_newCalDayComponent.numAppointments = 		5;
					_newCalDayComponent.SetDifficulties			(0, 0, 40, 60);

					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 370455, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 492777, true, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 591113, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 482001, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 504389, false, false, false, true));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 27:	// wednesday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 20, 80);

					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 643857, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 300787, false, false, true, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 871591, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 534160, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Medium, 103389, false, false, false, false));
					reqList.Add(new validLevels(6, Types.Difficulty.Hard, 143765, false, true, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 28:	// thursday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 20, 80);

					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 128387, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 481739, true, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 36335, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Medium, 213262, false, false, false, false));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 210624, false, false, false, true));
					reqList.Add(new validLevels(7, Types.Difficulty.Hard, 658771, false, true, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 29:	// friday
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(0, 0, 0, 100);

					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 930252, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 75075, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 349425, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 36163, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 153824, false, false, false, false));
					reqList.Add(new validLevels(8, Types.Difficulty.Hard, 225870, false, false, false, false));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
					
				default:	// shouldn't be used
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(20, 20, 20, 20);
					break;
			}

			_newCalDay.name = (_newCalDayComponent.dayIndex + 1).ToString() + " " + _newCalDayComponent.dayOfTheWeek.ToString();
			_newCalDayComponent.AddStatusOverlay();
		}

		// show instructions on specific weeks of the calendar view
		if (viewingWeek == 0)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstruction(0, false);
		}
		else if (viewingWeek == 1)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstruction(11, false);
		}
		else if (viewingWeek == 2)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstruction(13, false);
		}
		else if (viewingWeek == 3)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstruction(12, false);
		}
		else if (viewingWeek == 4)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstruction(14, false);
		}

		// show instructions on specific days of the calendar view
		if (furthestDayUnlocked == 2)
		{
			List<int> gameStartInstructionSeries = new List<int>();
			int[] temp = { 18, 19, 20, 21, 22, 23, 24, 25 };
			gameStartInstructionSeries.AddRange(temp);
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructionSeries(gameStartInstructionSeries, false);
		}
		else if (furthestDayUnlocked == 4)
		{
			List<int> gameStartInstructionSeries = new List<int>();
			int[] temp = { 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38 };
			gameStartInstructionSeries.AddRange(temp);
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructionSeries(gameStartInstructionSeries, false);
        }
		else if (furthestDayUnlocked == 8)
		{
			List<int> gameStartInstructionSeries = new List<int>();
			int[] temp = { 39, 40, 41, 42, 43, 44, 45 };
			gameStartInstructionSeries.AddRange(temp);
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructionSeries(gameStartInstructionSeries, false);
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

			// show debug button and allow player to click any level (but not save that the level is unlocked)
			if (debugActivateClickCount >= 5)
			{
				GUI.color = new Color(1, 1, 1, 1);
				isDebugActive = true;
				foreach (CalendarDay day in dayList)
				{
					day.isPlayable = true;
				}
			}
			// save data that the player has passed every day of a week, one week at a time
			if (debugActivateClickCount == 6)  { Debug_unlockLevelsOnWeek(1, 1); }
			if (debugActivateClickCount == 7)  { Debug_unlockLevelsOnWeek(2, 1); }
			if (debugActivateClickCount == 8)  { Debug_unlockLevelsOnWeek(3, 1); }
			if (debugActivateClickCount == 9)  { Debug_unlockLevelsOnWeek(4, 1); }
			if (debugActivateClickCount == 10) { Debug_unlockLevelsOnWeek(5, 1); }
			if (debugActivateClickCount == 11) { Debug_unlockLevelsOnWeek(6, 1); }
			// all levels will be 3-stared
			if (debugActivateClickCount == 12) { Debug_unlockLevelsOnWeek(6, 3); }
		}
	}

	void Debug_unlockLevelsOnWeek(int week, int numStars)
	{
		for (int i = 0; i < week * 5; i++) {
			SaveGame.SetHasCompletedAllRoundsInDay(i, true);
			SaveGame.SetDayIsPlayable(i, true);
			SaveGame.SetDayStarCount(i, Get_dayNumAppointments(i) * numStars);
			SaveGame.SetDayIsPlayable(i + 1, true);
			for (int j = 0; j < Get_dayNumAppointments(i); j++) {
				SaveGame.SetRoundStarCount(i, j, numStars);
			}
		}
	}
}
