using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Guild;

public static class GuildLevelTemplet
{
	private static GuildExpTemplet[] ExpTemplets;

	public static long MaxExpCumulated { get; private set; }

	public static int MaxLevel { get; private set; }

	public static void Join()
	{
		ExpTemplets = new GuildExpTemplet[NKMTempletContainer<GuildExpTemplet>.Values.Count()];
		foreach (GuildExpTemplet value in NKMTempletContainer<GuildExpTemplet>.Values)
		{
			if (value.GuildLevel > ExpTemplets.Length)
			{
				NKMTempletError.Add($"[GuildLevel] exp templet has invalid level data:{value.GuildLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildLevelTemplet.cs", 21);
				continue;
			}
			int num = value.GuildLevel - 1;
			ExpTemplets[num] = value;
		}
		GuildExpTemplet obj = ExpTemplets[ExpTemplets.Length - 1];
		MaxLevel = obj.GuildLevel;
		MaxExpCumulated = obj.GuildExpCumulated;
	}

	public static void Validate()
	{
		for (int i = 1; i < ExpTemplets.Length; i++)
		{
			int num = i + 1;
			GuildExpTemplet guildExpTemplet = ExpTemplets[i - 1];
			GuildExpTemplet guildExpTemplet2 = ExpTemplets[i];
			if (guildExpTemplet.GuildExpRequired >= guildExpTemplet2.GuildExpRequired)
			{
				NKMTempletError.Add($"[GuildLevel:{num}] prevLevel.GuildExpRequired({guildExpTemplet.GuildExpRequired}) >= currentLevel.GuildExpRequired:{guildExpTemplet2.GuildExpRequired}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildLevelTemplet.cs", 44);
			}
			if (guildExpTemplet.GuildExpCumulated >= guildExpTemplet2.GuildExpCumulated)
			{
				NKMTempletError.Add($"[GuildLevel:{num}] prevLevel.GuildExpCumulated({guildExpTemplet.GuildExpCumulated}) >= currentLevel.GuildExpCumulated:{guildExpTemplet2.GuildExpCumulated}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildLevelTemplet.cs", 49);
			}
			if (guildExpTemplet.MaxMemberCount > guildExpTemplet2.MaxMemberCount)
			{
				NKMTempletError.Add($"[GuildLevel:{num}] prevLevel.MaxMemberCount({guildExpTemplet.MaxMemberCount}) >= currentLevel.MaxMemberCount:{guildExpTemplet2.MaxMemberCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildLevelTemplet.cs", 54);
			}
			if (guildExpTemplet.WelfarePoint > guildExpTemplet2.WelfarePoint)
			{
				NKMTempletError.Add($"[GuildLevel:{num}] prevLevel.WelfarePoint({guildExpTemplet.WelfarePoint}) >= currentLevel.WelfarePoint:{guildExpTemplet2.WelfarePoint}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildLevelTemplet.cs", 59);
			}
			long num2 = guildExpTemplet.GuildExpCumulated + guildExpTemplet2.GuildExpRequired;
			if (num2 != guildExpTemplet2.GuildExpCumulated)
			{
				NKMTempletError.Add($"[GuildLevel:{num}] calculated({num2}) != currentLevel.GuildExpCumulated:{guildExpTemplet2.GuildExpCumulated}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildLevelTemplet.cs", 65);
			}
		}
	}

	public static (int level, long levelExp) CalculateCurrentLevel(long totalExp)
	{
		if (totalExp >= MaxExpCumulated)
		{
			return (level: MaxLevel, levelExp: 0L);
		}
		for (int i = 0; i < ExpTemplets.Length; i++)
		{
			GuildExpTemplet guildExpTemplet = ExpTemplets[i];
			if (totalExp < guildExpTemplet.GuildExpCumulated)
			{
				int guildLevel = guildExpTemplet.GuildLevel;
				long num = totalExp;
				if (i > 0)
				{
					GuildExpTemplet guildExpTemplet2 = ExpTemplets[i - 1];
					num -= guildExpTemplet2.GuildExpCumulated;
				}
				return (level: guildLevel, levelExp: num);
			}
		}
		Log.Error($"[GuildLevel] cannot find exp templet. totalExp:{totalExp} #expTemplets:{ExpTemplets.Length}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildLevelTemplet.cs", 97);
		return (level: 1, levelExp: 0L);
	}
}
