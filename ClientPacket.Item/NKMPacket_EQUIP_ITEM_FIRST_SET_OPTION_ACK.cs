using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_ACK)]
public sealed class NKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long equipUID;

	public int setOptionId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipUID);
		stream.PutOrGet(ref setOptionId);
	}
}
