using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_COLLECTION_MERGE_REQ)]
public sealed class NKMPacket_EVENT_COLLECTION_MERGE_REQ : ISerializable
{
	public int collectionMergeId;

	public int mergeRecipeGroupId;

	public List<long> consumeTrophyUids = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref collectionMergeId);
		stream.PutOrGet(ref mergeRecipeGroupId);
		stream.PutOrGet(ref consumeTrophyUids);
	}
}
