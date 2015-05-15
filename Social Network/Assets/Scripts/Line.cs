using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {

	public int myState;	// 0 = none, 1 = positive, -1 = negative
	private float fadeCounter;

	private bool prop_isVisible;
	public bool isVisible
	{
		get
		{
			return prop_isVisible;
		}
		set
		{
			prop_isVisible = value;
			isChanging = true;
			fadeCounter = 0;
		}
	}

	private Color myColor;
	private bool isChanging = false;
	private float fadeTime = 0.75f;
	private Color negativeColor;
	private Color positiveColor;

	public void SetColors (Color _negativeColor, Color _positiveColor)
	{
		positiveColor = _positiveColor;
		negativeColor = _negativeColor;
	}

	void Start () {
		if (myState == 0) { GetComponent<Renderer>().enabled = false; }
		else if (myState == 1) { myColor = positiveColor; }
		else if (myState == -1) { myColor = negativeColor; }

		GetComponent<Renderer>().material.color = myColor;
		GetComponent<Renderer>().material.SetColor("_Emission", myColor);
	}

	void Update () {
		if (isChanging)
		{
			fadeCounter += Time.deltaTime;
			if (fadeCounter >= fadeTime) { isChanging = false; }
		}

		if (isVisible) { myColor = new Color(this.GetComponent<Renderer>().material.color.r, this.GetComponent<Renderer>().material.color.g, this.GetComponent<Renderer>().material.color.b, fadeCounter/fadeTime); }
		else { myColor = new Color(this.GetComponent<Renderer>().material.color.r, this.GetComponent<Renderer>().material.color.g, this.GetComponent<Renderer>().material.color.b, 0); }
		
		GetComponent<Renderer>().material.color = myColor;
		GetComponent<Renderer>().material.SetColor("_Emission", myColor);
	}

}
