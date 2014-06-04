using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class class_LineDisplay : MonoBehaviour {
	
	private Dictionary<Person, List<GameObject>> lineConnections = new Dictionary<Person, List<GameObject>>();
	private float lineWidth = 0.1f;

	private bool linesAreInvisible = false;

	private float fadeCounter = 0.0f;
	
	#region StartAndUpdate

	void Start ()
	{
		foreach (Person person in GetComponentsInChildren<Person>())	// for each person
		{
			if (person != person.player)	// ...that isn't the player
			{
				List<GameObject> _lineList = new List<GameObject>();
				foreach (class_Relationship rel in person.relationshipList)	// for each of that person's relationships
				{
					if (!rel.relationshipMembers.Contains(person.player))		// ...that isn't with the player
					{
						GameObject _lineObject = CreateLineMesh(
							rel.relationshipMembers[0].GetComponent<personMovement>().originalPos,
							rel.relationshipMembers[1].GetComponent<personMovement>().originalPos,
							lineWidth,
							0.0f,
							3.0f); // create line using mesh method

						string _relName = "rel with " + rel.relationshipMembers[0].name + " & " + rel.relationshipMembers[1].name;
						_lineObject.gameObject.name = _relName;
						_lineObject.transform.parent = transform;					// puts the relationship line objects under the network manager
						_lineObject.renderer.enabled = true;
						_lineObject.renderer.material = new Material(Shader.Find("Transparent/VertexLit"));
						_lineObject.tag = "relationshipLines";

						// figure out state for Line component colors
						int relState;
						if (rel.relationshipValue > 0) { relState = 1; }
						else if (rel.relationshipValue == 0) { relState = 0; }
						else { relState = -1; }

						Line thisLine = _lineObject.AddComponent<Line>();
						thisLine.myState = relState;
						_lineList.Add(_lineObject);
					}
				}
				lineConnections[person] = _lineList;
			}
		}
		if (GameObject.Find("Clipboard").GetComponent<clipboard>().nextLevelUp.myLevel.isNoLines) { linesAreInvisible = true; print ("set lines to off"); }
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

	// Use a mesh for a line instead of LineRenderer, since that is buggy
	GameObject CreateLineMesh(Vector3 p1, Vector3 p2, float width, float distFromEnds, float distDepth)
	{
		GameObject lineMesh = new GameObject("lineMesh");
		Mesh newMesh = new Mesh();
		lineMesh.AddComponent("MeshFilter");
		lineMesh.AddComponent("MeshRenderer");

		Vector3 widthDirection = (Vector3.Cross (p1 - p2, Vector3.back).normalized) * width;
		Vector3 depthDirection = Vector3.forward;

		// Set positions of verts
		Vector3 backBottom = (p1 - distFromEnds*(p1 - p2)) + widthDirection + depthDirection*distDepth;
		Vector3 backTop = (p1 - distFromEnds*(p1 - p2)) - widthDirection + depthDirection*distDepth;
		Vector3 frontBottom = (p2 - distFromEnds*(p2 - p1)) + widthDirection + depthDirection*distDepth;
		Vector3 frontTop = (p2 - distFromEnds*(p2 - p1)) - widthDirection + depthDirection*distDepth;
		
		Vector3[] verts = new Vector3[4] { backBottom, backTop, frontBottom, frontTop };
		newMesh.vertices = verts;
		
		// Set the UVs
		Vector2[] uvs = new Vector2[newMesh.vertices.Length];
		for (int i = 0; i < uvs.Length; i++)
		{
			uvs[i] = new Vector2(newMesh.vertices[i].x, newMesh.vertices[i].z);
		}
		newMesh.uv = uvs;
		
		int[] triVerts = new int[6]	{ 0,1,2,2,1,3 };
		
		newMesh.triangles = triVerts;
		newMesh.RecalculateNormals();
		
		lineMesh.GetComponent<MeshFilter>().mesh = newMesh;

		return lineMesh;
	}
}
