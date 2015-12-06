using UnityEngine;
using System.Collections;

public static class ResourceManager {

	static private int numTips;		// calculate this from calendar if needed

	static private int OneStarTipValue = 10;
	static private int TwoStarTipValue = 25;
	static private int ThreeStarTipValue = 50;
	
	public static int GetNumTips()
	{
		return numTips;
	}

	public static void AddTips(int _numTipsToAdd)
	{
		numTips += _numTipsToAdd;
	}

	public static void SpendTips(int _numTipsToSpend)
	{
		if (numTips - _numTipsToSpend < 0)
		{
			Debug.LogError("numTips has gone below zero");
		}
		numTips -= _numTipsToSpend;
	}
}
