using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_CONTENTS_VERSION_REQ)]
public sealed class NKMPacket_CONTENTS_VERSION_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
