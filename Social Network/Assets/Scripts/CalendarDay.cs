using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
			if (value%5 == 0) { dayOfTheWeek = DayOfTheWeek.Monday; }
			else if (value%5 == 1) { dayOfTheWeek = DayOfTheWeek.Tuesday; }
			else if (value%5 == 2) { dayOfTheWeek = DayOfTheWeek.Wednesday; }
			else if (value%5 == 3) { dayOfTheWeek = DayOfTheWeek.Thursday; }
			else if (value%5 == 4) { dayOfTheWeek = DayOfTheWeek.Friday; }
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

    // text labels
    public GameObject m_text1;
	public GameObject m_text2;

	#region StartAndUpdate

	public void Init ()
	{
		GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		// create object for the calendar day number
		GameObject _textNumber = Instantiate(m_text1, transform.position, Quaternion.identity) as GameObject;
		_textNumber.transform.localScale *= 0.065f;
		_textNumber.transform.position = new Vector3(transform.position.x, transform.position.y + 0.65f, transform.position.z - 0.1f);
		_textNumber.transform.parent = transform;
		// create object for the week day name
		GameObject _textDay = Instantiate(m_text1, transform.position, Quaternion.identity) as GameObject;
		_textDay.transform.parent = transform;
		_textDay.transform.localScale *= 0.065f;
		_textDay.transform.position = new Vector3(transform.position.x, transform.position.y + 0.65f, transform.position.z - 0.1f);
		// create object for the star count
		GameObject _textStars = Instantiate(m_text2, transform.position, Quaternion.identity) as GameObject;
		_textStars.transform.parent = transform;
		_textStars.transform.localScale *= 0.06f;

		// create day number text
		_textNumber.transform.localScale = _textNumber.transform.localScale * 0.75f;
		_textNumber.transform.position = new Vector3(transform.position.x - 3.65f, transform.position.y + 0.65f, transform.position.z - 0.1f);
		_textNumber.GetComponent<TextMesh>().text = ((dayIndex + 1).ToString());

		// create day name text
		_textDay.transform.localScale = _textDay.transform.localScale * 0.6f;
		_textDay.transform.position = new Vector3(transform.position.x, transform.position.y + 0.65f, transform.position.z - 0.1f);
		_textDay.GetComponent<TextMesh>().text = (dayOfTheWeek.ToString());

		// create star count text
		_textStars.transform.localScale = _textStars.transform.localScale * 0.7f;
		_textStars.transform.position = new Vector3(transform.position.x + 0.0f, transform.position.y + -0.4f, transform.position.z - 0.2f);

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
			stringToDisplay = numAppointmentsCompleted + " of " + numAppointments.ToString() + " sessions";
			stringToDisplay += " - ";
			stringToDisplay += currentNumStars.ToString() + " of " + (numAppointments * 3).ToString() + " stars";
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
		_textStars.GetComponent<TextMesh>().text = stringToDisplay;

		AddStatusOverlay();
	}
	
	#endregion

	void AddStatusOverlay()
	{
		Vector3 overlayPosCenter = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 0.1f);
		Vector3 overlayPosLeft = new Vector3(this.transform.position.x - 3.2f, this.transform.position.y - 0.25f, this.transform.position.z - 0.1f);
		Vector3 overlayPosRight = new Vector3(this.transform.position.x + 3.2f, this.transform.position.y - 0.25f, this.transform.position.z - 0.1f);

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
