using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class Clipboard : MonoBehaviour {
	
	private bool isHiding = false;
	private Vector3 offscreenPosition;
	private Vector3 originalPosition;
	private CreateAndDestroyAppointment createAndDestroyLevelRef;
	private bool creatingInitialApptList = true;
	private List<ValidLevels> listOfSpecificallyRequestedLevels = new List<ValidLevels>();

	// appointments
	[SerializeField] private GameObject appointmentObject;
	private float appointmentSpacing = 1.75f;
	private float appointmentTop = 3.5f;

    private bool isInMotion = false;
	private Vector3 lastStepPosition;

    private Appointment nextLevelUp;
	[HideInInspector] public Difficulty currentLevelDifficulty;
    [HideInInspector] public int currentLevelNumBlocks;
	private GameObject startButton;

	[HideInInspector] public LevelSelector selectorRef;
	private int buttonState = 0;
	[SerializeField] private Material buttonTextDone;
	[SerializeField] private Material buttonTextBack;

	private bool isSwappingResultsPageForAppointments = false;
	private float timeToSwap = 0.15f;
	private float timeToSwapCounter;

	[SerializeField] private GameObject badgeCheck;
	[SerializeField] private GameObject badgeStar;
	private Vector3 badgeCheckOriginalPos;
	private Vector3 badgeStarOriginalPos;
	private float distanceToPushBadges = 2.0f;
	private float lerpSpeed = 0.1f;
	private bool areBadgesInFinalPosition = false;
	private bool needsToCheckProgressAgain = false;

	[SerializeField] private GameObject text;
    private bool isFirstCreation = true;

    struct levelOption
    {
        public Difficulty diff;
        public int level;
        public levelOption(int l, Difficulty d)
        {
            diff = d;
            level = l;
        }
    }

	#region StartAndUpdate

    void Awake()
    {
        InputManager.Instance.OnClick += OnClick;
    }

    private void OnClick(GameObject go)
    {
        if (isInMotion) { return; }      // don't accept clicks if it's in motion

        if (!isHiding)									// if the clipboard is visible
        {
            if (go.tag == "appointment")			// if clicking an appointment
            {
                nextLevelUp = go.GetComponent<Appointment>();
                createAndDestroyLevelRef.GetStartFromClipboard();
                currentLevelDifficulty = nextLevelUp.myLevel.difficulty;			// store level difficulty for scoring
                currentLevelNumBlocks = nextLevelUp.myLevel.level;
            }
            else if (go.name == "BackButton")	// if clicking the "back" button, go back to the calendar
            {
                if (buttonState == 0)				// click the "back" button to return to calendar
                {
                    createAndDestroyLevelRef.ReturnToCalendar();
                }

                else if (buttonState == 2)
                {
                    isSwappingResultsPageForAppointments = true;	// if clicking the "done" button from the results screen, go back to clipboard
                }
            }
        }
        else
        {
            if (go.name == "BackButton" && buttonState == 1)			// click the "back" button to give up on the level
            {
                createAndDestroyLevelRef.RoundEnd(false);
            }
        }
    }

	void Start () {

		timeToSwapCounter = timeToSwap;
		offscreenPosition = new Vector3(transform.position.x, transform.position.y - 13, transform.position.z);
		originalPosition = transform.position;

		createAndDestroyLevelRef = GameObject.FindGameObjectWithTag("persistentObject").GetComponent<CreateAndDestroyAppointment>();
		foreach (Transform GO in GetComponentsInChildren<Transform>())
		{
            if (GO.name == "BackButton") { startButton = GO.gameObject; }		// get reference to start button
		}

		selectorRef = GameObject.Find("LevelSelector").GetComponent<LevelSelector>();
		createAndDestroyLevelRef.levelsAvailable = selectorRef.dayToGenerate.numAppointments;

		CreateAllAppointments();

		// place badges
		badgeCheckOriginalPos = badgeCheck.transform.position;
		badgeStarOriginalPos = badgeStar.transform.position;
		badgeCheck.transform.position = new Vector3(badgeCheck.transform.position.x, badgeCheck.transform.position.y + distanceToPushBadges, badgeCheck.transform.position.z);
		badgeStar.transform.position = new Vector3(badgeStar.transform.position.x, badgeStar.transform.position.y + distanceToPushBadges, badgeStar.transform.position.z);

		GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(1);

		GameObject dayLabelText = Instantiate(text, new Vector3(gameObject.transform.position.x + 2.75f, gameObject.transform.position.y - 6.5f, gameObject.transform.position.x - 1.5f), Quaternion.identity) as GameObject;
		dayLabelText.transform.localScale = gameObject.transform.localScale * 0.003f;
		dayLabelText.transform.parent = gameObject.transform;
		TextMesh myLabelTextComponent = dayLabelText.GetComponent<TextMesh>();
		myLabelTextComponent.text = "Day " + (selectorRef.dayToGenerate.dayIndex_internal + 1);

        isFirstCreation = false;
	}

	// Update is called once per frame
	void Update ()
    {
        if (!isInMotion && isSwappingResultsPageForAppointments)
        {
            if (timeToSwapCounter > 0.0f)
            {
                HideClipboard();
                timeToSwapCounter -= Time.deltaTime;
            }
            else
            {
                createAndDestroyLevelRef.HideResultsPage();
                BringUpClipboard();
                ShowClipboardAppointments();
                timeToSwapCounter = timeToSwap;
                isSwappingResultsPageForAppointments = false;
            }
        }

		if (isHiding)			// if the clipboard is down
		{
			Vector3 _currentPos = transform.position;
			transform.position = Vector3.Lerp(_currentPos, offscreenPosition, 0.1f);
			// end lerp early
			if (Vector3.Distance(transform.position, offscreenPosition) < 0.1f) { transform.position = offscreenPosition; }
			buttonState = 1;
		}
		else 					// if the clipboard is up
		{
			Vector3 _currentPos = transform.position;
			transform.position = Vector3.Lerp(_currentPos, originalPosition, 0.1f);
			// end lerp early
			if (Vector3.Distance(transform.position, originalPosition) < 0.1f) { transform.position = originalPosition; }
			if (!createAndDestroyLevelRef.appointmentComplete) { buttonState = 0; }
			else { buttonState = 2; }

			// badge stuff
			if (!areBadgesInFinalPosition && !isInMotion)
			{
				bool isCheck = false;
				bool isStar = false;
				if (SaveGame.GetHasCompletedAllRoundsInDay(selectorRef.dayToGenerate.dayIndex_internal))
				{
					badgeCheck.transform.position = Vector3.Lerp(badgeCheck.transform.position, badgeCheckOriginalPos, lerpSpeed);
					isCheck = true;

					if ((selectorRef.dayToGenerate.numAppointments * 3) == SaveGame.GetDayStarCount(selectorRef.dayToGenerate.dayIndex_internal))
					{
						badgeStar.transform.position = Vector3.Lerp(badgeStar.transform.position, badgeStarOriginalPos, lerpSpeed);
						isStar = true;
					}
				}
				if (!isCheck && !isStar)
					areBadgesInFinalPosition = true;
				else if (isCheck && Mathf.Abs(badgeCheck.transform.position.y - badgeCheckOriginalPos.y) < 0.025f)
				{
					badgeCheck.transform.position = badgeCheckOriginalPos;
					if (!isStar)
					{
						areBadgesInFinalPosition = true;
					}
					else if (isStar && Mathf.Abs(badgeStar.transform.position.y - badgeStarOriginalPos.y) < 0.025f)
					{
						badgeStar.transform.position = badgeStarOriginalPos;
						areBadgesInFinalPosition = true;
					}
				}
			}
			if (needsToCheckProgressAgain && !isInMotion && !isHiding)
			{
				areBadgesInFinalPosition = false;
				needsToCheckProgressAgain = false;
			}
		}

		// update button with correct text for state
		if (buttonState == 0) { startButton.GetComponent<Renderer>().material = buttonTextBack; }
		else if (buttonState == 1) { startButton.GetComponent<Renderer>().material = buttonTextBack; }
		else if (buttonState == 2) { startButton.GetComponent<Renderer>().material = buttonTextDone; }
		else { Debug.LogError("Clipboard button state is invalid"); }

		//Check to see if the clipboard is in motion or not
		if (lastStepPosition != null && lastStepPosition != transform.position)	// if the clipboard is in motion
		{
			if (!isInMotion)
			{
				isInMotion = true;
				foreach (Appointment _a in transform.GetComponentsInChildren<Appointment>())
				{ _a.isLerping = false; }
			}
		}
		else { if (isInMotion)
			{ isInMotion = false; }
		}
		lastStepPosition = transform.position;
	}

	#endregion

	void CreateAllAppointments()
	{
		if (creatingInitialApptList)
		{
			listOfSpecificallyRequestedLevels.AddRange(selectorRef.dayToGenerate.specificLevelsRequested);
		}
		creatingInitialApptList = false;

		int _numLevels = createAndDestroyLevelRef.levelsAvailable;
		for (int i = 0; i < _numLevels; i++)				// for each of the appointments on the clipboard
		{
			Vector3 _pos = new Vector3(transform.position.x, transform.position.y - (i*appointmentSpacing) + appointmentTop, transform.position.z - 0.1f);
			GameObject _appt = Instantiate(appointmentObject, _pos, Quaternion.identity) as GameObject;
            _appt.name = "appointment " + i;
			Appointment _apptComponent = _appt.GetComponent<Appointment>();				// get reference to appointment component
			_apptComponent.Initialize();
			_appt.transform.parent = transform;
			_apptComponent.levelIndex = i;

			GenerateALevel(ref _apptComponent,
			               listOfSpecificallyRequestedLevels[0].difficulty,
			               listOfSpecificallyRequestedLevels[0].level,
			               listOfSpecificallyRequestedLevels[0].isFallToRed,
			               listOfSpecificallyRequestedLevels[0].isOneClick,
			               listOfSpecificallyRequestedLevels[0].isCantTouch,
			               listOfSpecificallyRequestedLevels[0].isNoLines,
			               listOfSpecificallyRequestedLevels[0].seed);
			listOfSpecificallyRequestedLevels.RemoveAt(0);
		}
	}

	void GenerateALevel(ref Appointment _appt, Difficulty _diff, int _levelNum, bool _special_FallToRed, bool _special_OneClick, bool _special_CantTouch, bool _special_NoLines, int seed)
	{
		string _appointmentText = "";
		string _difficultyText = "";

		createAndDestroyLevelRef.levelsAvailable--;

		ValidLevels requestedLevel;
		if (seed == -1)
		{
			requestedLevel = GameObject.Find("LevelSelector").GetComponent<LevelFactory>().GetALevel(
										_diff, _levelNum, _special_FallToRed, _special_OneClick, _special_CantTouch, _special_NoLines);
		}
		else
		{
			requestedLevel = GameObject.Find("LevelSelector").GetComponent<LevelFactory>().GetALevel(
				_diff, _levelNum, _special_FallToRed, _special_OneClick, _special_CantTouch, _special_NoLines, seed, false);
		}

		_appt.myLevel = requestedLevel;

		_appt.SetMySpecialOverlays();

		// build text to display on appointment
		if (requestedLevel.difficulty == Difficulty.VeryEasy) { _difficultyText = "Trivial"; }
		else if (_diff == Difficulty.Easy) { _difficultyText = "Minor"; }
		else if (_diff == Difficulty.Medium) { _difficultyText = "Major"; }
		else if (_diff == Difficulty.Hard) { _difficultyText = "Critical"; }

        _appointmentText += (_appt.myLevel.level).ToString();
        _appointmentText += " Patients,";
		_appointmentText += " Issues: ";
		_appointmentText += _difficultyText;
		_appt.myDisplayText_prop = _appointmentText;
	}

	void GenerateALevel(Appointment _appt, Difficulty _diff, int _levelNum, bool _special_FallToRed, bool _special_OneClick, bool _special_CantTouch, bool _special_NoLines)
	{
		GenerateALevel(ref _appt, _diff, _levelNum, _special_FallToRed, _special_OneClick, _special_CantTouch, _special_NoLines, -1);
	}

	void BringUpClipboard()
	{
		isHiding = false;

		needsToCheckProgressAgain = true;
	}

	public void HideClipboardAppointments()
	{
		foreach (Appointment a in transform.GetComponentsInChildren<Appointment>())
		{
			a.GetComponent<Renderer>().enabled = false;	// hide all appointments
			a.GetComponent<Collider>().enabled = false;
			foreach (MeshRenderer childRenderer in a.transform.GetComponentsInChildren<MeshRenderer>())
			{
				childRenderer.enabled = false;
			}
		}
	}

	void ShowClipboardAppointments()
	{
		foreach (Appointment a in transform.GetComponentsInChildren<Appointment>())
		{
			a.GetComponent<Renderer>().enabled = true;	// hide all appointments
			a.GetComponent<Collider>().enabled = true;
			foreach (MeshRenderer childRenderer in a.transform.GetComponentsInChildren<MeshRenderer>())
			{
				childRenderer.enabled = true;
			}
		}
	}

	public void HideClipboard()
	{
		isHiding = true;
	}

    public Appointment GetNextLevelUp()
    {
        return nextLevelUp;
    }

    void OnLevelWasLoaded(int level)
    {
        if (!isFirstCreation)
        {
            InputManager.Instance.OnClick += OnClick;
        }
    }
}
