using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;
using System.Linq;

public class clipboard : MonoBehaviour {
	
	private bool isHiding = false;
	private Vector3 offscreenPosition;
	private Vector3 originalPosition;
	private List<Difficulty> listDifficulty = new List<Difficulty>();
	private List<int> listLevel = new List<int>();
	private createAndDestroyLevel createAndDestroyLevelRef;
	private bool creatingInitialApptList = true;
	private List<SpecialLevelAttributes> listOfSpecialAttributes = new List<SpecialLevelAttributes>();
	private List<levelOption> listOfLevelOptions = new List<levelOption>();
	private List<validLevels> listOfSpecificallyRequestedLevels = new List<validLevels>();
	private int specificLevelsToCreate = 0;

	// appointments
	public GameObject appointmentObject;
	[HideInInspector]
	public int maxAppointmentsOnClipboard = 6;
	private float appointmentSpacing = 1.75f;
	private float appointmentTop = 3.5f;

	public bool isInMotion = false;
	private Vector3 lastStepPosition;
	
	public Appointment nextLevelUp;
	[HideInInspector]
	public Difficulty currentLevelDifficulty;
	public int currentLevelNumBlocks;
	private GameObject startButton;
	
	private List<Difficulty> listOfLevelDifficulties = new List<Difficulty>();
	private List<int> listofLevelNumber = new List<int>();

	public levelSelector selectorRef;
	private int buttonState = 0;
	public Material buttonTextStart;
	public Material buttonTextSkip;
	public Material buttonTextDone;
	public Material buttonTextBack;

    private float watchHandZOffset;
    private GameObject watchHand;

	private bool isSwappingResultsPageForAppointments = false;
	private float timeToSwap = 2.0f;
	private float timeToSwapCounter;

	#region StartAndUpdate

	void Start () {

		timeToSwapCounter = timeToSwap;
		offscreenPosition = new Vector3(transform.position.x, transform.position.y - 13, transform.position.z);
		originalPosition = transform.position;

		listDifficulty.Add(Difficulty.VeryEasy); listDifficulty.Add(Difficulty.Easy); listDifficulty.Add(Difficulty.Medium); listDifficulty.Add(Difficulty.Hard);
		listLevel.Add(3); listLevel.Add(4); listLevel.Add(5); listLevel.Add(6); listLevel.Add(7); listLevel.Add(8);
		createAndDestroyLevelRef = GameObject.FindGameObjectWithTag("persistentObject").GetComponent<createAndDestroyLevel>();
		foreach (Transform GO in GetComponentsInChildren<Transform>())
		{
			if (GO.name == "StartButton") { startButton = GO.gameObject; }		// get reference to start button
			if (GO.name == "watch hand") {watchHand = GO.gameObject; }			// get reference to watch hand
		}

		selectorRef = GameObject.Find("LevelSelector").GetComponent<levelSelector>();
		createAndDestroyLevelRef.m_levelsAvailable = selectorRef.dayToGenerate.numAppointments;

		CreateAllAppointments();
	}

	// Update is called once per frame
	void Update () {
		if (!isInMotion)
		{
			if (Input.GetMouseButtonDown(0))		// when you click
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 10.0f))
				{
					if (!isHiding)									// if the clipboard is visible
					{
						if (hit.transform.tag == "appointment")			// if clicking an appointment
						{
							nextLevelUp = hit.transform.gameObject.GetComponent<Appointment>();
							createAndDestroyLevelRef.GetStartFromClipboard();
							currentLevelDifficulty = nextLevelUp.myLevel.difficulty;			// store level difficulty for scoring
							currentLevelNumBlocks = nextLevelUp.myLevel.level;
						}
						else if (hit.transform.name == "StartButton")	// if clicking the "back" button, go back to the calendar
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
						if (hit.transform.name == "StartButton" && buttonState == 1)			// click the "back" button to give up on the level
						{
							createAndDestroyLevelRef.RoundEnd(false);
							createAndDestroyLevelRef.numLevelsCompletedInARow = 0;
						}
					}
				}
			}

			if (isSwappingResultsPageForAppointments)
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
			if (!createAndDestroyLevelRef.levelComplete) { buttonState = 0; }
			else { buttonState = 2; }
		}

		// update button with correct text for state
		if (buttonState == 0) { startButton.renderer.material = buttonTextBack; }
		else if (buttonState == 1) { startButton.renderer.material = buttonTextBack; }
		else if (buttonState == 2) { startButton.renderer.material = buttonTextDone; }
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
		int _maxAppts = maxAppointmentsOnClipboard;
		
		if (creatingInitialApptList)
		{
			if (selectorRef.dayToGenerate.hasSpecificLevelRequests)
			{
				listOfSpecificallyRequestedLevels.AddRange(selectorRef.dayToGenerate.specificLevelsRequested);
				specificLevelsToCreate = selectorRef.dayToGenerate.specificLevelsRequested.Count;
			}

			GenerateListOfDifficultiesAndLevels();
			
			if (createAndDestroyLevelRef.levelsAvailable < _maxAppts)	// if all the levels can fit on the clipboard...
			{
				_maxAppts = createAndDestroyLevelRef.levelsAvailable;		// ...then only have that many on the clipboard
			}
		}
		creatingInitialApptList = false;
		
		for (int i = 0; i < _maxAppts; i++)				// for each of the appointments on the clipboard
		{
			if (i > (transform.GetComponentsInChildren<Appointment>().Length - 1))		// if there's room for more appointments, then add a new one
			{
				Vector3 _pos = new Vector3(transform.position.x, transform.position.y - (i*appointmentSpacing) + appointmentTop, transform.position.z - 0.1f);
				GameObject _appt = Instantiate(appointmentObject, _pos, Quaternion.identity) as GameObject;
				Appointment _apptComponent = _appt.GetComponent<Appointment>();				// get reference to appointment component
				_apptComponent.Initialize();
				_appt.transform.parent = transform;

				int numIndexToGenerate = Random.Range(0, listOfLevelDifficulties.Count);	// this randomizes the order
				int numIndexToGenerate2 = Random.Range(0, listOfSpecialAttributes.Count);	// this randomizes the order

				bool s1 = false; bool s2 = false; bool s3 = false; bool s4 = false;
				if (listOfSpecialAttributes[numIndexToGenerate2] == SpecialLevelAttributes.FallToRed ) { s1 = true; }
				else if (listOfSpecialAttributes[numIndexToGenerate2] == SpecialLevelAttributes.OneClick) { s2 = true; }
				else if (listOfSpecialAttributes[numIndexToGenerate2] == SpecialLevelAttributes.CantTouch) { s3 = true; }
				else if (listOfSpecialAttributes[numIndexToGenerate2] == SpecialLevelAttributes.NoLines) { s4 = true; }

				if (specificLevelsToCreate > 0)
				{
					GenerateALevel(ref _apptComponent,
					               listOfSpecificallyRequestedLevels[0].difficulty,
					               listOfSpecificallyRequestedLevels[0].level,
					               listOfSpecificallyRequestedLevels[0].isFallToRed,
					               listOfSpecificallyRequestedLevels[0].isOneClick,
					               listOfSpecificallyRequestedLevels[0].isCantTouch,
					               listOfSpecificallyRequestedLevels[0].isNoLines,
					               listOfSpecificallyRequestedLevels[0].seed);
					listOfSpecificallyRequestedLevels.RemoveAt(0);
					specificLevelsToCreate--;
				}
				else
				{
					GenerateALevel(_apptComponent,
					               listOfLevelDifficulties[numIndexToGenerate],
					               listofLevelNumber[numIndexToGenerate],
					               s1, s2, s3, s4);					// pick a level for the appt
				}

				listOfLevelDifficulties.RemoveAt(numIndexToGenerate);	// remove used level difficulty
				listofLevelNumber.RemoveAt(numIndexToGenerate);			// remove used level number
				listOfSpecialAttributes.RemoveAt(numIndexToGenerate2);	// remove used special item
			}
		}
	}

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

	void GenerateListOfDifficultiesAndLevels()
	{
		for (float i = 0; i < createAndDestroyLevelRef.levelsAvailable; i++)
		{
			if ((i / createAndDestroyLevelRef.levelsAvailable) * 100.0f <= selectorRef.dayToGenerate.percentVeryEasy-1 
			    && selectorRef.dayToGenerate.percentVeryEasy != 0)
			{
				listOfLevelDifficulties.Add(Difficulty.VeryEasy);
				listofLevelNumber.Add(Random.Range(0, 2) + 3);
				// levels 3 or 4
			}
			else if ((i / createAndDestroyLevelRef.levelsAvailable) * 100.0f <= selectorRef.dayToGenerate.percentVeryEasy-1 
			         + selectorRef.dayToGenerate.percentEasy 
			         && selectorRef.dayToGenerate.percentEasy != 0)
			{
				listOfLevelDifficulties.Add(Difficulty.Easy);
				listofLevelNumber.Add(Random.Range(1, 3) + 3);
				// levels 4, 5
			}
			else if ((i / createAndDestroyLevelRef.levelsAvailable) * 100.0f <= selectorRef.dayToGenerate.percentVeryEasy-1 
			         + selectorRef.dayToGenerate.percentEasy 
			         + selectorRef.dayToGenerate.percentMedium 
			         && selectorRef.dayToGenerate.percentMedium != 0)
			{
				listOfLevelDifficulties.Add(Difficulty.Medium);
				listofLevelNumber.Add(Random.Range(2, listLevel.Count) + 3);
				// levels 5 and up
			}
			else
			{
				listOfLevelDifficulties.Add(Difficulty.Hard);
				listofLevelNumber.Add(Random.Range(3, listLevel.Count) + 3);
				// levels 6 and up
			}

			if (selectorRef.dayToGenerate.special_CantTouch > 0)
			{
				listOfSpecialAttributes.Add(SpecialLevelAttributes.CantTouch);
				selectorRef.dayToGenerate.special_CantTouch--;
			}
			else if (selectorRef.dayToGenerate.special_FallToRed > 0)
			{
				listOfSpecialAttributes.Add(SpecialLevelAttributes.FallToRed);
				selectorRef.dayToGenerate.special_FallToRed--;
			}
			else if (selectorRef.dayToGenerate.special_NoLines > 0)
			{
				listOfSpecialAttributes.Add(SpecialLevelAttributes.NoLines);
				selectorRef.dayToGenerate.special_NoLines--;
			}
			else if (selectorRef.dayToGenerate.special_OneClick > 0)
			{
				listOfSpecialAttributes.Add(SpecialLevelAttributes.OneClick);
				selectorRef.dayToGenerate.special_OneClick--;
			}
			else
			{
				listOfSpecialAttributes.Add(SpecialLevelAttributes.None);
			}
		}
	}

	void GenerateALevel(ref Appointment _appt, Difficulty _diff, int _levelNum, bool _special_FallToRed, bool _special_OneClick, bool _special_CantTouch, bool _special_NoLines, int seed)
	{
		string _appointmentText = "";
		string _difficultyText = "";

		createAndDestroyLevelRef.levelsAvailable--;

		validLevels requestedLevel;
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
				
        _appointmentText += "Patients: ";
        _appointmentText += (_appt.myLevel.level).ToString();
		_appointmentText += ", Issues: ";
		_appointmentText += _difficultyText;
		_appt.myDisplayText_prop = _appointmentText;
	}

	void GenerateALevel(Appointment _appt, Difficulty _diff, int _levelNum, bool _special_FallToRed, bool _special_OneClick, bool _special_CantTouch, bool _special_NoLines)
	{
		GenerateALevel(ref _appt, _diff, _levelNum, _special_FallToRed, _special_OneClick, _special_CantTouch, _special_NoLines, -1);
	}

	/*
	Appointment FindTopAppointment()
	{
		Appointment _highestAppointment = null;
		float _highestYValue = -999.0f;
		foreach (Appointment a in transform.GetComponentsInChildren<Appointment>())
		{
			if (a.transform.position.y > _highestYValue)
			{
				_highestYValue = a.transform.position.y;
				_highestAppointment = a;
			}
		}
		return _highestAppointment;
	}
	*/

	/*
	void PopOffFinishedLevel()
	{
		try { Destroy(FindTopAppointment().gameObject); }
		catch { }
	}
	*/

	public void BringUpClipboard()
	{
		isHiding = false;

		/*
		PopOffFinishedLevel();								// remove the level that was just played

		if (!createAndDestroyLevelRef.dayComplete)				// if the day is still going...
		{
			Invoke("RearrangeAllAppointments", 0.75f);			// arrange appts
			if (createAndDestroyLevelRef.levelsAvailable > 0)	// and if there are more to be added to the clipboard...
			{
				Invoke("CreateAllAppointments", 1.5f);					// ...create new appointments
			}
		}
		*/
	}

	public void HideClipboardAppointments()
	{
		foreach (Appointment a in transform.GetComponentsInChildren<Appointment>())
		{
			a.renderer.enabled = false;	// hide all appointments
			a.collider.enabled = false;
			a.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
		}
	}

	public void ShowClipboardAppointments()
	{
		foreach (Appointment a in transform.GetComponentsInChildren<Appointment>())
		{
			a.renderer.enabled = true;	// hide all appointments
			a.collider.enabled = true;
			a.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
		}
	}

	/*
	void RearrangeAllAppointments()
	{
		List<Appointment> _unsortedAppts = new List<Appointment>();
		List<Appointment> _sortedAppts = new List<Appointment>();
		_unsortedAppts.AddRange(transform.GetComponentsInChildren<Appointment>());

		int counter = 0;
		while (_unsortedAppts.Count > 0 && counter < 50)
		{
			Appointment _nextHighestAppointment = null;
			foreach (Appointment a in _unsortedAppts)		// find all appointments and sort into a top-down list
			{
				if (_nextHighestAppointment == null || _nextHighestAppointment.transform.position.y < a.transform.position.y)
				{
					_nextHighestAppointment = a;
				}
			}
			_unsortedAppts.Remove(_nextHighestAppointment);
			_sortedAppts.Add (_nextHighestAppointment);
			counter++;
		}

		int i = 0;
		foreach (Appointment a in _sortedAppts)
		{
			a.myLerpTarget = new Vector3(transform.position.x, transform.position.y - (i*appointmentSpacing) + appointmentTop, transform.position.z - 0.1f);
			a.isLerping = true;
			i++;
		}
	}
	*/

	public void HideClipboard()
	{
		isHiding = true;
	}
}
