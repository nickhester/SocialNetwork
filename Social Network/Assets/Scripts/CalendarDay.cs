using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Types;

public class CalendarDay : MonoBehaviour {
	
	public int dayIndex_internal;
	public int dayIndex
	{
		get
		{
			return this.dayIndex_internal;
		}
		set
		{
			this.dayIndex_internal = value;
			if (value % 5 == 0) { dayOfTheWeek = DayOfTheWeek.Monday; }
			else if (value % 5 == 1) { dayOfTheWeek = DayOfTheWeek.Tuesday; }
			else if (value % 5 == 2) { dayOfTheWeek = DayOfTheWeek.Wednesday; }
			else if (value % 5 == 3) { dayOfTheWeek = DayOfTheWeek.Thursday; }
			else if (value % 5 == 4) { dayOfTheWeek = DayOfTheWeek.Friday; }
		}
	}
	private int numStars;
	private DayOfTheWeek dayOfTheWeek;
	private bool hasGottenAllStars = false;
	public bool hasPassedAllRounds = false;
	private GameManager gameManager;

	// difficulty settings to pass to day creator
	public int numAppointments;
	//public int timeLimit;
	public bool isPlayable = false;
	public List<ValidLevels> specificLevelsRequested = new List<ValidLevels>();

    // special day attributes
	public GameObject specialMask_FallToRed;
	public GameObject specialMask_OneClick;
	public GameObject specialMask_CantTouch;
	public GameObject specialMask_NoLines;

	// overlays
	public GameObject stampCheck;
	public GameObject stampStar;
	public GameObject overlay_grey;
	private Vector3 m_overlay_left = new Vector3(-3.55f, -0.5f, -0.1f);
	private Vector3 m_overlay_right = new Vector3(3.55f, -0.5f, -0.1f);

	// labels
	public Text weekdayNameText;
	public Text daySummaryText;
	public Text dayNumberText;

	#region StartAndUpdate

	public void Init ()
	{
		GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		LocalizedTextManager localizedTextManager = gameManager.GetComponent<LocalizedTextManager>();

		// create day name text
		weekdayNameText.text = localizedTextManager.GetLocalizedString(dayOfTheWeek.ToString()).ToUpper();
		
		dayNumberText.text = (dayIndex + 1).ToString() + ".";

		// show number of rounds completed in that day
		int numAppointmentsCompleted = 0;
		int currentNumStars = 0;
		for (int i = 0; i < numAppointments; i++)
		{
			int thisRoundNumStars = SaveGame.GetRoundStarCount(dayIndex_internal, i);
			if (thisRoundNumStars > 0)
			{
				numAppointmentsCompleted++;
				currentNumStars += thisRoundNumStars;
			}
			
			// generate save game data for blob
			gameManager.gameDataBlob.AddDayStarCount(dayIndex_internal, i, thisRoundNumStars);
		}
		string stringToDisplay = "";
		if (isPlayable && numAppointmentsCompleted > 0)
		{
			stringToDisplay = numAppointmentsCompleted + " " + localizedTextManager.GetLocalizedString("of") + " " + numAppointments.ToString() + " " + localizedTextManager.GetLocalizedString("sessions");
			stringToDisplay += " - ";
			stringToDisplay += currentNumStars.ToString() + " " + localizedTextManager.GetLocalizedString("of") + " " + (numAppointments * 3).ToString() + " " + localizedTextManager.GetLocalizedString("stars");
			if (numAppointmentsCompleted == numAppointments)
			{
				hasPassedAllRounds = true;
				SaveGame.SetHasCompletedAllRoundsInDay(dayIndex_internal, hasPassedAllRounds);
			}
			if (currentNumStars == (numAppointments * 3))
			{
				hasGottenAllStars = true;
			}
		}
		else
		{
			stringToDisplay = "";
		}
		daySummaryText.text = stringToDisplay;

		AddStatusOverlay();
	}
	
	#endregion

	void AddStatusOverlay()
	{
		Vector3 overlayPosCenter = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 0.1f);
		Vector3 overlayPosLeft = this.transform.position + m_overlay_left;
		Vector3 overlayPosRight = this.transform.position + m_overlay_right;

		if (hasPassedAllRounds) { GameObject go = Instantiate(stampCheck, overlayPosLeft, Quaternion.identity) as GameObject; go.transform.parent = transform; }
		if (hasGottenAllStars) { GameObject go = Instantiate(stampStar, overlayPosRight, Quaternion.identity) as GameObject; go.transform.parent = transform; }

		if (!isPlayable) { GameObject go = Instantiate (overlay_grey, overlayPosCenter, Quaternion.identity) as GameObject; go.transform.parent = transform; }
	}

	public void SetSpecificLevel(ValidLevels levelRequested)
	{
		specificLevelsRequested.Add(levelRequested);
	}
	public void SetSpecificLevels(List<ValidLevels> levelsRequested)
	{
		specificLevelsRequested.AddRange(levelsRequested);
	}

	public DayOfTheWeek GetDayOfTheWeek()
	{
		return dayOfTheWeek;
	}

	public void ShowLock()
	{
		GameObject lockObject = GameObject.Find("Lock");
		lockObject.GetComponent<Renderer>().enabled = true;
		lockObject.GetComponent<Collider>().enabled = true;
		lockObject.transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z - 0.5f);
		lockObject.transform.SetParent(transform);
	}
}
