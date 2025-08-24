using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_KAKAO_MISSION_REFRESH_STATE_REQ)]
public sealed class NKMPacket_KAKAO_MISSION_REFRESH_STATE_REQ : ISerializable
{
	public int eventId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventId);
	}
}
