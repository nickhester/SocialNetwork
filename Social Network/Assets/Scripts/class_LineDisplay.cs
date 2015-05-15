using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class class_LineDisplay : MonoBehaviour {
	
	private Dictionary<Person, List<GameObject>> lineConnections = new Dictionary<Person, List<GameObject>>();
	private float lineWidth = 0.1f;
	private bool linesAreInvisible = false;
	private float fadeCounter = 0.0f;

	public Color posColor;
	public Color negColor;
	
	#region StartAndUpdate

	void Start ()
	{
		foreach (Person person in GetComponentsInChildren<Person>())	// for each person
		{
			List<GameObject> _lineList = new List<GameObject>();
			foreach (class_Relationship rel in person.relationshipList)	// for each of that person's relationships
			{
				GameObject _lineObject = LineGeneric.CreateLineMesh(
					rel.relationshipMembers[0].GetComponent<personMovement>().originalPos,
					rel.relationshipMembers[1].GetComponent<personMovement>().originalPos,
					lineWidth,
					0.0f,
					3.0f); // create line using mesh method

				string _relName = "rel with " + rel.relationshipMembers[0].name + " & " + rel.relationshipMembers[1].name;
				_lineObject.gameObject.name = _relName;
				_lineObject.transform.parent = transform;					// puts the relationship line objects under the network manager
				_lineObject.GetComponent<Renderer>().enabled = true;
				_lineObject.GetComponent<Renderer>().material = new Material(Shader.Find("Transparent/VertexLit"));
				_lineObject.tag = "relationshipLines";

				// figure out state for Line component colors
				int relState;
				if (rel.m_Friendship == Types.Friendship.Positive) { relState = 1; }
				else if (rel.m_Friendship == Types.Friendship.Neutral) { relState = 0; }
				else { relState = -1; }

				Line thisLine = _lineObject.AddComponent<Line>();
				thisLine.myState = relState;
				thisLine.SetColors(negColor, posColor);
				_lineList.Add(_lineObject);
			}
			lineConnections[person] = _lineList;
		}
		if (GameObject.Find("Clipboard").GetComponent<clipboard>().nextLevelUp.myLevel.isNoLines) { linesAreInvisible = true; }
	}
	
	// Update is called once per frame
	void Update ()
	{
		fadeCounter += Time.deltaTime;
	}

	#endregion

	public void TurnOffAllLines()
	{
		foreach (var list in lineConnections) {
			foreach (var line in list.Value) {
				line.GetComponent<Line>().isVisible = false;
			}
		}
	}

	public void DisplayLines(Person _person)
	{
		if (!linesAreInvisible)
		{
			foreach (GameObject line in lineConnections[_person])
			{
				line.GetComponent<Line>().isVisible = true;
			}
		}
	}
}
