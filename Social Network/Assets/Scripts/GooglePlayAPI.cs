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
			if (!success)
			{
				MonoBehaviour.print("GooglePlayGames failed to authenticate local user");
			}
		});
		MonoBehaviour.print("GooglePlayGames completed initialization");
	}

	public static void ReportStarCount(int starCount)
	{
		Social.ReportScore(starCount, "CgkIsqTI3JUKEAIQDw", (bool success) =>
		{
			if (!success)
			{
				MonoBehaviour.print("GooglePlayGames failed to ReportStarCount");
			}
			else
			{
				MonoBehaviour.print("GooglePlayGames succeeded in ReportStarCount");
			}
		});
	}

	public static void ReportWeekCompleted(int weekCompleted)
	{
		string achievementCode = "fail";

		switch (weekCompleted)
		{
			case 0:
			{
				achievementCode = "CgkIsqTI3JUKEAIQAQ";
				break;
			}
			case 1:
			{
				achievementCode = "CgkIsqTI3JUKEAIQAg";
				break;
			}
			case 2:
			{
				achievementCode = "CgkIsqTI3JUKEAIQAw";
				break;
			}
			case 3:
			{
				achievementCode = "CgkIsqTI3JUKEAIQBA";
				break;
			}
			case 4:
			{
				achievementCode = "CgkIsqTI3JUKEAIQBQ";
				break;
			}
			case 5:
			{
				achievementCode = "CgkIsqTI3JUKEAIQBg";
				break;
			}
			default:
			{
				break;
			}
		}

		SendReport(achievementCode);
	}

	public static void ReportWeekPerfected(int weekPerfected)
	{
		string achievementCode = "fail";

		switch (weekPerfected)
		{
			case 0:
			{
				achievementCode = "CgkIsqTI3JUKEAIQBw";
				break;
			}
			case 1:
			{
				achievementCode = "CgkIsqTI3JUKEAIQCA";
				break;
			}
			case 2:
			{
				achievementCode = "CgkIsqTI3JUKEAIQCQ";
				break;
			}
			case 3:
			{
				achievementCode = "CgkIsqTI3JUKEAIQCg";
				break;
			}
			case 4:
			{
				achievementCode = "CgkIsqTI3JUKEAIQCw";
				break;
			}
			case 5:
			{
				achievementCode = "CgkIsqTI3JUKEAIQDA";
				break;
			}
			default:
			{
				break;
			}
		}

		SendReport(achievementCode);
	}

	public static void ReportGameCompleted()
	{
		SendReport("CgkIsqTI3JUKEAIQDQ");
	}

	public static void ReportGamePerfected()
	{
		SendReport("CgkIsqTI3JUKEAIQDg");
	}

	static void SendReport(string code)
	{
		Social.ReportProgress(code, 100.0f, (bool success) =>
		{
			if (!success)
			{
				MonoBehaviour.print("GooglePlayGames failed to report Achievement (" + code + ")");
			}
			else
			{
				MonoBehaviour.print("GooglePlayGames succeeded in reporting Achievement (" + code + ")");
			}
		});
	}
}
