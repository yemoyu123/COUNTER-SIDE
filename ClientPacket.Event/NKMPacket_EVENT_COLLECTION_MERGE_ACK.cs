using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_COLLECTION_MERGE_ACK)]
public sealed class NKMPacket_EVENT_COLLECTION_MERGE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int collectionMergeId;

	public NKMRewardData rewardData;

	public List<long> consumeTrophyUids = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref collectionMergeId);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref consumeTrophyUids);
	}
}
