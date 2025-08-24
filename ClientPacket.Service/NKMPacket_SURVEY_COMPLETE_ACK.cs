using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_SURVEY_COMPLETE_ACK)]
public sealed class NKMPacket_SURVEY_COMPLETE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
	}
}
