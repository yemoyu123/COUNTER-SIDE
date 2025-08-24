using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMUnitMissionTemplet : INKMTemplet
{
	private readonly List<NKMUnitMissionStepTemplet> steps = new List<NKMUnitMissionStepTemplet>();

	private static string OpenTag = "TAG_COLLECTION_MISSION";

	public static bool EnableByTag = NKMOpenTagManager.IsOpened(OpenTag);

	public int MissionId { get; }

	public NKM_MISSION_COND MissionCondition { get; private set; }

	public NKM_UNIT_GRADE UnitGrade { get; private set; }

	public int Key => MissionId;

	public IReadOnlyList<NKMUnitMissionStepTemplet> Steps => steps;

	private NKMUnitMissionTemplet(int groupId)
	{
		MissionId = groupId;
	}

	public static void LoadFromLua()
	{
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_UNIT_MISSION_TEMPLET") || !nKMLua.OpenTable("UNIT_MISSION_TEMPLET"))
		{
			Log.ErrorAndExit("loading lua file failed. fileName:LUA_UNIT_MISSION_TEMPLET", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitMissionTemplet.cs", 36);
		}
		int num = 1;
		while (nKMLua.OpenTable(num++))
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitMissionTemplet.cs", 42))
			{
				nKMLua.CloseTable();
				continue;
			}
			int @int = nKMLua.GetInt32("MissionID");
			if (!NKMTempletContainer<NKMUnitMissionTemplet>.TryGetValue(@int, out var result))
			{
				result = new NKMUnitMissionTemplet(@int)
				{
					UnitGrade = nKMLua.GetEnum<NKM_UNIT_GRADE>("Unit_Grade"),
					MissionCondition = nKMLua.GetEnum<NKM_MISSION_COND>("Mission_Condition")
				};
				NKMTempletContainer<NKMUnitMissionTemplet>.Add(result, null);
			}
			NKMUnitMissionStepTemplet nKMUnitMissionStepTemplet = new NKMUnitMissionStepTemplet(result);
			nKMUnitMissionStepTemplet.Load(nKMLua);
			result.steps.Add(nKMUnitMissionStepTemplet);
			nKMLua.CloseTable();
		}
	}

	public static NKMUnitMissionTemplet Find(int missionId)
	{
		return NKMTempletContainer<NKMUnitMissionTemplet>.Find(missionId);
	}

	public void Join()
	{
		steps.Sort();
		for (int i = 0; i < steps.Count; i++)
		{
			steps[i].StepIndex = i;
		}
	}

	public void Validate()
	{
		if (!steps.Any())
		{
			NKMTempletError.Add($"[UnitMission] 미션 그룹 내에 미션이 없음. groupId:{MissionId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitMissionTemplet.cs", 84);
		}
		int[] array = (from e in steps
			group e by e.StepId into e
			where e.Count() > 1
			select e.Key).ToArray();
		if (array.Any())
		{
			string arg = string.Join(", ", array);
			NKMTempletError.Add($"[UnitMission] 미션 그룹 내에 중복된 미션이 있음. groupId:{MissionId} missionId:{arg}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitMissionTemplet.cs", 95);
		}
		foreach (NKMUnitMissionStepTemplet step in steps)
		{
			step.Validate(this);
		}
	}

	public static void CheckValidate()
	{
		if (NKMCollectionTeamUpGroupTemplet.EnableByTag && EnableByTag)
		{
			NKMTempletError.Add("[UnitMission] 팀업 태그와 유닛미션 태그가 둘다 켜졌습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitMissionTemplet.cs", 108);
		}
	}

	public bool HasMission(int missionId)
	{
		return steps.Any((NKMUnitMissionStepTemplet e) => e.StepId == missionId);
	}
}
