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

	// Use this for initialization
	void Start () {
		clickScale = 0.9f;
		cameraStartingPos = Camera.main.transform.position;
		cameraOptionsPos = new Vector3(cameraStartingPos.x, cameraStartingPos.y - 12.0f, cameraStartingPos.z);
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
				if (hit.transform.name == "button_Start")
				{
					ClickAButton(hit.transform.gameObject);
					clickedThisGO = hit.transform.gameObject;
				}
				else if (hit.transform.name == "button_options")
				{
					ClickAButton(hit.transform.gameObject);
					clickedThisGO = hit.transform.gameObject;
				}
				else if (hit.transform.name == "button_back")
				{
					ClickAButton(hit.transform.gameObject);
					clickedThisGO = hit.transform.gameObject;
				}
				else if (hit.transform.name == "button_clearProgress")
				{
					ClickAButton(hit.transform.gameObject);
					clickedThisGO = hit.transform.gameObject;
				}
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			if (clickedThisGO.name == "button_Start") { Application.LoadLevel("Scene_LevelSelection"); }
			else if (clickedThisGO.name == "button_options") { isLerpingTowardOptions = true; }
			else if (clickedThisGO.name == "button_back") { isLerpingTowardOptions = false; }
			else if (clickedThisGO.name == "button_clearProgress") { PlayerPrefs.DeleteAll(); }
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

}
