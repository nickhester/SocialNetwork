using UnityEngine;
using System.Collections;
using Types;

public class scoreTrackerOneDay : MonoBehaviour {

	public int score = 0;

	private int bonusForVeryEasy = 1;
	private int bonusForEasy = 2;
	private int bonusForMedium = 4;
	private int bonusForHard = 8;
	
	private int bonusSpecialMultiplier = 2;
	private int bonusPerfectMultiplier = 3;

	public GameObject My3dText;
	private GameObject My3dTextObject;
	private float textSize = 4.0f;
	private bool textIsDisplaying = false;
	private float startingHeight = 6.0f;
	private float endingHeight = -20.0f;
	private float currentAlphaValue = 0.0f;
	private bool alphaIsIncreasing = true;
	private float speedOfFade = 0.003f;
	private float maxFade = 0.15f;
	private float speedOfDrop = 5.0f;

	// Use this for initialization
	void Start () {
		score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (textIsDisplaying)
		{
			My3dTextObject.transform.Translate(Vector3.down * speedOfDrop * Time.deltaTime);
			if (currentAlphaValue < maxFade && alphaIsIncreasing)
			{
				currentAlphaValue += speedOfFade;
				My3dTextObject.renderer.material.color = new Color(0, 0, 0, currentAlphaValue);
			}
			else
			{
				currentAlphaValue -= speedOfFade;
				My3dTextObject.renderer.material.color = new Color(0, 0, 0, currentAlphaValue);
			}
			if (currentAlphaValue >= maxFade) { alphaIsIncreasing = false; }
			if (My3dTextObject.transform.position.y < endingHeight) { Destroy(My3dTextObject); textIsDisplaying = false; currentAlphaValue = 0; alphaIsIncreasing = true; }
		}
	}

	public void UpdateScore(Difficulty difficulty, int numActionsTaken, int numConsecutiveLevelsCompleted, bool isSpecial)
	{
		int scoreToAdd = 0;

		// add to score for difficulty
		if (difficulty == Difficulty.VeryEasy) { scoreToAdd += bonusForVeryEasy; }
		else if (difficulty == Difficulty.Easy) { scoreToAdd += bonusForEasy; }
		else if (difficulty == Difficulty.Medium) { scoreToAdd += bonusForMedium; }
		else if (difficulty == Difficulty.Hard) { scoreToAdd += bonusForHard; }

		print ("score - w/ diff bonus: " + scoreToAdd);

		// add to score for number of actions
		validLevels thisLevel = GameObject.Find("Clipboard").GetComponent<clipboard>().nextLevelUp.myLevel;

		// add score for perfect number of actions
		if (numActionsTaken <= thisLevel.numClicks) { scoreToAdd *= bonusPerfectMultiplier; }

		print ("score - w/ perfect bonus (" + numActionsTaken + "/" + thisLevel.numClicks + "):" + scoreToAdd);

		// add score for it being a special level
		if (isSpecial) { scoreToAdd *= bonusSpecialMultiplier; print ("score - w/ special bonus: " + scoreToAdd); }

		score += scoreToAdd;
		DisplayScorePopUp(scoreToAdd);
	}

	public void DisplayScorePopUp(int _displayScore)
	{
		My3dTextObject = Instantiate(My3dText, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + startingHeight, 0), Quaternion.identity) as GameObject;
		TextMesh _textComponent = My3dTextObject.GetComponent<TextMesh>();
		_textComponent.text = _displayScore.ToString();
		My3dTextObject.transform.localScale *= textSize;
		My3dTextObject.renderer.material.color = new Color(0, 0, 0, 0);
		textIsDisplaying = true;
	}
}
