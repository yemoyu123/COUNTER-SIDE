using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Pvp;

public sealed class NpcPvpTarget : ISerializable
{
	public int userLevel;

	public string userNickName;

	public long userFriendCode;

	public int score;

	public int tier;

	public NKMAsyncDeckData asyncDeck = new NKMAsyncDeckData();

	public bool isOpened;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userLevel);
		stream.PutOrGet(ref userNickName);
		stream.PutOrGet(ref userFriendCode);
		stream.PutOrGet(ref score);
		stream.PutOrGet(ref tier);
		stream.PutOrGet(ref asyncDeck);
		stream.PutOrGet(ref isOpened);
	}
}
