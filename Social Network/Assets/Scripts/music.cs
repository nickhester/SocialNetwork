using UnityEngine;
using System.Collections;

public class music : MonoBehaviour {

	private AudioSource myAudioComponent;

	// Use this for initialization
	void Start ()
	{
		GameObject foundMusicPlayer = GameObject.Find("Music Player");
		
		if (foundMusicPlayer != null && foundMusicPlayer != gameObject)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		myAudioComponent = gameObject.GetComponent<AudioSource>();

		if (SaveData.HasKey("isAudioOn") && SaveData.GetInt("isAudioOn") == 0)
		{
			myAudioComponent.enabled = false;
		}
		else if (myAudioComponent.enabled == true)
		{
			myAudioComponent.Play();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
