using UnityEngine;
using System.Collections;

public class audioToggle : MonoBehaviour {

	public Material matOn;
	public Material matOff;

	public bool audioIsOn = true;
	public enum typeOfAudio
	{
		music,
		sfx
	};
	public typeOfAudio audioType;

	// Use this for initialization
	void Start () {

		GameObject foundSelf = GameObject.Find(gameObject.name);
		
		if (foundSelf != null && foundSelf != gameObject)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		if (audioType == typeOfAudio.sfx)
		{
			// TODO: figure out why this isn't working!!!
			if (SaveGame.GetAudioOn_sfx())
			{
				gameObject.renderer.material = matOn;
				audioIsOn = true;
			}
			else
			{
				gameObject.renderer.material = matOff;
				audioIsOn = false;
			}
		}
		else if (audioType == typeOfAudio.music)
		{
			if (SaveGame.GetAudioOn_music())
			{
				gameObject.renderer.material = matOn;
				audioIsOn = true;
			}
			else
			{
				gameObject.renderer.material = matOff;
				audioIsOn = false;
			}
		}
		int a = 1 + 1;
		int b = a * a;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown(0))
		{
			if (getObjectAtMouse() == gameObject)
			{
				GameObject networkManagerObject = GameObject.Find("networkMgr");
				if (audioIsOn)
				{
					if (audioType == typeOfAudio.sfx) { SaveGame.SetAudioOn_sfx(false); }
					else { SaveGame.SetAudioOn_music(false); }

					gameObject.renderer.material = matOff;
					audioIsOn = false;

					if (audioType == typeOfAudio.music)
					{
					AudioSource audio = GameObject.Find("Music Player").GetComponent<AudioSource>();
					audio.enabled = false;
					}


				}
				else
				{
					if (audioType == typeOfAudio.sfx) { SaveGame.SetAudioOn_sfx(true); }
					else { SaveGame.SetAudioOn_music(true); }
					gameObject.renderer.material = matOn;
					audioIsOn = true;

					if (audioType == typeOfAudio.music)
					{
						AudioSource m_audio = GameObject.Find("Music Player").GetComponent<AudioSource>();
						m_audio.enabled = true;
						m_audio.Play();
					}
				}
				// push bool to network manager
				if ((audioType == typeOfAudio.sfx) && (networkManagerObject != null))
				{
					networkManagerObject.GetComponent<class_NetworkMgr>().isAudioOn_sfx = audioIsOn;
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


	void OnLevelWasLoaded(int level)
	{
		if (level == 0)
			Destroy(gameObject);
	}
}
