using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;


public class GooglePlayAPI : GameService
{

	public bool isPlayingOffline = false;
	private string saveDataName = "saveData";


	public void Initialize()
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

	public void ReportStarCount(int starCount)
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

	public void ReportWeekCompleted(int weekCompleted)
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

	public void ReportWeekPerfected(int weekPerfected)
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

	public void ReportGameCompleted()
	{
		SendReport("CgkIsqTI3JUKEAIQDQ");
	}

	public void ReportGamePerfected()
	{
		SendReport("CgkIsqTI3JUKEAIQDg");
	}

	public void SendReport(string code)
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

	public bool GetIsPlayingOffline()
	{
		return isPlayingOffline;
	}

	public void SetIsPlayingOffline(bool _isPlayingOffline)
	{
		isPlayingOffline = _isPlayingOffline;
	}

	//========= Google's specific functions

	public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
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
	
	public void OpenSavedgame(string filename)
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		
		savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
	}

	public void SaveGame(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime)
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

	public void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
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

	public void LoadGameData(ISavedGameMetadata game)
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
	}

	public void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
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
