using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_REQ)]
public sealed class NKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_REQ : ISerializable
{
	public long equipUID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipUID);
	}
}
