using System;
using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_CONTENTS_VERSION_ACK)]
public sealed class NKMPacket_CONTENTS_VERSION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public string contentsVersion;

	public List<string> contentsTag = new List<string>();

	public DateTime utcTime;

	public TimeSpan utcOffset;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref contentsVersion);
		stream.PutOrGet(ref contentsTag);
		stream.PutOrGet(ref utcTime);
		stream.PutOrGet(ref utcOffset);
	}
}
