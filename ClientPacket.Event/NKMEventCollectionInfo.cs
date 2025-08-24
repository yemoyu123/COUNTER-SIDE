using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMEventCollectionInfo : ISerializable
{
	public int eventId;

	public HashSet<int> goodsCollection = new HashSet<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventId);
		stream.PutOrGet(ref goodsCollection);
	}
}
