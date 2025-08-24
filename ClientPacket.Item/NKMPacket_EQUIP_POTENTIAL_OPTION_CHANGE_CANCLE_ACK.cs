using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_ACK)]
public sealed class NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMEquipItemData equipItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipItemData);
	}
}
