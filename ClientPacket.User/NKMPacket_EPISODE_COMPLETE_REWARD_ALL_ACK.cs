using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_EPISODE_COMPLETE_REWARD_ALL_ACK)]
public sealed class NKMPacket_EPISODE_COMPLETE_REWARD_ALL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardDate;

	public List<NKMEpisodeCompleteData> episodeCompleteData = new List<NKMEpisodeCompleteData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardDate);
		stream.PutOrGet(ref episodeCompleteData);
	}
}
