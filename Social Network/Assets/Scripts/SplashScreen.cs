﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour {

	private float countDownTimer = 3.0f;
	private float fadeOutSpeed = 0.5f;
	private Light light;
	public GameObject overlayObject;
	private Image overlay;

	void Start ()
	{
		light = GameObject.FindObjectOfType<Light>();
		overlay = overlayObject.GetComponent<Image>();
	}

	void Update ()
	{
		if (countDownTimer < 0.0f)
		{
			StartCoroutine("FadeToBlack");
		}
		else
		{
			countDownTimer -= Time.deltaTime;
		}
	}

	IEnumerator FadeToBlack()
	{
		while (true)
		{
			float value = overlay.color.a;
			value += Time.deltaTime * fadeOutSpeed;
			overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, value);
			if (overlay.color.a >= 1.0f)
			{
				break;
			}
			print(overlay.color.a);
			yield return null;
		}

		print("LOADING LEVEL");
		Application.LoadLevel("Scene_MainMenu");
	}
}
