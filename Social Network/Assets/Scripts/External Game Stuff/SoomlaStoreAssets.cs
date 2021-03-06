﻿using Soomla.Store;
using System.Collections.Generic;

public class SoomlaStoreAssets : IStoreAssets {

	public const string UNLOCK_ALL_LEVELS_ITEM_ID = "unlock_all_levels_item_id";
	//public const string UNLOCK_ALL_LEVELS_PRODUCT_ID = "android.test.purchased";
#if UNITY_ANDROID
	public const string UNLOCK_ALL_LEVELS_PRODUCT_ID = "unlock_all_levels";
#elif UNITY_IOS
	public const string UNLOCK_ALL_LEVELS_PRODUCT_ID = "upgrade_to_full";
#elif UNITY_EDITOR
	public const string UNLOCK_ALL_LEVELS_PRODUCT_ID = "unlock_all_levels";
#endif

	public int GetVersion()
	{
		return 3;
	}

	public VirtualCurrency[] GetCurrencies()
	{
		return new VirtualCurrency[] { };
	}

	public VirtualGood[] GetGoods()
	{
		return new VirtualGood[] { UNLOCK_ALL_LEVELS };
	}

	public VirtualCurrencyPack[] GetCurrencyPacks()
	{
		return new VirtualCurrencyPack[] { };
	}

	public VirtualCategory[] GetCategories()
	{
		return new VirtualCategory[] { GENERAL_CATEGORY };
	}

	public static VirtualGood UNLOCK_ALL_LEVELS = new LifetimeVG(
		"Unlock All Levels",					// Name
		"Unlock All Levels",					// Description
		UNLOCK_ALL_LEVELS_ITEM_ID,						// Item ID
		new PurchaseWithMarket(					// Purchase type (with real money $)
			UNLOCK_ALL_LEVELS_PRODUCT_ID,					// Product ID
			1.99								// Price (in real money $)
		)
	);

	public static VirtualCategory GENERAL_CATEGORY = new VirtualCategory(
		"General", new List<string>(new string[] { UNLOCK_ALL_LEVELS_ITEM_ID })
	);
}
