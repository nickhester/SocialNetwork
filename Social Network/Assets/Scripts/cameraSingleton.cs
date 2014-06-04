using UnityEngine;
using System.Collections;

public class cameraSingleton : MonoBehaviour {

	// Use this for initialization
	void Start () {

		GameObject[] foundCamera = GameObject.FindGameObjectsWithTag("MainCamera");

		foreach (GameObject i in foundCamera)
		{
			if (i != this.gameObject)
			{
				Destroy (gameObject);
			}
		}	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
