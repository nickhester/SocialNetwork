﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types;

public class NetworkManager : MonoBehaviour {

	#region Variables

    private List<Person> allPeople = new List<Person>();
	private List<Relationship> savedStateAllRelationships = new List<Relationship>();
    private List<Relationship> allRelationships = new List<Relationship>();
    [SerializeField] private bool isUsingSeed;
    [SerializeField] private int randomSeed;
    public int usedSeed;
    [SerializeField] private bool isReadingSeedFromFile;
	private bool levelIsComplete = false;
	private const int percentHasRelationship = 50;  // if this number changes, all random seed values will need to be regenerated (don't do it!)
	private ValidLevels currentLevelInfo;
    [SerializeField] private GameObject[] peoplePositionParents;
    [SerializeField] private GameObject PersonPrefab;
    private bool isDemonstrationMode = false;

	// Score
	private int numActionsTaken = 0;
	
	// selection cursor stuff
    [SerializeField] private GameObject selectionCursor;
	private GameObject selectionCursorInst;
    [SerializeField] private GameObject cursorSecondary;
	private GameObject cursorSecondaryInst;
	private Vector3 selectionCursorCurrentPos;
	private Vector3 selectionCursorTargetPos;
	private GameObject currentlySelectedPerson;
	[SerializeField] private Toggle revealToggle;

	// audio
	private AudioSource myAudioComponent;
    [SerializeField] private AudioClip audioActionPos;
    [SerializeField] private AudioClip audioActionNeg;
	[HideInInspector] public bool isAudioOn_sfx;

	private SpecialEffects specialEffects;
	private TextBubble textBubble;

	// for webplayer only
	[SerializeField] private Material redAndGreenButton_keys;

	#endregion

	#region StartAndUpdate

	void Start ()
	{
		GameObject.FindGameObjectWithTag("persistentObject").GetComponent<CreateAndDestroyAppointment>().OnAppointmentStarted();

        usedSeed = SeedTheLevel();
        
        // spawn appropriate number of people and add them to the allPeople list
        allPeople.AddRange(SpawnPeople(currentLevelInfo.level));

		InitiateLevel();
		SaveStartingState();

		// generate random order of faces
		int numFaces = GetAllPeople()[0].faces_normal.Count;
		List<int> sequentialList = new List<int>();
		for (int i = 0; i <= numFaces; i++)
		{
			sequentialList.Add(i);
		}
		List<int> randomList = new List<int>();

		for (int i = numFaces; i >= 0; i--)
		{
			int n = Random.Range(0, i);
			randomList.Add(sequentialList[n]);
			sequentialList.RemoveAt(n);
		}

		foreach (Person _person in GetAllPeople())
		{
			_person.Initialize(randomList[0]);
			randomList.RemoveAt(0);
        }

		selectionCursorInst = Instantiate(selectionCursor) as GameObject;
		selectionCursorInst.GetComponent<Renderer>().enabled = false;
		selectionCursorInst.transform.parent = transform;

		cursorSecondaryInst = Instantiate
			(
				cursorSecondary, new Vector3
				(
					selectionCursorInst.transform.position.x, selectionCursorInst.transform.position.y, selectionCursorInst.transform.position.z + 1.5f
				), Quaternion.identity
			) as GameObject;
		cursorSecondaryInst.GetComponent<Renderer>().enabled = false;
		cursorSecondaryInst.transform.parent = selectionCursorInst.transform;

		myAudioComponent = gameObject.GetComponent<AudioSource>() as AudioSource;
		if (SaveGame.GetAudioOn_sfx()) { isAudioOn_sfx = true; }
		else { isAudioOn_sfx = false; }

		// set restart button position (to the left of the audio icons)
		Vector3 MusicButtonPos = GameObject.Find("audioToggle_music").transform.position;
		Vector3 SFXButtonPos = GameObject.Find("audioToggle_sfx").transform.position;
		float difference = Vector3.Distance(MusicButtonPos, SFXButtonPos);
		GameObject restartLevelButton = GameObject.Find("restartLevel");
		if (restartLevelButton != null)
		{
			restartLevelButton.transform.position = new Vector3(MusicButtonPos.x - difference, MusicButtonPos.y, MusicButtonPos.z);
		}

		specialEffects = GetComponent<SpecialEffects>();
		// reset seed back to something random
		Random.seed = (int)System.DateTime.Now.Second;
		textBubble = GetComponent<TextBubble>();
		textBubble.Init(allPeople);

		#if UNITY_WEBPLAYER
		
		GameObject.Find("redAndGreenButtons").GetComponent<Renderer>().material = redAndGreenButton_keys;

		#endif
	}

	void Update ()
	{
		selectionCursorCurrentPos = selectionCursorInst.transform.position;
		
		if (Input.GetKeyDown(KeyCode.Q) && currentlySelectedPerson != null)
		{
			TakeAction(true);
		}
		else if (Input.GetKeyDown(KeyCode.W) && currentlySelectedPerson != null)
		{
			TakeAction(false);
		}
		
		selectionCursorInst.transform.position = Vector3.Lerp(selectionCursorCurrentPos, selectionCursorTargetPos, 0.25f);
		
		// Debug lines
		foreach (var relationship in allRelationships)
		{
			Color c;
			if (relationship.m_Friendship == Friendship.Positive) { c = Color.green; }
			else if (relationship.m_Friendship == Friendship.Negative) { c = Color.red; }
			else { c = Color.black; }
			Debug.DrawLine(relationship.relationshipMembers[0].transform.position, relationship.relationshipMembers[1].transform.position, c);
		}
	}

	#endregion

    void Awake()
    {
        InputManager.Instance.OnClick += OnClick;
    }

    void OnClick(GameObject go)
    {
        if (go.tag == "people")							// if you're clicking on a person
        {
			revealToggle.isOn = true;

            // move cursor position on click
            selectionCursorInst.GetComponent<Renderer>().enabled = true;		// turn on cursor
            cursorSecondaryInst.GetComponent<Renderer>().enabled = true;		// turn on cursor
            selectionCursorTargetPos = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z + 0.1f);	// move cursor to location
            currentlySelectedPerson = go.transform.gameObject;			// set the clicked person as the currently selected person
            GetComponent<LineDisplay>().DisplayLines(go.transform.GetComponent<Person>());		// turn on only the lines touching the selected person
        }
		else if (go.name == "Button_green" && currentlySelectedPerson != null)
		{
			TakeAction(true);
		}
		else if (go.name == "Button_red" && currentlySelectedPerson != null)
		{
			TakeAction(false);
		}
        else if (go.name == "restartLevel")
        {
            ReloadStartingState();
			GameManager gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
			MetricsLogger.Instance.LogCustomEvent("Appointment", "RestartUsed", gm.FormatDayAndLevel());
        }
		else if (go.name == "reveal toggle")
		{
			OnToggle_Reveal(go.GetComponent<Toggle>());
		}
    }

	public void ReceiveClickFromUIButton(GameObject _go)
	{
		InputManager.Instance.SendMouseClick(_go);
	}

	public void OnToggle_Reveal(Toggle toggle)
	{
		if (toggle.isOn)	// the toggle is going from blank to "reveal" (turn off lines)
		{
			if (currentlySelectedPerson != null)
			{
				GetComponent<LineDisplay>().DisplayLines(currentlySelectedPerson.transform.GetComponent<Person>());
			}
			else
			{
				GetComponent<LineDisplay>().TurnOffAllLines();
			}
		}
		else				// the toggle is going from "reveal" to blank (turn on lines)
		{
			GetComponent<LineDisplay>().DisplayAllLines();
		}
	}

	public void DeselectAllPeople()
	{
		selectionCursorInst.GetComponent<Renderer>().enabled = false;
		cursorSecondaryInst.GetComponent<Renderer>().enabled = false;
		currentlySelectedPerson = null;
		GetComponent<LineDisplay>().TurnOffAllLines();
	}

    public void OnDestroy()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnClick -= OnClick;
    }

    List<Person> SpawnPeople(int _numPeople)
    {
        List<Person> retVal = new List<Person>();
        if (peoplePositionParents[_numPeople] != null)
        {
            Transform[] peoplePositions = peoplePositionParents[_numPeople].transform.GetComponentsInChildren<Transform>();
            foreach (Transform position in peoplePositions)
            {
                if (position.gameObject != peoplePositionParents[_numPeople])   // don't include the parent
                {
                    GameObject go = Instantiate(PersonPrefab, position.position, Quaternion.identity) as GameObject;
                    go.transform.SetParent(transform);
                    go.name = position.gameObject.name;
                    retVal.Add(go.GetComponent<Person>());
                }
            }
        }
        else
        {
            Debug.LogError("People Position Group not found for current level");
        }

        retVal = retVal.OrderBy(item => item.gameObject.name).ToList();

        return retVal;
    }

    void TakeAction(bool _isPositive)
    {
		if (_isPositive)
        {
            TriggerRelationshipChange(currentlySelectedPerson, true);
            if (isAudioOn_sfx) { myAudioComponent.clip = audioActionPos; }
        }
        else
        {
            TriggerRelationshipChange(currentlySelectedPerson, false);
            if (isAudioOn_sfx) { myAudioComponent.clip = audioActionNeg; }
        }
        if (isAudioOn_sfx) { myAudioComponent.Play(); }
        numActionsTaken++;
		textBubble.ShowBubble(currentlySelectedPerson.transform.position, _isPositive);
    }

	#region TestRunMethods

	void SaveStartingState()
	{
		foreach (Relationship _rel in allRelationships)
		{
			savedStateAllRelationships.Add(_rel);
		}
	}

	public void ReloadStartingState()
	{
		GameObject.FindWithTag("GameManager").GetComponent<GameManager>().Event_AppointmentReset();
		foreach (Person _ppl in GetAllPeople())
		{
			_ppl.SetMood(Mood.Neutral);
			if (currentLevelInfo.isOneClick)
			{
				_ppl.DisableOneClickMask();
			}
		}
        numActionsTaken = 0;
		DeselectAllPeople();
		specialEffects.StopShakingExcitedly();
	}

    public void SetAsDemonstration(bool _isDemonstration)
    {
        isDemonstrationMode = _isDemonstration;
    }

	// save seed used whether randomly generated or user set
	public int SeedTheLevel()
	{
		int _usedSeed;
        currentLevelInfo = GameObject.FindWithTag("clipboard").GetComponent<Clipboard>().GetNextLevelUp().myLevel;

		if (currentLevelInfo.difficulty == Difficulty.VeryEasy) { isReadingSeedFromFile = true; }
		else if (currentLevelInfo.difficulty == Difficulty.Easy) { isReadingSeedFromFile = true; }
		else if (currentLevelInfo.difficulty == Difficulty.Medium) { isReadingSeedFromFile = true; }
		else if (currentLevelInfo.difficulty == Difficulty.Hard) { isReadingSeedFromFile = true; }
		else if (currentLevelInfo.difficulty == Difficulty.Unknown) { isReadingSeedFromFile = false; isUsingSeed = false; }
		else { print ("error -- difficulty could not be read (generating level randomly)"); isReadingSeedFromFile = false; isUsingSeed = false; }

		if (isReadingSeedFromFile)	// if using a level from a file
		{
			_usedSeed = currentLevelInfo.seed;
			Random.seed = _usedSeed;
		}
		else if (isUsingSeed)		// if using a set seed
		{
			Random.seed = randomSeed;
			_usedSeed = randomSeed;		// save the seed used
		}
		else 					// if using a random seed
		{
			_usedSeed = currentLevelInfo.seed;
			Random.seed = _usedSeed;		// save the seed used
			// copy used seed to the clipboard (windows clipboard)
			TextEditor te = new TextEditor(); te.content = new GUIContent(_usedSeed.ToString()); te.SelectAll(); te.Copy();
		}
		return _usedSeed;
	}

	#endregion

	#region EndLevelStuff

	// end level if winning requirements have been met
	public void EndIfDone()
	{
	    if (CheckWinningRequirements())
	    {
	        foreach (Person _person in GetAllPeople())
	        {
	            _person.GetComponent<Collider>().enabled = false;
	            _person.GetComponent<PersonMovement>().MoveOut();
	        }
	        GetComponent<LineDisplay>().TurnOffAllLines();
	        selectionCursorInst.GetComponent<Renderer>().enabled = false;
	        cursorSecondaryInst.GetComponent<Renderer>().enabled = false;
	        WinningScreen();
	    }
	}
	
	bool CheckWinningRequirements()
	{
		foreach (Person _person in GetAllPeople())
		{
			if (_person.GetMood() != Mood.Positive)
			{
				return false;
			}
		}

        if (isDemonstrationMode)
        {
            return false;
        }
		return true;
	}

	void WinningScreen()
	{
        if (!levelIsComplete)
        {
            GameObject.FindGameObjectWithTag("persistentObject").GetComponent<CreateAndDestroyAppointment>().RoundEnd(true, numActionsTaken);
            levelIsComplete = true;
        }
	}

	#endregion
	
	#region StartLevelStuff
	
	void InitiateLevel()
	{
		int indexCounter = 0;
		List<Person> notYetCompared = new List<Person>();
		notYetCompared.AddRange(GetAllPeople());

		foreach (Person person in GetAllPeople())							// go through each person
		{
			person.personalIndex = indexCounter;							// give each one an index
			person.name = "person " + person.personalIndex.ToString();		// set its name
			// set special attribute
			if (currentLevelInfo != null && currentLevelInfo.isCantTouch && indexCounter == currentLevelInfo.cantTouch) { person.canBeClicked = false; }
			indexCounter++;													// increment the index counter
			person.networkMgr = this;	// save reference to manager in people

			foreach (Person secondPerson in notYetCompared)			// go through each other person
			{
				if (secondPerson != person)			// if they haven't already been compared (don't repeat A to A)
				{
					Relationship newRel = gameObject.AddComponent<Relationship>() as Relationship;
					newRel.relationshipMembers.Add(person);
					newRel.relationshipMembers.Add (secondPerson);
					allRelationships.Add(newRel);
					person.relationshipList.Add(newRel);
					secondPerson.relationshipList.Add (newRel);
				}
			}
			notYetCompared.Remove(person);			// remove from list so you don't get duplicate relationships (a,b) & (b,a)
		}
		
		// Recalculate the values
		List<Friendship> RelationshipValues = new List<Friendship>();
		RelationshipValues = WeightedRelationships(allRelationships.Count, percentHasRelationship);


		foreach (Relationship rel in allRelationships)
		{
			if (RelationshipValues.Count > 0)
			{
				//int _mood = RelationshipValues[Random.Range (0, RelationshipValues.Count)];	// pick a random relationship to assign a value to // TODO: phase out rel values
				Friendship _friendship = RelationshipValues[Random.Range (0, RelationshipValues.Count)];	// pick a random relationship to assign a value to
				//rel.relationshipValue = _mood;		 // TODO: phase out rel values
				rel.m_Friendship = _friendship;
				RelationshipValues.Remove(_friendship);						// then remove that one you picked
			}
		}
	}
	
	List<Friendship> WeightedRelationships(int quantity, int _percentHasRelationship)
	{
		List<int> result = new List<int>();
		int numNonZero = (int)(quantity * (_percentHasRelationship/100.0f));
		for (int i = 0; i < quantity; i++)
		{
			if (i <= numNonZero)
			{
				int tempResult = 100;
				result.Add(tempResult * (Random.Range (-1, 2)));
			}
			else
			{
				int tempResult = 0;
				result.Add(tempResult);
			}
		}

		List<Friendship> newResult = new List<Friendship>();
		foreach (int i in result)	
		{
			if (i == -100) { newResult.Add(Friendship.Negative); }
			else if (i == 100) { newResult.Add(Friendship.Positive); }
			else { newResult.Add(Friendship.Neutral); }
		}
		return newResult;
	}

	#endregion

    // What to do when you click on a person
	public void TriggerRelationshipChange(GameObject personObject, bool isPositiveChange)
	{
		TriggerRelationshipChange(personObject, isPositiveChange, false);
	}

	// overload which doesn't trigger animations
	public void TriggerRelationshipChange(GameObject personObject, bool isPositiveChange, bool isDebugChange)
	{
	    Person targetPerson = personObject.GetComponent<Person>();  // get the clicked object's person component
	    
	    targetPerson.OnActivate(isPositiveChange, isDebugChange);      // tell that person's component to do what they do when they're activated
	}

    public List<Person> GetAllPeople()
    {
        return allPeople;
    }

    public int GetNumPeople()
    {
        return allPeople.Count;
    }

	public void CheckFinalMove()
	{
		Person finalMovePerson = null;

		List<Person> peopleWithOnlyPositiveRelationships = new List<Person>();
		List<Person> peopleWhoAreUnhappy = new List<Person>();
		foreach (Person _person in GetAllPeople())
		{
			if (_person.relationshipListNegative.Count() == 0)
			{
				peopleWithOnlyPositiveRelationships.Add(_person);
			}

			if (_person.GetMood() != Mood.Positive)
			{
				peopleWhoAreUnhappy.Add(_person);
			}
		}

		foreach (Person _person in peopleWithOnlyPositiveRelationships)
		{
			bool logDebug = false;
			if (logDebug) { print("checking " + _person.name); }
			bool hasFoundSolution = true;
			List<Person> allMyFriends = new List<Person>();
			foreach (Relationship _relation in _person.relationshipListPositive)
			{
				if (logDebug) { print("adding " + _relation.GetOppositeMember(_person).name + " to allMyFriends list"); }
				allMyFriends.Add(_relation.GetOppositeMember(_person));
			}
			// are all of peopleWhoAreUnhappy contained in allMyFriends?
			foreach (Person _unhappyPerson in peopleWhoAreUnhappy)
			{
				if (logDebug) { print("am i friends with " + _unhappyPerson + "?"); }
				if (_person != _unhappyPerson && !allMyFriends.Contains(_unhappyPerson))
				{
					if (logDebug) { print("no"); }
					hasFoundSolution = false;
				}
				else
				{
					if (logDebug) { print("yes"); }
				}
			}
			if (hasFoundSolution)
			{
				if (logDebug) { print("that'll do it!"); }
				finalMovePerson = _person;
				break;
			}
		}

		// if it found one...
		if (finalMovePerson != null)
		{
			specialEffects.ShakeExcitedly(finalMovePerson);
		}
		else
		{
			specialEffects.StopShakingExcitedly();
		}
	}
}
