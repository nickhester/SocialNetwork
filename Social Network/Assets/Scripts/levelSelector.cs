using UnityEngine;
using System.Collections;

public class levelSelector : MonoBehaviour {

	private CalendarDay _dayToGenerate;
	public CalendarDay dayToGenerate
	{
		get
		{
			_dayToGenerate.collider.enabled = false;
			_dayToGenerate.renderer.enabled = false;
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
			if (child != _calendarDay.transform) { Destroy(child.gameObject); }
		}
		_calendarDay.transform.parent = null;		// unparent calendar day so it doesn't get destroyed with the rest of them
		DontDestroyOnLoad(_calendarDay.gameObject);
		Application.LoadLevel("Scene_Clipboard");
		_dayToGenerate = _calendarDay;
	}
}