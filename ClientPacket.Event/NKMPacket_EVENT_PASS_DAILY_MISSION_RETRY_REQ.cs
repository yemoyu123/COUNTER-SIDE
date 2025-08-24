using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_REQ)]
public sealed class NKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_REQ : ISerializable
{
	public int missionId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref missionId);
	}
}
