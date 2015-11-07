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

	private float audioButtonScreenPos_X = 0.95f;
	private float audioButtonScreenPos_Y = 0.97f;
	private float audioIconSpacing = 0.81f;

    void Awake()
    {
        InputManager.Instance.OnClick += OnClick;
    }

    private void OnClick(GameObject go)
    {
        if (go.transform.tag == "calendarDay" && go.transform.gameObject.GetComponent<CalendarDay>().isPlayable)
        {
            CalendarDay hitDay = go.transform.GetComponent<CalendarDay>();
            SaveGame.lastCalendarDayClicked = hitDay.dayIndex_internal;
            GameObject.Find("LevelSelector").GetComponent<LevelSelector>().StartDay(hitDay);
        }
        else if (go.transform.name == "MainMenu")
        {
			UnloadCalendarAndLoadLevel("Scene_MainMenu");
        }
        else if (go.transform.name == "Next Week")
        {
            if (viewingWeek < (Mathf.Floor(daysToGenerate / 5.0f)) - 1)
            { viewingWeek++; }
        }
        else if (go.transform.name == "Last Week")
        {
            if (viewingWeek != 0)
            { viewingWeek--; }
        }
		else if (go.transform.name.StartsWith("UpgradeButton_Yes"))
		{
			GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(22, true);
			MetricsLogger.Instance.LogCustomEvent("Game", "Unlock", "Yes");
		}
		else if (go.transform.name.StartsWith("UpgradeButton_KeepPlaying"))
		{
			MetricsLogger.Instance.LogCustomEvent("Game", "Unlock", "KeepPlaying");
		}
		else if (go.transform.name.StartsWith("UpgradeButton_No"))
		{
			MetricsLogger.Instance.LogCustomEvent("Game", "Unlock", "No");
		}
		else if (go.transform.name.StartsWith("UpgradeButton_Unlock"))
		{
			Upgrade.PurchaseUpgrade(1);
			MetricsLogger.Instance.LogBusinessEvent("US Dollars", 199, "GameUpgrade", "N/A", "N/A", "N/A", "N/A");
		}
		else if (go.transform.name.StartsWith("UpgradeButton_Cancel"))
		{
			MetricsLogger.Instance.LogCustomEvent("Game", "Unlock", "Cancel");
		}
		else if (go.transform.name.StartsWith("UpgradeButton_RateIt"))
		{
			MetricsLogger.Instance.LogCustomEvent("Game", "Rate", "Accept");
			Application.OpenURL("market://details?id=com.hestergames.socialsessions");
		}
		else if (go.transform.name.StartsWith("Lock"))
		{
			GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(21, true);
		}

    }

	// Use this for initialization
	void Start () {

		// position audio icons on edge of screen
		Vector3 audioIconPositions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * audioButtonScreenPos_X, Screen.height * audioButtonScreenPos_Y));
		GameObject iconAudio = GameObject.Find("audioToggle_music");
		GameObject iconSfx = GameObject.Find("audioToggle_sfx");
		iconAudio.transform.position = new Vector3(audioIconPositions.x - audioIconSpacing, audioIconPositions.y, iconAudio.transform.position.z);
		iconSfx.transform.position = new Vector3(audioIconPositions.x, audioIconPositions.y, iconSfx.transform.position.z);

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
			{
				_newCalDayComponent.SetNumStars(1);
			}
			else if (SaveGame.GetDayStarCount(i) == 2)
			{
				_newCalDayComponent.SetNumStars(2);
			}
			else if (SaveGame.GetDayStarCount(i) == 3)
			{
				_newCalDayComponent.SetNumStars(3);
			}

			bool isShowingLock = false;
			if (i == Upgrade.dayLocked && !Upgrade.canVerifyUpgrade())
			{
				// if the unlock has not been purchased, show the lock icon
				isShowingLock = true;
				_newCalDayComponent.ShowLock();
			}

			if (i == 0 || SaveGame.GetHasCompletedAllRoundsInDay(i - 1))
			{
				if (!isShowingLock)
				{
					_newCalDayComponent.isPlayable = true;
				}
				viewingWeek = i/5;
				furthestDayUnlocked = i;
			}
			if (SaveGame.lastCalendarDayClicked != -1)
			{
				// if the last viewed day was a completed friday, then skip to the next week
				if (SaveGame.lastCalendarDayClicked % 5 == 4 && SaveGame.GetHasCompletedAllRoundsInDay(SaveGame.lastCalendarDayClicked))
					viewingWeek = (SaveGame.lastCalendarDayClicked / 5) + 1;
				// otherwise, keep the calendar on the day you last played
				else
					viewingWeek = SaveGame.lastCalendarDayClicked / 5;
				// finally, if the viewing week is a week that doesn't exist, bring it back in
				if (viewingWeek > (daysToGenerate/5))
					viewingWeek = daysToGenerate/5;
			}

			// configure settings for each day ##################################################
			List<ValidLevels> reqList = new List<ValidLevels>();
			switch (i)
			{
				// WEEK 1 --------------
				// This week, each level is hand-chosen to gradually increase difficulty

				case 0:	// monday
					_newCalDayComponent.isPlayable = true;					// the first level is always playable by default

					_newCalDayComponent.numAppointments = 3;

					reqList.Add(new ValidLevels(3, Types.Difficulty.VeryEasy, 874353, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(4, Types.Difficulty.VeryEasy, 894346, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(4, Types.Difficulty.VeryEasy, 577185, Types.SpecialLevel.None));

					_newCalDayComponent.SetSpecificLevels(reqList);

					break;
				case 1: // tuesday
					
					_newCalDayComponent.numAppointments = 3;
					
					reqList.Add(new ValidLevels(3, Types.Difficulty.VeryEasy, 278940, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(4, Types.Difficulty.VeryEasy, 85586, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(3, Types.Difficulty.VeryEasy, 315229, Types.SpecialLevel.None));

					_newCalDayComponent.SetSpecificLevels(reqList);
					
					break;
				case 2:	// wednesday
                    _newCalDayComponent.numAppointments = 3;
					
					reqList.Add(new ValidLevels(4, Types.Difficulty.VeryEasy, 990103, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(3, Types.Difficulty.VeryEasy, 116645, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Easy, 101640, Types.SpecialLevel.None));

					_newCalDayComponent.SetSpecificLevels(reqList);
					
					break;
				case 3:	// thursday
                    _newCalDayComponent.numAppointments = 4;
					
					reqList.Add(new ValidLevels(4, Types.Difficulty.Easy, 595357, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 950079, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Easy, 595034, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 773589, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					
					break;
				case 4:	// friday
                    _newCalDayComponent.numAppointments = 4;
					
					reqList.Add(new ValidLevels(5, Types.Difficulty.Easy, 674841, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 29864, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Easy, 131922, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 666502, Types.SpecialLevel.None));
					// cull //reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 996259, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 2 --------------
				// This week "cantTouch" special levels are introduced

				case 5:	// monday
                    _newCalDayComponent.numAppointments = 3;

					reqList.Add(new ValidLevels(4, Types.Difficulty.VeryEasy, 358450, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Easy, 35521, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(4, Types.Difficulty.Easy, 542022, Types.SpecialLevel.CantTouch));
					// cull //reqList.Add(new ValidLevels(5, Types.Difficulty.Easy, 594364, Types.SpecialLevel.None));
					// cull //reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 673234, Types.SpecialLevel.None));
					
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 6:	// tuesday
                    _newCalDayComponent.numAppointments = 4;

                    reqList.Add(new ValidLevels(4, Types.Difficulty.Easy, 367595, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 549206, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 395761, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 834762, Types.SpecialLevel.None));
					// cull //reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 531692, Types.SpecialLevel.CantTouch));
					
					
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 7:	// wednesday
                    _newCalDayComponent.numAppointments = 4;

                    reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 273965, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Easy, 135371, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 33313, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 172740, Types.SpecialLevel.CantTouch));
					// cull //reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 471243, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 8:	// thursday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 734778, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 602047, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 786434, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 475095, Types.SpecialLevel.None));
					// cull //reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 748285, Types.SpecialLevel.CantTouch));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 9:	// friday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 125373, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 721178, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 971095, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 109204, Types.SpecialLevel.None));
                    // cull //reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 408562, Types.SpecialLevel.None));
                    // cull //reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 405497, Types.SpecialLevel.None));
                    // cull //reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 694082, Types.SpecialLevel.None));
					// cull //reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 639339, Types.SpecialLevel.CantTouch));
					// cull //reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 555842, Types.SpecialLevel.CantTouch));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 3 --------------
				// This week "fallToRed" special levels are introduced
					
				case 10:	// monday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(5, Types.Difficulty.Easy, 497006, Types.SpecialLevel.FallToRed));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 923075, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 190513, Types.SpecialLevel.FallToRed));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 568529, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 11:	// tuesday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 996834, Types.SpecialLevel.FallToRed));
					reqList.Add(new ValidLevels(5, Types.Difficulty.Medium, 563331, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 37622, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 523313, Types.SpecialLevel.None));
					//reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 399938, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 12:	// wednesday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 393680, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 290475, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 225281, Types.SpecialLevel.FallToRed));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 458246, Types.SpecialLevel.None));
					//reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 608186, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 13:	// thursday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 530389, Types.SpecialLevel.FallToRed));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 697705, Types.SpecialLevel.FallToRed));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 469668, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 226248, Types.SpecialLevel.None));
					//reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 499828, Types.SpecialLevel.None));
					//reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 941858, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 14:	// friday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 729545, Types.SpecialLevel.FallToRed));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 383528, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 498042, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 373779, Types.SpecialLevel.FallToRed));
					//reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 578373, Types.SpecialLevel.None));
					//reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 795167, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 4 --------------
				// This week "oneClick" special levels are introduced
					
				case 15:	// monday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 226841, Types.SpecialLevel.OneClick));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 564213, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 823623, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 173226, Types.SpecialLevel.OneClick));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 16:	// tuesday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 299227, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 350236, Types.SpecialLevel.OneClick));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 495962, Types.SpecialLevel.FallToRed));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 878049, Types.SpecialLevel.None));
					//reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 456126, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 17:	// wednesday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 596355, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 831773, Types.SpecialLevel.OneClick));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 900183, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 305434, Types.SpecialLevel.OneClick));
					//reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 928784, Types.SpecialLevel.None));
					//reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 479115, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 18:	// thursday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 80819, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 255441, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 425810, Types.SpecialLevel.OneClick));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 459147, Types.SpecialLevel.FallToRed));
					//reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 471503, Types.SpecialLevel.None));
					//reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 207756, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 19:	// friday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 973499, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 192043, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 83437, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 607871, Types.SpecialLevel.OneClick));
					//reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 371048, Types.SpecialLevel.FallToRed));
					//reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 22442, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 5 --------------
				// This week "noLines" special levels are introduced
					
				case 20:	// monday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 927693, Types.SpecialLevel.NoLines));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 488836, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 452025, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 571617, Types.SpecialLevel.NoLines));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 21:	// tuesday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 479409, Types.SpecialLevel.OneClick));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 102959, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 315767, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 780861, Types.SpecialLevel.NoLines));
					//reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 460725, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 22:	// wednesday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 600791, Types.SpecialLevel.NoLines));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 789758, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 903815, Types.SpecialLevel.CantTouch));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 811732, Types.SpecialLevel.FallToRed));
					//reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 748769, Types.SpecialLevel.None));
					//reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 446865, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 23:	// thursday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(6, Types.Difficulty.Medium, 225978, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 384622, Types.SpecialLevel.NoLines));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 216198, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 875004, Types.SpecialLevel.FallToRed));
					//reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 544317, Types.SpecialLevel.CantTouch));
					//reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 125636, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 24:	// friday
                    _newCalDayComponent.numAppointments = 5;

					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 213290, Types.SpecialLevel.OneClick));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 186008, Types.SpecialLevel.NoLines));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 828931, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 542392, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 533452, Types.SpecialLevel.FallToRed));
					//reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 304916, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;

				// WEEK 6 --------------
				// everything... go nuts
					
				case 25:	// monday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 143004, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 582033, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 610995, Types.SpecialLevel.OneClick));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 351956, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 26:	// tuesday
                    _newCalDayComponent.numAppointments = 4;

					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 370455, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 591113, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 482001, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 504389, Types.SpecialLevel.NoLines));
					//reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 492777, Types.SpecialLevel.FallToRed));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 27:	// wednesday
                    _newCalDayComponent.numAppointments = 5;

					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 643857, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 871591, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 534160, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Medium, 103389, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(6, Types.Difficulty.Hard, 143765, Types.SpecialLevel.OneClick));
					//reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 300787, Types.SpecialLevel.CantTouch));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 28:	// thursday
                    _newCalDayComponent.numAppointments = 5;

					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 128387, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 481739, Types.SpecialLevel.FallToRed));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 36335, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 210624, Types.SpecialLevel.NoLines));
					reqList.Add(new ValidLevels(7, Types.Difficulty.Hard, 658771, Types.SpecialLevel.OneClick));
					//reqList.Add(new ValidLevels(8, Types.Difficulty.Medium, 213262, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
				case 29:	// friday
                    _newCalDayComponent.numAppointments = 5;

					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 930252, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 349425, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 36163, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 153824, Types.SpecialLevel.None));
					reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 225870, Types.SpecialLevel.None));
					//reqList.Add(new ValidLevels(8, Types.Difficulty.Hard, 75075, Types.SpecialLevel.None));
					_newCalDayComponent.SetSpecificLevels(reqList);
					break;
					
				default:	// shouldn't be used
                    Debug.LogError("falling to default switch statement for calendar day");
					_newCalDayComponent.numAppointments = 		6;
					_newCalDayComponent.SetDifficulties			(20, 20, 20, 20);
					break;
			}

			_newCalDay.name = (_newCalDayComponent.dayIndex + 1).ToString() + " " + _newCalDayComponent.GetDayOfTheWeek().ToString();
			_newCalDayComponent.AddStatusOverlay();
		}

		// show notifications on specific weeks of the calendar view
		if (viewingWeek == 0)
		{
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(0, false);
		}
		else if (viewingWeek == 1)
		{
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(11, false);
		}
		else if (viewingWeek == 2)
		{
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(12, false);
		}
		else if (viewingWeek == 3)
		{
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(13, false);
		}
		else if (viewingWeek == 4)
		{
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(14, false);
		}

        // show notifications on specific days of the calendar view
		if (furthestDayUnlocked == Upgrade.dayToRequestRating1)
		{
			GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(20, false);
		}
		if (furthestDayUnlocked == Upgrade.dayToRequestRating2)
		{
			GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(23, false);
		}
		else if (furthestDayUnlocked == Upgrade.dayLocked && !Upgrade.canVerifyUpgrade())
		{
			GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(21, true);
		}

		// initialize save game
		List<int> numPossibleStars = new List<int>();
		List<int> numAppointmentsPerDay = new List<int>();
		int numTotalPossibleStars = 0;
		for (int i = 0; i < Get_daysToGenerate()/5; i++) { numPossibleStars.Add(0); }		// add a "0" to the list for each week
		for (int i = 0; i < Get_daysToGenerate(); i++)		// "i" represents the day
		{
			numAppointmentsPerDay.Add(Get_dayNumAppointments(i));
			int weekIndex = i/5;
			numPossibleStars[weekIndex] += Get_dayNumAppointments(i) * 3;		// assign the max number of stars to each week
			numTotalPossibleStars += Get_dayNumAppointments(i) * 3;		// keep a running count of all stars possible total
		}
		SaveGame.Initialize(numPossibleStars, numAppointmentsPerDay, numTotalPossibleStars, Get_daysToGenerate());
		SaveGame.UpdateGameStats();
	}
	
	// Update is called once per frame
	void Update ()
    {
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
		if (true)//Debug.isDebugBuild)
		{
			// make any day playable whether or not you've "unlocked" it
			if (!isDebugActive)
				GUI.color = new Color(0, 0, 0, 0);

			if (GUI.Button(new Rect((Screen.width/2) - 100, Screen.height - 80, 200, 80), "*debug mode*\nclick to open levels"))
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
	}

	void Debug_unlockLevelsOnWeek(int week, int numStars)
	{
		for (int i = 0; i < week * 5; i++)
		{
			SaveGame.SetHasCompletedAllRoundsInDay(i, true);
			SaveGame.SetDayStarCount(i, Get_dayNumAppointments(i) * numStars);
			for (int j = 0; j < Get_dayNumAppointments(i); j++)
			{
				SaveGame.SetRoundStarCount(i, j, numStars);
			}
		}
		SaveGame.UpdateGameStats();
	}

	void UnloadCalendarAndLoadLevel(string _levelName)
	{
		Destroy(GameObject.FindObjectOfType<LevelSelector>().gameObject);
		Application.LoadLevel(_levelName);
	}

	public void ReloadCalendar()
	{
		UnloadCalendarAndLoadLevel("Scene_Calendar");
	}
}
