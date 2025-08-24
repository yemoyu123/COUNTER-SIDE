using Cs.Protocol;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_CONTRACT_REQ)]
public sealed class NKMPacket_CONTRACT_REQ : ISerializable
{
	public int contractId;

	public int count;

	public ContractCostType costType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref contractId);
		stream.PutOrGet(ref count);
		stream.PutOrGetEnum(ref costType);
	}
}
