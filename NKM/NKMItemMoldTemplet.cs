using System.Collections.Generic;
using Cs.Logging;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMItemMoldTemplet : INKMTemplet, INKMTempletEx
{
	public const int MAX_MATERIAL_COUNT = 4;

	public const int MAX_ITEM_MOLD_CRAFT_COUNT = 1000;

	private string intervalId;

	public int m_MoldID;

	public string m_MoldStrID;

	public int m_RewardGroupID;

	public NKM_CRAFT_TAB_TYPE m_TabType;

	public int m_Tier;

	public NKM_ITEM_DROP_POSITION m_ContentType;

	public NKM_ITEM_GRADE m_Grade;

	public ITEM_EQUIP_POSITION m_RewardEquipPosition = ITEM_EQUIP_POSITION.IEP_NONE;

	public NKM_UNIT_STYLE_TYPE m_RewardEquipUnitType = NKM_UNIT_STYLE_TYPE.NUST_ETC;

	private string m_OpenTag;

	public bool m_bPermanent;

	public string m_MoldName;

	public string m_MoldDesc;

	public int m_Time;

	public List<NKMItemMoldMaterialData> m_MaterialList = new List<NKMItemMoldMaterialData>(4);

	public Dictionary<int, int> m_dicRewardGroup = new Dictionary<int, int>();

	public string m_MoldIconName;

	public int m_ResetGroupId;

	public COUNT_RESET_TYPE m_StackType;

	public int m_StackCount;

	private string m_StackStartDateId;

	public int Key => m_MoldID;

	public static IEnumerable<NKMItemMoldTemplet> Values => NKMTempletContainer<NKMItemMoldTemplet>.Values;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public bool HasDateLimit => IntervalTemplet.IsValid;

	public NKMIntervalTemplet IntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public NKMIntervalTemplet StackStartDate { get; private set; } = NKMIntervalTemplet.Invalid;

	public bool IsEquipMold
	{
		get
		{
			NKM_CRAFT_TAB_TYPE tabType = m_TabType;
			if (tabType == NKM_CRAFT_TAB_TYPE.MT_EQUIP || (uint)(tabType - 8) <= 3u)
			{
				return true;
			}
			return false;
		}
	}

	public static NKMItemMoldTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 268))
		{
			return null;
		}
		NKMItemMoldTemplet nKMItemMoldTemplet = new NKMItemMoldTemplet();
		bool flag = true;
		flag &= lua.GetData("m_MoldID", ref nKMItemMoldTemplet.m_MoldID);
		flag &= lua.GetData("m_MoldStrID", ref nKMItemMoldTemplet.m_MoldStrID);
		flag &= lua.GetData("m_RewardGroupID", ref nKMItemMoldTemplet.m_RewardGroupID);
		flag &= lua.GetData("m_MoldTabID", ref nKMItemMoldTemplet.m_TabType);
		flag &= lua.GetData("m_ContentType", ref nKMItemMoldTemplet.m_ContentType);
		flag &= lua.GetData("m_Tier", ref nKMItemMoldTemplet.m_Tier);
		flag = lua.GetData("m_Grade", ref nKMItemMoldTemplet.m_Grade);
		flag = lua.GetData("m_RewardEquipUnitType", ref nKMItemMoldTemplet.m_RewardEquipUnitType);
		flag = lua.GetData("m_RewardEquipPosition", ref nKMItemMoldTemplet.m_RewardEquipPosition);
		flag &= lua.GetData("m_bPermanent", ref nKMItemMoldTemplet.m_bPermanent);
		flag &= lua.GetData("m_MoldName", ref nKMItemMoldTemplet.m_MoldName);
		flag &= lua.GetData("m_MoldDesc", ref nKMItemMoldTemplet.m_MoldDesc);
		flag &= lua.GetData("m_Time", ref nKMItemMoldTemplet.m_Time);
		lua.GetData("m_OpenTag", ref nKMItemMoldTemplet.m_OpenTag);
		lua.GetData("m_ResetGroupId", ref nKMItemMoldTemplet.m_ResetGroupId);
		lua.GetData("m_StackType", ref nKMItemMoldTemplet.m_StackType);
		lua.GetData("m_StackCount", ref nKMItemMoldTemplet.m_StackCount);
		lua.GetData("m_StackStartDateID", ref nKMItemMoldTemplet.m_StackStartDateId);
		for (int i = 1; i <= 4; i++)
		{
			NKMItemMoldMaterialData item = default(NKMItemMoldMaterialData);
			lua.GetData($"m_MaterialType{i}", ref item.m_MaterialType);
			lua.GetData($"m_MaterialID{i}", ref item.m_MaterialID);
			lua.GetData($"m_MaterialValue{i}", ref item.m_MaterialValue);
			if (item.m_MaterialID > 0)
			{
				nKMItemMoldTemplet.m_MaterialList.Add(item);
			}
		}
		flag &= lua.GetData("m_MoldIconName", ref nKMItemMoldTemplet.m_MoldIconName);
		lua.GetData("m_DateStrID", ref nKMItemMoldTemplet.intervalId);
		nKMItemMoldTemplet.CheckValidation();
		if (!flag)
		{
			Log.Error($"NKMItemMoldTemplet Load fail - {nKMItemMoldTemplet.m_MoldID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 312);
			return null;
		}
		if (NKMItemManager.m_dicMoldReward.ContainsKey(nKMItemMoldTemplet.m_RewardGroupID))
		{
			NKMItemManager.m_dicMoldReward[nKMItemMoldTemplet.m_RewardGroupID].Add(nKMItemMoldTemplet.m_RewardGroupID);
		}
		return nKMItemMoldTemplet;
	}

	public static NKMItemMoldTemplet Find(int key)
	{
		return NKMTempletContainer<NKMItemMoldTemplet>.Find(key);
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void JoinIntervalTemplet()
	{
		if (!string.IsNullOrEmpty(intervalId))
		{
			IntervalTemplet = NKMIntervalTemplet.Find(intervalId);
			if (IntervalTemplet == null)
			{
				IntervalTemplet = NKMIntervalTemplet.Unuseable;
				NKMTempletError.Add($"[Mold:{Key}]잘못된 interval id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 343);
				return;
			}
			if (IntervalTemplet.IsRepeatDate)
			{
				NKMTempletError.Add($"[Mold:{Key}] 반복 기간설정 사용 불가. id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 349);
				return;
			}
		}
		if (!string.IsNullOrEmpty(m_StackStartDateId))
		{
			StackStartDate = NKMIntervalTemplet.Find(m_StackStartDateId);
			if (StackStartDate == null)
			{
				IntervalTemplet = NKMIntervalTemplet.Unuseable;
				NKMTempletError.Add($"[Mold:{Key}]잘못된 interval id:{m_StackStartDateId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 360);
			}
			else if (StackStartDate.IsRepeatDate)
			{
				NKMTempletError.Add($"[Mold:{Key}] 반복 기간설정 사용 불가. id:{m_StackStartDateId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 366);
			}
		}
	}

	public void Validate()
	{
		if (m_ResetGroupId > 0 && NKMResetCounterGroupTemplet.Find(m_ResetGroupId) == null)
		{
			NKMTempletError.Add($"[NKMItemMoldTemplet:{Key}] 지정된 ResetGroupId와 매칭되는 템플릿이 존재하지 않습니다. m_ResetGroupId: {m_ResetGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 379);
		}
		else
		{
			if (m_StackCount <= 0)
			{
				return;
			}
			COUNT_RESET_TYPE stackType = m_StackType;
			if ((uint)(stackType - 1) > 2u)
			{
				NKMTempletError.Add($"[NKMItemMoldTemplet:{Key}] Invalid Stack Type", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 393);
			}
			if (m_ResetGroupId < 0)
			{
				NKMTempletError.Add($"[NKMItemMoldTemplet:{Key}] 누적형 카운트를 가진 MoldItem에 ResetGroupId가 지정되지 않았습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 399);
				return;
			}
			NKMResetCounterGroupTemplet nKMResetCounterGroupTemplet = NKMResetCounterGroupTemplet.Find(m_ResetGroupId);
			if (nKMResetCounterGroupTemplet == null)
			{
				NKMTempletError.Add($"[NKMItemMoldTemplet:{Key}] 지정된 ResetGroupId와 매칭되는 템플릿이 존재하지 않습니다. m_ResetGroupId: {m_ResetGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 406);
			}
			else if (nKMResetCounterGroupTemplet.Type != COUNT_RESET_TYPE.FIXED)
			{
				NKMTempletError.Add($"[NKMItemMoldTemplet:{Key}] 지정된 ResetGroupId에 매칭되는 타입은 FIXED만 지정할 수 있습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 412);
			}
		}
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}

	private void CheckValidation()
	{
		foreach (NKMItemMoldMaterialData material in m_MaterialList)
		{
			if (material.m_MaterialType != NKM_REWARD_TYPE.RT_MISC)
			{
				Log.ErrorAndExit($"[ItemMoldTemplet] 재료 아이템은 MISC만 넣을 수 있음 m_MoldID : {m_MoldID}, m_MaterialType1 : {material.m_MaterialType}, m_MaterialID1 : {material.m_MaterialID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMItemManagerEx.cs", 40);
			}
			if (material.m_MaterialID > 0 && !NKMRewardTemplet.IsValidReward(material.m_MaterialType, material.m_MaterialID))
			{
				Log.ErrorAndExit($"[ItemMoldTemplet] 재료 아이템이 존재하지 않음 m_MoldID : {m_MoldID}, m_MaterialType1 : {material.m_MaterialType}, m_MaterialID1 : {material.m_MaterialID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMItemManagerEx.cs", 47);
			}
		}
	}

	public string GetItemName()
	{
		return NKCStringTable.GetString(m_MoldName);
	}

	public string GetItemDesc()
	{
		return NKCStringTable.GetString(m_MoldDesc);
	}
}
