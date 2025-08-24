using System;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Office;

public sealed class NKMOfficeChatMessageData : ISerializable
{
	public long messageUid;

	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public int emotionId;

	public DateTime createdAt;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref messageUid);
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref emotionId);
		stream.PutOrGet(ref createdAt);
	}
}
