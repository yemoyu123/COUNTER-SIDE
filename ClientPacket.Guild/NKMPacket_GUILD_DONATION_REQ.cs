using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DONATION_REQ)]
public sealed class NKMPacket_GUILD_DONATION_REQ : ISerializable
{
	public long guildUid;

	public int donationId;

	public int donationCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref donationId);
		stream.PutOrGet(ref donationCount);
	}
}
