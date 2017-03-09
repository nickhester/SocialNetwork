using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	private Vector3 paperStartingPos;
	private Vector3 paperOptionsPos;
    private float paperPosOffset = 15.0f;
	private bool isShowingOptions = false;

	[SerializeField] private GameObject optionsScreen;
	[SerializeField] private GameObject paperObject;

	// external API stuff
	[HideInInspector]
	public bool isUsingExternalAPI;

    void Awake ()
    {
        InputManager.Instance.OnClick += OnClick;
    }

    void OnClick(GameObject go)
    {
        if (go.name == "Button - play")
		{
			SceneManager.LoadScene("Scene_Calendar");
		}
        else if (go.name == "Button - options")
		{
			ShowOptions(true);
		}
        else if (go.name == "Button - back")
		{
			GoBack();
		}
		else if (go.name == "Button - Restore Purchases")
		{
			// restore purchases explicitly
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().RestoreTransactions();
		}
        else if (go.name == "Button - Clear Progress")
		{
			GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(25, true);
		}
		else if (go.name == "Button Yes")
		{
			SaveGame.DeleteAll();
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().DeleteCloudSave();
			MetricsLogger.Instance.LogCustomEvent("Settings", "ClearGameProgress", "");
		}
		else if (go.name == "Button - Leaderboard")
		{
			if (!GameObject.FindObjectOfType<GameManager>().gameService.GetIsPlayingOffline())
			{
				// show leaderboard UI
				Social.ShowLeaderboardUI();
			}
		}

        else if (go.name == "Button - view credits")
        {
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(17, true);
        }

		else if (go.name == "Button - Language")
		{
			FindObjectOfType<LocalizedTextManager>().ToggleLanguageAndUpdateText();
		}
		else if (go.name == "Button - privacy policy")
		{
			Application.OpenURL("http://hestergames.blogspot.com/2017/03/privacy-policy-social-sessions-game.html");
		}
	}

	public void ReceiveClickFromUIButton(GameObject _go)
	{
		InputManager.Instance.SendMouseClick(_go);
	}

	void Start () {

		// set up paper movement
		paperStartingPos = paperObject.transform.position;
        paperOptionsPos = new Vector3(paperStartingPos.x, paperStartingPos.y - paperPosOffset, paperStartingPos.z);
		paperObject.transform.position = paperOptionsPos;

		// display software version number
		/*
		GameObject versionTextObject = Instantiate(text, new Vector3(2.5f, -26.4f, -1.0f), Quaternion.identity) as GameObject;
		versionTextObject.transform.localScale = versionTextObject.transform.localScale * 0.04f;
		versionTextObject.transform.parent = gameObject.transform;
		TextMesh myTextComponent = versionTextObject.GetComponent<TextMesh>();
		myTextComponent.text = versionText;
		*/

		SaveGame.lastCalendarDayClicked = -1;	// reset lastdayclicked counter
	}

	void Update ()
	{
		if (isShowingOptions)
		{
			// move paper down
			paperObject.transform.position = Vector3.Lerp(paperObject.transform.position, paperOptionsPos, 0.1f);
		}
		else
		{
			// move paper into place
			paperObject.transform.position = Vector3.Lerp(paperObject.transform.position, paperStartingPos, 0.1f);
		}

		if (Input.GetButtonDown("Cancel"))
		{
			if (isShowingOptions)
			{
				GoBack();
			}
			else
			{
				Application.Quit();
			}
		}
	}

	void ShowOptions(bool b)
	{
		if (b)
		{
			optionsScreen.SetActive(true);
			isShowingOptions = true;
		}
		else
		{
			optionsScreen.SetActive(false);
			isShowingOptions = false;
		}
	}

	void GoBack()
	{
		ShowOptions(false);
	}
}
