using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_SEASON_REWARD_ACK)]
public sealed class NKMPacket_GUILD_DUNGEON_SEASON_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public GuildDungeonRewardCategory rewardCategory;

	public int rewardCountValue;

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref rewardCategory);
		stream.PutOrGet(ref rewardCountValue);
		stream.PutOrGet(ref rewardData);
	}
}
