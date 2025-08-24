using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_REQ)]
public sealed class NKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_REQ : ISerializable
{
	public long equipUid;

	public int setOptionId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipUid);
		stream.PutOrGet(ref setOptionId);
	}
}
