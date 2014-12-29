using UnityEngine;
using System.Collections;

public class calendarProgressBar : MonoBehaviour {

	private Vector3 sessionBarStartPos;
	private Vector3 sessionBarEndPos;
	private Vector3 starBarStartPos;
	private Vector3 starBarEndPos;
	public float lineWidth;
	public float lineLength;
	public Color barColor;
	public GameObject text;

	// Use this for initialization
	void Start () {

		Calendar cal = GameObject.Find("Calendar").GetComponent<Calendar>();
		int numTotalDays = cal.Get_daysToGenerate();
		int numDaysCompleted = 0;
		int numTotalPossibleStars = 0;
		int numStarsAcquired = 0;
		
		for (int i = 0; i < numTotalDays; i++)
		{
			numTotalPossibleStars += cal.Get_dayNumAppointments(i) * 3;

			if (SaveGame.GetDayIsPlayable(i))
			{
				if (SaveGame.GetHasCompletedAllRoundsInDay(i))
				{
					numDaysCompleted = i + 1;
				}
				numStarsAcquired += SaveGame.GetDayStarCount(i);
			}
		}
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
		barSessionsComplete.renderer.material = new Material(Shader.Find("Transparent/VertexLit"));
		barSessionsComplete.renderer.material.color = barColor;
		barSessionsComplete.renderer.material.SetColor("_Emission", barColor);

		GameObject barStarsComplete = LineGeneric.CreateLineMesh(starBarStartPos, starBarEndPos, lineWidth, 0.0f, 0.0f);
		barStarsComplete.renderer.material = new Material(Shader.Find("Transparent/VertexLit"));
		barStarsComplete.renderer.material.color = barColor;
		barStarsComplete.renderer.material.SetColor("_Emission", barColor);

		// show game completion notes ("instruction" pages)
		if (numDaysCompleted == numTotalDays)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstruction(15, false);
		}
		if (numStarsAcquired == numTotalPossibleStars)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstruction(16, false);
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
		myTextStarsComponent.text = numStarsAcquired + "/" + numTotalPossibleStars;;
	}
}
