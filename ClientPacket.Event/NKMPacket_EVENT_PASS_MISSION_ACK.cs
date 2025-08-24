using System;
using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_MISSION_ACK)]
public sealed class NKMPacket_EVENT_PASS_MISSION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isFinalMissionCompleted;

	public EventPassMissionType missionType;

	public List<NKMEventPassMissionInfo> missionInfoList;

	public DateTime nextResetDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isFinalMissionCompleted);
		stream.PutOrGetEnum(ref missionType);
		stream.PutOrGet(ref missionInfoList);
		stream.PutOrGet(ref nextResetDate);
	}
}
