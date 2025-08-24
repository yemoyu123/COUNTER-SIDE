using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM;

public class NKMEquipTuningManager
{
	private static Dictionary<int, NKMEquipStatGroupTemplet> equipRandomStatGroups;

	public const int MAX_PRECISION = 100;

	public const int CREDIT_COST_REFINE = 150;

	public const int TUNING_MATERIAL_COST_REFINE = 1;

	public const int CREDIT_COST_STAT_CHANGE = 750;

	public const int TUNING_MATERIAL_COST_STAT_CHANGE = 1;

	public static void LoadFromLUA_EquipRandomStat(string fileName)
	{
		equipRandomStatGroups = (from e in NKMTempletLoader.LoadCommonPath("AB_SCRIPT", fileName, "ITEM_EQUIP_RANDOM_STAT", NKMEquipRandomStatTemplet.LoadFromLUA)
			group e by e.m_StatGroupID).ToDictionary((IGrouping<int, NKMEquipRandomStatTemplet> e) => e.Key, (IGrouping<int, NKMEquipRandomStatTemplet> e) => new NKMEquipStatGroupTemplet(e));
	}

	public static void Validate()
	{
		foreach (NKMEquipRandomStatTemplet item in equipRandomStatGroups.Values.SelectMany((NKMEquipStatGroupTemplet e) => e.Values))
		{
			item.Validate();
		}
	}

	public static bool TryGetStatGroupTemplet(int groupId, out NKMEquipStatGroupTemplet result)
	{
		return equipRandomStatGroups.TryGetValue(groupId, out result);
	}

	public static IReadOnlyList<NKMEquipRandomStatTemplet> GetEquipRandomStatGroupList(int statGroupID)
	{
		equipRandomStatGroups.TryGetValue(statGroupID, out var value);
		return value?.Values ?? null;
	}

	public static bool IsChangeableStatGroup(int statGroupID)
	{
		if (equipRandomStatGroups.TryGetValue(statGroupID, out var value))
		{
			return value.Count > 1;
		}
		return false;
	}

	public static NKMEquipRandomStatTemplet GetEquipRandomStat(int statGroupID, NKM_STAT_TYPE statType)
	{
		if (!equipRandomStatGroups.TryGetValue(statGroupID, out var value))
		{
			return null;
		}
		value.TryGetValue(statType, out var result);
		return result;
	}
}
