namespace NKC;

public class NKCMessageData
{
	public NKC_EVENT_MESSAGE m_NKC_EVENT_MESSAGE;

	public int m_MsgID2;

	public object m_Param1;

	public object m_Param2;

	public object m_Param3;

	public float m_fLatency;

	public NKCMessageData()
	{
		Init();
	}

	public void Init()
	{
		m_NKC_EVENT_MESSAGE = NKC_EVENT_MESSAGE.NEM_INVALID;
		m_MsgID2 = 0;
		m_Param1 = null;
		m_Param2 = null;
		m_Param3 = null;
		m_fLatency = 0f;
	}
}
