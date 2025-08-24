using Cs.Protocol;
using NKM;
using NKM.Templet;

namespace ClientPacket.Warfare;

public sealed class WarfareUnitData : ISerializable
{
	public enum Type : byte
	{
		User,
		Dungeon
	}

	public int warfareGameUnitUID;

	public Type unitType;

	public NKM_WARFARE_ENEMY_ACTION_TYPE warfareEnemyActionType;

	public NKMDeckIndex deckIndex;

	public long friendCode;

	public int dungeonID;

	public float hp;

	public float hpMax;

	public bool isTurnEnd;

	public int supply;

	public short tileIndex;

	public bool isTarget;

	public bool isSummonee;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref warfareGameUnitUID);
		stream.PutOrGetEnum(ref unitType);
		stream.PutOrGetEnum(ref warfareEnemyActionType);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref friendCode);
		stream.PutOrGet(ref dungeonID);
		stream.PutOrGet(ref hp);
		stream.PutOrGet(ref hpMax);
		stream.PutOrGet(ref isTurnEnd);
		stream.PutOrGet(ref supply);
		stream.PutOrGet(ref tileIndex);
		stream.PutOrGet(ref isTarget);
		stream.PutOrGet(ref isSummonee);
	}
}
