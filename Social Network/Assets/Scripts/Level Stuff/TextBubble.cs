using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TextBubble : MonoBehaviour {

	[SerializeField] private Image chatBubble;

	private string[] textGroup_positive = new string[]
	{
		"positiveComment01",
		"positiveComment02",
		"positiveComment03",
		"positiveComment04",
		"positiveComment05",
		"positiveComment06",
		"positiveComment07",
		"positiveComment08",
		"positiveComment09",
		"positiveComment10",
		"positiveComment11",
		"positiveComment12",
		"positiveComment13",
		"positiveComment14",
		"positiveComment15",
		"positiveComment16",
		"positiveComment17",
		"positiveComment18"
	};
	private string[] textGroup_negative = new string[]
	{
		"negativeComment01",
		"negativeComment02",
		"negativeComment03",
		"negativeComment04",
		"negativeComment05",
		"negativeComment06",
		"negativeComment07",
		"negativeComment08",
		"negativeComment09",
		"negativeComment10",
		"negativeComment11",
		"negativeComment12",
		"negativeComment13"
	};
	private string[] textGroup_bored = new string[]
	{
		"boredComment01",
		"boredComment02",
		"boredComment03",
		"boredComment04",
		"boredComment05",
		"boredComment06",
		"boredComment07",
		"boredComment08",
		"boredComment09",
		"boredComment10",
		"boredComment11"
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

			LocalizedTextManager localizedTextManager = FindObjectOfType<LocalizedTextManager>();
			textToDisplay = localizedTextManager.GetLocalizedString(textToDisplay);

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
