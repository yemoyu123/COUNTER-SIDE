using System.Collections.Generic;
using Cs.Logging;
using NKM.Contract2;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Guild;

public sealed class GuildAttendanceTemplet
{
	public readonly struct AdditionalReward
	{
		public int AttendanceCount { get; }

		public RewardUnit Item { get; }

		public AdditionalReward(int attendanceCount, RewardUnit item)
		{
			AttendanceCount = attendanceCount;
			Item = item;
		}

		public override string ToString()
		{
			return $"attendanceCount:{AttendanceCount} reward:{Item}";
		}
	}

	private enum RewardConditionType
	{
		ATEENDANCE_GUILD_MEMBER_CNT,
		ATTENDANCE_GUILD_GENERAL
	}

	private const string FileName = "LUA_GUILD_ATTENDANCE_TEMPLET";

	private const string TableName = "GUILD_ATTENDANCE_TEMPLET";

	private readonly List<RewardUnit> basicRewards = new List<RewardUnit>();

	private readonly List<AdditionalReward> additionalRewards = new List<AdditionalReward>();

	public static GuildAttendanceTemplet Instance { get; private set; }

	public IReadOnlyList<RewardUnit> BasicRewards => basicRewards;

	public IReadOnlyList<AdditionalReward> AdditionalRewards => additionalRewards;

	public static void LoadFromLua()
	{
		Instance = new GuildAttendanceTemplet();
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_GUILD_ATTENDANCE_TEMPLET") || !nKMLua.OpenTable("GUILD_ATTENDANCE_TEMPLET"))
		{
			Log.ErrorAndExit("loading lua file failed. fileName:LUA_GUILD_ATTENDANCE_TEMPLET", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildAttendanceTemplet.cs", 49);
		}
		int num = 1;
		while (nKMLua.OpenTable(num))
		{
			RewardConditionType rewardConditionType = nKMLua.GetEnum<RewardConditionType>("m_RewardCond");
			RewardUnit item = new RewardUnit(nKMLua.GetEnum<NKM_REWARD_TYPE>("m_RewardType"), nKMLua.GetInt32("m_RewardID"), nKMLua.GetInt32("m_RewardValue"));
			switch (rewardConditionType)
			{
			case RewardConditionType.ATTENDANCE_GUILD_GENERAL:
				Instance.basicRewards.Add(item);
				break;
			case RewardConditionType.ATEENDANCE_GUILD_MEMBER_CNT:
			{
				int @int = nKMLua.GetInt32("m_RewardCondValue");
				Instance.additionalRewards.Add(new AdditionalReward(@int, item));
				break;
			}
			}
			num++;
			nKMLua.CloseTable();
		}
		Instance.additionalRewards.Sort((AdditionalReward a, AdditionalReward b) => a.AttendanceCount.CompareTo(b.AttendanceCount));
	}

	public static void Validate()
	{
		foreach (RewardUnit basicReward in Instance.basicRewards)
		{
			basicReward.Validate("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildAttendanceTemplet.cs", 87);
		}
		foreach (AdditionalReward additionalReward in Instance.additionalRewards)
		{
			if (additionalReward.AttendanceCount <= 0)
			{
				NKMTempletError.Add($"[GuildAttendance] invalid AttendanceCount:{additionalReward.AttendanceCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildAttendanceTemplet.cs", 94);
			}
			additionalReward.Item.Validate("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildAttendanceTemplet.cs", 97);
		}
	}

	public IEnumerable<RewardUnit> GetRewards(int yestardayAttendanceCount)
	{
		foreach (RewardUnit basicReward in BasicRewards)
		{
			yield return basicReward;
		}
		RewardUnit rewardUnit = null;
		for (int i = 0; i < additionalRewards.Count; i++)
		{
			AdditionalReward additionalReward = additionalRewards[i];
			if (additionalReward.AttendanceCount > yestardayAttendanceCount)
			{
				break;
			}
			rewardUnit = additionalReward.Item;
		}
		if (rewardUnit != null)
		{
			yield return rewardUnit;
		}
	}
}
