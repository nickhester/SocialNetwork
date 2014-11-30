using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;
using System;

public class createAndDestroyLevel : MonoBehaviour {

    #region Variables

	// score factors
	[HideInInspector]
	public Difficulty difficultySelection;
	[HideInInspector]
	public int numLevelsCompletedInARow = 0;
	public GUIStyle dayCompleteScreenStyle;
	private bool hasDisplayedLevelEndScreen = false;
	public Material notesFor0Stars;
	public Material notesFor1Star;
	public Material notesFor2Stars;
	public Material notesFor3Stars;
	private GameObject resultsPage;
	private GameObject resultsNotes;
	private bool isDisplayingScore = false;

	// clipboard
	public GameObject myClipboard;
	public clipboard myClipboardComponent;
	private bool isClipboardUp = true;

	// level parameters
	public int m_levelsAvailable = 8;
	private bool hasLevelCountBeenSet = false;
	public int levelsAvailable
	{
		get
		{
			if (!hasLevelCountBeenSet) { hasLevelCountBeenSet = true; }	// save the original value
			return m_levelsAvailable;		// actually set this variable
		}
		set
		{
			if (!hasLevelCountBeenSet) { hasLevelCountBeenSet = true; }
			m_levelsAvailable = value;
		}
	}
	public int levelDuration = 180;

	// level parameter helpers
	public float timeLeft;
	public int levelsLeftToComplete;
	public bool levelComplete = false;
	private float waitToShowResultsAfterFinished = 2.0f;
	private float waitToShowResultsAfterFinishedCounter;
	private bool gameplayHasStarted = false;
	private bool isShowingDebugControls = false;
	private int currentLevelLoaded;

	System.Random rand = new System.Random();

    #endregion
	
	#region StartAndUpdate
	
	void Start () {
		myClipboardComponent = myClipboard.GetComponent<clipboard>();
		timeLeft = levelDuration;
		// add levels available and levels on clipboard (b/c levels available has already been subtracted from)
		levelsLeftToComplete = m_levelsAvailable;
		resultsPage = GameObject.Find("results page");
		resultsPage.renderer.enabled = false;
		resultsNotes = GameObject.Find("results notes");
		resultsNotes.renderer.enabled = false;
		waitToShowResultsAfterFinishedCounter = waitToShowResultsAfterFinished;
	}

	void Update () {

		if (levelComplete) { waitToShowResultsAfterFinishedCounter -= Time.deltaTime; }
	}

	#endregion

	public void ReturnToCalendar()
	{
		Destroy(myClipboardComponent.selectorRef.dayToGenerate.gameObject);
		Destroy(myClipboardComponent.selectorRef.gameObject);
		Destroy(myClipboardComponent.gameObject);
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
		// TODO: Get the level that was clicked, which was not necessarily the next level up
		Appointment _thisLevel = myClipboardComponent.nextLevelUp;
		bool isSpecialLevel = false;
		if (_thisLevel.myLevel.isCantTouch || _thisLevel.myLevel.isFallToRed || _thisLevel.myLevel.isNoLines || _thisLevel.myLevel.isOneClick)
		{ isSpecialLevel = true; }
		scoreTrackerOneDay st = GetComponent<scoreTrackerOneDay>();

		if (levelSuccess)
		{
			st.UpdateScore(myClipboardComponent.currentLevelDifficulty, numActionsTaken, isSpecialLevel);
		}

		st.UpdateMaxScore(myClipboardComponent.currentLevelDifficulty, isSpecialLevel, numActionsTaken);
		levelsLeftToComplete--;
		float waitTimeForClipboard = (levelSuccess ? 1.0f : 0.0f);
		myClipboardComponent.Invoke("BringUpClipboard", waitTimeForClipboard);
		isClipboardUp = true;
		Invoke ("DestroyOldLevel", waitTimeForClipboard);
		//if (levelsLeftToComplete == 0) { DayEnd(); }		// don't need this any more b/c the day never really "ends", but maybe we can use it for some kind of completion screen

		if (!levelSuccess) { return; }

		// show score instructions the first time you finish a round
		GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructions(3);

		if (!hasDisplayedLevelEndScreen)
		{
			levelComplete = true;
			myClipboardComponent.HideClipboardAppointments();
			bool receivedStar = false;
			int thisScore = st.score;

			// TODO: save by round, not by day
			string thisDayString = "M1_D" + myClipboardComponent.selectorRef.dayToGenerate.dayIndex;
			string nextDayString = "M1_D" + (myClipboardComponent.selectorRef.dayToGenerate.dayIndex + 1);
			
			SaveData.SetInt(thisDayString + "_hasBeenCompleted", 1);
			if (thisScore > SaveData.GetInt(thisDayString + "_highestScore"))
			{ SaveData.SetInt(thisDayString + "_highestScore", thisScore); }
			
			int[] levelRequirements = new int[3];
			levelRequirements = st.GetStarRequirements();

			if (thisScore >= levelRequirements[2])
			{
				SaveData.SetInt(thisDayString + "_starCount", 3);
				receivedStar = true;
			}
			else if (thisScore >= levelRequirements[1]
			         && SaveData.GetInt(thisDayString + "_starCount") < 2)
			{
				SaveData.SetInt(thisDayString + "_starCount", 2);
				receivedStar = true;
			}
			else if (thisScore >= levelRequirements[0]
			         && SaveData.GetInt(thisDayString + "_starCount") < 1)
			{
				SaveData.SetInt(thisDayString + "_starCount", 1);
				receivedStar = true;
			}
			
			if (receivedStar) { SaveData.SetInt(nextDayString + "_isPlayable", 1); }
			
			SaveData.SetInt("totalPointsAccumulatedOverall", SaveData.GetInt("totalPointsAccumulatedOverall") + thisScore);
			print ("total points accumulated overall: " + (SaveData.GetInt("totalPointsAccumulatedOverall")));
			
			SaveData.Save();
			
			hasDisplayedLevelEndScreen = true;
		}
	}
	
	void LoadNewLevel (validLevels _aSpecificLevel, bool isFromAppointment)
	{
		// if this isn't a level from the clipboard, it needs to be set as the clipboard's nextLevelUp b/c that's where the NetworkMgr looks for it
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
		currentLevelLoaded = _aSpecificLevel.level;
	}

	// method overload
	void LoadNewLevel(validLevels _aSpecificLevel)
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
		validLevels _incomingLevel = myClipboardComponent.nextLevelUp.myLevel;
		difficultySelection = _incomingLevel.difficulty;
		LoadNewLevel(_incomingLevel);

		//class_NetworkMgr mgr = GameObject.Find("networkMgr").GetComponent<class_NetworkMgr>();

		gameplayHasStarted = true;
		numLevelsCompletedInARow++;
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
		resultsPage.renderer.enabled = false;
		foreach (MeshRenderer mr in resultsPage.GetComponentsInChildren<MeshRenderer>())
		{
			mr.enabled = false;
		}
		isDisplayingScore = false;
		waitToShowResultsAfterFinishedCounter = waitToShowResultsAfterFinished;
		levelComplete = false;
	}

	void OnGUI()
	{
		/*
		if (GUI.Button(new Rect(0, Screen.height - 40, 40, 20), "Gen"))
		{ isShowingDebugControls = true; }

		if (isShowingDebugControls)
			{
			// Buttons to load new level
			if (GUI.Button(new Rect(10, 10, 90, 25), "New 3")) { MakeNewTestLevel(3); currentLevelLoaded = 3; }
			if (GUI.Button(new Rect(10, 40, 90, 25), "New 4")) { MakeNewTestLevel(4); currentLevelLoaded = 4; }
			if (GUI.Button(new Rect(10, 70, 90, 25), "New 5")) { MakeNewTestLevel(5); currentLevelLoaded = 5; }
			if (GUI.Button(new Rect(10, 100, 90, 25), "New 6")) { MakeNewTestLevel(6); currentLevelLoaded = 6; }
			if (GUI.Button(new Rect(10, 130, 90, 25), "New 7")) { MakeNewTestLevel(7); currentLevelLoaded = 7; }
			if (GUI.Button(new Rect(10, 160, 90, 25), "New 8")) { MakeNewTestLevel(8); currentLevelLoaded = 8; }

			if (GUI.Button(new Rect(Screen.width - 300, Screen.height - 50, 75, 25), "series"))
			{
				levelTester t = GetComponent<levelTester>();
				t.numLevelsLeftToRun = t.numLevelsToRun;
				t.levelToLoad = currentLevelLoaded;
			}
		}
		*/

		if (levelComplete)
		{
			resultsPage.renderer.enabled = true;
		}

		if (levelComplete && waitToShowResultsAfterFinishedCounter <= 0 && !isDisplayingScore)
		{
			isDisplayingScore = true;

			scoreTrackerOneDay st = GetComponent<scoreTrackerOneDay>();
			int resultPoints = st.score;
			int resultStars = 0;
			if (st.score < st.GetStarRequirements()[0])
			{
				resultStars = 0;
				resultsNotes.renderer.material = notesFor0Stars;
			}
			else if (st.score < st.GetStarRequirements()[1])
			{
				resultStars = 1;
				resultsNotes.renderer.material = notesFor1Star;
			}
			else if (st.score < st.GetStarRequirements()[2])
			{
				resultStars = 2;
				resultsNotes.renderer.material = notesFor2Stars;
			}
			else if (st.score >= st.GetStarRequirements()[2])
			{
				resultStars = 3;
				resultsNotes.renderer.material = notesFor3Stars;
			}
			resultsNotes.renderer.enabled = true;

			foreach (TextMesh scoreText in resultsPage.GetComponentsInChildren<TextMesh>())
			{
				scoreText.renderer.enabled = true;
				if (scoreText.gameObject.name == "points")
				{
					scoreText.text = resultPoints.ToString();
				}
				else if (scoreText.gameObject.name == "stars")
				{
					scoreText.text = resultStars.ToString();
				}
			}
		}
	}
}
