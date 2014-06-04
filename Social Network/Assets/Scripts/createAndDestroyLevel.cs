using UnityEngine;
using System.Collections;
using Types;

public class createAndDestroyLevel : MonoBehaviour {

    #region Variables

	// score factors
	[HideInInspector]
	public Difficulty difficultySelection;
	[HideInInspector]
	public int numLevelsCompletedInARow = 0;
	public GUIStyle dayCompleteScreenStyle;
	private bool hasRunDayEnd = false;

	// clipboard
	public GameObject myClipboard;
	public clipboard myClipboardComponent;
	private bool isClipboardUp = true;

	// level parameters
	public int m_levelsAvailable = 8;
	public int m_levelsAvailableOffScreen;
	private bool hasLevelCountBeenSet = false;
	public int levelsAvailable
	{
		get
		{
			if (!hasLevelCountBeenSet) { m_levelsAvailableOffScreen = m_levelsAvailable; hasLevelCountBeenSet = true; }	// save the original value
			return m_levelsAvailableOffScreen;		// actually set this variable
		}
		set
		{
			if (!hasLevelCountBeenSet) { m_levelsAvailableOffScreen = m_levelsAvailable; hasLevelCountBeenSet = true; }
			m_levelsAvailableOffScreen = value;
		}
	}
	public int levelDuration = 180;

	// level parameter helpers
	public float timeLeft;
	public int levelsLeftToComplete;
	public bool dayComplete = false;
	private float waitToShowResultsAfterFinished = 1.25f;
	private bool gameplayHasStarted = false;
	private bool isShowingDebugControls = false;

    #endregion
	
	#region StartAndUpdate
	
	void Start () {
		myClipboardComponent = myClipboard.GetComponent<clipboard>();
		timeLeft = levelDuration;
		// add levels available and levels on clipboard (b/c levels available has already been subtracted from)
		levelsLeftToComplete = m_levelsAvailable;
	}

	void Update () {
		if (gameplayHasStarted && !dayComplete)
		{
			timeLeft -= Time.deltaTime;
			if (timeLeft <= 0)
			{
				if (isClipboardUp)
				{
					myClipboardComponent.ClearClipboard();
					DayEnd();
				}
				else if (timeLeft < 0) { timeLeft = 0; }		// if the timer gets below 0, set it to 0
			}
		}

		if (isClipboardUp && timeLeft <= 0)
		{
			if (timeLeft <= 0)
			{
				myClipboardComponent.ClearClipboard();
				DayEnd();
			}
		}
		if (dayComplete) { waitToShowResultsAfterFinished -= Time.deltaTime; }
	}

	#endregion

	void DayEnd()
	{
		if (!hasRunDayEnd)
		{
			dayComplete = true;
			bool receivedStar = false;
			int thisScore = GetComponent<scoreTrackerOneDay>().score;

			string thisDayString = "M1_D" + myClipboardComponent.selectorRef.dayToGenerate.dayIndex;
			string nextDayString = "M1_D" + (myClipboardComponent.selectorRef.dayToGenerate.dayIndex + 1);

			PlayerPrefs.SetInt(thisDayString + "_hasBeenCompleted", 1);
			if (thisScore > PlayerPrefs.GetInt(thisDayString + "_highestScore"))
			{ PlayerPrefs.SetInt(thisDayString + "_highestScore", thisScore); }

			if (thisScore >= myClipboardComponent.selectorRef.dayToGenerate.pointsRequiredForThreeStars)
			{
				PlayerPrefs.SetInt(thisDayString + "_starCount", 3);
				receivedStar = true;
			}
			else if (thisScore >= myClipboardComponent.selectorRef.dayToGenerate.pointsRequiredForTwoStars
			         && PlayerPrefs.GetInt(thisDayString + "_starCount", 0) < 2)
			{
				PlayerPrefs.SetInt(thisDayString + "_starCount", 2);
				receivedStar = true;
			}
			else if (thisScore >= myClipboardComponent.selectorRef.dayToGenerate.pointsRequiredForOneStar
			         && PlayerPrefs.GetInt(thisDayString + "_starCount", 0) < 1)
			{
				PlayerPrefs.SetInt(thisDayString + "_starCount", 1);
				receivedStar = true;
			}

			if (receivedStar) { PlayerPrefs.SetInt(nextDayString + "_isPlayable", 1); }

			PlayerPrefs.SetInt("totalPointsAccumulatedOverall", PlayerPrefs.GetInt("totalPointsAccumulatedOverall", 0) + thisScore);
			print ("total points accumulated overall: " + (PlayerPrefs.GetInt("totalPointsAccumulatedOverall")));

			PlayerPrefs.Save();

			hasRunDayEnd = true;
		}

	}

	public void ReturnToLevelSelection()
	{
		Destroy(myClipboardComponent.selectorRef.dayToGenerate.gameObject);
		Destroy(myClipboardComponent.selectorRef.gameObject);
		Destroy(myClipboardComponent.gameObject);
		Application.LoadLevel("Scene_LevelSelection");
		Destroy (gameObject);
	}

	// duplicate for default argument (see other "RoundEnd" function)
	public void RoundEnd(bool levelSuccess)
	{
		RoundEnd(levelSuccess, 0);
	}

	public void RoundEnd(bool levelSuccess, int numActionsTaken)
	{
		if (levelSuccess)
		{
			Appointment _thisLevel = myClipboardComponent.nextLevelUp;
			bool isSpecialLevel = false;
			if (_thisLevel.myLevel.isCantTouch || _thisLevel.myLevel.isFallToRed || _thisLevel.myLevel.isNoLines || _thisLevel.myLevel.isOneClick)
			{ isSpecialLevel = true; }
			GetComponent<scoreTrackerOneDay>().UpdateScore(myClipboardComponent.currentLevelDifficulty, numActionsTaken, numLevelsCompletedInARow, isSpecialLevel);
		}

		levelsLeftToComplete--;
		myClipboardComponent.Invoke("BringUpClipboard", (levelSuccess ? 1.0f : 0.0f));
		isClipboardUp = true;
		Invoke ("DestroyOldLevel", 1.0f);
		if (levelsLeftToComplete == 0) { DayEnd(); }
	}

	// void LoadNewLevel (string _LevelToLoad)
	void LoadNewLevel (validLevels _aSpecificLevel, bool isFromAppointment)
	{
		if (!isFromAppointment)
		{
			myClipboardComponent.nextLevelUp.myLevel = _aSpecificLevel;
		}
		Application.LoadLevelAdditive("Scene_" + _aSpecificLevel.level + "boxes");
		if (isClipboardUp)
		{
			myClipboardComponent.HideClipboard();
			isClipboardUp = false;
		}
	}

	// method overload
	void LoadNewLevel(validLevels _aSpecificLevel)
	{
		LoadNewLevel(_aSpecificLevel, true);
	}

	void DestroyOldLevel()
	{
		Destroy(GameObject.Find("LevelParent"));
	}

	public void GetStartFromClipboard()
	{
		validLevels _incomingLevel = myClipboardComponent.nextLevelUp.myLevel;
		difficultySelection = _incomingLevel.difficulty;
		LoadNewLevel(_incomingLevel);

		//class_NetworkMgr mgr = GameObject.Find("networkMgr").GetComponent<class_NetworkMgr>();

		gameplayHasStarted = true;
		numLevelsCompletedInARow++;
	}

	int[] GetDayRequirements()
	{
		CalendarDay _thisDay = GameObject.Find("LevelSelector").GetComponent<levelSelector>().dayToGenerate;
		int[] _starReq = { _thisDay.pointsRequiredForOneStar, _thisDay.pointsRequiredForTwoStars, _thisDay.pointsRequiredForThreeStars };
		return _starReq;
	}

	void MakeNewTestLevel(int _levelNumber)
	{
		int randSeed = Random.Range(0, 1000000);
		DestroyOldLevel();
		LoadNewLevel(GameObject.Find("LevelSelector").GetComponent<LevelFactory>().GetALevel(Difficulty.Unknown, _levelNumber, randSeed), false);
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(0, Screen.height - 40, 40, 20), "Gen"))
		{ isShowingDebugControls = true; }

		if (isShowingDebugControls)
			{
			// Buttons to load new level
			if (GUI.Button(new Rect(10, 10, 90, 25), "New 3")) { MakeNewTestLevel(3); }
			if (GUI.Button(new Rect(10, 40, 90, 25), "New 4")) { MakeNewTestLevel(4); }
			if (GUI.Button(new Rect(10, 70, 90, 25), "New 5")) { MakeNewTestLevel(5); }
			if (GUI.Button(new Rect(10, 100, 90, 25), "New 6")) { MakeNewTestLevel(6); }
			if (GUI.Button(new Rect(10, 130, 90, 25), "New 7")) { MakeNewTestLevel(7); }
			if (GUI.Button(new Rect(10, 160, 90, 25), "New 8")) { MakeNewTestLevel(8); }
		}

		if (dayComplete && waitToShowResultsAfterFinished <= 0)
		{
			string daySummary = "You earned " + GetComponent<scoreTrackerOneDay>().score.ToString() + " points";
			daySummary += "\n\n" + GetDayRequirements()[0] + " points = one star";
			daySummary += "\n" + GetDayRequirements()[1] + " points = two stars";
			daySummary += "\n" + GetDayRequirements()[2] + " points = three stars";
			daySummary += "\n\n" + "(at least one star required to progress)";

			int percentOfHoriz = 70;
			int percentOfVert = 65;
			int offsetFromCenterHoriz = 0;
			int offsetFromCenterVert = 50;
			GUI.Box (new Rect(
				(Screen.width/2 - (Screen.width/2.0f)*(percentOfHoriz/100.0f)) + offsetFromCenterHoriz,
				(Screen.height/2 - (Screen.height/2.0f)*(percentOfVert/100.0f)) + offsetFromCenterVert,
				(Screen.width/2.0f)*(percentOfHoriz/100.0f)*2,
				(Screen.height/2.0f)*(percentOfVert/100.0f)*2
				), daySummary, dayCompleteScreenStyle);
		}

		//GUI.Box (new Rect(100, Screen.height - 60, 60, 30), ((int)timeLeft).ToString());	// display countdown timer
	}
}
