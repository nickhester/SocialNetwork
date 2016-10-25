using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This class is optimized for my localization table
 * but should be just a generic table
 * since I don't have access to C#'s data table by default
*/
public class CustomDataTable
{
	Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

	public CustomDataTable()
	{
		//
	}
	
	public string GetEntry(string row, string column)
	{
		if (data.ContainsKey(row) && data[row].ContainsKey(column))
		{
			return data[row][column];
		}
		
		Debug.LogError("Error Getting Localized Text");
		return "error getting text";
	}

	public void AddEntry(string row, string column, string value)
	{
		if (data.ContainsKey(row))		// if the language dictionary already exists
		{
			data[row].Add(column, value);
		}
		else
		{
			data.Add(row, new Dictionary<string, string>());
			data[row].Add(column, value);
		}
	}
}
