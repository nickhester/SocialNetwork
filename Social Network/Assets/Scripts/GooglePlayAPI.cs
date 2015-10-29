using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public static class GooglePlayAPI {

	private static bool isUsingGooglePlay = true;

	public static void Initialize()
	{
		PlayGamesPlatform.Activate();

		Social.localUser.Authenticate((bool success) =>
		{
			// don't know what to do if this fails...
		});
	}

	public static void ReportStarCount(int starCount)
	{
		if (!isUsingGooglePlay) { return; }
		
		Application.ExternalCall("kongregate.stats.submit", "StarCount", starCount);	// max type
	}

	public static void ReportWeekCompleted(int weekCompleted)
	{
		if (!isUsingGooglePlay) { return; }
		Application.ExternalCall("kongregate.stats.submit", "Week" + (weekCompleted + 1) + "Completed", 1);	// max type
	}

	public static void ReportWeekPerfected(int weekPerfected)
	{
		if (!isUsingGooglePlay) { return; }
		Application.ExternalCall("kongregate.stats.submit", "Week" + (weekPerfected + 1) + "Perfected", 1);	// max type
	}

	public static void ReportGameCompleted()
	{
		if (!isUsingGooglePlay) { return; }
		Application.ExternalCall("kongregate.stats.submit", "GameComplete", 1);	// max type
	}

	public static void ReportGamePerfected()
	{
		if (!isUsingGooglePlay) { return; }
		Application.ExternalCall("kongregate.stats.submit", "GamePerfected", 1);	// max type
	}
}
