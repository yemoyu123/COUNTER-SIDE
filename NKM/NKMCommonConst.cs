using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.EventPass;
using NKM.Guild;
using NKM.Item;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public static class NKMCommonConst
{
	public readonly struct ImprintMainOptEffect
	{
		public float MainStatMultiply { get; }

		public float SubStatMultiply { get; }

		public ImprintMainOptEffect(float mainStatMultiply, float subStatMultiply)
		{
			MainStatMultiply = mainStatMultiply;
			SubStatMultiply = subStatMultiply;
		}

		public float GetMultiplyValue(bool isMainStat)
		{
			if (!isMainStat)
			{
				return SubStatMultiply;
			}
			return MainStatMultiply;
		}

		public void Validate()
		{
			if (MainStatMultiply < 1f)
			{
				NKMTempletError.Add($"[LUA_COMMON_CONST] 장비 각인 계수가 잘못 되었습니다. MainStatMultiply:{SubStatMultiply}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 768);
			}
			if (SubStatMultiply < 1f)
			{
				NKMTempletError.Add($"[LUA_COMMON_CONST] 장비 각인 계수가 잘못 되었습니다. SubStatMultiply:{SubStatMultiply}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 773);
			}
		}
	}

	public const float m_fDELTA_TIME_FACTOR_1 = 1.1f;

	public const float m_fDELTA_TIME_FACTOR_2 = 1.5f;

	public const float m_fDELTA_TIME_FACTOR_05 = 0.6f;

	public static bool USE_ROLLBACK = false;

	public static float SUMMON_UNIT_NOEVENT_TIME = 0.5f;

	public static bool FIND_TARGET_USE_UNITSIZE = true;

	public const string ReplayFormatVersion = "RV006";

	public static int ENHANCE_CREDIT_COST_PER_UNIT = 300;

	public static float ENHANCE_CREDIT_COST_FACTOR = 0.05f;

	public static float ENHANCE_EXP_BONUS_FACTOR = 0.05f;

	public static float DiveRepairHpRate;

	public static float DiveStormHpRate;

	public static float OPERATOR_SKILL_STOP_TIME = 1f;

	public static float OPERATOR_SKILL_DELAY_START_TIME = 1f;

	public static int EQUIP_PRESET_BASIC_COUNT = 20;

	public static int EQUIP_PRESET_MAX_COUNT = 100;

	public static int EQUIP_PRESET_EXPAND_COST_ITEM_ID = 101;

	public static int EQUIP_PRESET_EXPAND_COST_VALUE = 50;

	public static int EQUIP_PRESET_NAME_MAX_LENGTH = 15;

	public static int FeaturedListExhibitCount = 4;

	public static double FeaturedListTotalPaymentThreshold = 119000.0;

	public static TimeSpan RECHARGE_TIME;

	public static int EVENT_RACE_PLAY_COUNT = 3;

	public static int SkipCostMiscItemId = 3;

	public static int SkipCostMiscItemCount = 0;

	public static int EVENT_BET_MAX_COUNT = 100;

	public static int MaxExtractUnitSelect = 5;

	public static int ExtractBonusRatePercent_Awaken = 80;

	public static int ExtractBonusRatePercent_SSR = 20;

	public static int ExtractBonusRatePercent_SR = 5;

	public static int RearmamentCostItemCount = 5;

	public static int RearmamentMaxGrade = 5;

	public static ImprintMainOptEffect ImprintMainOptEffectWeapon;

	public static ImprintMainOptEffect ImprintMainOptEffectDefence;

	public static ImprintMainOptEffect ImprintMainOptEffectAccessary;

	public static int ImprintCostItemId = 1;

	public static int ImprintCostItemCount = 300000;

	public static int ReImprintCostItemId = 1;

	public static int ReImprintCostItemCount = 500000;

	public static string ImprintOpenTag = "EQUIP_IMPRINT";

	public static string ConvertTitle;

	public static string ConvertMessage;

	public static string DeleteTitle;

	public static string DeleteMessage;

	public static int DropInfoShopLimit = 10;

	public static int DropInfoWorldMapMissionLimit = 10;

	public static int DropInfoRaidLimit = 10;

	public static int DropInfoShadowPalace = 10;

	public static int DropInfoDiveLimit = 10;

	public static int DropInfoFiercePointReward = 10;

	public static int DropInfoRandomMoldBox = 10;

	public static int DropInfoUnitDismiss = 10;

	public static int DropInfoUnitExtract = 10;

	public static int DropInfoMainStreamLimit = 10;

	public static int DropInfoSupplyLimit = 10;

	public static int DropInfoDailyLimit = 10;

	public static int DropInfoSideStoryLimit = 10;

	public static int DropInfoChallengeLimit = 10;

	public static int DropInfoCounterCase = 10;

	public static int DropInfoFieldLimit = 10;

	public static int DropInfoEventLimit = 10;

	public static int DropInfoTrimDungeon = 10;

	public static int DropInfoSubStreamShop = 50;

	public static float DungeonPaybackRatio = 0.5f;

	public static int INVENTORY_UNIT_EXPAND_COUNT = 0;

	public static int INVENTORY_EQUIP_EXPAND_COUNT = 0;

	public static int INVENTORY_SHIP_EXPAND_COUNT = 0;

	public static int INVENTORY_OPERATOR_EXPAND_COUNT = 0;

	public static float[] VALID_LAND_PVE = new float[3] { 0.4f, 0.6f, 0.8f };

	public static float[] VALID_LAND_PVP = new float[3] { 0.4f, 0.6f, 0.8f };

	public static float PVE_SUMMON_MIN_POS = 0f;

	public static float PVP_SUMMON_MIN_POS = 250f;

	public static float PVP_AFK_WARNING_TIME = 5f;

	public static float PVP_AFK_AUTO_TIME = 10f;

	public static HashSet<NKM_GAME_TYPE> PVP_AFK_APPLY_MODE = new HashSet<NKM_GAME_TYPE>();

	public static bool PVP_USE_UP_COST_DOWN = true;

	public static int ShipLimitBreakItemCount = 3;

	public static int ShipModuleReqItemCount = 4;

	public static int ShipCmdModuleSlotCount = 2;

	public static int ShipCmdModuleCount = 3;

	public static float RecallRewardUnitPieceToPoint = 16.67f;

	public static int MaxStageFavoriteCount = 30;

	public static int TacticReturnMaxCount = 50;

	public static string TacticReturnDateString = "DATE_TACTIC_UPDATE_RETURN";

	public static int ReactorMaxLevel = 5;

	public static int ReactorMaxReqItemCount = 3;

	public static float RelicRerollCountFactor = 1.63f;

	public static int RelicRerollLimitCount = 100;

	public static int TournamentEmptyUserSlot = -1;

	public static int TournamentUndecidedSlot = 0;

	public static int TournamentGroupCount = 4;

	public static int TournamentDefaultBotCount = 1024;

	public static int TournamentPredictionCoolTime = 60;

	public static int TournamentQualifyCoolTime = 60;

	public static int TournamentPredictionJoinRewardItemID = 2;

	public static int TournamentPredictionJoinRewardItemValue = 3000;

	public static bool TournamentUseBot = true;

	public static int TournamentMinimumDeckCP = 100000;

	public static int TournamentBanHighUnitCount = 3;

	public static int TournamentBanHighShipCount = 3;

	public static List<int> IgnoreCollaboUnitID = new List<int>();

	public static int TuningBonusResetGroupID = 1013;

	public static int TuningBonusCount = 100;

	public static int SetBonusResetGroupID = 1035;

	public static int SetBonusCount = 100;

	public static int RaidPointReqItemDecline = 70000;

	public static int CustomCastingVoteCount = 3;

	public static int MaxCastingVoteCount = 3;

	public static int MaxCastingBanLevel = 2;

	public static HashSet<NKM_GAME_TYPE> SupportUnitApplyGameList = new HashSet<NKM_GAME_TYPE>();

	public static int SuportingRewardCount = 1;

	public static int SupportingRewardMiscItemId = 1;

	public static int SupportingRewardMiscItemCount = 1;

	public static int WarfareRecoverItemCost { get; private set; }

	public static int DiveArtifactReturnItemId { get; private set; }

	public static NegotiationTemplet Negotiation { get; private set; }

	public static GuildTemplet Guild { get; private set; }

	public static int SubscriptionBuyCriteriaDate { get; private set; }

	public static NKMOperatorConstTemplet OperatorConstTemplet { get; private set; }

	public static GuildDungeonConstTemplet GuildDungeonConstTemplet { get; private set; }

	public static NKMOfficeConst Office { get; } = new NKMOfficeConst();

	public static NKMDeckConst Deck { get; } = new NKMDeckConst();

	public static NKMBackgroundConst BackgroundInfo { get; } = new NKMBackgroundConst();

	public static NKMEquipEnchantMiscConst EquipEnchantMiscConst { get; } = new NKMEquipEnchantMiscConst();

	public static void LoadFromLUA(string fileName)
	{
		bool flag = true;
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath("AB_SCRIPT", fileName))
			{
				Log.ErrorAndExit("fail loading lua file:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 214);
				return;
			}
			if (nKMLua.OpenTable("m_SystemData"))
			{
				flag &= nKMLua.GetData("m_fConstEvade", ref NKMUnitStatManager.m_fConstEvade);
				flag &= nKMLua.GetData("m_fConstHit", ref NKMUnitStatManager.m_fConstHit);
				flag &= nKMLua.GetData("m_fConstCritical", ref NKMUnitStatManager.m_fConstCritical);
				flag &= nKMLua.GetData("m_fConstDef", ref NKMUnitStatManager.m_fConstDef);
				flag &= nKMLua.GetData("m_fConstEvadeDamage", ref NKMUnitStatManager.m_fConstEvadeDamage);
				flag &= nKMLua.GetData("m_fLONG_RANGE", ref NKMUnitStatManager.m_fLONG_RANGE);
				flag &= nKMLua.GetData("m_fPercentPerBanLevel", ref NKMUnitStatManager.m_fPercentPerBanLevel);
				flag &= nKMLua.GetData("m_fMaxPercentPerBanLevel", ref NKMUnitStatManager.m_fMaxPercentPerBanLevel);
				nKMLua.GetData("m_fPercentPerUpLevel", ref NKMUnitStatManager.m_fPercentPerUpLevel);
				nKMLua.GetData("m_fMaxPercentPerUpLevel", ref NKMUnitStatManager.m_fMaxPercentPerUpLevel);
				flag &= nKMLua.GetData("m_fDEFENDER_PROTECT_RATE", ref NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE);
				flag &= nKMLua.GetData("m_fDEFENDER_PROTECT_RATE_MAX", ref NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE_MAX);
				flag &= nKMLua.GetData("ROLE_TYPE_BONUS_FACTOR", ref NKMUnitStatManager.ROLE_TYPE_BONUS_FACTOR);
				flag &= nKMLua.GetData("ROLE_TYPE_REDUCE_FACTOR", ref NKMUnitStatManager.ROLE_TYPE_REDUCE_FACTOR);
				nKMLua.GetData("m_fConstSource_Attack", ref NKMUnitStatManager.m_fConstSourceAttack);
				nKMLua.GetData("m_fConstSource_Attack_Max", ref NKMUnitStatManager.m_fSourceAttackStatMax);
				nKMLua.GetData("m_fConstSource_Defend", ref NKMUnitStatManager.m_fConstSourceDefend);
				nKMLua.CloseTable();
			}
			if (nKMLua.OpenTable("UNIT_ENHANCE_DATA"))
			{
				flag &= nKMLua.GetData("ENHANCE_CREDIT_COST_PER_UNIT", ref ENHANCE_CREDIT_COST_PER_UNIT);
				flag &= nKMLua.GetData("ENHANCE_CREDIT_COST_FACTOR", ref ENHANCE_CREDIT_COST_FACTOR);
				flag &= nKMLua.GetData("ENHANCE_EXP_BONUS_FACTOR", ref ENHANCE_EXP_BONUS_FACTOR);
				nKMLua.CloseTable();
			}
			int rValue = 0;
			using (nKMLua.OpenTable("Warfare", "[CommonConst] loading Warfare table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 255))
			{
				flag &= nKMLua.GetData("RecoverCost", ref rValue);
				WarfareRecoverItemCost = rValue;
				nKMLua.CloseTable();
			}
			using (nKMLua.OpenTable("Dive", "[CommonConst] loading Dive table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 262))
			{
				flag &= nKMLua.GetData("ArtifactReturnItemId", ref rValue);
				DiveArtifactReturnItemId = rValue;
				flag &= nKMLua.GetData("DiveStormCostMiscId", ref NKMDiveTemplet.DiveStormCostMiscId);
				flag &= nKMLua.GetData("DiveStormCostMultiply", ref NKMDiveTemplet.DiveStormCostMultiply);
				flag &= nKMLua.GetData("DiveStormRewardMiscId", ref NKMDiveTemplet.DiveStormRewardMiscId);
				flag &= nKMLua.GetData("DiveStormRewardMultiply", ref NKMDiveTemplet.DiveStormRewardMultiply);
				flag &= nKMLua.GetData("DiveRepairHP_RATE", ref DiveRepairHpRate);
				flag &= nKMLua.GetData("DiveStormHP_RATE", ref DiveStormHpRate);
			}
			using (nKMLua.OpenTable("Negotiation", "[CommonConst] loading Negotiation table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 276))
			{
				Negotiation = new NegotiationTemplet();
				Negotiation.LoadFromLua(nKMLua);
			}
			using (nKMLua.OpenTable("Guild", "[CommonConst] loading Guild table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 282))
			{
				Guild = new GuildTemplet();
				Guild.LoadFromLua(nKMLua);
			}
			using (nKMLua.OpenTable("REWARD_MULTIPLY", "[CommonConst] loading Reward_Multiply table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 288))
			{
				flag &= NKMRewardMultiplyTemplet.LoadFromLUA(nKMLua, NKMRewardMultiplyTemplet.ScopeType.General);
				flag &= NKMRewardMultiplyTemplet.LoadFromLUA(nKMLua, NKMRewardMultiplyTemplet.ScopeType.ShadowPalace);
			}
			using (nKMLua.OpenTable("SUBSCRIPTION_DATA", "[CommonConst] loading Subscription_Data table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 294))
			{
				flag &= nKMLua.GetData("BUY_CRITERIA_DATE", ref rValue);
				SubscriptionBuyCriteriaDate = rValue;
			}
			using (nKMLua.OpenTable("MENTORING_DATA", "[CommonConst] loading MentoringData table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 300))
			{
				flag &= nKMLua.GetData("MENTOR_ADD_LIMIT_LEVEL", ref NKMMentoringConst.MentorAddLimitLevel);
				flag &= nKMLua.GetData("MENTEE_INVITE_LIMIT_COUNT", ref NKMMentoringConst.MenteeInviteLimitCount);
				flag &= nKMLua.GetData("MENTEE_DELETION_AFTER_DAYS", ref NKMMentoringConst.MenteeDeletionAfterDays);
				flag &= nKMLua.GetData("MENTEE_LIMIT_BELONG_COUNT", ref NKMMentoringConst.MenteeLimitBelongCount);
				flag &= nKMLua.GetData("MENTORING_SEASON_INIT_MINUTES", ref NKMMentoringConst.MentoringSeasonInitMinutes);
				flag &= nKMLua.GetData("INVITATION_EXPIRE_DAYS", ref NKMMentoringConst.InvitationExpireDays);
				nKMLua.GetData("MENTEE_ADD_LIMIT_LEVEL", ref NKMMentoringConst.MenteeAddLimitLevel);
				nKMLua.GetData("MENTORING_ADD_LIMIT_ACTIVE_DAYS", ref NKMMentoringConst.MentoringAddLimitActiveDays);
			}
			using (nKMLua.OpenTable("FIERCE_DATA", "[FierceConst] loading fierceData table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 312))
			{
				flag &= nKMLua.GetData("MAX_BOSS_RANKING_COUNT", ref NKMFierceConst.MaxBossRankingCount);
				flag &= nKMLua.GetData("MAX_TOTAL_RANKING_COUNT", ref NKMFierceConst.MaxTotalRankingCount);
				flag &= nKMLua.GetData("RANKING_INTERVAL_MINUTES", ref NKMFierceConst.RankingIntervalMinutes);
				flag &= nKMLua.GetData("PROFILE_INTERVAL_MINUTES", ref NKMFierceConst.ProfileIntervalMinutes);
			}
			using (nKMLua.OpenTable("Operater", "[OperaterNegotiation] loading opteration negotiation data table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 320))
			{
				OperatorConstTemplet = new NKMOperatorConstTemplet();
				OperatorConstTemplet.LoadFromLua(nKMLua);
				flag &= nKMLua.GetData("OPERATOR_SKILL_STOP_TIME", ref OPERATOR_SKILL_STOP_TIME);
				flag &= nKMLua.GetData("OPERATOR_SKILL_DELAY_START_TIME", ref OPERATOR_SKILL_DELAY_START_TIME);
			}
			using (nKMLua.OpenTable("GUILD_DUNGEON", "[GuildDungeonConst] loading GuildDungeon table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 330))
			{
				GuildDungeonConstTemplet = new GuildDungeonConstTemplet();
				GuildDungeonConstTemplet.LoadFromLua(nKMLua);
			}
			using (nKMLua.OpenTable("EVENT_PASS_DATA", "[EventPassConst] loading GuildDungeon table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 336))
			{
				flag &= nKMLua.GetData("FREE_MISSION_REROLL_COUNT", ref NKMEventPassConst.FreeMissionRerollCount);
				flag &= nKMLua.GetData("PAY_MISSION_REROLL_COUNT", ref NKMEventPassConst.PayMissionRerollCount);
				NKMEventPassConst.TotalMissionRerollCount = NKMEventPassConst.FreeMissionRerollCount + NKMEventPassConst.PayMissionRerollCount;
			}
			using (nKMLua.OpenTable("EQUIP_PRESET", "[EquipPreset] loading EquipPreset table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 344))
			{
				flag &= nKMLua.GetData("EQUIP_PRESET_BASIC_COUNT", ref EQUIP_PRESET_BASIC_COUNT);
				flag &= nKMLua.GetData("EQUIP_PRESET_MAX_COUNT", ref EQUIP_PRESET_MAX_COUNT);
				flag &= nKMLua.GetData("EQUIP_PRESET_EXPAND_COST_ITEM_ID", ref EQUIP_PRESET_EXPAND_COST_ITEM_ID);
				flag &= nKMLua.GetData("EQUIP_PRESET_EXPAND_COST_VALUE", ref EQUIP_PRESET_EXPAND_COST_VALUE);
				flag &= nKMLua.GetData("EQUIP_PRESET_NAME_MAX_LENGTH", ref EQUIP_PRESET_NAME_MAX_LENGTH);
			}
			using (nKMLua.OpenTable("SHOP", "[Shop] loading Shop table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 353))
			{
				flag &= nKMLua.GetData("FeaturedListExhibitCount", ref FeaturedListExhibitCount);
				flag &= nKMLua.GetData("FeaturedListTotalPaymentThreshold", ref FeaturedListTotalPaymentThreshold);
			}
			using (nKMLua.OpenTable("Eternium_Recharge", "[Eternium Recharge Const] loading Eternium Recharge table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 359))
			{
				int rValue2 = 0;
				flag &= nKMLua.GetData("Recharge_Time", ref rValue2);
				RECHARGE_TIME = TimeSpan.FromSeconds(rValue2);
			}
			using (nKMLua.OpenTable("EVENT_RACE", "[Event Race Const] loading Event Race table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 366))
			{
				flag &= nKMLua.GetData("RACE_PLAY_COUNT", ref EVENT_RACE_PLAY_COUNT);
			}
			using (nKMLua.OpenTable("Skip", "[Skip Const] loading skip table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 371))
			{
				flag &= nKMLua.GetData("MiscItemId", ref SkipCostMiscItemId);
				flag &= nKMLua.GetData("MiscItemCount", ref SkipCostMiscItemCount);
			}
			using (nKMLua.OpenTable("EXTRACT_BONUS", "[Extract Bonus Const] loading Extract Bonus table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 377))
			{
				flag &= nKMLua.GetData("MaxExtractUnitSelect", ref MaxExtractUnitSelect);
				flag &= nKMLua.GetData("ExtractBonusRatePercent_Awaken", ref ExtractBonusRatePercent_Awaken);
				flag &= nKMLua.GetData("ExtractBonusRatePercent_SSR", ref ExtractBonusRatePercent_SSR);
				flag &= nKMLua.GetData("ExtractBonusRatePercent_SR", ref ExtractBonusRatePercent_SR);
			}
			if (NKMOpenTagManager.IsOpened(ImprintOpenTag))
			{
				using (nKMLua.OpenTable("IMPRINT_STAT", "[Equip Imprint] loading Equip Imprint table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 387))
				{
					ImprintMainOptEffectWeapon = new ImprintMainOptEffect(nKMLua.GetFloat("IEP_WEAPON_MAIN_OPTION_MAIN_STAT"), nKMLua.GetFloat("IEP_WEAPON_MAIN_OPTION_SUB_STAT"));
					ImprintMainOptEffectDefence = new ImprintMainOptEffect(nKMLua.GetFloat("IEP_DEFENCE_MAIN_OPTION_MAIN_STAT"), nKMLua.GetFloat("IEP_DEFENCE_MAIN_OPTION_SUB_STAT"));
					ImprintMainOptEffectAccessary = new ImprintMainOptEffect(nKMLua.GetFloat("IEP_ACC_MAIN_OPTION_MAIN_STAT"), nKMLua.GetFloat("IEP_ACC_MAIN_OPTION_SUB_STAT"));
					nKMLua.GetData("IEP_IMPRINT_COST_ITEM_ID", ref ImprintCostItemId);
					nKMLua.GetData("IEP_IMPRINT_COST_ITEM_COUNT", ref ImprintCostItemCount);
					nKMLua.GetData("IEP_RE_IMPRINT_COST_ITEM_ID", ref ReImprintCostItemId);
					nKMLua.GetData("IEP_RE_IMPRINT_COST_ITEM_COUNT", ref ReImprintCostItemCount);
				}
			}
			Office.Load(nKMLua);
			using (nKMLua.OpenTable("ITEM_CONVERT", "[CommonConst] loading Item Convert table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 407))
			{
				using (nKMLua.OpenTable("NotifyMailText", "[CommonConst] loading Item Convert table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 409))
				{
					flag &= nKMLua.GetData("ConvertTitle", ref ConvertTitle);
					flag &= nKMLua.GetData("ConvertMessage", ref ConvertMessage);
					flag &= nKMLua.GetData("DeleteTitle", ref DeleteTitle);
					flag &= nKMLua.GetData("DeleteMessage", ref DeleteMessage);
				}
			}
			using (nKMLua.OpenTable("ITEM_DROP_INFO_MAX_COUNT", "[CommonConst] loading Item Drop Info Max Count table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 418))
			{
				flag &= nKMLua.GetData("SHOP_CASH_TEMPLET", ref DropInfoShopLimit);
				flag &= nKMLua.GetData("WORLDMAP_MISSION_TEMPLET", ref DropInfoWorldMapMissionLimit);
				flag &= nKMLua.GetData("RAID_TEMPLET", ref DropInfoRaidLimit);
				flag &= nKMLua.GetData("SHADOW_PALACE", ref DropInfoShadowPalace);
				flag &= nKMLua.GetData("DIVE_TEMPLET", ref DropInfoDiveLimit);
				flag &= nKMLua.GetData("FIERCE_POINT_REWARD", ref DropInfoFiercePointReward);
				flag &= nKMLua.GetData("RANDOM_MOLD_BOX", ref DropInfoRandomMoldBox);
				flag &= nKMLua.GetData("UNIT_DISMISS", ref DropInfoUnitDismiss);
				flag &= nKMLua.GetData("UNIT_EXTRACT", ref DropInfoUnitExtract);
				flag &= nKMLua.GetData("TRIM_DUNGEON", ref DropInfoTrimDungeon);
				flag &= nKMLua.GetData("SUBSTREAM_SHOP", ref DropInfoSubStreamShop);
			}
			using (nKMLua.OpenTable("ITEM_DROP_INFO_EPISODE_MAX_COUNT", "[CommonConst] loading Item Drop Info Episode Max Count table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 433))
			{
				flag &= nKMLua.GetData("EC_MAINSTREAM", ref DropInfoMainStreamLimit);
				flag &= nKMLua.GetData("EC_SUPPLY", ref DropInfoSupplyLimit);
				flag &= nKMLua.GetData("EC_DAILY", ref DropInfoDailyLimit);
				flag &= nKMLua.GetData("EC_SIDESTORY", ref DropInfoSideStoryLimit);
				flag &= nKMLua.GetData("EC_CHALLENGE", ref DropInfoChallengeLimit);
				flag &= nKMLua.GetData("EC_COUNTERCASE", ref DropInfoCounterCase);
				flag &= nKMLua.GetData("EC_FIELD", ref DropInfoFieldLimit);
				flag &= nKMLua.GetData("EC_EVENT", ref DropInfoEventLimit);
			}
			using (nKMLua.OpenTable("Dungeon", "[Dungeon] loading payback data table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 445))
			{
				flag &= nKMLua.GetData("PaybackRatio", ref DungeonPaybackRatio);
			}
			using (nKMLua.OpenTable("INVENTORY_AD_EXPAND", "[INVENTORY_AD_EXPAND] loading ad reward data table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 450))
			{
				flag &= nKMLua.GetData("INVENTORY_UNIT_EXPAND_COUNT", ref INVENTORY_UNIT_EXPAND_COUNT);
				flag &= nKMLua.GetData("INVENTORY_EQUIP_EXPAND_COUNT", ref INVENTORY_EQUIP_EXPAND_COUNT);
				flag &= nKMLua.GetData("INVENTORY_SHIP_EXPAND_COUNT", ref INVENTORY_SHIP_EXPAND_COUNT);
				flag &= nKMLua.GetData("INVENTORY_OPERATOR_EXPAND_COUNT", ref INVENTORY_OPERATOR_EXPAND_COUNT);
			}
			using (nKMLua.OpenTable("GAME", "[GAME] loading game data table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 458))
			{
				if (nKMLua.GetDataList("VALID_LAND_PVE", out List<float> result, nullIfEmpty: false))
				{
					VALID_LAND_PVE = result.ToArray();
				}
				else
				{
					Log.ErrorAndExit("[GAME/VALID_LAND_PVE] loading game data table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 467);
				}
				if (nKMLua.GetDataList("VALID_LAND_PVP", out result, nullIfEmpty: false))
				{
					VALID_LAND_PVP = result.ToArray();
				}
				else
				{
					Log.ErrorAndExit("[GAME/VALID_LAND_PVP] loading game data table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 476);
				}
				flag &= nKMLua.GetData("PVE_SUMMON_MIN_POS", ref PVE_SUMMON_MIN_POS);
				flag &= nKMLua.GetData("PVP_SUMMON_MIN_POS", ref PVP_SUMMON_MIN_POS);
				flag &= nKMLua.GetData("PVP_AFK_WARNING_TIME", ref PVP_AFK_WARNING_TIME);
				flag &= nKMLua.GetData("PVP_AFK_AUTO_TIME", ref PVP_AFK_AUTO_TIME);
				nKMLua.GetData("PVP_USE_UP_COST_DOWN", ref PVP_USE_UP_COST_DOWN);
				nKMLua.GetDataListEnum("PVP_AFK_APPLY_MODE", PVP_AFK_APPLY_MODE);
				nKMLua.GetData("USE_ROLLBACK", ref USE_ROLLBACK);
				nKMLua.GetData("FIND_TARGET_USE_UNITSIZE", ref FIND_TARGET_USE_UNITSIZE);
			}
			using (nKMLua.OpenTable("SHIP_LIMITBREAK", "[SHIP_LIMITBREAK] loading ship limitbreak data table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 494))
			{
				flag &= nKMLua.GetData("ShipLimitBreakItemCount", ref ShipLimitBreakItemCount);
				flag &= nKMLua.GetData("ModuleReqItemCount", ref ShipModuleReqItemCount);
			}
			using (nKMLua.OpenTable("RECALL", "[RECALL] loading recall data table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 500))
			{
				nKMLua.GetData("RECALL_REWARD_UNIT_PIECE_TO_POINT", ref RecallRewardUnitPieceToPoint);
			}
			using (nKMLua.OpenTable("STAGE_FAVORITE", "[STAGE_FAVORITE] loading favorite data table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 505))
			{
				nKMLua.GetData("MAX_STAGE_FAVORITE_COUNT", ref MaxStageFavoriteCount);
			}
			using (nKMLua.OpenTable("TACTIC_UPDATE", "[TACTIC_UPDATE] loading tactic update table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 510))
			{
				nKMLua.GetData("TACTIC_UPDATE_RETURN_COUNT", ref TacticReturnMaxCount);
				nKMLua.GetData("TACTIC_UPDATE_RETURN_DATE", ref TacticReturnDateString);
			}
			using (nKMLua.OpenTable("EquipEnchantModule", "[EquipEnchantModule] loading EquipEnchantModule table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 516))
			{
				EquipEnchantMiscConst.LoadFromLua(nKMLua);
			}
			BackgroundInfo.Load(nKMLua);
			using (nKMLua.OpenTable("Reactor", "[Reactor] loading reactor table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 523))
			{
				flag &= nKMLua.GetData("MaxReactorLevel", ref ReactorMaxLevel);
				flag &= nKMLua.GetData("MaxReactorReqItemCount", ref ReactorMaxReqItemCount);
			}
			using (nKMLua.OpenTable("RelicReroll", "[RelicReroll] loading relicreroll table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 529))
			{
				flag &= nKMLua.GetData("RelicRerollCountFactor", ref RelicRerollCountFactor);
				flag &= nKMLua.GetData("RelicRerollLimitCount", ref RelicRerollLimitCount);
			}
			using (nKMLua.OpenTable("Tournament", "[Tournament] loading tournament table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 535))
			{
				flag &= nKMLua.GetData("EmptyUserSlot", ref TournamentEmptyUserSlot);
				flag &= nKMLua.GetData("TournamentPredictionCoolTime", ref TournamentPredictionCoolTime);
				flag &= nKMLua.GetData("TournamentQualifyCoolTime", ref TournamentQualifyCoolTime);
				flag &= nKMLua.GetData("PredictionJoinRewardItemID", ref TournamentPredictionJoinRewardItemID);
				flag &= nKMLua.GetData("PredictionJoinRewardItemValue", ref TournamentPredictionJoinRewardItemValue);
				nKMLua.GetData("TournamentDefaultBotCount", ref TournamentDefaultBotCount);
				nKMLua.GetData("TournamentUseBot", ref TournamentUseBot);
				nKMLua.GetData("TournamentUndecidedSlot", ref TournamentUndecidedSlot);
				nKMLua.GetData("TournamentMinimumDeckCP", ref TournamentMinimumDeckCP);
				nKMLua.GetData("TournamentBanHighUnitCount", ref TournamentBanHighUnitCount);
				nKMLua.GetData("TournamentBanHighShipCount", ref TournamentBanHighShipCount);
			}
			using (nKMLua.OpenTable("BinaryBonus", "[BinaryBonus] loading binaryBounus table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 551))
			{
				flag &= nKMLua.GetData("TuningBonusResetGroupID", ref TuningBonusResetGroupID);
				flag &= nKMLua.GetData("TuningBonusCount", ref TuningBonusCount);
				flag &= nKMLua.GetData("SetBonusResetGroupID", ref SetBonusResetGroupID);
				flag &= nKMLua.GetData("SetBonusCount", ref SetBonusCount);
			}
			using (nKMLua.OpenTable("Raid", "[Raid] loading Raid table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 559))
			{
				flag &= nKMLua.GetData("Raid_Point_ReqItemDecline", ref RaidPointReqItemDecline);
			}
			using (nKMLua.OpenTable("CastingBan", "[CastingBan] loading CastingBan table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 564))
			{
				flag &= nKMLua.GetData("MaxBanCount", ref CustomCastingVoteCount);
				flag &= nKMLua.GetData("MaxBanLevel", ref MaxCastingBanLevel);
			}
			using (nKMLua.OpenTable("EventDeck", "[EventDeck] loading EventDeck table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 570))
			{
				nKMLua.GetDataList("IgnoreCollaboUnitID", out IgnoreCollaboUnitID, nullIfEmpty: false);
			}
			using (nKMLua.OpenTable("SupportUnit", "[SupportUnit] loading SupportUnit table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 575))
			{
				nKMLua.GetDataListEnum("ApplyGameTypeList", out SupportUnitApplyGameList, nullIfEmpty: true);
				flag &= nKMLua.GetData("SuportingRewardCount", ref SuportingRewardCount);
				flag &= nKMLua.GetData("SupportingRewardMiscItemId", ref SupportingRewardMiscItemId);
				flag &= nKMLua.GetData("SupportingRewardMiscItemCount", ref SupportingRewardMiscItemCount);
			}
		}
		if (!flag)
		{
			Log.ErrorAndExit("fail loading lua file:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 586);
		}
	}

	public static void Join()
	{
		Negotiation.Join();
		Guild.Join();
		Office.Join();
		EquipEnchantMiscConst.Join();
	}

	public static void Validate()
	{
		Negotiation.Validate();
		Guild.Validate();
		Office.Validate();
		Deck.Validate();
		OperatorConstTemplet.Validate();
		EquipEnchantMiscConst.Validate();
		NKMRewardMultiplyTemplet.Validate();
		if (NKMItemManager.GetItemMiscTempletByID(EQUIP_PRESET_EXPAND_COST_ITEM_ID) == null)
		{
			Log.ErrorAndExit($"[EquipPreset] invalid cost itemId:{EQUIP_PRESET_EXPAND_COST_ITEM_ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 611);
		}
		if (EQUIP_PRESET_EXPAND_COST_VALUE < 0)
		{
			Log.ErrorAndExit($"[EquipPreset] invalid cost count:{EQUIP_PRESET_EXPAND_COST_VALUE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 616);
		}
		if (EQUIP_PRESET_NAME_MAX_LENGTH < 0 || EQUIP_PRESET_NAME_MAX_LENGTH > 15)
		{
			Log.ErrorAndExit($"[EquipPreset] invalid name Max Length. input:{EQUIP_PRESET_NAME_MAX_LENGTH}. DB coverage:1~15", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 621);
		}
		if (RECHARGE_TIME.TotalSeconds <= 0.0)
		{
			Log.ErrorAndExit("[LUA_COMMON_CONST] Invalid Eternium Recharge Time.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 626);
		}
		if (ExtractBonusRatePercent_Awaken < 0 || ExtractBonusRatePercent_SSR < 0 || ExtractBonusRatePercent_SR < 0)
		{
			Log.ErrorAndExit($"[LUA_COMMON_CONST] 유닛 추출 보너스 퍼센트(ExtractBonusRatePercent)가 잘못되었습니다. Awaken:{ExtractBonusRatePercent_Awaken} SSR:{ExtractBonusRatePercent_SSR}, SR:{ExtractBonusRatePercent_SR}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 633);
		}
		if (NKMOpenTagManager.IsOpened(ImprintOpenTag))
		{
			ImprintMainOptEffectWeapon.Validate();
			ImprintMainOptEffectDefence.Validate();
			ImprintMainOptEffectAccessary.Validate();
			if (NKMItemManager.GetItemMiscTempletByID(ImprintCostItemId) == null)
			{
				NKMTempletError.Add($"[LUA_COMMON_CONST] 장비 각인 아이템 아이디가 잘못 되었습니다. ImprintCostItemId:{ImprintCostItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 644);
			}
			if (ImprintCostItemCount <= 0)
			{
				NKMTempletError.Add($"[LUA_COMMON_CONST] 장비 각인 아이템 개수가 잘못 되었습니다. ImprintCostItemCount:{ImprintCostItemCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 649);
			}
			if (NKMItemManager.GetItemMiscTempletByID(ReImprintCostItemId) == null)
			{
				NKMTempletError.Add($"[LUA_COMMON_CONST] 장비 재각인 아이템 아이디가 잘못 되었습니다. ReImprintCostItemId:{ReImprintCostItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 654);
			}
			if (ReImprintCostItemCount <= 0)
			{
				NKMTempletError.Add($"[LUA_COMMON_CONST] 장비 재각인 아이템 개수가 잘못 되었습니다. ReImprintCostItemCount:{ReImprintCostItemCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 659);
			}
		}
		if (DungeonPaybackRatio <= 0f || DungeonPaybackRatio > 1f)
		{
			NKMTempletError.Add($"[LUA_COMMON_CONST] 이터니움 환급 비율이 잘못되었습니다. Dungeon/PaybackRatio:{DungeonPaybackRatio}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 665);
		}
		if (NKMItemMiscTemplet.Find(SkipCostMiscItemId) == null)
		{
			Log.ErrorAndExit($"[LUA_COMMON_CONST] 스킵 비용 misc id가 잘못되었습니다. miscId:{SkipCostMiscItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 670);
		}
		if (SkipCostMiscItemCount != 0)
		{
			Log.ErrorAndExit($"[LUA_COMMON_CONST] 스킵 비용이 0이 아닙니다. Count:{SkipCostMiscItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 675);
		}
		if (DiveRepairHpRate < 0f || DiveRepairHpRate > 100f)
		{
			NKMTempletError.Add($"[LUA_COMMON_CONST] 다이브 함선 수리 비율이 0 이하거나 100 이상입니다. Rate:{DiveRepairHpRate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 680);
		}
		if (DiveStormHpRate < -100f || DiveStormHpRate > 0f)
		{
			NKMTempletError.Add($"[LUA_COMMON_CONST] 다이브 함선 체력 감소 비율이 -100 이하거나 0 이상입니다. Rate:{DiveStormHpRate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 685);
		}
		if (RelicRerollCountFactor <= 0f)
		{
			NKMTempletError.Add($"[LUA_COMMON_CONST] 렐릭 장비 잠재능력 재설정 값은 0 이하로 내려갈 수 없음 RelicRerollCountFactor:{RelicRerollCountFactor}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 690);
		}
		if (RelicRerollLimitCount <= 0)
		{
			NKMTempletError.Add($"[LUA_COMMON_CONST] 렐릭 장비 잠재능력 재설정 한계 값은 0 이하로 내려갈 수 없음 RelicRerollLimitCount:{RelicRerollLimitCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 695);
		}
		if (OperatorConstTemplet != null)
		{
			if (OperatorConstTemplet.hostUnits.Exists((NKMOperatorConstTemplet.HostUnit e) => e.itemId <= 0))
			{
				NKMTempletError.Add("[LUA_COMMON_CONST] 오퍼레이터 강화에 필요한 재화 아이템 id가 잘못된 경우가 존재.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 702);
			}
			if (OperatorConstTemplet.hostUnits.Exists((NKMOperatorConstTemplet.HostUnit e) => e.itemCount <= 0))
			{
				NKMTempletError.Add("[LUA_COMMON_CONST] 오퍼레이터 강화에 필요한 재화 아이템 개수가 잘못된 경우가 존재.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 707);
			}
			if (OperatorConstTemplet.hostUnits.Exists((NKMOperatorConstTemplet.HostUnit e) => e.extractPriceItemId <= 0))
			{
				NKMTempletError.Add("[LUA_COMMON_CONST] 오퍼레이터 추출 시 추가 필요 아이템의 id가 잘못된 경우가 존재.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 712);
			}
			if (OperatorConstTemplet.hostUnits.Exists((NKMOperatorConstTemplet.HostUnit e) => e.extractPrice <= 0))
			{
				NKMTempletError.Add("[LUA_COMMON_CONST] 오퍼레이터 추출 시 추가 필요 아이템의 개수가 잘못된 경우가 존재.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 717);
			}
			if (OperatorConstTemplet.listPassiveToken.Any((NKMOperatorConstTemplet.PassiveToken e) => e.ItemID.Any((int i) => i <= 0)))
			{
				NKMTempletError.Add("[LUA_COMMON_CONST] 오퍼레이터 토큰 아이템의 id에 0 이하 정보가 존재", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 722);
			}
		}
		if (CustomCastingVoteCount > MaxCastingVoteCount)
		{
			NKMTempletError.Add("[LUA_COMMON_CONST] 지정 된 캐스팅 밴의 최댓값이 한계를 넘음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMCommonConst.cs", 728);
		}
	}

	public static ImprintMainOptEffect GetEquipIimprintMainOptEffect(ITEM_EQUIP_POSITION equipPosition)
	{
		switch (equipPosition)
		{
		case ITEM_EQUIP_POSITION.IEP_WEAPON:
			return ImprintMainOptEffectWeapon;
		case ITEM_EQUIP_POSITION.IEP_DEFENCE:
			return ImprintMainOptEffectDefence;
		case ITEM_EQUIP_POSITION.IEP_ACC:
		case ITEM_EQUIP_POSITION.IEP_ACC2:
			return ImprintMainOptEffectAccessary;
		default:
			return new ImprintMainOptEffect(1f, 1f);
		}
	}
}
