using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
	public string stringName;

	void Start ()
	{
		Text myTextObject = GetComponent<Text>();
		LocalizedTextManager localizedTextManager = FindObjectOfType<LocalizedTextManager>();
		myTextObject.text = localizedTextManager.GetLocalizedString(stringName);
	}
}
