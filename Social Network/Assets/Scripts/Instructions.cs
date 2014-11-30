using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Instructions : MonoBehaviour {

	public int instructionIndex;
	public List<Material> instructionMats;
	public bool hasClickedDown = false;

	// Use this for initialization
	void Start () {
		gameObject.renderer.enabled = false;
		gameObject.collider.enabled = false;
		ShowInstructions(instructionIndex);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && gameObject.renderer.enabled)
		{
			// HACK: come up with a better system for showing a series of instructions
			if (instructionIndex == 2)
			{
				SaveData.SetInt("hasSeenInstruction" + instructionIndex, 1);
				instructionIndex = 3;
				ShowInstructions(6);
			}
			else if (instructionIndex == 6) { instructionIndex = 7; ShowInstructions(7); }
			else if (instructionIndex == 7) { instructionIndex = 8; ShowInstructions(8); }
			else if (instructionIndex == 8) { instructionIndex = 9; ShowInstructions(9); }
			else if (instructionIndex == 9) { instructionIndex = 10; ShowInstructions(10); }
			else
			{
				SaveData.SetInt("hasSeenInstruction" + instructionIndex, 1);
				gameObject.renderer.enabled = false;
				gameObject.collider.enabled = false;
			}
		}
	}

	public void ShowInstructions(int index)
	{
		instructionIndex = index;
		if (SaveData.GetInt("hasSeenInstruction" + instructionIndex) != 1)
		{
			gameObject.renderer.material = instructionMats[instructionIndex];
			gameObject.renderer.enabled = true;
			gameObject.collider.enabled = true;
		}
	}
}
