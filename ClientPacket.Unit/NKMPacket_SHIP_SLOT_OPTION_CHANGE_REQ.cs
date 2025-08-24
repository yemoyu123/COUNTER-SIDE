using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_SLOT_OPTION_CHANGE_REQ)]
public sealed class NKMPacket_SHIP_SLOT_OPTION_CHANGE_REQ : ISerializable
{
	public long shipUid;

	public int moduleId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref shipUid);
		stream.PutOrGet(ref moduleId);
	}
}
