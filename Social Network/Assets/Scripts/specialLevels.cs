using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class SpecialLevels : MonoBehaviour {

	private bool fallsToRed = false;
	private float fallToRedTimer = 0.0f;
	private NetworkManager manager;
	private float fallToRedSeconds = 4.0f;

	void Start ()
	{
        if (GameObject.Find("Clipboard").GetComponent<Clipboard>().GetNextLevelUp().myLevel.isFallToRed)
		{
			fallsToRed = true;
		}
        manager = GameObject.FindGameObjectWithTag("networkManager").GetComponent<NetworkManager>();
	}

	void Update ()
	{
		fallToRedTimer += Time.deltaTime;
		if (fallsToRed)
		{
			if (fallToRedTimer >= fallToRedSeconds)
			{
				fallToRedTimer = 0;
				manager.GetAllPeople()[Random.Range(0, manager.GetNumPeople())].m_Mood = Mood.Negative;
			}
		}
	}
}
