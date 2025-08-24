using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_ZLONG_LOGIN_REQ)]
public sealed class NKMPacket_ZLONG_LOGIN_REQ : ISerializable
{
	public int protocolVersion;

	public NKMUserMobileData userMobileData;

	public string deviceUid;

	public string opcode;

	public long channelId;

	public string tokenData;

	public string zlDeviceId;

	public string operators;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref protocolVersion);
		stream.PutOrGet(ref userMobileData);
		stream.PutOrGet(ref deviceUid);
		stream.PutOrGet(ref opcode);
		stream.PutOrGet(ref channelId);
		stream.PutOrGet(ref tokenData);
		stream.PutOrGet(ref zlDeviceId);
		stream.PutOrGet(ref operators);
	}
}
