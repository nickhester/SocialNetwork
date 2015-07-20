using UnityEngine;
using System.Collections;

public class CalendarProgressBar : MonoBehaviour {

	private Vector3 sessionBarStartPos;
	private Vector3 sessionBarEndPos;
	private Vector3 starBarStartPos;
	private Vector3 starBarEndPos;
	[SerializeField] private float lineWidth;
	[SerializeField] private float lineLength;
	[SerializeField] private Color barColor;
	[SerializeField] private GameObject text;

	// Use this for initialization
	void Start ()
	{
		Calendar cal = GameObject.Find("Calendar").GetComponent<Calendar>();
		int numTotalDays = cal.Get_daysToGenerate();
		int numDaysCompleted = SaveGame.numDaysCompleted;
		int numTotalPossibleStars = SaveGame.numTotalPossibleStars;
		int numStarsAcquired = SaveGame.numStarsAcquired;

		float progressThroughDays = (float)numDaysCompleted/(float)numTotalDays;
		float progressThroughStars = (float)numStarsAcquired/(float)numTotalPossibleStars;

		// Start line creation

		foreach (Transform t in GetComponentsInChildren<Transform>())
		{
			if (t.name == "sessionBarMarker")
			{
				sessionBarStartPos = t.position;
				sessionBarEndPos = t.position;
				sessionBarEndPos.x += lineLength * progressThroughDays;
			}
			else if (t.name == "starBarMarker")
			{
				starBarStartPos = t.position;
				starBarEndPos = t.position;
				starBarEndPos.x += lineLength * progressThroughStars;
			}
		}

		GameObject barSessionsComplete = LineGeneric.CreateLineMesh(sessionBarStartPos, sessionBarEndPos, lineWidth, 0.0f, 0.0f);
		barSessionsComplete.GetComponent<Renderer>().material = new Material(Shader.Find("Transparent/VertexLit"));
		barSessionsComplete.GetComponent<Renderer>().material.color = barColor;
		barSessionsComplete.GetComponent<Renderer>().material.SetColor("_Emission", barColor);

		GameObject barStarsComplete = LineGeneric.CreateLineMesh(starBarStartPos, starBarEndPos, lineWidth, 0.0f, 0.0f);
		barStarsComplete.GetComponent<Renderer>().material = new Material(Shader.Find("Transparent/VertexLit"));
		barStarsComplete.GetComponent<Renderer>().material.color = barColor;
		barStarsComplete.GetComponent<Renderer>().material.SetColor("_Emission", barColor);

		// show game completion notes ("notification" pages)
		if (numDaysCompleted == numTotalDays)
		{
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(15);
		}
		if (numStarsAcquired == numTotalPossibleStars)
		{
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(16);
		}

		// Create text
		GameObject myTextSessions = Instantiate(text, new Vector3(transform.position.x + 2.4f, transform.position.y + 0.5f, transform.position.z - 0.1f), Quaternion.identity) as GameObject;
		myTextSessions.transform.localScale = myTextSessions.transform.localScale * 0.033f;
		myTextSessions.transform.parent = gameObject.transform;
		TextMesh myTextSessionsComponent = myTextSessions.GetComponent<TextMesh>();
		myTextSessionsComponent.anchor = TextAnchor.MiddleRight;
		myTextSessionsComponent.text = numDaysCompleted + "/" + numTotalDays;

		GameObject myTextStars = Instantiate(text, new Vector3(transform.position.x + 2.4f, transform.position.y - 0.2f, transform.position.z - 0.1f), Quaternion.identity) as GameObject;
		myTextStars.transform.localScale = myTextStars.transform.localScale * 0.033f;
		myTextStars.transform.parent = gameObject.transform;
		TextMesh myTextStarsComponent = myTextStars.GetComponent<TextMesh>();
		myTextStarsComponent.anchor = TextAnchor.MiddleRight;
		myTextStarsComponent.text = numStarsAcquired + "/" + numTotalPossibleStars;
	}
}
