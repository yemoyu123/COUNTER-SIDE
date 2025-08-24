using System;
using System.Collections.Generic;
using ClientPacket.Event;
using NKC.Advertise;
using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine.UI;

namespace NKC;

public static class NKCAdManager
{
	private static List<NKMADItemRewardInfo> itemRewardInfos = new List<NKMADItemRewardInfo>();

	private static List<NKM_INVENTORY_EXPAND_TYPE> inventoryExpandRewardInfos = new List<NKM_INVENTORY_EXPAND_TYPE>();

	public static NKMADItemRewardInfo GetItemRewardInfo(int itemId)
	{
		return itemRewardInfos.Find((NKMADItemRewardInfo e) => e.adItemId == itemId);
	}

	public static void SetItemRewardInfo(List<NKMADItemRewardInfo> _itemRewardInfos)
	{
		itemRewardInfos = _itemRewardInfos;
	}

	public static void SetInventoryExpandRewardInfo(List<NKM_INVENTORY_EXPAND_TYPE> _inventoryExpandRewardInfos)
	{
		inventoryExpandRewardInfos = _inventoryExpandRewardInfos;
	}

	public static void UpdateItemRewardInfo(NKMADItemRewardInfo itemRewardInfo)
	{
		int num = itemRewardInfos.FindIndex((NKMADItemRewardInfo e) => e.adItemId == itemRewardInfo.adItemId);
		if (num < 0 || num >= itemRewardInfos.Count)
		{
			itemRewardInfos.Add(itemRewardInfo);
		}
		else
		{
			itemRewardInfos[num] = itemRewardInfo;
		}
	}

	public static void UpdateInventoryRewardInfo(NKM_INVENTORY_EXPAND_TYPE inventoryType)
	{
		int num = inventoryExpandRewardInfos.FindIndex((NKM_INVENTORY_EXPAND_TYPE e) => e == inventoryType);
		if (num < 0 || num >= inventoryExpandRewardInfos.Count)
		{
			inventoryExpandRewardInfos.Add(inventoryType);
		}
	}

	public static bool IsOpenTagEnabled()
	{
		return NKMADTemplet.EnableByTag;
	}

	public static bool IsAdRewardItem(int itemId)
	{
		NKMADTemplet nKMADTemplet = NKMADTemplet.Find(itemId);
		if (NKMADTemplet.EnableByTag && nKMADTemplet != null && NKMOpenTagManager.IsOpened(nKMADTemplet.OpenTag))
		{
			return NKCAdBase.Instance.IsAdvertiseEnabled();
		}
		return false;
	}

	public static bool IsAdRewardInventory(NKM_INVENTORY_EXPAND_TYPE inventoryType)
	{
		bool flag = false;
		switch (inventoryType)
		{
		case NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT:
			flag = NKMCommonConst.INVENTORY_UNIT_EXPAND_COUNT > 0;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP:
			flag = NKMCommonConst.INVENTORY_EQUIP_EXPAND_COUNT > 0;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP:
			flag = NKMCommonConst.INVENTORY_SHIP_EXPAND_COUNT > 0;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_OPERATOR:
			flag = NKMCommonConst.INVENTORY_OPERATOR_EXPAND_COUNT > 0;
			break;
		}
		if (NKMADTemplet.EnableByTag && flag)
		{
			return NKCAdBase.Instance.IsAdvertiseEnabled();
		}
		return false;
	}

	public static int GetAdItemRewardRemainDailyCount(int itemId)
	{
		return GetItemRewardInfo(itemId)?.remainDailyLimit ?? GetAdItemRewardMaxDailyCount(itemId);
	}

	public static int GetAdItemRewardMaxDailyCount(int itemId)
	{
		return NKMADTemplet.Find(itemId)?.DayLimit ?? 0;
	}

	public static bool AdItemRewardCoolTimeFinished(int itemId)
	{
		NKMADItemRewardInfo itemRewardInfo = GetItemRewardInfo(itemId);
		if (itemRewardInfo == null)
		{
			return true;
		}
		if (itemRewardInfo.remainDailyLimit <= 0)
		{
			return false;
		}
		NKMADTemplet nKMADTemplet = NKMADTemplet.Find(itemId);
		if (nKMADTemplet == null)
		{
			return false;
		}
		return NKCSynchronizedTime.IsFinished(NKCSynchronizedTime.ToUtcTime(itemRewardInfo.latestRewardTime.AddSeconds(nKMADTemplet.WatchCoolTime + 1)));
	}

	public static TimeSpan GetAdItemRewardCoolTime(int itemId)
	{
		NKMADItemRewardInfo itemRewardInfo = GetItemRewardInfo(itemId);
		if (itemRewardInfo == null)
		{
			return new TimeSpan(0, 0, 0);
		}
		NKMADTemplet nKMADTemplet = NKMADTemplet.Find(itemId);
		if (nKMADTemplet == null)
		{
			return new TimeSpan(long.MaxValue);
		}
		return NKCSynchronizedTime.GetTimeLeft(NKCSynchronizedTime.ToUtcTime(itemRewardInfo.latestRewardTime.AddSeconds(nKMADTemplet.WatchCoolTime + 1)));
	}

	public static bool InventoryRewardReceived(NKM_INVENTORY_EXPAND_TYPE inventoryType)
	{
		return inventoryExpandRewardInfos.Contains(inventoryType);
	}

	public static void WatchItemRewardAd(int itemId)
	{
		NKCAdBase.AD_TYPE aD_TYPE = NKCAdBase.AD_TYPE.CREDIT;
		switch (itemId)
		{
		case 1:
			aD_TYPE = NKCAdBase.AD_TYPE.CREDIT;
			break;
		case 2:
			aD_TYPE = NKCAdBase.AD_TYPE.ETERNIUM;
			break;
		default:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ERROR);
			return;
		}
		NKCAdBase.Instance.WatchRewardedAd(aD_TYPE, () => NKCPacketSender.Send_NKMPacket_AD_ITEM_REWARD_REQ(itemId), OnAdFailedToShow);
	}

	public static void WatchInventoryRewardAd(NKM_INVENTORY_EXPAND_TYPE inventoryType)
	{
		NKCAdBase.AD_TYPE aD_TYPE = NKCAdBase.AD_TYPE.UNIT_INV;
		switch (inventoryType)
		{
		case NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT:
			aD_TYPE = NKCAdBase.AD_TYPE.UNIT_INV;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP:
			aD_TYPE = NKCAdBase.AD_TYPE.EQUIP_INV;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP:
			aD_TYPE = NKCAdBase.AD_TYPE.SHIP_INV;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_OPERATOR:
			aD_TYPE = NKCAdBase.AD_TYPE.OPERATOR_INV;
			break;
		default:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ERROR);
			return;
		}
		NKCAdBase.Instance.WatchRewardedAd(aD_TYPE, () => NKCPacketSender.Send_NKMPacket_AD_INVENTORY_EXPAND_REQ(inventoryType), OnAdFailedToShow);
	}

	private static void OnAdFailedToShow(NKCAdBase.NKC_ADMOB_ERROR_CODE resultCode, string message)
	{
		switch (resultCode)
		{
		case NKCAdBase.NKC_ADMOB_ERROR_CODE.NARC_FAILED_TO_LOAD:
			if (NKCDefineManager.DEFINE_USE_CHEAT())
			{
				string content2 = (string.IsNullOrEmpty(message) ? NKCUtilString.GET_STRING_AD_FAILED_TO_LOAD : (NKCUtilString.GET_STRING_AD_FAILED_TO_LOAD + "\n" + message));
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, content2);
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_AD_FAILED_TO_LOAD);
			}
			break;
		case NKCAdBase.NKC_ADMOB_ERROR_CODE.NARC_FAILED_TO_SHOW:
			if (NKCDefineManager.DEFINE_USE_CHEAT())
			{
				string content = (string.IsNullOrEmpty(message) ? NKCUtilString.GET_STRING_AD_FAILED_TO_SHOW : (NKCUtilString.GET_STRING_AD_FAILED_TO_SHOW + "\n" + message));
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, content);
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_AD_FAILED_TO_SHOW);
			}
			break;
		default:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_ERROR);
			break;
		}
	}

	public static void SetItemRewardAdButtonState(NKCPopupItemBox.eMode mode, int itemId, NKCUIComStateButton btnAdOn, NKCUIComStateButton btnAdOff, Text lbAdRemainCount)
	{
		if (mode != NKCPopupItemBox.eMode.MoveToShop || !IsAdRewardItem(itemId))
		{
			NKCUtil.SetGameobjectActive(btnAdOn, bValue: false);
			NKCUtil.SetGameobjectActive(btnAdOff, bValue: false);
			return;
		}
		if (GetAdItemRewardRemainDailyCount(itemId) <= 0)
		{
			NKCUtil.SetGameobjectActive(btnAdOn, bValue: false);
			NKCUtil.SetGameobjectActive(btnAdOff, bValue: true);
			return;
		}
		NKCUtil.SetGameobjectActive(btnAdOn, bValue: true);
		NKCUtil.SetGameobjectActive(btnAdOff, bValue: false);
		NKCUtil.SetLabelText(lbAdRemainCount, $"({GetAdItemRewardRemainDailyCount(itemId)}/{GetAdItemRewardMaxDailyCount(itemId)})");
		bool flag = AdItemRewardCoolTimeFinished(itemId);
		btnAdOn?.SetLock(!flag);
	}

	public static void UpdateItemRewardAdCoolTime(int itemId, NKCUIComStateButton btnAdOn, NKCUIComStateButton btnAdOff, Text lbAdCoolTime, Text lbAdRemainCount)
	{
		if (!NKCAdBase.Instance.IsAdvertiseEnabled())
		{
			return;
		}
		bool flag = btnAdOn != null && btnAdOn.gameObject.activeSelf && btnAdOn.m_bLock;
		bool flag2 = btnAdOff != null && btnAdOff.gameObject.activeSelf;
		if (!flag && !flag2)
		{
			return;
		}
		TimeSpan adItemRewardCoolTime = GetAdItemRewardCoolTime(itemId);
		if (flag)
		{
			if (adItemRewardCoolTime.Ticks > 0)
			{
				NKCUtil.SetLabelText(lbAdCoolTime, NKCSynchronizedTime.GetTimeSpanString(adItemRewardCoolTime) ?? "");
				return;
			}
			NKCUtil.SetLabelText(lbAdRemainCount, $"({GetAdItemRewardRemainDailyCount(itemId)}/{GetAdItemRewardMaxDailyCount(itemId)})");
			btnAdOn?.SetLock(value: false);
		}
		else if (flag2 && adItemRewardCoolTime.Ticks <= 0)
		{
			SetItemRewardAdButtonState(NKCPopupItemBox.eMode.MoveToShop, itemId, btnAdOn, btnAdOff, lbAdRemainCount);
		}
	}
}
