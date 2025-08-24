using Cs.Protocol;

namespace NKM;

public class NKMBanOperatorData : ISerializable
{
	public int m_OperatorID;

	public byte m_BanLevel;

	public void DeepCopyFromSource(NKMBanOperatorData source)
	{
		m_OperatorID = source.m_OperatorID;
		m_BanLevel = source.m_BanLevel;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_OperatorID);
		stream.PutOrGet(ref m_BanLevel);
	}
}
