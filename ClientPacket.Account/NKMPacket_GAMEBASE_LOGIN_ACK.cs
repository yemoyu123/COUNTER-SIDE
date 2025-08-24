using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_GAMEBASE_LOGIN_ACK)]
public sealed class NKMPacket_GAMEBASE_LOGIN_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public string accessToken;

	public string gameServerIP;

	public int gameServerPort;

	public string contentsVersion;

	public List<string> contentsTag = new List<string>();

	public List<string> openTag = new List<string>();

	public int resultCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref accessToken);
		stream.PutOrGet(ref gameServerIP);
		stream.PutOrGet(ref gameServerPort);
		stream.PutOrGet(ref contentsVersion);
		stream.PutOrGet(ref contentsTag);
		stream.PutOrGet(ref openTag);
		stream.PutOrGet(ref resultCode);
	}
}
