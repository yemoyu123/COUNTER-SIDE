namespace NKM;

public class NKMEventChangeRemainTime : NKMUnitStateEventOneTime
{
	public float m_TimeSeconds;

	public bool m_bDelta;

	public override EventRollbackType RollbackType => EventRollbackType.Prohibited;

	public override EventHostType HostType => EventHostType.Server;

	public void DeepCopyFromSource(NKMEventChangeRemainTime source)
	{
		DeepCopy(source);
		m_TimeSeconds = source.m_TimeSeconds;
		m_bDelta = source.m_bDelta;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_TimeSeconds", ref m_TimeSeconds);
		cNKMLua.GetData("m_bDelta", ref m_bDelta);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMGame.ChangeRemainGameTime(m_TimeSeconds, m_bDelta, bShowEffect: true);
	}
}
