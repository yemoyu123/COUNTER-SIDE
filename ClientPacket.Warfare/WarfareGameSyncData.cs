using Cs.Protocol;

namespace ClientPacket.Warfare;

public sealed class WarfareGameSyncData : ISerializable
{
	public NKM_WARFARE_GAME_STATE warfareGameState;

	public bool isTurnA;

	public bool isWinTeamA;

	public int turnCount;

	public int firstAttackCount;

	public int battleAllyUid;

	public int battleMonsterUid;

	public int holdCount;

	public short containerCount;

	public byte alliesKillCount;

	public byte enemiesKillCount;

	public byte targetKillCount;

	public byte assistCount;

	public byte supplyUseCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref warfareGameState);
		stream.PutOrGet(ref isTurnA);
		stream.PutOrGet(ref isWinTeamA);
		stream.PutOrGet(ref turnCount);
		stream.PutOrGet(ref firstAttackCount);
		stream.PutOrGet(ref battleAllyUid);
		stream.PutOrGet(ref battleMonsterUid);
		stream.PutOrGet(ref holdCount);
		stream.PutOrGet(ref containerCount);
		stream.PutOrGet(ref alliesKillCount);
		stream.PutOrGet(ref enemiesKillCount);
		stream.PutOrGet(ref targetKillCount);
		stream.PutOrGet(ref assistCount);
		stream.PutOrGet(ref supplyUseCount);
	}
}
