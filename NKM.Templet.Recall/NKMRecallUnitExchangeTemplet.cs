using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet.Recall;

public sealed class NKMRecallUnitExchangeTemplet : INKMTemplet
{
	private static Dictionary<int, List<NKMRecallUnitExchangeTemplet>> dataMap = new Dictionary<int, List<NKMRecallUnitExchangeTemplet>>();

	public int Index { get; private set; }

	public int UnitExchangeGroupId { get; private set; }

	public int UnitId { get; private set; }

	public NKM_UNIT_TYPE UnitType { get; private set; }

	public int Key => Index;

	public static NKMRecallUnitExchangeTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallUnitExchangeTemplet.cs", 19))
		{
			return null;
		}
		return new NKMRecallUnitExchangeTemplet
		{
			Index = lua.GetInt32("Index"),
			UnitExchangeGroupId = lua.GetInt32("UnitExchangeGroupID"),
			UnitId = lua.GetInt32("UnitID"),
			UnitType = lua.GetEnum<NKM_UNIT_TYPE>("m_NKM_UNIT_TYPE")
		};
	}

	public static IReadOnlyList<NKMRecallUnitExchangeTemplet> GetUnitGroupTemplet(int groupId)
	{
		dataMap.TryGetValue(groupId, out var value);
		return value;
	}

	public void Join()
	{
		if (!dataMap.ContainsKey(UnitExchangeGroupId))
		{
			dataMap.Add(UnitExchangeGroupId, new List<NKMRecallUnitExchangeTemplet>());
		}
		dataMap[UnitExchangeGroupId].Add(this);
	}

	public void Validate()
	{
		NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(UnitId);
		if (unitTemplet == null)
		{
			NKMTempletError.Add($"[NKMRecallUnitExchangeTemplet] unit id가 유효하지 않습니다. unit id: {UnitId}, index: {Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallUnitExchangeTemplet.cs", 57);
		}
		else if (unitTemplet.m_UnitTempletBase.m_NKM_UNIT_TYPE != UnitType)
		{
			NKMTempletError.Add($"[NKMRecallUnitExchangeTemplet] 입력한 unit type 값과 실제 unit type 값이 다릅니다. unitId:{UnitId}, index:{Index} unitType:{UnitType} templetUnitType:{unitTemplet.m_UnitTempletBase.m_NKM_UNIT_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallUnitExchangeTemplet.cs", 63);
		}
	}

	public static void Drop()
	{
		dataMap.Clear();
	}
}
