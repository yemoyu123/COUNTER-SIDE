namespace NKM;

public class NKMTimeStamp : NKMObjectPoolData
{
	public bool m_FramePass;

	public NKMTimeStamp()
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKMTimeStamp;
	}

	public override void Close()
	{
		m_FramePass = false;
	}
}
