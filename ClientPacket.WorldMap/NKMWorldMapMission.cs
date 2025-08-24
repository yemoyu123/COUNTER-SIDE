using System;
using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.WorldMap;

public sealed class NKMWorldMapMission : ISerializable
{
	public int currentMissionID;

	public long completeTime;

	public DateTime startDate;

	public List<int> stMissionIDList = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref currentMissionID);
		stream.PutOrGet(ref completeTime);
		stream.PutOrGet(ref startDate);
		stream.PutOrGet(ref stMissionIDList);
	}
}
