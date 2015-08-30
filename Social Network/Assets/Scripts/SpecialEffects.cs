using UnityEngine;
using System.Collections;

public class SpecialEffects : MonoBehaviour
{
	bool isShaking = false;
	GameObject shakingObject;
	float shakeFrequency1 = 49.0f;
	float shakeFrequency2 = 35.0f;
	float shakeScale = 0.012f;

	void Update ()
	{
		if (isShaking)
		{
			Vector3 pos = shakingObject.transform.position;
			shakingObject.transform.position = new Vector3(pos.x + (Mathf.Sin(Time.time * shakeFrequency1) * shakeScale), pos.y + (Mathf.Sin(Time.time * shakeFrequency2) * shakeScale), pos.z);
		}
	}

	public void ShakeExcitedly(GameObject go)
	{
		isShaking = true;
		shakingObject = go;
		go.GetComponent<Person>().SetAsExcited(true);
	}

	public void ShakeExcitedly(Person person)
	{
		ShakeExcitedly(person.gameObject);
	}

	public void StopShakingExcitedly()
	{
		if (isShaking)
		{
			isShaking = false;
			shakingObject.GetComponent<Person>().SetAsExcited(false);
			shakingObject = null;
		}
	}
}
