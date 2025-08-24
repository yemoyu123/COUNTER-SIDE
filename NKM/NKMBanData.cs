using Cs.Protocol;

namespace NKM;

public class NKMBanData : ISerializable
{
	public int m_UnitID;

	public byte m_BanLevel;

	public void DeepCopyFromSource(NKMBanData source)
	{
		m_UnitID = source.m_UnitID;
		m_BanLevel = source.m_BanLevel;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_UnitID);
		stream.PutOrGet(ref m_BanLevel);
	}
}
