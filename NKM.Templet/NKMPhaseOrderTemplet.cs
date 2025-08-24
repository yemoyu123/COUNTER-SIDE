using System;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMPhaseOrderTemplet
{
	private static readonly TimeSpan MaxPhaseTime = TimeSpan.FromMinutes(5.0);

	private string dungeonStringId;

	public int PhaseGroupId { get; private set; }

	public int PhaseOrder { get; private set; }

	public int PhaseTimeSec { get; private set; }

	public NKMDungeonTempletBase Dungeon { get; private set; }

	public static NKMPhaseOrderTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMPhaseOrderTemplet.cs", 21))
		{
			return null;
		}
		return new NKMPhaseOrderTemplet
		{
			PhaseGroupId = lua.GetInt32("m_PhaseGroupID"),
			PhaseOrder = lua.GetInt32("m_PhaseOrder"),
			PhaseTimeSec = lua.GetInt32("m_PhaseTime"),
			dungeonStringId = lua.GetString("m_DungeonStrID")
		};
	}

	public void Join(NKMPhaseGroupTemplet groupTemplet)
	{
		Dungeon = NKMDungeonManager.GetDungeonTempletBase(dungeonStringId);
		if (Dungeon == null)
		{
			NKMTempletError.Add($"[PhaseOrder] invalid dungeonId:{dungeonStringId} phaseGroup:{PhaseGroupId} phaseOrder:{PhaseOrder}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMPhaseOrderTemplet.cs", 38);
		}
		else
		{
			Dungeon.AddReference(groupTemplet);
		}
	}

	public void Validate()
	{
		if (PhaseTimeSec <= 0 || (double)PhaseTimeSec > MaxPhaseTime.TotalSeconds)
		{
			NKMTempletError.Add($"[PhaseOrder] invalid phase time:{PhaseTimeSec}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMPhaseOrderTemplet.cs", 50);
		}
		if (Dungeon != null && Dungeon.StageTemplet != null)
		{
			NKMTempletError.Add($"[PhaseOrder] 페이즈 던전은 스테이지에 직접 연결할 수 없음. dungeonId:{Dungeon.m_DungeonID} stageId:{Dungeon.StageTemplet.StrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMPhaseOrderTemplet.cs", 55);
		}
	}
}
