﻿using UnityEngine;
using System.Collections;
using Soomla.Store;

public class GameManager : MonoBehaviour {

	private float appointmentStartTime;
	private int numAppointmentsThisSession = 0;
	private Clipboard currentClipboard;
	private float currentGameSessionTime = 0.0f;

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

		RestartSessionMetrics();
		MetricsLogger.Instance.SendDataOnStart();

		// soomla store stuff
		
		StoreEvents.OnItemPurchased += onItemPurchased;
		StoreEvents.OnRestoreTransactionsFinished += onRestoreTransactionsFinished;
		if (!SoomlaStore.Initialized)
		{
			SoomlaStore.Initialize(new SoomlaStoreAssets());
		}
		
	}
	
	// soomla event - item purchased
	public void onItemPurchased(PurchasableVirtualItem pvi, string payload)
	{
		GameObject.FindObjectOfType<Calendar>().ReloadCalendar();
		Upgrade.PurchaseUpgrade_callback(true);
	}
	
	// soomla event - restore transactions
	public void onRestoreTransactionsFinished(bool success)
	{

	}

	void RestartSessionMetrics()
	{
		numAppointmentsThisSession = 0;
		currentGameSessionTime = 0.0f;
	}

	void SendSessionMetrics()
	{
		// log session end metrics
		MetricsLogger.Instance.LogOnSessionEnd(numAppointmentsThisSession, currentGameSessionTime);
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

	public void Event_AppointmentEnd(int _numActionsTaken, int _score)
	{
		float appointmentDuration = Time.time - appointmentStartTime;

		MetricsLogger.Instance.LogOnAppointmentEnd(
			GetClipboard().GetNextLevelUp().GetMyDayIndex(),
			GetClipboard().GetNextLevelUp().GetMyLevelIndex(),
			_numActionsTaken,
			_score,
			appointmentDuration);
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
		if (GetClipboard() == null)
		{
			return "-1--1";
		}
		else
		{
			return GetClipboard().GetNextLevelUp().GetMyDayIndex().ToString("D2") + "-" + GetClipboard().GetNextLevelUp().GetMyLevelIndex().ToString("D2");
		}
	}

	public int GetDay()
	{
		return GetClipboard().GetNextLevelUp().GetMyDayIndex();
	}

	public int GetLevel()
	{
		return GetClipboard().GetNextLevelUp().GetMyLevelIndex();
	}
}
