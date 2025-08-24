using System;
using Cs.Protocol;

namespace ClientPacket.WorldMap;

public sealed class NKMWorldMapEventGroup : ISerializable
{
	public int worldmapEventID;

	public DateTime eventGroupEndDate;

	public long eventUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref worldmapEventID);
		stream.PutOrGet(ref eventGroupEndDate);
		stream.PutOrGet(ref eventUid);
	}
}
