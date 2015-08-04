using UnityEngine;
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
    private TextAsset validSeedList;        // not being used now, but might be needed to do level "show me"
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

	// audio
	private AudioSource myAudioComponent;
    [SerializeField] private AudioClip audioActionPos;
    [SerializeField] private AudioClip audioActionNeg;
	[HideInInspector] public bool isAudioOn_sfx;

	// for webplayer only
	[SerializeField] private Material redAndGreenButton_keys;

	private float restartButtonOffset = -0.8f;

	#endregion

	#region StartAndUpdate

	void Start ()
	{
        validSeedList = Resources.Load("validSeedList") as TextAsset;
        usedSeed = SeedTheLevel();
        
        // spawn appropriate number of people and add them to the allPeople list
        allPeople.AddRange(SpawnPeople(currentLevelInfo.level));

		InitiateLevel();
		SaveStartingState();

		foreach (Person _person in GetAllPeople())
		{
            _person.Initialize();
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

		// set restart button position (just put it to the left of the audio icons
		Vector3 audioButtonPos = GameObject.Find("audioToggle_music").transform.position;
		GameObject restartLevelButton = GameObject.Find("restartLevel");
		if (restartLevelButton != null)
		{
			restartLevelButton.transform.position = new Vector3(audioButtonPos.x + restartButtonOffset, audioButtonPos.y, audioButtonPos.z);
		}

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
        if (go.transform.tag == "people")							// if you're clicking on a person
        {
            // move cursor position on click
            selectionCursorInst.GetComponent<Renderer>().enabled = true;		// turn on cursor
            cursorSecondaryInst.GetComponent<Renderer>().enabled = true;		// turn on cursor
            selectionCursorTargetPos = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z + 0.1f);	// move cursor to location
            currentlySelectedPerson = go.transform.gameObject;			// set the clicked person as the currently selected person
            GetComponent<LineDisplay>().TurnOffAllLines();		// turn off all the lines
            GetComponent<LineDisplay>().DisplayLines(go.transform.GetComponent<Person>());		// turn on only the lines touching the selected person
        }
        else if (go.transform.name == "Button_green" && currentlySelectedPerson != null)
        {
            TakeAction(true);
        }
        else if (go.transform.name == "Button_red" && currentlySelectedPerson != null)
        {
            TakeAction(false);
        }
        else if (go.transform.name == "restartLevel")
        {
            ReloadStartingState();
			if (currentLevelInfo.isOneClick)
			{
				foreach (Person person in allPeople)
				{
					person.DisableOneClickMask();
				}
			}
			GameManager gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
			MetricsLogger.Instance.LogCustomEvent("Appointment", "RestartUsed", gm.FormatDayAndLevel());
        }
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

    void TakeAction(bool isPositive)
    {
        if (isPositive)
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
			_ppl.m_Mood = Mood.Neutral;
		}
        numActionsTaken = 0;
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
	
	public bool CheckWinningRequirements()
	{
		foreach (Person _person in GetAllPeople())
		{
			if (_person.m_Mood != Mood.Positive)
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
}
