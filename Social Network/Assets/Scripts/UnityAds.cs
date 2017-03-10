using UnityEngine;
using UnityEngine.Advertisements;

public static class UnityAds
{
	static public int adShowChanceCounter = 0;
	static public int adShowChanceCounterIncrease = 0;

	static public void ShowRewardedAd(int origin)		// 0 == show me
	{
	if (Advertisement.IsReady("rewardedVideo"))
	{
		if (origin == 0)
		{
			var options = new ShowOptions { resultCallback = HandleShowResult_0 };
			Advertisement.Show("rewardedVideo", options);
		}
	}
	}

	static public void ShowPlacementAd()
	{
		Advertisement.Show();
	}
	
	static public void ShowPlacementAdChance(int everyNTimes, int increaseBy)
	{
		adShowChanceCounter++;
		if (adShowChanceCounter % (everyNTimes + adShowChanceCounterIncrease) == 0)
		{
			adShowChanceCounterIncrease += increaseBy;
			adShowChanceCounter = 0;
			ShowPlacementAd();
		}
	}

	static private void HandleShowResult_0(ShowResult result)
	{
	switch (result)
	{
		case ShowResult.Finished:
		Debug.Log("The ad was successfully shown.");
		GameObject.FindObjectOfType<Clipboard>().StartShowMe();
		break;
		case ShowResult.Skipped:
		Debug.Log("The ad was skipped before reaching the end.");
		break;
		case ShowResult.Failed:
		Debug.LogError("The ad failed to be shown.");
		break;
	}
	}
}