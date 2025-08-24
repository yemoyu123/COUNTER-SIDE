using System;
using Cs.Protocol;

namespace ClientPacket.Guild;

public sealed class GuildDungeonMemberArena : ISerializable
{
	public int arenaId;

	public int grade;

	public DateTime regDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref arenaId);
		stream.PutOrGet(ref grade);
		stream.PutOrGet(ref regDate);
	}
}
