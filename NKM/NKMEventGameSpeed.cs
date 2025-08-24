using NKM.Unit;

namespace NKM;

public class NKMEventGameSpeed : NKMUnitStateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public float m_fGameSpeed = 1f;

	public float m_fTrackingTime;

	public override EventRollbackType RollbackType => EventRollbackType.Prohibited;

	public override EventHostType HostType => EventHostType.Both;

	public void DeepCopyFromSource(NKMEventGameSpeed source)
	{
		DeepCopy(source);
		m_fGameSpeed = source.m_fGameSpeed;
		m_fTrackingTime = source.m_fTrackingTime;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_fGameSpeed", ref m_fGameSpeed);
		cNKMLua.GetData("m_fTrackingTime", ref m_fTrackingTime);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMGame.SetGameSpeed(m_fGameSpeed, m_fTrackingTime);
	}
}
