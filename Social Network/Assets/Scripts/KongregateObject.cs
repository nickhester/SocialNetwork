using UnityEngine;
using System.Collections;

public class KongregateObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//initialize external API
		if (KongregateAPI.isUsingKongregate && !KongregateAPI.isKongregateLoaded)
		{
			GameObject.Find("Main Menu").GetComponent<mainMenu>().isUsingExternalAPI = true; 
			KongregateAPI.Initialize();
		}
	}

	public void OnKongregateAPILoaded(string userInfoString)
	{
		KongregateAPI.OnKongregateAPILoaded(userInfoString);
	}
}
