using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMKillCountTemplet : INKMTemplet
{
	private readonly List<NKMKillCountStepTemplet> userSteps = new List<NKMKillCountStepTemplet>();

	private readonly List<NKMKillCountStepTemplet> serverSteps = new List<NKMKillCountStepTemplet>();

	private readonly List<int> targetStageIds = new List<int>();

	public IEnumerable<NKMKillCountStepTemplet> UserSteps => userSteps;

	public IEnumerable<NKMKillCountStepTemplet> ServerSteps => serverSteps;

	public IEnumerable<int> TargetStageIds => targetStageIds;

	public int Key => EventId;

	public int EventId { get; }

	public string OpenTag { get; private set; }

	public UnlockInfo UnlockInfo { get; private set; }

	public int GetMaxServerStep()
	{
		if (serverSteps.Count <= 0)
		{
			return 0;
		}
		List<int> list = new List<int>();
		foreach (NKMKillCountStepTemplet serverStep in serverSteps)
		{
			list.Add(serverStep.StepId);
		}
		list.Sort();
		return list[list.Count - 1];
	}

	public int GetMaxUserStep()
	{
		if (userSteps.Count <= 0)
		{
			return 0;
		}
		List<int> list = new List<int>();
		foreach (NKMKillCountStepTemplet userStep in userSteps)
		{
			list.Add(userStep.StepId);
		}
		list.Sort();
		return list[list.Count - 1];
	}

	private NKMKillCountTemplet(int eventId)
	{
		EventId = eventId;
	}

	public static void LoadFromLua()
	{
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_EVENT_KILLCOUNT_TEMPLET") || !nKMLua.OpenTable("EVENT_KILLCOUNT_TEMPLET"))
		{
			Log.ErrorAndExit("loading lua file failed. fileName:LUA_EVENT_KILLCOUNT_TEMPLET", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 35);
		}
		int num = 1;
		while (nKMLua.OpenTable(num++))
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 41))
			{
				nKMLua.CloseTable();
				continue;
			}
			int @int = nKMLua.GetInt32("m_EventID");
			if (!NKMTempletContainer<NKMKillCountTemplet>.TryGetValue(@int, out var result))
			{
				result = new NKMKillCountTemplet(@int);
				NKMTempletContainer<NKMKillCountTemplet>.Add(result, null);
				result.OpenTag = nKMLua.GetString("m_OpenTag");
				nKMLua.GetData("m_TargetStage", result.targetStageIds);
				result.UnlockInfo = UnlockInfo.LoadFromLua(nKMLua);
			}
			NKMKillCountStepTemplet nKMKillCountStepTemplet = new NKMKillCountStepTemplet();
			nKMKillCountStepTemplet.Load(nKMLua);
			if (nKMKillCountStepTemplet.IsUserStep)
			{
				result.userSteps.Add(nKMKillCountStepTemplet);
			}
			else
			{
				result.serverSteps.Add(nKMKillCountStepTemplet);
			}
			nKMLua.CloseTable();
		}
	}

	public static NKMKillCountTemplet Find(int key)
	{
		return NKMTempletContainer<NKMKillCountTemplet>.Find(key);
	}

	public void Join()
	{
		foreach (int targetStageId in targetStageIds)
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(targetStageId);
			if (nKMStageTempletV == null)
			{
				NKMTempletError.Add($"[KillCount] 스테이지 id가 유효하지 않음 eventId:{EventId} stageId:{targetStageId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 84);
			}
			else if (nKMStageTempletV.KillCountTemplet != null)
			{
				NKMTempletError.Add($"[KillCount] 스테이지당 하나의 killCount만 연결 가능. stageId:{targetStageId} eventId-1:{EventId} eventId-2:{nKMStageTempletV.KillCountTemplet.EventId} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 90);
			}
			else
			{
				nKMStageTempletV.KillCountTemplet = this;
			}
		}
	}

	public void Validate()
	{
		if (string.IsNullOrEmpty(OpenTag))
		{
			NKMTempletError.Add($"[KillCount] OpenTag가 올바르지 않음. eventId:{EventId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 102);
		}
		if (!targetStageIds.Any())
		{
			NKMTempletError.Add($"[KillCount] 스테이지 정보가 없음. eventId:{EventId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 107);
		}
		if (!userSteps.Any())
		{
			NKMTempletError.Add($"[KillCount] 개인보상 정보가 없음. eventId:{EventId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 112);
		}
		userSteps.Sort();
		serverSteps.Sort();
		for (int i = 0; i < userSteps.Count; i++)
		{
			if (userSteps[i].StepId != i + 1)
			{
				NKMTempletError.Add($"[KillCount] user stepId 정의 오류. index:{i} stepId:{userSteps[i].StepId} eventId:{EventId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 123);
			}
		}
		for (int j = 0; j < serverSteps.Count; j++)
		{
			if (serverSteps[j].StepId != j + 1)
			{
				NKMTempletError.Add($"[KillCount] server stepId 정의 오류. index:{j} stepId:{serverSteps[j].StepId} eventId:{EventId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 131);
			}
		}
		foreach (NKMKillCountStepTemplet userStep in userSteps)
		{
			userStep.Validate(this);
		}
		if (!serverSteps.Any())
		{
			NKMTempletError.Add($"[KillCount] 서버보상 정보가 없음. eventId:{EventId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMKillCountTemplet.cs", 142);
		}
		foreach (NKMKillCountStepTemplet serverStep in serverSteps)
		{
			serverStep.Validate(this);
		}
	}

	public bool TryGetUserStep(int stepId, out NKMKillCountStepTemplet result)
	{
		result = userSteps.FirstOrDefault((NKMKillCountStepTemplet e) => e.StepId == stepId);
		return result != null;
	}

	public bool TryGetServerStep(int stepId, out NKMKillCountStepTemplet result)
	{
		result = ServerSteps.FirstOrDefault((NKMKillCountStepTemplet e) => e.StepId == stepId);
		return result != null;
	}
}
