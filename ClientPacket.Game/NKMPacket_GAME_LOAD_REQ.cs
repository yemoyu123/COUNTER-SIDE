using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_LOAD_REQ)]
public sealed class NKMPacket_GAME_LOAD_REQ : ISerializable
{
	public bool isDev;

	public byte selectDeckIndex;

	public int stageID;

	public int diveStageID;

	public int dungeonID;

	public int palaceID;

	public int fierceBossId;

	public long supportingUserUid;

	public NKMEventDeckData eventDeckData;

	public int rewardMultiply;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isDev);
		stream.PutOrGet(ref selectDeckIndex);
		stream.PutOrGet(ref stageID);
		stream.PutOrGet(ref diveStageID);
		stream.PutOrGet(ref dungeonID);
		stream.PutOrGet(ref palaceID);
		stream.PutOrGet(ref fierceBossId);
		stream.PutOrGet(ref supportingUserUid);
		stream.PutOrGet(ref eventDeckData);
		stream.PutOrGet(ref rewardMultiply);
	}
}
