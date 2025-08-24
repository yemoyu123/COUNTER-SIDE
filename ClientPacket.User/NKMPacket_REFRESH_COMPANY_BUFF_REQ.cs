using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_REFRESH_COMPANY_BUFF_REQ)]
public sealed class NKMPacket_REFRESH_COMPANY_BUFF_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
