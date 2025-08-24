using Cs.Protocol;

namespace NKM;

public class NKMShipResultData : ISerializable
{
	public short m_GameUnitUID;

	public float m_fHP;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_GameUnitUID);
		stream.PutOrGet(ref m_fHP);
	}
}
