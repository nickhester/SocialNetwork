using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour {

	private float countDownTimer = 3.0f;
	private float fadeOutSpeed = 3.5f;
	public GameObject overlayObject;
	private Image overlay;
	private bool hasStartedFade = false;

	void Start ()
	{
		overlay = overlayObject.GetComponent<Image>();
	}

	void Update ()
	{
		if (!hasStartedFade && !Application.isShowingSplashScreen)
		{
			if (countDownTimer < 0.0f)
			{
				hasStartedFade = true;
				StartCoroutine("FadeToBlack");
			}
			else
			{
				countDownTimer -= Time.deltaTime;
			}
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
			yield return null;
		}

		Application.LoadLevel("Scene_MainMenu");
	}
}
