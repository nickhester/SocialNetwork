using UnityEngine;
using System.Collections;

public class mainMenu : MonoBehaviour {

	private GameObject clickedThisGO;
	private float clickScale;
	private Vector3 originalScale;
	private Vector3 cameraStartingPos;
	private Vector3 cameraOptionsPos;
	private bool isLerpingTowardOptions = false;
	private bool isClickingButton = false;
	private bool backgroundIsSplitting = false;
	private GameObject rh;
	private GameObject lh;
	private GameObject mainMenuParent;
	private GameObject mainTitle;
	private Vector3 mainTitleOriginalPosition;
	private bool isMainTitleLerping = true;
	private float mainTitleSinSpeed = 1.25f;
	private float mainTitleSinSize = 0.1f;
	private float sinWaveCounter = 0.0f;

	private float splitSpeed = 9.0f;
	private float splitCounter = 0.0f;
	private float splitLimit = 2.0f;
	public Material confirmClearProgressImage;
	public Material progressClearedImage;

	// Use this for initialization
	void Start () {
		clickScale = 0.9f;
		cameraStartingPos = Camera.main.transform.position;
		cameraOptionsPos = new Vector3(cameraStartingPos.x, cameraStartingPos.y - 12.0f, cameraStartingPos.z);

		rh = GameObject.Find("Main Menu background rightHalf");
		lh = GameObject.Find("Main Menu background leftHalf");
		mainMenuParent = GameObject.Find("Main Menu");
		mainTitle = GameObject.Find("mainTitle");
		mainTitleOriginalPosition = mainTitle.transform.position;
		mainTitle.transform.position = new Vector3(mainTitleOriginalPosition.x, mainTitleOriginalPosition.y + 6.0f, mainTitleOriginalPosition.z);
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButton(0))		// when you left click
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 10.0f))
			{
				if (hit.transform.name.StartsWith("button_"))
				{
					ClickAButton(hit.transform.gameObject);
					clickedThisGO = hit.transform.gameObject;
				}
			}
		}

		if (Input.GetMouseButtonUp(0) && clickedThisGO != null)
		{
			if (clickedThisGO.name == "button_Start") { SplitBackground(); }
			else if (clickedThisGO.name == "button_options") { isLerpingTowardOptions = true; }
			else if (clickedThisGO.name == "button_back") { isLerpingTowardOptions = false; }
			else if (clickedThisGO.name == "button_clearProgress") { clickedThisGO.renderer.material = confirmClearProgressImage; clickedThisGO.name = "button_clearProgressConfirm"; }
			else if (clickedThisGO.name == "button_clearProgressConfirm") { clickedThisGO.renderer.material = progressClearedImage; clickedThisGO.name = "progressCleared"; SaveData.DeleteAll(); }
			else if (clickedThisGO.name == "button_audioOn") { TurnAudioOn(); SaveData.SetInt("isAudioOn", 1); }
			else if (clickedThisGO.name == "button_audioOff") { TurnAudioOff(); SaveData.SetInt("isAudioOn", 0); }
			isClickingButton = false;
			UnclickAButton(clickedThisGO);
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
	}

	void SplitBackground()
	{
		backgroundIsSplitting = true;
		foreach (MeshCollider col in mainMenuParent.GetComponentsInChildren<MeshCollider>())
		{
			col.enabled = false;
		}
	}

	void ClickAButton(GameObject hit)
	{
		if (!isClickingButton) { originalScale = hit.transform.localScale; isClickingButton = true; }
		hit.transform.localScale = originalScale * clickScale;
	}

	void UnclickAButton(GameObject hit)
	{
		hit.transform.localScale = originalScale;
	}

	void TurnAudioOff()
	{
		AudioSource audio = GameObject.Find("Music Player").GetComponent<AudioSource>();
		audio.enabled = false;
	}

	void TurnAudioOn()
	{
		AudioSource m_audio = GameObject.Find("Music Player").GetComponent<AudioSource>();
		m_audio.enabled = true;
		m_audio.Play();
	}
}
