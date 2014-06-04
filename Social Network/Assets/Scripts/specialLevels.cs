using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class specialLevels : MonoBehaviour {

	private bool fallsToRed = false;
	private float fallToRedTimer = 0.0f;
	private Person player;
	private List<class_Relationship> playerRels = new List<class_Relationship>();
	private float fallToRedSeconds = 4.0f;

	void Start ()
	{
		if (GameObject.Find("Clipboard").GetComponent<clipboard>().nextLevelUp.myLevel.isFallToRed)
		{
			fallsToRed = true;
		}
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Person>();


	}

	void Update ()
	{
		if (playerRels.Count == 0) { playerRels.AddRange(player.relationshipList); }

		fallToRedTimer += Time.deltaTime;
		if (fallsToRed)
		{
			if (fallToRedTimer >= fallToRedSeconds)
			{
				fallToRedTimer = 0;
				player.AffectRelationship(-200, playerRels[Random.Range(0, playerRels.Count)]);
			}
		}
	}
}
