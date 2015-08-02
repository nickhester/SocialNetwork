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

	public void LogMetric(string metricName, int value)
	{
		print(metricName + " " + value);
	}

	public void LogMetric(string metricName, float value)
	{
		print(metricName + " " + value);
	}

	public void LogCustomEvent(string _01, string _02, string _03, float score)
	{
		GameAnalytics.NewDesignEvent(_01 + ":" + _02 + ":" + _03, score);
	}

	public void LogBusinessEvent(string _currency, int _amount, string _itemType, string _itemId, string _cartType, string _receipt, string _signature)
	{
		GameAnalytics.NewBusinessEventGooglePlay(_currency, _amount, _itemType, _itemId, _cartType, _receipt, _signature);
	}

	public void LogProgressionEvent(string _01, string _02, string _03, int score)
	{
		GameAnalytics.NewProgressionEvent(GA_Progression.GAProgressionStatus.GAProgressionStatusComplete, _01, _02, _03, score);
	}
}
