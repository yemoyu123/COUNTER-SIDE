using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_INVENTORY_EXPAND_REQ)]
public sealed class NKMPacket_INVENTORY_EXPAND_REQ : ISerializable
{
	public NKM_INVENTORY_EXPAND_TYPE inventoryExpandType;

	public int count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref inventoryExpandType);
		stream.PutOrGet(ref count);
	}
}
