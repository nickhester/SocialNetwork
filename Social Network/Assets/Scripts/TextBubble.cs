using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBubble : MonoBehaviour {

	[SerializeField] private Image chatBubble;

	private string[] textGroup_positive = new string[]
	{
		"I appreciate that",
		"Thank you",
		"Do you really mean that?"
	};
	private string[] textGroup_negative = new string[]
	{
		"I thought I could trust you",
		"Why am I paying you for this?",
		"Maybe I don't get your humor"
	};

	private float bubbleShowProbability = 0.25f;

	private float scalePositionTowardCenter = 0.75f;
	private float verticalCenter = 2.5f;
	private float verticalDistanceFromPerson = 1.5f;

	private bool isBubbleVisible = false;
	private float visibilityDuration = 2.5f;
	private float visibilityCounter = 0.0f;
	
	void Update ()
	{
		if (isBubbleVisible)
		{
			visibilityCounter += Time.deltaTime;
			if (visibilityCounter > visibilityDuration)
			{
				HideBubble();
			}
		}
	}

	public void ShowBubble(Vector3 _positionOfPerson, bool _isPositive)
	{
		if (Random.Range(0.0f, 1.0f) < bubbleShowProbability)
		{
			// move position toward center by amount
			Vector3 _posForBubble = _positionOfPerson;
			_posForBubble.y += (_positionOfPerson.y > verticalCenter ? -verticalDistanceFromPerson : verticalDistanceFromPerson);
			_posForBubble.x *= scalePositionTowardCenter;

			chatBubble.gameObject.transform.position = _posForBubble;
			chatBubble.gameObject.SetActive(true);

			string textToDisplay = (_isPositive ? textGroup_positive[Random.Range(0, textGroup_positive.Length)] : textGroup_negative[Random.Range(0, textGroup_negative.Length)]);
			chatBubble.GetComponentInChildren<Text>().text = textToDisplay;

			isBubbleVisible = true;
			visibilityCounter = 0.0f;
		}
	}

	public void HideBubble()
	{
		chatBubble.gameObject.SetActive(false);
		isBubbleVisible = false;
	}
}
