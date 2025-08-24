using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet.Recall;

public sealed class NKMRecallUnitGroupTemplet : INKMTemplet
{
	private static Dictionary<int, List<NKMRecallUnitGroupTemplet>> groupDataMap = new Dictionary<int, List<NKMRecallUnitGroupTemplet>>();

	public int Index { get; private set; }

	public int UnitExchangeGroupId { get; private set; }

	public string UnitStrId { get; private set; }

	public int Key => Index;

	public static NKMRecallUnitGroupTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallUnitGroupTemplet.cs", 23))
		{
			return null;
		}
		NKMRecallUnitGroupTemplet nKMRecallUnitGroupTemplet = new NKMRecallUnitGroupTemplet();
		int rValue = 0;
		bool data = cNKMLua.GetData("Index", ref rValue);
		nKMRecallUnitGroupTemplet.Index = rValue;
		int rValue2 = 0;
		bool num = data & cNKMLua.GetData("UnitExchangeGroupID", ref rValue2);
		nKMRecallUnitGroupTemplet.UnitExchangeGroupId = rValue2;
		string rValue3 = string.Empty;
		bool num2 = num & cNKMLua.GetData("UnitStrID", ref rValue3);
		nKMRecallUnitGroupTemplet.UnitStrId = rValue3;
		if (!num2)
		{
			Log.ErrorAndExit("[NKMRecallUnitGroupTemplet] lua loading failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallUnitGroupTemplet.cs", 44);
			return null;
		}
		return nKMRecallUnitGroupTemplet;
	}

	public static IReadOnlyList<NKMRecallUnitGroupTemplet> GetUnitGroupTemplet(int groupId)
	{
		groupDataMap.TryGetValue(groupId, out var value);
		return value;
	}

	public void Join()
	{
		if (!groupDataMap.ContainsKey(UnitExchangeGroupId))
		{
			groupDataMap.Add(UnitExchangeGroupId, new List<NKMRecallUnitGroupTemplet>());
		}
		groupDataMap[UnitExchangeGroupId].Add(this);
	}

	public void Validate()
	{
	}
}
