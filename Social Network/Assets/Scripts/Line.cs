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
	
	void Start () {
		if (myState == 0) { renderer.enabled = false; }
		else if (myState == 1) { myColor = new Color(0, 1, 0); }
		else if (myState == -1) { myColor = new Color(1, 0, 0); }

		renderer.material.color = myColor;
		renderer.material.SetColor("_Emission", myColor);
	}

	void Update () {
		if (isChanging)
		{
			fadeCounter += Time.deltaTime;
			if (fadeCounter >= fadeTime) { isChanging = false; }
		}

		if (isVisible) { myColor = new Color(this.renderer.material.color.r, this.renderer.material.color.g, this.renderer.material.color.b, fadeCounter/fadeTime); }
		else { myColor = new Color(this.renderer.material.color.r, this.renderer.material.color.g, this.renderer.material.color.b, 0); }
		
		renderer.material.color = myColor;
		renderer.material.SetColor("_Emission", myColor);
	}

}
