using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_ACK)]
public sealed class NKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long equipUid;

	public int setOptionId;

	public NKMResetCount resetCount = new NKMResetCount();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipUid);
		stream.PutOrGet(ref setOptionId);
		stream.PutOrGet(ref resetCount);
	}
}
