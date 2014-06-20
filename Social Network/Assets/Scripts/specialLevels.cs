using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class specialLevels : MonoBehaviour {

	private bool fallsToRed = false;
	private float fallToRedTimer = 0.0f;
	private class_NetworkMgr manager;
	private float fallToRedSeconds = 4.0f;

	void Start ()
	{
		if (GameObject.Find("Clipboard").GetComponent<clipboard>().nextLevelUp.myLevel.isFallToRed)
		{
			fallsToRed = true;
		}
		manager = GameObject.Find("networkMgr").GetComponent<class_NetworkMgr>();
	}

	void Update ()
	{
		fallToRedTimer += Time.deltaTime;
		if (fallsToRed)
		{
			if (fallToRedTimer >= fallToRedSeconds)
			{
				fallToRedTimer = 0;
				manager.allPeople[Random.Range(0, manager.allPeople.Count)].m_Mood = Mood.Negative;
			}
		}
	}
}
