using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_REQ)]
public sealed class NKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_REQ : ISerializable
{
	public EventPassMissionType missionType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref missionType);
	}
}
