using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class Clipboard : MonoBehaviour
{
    #region Variables

    private bool isHiding = false;
	private Vector3 offscreenPosition;
	private Vector3 originalPosition;
	private CreateAndDestroyAppointment createAndDestroyLevelRef;
	private bool creatingInitialApptList = true;
	private List<ValidLevels> listOfSpecificallyRequestedLevels = new List<ValidLevels>();

	// appointments
	[SerializeField] private GameObject appointmentObject;
	private float appointmentSpacing = 2.0f;
	private float appointmentTop = 3.2f;

    private bool isInMotion = false;

    private Appointment nextLevelUp;
	[HideInInspector] public Difficulty currentLevelDifficulty;
    [HideInInspector] public int currentLevelNumBlocks;
	private GameObject startButton;
	private Renderer startButton_renderer;

	[HideInInspector] public LevelSelector selectorRef;
	private int buttonState = 0;
	[SerializeField] private Material buttonTextDone;
	[SerializeField] private Material buttonTextBack;

	private float timeToSwap = 0.68f;
    [SerializeField] private float lerpSpeed;

	[SerializeField] private GameObject badgeCheck;
	[SerializeField] private GameObject badgeStar;
	private Vector3 badgeCheckOriginalPos;
	private Vector3 badgeStarOriginalPos;
	private float distanceToPushBadges = 2.0f;
    private Vector3 currentLerpTarget;

    [SerializeField] private GameObject showMeBanner;
	private float showMeBannerScreenPos_X = 0.084f;
	private float showMeBannerScreenPos_Y = 0.95f;
    private Vector3 showMeOutPosition;
    private Vector3 showMeInPosition;
    private float distanceToPushShowMeBanner = 3.0f;
	private int daysToShowBanner = 5;

	private GameManager gameManager;
	private GameObject restartFromResultsScreenButton;

	[SerializeField] private GameObject text;
    private bool isFirstCreation = true;

    struct LerpPackage
    {
        public GameObject go;
        public Vector3 start;
        public Vector3 end;

        public LerpPackage(GameObject _go, Vector3 _start, Vector3 _end)
        {
            go = _go;
            start = _start;
            end = _end;
        }
    }

    #endregion

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
                SetNextLevelUp(go.GetComponent<Appointment>());
                createAndDestroyLevelRef.GetStartFromClipboard();
                currentLevelDifficulty = GetNextLevelUp().myLevel.difficulty;			// store level difficulty for scoring
                currentLevelNumBlocks = GetNextLevelUp().myLevel.level;

                // bring up the ShowMe banner if first week
                // ShowMe stuff
                Appointment _currentAppointment = GetNextLevelUp();
				//if (_currentAppointment.GetMyDayIndex() < daysToShowBanner && !(_currentAppointment.GetMyDayIndex() == 0 && _currentAppointment.GetMyLevelIndex() == 0))
				if (!(_currentAppointment.GetMyDayIndex() == 0 && _currentAppointment.GetMyLevelIndex() == 0))
                {
                    StartCoroutine(SlideObject(new LerpPackage(showMeBanner, showMeInPosition, showMeOutPosition)));
                }

				// log event to game manager
				gameManager.Event_AppointmentStart();
            }
            else if (go.name == "BackButton")	// if clicking the "back" button, go back to the calendar
            {
				GoBack();
            }
			else if (go.name == "RestartFromResultsScreenButton")	// if clicking the restart button from the clipboard, immediately restart previous level
			{
				HideClipboard();
				Invoke("CloseResultsPage", timeToSwap);

				// the following includes some copy/paste from the previous appointment start section. I'm not proud of that.
				createAndDestroyLevelRef.GetStartFromClipboard();
				Appointment _currentAppointment = GetNextLevelUp();
				if (_currentAppointment.GetMyDayIndex() < daysToShowBanner)
				{
					StartCoroutine(SlideObject(new LerpPackage(showMeBanner, showMeInPosition, showMeOutPosition)));
				}
				gameManager.Event_AppointmentStart();
				MetricsLogger.Instance.LogCustomEvent("Appointment", "RestartUsed", gameManager.FormatDayAndLevel());
			}
        }
        else
        {
            if (go.name == "BackButton" && buttonState == 1)			// click the "back" button to give up on the level
            {
                createAndDestroyLevelRef.RoundEnd(false);
            }
            else if (go == showMeBanner)
            {
                // restart the level and set level as not winnable/trackable
                createAndDestroyLevelRef.RestartLevel(true);
                GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(100, true);
				MetricsLogger.Instance.LogCustomEvent("Appointment", "HelpMeUsed", gameManager.FormatDayAndLevel());
                // wait for callback
            }
        }
    }

    public void Callback_CompletedShowMe()
    {
        createAndDestroyLevelRef.RestartLevel(false);
    }

	#region StartAndUpdate

	void Start () {
		gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
		gameManager.Register_Clipboard(this);

		offscreenPosition = new Vector3(transform.position.x, transform.position.y - 13, transform.position.z);
		originalPosition = transform.position;

		createAndDestroyLevelRef = GameObject.FindGameObjectWithTag("persistentObject").GetComponent<CreateAndDestroyAppointment>();
		foreach (Transform GO in GetComponentsInChildren<Transform>())
		{
            if (GO.name == "BackButton")
			{
				// get reference to start button
				startButton = GO.gameObject;
				startButton_renderer = startButton.GetComponent<Renderer>();
			}
		}

		selectorRef = GameObject.Find("LevelSelector").GetComponent<LevelSelector>();
		createAndDestroyLevelRef.levelsAvailable = selectorRef.dayToGenerate.numAppointments;

		CreateAllAppointments();

		// place badges
		badgeCheckOriginalPos = badgeCheck.transform.localPosition;
        badgeStarOriginalPos = badgeStar.transform.localPosition;
		badgeCheck.transform.position = new Vector3(badgeCheck.transform.position.x, badgeCheck.transform.position.y + distanceToPushBadges, badgeCheck.transform.position.z);
		badgeStar.transform.position = new Vector3(badgeStar.transform.position.x, badgeStar.transform.position.y + distanceToPushBadges, badgeStar.transform.position.z);

        // place showMe banner
		Vector3 showMeScreenEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * showMeBannerScreenPos_X, Screen.height * showMeBannerScreenPos_Y));
        showMeOutPosition = new Vector3(
			showMeScreenEdge.x,
			showMeScreenEdge.y,
			showMeBanner.transform.position.z);
		showMeInPosition = new Vector3(showMeOutPosition.x - distanceToPushShowMeBanner, showMeOutPosition.y, showMeOutPosition.z);
        showMeBanner.transform.position = showMeInPosition;

        // show notifications at start of clipboard
        if (selectorRef.dayToGenerate.dayIndex_internal == 0)
        {
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(1, false);
        }
        else if (selectorRef.dayToGenerate.dayIndex_internal == 4)
        {
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(9, false);
        }
		
		GameObject dayLabelText = Instantiate(text, new Vector3(gameObject.transform.position.x + 2.75f, gameObject.transform.position.y - 6.5f, gameObject.transform.position.x - 1.5f), Quaternion.identity) as GameObject;
		dayLabelText.transform.localScale = gameObject.transform.localScale * 0.003f;
		dayLabelText.transform.parent = gameObject.transform;
		TextMesh myLabelTextComponent = dayLabelText.GetComponent<TextMesh>();
		myLabelTextComponent.text = "Day " + (selectorRef.dayToGenerate.dayIndex_internal + 1);

		restartFromResultsScreenButton = GameObject.Find("RestartFromResultsScreenButton");
		ShowRestartButton(false);

        isFirstCreation = false;
	}

	void Update ()
    {
		if (isHiding)			// if the clipboard is down
		{
            buttonState = 1;
		}
		else 					// if the clipboard is up
		{
			if (!createAndDestroyLevelRef.appointmentComplete)
            {
                buttonState = 0;
            }
			else
            {
                buttonState = 2;
            }

			// badge stuff
			if (!isInMotion && SaveGame.GetHasCompletedAllRoundsInDay(selectorRef.dayToGenerate.dayIndex_internal))
			{
                StartCoroutine(SlideObject(new LerpPackage(badgeCheck, badgeCheck.transform.localPosition, badgeCheckOriginalPos)));

				if ((selectorRef.dayToGenerate.numAppointments * 3) == SaveGame.GetDayStarCount(selectorRef.dayToGenerate.dayIndex_internal))
				{
                    StartCoroutine(SlideObject(new LerpPackage(badgeStar, badgeStar.transform.localPosition, badgeStarOriginalPos)));
				}
			}
		}

		// update button with correct text for state
		if (buttonState == 0)
        {
			startButton_renderer.material = buttonTextBack;
        }
		else if (buttonState == 1) 
        {
			startButton_renderer.material = buttonTextBack;
        }
		else if (buttonState == 2) 
        {
			startButton_renderer.material = buttonTextDone;
        }
		else { Debug.LogError("Clipboard button state is invalid"); }

		if (Input.GetButtonDown("Cancel"))
		{
			GoBack();
		}
	}

	#endregion

    void SwapClipboardPages()
    {
		CloseResultsPage();
        BringUpClipboard();
        OnReturnToClipboard();
    }

	void CloseResultsPage()
	{
		createAndDestroyLevelRef.HideResultsPage();
		ShowClipboardAppointments(true);
		ShowRestartButton(false);
	}

    void OnReturnToClipboard()
    {
		GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(4, false);

		if (SaveGame.GetHasCompletedAllRoundsInDay(0))
		{
			GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(19, false);
		}
    }

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

	public void ShowClipboardAppointments(bool show)
	{
		foreach (Appointment a in transform.GetComponentsInChildren<Appointment>())
		{
            a.GetComponent<Renderer>().enabled = show;	// show all appointments
            a.GetComponent<Collider>().enabled = show;
			foreach (MeshRenderer childRenderer in a.transform.GetComponentsInChildren<MeshRenderer>())
			{
                childRenderer.enabled = show;
			}
		}
	}

    public Appointment GetNextLevelUp()
    {
        return nextLevelUp;
    }

    public void SetNextLevelUp(Appointment _a)
    {
        nextLevelUp = _a;
    }

    void OnLevelWasLoaded(int level)
    {
        if (!isFirstCreation)
        {
            InputManager.Instance.OnClick += OnClick;
        }
    }

    public void HideClipboard()
    {
        currentLerpTarget = offscreenPosition;
        isHiding = true;
        StartCoroutine(SlideObject(new LerpPackage(gameObject, transform.position, currentLerpTarget)));
    }

    void BringUpClipboard()
    {
        // note: this is not called the first time the clipboard is seen b/c it starts already up
        currentLerpTarget = originalPosition;
        isHiding = false;
        StartCoroutine(SlideObject(new LerpPackage(gameObject, transform.position, currentLerpTarget)));
        StartCoroutine(SlideObject(new LerpPackage(showMeBanner, showMeOutPosition, showMeInPosition)));
    }

    IEnumerator SlideObject(LerpPackage _input)
    {
        bool _isMovingClipboard = false;
        if (_input.go == this.gameObject)   // special case for moving clipboard
        {
            _isMovingClipboard = true;
            
        }
        if (_isMovingClipboard)
        {
            isInMotion = true;
        }
        
        float progress = 0.0f;
        Vector3 startingPos = _input.start;
        Vector3 targetPos = _input.end;

        // early out if the object is already basically there
        if (Vector3.Distance(_input.go.transform.position, targetPos) < 0.1f)
        {
            _input.go.transform.localPosition = targetPos;
        }
        else
        {
            while (progress < 1.0f)
            {
                // lerp in local position
                _input.go.transform.localPosition = Vector3.Lerp(startingPos, targetPos, Lerp_FastInEaseOut(progress));
                progress += Time.deltaTime * lerpSpeed;
                yield return null;
            }
			// make sure it makes it all the way down
			_input.go.transform.localPosition = targetPos;
        }
        
        if (_isMovingClipboard)
        {
            isInMotion = false;
        }
    }

    float Lerp_FastInEaseOut(float _progress)
    {
        return Mathf.Sin(_progress * (Mathf.PI / 2.0f));
    }

    public bool GetIsClipboardUp()
    {
        return !isHiding;
    }

	public void ShowRestartButton(bool _show)
	{
		
		if (_show)
		{
			restartFromResultsScreenButton.SetActive(true);
		}
		else
		{
			restartFromResultsScreenButton.SetActive(false);
		}
	}

	void GoBack()
	{
		if (!isHiding)									// if the clipboard is visible
		{
			if (buttonState == 0)				// click the "back" button to return to calendar
			{
				createAndDestroyLevelRef.ReturnToCalendar();
			}

			else if (buttonState == 2)
			{
				HideClipboard();                              // if clicking the "done" button from the results screen, go back to clipboard
				Invoke("SwapClipboardPages", timeToSwap);
			}
		}
		else
		{
			if (buttonState == 1)			// click the "back" button to give up on the level
			{
				createAndDestroyLevelRef.RoundEnd(false);
			}
		}
	}
}
