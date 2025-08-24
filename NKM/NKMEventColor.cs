using NKM.Unit;

namespace NKM;

public class NKMEventColor : NKMUnitStateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public float m_fColorR = 1f;

	public float m_fColorG = 1f;

	public float m_fColorB = 1f;

	public float m_fTrackTime;

	public float m_fColorTime;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Client;

	public void DeepCopyFromSource(NKMEventColor source)
	{
		DeepCopy(source);
		m_fColorR = source.m_fColorR;
		m_fColorG = source.m_fColorG;
		m_fColorB = source.m_fColorB;
		m_fTrackTime = source.m_fTrackTime;
		m_fColorTime = source.m_fColorTime;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_fColorR", ref m_fColorR);
		cNKMLua.GetData("m_fColorG", ref m_fColorG);
		cNKMLua.GetData("m_fColorB", ref m_fColorB);
		cNKMLua.GetData("m_fTrackTime", ref m_fTrackTime);
		cNKMLua.GetData("m_fColorTime", ref m_fColorTime);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.ApplyEventColor(this);
	}
}
