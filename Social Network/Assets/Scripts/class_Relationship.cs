using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class class_Relationship : MonoBehaviour {

	public int relationshipValue;
	public Friendship m_Friendship;
	public List<Person> relationshipMembers = new List<Person>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Person GetOppositeMember(Person _person)
	{
		foreach (Person member in relationshipMembers)
		{
			if (member != _person)
			{
				return member;
			}
		}
		print ("ERROR!");
		return null;
	}
}
