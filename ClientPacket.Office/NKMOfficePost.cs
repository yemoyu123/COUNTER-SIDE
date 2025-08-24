using System;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Office;

public sealed class NKMOfficePost : ISerializable
{
	public long postUid;

	public NKMCommonProfile senderProfile = new NKMCommonProfile();

	public NKMGuildSimpleData senderGuildData = new NKMGuildSimpleData();

	public DateTime expirationDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref postUid);
		stream.PutOrGet(ref senderProfile);
		stream.PutOrGet(ref senderGuildData);
		stream.PutOrGet(ref expirationDate);
	}
}
