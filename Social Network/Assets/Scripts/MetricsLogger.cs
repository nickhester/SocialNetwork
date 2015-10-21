﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using GameAnalyticsSDK;
using UnityEngine.Analytics;

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

	public void SendDataOnStart()
	{
		//Analytics.SetUserGender(Gender.Male);
		//Analytics.SetUserBirthYear(1990);
	}

	public void LogCustomEvent(string _01, string _02, string _03, float score)
	{
		//GameAnalytics.NewDesignEvent(_01 + ":" + _02 + ":" + _03, score);
		Analytics.CustomEvent(_01 + ":" + _02 + ":" + _03, new Dictionary<string, object>() 
		{
			{ "data0", score }
		});
	}

	public void LogCustomEvent(string _01, string _02, string _03)
	{
		//GameAnalytics.NewDesignEvent(_01 + ":" + _02 + ":" + _03);
		Analytics.CustomEvent(_01 + ":" + _02 + ":" + _03, new Dictionary<string, object>() 
		{
			{ "data1", 0 }
		});
	}

	public void LogBusinessEvent(string _currency, int _amountInCents, string _itemType, string _itemId, string _cartType, string _receipt, string _signature)
	{
		// if you switch back to GameAnalytics, then you need to convert the currency to number of cents
		//GameAnalytics.NewBusinessEventGooglePlay(_currency, _amountInCents, _itemType, _itemId, _cartType, _receipt, _signature);
		Analytics.Transaction(_itemType, ((decimal)_amountInCents / 100.0m), "USD", _receipt, _signature);
	}

	public void LogProgressionEvent_Complete(string _01)
	{
		//GameAnalytics.NewProgressionEvent(GA_Progression.GAProgressionStatus.GAProgressionStatusComplete, _01);
	}

	public void LogProgressionEvent_Start(string _01)
	{
		//GameAnalytics.NewProgressionEvent(GA_Progression.GAProgressionStatus.GAProgressionStatusStart, _01);
	}
}
