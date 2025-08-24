using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_RANDOM_ITEM_BOX_OPEN_REQ)]
public sealed class NKMPacket_RANDOM_ITEM_BOX_OPEN_REQ : ISerializable
{
	public int itemID;

	public int count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref itemID);
		stream.PutOrGet(ref count);
	}
}
