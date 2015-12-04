using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TextBubble : MonoBehaviour {

	[SerializeField] private Image chatBubble;

	private string[] textGroup_positive = new string[]
	{
		"I appreciate that",
		"Thank you",
		"Do you really mean that?",
		"You're just saying that",
		"Aww, shucks",
		"You just made my day",
		"Worth every penny",
		"Am I your favorite?",
		"My glass is half full now",
		"You're the best",
		"I'm glad I got up this morning",
		"Yippee!",
		"What a kind soul",
		"Likewise",
		"Gracias",
		"You inspire me",
		"Gee whiz",
		"I feel warm inside"
	};
	private string[] textGroup_negative = new string[]
	{
		"I thought I could trust you",
		"Why am I paying you for this?",
		"Maybe I don't get your humor",
		"You don't really mean that",
		"It's all meaningless",
		"What kind of advice is that?",
		"Can I see your credentials?",
		"How dare you",
		"This is not going my way",
		"Sticks and stones... nevermind",
		"Why?",
		"You're cruel",
		"Is this for the greater good?"
	};
	private string[] textGroup_bored = new string[]
	{
		"Should I go and come back?",
		"Am I paying for this time?",
		"Are we still on the clock?",
		"I think I just fell asleep",
		"Make me happy please",
		"Are you still there?",
		"You just take your time",
		"Don't let me rush you",
		"I'm getting veeery sleeepy",
		"Can I just get some pills?",
		"Are we there yet?"
	};

	private List<Person> people;
	private float bubbleShowProbability = 0.22f;

	private float scalePositionTowardCenter = 0.75f;
	private float verticalCenter = 2.5f;
	private float verticalDistanceFromPerson = 1.25f;
	private int defaultFontSize;
	private int smallTextFontIncrease = 4;

	private bool isBubbleVisible = false;
	private float visibilityDuration = 2.8f;
	private float visibilityCounter = 0.0f;

	private float showBoredTextDuration = 10.0f;
	private float showBoredTextCounter = 0.0f;
	private float showBoredTextProbability = 0.4f;
	
	public void Init (List<Person> _people)
	{
		people = _people;
	}

	void Start ()
	{
		defaultFontSize = chatBubble.transform.GetChild(0).GetComponent<Text>().fontSize;
	}

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

		showBoredTextCounter += Time.deltaTime;
		if (showBoredTextCounter > showBoredTextDuration)
		{
			Person _personToShow = people[Random.Range(0, people.Count)];
			ShowBubble(_personToShow.transform.position, textGroup_bored, showBoredTextProbability);
		}
	}

	public void ShowBubble(Vector3 _positionOfPerson, string[] textOptions, float _showProbability)
	{
		if (Random.Range(0.0f, 1.0f) < _showProbability)
		{
			// move position toward center by amount
			Vector3 _posForBubble = _positionOfPerson;
			_posForBubble.y += (_positionOfPerson.y > verticalCenter ? -verticalDistanceFromPerson : verticalDistanceFromPerson);
			_posForBubble.x *= scalePositionTowardCenter;

			chatBubble.gameObject.transform.position = _posForBubble;
			chatBubble.gameObject.SetActive(true);

			string textToDisplay = (textOptions[Random.Range(0, textOptions.Length)]);
			Text t = chatBubble.GetComponentInChildren<Text>();
			t.text = textToDisplay;
			if (t.text.Length < 20)
			{
				t.fontSize = defaultFontSize + smallTextFontIncrease;
			}
			else
			{
				t.fontSize = defaultFontSize;
			}
			
			isBubbleVisible = true;
			visibilityCounter = 0.0f;
		}
		showBoredTextCounter = 0.0f;
	}

	public void ShowBubble(Vector3 _positionOfPerson, bool _isPositive)
	{
		if (_isPositive)
		{
			ShowBubble(_positionOfPerson, textGroup_positive, bubbleShowProbability);
		}
		else
		{
			ShowBubble(_positionOfPerson, textGroup_negative, bubbleShowProbability);
		}
	}

	public void HideBubble()
	{
		chatBubble.gameObject.SetActive(false);
		isBubbleVisible = false;
	}
}
