using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Mode;
using ClientPacket.Warfare;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_END_NOT)]
public sealed class NKMPacket_GAME_END_NOT : ISerializable
{
	public bool win;

	public bool giveup;

	public bool restart;

	public NKMDungeonClearData dungeonClearData;

	public NKMPhaseClearData phaseClearData;

	public NKMEpisodeCompleteData episodeCompleteData;

	public NKMDeckIndex deckIndex;

	public WarfareSyncData warfareSyncData;

	public NKMPVPResultDataForClient pvpResultData;

	public NKMDiveSyncData diveSyncData;

	public NKMRaidBossResultData raidBossResultData = new NKMRaidBossResultData();

	public NKMGameRecord gameRecord;

	public List<UnitLoyaltyUpdateData> updatedUnits = new List<UnitLoyaltyUpdateData>();

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	public NKMStagePlayData stagePlayData = new NKMStagePlayData();

	public NKMShadowGameResult shadowGameResult = new NKMShadowGameResult();

	public NKMFierceResultData fierceResultData = new NKMFierceResultData();

	public PhaseModeState phaseModeState;

	public long killCountDelta;

	public NKMKillCountData killCountData;

	public TrimModeState trimModeState;

	public float totalPlayTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref win);
		stream.PutOrGet(ref giveup);
		stream.PutOrGet(ref restart);
		stream.PutOrGet(ref dungeonClearData);
		stream.PutOrGet(ref phaseClearData);
		stream.PutOrGet(ref episodeCompleteData);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref warfareSyncData);
		stream.PutOrGet(ref pvpResultData);
		stream.PutOrGet(ref diveSyncData);
		stream.PutOrGet(ref raidBossResultData);
		stream.PutOrGet(ref gameRecord);
		stream.PutOrGet(ref updatedUnits);
		stream.PutOrGet(ref costItemDataList);
		stream.PutOrGet(ref stagePlayData);
		stream.PutOrGet(ref shadowGameResult);
		stream.PutOrGet(ref fierceResultData);
		stream.PutOrGet(ref phaseModeState);
		stream.PutOrGet(ref killCountDelta);
		stream.PutOrGet(ref killCountData);
		stream.PutOrGet(ref trimModeState);
		stream.PutOrGet(ref totalPlayTime);
	}
}
