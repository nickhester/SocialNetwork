using UnityEngine;
using System.Collections;
using Soomla.Store;

public static class Upgrade {

	public static bool hasVerifiedUpgrade = false;
	public static int dayLocked = 8;
	public static int dayToWarn = 6;

	public static void PurchaseUpgrade(int paymentOption)
	{
		// purchase upgrade through Soomla store
		StoreInventory.BuyItem(SoomlaStoreAssets.UNLOCK_ALL_LEVELS_ID);
		MonoBehaviour.print("Purchasing upgrade!");
		
#if UNITY_EDITOR
		// if you're in the editor, just send the callback immediately
		PurchaseUpgrade_callback(true);
		MonoBehaviour.print("Just approve it (unity editor)");
#endif

	}

	public static void PurchaseUpgrade_callback(bool success)
	{
		SaveGame.SetCustomString("hasUpgraded", "true");
		hasVerifiedUpgrade = true;
	}

	public static bool canVerifyUpgrade()
	{
		if (!hasVerifiedUpgrade)
		{
			VerifyUpgrade();
		}
		return hasVerifiedUpgrade;
	}

	public static bool VerifyUpgrade()
	{
		// TODO: verify upgrade from app store
		if (SaveGame.GetCustomString("hasUpgraded") == "true")
		{
			hasVerifiedUpgrade = true;
			return true;
		}
		return false;
	}
}
