using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using Cs.Math.Lottery;
using NKM.Templet.Base;

namespace NKM.Contract2;

public sealed class SelectableUnitPoolTemplet : INKMTemplet
{
	public const int SlotCount = 10;

	public readonly RatioLottery<SelectableUnitTemplet>[] lotteries = new RatioLottery<SelectableUnitTemplet>[10];

	public int Key { get; }

	public string StringId { get; }

	public IReadOnlyList<IReadonlyLottery<SelectableUnitTemplet>> Lotteries => lotteries;

	public SelectableUnitPoolTemplet(int key, string stringId)
	{
		Key = key;
		StringId = stringId;
		for (int i = 0; i < lotteries.Length; i++)
		{
			lotteries[i] = new RatioLottery<SelectableUnitTemplet>();
		}
	}

	public static void LoadFile()
	{
		string bundleName = "AB_SCRIPT";
		string text = "LUA_SELECTABLE_CONTRACT_UNIT_POOL";
		string text2 = "SELECTABLE_UNIT_POOL";
		Dictionary<int, SelectableUnitPoolTemplet> dictionary = new Dictionary<int, SelectableUnitPoolTemplet>();
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath(bundleName, text) || !nKMLua.OpenTable(text2))
			{
				Log.ErrorAndExit("[SelectableUnitPool] loading file failed. fileName:" + text + " tablName:" + text2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableUnitPoolTemplet.cs", 43);
			}
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				if (NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableUnitPoolTemplet.cs", 49))
				{
					if (!nKMLua.GetData("m_UnitPoolId", out var rValue, 0))
					{
						Log.ErrorAndExit($"[SelectableUnitPool] loading key failed. id:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableUnitPoolTemplet.cs", 53);
					}
					if (!dictionary.TryGetValue(rValue, out var value))
					{
						if (!nKMLua.GetData("m_UnitPoolStrId", out var rValue2, null))
						{
							Log.ErrorAndExit($"[SelectableUnitPool] loading key failed. id:{rValue} strId:{rValue2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableUnitPoolTemplet.cs", 60);
						}
						value = new SelectableUnitPoolTemplet(rValue, rValue2);
						dictionary.Add(rValue, value);
					}
					value.LoadFromLua(nKMLua);
				}
				num++;
				nKMLua.CloseTable();
			}
		}
		NKMTempletContainer<SelectableUnitPoolTemplet>.Load(dictionary.Values, (SelectableUnitPoolTemplet e) => e.StringId);
	}

	public void LoadFromLua(NKMLua lua)
	{
		if ((1u & (lua.GetData("m_SlotNumber", out var rValue, -1) ? 1u : 0u) & (lua.GetData("m_UnitStrId", out var rValue2, null) ? 1u : 0u) & (lua.GetData("m_Ratio", out var rValue3, 0) ? 1u : 0u)) == 0)
		{
			Log.ErrorAndExit($"[SelectableUnitPool] loading failed. id:{Key} strId:{StringId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableUnitPoolTemplet.cs", 87);
		}
		SelectableUnitTemplet value = new SelectableUnitTemplet(rValue2, rValue3);
		int num = rValue - 1;
		lotteries[num].AddCase(rValue3, value);
	}

	public void Join()
	{
		foreach (SelectableUnitTemplet item in lotteries.SelectMany((RatioLottery<SelectableUnitTemplet> e) => e.CaseValues))
		{
			item.Join();
		}
		for (int num = 0; num < lotteries.Length; num++)
		{
			RatioLottery<SelectableUnitTemplet> obj = lotteries[num];
			int num2 = num + 1;
			if ((from e in obj.CaseValues
				group e by e.UnitStringId).Any((IGrouping<string, SelectableUnitTemplet> e) => e.Count() > 1))
			{
				NKMTempletError.Add($"[{Key}]{StringId} 슬롯 내에 중복된 유닛이 존재. slotNumber:{num2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableUnitPoolTemplet.cs", 109);
			}
			if (obj.Count == 0)
			{
				NKMTempletError.Add($"[{Key}]{StringId} 비어있는 슬롯 존재. slotNumber:{num2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableUnitPoolTemplet.cs", 114);
			}
		}
	}

	public void Validate()
	{
		foreach (SelectableUnitTemplet item in lotteries.SelectMany((RatioLottery<SelectableUnitTemplet> e) => e.CaseValues))
		{
			item.Validate();
		}
	}
}
