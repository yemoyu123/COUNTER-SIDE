using NKM.Templet.Base;

namespace NKM.Guild;

public sealed class GuildExpTemplet : INKMTemplet
{
	public int GuildLevel { get; private set; }

	public long GuildExpRequired { get; private set; }

	public long GuildExpCumulated { get; private set; }

	public int MaxMemberCount { get; private set; }

	public int WelfarePoint { get; private set; }

	public int Key => GuildLevel;

	public static GuildExpTemplet LoadFromLua(NKMLua lua)
	{
		return new GuildExpTemplet
		{
			GuildLevel = lua.GetInt32("m_GuildLv"),
			GuildExpRequired = lua.GetInt64("m_GuildExpRequired"),
			GuildExpCumulated = lua.GetInt64("m_GuildExpCumulated"),
			MaxMemberCount = lua.GetInt32("m_GuildLvPersonCapacity"),
			WelfarePoint = lua.GetInt32("m_GuildLvWelfarePoint")
		};
	}

	public static GuildExpTemplet Find(int guildLevel)
	{
		return NKMTempletContainer<GuildExpTemplet>.Find(guildLevel);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (GuildLevel <= 0)
		{
			NKMTempletError.Add($"[Guild] invalid level:{GuildLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildExpTemplet.cs", 38);
		}
		if (GuildExpRequired <= 0)
		{
			NKMTempletError.Add($"[Guild] invalid GuildExpRequired:{GuildExpRequired}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildExpTemplet.cs", 43);
		}
		if (GuildExpCumulated <= 0)
		{
			NKMTempletError.Add($"[Guild] invalid GuildExpRequired:{GuildExpCumulated}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildExpTemplet.cs", 48);
		}
		if (MaxMemberCount <= 0)
		{
			NKMTempletError.Add($"[Guild] invalid GuildExpRequired:{MaxMemberCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildExpTemplet.cs", 53);
		}
		if (WelfarePoint <= 0)
		{
			NKMTempletError.Add($"[Guild] invalid GuildExpRequired:{WelfarePoint}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildExpTemplet.cs", 58);
		}
		if (GuildExpRequired > GuildExpCumulated)
		{
			NKMTempletError.Add($"[Guild] GuildExpRequired({GuildExpRequired}) > GuildExpCumulated:{GuildExpCumulated}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildExpTemplet.cs", 63);
		}
	}
}
