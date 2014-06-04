using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class class_Entity : MonoBehaviour {

	public Dictionary<class_Entity, int> relationships;
	public int relationshipWithPlayer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))

			{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100.0f))
			{
				print ("hit something" + hit.transform.name);
			}
		}
	}	
}

