using UnityEngine;
using System.Collections;

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
		DontDestroyOnLoad(gameObject);

		RestartSessionMetrics();
	}

	void RestartSessionMetrics()
	{
		numAppointmentsThisSession = 0;
		currentGameSessionTime = 0.0f;
	}

	void SendSessionMetrics()
	{
		// log num appointments played this session
		MetricsLogger.Instance.LogCustomEvent("GameSession", "AppointmentsPlayedThisSession", "", numAppointmentsThisSession);
		// log this session play time
		MetricsLogger.Instance.LogCustomEvent("GameSession", "PlayTimeThisGameSession", "", currentGameSessionTime);
	}

	void SendSessionMetrics2()
	{
		// log num appointments played this session
		MetricsLogger.Instance.LogCustomEvent("GameSession", "AppointmentsPlayedThisSession2", "", numAppointmentsThisSession);
		// log this session play time
		MetricsLogger.Instance.LogCustomEvent("GameSession", "PlayTimeThisGameSession2", "", currentGameSessionTime);
	}

	void SendSessionMetrics3()
	{
		// log num appointments played this session
		MetricsLogger.Instance.LogCustomEvent("GameSession", "AppointmentsPlayedThisSession3", "close", numAppointmentsThisSession);
		// log this session play time
		MetricsLogger.Instance.LogCustomEvent("GameSession", "PlayTimeThisGameSession3", "close", currentGameSessionTime);
	}

	void SendSessionMetrics4()
	{
		// log num appointments played this session
		MetricsLogger.Instance.LogCustomEvent("GameSession", "AppointmentsPlayedThisSession4", "close", numAppointmentsThisSession);
		// log this session play time
		MetricsLogger.Instance.LogCustomEvent("GameSession", "PlayTimeThisGameSession4", "close", currentGameSessionTime);
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
	}

	#endregion
	
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
	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			SendSessionMetrics3();
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
}
