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
		if (currentClipboard == null)
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
		if (currentClipboard == null)
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
		appointmentStartTime = Time.time;
		numAppointmentsThisSession++;
	}

	public void Event_AppointmentReset()
	{
		appointmentStartTime = Time.time;
	}

	public void Event_AppointmentEnd(string _level)
	{
		float appointmentDuration = Time.time - appointmentStartTime;

		MetricsLogger.Instance.LogCustomEvent("Appointment", "AppointmentLength", _level, appointmentDuration);
	}

	#endregion

	void OnApplicationQuit()
	{
		// log num appointments played this session
		MetricsLogger.Instance.LogCustomEvent("GameSession", "AppointmentsPlayedThisSession", "", numAppointmentsThisSession);
		// log this session play time
		MetricsLogger.Instance.LogCustomEvent("GameSession", "PlayTimeThisGameSession", "", currentGameSessionTime);
	}
}
