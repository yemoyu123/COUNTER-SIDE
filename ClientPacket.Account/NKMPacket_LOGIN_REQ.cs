using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_LOGIN_REQ)]
public sealed class NKMPacket_LOGIN_REQ : ISerializable
{
	public long protocolVersion;

	public string accountID;

	public string password;

	public NKM_USER_AUTH_LEVEL userAuthLevel;

	public string deviceUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref protocolVersion);
		stream.PutOrGet(ref accountID);
		stream.PutOrGet(ref password);
		stream.PutOrGetEnum(ref userAuthLevel);
		stream.PutOrGet(ref deviceUid);
	}
}
