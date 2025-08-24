using System.Collections.Generic;
using ClientPacket.Shop;
using Cs.Logging;
using NKC.Publisher;
using NKM;
using NKM.Shop;

namespace NKC;

public static class NKCMMPManager
{
	private const string logPrefix = "NKCMMPManager";

	private static bool m_enableLogging = false;

	private static bool m_started = false;

	private static Dictionary<int, string> m_playCutSceneEvent = new Dictionary<int, string>();

	private static Dictionary<int, string> m_warFareResultEvent = new Dictionary<int, string>();

	private static Dictionary<int, string> m_playTutorialEvent = new Dictionary<int, string>();

	private static Dictionary<int, string> m_dungeonEnterEvent = new Dictionary<int, string>();

	private static Dictionary<int, string> m_dungeonClearEvent = new Dictionary<int, string>();

	private static Dictionary<int, string> m_cashItemPurchaseEvent = new Dictionary<int, string>();

	private static Dictionary<int, string> m_userLevelUpEvent = new Dictionary<int, string>();

	private static NKCMMPModule module = null;

	public static void Init()
	{
		if (!m_started)
		{
			if (module == null)
			{
				Log.Debug("[NKCMMPManager] StartManual - [NKCMMPManager] Symbol not defined", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 48);
				return;
			}
			m_enableLogging = true;
			module.Init(m_enableLogging);
			RegisterEventID();
			m_started = true;
		}
	}

	public static void RegisterEventID()
	{
		m_playCutSceneEvent.Clear();
		AddCutSceneEvent(7, "07_prologue");
		AddCutSceneEvent(8, "08_ep1_1_1_stage_end");
		AddCutSceneEvent(9, "10_ep1_1_2_stage_start");
		AddCutSceneEvent(11, "11_ep1_1_3_stage_start");
		AddCutSceneEvent(13, "12_ep1_1_4_stage_start");
		AddCutSceneEvent(14, "13_ep1_1_4_cutscene2_start");
		AddCutSceneEvent(17, "19_ep1_2_1_missionstart");
		AddCutSceneEvent(18, "20_ep1_2_1_cutscene1_start");
		AddCutSceneEvent(19, "21_ep1_2_1_stage_start");
		AddCutSceneEvent(20, "22_ep1_2_1_stage_result");
		AddCutSceneEvent(21, "23_ep1_2_2_stage_start");
		AddCutSceneEvent(23, "24_ep1_2_3_stage_start");
		AddCutSceneEvent(24, "25_ep1_2_4_stage_start");
		AddCutSceneEvent(25, "26_ep1_2_4_cutscene2_start");
		m_warFareResultEvent.Clear();
		m_dungeonEnterEvent.Clear();
		AddDungeonEnterEvent(1004, "09_ep1_1_1_stage_start");
		m_dungeonClearEvent.Clear();
		m_playTutorialEvent.Clear();
		AddTutorialEvent(721, "14_tutorial_hq1_start");
		AddTutorialEvent(732, "16_tutorial_hq2_ceooffice");
		AddTutorialEvent(181, "17_tutorial_gacha_newbie");
		AddTutorialEvent(460, "18_tutorial_missions");
		AddTutorialEvent(120, "27_gacha_newbie_special");
		m_cashItemPurchaseEvent.Clear();
		AddUserLevelUpEvent(2, "28_level_02");
		AddUserLevelUpEvent(3, "29_level_03");
		AddUserLevelUpEvent(4, "30_level_04");
		AddUserLevelUpEvent(5, "31_level_05");
		AddUserLevelUpEvent(6, "32_level_06");
		AddUserLevelUpEvent(7, "33_level_07");
		AddUserLevelUpEvent(8, "34_level_08");
		AddUserLevelUpEvent(9, "35_level_09");
		AddUserLevelUpEvent(10, "36_level_10");
		AddUserLevelUpEvent(20, "37_level_20");
		AddUserLevelUpEvent(30, "38_level_30");
		AddUserLevelUpEvent(40, "39_level_40");
		AddUserLevelUpEvent(50, "40_level_50");
	}

	private static void AddCutSceneEvent(int cutSceneID, string eventID)
	{
		if (!m_playCutSceneEvent.ContainsKey(cutSceneID))
		{
			m_playCutSceneEvent.Add(cutSceneID, eventID);
		}
	}

	private static void AddTutorialEvent(int tutorialID, string eventID)
	{
		if (!m_playTutorialEvent.ContainsKey(tutorialID))
		{
			m_playTutorialEvent.Add(tutorialID, eventID);
		}
	}

	private static void AddCashItemPurchaseEvent(int productID, string eventID)
	{
		if (!m_cashItemPurchaseEvent.ContainsKey(productID))
		{
			m_cashItemPurchaseEvent.Add(productID, eventID);
		}
	}

	private static void AddDungeonEnterEvent(int dungeonID, string eventID)
	{
		if (!m_dungeonEnterEvent.ContainsKey(dungeonID))
		{
			m_dungeonEnterEvent.Add(dungeonID, eventID);
		}
	}

	private static void AddUserLevelUpEvent(int level, string eventID)
	{
		if (!m_userLevelUpEvent.ContainsKey(level))
		{
			m_userLevelUpEvent.Add(level, eventID);
		}
	}

	public static void OnCustomEvent(string eventId)
	{
		if (module != null)
		{
			if (m_enableLogging)
			{
				Log.Debug("[NKCMMPManager] OnCustomEvent EventID[" + eventId + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 167);
			}
			SendMMPEvent(eventId);
		}
	}

	public static void OnPlayCutScene(int cutSceneID)
	{
		if (module != null)
		{
			if (m_enableLogging)
			{
				Log.Debug(string.Format("[{0}] OnPlayCutScene CutSceneID[{1}]", "NKCMMPManager", cutSceneID), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 182);
			}
			if (m_playCutSceneEvent.TryGetValue(cutSceneID, out var value))
			{
				SendMMPEvent(value);
			}
		}
	}

	public static void OnWarfareResult(int warfareID)
	{
		if (module != null)
		{
			if (m_enableLogging)
			{
				Log.Debug(string.Format("[{0}] OnWarfareResult warfareID[{1}]", "NKCMMPManager", warfareID), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 200);
			}
			if (m_warFareResultEvent.TryGetValue(warfareID, out var value))
			{
				SendMMPEvent(value);
			}
		}
	}

	public static void OnPlayTutorial(int tutorialID)
	{
		if (module != null)
		{
			if (m_enableLogging)
			{
				Log.Debug(string.Format("[{0}] OnPlayTutorial eventID[{1}]", "NKCMMPManager", tutorialID), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 218);
			}
			if (m_playTutorialEvent.TryGetValue(tutorialID, out var value))
			{
				SendMMPEvent(value);
			}
		}
	}

	public static void OnEnterDungeon(int dungeonID)
	{
		if (module != null)
		{
			if (m_enableLogging)
			{
				Log.Debug(string.Format("[{0}] OnEnterDungeon dungeonID[{1}]", "NKCMMPManager", dungeonID), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 236);
			}
			if (m_dungeonEnterEvent.TryGetValue(dungeonID, out var value))
			{
				SendMMPEvent(value);
			}
		}
	}

	public static void OnClearDungeon(int dungeonID)
	{
		if (module != null)
		{
			if (m_enableLogging)
			{
				Log.Debug(string.Format("[{0}] OnClearDungeon dungeonID[{1}]", "NKCMMPManager", dungeonID), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 254);
			}
			if (m_dungeonClearEvent.TryGetValue(dungeonID, out var value))
			{
				SendMMPEvent(value);
			}
		}
	}

	public static void OnUserLevelUp(int level)
	{
		if (module != null)
		{
			if (m_enableLogging)
			{
				Log.Debug(string.Format("[{0}] OnUserLevelUp level[{1}]", "NKCMMPManager", level), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 272);
			}
			if (m_userLevelUpEvent.TryGetValue(level, out var value))
			{
				SendMMPEvent(value);
			}
		}
	}

	public static void SendMMPEvent(string eventId)
	{
		if (module == null)
		{
			Log.Debug("[NKCMMPManager] MMPModule is null [" + eventId + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 285);
		}
		else
		{
			module.SendEvent(eventId);
		}
	}

	public static void SendMMPRevenueEvent(string eventId, int productId, double localPrice, string priceCurrency)
	{
		if (module == null)
		{
			Log.Debug("[NKCMMPManager] MMPModule is null [" + eventId + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 296);
		}
		else
		{
			module.SendRevenueEvent(eventId, productId, localPrice, priceCurrency);
		}
	}

	public static void OnTrackPurchase(ShopItemTemplet productTemplet)
	{
		if (module == null || productTemplet == null)
		{
			return;
		}
		if (m_enableLogging)
		{
			Log.Debug(string.Format("[{0}] OnTrackPurchase ProductID[{1}] PriceItemID[{2}]", "NKCMMPManager", productTemplet.m_ProductID, productTemplet.m_PriceItemID), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 330);
		}
		if (productTemplet.m_PriceItemID == 0)
		{
			double num = 0.0;
			string text = null;
			num = decimal.ToDouble(NKCPublisherModule.InAppPurchase.GetLocalPrice(productTemplet.m_MarketID, productTemplet.m_ProductID));
			text = NKCPublisherModule.InAppPurchase.GetPriceCurrency(productTemplet.m_MarketID, productTemplet.m_ProductID);
			SendMMPRevenueEvent("37_inapp_purchase", productTemplet.m_ProductID, num, text);
		}
		if (productTemplet.m_PriceItemID == 102)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null && myUserData.m_ShopData != null && myUserData.m_ShopData.histories != null)
			{
				int num2 = 0;
				foreach (NKMShopPurchaseHistory value2 in myUserData.m_ShopData.histories.Values)
				{
					if (value2 != null && ShopItemTemplet.Find(value2.shopId).m_PriceItemID == 102)
					{
						num2 += value2.purchaseTotalCount;
					}
				}
				if (num2 == 1)
				{
					SendMMPEvent("44_first_coin_use");
				}
			}
		}
		if (m_cashItemPurchaseEvent.TryGetValue(productTemplet.m_ProductID, out var value))
		{
			SendMMPEvent(value);
		}
	}

	public static void HandleGooglePlayId(string adId)
	{
		Log.Debug("Google Play Ad ID = " + adId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPManager.cs", 382);
	}
}
