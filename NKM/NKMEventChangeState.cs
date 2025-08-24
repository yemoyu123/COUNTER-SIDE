using System.Collections.Generic;
using NKM.Unit;

namespace NKM;

public class NKMEventChangeState : NKMUnitStateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public int m_TargetUnitID;

	public string m_ChangeState = "";

	public bool m_CheckCooltime;

	public override EventRollbackType RollbackType => EventRollbackType.Prohibited;

	public override EventHostType HostType => EventHostType.Server;

	public void DeepCopyFromSource(NKMEventChangeState source)
	{
		DeepCopy(source);
		m_TargetUnitID = source.m_TargetUnitID;
		m_ChangeState = source.m_ChangeState;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_TargetUnitID", ref m_TargetUnitID);
		cNKMLua.GetData("m_ChangeState", ref m_ChangeState);
		cNKMLua.GetData("m_CheckCooltime", ref m_CheckCooltime);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		if (m_TargetUnitID <= 0)
		{
			if (!m_CheckCooltime || cNKMUnit.CheckStateCoolTime(m_ChangeState))
			{
				cNKMUnit.StateChange(m_ChangeState);
			}
			return;
		}
		List<NKMUnit> list = new List<NKMUnit>();
		cNKMGame.GetUnitByUnitID(list, m_TargetUnitID);
		for (int i = 0; i < list.Count; i++)
		{
			NKMUnit nKMUnit = list[i];
			if (nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DYING && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE && !cNKMGame.IsEnemy(cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				if (m_CheckCooltime && !nKMUnit.CheckStateCoolTime(m_ChangeState))
				{
					break;
				}
				nKMUnit.StateChange(m_ChangeState);
			}
		}
	}
}
