using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_LOCK_EQUIP_ITEM_ACK)]
public sealed class NKMPacket_LOCK_EQUIP_ITEM_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long equipItemUID;

	public bool isLock;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipItemUID);
		stream.PutOrGet(ref isLock);
	}
}
