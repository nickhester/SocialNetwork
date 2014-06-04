using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour {

	#region Variables

	public List<class_Relationship> relationshipList;
	public List<class_Relationship> relationshipListNonZeroNoPlayer;
	public List<class_Relationship> relationshipListNegativeNoPlayer;
	public List<class_Relationship> relationshipListPositiveNoPlayer;
	public int personalIndex;
	[HideInInspector]
	public class_NetworkMgr manager;
	[HideInInspector]
	public Person player;
	[HideInInspector]
	public class_Relationship relWithPlayer;
	[HideInInspector]
	public Transform myTransform;
	public GameObject myMaxIndicator;
	private GameObject _myMaxIndicator;
	private bool hasBeenActivatedOnce = false;
	public bool canBeClicked = true;

	// faces
	public Material facialArt1_happy;
	public Material facialArt1_sad;
	public Material facialArt1_normal;
	public Material statusCircleGreen;
	public Material statusCircleRed;

	public GameObject pulseParti;
	public float animationTime = 0.25f;
	private Dictionary<class_Relationship, int> listOfNonPlayerAffectedRels = new Dictionary<class_Relationship, int>();
	public GameObject Mask_CannotClick;
	public GameObject myMask_CannotClick;

	#endregion
	
	#region StartAndUpdate

	void Start () {

	}

	public void Initialize()
	{
		myTransform = transform;
		manager = transform.parent.GetComponent<class_NetworkMgr>() as class_NetworkMgr;
		foreach (class_Relationship _rel in relationshipList)
		{
			if (_rel.relationshipMembers.Contains(player))
			{
				relWithPlayer = _rel;
			}
			if (_rel.relationshipValue != 0 && !_rel.relationshipMembers.Contains(player)) { relationshipListNonZeroNoPlayer.Add(_rel); }
			if (_rel.relationshipValue < 0 && !_rel.relationshipMembers.Contains(player)) { relationshipListNegativeNoPlayer.Add(_rel); }
			if (_rel.relationshipValue > 0 && !_rel.relationshipMembers.Contains(player)) { relationshipListPositiveNoPlayer.Add(_rel); }
		}
		
		Vector3 positionJustBehindPerson = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
		_myMaxIndicator = Instantiate(myMaxIndicator, positionJustBehindPerson, Quaternion.identity) as GameObject;
		_myMaxIndicator.transform.localScale = new Vector3(1.9f, 1.9f, 1);
		_myMaxIndicator.renderer.enabled = false;
		_myMaxIndicator.transform.parent = transform;
		
		if (!canBeClicked)
		{
			myMask_CannotClick = Instantiate(Mask_CannotClick, transform.position, Quaternion.identity) as GameObject;
			myMask_CannotClick.transform.parent = transform;
		}
	}

	void Update () {
		if (relWithPlayer.relationshipValue == -100)
		{
			_myMaxIndicator.renderer.enabled = true;
			_myMaxIndicator.renderer.material = statusCircleRed;
			renderer.material = facialArt1_sad;
		}
		if (relWithPlayer.relationshipValue == 100)
		{
			_myMaxIndicator.renderer.enabled = true;
			_myMaxIndicator.renderer.material = statusCircleGreen;
			renderer.material = facialArt1_happy;
		}
	}

	#endregion
	
	// get my neighbor along a given relationship
	public Person GetMyNeighbor(class_Relationship _rel, Person _self)
	{
		foreach (Person _per in _rel.relationshipMembers)
		{
			if (_per != _self)
			{
				return _per;
			}
		}
		return null;
	}

	// duplicate for default value
	public Person GetMyNeighbor(class_Relationship _rel)
	{
		return GetMyNeighbor(_rel, this);
	}

	// default value method
	public void OnActivate(bool isPositiveChange)
	{
		OnActivate(isPositiveChange, false);
	}

	public void OnActivate(bool isPositiveChange, bool isDebugChange)
	{
		// check if in special game, if its already been clicked once. if so, don't allow click
		if (hasBeenActivatedOnce && GameObject.Find("Clipboard").GetComponent<clipboard>().nextLevelUp.myLevel.isOneClick)
		{ return; }
		if (!canBeClicked)
		{ return; }

		List<class_Relationship> relsNotWithPlayer = new List<class_Relationship>();
		foreach (class_Relationship rel in relationshipList)					// go through my relationships
		{
			if (rel.relationshipMembers.Contains(player))		// if it's with the player person
			{
				if (isPositiveChange)
				{
					// leave "changeByAmount" positive
				}
				else
				{
					manager.changeByAmount = -manager.changeByAmount;			// get negative of change by amount
				}
				AffectRelationship (manager.changeByAmount, rel);		// affect relationship between player and target by amount
			}
			else { relsNotWithPlayer.Add(rel); }
		}

		listOfNonPlayerAffectedRels.Clear();					// start with cleared list of affected relationships
		foreach (var otherRelationship in relsNotWithPlayer)			// for all relationships between the person and other people (not player)...
		{
			foreach (var _relationship in GetMyNeighbor(otherRelationship).relationshipList)		// ...get their list of relationships...
			{
				if (_relationship.relationshipMembers.Contains(player))	// ...with the player...
				{
					listOfNonPlayerAffectedRels.Add(_relationship, (int)(manager.changeByAmount * ((float)otherRelationship.relationshipValue / 100.0f)));
				}
			}
		}

		float _animationTime = animationTime;
		if (isDebugChange) { AffectListOfRelationships(false); _animationTime = 0; }
		else { Invoke ("AffectListOfRelationships", animationTime); }

		manager.changeByAmount = Mathf.Abs(manager.changeByAmount);		// set changeByAmount back to its absolute value

		if (!isDebugChange)
		{
			foreach (class_Relationship _rel in relationshipListNonZeroNoPlayer)
			{
				EffectPulse(transform.position, GetMyNeighbor(_rel).gameObject.transform.position, (isPositiveChange ? true : false), (_rel.relationshipValue == -100 ? true : false));
			}
		}
		hasBeenActivatedOnce = true;
		if (myMask_CannotClick == null && GameObject.Find("Clipboard").GetComponent<clipboard>().nextLevelUp.myLevel.isOneClick)
		{
			myMask_CannotClick = Instantiate(Mask_CannotClick, transform.position, Quaternion.identity) as GameObject;
			myMask_CannotClick.transform.parent = transform;
		}
	}

	// default method
	void AffectListOfRelationships()
	{
		AffectListOfRelationships(true);
	}

	void AffectListOfRelationships(bool finishIfDone)
	{
		foreach (var _rel in listOfNonPlayerAffectedRels)
		{
			AffectRelationship(_rel.Value, _rel.Key);
		}
		manager.CalculateScore();
		if (finishIfDone) { manager.EndIfDone(); }
	}

	public void AffectRelationship(int _amount, class_Relationship _relationship)
	{
		_relationship.relationshipValue += _amount;
		if (_relationship.relationshipValue > 100) {	_relationship.relationshipValue = 100;	}
		else if (_relationship.relationshipValue < -100)	{	_relationship.relationshipValue = -100;	}
	}

	void EffectPulse(Vector3 startPos, Vector3 endPos, bool isGreen, bool changesColor)
	{
		GameObject _parti = Instantiate(pulseParti, startPos, Quaternion.identity) as GameObject;
		parti_pulse _partiComponent = _parti.GetComponent<parti_pulse>();
		_partiComponent.startingPos = startPos;
		_partiComponent.endingPos = endPos;
		_partiComponent.timeToMove = animationTime;

		if (isGreen) { _partiComponent.isGreenAction = true; }
		else { _partiComponent.isGreenAction = false; }

		if (changesColor) { _partiComponent.changesColor = true; }
		else { _partiComponent.changesColor = false; }

		Destroy (_parti, animationTime + 0.5f);
	}
}
