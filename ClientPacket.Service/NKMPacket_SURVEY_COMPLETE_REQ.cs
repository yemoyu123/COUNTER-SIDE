using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_SURVEY_COMPLETE_REQ)]
public sealed class NKMPacket_SURVEY_COMPLETE_REQ : ISerializable
{
	public long surveyId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref surveyId);
	}
}
