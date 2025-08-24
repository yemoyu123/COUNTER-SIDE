using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_OPEN_SOCKET_REQ)]
public sealed class NKMPacket_EQUIP_OPEN_SOCKET_REQ : ISerializable
{
	public long equipUid;

	public int socketIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipUid);
		stream.PutOrGet(ref socketIndex);
	}
}
