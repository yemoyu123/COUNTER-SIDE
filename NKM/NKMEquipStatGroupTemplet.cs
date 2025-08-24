using System.Collections.Generic;
using System.Linq;
using Cs.Math;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMEquipStatGroupTemplet
{
	private readonly Dictionary<NKM_STAT_TYPE, NKMEquipRandomStatTemplet> stats;

	private readonly IReadOnlyList<NKMEquipRandomStatTemplet> list;

	public int GroupId { get; }

	public int Count => stats.Count;

	public IReadOnlyList<NKMEquipRandomStatTemplet> Values => list;

	public NKMEquipStatGroupTemplet(IGrouping<int, NKMEquipRandomStatTemplet> group)
	{
		GroupId = group.Key;
		NKM_STAT_TYPE[] array = (from e in @group
			group e by e.m_StatType into e
			where e.Count() > 1
			select e.Key).ToArray();
		if (array.Any())
		{
			NKMTempletError.Add(string.Format("[EquipStat] duplicated type exist. groupId:{0} stats:{1}", GroupId, string.Join(", ", array)), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEquipRandomStatTemplet.cs", 170);
		}
		stats = group.ToDictionary((NKMEquipRandomStatTemplet e) => e.m_StatType);
		list = group.ToList();
	}

	public bool TryGetValue(NKM_STAT_TYPE statType, out NKMEquipRandomStatTemplet result)
	{
		if (stats.TryGetValue(statType, out result))
		{
			return true;
		}
		NKM_STAT_TYPE factorStat = NKMUnitStatManager.GetFactorStat(statType);
		if (factorStat != NKM_STAT_TYPE.NST_END)
		{
			return stats.TryGetValue(factorStat, out result);
		}
		return false;
	}

	public EQUIP_ITEM_STAT GenerateSubStat(NKM_STAT_TYPE statType, int precision)
	{
		if (!TryGetValue(statType, out var result))
		{
			return null;
		}
		return result.GenerateSubStat(precision);
	}

	public bool PickRandomStat(NKM_STAT_TYPE? exception, out NKMEquipRandomStatTemplet statTemplet)
	{
		IReadOnlyList<NKMEquipRandomStatTemplet> readOnlyList = null;
		readOnlyList = ((!exception.HasValue) ? list : list.Where((NKMEquipRandomStatTemplet e) => e.m_StatType != exception).ToArray());
		if (readOnlyList.Count == 0)
		{
			statTemplet = null;
			return false;
		}
		int index = RandomGenerator.ArrayIndex(readOnlyList.Count);
		statTemplet = readOnlyList[index];
		return true;
	}
}
