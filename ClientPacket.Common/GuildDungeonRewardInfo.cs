using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class GuildDungeonRewardInfo : ISerializable
{
	public int currentSeasonId;

	public List<GuildDungeonSeasonRewardData> lastSeasonRewardData = new List<GuildDungeonSeasonRewardData>();

	public bool canReward;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref currentSeasonId);
		stream.PutOrGet(ref lastSeasonRewardData);
		stream.PutOrGet(ref canReward);
	}
}
