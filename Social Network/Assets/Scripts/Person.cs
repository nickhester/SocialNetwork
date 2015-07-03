using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class Person : MonoBehaviour {

	#region Variables

	public List<Relationship> relationshipList;
	public List<Relationship> relationshipListNonZero;
	public List<Relationship> relationshipListNegative;
	public List<Relationship> relationshipListPositive;
	public int personalIndex;
	public Mood m_Mood = Mood.Neutral;
	[HideInInspector]
	public NetworkManager networkMgr;
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
	private Dictionary<Relationship, Mood> listOfAffectedRels = new Dictionary<Relationship, Mood>();
	public GameObject Mask_CannotClick;
	public GameObject myMask_CannotClick;

	#endregion
	
	#region StartAndUpdate

	public void Initialize()
	{
		myTransform = transform;
		networkMgr = GameObject.FindGameObjectWithTag("networkManager").GetComponent<NetworkManager>();
		foreach (Relationship _rel in relationshipList)
		{
			if (_rel.m_Friendship != Friendship.Neutral) { relationshipListNonZero.Add(_rel); }
			if (_rel.m_Friendship < Friendship.Negative) { relationshipListNegative.Add(_rel); }
			if (_rel.m_Friendship > Friendship.Positive) { relationshipListPositive.Add(_rel); }
		}
		
		Vector3 positionJustBehindPerson = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
		_myMaxIndicator = Instantiate(myMaxIndicator, positionJustBehindPerson, Quaternion.identity) as GameObject;
		_myMaxIndicator.transform.localScale = new Vector3(1.9f, 1.9f, 1);
		_myMaxIndicator.GetComponent<Renderer>().enabled = false;
		_myMaxIndicator.transform.parent = transform;
		
		if (!canBeClicked)
		{
			myMask_CannotClick = Instantiate(Mask_CannotClick, transform.position, Quaternion.identity) as GameObject;
			myMask_CannotClick.transform.parent = transform;
		}
	}

	void Update () {
		if (m_Mood == Mood.Negative)
		{
			_myMaxIndicator.GetComponent<Renderer>().enabled = true;
			_myMaxIndicator.GetComponent<Renderer>().material = statusCircleRed;
			GetComponent<Renderer>().material = facialArt1_sad;
		}
		if (m_Mood == Mood.Positive)
		{
			_myMaxIndicator.GetComponent<Renderer>().enabled = true;
			_myMaxIndicator.GetComponent<Renderer>().material = statusCircleGreen;
			GetComponent<Renderer>().material = facialArt1_happy;
		}
	}

	#endregion
	
	// get my neighbor along a given relationship
	public Person GetMyNeighbor(Relationship _rel, Person _self)
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
	public Person GetMyNeighbor(Relationship _rel)
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
        if (hasBeenActivatedOnce && GameObject.Find("Clipboard").GetComponent<Clipboard>().GetNextLevelUp().myLevel.isOneClick)
		{ return; }
		if (!canBeClicked)
		{ return; }

		Mood changeToMood;

		if (isPositiveChange)
		{
			changeToMood = Mood.Positive;
		}
		else
		{
			changeToMood = Mood.Negative;
		}
		this.m_Mood = changeToMood;

		listOfAffectedRels.Clear();					// start with cleared list of affected relationships
		foreach (Relationship rel in relationshipList)					// go through my relationships
		{
			if (rel.m_Friendship == Friendship.Positive) { listOfAffectedRels.Add(rel, changeToMood); }
			else if (rel.m_Friendship == Friendship.Negative)
			{
				if (changeToMood == Mood.Positive) { listOfAffectedRels.Add(rel, Mood.Negative); }
				else if (changeToMood == Mood.Negative) { listOfAffectedRels.Add(rel, Mood.Positive); }
			}
		}

		if (isDebugChange) { AffectListOfRelationships(false); }
		else { Invoke ("AffectListOfRelationships", animationTime); }

		if (!isDebugChange)
		{
			foreach (Relationship _rel in relationshipListNonZero)
			{
				EffectPulse(transform.position, GetMyNeighbor(_rel).gameObject.transform.position, isPositiveChange, (_rel.m_Friendship == Friendship.Negative));
			}
		}
		hasBeenActivatedOnce = true;
        if (myMask_CannotClick == null && GameObject.Find("Clipboard").GetComponent<Clipboard>().GetNextLevelUp().myLevel.isOneClick)
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
		foreach (var _rel in listOfAffectedRels)
		{
			AffectRelationship(_rel.Value, _rel.Key);
		}
		if (finishIfDone) { networkMgr.EndIfDone(); }
	}
	
	public void AffectRelationship(Mood _moodTarget, Relationship _relationship)
	{
		_relationship.GetOppositeMember(this).m_Mood = _moodTarget;
	}

	void EffectPulse(Vector3 startPos, Vector3 endPos, bool isGreen, bool changesColor)
	{
		GameObject _parti = Instantiate(pulseParti, startPos, Quaternion.identity) as GameObject;
		Parti_Pulse _partiComponent = _parti.GetComponent<Parti_Pulse>();
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
