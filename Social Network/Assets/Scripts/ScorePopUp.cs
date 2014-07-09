using UnityEngine;
using System.Collections;

public class ScorePopUp : MonoBehaviour {

	private GameObject My3dText;
	private GameObject My3dTextObject;
	private float textSize = 4.0f;
	private bool textIsDisplaying = false;
	private float startingHeight = 6.0f;
	private float endingHeight = -20.0f;
	private float currentAlphaValue = 0.0f;
	private bool alphaIsIncreasing = true;
	private float speedOfFade = 0.005f;
	private float maxFade = 0.20f;
	private float speedOfDrop = 5.0f;

	// Use this for initialization
	void Start () {

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

	public void DisplayScorePopUp(string _displayScore, int _textSizeOffset, int _heightOffset)
	{
		textSize += _textSizeOffset;
		startingHeight += _heightOffset;
		endingHeight += _heightOffset;

		My3dText = Resources.Load<GameObject>("3DText");
		My3dTextObject = Instantiate(My3dText, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + startingHeight, 0), Quaternion.identity) as GameObject;
		TextMesh _textComponent = My3dTextObject.GetComponent<TextMesh>();
		_textComponent.text = _displayScore.ToString();
		My3dTextObject.transform.localScale *= textSize;
		My3dTextObject.renderer.material.color = new Color(0, 0, 0, 0);
		textIsDisplaying = true;
	}

	public void DisplayScorePopUp(string _displayScore)
	{
		DisplayScorePopUp(_displayScore, 0, 0);
	}
}
