using UnityEngine;
using System.Collections;

public class audioToggle : MonoBehaviour {

	public Material matOn;
	public Material matOff;

	private bool musicIsOn;

	// Use this for initialization
	void Start () {

		GameObject foundSelf = GameObject.Find(gameObject.name);
		
		if (foundSelf != null && foundSelf != gameObject)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		if (SaveGame.GetAudioOn() == true)
		{
			gameObject.renderer.material = matOn;
			musicIsOn = true;
		}
		else
		{
			gameObject.renderer.material = matOff;
			musicIsOn = false;
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown(0))
		{
			if (getObjectAtMouse() == gameObject)
			{
				if (musicIsOn == true)
				{
					SaveGame.SetAudioOn(false);
					gameObject.renderer.material = matOff;
					musicIsOn = false;

					AudioSource audio = GameObject.Find("Music Player").GetComponent<AudioSource>();
					audio.enabled = false;
				}
				else
				{
					SaveGame.SetAudioOn(true);
					gameObject.renderer.material = matOn;
					musicIsOn = true;

					AudioSource m_audio = GameObject.Find("Music Player").GetComponent<AudioSource>();
					m_audio.enabled = true;
					m_audio.Play();
				}
			}
		}
	
	}


	GameObject getObjectAtMouse()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 10.0f))
		{
			return hit.transform.gameObject;
		}
		return null;
	}
}
