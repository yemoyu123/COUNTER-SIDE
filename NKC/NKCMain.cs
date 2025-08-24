using System.Collections.Generic;
using System.Linq;
using AssetBundles;
using Cs.Engine.Util;
using Cs.Logging;
using NKC.Loading;
using NKC.Localization;
using NKC.StringSearch;
using NKC.Templet;
using NKM;
using NKM.Contract2;
using NKM.Event;
using NKM.EventPass;
using NKM.Game;
using NKM.Guild;
using NKM.Item;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Templet.Office;
using NKM.Templet.Recall;
using NKM.Unit;
using UnityEngine;

namespace NKC;

public static class NKCMain
{
	public const string marketID = "Steam";

	public const string NKC_SAFE_MODE_KEY = "NKC_SAFE_MODE_KEY";

	public static bool m_ranAsSafeMode;

	private static void NKCInitLocalContentsVersion()
	{
		string text = PlayerPrefs.GetString("LOCAL_SAVE_CONTENTS_VERSION_KEY");
		Log.Info("NKCInitLocalContentsVersion LocalCV[" + text + "], Key[LOCAL_SAVE_CONTENTS_VERSION_KEY]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCMain.cs", 40);
		if (!string.IsNullOrEmpty(text))
		{
			NKMContentsVersion nKMContentsVersion = NKMContentsVersion.Create(text);
			if (nKMContentsVersion != null && NKMContentsVersionManager.CurrentVersion < nKMContentsVersion)
			{
				Log.Info("Created LocalContentsVersion LocalCV[" + text + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCMain.cs", 51);
				NKMContentsVersionManager.SetCurrent(text);
			}
		}
	}

	public static void InvalidateSafeMode()
	{
		m_ranAsSafeMode = false;
		NKCContentsVersionManager.s_DefaultTagLoaded = false;
		PlayerPrefs.DeleteKey("NKC_SAFE_MODE_KEY");
	}

	public static bool IsSafeMode()
	{
		if (!NKCDefineManager.DEFINE_CAN_ONLY_LOAD_MIN_TEMPLET())
		{
			return false;
		}
		if (NKCContentsVersionManager.s_DefaultTagLoaded)
		{
			return true;
		}
		if (NKCDefineManager.DEFINE_CHECKVERSION() && !ContentsVersionChecker.VersionAckReceived)
		{
			return true;
		}
		if (PlayerPrefs.GetInt("NKC_SAFE_MODE_KEY", 0) == 1)
		{
			return true;
		}
		return false;
	}

	public static void NKCInit()
	{
		List<string> list = new List<string>();
		NKMDataVersion.LoadFromLUA();
		NKMContentsVersionManager.LoadDefaultVersion();
		if (NKCDefineManager.DEFINE_UNITY_EDITOR())
		{
			NKCContentsVersionManager.TryRecoverTag();
		}
		NKCInitLocalContentsVersion();
		if (NKCDefineManager.DEFINE_UNITY_EDITOR())
		{
			NKM_NATIONAL_CODE currentNationalCode = NKCGameOptionData.LoadLanguageCode(NKM_NATIONAL_CODE.NNC_END);
			NKC_VOICE_CODE nKC_VOICE_CODE = NKCUIVoiceManager.LoadLocalVoiceCode();
			NKCUIVoiceManager.SetVoiceCode(nKC_VOICE_CODE);
			AssetBundleManager.ActiveVariants = NKCLocalization.GetVariants(currentNationalCode, nKC_VOICE_CODE);
		}
		NKMCommonConst.LoadFromLUA("LUA_COMMON_CONST");
		NKCClientConst.LoadFromLUA("LUA_CLIENT_CONST");
		NKMTempletContainer<NKCLoginBackgroundTemplet>.Load("AB_SCRIPT", "LUA_LOGIN_BACKGROUND", "LOGIN_BACKGROUND", NKCLoginBackgroundTemplet.LoadFromLUA);
		if (IsSafeMode())
		{
			m_ranAsSafeMode = true;
			return;
		}
		NKMAnimDataManager.LoadFromLUA("LUA_ANIM_DATA");
		NKMShipSkillManager.LoadFromLUA("LUA_SHIP_SKILL_TEMPLET");
		NKMUnitSkillManager.LoadFromLUA("LUA_UNIT_SKILL_TEMPLET");
		NKMTacticalCommandManager.LoadFromLUA("LUA_TACTICAL_COMMAND_TEMPLET");
		NKMItemManager.LoadFromLUA_ITEM_MISC("LUA_ITEM_MISC_TEMPLET");
		NKMTempletContainer<NKMOfficeInteriorTemplet>.Load("AB_SCRIPT", "LUA_ITEM_INTERIOR_TEMPLET", "ITEM_INTERIOR_PREFAB", NKMOfficeInteriorTemplet.LoadFromLua);
		NKMOfficeInteriorTemplet.MergeContainer();
		NKMOfficeGradeTemplet.LoadFromLua();
		list.Clear();
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET");
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET2");
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET3");
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET4");
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET5");
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET6");
		NKMDETempletManager.LoadFromLUA(list, bReload: true);
		NKMCommonUnitEvent.LoadFromLUA("LUA_COMMON_UNIT_EVENT_HEAL", bReload: true);
		string[] fileNames = new string[4] { "LUA_UNIT_TEMPLET_BASE", "LUA_UNIT_TEMPLET_BASE2", "LUA_UNIT_TEMPLET_BASE_SD", "LUA_UNIT_TEMPLET_BASE_OPR" };
		NKMTempletContainer<NKMUnitTempletBase>.Load("AB_SCRIPT_UNIT_DATA", fileNames, "m_dicNKMUnitTempletBaseByStrID", NKMUnitTempletBase.LoadFromLUA, (NKMUnitTempletBase e) => e.m_UnitStrID);
		fileNames = new string[4] { "LUA_UNIT_STAT_TEMPLET", "LUA_UNIT_STAT_TEMPLET2", "LUA_UNIT_STAT_TEMPLET_SD", "LUA_UNIT_STAT_TEMPLET_OPR" };
		NKMUnitManager.LoadFromLUA(fileNames, "", "");
		NKMSkinManager.LoadFromLua();
		NKMEquipTuningManager.LoadFromLUA_EquipRandomStat("LUA_ITEM_EQUIP_RANDOM_STAT");
		NKMItemManager.LoadFromLUA_EquipSetOption("LUA_ITEM_EQUIP_SET_OPTION");
		NKMItemManager.LoadFromLUA_Item_Equip("LUA_ITEM_EQUIP_TEMPLET");
		NKMItemManager.LoadFromLUA_EquipEnchantExp("LUA_EQUIP_ENCHANT_EXP_TABLE");
		NKMTempletContainer<NKMItemEquipUpgradeTemplet>.Load("AB_SCRIPT", "LUA_ITEM_EQUIP_UPGRADE", "ITEM_EQUIP_UPGRADE", NKMItemEquipUpgradeTemplet.LoadFromLua);
		NKMItemManager.LoadFromLua_Item_Mold("LUA_ITEM_MOLD_TEMPLET");
		NKMItemManager.LoadFromLua_Random_Mold_Box("LUA_RANDOM_MOLD_BOX_CL");
		NKMItemManager.LoadFromLua_Item_Mold_Tab("LUA_ITEM_MOLD_TAB");
		NKMItemManager.LoadFromLua_Item_AutoWeight("LUA_EQUIP_AUTO_WEIGHT_CL");
		NKMRewardManager.LoadFromLUA("LUA_REWARD_TEMPLET_CL");
		NKMRandomGradeManager.LoadFromLUA("LUA_RANDOM_GRADE");
		NKMMapManager.LoadFromLUA("LUA_MAP_TEMPLET");
		NKMUnitListTemplet.LoadFromLua();
		NKMDungeonManager.LoadFromLUA_EventDeckInfo();
		NKMDungeonManager.LoadFromLUA("LUA_DUNGEON_TEMPLET_BASE", "", bFullLoad: false);
		NKMTempletContainer<NKMWarfareTemplet>.Load("AB_SCRIPT_WARFARE", "LUA_WARFARE_TEMPLET", "m_dicNKMWarfareTemplet", NKMWarfareTemplet.LoadFromLUA, (NKMWarfareTemplet e) => e.m_WarfareStrID);
		NKMTempletContainer<NKMDiveTemplet>.Load("AB_SCRIPT", "LUA_DIVE_TEMPLET", "DIVE_TEMPLET", NKMDiveTemplet.LoadFromLUA);
		NKMTempletContainer<NKMDiveArtifactTemplet>.Load("AB_SCRIPT", "LUA_DIVE_ARTIFACT", "DIVE_ARTIFACT", NKMDiveArtifactTemplet.LoadFromLUA);
		NKMEpisodeMgr.LoadFromLUA("LUA_STAGE_TEMPLET");
		NKMTempletContainer<NKMEpisodeTempletV2>.Load("AB_SCRIPT", "LUA_EPISODE_TEMPLET_V2", "EPISODE_TEMPLET_V2", NKMEpisodeTempletV2.LoadFromLUA);
		NKMEpisodeMgr.LoadClientOnlyData();
		NKMTempletContainer<NKMEpisodeGroupTemplet>.Load("AB_SCRIPT", "LUA_EPISODE_GROUP_TEMPLET", "EPISODE_GROUP_TEMPLET", NKMEpisodeGroupTemplet.LoadFromLUA);
		NKMTempletContainer<NKCEpisodeSummaryTemplet>.Load("AB_SCRIPT", "LUA_EPISODE_SUMMARY_TEMPLET", "EPISODE_SUMMARY_TEMPLET", NKCEpisodeSummaryTemplet.LoadFromLua);
		list.Clear();
		list.Add("LUA_DAMAGE_TEMPLET");
		list.Add("LUA_DAMAGE_TEMPLET2");
		list.Add("LUA_DAMAGE_TEMPLET3");
		list.Add("LUA_DAMAGE_TEMPLET4");
		list.Add("LUA_DAMAGE_TEMPLET5");
		list.Add("LUA_DAMAGE_TEMPLET6");
		fileNames = new string[6] { "LUA_DAMAGE_TEMPLET_BASE", "LUA_DAMAGE_TEMPLET_BASE2", "LUA_DAMAGE_TEMPLET_BASE3", "LUA_DAMAGE_TEMPLET_BASE4", "LUA_DAMAGE_TEMPLET_BASE5", "LUA_DAMAGE_TEMPLET_BASE6" };
		NKMDamageManager.LoadFromLUA(fileNames, list);
		NKMBuffManager.LoadFromLUA();
		NKMBuffTemplet.ParseAllSkinDic();
		NKMEventConditionV2.LoadTempletMacro();
		NKMTempletContainer<NKMCompanyBuffTemplet>.Load("AB_SCRIPT", "LUA_COMPANY_BUFF_TEMPLET", "COMPANY_BUFF_TEMPLET", NKMCompanyBuffTemplet.LoadFromLua);
		NKMTempletContainer<NKMUserExpTemplet>.Load("AB_SCRIPT", "LUA_PLAYER_EXP_TABLE", "m_PlayerExpTable", NKMUserExpTemplet.LoadFromLUA);
		NKMUnitExpTableContainer.LoadFromLua(server: false);
		NKMUnitLimitBreakManager.LoadFromLua("LUA_LIMITBREAK_INFO", "LUA_LIMITBREAK_SUBSTITUTE_ITEM");
		NKMWorldMapManager.LoadFromLUA();
		NKMTempletContainer<NKMShipBuildTemplet>.Load("AB_SCRIPT", "LUA_SHIP_BUILD_TEMPLET", "SHIP_BUILD_TEMPLET", NKMShipBuildTemplet.LoadFromLUA);
		NKMShipLevelUpTemplet.LoadFromLua("LUA_SHIP_LEVELUP_TEMPLET", "SHIP_LEVELUP_TEMPLET");
		ShopTabTempletContainer.Load();
		NKMTempletLoader.Load("AB_SCRIPT", "m_ShopTable", ShopItemTemplet.LoadFromLUA, "LUA_SHOP_TEMPLET_01", "LUA_SHOP_TEMPLET_02");
		NKCRandomBoxManager.LoadFromLUA();
		NKMTempletContainer<NKCContractCategoryTemplet>.Load("AB_SCRIPT", "LUA_CONTRACT_CATEGORY", "CONTRACT_CATEGORY", NKCContractCategoryTemplet.LoadFromLUA);
		ContractTempletLoader.Load();
		NKMMissionManager.LoadFromLUA("LUA_MISSION_TEMPLET", "LUA_MISSION_TAB_TEMPLET");
		NKMUnitStatManager.LoadFromLua();
		NKMAttendanceManager.LoadFromLua();
		NKCNewsManager.LoadFromLua();
		NKMBattleConditionManager.LoadFromLua();
		NKMPvpCommonConst.LoadFromLua();
		NKCPVPManager.LoadFromLua();
		NKMEventManager.LoadFromLua();
		NKMTempletContainer<NKMCollectionTeamUpGroupTemplet>.Load(from e in NKMTempletLoader<NKMCollectionTeamUpTemplet>.LoadGroup("AB_SCRIPT", "LUA_COLLECTION_TEAMUP_TEMPLET", "COLLECTION_TEAMUP_TEMPLET", NKMCollectionTeamUpTemplet.LoadFromLUA)
			select new NKMCollectionTeamUpGroupTemplet(e.Key, e.Value), null);
		NKMTempletContainer<NKMRaidTemplet>.Load("AB_SCRIPT", "LUA_RAID_TEMPLET", "m_RaidTemplet", NKMRaidTemplet.LoadFromLUA);
		NKMTempletContainer<NKMRaidBuffTemplet>.Load("AB_SCRIPT", "LUA_RAID_BUFF_TEMPLET", "m_RaidBuffTemplet", NKMRaidBuffTemplet.LoadFromLUA);
		NKMTempletContainer<NKMContentUnlockTemplet>.Load("AB_SCRIPT", "LUA_CONTENTS_UNLOCK_TEMPLET", "CONTENTS_UNLOCK_TEMPLET", NKMContentUnlockTemplet.LoadFromLUA);
		NKCFilterManager.LoadFromLua();
		NKCTutorialManager.LoadFromLua();
		NKMTempletContainer<NKCCollectionVoiceTemplet>.Load("AB_SCRIPT", "LUA_COLLECTION_VOICE_TEMPLET", "COLLECTION_VOICE_TEMPLET", NKCCollectionVoiceTemplet.LoadLua);
		NKMTempletContainer<NKMEmoticonTemplet>.Load("AB_SCRIPT_ITEM_TEMPLET", "LUA_ITEM_EMOTICON_TEMPLET", "EMOTICON_MANAGER", NKMEmoticonTemplet.LoadFromLUA);
		NKMTempletContainer<NKCCurrencyTemplet>.Load("AB_SCRIPT", "LUA_CURRENCY_TEMPLET", "CURRENCY_TEMPLET", NKCCurrencyTemplet.LoadFromLua, (NKCCurrencyTemplet e) => e.m_Code);
		NKMTempletContainer<NKCLobbyIconTemplet>.Load("AB_SCRIPT", "LUA_LOBBY_ICON_TEMPLET", "LOBBY_ICON_TEMPLET", NKCLobbyIconTemplet.LoadFromLUA);
		NKMTempletContainer<NKCShopBannerTemplet>.Load("AB_SCRIPT", "LUA_SHOP_BANNER_TEMPLET", "SHOP_BANNER_TEMPLET", NKCShopBannerTemplet.LoadFromLUA);
		NKCShopCategoryTemplet.Load();
		NKMTempletContainer<NKMEventTabTemplet>.Load("AB_SCRIPT", "LUA_EVENT_TAB_TEMPLET", "EVENT_TAB_TEMPLET", NKMEventTabTemplet.LoadFromLUA);
		NKMTempletContainer<NKCEventMissionTemplet>.Load("AB_SCRIPT", "LUA_EVENT_MISSION_TEMPLET", "EVENT_MISSION_TEMPLET", NKCEventMissionTemplet.LoadFromLUA);
		NKMTempletContainer<NKMPieceTemplet>.Load("AB_SCRIPT_ITEM_TEMPLET", "LUA_PIECE_TEMPLET", "PIECE_TEMPLET", NKMPieceTemplet.LoadFromLUA);
		NKMCommonConst.Join();
		NKMTempletContainer<NKCBackgroundTemplet>.Load("AB_SCRIPT_ITEM_TEMPLET", "LUA_ITEM_BACKGROUND_PREFAB", "ITEM_BACKGROUND_PREFAB", NKCBackgroundTemplet.LoadFromLUA);
		NKCLoginCutSceneManager.LoadFromLua();
		NKMShadowPalaceManager.LoadFromLua();
		NKMTempletContainer<NKMLeaderBoardTemplet>.Load("AB_SCRIPT", "LUA_LEADERBOARD_TEMPLET", "LEADERBOARD_TEMPLET", NKMLeaderBoardTemplet.LoadFromLUA);
		NKMTempletContainer<NKMGuildBadgeColorTemplet>.Load("AB_SCRIPT", "LUA_GUILD_BADGE_COLOR_TEMPLET", "GUILD_BADGE_COLOR_TEMPLET", NKMGuildBadgeColorTemplet.LoadFromLUA);
		NKMTempletContainer<NKMGuildBadgeFrameTemplet>.Load("AB_SCRIPT", "LUA_GUILD_BADGE_FRAME_TEMPLET", "GUILD_BADGE_FRAME_TEMPLET", NKMGuildBadgeFrameTemplet.LoadFromLUA);
		NKMTempletContainer<NKMGuildBadgeMarkTemplet>.Load("AB_SCRIPT", "LUA_GUILD_BADGE_MARK_TEMPLET", "GUILD_BADGE_MARK_TEMPLET", NKMGuildBadgeMarkTemplet.LoadFromLUA);
		NKCFierceBattleSupportDataMgr.LoadFromLua();
		NKMTempletContainer<GuildExpTemplet>.Load("AB_SCRIPT", "LUA_GUILD_EXP_TEMPLET", "GUILD_EXP_TEMPLET", GuildExpTemplet.LoadFromLua);
		GuildAttendanceTemplet.LoadFromLua();
		NKMTempletContainer<GuildDonationTemplet>.Load("AB_SCRIPT", "LUA_GUILD_DONATION_TEMPLET", "GUILD_DONATION_TEMPLET", GuildDonationTemplet.LoadFromLua);
		NKMTempletContainer<GuildWelfareTemplet>.Load("AB_SCRIPT", "LUA_GUILD_WELFARE_TEMPLET", "GUILD_WELFARE_TEMPLET", GuildWelfareTemplet.LoadFromLua);
		NKMTempletContainer<NKMMentoringTemplet>.Load("AB_SCRIPT", "LUA_MENTORING_SEASON_TEMPLET", "MENTORING_SEASON_TEMPLET", NKMMentoringTemplet.LoadFromLua);
		NKMTempletContainer<NKMMentoringRewardTemplet>.Load("AB_SCRIPT", "LUA_MENTORING_SEASON_REWARD_TEMPLET", "MENTORING_SEASON_REWARD_TEMPLET", NKMMentoringRewardTemplet.LoadFromLua);
		NKCLoadingScreenManager.LoadFromLua();
		NKMTempletContainer<GuildSeasonTemplet>.Load("AB_SCRIPT", "LUA_GUILD_SEASON_TEMPLET", "GUILD_SEASON_TEMPLET", GuildSeasonTemplet.LoadFromLua);
		GuildDungeonTempletManager.LoadFromLua();
		NKMTempletContainer<NKMLobbyFaceTemplet>.Load("AB_SCRIPT", "LUA_LOBBY_FACE_TEMPLET", "m_LobbyFaceTemplet", NKMLobbyFaceTemplet.LoadFromLUA);
		NKMCustomPackageGroupTemplet.LoadFromLua();
		NKMTempletContainer<NKMEventPassTemplet>.Load("AB_SCRIPT", "LUA_EVENT_PASS_TEMPLET", "EVENT_PASS_TEMPLET", NKMEventPassTemplet.LoadFromLUA);
		NKMTempletContainer<NKMEventPassRewardTemplet>.Load("AB_SCRIPT", "LUA_EVENT_PASS_REWARD_TEMPLET", "EVENT_PASS_REWARD_TEMPLET", NKMEventPassRewardTemplet.LoadFromLUA);
		NKMTempletContainer<NKMEventPassMissionGroupTemplet>.Load("AB_SCRIPT", "LUA_EVENT_PASS_MISSION_GROUP_TEMPLET", "EVENT_PASS_MISSION_GROUP_TEMPLET", NKMEventPassMissionGroupTemplet.LoadFromLUA);
		NKMTempletContainer<NKMOperatorExpTemplet>.Load(from e in NKMTempletLoader<NKMOperatorExpData>.LoadGroup("AB_SCRIPT_UNIT_DATA", "LUA_OPERATOR_EXP_TEMPLET", "m_OperatorExpTable", NKMOperatorExpData.LoadFromLua)
			select new NKMOperatorExpTemplet((NKM_UNIT_GRADE)e.Key, e.Value), null);
		NKMTempletContainer<NKMOperatorSkillTemplet>.Load("AB_SCRIPT_UNIT_DATA", "LUA_OPERATOR_SKILL_TEMPLET", "m_OperatorSkillTable", NKMOperatorSkillTemplet.LoadFromLua, (NKMOperatorSkillTemplet e) => e.m_OperSkillStrID);
		NKMTempletContainer<NKMOperatorRandomPassiveGroupTemplet>.Load(from e in NKMTempletLoader<NKMOperatorRandomPassiveTemplet>.LoadGroup("AB_SCRIPT_UNIT_DATA", "LUA_OPERATOR_RANDOM_PASSIVE_TEMPLET", "m_OperatorRandomPassiveTable", NKMOperatorRandomPassiveTemplet.LoadFromLua)
			select new NKMOperatorRandomPassiveGroupTemplet(e.Key, e.Value), null);
		NKMTempletContainer<NKCStatInfoTemplet>.Load("AB_SCRIPT", "LUA_STAT_INFO_TEMPLET", "STAT_INFO_TEMPLET", NKCStatInfoTemplet.LoadFromLUA);
		NKCStatInfoTemplet.MakeGroups();
		NKMTempletContainer<NKMRecallTemplet>.Load("AB_SCRIPT", "LUA_RECALL_TEMPLET", "RECALL_UNIT_TEMPLET", NKMRecallTemplet.LoadFromLUA);
		NKMTempletContainer<NKMRecallUnitExchangeTemplet>.Load("AB_SCRIPT", "LUA_RECALL_EXCHANGE_UNIT_LIST", "RECALL_EXCHANGE_UNIT_LIST", NKMRecallUnitExchangeTemplet.LoadFromLUA);
		NKMTempletContainer<NKMEventWechatCouponTemplet>.Load("AB_SCRIPT", "LUA_WECHAT_COUPON_TEMPLET", "WECHAT_COUPON_TEMPLET", NKMEventWechatCouponTemplet.LoadFromLua);
		NKMTempletContainer<NKMPhaseTemplet>.Load("AB_SCRIPT", "LUA_PHASE_TEMPLET", "PHASE_TEMPLET", NKMPhaseTemplet.LoadFromLua, (NKMPhaseTemplet e) => e.StrId);
		NKMPhaseGroupTemplet.LoadFromLua();
		NKMTempletContainer<NKMDefenceRankRewardTemplet>.Load("AB_SCRIPT", "LUA_DEFENCE_RANK_REWARD_TEMPLET", "DEFENCE_RANK_REWARD_TEMPLET", NKMDefenceRankRewardTemplet.LoadFromLua);
		NKMTempletContainer<NKMDefenceTemplet>.Load("AB_SCRIPT", "LUA_DEFENCE_TEMPLET", "DEFENCE_TEMPLET", NKMDefenceTemplet.LoadFromLUA);
		NKMKillCountTemplet.LoadFromLua();
		NKMUnitMissionTemplet.LoadFromLua();
		NKCVoiceActorNameTemplet.LoadFromLua();
		NKCVoiceActorStringTemplet.LoadFromLua();
		NKMPotentialOptionGroupTemplet.LoadFromLua();
		NKCItemDropInfoTemplet.LoadFromLua();
		NKMTempletContainer<NKMRaidSeasonTemplet>.Load("AB_SCRIPT", "LUA_RAID_SEASON_TEMPLET", "RAID_SEASON_TEMPLET", NKMRaidSeasonTemplet.LoadFromLua);
		NKMTempletContainer<NKMRaidSeasonRewardTemplet>.Load("AB_SCRIPT", "LUA_RAID_SEASON_REWARD_TEMPLET", "REWARD_BOARD_TEMPLET", NKMRaidSeasonRewardTemplet.LoadFromLua);
		NKMTempletContainer<NKMADTemplet>.Load("AB_SCRIPT", "LUA_AD_TEMPLET", "AD_TEMPLET", NKMADTemplet.LoadFromLua);
		NKMTrimIntervalTemplet.Load("AB_SCRIPT", "LUA_TRIM_INTERVAL");
		NKMTempletContainer<NKMTrimTemplet>.Load("AB_SCRIPT", "LUA_TRIM_TEMPLET", "TRIM_TEMPLET", NKMTrimTemplet.LoadFromLua);
		NKMTrimDungeonTemplet.Load("AB_SCRIPT", "LUA_TRIM_DUNGEON");
		NKMTrimCombatPenaltyTemplet.Load("AB_SCRIPT", "LUA_TRIM_COMBAT_PENALTY");
		NKMTrimPointTemplet.Load("AB_SCRIPT", "LUA_TRIM_POINT");
		NKCTrimRewardTemplet.Load("AB_SCRIPT", "LUA_TRIM_REWARD_CL");
		NKMEventCollectionTemplet.LoadFromLua();
		NKMEventCollectionMergeTemplet.LoadFromLua();
		NKMTempletContainer<NKMEventCollectionIndexTemplet>.Load("AB_SCRIPT", "LUA_EVENT_COLLECTION_INDEX_TEMPLET", "EVENT_COLLECTION_INDEX_TEMPLET", NKMEventCollectionIndexTemplet.LoadFromLua);
		NKMTempletContainer<NKCEventPaybackTemplet>.Load("AB_SCRIPT", "LUA_EVENT_PAYBACK_TEMPLET", "EVENT_PAYBACK_TEMPLET", NKCEventPaybackTemplet.LoadFromLUA);
		NKMTempletContainer<NKCLobbyEventIndexTemplet>.Load("AB_SCRIPT", "LUA_EVENT_LOBBY_INDEX_TEMPLET", "EVENT_LOBBY_INDEX_TEMPLET", NKCLobbyEventIndexTemplet.LoadFromLUA);
		NKMTempletContainer<NKMPointExchangeTemplet>.Load("AB_SCRIPT", "LUA_POINTEXCHANGE_TEMPLET", "POINTEXCHANGE_TEMPLET", NKMPointExchangeTemplet.LoadFromLua);
		NKMTempletContainer<NKMEventPvpRewardTemplet>.Load("AB_SCRIPT", "LUA_PVP_EVENTMATCH_REWARD", "PVP_EVENTMATCH_REWARD", NKMEventPvpRewardTemplet.LoadFromLua);
		NKMTempletContainer<NKMEventPvpSeasonTemplet>.Load("AB_SCRIPT", "LUA_PVP_EVENTMATCH_SEASON", "PVP_EVENTMATCH_SEASON", NKMEventPvpSeasonTemplet.LoadFromLua);
		NKMTempletContainer<NKCGauntletKeywordTemplet>.Load("AB_SCRIPT", "LUA_PVP_KEYWORD", "PVP_KEYWORD", NKCGauntletKeywordTemplet.LoadFromLua);
		NKMTempletContainer<NKCGauntletKeywordSlotTemplet>.Load("AB_SCRIPT", "LUA_PVP_KEYWORD_SLOT", "PVP_KEYWORD_SLOT", NKCGauntletKeywordSlotTemplet.LoadFromLua);
		NKMTempletContainer<NKMPostTemplet>.Load("AB_SCRIPT", "LUA_POST_TEMPLET", "POST_TEMPLET", NKMPostTemplet.LoadFromLua);
		NKCSearchKeywordTemplet.LoadFromLua();
		NKCRearmamentUtil.Init();
		NKMTempletContainer<NKMShipLimitBreakTemplet>.Load("AB_SCRIPT", "LUA_SHIP_LIMITBREAK_TEMPLET", "SHIP_LIMITBREAK_TEMPLET", NKMShipLimitBreakTemplet.LoadFromLUA);
		NKMShipModuleGroupTemplet.LoadFromLua();
		NKMTempletContainer<NKMShipCommandModuleTemplet>.Load("AB_SCRIPT", "LUA_COMMANDMODULE_TEMPLET", "COMMANDMODULE_TEMPLET", NKMShipCommandModuleTemplet.LoadFromLUA);
		NKMTempletContainer<NKCMonsterTagTemplet>.Load("AB_SCRIPT", "LUA_MONSTER_TAG_TEMPLET", "MONSTER_TAG", NKCMonsterTagTemplet.LoadFromLUA);
		NKMTempletContainer<NKCMonsterTagInfoTemplet>.Load("AB_SCRIPT", "LUA_MONSTER_TAG_INFO_TEMPLET", "MONSTER_TAG_INFO", NKCMonsterTagInfoTemplet.LoadFromLUA);
		NKMTempletContainer<NKCBGMInfoTemplet>.Load("AB_SCRIPT", "LUA_BGM_INFO_TEMPLETE", "BGM_INFO_TEMPLETE", NKCBGMInfoTemplet.LoadFromLUA);
		NKMTempletContainer<NKMUnitStatusTemplet>.Load("AB_SCRIPT", "LUA_UNIT_STATUS_TEMPLET", "UNIT_STATUS_TEMPLET", NKMUnitStatusTemplet.LoadFromLua);
		NKMTacticUpdateTemplet.LoadFromLua();
		NKMTempletContainer<NKCEquipRecommendListTemplet>.Load("AB_SCRIPT", "LUA_EQUIP_RECOMMEND_LIST", "EQUIP_RECOMMEND_LIST", NKCEquipRecommendListTemplet.LoadFromLua);
		NKMTempletContainer<NKCUnitEquipRecommendTemplet>.Load("AB_SCRIPT", "LUA_UNIT_EQUIP_RECOMMEND", "UNIT_EQUIP_RECOMMEND", NKCUnitEquipRecommendTemplet.LoadFromLua);
		NKMTempletContainer<NKMResetCounterGroupTemplet>.Load("AB_SCRIPT", "LUA_RESET_COUNT_TEMPLET", "RESET_COUNTER", NKMResetCounterGroupTemplet.LoadFromLua);
		NKMTempletContainer<NKMUnitReactorTemplet>.Load("AB_SCRIPT", "LUA_REACTOR_TEMPLET", "REACTOR_TEMPLET", NKMUnitReactorTemplet.LoadFromLua);
		NKMTempletContainer<NKMReactorSkillTemplet>.Load("AB_SCRIPT", "LUA_REACTOR_SKILL_TEMPLET", "REACTOR_SKILL_TEMPLET", NKMReactorSkillTemplet.LoadFromLua);
		NKMTempletContainer<NKMTitleTemplet>.Load("AB_SCRIPT", "LUA_USER_TITLE_TEMPLET", "USER_TITLE_TEMPLET", NKMTitleTemplet.LoadFromLuaEx);
		NKMTempletContainer<NKMEventRaceTemplet>.Load("AB_SCRIPT", "LUA_EVENT_RACE_TEMPLET", "EVENT_RACE_TEMPLET", NKMEventRaceTemplet.LoadFromLua);
		NKMTournamentPredictRewardGroupTemplet.LoadFromLua();
		NKMTempletContainer<NKMTournamentRankRewardTemplet>.Load("AB_SCRIPT", "LUA_TOURNAMENT_RANK_REWARD", "TOURNAMENT_RANK_REWARD", NKMTournamentRankRewardTemplet.LoadFromLua);
		NKMTempletContainer<NKMTournamentTemplet>.Load("AB_SCRIPT", "LUA_TOURNAMENT_TEMPLET", "TOURNAMENT_TEMPLET", NKMTournamentTemplet.LoadFromLua);
		NKMPrecisionWeightTemplet.LoadFromLuaForClient();
		NKMTempletContainer<NKMDefenceScoreRewardTemplet>.Load("AB_SCRIPT", "LUA_DEFENCE_SCORE_REWARD_TEMPLET", "DEFENCE_SCORE_REWARD_TEMPLET", NKMDefenceScoreRewardTemplet.LoadFromLua);
		NKMTempletContainer<NKMCustomBoxTemplet>.Load("AB_SCRIPT_ITEM_TEMPLET", "LUA_CUSTOM_BOX_TEMPLET", "CUSTOM_BOX", NKMCustomBoxTemplet.LoadFromLua);
		NKCKeywordTemplet.Load();
		NKMTempletContainer<NKCUserTitleCategoryTemplet>.Load("AB_SCRIPT", "LUA_USER_TITLE_CATEGORY_TEMPLET", "USER_TITLE_CATEGORY_TEMPLET", NKCUserTitleCategoryTemplet.LoadFromLua);
		NKMTempletContainer<NKMScoreRewardTemplet>.Load("AB_SCRIPT", "LUA_SCORE_REWARD_TEMPLET", "SCORE_REWARD_TEMPLET", NKMScoreRewardTemplet.LoadFromLUA);
		NKMTempletContainer<NKMMatchTenTemplet>.Load("AB_SCRIPT", "LUA_MATCH_TEN_TEMPLET", "MATCH_TEN_TEMPLET", NKMMatchTenTemplet.LoadFromLUA);
		NKMTempletContainer<NKCUnitTagInfoTemplet>.Load("AB_SCRIPT", "LUA_UNIT_TAG_INFO_TEMPLET", "m_UnitTagInfoTemplet", NKCUnitTagInfoTemplet.LoadFromLUA, (NKCUnitTagInfoTemplet e) => e.strID);
		NKMMiniGameManager.LoadFromLua();
		NKCTempletContainerUtil.InvokeJoin();
		ShopTabTempletContainer.Join();
		NKMPvpCommonConst.Instance.Join();
		NKCPVPManager.Join();
		GuildDungeonTempletManager.Join();
		NKCLoginCutSceneManager.Join();
		NKMTempletContainer<NKMEventWechatCouponTemplet>.Join();
		NKMBattleConditionManager.Join();
		NKMTempletContainer<NKMUnitListTemplet>.Join();
		NKMMiniGameManager.Join();
		if (NKMOpenTagManager.TagCount > 0)
		{
			NKCTempletContainerUtil.InvokeValidate();
		}
		NKMMissionManager.CheckValidation();
		NKMMiniGameManager.Validate();
		NKCDiveManager.Initialize();
		NKMEpisodeMgr.Initialize();
		NKMEpisodeMgr.SortEpisodeTemplets();
		NKCLeaderBoardManager.Initialize();
		NKCGuildManager.Initialize();
		NKCChatManager.Initialize();
		NKMConst.Validate();
		NKCUnitMissionManager.Init();
		NKMTrimDungeonTemplet.Validate();
		NKMTempletContainer<NKMTrimTemplet>.Validate();
		NKCTournamentManager.Initialize();
	}

	public static void QuitGame()
	{
		Log.Debug("[QuitGame]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCMain.cs", 909);
		Application.Quit();
	}
}
