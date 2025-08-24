using NKM.Unit;

namespace NKM;

public class NKMEventHyperSkillCutIn : NKMUnitStateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public float m_fDurationTime = 1f;

	public string m_BGEffectName = "";

	public string m_CutInEffectName = "";

	public string m_CutInEffectAnimName = "BASE";

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Client;

	public NKMEventHyperSkillCutIn()
	{
		m_bStateEndTime = false;
	}

	public void DeepCopyFromSource(NKMEventHyperSkillCutIn source)
	{
		DeepCopy(source);
		m_BGEffectName = source.m_BGEffectName;
		m_CutInEffectName = source.m_CutInEffectName;
		m_CutInEffectAnimName = source.m_CutInEffectAnimName;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		m_bStateEndTime = false;
		cNKMLua.GetData("m_BGEffectName", ref m_BGEffectName);
		cNKMLua.GetData("m_CutInEffectName", ref m_CutInEffectName);
		cNKMLua.GetData("m_CutInEffectAnimName", ref m_CutInEffectAnimName);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.ApplyEventHyperSkillCutIn(this);
	}
}
