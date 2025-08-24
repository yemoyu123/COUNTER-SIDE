using Cs.Protocol;

namespace ClientPacket.Guild;

public sealed class GuildDungeonArena : ISerializable
{
	public int arenaIndex;

	public int totalMedalCount;

	public long playUserUid;

	public int flagIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref arenaIndex);
		stream.PutOrGet(ref totalMedalCount);
		stream.PutOrGet(ref playUserUid);
		stream.PutOrGet(ref flagIndex);
	}
}
