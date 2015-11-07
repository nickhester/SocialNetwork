using UnityEngine;
using System.Collections;

public class ActivateCondition : MonoBehaviour {

	public bool activeOnlyInUnityEditor = true;

	void Start()
	{
		bool isUnityEditor = false;
#if UNITY_EDITOR
		isUnityEditor = true;
#endif
		if (!isUnityEditor && activeOnlyInUnityEditor)
		{
			gameObject.SetActive(false);
		}
	}

}
