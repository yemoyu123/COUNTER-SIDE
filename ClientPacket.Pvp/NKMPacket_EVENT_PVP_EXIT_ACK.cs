using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PVP_EXIT_ACK)]
public sealed class NKMPacket_EVENT_PVP_EXIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
	}
}
