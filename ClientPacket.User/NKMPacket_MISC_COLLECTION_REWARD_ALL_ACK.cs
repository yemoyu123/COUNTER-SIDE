using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_MISC_COLLECTION_REWARD_ALL_ACK)]
public sealed class NKMPacket_MISC_COLLECTION_REWARD_ALL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKM_ITEM_MISC_TYPE miscType;

	public NKMRewardData rewardData;

	public List<NKMMiscCollectionData> miscCollectionDatas = new List<NKMMiscCollectionData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref miscType);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref miscCollectionDatas);
	}
}
