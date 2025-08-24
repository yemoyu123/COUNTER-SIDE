using Cs.Protocol;

namespace NKM;

public class NKMBanShipData : ISerializable
{
	public int m_ShipGroupID;

	public byte m_BanLevel;

	public void DeepCopyFromSource(NKMBanShipData source)
	{
		m_ShipGroupID = source.m_ShipGroupID;
		m_BanLevel = source.m_BanLevel;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_ShipGroupID);
		stream.PutOrGet(ref m_BanLevel);
	}
}
