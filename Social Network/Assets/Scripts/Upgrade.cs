using UnityEngine;
using System.Collections;

public static class Upgrade {

	public static bool hasVerifiedUpgrade = false;
	public static int dayLocked = 8;
	public static int dayToWarn = 6;

	public static void PurchaseUpgrade(int paymentOption)
	{
		// TODO: send player to app store for order transaction and confirmation

		// for now just save to player save data
		SaveGame.SetCustomString("hasUpgraded", "true");

		MonoBehaviour.print("Purchasing upgrade!");
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
