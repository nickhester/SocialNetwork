using UnityEngine;
using System.Collections;

public class levelSelector : MonoBehaviour {

	private CalendarDay _dayToGenerate;
	public CalendarDay dayToGenerate
	{
		get
		{
			_dayToGenerate.GetComponent<Collider>().enabled = false;
			_dayToGenerate.GetComponent<Renderer>().enabled = false;
			foreach (Renderer r in _dayToGenerate.GetComponentsInChildren<Renderer>())
			{
				r.enabled = false;
			}
			return this._dayToGenerate;
		}
		private set
		{
		}
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartDay(CalendarDay _calendarDay)
	{
		foreach (Transform child in _calendarDay.transform.GetComponentsInChildren<Transform>())
		{
			if (child != _calendarDay.transform) {  }
		}
		_calendarDay.transform.parent = null;		// unparent calendar day so it doesn't get destroyed with the rest of them
		DontDestroyOnLoad(_calendarDay.gameObject);
		Application.LoadLevel("Scene_Clipboard");
		_dayToGenerate = _calendarDay;
	}
}