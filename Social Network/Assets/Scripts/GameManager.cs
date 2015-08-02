using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private float appointmentStartTime;
	private int numAppointmentsThisSession = 0;
	private Clipboard currentClipboard;

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

	public void Register_Clipboard(Clipboard _myClipboard)
	{
		currentClipboard = _myClipboard;
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

		MetricsLogger.Instance.LogMetric("AppointmentLength " + _level, appointmentDuration);
	}

	#endregion

	void OnApplicationQuit()
	{
		MetricsLogger.Instance.LogMetric("AppointmentsPlayedThisSession ", numAppointmentsThisSession);
	}
}
