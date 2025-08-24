using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_SURVEY_RESET_NOT)]
public sealed class NKMPacket_SURVEY_RESET_NOT : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
