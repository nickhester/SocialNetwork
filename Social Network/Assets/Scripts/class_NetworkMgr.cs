﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types;

public class class_NetworkMgr : MonoBehaviour {

	#region Variables

	public List<Person> allPeople = new List<Person>();
	public List<Person> allPeopleExceptPlayer = new List<Person>();
	private Dictionary<class_Relationship, int> savedStateAllRelationships = new Dictionary<class_Relationship, int>();
	public List<class_Relationship> allRelationships = new List<class_Relationship>();
	[HideInInspector]
	public Person player;
	public int changeByAmount = 200;
	private bool showDebug = false;
	public GameObject hitParticle;
	public bool isUsingSeed = false;
	public int randomSeed = 0;
	public int usedSeed;
	public bool isReadingSeedFromFile;
	public TextAsset validSeedList;
	private bool levelIsComplete = false;
	private int percentHasRelationship = 50;
	private validLevels levelUsed;

	// Debug and Score
	private string DebugSummary = "";
	private int playerRelTotal = 0;
	private int playerRelTotalCounter = 0;
	private int relAverage = 0;
	public string AIbutton = "AI";
	[HideInInspector]
	public int AIScoreReturn;
	[HideInInspector]
	public int numActionsTaken = 0;
	private levelTester levelTesterRef;
	
	// selection cursor stuff
	public GameObject selectionCursor;
	private GameObject selectionCursorInst;
	public GameObject cursorSecondary;
	private GameObject cursorSecondaryInst;
	private Vector3 selectionCursorCurrentPos;
	private Vector3 selectionCursorTargetPos;
	private GameObject currentlySelectedPerson;

	// special attributes
	public bool special_FallToRed = false;
	public bool special_OneClick = false;
	public bool special_CantTouchLines = false;
	public bool special_NoLines = false;

	#endregion

	#region StartAndUpdate

	void Start ()
	{
		levelTesterRef = GameObject.FindWithTag("levelTester").GetComponent<levelTester>();

		allPeople.AddRange(GetComponentsInChildren<Person>());
		
		foreach (Person _per in allPeople)
		{
			if (_per.gameObject.tag == "Player") { player = _per.GetComponent<Person>() as Person; } // pick out "player"
		}

		allPeopleExceptPlayer.AddRange(allPeople);
		allPeopleExceptPlayer.Remove(player);

		usedSeed = SeedTheLevel();

		InitiateLevel();
		SaveStartingState();
		CalculateScore();

		foreach (Person _per in allPeople)
		{ _per.Initialize(); }

		selectionCursorInst = Instantiate(selectionCursor) as GameObject;
		selectionCursorInst.renderer.enabled = false;
		selectionCursorInst.transform.parent = transform;

		cursorSecondaryInst = Instantiate
			(
			cursorSecondary, new Vector3
			(
			selectionCursorInst.transform.position.x, selectionCursorInst.transform.position.y, selectionCursorInst.transform.position.z + 1.5f
			), Quaternion.identity
			) as GameObject;
		cursorSecondaryInst.renderer.enabled = false;
		cursorSecondaryInst.transform.parent = selectionCursorInst.transform;
	}

	void Update ()
	{
		
		selectionCursorCurrentPos = selectionCursorInst.transform.position;
		
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))		// if clicking either mouse button
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100.0f))
			{
				if (hit.transform.tag == "people")							// if you're clicking on a person
				{
					// move cursor position on click
					selectionCursorInst.renderer.enabled = true;		// turn on cursor
					cursorSecondaryInst.renderer.enabled = true;		// turn on cursor
					selectionCursorTargetPos = new Vector3(hit.transform.position.x, hit.transform.position.y, hit.transform.position.z + 0.1f);	// move cursor to location
					currentlySelectedPerson = hit.transform.gameObject;			// set the clicked person as the currently selected person
					GetComponent<class_LineDisplay>().TurnOffAllLines();		// turn off all the lines
					GetComponent<class_LineDisplay>().DisplayLines(hit.transform.GetComponent<Person>());		// turn on only the lines touching the selected person
				}
				else if (hit.transform.name == "Button_green" && currentlySelectedPerson != null)
				{
					TriggerRelationshipChange(currentlySelectedPerson, true);
					numActionsTaken++;
				}
				else if (hit.transform.name == "Button_red" && currentlySelectedPerson != null)
				{
					TriggerRelationshipChange(currentlySelectedPerson, false);
					numActionsTaken++;
				}
			}
		}
		
		selectionCursorInst.transform.position = Vector3.Lerp(selectionCursorCurrentPos, selectionCursorTargetPos, 0.25f);
		
		// Debug lines
		foreach (var relationship in allRelationships)
		{
			float redValue;
			float greenValue;
			if (relationship.relationshipValue > 0) { greenValue = (float)relationship.relationshipValue / 100; redValue = 0; }
			else { greenValue = 0; redValue = -(float)relationship.relationshipValue / 100; }
			
			Debug.DrawLine(relationship.relationshipMembers[0].transform.position, relationship.relationshipMembers[1].transform.position, 
			               new Color(redValue, greenValue, 0), 0.1f);
		}
	}

	#endregion

	#region TestRunMethods

	void RestartCurrentLevel()
	{
		isUsingSeed = true;
		randomSeed = usedSeed;
		CalculateScore();
	}

	void SaveStartingState()
	{
		foreach (class_Relationship _rel in allRelationships)
		{
			savedStateAllRelationships.Add(_rel, _rel.relationshipValue);
		}
	}

	public void ReloadStartingState()
	{
		foreach (class_Relationship _rel in allRelationships)
		{
			_rel.relationshipValue = savedStateAllRelationships[_rel];
		}
		CalculateScore();
	}

	// save seed used whether randomly generated or user set
	public int SeedTheLevel()
	{
		int _usedSeed;
		levelUsed = GameObject.FindWithTag("clipboard").GetComponent<clipboard>().nextLevelUp.myLevel;

		if (levelUsed.difficulty == Difficulty.VeryEasy) { isReadingSeedFromFile = true; }
		else if (levelUsed.difficulty == Difficulty.Easy) { isReadingSeedFromFile = true; }
		else if (levelUsed.difficulty == Difficulty.Medium) { isReadingSeedFromFile = true; }
		else if (levelUsed.difficulty == Difficulty.Hard) { isReadingSeedFromFile = true; }
		else if (levelUsed.difficulty == Difficulty.Unknown) { isReadingSeedFromFile = false; isUsingSeed = false; }
		else { print ("error -- difficulty could not be read (generating level randomly)"); isReadingSeedFromFile = false; isUsingSeed = false; }

		if (isReadingSeedFromFile)	// if using a level from a file
		{
			_usedSeed = levelUsed.seed;
			Random.seed = _usedSeed;
		}
		else if (isUsingSeed)		// if using a set seed
		{
			Random.seed = randomSeed;
			_usedSeed = randomSeed;		// save the seed used
		}
		else 					// if using a random seed
		{
			_usedSeed = levelUsed.seed;
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
	        foreach (Person _person in allPeople)
	        {
	            _person.collider.enabled = false;
	            _person.GetComponent<personMovement>().MoveOut();
	        }
	        GetComponent<class_LineDisplay>().TurnOffAllLines();
	        selectionCursorInst.renderer.enabled = false;
	        cursorSecondaryInst.renderer.enabled = false;
	        WinningScreen();
	    }
	}
	
	public bool CheckWinningRequirements()
	{
		if (relAverage >= 100) { return true; }
		else { return false; }
	}

	void WinningScreen()
	{
        if (!levelIsComplete)
        {
            GameObject.FindGameObjectWithTag("persistentObject").GetComponent<createAndDestroyLevel>().RoundEnd(true, numActionsTaken);
            levelIsComplete = true;
        }
	}

	public void CalculateScore()
	{
	    int counter = 0;
	    playerRelTotal = 0;
	    playerRelTotalCounter = 0;
	    DebugSummary = "\n==Debug==\n";
	    foreach (class_Relationship _relationship in allRelationships)
	    {
	        DebugSummary += counter + " (" + _relationship.relationshipMembers[0].name.Substring(7) + "," + _relationship.relationshipMembers[1].name.Substring(7) + "): ";
	        DebugSummary += _relationship.relationshipValue + "\n";
	        counter++;
	        if (_relationship.relationshipMembers.Contains(player))     // if the relationship is with the player
	        {
	            playerRelTotal += _relationship.relationshipValue;
	            playerRelTotalCounter++;
	        }
	    }
	    relAverage = playerRelTotal / playerRelTotalCounter;
	    DebugSummary += "\nStrength: " + changeByAmount;
	}

	#endregion
	
	#region StartLevelStuff
	
	void InitiateLevel()
	{
		int indexCounter = 0;
		List<Person> notYetCompared = new List<Person>();
		notYetCompared.AddRange(allPeople);

		foreach (Person person in allPeople)							// go through each person
		{
			person.personalIndex = indexCounter;							// give each one an index
			person.name = "person " + person.personalIndex.ToString();		// set its name
			// set special attribute
			if (levelUsed != null && levelUsed.isCantTouch && indexCounter == levelUsed.cantTouch) { person.canBeClicked = false; }
			indexCounter++;													// increment the index counter
			person.manager = this;	// save reference to manager in people

			foreach (Person secondPerson in notYetCompared)			// go through each other person
			{
				if (secondPerson != person)			// if they haven't already been compared (don't repeat A to A)
				{
					class_Relationship newRel = gameObject.AddComponent("class_Relationship") as class_Relationship;
					// TODO: is this line necessary? // newRel.relationshipValue = ExponentialWeight(200);		// set random relationship value
					newRel.relationshipMembers.Add(person);
					newRel.relationshipMembers.Add (secondPerson);
					allRelationships.Add(newRel);
					person.relationshipList.Add(newRel);
					secondPerson.relationshipList.Add (newRel);
				}
				person.player = player;
			}
			notYetCompared.Remove(person);			// remove from list so you don't get duplicate relationships (a,b) & (b,a)
		}
		
		// Renumber the values
		List<int> RelationshipValues = new List<int>();
		RelationshipValues = WeightedRelationships(allRelationships.Count - (player.relationshipList.Count), percentHasRelationship);


		foreach (class_Relationship rel in allRelationships)
		{
			Random.Range(0,1);		// just a random call to keep the seed in sync with old data
			if (!rel.relationshipMembers.Contains(player) && RelationshipValues.Count > 0)
			{
				int _i = RelationshipValues[Random.Range (0, RelationshipValues.Count)];	// pick a random relationship to assign a value to
				rel.relationshipValue = _i;
				RelationshipValues.Remove(_i);						// then remove that one you picked
			}
		}
	}

	// TODO: remove this function
//	// returns an even spread of numbers for relationships
//	int ExponentialWeight(int divisionFactor)
//	{
//	    int diceRoll = Random.Range(0, 101);                    // get a number from 0 to 100
//	    diceRoll = (diceRoll * diceRoll) / divisionFactor;      // divide by the "division factor"
//	    if (diceRoll > 100) { diceRoll = 100; }                 // clamp to 100
//	    if (diceRoll < -100) { diceRoll = -100; }                   // clamp to -100
//	    if (Random.Range(0, 2) == 1) { diceRoll = diceRoll * -1; }  // choose whether positive or negative
//	    return diceRoll;
//	}
	
	List<int> WeightedRelationships(int quantity, int _percentHasRelationship)
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
		return result;
	}

	#endregion

	public void TriggerRelationshipChange(GameObject personObject, bool isPositiveChange)
	{
		TriggerRelationshipChange(personObject, isPositiveChange, false);
	}

	// What to do when you click on a person
	public void TriggerRelationshipChange(GameObject personObject, bool isPositiveChange, bool isDebugChange)
	{
	    Person targetPerson = personObject.GetComponent<Person>();  // get the clicked object's person component
	    
	    targetPerson.OnActivate(isPositiveChange, isDebugChange);      // tell that person's component to do what they do when they're activated
	}

	void OnGUI()
	{
		// Debug info
		if (GUI.Button(new Rect(0, Screen.height - 20, 50, 20), "Debug")) { showDebug = true; }
		if (showDebug)
		{
			//GUI.Box (new Rect(0, 0, 100, Screen.height), DebugSummary);
			if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 50, 75, 25), AIbutton)) { levelTesterRef.ComputerPlay(10000, 1); }
			if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 50, 75, 25), "path"))
			{
				if (!levelTesterRef.HasFailCase())
				{
					levelTesterRef.PathfindLevel_BreadthFirst();
				}
			}

		}
	}
}
