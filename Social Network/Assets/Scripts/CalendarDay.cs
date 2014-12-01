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
	public int numStars;
	public DayOfTheWeek dayOfTheWeek;

	// difficulty settings to pass to day creator
	public int percentVeryEasy = 0;
	public int percentEasy = 0;
	public int percentMedium = 0;
	public int percentHard = 0;
	public int numAppointments;
	//public int timeLimit;
	public bool isPlayable = false;
	public bool hasBeenCompleted = false;
	public bool hasSpecificLevelRequests = false;
	public List<validLevels> specificLevelsRequested = new List<validLevels>();

    // special day attributes
    public int special_FallToRed = 0;          // people randomly fall back to red
	public int special_OneClick = 0;           // can only click each person once
	public int special_CantTouch = 0;          // a certain person you can't click on
	public int special_NoLines = 0;            // lines do not display
	public GameObject specialMask_FallToRed;
	public GameObject specialMask_OneClick;
	public GameObject specialMask_CantTouch;
	public GameObject specialMask_NoLines;

	// requirements for this level
	public int pointsRequiredForOneStar = 0;
	public int pointsRequiredForTwoStars = 0;
	public int pointsRequiredForThreeStars = 0;
	public int topScore = 0;

	// overlays
	public GameObject stars_1;
	public GameObject stars_2;
	public GameObject stars_3;
	public GameObject overlay_grey;

    // text labels
    public GameObject m_text;

	#region StartAndUpdate

	void Start () {
		// create object for the calendar day number
		GameObject _textNumber = Instantiate(m_text, transform.position, Quaternion.identity) as GameObject;
		_textNumber.transform.parent = transform;
		// create object for the week day name
		GameObject _textDay = Instantiate(m_text, transform.position, Quaternion.identity) as GameObject;
		_textDay.transform.parent = transform;
		// create object for the star count
		GameObject _textStars = Instantiate(m_text, transform.position, Quaternion.identity) as GameObject;
		_textStars.transform.parent = transform;
		
		_textNumber.transform.localScale = _textNumber.transform.localScale * 0.75f;
		_textNumber.transform.position = new Vector3(transform.position.x - 3.65f, transform.position.y + 0.65f, transform.position.z);
		_textNumber.GetComponent<TextMesh>().text = ((dayIndex + 1).ToString());
		
		_textDay.transform.localScale = _textDay.transform.localScale * 0.6f;
		_textDay.transform.position = new Vector3(transform.position.x, transform.position.y + 0.65f, transform.position.z);
		_textDay.GetComponent<TextMesh>().text = (dayOfTheWeek.ToString());

		_textStars.transform.localScale = _textStars.transform.localScale * 1.0f;
		_textStars.transform.position = new Vector3(transform.position.x + 2.0f, transform.position.y + -0.5f, transform.position.z);
		_textStars.GetComponent<TextMesh>().text = (numStars.ToString() + " / " + "#" + " stars");
		// TODO: figure out how to display maximum potential number of stars ("3/5 stars")
	}
	
	void Update () {
		
	}

	#endregion

	public void SetSpecialAttributes(int _fallToRed, int _oneClick, int _cantTouch, int _noLines)
	{
		special_FallToRed = _fallToRed;
		special_OneClick = _oneClick;
		special_CantTouch = _cantTouch;
		special_NoLines = _noLines;
	}

	public void AddStatusOverlay()
	{
		Vector3 overlayPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 0.1f);
		if (numStars == 1) { GameObject go = Instantiate(stars_1, overlayPos, Quaternion.identity) as GameObject; go.transform.parent = transform; }
		else if (numStars == 2) { GameObject go = Instantiate(stars_2, overlayPos, Quaternion.identity) as GameObject; go.transform.parent = transform; }
		else if (numStars == 3) { GameObject go = Instantiate(stars_3, overlayPos, Quaternion.identity) as GameObject; go.transform.parent = transform; }

		if (!isPlayable) { GameObject go = Instantiate (overlay_grey, overlayPos, Quaternion.identity) as GameObject; go.transform.parent = transform; }
	}

	public void SetDifficulties(int _VeryEasy, int _Easy, int _Medium, int _Hard)
	{
		percentVeryEasy = _VeryEasy; percentEasy = _Easy; percentMedium = _Medium; percentHard = _Hard;
	}

	public void SetSpecificLevel(validLevels levelRequested)
	{
		hasSpecificLevelRequests = true;
		specificLevelsRequested.Add(levelRequested);
	}
	public void SetSpecificLevels(List<validLevels> levelsRequested)
	{
		hasSpecificLevelRequests = true;
		specificLevelsRequested.AddRange(levelsRequested);
	}

	public void SetRequirementsForStars(int _oneStar, int _twoStars, int _threeStars)
	{
		pointsRequiredForOneStar = _oneStar; pointsRequiredForTwoStars = _twoStars; pointsRequiredForThreeStars = _threeStars;
	}
}
