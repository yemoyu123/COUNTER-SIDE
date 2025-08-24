using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Raid;

public sealed class NKMRaidJoinData : ISerializable
{
	public long userUID;

	public long friendCode;

	public string nickName;

	public int mainUnitID;

	public int mainUnitSkinID;

	public float damage;

	public bool highScore;

	public short tryCount;

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	public bool tryAssist;

	public int level;

	public int titleId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUID);
		stream.PutOrGet(ref friendCode);
		stream.PutOrGet(ref nickName);
		stream.PutOrGet(ref mainUnitID);
		stream.PutOrGet(ref mainUnitSkinID);
		stream.PutOrGet(ref damage);
		stream.PutOrGet(ref highScore);
		stream.PutOrGet(ref tryCount);
		stream.PutOrGet(ref guildData);
		stream.PutOrGet(ref tryAssist);
		stream.PutOrGet(ref level);
		stream.PutOrGet(ref titleId);
	}
}
