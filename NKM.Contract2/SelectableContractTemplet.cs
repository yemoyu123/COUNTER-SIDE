using System.Collections.Generic;
using NKM.Contract2.Detail;
using NKM.Templet.Base;

namespace NKM.Contract2;

[SkipDerivedClassJoin]
public sealed class SelectableContractTemplet : ContractTempletBase
{
	private string m_SelectableUnitPoolId;

	public int m_UnitPoolChangeCount;

	public MiscItemUnit m_RequireItem;

	public SelectableUnitPoolTemplet UnitPoolTemplet { get; private set; }

	public override bool IsClosingType => true;

	public static IEnumerable<SelectableContractTemplet> Values => NKMTempletContainer<SelectableContractTemplet>.Values;

	private SelectableContractTemplet(ContractBaseData baseData)
		: base(baseData)
	{
	}

	public static SelectableContractTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableContractTemplet.cs", 27))
		{
			return null;
		}
		if (!lua.GetData("m_ContractID", out var rValue, -1))
		{
			NKMTempletError.Add("[SelectableContract] m_ContractID column loading failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableContractTemplet.cs", 34);
			return null;
		}
		ContractBaseData contractBaseData = NKMTempletContainer<ContractBaseData>.Find(rValue);
		if (contractBaseData == null)
		{
			NKMTempletError.Add($"[SelectableContract] ContractId not found in tabTamplet:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableContractTemplet.cs", 41);
			return null;
		}
		SelectableContractTemplet selectableContractTemplet = new SelectableContractTemplet(contractBaseData);
		int num = (int)(1u & (lua.GetData("m_UnitPoolChangeCount", ref selectableContractTemplet.m_UnitPoolChangeCount) ? 1u : 0u)) & (lua.GetData("m_SelectableUnitPoolId", ref selectableContractTemplet.m_SelectableUnitPoolId) ? 1 : 0);
		if (lua.GetData("m_RequireItemID", out var rValue2, -1))
		{
			lua.GetData("m_RequireItemValue", out var rValue3, -1);
			selectableContractTemplet.m_RequireItem = new MiscItemUnit(rValue2, rValue3);
		}
		if (num == 0)
		{
			NKMTempletError.Add($"[{selectableContractTemplet.Key}]{selectableContractTemplet.ContractStrID} loading failed. id:{selectableContractTemplet.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableContractTemplet.cs", 58);
			return null;
		}
		return selectableContractTemplet;
	}

	public override HashSet<int> GetPriceItemIDSet()
	{
		if (m_RequireItem != null)
		{
			return new HashSet<int> { m_RequireItem.ItemId };
		}
		return new HashSet<int>();
	}

	public static SelectableContractTemplet Find(int key)
	{
		return NKMTempletContainer<SelectableContractTemplet>.Find(key);
	}

	public override void Join()
	{
		m_RequireItem?.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableContractTemplet.cs", 79);
		UnitPoolTemplet = NKMTempletContainer<SelectableUnitPoolTemplet>.Find(m_SelectableUnitPoolId);
		if (UnitPoolTemplet == null)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID} invalid unit pool id:{m_SelectableUnitPoolId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableContractTemplet.cs", 83);
		}
	}

	public override void Validate()
	{
	}
}
