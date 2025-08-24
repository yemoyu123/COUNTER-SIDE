using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PROFILE_REQ)]
public sealed class NKMPacket_EQUIP_PROFILE_REQ : ISerializable
{
	public long unitUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
	}
}
