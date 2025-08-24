using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_LOCK_EQUIP_ITEM_REQ)]
public sealed class NKMPacket_LOCK_EQUIP_ITEM_REQ : ISerializable
{
	public long equipItemUID;

	public bool isLock;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipItemUID);
		stream.PutOrGet(ref isLock);
	}
}
