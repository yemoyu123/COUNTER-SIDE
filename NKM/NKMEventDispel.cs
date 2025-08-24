using NKM.Game;
using NKM.Unit;

namespace NKM;

public class NKMEventDispel : NKMUnitStateEventOneTime, INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public bool m_bDebuff = true;

	public bool m_bDeleteInfinity;

	public float m_fRangeMin;

	public float m_fRangeMax;

	public bool m_bUseUnitSize;

	public bool m_bUseTriggerTargetRange;

	public int m_MaxCount;

	public bool m_bTargetSelf;

	public bool m_bCanDispelStatus;

	public int m_DispelCountPerSkillLevel;

	public NKMEventConditionV2 m_ConditionTarget;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public void DeepCopyFromSource(NKMEventDispel source)
	{
		DeepCopy(source);
		m_bDebuff = source.m_bDebuff;
		m_bDeleteInfinity = source.m_bDeleteInfinity;
		m_fRangeMin = source.m_fRangeMin;
		m_fRangeMax = source.m_fRangeMax;
		m_bUseUnitSize = source.m_bUseUnitSize;
		m_MaxCount = source.m_MaxCount;
		m_bTargetSelf = source.m_bTargetSelf;
		m_bCanDispelStatus = source.m_bCanDispelStatus;
		m_DispelCountPerSkillLevel = source.m_DispelCountPerSkillLevel;
		m_ConditionTarget = NKMEventConditionV2.Clone(source.m_ConditionTarget);
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bDebuff", ref m_bDebuff);
		cNKMLua.GetData("m_bDeleteInfinity", ref m_bDeleteInfinity);
		cNKMLua.GetData("m_fRangeMin", ref m_fRangeMin);
		cNKMLua.GetData("m_fRangeMax", ref m_fRangeMax);
		cNKMLua.GetData("m_bUseUnitSize", ref m_bUseUnitSize);
		cNKMLua.GetData("m_MaxCount", ref m_MaxCount);
		cNKMLua.GetData("m_bTargetSelf", ref m_bTargetSelf);
		cNKMLua.GetData("m_bCanDispelStatus", ref m_bCanDispelStatus);
		cNKMLua.GetData("m_DispelCountPerSkillLevel", ref m_DispelCountPerSkillLevel);
		m_ConditionTarget = NKMEventConditionV2.LoadFromLUA(cNKMLua, "m_ConditionTarget");
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.SetDispel(this);
	}

	public override void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime)
	{
		cNKMUnit.SetDispel(this);
	}
}
