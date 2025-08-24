using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Shop;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMPvpCommonConst
{
	public sealed class DraftBanConst
	{
		public int MinUnitCount { get; private set; }

		public int MinShipCount { get; private set; }

		public void LoadFromLua(NKMLua lua)
		{
			using (lua.OpenTable("DraftBan", "DraftBan table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 239))
			{
				MinUnitCount = lua.GetInt32("MinUnitCount");
				MinShipCount = lua.GetInt32("MinShipCount");
			}
		}
	}

	public sealed class LeaguePvpConst
	{
		public readonly struct DeadlineBuffCondition
		{
			public int ReaminTimeSec { get; }

			public int BuffLevel { get; }

			public DeadlineBuffCondition(int remainTimeSec, int buffLevel)
			{
				ReaminTimeSec = remainTimeSec;
				BuffLevel = buffLevel;
			}
		}

		private string rageBuffId;

		private string deadlineBuffId;

		private string uiRageBuffId;

		private string uiDeadlineBuffId;

		private readonly List<DayOfWeek> openDaysOfWeek = new List<DayOfWeek>();

		private readonly List<DeadlineBuffCondition> deadlineBuffConditions = new List<DeadlineBuffCondition>();

		public IReadOnlyList<DayOfWeek> OpenDaysOfWeek => openDaysOfWeek;

		public TimeSpan OpenTimeStart { get; private set; }

		public TimeSpan OpenTimeEnd { get; private set; }

		public float ShipHpMultiply { get; private set; }

		public float ShipAttackPowerMultiply { get; private set; }

		public int TotalGameTimeSec { get; private set; }

		public float RageBuffShipHpRate { get; private set; }

		public NKMBuffTemplet RageBuff { get; private set; }

		public NKMBuffTemplet UiRageBuff { get; private set; }

		public NKMBuffTemplet DeadlineBuff { get; private set; }

		public NKMBuffTemplet UiDeadlineBuff { get; private set; }

		public TimeSpan CalculateTimeStart { get; private set; }

		public int CalculateTimeInterval { get; private set; }

		public bool UserRangeBuff { get; private set; }

		public bool UseDeadlineBuff { get; private set; }

		public bool IsValidTime(DateTime currentSvc)
		{
			if (!openDaysOfWeek.Any((DayOfWeek e) => e == currentSvc.DayOfWeek))
			{
				return false;
			}
			TimeSpan timeOfDay = currentSvc.TimeOfDay;
			if (OpenTimeStart <= timeOfDay)
			{
				return timeOfDay < OpenTimeEnd;
			}
			return false;
		}

		public void LoadFromLua(NKMLua lua)
		{
			if (!lua.OpenTable("LeaguePvp"))
			{
				NKMTempletError.Add("[LeaguePvp] open table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 288);
				return;
			}
			openDaysOfWeek.Clear();
			using (lua.OpenTable("OpenDaysOfWeek", "OpenDaysOfWeek table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 294))
			{
				int num = 1;
				SHOP_RESET_TYPE? result = null;
				while (lua.GetExplicitEnum(num, ref result))
				{
					if (!result.Value.ToDayOfWeek(out var dayOfWeek))
					{
						NKMTempletError.Add($"[LeaguePvp] invalid value:{result.Value}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 302);
						num++;
					}
					else if (openDaysOfWeek.Any((DayOfWeek e) => e == dayOfWeek))
					{
						NKMTempletError.Add($"[LeaguePvp] duplicated value:{result.Value}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 309);
						num++;
					}
					else
					{
						openDaysOfWeek.Add(dayOfWeek);
						num++;
					}
				}
			}
			OpenTimeStart = lua.GetTimeSpan("OpenTimeStart");
			OpenTimeEnd = lua.GetTimeSpan("OpenTimeEnd");
			ShipHpMultiply = lua.GetFloat("ShipHpMultiply");
			ShipAttackPowerMultiply = lua.GetFloat("ShipAttackPowerMultiply");
			TotalGameTimeSec = lua.GetInt32("TotalGameTimeSec");
			RageBuffShipHpRate = lua.GetFloat("RageBuff_ShipHpRate");
			rageBuffId = lua.GetString("RageBuff_Id");
			deadlineBuffId = lua.GetString("DeadlineBuff_Id");
			uiRageBuffId = lua.GetString("UiRageBuff_Id");
			uiDeadlineBuffId = lua.GetString("UiDeadlineBuff_Id");
			UseDeadlineBuff = lua.GetBoolean("UseDeadlineBuff");
			UserRangeBuff = lua.GetBoolean("UseRageBuff");
			CalculateTimeStart = lua.GetTimeSpan("CalculateTimeStart");
			CalculateTimeInterval = lua.GetInt32("CalculateTimeInterval");
			using (lua.OpenTable("DeadlineBuff_Condition", "DeadlineBuff_Condition table open failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 336))
			{
				int num2 = 1;
				while (lua.OpenTable(num2++))
				{
					DeadlineBuffCondition item = new DeadlineBuffCondition(lua.GetInt32("RemainTimeSec"), lua.GetInt32("BuffLevel"));
					deadlineBuffConditions.Add(item);
					lua.CloseTable();
				}
			}
			lua.CloseTable();
		}

		public bool GetDeadlineBuffCondition(float gameRemainTime, out DeadlineBuffCondition result)
		{
			foreach (DeadlineBuffCondition deadlineBuffCondition in deadlineBuffConditions)
			{
				if (gameRemainTime < (float)deadlineBuffCondition.ReaminTimeSec)
				{
					result = deadlineBuffCondition;
					return true;
				}
			}
			result = default(DeadlineBuffCondition);
			return false;
		}

		public float GetDeadlineBuffConditionTimeMax()
		{
			return deadlineBuffConditions.Last().ReaminTimeSec;
		}

		public NKMBuffTemplet GetRageBuff(bool isUnit)
		{
			if (!isUnit)
			{
				return UiRageBuff;
			}
			return RageBuff;
		}

		public NKMBuffTemplet GetDeadlineBuff(bool isUnit)
		{
			if (!isUnit)
			{
				return UiDeadlineBuff;
			}
			return DeadlineBuff;
		}

		public void Join()
		{
			if (!string.IsNullOrEmpty(rageBuffId))
			{
				RageBuff = NKMBuffTemplet.Find(rageBuffId);
				if (RageBuff == null)
				{
					NKMTempletError.Add("[LeaguePvpConst] invalid RageBuff_Id:" + rageBuffId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 389);
				}
			}
			if (!string.IsNullOrEmpty(uiRageBuffId))
			{
				UiRageBuff = NKMBuffTemplet.Find(uiRageBuffId);
				if (UiRageBuff == null)
				{
					NKMTempletError.Add("[LeaguePvpConst] invalid UiRageBuff_Id:" + uiRageBuffId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 398);
				}
			}
			if (!string.IsNullOrEmpty(deadlineBuffId))
			{
				DeadlineBuff = NKMBuffTemplet.Find(deadlineBuffId);
				if (DeadlineBuff == null)
				{
					NKMTempletError.Add("[LeaguePvpConst] invalid DeadlineBuff_Id:" + deadlineBuffId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 407);
				}
			}
			if (!string.IsNullOrEmpty(uiDeadlineBuffId))
			{
				UiDeadlineBuff = NKMBuffTemplet.Find(uiDeadlineBuffId);
				if (UiDeadlineBuff == null)
				{
					NKMTempletError.Add("[LeaguePvpConst] invalid UiDeadlineBuff_Id:" + uiDeadlineBuffId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 416);
				}
			}
			deadlineBuffConditions.Sort((DeadlineBuffCondition a, DeadlineBuffCondition b) => a.ReaminTimeSec.CompareTo(b.ReaminTimeSec));
		}

		public void Validate()
		{
			if (RageBuffShipHpRate <= 0f || RageBuffShipHpRate >= 1f)
			{
				NKMTempletError.Add($"[LeaguePvpConst] invalid RageBuff_ShipHpRate:{RageBuffShipHpRate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 428);
			}
			if (OpenTimeStart >= OpenTimeEnd)
			{
				NKMTempletError.Add($"[LeaguePvp] invalid open time. start:{OpenTimeStart} end:{OpenTimeEnd}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 433);
			}
			if (DeadlineBuff != null && !deadlineBuffConditions.Any())
			{
				NKMTempletError.Add("[LeaguePvp] 데드라인 버프가 설정되었지만 조건 설정이 비어있음. buffId:" + deadlineBuffId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 438);
			}
			if (deadlineBuffConditions.Any() && DeadlineBuff == null)
			{
				NKMTempletError.Add($"[LeaguePvp] 데드라인 버프 조건이 설정되었으나 버프 아이디가 비어있음. #conditions:{deadlineBuffConditions.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 443);
			}
			if (TotalGameTimeSec <= 0 || TotalGameTimeSec >= 3600)
			{
				NKMTempletError.Add($"[LeaguePvp] 게임시간 설정은 1초 ~ 1시간 이내의 값만 허용. TotalGameTimeSec:{TotalGameTimeSec}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 448);
			}
			if (ShipHpMultiply <= 0f || ShipHpMultiply >= 10f)
			{
				NKMTempletError.Add($"[LeaguePvp] 함선 체력 배율 허용범위 벗어남. ShipHpMultiply:{ShipHpMultiply}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 453);
			}
			if (ShipAttackPowerMultiply <= 0f || ShipAttackPowerMultiply >= 10f)
			{
				NKMTempletError.Add($"[LeaguePvp] 함선 공격력 배율 허용범위 벗어남. ShipAttackPowerMultiply:{ShipAttackPowerMultiply}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 458);
			}
		}
	}

	private const string TableName = "Pvp";

	private int leagueResetScore;

	private int scoreMinIntervalUnit;

	private int rankLosePenaltyScore;

	private int defaultRankListMaxCount;

	private int leagueRankListMaxCount;

	private int rankSimpleCount;

	private int unitBanExceptionScore;

	private int rotationUnitExceptionScore;

	private int asyncTicketChargeInterval;

	private int asyncTicketMaxCount;

	private int asyncTicketChargeCount;

	private TimeSpan asyncTicketChargeTimeSpan;

	private int maxHistoryCount = 30;

	private long chargePointRefreshIntervalTicks = 216000000000L;

	private int chargePointMaxCountForPractice = 100;

	private int chargePointMax = 900;

	private int chargePointCount = 225;

	private int asyncPvpWinPoint = 75;

	private int asyncPvpLosePoint = 50;

	private int rankPvpWinPoint = 150;

	private int rankPvpLosePoint = 125;

	private int leaguePvpWinPoint = 150;

	private int leaguePvpLosePoint = 90;

	private int unlimitedPvpWinPoint = 150;

	private int unlimitedPvpLosePoint = 90;

	private int rankPvpOpenPoint = 1000;

	private int leaguePvpOpenPoint = 3000;

	private int asyncTargetCountStandardTime = 30;

	private string rankUnlockPopupTitle = "";

	private string rankUnlockPopupDesc = "";

	private string rankUnlockPopupImageName = "";

	private string leagueUnlockPopupTitle = "";

	private string leagueUnlockPopupDesc = "";

	private string leagueUnlockPopupImageName = "";

	private bool useDoubleCostTimeRank = true;

	private bool useDoubleCostTimeArcade = true;

	private int pvpAsyncSurrenderPossibilityTime = 20;

	private int pvpRankSurrenderPossibilityTime = 20;

	private int pvpFriendlySurrenderPossibilityTime = 20;

	private int pvpEventSurrenderPossibilityTime = 20;

	private int pvpLeagueSurrenderPossibilityTime = 20;

	private int pvpUnlimitedSurrenderPossibilityTime = 20;

	private int pvpRoomMaxPlayerCount = 30;

	public UnlockInfo RankUnlockInfo;

	public UnlockInfo LeagueUnlockInfo;

	public static NKMPvpCommonConst Instance { get; } = new NKMPvpCommonConst();

	public int DEMOTION_SCORE { get; private set; }

	public int LEAGUE_PVP_DEMOTION_SCORE { get; private set; }

	public int LEAGUE_PVP_RESET_SCORE => leagueResetScore;

	public int SCORE_MIN_INTERVAL_UNIT => scoreMinIntervalUnit;

	public int RANK_PVP_LOSE_PENALTY_SCORE => rankLosePenaltyScore;

	public int ALL_RANK_LIST_MAX_COUNT => defaultRankListMaxCount;

	public int LEAGUE_RANK_LIST_MAX_COUNT => leagueRankListMaxCount;

	public int RANK_SIMPLE_COUNT => rankSimpleCount;

	public int PvpUnitBanExceptionScore => unitBanExceptionScore;

	public int RotationUnitExceptionScore => rotationUnitExceptionScore;

	public int AsyncTicketChargeInterval => asyncTicketChargeInterval;

	public int AsyncTicketMaxCount => asyncTicketMaxCount;

	public int AsyncTicketChargeCount => asyncTicketChargeCount;

	public TimeSpan AsyncTicketChargeTimeSpan => asyncTicketChargeTimeSpan;

	public bool UseDoubleCostTimeArcade => useDoubleCostTimeArcade;

	public bool UseDoubleCostTimeRank => useDoubleCostTimeRank;

	public int MaxHistoryCount => maxHistoryCount;

	public long CHARGE_POINT_REFRESH_INTERVAL_TICKS => chargePointRefreshIntervalTicks;

	public TimeSpan ChargePointRefreshInterval => new TimeSpan(chargePointRefreshIntervalTicks);

	public int CHARGE_POINT_MAX_COUNT_FOR_PRACTICE => chargePointMaxCountForPractice;

	public int CHARGE_POINT_MAX_COUNT => chargePointMax;

	public int CHARGE_POINT_ONE_STEP => chargePointCount;

	public int ASYNC_PVP_WIN_POINT => asyncPvpWinPoint;

	public int ASYNC_PVP_LOSE_POINT => asyncPvpLosePoint;

	public int RANK_PVP_WIN_POINT => rankPvpWinPoint;

	public int RANK_PVP_LOSE_POINT => rankPvpLosePoint;

	public int LEAGUE_PVP_WIN_POINT => leaguePvpWinPoint;

	public int LEAGUE_PVP_LOSE_POINT => leaguePvpLosePoint;

	public int UNLIMITED_PVP_WIN_POINT => unlimitedPvpWinPoint;

	public int UNLIMITED_PVP_LOSE_POINT => unlimitedPvpLosePoint;

	public int RANK_PVP_OPEN_POINT => rankPvpOpenPoint;

	public int LEAGUE_PVP_OPEN_POINT => leaguePvpOpenPoint;

	public int ASYNC_TARTGET_COUNT_STANDARD_ME => asyncTargetCountStandardTime;

	public string RankUnlockPopupTitle => rankUnlockPopupTitle;

	public string RankUnlockPopupDesc => rankUnlockPopupDesc;

	public string RankUnlockPopupImageName => rankUnlockPopupImageName;

	public string LeagueUnlockPopupTitle => leagueUnlockPopupTitle;

	public string LeagueUnlockPopupDesc => leagueUnlockPopupDesc;

	public string LeagueUnlockPopupImageName => leagueUnlockPopupImageName;

	public int PvpAsyncSurrenderPossibilityTime => pvpAsyncSurrenderPossibilityTime;

	public int PvpRankSurrenderPossibilityTime => pvpRankSurrenderPossibilityTime;

	public int PvpFriendlySurrenderPossibilityTime => pvpFriendlySurrenderPossibilityTime;

	public int PvpEventSurrenderPossibilityTime => pvpEventSurrenderPossibilityTime;

	public int PvpLeagueSurrenderPossibilityTime => pvpLeagueSurrenderPossibilityTime;

	public int PvpUnlimitedSurrenderPossibilityTime => pvpUnlimitedSurrenderPossibilityTime;

	public int PvpRoomMaxPlayerCount => pvpRoomMaxPlayerCount;

	public DraftBanConst DraftBan { get; } = new DraftBanConst();

	public LeaguePvpConst LeaguePvp { get; } = new LeaguePvpConst();

	public static void LoadFromLua()
	{
		string text = "LUA_PVP_CONST";
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT", text))
		{
			Log.ErrorAndExit("fail loading lua file:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 130);
			return;
		}
		if (!nKMLua.OpenTable("PVP_CONST"))
		{
			Log.ErrorAndExit("fail open table:PVP_CONST", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 136);
		}
		Instance.LoadFromLua(nKMLua);
	}

	public void Join()
	{
		LeaguePvp.Join();
	}

	public void Validate()
	{
		LeaguePvp.Validate();
		if (pvpRoomMaxPlayerCount < 2 || pvpRoomMaxPlayerCount > 30)
		{
			NKMTempletError.Add($"[NKMPvpCommonConst] pvpRoomMaxPlayerCount(관전 방 최대 참여 인원) 범위 지정 에러 pvpRoomMaxPlayerCount:{pvpRoomMaxPlayerCount} < 2 이거나 pvpRoomMaxPlayerCount:{pvpRoomMaxPlayerCount} > 30 인 경우.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 154);
		}
	}

	private void LoadFromLua(NKMLua lua)
	{
		bool flag = true;
		DEMOTION_SCORE = lua.GetInt32("DemotionScore");
		LEAGUE_PVP_DEMOTION_SCORE = lua.GetInt32("LeagueDemotionScore");
		flag &= lua.GetData("LeagueResetScore", ref leagueResetScore);
		flag &= lua.GetData("ScoreMinIntervalUnit", ref scoreMinIntervalUnit);
		flag &= lua.GetData("RankLosePenaltyScore", ref rankLosePenaltyScore);
		flag &= lua.GetData("DefaultRankListMaxCount", ref defaultRankListMaxCount);
		flag &= lua.GetData("LeagueRankListMaxCount", ref leagueRankListMaxCount);
		flag &= lua.GetData("RankSimpleCount", ref rankSimpleCount);
		flag &= lua.GetData("UnitBanExceptionScore", ref unitBanExceptionScore);
		flag &= lua.GetData("RotationUnitExceptionScore", ref rotationUnitExceptionScore);
		flag &= lua.GetData("AsyncTicketChargeInterval", ref asyncTicketChargeInterval);
		flag &= lua.GetData("AsyncTicketMaxCount", ref asyncTicketMaxCount);
		flag &= lua.GetData("AsyncTicketChargeCount", ref asyncTicketChargeCount);
		flag &= lua.GetData("MaxHistoryCount", ref maxHistoryCount);
		flag &= lua.GetData("ChargePointRefreshIntervalTicks", ref chargePointRefreshIntervalTicks);
		flag &= lua.GetData("ChargePointMaxCountForPractice", ref chargePointMaxCountForPractice);
		flag &= lua.GetData("ChargePointMax", ref chargePointMax);
		flag &= lua.GetData("ChargePointCount", ref chargePointCount);
		flag &= lua.GetData("AsyncPvpWinPoint", ref asyncPvpWinPoint);
		flag &= lua.GetData("AsyncPvpLosePoint", ref asyncPvpLosePoint);
		flag &= lua.GetData("RankPvpWinPoint", ref rankPvpWinPoint);
		flag &= lua.GetData("RankPvpLosePoint", ref rankPvpLosePoint);
		flag &= lua.GetData("LeaguePvpWinPoint", ref leaguePvpWinPoint);
		flag &= lua.GetData("LeaguePvpLosePoint", ref leaguePvpLosePoint);
		flag &= lua.GetData("UnlimitedPvpWinPoint", ref unlimitedPvpWinPoint);
		flag &= lua.GetData("UnlimitedPvpLosePoint", ref unlimitedPvpLosePoint);
		flag &= lua.GetData("RankPvpOpenPoint", ref rankPvpOpenPoint);
		flag &= lua.GetData("LeaguePvpOpenPoint", ref leaguePvpOpenPoint);
		flag &= lua.GetData("AsyncTargetCountStandardTime", ref asyncTargetCountStandardTime);
		flag &= lua.GetData("UseDoubleCostTimeRank", ref useDoubleCostTimeRank);
		flag &= lua.GetData("UseDoubleCostTimeArcade", ref useDoubleCostTimeArcade);
		lua.GetData("PvpAsyncSurrenderPossibilityTime", ref pvpAsyncSurrenderPossibilityTime);
		lua.GetData("PvpRankSurrenderPossibilityTime", ref pvpRankSurrenderPossibilityTime);
		lua.GetData("PvpFriendlySurrenderPossibilityTime", ref pvpFriendlySurrenderPossibilityTime);
		lua.GetData("PvpEventSurrenderPossibilityTime", ref pvpEventSurrenderPossibilityTime);
		lua.GetData("PvpLeagueSurrenderPossibilityTime", ref pvpLeagueSurrenderPossibilityTime);
		lua.GetData("PvpUnlimitedSurrenderPossibilityTime", ref pvpUnlimitedSurrenderPossibilityTime);
		lua.GetData("PvpRoomMaxPlayerCount", ref pvpRoomMaxPlayerCount);
		using (lua.OpenTable("RankUnlockInfo", "RankUnlockInfo table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 205))
		{
			RankUnlockInfo = UnlockInfo.LoadFromLua(lua);
			lua.GetData("m_strPopupTitle", ref rankUnlockPopupTitle);
			lua.GetData("m_strPopupDesc", ref rankUnlockPopupDesc);
			lua.GetData("m_strPopupImageName", ref rankUnlockPopupImageName);
		}
		using (lua.OpenTable("LeagueUnlockInfo", "LeagueUnlockInfo table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 213))
		{
			LeagueUnlockInfo = UnlockInfo.LoadFromLua(lua);
			lua.GetData("m_strPopupTitle", ref leagueUnlockPopupTitle);
			lua.GetData("m_strPopupDesc", ref leagueUnlockPopupDesc);
			lua.GetData("m_strPopupImageName", ref leagueUnlockPopupImageName);
		}
		DraftBan.LoadFromLua(lua);
		LeaguePvp.LoadFromLua(lua);
		if (!flag)
		{
			Log.ErrorAndExit("lua loading fail. tableName:Pvp", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPCommonConst.cs", 226);
		}
		asyncTicketChargeTimeSpan = TimeSpan.FromSeconds(asyncTicketChargeInterval);
	}
}
