using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletIntro : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_INTRO";

	[Header("랭크전")]
	public NKCUIComStateButton m_csbtnRank;

	public Text m_lbRankSeason;

	public GameObject m_objRankBattleCond;

	public Image m_imgRankBattleCond;

	public Text m_lbRankBattleCondTitle;

	public Text m_lbRankBattleCondDesc;

	public GameObject m_objRankLocked;

	public Text m_lbRankLocked;

	public Image m_imgRankSeasonIcon;

	public NKCUIGauntletKeywordGroup m_rankKeywordGroup;

	[Header("전략전")]
	public NKCUIComStateButton m_csbtnAsync;

	public Text m_lbAsyncSeason;

	public GameObject m_objAsyncBattleCond;

	public Image m_imgAsyncBattleCond;

	public Text m_lbAsyncBattleCondTitle;

	public Text m_lbAsyncBattleCondDesc;

	public Image m_imgAsyncSeasonIcon;

	public NKCUIGauntletKeywordGroup m_asyncKeywordGroup;

	[Header("전략전V2")]
	public NKCUIComStateButton m_csbtnAsyncV2;

	public Text m_lbAsyncSeasonV2;

	public GameObject m_objAsyncBattleCondV2;

	public Image m_imgAsyncBattleCondV2;

	public Text m_lbAsyncBattleCondTitleV2;

	public Text m_lbAsyncBattleCondDescV2;

	public Image m_imgAsyncSeasonIconV2;

	public NKCUIGauntletKeywordGroup m_asyncV2KeywordGroup;

	[Header("리그전")]
	public NKCUIComStateButton m_csbtnLeague;

	public Text m_lbLeagueSeason;

	public GameObject m_objLeagueBattleCond;

	public Image m_imgLeagueBattleCond;

	public Text m_lbLeagueBattleCondTitle;

	public Text m_lbLeagueBattleCondDesc;

	public GameObject m_objLeagueLocked;

	public Text m_lbLeagueLocked;

	public GameObject m_objLeagueOpenCond;

	public Text m_lbLeagueOpenDaysOfWeek;

	public Text m_lbLeagueOpenTime;

	public NKCUIGauntletKeywordGroup m_leagueKeywordGroup;

	[Header("친선전")]
	public NKCUIComStateButton m_csbtnPrivatePvp;

	public NKCUIGauntletKeywordGroup m_privateKeywordGroup;

	[Header("이벤트전")]
	public NKCUIComStateButton m_csbtnEventMatch;

	public Text m_lbEventPvpSeason;

	public Text m_lbEventMatchInterval;

	public NKCUIGauntletKeywordGroup m_eventKeywordGroup;

	public Image m_imgEventSeasonArt;

	public GameObject m_objEventmatchLocked;

	[Header("Fallback BG")]
	public GameObject m_objBGFallBack;

	private bool m_bInit;

	private bool m_bOpenAsyncNewMode;

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override string GuideTempletID => "ARTICLE_PVP_RANK";

	public override List<int> UpsideMenuShowResourceList => new List<int> { 5, 101 };

	public static NKCAssetResourceData OpenInstanceAsync()
	{
		return NKCUIBase.OpenInstanceAsync<NKCUIBaseSceneMenu>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_INTRO");
	}

	public static bool CheckInstanceLoaded(NKCAssetResourceData loadResourceData, out NKCUIGauntletIntro retVal)
	{
		return NKCUIBase.CheckInstanceLoaded<NKCUIGauntletIntro>(loadResourceData, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), out retVal);
	}

	public void CloseInstance()
	{
		int num = NKCAssetResourceManager.CloseResource("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_INTRO");
		Debug.Log($"gauntlet intro close resource retval is {num}");
		Object.Destroy(base.gameObject);
	}

	public void InitUI()
	{
		if (!m_bInit)
		{
			m_csbtnRank.PointerClick.RemoveAllListeners();
			m_csbtnRank.PointerClick.AddListener(OnClickRank);
			m_csbtnRank.m_bGetCallbackWhileLocked = true;
			NKCUtil.SetBindFunction(m_csbtnAsync, OnClickAsync);
			NKCUtil.SetBindFunction(m_csbtnAsyncV2, OnClickAsync);
			m_bOpenAsyncNewMode = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_NEW_MODE);
			NKCUtil.SetGameobjectActive(m_csbtnAsync, !m_bOpenAsyncNewMode);
			NKCUtil.SetGameobjectActive(m_csbtnAsyncV2, m_bOpenAsyncNewMode);
			m_csbtnLeague.PointerClick.RemoveAllListeners();
			m_csbtnLeague.PointerClick.AddListener(OnClickLeague);
			m_csbtnLeague.m_bGetCallbackWhileLocked = true;
			if (m_csbtnPrivatePvp != null)
			{
				m_csbtnPrivatePvp.PointerClick.RemoveAllListeners();
				m_csbtnPrivatePvp.PointerClick.AddListener(OnClickPrivatePvp);
			}
			NKCUtil.SetButtonClickDelegate(m_csbtnEventMatch, OnClickEventBattle);
			if (m_csbtnEventMatch != null)
			{
				m_csbtnEventMatch.m_bGetCallbackWhileLocked = true;
			}
			m_rankKeywordGroup?.Init();
			m_asyncKeywordGroup?.Init();
			m_asyncV2KeywordGroup?.Init();
			m_leagueKeywordGroup?.Init();
			m_privateKeywordGroup?.Init();
			m_eventKeywordGroup?.Init();
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			m_bInit = true;
		}
	}

	public void Open()
	{
		UIOpened();
		NKCScenManager.CurrentUserData();
		NKCUtil.SetGameobjectActive(m_csbtnPrivatePvp, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_FRIENDLY_MODE));
		if (m_csbtnPrivatePvp.gameObject.activeSelf)
		{
			m_privateKeywordGroup?.SetData(NKM_GAME_TYPE.NGT_PVP_PRIVATE);
		}
		NKMPvpRankSeasonTemplet pvpRankSeasonTemplet = NKCPVPManager.GetPvpRankSeasonTemplet(NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime()));
		bool bValue = false;
		if (pvpRankSeasonTemplet != null)
		{
			if (pvpRankSeasonTemplet.IsSeasonLobbyPrefab)
			{
				NKCUtil.SetLabelText(m_lbRankSeason, NKCStringTable.GetString(pvpRankSeasonTemplet.SeasonStrID));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbRankSeason, string.Format(NKCUtilString.GET_STRING_GAUNTLET_SEASON_NUMBERING_NAME, pvpRankSeasonTemplet.SeasonLobbyName));
			}
			bValue = !string.IsNullOrEmpty(pvpRankSeasonTemplet.SeasonIcon);
			NKCUtil.SetImageSprite(m_imgRankSeasonIcon, NKCUtil.GetSpriteBattleConditionICon(pvpRankSeasonTemplet.SeasonIcon));
		}
		SetBattleCondition(pvpRankSeasonTemplet, m_objRankBattleCond, m_imgRankBattleCond, m_lbRankBattleCondTitle, m_lbRankBattleCondDesc);
		NKCUtil.SetGameobjectActive(m_imgRankSeasonIcon, bValue);
		m_rankKeywordGroup?.SetData(NKM_GAME_TYPE.NGT_PVP_RANK);
		if (NKCPVPManager.IsPvpRankUnlocked())
		{
			m_csbtnRank.UnLock();
			NKCUtil.SetGameobjectActive(m_objRankLocked, bValue: false);
		}
		else
		{
			m_csbtnRank.Lock();
			NKCUtil.SetGameobjectActive(m_objRankLocked, bValue: true);
			NKCUtil.SetLabelText(m_lbRankLocked, string.Format(NKCUtilString.GET_STRING_GAUNTLET_OPEN_RANK_MODE, NKMPvpCommonConst.Instance.RANK_PVP_OPEN_POINT));
		}
		if (!m_bOpenAsyncNewMode)
		{
			bool bValue2 = false;
			NKMPvpRankSeasonTemplet pvpAsyncSeasonTemplet = NKCPVPManager.GetPvpAsyncSeasonTemplet(NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime()));
			if (pvpAsyncSeasonTemplet != null)
			{
				if (pvpAsyncSeasonTemplet.IsSeasonLobbyPrefab)
				{
					NKCUtil.SetLabelText(m_lbAsyncSeason, NKCStringTable.GetString(pvpAsyncSeasonTemplet.SeasonStrID));
				}
				else
				{
					NKCUtil.SetLabelText(m_lbAsyncSeason, NKCUtilString.GET_STRING_GAUNTLET_SEASON_NUMBERING_NAME, pvpAsyncSeasonTemplet.SeasonLobbyName);
				}
				bValue2 = !string.IsNullOrEmpty(pvpAsyncSeasonTemplet.SeasonIcon);
				NKCUtil.SetImageSprite(m_imgAsyncSeasonIcon, NKCUtil.GetSpriteBattleConditionICon(pvpAsyncSeasonTemplet.SeasonIcon));
			}
			SetBattleCondition(pvpAsyncSeasonTemplet, m_objAsyncBattleCond, m_imgAsyncBattleCond, m_lbAsyncBattleCondTitle, m_lbAsyncBattleCondDesc);
			NKCUtil.SetGameobjectActive(m_imgAsyncSeasonIcon, bValue2);
			m_asyncKeywordGroup?.SetData(NKM_GAME_TYPE.NGT_ASYNC_PVP);
		}
		else
		{
			bool bValue3 = false;
			NKMPvpRankSeasonTemplet pvpAsyncSeasonTemplet2 = NKCPVPManager.GetPvpAsyncSeasonTemplet(NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime()));
			if (pvpAsyncSeasonTemplet2 != null)
			{
				if (pvpAsyncSeasonTemplet2.IsSeasonLobbyPrefab)
				{
					NKCUtil.SetLabelText(m_lbAsyncSeasonV2, NKCStringTable.GetString(pvpAsyncSeasonTemplet2.SeasonStrID));
				}
				else
				{
					NKCUtil.SetLabelText(m_lbAsyncSeasonV2, NKCUtilString.GET_STRING_GAUNTLET_SEASON_NUMBERING_NAME, pvpAsyncSeasonTemplet2.SeasonLobbyName);
				}
				bValue3 = !string.IsNullOrEmpty(pvpAsyncSeasonTemplet2.SeasonIcon);
				NKCUtil.SetImageSprite(m_imgAsyncSeasonIconV2, NKCUtil.GetSpriteBattleConditionICon(pvpAsyncSeasonTemplet2.SeasonIcon));
			}
			SetBattleCondition(pvpAsyncSeasonTemplet2, m_objAsyncBattleCondV2, m_imgAsyncBattleCondV2, m_lbAsyncBattleCondTitleV2, m_lbAsyncBattleCondDescV2);
			NKCUtil.SetGameobjectActive(m_imgAsyncSeasonIconV2, bValue3);
			m_asyncV2KeywordGroup?.SetData(NKM_GAME_TYPE.NGT_PVP_STRATEGY);
		}
		NKCUtil.SetGameobjectActive(m_csbtnLeague, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLeagueBattleCond, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnEventMatch, bValue: false);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ARCADE_MODE))
		{
			bool flag = NKCEventPvpMgr.CanAccessEventPvp();
			NKCUtil.SetGameobjectActive(m_csbtnEventMatch, flag);
			if (flag)
			{
				m_csbtnEventMatch?.SetLock(!flag);
				if (flag)
				{
					NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
					NKCUtil.SetGameobjectActive(m_lbEventPvpSeason, bValue: true);
					Sprite lobbySeasonArt = NKCEventPvpMgr.GetLobbySeasonArt();
					NKCUtil.SetImageSprite(m_imgEventSeasonArt, lobbySeasonArt, bDisableIfSpriteNull: true);
					if (eventPvpSeasonTemplet != null)
					{
						NKCUtil.SetLabelText(m_lbEventPvpSeason, NKCStringTable.GetString(eventPvpSeasonTemplet.SeasonName));
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lbEventPvpSeason, bValue: false);
					NKCUtil.SetImageSprite(m_imgEventSeasonArt, null, bDisableIfSpriteNull: true);
				}
				NKCUtil.SetGameobjectActive(m_objEventmatchLocked, !flag);
				m_eventKeywordGroup?.SetData(NKM_GAME_TYPE.NGT_PVP_EVENT);
				string eventMatchIntervalString = NKCEventPvpMgr.GetEventMatchIntervalString();
				NKCUtil.SetLabelText(m_lbEventMatchInterval, eventMatchIntervalString);
			}
		}
		bool flag2 = NKCScenManager.GetScenManager().GetGameOptionData()?.UseVideoTexture ?? false;
		NKCUtil.SetGameobjectActive(m_objBGFallBack, !flag2);
		TutorialCheck();
	}

	private void SetBattleCondition(NKMPvpRankSeasonTemplet templet, GameObject objBattleCond, Image imgBattleCond, Text lbBattleCondTitle, Text lbBattleCondDesc)
	{
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(objBattleCond, bValue: false);
		}
		else if (!string.IsNullOrEmpty(templet.SeasonBattleCondition))
		{
			NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(templet.SeasonBattleCondition);
			if (templetByStrID != null)
			{
				NKCUtil.SetGameobjectActive(objBattleCond, bValue: true);
				NKCUtil.SetImageSprite(imgBattleCond, NKCUtil.GetSpriteBattleConditionICon(templetByStrID));
				NKCUtil.SetLabelText(lbBattleCondTitle, templetByStrID.BattleCondName_Translated);
				NKCUtil.SetLabelText(lbBattleCondDesc, templetByStrID.BattleCondDesc_Translated);
			}
			else
			{
				NKCUtil.SetGameobjectActive(objBattleCond, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(objBattleCond, bValue: false);
		}
	}

	private void OnClickRank()
	{
		if (m_csbtnRank.m_bLock)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(string.Format(NKCUtilString.GET_STRING_GAUNTLET_NOT_OPEN_RANK_MODE, NKMPvpCommonConst.Instance.RANK_PVP_OPEN_POINT), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	private void OnClickAsync()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	private void OnClickLeague()
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_LEAGUE_MODE))
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_LEAGUE_TAG_CLOSE_MESSAGE);
			return;
		}
		if (!NKCPVPManager.IsPvpLeagueUnlocked())
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(string.Format(NKCUtilString.GET_STRING_GAUNTLET_NOT_OPEN_LEAGUE_MODE, NKMPvpCommonConst.Instance.LEAGUE_PVP_OPEN_POINT), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	private void OnClickLeagueLocked()
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_LEAGUE_MODE))
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_LEAGUE_TAG_CLOSE_MESSAGE);
		}
		else if (!NKCPVPManager.IsPvpLeagueUnlocked())
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(string.Format(NKCUtilString.GET_STRING_GAUNTLET_NOT_OPEN_LEAGUE_MODE, NKMPvpCommonConst.Instance.LEAGUE_PVP_OPEN_POINT), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
	}

	private void OnClickPrivatePvp()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_FRIENDLY_MODE))
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
		}
	}

	private void OnClickEventBattle()
	{
		if (!NKCEventPvpMgr.CanAccessEventPvp())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_EVENTMATCH_CANNOT_ENTER);
			return;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	public override void CloseInternal()
	{
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
	}

	public void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Gauntlet);
	}
}
