using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PVP_RANK_WEEK_REWARD_ACK)]
public sealed class NKMPacket_PVP_RANK_WEEK_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	public PvpState pvpData;

	public bool isScoreChanged;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref pvpData);
		stream.PutOrGet(ref isScoreChanged);
	}
}
