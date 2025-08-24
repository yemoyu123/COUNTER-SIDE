using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using NKM.Templet.Base;

namespace NKM.Contract2;

public sealed class BonusGroupTemplet : INKMTemplet
{
	private readonly Dictionary<int, ContractTempletV2> contracts = new Dictionary<int, ContractTempletV2>();

	private readonly Dictionary<int, CustomPickupContractTemplet> customPickUpContracts = new Dictionary<int, CustomPickupContractTemplet>();

	public bool HasContract => contracts.Any();

	public bool HasCustomPickUp => customPickUpContracts.Any();

	public int Key { get; }

	public int RequireTryCount { get; }

	public DateTime ExpireDate { get; private set; }

	public int ShareCount => contracts.Count + customPickUpContracts.Count;

	public BonusGroupTemplet(int groupId, int requireTryCount)
	{
		Key = groupId;
		RequireTryCount = requireTryCount;
	}

	public static BonusGroupTemplet Find(int key)
	{
		return NKMTempletContainer<BonusGroupTemplet>.Find(key);
	}

	public void Join()
	{
		if (ShareCount == 1)
		{
			if (HasContract)
			{
				ExpireDate = contracts.Values.First().ExpireDbDate;
			}
			else if (HasCustomPickUp)
			{
				ExpireDate = customPickUpContracts.Values.First().EndDate;
			}
			else
			{
				ExpireDate = ServiceTime.Forever;
			}
		}
		else
		{
			ExpireDate = ServiceTime.Forever;
		}
	}

	public void Validate()
	{
		if (RequireTryCount == 0)
		{
			string arg = string.Join(", ", contracts.Values.Select((ContractTempletV2 e) => $"[{e.Key}]{e.ContractStrID}"));
			NKMTempletError.Add($"[ContractBonusGroup] invalid requireTryCount:{RequireTryCount} member:{arg}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/BonusGroupTemplet.cs", 59);
		}
		if (HasContract || HasCustomPickUp)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (int item in contracts.Values.Select((ContractTempletV2 e) => e.m_ContractBounsItemReqireCount))
			{
				if (!hashSet.Contains(item))
				{
					hashSet.Add(item);
				}
			}
			foreach (int item2 in customPickUpContracts.Values.Select((CustomPickupContractTemplet e) => e.ContractBounsItemReqireCount))
			{
				if (!hashSet.Contains(item2))
				{
					hashSet.Add(item2);
				}
			}
			if (hashSet.Count > 1)
			{
				NKMTempletError.Add($"[ContractBonusGroup] invalid requireTryCount:{RequireTryCount} member:{hashSet}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/BonusGroupTemplet.cs", 84);
			}
			List<int> list = contracts.Keys.ToList();
			list.AddRange(customPickUpContracts.Keys.ToList());
			if ((from e in list
				group e by e).Count() != list.Count)
			{
				NKMTempletError.Add($"[ContractBonusGroup:{Key}] 중복 Key 값 존재. key:{Key} shareCount:{ShareCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/BonusGroupTemplet.cs", 92);
			}
		}
		if (ShareCount == 1)
		{
			if (HasContract && ExpireDate != contracts.Values.First().ExpireDbDate)
			{
				NKMTempletError.Add($"[ContractBonusGroup:{Key}] 유효기한 계산 오류. expireDate:{ExpireDate} shareCount:{ShareCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/BonusGroupTemplet.cs", 102);
			}
			if (HasCustomPickUp && ExpireDate != customPickUpContracts.Values.First().EndDate)
			{
				NKMTempletError.Add($"[ContractBonusGroup:{Key}] 유효기한 계산 오류. expireDate:{ExpireDate} shareCount:{ShareCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/BonusGroupTemplet.cs", 110);
			}
		}
		if (ShareCount > 1 && ExpireDate != ServiceTime.Forever)
		{
			NKMTempletError.Add($"[ContractBonusGroup:{Key}] 유효기한 계산 오류. expireDate:{ExpireDate} shareCount:{ShareCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/BonusGroupTemplet.cs", 118);
		}
	}

	public void Add(ContractTempletV2 contractTemplet)
	{
		if (!contracts.ContainsKey(contractTemplet.Key))
		{
			contracts.Add(contractTemplet.Key, contractTemplet);
		}
	}

	public void Add(CustomPickupContractTemplet contractTemplet)
	{
		if (!customPickUpContracts.ContainsKey(contractTemplet.Key))
		{
			customPickUpContracts.Add(contractTemplet.Key, contractTemplet);
		}
	}
}
