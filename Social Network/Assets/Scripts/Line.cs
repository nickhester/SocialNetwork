using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {

	private Renderer m_renderer;
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
	private const float fadeTime = 0.75f;
	private Color negativeColor;
	private Color positiveColor;
	private const float fadedAlphaValue = 0.0f;

	public void SetColors (Color _negativeColor, Color _positiveColor)
	{
		positiveColor = _positiveColor;
		negativeColor = _negativeColor;
	}

	void Start () {
		m_renderer = GetComponent<Renderer>();
		if (myState == 0) { m_renderer.enabled = false; }
		else if (myState == 1) { myColor = positiveColor; }
		else if (myState == -1) { myColor = negativeColor; }

		m_renderer.material.color = myColor;
		m_renderer.material.SetColor("_Emission", myColor);
	}

	void Update () {
		if (isChanging)
		{
			fadeCounter += Time.deltaTime;
			if (fadeCounter >= fadeTime)
			{
				isChanging = false;
			}

			if (isVisible)
			{
				myColor = new Color(m_renderer.material.color.r, m_renderer.material.color.g, m_renderer.material.color.b, (fadeCounter / fadeTime) + fadedAlphaValue);
			}
			else
			{
				myColor = new Color(m_renderer.material.color.r, m_renderer.material.color.g, m_renderer.material.color.b, fadedAlphaValue);
			}
			m_renderer.material.color = myColor;
			m_renderer.material.SetColor("_Emission", myColor);
		}
	}

}
