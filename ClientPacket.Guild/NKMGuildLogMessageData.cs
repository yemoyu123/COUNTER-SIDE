using System;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Guild;

public sealed class NKMGuildLogMessageData : ISerializable
{
	public long messageUid;

	public GuildLogType guildLogType;

	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public int emotionId;

	public string message;

	public DateTime createdAt;

	public long typeParam;

	public bool blocked;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref messageUid);
		stream.PutOrGetEnum(ref guildLogType);
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref emotionId);
		stream.PutOrGet(ref message);
		stream.PutOrGet(ref createdAt);
		stream.PutOrGet(ref typeParam);
		stream.PutOrGet(ref blocked);
	}
}
