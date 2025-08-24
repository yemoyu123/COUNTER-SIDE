using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet.Office;

[DebuggerDisplay("{Grade} score:{ScoreMax}")]
public sealed class NKMOfficeGradeTemplet
{
	private static readonly List<NKMOfficeGradeTemplet> Templets = new List<NKMOfficeGradeTemplet>();

	public OfficeGrade Grade { get; private set; }

	public string GradeLiteral { get; private set; }

	public int ScoreMax { get; private set; }

	public TimeSpan ChargingTime { get; private set; }

	public int ChargingTimeHour => (int)ChargingTime.TotalHours;

	public int PartyRewardLoyalty { get; private set; }

	public NKM_REWARD_TYPE PartyRewardType { get; private set; }

	public int PartyRewardId { get; private set; }

	public int PartyRewardValueMin { get; private set; }

	public int PartyRewardValueMax { get; private set; }

	public static NKMOfficeGradeTemplet Find(OfficeGrade grade)
	{
		return Templets.Find((NKMOfficeGradeTemplet e) => e.Grade == grade);
	}

	public static NKMOfficeGradeTemplet Find(int score)
	{
		return Templets.FirstOrDefault((NKMOfficeGradeTemplet e) => score <= e.ScoreMax) ?? Templets.Last();
	}

	public static void LoadFromLua()
	{
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_OFFICE_GRADE_TEMPLET") || !nKMLua.OpenTable("m_OfficeGrade"))
			{
				Log.ErrorAndExit("loading lua file failed. fileName:LUA_OFFICE_GRADE_TEMPLET", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeGradeTemplet.cs", 54);
			}
			int num = 1;
			while (nKMLua.OpenTable(num++))
			{
				if (!NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeGradeTemplet.cs", 60))
				{
					nKMLua.CloseTable();
					continue;
				}
				NKMOfficeGradeTemplet nKMOfficeGradeTemplet = new NKMOfficeGradeTemplet();
				if (!nKMOfficeGradeTemplet.Load(nKMLua))
				{
					nKMLua.CloseTable();
					continue;
				}
				Templets.Add(nKMOfficeGradeTemplet);
				nKMLua.CloseTable();
			}
		}
		Templets.Sort((NKMOfficeGradeTemplet a, NKMOfficeGradeTemplet b) => a.ScoreMax.CompareTo(b.ScoreMax));
		if ((from e in Templets
			group e by e.Grade).Any((IGrouping<OfficeGrade, NKMOfficeGradeTemplet> e) => e.Count() > 1))
		{
			string text = string.Join(", ", Templets.Select((NKMOfficeGradeTemplet e) => e.GradeLiteral));
			NKMTempletError.Add("[OfficeGrade] OfficeGrade duplicated. list:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeGradeTemplet.cs", 85);
		}
	}

	public static void Drop()
	{
		Templets.Clear();
	}

	public static void Validate()
	{
		foreach (NKMOfficeGradeTemplet templet in Templets)
		{
			templet.ValidateServerOnly();
		}
	}

	private bool Load(NKMLua lua)
	{
		GradeLiteral = lua.GetString("OfficeGrade");
		ScoreMax = lua.GetInt32("ScoreMax");
		ChargingTime = TimeSpan.FromHours(lua.GetInt32("ChargingTime"));
		PartyRewardLoyalty = lua.GetInt32("PartyRewardLoyalty");
		PartyRewardType = lua.GetEnum<NKM_REWARD_TYPE>("PartyRewardType");
		PartyRewardId = lua.GetInt32("PartyRewardId");
		PartyRewardValueMin = lua.GetInt32("PartyRewardValue_Min");
		PartyRewardValueMax = lua.GetInt32("PartyRewardValue_Max");
		if (!Enum.TryParse<OfficeGrade>("Grade" + GradeLiteral, out var result))
		{
			NKMTempletError.Add("[OfficeGrade] invalid OfficeGrade:" + GradeLiteral, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeGradeTemplet.cs", 115);
			return false;
		}
		Grade = result;
		return true;
	}

	private void ValidateServerOnly()
	{
		if (!NKMRewardTemplet.IsValidReward(PartyRewardType, PartyRewardId))
		{
			NKMTempletError.Add($"[OfficeGrade] 회식 보상 값이 올바르지 않음. grade:{Grade} PartyRewardType:{PartyRewardType} PartyRewardId:{PartyRewardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeGradeTemplet.cs", 128);
		}
		if (PartyRewardValueMax < PartyRewardValueMin)
		{
			NKMTempletError.Add($"[OfficeGrade] 회식 보상 최소~최대 수치 오류. grade:{Grade} min:{PartyRewardValueMin} max:{PartyRewardValueMax}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeGradeTemplet.cs", 134);
		}
		if (PartyRewardLoyalty <= 0)
		{
			NKMTempletError.Add($"[OfficeGrade] 회식 보상 애사심 수치 오류. grade:{Grade} PartyRewardLoyalty:{PartyRewardLoyalty}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOfficeGradeTemplet.cs", 139);
		}
	}
}
