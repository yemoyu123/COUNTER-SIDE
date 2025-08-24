using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Pvp;

public sealed class AsyncPvpTarget : ISerializable
{
	public int userLevel;

	public string userNickName;

	public long userFriendCode;

	public int rank;

	public int score;

	public int tier;

	public int mainUnitId;

	public int mainUnitSkinId;

	public int selfieFrameId;

	public NKMAsyncDeckData asyncDeck = new NKMAsyncDeckData();

	public NKMGuildSimpleData guildData = new NKMGuildSimpleData();

	public int mainUnitTacticLevel;

	public int titleId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userLevel);
		stream.PutOrGet(ref userNickName);
		stream.PutOrGet(ref userFriendCode);
		stream.PutOrGet(ref rank);
		stream.PutOrGet(ref score);
		stream.PutOrGet(ref tier);
		stream.PutOrGet(ref mainUnitId);
		stream.PutOrGet(ref mainUnitSkinId);
		stream.PutOrGet(ref selfieFrameId);
		stream.PutOrGet(ref asyncDeck);
		stream.PutOrGet(ref guildData);
		stream.PutOrGet(ref mainUnitTacticLevel);
		stream.PutOrGet(ref titleId);
	}
}
