using Cs.Protocol;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_REQ)]
public sealed class NKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_REQ : ISerializable
{
	public int contractId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref contractId);
	}
}
