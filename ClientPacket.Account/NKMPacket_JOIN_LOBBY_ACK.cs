using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Contract;
using ClientPacket.Event;
using ClientPacket.Lobby;
using ClientPacket.Mode;
using ClientPacket.Office;
using ClientPacket.Pvp;
using ClientPacket.Shop;
using ClientPacket.Unit;
using ClientPacket.User;
using ClientPacket.Warfare;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_JOIN_LOBBY_ACK)]
public sealed class NKMPacket_JOIN_LOBBY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long friendCode;

	public NKMUserData userData;

	public NKMLobbyData lobbyData = new NKMLobbyData();

	public NKMGameData gameData;

	public WarfareGameData warfareGameData = new WarfareGameData();

	public DateTime utcTime;

	public TimeSpan utcOffset;

	public DateTime lastCreditSupplyTakeTime;

	public DateTime lastEterniumSupplyTakeTime;

	public double totalPaidAmount;

	public List<ShopChainTabNextResetData> shopChainTabNestResetList = new List<ShopChainTabNextResetData>();

	public NKMPvpBanResult pvpBanResult = new NKMPvpBanResult();

	public PvpState asyncPvpState;

	public PvpState leaguePvpState;

	public DateTime pvpPointChargeTime;

	public bool rankPvpOpen;

	public bool leaguePvpOpen;

	public List<NKMReturningUserState> ReturningUserStates = new List<NKMReturningUserState>();

	public List<NKMContractState> contractState = new List<NKMContractState>();

	public List<NKMContractBonusState> contractBonusState = new List<NKMContractBonusState>();

	public NKMSelectableContractState selectableContractState = new NKMSelectableContractState();

	public List<NKMStagePlayData> stagePlayDataList = new List<NKMStagePlayData>();

	public EventInfo eventInfo = new EventInfo();

	public string reconnectKey;

	public ZlongUserData zlongUserData = new ZlongUserData();

	public NKMBackgroundInfo backGroundInfo = new NKMBackgroundInfo();

	public PrivateGuildData privateGuildData = new PrivateGuildData();

	public DateTime blockMuteEndDate;

	public bool marketReviewCompletion;

	public bool fierceDailyRewardReceived;

	public GuildDungeonRewardInfo guildDungeonRewardInfo = new GuildDungeonRewardInfo();

	public NKMEquipTuningCandidate equipTuningCandidate = new NKMEquipTuningCandidate();

	public DraftPvpRoomData leaguePvpRoomData;

	public List<PvpSingleHistory> leaguePvpHistories = new List<PvpSingleHistory>();

	public List<PvpSingleHistory> privatePvpHistories = new List<PvpSingleHistory>();

	public NKMMyOfficeState officeState;

	public KakaoMissionData kakaoMissionData;

	public List<int> unlockedStageIds = new List<int>();

	public List<NKMPhaseClearData> phaseClearDataList = new List<NKMPhaseClearData>();

	public PhaseModeState phaseModeState;

	public List<NKMServerKillCountData> serverKillCountDataList = new List<NKMServerKillCountData>();

	public List<NKMKillCountData> killCountDataList = new List<NKMKillCountData>();

	public List<NKMUnitMissionData> completedUnitMissions = new List<NKMUnitMissionData>();

	public List<NKMUnitMissionData> rewardEnableUnitMissions = new List<NKMUnitMissionData>();

	public PvpCastingVoteData pvpCastingVoteData = new PvpCastingVoteData();

	public List<NKMIntervalData> intervalData = new List<NKMIntervalData>();

	public List<NKMConsumerPackageData> consumerPackages = new List<NKMConsumerPackageData>();

	public NpcPvpData npcPvpData;

	public NKMTrimIntervalData trimIntervalData = new NKMTrimIntervalData();

	public List<NKMTrimClearData> trimClearList = new List<NKMTrimClearData>();

	public NKMShipModuleCandidate shipSlotCandidate = new NKMShipModuleCandidate();

	public TrimModeState trimModeState;

	public bool enableAccountLink;

	public NKMEventCollectionInfo eventCollectionInfo = new NKMEventCollectionInfo();

	public NKMUserProfileData userProfileData = new NKMUserProfileData();

	public NKMShortCutInfo lastPlayInfo = new NKMShortCutInfo();

	public PvpState eventPvpState;

	public List<NKMCustomPickupContract> customPickupContracts = new List<NKMCustomPickupContract>();

	public NKMPotentialOptionChangeCandidate potentialOptionCandidate = new NKMPotentialOptionChangeCandidate();

	public PvpCastingVoteData pvpDraftVoteData = new PvpCastingVoteData();

	public NKMSupportUnitData supportUnitProfileData = new NKMSupportUnitData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref friendCode);
		stream.PutOrGet(ref userData);
		stream.PutOrGet(ref lobbyData);
		stream.PutOrGet(ref gameData);
		stream.PutOrGet(ref warfareGameData);
		stream.PutOrGet(ref utcTime);
		stream.PutOrGet(ref utcOffset);
		stream.PutOrGet(ref lastCreditSupplyTakeTime);
		stream.PutOrGet(ref lastEterniumSupplyTakeTime);
		stream.PutOrGet(ref totalPaidAmount);
		stream.PutOrGet(ref shopChainTabNestResetList);
		stream.PutOrGet(ref pvpBanResult);
		stream.PutOrGet(ref asyncPvpState);
		stream.PutOrGet(ref leaguePvpState);
		stream.PutOrGet(ref pvpPointChargeTime);
		stream.PutOrGet(ref rankPvpOpen);
		stream.PutOrGet(ref leaguePvpOpen);
		stream.PutOrGet(ref ReturningUserStates);
		stream.PutOrGet(ref contractState);
		stream.PutOrGet(ref contractBonusState);
		stream.PutOrGet(ref selectableContractState);
		stream.PutOrGet(ref stagePlayDataList);
		stream.PutOrGet(ref eventInfo);
		stream.PutOrGet(ref reconnectKey);
		stream.PutOrGet(ref zlongUserData);
		stream.PutOrGet(ref backGroundInfo);
		stream.PutOrGet(ref privateGuildData);
		stream.PutOrGet(ref blockMuteEndDate);
		stream.PutOrGet(ref marketReviewCompletion);
		stream.PutOrGet(ref fierceDailyRewardReceived);
		stream.PutOrGet(ref guildDungeonRewardInfo);
		stream.PutOrGet(ref equipTuningCandidate);
		stream.PutOrGet(ref leaguePvpRoomData);
		stream.PutOrGet(ref leaguePvpHistories);
		stream.PutOrGet(ref privatePvpHistories);
		stream.PutOrGet(ref officeState);
		stream.PutOrGet(ref kakaoMissionData);
		stream.PutOrGet(ref unlockedStageIds);
		stream.PutOrGet(ref phaseClearDataList);
		stream.PutOrGet(ref phaseModeState);
		stream.PutOrGet(ref serverKillCountDataList);
		stream.PutOrGet(ref killCountDataList);
		stream.PutOrGet(ref completedUnitMissions);
		stream.PutOrGet(ref rewardEnableUnitMissions);
		stream.PutOrGet(ref pvpCastingVoteData);
		stream.PutOrGet(ref intervalData);
		stream.PutOrGet(ref consumerPackages);
		stream.PutOrGet(ref npcPvpData);
		stream.PutOrGet(ref trimIntervalData);
		stream.PutOrGet(ref trimClearList);
		stream.PutOrGet(ref shipSlotCandidate);
		stream.PutOrGet(ref trimModeState);
		stream.PutOrGet(ref enableAccountLink);
		stream.PutOrGet(ref eventCollectionInfo);
		stream.PutOrGet(ref userProfileData);
		stream.PutOrGet(ref lastPlayInfo);
		stream.PutOrGet(ref eventPvpState);
		stream.PutOrGet(ref customPickupContracts);
		stream.PutOrGet(ref potentialOptionCandidate);
		stream.PutOrGet(ref pvpDraftVoteData);
		stream.PutOrGet(ref supportUnitProfileData);
	}
}
