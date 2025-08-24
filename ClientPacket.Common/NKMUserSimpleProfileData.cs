using System;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMUserSimpleProfileData : ISerializable
{
	public long userUid;

	public long friendCode;

	public string nickname;

	public int level;

	public int mainUnitId;

	public int mainUnitSkinId;

	public int frameId;

	public int mainUnitTacticLevel;

	public int titleId;

	public int pvpScore;

	public int pvpTier;

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	public DateTime lastLoginDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
		stream.PutOrGet(ref friendCode);
		stream.PutOrGet(ref nickname);
		stream.PutOrGet(ref level);
		stream.PutOrGet(ref mainUnitId);
		stream.PutOrGet(ref mainUnitSkinId);
		stream.PutOrGet(ref frameId);
		stream.PutOrGet(ref mainUnitTacticLevel);
		stream.PutOrGet(ref titleId);
		stream.PutOrGet(ref pvpScore);
		stream.PutOrGet(ref pvpTier);
		stream.PutOrGet(ref guildData);
		stream.PutOrGet(ref lastLoginDate);
	}
}
