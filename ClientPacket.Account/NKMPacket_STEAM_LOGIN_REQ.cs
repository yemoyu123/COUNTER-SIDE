using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_STEAM_LOGIN_REQ)]
public sealed class NKMPacket_STEAM_LOGIN_REQ : ISerializable
{
	public int protocolVersion;

	public string deviceUid;

	public string accessToken;

	public string accountId;

	public NKMUserMobileData userMobileData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref protocolVersion);
		stream.PutOrGet(ref deviceUid);
		stream.PutOrGet(ref accessToken);
		stream.PutOrGet(ref accountId);
		stream.PutOrGet(ref userMobileData);
	}
}
