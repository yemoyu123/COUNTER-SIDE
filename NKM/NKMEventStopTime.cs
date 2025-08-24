using Cs.Math;
using NKM.Unit;

namespace NKM;

public class NKMEventStopTime : NKMUnitStateEventOneTime, INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public NKM_STOP_TIME_INDEX m_StopTimeIndex;

	public float m_fStopTime = -1f;

	public float m_fStopReserveTime = -1f;

	public bool m_bStopSelf = true;

	public bool m_bStopSummonee = true;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Both;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		bool data = cNKMLua.GetData("m_StopTimeIndex", ref m_StopTimeIndex);
		cNKMLua.GetData("m_fStopTime", ref m_fStopTime);
		cNKMLua.GetData("m_fStopReserveTime", ref m_fStopReserveTime);
		cNKMLua.GetData("m_bMyStop", ref m_bStopSelf);
		cNKMLua.GetData("m_bSummoneeStop", ref m_bStopSummonee);
		return data;
	}

	public void DeepCopyFromSource(NKMEventStopTime source)
	{
		DeepCopy(source);
		m_StopTimeIndex = source.m_StopTimeIndex;
		m_fStopTime = source.m_fStopTime;
		m_fStopReserveTime = source.m_fStopReserveTime;
		m_bStopSelf = source.m_bStopSelf;
		m_bStopSummonee = source.m_bStopSummonee;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		if (!m_fStopReserveTime.IsNearlyEqual(-1f))
		{
			cNKMUnit.SetStopReserveTime(m_fStopReserveTime);
		}
		cNKMGame.SetStopTime(cNKMUnit.GetUnitDataGame().m_GameUnitUID, m_fStopTime, m_bStopSelf, m_bStopSummonee, m_StopTimeIndex);
	}

	public override void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime)
	{
		if (!m_fStopReserveTime.IsNearlyEqual(-1f))
		{
			cNKMUnit.SetStopReserveTime(m_fStopReserveTime);
		}
		cNKMGame.SetStopTime(cNKMUnit.GetUnitDataGame().m_GameUnitUID, m_fStopTime, m_bStopSelf, m_bStopSummonee, m_StopTimeIndex);
	}
}
