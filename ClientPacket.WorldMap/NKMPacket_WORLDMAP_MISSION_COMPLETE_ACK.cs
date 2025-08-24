using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_MISSION_COMPLETE_ACK)]
public sealed class NKMPacket_WORLDMAP_MISSION_COMPLETE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int cityID;

	public int clearedMissionID;

	public int level;

	public int exp;

	public List<int> stMissionIDList = new List<int>();

	public NKMRewardData rewardData;

	public bool isSuccess;

	public NKMWorldMapEventGroup worldMapEventGroup = new NKMWorldMapEventGroup();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref clearedMissionID);
		stream.PutOrGet(ref level);
		stream.PutOrGet(ref exp);
		stream.PutOrGet(ref stMissionIDList);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref isSuccess);
		stream.PutOrGet(ref worldMapEventGroup);
	}
}
