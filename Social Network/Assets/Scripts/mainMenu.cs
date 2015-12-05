using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class MainMenu : MonoBehaviour {

	private GameObject clickedThisGO;
	private float clickScale;
	private Vector3 originalScale;
	private Vector3 cameraStartingPos;
	private Vector3 cameraOptionsPos;
    private float cameraOptionsPosOffset = 19.2f;
	private bool isLerpingTowardOptions = false;
	private bool isClickingButton = false;
	private bool backgroundIsSplitting = false;
	private GameObject rh;
	private GameObject lh;
	private GameObject mainMenuParent;
	private GameObject mainTitle;
	private Vector3 mainTitleOriginalPosition;
    private float mainTitleDistanceStartUp = 10.0f;
	private bool isMainTitleLerping = true;
	private float mainTitleSinSpeed = 1.25f;
	private float mainTitleSinSize = 0.16f;
	private float sinWaveCounter = 0.0f;

	private float splitSpeed = 13.0f;
	private float splitCounter = 0.0f;
	private float splitLimit = 2.0f;

	private string versionText = "Version 3.2";

	[SerializeField] private Material confirmClearProgressImage;
    [SerializeField] private Material progressClearedImage;
    [SerializeField] private GameObject text;

    [SerializeField] private Material audio_sfx;
    [SerializeField] private Material audio_music;
    [SerializeField] private Material audio_off;

	// external API stuff
	[HideInInspector]
	public bool isUsingExternalAPI;

    void Awake ()
    {
        InputManager.Instance.OnClick += OnClick;
    }

    void OnClick(GameObject go)
    {
        if (go.name == "button_Start")
		{
			SplitBackground();
		}
        else if (go.name == "button_options")
		{
			isLerpingTowardOptions = true;
		}
        else if (go.name == "button_back")
		{
			GoBack();
		}
        else if (go.name == "button_clearProgress")
		{
			go.GetComponent<Renderer>().material = confirmClearProgressImage;
			go.name = "button_clearProgressConfirm";
		}
        else if (go.name == "button_clearProgressConfirm")
		{
			go.GetComponent<Renderer>().material = progressClearedImage;
			go.name = "progressCleared";
			SaveGame.DeleteAll();
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().DeleteCloudSave();
			MetricsLogger.Instance.LogCustomEvent("Settings", "ClearGameProgress", "");
		}
		else if (go.name == "button_leaderboard")
		{
			if (!GooglePlayAPI.isPlayingOffline)
			{
				// show leaderboard UI
				Social.ShowLeaderboardUI();
			}
		}

        // AUDIO
        else if (go.name == "button_sfx")
        {
            if (SaveGame.GetAudioOn_sfx())
            {
                SaveGame.SetAudioOn_sfx(false);
                go.GetComponent<Renderer>().material = audio_off;
				MetricsLogger.Instance.LogCustomEvent("Settings", "TurnOffSFX", "MainMenu");
            }
            else
            {
                SaveGame.SetAudioOn_sfx(true);
                go.GetComponent<Renderer>().material = audio_sfx;
				MetricsLogger.Instance.LogCustomEvent("Settings", "TurnOnSFX", "MainMenu");
            }
        }
        else if (go.name == "button_music")
        {
            if (SaveGame.GetAudioOn_music())
            {
                TurnMusicOff();
                SaveGame.SetAudioOn_music(false);
                go.GetComponent<Renderer>().material = audio_off;
				MetricsLogger.Instance.LogCustomEvent("Settings", "TurnOffMusic", "MainMenu");
            }
            else
            {
                TurnMusicOn();
                SaveGame.SetAudioOn_music(true);
                go.GetComponent<Renderer>().material = audio_music;
				MetricsLogger.Instance.LogCustomEvent("Settings", "TurnOnMusic", "MainMenu");
            }
        }
        else if (go.name == "button_viewCredits")
        {
            GameObject.Find("NotificationManager").GetComponent<NotificationManager>().DisplayNotification(17, true);
        }
    }

	// Use this for initialization
	void Start () {

		clickScale = 0.9f;
		cameraStartingPos = Camera.main.transform.position;
        cameraOptionsPos = new Vector3(cameraStartingPos.x, cameraStartingPos.y - cameraOptionsPosOffset, cameraStartingPos.z);

		rh = GameObject.Find("Main Menu background rightHalf");
		lh = GameObject.Find("Main Menu background leftHalf");
		mainMenuParent = GameObject.Find("Main Menu");
		mainTitle = GameObject.Find("mainTitle");
		mainTitleOriginalPosition = mainTitle.transform.position;
        mainTitle.transform.position = new Vector3(mainTitleOriginalPosition.x, mainTitleOriginalPosition.y + mainTitleDistanceStartUp, mainTitleOriginalPosition.z);

		// display software version number
		GameObject versionTextObject = Instantiate(text, new Vector3(2.5f, -26.4f, -1.0f), Quaternion.identity) as GameObject;
		versionTextObject.transform.localScale = versionTextObject.transform.localScale * 0.04f;
		versionTextObject.transform.parent = gameObject.transform;
		TextMesh myTextComponent = versionTextObject.GetComponent<TextMesh>();
		myTextComponent.text = versionText;

		// make sure audio icons check are accurately on or off
		if (SaveGame.GetAudioOn_music() == false)
			GameObject.Find("button_music").GetComponent<Renderer>().material = audio_off;
		if (SaveGame.GetAudioOn_sfx() == false)
			GameObject.Find("button_sfx").GetComponent<Renderer>().material = audio_off;

		SaveGame.lastCalendarDayClicked = -1;	// reset lastdayclicked counter
	}

	GameObject getObjectAtMouse()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 10.0f))
		{
			if (hit.transform.name.StartsWith("button_"))
			{
				return hit.transform.gameObject;
			}
		}
		return null;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			clickedThisGO = getObjectAtMouse();
			if (clickedThisGO != null)
			{
				DepressAButton(clickedThisGO);
			}
		}
        
		if (Input.GetMouseButtonUp(0) && clickedThisGO != null)
		{
			
			isClickingButton = false;
			UnpressAButton(clickedThisGO);
		}
        
		if (isLerpingTowardOptions)
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraOptionsPos, 0.1f);
		}
		else
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraStartingPos, 0.1f);
		}

		if (backgroundIsSplitting)
		{
			rh.transform.Translate(Vector3.right * splitSpeed * Time.deltaTime);
			lh.transform.Translate(Vector3.left * splitSpeed * Time.deltaTime);
			mainMenuParent.transform.Translate(Vector3.left * splitSpeed * Time.deltaTime);
			splitCounter += Time.deltaTime;

			if (splitCounter >= splitLimit / 2.0f)
			{
				Camera.main.backgroundColor = new Color(Camera.main.backgroundColor.r - 0.05f, Camera.main.backgroundColor.g - 0.05f, Camera.main.backgroundColor.b - 0.05f);
			}
			if (splitCounter >= splitLimit)
			{
				Application.LoadLevel("Scene_Calendar");
			}
		}

		// main title movement
		mainTitle.transform.position = Vector3.Lerp(mainTitle.transform.position, mainTitleOriginalPosition, 0.1f);			// lerp down into position
		mainTitle.transform.position = new Vector3(mainTitleOriginalPosition.x, mainTitle.transform.position.y, mainTitle.transform.position.z);	// keep centered
		if (Mathf.Abs(mainTitle.transform.position.y - mainTitleOriginalPosition.y) < 0.005f)
		{
			isMainTitleLerping = false;
		}
		if (!isMainTitleLerping)
		{
			mainTitle.transform.position = new Vector3(
				mainTitleOriginalPosition.x,
				mainTitleOriginalPosition.y + Mathf.Sin(sinWaveCounter * mainTitleSinSpeed) * mainTitleSinSize,
				mainTitleOriginalPosition.z);
			sinWaveCounter += Time.deltaTime;
		}

		if (Input.GetButtonDown("Cancel"))
		{
			GoBack();
		}
	}

	void SplitBackground()
	{
		backgroundIsSplitting = true;
		foreach (MeshCollider col in mainMenuParent.GetComponentsInChildren<MeshCollider>())
		{
			col.enabled = false;
		}
	}

	void DepressAButton(GameObject hit)
	{
		if (!isClickingButton) { originalScale = hit.transform.localScale; isClickingButton = true; }
		hit.transform.localScale = originalScale * clickScale;
	}

	void UnpressAButton(GameObject hit)
	{
		hit.transform.localScale = originalScale;
	}

	void TurnMusicOff()
	{
		AudioSource audio = GameObject.Find("Music Player").GetComponent<AudioSource>();
		audio.enabled = false;
	}

	void TurnMusicOn()
	{
		AudioSource m_audio = GameObject.Find("Music Player").GetComponent<AudioSource>();
		m_audio.enabled = true;
		m_audio.Play();
	}

	void GoBack()
	{
		isLerpingTowardOptions = false;
	}
}
