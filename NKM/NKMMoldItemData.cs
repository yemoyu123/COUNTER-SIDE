using Cs.Protocol;

namespace NKM;

public class NKMMoldItemData : ISerializable
{
	public int m_MoldID;

	public long m_Count;

	public NKMMoldItemData()
	{
	}

	public NKMMoldItemData(int moldID, long Count)
	{
		m_MoldID = moldID;
		m_Count = Count;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_MoldID);
		stream.PutOrGet(ref m_Count);
	}
}
