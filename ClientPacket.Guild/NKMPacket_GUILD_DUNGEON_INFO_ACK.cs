using System;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_INFO_ACK)]
public sealed class NKMPacket_GUILD_DUNGEON_INFO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public GuildDungeonState guildDungeonState;

	public int seasonId;

	public int sessionId;

	public DateTime currentSessionEndDate;

	public DateTime NextSessionStartDate;

	public List<GuildDungeonArena> arenaList = new List<GuildDungeonArena>();

	public List<GuildDungeonSeasonRewardData> lastSeasonRewardData = new List<GuildDungeonSeasonRewardData>();

	public GuildDungeonBoss bossData = new GuildDungeonBoss();

	public int arenaTicketBuyCount;

	public bool canReward;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref guildDungeonState);
		stream.PutOrGet(ref seasonId);
		stream.PutOrGet(ref sessionId);
		stream.PutOrGet(ref currentSessionEndDate);
		stream.PutOrGet(ref NextSessionStartDate);
		stream.PutOrGet(ref arenaList);
		stream.PutOrGet(ref lastSeasonRewardData);
		stream.PutOrGet(ref bossData);
		stream.PutOrGet(ref arenaTicketBuyCount);
		stream.PutOrGet(ref canReward);
	}
}
