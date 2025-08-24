using System.Collections.Generic;
using System.Linq;
using NKM.Contract2.Detail;
using NKM.Templet.Base;

namespace NKM.Contract2;

public static class ContractTempletLoader
{
	public static Dictionary<int, HashSet<int>> m_triggerStageClearIdMap = new Dictionary<int, HashSet<int>>();

	public static Dictionary<int, HashSet<int>> m_triggerShopProductIdMap = new Dictionary<int, HashSet<int>>();

	public static void Load()
	{
		ClearTriggerList();
		NKMTempletContainer<ContractBaseData>.Load("AB_SCRIPT", "LUA_CONTRACT_TAB_TABLE", "CONTRACT_TAB", ContractBaseData.LoadFromLua, (ContractBaseData e) => e.m_ContractStrID);
		NKMTempletContainer<ContractTempletV2>.Load("AB_SCRIPT", "LUA_CONTRACT", "CONTRACT", ContractTempletV2.LoadFromLua, (ContractTempletV2 e) => e.ContractStrID);
		NKMTempletContainer<SelectableContractTemplet>.Load("AB_SCRIPT", "LUA_SELECTABLE_CONTRACT", "SELECTABLE_CONTRACT", SelectableContractTemplet.LoadFromLua, (SelectableContractTemplet e) => e.ContractStrID);
		NKMTempletContainer<MiscContractTemplet>.Load("AB_SCRIPT", "LUA_MISC_CONTRACT", "MISC_CONTRACT", MiscContractTemplet.LoadFromLua);
		NKMTempletContainer<ContractTempletBase>.AddRange(NKMTempletContainer<ContractTempletV2>.Values, (ContractTempletBase e) => e.ContractStrID);
		NKMTempletContainer<ContractTempletBase>.AddRange(NKMTempletContainer<SelectableContractTemplet>.Values, (ContractTempletBase e) => e.ContractStrID);
		NKMTempletContainer<RandomGradeTempletV2>.Load("AB_SCRIPT", "LUA_RANDOM_GRADE_TABLE", "RANDOM_GRADE", RandomGradeTempletV2.LoadFromLua, (RandomGradeTempletV2 e) => e.StringId);
		RandomUnitPoolTempletV2.LoadFile();
		SelectableUnitPoolTemplet.LoadFile();
		NKMTempletContainer<CustomPickupContractTemplet>.Load("AB_SCRIPT", "LUA_CONTRACT_CUSTOM_PICKUP", "CUSTOM_PICKUP", CustomPickupContractTemplet.LoadFromLua);
	}

	public static void ClearTriggerList()
	{
		m_triggerStageClearIdMap.Clear();
		m_triggerShopProductIdMap.Clear();
	}

	public static void AddTriggerShopProductId(int shopProductId, int contractId)
	{
		if (m_triggerShopProductIdMap.TryGetValue(shopProductId, out var value))
		{
			if (value.Contains(contractId))
			{
				NKMTempletError.Add($"[ContractTempletLoader] 중복된 상품구매트리거값[{shopProductId}]이 존재 ContractId[{contractId}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletLoader.cs", 80);
			}
			else
			{
				value.Add(contractId);
			}
		}
		else
		{
			value = new HashSet<int>();
			value.Add(contractId);
			m_triggerShopProductIdMap.Add(shopProductId, value);
		}
	}

	public static void AddTriggerStageClearId(int stageId, int contractId)
	{
		if (m_triggerStageClearIdMap.TryGetValue(stageId, out var value))
		{
			if (value.Contains(contractId))
			{
				NKMTempletError.Add($"[ContractTempletLoader] 중복된 스테이지 클리어 트리거값[{stageId}]이 존재 ContractId[{contractId}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletLoader.cs", 101);
			}
			else
			{
				value.Add(contractId);
			}
		}
		else
		{
			value = new HashSet<int>();
			value.Add(contractId);
			m_triggerStageClearIdMap.Add(stageId, value);
		}
	}

	public static List<int> GetTriggeredShopProductIdList(int shopProductId)
	{
		if (m_triggerShopProductIdMap == null)
		{
			return null;
		}
		if (!m_triggerShopProductIdMap.TryGetValue(shopProductId, out var value))
		{
			return null;
		}
		if (value.Count == 0)
		{
			return null;
		}
		return value.Where((int e) => CheckOpenedContract(e)).ToList();
	}

	public static List<int> GetTriggeredStageClearIdList(int stageId)
	{
		if (m_triggerStageClearIdMap == null)
		{
			return null;
		}
		if (!m_triggerStageClearIdMap.TryGetValue(stageId, out var value))
		{
			return null;
		}
		if (value.Count == 0)
		{
			return null;
		}
		return value.Where((int e) => CheckOpenedContract(e)).ToList();
	}

	public static bool CheckOpenedContract(int contractId)
	{
		ContractTempletBase contractTempletBase = NKMTempletContainer<ContractTempletBase>.Find(contractId);
		if (contractTempletBase != null)
		{
			if (!contractTempletBase.EnableByTag)
			{
				return false;
			}
		}
		else
		{
			CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(contractId);
			if (customPickupContractTemplet == null)
			{
				return false;
			}
			if (!customPickupContractTemplet.EnableByTag)
			{
				return false;
			}
		}
		return true;
	}
}
