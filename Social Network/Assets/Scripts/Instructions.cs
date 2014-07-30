using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Instructions : MonoBehaviour {

	public int instructionIndex;
	public List<Material> instructionMats;

	// Use this for initialization
	void Start () {
		gameObject.renderer.enabled = false;
		gameObject.collider.enabled = false;
		ShowInstructions(instructionIndex);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			PlayerPrefs.SetInt("hasSeenInstruction" + instructionIndex, 1);
			gameObject.renderer.enabled = false;
			gameObject.collider.enabled = false;
		}
	}

	public void ShowInstructions(int index)
	{
		instructionIndex = index;
		if (PlayerPrefs.GetInt("hasSeenInstruction" + instructionIndex) != 1)
		{
			gameObject.renderer.material = instructionMats[instructionIndex];
			gameObject.renderer.enabled = true;
			gameObject.collider.enabled = true;
		}
	}
}
