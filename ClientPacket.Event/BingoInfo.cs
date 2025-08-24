using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class BingoInfo : ISerializable
{
	public int eventId;

	public List<int> tileValueList = new List<int>();

	public List<int> markTileIndexList = new List<int>();

	public List<int> rewardList = new List<int>();

	public int mileage;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventId);
		stream.PutOrGet(ref tileValueList);
		stream.PutOrGet(ref markTileIndexList);
		stream.PutOrGet(ref rewardList);
		stream.PutOrGet(ref mileage);
	}
}
