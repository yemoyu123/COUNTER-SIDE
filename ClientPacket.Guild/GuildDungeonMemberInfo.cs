using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Guild;

public sealed class GuildDungeonMemberInfo : ISerializable
{
	public NKMCommonProfile profile = new NKMCommonProfile();

	public List<GuildDungeonMemberArena> arenaList = new List<GuildDungeonMemberArena>();

	public int bossPoint;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref profile);
		stream.PutOrGet(ref arenaList);
		stream.PutOrGet(ref bossPoint);
	}
}
