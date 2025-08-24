namespace NKM;

public class NKMEventText : NKMUnitStateEventOneTime
{
	public string m_Text = "";

	public float m_fTime;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Client;

	public void DeepCopyFromSource(NKMEventText source)
	{
		DeepCopy(source);
		m_Text = source.m_Text;
		m_fTime = source.m_fTime;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_Text", ref m_Text);
		cNKMLua.GetData("m_fTime", ref m_fTime);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.ApplyEventText(this);
	}
}
