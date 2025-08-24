using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_ACK)]
public sealed class NKMPacket_EVENT_PASS_FINAL_MISSION_COMPLETE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int totalExp;

	public EventPassMissionType missionType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref totalExp);
		stream.PutOrGetEnum(ref missionType);
	}
}
