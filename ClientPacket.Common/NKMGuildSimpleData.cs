using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMGuildSimpleData : ISerializable
{
	public long guildUid;

	public string guildName;

	public long badgeId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref guildName);
		stream.PutOrGet(ref badgeId);
	}
}
