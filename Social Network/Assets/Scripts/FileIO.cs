using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FileIO {

	TextAsset textFile;
	string fileName;

	public FileIO(string _fileName)
	{
		fileName = _fileName;
		RefreshFile();
	}

	public void AppendToFile(string stringToAppend)
	{
		RefreshFile();

		stringToAppend = stringToAppend.TrimEnd('\n');

		using (StreamWriter sw = new StreamWriter(Application.dataPath + "\\Resources\\" + fileName + ".txt", true))
		{
			sw.AutoFlush = true;
			sw.WriteLine(stringToAppend);

		}
	}

	public string GetFileText()
	{
		return textFile.text;
	}

	void RefreshFile()
	{
		textFile = Resources.Load<TextAsset>(fileName);
	}
}
