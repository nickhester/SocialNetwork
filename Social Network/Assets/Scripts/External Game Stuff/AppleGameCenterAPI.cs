using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class AppleGameCenterAPI : GameService
{
	public bool isPlayingOffline = false;

	public void Initialize()
	{
		if (!isPlayingOffline)
		{
			Social.localUser.Authenticate((bool success) =>
			{
				if (success)
				{
					MonoBehaviour.print("Social initialization succeeded");

					// TODO: open saved game here

					isPlayingOffline = false;
				}
				else
				{
					MonoBehaviour.print("AppleGameCenter failed to authenticate local user");
					isPlayingOffline = true;
				}
			});
		}
	}

	public void ReportStarCount(int starCount)
	{
		Social.ReportScore(starCount, "star_count", (bool success) =>
		{
			if (!success)
			{
				MonoBehaviour.print("AppleGameCenter failed to ReportStarCount");
			}
			else
			{
				MonoBehaviour.print("AppleGameCenter succeeded in ReportStarCount");
			}
		});
	}

	public void ReportWeekCompleted(int weekCompleted)
	{
		string achievementCode = "fail";

		switch (weekCompleted)
		{
			case 0:
				{
					achievementCode = "week_1_completed";
					break;
				}
			case 1:
				{
					achievementCode = "week_2_completed";
					break;
				}
			case 2:
				{
					achievementCode = "week_3_completed";
					break;
				}
			case 3:
				{
					achievementCode = "week_4_completed";
					break;
				}
			case 4:
				{
					achievementCode = "week_5_completed";
					break;
				}
			case 5:
				{
					achievementCode = "week_6_completed";
					break;
				}
			default:
				{
					break;
				}
		}

		SendReport(achievementCode);
	}

	public void ReportWeekPerfected(int weekPerfected)
	{
		string achievementCode = "fail";

		switch (weekPerfected)
		{
			case 0:
				{
					achievementCode = "week_1_perfected";
					break;
				}
			case 1:
				{
					achievementCode = "week_2_perfected";
					break;
				}
			case 2:
				{
					achievementCode = "week_3_perfected";
					break;
				}
			case 3:
				{
					achievementCode = "week_4_perfected";
					break;
				}
			case 4:
				{
					achievementCode = "week_5_perfected";
					break;
				}
			case 5:
				{
					achievementCode = "week_6_perfected";
					break;
				}
			default:
				{
					break;
				}
		}

		SendReport(achievementCode);
	}

	public void ReportGameCompleted()
	{
		SendReport("all_sessions_completed");
	}

	public void ReportGamePerfected()
	{
		SendReport("all_sessions_perfected");
	}

	public void SendReport(string code)
	{
		Social.ReportProgress(code, 100.0f, (bool success) =>
		{
			if (!success)
			{
				MonoBehaviour.print("AppleGameCenter failed to report Achievement (" + code + ")");
			}
			else
			{
				MonoBehaviour.print("AppleGameCenter succeeded in reporting Achievement (" + code + ")");
			}
		});
	}

	public bool GetIsPlayingOffline()
	{
		return isPlayingOffline;
	}

	public void SetIsPlayingOffline(bool _isPlayingOffline)
	{
		isPlayingOffline = _isPlayingOffline;
	}
}
