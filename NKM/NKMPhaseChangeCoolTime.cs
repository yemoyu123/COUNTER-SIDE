namespace NKM;

public class NKMPhaseChangeCoolTime
{
	public string m_StateName = "";

	public float m_fCoolTime;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_StateName", ref m_StateName);
		cNKMLua.GetData("m_fCoolTime", ref m_fCoolTime);
		return true;
	}

	public void DeepCopyFromSource(NKMPhaseChangeCoolTime source)
	{
		m_StateName = source.m_StateName;
		m_fCoolTime = source.m_fCoolTime;
	}
}
