using UnityEngine;
using System.Collections;
using GameAnalyticsSDK;

public class MetricsLogger : MonoBehaviour {

    // singleton
	private static MetricsLogger instance;
    // constructor
	private MetricsLogger() { }
    // instance
	public static MetricsLogger Instance
    {
        get
        {
            if (instance == null)
            {
				instance = GameObject.FindObjectOfType(typeof(MetricsLogger)) as MetricsLogger;
            }
            return instance;
        }
    }

	public void LogCustomEvent(string _01, string _02, string _03, float score)
	{
		GameAnalytics.NewDesignEvent(_01 + ":" + _02 + ":" + _03, score);
	}

	public void LogCustomEvent(string _01, string _02, string _03)
	{
		GameAnalytics.NewDesignEvent(_01 + ":" + _02 + ":" + _03);
	}

	public void LogBusinessEvent(string _currency, int _amount, string _itemType, string _itemId, string _cartType, string _receipt, string _signature)
	{
		GameAnalytics.NewBusinessEventGooglePlay(_currency, _amount, _itemType, _itemId, _cartType, _receipt, _signature);
	}

	public void LogProgressionEvent_Complete(string _01)
	{
		GameAnalytics.NewProgressionEvent(GA_Progression.GAProgressionStatus.GAProgressionStatusComplete, _01);
	}

	public void LogProgressionEvent_Start(string _01)
	{
		GameAnalytics.NewProgressionEvent(GA_Progression.GAProgressionStatus.GAProgressionStatusStart, _01);
	}
}
