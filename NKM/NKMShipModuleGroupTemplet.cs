using System.Collections.Generic;
using System.Linq;
using Cs.Math.Lottery;
using NKM.Templet.Base;

namespace NKM;

public class NKMShipModuleGroupTemplet
{
	private static Dictionary<int, List<NKMCommandModulePassiveTemplet>> commandModulePassiveTemplets = new Dictionary<int, List<NKMCommandModulePassiveTemplet>>();

	private static Dictionary<int, List<NKMCommandModuleRandomStatTemplet>> commandModuleStatTemplets = new Dictionary<int, List<NKMCommandModuleRandomStatTemplet>>();

	private static Dictionary<int, RatioLottery<NKMCommandModulePassiveTemplet>> commandModuleRatios = new Dictionary<int, RatioLottery<NKMCommandModulePassiveTemplet>>();

	public static void LoadFromLua()
	{
		commandModulePassiveTemplets = (from e in NKMTempletLoader<NKMCommandModulePassiveTemplet>.LoadGroup("AB_SCRIPT", "LUA_COMMANDMODULE_PASSIVE_TEMPLET", "COMMANDMODULE_PASSIVE_TEMPLET", NKMCommandModulePassiveTemplet.LoadFromLUA).SelectMany((KeyValuePair<int, List<NKMCommandModulePassiveTemplet>> e) => e.Value).ToList()
			group e by e.PassiveGroupId).ToDictionary((IGrouping<int, NKMCommandModulePassiveTemplet> e) => e.Key, (IGrouping<int, NKMCommandModulePassiveTemplet> e) => e.ToList());
		commandModuleStatTemplets = (from e in NKMTempletLoader<NKMCommandModuleRandomStatTemplet>.LoadGroup("AB_SCRIPT", "LUA_COMMANDMODULE_RANDOM_STAT", "COMMANDMODULE_RANDOM_STAT", NKMCommandModuleRandomStatTemplet.LoadFromLUA).SelectMany((KeyValuePair<int, List<NKMCommandModuleRandomStatTemplet>> e) => e.Value).ToList()
			group e by e.Key).ToDictionary((IGrouping<int, NKMCommandModuleRandomStatTemplet> e) => e.Key, (IGrouping<int, NKMCommandModuleRandomStatTemplet> e) => e.ToList());
		if (commandModulePassiveTemplets == null || commandModuleStatTemplets == null)
		{
			NKMTempletError.Add("[NKMShipModuleGroupTemplet] \ufffdԼ\ufffd \ufffd\u033d\ufffd \ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd load \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 876);
		}
	}

	public static void Join()
	{
		foreach (NKMCommandModuleRandomStatTemplet item in commandModuleStatTemplets.Values.SelectMany((List<NKMCommandModuleRandomStatTemplet> e) => e).Distinct())
		{
			item.Join();
		}
		foreach (NKMCommandModulePassiveTemplet item2 in commandModulePassiveTemplets.Values.SelectMany((List<NKMCommandModulePassiveTemplet> e) => e).Distinct())
		{
			if (!commandModuleRatios.TryGetValue(item2.PassiveGroupId, out var value))
			{
				RatioLottery<NKMCommandModulePassiveTemplet> ratioLottery = new RatioLottery<NKMCommandModulePassiveTemplet>();
				ratioLottery.AddCase(item2.Ratio, item2);
				commandModuleRatios.Add(item2.PassiveGroupId, ratioLottery);
			}
			else
			{
				value.AddCase(item2.Ratio, item2);
			}
		}
	}

	public static void Validate()
	{
		foreach (NKMCommandModulePassiveTemplet item in commandModulePassiveTemplets.Values.SelectMany((List<NKMCommandModulePassiveTemplet> e) => e).Distinct())
		{
			item.Validate();
		}
		foreach (NKMCommandModuleRandomStatTemplet item2 in commandModuleStatTemplets.Values.SelectMany((List<NKMCommandModuleRandomStatTemplet> e) => e).Distinct())
		{
			item2.Validate();
		}
	}

	public static IReadOnlyList<NKMCommandModulePassiveTemplet> GetPassiveListsByGroupId(int passiveGroupId)
	{
		if (!commandModulePassiveTemplets.TryGetValue(passiveGroupId, out var value))
		{
			return null;
		}
		return value;
	}

	public static IReadOnlyList<NKMCommandModuleRandomStatTemplet> GetStatListsByGroupId(int statGroupId)
	{
		if (!commandModuleStatTemplets.TryGetValue(statGroupId, out var value))
		{
			return null;
		}
		return value;
	}

	public static NKMCommandModulePassiveTemplet Decide(int passiveGroupId)
	{
		if (!commandModuleRatios.TryGetValue(passiveGroupId, out var value))
		{
			return null;
		}
		return value.Decide();
	}
}
