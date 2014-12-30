using UnityEngine;
using System.Collections;

public static class KongregateAPI {

	// set this to "true" manually if using Kongregate API
	public static bool isUsingKongregate = false;

	public static bool isKongregateLoaded = false;
	public static int userId = 0;
	static string username = "Guest";
	static string gameAuthToken = "";

	public static void Initialize()
	{
		Application.ExternalEval(
			"if(typeof(kongregateUnitySupport) != 'undefined'){" +
			" kongregateUnitySupport.initAPI('kongregateAPIObject', 'OnKongregateAPILoaded');" +
			"};"
			);
	}

	public static void OnKongregateAPILoaded(string userInfoString)
	{
		isKongregateLoaded = true;

		string[] parameters = userInfoString.Split('|');
		userId = int.Parse(parameters[0]);
		username = parameters[1];
		gameAuthToken = parameters[2];
    }

	public static void ReportStarCount(int starCount)
	{
		Application.ExternalCall("kongregate.stats.submit", "StarCount", starCount);	// max type
	}
    
	public static void ReportWeekCompleted(int weekCompleted)
	{
		Application.ExternalCall("kongregate.stats.submit", "Week" + (weekCompleted + 1) + "Completed", 1);	// max type
	}

	public static void ReportWeekPerfected(int weekPerfected)
	{
		Application.ExternalCall("kongregate.stats.submit", "Week" + (weekPerfected + 1) + "Perfected", 1);	// max type
	}

	public static void ReportGameCompleted()
	{
		Application.ExternalCall("kongregate.stats.submit", "GameComplete", 1);	// max type
	}

	public static void ReportGamePerfected()
	{
		Application.ExternalCall("kongregate.stats.submit", "GamePerfected", 1);	// max type
	}
}
