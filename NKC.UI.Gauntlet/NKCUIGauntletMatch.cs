using ClientPacket.Common;
using ClientPacket.Pvp;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletMatch : NKCUIBase
{
	public enum NKC_GAUNTLET_MATCH_STATE
	{
		NGMS_NONE,
		NGMS_SEARCHING,
		NGMS_SEARCH_COMPLETE
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_MATCH";

	private bool m_bInit;

	public NKCUICharacterView m_NKCUICharacterView_1P;

	public NKCUILeagueTier m_NKCUILeagueTier_1P;

	public Text m_lb_1P_Score;

	public Text m_lb_1P_LV;

	public Text m_lb_1P_Name;

	public GameObject m_obj_1P_WinStreak;

	public Text m_lb_1P_WinStreak;

	public GameObject m_obj_1P_DemotionAlert;

	public GameObject m_Guild_1P;

	public NKCUIGuildBadge m_GuildBadge_1P;

	public Text m_GuildName_1P;

	public NKCUIComTitlePanel m_titlePanel_1P;

	public GameObject m_obj_2P_Searching;

	public GameObject m_obj_2P_Info;

	public GameObject m_obj_2P_TierIcon;

	public NKCUIComStateButton m_csbtn_2P_MatchCancel;

	public NKCUICharacterView m_NKCUICharacterView_2P;

	public NKCUILeagueTier m_NKCUILeagueTier_2P;

	public Text m_lb_2P_Score;

	public Text m_lb_2P_LV;

	public Text m_lb_2P_Name;

	public GameObject m_obj_2P_WinStreak;

	public Text m_lb_2P_WinStreak;

	public GameObject m_obj_2P_DemotionAlert;

	public Animator m_amtor2PChar;

	public GameObject m_Guild_2P;

	public NKCUIGuildBadge m_GuildBadge_2P;

	public Text m_GuildName_2P;

	public NKCUIComTitlePanel m_titlePanel_2P;

	public GameObject m_objCenterReady;

	public GameObject m_objCenterVS;

	[Header("Fallback BG")]
	public GameObject m_objBGFallBack;

	private NKC_GAUNTLET_MATCH_STATE m_NKC_GAUNTLET_MATCH_STATE;

	private static byte m_sSelectDeckIndex;

	private const float VS_SHOW_TIME = 3f;

	private float m_fVSShowElapsedTime;

	private bool m_bVSShowTime;

	private NKM_GAME_TYPE m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_PVP_RANK;

	private const int WIN_STREAK_SHOW_COUNT = 2;

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public NKC_GAUNTLET_MATCH_STATE Get_NKC_GAUNTLET_MATCH_STATE()
	{
		return m_NKC_GAUNTLET_MATCH_STATE;
	}

	public static void SetDeckIndex(byte index)
	{
		m_sSelectDeckIndex = index;
	}

	public static NKCAssetResourceData OpenInstanceAsync()
	{
		return NKCUIBase.OpenInstanceAsync<NKCUIGauntletMatch>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_MATCH");
	}

	public static bool CheckInstanceLoaded(NKCAssetResourceData loadResourceData, out NKCUIGauntletMatch retVal)
	{
		return NKCUIBase.CheckInstanceLoaded<NKCUIGauntletMatch>(loadResourceData, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), out retVal);
	}

	public void CloseInstance()
	{
		NKCAssetResourceManager.CloseResource("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_MATCH");
		Object.Destroy(base.gameObject);
	}

	public void InitUI()
	{
		if (!m_bInit)
		{
			m_csbtn_2P_MatchCancel.PointerClick.RemoveAllListeners();
			m_csbtn_2P_MatchCancel.PointerClick.AddListener(SendMatchCancelREQ);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			m_bInit = true;
		}
	}

	private void SendMatchCancelREQ()
	{
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			NKMPacket_LEAGUE_PVP_MATCH_CANCEL_REQ packet = new NKMPacket_LEAGUE_PVP_MATCH_CANCEL_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
		else if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_EVENT)
		{
			NKCPacketSender.Send_NKMPacket_EVENT_PVP_GAME_MATCH_CANCEL_REQ();
		}
		else
		{
			NKMPacket_PVP_GAME_MATCH_CANCEL_REQ packet2 = new NKMPacket_PVP_GAME_MATCH_CANCEL_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet2, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}

	public void Open(NKM_GAME_TYPE eNKM_GAME_TYPE)
	{
		m_NKM_GAME_TYPE = eNKM_GAME_TYPE;
		m_bVSShowTime = false;
		m_fVSShowElapsedTime = 0f;
		switch (eNKM_GAME_TYPE)
		{
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
			m_NKC_GAUNTLET_MATCH_STATE = NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCH_COMPLETE;
			break;
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
		{
			m_NKC_GAUNTLET_MATCH_STATE = NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCHING;
			NKMPacket_LEAGUE_PVP_MATCH_REQ nKMPacket_LEAGUE_PVP_MATCH_REQ = new NKMPacket_LEAGUE_PVP_MATCH_REQ();
			nKMPacket_LEAGUE_PVP_MATCH_REQ.selectDeckIndex = m_sSelectDeckIndex;
			nKMPacket_LEAGUE_PVP_MATCH_REQ.gameType = eNKM_GAME_TYPE;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LEAGUE_PVP_MATCH_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
			break;
		}
		case NKM_GAME_TYPE.NGT_PVP_EVENT:
			m_NKC_GAUNTLET_MATCH_STATE = NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCHING;
			NKCPacketSender.Send_NKMPacket_EVENT_PVP_GAME_MATCH_REQ(NKCUtil.FindPVPSeasonIDForEvent(), NKCEventPvpMgr.EventDeckData, eNKM_GAME_TYPE);
			break;
		default:
		{
			m_NKC_GAUNTLET_MATCH_STATE = NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCHING;
			NKMPacket_PVP_GAME_MATCH_REQ nKMPacket_PVP_GAME_MATCH_REQ = new NKMPacket_PVP_GAME_MATCH_REQ();
			nKMPacket_PVP_GAME_MATCH_REQ.selectDeckIndex = m_sSelectDeckIndex;
			nKMPacket_PVP_GAME_MATCH_REQ.gameType = eNKM_GAME_TYPE;
			if (eNKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_RANK)
			{
				nKMPacket_PVP_GAME_MATCH_REQ.usingBot = PlayerPrefs.GetInt(NKCUIGauntletLobbyRightSideRank.RankBotMatchLocalSaveKey, 0) == 1;
			}
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_PVP_GAME_MATCH_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
			break;
		}
		}
		NKCUtil.SetGameobjectActive(m_obj_1P_DemotionAlert, bValue: false);
		NKCUtil.SetGameobjectActive(m_obj_2P_DemotionAlert, bValue: false);
		SetBasicUI();
		Set_1P_UI();
		bool flag = NKCScenManager.GetScenManager().GetGameOptionData()?.UseVideoTexture ?? false;
		NKCUtil.SetGameobjectActive(m_objBGFallBack, !flag);
		UIOpened();
	}

	public void SetTarget(AsyncPvpTarget target)
	{
		NKM_GAME_TYPE nKM_GAME_TYPE = m_NKM_GAME_TYPE;
		if ((nKM_GAME_TYPE == NKM_GAME_TYPE.NGT_ASYNC_PVP || nKM_GAME_TYPE - 20 <= NKM_GAME_TYPE.NGT_PRACTICE) && m_NKC_GAUNTLET_MATCH_STATE == NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCH_COMPLETE)
		{
			Set_2P_UI(target);
			m_bVSShowTime = true;
			m_fVSShowElapsedTime = 0f;
		}
	}

	public void SetTarget(NpcPvpTarget target)
	{
		NKM_GAME_TYPE nKM_GAME_TYPE = m_NKM_GAME_TYPE;
		if ((nKM_GAME_TYPE == NKM_GAME_TYPE.NGT_ASYNC_PVP || nKM_GAME_TYPE - 20 <= NKM_GAME_TYPE.NGT_PRACTICE) && m_NKC_GAUNTLET_MATCH_STATE == NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCH_COMPLETE)
		{
			Set_2P_UI(target);
			m_bVSShowTime = true;
			m_fVSShowElapsedTime = 0f;
		}
	}

	private void Update()
	{
		if (m_bVSShowTime)
		{
			m_fVSShowElapsedTime += Time.deltaTime;
			if (m_fVSShowElapsedTime >= 3f)
			{
				m_fVSShowElapsedTime = 0f;
				m_bVSShowTime = false;
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
			}
		}
	}

	private void Set_1P_UI()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_EVENT)
		{
			NKMDungeonEventDeckTemplet nKMDungeonEventDeckTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet()?.EventDeckTemplet;
			if (NKCEventPvpMgr.EventDeckData != null && nKMDungeonEventDeckTemplet != null)
			{
				NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = nKMDungeonEventDeckTemplet.GetUnitSlot(NKCEventPvpMgr.EventDeckData.m_LeaderIndex);
				if (NKCEventPvpMgr.IsDeteminedSlotType(unitSlot.m_eType))
				{
					m_NKCUICharacterView_1P.SetCharacterIllust(unitSlot.m_ID, unitSlot.m_SkinID, bAsync: false, bEnableBackground: false);
				}
				else if (unitSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
				{
					m_NKCUICharacterView_1P.CloseCharacterIllust();
				}
				else
				{
					NKCEventPvpMgr.EventDeckData.m_dicUnit.TryGetValue(NKCEventPvpMgr.EventDeckData.m_LeaderIndex, out var value);
					NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(value);
					m_NKCUICharacterView_1P.SetCharacterIllust(unitFromUID, bAsync: false, bEnableBackground: false);
				}
				m_NKCUICharacterView_1P.PlayEffect(NKCUICharacterView.EffectType.VersusMaskL);
			}
			else if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_EVENT)
			{
				NKMUserData myUserData2 = NKCScenManager.GetScenManager().GetMyUserData();
				if (myUserData2 != null)
				{
					m_NKCUICharacterView_1P.SetCharacterIllust(myUserData2.UserProfileData.commonProfile.mainUnitId, myUserData2.UserProfileData.commonProfile.mainUnitSkinId, bAsync: false, bEnableBackground: false);
				}
			}
		}
		else
		{
			NKMDeckIndex deckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_PVP, m_sSelectDeckIndex);
			NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(deckIndex);
			if (deckData != null)
			{
				NKMUnitData deckUnitByIndex = myUserData.m_ArmyData.GetDeckUnitByIndex(deckIndex, deckData.m_LeaderIndex);
				if (deckUnitByIndex != null)
				{
					m_NKCUICharacterView_1P.SetCharacterIllust(deckUnitByIndex, bAsync: false, bEnableBackground: false);
					m_NKCUICharacterView_1P.PlayEffect(NKCUICharacterView.EffectType.VersusMaskL);
				}
			}
		}
		m_lb_1P_Name.text = NKCUtilString.GetUserNickname(myUserData.m_UserNickName, bOpponent: false);
		m_lb_1P_LV.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, myUserData.UserLevel);
		if (NKCGuildManager.HasGuild())
		{
			NKCUtil.SetGameobjectActive(m_Guild_1P, bValue: true);
			m_GuildBadge_1P?.SetData(NKCGuildManager.MyGuildData.badgeId, bOpponent: false);
			NKCUtil.SetLabelText(m_GuildName_1P, NKCUtilString.GetUserGuildName(NKCGuildManager.MyGuildData.name, bOpponent: false));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_Guild_1P, bValue: false);
		}
		PvpState pvPDataByGameType = GetPvPDataByGameType(m_NKM_GAME_TYPE);
		int num = NKCPVPManager.FindPvPSeasonID(m_NKM_GAME_TYPE, NKCSynchronizedTime.GetServerUTCTime());
		if (pvPDataByGameType != null)
		{
			int score = 0;
			if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_EVENT)
			{
				m_NKCUILeagueTier_1P.SetDisableNormalTier();
				m_NKCUILeagueTier_1P.SetEventmatchIcon(m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_EVENT);
			}
			else if (pvPDataByGameType.SeasonID != num)
			{
				score = NKCPVPManager.GetResetScore(pvPDataByGameType.SeasonID, pvPDataByGameType.Score, m_NKM_GAME_TYPE);
				m_NKCUILeagueTier_1P.SetUI(NKCPVPManager.GetTierIconByScore(m_NKM_GAME_TYPE, num, score), NKCPVPManager.GetTierNumberByScore(m_NKM_GAME_TYPE, num, score));
			}
			else
			{
				score = pvPDataByGameType.Score;
				m_NKCUILeagueTier_1P.SetUI(NKCPVPManager.GetTierIconByTier(m_NKM_GAME_TYPE, num, pvPDataByGameType.LeagueTierID), NKCPVPManager.GetTierNumberByTier(m_NKM_GAME_TYPE, num, pvPDataByGameType.LeagueTierID));
			}
			int winStreak = pvPDataByGameType.WinStreak;
			m_lb_1P_Score.text = score.ToString();
			bool flag = m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE;
			NKCUtil.SetGameobjectActive(m_lb_1P_Score, !flag && m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_PVP_EVENT);
			NKCUtil.SetGameobjectActive(m_obj_1P_WinStreak, winStreak >= 2 && !flag);
			if (winStreak >= 2)
			{
				m_lb_1P_WinStreak.text = string.Format(NKCUtilString.GET_STRING_GAUNTLET_WIN_STREAK_ONE_PARAM, winStreak);
			}
		}
		else
		{
			m_NKCUILeagueTier_1P.SetDisableNormalTier();
			m_NKCUILeagueTier_1P.SetEventmatchIcon(m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_EVENT);
			NKCUtil.SetGameobjectActive(m_lb_1P_Score, bValue: false);
			NKCUtil.SetGameobjectActive(m_obj_1P_WinStreak, bValue: false);
		}
		m_titlePanel_1P?.SetData((myUserData.UserProfileData != null) ? myUserData.UserProfileData.commonProfile : null);
	}

	private void Set_2P_UI(NKMGameData gameData)
	{
		NKM_TEAM_TYPE nKM_TEAM_TYPE = ((NKCScenManager.CurrentUserData() == null) ? NKM_TEAM_TYPE.NTT_A1 : gameData.GetTeamType(NKCScenManager.CurrentUserData().m_UserUID));
		NKMGameTeamData nKMGameTeamData = null;
		nKMGameTeamData = ((nKM_TEAM_TYPE != NKM_TEAM_TYPE.NTT_A1) ? gameData.m_NKMGameTeamDataA : gameData.m_NKMGameTeamDataB);
		bool flag = false;
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_EVENT)
		{
			NKMDungeonEventDeckTemplet nKMDungeonEventDeckTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet()?.EventDeckTemplet;
			if (nKMDungeonEventDeckTemplet != null && nKMDungeonEventDeckTemplet.HasRandomUnitSlot())
			{
				flag = true;
			}
		}
		if (flag)
		{
			m_NKCUICharacterView_2P.CloseCharacterIllust();
		}
		else
		{
			m_NKCUICharacterView_2P.SetCharacterIllust(nKMGameTeamData.GetLeaderUnitData(), bAsync: false, bEnableBackground: false);
			m_NKCUICharacterView_2P.PlayEffect(NKCUICharacterView.EffectType.VersusMaskR);
		}
		m_lb_2P_Name.text = NKCUtilString.GetUserNickname(nKMGameTeamData.m_UserNickname, bOpponent: true);
		m_lb_2P_LV.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, nKMGameTeamData.m_UserLevel);
		if (nKMGameTeamData.guildSimpleData != null && nKMGameTeamData.guildSimpleData.guildUid > 0)
		{
			NKCUtil.SetGameobjectActive(m_Guild_2P, bValue: true);
			m_GuildBadge_2P?.SetData(nKMGameTeamData.guildSimpleData.badgeId, bOpponent: true);
			NKCUtil.SetLabelText(m_GuildName_2P, NKCUtilString.GetUserGuildName(nKMGameTeamData.guildSimpleData.guildName, bOpponent: true));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_Guild_2P, bValue: false);
		}
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_RANK || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			m_lb_2P_Score.text = nKMGameTeamData.m_Score.ToString();
			NKCUtil.SetGameobjectActive(m_lb_2P_Score, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lb_2P_Score, bValue: false);
		}
		int num = NKCPVPManager.FindPvPSeasonID(m_NKM_GAME_TYPE, NKCSynchronizedTime.GetServerUTCTime());
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_EVENT)
		{
			m_NKCUILeagueTier_2P.SetDisableNormalTier();
			m_NKCUILeagueTier_2P.SetEventmatchIcon(value: true);
		}
		else
		{
			m_NKCUILeagueTier_2P.SetUI(NKCPVPManager.GetTierIconByTier(m_NKM_GAME_TYPE, num, nKMGameTeamData.m_Tier), NKCPVPManager.GetTierNumberByTier(m_NKM_GAME_TYPE, num, nKMGameTeamData.m_Tier));
		}
		NKCUtil.SetGameobjectActive(m_obj_2P_WinStreak, nKMGameTeamData.m_WinStreak >= 2);
		m_lb_2P_WinStreak.text = string.Format(NKCUtilString.GET_STRING_GAUNTLET_WIN_STREAK_ONE_PARAM, nKMGameTeamData.m_WinStreak);
		m_titlePanel_2P?.SetData(nKMGameTeamData.m_userCommonProfile);
		PvpState pvpState = null;
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_RANK)
		{
			pvpState = NKCScenManager.GetScenManager().GetMyUserData().m_PvpData;
		}
		else if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			pvpState = NKCScenManager.GetScenManager().GetMyUserData().m_LeagueData;
		}
		if (pvpState != null)
		{
			int finalPVPScore = NKCUtil.GetFinalPVPScore(pvpState, m_NKM_GAME_TYPE);
			if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_RANK)
			{
				NKMPvpRankTemplet rankTempletByScore = NKCPVPManager.GetRankTempletByScore(m_NKM_GAME_TYPE, num, finalPVPScore);
				NKMPvpRankTemplet rankTempletByTier = NKCPVPManager.GetRankTempletByTier(m_NKM_GAME_TYPE, num, pvpState.LeagueTierID);
				if (NKCUtil.IsPVPDemotionAlert(rankTempletByScore, rankTempletByTier, finalPVPScore))
				{
					NKCUtil.SetGameobjectActive(m_obj_1P_DemotionAlert, bValue: true);
				}
			}
			else if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
			{
				NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(num);
				NKMLeaguePvpRankTemplet byScore = nKMLeaguePvpRankSeasonTemplet.RankGroup.GetByScore(finalPVPScore);
				NKMLeaguePvpRankTemplet byTier = nKMLeaguePvpRankSeasonTemplet.RankGroup.GetByTier(pvpState.LeagueTierID);
				if (NKCUtil.IsPVPDemotionAlert(byScore, byTier, finalPVPScore))
				{
					NKCUtil.SetGameobjectActive(m_obj_1P_DemotionAlert, bValue: true);
				}
			}
		}
		int score = nKMGameTeamData.m_Score;
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_RANK)
		{
			NKMPvpRankTemplet rankTempletByScore2 = NKCPVPManager.GetRankTempletByScore(m_NKM_GAME_TYPE, num, score);
			NKMPvpRankTemplet rankTempletByTier2 = NKCPVPManager.GetRankTempletByTier(m_NKM_GAME_TYPE, num, nKMGameTeamData.m_Tier);
			if (NKCUtil.IsPVPDemotionAlert(rankTempletByScore2, rankTempletByTier2, score))
			{
				NKCUtil.SetGameobjectActive(m_obj_2P_DemotionAlert, bValue: true);
			}
		}
		else if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet2 = NKMLeaguePvpRankSeasonTemplet.Find(num);
			NKMLeaguePvpRankTemplet byScore2 = nKMLeaguePvpRankSeasonTemplet2.RankGroup.GetByScore(score);
			NKMLeaguePvpRankTemplet byTier2 = nKMLeaguePvpRankSeasonTemplet2.RankGroup.GetByTier(nKMGameTeamData.m_Tier);
			if (NKCUtil.IsPVPDemotionAlert(byScore2, byTier2, score))
			{
				NKCUtil.SetGameobjectActive(m_obj_2P_DemotionAlert, bValue: true);
			}
		}
		m_amtor2PChar.Play("NKM_UI_GAUNTLET_VERSUS_2P_UNIT_INTRO");
	}

	private void Set_2P_UI(AsyncPvpTarget target)
	{
		if (target != null)
		{
			Set_2P_UI(target.asyncDeck, target.userNickName, target.userLevel, target.guildData, target.score, target.tier, target.titleId);
		}
	}

	private void Set_2P_UI(NpcPvpTarget target)
	{
		if (target != null)
		{
			Set_2P_UI(target.asyncDeck, target.userNickName, target.userLevel, null, target.score, target.tier, 0);
		}
	}

	private void Set_2P_UI(NKMAsyncDeckData asyncDeck, string userNickName, int userLv, NKMGuildSimpleData guildData, int score, int tier, int titleId)
	{
		NKMAsyncUnitData asyncUnitData = null;
		for (int i = 0; i < asyncDeck.units.Count; i++)
		{
			if (asyncDeck.units[i] != null && asyncDeck.units[i].unitId > 0)
			{
				asyncUnitData = asyncDeck.units[i];
				break;
			}
		}
		NKMUnitData unitData = NKMDungeonManager.MakeUnitData(asyncUnitData, -1L);
		m_NKCUICharacterView_2P.SetCharacterIllust(unitData, bAsync: false, bEnableBackground: false);
		m_NKCUICharacterView_2P.PlayEffect(NKCUICharacterView.EffectType.VersusMaskR);
		m_lb_2P_Name.text = NKCUtilString.GetUserNickname(userNickName, bOpponent: true);
		m_lb_2P_LV.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, userLv);
		if (guildData != null && guildData.guildUid > 0)
		{
			NKCUtil.SetGameobjectActive(m_Guild_2P, bValue: true);
			m_GuildBadge_2P?.SetData(guildData.badgeId, bOpponent: true);
			NKCUtil.SetLabelText(m_GuildName_2P, NKCUtilString.GetUserGuildName(guildData.guildName, bOpponent: true));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_Guild_2P, bValue: false);
		}
		NKCUtil.SetLabelText(m_lb_2P_Score, score.ToString());
		NKCUtil.SetGameobjectActive(m_lb_2P_Score, bValue: true);
		int seasonID = NKCPVPManager.FindPvPSeasonID(m_NKM_GAME_TYPE, NKCSynchronizedTime.GetServerUTCTime());
		m_NKCUILeagueTier_2P.SetUI(NKCPVPManager.GetTierIconByTier(m_NKM_GAME_TYPE, seasonID, tier), NKCPVPManager.GetTierNumberByTier(m_NKM_GAME_TYPE, seasonID, tier));
		NKCUtil.SetGameobjectActive(m_obj_2P_WinStreak, bValue: false);
		m_titlePanel_2P?.SetData(titleId);
		m_amtor2PChar.Play("NKM_UI_GAUNTLET_VERSUS_2P_UNIT_INTRO");
	}

	private void SetBasicUI()
	{
		NKCUtil.SetGameobjectActive(m_objCenterReady, m_NKC_GAUNTLET_MATCH_STATE == NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCHING);
		NKCUtil.SetGameobjectActive(m_objCenterVS, m_NKC_GAUNTLET_MATCH_STATE == NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCH_COMPLETE);
		NKCUtil.SetGameobjectActive(m_obj_2P_Searching, m_NKC_GAUNTLET_MATCH_STATE == NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCHING);
		NKCUtil.SetGameobjectActive(m_obj_2P_Info, m_NKC_GAUNTLET_MATCH_STATE == NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCH_COMPLETE);
		NKCUtil.SetGameobjectActive(m_obj_2P_TierIcon, m_NKC_GAUNTLET_MATCH_STATE == NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCH_COMPLETE);
	}

	public void OnRecv(NKMPacket_PVP_GAME_MATCH_CANCEL_ACK cPacket)
	{
		m_NKC_GAUNTLET_MATCH_STATE = NKC_GAUNTLET_MATCH_STATE.NGMS_NONE;
		switch (m_NKM_GAME_TYPE)
		{
		case NKM_GAME_TYPE.NGT_PVP_RANK:
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_MATCH_READY);
			break;
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_ASYNC_READY);
			break;
		}
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_MATCH_CANCEL_ACK cPacket)
	{
		m_NKC_GAUNTLET_MATCH_STATE = NKC_GAUNTLET_MATCH_STATE.NGMS_NONE;
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	public void OnRecv(NKMPacket_EVENT_PVP_GAME_MATCH_CANCEL_ACK cPacket)
	{
		m_NKC_GAUNTLET_MATCH_STATE = NKC_GAUNTLET_MATCH_STATE.NGMS_NONE;
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_ACCEPT_NOT cPacket)
	{
		m_NKC_GAUNTLET_MATCH_STATE = NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCH_COMPLETE;
		m_bVSShowTime = false;
		m_fVSShowElapsedTime = 0f;
	}

	public void OnRecv(NKMPacket_PVP_GAME_MATCH_COMPLETE_NOT cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT)
	{
		m_NKC_GAUNTLET_MATCH_STATE = NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCH_COMPLETE;
		NKM_GAME_TYPE nKM_GAME_TYPE = m_NKM_GAME_TYPE;
		if (nKM_GAME_TYPE != NKM_GAME_TYPE.NGT_ASYNC_PVP && nKM_GAME_TYPE - 20 > NKM_GAME_TYPE.NGT_PRACTICE)
		{
			SetBasicUI();
			Set_2P_UI(cNKMPacket_PVP_GAME_MATCH_COMPLETE_NOT.gameData);
		}
		m_bVSShowTime = true;
		m_fVSShowElapsedTime = 0f;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
		if (m_NKC_GAUNTLET_MATCH_STATE == NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCHING)
		{
			SendMatchCancelREQ();
		}
		NKCEventPvpMgr.EventDeckData = null;
	}

	public override void OnBackButton()
	{
		if (m_NKC_GAUNTLET_MATCH_STATE == NKC_GAUNTLET_MATCH_STATE.NGMS_SEARCHING && !NKMPopUpBox.IsOpenedWaitBox())
		{
			SendMatchCancelREQ();
		}
	}

	private PvpState GetPvPDataByGameType(NKM_GAME_TYPE gameType)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return null;
		}
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
			return myUserData.m_LeagueData;
		case NKM_GAME_TYPE.NGT_PVP_RANK:
			return myUserData.m_PvpData;
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
			return myUserData.m_AsyncData;
		case NKM_GAME_TYPE.NGT_PVP_EVENT:
			return myUserData.m_eventPvpData;
		default:
			return null;
		}
	}

	public bool GetVSShowTime()
	{
		return m_bVSShowTime;
	}
}
