using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using Cs.Core.Util;
using Cs.Logging;
using Cs.Shared.Time;
using NKC;
using NKC.UI;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKM;

public static class NKMItemManager
{
	public const int IMI_ITEM_MISC_MAKE_WARFARE_REPAIR = 1016;

	public const int IMI_ITEM_MISC_MAKE_WARFARE_SUPPLY = 1017;

	public const int NKM_ENCHANT_CREDIT_MULTI_VALUE = 8;

	private static Dictionary<int, NKMItemMiscTemplet> resourceTemplets;

	private static Dictionary<int, NKMEquipEnchantExpTemplet> m_dicEquipEnchantExpTemplet = null;

	public static Dictionary<int, NKMEquipTemplet> m_dicItemEquipTempletByID = null;

	public static Dictionary<int, List<int>> m_dicMoldReward = new Dictionary<int, List<int>>();

	public static Dictionary<int, NKMItemEquipSetOptionTemplet> m_dicItemEquipSetOptionTempletByID = null;

	public static List<NKMItemEquipSetOptionTemplet> m_lstItemEquipSetOptionTemplet = null;

	private static List<NKMResetCount> m_ResetCount = new List<NKMResetCount>();

	private static Dictionary<int, NKCRandomMoldBoxTemplet> m_dicRandomMoldBoxTemplet = null;

	public static Dictionary<int, Dictionary<NKM_REWARD_TYPE, List<int>>> m_dicRandomMoldBox = null;

	private static Dictionary<int, NKCRandomMoldTabTemplet> m_dicMoldTabTemplet = null;

	private static Dictionary<int, NKCEquipAutoWeightTemplet> m_dicAutoWeightTemplet = null;

	public static IReadOnlyDictionary<int, NKMItemMiscTemplet> ResourceTemplets => resourceTemplets;

	public static Dictionary<int, NKCRandomMoldBoxTemplet> RandomMoldBoxTemplet => m_dicRandomMoldBoxTemplet;

	public static Dictionary<int, NKCRandomMoldTabTemplet> MoldTapTemplet => m_dicMoldTabTemplet;

	public static Dictionary<int, NKCEquipAutoWeightTemplet> AutoWeightTemplet => m_dicAutoWeightTemplet;

	public static bool LoadFromLUA_Item_Equip(string filename)
	{
		m_dicItemEquipTempletByID = NKMTempletLoader.LoadDictionary("AB_SCRIPT_ITEM_TEMPLET", filename, "ITEM_EQUIP_TEMPLET", NKMEquipTemplet.LoadFromLUA);
		NKMTempletContainer<NKMEquipTemplet>.Load(m_dicItemEquipTempletByID.Values, (NKMEquipTemplet e) => e.m_ItemEquipStrID);
		return m_dicItemEquipTempletByID != null;
	}

	public static bool LoadFromLUA_EquipEnchantExp(string filename)
	{
		m_dicEquipEnchantExpTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT_ITEM_TEMPLET", filename, "EQUIP_ENCHANT_EXP_TABLE", NKMEquipEnchantExpTemplet.LoadFromLUA);
		return m_dicEquipEnchantExpTemplet != null;
	}

	public static void LoadFromLUA_ITEM_MISC(string fileName)
	{
		NKMTempletContainer<NKMItemMiscTemplet>.Load("AB_SCRIPT_ITEM_TEMPLET", fileName, "ITEM_MISC_TEMPLET", NKMItemMiscTemplet.LoadFromLUA, (NKMItemMiscTemplet e) => e.m_ItemMiscStrID);
		CheckValidation();
		resourceTemplets = NKMItemMiscTemplet.Values.Where((NKMItemMiscTemplet e) => e.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_RESOURCE).ToDictionary((NKMItemMiscTemplet e) => e.Key);
	}

	public static void LoadFromLua_Item_Mold(string filename)
	{
		NKMTempletContainer<NKMItemMoldTemplet>.Load("AB_SCRIPT_ITEM_TEMPLET", filename, "ITEM_MOLD_TEMPLET", NKMItemMoldTemplet.LoadFromLUA);
	}

	public static bool LoadFromLUA_EquipSetOption(string filename)
	{
		m_dicItemEquipSetOptionTempletByID = NKMTempletLoader.LoadDictionary("AB_SCRIPT_ITEM_TEMPLET", filename, "ITEM_EQUIP_SET_OPTION", NKMItemEquipSetOptionTemplet.LoadFromLUA);
		bool num = m_dicItemEquipSetOptionTempletByID != null;
		if (num)
		{
			m_lstItemEquipSetOptionTemplet = new List<NKMItemEquipSetOptionTemplet>(m_dicItemEquipSetOptionTempletByID.Values);
		}
		return num;
	}

	public static NKMItemMiscTemplet GetItemMiscTempletByID(int itemMiscID)
	{
		return NKMTempletContainer<NKMItemMiscTemplet>.Find(itemMiscID);
	}

	public static NKMEquipTemplet GetEquipTemplet(int equipID)
	{
		if (m_dicItemEquipTempletByID.TryGetValue(equipID, out var value))
		{
			return value;
		}
		return null;
	}

	public static NKMEquipTemplet GetEquipTemplet(string equipStrID)
	{
		return NKMTempletContainer<NKMEquipTemplet>.Find(equipStrID);
	}

	public static NKMItemMoldTemplet GetItemMoldTempletByID(int moldID)
	{
		return NKMItemMoldTemplet.Find(moldID);
	}

	public static NKMEquipEnchantExpTemplet GetEquipEnchantExpTemplet(int tierId, int equipEnchantLevel)
	{
		int hashCode = (tierId, equipEnchantLevel).GetHashCode();
		m_dicEquipEnchantExpTemplet.TryGetValue(hashCode, out var value);
		return value;
	}

	public static int GetEnchantRequireExp(int target_tier, int target_level, NKM_ITEM_GRADE target_grade)
	{
		NKMEquipEnchantExpTemplet equipEnchantExpTemplet = GetEquipEnchantExpTemplet(target_tier, target_level);
		if (equipEnchantExpTemplet == null)
		{
			return -1;
		}
		return target_grade switch
		{
			NKM_ITEM_GRADE.NIG_N => equipEnchantExpTemplet.m_ReqLevelupExp_N, 
			NKM_ITEM_GRADE.NIG_R => equipEnchantExpTemplet.m_ReqLevelupExp_R, 
			NKM_ITEM_GRADE.NIG_SR => equipEnchantExpTemplet.m_ReqLevelupExp_SR, 
			NKM_ITEM_GRADE.NIG_SSR => equipEnchantExpTemplet.m_ReqLevelupExp_SSR, 
			_ => -1, 
		};
	}

	public static int GetEquipEnchantFeedExp(NKMEquipItemData equipItemData)
	{
		NKMEquipTemplet equipTemplet = GetEquipTemplet(equipItemData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return -1;
		}
		NKMEquipEnchantExpTemplet equipEnchantExpTemplet = GetEquipEnchantExpTemplet(equipTemplet.m_NKM_ITEM_TIER, equipItemData.m_EnchantLevel);
		if (equipEnchantExpTemplet == null)
		{
			return -1;
		}
		float reqEnchantFeedEXPBonusRate = equipEnchantExpTemplet.m_ReqEnchantFeedEXPBonusRate;
		return (int)((float)equipTemplet.m_FeedEXP * reqEnchantFeedEXPBonusRate);
	}

	public static NKM_ERROR_CODE CanEnchantItem(NKMUserData userdata, NKMEquipItemData target_equip_item)
	{
		return userdata.m_ArmyData.GetUnitDeckState(target_equip_item.m_OwnerUnitUID) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKM_ERROR_CODE.NEC_OK, 
		};
	}

	public static NKM_ERROR_CODE CanEnchantItem(NKMUserData userdata, NKMEquipItemData target_equip_item, List<long> material_item_list, NKMSupportUnitData supportUnitData = null)
	{
		if (material_item_list == null || material_item_list.Count <= 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_ITEM_UID;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = CanEnchantItem(userdata, target_equip_item);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		int num = 0;
		foreach (long item in material_item_list)
		{
			if (item == target_equip_item.m_ItemUid)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INVALID_ITEM_UID;
			}
			NKMEquipItemData itemEquip = userdata.m_InventoryData.GetItemEquip(item);
			if (itemEquip == null)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INVALID_ITEM_UID;
			}
			nKM_ERROR_CODE = CanRemoveItem(itemEquip, supportUnitData);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE;
			}
			int equipEnchantFeedExp = GetEquipEnchantFeedExp(itemEquip);
			if (equipEnchantFeedExp == -1)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INVALID_EQUIP_ITEM;
			}
			num += equipEnchantFeedExp;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanRemoveItem(NKMEquipItemData equip_item_data, NKMSupportUnitData supportUnitData = null)
	{
		if (equip_item_data.m_bLock)
		{
			return NKM_ERROR_CODE.NEC_FAIL_ITEM_LOCKED;
		}
		if (equip_item_data.m_OwnerUnitUID > 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_EQUIP_ITEM;
		}
		if (supportUnitData != null && supportUnitData.asyncUnitEquip != null && supportUnitData.asyncUnitEquip.equips.Where((NKMEquipItemData e) => e.m_ItemUid == equip_item_data.m_ItemUid).FirstOrDefault() != null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_CONTAIN_SUPPORT_UNIT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static bool IsRedudantItemProhibited(NKM_REWARD_TYPE rewardType, int itemID)
	{
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_MISC:
		{
			NKMItemMiscTemplet itemMiscTempletByID = GetItemMiscTempletByID(itemID);
			if (itemMiscTempletByID == null)
			{
				return false;
			}
			return IsRedudantItemProhibited(itemMiscTempletByID.m_ItemMiscType, itemMiscTempletByID.m_ItemMiscSubType);
		}
		case NKM_REWARD_TYPE.RT_SKIN:
		case NKM_REWARD_TYPE.RT_EMOTICON:
			return true;
		default:
			return false;
		}
	}

	public static bool IsRedudantItemProhibited(NKM_ITEM_MISC_TYPE itemType, NKM_ITEM_MISC_SUBTYPE subType)
	{
		switch (itemType)
		{
		case NKM_ITEM_MISC_TYPE.IMT_EMBLEM:
		case NKM_ITEM_MISC_TYPE.IMT_EMBLEM_RANK:
		case NKM_ITEM_MISC_TYPE.IMT_BACKGROUND:
		case NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME:
		case NKM_ITEM_MISC_TYPE.IMT_TITLE:
			return true;
		case NKM_ITEM_MISC_TYPE.IMT_INTERIOR:
			return subType == NKM_ITEM_MISC_SUBTYPE.IMST_INTERIOR_DECO;
		default:
			return false;
		}
	}

	public static NKMItemEquipSetOptionTemplet GetEquipSetOptionTemplet(int optionID)
	{
		if (m_lstItemEquipSetOptionTemplet != null && m_lstItemEquipSetOptionTemplet.Count > 0)
		{
			return m_lstItemEquipSetOptionTemplet.Find((NKMItemEquipSetOptionTemplet e) => e.m_EquipSetID == optionID);
		}
		return null;
	}

	public static List<NKMItemEquipSetOptionTemplet> GetActivatedSetItem(NKMUnitData unitData, NKMInventoryData inventoryData)
	{
		return GetActivatedSetItem(unitData.GetEquipmentSet(inventoryData));
	}

	public static List<NKMItemEquipSetOptionTemplet> GetActivatedSetItem(NKMEquipmentSet equipmentSet)
	{
		List<NKMItemEquipSetOptionTemplet> list = new List<NKMItemEquipSetOptionTemplet>();
		if (equipmentSet == null)
		{
			return list;
		}
		List<NKMEquipItemData> list2 = new List<NKMEquipItemData>();
		if (equipmentSet.Weapon != null && equipmentSet.Weapon.m_SetOptionId != 0)
		{
			list2.Add(equipmentSet.Weapon);
		}
		if (equipmentSet.Defence != null && equipmentSet.Defence.m_SetOptionId != 0)
		{
			list2.Add(equipmentSet.Defence);
		}
		if (equipmentSet.Accessory != null && equipmentSet.Accessory.m_SetOptionId != 0)
		{
			list2.Add(equipmentSet.Accessory);
		}
		if (equipmentSet.Accessory2 != null && equipmentSet.Accessory2.m_SetOptionId != 0)
		{
			list2.Add(equipmentSet.Accessory2);
		}
		foreach (NKMEquipItemData equipItem in list2)
		{
			if (list.Find((NKMItemEquipSetOptionTemplet e) => e.m_EquipSetID == equipItem.m_SetOptionId) != null)
			{
				continue;
			}
			List<NKMEquipItemData> list3 = list2.FindAll((NKMEquipItemData e) => e.m_SetOptionId == equipItem.m_SetOptionId);
			NKMItemEquipSetOptionTemplet equipSetOptionTemplet = GetEquipSetOptionTemplet(equipItem.m_SetOptionId);
			if (equipSetOptionTemplet == null)
			{
				Log.Error("SetOptionTemplet not exist! SetOptionID : " + equipItem.m_SetOptionId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 790);
				continue;
			}
			int num = list3.Count / equipSetOptionTemplet.m_EquipSetPart;
			for (int num2 = 0; num2 < num; num2++)
			{
				list.Add(equipSetOptionTemplet);
			}
		}
		return list;
	}

	public static HashSet<long> GetSetItemActivatedMark(NKMEquipmentSet equipmentSet)
	{
		HashSet<long> hashSet = new HashSet<long>();
		if (equipmentSet == null)
		{
			return hashSet;
		}
		List<NKMEquipItemData> list = new List<NKMEquipItemData>();
		if (equipmentSet.Weapon != null && equipmentSet.Weapon.m_SetOptionId != 0)
		{
			list.Add(equipmentSet.Weapon);
		}
		if (equipmentSet.Defence != null && equipmentSet.Defence.m_SetOptionId != 0)
		{
			list.Add(equipmentSet.Defence);
		}
		if (equipmentSet.Accessory != null && equipmentSet.Accessory.m_SetOptionId != 0)
		{
			list.Add(equipmentSet.Accessory);
		}
		if (equipmentSet.Accessory2 != null && equipmentSet.Accessory2.m_SetOptionId != 0)
		{
			list.Add(equipmentSet.Accessory2);
		}
		HashSet<long> hashSet2 = new HashSet<long>();
		foreach (NKMEquipItemData equipItem in list)
		{
			if (hashSet2.Contains(equipItem.m_ItemUid))
			{
				continue;
			}
			List<NKMEquipItemData> list2 = list.FindAll((NKMEquipItemData e) => e.m_SetOptionId == equipItem.m_SetOptionId);
			NKMItemEquipSetOptionTemplet equipSetOptionTemplet = GetEquipSetOptionTemplet(equipItem.m_SetOptionId);
			if (equipSetOptionTemplet == null)
			{
				Log.Error("SetOptionTemplet not exist! SetOptionID : " + equipItem.m_SetOptionId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 839);
				continue;
			}
			int num = list2.Count / equipSetOptionTemplet.m_EquipSetPart;
			for (int num2 = 0; num2 < list2.Count; num2++)
			{
				hashSet2.Add(list2[num2].m_ItemUid);
				if (num2 < num * equipSetOptionTemplet.m_EquipSetPart)
				{
					hashSet.Add(list2[num2].m_ItemUid);
				}
			}
		}
		return hashSet;
	}

	private static void CheckValidation()
	{
		if (GetItemMiscTempletByID(1) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 크레딧 자원이 존재하지 않음 m_ItemMiscID : {1}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 863);
		}
		if (GetItemMiscTempletByID(2) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 이터니움 자원이 존재하지 않음 m_ItemMiscID : {2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 868);
		}
		if (GetItemMiscTempletByID(3) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 정보 자원이 존재하지 않음 m_ItemMiscID : {3}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 873);
		}
		if (GetItemMiscTempletByID(4) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 연수티켓 자원이 존재하지 않음 m_ItemMiscID : {4}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 878);
		}
		if (GetItemMiscTempletByID(15) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 공격 모의작전 연수티켓 자원이 존재하지 않음 m_ItemMiscID : {15}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 883);
		}
		if (GetItemMiscTempletByID(16) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 방어 모의작전 연수티켓 자원이 존재하지 않음 m_ItemMiscID : {16}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 888);
		}
		if (GetItemMiscTempletByID(17) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 대공 모의작전 연수티켓 자원이 존재하지 않음 m_ItemMiscID : {17}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 893);
		}
		if (GetItemMiscTempletByID(11) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 허수코어 자원이 존재하지 않음 m_ItemMiscID : {11}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 898);
		}
		if (GetItemMiscTempletByID(101) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 캐시 자원이 존재하지 않음 m_ItemMiscID : {101}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 903);
		}
		if (GetItemMiscTempletByID(202) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 업적 점수 자원이 존재하지 않음 m_ItemMiscID : {202}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 908);
		}
		if (GetItemMiscTempletByID(501) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 사장 경험치 자원이 존재하지 않음 m_ItemMiscID : {501}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 913);
		}
		if (GetItemMiscTempletByID(1012) == null)
		{
			NKMTempletError.Add($"[ItemMiscTemplet] 즉시 제작권 아이템이 존재하지 않음 m_ItemMiscID : {1012}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 918);
		}
	}

	public static int GetEnchantRequireExp(NKMEquipItemData equipItemData)
	{
		NKMEquipTemplet equipTemplet = GetEquipTemplet(equipItemData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			Log.ErrorAndExit($"[NKMEquipTemplet] Fail - Item Equip ID : {equipItemData.m_ItemEquipID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMItemManagerEx.cs", 428);
			return -1;
		}
		return GetEnchantRequireExp(equipTemplet.m_NKM_ITEM_TIER, equipItemData.m_EnchantLevel, equipTemplet.m_NKM_ITEM_GRADE);
	}

	public static int GetMaxEquipEnchantLevel(int equipTier)
	{
		int i;
		for (i = 0; GetEquipEnchantExpTemplet(equipTier, i) != null; i++)
		{
		}
		return i - 1;
	}

	public static int GetMaxEquipEnchantExp(int equipID)
	{
		NKMEquipTemplet equipTemplet = GetEquipTemplet(equipID);
		if (equipTemplet == null)
		{
			return 0;
		}
		int i = 0;
		int maxEquipEnchantLevel = GetMaxEquipEnchantLevel(equipTemplet.m_NKM_ITEM_TIER);
		int num = 0;
		for (; i <= maxEquipEnchantLevel && GetEquipEnchantExpTemplet(equipTemplet.m_NKM_ITEM_TIER, i) != null; i++)
		{
			int enchantRequireExp = GetEnchantRequireExp(equipTemplet.m_NKM_ITEM_TIER, i, equipTemplet.m_NKM_ITEM_GRADE);
			if (enchantRequireExp > 0)
			{
				num += enchantRequireExp;
			}
		}
		return num;
	}

	public static int GetNeedExpToMaxLevel(NKMEquipItemData equipData)
	{
		NKMEquipTemplet equipTemplet = GetEquipTemplet(equipData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return 0;
		}
		int maxEquipEnchantLevel = GetMaxEquipEnchantLevel(equipTemplet.m_NKM_ITEM_TIER);
		int num = GetEnchantRequireExp(equipTemplet.m_NKM_ITEM_TIER, equipData.m_EnchantLevel, equipTemplet.m_NKM_ITEM_GRADE) - equipData.m_EnchantExp;
		for (int i = equipData.m_EnchantLevel + 1; i < maxEquipEnchantLevel; i++)
		{
			num += GetEnchantRequireExp(equipTemplet.m_NKM_ITEM_TIER, i, equipTemplet.m_NKM_ITEM_GRADE);
		}
		return num;
	}

	public static List<NKMItemMiscTemplet> GetItemMiscTempletListByType(NKM_ITEM_MISC_TYPE type)
	{
		return NKMItemMiscTemplet.Values.Where((NKMItemMiscTemplet e) => e.m_ItemMiscType == type).ToList();
	}

	public static List<NKMMoldItemData> GetMoldItemData(NKM_CRAFT_TAB_TYPE type)
	{
		List<NKMMoldItemData> list = new List<NKMMoldItemData>();
		foreach (NKMItemMoldTemplet value in NKMTempletContainer<NKMItemMoldTemplet>.Values)
		{
			if (value.EnableByTag && value.m_TabType == type)
			{
				long count = 0L;
				NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
				if (nKMUserData != null && nKMUserData.m_CraftData != null)
				{
					count = nKMUserData.m_CraftData.GetMoldCount(value.m_MoldID);
				}
				list.Add(new NKMMoldItemData(value.m_MoldID, count));
			}
		}
		return list;
	}

	public static NKMItemMiscTemplet GetItemMiscTempletByRewardType(NKM_REWARD_TYPE type)
	{
		if (type == NKM_REWARD_TYPE.RT_USER_EXP)
		{
			return NKMItemMiscTemplet.Find(501);
		}
		return null;
	}

	public static int GetMoldCount(NKM_CRAFT_TAB_TYPE type = NKM_CRAFT_TAB_TYPE.MT_EQUIP)
	{
		int num = 0;
		foreach (NKMItemMoldTemplet value in NKMTempletContainer<NKMItemMoldTemplet>.Values)
		{
			if (value.EnableByTag && value.m_TabType == type)
			{
				num++;
			}
		}
		return num;
	}

	public static NKCRandomMoldBoxTemplet GetRandomMoldBoxTemplet(int rewardID)
	{
		foreach (KeyValuePair<int, NKCRandomMoldBoxTemplet> item in m_dicRandomMoldBoxTemplet)
		{
			if (item.Value.m_RewardID == rewardID)
			{
				return item.Value;
			}
		}
		return null;
	}

	public static bool LoadFromLua_Random_Mold_Box(string filename)
	{
		m_dicRandomMoldBoxTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT_ITEM_TEMPLET", filename, "RANDOM_MOLD_BOX", NKCRandomMoldBoxTemplet.LoadFromLUA);
		if (m_dicRandomMoldBoxTemplet != null)
		{
			m_dicRandomMoldBox = new Dictionary<int, Dictionary<NKM_REWARD_TYPE, List<int>>>();
			foreach (KeyValuePair<int, NKCRandomMoldBoxTemplet> item in m_dicRandomMoldBoxTemplet)
			{
				if (item.Value != null)
				{
					if (!m_dicRandomMoldBox.ContainsKey(item.Value.m_RewardGroupID))
					{
						Dictionary<NKM_REWARD_TYPE, List<int>> dictionary = new Dictionary<NKM_REWARD_TYPE, List<int>>();
						dictionary.Add(item.Value.m_reward_type, new List<int> { item.Value.m_RewardID });
						m_dicRandomMoldBox.Add(item.Value.m_RewardGroupID, dictionary);
					}
					else if (!m_dicRandomMoldBox[item.Value.m_RewardGroupID].ContainsKey(item.Value.m_reward_type))
					{
						m_dicRandomMoldBox[item.Value.m_RewardGroupID].Add(item.Value.m_reward_type, new List<int> { item.Value.m_RewardID });
					}
					else
					{
						m_dicRandomMoldBox[item.Value.m_RewardGroupID][item.Value.m_reward_type].Add(item.Value.m_RewardID);
					}
				}
			}
		}
		foreach (KeyValuePair<int, Dictionary<NKM_REWARD_TYPE, List<int>>> item2 in m_dicRandomMoldBox)
		{
			if (item2.Value == null)
			{
				continue;
			}
			foreach (KeyValuePair<NKM_REWARD_TYPE, List<int>> item3 in item2.Value)
			{
				if (item3.Value != null)
				{
					item3.Value.Sort(new CompTemplet.CompNET());
				}
			}
		}
		Debug.Log($"LoadFromLua_Random_Mold_Box : result - {m_dicRandomMoldBox.Count}");
		return true;
	}

	public static bool LoadFromLua_Item_Mold_Tab(string filename)
	{
		m_dicMoldTabTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT_ITEM_TEMPLET", filename, "ITEM_MOLD_TAB", NKCRandomMoldTabTemplet.LoadFromLUA);
		_ = m_dicRandomMoldBoxTemplet;
		Debug.Log($"LoadFromLua_Item_Mold_Tab : result - {m_dicMoldTabTemplet.Count}");
		return true;
	}

	public static bool LoadFromLua_Item_AutoWeight(string filename)
	{
		m_dicAutoWeightTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT_ITEM_TEMPLET", filename, "EQUIP_AUTO_WEIGHT", NKCEquipAutoWeightTemplet.LoadFromLUA);
		_ = m_dicAutoWeightTemplet;
		Debug.Log($"LoadFromLua_Item_AutoWeight : result - {m_dicAutoWeightTemplet.Count}");
		return true;
	}

	public static float GetOptionWeight(NKM_STAT_TYPE targetOption, NKM_UNIT_ROLE_TYPE unitRole)
	{
		if (m_dicAutoWeightTemplet.ContainsKey((int)targetOption))
		{
			NKCEquipAutoWeightTemplet nKCEquipAutoWeightTemplet = m_dicAutoWeightTemplet[(int)targetOption];
			return unitRole switch
			{
				NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => nKCEquipAutoWeightTemplet.NURT_DEFENDER, 
				NKM_UNIT_ROLE_TYPE.NURT_RANGER => nKCEquipAutoWeightTemplet.NURT_RANGER, 
				NKM_UNIT_ROLE_TYPE.NURT_STRIKER => nKCEquipAutoWeightTemplet.NURT_STRIKER, 
				NKM_UNIT_ROLE_TYPE.NURT_SNIPER => nKCEquipAutoWeightTemplet.NURT_SNIPER, 
				NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => nKCEquipAutoWeightTemplet.NURT_SUPPORTER, 
				NKM_UNIT_ROLE_TYPE.NURT_TOWER => nKCEquipAutoWeightTemplet.NURT_TOWER, 
				NKM_UNIT_ROLE_TYPE.NURT_SIEGE => nKCEquipAutoWeightTemplet.NURT_SIEGE, 
				NKM_UNIT_ROLE_TYPE.NURT_INVALID => nKCEquipAutoWeightTemplet.NURT_INVALID, 
				_ => 0f, 
			};
		}
		return 0f;
	}

	public static string GetSetOptionDescription(NKM_STAT_TYPE type, float fValue)
	{
		if (NKMUnitStatManager.IsPercentStat(type))
		{
			if (NKCUtilString.IsNameReversedIfNegative(type) && fValue < 0f)
			{
				return string.Format(NKCUtilString.GetStatShortName(type, fValue) + " {0:P1}", 0f - fValue);
			}
			return string.Format(NKCUtilString.GetStatShortName(type) + " {0:P1}", fValue);
		}
		if (NKCUtilString.IsNameReversedIfNegative(type) && fValue < 0f)
		{
			return string.Format(NKCUtilString.GetStatShortName(type, fValue) + " {0}", 0f - fValue);
		}
		return string.Format(NKCUtilString.GetStatShortName(type) + " {0}", fValue);
	}

	public static bool IsActiveSetOptionItem(long itemUID)
	{
		return IsActiveSetOptionItem(NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(itemUID));
	}

	public static bool IsActiveSetOptionItem(NKMEquipItemData itemData)
	{
		if (itemData == null)
		{
			return false;
		}
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = GetEquipSetOptionTemplet(itemData.m_SetOptionId);
		if (equipSetOptionTemplet == null)
		{
			return false;
		}
		if (equipSetOptionTemplet.m_EquipSetPart == 1)
		{
			return true;
		}
		List<long> matchedSetOptionItemList = GetMatchedSetOptionItemList(itemData.m_ItemUid);
		if (matchedSetOptionItemList.Count < equipSetOptionTemplet.m_EquipSetPart)
		{
			return false;
		}
		if (matchedSetOptionItemList.Count == equipSetOptionTemplet.m_EquipSetPart)
		{
			return true;
		}
		while (matchedSetOptionItemList.Count >= equipSetOptionTemplet.m_EquipSetPart)
		{
			if (matchedSetOptionItemList.GetRange(0, equipSetOptionTemplet.m_EquipSetPart).FindIndex((long e) => e == itemData.m_ItemUid) >= 0)
			{
				return true;
			}
			matchedSetOptionItemList.RemoveRange(0, equipSetOptionTemplet.m_EquipSetPart);
		}
		return false;
	}

	public static bool IsActiveSetOptionItem(NKMEquipItemData itemData, NKMEquipmentSet equipSet)
	{
		if (itemData == null)
		{
			return false;
		}
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = GetEquipSetOptionTemplet(itemData.m_SetOptionId);
		if (equipSetOptionTemplet == null)
		{
			return false;
		}
		if (equipSetOptionTemplet.m_EquipSetPart == 1)
		{
			return true;
		}
		List<long> matchingSetOptionItem = GetMatchingSetOptionItem(itemData, equipSet);
		if (matchingSetOptionItem.Count < equipSetOptionTemplet.m_EquipSetPart)
		{
			return false;
		}
		if (matchingSetOptionItem.Count == equipSetOptionTemplet.m_EquipSetPart)
		{
			return true;
		}
		while (matchingSetOptionItem.Count >= equipSetOptionTemplet.m_EquipSetPart)
		{
			if (matchingSetOptionItem.GetRange(0, equipSetOptionTemplet.m_EquipSetPart).FindIndex((long e) => e == itemData.m_ItemUid) >= 0)
			{
				return true;
			}
			matchingSetOptionItem.RemoveRange(0, equipSetOptionTemplet.m_EquipSetPart);
		}
		return false;
	}

	public static int GetMatchingSetOptionItem(NKMEquipItemData itemData)
	{
		if (itemData == null)
		{
			return 0;
		}
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = GetEquipSetOptionTemplet(itemData.m_SetOptionId);
		if (equipSetOptionTemplet == null)
		{
			return 0;
		}
		if (equipSetOptionTemplet.m_EquipSetPart == 1)
		{
			return 1;
		}
		List<long> matchedSetOptionItemList = GetMatchedSetOptionItemList(itemData.m_ItemUid);
		if (matchedSetOptionItemList.Count < equipSetOptionTemplet.m_EquipSetPart)
		{
			return matchedSetOptionItemList.Count;
		}
		if (matchedSetOptionItemList.Count == equipSetOptionTemplet.m_EquipSetPart)
		{
			return equipSetOptionTemplet.m_EquipSetPart;
		}
		while (matchedSetOptionItemList.Count >= equipSetOptionTemplet.m_EquipSetPart)
		{
			List<long> range = matchedSetOptionItemList.GetRange(0, equipSetOptionTemplet.m_EquipSetPart);
			if (range.FindIndex((long e) => e == itemData.m_ItemUid) >= 0)
			{
				return range.Count;
			}
			matchedSetOptionItemList.RemoveRange(0, equipSetOptionTemplet.m_EquipSetPart);
		}
		return matchedSetOptionItemList.Count;
	}

	public static int GetExpactSetOptionMatchingCnt(NKMEquipItemData itemData, int newSetOptionID)
	{
		if (itemData == null)
		{
			return 1;
		}
		if (itemData.m_OwnerUnitUID <= 0)
		{
			return 1;
		}
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = GetEquipSetOptionTemplet(newSetOptionID);
		if (equipSetOptionTemplet == null)
		{
			return 1;
		}
		List<long> matchedSetOptionItemList = GetMatchedSetOptionItemList(itemData, newSetOptionID, IgnoreEquipType: true);
		if (matchedSetOptionItemList.Count == equipSetOptionTemplet.m_EquipSetPart)
		{
			return equipSetOptionTemplet.m_EquipSetPart;
		}
		while (matchedSetOptionItemList.Count >= equipSetOptionTemplet.m_EquipSetPart)
		{
			List<long> range = matchedSetOptionItemList.GetRange(0, equipSetOptionTemplet.m_EquipSetPart);
			if (range.FindIndex((long e) => e == itemData.m_ItemUid) >= 0)
			{
				return range.Count;
			}
			matchedSetOptionItemList.RemoveRange(0, equipSetOptionTemplet.m_EquipSetPart);
		}
		return matchedSetOptionItemList.Count;
	}

	private static List<long> GetMatchedSetOptionItemList(long itemUID)
	{
		NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(itemUID);
		if (itemEquip == null)
		{
			return new List<long>();
		}
		return GetMatchingItemList(itemEquip);
	}

	private static List<long> GetMatchingItemList(NKMEquipItemData itemData)
	{
		List<long> list = new List<long>();
		if (GetEquipSetOptionTemplet(itemData.m_SetOptionId) == null)
		{
			return list;
		}
		NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(itemData.m_OwnerUnitUID);
		if (unitFromUID == null)
		{
			return list;
		}
		NKMEquipmentSet equipmentSet = unitFromUID.GetEquipmentSet(NKCScenManager.CurrentUserData().m_InventoryData);
		if (equipmentSet == null)
		{
			return list;
		}
		if (equipmentSet.Weapon != null && equipmentSet.Weapon.m_SetOptionId == itemData.m_SetOptionId)
		{
			list.Add(equipmentSet.Weapon.m_ItemUid);
		}
		if (equipmentSet.Defence != null && equipmentSet.Defence.m_SetOptionId == itemData.m_SetOptionId)
		{
			list.Add(equipmentSet.Defence.m_ItemUid);
		}
		if (equipmentSet.Accessory != null && equipmentSet.Accessory.m_SetOptionId == itemData.m_SetOptionId)
		{
			list.Add(equipmentSet.Accessory.m_ItemUid);
		}
		if (equipmentSet.Accessory2 != null && equipmentSet.Accessory2.m_SetOptionId == itemData.m_SetOptionId)
		{
			list.Add(equipmentSet.Accessory2.m_ItemUid);
		}
		return list;
	}

	private static List<long> GetMatchingSetOptionItem(NKMEquipItemData itemData, NKMEquipmentSet equipmentSet)
	{
		List<long> list = new List<long>();
		if (GetEquipSetOptionTemplet(itemData.m_SetOptionId) == null)
		{
			return list;
		}
		if (equipmentSet.Weapon != null && equipmentSet.Weapon.m_SetOptionId == itemData.m_SetOptionId)
		{
			list.Add(equipmentSet.Weapon.m_ItemUid);
		}
		if (equipmentSet.Defence != null && equipmentSet.Defence.m_SetOptionId == itemData.m_SetOptionId)
		{
			list.Add(equipmentSet.Defence.m_ItemUid);
		}
		if (equipmentSet.Accessory != null && equipmentSet.Accessory.m_SetOptionId == itemData.m_SetOptionId)
		{
			list.Add(equipmentSet.Accessory.m_ItemUid);
		}
		if (equipmentSet.Accessory2 != null && equipmentSet.Accessory2.m_SetOptionId == itemData.m_SetOptionId)
		{
			list.Add(equipmentSet.Accessory2.m_ItemUid);
		}
		return list;
	}

	private static List<long> GetMatchedSetOptionItemList(NKMEquipItemData itemData, int SetOptionID, bool IgnoreEquipType = false)
	{
		List<long> list = new List<long>();
		NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(itemData.m_OwnerUnitUID);
		if (unitFromUID != null)
		{
			NKMEquipmentSet equipmentSet = unitFromUID.GetEquipmentSet(NKCScenManager.CurrentUserData().m_InventoryData);
			if (equipmentSet != null)
			{
				if (equipmentSet.Weapon != null && (equipmentSet.Weapon.m_SetOptionId == SetOptionID || (IgnoreEquipType && itemData.m_ItemUid == equipmentSet.Weapon.m_ItemUid)))
				{
					list.Add(equipmentSet.Weapon.m_ItemUid);
				}
				if (equipmentSet.Defence != null && (equipmentSet.Defence.m_SetOptionId == SetOptionID || (IgnoreEquipType && itemData.m_ItemUid == equipmentSet.Defence.m_ItemUid)))
				{
					list.Add(equipmentSet.Defence.m_ItemUid);
				}
				if (equipmentSet.Accessory != null && (equipmentSet.Accessory.m_SetOptionId == SetOptionID || (IgnoreEquipType && itemData.m_ItemUid == equipmentSet.Accessory.m_ItemUid)))
				{
					list.Add(equipmentSet.Accessory.m_ItemUid);
				}
				if (equipmentSet.Accessory2 != null && (equipmentSet.Accessory2.m_SetOptionId == SetOptionID || (IgnoreEquipType && itemData.m_ItemUid == equipmentSet.Accessory2.m_ItemUid)))
				{
					list.Add(equipmentSet.Accessory2.m_ItemUid);
				}
			}
		}
		return list;
	}

	public static ITEM_EQUIP_POSITION GetItemEquipPosition(long itemUID)
	{
		return GetItemEquipPosition(NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(itemUID));
	}

	public static ITEM_EQUIP_POSITION GetItemEquipPosition(NKMEquipItemData eqipItem)
	{
		ITEM_EQUIP_POSITION result = ITEM_EQUIP_POSITION.IEP_NONE;
		if (eqipItem != null && eqipItem.m_OwnerUnitUID > 0)
		{
			NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(eqipItem.m_OwnerUnitUID);
			if (unitFromUID != null)
			{
				if (unitFromUID.GetEquipItemWeaponUid() == eqipItem.m_ItemUid)
				{
					result = ITEM_EQUIP_POSITION.IEP_WEAPON;
				}
				else if (unitFromUID.GetEquipItemDefenceUid() == eqipItem.m_ItemUid)
				{
					result = ITEM_EQUIP_POSITION.IEP_DEFENCE;
				}
				else if (unitFromUID.GetEquipItemAccessoryUid() == eqipItem.m_ItemUid)
				{
					result = ITEM_EQUIP_POSITION.IEP_ACC;
				}
				else if (unitFromUID.GetEquipItemAccessory2Uid() == eqipItem.m_ItemUid)
				{
					result = ITEM_EQUIP_POSITION.IEP_ACC2;
				}
			}
		}
		return result;
	}

	public static NKM_ERROR_CODE CanEnchantItem(NKMUserData userdata, long itemUID)
	{
		NKMEquipItemData itemEquip = userdata.m_InventoryData.GetItemEquip(itemUID);
		return CanEnchantItem(userdata, itemEquip);
	}

	public static NKM_ERROR_CODE CanEnchantItem(NKMUserData userData, long equipUID, List<MiscItemData> lstMaterials)
	{
		NKMEquipItemData itemEquip = userData.m_InventoryData.GetItemEquip(equipUID);
		NKM_ERROR_CODE nKM_ERROR_CODE = CanEnchantItem(userData, itemEquip);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		int num = 0;
		for (int i = 0; i < lstMaterials.Count; i++)
		{
			num += lstMaterials[i].count * NKMCommonConst.EquipEnchantMiscConst.Materials[i].Exp;
		}
		int credit = num * 8;
		NKCCompanyBuff.SetDiscountOfCreditInEnchantTuning(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit);
		if (userData.GetCredit() < credit)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanEnchantItem(NKMUserData userdata, long equipUID, List<long> material_item_list)
	{
		if (material_item_list == null || material_item_list.Count <= 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_ITEM_UID;
		}
		NKMEquipItemData itemEquip = userdata.m_InventoryData.GetItemEquip(equipUID);
		NKM_ERROR_CODE nKM_ERROR_CODE = CanEnchantItem(userdata, itemEquip);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		int num = 0;
		foreach (long item in material_item_list)
		{
			if (item == itemEquip.m_ItemUid)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INVALID_ITEM_UID;
			}
			NKMEquipItemData itemEquip2 = userdata.m_InventoryData.GetItemEquip(item);
			if (itemEquip2 == null)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INVALID_ITEM_UID;
			}
			nKM_ERROR_CODE = CanRemoveItem(itemEquip2);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE;
			}
			int equipEnchantFeedExp = GetEquipEnchantFeedExp(itemEquip2);
			if (equipEnchantFeedExp == -1)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INVALID_EQUIP_ITEM;
			}
			num += equipEnchantFeedExp;
		}
		int credit = num * 8;
		NKCCompanyBuff.SetDiscountOfCreditInEnchantTuning(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit);
		if (userdata.GetCredit() < credit)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static void UnEquip(long itemUID)
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(itemUID);
		if (itemEquip == null || itemEquip.m_OwnerUnitUID <= 0)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = GetEquipTemplet(itemEquip.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return;
		}
		NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(itemEquip.m_OwnerUnitUID);
		if (unitFromUID != null)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = equipTemplet.CanUnEquipByUnit(NKCScenManager.GetScenManager().GetMyUserData(), unitFromUID);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
				return;
			}
			ITEM_EQUIP_POSITION itemEquipPosition = GetItemEquipPosition(itemEquip.m_ItemUid);
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip: false, itemEquip.m_OwnerUnitUID, itemEquip.m_ItemUid, itemEquipPosition);
		}
	}

	public static NKC_EQUIP_UPGRADE_STATE CanUpgradeEquipByCoreID(NKMEquipItemData coreEquipData)
	{
		if (coreEquipData == null)
		{
			return NKC_EQUIP_UPGRADE_STATE.NOT_HAVE;
		}
		if (NKMTempletContainer<NKMItemEquipUpgradeTemplet>.Find((NKMItemEquipUpgradeTemplet x) => x.CoreEquipTemplet.m_ItemEquipID == coreEquipData.m_ItemEquipID) == null)
		{
			return NKC_EQUIP_UPGRADE_STATE.NOT_HAVE;
		}
		NKMEquipTemplet equipTemplet = GetEquipTemplet(coreEquipData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return NKC_EQUIP_UPGRADE_STATE.NONE;
		}
		if (coreEquipData.m_EnchantLevel < equipTemplet.m_MaxEnchantLevel)
		{
			return NKC_EQUIP_UPGRADE_STATE.NEED_ENHANCE;
		}
		if (coreEquipData.m_Precision < 100 || coreEquipData.m_Precision2 < 100)
		{
			return NKC_EQUIP_UPGRADE_STATE.NEED_PRECISION;
		}
		return NKC_EQUIP_UPGRADE_STATE.UPGRADABLE;
	}

	public static NKC_EQUIP_UPGRADE_STATE GetSetUpgradeSlotState(NKMItemEquipUpgradeTemplet upgradeTemplet, ref List<NKMEquipItemData> lstCoreEquipData)
	{
		NKMEquipTemplet equipTemplet = GetEquipTemplet(upgradeTemplet.CoreEquipTemplet.m_ItemEquipID);
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		lstCoreEquipData = new List<NKMEquipItemData>();
		bool flag = false;
		foreach (KeyValuePair<long, NKMEquipItemData> equipItem in inventoryData.EquipItems)
		{
			bool flag2 = false;
			bool flag3 = false;
			if (equipItem.Value.m_ItemEquipID == equipTemplet.m_ItemEquipID)
			{
				if (equipItem.Value.m_EnchantLevel == GetMaxEquipEnchantLevel(equipTemplet.m_MaxEnchantLevel))
				{
					flag2 = true;
				}
				if (equipItem.Value.m_Precision >= 100 && equipItem.Value.m_Precision2 >= 100)
				{
					flag3 = true;
				}
				if (flag2 && flag3)
				{
					flag = true;
				}
				lstCoreEquipData.Add(equipItem.Value);
			}
		}
		if (lstCoreEquipData.Count == 0)
		{
			return NKC_EQUIP_UPGRADE_STATE.NOT_HAVE;
		}
		if (flag)
		{
			return NKC_EQUIP_UPGRADE_STATE.UPGRADABLE;
		}
		return NKC_EQUIP_UPGRADE_STATE.NEED_ENHANCE;
	}

	public static NKC_EQUIP_UPGRADE_STATE GetEquipUpgradableState(NKMEquipItemData coreEquipData)
	{
		NKMItemEquipUpgradeTemplet nKMItemEquipUpgradeTemplet = NKMTempletContainer<NKMItemEquipUpgradeTemplet>.Find((NKMItemEquipUpgradeTemplet x) => x.CoreEquipTemplet.m_ItemEquipID == coreEquipData.m_ItemEquipID);
		if (nKMItemEquipUpgradeTemplet == null)
		{
			return NKC_EQUIP_UPGRADE_STATE.NOT_HAVE;
		}
		NKMEquipTemplet equipTemplet = GetEquipTemplet(nKMItemEquipUpgradeTemplet.CoreEquipTemplet.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return NKC_EQUIP_UPGRADE_STATE.NOT_HAVE;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (coreEquipData.m_ItemEquipID != equipTemplet.m_ItemEquipID)
		{
			return NKC_EQUIP_UPGRADE_STATE.NOT_HAVE;
		}
		if (coreEquipData.m_EnchantLevel == GetMaxEquipEnchantLevel(equipTemplet.m_MaxEnchantLevel))
		{
			flag2 = true;
		}
		if (coreEquipData.m_Precision >= 100 && coreEquipData.m_Precision2 >= 100)
		{
			flag3 = true;
		}
		if (flag2 && flag3)
		{
			flag = true;
		}
		if (flag)
		{
			return NKC_EQUIP_UPGRADE_STATE.UPGRADABLE;
		}
		return NKC_EQUIP_UPGRADE_STATE.NEED_ENHANCE;
	}

	public static bool IsRelicRerollTarget(NKMEquipItemData equipData)
	{
		if (GetEquipTemplet(equipData.m_ItemEquipID).IsRelic() && equipData.potentialOptions.Count > 0 && equipData.potentialOptions[0] != null)
		{
			if (equipData.potentialOptions[0].precisionChangeCount >= NKMCommonConst.RelicRerollLimitCount)
			{
				return false;
			}
			int num = equipData.potentialOptions[0].sockets.Length;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (equipData.potentialOptions[0].sockets[i] != null)
				{
					num2++;
				}
			}
			if (num2 == num)
			{
				return true;
			}
		}
		return false;
	}

	public static void InitResetCount()
	{
		m_ResetCount = new List<NKMResetCount>();
	}

	public static int GetRemainResetCount(int groupID)
	{
		NKMResetCounterGroupTemplet nKMResetCounterGroupTemplet = NKMResetCounterGroupTemplet.Find(groupID);
		if (nKMResetCounterGroupTemplet != null && nKMResetCounterGroupTemplet.IsValid())
		{
			return m_ResetCount.Find((NKMResetCount x) => x.groupId == groupID)?.count ?? nKMResetCounterGroupTemplet.MaxCount;
		}
		return -1;
	}

	public static void SetResetCount(List<NKMResetCount> lstResetCount)
	{
		m_ResetCount = lstResetCount;
	}

	public static void SetResetCount(NKMResetCount resetCount)
	{
		int num = m_ResetCount.FindIndex((NKMResetCount x) => x.groupId == resetCount.groupId);
		if (num >= 0)
		{
			m_ResetCount[num] = resetCount;
		}
		else
		{
			m_ResetCount.Add(resetCount);
		}
	}

	public static void SetResetCount(int groupID, int count)
	{
		SetResetCount(new NKMResetCount
		{
			groupId = groupID,
			count = count
		});
	}

	public static bool IsStackMoldItem(NKMItemMoldTemplet moldItemTemplet)
	{
		if (moldItemTemplet != null && moldItemTemplet.m_StackCount > 0)
		{
			return moldItemTemplet.m_ResetGroupId != 0;
		}
		return false;
	}

	public static int GetRemainResetCountStack(NKMItemMoldTemplet moldItemTemplet)
	{
		if (IsStackMoldItem(moldItemTemplet))
		{
			return (int)(GetCalcResetCount(moldItemTemplet.m_StackType, moldItemTemplet.StackStartDate.StartDate) * moldItemTemplet.m_StackCount) - GetRemainResetCount(moldItemTemplet.m_ResetGroupId);
		}
		return 0;
	}

	private static long GetCalcResetCount(COUNT_RESET_TYPE resetType, DateTime startDate)
	{
		DateTime recent = ServiceTime.Recent;
		switch (resetType)
		{
		case COUNT_RESET_TYPE.DAY:
		{
			DateTime dateTime4 = CalcLastReset(startDate);
			DateTime dateTime5 = CalcLastReset(recent);
			if ((long)(dateTime5 - dateTime4).TotalDays <= 0)
			{
				return 0L;
			}
			return (long)(dateTime5 - dateTime4).TotalDays;
		}
		case COUNT_RESET_TYPE.WEEK:
		{
			DateTime dateTime3 = CalcLastReset(startDate, DayOfWeek.Monday, TimeReset.ResetHourSpan);
			double totalDays = (CalcLastReset(recent, DayOfWeek.Monday, TimeReset.ResetHourSpan) - dateTime3).TotalDays;
			if ((long)totalDays / 7 <= 0)
			{
				return 0L;
			}
			return (long)totalDays / 7;
		}
		case COUNT_RESET_TYPE.MONTH:
		{
			DateTime dateTime = CalcLastReset(startDate, TimeReset.ResetHourSpan);
			DateTime dateTime2 = CalcLastReset(recent, TimeReset.ResetHourSpan);
			int num = (dateTime2.Year - dateTime.Year) * 12 + (dateTime2.Month - dateTime.Month);
			return (num > 0) ? num : 0;
		}
		default:
			return 0L;
		}
	}

	private static DateTime CalcLastReset(DateTime current)
	{
		DateTime result = current.Date + TimeReset.ResetHourSpan;
		if (current.TimeOfDay < TimeReset.ResetHourSpan)
		{
			result -= TimeSpan.FromDays(1.0);
		}
		return result;
	}

	private static DateTime CalcLastReset(DateTime current, DayOfWeek dayOfWeek, TimeSpan ResetHourSpan)
	{
		DateTime dateTime = current.Date + ResetHourSpan + TimeSpan.FromDays(dayOfWeek - current.DayOfWeek);
		if (current < dateTime)
		{
			return dateTime.AddDays(-7.0);
		}
		return dateTime;
	}

	private static DateTime CalcLastReset(DateTime current, TimeSpan resetHourSpan)
	{
		DateTime dateTime = new DateTime(current.Year, current.Month, 1, 0, 0, 0) + resetHourSpan;
		if (current < dateTime)
		{
			return dateTime.AddMonths(-1);
		}
		return dateTime;
	}
}
