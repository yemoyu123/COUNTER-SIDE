using System;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_NXPC_LOGIN_REQ)]
public sealed class NKMPacket_NXPC_LOGIN_REQ : ISerializable
{
	public int protocolVersion;

	public string nexonPassport;

	public NKMUserMobileData userMobileData;

	public DateTime ssoLoginDate;

	public string deviceUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref protocolVersion);
		stream.PutOrGet(ref nexonPassport);
		stream.PutOrGet(ref userMobileData);
		stream.PutOrGet(ref ssoLoginDate);
		stream.PutOrGet(ref deviceUid);
	}
}
