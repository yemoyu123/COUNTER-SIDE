using NKM.Unit;

namespace NKM;

public class NKMEventAgro : NKMUnitStateEventOneTime, INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public bool m_bGetAgro = true;

	public float m_fRange;

	public float m_fDurationTime = 999999f;

	public int m_MaxCount;

	public bool m_bUseUnitSize;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Both;

	public void DeepCopyFromSource(NKMEventAgro source)
	{
		DeepCopy(source);
		m_bGetAgro = source.m_bGetAgro;
		m_fRange = source.m_fRange;
		m_fDurationTime = source.m_fDurationTime;
		m_MaxCount = source.m_MaxCount;
		m_bUseUnitSize = source.m_bUseUnitSize;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bGetAgro", ref m_bGetAgro);
		cNKMLua.GetData("m_fRange", ref m_fRange);
		cNKMLua.GetData("m_bStateEndTime", ref m_bStateEndTime);
		cNKMLua.GetData("m_fDurationTime", ref m_fDurationTime);
		cNKMLua.GetData("m_bUseUnitSize", ref m_bUseUnitSize);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.SetAgro(m_bGetAgro, m_fRange, m_fDurationTime, m_MaxCount, m_bUseUnitSize);
	}

	public override void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime)
	{
		cNKMUnit.SetAgro(m_bGetAgro, m_fRange, m_fDurationTime, m_MaxCount, m_bUseUnitSize);
	}
}
