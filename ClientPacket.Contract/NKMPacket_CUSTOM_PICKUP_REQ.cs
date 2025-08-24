using Cs.Protocol;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_CUSTOM_PICKUP_REQ)]
public sealed class NKMPacket_CUSTOM_PICKUP_REQ : ISerializable
{
	public int customPickupId;

	public int count;

	public ContractCostType costType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref customPickupId);
		stream.PutOrGet(ref count);
		stream.PutOrGetEnum(ref costType);
	}
}
