using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_COUNTERCASE_UNLOCK_ACK)]
public sealed class NKMPacket_COUNTERCASE_UNLOCK_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int dungeonID;

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref dungeonID);
		stream.PutOrGet(ref costItemData);
	}
}
