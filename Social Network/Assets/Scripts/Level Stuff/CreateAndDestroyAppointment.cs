//#define DEBUG_LEVEL_TESTER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;
using System;

public class CreateAndDestroyAppointment : MonoBehaviour {

    #region Variables

	// score factors
	[HideInInspector]
	public Difficulty difficultySelection;
	[HideInInspector]
	public GUIStyle dayCompleteScreenStyle;
	private bool hasDisplayedLevelEndScreen = false;
	private GameObject resultsPage;
	private bool isDisplayingScore = false;
	private int numActions;

	private GameManager gameManager;

	// appointment parameters
	public int levelsAvailable = 8;

	// level parameter helpers
	[HideInInspector]
	public bool appointmentComplete = false;
	private float waitToShowResultsAfterFinished = 0.0f;
	private float waitToShowResultsAfterFinishedCounter;

#if (DEBUG_LEVEL_TESTER)
	// debug stuff that is (should be) commented out
	private bool isShowingDebugControls = false;
#endif

	System.Random rand = new System.Random();

    #endregion
	
	#region StartAndUpdate
	
	void Start ()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		// add levels available and levels on clipboard (b/c levels available has already been subtracted from)
		resultsPage = GameObject.Find("results page");
		resultsPage.GetComponent<Renderer>().enabled = false;
		waitToShowResultsAfterFinishedCounter = waitToShowResultsAfterFinished;
	}

	void Update ()
	{
		if (appointmentComplete)
		{
			waitToShowResultsAfterFinishedCounter -= Time.deltaTime;
		}
	}

	#endregion

	public void ReturnToCalendar()
	{
		Destroy(gameManager.GetClipboard().selectorRef.dayToGenerate.gameObject);
		Destroy(gameManager.GetClipboard().selectorRef.gameObject);
		Destroy(gameManager.GetClipboard().gameObject);
		Application.LoadLevel("Scene_Calendar");
		Destroy (gameObject);
	}

	// duplicate for default argument (see other "RoundEnd" function)
	public void RoundEnd(bool levelSuccess)
	{
		RoundEnd(levelSuccess, 0);
	}

	public void RoundEnd(bool levelSuccess, int numActionsTaken)
	{
		Appointment _thisLevel = gameManager.GetClipboard().GetNextLevelUp();
		bool isSpecialLevel = false;
		if (_thisLevel.myLevel.isCantTouch || _thisLevel.myLevel.isFallToRed || _thisLevel.myLevel.isNoLines || _thisLevel.myLevel.isOneClick)
		{
			isSpecialLevel = true;
		}

		ScoreTrackerOneRound st = GetComponent<ScoreTrackerOneRound>();
		st.Reset();

		if (levelSuccess)
		{
			st.UpdateScore(gameManager.GetClipboard().currentLevelDifficulty, numActionsTaken, isSpecialLevel);
			numActions = numActionsTaken;

			// log event to game manager
			gameManager.Event_AppointmentEnd();
			// log metrics
			MetricsLogger.Instance.LogCustomEvent("Appointment", "NumActions", gameManager.FormatDayAndLevel(), numActionsTaken);
			MetricsLogger.Instance.LogCustomEvent("Appointment", "NumStars", gameManager.FormatDayAndLevel(), st.GetScore());
		}

		float waitTimeForClipboard = (levelSuccess ? 1.0f : 0.0f);
		gameManager.GetClipboard().Invoke("BringUpClipboard", waitTimeForClipboard);
		Invoke ("DestroyOldLevel", waitTimeForClipboard);

		// display level end screen, etc.
		if (!hasDisplayedLevelEndScreen && levelSuccess)
		{
			// show score notifications on level end
            if (_thisLevel.GetMyDayIndex() == 0)
            {
                GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(3, false);
            }
            
			appointmentComplete = true;
			gameManager.GetClipboard().ShowClipboardAppointments(false);

			// show "restart" button if earned less than 3 stars
			if (st.GetScore() < 3)
			{
				gameManager.GetClipboard().ShowRestartButton(true);
			}
			else
			{
				// trigger star particle for 3 stars
				gameManager.GetClipboard().Invoke("TriggerStarParticles", waitTimeForClipboard + 0.5f);
			}

			int currentDayIndex = gameManager.GetClipboard().selectorRef.dayToGenerate.dayIndex;

			if (st.GetScore() > SaveGame.GetRoundStarCount(currentDayIndex, _thisLevel.levelIndex))
			{
				SaveGame.SetRoundStarCount(currentDayIndex, _thisLevel.levelIndex, st.GetScore());
			}

			// Check to see if all rounds in day received a star, and also tally stars for the day
			bool doAllRoundsInDayHaveStars = true;
			int howManyStarsTotalDay = 0;

			for (int i = 0; i < gameManager.GetClipboard().selectorRef.dayToGenerate.numAppointments; i++)
			{
				int thisDayStarCount = SaveGame.GetRoundStarCount(currentDayIndex, i);
				howManyStarsTotalDay += thisDayStarCount;
				if (thisDayStarCount < 1)
				{
					doAllRoundsInDayHaveStars = false;
				}
			}

			// update day's star count
			SaveGame.SetDayStarCount(currentDayIndex, howManyStarsTotalDay);
			// if true, unlock next day
			if (doAllRoundsInDayHaveStars)
			{
				SaveGame.SetHasCompletedAllRoundsInDay(currentDayIndex, true);
			}
			SaveGame.UpdateGameStats();

			gameManager.UpdateCloudSaveFromLocal();
			
			hasDisplayedLevelEndScreen = true;
		}
		if (levelSuccess)
		{
			_thisLevel.UpdateStarCount();
		}
	}
	
	void LoadNewLevel (ValidLevels _aSpecificLevel, bool isFromAppointment)
	{
		// if this isn't a level from the clipboard, it needs to be set as the clipboard's nextLevelUp b/c that's where the NetworkMgr looks for it
		if (!isFromAppointment)
		{
			gameManager.GetClipboard().GetNextLevelUp().myLevel = _aSpecificLevel;
		}
        Application.LoadLevelAdditive("Scene_Appointment");
	}

	public void OnAppointmentStarted()
	{
		if (gameManager.GetClipboard().GetIsClipboardUp())
		{
			gameManager.GetClipboard().HideClipboard();
		}
	}

	// method overload
	void LoadNewLevel(ValidLevels _aSpecificLevel)
	{
		LoadNewLevel(_aSpecificLevel, true);
	}

	void DestroyOldLevel()
	{
		foreach (GameObject aLevelParent in GameObject.FindGameObjectsWithTag("levelParent"))
		{
			Destroy(aLevelParent);
		}
	}

	public void GetStartFromClipboard()
	{
		Appointment _nextAppointment = gameManager.GetClipboard().GetNextLevelUp();

        ValidLevels _incomingLevel = _nextAppointment.myLevel;
		difficultySelection = _incomingLevel.difficulty;
		LoadNewLevel(_incomingLevel);

		// showing notifications at beginning of sessions
        if (_nextAppointment.GetMyDayIndex() == 0 && _nextAppointment.levelIndex == 0)
        {
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(2, false);
        }
        else if (_nextAppointment.GetMyDayIndex() == 0 && _nextAppointment.levelIndex == 1)
        {
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(6, false);
        }
        else if (_nextAppointment.GetMyDayIndex() == 0 && _nextAppointment.levelIndex == 2)
        {
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(18, false);
        }
		else if (_nextAppointment.GetMyDayIndex() == 1 && _nextAppointment.levelIndex == 0)
        {
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(5, false);
        }
        else if (_nextAppointment.GetMyDayIndex() == 2 && _nextAppointment.levelIndex == 0)
        {
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(7, false);
        }
		else if (_nextAppointment.GetMyDayIndex() == 3 && _nextAppointment.levelIndex == 0)
		{
			GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(8, false);
		}
		else if (_nextAppointment.GetMyDayIndex() == 4 && _nextAppointment.levelIndex == 2)
		{
			GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(10, false);
		}
	}

	public void MakeNewTestLevel(int _levelNumber)
	{
		int randSeed = rand.Next(0, 1000000);
		print (randSeed);
		DestroyOldLevel();
		LoadNewLevel(GameObject.Find("LevelSelector").GetComponent<LevelFactory>().GetALevel(Difficulty.Unknown, _levelNumber, randSeed, true), false);
	}

	public void HideResultsPage()
	{
		resultsPage.GetComponent<Renderer>().enabled = false;
		foreach (MeshRenderer mr in resultsPage.GetComponentsInChildren<MeshRenderer>())
		{
			mr.enabled = false;
		}
		isDisplayingScore = false;
		waitToShowResultsAfterFinishedCounter = waitToShowResultsAfterFinished;
		appointmentComplete = false;
		hasDisplayedLevelEndScreen = false;
	}

    public void RestartLevel(bool _isDemonstration)
    {
        NetworkManager nm = GameObject.FindGameObjectWithTag("networkManager").GetComponent<NetworkManager>();
        nm.ReloadStartingState();
        nm.SetAsDemonstration(_isDemonstration);
    }

	void OnGUI()
	{

#if (DEBUG_LEVEL_TESTER)

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

			if (GUI.Button(new Rect(Screen.width - 300, Screen.height - 50, 75, 25), "run test"))
			{
				GetComponent<LevelTester>().RunLevelTests();
			}
		}
		
#endif

		if (appointmentComplete)
		{
			resultsPage.GetComponent<Renderer>().enabled = true;
		}

		if (appointmentComplete && waitToShowResultsAfterFinishedCounter <= 0 && !isDisplayingScore)
		{
			isDisplayingScore = true;

			ScoreTrackerOneRound st = GetComponent<ScoreTrackerOneRound>();
			int resultActions = numActions;
			int resultStars = st.GetScore();

			foreach (TextMesh resultText in resultsPage.GetComponentsInChildren<TextMesh>())
			{
				resultText.GetComponent<Renderer>().enabled = true;
				if (resultText.gameObject.name == "actions")
				{
					resultText.text = resultActions.ToString();
				}
				else if (resultText.gameObject.name == "stars")
				{
					resultText.text = resultStars.ToString();
				}
			}
		}
	}
}
