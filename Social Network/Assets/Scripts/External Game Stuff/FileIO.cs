using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FileIO {

	TextAsset textFile;
	string fileName;
	string fileExtension;

	public FileIO(string _fileName)
	{
		fileName = _fileName;
		RefreshFile();
	}

	public FileIO(string _fileName, string _extension)
	{
		fileName = _fileName;
		fileExtension = _extension;
		RefreshFile();
	}

	public void AppendToFile(string stringToAppend)
	{
		RefreshFile();

		stringToAppend = stringToAppend.TrimEnd('\n');

		using (StreamWriter sw = new StreamWriter(GetPath(), true))
		{
			sw.AutoFlush = true;
			sw.WriteLine(stringToAppend);
		}
	}

	public string GetFileText()
	{
		//return textFile.text;
		string text = "";
		using (StreamReader sr = new StreamReader(GetPath(), true))
		{
			text = sr.ReadToEnd();
		}
		return text;
	}

	void RefreshFile()
	{
		textFile = Resources.Load<TextAsset>(fileName);
	}

	string GetPath()
	{
		string path = Application.dataPath + "\\Resources\\" + fileName + "." + fileExtension;
		return path;
	}
}
