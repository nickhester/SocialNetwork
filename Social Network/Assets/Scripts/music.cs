using UnityEngine;
using System.Collections;

public class music : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{		
		GameObject foundMusicPlayer = GameObject.Find("Music Player");
		
		if (foundMusicPlayer != null && foundMusicPlayer != gameObject)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
