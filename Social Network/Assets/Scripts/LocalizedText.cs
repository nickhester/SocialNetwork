using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
	public string stringName;

	void Start ()
	{
		SetLocalizedText();
	}

	public void SetLocalizedText()
	{
		Text myTextObject = GetComponent<Text>();
		LocalizedTextManager localizedTextManager = FindObjectOfType<LocalizedTextManager>();
		myTextObject.text = localizedTextManager.GetLocalizedString(stringName);
	}
}
