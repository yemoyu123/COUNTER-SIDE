using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_JOIN_ACK)]
public sealed class NKMPacket_GUILD_JOIN_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool needApproval;

	public long guildUid;

	public PrivateGuildData privateGuildData = new PrivateGuildData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref needApproval);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref privateGuildData);
	}
}
