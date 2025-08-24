using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_RECONNECT_ACK)]
public sealed class NKMPacket_RECONNECT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public string accessToken;

	public string gameServerIp;

	public int gameServerPort;

	public string contentsVersion;

	public List<string> contentsTag = new List<string>();

	public List<string> openTag = new List<string>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref accessToken);
		stream.PutOrGet(ref gameServerIp);
		stream.PutOrGet(ref gameServerPort);
		stream.PutOrGet(ref contentsVersion);
		stream.PutOrGet(ref contentsTag);
		stream.PutOrGet(ref openTag);
	}
}
