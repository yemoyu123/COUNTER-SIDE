using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class GuildDungeonSeasonRewardData : ISerializable
{
	public GuildDungeonRewardCategory category;

	public int totalValue;

	public int receivedValue;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref category);
		stream.PutOrGet(ref totalValue);
		stream.PutOrGet(ref receivedValue);
	}
}
