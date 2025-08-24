using System;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class FriendListData : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public DateTime lastLoginDate;

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	public bool hasOffice;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref lastLoginDate);
		stream.PutOrGet(ref guildData);
		stream.PutOrGet(ref hasOffice);
	}
}
