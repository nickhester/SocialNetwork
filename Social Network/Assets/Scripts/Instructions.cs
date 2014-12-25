using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Instructions : MonoBehaviour {

	public List<int> instructionsToShow = new List<int>();
	public List<Material> instructionMats = new List<Material>();
	public bool hasClickedDown = false;
	private bool hasBeenDisplayingForOneFrame = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (gameObject.renderer.enabled && Input.GetMouseButtonDown(0) && hasBeenDisplayingForOneFrame)
		{
			if (instructionsToShow.Count != 0)
			{
				DisplayNextInstruction(true);
			}
			else
			{
				gameObject.renderer.enabled = false;
				gameObject.collider.enabled = false;
			}
		}

		// make sure not to take a click from something else
		if (gameObject.renderer.enabled)
			hasBeenDisplayingForOneFrame = true;
	}

	public void ShowInstruction(int index, bool forceShow)
	{
		instructionsToShow.Clear();
		instructionsToShow.Add(index);
		DisplayNextInstruction(forceShow);
	}

	private void DisplayNextInstruction(bool forceShow)
	{
		bool hasSeenThisOne = SaveGame.GetSeenInstruction(instructionsToShow[0]);
		if (forceShow || !hasSeenThisOne)
		{
			gameObject.renderer.material = instructionMats[instructionsToShow[0]];
			gameObject.renderer.enabled = true;
			gameObject.collider.enabled = true;
			hasBeenDisplayingForOneFrame = false;
		}
		if (!forceShow)
		{
			SaveGame.SetSeenInstruction(instructionsToShow[0], true);
		}
		instructionsToShow.RemoveAt(0);
	}

	public void ShowInstructionSeries(List<int> indicies, bool forceShow)
	{
		instructionsToShow.Clear();
		foreach (int i in indicies)
		{
			instructionsToShow.Add(i);
		}
		DisplayNextInstruction(forceShow);
	}
}
