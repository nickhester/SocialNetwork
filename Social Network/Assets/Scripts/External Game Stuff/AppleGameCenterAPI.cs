﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class AppleGameCenterAPI : GameService
{
	public bool isPlayingOffline = false;

	public void Initialize()
	{
		if (!isPlayingOffline)
		{
			// TODO: initialize google play to cloud save

			Social.localUser.Authenticate((bool success) =>
			{
				if (success)
				{
					MonoBehaviour.print("Social initialization succeeded");

					Social.LoadAchievements((IAchievement[] achievements) =>
					{
						if (achievements.Length != 0)
						{

						}
						else
						{
							MonoBehaviour.print("Found " + achievements.Length + " Achievements");
						}
					});

					// TODO: open saved game here

					isPlayingOffline = false;
				}
				else
				{
					MonoBehaviour.print("AppleGameCenter failed to authenticate local user");
					isPlayingOffline = true;
				}
			});
			GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
		}
	}

	public void ReportStarCount(int starCount)
	{
		Social.ReportScore(starCount, "star_count", (bool success) =>
		{
			if (success)
			{
				MonoBehaviour.print("AppleGameCenter succeeded in ReportStarCount");
			}
			else
			{
				MonoBehaviour.print("AppleGameCenter failed to ReportStarCount");
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

	[DllImport("__Internal")]
	private static extern void _ReportAchievement(string achievementID, float progress);

	public void SendReport(string code)
	{
		_ReportAchievement(code, 100.0f);

		/*
		Social.ReportProgress(code, 100.0f, (bool success) =>
		{
			if (success)
			{
				MonoBehaviour.print("AppleGameCenter succeeded in reporting Achievement (" + code + ")");
			}
			else
			{
				MonoBehaviour.print("AppleGameCenter failed to report Achievement (" + code + ")");
			}
		});
		*/
	}

	public bool GetIsPlayingOffline()
	{
		return isPlayingOffline;
	}

	public void SetIsPlayingOffline(bool _isPlayingOffline)
	{
		isPlayingOffline = _isPlayingOffline;
	}

	public void LeaveRating()
	{
		Application.OpenURL("https://itunes.apple.com/us/app/social-sessions-game/id1066662142");
	}
}
