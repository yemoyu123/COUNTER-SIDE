using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_ASYNC_PVP_RANK_SEASON_REWARD_ACK)]
public sealed class NKMPacket_ASYNC_PVP_RANK_SEASON_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	public PvpState pvpState;

	public NpcPvpData npcPvpData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref pvpState);
		stream.PutOrGet(ref npcPvpData);
	}
}
