using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_SKIP_ACK)]
public sealed class NKMPacket_DIVE_SKIP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMRewardData> rewardDatas = new List<NKMRewardData>();

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	public int deletedEventCityId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardDatas);
		stream.PutOrGet(ref costItems);
		stream.PutOrGet(ref deletedEventCityId);
	}
}
