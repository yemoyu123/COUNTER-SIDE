using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Item;
using Cs.Logging;
using Cs.Math;
using NKC;
using NKM.Contract2;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMEquipTemplet : INKMTemplet
{
	public struct ItemStatData
	{
		public float m_fBaseValue;

		public float m_fLevelUpValue;
	}

	public struct OnRemoveItemData
	{
		public int m_ItemID;

		public int m_ItemCount;

		public OnRemoveItemData(int itemID, int itemCount)
		{
			m_ItemID = itemID;
			m_ItemCount = itemCount;
		}
	}

	private readonly NKMEquipStatGroupTemplet[] statGroups = new NKMEquipStatGroupTemplet[2];

	public int m_ItemEquipID;

	public string m_ItemEquipStrID = "";

	public string m_ItemEquipName = "";

	public string m_ItemEquipDesc = "";

	public List<string> m_lstSearchKeyword;

	public NKM_ITEM_GRADE m_NKM_ITEM_GRADE;

	public int m_NKM_ITEM_TIER = 1;

	public ITEM_EQUIP_POSITION m_ItemEquipPosition;

	public NKM_UNIT_STYLE_TYPE m_EquipUnitStyleType;

	public string m_ItemEquipIconName = "";

	public int m_MaxEnchantLevel = 1;

	public List<OnRemoveItemData> m_OnRemoveItemList = new List<OnRemoveItemData>();

	public NKM_STAT_TYPE m_StatType = NKM_STAT_TYPE.NST_END;

	public ItemStatData m_StatData;

	public int m_StatGroupID;

	public int m_StatGroupID_2;

	public int m_FeedEXP;

	public int m_PrecisionReqResource;

	public int m_PrecisionReqItem;

	public int m_RandomStatReqResource;

	public int m_RandomStatReqItem;

	public int m_RandomSetReqResource;

	public int m_RandomSetReqItemID;

	public int m_RandomSetReqItemValue;

	public List<int> m_lstSetGroup;

	public bool m_bItemFirstLock;

	public bool m_bRelic;

	public int potentialOptionGroupId;

	public int potentialOptionGroupId2;

	public readonly List<MiscItemUnit>[] socketReqResource = new List<MiscItemUnit>[3];

	public bool m_bShowEffect;

	public int m_RelicRerollReqResource;

	public float m_RelicRerollReqResourceFactor;

	public MiscItemUnit m_RelicRerollItemMisc;

	private readonly List<int> m_lstPrivateUnitID = new List<int>();

	public IReadOnlyList<NKMEquipStatGroupTemplet> StatGroups => statGroups;

	public IReadOnlyList<int> SetGroupList
	{
		get
		{
			if (m_lstSetGroup != null)
			{
				return m_lstSetGroup.AsReadOnly();
			}
			return null;
		}
	}

	public IReadOnlyList<int> PrivateUnitList => m_lstPrivateUnitID;

	public int Key => m_ItemEquipID;

	public bool IsEquipEnable => NKMUnitTempletBase.IsUnitStyleType(m_EquipUnitStyleType);

	public NKM_EQUIP_PRESET_TYPE PresetType { get; private set; }

	public NKMPotentialOptionGroupTemplet PotentialOptionGroupTemplet { get; private set; }

	public NKMPotentialOptionGroupTemplet PotentialOptionGroupTemplet2 { get; private set; }

	public bool IsPrivateEquip()
	{
		return m_lstPrivateUnitID.Count > 0;
	}

	public int GetPrivateUnitID()
	{
		if (m_lstPrivateUnitID.Count == 0)
		{
			return 0;
		}
		return m_lstPrivateUnitID[0];
	}

	public bool IsPrivateEquipForUnit(int unitID)
	{
		if (m_lstPrivateUnitID.Count > 0)
		{
			return NKMUnitManager.CheckContainsBaseUnit(m_lstPrivateUnitID, unitID);
		}
		return false;
	}

	public NKM_ERROR_CODE CanEquipByUnitID(int unitID)
	{
		if (NKMUnitManager.GetUnitTempletBase(unitID).m_NKM_UNIT_STYLE_TYPE != m_EquipUnitStyleType)
		{
			return NKM_ERROR_CODE.NEX_FAIL_CANNOT_EQUIP_ITEM;
		}
		if (m_lstPrivateUnitID.Count > 0 && !NKMUnitManager.CheckContainsBaseUnit(m_lstPrivateUnitID, unitID))
		{
			return NKM_ERROR_CODE.NEX_FAIL_CANNOT_EQUIP_ITEM;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE CanEquipByUnit(NKMUserData userdata, NKMUnitData unitData)
	{
		if (unitData.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED;
		}
		if (NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID).m_NKM_UNIT_STYLE_TYPE != m_EquipUnitStyleType)
		{
			return NKM_ERROR_CODE.NEX_FAIL_CANNOT_EQUIP_ITEM;
		}
		if (m_lstPrivateUnitID.Count > 0 && !NKMUnitManager.CheckContainsBaseUnit(m_lstPrivateUnitID, unitData.m_UnitID))
		{
			return NKM_ERROR_CODE.NEX_FAIL_CANNOT_EQUIP_ITEM;
		}
		return userdata.m_ArmyData.GetUnitDeckState(unitData) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKM_ERROR_CODE.NEC_OK, 
		};
	}

	public NKM_ERROR_CODE CanUnEquipByUnit(NKMUserData userdata, NKMUnitData unit)
	{
		return userdata.m_ArmyData.GetUnitDeckState(unit) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKM_ERROR_CODE.NEC_OK, 
		};
	}

	public int MakeSetOption(int currentSetOption = 0)
	{
		if (m_lstSetGroup == null || m_lstSetGroup.Count == 0)
		{
			return 0;
		}
		List<int> list = m_lstSetGroup;
		if (currentSetOption != 0)
		{
			list = m_lstSetGroup.Where((int e) => e != currentSetOption).ToList();
		}
		int index = RandomGenerator.Next(list.Count);
		return list[index];
	}

	public bool ValidSetOption(int setOptionId)
	{
		if (m_lstSetGroup != null)
		{
			if (m_lstSetGroup.Count != 0)
			{
				if (!m_lstSetGroup.Contains(setOptionId))
				{
					return false;
				}
				return true;
			}
			return false;
		}
		return false;
	}

	public bool CheckEquipEnable(int unitId)
	{
		if (!IsPrivateEquip())
		{
			return true;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitTempletBase.Find(unitId);
		if (unitTempletBase == null)
		{
			return false;
		}
		return m_lstPrivateUnitID.Any((int e) => unitTempletBase.IsSameBaseUnit(e));
	}

	public static NKMEquipTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 230))
		{
			return null;
		}
		NKMEquipTemplet nKMEquipTemplet = new NKMEquipTemplet();
		bool flag = true;
		flag &= lua.GetData("m_ItemEquipID", ref nKMEquipTemplet.m_ItemEquipID);
		flag &= lua.GetData("m_ItemEquipStrID", ref nKMEquipTemplet.m_ItemEquipStrID);
		flag &= lua.GetData("m_ItemEquipName", ref nKMEquipTemplet.m_ItemEquipName);
		flag &= lua.GetData("m_ItemEquipDesc", ref nKMEquipTemplet.m_ItemEquipDesc);
		lua.GetDataList("m_lstSearchKeyword", out nKMEquipTemplet.m_lstSearchKeyword, nullIfEmpty: true);
		flag &= lua.GetData("m_NKM_ITEM_GRADE", ref nKMEquipTemplet.m_NKM_ITEM_GRADE);
		flag &= lua.GetData("m_NKM_ITEM_TIER", ref nKMEquipTemplet.m_NKM_ITEM_TIER);
		flag &= lua.GetData("m_ItemEquipPosition", ref nKMEquipTemplet.m_ItemEquipPosition);
		flag &= lua.GetData("m_EquipUnitStyleType", ref nKMEquipTemplet.m_EquipUnitStyleType);
		flag &= lua.GetData("m_ItemEquipIconName", ref nKMEquipTemplet.m_ItemEquipIconName);
		flag &= lua.GetData("m_MaxEnchantLevel", ref nKMEquipTemplet.m_MaxEnchantLevel);
		int rValue = 0;
		int rValue2 = 0;
		lua.GetData("m_OnRemoveItemID_1", ref rValue);
		lua.GetData("m_OnRemoveItemCount_1", ref rValue2);
		if (rValue > 0 && rValue2 > 0)
		{
			nKMEquipTemplet.m_OnRemoveItemList.Add(new OnRemoveItemData(rValue, rValue2));
		}
		rValue = 0;
		rValue2 = 0;
		lua.GetData("m_OnRemoveItemID_2", ref rValue);
		lua.GetData("m_OnRemoveItemCount_2", ref rValue2);
		if (rValue > 0 && rValue2 > 0)
		{
			nKMEquipTemplet.m_OnRemoveItemList.Add(new OnRemoveItemData(rValue, rValue2));
		}
		if (lua.OpenTable("m_lstPrivateUnitID"))
		{
			nKMEquipTemplet.m_lstPrivateUnitID.Clear();
			int i = 1;
			for (int rValue3 = 0; lua.GetData(i, ref rValue3); i++)
			{
				nKMEquipTemplet.m_lstPrivateUnitID.Add(rValue3);
			}
			lua.CloseTable();
		}
		nKMEquipTemplet.m_StatType = NKM_STAT_TYPE.NST_END;
		lua.GetData("STAT_TYPE_1", ref nKMEquipTemplet.m_StatType);
		flag &= lua.GetData("STAT_VALUE_1", ref nKMEquipTemplet.m_StatData.m_fBaseValue);
		flag &= lua.GetData("STAT_LEVELUP_VALUE_1", ref nKMEquipTemplet.m_StatData.m_fLevelUpValue);
		lua.GetData("m_StatGroupID", ref nKMEquipTemplet.m_StatGroupID);
		lua.GetData("m_StatGroupID_2", ref nKMEquipTemplet.m_StatGroupID_2);
		lua.GetData("m_FeedEXP", ref nKMEquipTemplet.m_FeedEXP);
		lua.GetData("m_PrecisionReqResource", ref nKMEquipTemplet.m_PrecisionReqResource);
		lua.GetData("m_PrecisionReqItem", ref nKMEquipTemplet.m_PrecisionReqItem);
		lua.GetData("m_RandomStatReqResource", ref nKMEquipTemplet.m_RandomStatReqResource);
		lua.GetData("m_RandomStatReqItem", ref nKMEquipTemplet.m_RandomStatReqItem);
		lua.GetData("m_RandomSetReqResource", ref nKMEquipTemplet.m_RandomSetReqResource);
		lua.GetData("m_RandomSetReqItemID", ref nKMEquipTemplet.m_RandomSetReqItemID);
		lua.GetData("m_RandomSetReqItemValue", ref nKMEquipTemplet.m_RandomSetReqItemValue);
		lua.GetData("m_bItemFirstLock", ref nKMEquipTemplet.m_bItemFirstLock);
		lua.GetData("m_bRelic", ref nKMEquipTemplet.m_bRelic);
		lua.GetData("m_PotentialOptionGroupID", ref nKMEquipTemplet.potentialOptionGroupId);
		lua.GetData("m_SubPotentialOptionGroupID", ref nKMEquipTemplet.potentialOptionGroupId2);
		lua.GetData("m_bEffect", ref nKMEquipTemplet.m_bShowEffect);
		lua.GetData("m_RelicRerollReqResource", ref nKMEquipTemplet.m_RelicRerollReqResource);
		lua.GetData("m_RelicRerollReqResourceFactor", ref nKMEquipTemplet.m_RelicRerollReqResourceFactor);
		if (lua.OpenTable("m_SetGroup"))
		{
			nKMEquipTemplet.m_lstSetGroup = new List<int>();
			int j = 1;
			for (int rValue4 = 0; lua.GetData(j, ref rValue4); j++)
			{
				nKMEquipTemplet.m_lstSetGroup.Add(rValue4);
			}
			lua.CloseTable();
		}
		if (nKMEquipTemplet.m_bRelic)
		{
			for (int k = 0; k < nKMEquipTemplet.socketReqResource.Length; k++)
			{
				int num = k + 1;
				List<MiscItemUnit> list = new List<MiscItemUnit>();
				nKMEquipTemplet.socketReqResource[k] = list;
				int itemId = 1;
				lua.GetData($"Socket{num}_ReqResource", out var rValue5, 0L);
				if (rValue5 > 0)
				{
					list.Add(new MiscItemUnit(itemId, rValue5));
				}
				itemId = 0;
				rValue5 = 0L;
				lua.GetData($"Socket{num}_OpenItemID", ref itemId);
				lua.GetData($"Socket{num}_OpenCount", ref rValue5);
				if (itemId > 0)
				{
					list.Add(new MiscItemUnit(itemId, rValue5));
				}
			}
			if (nKMEquipTemplet.m_RelicRerollItemMisc == null)
			{
				int rValue6 = 0;
				int rValue7 = 0;
				lua.GetData("m_RelicRerollReqItemID", ref rValue6);
				lua.GetData("m_RelicRerollReqItemValue", ref rValue7);
				nKMEquipTemplet.m_RelicRerollItemMisc = new MiscItemUnit(rValue6, rValue7);
			}
		}
		if (!flag)
		{
			Log.Error($"NKMEquipTemplet Load - {nKMEquipTemplet.m_ItemEquipID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 361);
			return null;
		}
		return nKMEquipTemplet;
	}

	public void Join()
	{
		if (NKMUnitTempletBase.IsUnitStyleType(m_EquipUnitStyleType))
		{
			PresetType = UnitStyleToEquipPreset(m_EquipUnitStyleType);
		}
		if (m_StatGroupID > 0)
		{
			if (!NKMEquipTuningManager.TryGetStatGroupTemplet(m_StatGroupID, out var result))
			{
				NKMTempletError.Add($"[EquipTemplet:{m_ItemEquipID}] 랜덤스탯 그룹이 존재하지 않음 m_StatGroupID:{m_StatGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 379);
			}
			else
			{
				statGroups[0] = result;
			}
		}
		if (m_StatGroupID_2 > 0)
		{
			if (!NKMEquipTuningManager.TryGetStatGroupTemplet(m_StatGroupID_2, out var result2))
			{
				NKMTempletError.Add($"[EquipTemplet:{m_ItemEquipID}] 랜덤스탯 그룹이 존재하지 않음 m_StatGroupID:{m_StatGroupID_2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 391);
			}
			else
			{
				statGroups[1] = result2;
			}
		}
		if (!m_bRelic)
		{
			return;
		}
		foreach (MiscItemUnit item in socketReqResource.SelectMany((List<MiscItemUnit> e) => e))
		{
			item.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 403);
		}
		m_RelicRerollItemMisc.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 406);
		PotentialOptionGroupTemplet = NKMPotentialOptionGroupTemplet.Find(potentialOptionGroupId);
		if (PotentialOptionGroupTemplet == null)
		{
			NKMTempletError.Add($"[EquipTemplet:{m_ItemEquipID}] 잠재 1옵션 그룹아이디가 유효하지 않음:{potentialOptionGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 411);
		}
		if (potentialOptionGroupId2 > 0)
		{
			PotentialOptionGroupTemplet2 = NKMPotentialOptionGroupTemplet.Find(potentialOptionGroupId2);
			if (PotentialOptionGroupTemplet2 == null)
			{
				NKMTempletError.Add($"[EquipTemplet:{m_ItemEquipID}] 잠재 2옵션 그룹아이디가 유효하지 않음:{potentialOptionGroupId2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 419);
			}
		}
	}

	public void Validate()
	{
		if (NKMUnitTempletBase.IsUnitStyleType(m_EquipUnitStyleType) && (PresetType == NKM_EQUIP_PRESET_TYPE.NEPT_INVLID || PresetType == NKM_EQUIP_PRESET_TYPE.NEPT_NONE))
		{
			NKMTempletError.Add($"[Equip:{Key}] 장비 프리셋 타입 계산 오류. styleType:{m_EquipUnitStyleType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 432);
			return;
		}
		foreach (int item in m_lstPrivateUnitID)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(item);
			if (nKMUnitTempletBase == null)
			{
				NKMTempletError.Add($"[Equip:{Key}] 잘못된 유닛아이디(전용장비 대상):{item}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 442);
			}
			else if (nKMUnitTempletBase.IsRearmUnit)
			{
				NKMTempletError.Add($"[Equip:{Key}] 전용장비 대상 unitId에는 재무장 유닛id를 넣을 수 없음. unitId:{item} rearmGrade:{nKMUnitTempletBase.m_RearmGrade}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 449);
			}
		}
		if (m_lstSetGroup != null)
		{
			foreach (int item2 in m_lstSetGroup)
			{
				if (item2 != 0 && !NKMItemManager.m_dicItemEquipSetOptionTempletByID.ContainsKey(item2))
				{
					NKMTempletError.Add($"[EquipTemplet] 세트옵션이 존재하지 않음. m_ItemEquipID : {m_ItemEquipID}, SetOptionId:{item2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 460);
				}
			}
		}
		if (!m_bRelic)
		{
			if (m_RelicRerollReqResource > 0 || (double)m_RelicRerollReqResourceFactor > 0.0)
			{
				NKMTempletError.Add($"[EquipTemplet] 렐릭장비가 아닌데 잠재능력 변경 값이 있음.  m_ItemEquipID : {m_ItemEquipID}, m_RelicRerollReqResource:{m_RelicRerollReqResource}, m_RelicRerollReqResourceFactor: {m_RelicRerollReqResourceFactor}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 469);
				return;
			}
			if (m_RelicRerollItemMisc != null)
			{
				NKMTempletError.Add($"[EquipTemplet] 렐릭장비가 아닌데 잠재능력 변경 값이 있음.  m_ItemEquipID : {m_ItemEquipID}, m_RandomRelicReqItemID:{m_RelicRerollItemMisc.ItemId}, m_RandomRelicReqItemValue: {m_RelicRerollItemMisc.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 475);
			}
		}
		if (m_bRelic)
		{
			if (potentialOptionGroupId < 0)
			{
				NKMTempletError.Add($"[EquipTemplet] 렐릭 아이템에 잠재옵션 group Id 값이 비정상인 경우.  m_ItemEquipID : {m_ItemEquipID}, potentialOptionGroupId: {potentialOptionGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 483);
			}
			if (m_RelicRerollReqResource <= 0 || (double)m_RelicRerollReqResourceFactor <= 0.0)
			{
				NKMTempletError.Add($"[EquipTemplet] 렐릭 아이템에 잠재능력 변경 재화 값 혹은 재화 계산 배율값이 비정상.  m_ItemEquipID : {m_ItemEquipID}, m_RelicRerollReqResource:{m_RelicRerollReqResource}, m_RelicRerollReqResourceFactor: {m_RelicRerollReqResourceFactor}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 488);
			}
			if (m_RelicRerollItemMisc == null)
			{
				NKMTempletError.Add($"[EquipTemplet] 렐릭 아이템에 잠재능력 변경 재화 정보가 없음.  m_ItemEquipID : {m_ItemEquipID}, m_RandomRelicReqItemID:{m_RelicRerollItemMisc.ItemId}, m_RandomRelicReqItemValue: {m_RelicRerollItemMisc.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipTemplet.cs", 493);
			}
		}
	}

	public bool IsValidEquipPosition(ITEM_EQUIP_POSITION equipPosition)
	{
		switch (equipPosition)
		{
		case ITEM_EQUIP_POSITION.IEP_WEAPON:
		case ITEM_EQUIP_POSITION.IEP_DEFENCE:
			return m_ItemEquipPosition == equipPosition;
		case ITEM_EQUIP_POSITION.IEP_ACC:
		case ITEM_EQUIP_POSITION.IEP_ACC2:
			return m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ACC;
		default:
			return false;
		}
	}

	public static NKM_EQUIP_PRESET_TYPE UnitStyleToEquipPreset(NKM_UNIT_STYLE_TYPE unitStyleType)
	{
		return unitStyleType switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_COUNTER => NKM_EQUIP_PRESET_TYPE.NEPT_COUNTER, 
			NKM_UNIT_STYLE_TYPE.NUST_SOLDIER => NKM_EQUIP_PRESET_TYPE.NEPT_SOLDIER, 
			NKM_UNIT_STYLE_TYPE.NUST_MECHANIC => NKM_EQUIP_PRESET_TYPE.NEPT_MECHANIC, 
			_ => NKM_EQUIP_PRESET_TYPE.NEPT_INVLID, 
		};
	}

	public string GetItemName()
	{
		return NKCStringTable.GetString(m_ItemEquipName);
	}

	public string GetItemDesc()
	{
		return NKCStringTable.GetString(m_ItemEquipDesc);
	}

	public bool IsRelic()
	{
		return m_bRelic;
	}

	public int GetPotentialOptionGroupID()
	{
		return potentialOptionGroupId;
	}

	public int GetPotentialOptionGroupID2()
	{
		return potentialOptionGroupId2;
	}

	public List<MiscItemUnit> GetSocketOpenResource(int socketIndex)
	{
		if (socketReqResource.Length <= socketIndex)
		{
			return new List<MiscItemUnit>();
		}
		return socketReqResource[socketIndex];
	}

	public List<MiscItemUnit> GetRelicRerollResource(int changedCount = 0)
	{
		List<MiscItemUnit> list = new List<MiscItemUnit>();
		long num = m_RelicRerollReqResource;
		double value = (double)m_RelicRerollReqResourceFactor * Math.Pow(changedCount, NKMCommonConst.RelicRerollCountFactor);
		value = Math.Round(value, 2);
		num = ((changedCount == 0) ? m_RelicRerollReqResource : ((long)((double)m_RelicRerollReqResource * value)));
		list.Add(new MiscItemUnit(1, num));
		if (m_RelicRerollItemMisc != null)
		{
			list.Add(m_RelicRerollItemMisc);
		}
		return list;
	}
}
