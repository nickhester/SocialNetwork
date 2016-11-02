using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
	public string stringName;
	public bool alwaysResizeToBestFit = false;	// this applies to all languages except English

	void Start ()
	{
		SetLocalizedText();
	}

	public void SetLocalizedText()
	{
		Text myTextObject = GetComponent<Text>();
		LocalizedTextManager localizedTextManager = FindObjectOfType<LocalizedTextManager>();
		myTextObject.text = localizedTextManager.GetLocalizedString(stringName);

		if (localizedTextManager.currentLanguage == LocalizedTextManager.Language.English)
		{
			myTextObject.resizeTextForBestFit = false;
		}
		else if (localizedTextManager.currentLanguage == LocalizedTextManager.Language.Arabic
			|| alwaysResizeToBestFit)
		{
			myTextObject.resizeTextForBestFit = true;
		}
		else
		{
			myTextObject.resizeTextForBestFit = false;
		}
	}
}
