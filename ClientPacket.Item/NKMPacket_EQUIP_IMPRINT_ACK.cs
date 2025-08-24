using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_IMPRINT_ACK)]
public sealed class NKMPacket_EQUIP_IMPRINT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMEquipItemData equipItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipItemData);
	}
}
