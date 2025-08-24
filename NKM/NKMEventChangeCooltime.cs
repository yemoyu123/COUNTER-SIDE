namespace NKM;

public class NKMEventChangeCooltime : NKMUnitStateEventOneTime
{
	public enum ChangeType : short
	{
		SET_RATIO,
		ADD_SECONDS
	}

	public string m_TargetStateName = "";

	public float m_fChangeValue;

	public ChangeType m_eChangeType;

	public bool m_bUseTriggerTargetRange;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public void DeepCopyFromSource(NKMEventChangeCooltime source)
	{
		DeepCopy(source);
		m_TargetStateName = source.m_TargetStateName;
		m_fChangeValue = source.m_fChangeValue;
		m_eChangeType = source.m_eChangeType;
		m_bUseTriggerTargetRange = source.m_bUseTriggerTargetRange;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_bStateEndTime = false;
		cNKMLua.GetData("m_fEventAnimTime", ref m_fEventTime);
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_TargetStateName", ref m_TargetStateName);
		cNKMLua.GetDataEnum<ChangeType>("m_eChangeType", out m_eChangeType);
		cNKMLua.GetData("m_fChangeValue", ref m_fChangeValue);
		cNKMLua.GetData("m_bUseTriggerTargetRange", ref m_bUseTriggerTargetRange);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		NKMUnit triggerTargetUnit = cNKMUnit.GetTriggerTargetUnit(m_bUseTriggerTargetRange);
		switch (m_eChangeType)
		{
		case ChangeType.SET_RATIO:
			triggerTargetUnit.SetStateCoolTime(m_TargetStateName, bMax: true, m_fChangeValue);
			break;
		case ChangeType.ADD_SECONDS:
			triggerTargetUnit.SetStateCoolTimeAdd(m_TargetStateName, m_fChangeValue);
			break;
		}
		triggerTargetUnit.SetPushSync();
	}
}
