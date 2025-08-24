using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_SLOT_LOCK_REQ)]
public sealed class NKMPacket_SHIP_SLOT_LOCK_REQ : ISerializable
{
	public long shipUid;

	public int moduleId;

	public int slotId;

	public bool locked;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref shipUid);
		stream.PutOrGet(ref moduleId);
		stream.PutOrGet(ref slotId);
		stream.PutOrGet(ref locked);
	}
}
