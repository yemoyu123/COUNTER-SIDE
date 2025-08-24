using System;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_NXTOY_LOGIN_REQ)]
public sealed class NKMPacket_NXTOY_LOGIN_REQ : ISerializable
{
	public int protocolVersion;

	public NKMNXToyData nxToyData;

	public NKMUserMobileData userMobileData;

	public DateTime toyLoginDate;

	public string deviceUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref protocolVersion);
		stream.PutOrGet(ref nxToyData);
		stream.PutOrGet(ref userMobileData);
		stream.PutOrGet(ref toyLoginDate);
		stream.PutOrGet(ref deviceUid);
	}
}
