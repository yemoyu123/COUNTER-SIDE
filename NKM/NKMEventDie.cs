using NKM.Unit;

namespace NKM;

public class NKMEventDie : NKMUnitStateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public bool m_bImmediateDie;

	public override EventRollbackType RollbackType => EventRollbackType.Prohibited;

	public override EventHostType HostType => EventHostType.Both;

	public void DeepCopyFromSource(NKMEventDie source)
	{
		DeepCopy(source);
		m_bImmediateDie = source.m_bImmediateDie;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bImmediateDie", ref m_bImmediateDie);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.EventDie(m_bImmediateDie);
	}
}
