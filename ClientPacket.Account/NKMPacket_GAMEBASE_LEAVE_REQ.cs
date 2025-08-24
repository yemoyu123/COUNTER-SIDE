using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_GAMEBASE_LEAVE_REQ)]
public sealed class NKMPacket_GAMEBASE_LEAVE_REQ : ISerializable
{
	public string userId;

	public string accessToken;

	public string idpCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userId);
		stream.PutOrGet(ref accessToken);
		stream.PutOrGet(ref idpCode);
	}
}
