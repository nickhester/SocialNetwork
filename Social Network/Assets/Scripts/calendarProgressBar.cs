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

	// Use this for initialization
	void Start () {

		calendar cal = GameObject.Find("Calendar").GetComponent<calendar>();
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

		GameObject barStarsComplete = LineGeneric.CreateLineMesh(starBarStartPos, starBarEndPos, lineWidth, 0.0f, 0.0f);
		barStarsComplete.renderer.material = new Material(Shader.Find("Transparent/VertexLit"));
		barStarsComplete.renderer.material.color = barColor;

		// show game completion notes ("instruction" pages)
		if (numDaysCompleted == numTotalDays)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructions(15);
		}
		if (numStarsAcquired == numTotalPossibleStars)
		{
			GameObject.Find("instructions").GetComponent<Instructions>().ShowInstructions(16);
		}
	}
}
