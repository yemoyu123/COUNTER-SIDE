using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_AD_INVENTORY_EXPAND_REQ)]
public sealed class NKMPacket_AD_INVENTORY_EXPAND_REQ : ISerializable
{
	public NKM_INVENTORY_EXPAND_TYPE inventoryExpandType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref inventoryExpandType);
	}
}
