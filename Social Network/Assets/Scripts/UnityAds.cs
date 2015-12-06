using UnityEngine;
using UnityEngine.Advertisements;

public static class UnityAds
{
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