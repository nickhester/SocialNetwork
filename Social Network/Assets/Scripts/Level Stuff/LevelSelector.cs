using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour {

	private CalendarDay _dayToGenerate;
	public CalendarDay dayToGenerate
	{
		get
		{
			_dayToGenerate.gameObject.SetActive(false);
			return this._dayToGenerate;
		}
		private set
		{
		}
	}

	void Start ()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void StartDay(CalendarDay _calendarDay)
	{
		foreach (Transform child in _calendarDay.transform.GetComponentsInChildren<Transform>())
		{
			if (child != _calendarDay.transform) {  }
		}
		_calendarDay.transform.parent = null;		// unparent calendar day so it doesn't get destroyed with the rest of them
		DontDestroyOnLoad(_calendarDay.gameObject);
		SceneManager.LoadScene("Scene_Clipboard");
		_dayToGenerate = _calendarDay;
	}
}