using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_IMPRINT_REQ)]
public sealed class NKMPacket_EQUIP_IMPRINT_REQ : ISerializable
{
	public long equipUid;

	public int unitId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipUid);
		stream.PutOrGet(ref unitId);
	}
}
