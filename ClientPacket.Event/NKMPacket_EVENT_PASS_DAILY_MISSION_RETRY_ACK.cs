using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_ACK)]
public sealed class NKMPacket_EVENT_PASS_DAILY_MISSION_RETRY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMEventPassMissionInfo missionInfo = new NKMEventPassMissionInfo();

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref missionInfo);
		stream.PutOrGet(ref costItems);
	}
}
