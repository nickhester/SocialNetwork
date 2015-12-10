using UnityEngine;
using System.Collections;

public class SpecialEffects : MonoBehaviour
{
	bool isShaking = false;
	GameObject shakingObject;
	float shakeFrequency1 = 49.0f;
	float shakeFrequency2 = 35.0f;
	float shakeScale = 0.012f;
	private Vector3 shakingObjectNewPosition = Vector3.zero;

	void Update ()
	{
		if (isShaking)
		{
			Vector3 pos = shakingObject.transform.position;
			shakingObjectNewPosition.x = pos.x + (Mathf.Sin(Time.time * shakeFrequency1) * shakeScale);
			shakingObjectNewPosition.y = pos.y + (Mathf.Sin(Time.time * shakeFrequency2) * shakeScale);
			shakingObjectNewPosition.z = pos.z;
			shakingObject.transform.position = shakingObjectNewPosition;
		}
	}

	public void ShakeExcitedly(GameObject go)
	{
		if (shakingObject != null)
		{
			shakingObject.GetComponent<Person>().SetAsExcited(false);
		}
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
