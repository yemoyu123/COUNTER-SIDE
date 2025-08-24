using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_AD_INVENTORY_EXPAND_ACK)]
public sealed class NKMPacket_AD_INVENTORY_EXPAND_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKM_INVENTORY_EXPAND_TYPE inventoryExpandType;

	public int expandedCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref inventoryExpandType);
		stream.PutOrGet(ref expandedCount);
	}
}
