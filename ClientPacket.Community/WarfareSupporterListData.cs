using System;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Community;

public sealed class WarfareSupporterListData : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public NKMDummyDeckData deckData;

	public DateTime lastLoginDate;

	public DateTime lastUsedDate;

	public string message;

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref deckData);
		stream.PutOrGet(ref lastLoginDate);
		stream.PutOrGet(ref lastUsedDate);
		stream.PutOrGet(ref message);
		stream.PutOrGet(ref guildData);
	}
}
