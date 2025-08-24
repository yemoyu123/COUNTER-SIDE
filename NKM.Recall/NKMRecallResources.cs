using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet;
using NKM.Unit;

namespace NKM.Recall;

public sealed class NKMRecallResources
{
	private readonly Dictionary<int, NKMItemMiscData> resourcesMap = new Dictionary<int, NKMItemMiscData>();

	private readonly NKMUnitData unitData;

	public IEnumerable<NKMItemMiscData> Resources => resourcesMap.Values;

	public NKMRecallResources(NKMUnitData unitData)
	{
		this.unitData = unitData;
	}

	public void Recall()
	{
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitData.m_UnitID);
		if (nKMUnitTempletBase != null)
		{
			if (nKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				ConvertUnitToResources(nKMUnitTempletBase);
			}
			else if (nKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				ConvertShipToResources();
			}
		}
	}

	private void ConvertShipToResources()
	{
		if (unitData.m_UnitLevel <= 1)
		{
			return;
		}
		List<int> list = new List<int>();
		int num = unitData.m_UnitID;
		for (NKMShipBuildTemplet shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(num); shipBuildTemplet != null; shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(num))
		{
			if (shipBuildTemplet.UpgradeMaterialList.Count > 0)
			{
				foreach (UpgradeMaterial upgradeMaterial in shipBuildTemplet.UpgradeMaterialList)
				{
					AddResource(upgradeMaterial.m_ShipUpgradeMaterial, upgradeMaterial.m_ShipUpgradeMaterialCount);
				}
			}
			if (shipBuildTemplet.ShipUpgradeCredit > 0)
			{
				AddResource(1, shipBuildTemplet.ShipUpgradeCredit);
			}
			list.Insert(0, num);
			num -= 1000;
		}
		int num2 = 0;
		foreach (int item in list)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(item);
			if (nKMUnitTempletBase == null)
			{
				Log.Error($"Can not found UnitTempletBase. ShipId:{item}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Recall/NKMRecallResources.cs", 76);
				continue;
			}
			NKMShipLevelUpTemplet nKMShipLevelUpTemplet = NKMShipLevelUpTemplet.Find(nKMUnitTempletBase.m_StarGradeMax, nKMUnitTempletBase.m_NKM_UNIT_GRADE, unitData.m_LimitBreakLevel);
			if (nKMShipLevelUpTemplet == null)
			{
				Log.Error($"Can not found ShipLevelIpTemplet. GradeMax:{nKMUnitTempletBase.m_StarGradeMax} Grade:{nKMUnitTempletBase.m_NKM_UNIT_GRADE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Recall/NKMRecallResources.cs", 83);
				continue;
			}
			int num3 = Math.Min(nKMShipLevelUpTemplet.ShipMaxLevel, unitData.m_UnitLevel);
			num3 -= num2;
			if (num3 <= 0)
			{
				break;
			}
			foreach (LevelupMaterial shipLevelupMaterial in nKMShipLevelUpTemplet.ShipLevelupMaterialList)
			{
				int count = shipLevelupMaterial.m_LevelupMaterialCount * num3;
				AddResource(shipLevelupMaterial.m_LevelupMaterialItemID, count);
			}
			int count2 = nKMShipLevelUpTemplet.ShipUpgradeCredit * num3;
			AddResource(1, count2);
			num2 += num3;
		}
	}

	private void ConvertUnitToResources(NKMUnitTempletBase unitTempletBase)
	{
		ConvertUnitExpToResources();
		ConvertLimitBreakToResources(unitTempletBase);
		ConvertSkillToResources();
		if (unitData.IsPermanentContract)
		{
			AddResource(1001, 1);
		}
	}

	private void ConvertLimitBreakToResources(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase.BaseUnit != null)
		{
			unitTempletBase = unitTempletBase.BaseUnit;
		}
		int num = unitData.m_LimitBreakLevel;
		while (num > 0)
		{
			NKMLimitBreakItemTemplet lBSubstituteItemInfo = NKMUnitLimitBreakManager.GetLBSubstituteItemInfo(unitTempletBase.m_NKM_UNIT_STYLE_TYPE, unitTempletBase.m_NKM_UNIT_GRADE, num);
			if (lBSubstituteItemInfo == null)
			{
				Log.Error($"Can not found NKMLimitBreakItemTemplet {unitTempletBase.m_NKM_UNIT_STYLE_TYPE}, {unitTempletBase.m_NKM_UNIT_GRADE}, {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Recall/NKMRecallResources.cs", 131);
				num--;
				continue;
			}
			foreach (NKMLimitBreakItemTemplet.ItemRequirement item in lBSubstituteItemInfo.m_lstRequiredItem)
			{
				AddResource(item.itemID, item.count);
			}
			AddResource(1, lBSubstituteItemInfo.m_CreditReq);
			num--;
		}
	}

	private void ConvertUnitExpToResources()
	{
		NKMUnitExpTemplet nKMUnitExpTemplet = unitData?.GetExpTemplet();
		if (nKMUnitExpTemplet == null)
		{
			return;
		}
		int num = nKMUnitExpTemplet.m_iExpCumulated + unitData.m_iUnitLevelEXP;
		if (num > 0 && NKMCommonConst.Negotiation.Materials.Count != 0)
		{
			NegotiationMaterial negotiationMaterial = NKMCommonConst.Negotiation.Materials.OrderByDescending((NegotiationMaterial e) => e.Exp).First();
			int num2 = (int)Math.Ceiling((float)num / (float)negotiationMaterial.Exp);
			if (num2 != 0)
			{
				AddResource(negotiationMaterial.ItemId, num2);
				AddResource(1, negotiationMaterial.Credit * num2);
			}
		}
	}

	private void ConvertSkillToResources()
	{
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitData.m_UnitID);
		if (nKMUnitTempletBase == null)
		{
			return;
		}
		for (int i = 0; i < unitData.m_aUnitSkillLevel.Length; i++)
		{
			int num = unitData.m_aUnitSkillLevel[i];
			string text = nKMUnitTempletBase.m_lstSkillStrID[i];
			while (num > 1)
			{
				NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(text, num);
				if (skillTemplet == null)
				{
					Log.Error($"Can not found UnitSkillTemplet. {text}, {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Recall/NKMRecallResources.cs", 195);
					continue;
				}
				foreach (NKMUnitSkillTemplet.NKMUpgradeReqItem item in skillTemplet.m_lstUpgradeReqItem)
				{
					AddResource(item.ItemID, item.ItemCount);
				}
				num--;
			}
		}
	}

	private void AddResource(int itemId, int count)
	{
		if (!resourcesMap.ContainsKey(itemId))
		{
			resourcesMap.Add(itemId, new NKMItemMiscData
			{
				ItemID = itemId,
				CountFree = count,
				CountPaid = 0L,
				BonusRatio = 0
			});
		}
		else
		{
			resourcesMap[itemId].CountFree += count;
		}
	}
}
