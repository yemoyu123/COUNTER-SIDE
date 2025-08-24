using System.Collections.Generic;
using ClientPacket.Pvp;
using ClientPacket.User;
using NKC.UI.Shop;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobby : NKCUIBase
{
	public NKCUIComToggle m_ctglRank;

	public NKCUIComToggle m_ctglAsync;

	public NKCUIComToggle m_ctglAsyncV2;

	public NKCUIComToggle m_ctglReplay;

	public NKCUIComToggle m_ctglCustom;

	public NKCUIComToggle m_ctglLeague;

	public NKCUIComToggle m_ctglEvent;

	public NKCUIComStateButton m_csbtnPVPShop;

	public NKCUIComStateButton m_csbtnBGM;

	public Text m_lbBGMTitle;

	public Animator m_amtorLeft;

	public NKCUIGauntletLobbyRank m_NKCUIGauntletLobbyRank;

	public NKCUIGauntletLobbyAsync m_NKCUIGauntletLobbyAsync;

	public NKCUIGauntletLobbyAsyncV2 m_NKCUIGauntletLobbyAsyncV2;

	public NKCUIGauntletLobbyReplay m_NKCUIGauntletLobbyReplay;

	public NKCUIGauntletLobbyCustom m_NKCUIGauntletLobbyCustom;

	public NKCUIGauntletLobbyLeague m_NKCUIGauntletLobbyLeague;

	public NKCUIGauntletLobbyEvent m_NKCUIGauntletLobbyEvent;

	[Header("Fallback BG")]
	public GameObject m_objBGFallBack;

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_LOBBY";

	private bool m_bInit;

	private NKC_GAUNTLET_LOBBY_TAB m_NKC_GAUNTLET_LOBBY_TAB;

	private NKCUIGauntletLobbyAsyncV2.PVP_ASYNC_TYPE m_reservedAsyncTab = NKCUIGauntletLobbyAsyncV2.PVP_ASYNC_TYPE.MAX;

	private bool m_bOpenAsyncNewMode;

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override string GuideTempletID => m_NKC_GAUNTLET_LOBBY_TAB switch
	{
		NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC => "ARTICLE_PVP_ASYNC", 
		NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE => "ARTICLE_PVP_FRIENDLY", 
		_ => "ARTICLE_PVP_RANK", 
	};

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC)
			{
				return new List<int> { 13, 5, 101 };
			}
			return new List<int> { 5, 101 };
		}
	}

	private GameObject UIGauntletLobbyAsync
	{
		get
		{
			if (!m_bOpenAsyncNewMode)
			{
				return m_NKCUIGauntletLobbyAsync.gameObject;
			}
			return m_NKCUIGauntletLobbyAsyncV2.gameObject;
		}
	}

	private NKCUIComToggle ctglAsync
	{
		get
		{
			if (!m_bOpenAsyncNewMode)
			{
				return m_ctglAsync;
			}
			return m_ctglAsyncV2;
		}
	}

	public static NKCAssetResourceData OpenInstanceAsync()
	{
		return NKCUIBase.OpenInstanceAsync<NKCUIBaseSceneMenu>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_LOBBY");
	}

	public static bool CheckInstanceLoaded(NKCAssetResourceData loadResourceData, out NKCUIGauntletLobby retVal)
	{
		return NKCUIBase.CheckInstanceLoaded<NKCUIGauntletLobby>(loadResourceData, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), out retVal);
	}

	public RANK_TYPE GetCurrRankType()
	{
		if (m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC)
		{
			return m_NKCUIGauntletLobbyAsync.GetCurrRankType();
		}
		if (m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE)
		{
			return m_NKCUIGauntletLobbyLeague.GetCurrRankType();
		}
		return m_NKCUIGauntletLobbyRank.GetCurrRankType();
	}

	public NKC_GAUNTLET_LOBBY_TAB GetCurrentLobbyTab()
	{
		return m_NKC_GAUNTLET_LOBBY_TAB;
	}

	public void CloseInstance()
	{
		m_NKCUIGauntletLobbyRank?.ClearCacheData();
		if (m_bOpenAsyncNewMode)
		{
			m_NKCUIGauntletLobbyAsyncV2?.ClearCacheData();
		}
		else
		{
			m_NKCUIGauntletLobbyAsync?.ClearCacheData();
		}
		m_NKCUIGauntletLobbyReplay?.ClearCacheData();
		m_NKCUIGauntletLobbyCustom?.ClearCacheData();
		m_NKCUIGauntletLobbyLeague?.ClearCacheData();
		m_NKCUIGauntletLobbyEvent?.ClearCacheData();
		NKCAssetResourceManager.CloseResource("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_LOBBY");
		Object.Destroy(base.gameObject);
	}

	public void InitUI()
	{
		if (!m_bInit)
		{
			m_bOpenAsyncNewMode = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_NEW_MODE);
			m_ctglRank.OnValueChanged.RemoveAllListeners();
			m_ctglRank.OnValueChanged.AddListener(OnValueChangedRank);
			m_ctglRank.m_bGetCallbackWhileLocked = true;
			NKCUtil.SetToggleValueChangedDelegate(ctglAsync, OnValueChangedAsync);
			NKCUtil.SetGameobjectActive(m_ctglAsyncV2, m_bOpenAsyncNewMode);
			NKCUtil.SetGameobjectActive(m_ctglAsync, !m_bOpenAsyncNewMode);
			m_ctglReplay.OnValueChanged.RemoveAllListeners();
			m_ctglReplay.OnValueChanged.AddListener(OnValueChangedReplay);
			NKCUtil.SetGameobjectActive(m_ctglReplay, NKCReplayMgr.IsReplayLobbyTabOpened());
			m_ctglLeague.OnValueChanged.RemoveAllListeners();
			m_ctglLeague.OnValueChanged.AddListener(OnValueChangedLeague);
			NKCUtil.SetGameobjectActive(m_ctglLeague, bValue: false);
			m_ctglLeague.m_bGetCallbackWhileLocked = true;
			if (m_ctglCustom != null)
			{
				m_ctglCustom.OnValueChanged.RemoveAllListeners();
				m_ctglCustom.OnValueChanged.AddListener(OnValueChangedCustom);
				NKCUtil.SetGameobjectActive(m_ctglCustom, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_FRIENDLY_MODE));
			}
			NKCUtil.SetGameobjectActive(m_ctglEvent, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ARCADE_MODE) && NKCEventPvpMgr.CanAccessEventPvp());
			NKCUtil.SetToggleValueChangedDelegate(m_ctglEvent, OnValueChangedEvent);
			NKCUtil.SetBindFunction(m_csbtnPVPShop, OnClickPVPShop);
			NKCUtil.SetBindFunction(m_csbtnBGM, OnClickBGM);
			m_NKCUIGauntletLobbyRank?.Init();
			if (!m_bOpenAsyncNewMode)
			{
				m_NKCUIGauntletLobbyAsync?.Init();
			}
			else
			{
				m_NKCUIGauntletLobbyAsyncV2?.Init();
			}
			m_NKCUIGauntletLobbyCustom?.Init();
			m_NKCUIGauntletLobbyLeague?.Init();
			m_NKCUIGauntletLobbyEvent?.Init();
			if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_REPLAY))
			{
				NKCScenManager.GetScenManager().GetNKCReplayMgr().ReadReplayData();
			}
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			m_bInit = true;
		}
	}

	private void OnLeftMenuTabChanged(NKC_GAUNTLET_LOBBY_TAB eNKC_GAUNTLET_LOBBY_TAB)
	{
		m_NKC_GAUNTLET_LOBBY_TAB = eNKC_GAUNTLET_LOBBY_TAB;
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(m_NKC_GAUNTLET_LOBBY_TAB);
		ResetUIByCurrTab();
		TryGetRewardREQ();
	}

	private void OnValueChangedRank(bool bSet)
	{
		if (m_ctglRank.m_bLock)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(string.Format(NKCUtilString.GET_STRING_GAUNTLET_NOT_OPEN_RANK_MODE, NKMPvpCommonConst.Instance.RANK_PVP_OPEN_POINT), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (bSet)
		{
			OnLeftMenuTabChanged(NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK);
		}
	}

	private void OnValueChangedAsync(bool bSet)
	{
		if (bSet)
		{
			OnLeftMenuTabChanged(NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC);
		}
	}

	private void OnValueChangedReplay(bool bSet)
	{
		if (bSet)
		{
			OnLeftMenuTabChanged(NKC_GAUNTLET_LOBBY_TAB.NGLT_REPLAY);
		}
	}

	private void OnValueChangedLeague(bool bSet)
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_LEAGUE_MODE))
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_COMING_SOON_SYSTEM);
		}
		else if (m_ctglLeague.m_bLock)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(string.Format(NKCUtilString.GET_STRING_GAUNTLET_NOT_OPEN_LEAGUE_MODE, NKMPvpCommonConst.Instance.LEAGUE_PVP_OPEN_POINT), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (bSet)
		{
			OnLeftMenuTabChanged(NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE);
		}
	}

	private void OnValueChangedCustom(bool bSet)
	{
		if (bSet)
		{
			OnLeftMenuTabChanged(NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE);
		}
	}

	private void OnValueChangedEvent(bool bSet)
	{
		if (bSet)
		{
			OnLeftMenuTabChanged(NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT);
		}
	}

	private void OnClickPVPShop()
	{
		NKCUIShop.ShopShortcut("TAB_SEASON_GAUNTLET");
	}

	private void OnClickBGM()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_JUKEBOX_CONTENTS_UNLOCK);
			return;
		}
		int jukeboxBgmId = NKCScenManager.GetScenManager().GetMyUserData().m_JukeboxData.GetJukeboxBgmId(NKM_BGM_TYPE.PVP_INGAME);
		NKCUIJukeBox.Instance.Open(jukeboxBgmId);
	}

	private void ResetUIByCurrTab()
	{
		NKCUtil.SetGameobjectActive(m_NKCUIGauntletLobbyRank, m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK);
		NKCUtil.SetGameobjectActive(UIGauntletLobbyAsync, m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC);
		NKCUtil.SetGameobjectActive(m_NKCUIGauntletLobbyReplay, m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_REPLAY && NKCReplayMgr.IsReplayLobbyTabOpened());
		NKCUtil.SetGameobjectActive(m_NKCUIGauntletLobbyCustom, m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE && NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_FRIENDLY_MODE));
		NKCUtil.SetGameobjectActive(m_NKCUIGauntletLobbyLeague, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUIGauntletLobbyEvent, m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT);
		switch (m_NKC_GAUNTLET_LOBBY_TAB)
		{
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK:
			m_NKCUIGauntletLobbyRank?.SetUI();
			break;
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC:
			if (m_bOpenAsyncNewMode)
			{
				m_NKCUIGauntletLobbyAsyncV2?.SetUI(m_reservedAsyncTab);
			}
			else
			{
				m_NKCUIGauntletLobbyAsync?.SetUI();
			}
			break;
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_REPLAY:
			m_NKCUIGauntletLobbyReplay?.SetUI();
			break;
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE:
			m_NKCUIGauntletLobbyCustom?.SetUI();
			break;
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE:
			m_NKCUIGauntletLobbyLeague?.SetUI();
			break;
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT:
			m_NKCUIGauntletLobbyEvent?.SetUI();
			break;
		}
		UpdateUpsideMenu();
	}

	public void Open(NKC_GAUNTLET_LOBBY_TAB _NKC_GAUNTLET_LOBBY_TAB = NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK, RANK_TYPE eRANK_TYPE = RANK_TYPE.MY_LEAGUE, NKCUIGauntletLobbyAsyncV2.PVP_ASYNC_TYPE asyncTab = NKCUIGauntletLobbyAsyncV2.PVP_ASYNC_TYPE.MAX)
	{
		m_NKC_GAUNTLET_LOBBY_TAB = _NKC_GAUNTLET_LOBBY_TAB;
		m_reservedAsyncTab = asyncTab;
		m_NKCUIGauntletLobbyRank.SetCurrRankType(eRANK_TYPE);
		bool flag = NKCScenManager.GetScenManager().GetGameOptionData()?.UseVideoTexture ?? false;
		NKCUtil.SetGameobjectActive(m_objBGFallBack, !flag);
		UIOpened();
		m_amtorLeft.Play("NKM_UI_GAUNTLET_LOBBY_CONTENT_INTRO_LEFT");
		if (!NKCPVPManager.IsPvpRankUnlocked())
		{
			m_ctglRank.Lock();
		}
		else
		{
			m_ctglRank.UnLock();
		}
		if (!NKCEventPvpMgr.CanAccessEventPvp() || !NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ARCADE_MODE))
		{
			NKCUtil.SetGameobjectActive(m_ctglEvent, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_ctglEvent, bValue: true);
			m_ctglEvent.UnLock();
		}
		if (m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK)
		{
			m_ctglRank.Select(bSelect: false, bForce: true);
			m_ctglRank.Select(bSelect: true);
		}
		else if (m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC)
		{
			ctglAsync.Select(bSelect: false, bForce: true);
			ctglAsync.Select(bSelect: true);
		}
		else if (m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_REPLAY)
		{
			m_ctglReplay.Select(bSelect: false, bForce: true);
			m_ctglReplay.Select(bSelect: true);
		}
		else if (m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE)
		{
			m_ctglCustom.Select(bSelect: false, bForce: true);
			m_ctglCustom.Select(bSelect: true);
		}
		else if (m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT)
		{
			m_ctglEvent.Select(bSelect: false, bForce: true);
			m_ctglEvent.Select(bSelect: true);
		}
		ResetBGMTitle();
		CheckTutorial();
		NKCRankPVPMgr.SetPickRateData(null);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_NKCUIGauntletLobbyRank?.Close();
		m_NKCUIGauntletLobbyAsync?.Close();
		m_NKCUIGauntletLobbyAsyncV2?.Close();
		m_NKCUIGauntletLobbyReplay?.Close();
		m_NKCUIGauntletLobbyCustom?.Close();
		m_NKCUIGauntletLobbyLeague?.Close();
		m_NKCUIGauntletLobbyEvent?.Close();
	}

	public override void OnBackButton()
	{
		if (m_NKC_GAUNTLET_LOBBY_TAB != NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT || !(m_NKCUIGauntletLobbyEvent != null) || m_NKCUIGauntletLobbyEvent.CanClose())
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_INTRO);
		}
	}

	public void OnRecv(NKMPacket_PVP_RANK_LIST_ACK cNKMPacket_PVP_RANK_LIST_ACK)
	{
		if (m_NKCUIGauntletLobbyRank != null)
		{
			m_NKCUIGauntletLobbyRank.OnRecv(cNKMPacket_PVP_RANK_LIST_ACK);
		}
	}

	public void OnRecv(NKMPacket_NPC_PVP_TARGET_LIST_ACK sPacket)
	{
		m_NKCUIGauntletLobbyAsyncV2?.OnRecv(sPacket);
	}

	public void OnRecv(NKMPacket_PVP_CASTING_VOTE_UNIT_ACK sPacket)
	{
		if (m_NKCUIGauntletLobbyRank != null)
		{
			m_NKCUIGauntletLobbyRank.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_PVP_CASTING_VOTE_SHIP_ACK sPacket)
	{
		if (m_NKCUIGauntletLobbyRank != null)
		{
			m_NKCUIGauntletLobbyRank.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_PVP_CASTING_VOTE_OPERATOR_ACK sPacket)
	{
		if (m_NKCUIGauntletLobbyRank != null)
		{
			m_NKCUIGauntletLobbyRank.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_RANK_LIST_ACK sPacket)
	{
		if (m_NKCUIGauntletLobbyLeague != null)
		{
			m_NKCUIGauntletLobbyLeague.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_PVP_CHARGE_POINT_REFRESH_ACK cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK)
	{
		m_NKCUIGauntletLobbyRank?.OnRecv(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK);
		m_NKCUIGauntletLobbyLeague?.OnRecv(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK);
		m_NKCUIGauntletLobbyEvent?.OnRecv(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK);
		if (!m_bOpenAsyncNewMode)
		{
			m_NKCUIGauntletLobbyAsync?.OnRecv(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK);
		}
		else
		{
			m_NKCUIGauntletLobbyAsyncV2?.OnRecv(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK);
		}
	}

	public void OnRecv(NKMPacket_PVP_RANK_WEEK_REWARD_ACK sPacket)
	{
		m_NKCUIGauntletLobbyRank?.OnRecv(sPacket);
	}

	public void OnRecv(NKMPacket_PVP_RANK_SEASON_REWARD_ACK cNKMPacket_PVP_RANK_SEASON_REWARD_ACK)
	{
		m_NKCUIGauntletLobbyRank?.OnRecv(cNKMPacket_PVP_RANK_SEASON_REWARD_ACK);
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_RANK_SEASON_REWARD_ACK packet)
	{
		if (!m_bOpenAsyncNewMode)
		{
			m_NKCUIGauntletLobbyAsync?.OnRecv(packet);
		}
		else
		{
			m_NKCUIGauntletLobbyAsyncV2?.OnRecv(packet);
		}
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_WEEKLY_REWARD_ACK packet)
	{
		m_NKCUIGauntletLobbyLeague?.OnRecv(packet);
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_SEASON_REWARD_ACK packet)
	{
		m_NKCUIGauntletLobbyLeague?.OnRecv(packet);
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_TARGET_LIST_ACK packet)
	{
		if (!m_bOpenAsyncNewMode)
		{
			m_NKCUIGauntletLobbyAsync?.OnRecv(packet);
		}
		else
		{
			m_NKCUIGauntletLobbyAsyncV2?.OnRecv(packet);
		}
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_RANK_LIST_ACK packet)
	{
		if (!m_bOpenAsyncNewMode)
		{
			m_NKCUIGauntletLobbyAsync?.OnRecv(packet);
		}
		else
		{
			m_NKCUIGauntletLobbyAsyncV2?.OnRecv(packet);
		}
	}

	public void OnRecv(NKMPacket_REVENGE_PVP_TARGET_LIST_ACK sPacket)
	{
		m_NKCUIGauntletLobbyAsyncV2?.OnRecv(sPacket);
	}

	public void OnRecvEventPvpSeasonInfo()
	{
		m_NKCUIGauntletLobbyEvent?.OnRecvEventPvpSeasonInfo();
	}

	public void OnRecvEventPvpReward()
	{
		m_NKCUIGauntletLobbyEvent?.OnRecvEventPvpReward();
	}

	public void OnRecv(NKMPacket_UPDATE_DEFENCE_DECK_ACK packet)
	{
		if (!m_bOpenAsyncNewMode)
		{
			m_NKCUIGauntletLobbyAsync?.OnRecv(packet);
		}
		else
		{
			m_NKCUIGauntletLobbyAsyncV2?.OnRecv(packet);
		}
	}

	private void TryGetRewardREQ()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		PvpState pvpData;
		int seasonID;
		int weekID;
		if (m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK)
		{
			pvpData = myUserData.m_PvpData;
			seasonID = NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime());
			weekID = NKCPVPManager.GetWeekIDForRank(NKCSynchronizedTime.GetServerUTCTime(), seasonID);
		}
		else if (m_NKC_GAUNTLET_LOBBY_TAB == NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC)
		{
			pvpData = myUserData.m_AsyncData;
			seasonID = NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime());
			weekID = NKCPVPManager.GetWeekIDForAsync(NKCSynchronizedTime.GetServerUTCTime(), seasonID);
		}
		else
		{
			if (m_NKC_GAUNTLET_LOBBY_TAB != NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE)
			{
				return;
			}
			pvpData = myUserData.m_LeagueData;
			seasonID = NKCUtil.FindPVPSeasonIDForLeague(NKCSynchronizedTime.GetServerUTCTime());
			weekID = NKCPVPManager.GetWeekIDForLeague(NKCSynchronizedTime.GetServerUTCTime(), seasonID);
		}
		if (NKCPVPManager.CanRewardWeek(GetGameType(m_NKC_GAUNTLET_LOBBY_TAB), pvpData, seasonID, weekID, NKCSynchronizedTime.GetServerUTCTime()) == NKM_ERROR_CODE.NEC_OK)
		{
			SendWeekRewardREQ(m_NKC_GAUNTLET_LOBBY_TAB);
		}
		else if (NKCPVPManager.CanRewardSeason(pvpData, seasonID, NKCSynchronizedTime.GetServerUTCTime()) == NKM_ERROR_CODE.NEC_OK)
		{
			SendSeasonRewardREQ(m_NKC_GAUNTLET_LOBBY_TAB);
		}
	}

	private NKM_GAME_TYPE GetGameType(NKC_GAUNTLET_LOBBY_TAB gauntletLobbyType)
	{
		return gauntletLobbyType switch
		{
			NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC => NKM_GAME_TYPE.NGT_ASYNC_PVP, 
			NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK => NKM_GAME_TYPE.NGT_PVP_RANK, 
			NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE => NKM_GAME_TYPE.NGT_PVP_LEAGUE, 
			NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE => NKM_GAME_TYPE.NGT_PVP_PRIVATE, 
			_ => NKM_GAME_TYPE.NGT_INVALID, 
		};
	}

	private void SendWeekRewardREQ(NKC_GAUNTLET_LOBBY_TAB tab)
	{
		switch (tab)
		{
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK:
		{
			NKMPacket_PVP_RANK_WEEK_REWARD_REQ packet2 = new NKMPacket_PVP_RANK_WEEK_REWARD_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet2, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			break;
		}
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC:
		{
			NKMPacket_ASYNC_PVP_RANK_WEEK_REWARD_REQ packet = new NKMPacket_ASYNC_PVP_RANK_WEEK_REWARD_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			break;
		}
		}
	}

	private void SendSeasonRewardREQ(NKC_GAUNTLET_LOBBY_TAB tab)
	{
		switch (tab)
		{
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK:
		{
			NKMPacket_PVP_RANK_SEASON_REWARD_REQ packet3 = new NKMPacket_PVP_RANK_SEASON_REWARD_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet3, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			break;
		}
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC:
		{
			NKMPacket_ASYNC_PVP_RANK_SEASON_REWARD_REQ packet2 = new NKMPacket_ASYNC_PVP_RANK_SEASON_REWARD_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet2, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			break;
		}
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE:
		{
			NKMPacket_LEAGUE_PVP_SEASON_REWARD_REQ packet = new NKMPacket_LEAGUE_PVP_SEASON_REWARD_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
			break;
		}
		}
	}

	public void OnRecv(NKMPacket_JUKEBOX_CHANGE_BGM_ACK sPacket)
	{
		ResetBGMTitle();
	}

	private void ResetBGMTitle()
	{
		NKCBGMInfoTemplet nKCBGMInfoTemplet = NKCBGMInfoTemplet.Find(NKCScenManager.GetScenManager().GetMyUserData().m_JukeboxData.GetJukeboxBgmId(NKM_BGM_TYPE.PVP_INGAME));
		if (nKCBGMInfoTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbBGMTitle, NKCStringTable.GetString(nKCBGMInfoTemplet.m_BgmNameStringID));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbBGMTitle, NKCUtilString.GET_STRING_JUKEBOX_MUSIC_DEFAULT);
		}
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.GauntletLobby);
	}
}
