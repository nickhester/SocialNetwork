using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.IO;
using System;
using Soomla.Store;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GameManager : MonoBehaviour {

	private float appointmentStartTime;
	private int numAppointmentsThisSession = 0;
	private Clipboard currentClipboard;
	private float currentGameSessionTime = 0.0f;
	private int targetFrameRate = 60;
	
	public GameDataBlob gameDataBlob;
	public int pendingCloudSaveOperation = 0;	// 1 == load from cloud, 2 == save to cloud, 3 == save empty data to cloud to clear save data
	public int saveDataFormatVersion = 1;
	public Texture2D saveGameImage;

	public GameService gameService;

	void Start()
	{
		// singleton
		GameObject[] foundManager = GameObject.FindGameObjectsWithTag("GameManager");
		foreach (GameObject i in foundManager)
		{
			if (i != this.gameObject)
			{
				Destroy(gameObject);
			}
		}
		Initialize();
	}

	void Initialize()
	{
		DontDestroyOnLoad(gameObject);

		Application.targetFrameRate = targetFrameRate;

		RestartSessionMetrics();

		// soomla store stuff
		StoreEvents.OnItemPurchased += onItemPurchased;
		StoreEvents.OnRestoreTransactionsFinished += onRestoreTransactionsFinished;
		if (!SoomlaStore.Initialized)
		{
			SoomlaStore.Initialize(new SoomlaStoreAssets());
		}

		gameDataBlob = new GameDataBlob();
		gameDataBlob.Init(saveDataFormatVersion);

		// initialize google play to cloud save
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			.EnableSavedGames()
			.Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();

		// create generic game service class
		

		// if it's set on Android, but it's playing in the Editor...
#if UNITY_EDITOR
		gameService = new GameServiceMock();
#endif
#if UNITY_IOS
		gameService = new AppleGameCenterAPI();
		gameService.Initialize();
#endif
#if UNITY_ANDROID
		gameService = new GooglePlayAPI();
#endif
		gameService.Initialize();

		// callback is managed in GooglePlayAPI 
		pendingCloudSaveOperation = 1;
	}

	// soomla event - item purchased
	public void onItemPurchased(PurchasableVirtualItem pvi, string payload)
	{
		print ("onItemPurchased called");
		Calendar c = GameObject.FindObjectOfType<Calendar>();
		if (c != null)
		{
			c.ReloadCalendar();
		}
		Upgrade.PurchaseUpgrade_callback(true);
	}

	// soomla event - restore transactions
	public void onRestoreTransactionsFinished(bool success)
	{
		print("onRestoreTransactionsFinished called");
	}

	void RestartSessionMetrics()
	{
		numAppointmentsThisSession = 0;
		currentGameSessionTime = 0.0f;
	}

	void SendSessionMetrics()
	{
		// log num appointments played this session
		MetricsLogger.Instance.LogCustomEvent("GameSession", "AppointmentsPlayedThisSession", "close", numAppointmentsThisSession);
		// log this session play time
		MetricsLogger.Instance.LogCustomEvent("GameSession", "PlayTimeThisGameSession", "close", currentGameSessionTime);
	}

	void Update()
	{
		currentGameSessionTime += Time.deltaTime;
	}

	public void Register_Clipboard(Clipboard _myClipboard)
	{
		currentClipboard = _myClipboard;
	}

	public Clipboard GetClipboard()
	{
		return currentClipboard;
	}

	public int GetCurrentDay()
	{
		if (currentClipboard != null)
		{
			return currentClipboard.GetNextLevelUp().GetMyDayIndex();
		}
		else
		{
			return -1;
		}
	}

	public int GetCurrentAppointment()
	{
		if (currentClipboard != null)
		{
			return currentClipboard.GetNextLevelUp().GetMyLevelIndex();
		}
		else
		{
			return -1;
		}
	}

	#region events

	public void Event_AppointmentStart()
	{
		MetricsLogger.Instance.LogProgressionEvent_Start(FormatDayAndLevel());
		appointmentStartTime = Time.time;
		numAppointmentsThisSession++;
	}

	public void Event_AppointmentReset()
	{
		appointmentStartTime = Time.time;
	}

	public void Event_AppointmentEnd()
	{
		float appointmentDuration = Time.time - appointmentStartTime;

		MetricsLogger.Instance.LogCustomEvent("Appointment", "AppointmentLength", FormatDayAndLevel(), appointmentDuration);
		MetricsLogger.Instance.LogProgressionEvent_Complete(FormatDayAndLevel());

		// add to total accumulated time
		SaveGame.SetTotalAppointmentTime(SaveGame.GetTotalAppointmentTime() + appointmentDuration);
	}

	#endregion
	/*
	void OnApplicationQuit()
	{
		SendSessionMetrics2();
	}
	
	void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			SendSessionMetrics();
		}
		else
		{
			RestartSessionMetrics();
		}
	}
	*/
	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			SendSessionMetrics();
		}
		else
		{
			RestartSessionMetrics();
		}
	}

	public string FormatDayAndLevel()
	{
		if (GetClipboard() == null || GetClipboard().GetNextLevelUp() == null)
		{
			return "-1--1";
		}
		else
		{
			return GetClipboard().GetNextLevelUp().GetMyDayIndex().ToString("D2") + "-" + GetClipboard().GetNextLevelUp().GetMyLevelIndex().ToString("D2");
		}
	}

	public void UpdateLocalSaveDataFromBlob()
	{
		SaveGame.CloudSaveToLocalSave(gameDataBlob);
	}

	public void UpdateCloudSaveFromLocal()
	{
		pendingCloudSaveOperation = 2;
		gameDataBlob.UpdateToSend(saveDataFormatVersion);
		gameService.Initialize();
	}

	public void DeleteCloudSave()
	{
		pendingCloudSaveOperation = 3;
		gameService.Initialize();
	}

	public void callback_OnSavedGameOpened(GooglePlayGames.BasicApi.SavedGame.ISavedGameMetadata game)
	{
#if UNITY_ANDROID
		GooglePlayAPI gp = gameService as GooglePlayAPI;
		if (pendingCloudSaveOperation == 1)
		{
			// ready to load from cloud
			gp.LoadGameData(game);
		}
		else if (pendingCloudSaveOperation == 2)
		{
			// ready to save to cloud
			byte[] byteArrayToSend = ToByteArray(gameDataBlob);
			gp.SaveGame(game, byteArrayToSend, System.TimeSpan.FromSeconds(gameDataBlob.totalAppointmentTime));
		}
		else if (pendingCloudSaveOperation == 3)
		{
			// ready to send an invalid cloud save data to clear cloud save data
			byte[] byteArrayToSend = new byte[] { 0, 1, 2, 3 };
			gp.SaveGame(game, byteArrayToSend, System.TimeSpan.FromSeconds(0.0f));
		}
#endif

	}

	public void callback_OnSavedGameDataRead(byte[] data)
	{
		if (data.Length == 0)
		{
			Debug.LogWarning("gameDataBlob from data is 0 length");
			return;
		}

		// read byte data
		gameDataBlob = FromByteArray(data) as GameDataBlob;

		if (gameDataBlob == null)
		{
			Debug.LogWarning("gameDataBlob from data is null");
			return;
		}

		print("gameDataBlob looks good, sending it to UpdateLocalSaveDataFromBlob()");
		UpdateLocalSaveDataFromBlob();
	}

	byte[] ToByteArray(object source)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		using (MemoryStream stream = new MemoryStream())
		{
			formatter.Serialize(stream, source);
			return stream.ToArray();
		}
	}

	object FromByteArray(byte[] byteArrayInput)
	{
		
		BinaryFormatter formatter = new BinaryFormatter();
		using (MemoryStream stream = new MemoryStream(byteArrayInput))
		{
			return formatter.Deserialize(stream);
		}
	}
}
