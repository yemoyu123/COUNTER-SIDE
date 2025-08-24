using System.Collections.Generic;
using ClientPacket.Community;
using Cs.Protocol;

namespace ClientPacket.Warfare;

public sealed class WarfareGameData : ISerializable
{
	public NKM_WARFARE_GAME_STATE warfareGameState;

	public int warfareTempletID;

	public List<WarfareTileData> warfareTileDataList = new List<WarfareTileData>();

	public WarfareTeamData warfareTeamDataA = new WarfareTeamData();

	public WarfareTeamData warfareTeamDataB = new WarfareTeamData();

	public bool isTurnA;

	public int turnCount;

	public int firstAttackCount;

	public int battleAllyUid;

	public int battleMonsterUid;

	public bool isWinTeamA;

	public long expireTimeTick;

	public int holdCount;

	public short containerCount;

	public byte flagshipDeckIndex;

	public byte alliesKillCount;

	public byte enemiesKillCount;

	public byte targetKillCount;

	public byte assistCount;

	public byte supplyUseCount;

	public WarfareSupporterListData supportUnitData = new WarfareSupporterListData();

	public int rewardMultiply;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref warfareGameState);
		stream.PutOrGet(ref warfareTempletID);
		stream.PutOrGet(ref warfareTileDataList);
		stream.PutOrGet(ref warfareTeamDataA);
		stream.PutOrGet(ref warfareTeamDataB);
		stream.PutOrGet(ref isTurnA);
		stream.PutOrGet(ref turnCount);
		stream.PutOrGet(ref firstAttackCount);
		stream.PutOrGet(ref battleAllyUid);
		stream.PutOrGet(ref battleMonsterUid);
		stream.PutOrGet(ref isWinTeamA);
		stream.PutOrGet(ref expireTimeTick);
		stream.PutOrGet(ref holdCount);
		stream.PutOrGet(ref containerCount);
		stream.PutOrGet(ref flagshipDeckIndex);
		stream.PutOrGet(ref alliesKillCount);
		stream.PutOrGet(ref enemiesKillCount);
		stream.PutOrGet(ref targetKillCount);
		stream.PutOrGet(ref assistCount);
		stream.PutOrGet(ref supplyUseCount);
		stream.PutOrGet(ref supportUnitData);
		stream.PutOrGet(ref rewardMultiply);
	}
}
