using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using NKM.Templet.Recall;
using NKM.Unit;
using UnityEngine;

namespace NKC;

public static class NKCRecallManager
{
	public static bool m_bWaitingRecallProcess = false;

	private static int m_EquippedCount = 0;

	private static NKMUnitData m_SourceUnitData = null;

	private static Dictionary<int, int> m_dicTargetUnitID = new Dictionary<int, int>();

	public static void UnEquipAndRecall(NKMUnitData sourceUnitData, Dictionary<int, int> targetUnitIDs)
	{
		m_EquippedCount = 0;
		m_SourceUnitData = sourceUnitData;
		m_dicTargetUnitID = targetUnitIDs;
		m_EquippedCount += SendEquipItem(bEquip: false, m_SourceUnitData.GetEquipItemWeaponUid(), ITEM_EQUIP_POSITION.IEP_WEAPON);
		m_EquippedCount += SendEquipItem(bEquip: false, m_SourceUnitData.GetEquipItemDefenceUid(), ITEM_EQUIP_POSITION.IEP_DEFENCE);
		m_EquippedCount += SendEquipItem(bEquip: false, m_SourceUnitData.GetEquipItemAccessoryUid(), ITEM_EQUIP_POSITION.IEP_ACC);
		m_EquippedCount += SendEquipItem(bEquip: false, m_SourceUnitData.GetEquipItemAccessory2Uid(), ITEM_EQUIP_POSITION.IEP_ACC2);
		m_bWaitingRecallProcess = true;
		if (m_EquippedCount == 0)
		{
			OnUnequipComplete();
		}
	}

	private static int SendEquipItem(bool bEquip, long targetEquipUId, ITEM_EQUIP_POSITION equipPosition)
	{
		if (m_SourceUnitData == null || targetEquipUId <= 0)
		{
			return 0;
		}
		NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip, m_SourceUnitData.m_UnitUID, targetEquipUId, equipPosition);
		return 1;
	}

	public static void OnUnequipComplete()
	{
		if (m_bWaitingRecallProcess && m_EquippedCount > 0)
		{
			m_EquippedCount--;
			if (m_bWaitingRecallProcess && m_EquippedCount == 0)
			{
				NKCPacketSender.Send_NKMPacket_RECALL_UNIT_REQ(m_SourceUnitData.m_UnitUID, m_dicTargetUnitID);
				m_bWaitingRecallProcess = false;
				m_SourceUnitData = null;
				m_dicTargetUnitID.Clear();
			}
		}
		else if (m_EquippedCount == 0)
		{
			NKCPacketSender.Send_NKMPacket_RECALL_UNIT_REQ(m_SourceUnitData.m_UnitUID, m_dicTargetUnitID);
			m_bWaitingRecallProcess = false;
			m_SourceUnitData = null;
			m_dicTargetUnitID.Clear();
		}
	}

	public static bool IsRecallTargetUnit(NKMUnitData unitData, DateTime utcNow)
	{
		if (unitData == null)
		{
			return false;
		}
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitData.m_UnitID);
		if (nKMUnitTempletBase == null)
		{
			return false;
		}
		int num = unitData.m_UnitID;
		if (nKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			num = GetFirstLevelShipID(num);
		}
		NKMRecallTemplet nKMRecallTemplet = NKMRecallTemplet.Find(num, NKMTime.UTCtoLocal(utcNow));
		if (nKMRecallTemplet == null)
		{
			return false;
		}
		if (!IsValidRegTime(nKMRecallTemplet, unitData.m_regDate))
		{
			return false;
		}
		return IsRecallTargetUnit(num, utcNow);
	}

	private static bool IsRecallTargetUnit(int unitID, DateTime utcNow)
	{
		NKMRecallTemplet nKMRecallTemplet = NKMRecallTemplet.Find(unitID, NKMTime.UTCtoLocal(utcNow));
		if (nKMRecallTemplet != null)
		{
			if (NKCScenManager.CurrentUserData().m_RecallHistoryData.TryGetValue(unitID, out var value))
			{
				return !IsValidTime(nKMRecallTemplet, value.lastUpdateDate);
			}
			return true;
		}
		return false;
	}

	public static int GetFirstLevelShipID(int shipID)
	{
		NKMShipBuildTemplet nKMShipBuildTemplet = NKMShipBuildTemplet.Find(shipID);
		if (nKMShipBuildTemplet == null)
		{
			return shipID;
		}
		while (NKMShipBuildTemplet.Find(shipID) != null)
		{
			nKMShipBuildTemplet = NKMShipBuildTemplet.Find(shipID);
			shipID -= 1000;
		}
		return nKMShipBuildTemplet.ShipID;
	}

	public static bool IsValidTime(NKMRecallTemplet recallTemplet, DateTime utcNow)
	{
		if (utcNow < recallTemplet.IntervalTemplet.GetStartDateUtc() || utcNow >= recallTemplet.IntervalTemplet.GetEndDateUtc())
		{
			return false;
		}
		return true;
	}

	public static bool IsValidRegTime(NKMRecallTemplet recallTemplet, DateTime regDateUtc)
	{
		if (regDateUtc < recallTemplet.IntervalTemplet.GetStartDateUtc() || regDateUtc >= recallTemplet.IntervalTemplet.GetEndDateUtc())
		{
			return true;
		}
		return false;
	}

	public static Dictionary<int, NKMItemMiscData> ConvertLimitBreakToResources(NKMUnitData unitData)
	{
		Dictionary<int, NKMItemMiscData> resourcesMap = new Dictionary<int, NKMItemMiscData>();
		if (unitData != null)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitData.m_UnitID);
			if (nKMUnitTempletBase != null)
			{
				int num = unitData.m_LimitBreakLevel;
				if (nKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
				{
					if (nKMUnitTempletBase.BaseUnit != null)
					{
						nKMUnitTempletBase = nKMUnitTempletBase.BaseUnit;
					}
					while (num > 0)
					{
						NKMLimitBreakItemTemplet lBSubstituteItemInfo = NKMUnitLimitBreakManager.GetLBSubstituteItemInfo(nKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE, nKMUnitTempletBase.m_NKM_UNIT_GRADE, num);
						NKMLimitBreakTemplet lBInfo = NKMUnitLimitBreakManager.GetLBInfo(num);
						if (lBSubstituteItemInfo == null)
						{
							Log.Error($"Can not found NKMLimitBreakItemTemplet {nKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE}, {nKMUnitTempletBase.m_NKM_UNIT_GRADE}, {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCRecallManager.cs", 173);
							num--;
							continue;
						}
						if (lBInfo == null)
						{
							Log.Error($"Can not found NKMLimitBreakTemplet {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCRecallManager.cs", 180);
							num--;
							continue;
						}
						foreach (NKMLimitBreakItemTemplet.ItemRequirement item in lBSubstituteItemInfo.m_lstRequiredItem)
						{
							if (resourcesMap.ContainsKey(item.itemID))
							{
								resourcesMap[item.itemID].CountFree += item.count * lBInfo.m_iUnitRequirement;
							}
							else
							{
								resourcesMap.Add(item.itemID, new NKMItemMiscData(item.itemID, item.count * lBInfo.m_iUnitRequirement, 0L));
							}
						}
						if (resourcesMap.ContainsKey(1))
						{
							resourcesMap[1].CountFree += lBSubstituteItemInfo.m_CreditReq;
						}
						else
						{
							resourcesMap.Add(1, new NKMItemMiscData(1, lBSubstituteItemInfo.m_CreditReq, 0L));
						}
						num--;
					}
				}
				else if (nKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
				{
					while (num > 0)
					{
						NKMShipLimitBreakTemplet shipLimitBreakTemplet = NKMShipManager.GetShipLimitBreakTemplet(nKMUnitTempletBase.m_UnitID, num);
						if (shipLimitBreakTemplet == null)
						{
							Log.Error($"Can not found NKMShipLimitBreakTemplet {nKMUnitTempletBase.m_UnitID}, {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCRecallManager.cs", 209);
							num--;
							continue;
						}
						if (shipLimitBreakTemplet.ListMaterialShipId.Count > 0)
						{
							int num2 = int.MaxValue;
							for (int i = 0; i < shipLimitBreakTemplet.ListMaterialShipId.Count; i++)
							{
								if (num2 < shipLimitBreakTemplet.ListMaterialShipId[i])
								{
									num2 = shipLimitBreakTemplet.ListMaterialShipId[i];
								}
							}
							NKMShipBuildTemplet nKMShipBuildTemplet = NKMShipBuildTemplet.Find(NKMShipManager.GetMinLevelShipID(num2));
							if (nKMShipBuildTemplet == null)
							{
								continue;
							}
							AddResource(nKMShipBuildTemplet.BuildMaterialList, ref resourcesMap);
						}
						foreach (MiscItemUnit shipLimitBreakItem in shipLimitBreakTemplet.ShipLimitBreakItems)
						{
							if (resourcesMap.ContainsKey(shipLimitBreakItem.ItemId))
							{
								resourcesMap[shipLimitBreakItem.ItemId].CountFree += shipLimitBreakItem.Count;
							}
							else
							{
								resourcesMap.Add(shipLimitBreakItem.ItemId, new NKMItemMiscData(shipLimitBreakItem.ItemId, shipLimitBreakItem.Count, 0L));
							}
						}
						num--;
					}
				}
			}
		}
		return resourcesMap;
	}

	public static Dictionary<int, NKMItemMiscData> ConvertUnitExpToResources(NKMUnitData unitData)
	{
		Dictionary<int, NKMItemMiscData> resourcesMap = new Dictionary<int, NKMItemMiscData>();
		NKMUnitExpTemplet nKMUnitExpTemplet = unitData?.GetExpTemplet();
		if (nKMUnitExpTemplet == null)
		{
			return resourcesMap;
		}
		int num = 0;
		NKMUnitTempletBase unitTempletBase = unitData.GetUnitTempletBase();
		if (unitTempletBase != null && unitTempletBase.IsRearmUnit)
		{
			NKMUnitExpTemplet nKMUnitExpTemplet2 = NKMUnitExpTemplet.FindByUnitId(unitTempletBase.BaseUnit.m_UnitID, 110);
			if (nKMUnitExpTemplet2 != null)
			{
				num = nKMUnitExpTemplet2.m_iExpCumulated;
			}
		}
		int num2 = nKMUnitExpTemplet.m_iExpCumulated + unitData.m_iUnitLevelEXP + num;
		if (num2 <= 0)
		{
			return resourcesMap;
		}
		if (NKMCommonConst.Negotiation.Materials.Count == 0)
		{
			return resourcesMap;
		}
		NegotiationMaterial negotiationMaterial = NKMCommonConst.Negotiation.Materials.OrderByDescending((NegotiationMaterial e) => e.Exp).First();
		int num3 = (int)Math.Ceiling((float)num2 / (float)negotiationMaterial.Exp);
		if (num3 == 0)
		{
			return resourcesMap;
		}
		AddResource(negotiationMaterial.ItemId, num3, ref resourcesMap);
		AddResource(1, negotiationMaterial.Credit * num3, ref resourcesMap);
		return resourcesMap;
	}

	public static Dictionary<int, NKMItemMiscData> ConvertSkillToResources(NKMUnitData unitData)
	{
		Dictionary<int, NKMItemMiscData> resourcesMap = new Dictionary<int, NKMItemMiscData>();
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitData.m_UnitID);
		if (nKMUnitTempletBase == null)
		{
			return resourcesMap;
		}
		int num = 0;
		for (int i = 0; i < unitData.m_aUnitSkillLevel.Length && i < nKMUnitTempletBase.m_lstSkillStrID.Count; i++)
		{
			int num2 = unitData.m_aUnitSkillLevel[i];
			string text = nKMUnitTempletBase.m_lstSkillStrID[i];
			while (num2 > 1)
			{
				NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(text, num2);
				if (skillTemplet == null)
				{
					Log.Error($"Can not found UnitSkillTemplet. {text}, {num2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCRecallManager.cs", 318);
					continue;
				}
				foreach (NKMUnitSkillTemplet.NKMUpgradeReqItem item in skillTemplet.m_lstUpgradeReqItem)
				{
					if (NKMItemMiscTemplet.Find(item.ItemID).m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_PIECE)
					{
						num += item.ItemCount;
					}
					else
					{
						AddResource(item.ItemID, item.ItemCount, ref resourcesMap);
					}
				}
				num2--;
			}
		}
		if (num > 0)
		{
			AddResource(401, Mathf.CeilToInt((float)num * NKMCommonConst.RecallRewardUnitPieceToPoint), ref resourcesMap);
		}
		return resourcesMap;
	}

	public static Dictionary<int, NKMItemMiscData> ConvertUnitLifeTimeToResource(NKMUnitData unitData)
	{
		Dictionary<int, NKMItemMiscData> resourcesMap = new Dictionary<int, NKMItemMiscData>();
		if (unitData.IsPermanentContract)
		{
			AddResource(1024, 1, ref resourcesMap);
		}
		return resourcesMap;
	}

	public static Dictionary<int, NKMItemMiscData> ConvertUnitReactorToResource(NKMUnitData unitData)
	{
		Dictionary<int, NKMItemMiscData> resourcesMap = new Dictionary<int, NKMItemMiscData>();
		if (unitData != null && unitData.reactorLevel > 0)
		{
			NKMUnitTempletBase unitTempletBase = unitData.GetUnitTempletBase();
			if (unitTempletBase != null)
			{
				NKMUnitReactorTemplet nKMUnitReactorTemplet = NKMUnitReactorTemplet.Find(unitTempletBase.m_ReactorId);
				if (nKMUnitReactorTemplet != null)
				{
					NKMReactorSkillTemplet[] skillTemplets = nKMUnitReactorTemplet.GetSkillTemplets();
					for (int i = 0; i < unitData.reactorLevel; i++)
					{
						if (skillTemplets[i] == null)
						{
							continue;
						}
						foreach (MiscItemUnit reqItem in skillTemplets[i].ReqItems)
						{
							AddResource(reqItem.ItemId, (int)reqItem.Count, ref resourcesMap);
						}
					}
				}
			}
		}
		return resourcesMap;
	}

	public static Dictionary<int, NKMItemMiscData> ConvertUnitRearmToResource(NKMUnitData unitData)
	{
		Dictionary<int, NKMItemMiscData> resourcesMap = new Dictionary<int, NKMItemMiscData>();
		NKMUnitTempletBase unitTempletBase = unitData.GetUnitTempletBase();
		if (unitTempletBase != null && unitTempletBase.IsRearmUnit)
		{
			NKMUnitRearmamentTemplet rearmamentTemplet = NKCRearmamentUtil.GetRearmamentTemplet(unitTempletBase.m_UnitID);
			if (rearmamentTemplet != null)
			{
				foreach (MiscItemUnit rearmamentUseItem in rearmamentTemplet.RearmamentUseItems)
				{
					AddResource(rearmamentUseItem.ItemId, (int)rearmamentUseItem.Count, ref resourcesMap);
				}
			}
		}
		return resourcesMap;
	}

	public static Dictionary<int, NKMItemMiscData> ConvertRecallConditionToResources(NKMRecallTemplet recallUnitTemplet, NKMUnitData unitData)
	{
		Dictionary<int, NKMItemMiscData> resourcesMap = new Dictionary<int, NKMItemMiscData>();
		if (unitData.GetUnitTempletBase().ContractEnableByTag && unitData.tacticLevel > 0)
		{
			int num = 0;
			switch (recallUnitTemplet.RecallItemCondition)
			{
			case Recall_Condition.DEFAULT:
				num = 1;
				break;
			case Recall_Condition.TACTIC_UPDATE:
				num = unitData.tacticLevel;
				break;
			}
			for (int i = 0; i < num; i++)
			{
				AddResource(recallUnitTemplet.RecallItemID, recallUnitTemplet.RecallItemQuantity, ref resourcesMap);
			}
		}
		return resourcesMap;
	}

	public static Dictionary<int, NKMItemMiscData> ConvertShipBuildToResource(NKMUnitData unitData)
	{
		Dictionary<int, NKMItemMiscData> resourcesMap = new Dictionary<int, NKMItemMiscData>();
		if (unitData.m_UnitLevel < 1)
		{
			return resourcesMap;
		}
		int num = unitData.m_UnitID;
		NKMShipBuildTemplet nKMShipBuildTemplet = NKMShipBuildTemplet.Find(num);
		while (NKMShipBuildTemplet.Find(num) != null)
		{
			nKMShipBuildTemplet = NKMShipBuildTemplet.Find(num);
			num -= 1000;
		}
		AddResource(nKMShipBuildTemplet.BuildMaterialList, ref resourcesMap);
		return resourcesMap;
	}

	public static Dictionary<int, NKMItemMiscData> ConvertShipToResources(NKMUnitData unitData)
	{
		Dictionary<int, NKMItemMiscData> resourcesMap = new Dictionary<int, NKMItemMiscData>();
		if (unitData.m_UnitLevel <= 1)
		{
			return resourcesMap;
		}
		List<int> list = new List<int>();
		int num = unitData.m_UnitID;
		for (NKMShipBuildTemplet shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(num); shipBuildTemplet != null; shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(num))
		{
			AddResource(shipBuildTemplet.UpgradeMaterialList, ref resourcesMap);
			if (shipBuildTemplet.ShipUpgradeCredit > 0)
			{
				AddResource(1, shipBuildTemplet.ShipUpgradeCredit, ref resourcesMap);
			}
			list.Insert(0, num);
			num -= 1000;
		}
		int num2 = 1;
		int num3 = unitData.m_LimitBreakLevel;
		if (num3 > 0)
		{
			while (num3 > 0)
			{
				NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(num);
				if (nKMUnitTempletBase == null)
				{
					Log.Error($"Can not found UnitTempletBase. ShipId:{num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCRecallManager.cs", 488);
					continue;
				}
				NKMShipLevelUpTemplet nKMShipLevelUpTemplet = NKMShipLevelUpTemplet.Find(nKMUnitTempletBase.m_StarGradeMax, nKMUnitTempletBase.m_NKM_UNIT_GRADE, num3);
				if (nKMShipLevelUpTemplet == null)
				{
					Log.Error($"Can not found ShipLevelIpTemplet. GradeMax:{nKMUnitTempletBase.m_StarGradeMax} Grade:{nKMUnitTempletBase.m_NKM_UNIT_GRADE} LimitBreakLevel:{unitData.m_LimitBreakLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCRecallManager.cs", 495);
					continue;
				}
				int num4 = Math.Min(nKMShipLevelUpTemplet.ShipMaxLevel, unitData.m_UnitLevel);
				num4 -= num2;
				if (num4 <= 0)
				{
					break;
				}
				foreach (LevelupMaterial shipLevelupMaterial in nKMShipLevelUpTemplet.ShipLevelupMaterialList)
				{
					AddResource(itemCount: shipLevelupMaterial.m_LevelupMaterialCount * num4, itemID: shipLevelupMaterial.m_LevelupMaterialItemID, resourcesMap: ref resourcesMap);
				}
				int itemCount = nKMShipLevelUpTemplet.ShipUpgradeCredit * num4;
				AddResource(1, itemCount, ref resourcesMap);
				num2 += num4;
				num3--;
			}
		}
		foreach (int item in list)
		{
			NKMUnitTempletBase nKMUnitTempletBase2 = NKMUnitTempletBase.Find(item);
			if (nKMUnitTempletBase2 == null)
			{
				Log.Error($"Can not found UnitTempletBase. ShipId:{item}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCRecallManager.cs", 528);
				continue;
			}
			NKMShipLevelUpTemplet nKMShipLevelUpTemplet2 = NKMShipLevelUpTemplet.Find(nKMUnitTempletBase2.m_StarGradeMax, nKMUnitTempletBase2.m_NKM_UNIT_GRADE);
			if (nKMShipLevelUpTemplet2 == null)
			{
				Log.Error($"Can not found ShipLevelIpTemplet. GradeMax:{nKMUnitTempletBase2.m_StarGradeMax} Grade:{nKMUnitTempletBase2.m_NKM_UNIT_GRADE} LimitBreakLevel:{0}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCRecallManager.cs", 535);
				continue;
			}
			int num5 = Math.Min(nKMShipLevelUpTemplet2.ShipMaxLevel, unitData.m_UnitLevel);
			num5 -= num2;
			if (num5 <= 0)
			{
				break;
			}
			foreach (LevelupMaterial shipLevelupMaterial2 in nKMShipLevelUpTemplet2.ShipLevelupMaterialList)
			{
				AddResource(itemCount: shipLevelupMaterial2.m_LevelupMaterialCount * num5, itemID: shipLevelupMaterial2.m_LevelupMaterialItemID, resourcesMap: ref resourcesMap);
			}
			int itemCount2 = nKMShipLevelUpTemplet2.ShipUpgradeCredit * num5;
			AddResource(1, itemCount2, ref resourcesMap);
			num2 += num5;
		}
		return resourcesMap;
	}

	private static void AddResource(List<BuildMaterial> lstMaterial, ref Dictionary<int, NKMItemMiscData> resourcesMap)
	{
		for (int i = 0; i < lstMaterial.Count; i++)
		{
			AddResource(lstMaterial[i].m_ShipBuildMaterialID, lstMaterial[i].m_ShipBuildMaterialCount, ref resourcesMap);
		}
	}

	private static void AddResource(List<UpgradeMaterial> lstMaterial, ref Dictionary<int, NKMItemMiscData> resourcesMap)
	{
		for (int i = 0; i < lstMaterial.Count; i++)
		{
			AddResource(lstMaterial[i].m_ShipUpgradeMaterial, lstMaterial[i].m_ShipUpgradeMaterialCount, ref resourcesMap);
		}
	}

	private static void AddResource(int itemID, int itemCount, ref Dictionary<int, NKMItemMiscData> resourcesMap)
	{
		if (resourcesMap.ContainsKey(itemID))
		{
			resourcesMap[itemID].CountFree += itemCount;
			return;
		}
		resourcesMap.Add(itemID, new NKMItemMiscData
		{
			ItemID = itemID,
			CountFree = itemCount,
			CountPaid = 0L,
			BonusRatio = 0
		});
	}
}
