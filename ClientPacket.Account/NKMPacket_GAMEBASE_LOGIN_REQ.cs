using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_GAMEBASE_LOGIN_REQ)]
public sealed class NKMPacket_GAMEBASE_LOGIN_REQ : ISerializable
{
	public int protocolVersion;

	public NKMUserMobileData userMobileData;

	public string deviceUid;

	public string userId;

	public string accessToken;

	public string idpCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref protocolVersion);
		stream.PutOrGet(ref userMobileData);
		stream.PutOrGet(ref deviceUid);
		stream.PutOrGet(ref userId);
		stream.PutOrGet(ref accessToken);
		stream.PutOrGet(ref idpCode);
	}
}
