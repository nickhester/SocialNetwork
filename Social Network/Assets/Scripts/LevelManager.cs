using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	// public variables
	public int currentSeed;
	public List<Person> allPeople = new List<Person>();

	// private variables


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	int SeedTheLevel(int specificSeed)
	{
		// TODO: set the level's seed with the given number
	}

	int SeedTheLevel()
	{
		// TODO: set the level's seed with a random number
	}

	void InitiateLevel()
	{
		int indexCounter = 0;
		List<Person> notYetCompared = new List<Person>();
		notYetCompared.AddRange(allPeople);
		
		foreach (Person person in allPeople)							// go through each person
		{
			person.personalIndex = indexCounter;							// give each one an index
			person.name = "person " + person.personalIndex.ToString();		// set its name

			// set special attribute TODO: see if this is still applicable in new system
			if (levelUsed != null && levelUsed.isCantTouch && indexCounter == levelUsed.cantTouch) { person.canBeClicked = false; }

			indexCounter++;													// increment the index counter
			person.manager = this;	// save reference to manager in people
			
			foreach (Person secondPerson in notYetCompared)			// go through each other person
			{
				if (secondPerson != person)			// if they haven't already been compared (don't repeat A to A)
				{
					class_Relationship newRel = gameObject.AddComponent("class_Relationship") as class_Relationship;
					newRel.relationshipValue = ExponentialWeight(200);		// set random relationship value
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
		RelationshipValues = WeightedRelationships(allRelationships.Count - (player.relationshipList.Count), 10, 10, 200);
		
		
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
	*/
}
