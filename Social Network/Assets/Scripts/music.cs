using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	private AudioSource myAudioComponent;

	void Start ()
	{
		GameObject foundMusicPlayer = GameObject.Find("Music Player");
		
		if (foundMusicPlayer != null && foundMusicPlayer != gameObject)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		myAudioComponent = gameObject.GetComponent<AudioSource>();

		if (!SaveGame.GetAudioOn_music())
		{
			myAudioComponent.enabled = false;
		}
		else if (myAudioComponent.enabled == true)
		{
			myAudioComponent.Play();
		}
	}
}
