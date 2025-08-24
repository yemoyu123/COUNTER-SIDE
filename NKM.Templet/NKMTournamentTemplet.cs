using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using Cs.Core.Util;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMTournamentTemplet : INKMTemplet, INKMTempletEx
{
	public const int TournamentCastingVoteMaxCount = 3;

	private readonly List<string> battleConditionIds = new List<string>();

	private int index;

	private int tournamentId;

	private string openTag;

	private string tournamentInterval;

	private int mapGroup;

	private string gameStateRateId;

	private bool noshuffleDeck;

	private bool forcedBanEquip;

	private float startCost;

	private int respawnCountMaxSameTime;

	private int rankRewardGroupId;

	private int predictRewardGroupId;

	private bool useUnitBan;

	private int unitBanCount;

	private int shipBanCount;

	private bool unify;

	private List<int> globalBanUnits = new List<int>();

	private string tournamentSeasonDesc;

	private string tournamentTryoutDesc;

	private string musicAssetName;

	private string seasonTitle;

	private string deckEnterInterval;

	private string qualifyInterval;

	private string castingBanInterval;

	private string GroupRoundInterval;

	private string GroupBettingInterval_1;

	private string GroupBettingInterval_2;

	private string GroupBettingInterval_3;

	private string GroupBettingInterval_4;

	private string finalBettingInterval;

	private string finalRoundInterval;

	private string rewardInterval;

	private NKMIntervalTemplet intervalTemplet;

	private NKMTournamentPredictRewardGroupTemplet predictRewardGroupTemplet;

	private NKMIntervalTemplet castingBanIntervalTemplet;

	private NKMIntervalTemplet DeckEnterIntervalTemplet;

	private NKMIntervalTemplet QualifyIntervalTemplet;

	private NKMIntervalTemplet GroupRoundIntervalTemplet;

	private NKMIntervalTemplet GroupBettingIntervalTemplet_1;

	private NKMIntervalTemplet GroupBettingIntervalTemplet_2;

	private NKMIntervalTemplet GroupBettingIntervalTemplet_3;

	private NKMIntervalTemplet GroupBettingIntervalTemplet_4;

	private NKMIntervalTemplet FinalRoundIntervalTemplet;

	private NKMIntervalTemplet FinalBettingIntervalTemplet;

	private NKMIntervalTemplet RewardIntervalTemplet;

	public List<NKMBattleConditionTemplet> BattleConditionTemplets = new List<NKMBattleConditionTemplet>();

	public NKMDeckCondition m_DeckCondition;

	public int MapGroupId => mapGroup;

	public string GameStatRateID => gameStateRateId;

	public bool NoShuffleDeck => noshuffleDeck;

	public bool ForcedBanEquip => forcedBanEquip;

	public int RespawnCountMaxSameTime => respawnCountMaxSameTime;

	public float StartCost => startCost;

	public bool UseCastingBan => useUnitBan;

	public NKMTournamentPredictRewardGroupTemplet PredictRewardGroupTemplet => predictRewardGroupTemplet;

	public IEnumerable<NKMTournamentRankRewardTemplet> rankRewardTemplets { get; private set; }

	public static IEnumerable<NKMTournamentTemplet> Values => NKMTempletContainer<NKMTournamentTemplet>.Values;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public int Key => tournamentId;

	public NKMIntervalTemplet IntervalTemplet => intervalTemplet;

	public DateTime GetQualifyStartTime => QualifyIntervalTemplet.StartDate;

	public DateTime GetDeckEnterEndTime => DeckEnterIntervalTemplet.EndDate;

	public string TournamentInterval => tournamentInterval;

	public NKMIntervalTemplet CastingVoteIntervalTemplet => castingBanIntervalTemplet;

	public IReadOnlyList<int> GlobalBanUnits => globalBanUnits;

	public int UnitBanCount => unitBanCount;

	public int ShipBanCount => shipBanCount;

	public bool IsUnify => unify;

	public static NKMTournamentTemplet Find(int key)
	{
		return NKMTempletContainer<NKMTournamentTemplet>.Find((NKMTournamentTemplet x) => x.tournamentId == key);
	}

	public static NKMTournamentTemplet Find(DateTime current)
	{
		return NKMTempletContainer<NKMTournamentTemplet>.Find((NKMTournamentTemplet x) => x.intervalTemplet.IsValidTime(current));
	}

	public bool IsStartGlobalTryOut(DateTime Current)
	{
		return DeckEnterIntervalTemplet.EndDate.AddHours(2.0) < Current;
	}

	public bool IsEndGlobalTryOut(DateTime Current)
	{
		return DeckEnterIntervalTemplet.EndDate.AddMinutes(30.0) < Current;
	}

	public static NKMTournamentTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 105))
		{
			return null;
		}
		bool flag = true;
		NKMTournamentTemplet nKMTournamentTemplet = new NKMTournamentTemplet();
		flag &= lua.GetData("INDEX", ref nKMTournamentTemplet.index);
		flag &= lua.GetData("TournamentID", ref nKMTournamentTemplet.tournamentId);
		lua.GetData("OpenTag", ref nKMTournamentTemplet.openTag);
		flag &= lua.GetData("TournamentInterval", ref nKMTournamentTemplet.tournamentInterval);
		flag &= lua.GetData("DeckEnterInterval", ref nKMTournamentTemplet.deckEnterInterval);
		flag &= lua.GetData("QualifyInterval", ref nKMTournamentTemplet.qualifyInterval);
		flag &= lua.GetData("GroupRoundInterval", ref nKMTournamentTemplet.GroupRoundInterval);
		flag &= lua.GetData("GroupBettingInterval_01", ref nKMTournamentTemplet.GroupBettingInterval_1);
		flag &= lua.GetData("GroupBettingInterval_02", ref nKMTournamentTemplet.GroupBettingInterval_2);
		flag &= lua.GetData("GroupBettingInterval_03", ref nKMTournamentTemplet.GroupBettingInterval_3);
		flag &= lua.GetData("GroupBettingInterval_04", ref nKMTournamentTemplet.GroupBettingInterval_4);
		flag &= lua.GetData("FinalRoundInterval", ref nKMTournamentTemplet.finalRoundInterval);
		flag &= lua.GetData("FinalBettingInterval", ref nKMTournamentTemplet.finalBettingInterval);
		flag &= lua.GetData("RewardInterval", ref nKMTournamentTemplet.rewardInterval);
		flag &= lua.GetData("MapGroupId", ref nKMTournamentTemplet.mapGroup);
		flag &= lua.GetData("GameStateRateID", ref nKMTournamentTemplet.gameStateRateId);
		lua.GetData("bUnitBan", ref nKMTournamentTemplet.useUnitBan);
		if (nKMTournamentTemplet.useUnitBan)
		{
			flag &= lua.GetData("CastingBanInterval", ref nKMTournamentTemplet.castingBanInterval);
			flag &= lua.GetData("UnitBanCount", ref nKMTournamentTemplet.unitBanCount);
			flag &= lua.GetData("ShipBanCount", ref nKMTournamentTemplet.shipBanCount);
		}
		lua.GetDataList("BattleConditionID", out List<string> result, nullIfEmpty: false);
		if (result != null)
		{
			nKMTournamentTemplet.battleConditionIds.AddRange(result);
		}
		NKMDeckCondition.LoadFromLua(lua, "DECK_CONDITION", out nKMTournamentTemplet.m_DeckCondition, $"NKMTournamentTemplet DeckCondition Parse Fail. id : {nKMTournamentTemplet.tournamentId}");
		lua.GetData("bNoShuffleDeck", ref nKMTournamentTemplet.noshuffleDeck);
		lua.GetData("bForcedBanEquip", ref nKMTournamentTemplet.forcedBanEquip);
		lua.GetData("fStartCost", ref nKMTournamentTemplet.startCost);
		lua.GetData("RespawnCountMaxSameTime", ref nKMTournamentTemplet.respawnCountMaxSameTime);
		lua.GetData("ResultRewardGroupID", ref nKMTournamentTemplet.rankRewardGroupId);
		lua.GetData("PredictRewardGroupID", ref nKMTournamentTemplet.predictRewardGroupId);
		lua.GetData("SeasonDesc", ref nKMTournamentTemplet.tournamentSeasonDesc);
		lua.GetData("QualifyDesc", ref nKMTournamentTemplet.tournamentTryoutDesc);
		lua.GetData("MusicAssetName", ref nKMTournamentTemplet.musicAssetName);
		lua.GetData("SeasonTitle", ref nKMTournamentTemplet.seasonTitle);
		lua.GetData("bUnify", ref nKMTournamentTemplet.unify);
		lua.GetDataList("GlobalBanUnit", out nKMTournamentTemplet.globalBanUnits, nullIfEmpty: false);
		if (!flag)
		{
			return null;
		}
		return nKMTournamentTemplet;
	}

	public static NKM_ERROR_CODE VaildateTournamentSlots(NKMTournamentGroups group, List<long> slotDatas)
	{
		if (group == NKMTournamentGroups.None)
		{
			return NKM_ERROR_CODE.NEC_FAIL_TOURNAMENT_INVALID_GROUP;
		}
		int num = 0;
		num = ((group != NKMTournamentGroups.Finals && group != NKMTournamentGroups.GlobalFinals) ? ((int)Math.Pow(2.0, 4.0) - 1) : ((int)Math.Pow(2.0, 3.0)));
		if (slotDatas.Count != num)
		{
			return NKM_ERROR_CODE.NEC_FAIL_TOURNAMENT_WRONG_SLOT_COUNT;
		}
		if (group == NKMTournamentGroups.Finals || group == NKMTournamentGroups.GlobalFinals)
		{
			List<long> list = new List<long>();
			list.AddRange(slotDatas.GetRange(0, num - 1));
			_ = slotDatas[num - 1];
			for (int i = 0; i < list.Count; i++)
			{
				if (!ValidateSlot(list, i))
				{
					return NKM_ERROR_CODE.NEC_FAIL_TOURNAMENT_INVALID_INDEX;
				}
			}
		}
		else
		{
			for (int j = 0; j < slotDatas.Count; j++)
			{
				if (!ValidateSlot(slotDatas, j))
				{
					return NKM_ERROR_CODE.NEC_FAIL_TOURNAMENT_INVALID_INDEX;
				}
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static bool ValidateSlot(List<long> slotDatas, int index)
	{
		int num = index * 2 + 1;
		int num2 = index * 2 + 2;
		if (num > slotDatas.Count - 1 || num2 > slotDatas.Count - 1)
		{
			return true;
		}
		if (slotDatas[index] != slotDatas[num])
		{
			return slotDatas[index] == slotDatas[num2];
		}
		return true;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
			JoinRewardTemplet();
		}
		foreach (string battleConditionId in battleConditionIds)
		{
			NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(battleConditionId);
			if (templetByStrID != null)
			{
				BattleConditionTemplets.Add(templetByStrID);
			}
		}
	}

	public void JoinRewardTemplet()
	{
		predictRewardGroupTemplet = NKMTournamentPredictRewardGroupTemplet.Find(predictRewardGroupId);
		if (predictRewardGroupTemplet == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] PredictRewardGroupID을 찾을 수 없음. tournamentId:{tournamentId} predictRewardGroupId:{predictRewardGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 266);
		}
		List<NKMTournamentRankRewardTemplet> source = NKMTournamentRankRewardTemplet.Values.ToList().FindAll((NKMTournamentRankRewardTemplet e) => e.RankRewardGroupId == rankRewardGroupId);
		rankRewardTemplets = source.OrderBy((NKMTournamentRankRewardTemplet e) => e.RankValue);
	}

	public void Validate()
	{
		if (NKMUtil.IsServer)
		{
			ValidateIntervalTemplet();
		}
		if (!useUnitBan)
		{
			return;
		}
		if (unitBanCount <= 0)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 토너먼트 유닛 밴 카운트가 0 보다 작음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 284);
		}
		if (shipBanCount <= 0)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 토너먼트 함선 밴 카운트가 0 보다 작음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 289);
		}
		if (unitBanCount < NKMCommonConst.TournamentBanHighUnitCount)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 지정되는 유닛 밴 카운트가 최댓값을 넘음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 294);
		}
		if (shipBanCount < NKMCommonConst.TournamentBanHighShipCount)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 지정되는 함선 밴 카운트가 최댓값을 넘음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 299);
		}
		if (globalBanUnits.Count <= 0)
		{
			return;
		}
		foreach (int globalBanUnit in globalBanUnits)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(globalBanUnit);
			if (nKMUnitTempletBase == null)
			{
				NKMTempletError.Add($"[NKMTournamentTemplet] GlobalBan Unit의 id가 비정상:{globalBanUnit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 309);
			}
			else if (!nKMUnitTempletBase.PickupEnableByTag)
			{
				NKMTempletError.Add($"[NKMTournamentTemplet] GlobalBan Unit의 m_FirstOpenTag가 비활성화된 상태:{globalBanUnit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 315);
			}
		}
	}

	public NKMTournamentState GetState(DateTime current)
	{
		if (useUnitBan)
		{
			NKMIntervalTemplet castingVoteIntervalTemplet = CastingVoteIntervalTemplet;
			if (castingVoteIntervalTemplet != null && castingVoteIntervalTemplet.IsValidTime(current))
			{
				return NKMTournamentState.BanVote;
			}
		}
		if (DeckEnterIntervalTemplet.IsValidTime(current))
		{
			return NKMTournamentState.PreBooking;
		}
		if (QualifyIntervalTemplet.IsValidTime(current))
		{
			return NKMTournamentState.Tryout;
		}
		if (GroupRoundIntervalTemplet.IsValidTime(current))
		{
			return NKMTournamentState.Final32;
		}
		if (FinalRoundIntervalTemplet.IsValidTime(current))
		{
			return NKMTournamentState.Final4;
		}
		if (RewardIntervalTemplet.IsValidTime(current))
		{
			return NKMTournamentState.Closing;
		}
		if (intervalTemplet.IsValidTime(current))
		{
			return NKMTournamentState.Progressing;
		}
		return NKMTournamentState.Ended;
	}

	public NKMTournamentState GetPrevState(DateTime current)
	{
		NKMTournamentState state = GetState(current);
		if (state != NKMTournamentState.Progressing)
		{
			return state;
		}
		if (useUnitBan && current < CastingVoteIntervalTemplet.StartDate)
		{
			return NKMTournamentState.Ended;
		}
		if (current < DeckEnterIntervalTemplet.StartDate)
		{
			return NKMTournamentState.BanVote;
		}
		if (current < QualifyIntervalTemplet.StartDate)
		{
			return NKMTournamentState.PreBooking;
		}
		if (current < GroupRoundIntervalTemplet.StartDate)
		{
			return NKMTournamentState.Tryout;
		}
		if (current < FinalRoundIntervalTemplet.StartDate)
		{
			return NKMTournamentState.Final32;
		}
		if (current < RewardIntervalTemplet.StartDate)
		{
			return NKMTournamentState.Final4;
		}
		return NKMTournamentState.Ended;
	}

	public NKMTournamentState GetPrevState2(DateTime current)
	{
		NKMTournamentState state = GetState(current);
		if (state != NKMTournamentState.Progressing)
		{
			return state;
		}
		if (useUnitBan && current < CastingVoteIntervalTemplet.StartDate)
		{
			return NKMTournamentState.Ended;
		}
		if (current < DeckEnterIntervalTemplet.StartDate)
		{
			if (!useUnitBan)
			{
				return NKMTournamentState.Ended;
			}
			return NKMTournamentState.BanVote;
		}
		if (current < QualifyIntervalTemplet.StartDate)
		{
			return NKMTournamentState.Tryout;
		}
		if (current < GroupRoundIntervalTemplet.StartDate)
		{
			return NKMTournamentState.Final32;
		}
		if (current < FinalRoundIntervalTemplet.StartDate)
		{
			return NKMTournamentState.Final4;
		}
		if (current < RewardIntervalTemplet.StartDate)
		{
			return NKMTournamentState.Final4;
		}
		return NKMTournamentState.Ended;
	}

	public bool isEndBettingTime(NKMTournamentGroups group, DateTime current)
	{
		switch (group)
		{
		case NKMTournamentGroups.Finals:
		case NKMTournamentGroups.GlobalFinals:
			if (FinalBettingIntervalTemplet.IsValidTime(current))
			{
				return true;
			}
			break;
		case NKMTournamentGroups.GroupA:
		case NKMTournamentGroups.GlobalGroupA:
			if (GroupBettingIntervalTemplet_1.IsValidTime(current))
			{
				return true;
			}
			break;
		case NKMTournamentGroups.GroupB:
		case NKMTournamentGroups.GlobalGroupB:
			if (GroupBettingIntervalTemplet_2.IsValidTime(current))
			{
				return true;
			}
			break;
		case NKMTournamentGroups.GroupC:
		case NKMTournamentGroups.GlobalGroupC:
			if (GroupBettingIntervalTemplet_3.IsValidTime(current))
			{
				return true;
			}
			break;
		case NKMTournamentGroups.GroupD:
		case NKMTournamentGroups.GlobalGroupD:
			if (GroupBettingIntervalTemplet_4.IsValidTime(current))
			{
				return true;
			}
			break;
		}
		return false;
	}

	public bool isBlindTime(NKMTournamentGroups group, DateTime current)
	{
		switch (group)
		{
		case NKMTournamentGroups.Finals:
		case NKMTournamentGroups.GlobalFinals:
			if (current < FinalBettingIntervalTemplet.EndDate)
			{
				return true;
			}
			break;
		case NKMTournamentGroups.GroupA:
		case NKMTournamentGroups.GlobalGroupA:
			if (current < GroupBettingIntervalTemplet_1.EndDate)
			{
				return true;
			}
			break;
		case NKMTournamentGroups.GroupB:
		case NKMTournamentGroups.GlobalGroupB:
			if (current < GroupBettingIntervalTemplet_2.EndDate)
			{
				return true;
			}
			break;
		case NKMTournamentGroups.GroupC:
		case NKMTournamentGroups.GlobalGroupC:
			if (current < GroupBettingIntervalTemplet_3.EndDate)
			{
				return true;
			}
			break;
		case NKMTournamentGroups.GroupD:
		case NKMTournamentGroups.GlobalGroupD:
			if (current < GroupBettingIntervalTemplet_4.EndDate)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private void JoinIntervalTemplet()
	{
		intervalTemplet = NKMIntervalTemplet.Find(tournamentInterval);
		if (intervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] interval templet을 찾을 수 없음. tournamentId:{tournamentId} dateStrId:{tournamentInterval}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 596);
		}
		DeckEnterIntervalTemplet = NKMIntervalTemplet.Find(deckEnterInterval);
		if (DeckEnterIntervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] DeckEnterIntervalTemplet templet을 찾을 수 없음. tournamentId:{tournamentId} deckEnterIntervalStrId:{deckEnterInterval}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 602);
		}
		QualifyIntervalTemplet = NKMIntervalTemplet.Find(qualifyInterval);
		if (QualifyIntervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] QualifyIntervalTemplet templet을 찾을 수 없음. tournamentId:{tournamentId} qualifyIntervalStrId:{qualifyInterval}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 608);
		}
		GroupRoundIntervalTemplet = NKMIntervalTemplet.Find(GroupRoundInterval);
		if (GroupRoundIntervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] GroupRoundIntervalTemplet templet을 찾을 수 없음. tournamentId:{tournamentId} groupRoundIntervalStrId:{GroupRoundInterval}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 614);
		}
		GroupBettingIntervalTemplet_1 = NKMIntervalTemplet.Find(GroupBettingInterval_1);
		if (GroupBettingIntervalTemplet_1 == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] GroupBettingIntervalTemplet_1 templet을 찾을 수 없음. tournamentId:{tournamentId} groupBettingIntervalStrId:{GroupBettingInterval_1}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 620);
		}
		GroupBettingIntervalTemplet_2 = NKMIntervalTemplet.Find(GroupBettingInterval_2);
		if (GroupBettingIntervalTemplet_2 == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] GroupBettingIntervalTemplet_2 templet을 찾을 수 없음. tournamentId:{tournamentId} groupBettingIntervalStrId:{GroupBettingInterval_2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 626);
		}
		GroupBettingIntervalTemplet_3 = NKMIntervalTemplet.Find(GroupBettingInterval_3);
		if (GroupBettingIntervalTemplet_3 == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] GroupBettingIntervalTemplet_3 templet을 찾을 수 없음. tournamentId:{tournamentId} groupBettingIntervalStrId:{GroupBettingInterval_3}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 632);
		}
		GroupBettingIntervalTemplet_4 = NKMIntervalTemplet.Find(GroupBettingInterval_4);
		if (GroupBettingIntervalTemplet_4 == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] GroupBettingIntervalTemplet_4 templet을 찾을 수 없음. tournamentId:{tournamentId} groupBettingIntervalStrId:{GroupBettingInterval_4}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 638);
		}
		FinalBettingIntervalTemplet = NKMIntervalTemplet.Find(finalBettingInterval);
		if (FinalBettingIntervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] FinalBettingIntervalTemplet templet을 찾을 수 없음. tournamentId:{tournamentId} finalBettingIntervalStrId:{finalBettingInterval}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 644);
		}
		FinalRoundIntervalTemplet = NKMIntervalTemplet.Find(finalRoundInterval);
		if (FinalRoundIntervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] FinalRoundIntervalTemplet templet을 찾을 수 없음. tournamentId:{tournamentId} finalRoundIntervalStrId:{finalRoundInterval}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 650);
		}
		RewardIntervalTemplet = NKMIntervalTemplet.Find(rewardInterval);
		if (RewardIntervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMTournamentTemplet] RewardIntervalTemplet templet을 찾을 수 없음. tournamentId:{tournamentId} rewardIntervalStrId:{rewardInterval}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 656);
		}
		if (useUnitBan)
		{
			castingBanIntervalTemplet = NKMIntervalTemplet.Find(castingBanInterval);
			if (castingBanIntervalTemplet == null)
			{
				NKMTempletError.Add($"[NKMTournamentTemplet] CastingBanIntervalTemplet templet을 찾을 수 없음. tournamentId:{tournamentId} deckEnterIntervalStrId:{castingBanInterval}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 664);
			}
		}
	}

	private void ValidateIntervalTemplet()
	{
		if (useUnitBan && DeckEnterIntervalTemplet.StartDate <= castingBanIntervalTemplet.EndDate)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 토너먼트 덱 등록 기간이 캐스팅 밴을 지정하는 시간보다 빠름", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 677);
		}
		if (QualifyIntervalTemplet.StartDate <= DeckEnterIntervalTemplet.EndDate)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 토너먼트 덱 등록 종료 시간보다 예선전 시작 시간이 빠름.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 683);
		}
		if (GroupRoundIntervalTemplet.StartDate <= QualifyIntervalTemplet.EndDate)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 토너먼트 예선전 종료 시간보다 32강 시작 시간이 빠름.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 688);
		}
		if (FinalBettingIntervalTemplet.StartDate <= GroupRoundIntervalTemplet.EndDate)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 토너먼트 32강 종료 시간보다 4강 시작 시간이 빠름.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 693);
		}
		if (RewardIntervalTemplet.StartDate <= FinalBettingIntervalTemplet.EndDate)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 토너먼트 4강 종료 시간보다 보상 지급 시작 시간이 빠름.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 698);
		}
		if (GroupRoundIntervalTemplet.StartDate > GroupBettingIntervalTemplet_1.StartDate || GroupRoundIntervalTemplet.StartDate > GroupBettingIntervalTemplet_2.StartDate || GroupRoundIntervalTemplet.StartDate > GroupBettingIntervalTemplet_3.StartDate || GroupRoundIntervalTemplet.StartDate > GroupBettingIntervalTemplet_4.StartDate)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 각 조 응원하기 시작 시간 중 토너먼트 32강 시작 시간 이전인 경우가 존재.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 708);
		}
		if (GroupRoundIntervalTemplet.EndDate <= GroupBettingIntervalTemplet_1.EndDate || GroupRoundIntervalTemplet.EndDate <= GroupBettingIntervalTemplet_2.EndDate || GroupRoundIntervalTemplet.EndDate <= GroupBettingIntervalTemplet_3.EndDate || GroupRoundIntervalTemplet.EndDate <= GroupBettingIntervalTemplet_4.EndDate)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 토너먼트 각 조 응원하기 종료 시간보다 32강 종료 시간이 빠른 경우가 존재.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 717);
		}
		if (FinalBettingIntervalTemplet.StartDate < FinalRoundIntervalTemplet.StartDate)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 토너먼트 4강 응원하기 시작 시간이 토너먼트 4강 시작 이전으로 설정.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 723);
		}
		if (FinalRoundIntervalTemplet.EndDate <= FinalBettingIntervalTemplet.EndDate)
		{
			NKMTempletError.Add("[NKMTournamentTemplet] 토너먼트 4강 응원하기 종료 시간보다 4강 종료 시간이 빠름.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentTemplet.cs", 728);
		}
	}

	public void PostJoin()
	{
		Join();
		JoinIntervalTemplet();
		JoinRewardTemplet();
	}

	public string GetTournamentSeasonTitle()
	{
		return NKCStringTable.GetString(seasonTitle);
	}

	public string GetTournamentSeasonDesc()
	{
		return NKCStringTable.GetString(tournamentSeasonDesc);
	}

	public string GetTournamentTryouyDesc()
	{
		return NKCStringTable.GetString(tournamentTryoutDesc);
	}

	public string GetMusicAssetName()
	{
		return musicAssetName;
	}

	public bool CanEnterDeckApply(DateTime serviceTime)
	{
		return DeckEnterIntervalTemplet.IsValidTime(serviceTime);
	}

	public bool CanEnterTournamentLobby(DateTime serviceTime)
	{
		return GroupRoundIntervalTemplet.GetStartDate() <= serviceTime;
	}

	public DateTime GetTournamentStateStartDate(NKMTournamentState state)
	{
		switch (state)
		{
		case NKMTournamentState.BanVote:
			return CastingVoteIntervalTemplet.GetStartDate();
		case NKMTournamentState.PreBooking:
			return DeckEnterIntervalTemplet.GetStartDate();
		case NKMTournamentState.Tryout:
			return QualifyIntervalTemplet.GetStartDate();
		case NKMTournamentState.Final32:
			return GroupRoundIntervalTemplet.GetStartDate();
		case NKMTournamentState.Final4:
			return FinalRoundIntervalTemplet.GetStartDate();
		case NKMTournamentState.Closing:
			return RewardIntervalTemplet.GetStartDate();
		case NKMTournamentState.Progressing:
			if (ServiceTime.Now < GetTournamentStateStartDate(NKMTournamentState.Tryout))
			{
				return GetTournamentStateEndDate(NKMTournamentState.PreBooking);
			}
			if (ServiceTime.Now < GetTournamentStateStartDate(NKMTournamentState.Final32))
			{
				return GetTournamentStateEndDate(NKMTournamentState.Tryout);
			}
			if (ServiceTime.Now < GetTournamentStateStartDate(NKMTournamentState.Final4))
			{
				return GetTournamentStateEndDate(NKMTournamentState.Final32);
			}
			if (ServiceTime.Now < GetTournamentStateStartDate(NKMTournamentState.Closing))
			{
				return GetTournamentStateEndDate(NKMTournamentState.Final4);
			}
			Log.Error($"CurState : {state}, Time : ${ServiceTime.Now}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/Templet/NKMTournamentTempletEx.cs", 85);
			return default(DateTime);
		default:
			Log.Error($"CurState : {state}, Time : ${ServiceTime.Now}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/Templet/NKMTournamentTempletEx.cs", 91);
			return default(DateTime);
		}
	}

	public DateTime GetTournamentStateEndDate(NKMTournamentState state)
	{
		switch (state)
		{
		case NKMTournamentState.BanVote:
			return CastingVoteIntervalTemplet.GetEndDate();
		case NKMTournamentState.PreBooking:
			return DeckEnterIntervalTemplet.GetEndDate();
		case NKMTournamentState.Tryout:
			return QualifyIntervalTemplet.GetEndDate();
		case NKMTournamentState.Final32:
			return GroupRoundIntervalTemplet.GetEndDate();
		case NKMTournamentState.Final4:
			return FinalRoundIntervalTemplet.GetEndDate();
		case NKMTournamentState.Closing:
			return RewardIntervalTemplet.GetEndDate();
		case NKMTournamentState.Progressing:
			if (ServiceTime.Now < GetTournamentStateStartDate(NKMTournamentState.Tryout))
			{
				return GetTournamentStateStartDate(NKMTournamentState.Tryout);
			}
			if (ServiceTime.Now < GetTournamentStateStartDate(NKMTournamentState.Final32))
			{
				return GetTournamentStateStartDate(NKMTournamentState.Final32);
			}
			if (ServiceTime.Now < GetTournamentStateStartDate(NKMTournamentState.Final4))
			{
				return GetTournamentStateStartDate(NKMTournamentState.Final4);
			}
			if (ServiceTime.Now < GetTournamentStateStartDate(NKMTournamentState.Closing))
			{
				return GetTournamentStateStartDate(NKMTournamentState.Closing);
			}
			Log.Error($"CurState : {state}, Time : ${ServiceTime.Now}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/Templet/NKMTournamentTempletEx.cs", 130);
			return default(DateTime);
		default:
			Log.Error($"CurState : {state}, Time : ${ServiceTime.Now}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/Templet/NKMTournamentTempletEx.cs", 136);
			return default(DateTime);
		}
	}

	public string GetGroupRoundTimeIntervalString()
	{
		return NKCUtilString.GetTimeIntervalString(GroupRoundIntervalTemplet.GetStartDate(), GroupRoundIntervalTemplet.GetEndDate(), NKMTime.INTERVAL_FROM_UTC);
	}

	public string GetFinalRouneTimeIntervalString()
	{
		return NKCUtilString.GetTimeIntervalString(FinalRoundIntervalTemplet.GetStartDate(), FinalRoundIntervalTemplet.GetEndDate(), NKMTime.INTERVAL_FROM_UTC);
	}

	public string GetGroupCheeringRemainTimeString(NKMTournamentGroups group)
	{
		if (group == NKMTournamentGroups.None)
		{
			return "";
		}
		return NKCUtilString.GetTimeString(NKCSynchronizedTime.ToUtcTime(GetGroupCheeringEndTime(group)), bSeconds: false);
	}

	public DateTime GetGroupCheeringStartTime(NKMTournamentGroups group)
	{
		switch (group)
		{
		case NKMTournamentGroups.GroupA:
		case NKMTournamentGroups.GlobalGroupA:
			return GroupBettingIntervalTemplet_1.GetStartDate();
		case NKMTournamentGroups.GroupB:
		case NKMTournamentGroups.GlobalGroupB:
			return GroupBettingIntervalTemplet_2.GetStartDate();
		case NKMTournamentGroups.GroupC:
		case NKMTournamentGroups.GlobalGroupC:
			return GroupBettingIntervalTemplet_3.GetStartDate();
		case NKMTournamentGroups.GroupD:
		case NKMTournamentGroups.GlobalGroupD:
			return GroupBettingIntervalTemplet_4.GetStartDate();
		case NKMTournamentGroups.Finals:
		case NKMTournamentGroups.GlobalFinals:
			return FinalBettingIntervalTemplet.GetStartDate();
		default:
			return default(DateTime);
		}
	}

	public DateTime GetGroupCheeringEndTime(NKMTournamentGroups group)
	{
		switch (group)
		{
		case NKMTournamentGroups.GroupA:
		case NKMTournamentGroups.GlobalGroupA:
			return GroupBettingIntervalTemplet_1.GetEndDate();
		case NKMTournamentGroups.GroupB:
		case NKMTournamentGroups.GlobalGroupB:
			return GroupBettingIntervalTemplet_2.GetEndDate();
		case NKMTournamentGroups.GroupC:
		case NKMTournamentGroups.GlobalGroupC:
			return GroupBettingIntervalTemplet_3.GetEndDate();
		case NKMTournamentGroups.GroupD:
		case NKMTournamentGroups.GlobalGroupD:
			return GroupBettingIntervalTemplet_4.GetEndDate();
		case NKMTournamentGroups.Finals:
		case NKMTournamentGroups.GlobalFinals:
			return FinalBettingIntervalTemplet.GetEndDate();
		default:
			return default(DateTime);
		}
	}

	public bool IsGroupCheeringTime(NKMTournamentGroups group)
	{
		if (group == NKMTournamentGroups.None)
		{
			return false;
		}
		DateTime startTimeUTC = NKCSynchronizedTime.ToUtcTime(GetGroupCheeringStartTime(group));
		DateTime finishTimeUTC = NKCSynchronizedTime.ToUtcTime(GetGroupCheeringEndTime(group));
		return NKCSynchronizedTime.IsEventTime(startTimeUTC, finishTimeUTC);
	}
}
