using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_JOIN_LOBBY_REQ)]
public sealed class NKMPacket_JOIN_LOBBY_REQ : ISerializable
{
	public int protocolVersion;

	public string accessToken;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref protocolVersion);
		stream.PutOrGet(ref accessToken);
	}
}
