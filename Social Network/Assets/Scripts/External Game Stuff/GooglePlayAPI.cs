using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;
using System;


public static class GooglePlayAPI {

	private static bool isUsingGooglePlay = true;
	public static bool isPlayingOffline = false;
	private static string saveDataName = "saveData";

	public static void Initialize()
	{
		if (!isPlayingOffline)
		{
			Social.localUser.Authenticate((bool success) =>
			{
				if (success)
				{
					MonoBehaviour.print("GooglePlayGames completed initialization");
					OpenSavedgame(saveDataName);
					isPlayingOffline = false;
				}
				else
				{
					MonoBehaviour.print("GooglePlayGames failed to authenticate local user");
					isPlayingOffline = true;
				}
			});
		}
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

	public static void SendCloudSavedGame()
	{

	}

	//========= Google's specific functions

	static public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			MonoBehaviour.print("has opened saved game (cloud save)");
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().callback_OnSavedGameOpened(game);
		}
		else
		{
			MonoBehaviour.print("could not open saved game (cloud save)");
		}
	}
	
	static public void OpenSavedgame(string filename)
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		
		savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
	}

	static public void SaveGame(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime)
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

		SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
		builder = builder
			.WithUpdatedPlayedTime(totalPlaytime)
			.WithUpdatedDescription("Saved game at " + DateTime.Now);

		// my customization
		Texture2D savedImage = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().saveGameImage;
		// end my customization

		if (savedImage != null)
		{
			// This assumes that savedImage is an instance of Texture2D
			// and that you have already called a function equivalent to
			// getScreenshot() to set savedImage
			// NOTE: see sample definition of getScreenshot() method below
			byte[] pngData = savedImage.EncodeToPNG();
			builder = builder.WithUpdatedPngCoverImage(pngData);
		}
		SavedGameMetadataUpdate updatedMetadata = builder.Build();
		savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
	}

	static public void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			// handle reading or writing of saved game.
		}
		else
		{
			// handle error
		}
	}

	/*
	static public Texture2D getScreenshot()
	{
		// Create a 2D texture that is 1024x700 pixels from which the PNG will be
		// extracted
		Texture2D screenShot = new Texture2D(1024, 700);

		// Takes the screenshot from top left hand corner of screen and maps to top
		// left hand corner of screenShot texture
		screenShot.ReadPixels(
			new Rect(0, 0, Screen.width, (Screen.width / 1024) * 700), 0, 0);
		return screenShot;
	}
	*/

	static public void LoadGameData(ISavedGameMetadata game)
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
	}

	static public void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			// handle processing the byte array data
			MonoBehaviour.print("has read saved game (cloud save)");
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().callback_OnSavedGameDataRead(data);
		}
		else
		{
			// handle error
		}
	}
}
