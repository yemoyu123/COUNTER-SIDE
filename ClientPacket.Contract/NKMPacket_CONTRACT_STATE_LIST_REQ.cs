using Cs.Protocol;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_CONTRACT_STATE_LIST_REQ)]
public sealed class NKMPacket_CONTRACT_STATE_LIST_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
