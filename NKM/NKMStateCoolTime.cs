namespace NKM;

public class NKMStateCoolTime : NKMObjectPoolData
{
	public float m_CoolTime;

	public NKMStateCoolTime()
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKMStateCoolTime;
	}

	public override void Close()
	{
		m_CoolTime = 0f;
	}
}
