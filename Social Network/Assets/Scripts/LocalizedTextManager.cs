using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LocalizedTextManager : MonoBehaviour
{
	private bool hasBeenInitizlied = false;
	private CustomDataTable localizedDataTableObject;
	
	public enum Language
	{
		English,
		Spanish,
		Arabic,
		Romanian
	}
	public Language currentLanguage;

	void Start ()
	{
		Initialize();
	}

	public void ToggleLanguageAndUpdateText()
	{
		int newLanguageIndex = ((int)(currentLanguage + 1));
		int numLanguages = (Enum.GetNames(typeof(Language)).Length);
		currentLanguage = (Language)(newLanguageIndex % numLanguages);

		List<LocalizedText> localizedTextObjects = new List<LocalizedText>(FindObjectsOfType<LocalizedText>());
		for (int i = 0; i < localizedTextObjects.Count; i++)
		{
			localizedTextObjects[i].SetLocalizedText();
		}

		MetricsLogger.Instance.LogCustomEvent("Settings", "LanguageManualToggle", "");
		SaveGame.SetLanguage(currentLanguage);
	}

	void Initialize()
	{
		if (!hasBeenInitizlied)
		{
			// check save data for previous language setting
			if (SaveGame.GetHasSetLanguage())
			{
				currentLanguage = SaveGame.GetLanguage();
			}
			else
			{
				// check system language
				switch (Application.systemLanguage)
				{
					case SystemLanguage.Arabic:
						{
							currentLanguage = Language.Arabic;
							MetricsLogger.Instance.LogCustomEvent("Settings", "SystemLanguageSet", "Arabic");
							break;
						}
					case SystemLanguage.English:
						{
							currentLanguage = Language.English;
							MetricsLogger.Instance.LogCustomEvent("Settings", "SystemLanguageSet", "English");
							break;
						}
					case SystemLanguage.Spanish:
						{
							currentLanguage = Language.Spanish;
							MetricsLogger.Instance.LogCustomEvent("Settings", "SystemLanguageSet", "Spanish");
							break;
						}
					case SystemLanguage.Romanian:
						{
							currentLanguage = Language.Romanian;
							MetricsLogger.Instance.LogCustomEvent("Settings", "SystemLanguageSet", "Romanian");
							break;
						}
					default:
						{
							currentLanguage = Language.English;
							MetricsLogger.Instance.LogCustomEvent("Settings", "SystemLanguageSet", "Unknown");
							break;
						}
				}
			}

			FileIO localizedTextFile = new FileIO("localizedText", "csv");
			localizedDataTableObject = ReadCSV(localizedTextFile.GetFileText());
			hasBeenInitizlied = true;
		}
	}

	public string GetLocalizedString(string _name)
	{
		Initialize();

		string currentLanguageString = "";
		switch (currentLanguage)
		{
			case Language.English:
				currentLanguageString = "English";
				break;
			case Language.Spanish:
				currentLanguageString = "Spanish";
				break;
			case Language.Arabic:
				currentLanguageString = "Arabic";
				break;
			case Language.Romanian:
				currentLanguageString = "Romanian";
				break;
			default:
				currentLanguageString = "English";
				break;
		}

		string returnString = localizedDataTableObject.GetEntry(currentLanguageString, _name);
		return returnString;
	}

	
	public CustomDataTable ReadCSV(string csvText)
	{
		CustomDataTable table = new CustomDataTable();

		// collect column headers to use as languages
		string[] lineArray = csvText.Split('\n');
		List<string> columnHeaders = null;

		for (int i = 0; i < lineArray.Length; i++)
		{
			string oneLine = lineArray[i];
			oneLine = oneLine.TrimEnd();
			string[] columnItemArray = oneLine.Split(',');

			if (i == 0)	// get the language column headers
			{
				columnHeaders = new List<string>(columnItemArray);
			}
			else
			{
				string rowHeader = columnItemArray[0];
				for (int j = 1; j < columnItemArray.Length; j++)	// start at 1 to skip row header
				{
					string contents = columnItemArray[j];
					// replace escaped escape characters with literal escape characters
					contents = contents.Replace("\\n", "\n");
					contents = contents.Replace("*c*", ",");

					// reverse order if language requires
					if (columnHeaders[j] == "arabic")
					{
						char[] charArray = contents.ToCharArray();
						Array.Reverse(charArray);
						contents = new string(charArray);
					}
					
					table.AddEntry(columnHeaders[j], rowHeader, contents);
				}
			}
		}

		return table;
	}
	
}
