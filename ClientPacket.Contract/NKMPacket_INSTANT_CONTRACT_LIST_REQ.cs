using Cs.Protocol;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_INSTANT_CONTRACT_LIST_REQ)]
public sealed class NKMPacket_INSTANT_CONTRACT_LIST_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
