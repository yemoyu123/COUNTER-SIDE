using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_SEASON_REWARD_REQ)]
public sealed class NKMPacket_GUILD_DUNGEON_SEASON_REWARD_REQ : ISerializable
{
	public GuildDungeonRewardCategory rewardCategory;

	public int rewardCountValue;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref rewardCategory);
		stream.PutOrGet(ref rewardCountValue);
	}
}
