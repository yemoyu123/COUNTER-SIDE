using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Raid;

public sealed class NKMCoopRaidData : ISerializable
{
	public long raidUID;

	public int stageID;

	public long userUID;

	public long friendCode;

	public string nickname;

	public int mainUnitID;

	public int mainUnitSkinID;

	public int mainUnitTacticLevel;

	public float curHP;

	public float maxHP;

	public long expireDate;

	public int seasonID;

	public int cityId;

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	public List<NKMRaidJoinData> raidJoinDataList = new List<NKMRaidJoinData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref raidUID);
		stream.PutOrGet(ref stageID);
		stream.PutOrGet(ref userUID);
		stream.PutOrGet(ref friendCode);
		stream.PutOrGet(ref nickname);
		stream.PutOrGet(ref mainUnitID);
		stream.PutOrGet(ref mainUnitSkinID);
		stream.PutOrGet(ref mainUnitTacticLevel);
		stream.PutOrGet(ref curHP);
		stream.PutOrGet(ref maxHP);
		stream.PutOrGet(ref expireDate);
		stream.PutOrGet(ref seasonID);
		stream.PutOrGet(ref cityId);
		stream.PutOrGet(ref guildData);
		stream.PutOrGet(ref raidJoinDataList);
	}
}
