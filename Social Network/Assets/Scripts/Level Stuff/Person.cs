using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class Person : MonoBehaviour {

	#region Variables

	private Renderer m_renderer;
	public List<Relationship> relationshipList;
	public List<Relationship> relationshipListNonZero;
	public List<Relationship> relationshipListNegative;
	public List<Relationship> relationshipListPositive;
	public int personalIndex;
	private Mood m_mood = Mood.Neutral;
	[HideInInspector]
	public NetworkManager networkMgr;
	[HideInInspector]
	public Transform myTransform;
	public GameObject myMaxIndicator;
	private GameObject _myMaxIndicator;
	private Renderer _myMaxIndicator_renderer;
	private bool hasBeenActivatedOnce = false;
	public bool canBeClicked = true;

	// faces
	public Material facialArt_happy;
	public Material facialArt_sad;
	public Material facialArt_normal;
	public Material facialArt_excited;
	private Material m_facialArt_happy;
	private Material m_facialArt_sad;
	private Material m_facialArt_normal;
	private Material m_facialArt_excited;
	public List<Texture> faces_happy = new List<Texture>();
	public List<Texture> faces_sad = new List<Texture>();
	public List<Texture> faces_normal = new List<Texture>();
	public List<Texture> faces_excited = new List<Texture>();
	public Material statusCircleGreen;
	public Material statusCircleRed;

	public GameObject pulseParti;
	public float animationTime = 0.25f;
	private Dictionary<Relationship, Mood> listOfAffectedRels = new Dictionary<Relationship, Mood>();
	public GameObject Mask_CannotClick;
	public GameObject myMask_CannotClick;

	private bool isExcited = false;

	#endregion
	
	#region StartAndUpdate

	public void Initialize(int _personIndex)
	{
		m_renderer = GetComponent<Renderer>();

		m_facialArt_happy = new Material(facialArt_happy);
		m_facialArt_sad = new Material(facialArt_sad);
		m_facialArt_normal = new Material(facialArt_normal);
		m_facialArt_excited = new Material(facialArt_excited);

		m_facialArt_happy.SetTexture("_MainTex", faces_happy[_personIndex]);
		m_facialArt_sad.SetTexture("_MainTex", faces_sad[_personIndex]);
		m_facialArt_normal.SetTexture("_MainTex", faces_normal[_personIndex]);
		m_facialArt_excited.SetTexture("_MainTex", faces_excited[_personIndex]);
		
		myTransform = transform;
		networkMgr = GameObject.FindGameObjectWithTag("networkManager").GetComponent<NetworkManager>();
		foreach (Relationship _rel in relationshipList)
		{
			if (_rel.m_Friendship != Friendship.Neutral) { relationshipListNonZero.Add(_rel); }
			if (_rel.m_Friendship == Friendship.Negative) { relationshipListNegative.Add(_rel); }
			if (_rel.m_Friendship == Friendship.Positive) { relationshipListPositive.Add(_rel); }
		}
		
		Vector3 positionJustBehindPerson = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
		_myMaxIndicator = Instantiate(myMaxIndicator, positionJustBehindPerson, Quaternion.identity) as GameObject;
		_myMaxIndicator.transform.localScale = new Vector3(1.9f, 1.9f, 1);
		_myMaxIndicator_renderer = _myMaxIndicator.GetComponent<Renderer>();
		_myMaxIndicator_renderer.enabled = false;
		_myMaxIndicator.transform.parent = transform;
		
		SetMood(Mood.Neutral);
		
		if (!canBeClicked)
		{
			myMask_CannotClick = Instantiate(Mask_CannotClick, transform.position, Quaternion.identity) as GameObject;
			myMask_CannotClick.transform.parent = transform;
		}
	}

	void Update ()
	{
		
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
		SetMood(changeToMood);

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
			EnableOneClickMask();
		}
	}

	void EnableOneClickMask()
	{
		myMask_CannotClick = Instantiate(Mask_CannotClick, transform.position, Quaternion.identity) as GameObject;
		myMask_CannotClick.transform.parent = transform;
	}

	public void DisableOneClickMask()
	{
		Destroy(myMask_CannotClick);
		myMask_CannotClick = null;
		hasBeenActivatedOnce = false;
		canBeClicked = true;
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
		if (finishIfDone)
		{
			networkMgr.EndIfDone();
		}
		networkMgr.CheckFinalMove();
	}
	
	public void AffectRelationship(Mood _moodTarget, Relationship _relationship)
	{
		_relationship.GetOppositeMember(this).SetMood(_moodTarget);
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

	public void SetAsExcited(bool _isExcited)
	{
		isExcited = _isExcited;
		SetMood(GetMood());
	}

	public void SetMood(Mood _newMood)
	{
		m_mood = _newMood;
		if (m_mood == Mood.Negative)
		{
			_myMaxIndicator_renderer.enabled = true;
			_myMaxIndicator_renderer.material = statusCircleRed;
			m_renderer.material = m_facialArt_sad;
		}
		else if (m_mood == Mood.Positive)
		{
			_myMaxIndicator_renderer.enabled = true;
			_myMaxIndicator_renderer.material = statusCircleGreen;
			m_renderer.material = m_facialArt_happy;
		}
		else if (m_mood == Mood.Neutral)
		{
			_myMaxIndicator_renderer.enabled = false;
			m_renderer.material = m_facialArt_normal;
		}

		if (isExcited)
		{
			m_renderer.material = m_facialArt_excited;
		}
	}

	public Mood GetMood()
	{
		return m_mood;
	}
}
