using UnityEngine;
using System.Collections;
using Soomla.Store;

public static class Upgrade {

	public static bool hasVerifiedUpgrade = false;
	public static int dayLocked = 8;
	public static int dayToRequestRating1 = 6;
	public static int dayToRequestRating2 = 11;

	public static void PurchaseUpgrade(int paymentOption)
	{
		// purchase upgrade through Soomla store
		StoreInventory.BuyItem(SoomlaStoreAssets.UNLOCK_ALL_LEVELS_ITEM_ID);
		MonoBehaviour.print("Purchasing upgrade!");

#if UNITY_EDITOR
		// if you're in the editor, just send the callback immediately
		PurchaseUpgrade_callback(true);
		MonoBehaviour.print("Just approve it (unity editor)");
#endif

	}

	public static void PurchaseUpgrade_callback(bool success)
	{
		MonoBehaviour.print("Purchasing callback received");
		SaveGame.SetHasUpgraded(true);
		hasVerifiedUpgrade = true;
	}

	public static void RestorePurchase_callback(bool success)
	{
		MonoBehaviour.print("Restore Purchase callback received");
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
		if (SaveGame.GetHasUpgraded())
		{
			hasVerifiedUpgrade = true;
			return true;
		}
		return false;
	}
}
