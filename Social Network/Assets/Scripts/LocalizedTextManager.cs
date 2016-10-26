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
		Arabic
	}
	public Language currentLanguage;

	void Start ()
	{
		Initialize();
	}

	void Initialize()
	{
		if (!hasBeenInitizlied)
		{
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
				currentLanguageString = "english";
				break;
			case Language.Spanish:
				currentLanguageString = "spanish";
				break;
			case Language.Arabic:
				currentLanguageString = "arabic";
				break;
			default:
				currentLanguageString = "english";
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
