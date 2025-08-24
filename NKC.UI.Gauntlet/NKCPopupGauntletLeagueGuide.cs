using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletLeagueGuide : NKCUIBase
{
	public enum NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB
	{
		NPGLGT_SEASON,
		NPGLGT_WEEK,
		NPGLGT_HELP,
		NPGLGT_RANK
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_POPUP_LEAGUE_GUIDE";

	public GameObject m_objSeasonWeekPage;

	public GameObject m_objHelpPage;

	public Image m_imgTierBG;

	public Image m_imgTierBG_Help;

	public Text m_lbLeftCenter;

	public Text m_lbLeftBottomTitle;

	public Text m_lbLeftBottomPeriod;

	public Text m_lbLeftBottomDesc;

	public Text m_lbRightHelpDesc;

	public NKCUIComToggle m_ctglSeason;

	public NKCUIComToggle m_ctglWeek;

	public NKCUIComToggle m_ctglHelp;

	public NKCUIComToggle m_ctglRank;

	public NKCUIComStateButton m_csbtnClose;

	public LoopVerticalScrollRect m_lvsrSeason;

	public Transform m_trContentSeason;

	public LoopVerticalScrollRect m_lvsrWeekly;

	public Transform m_trContentWeekly;

	public LoopVerticalScrollRect m_lvsrRank;

	public Transform m_trContentRank;

	public GameObject m_objWeeklyRewardLock;

	[Header("Sprite")]
	public Sprite SpriteTierBG_League;

	public Sprite SpriteTierBG_Rank;

	public Sprite SpriteTierBG_Async;

	private NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB;

	private NKC_GAUNTLET_LOBBY_TAB m_NKC_GAUNTLET_LOBBY_TAB;

	private List<NKMPvpRankTemplet> m_lstNKMPvpRankTempletSorted;

	private NKMPvpRankTemplet m_MyRankTemplet;

	private List<NKMLeaguePvpRankTemplet> m_lstNKMLeaguePvpRankTempletSorted;

	private NKMLeaguePvpRankTemplet m_MyLeagueRankTemplet;

	private List<NKMPvpRankSeasonRewardTemplet> m_lstNKMPvpRankSeasonRewardTemplet;

	private bool m_bLoopScrollInit;

	private bool m_bOpenSeason;

	private bool m_bOpenWeek;

	private float m_fPrevUpdateTime;

	private NKM_GAME_TYPE m_NKM_GAME_TYPE;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override List<int> UpsideMenuShowResourceList => new List<int> { 5, 101 };

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET_LEAUGE_GUIDE;

	public static NKCPopupGauntletLeagueGuide OpenInstance()
	{
		NKCPopupGauntletLeagueGuide instance = NKCUIManager.OpenNewInstance<NKCPopupGauntletLeagueGuide>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_POPUP_LEAGUE_GUIDE", NKCUIManager.eUIBaseRect.UIFrontCommon, null).GetInstance<NKCPopupGauntletLeagueGuide>();
		instance.InitUI();
		return instance;
	}

	public override void OnCloseInstance()
	{
		if (m_lvsrSeason != null)
		{
			m_lvsrSeason.ClearCells();
		}
		if (m_lvsrWeekly != null)
		{
			m_lvsrWeekly.ClearCells();
		}
	}

	public void InitUI()
	{
		m_ctglSeason.OnValueChanged.RemoveAllListeners();
		m_ctglSeason.OnValueChanged.AddListener(OnValueChangedSeason);
		m_ctglWeek.OnValueChanged.RemoveAllListeners();
		m_ctglWeek.OnValueChanged.AddListener(OnValueChangedWeek);
		m_ctglHelp.OnValueChanged.RemoveAllListeners();
		m_ctglHelp.OnValueChanged.AddListener(OnValueChangedHelp);
		m_ctglRank.OnValueChanged.RemoveAllListeners();
		m_ctglRank.OnValueChanged.AddListener(OnValueChangedRank);
		m_csbtnClose.PointerClick.RemoveAllListeners();
		m_csbtnClose.PointerClick.AddListener(base.Close);
		m_lvsrSeason.dOnGetObject += GetSlotSeason;
		m_lvsrSeason.dOnReturnObject += ReturnSlot;
		m_lvsrSeason.dOnProvideData += ProvideDataSeason;
		m_lvsrSeason.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_lvsrSeason);
		m_lvsrWeekly.dOnGetObject += GetSlotWeek;
		m_lvsrWeekly.dOnReturnObject += ReturnSlot;
		m_lvsrWeekly.dOnProvideData += ProvideDataWeek;
		m_lvsrWeekly.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_lvsrWeekly);
		m_lvsrRank.dOnGetObject += GetSlotRank;
		m_lvsrRank.dOnReturnObject += ReturnSlot;
		m_lvsrRank.dOnProvideData += ProvideDataRank;
		m_lvsrRank.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_lvsrRank);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public RectTransform GetSlotSeason(int index)
	{
		return NKCPopupGauntletLGSlot.GetNewInstance(m_trContentSeason).GetComponent<RectTransform>();
	}

	public void ReturnSlot(Transform tr)
	{
		NKCPopupGauntletLGSlot component = tr.GetComponent<NKCPopupGauntletLGSlot>();
		tr.SetParent(base.transform);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideDataSeason(Transform tr, int index)
	{
		if (index < 0)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
		}
		else if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			if (index >= m_lstNKMLeaguePvpRankTempletSorted.Count)
			{
				NKCUtil.SetGameobjectActive(tr, bValue: false);
				return;
			}
			NKCPopupGauntletLGSlot component = tr.GetComponent<NKCPopupGauntletLGSlot>();
			if (component != null)
			{
				component.SetUI(bSeason: true, m_lstNKMLeaguePvpRankTempletSorted[index], m_MyLeagueRankTemplet == m_lstNKMLeaguePvpRankTempletSorted[index]);
			}
		}
		else if (index >= m_lstNKMPvpRankTempletSorted.Count)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
		}
		else
		{
			NKCPopupGauntletLGSlot component2 = tr.GetComponent<NKCPopupGauntletLGSlot>();
			if (component2 != null)
			{
				component2.SetUI(bSeason: true, m_lstNKMPvpRankTempletSorted[index], m_MyRankTemplet == m_lstNKMPvpRankTempletSorted[index]);
			}
		}
	}

	public RectTransform GetSlotWeek(int index)
	{
		return NKCPopupGauntletLGSlot.GetNewInstance(m_trContentWeekly).GetComponent<RectTransform>();
	}

	public void ProvideDataWeek(Transform tr, int index)
	{
		NKCPopupGauntletLGSlot component = tr.GetComponent<NKCPopupGauntletLGSlot>();
		if (component != null)
		{
			component.SetUI(bSeason: false, m_lstNKMPvpRankTempletSorted[index], m_MyRankTemplet == m_lstNKMPvpRankTempletSorted[index]);
		}
	}

	public RectTransform GetSlotRank(int index)
	{
		return NKCPopupGauntletLGSlot.GetNewInstance(m_trContentRank).GetComponent<RectTransform>();
	}

	public void ProvideDataRank(Transform tr, int index)
	{
		NKCPopupGauntletLGSlot component = tr.GetComponent<NKCPopupGauntletLGSlot>();
		if (!(component == null))
		{
			component.SetUI(m_lstNKMPvpRankSeasonRewardTemplet[index]);
		}
	}

	private void OnValueChangedSeason(bool bSet)
	{
		if (bSet)
		{
			m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB = NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_SEASON;
			SetUIByTab();
		}
	}

	private void OnValueChangedWeek(bool bSet)
	{
		if (bSet)
		{
			m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB = NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_WEEK;
			SetUIByTab();
		}
	}

	private void OnValueChangedHelp(bool bSet)
	{
		if (bSet)
		{
			m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB = NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_HELP;
			SetUIByTab();
		}
	}

	private void OnValueChangedRank(bool bSet)
	{
		if (bSet)
		{
			m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB = NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_RANK;
			SetUIByTab();
		}
	}

	public override void CloseInternal()
	{
		m_lstNKMPvpRankSeasonRewardTemplet = null;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void SetUIByTab()
	{
		NKCUtil.SetGameobjectActive(m_objSeasonWeekPage, m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_SEASON || m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_WEEK || m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_RANK);
		NKCUtil.SetGameobjectActive(m_objHelpPage, m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_HELP);
		NKCUtil.SetGameobjectActive(m_lvsrSeason.gameObject, m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_SEASON);
		NKCUtil.SetGameobjectActive(m_lvsrWeekly.gameObject, m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_WEEK);
		NKCUtil.SetGameobjectActive(m_lvsrRank.gameObject, m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_RANK);
		if (m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_SEASON)
		{
			NKCUtil.SetGameobjectActive(m_objWeeklyRewardLock, bValue: false);
			m_lvsrSeason.TotalCount = GetListCount();
			m_lvsrSeason.velocity = new Vector2(0f, 0f);
			if (!m_bOpenSeason)
			{
				int myRankIndex = GetMyRankIndex();
				m_lvsrSeason.SetIndexPosition(myRankIndex);
				m_bOpenSeason = true;
			}
			else
			{
				m_lvsrSeason.RefreshCells();
			}
			m_lbLeftBottomTitle.text = string.Format(NKCUtilString.GET_STRING_GAUNTLET_SEASON_TITLE_ONE_PARAM, GetSeasonStrID());
			m_lbLeftBottomPeriod.text = NKMTime.UTCtoLocal(GetSeasonStartDate()).ToString("yyyy. MM. dd") + " ~ " + NKMTime.UTCtoLocal(GetSeasonEndDate()).ToString("yyyy. MM. dd");
			m_lbLeftBottomDesc.text = GetDesc(m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB);
		}
		else if (m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_WEEK)
		{
			m_lvsrWeekly.TotalCount = m_lstNKMPvpRankTempletSorted.Count;
			m_lvsrWeekly.velocity = new Vector2(0f, 0f);
			if (!m_bOpenWeek)
			{
				m_lvsrWeekly.SetIndexPosition(0);
				m_bOpenWeek = true;
			}
			else
			{
				m_lvsrWeekly.RefreshCells();
			}
			m_lbLeftBottomTitle.text = NKCUtilString.GET_STRING_GAUNTLET_WEEK_LEAGUE;
			UpdateWeekPeriod();
			m_lbLeftBottomDesc.text = GetDesc(m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB);
		}
		else if (m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_RANK)
		{
			m_lvsrRank.TotalCount = m_lstNKMPvpRankSeasonRewardTemplet.Count;
			m_lvsrRank.velocity = new Vector2(0f, 0f);
			m_lvsrRank.SetIndexPosition(0);
			m_lbLeftBottomTitle.text = string.Format(NKCUtilString.GET_STRING_GAUNTLET_SEASON_TITLE_ONE_PARAM, GetSeasonStrID());
			m_lbLeftBottomPeriod.text = NKMTime.UTCtoLocal(GetSeasonStartDate()).ToString("yyyy. MM. dd") + " ~ " + NKMTime.UTCtoLocal(GetSeasonEndDate()).ToString("yyyy. MM. dd");
			m_lbLeftBottomDesc.text = GetDesc(NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_SEASON);
		}
		else if (m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_HELP)
		{
			m_lbLeftBottomDesc.text = GetDesc(m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB);
		}
	}

	private int GetListCount()
	{
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			return m_lstNKMLeaguePvpRankTempletSorted.Count;
		}
		return m_lstNKMPvpRankTempletSorted.Count;
	}

	private int GetMyRankIndex()
	{
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			return m_lstNKMLeaguePvpRankTempletSorted.FindIndex((NKMLeaguePvpRankTemplet v) => v == m_MyLeagueRankTemplet);
		}
		return m_lstNKMPvpRankTempletSorted.FindIndex((NKMPvpRankTemplet v) => v == m_MyRankTemplet);
	}

	private string GetSeasonStrID()
	{
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(ServiceTime.Recent);
			if (nKMLeaguePvpRankSeasonTemplet != null)
			{
				return nKMLeaguePvpRankSeasonTemplet.GetSeasonStrId();
			}
		}
		else
		{
			int seasonID = FindPvpSeasonID(NKCSynchronizedTime.GetServerUTCTime());
			NKMPvpRankSeasonTemplet seasonTemplet = GetSeasonTemplet(seasonID);
			if (seasonTemplet != null)
			{
				return seasonTemplet.GetSeasonStrID();
			}
		}
		return string.Empty;
	}

	private DateTime GetSeasonStartDate()
	{
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(ServiceTime.Recent);
			if (nKMLeaguePvpRankSeasonTemplet != null)
			{
				return nKMLeaguePvpRankSeasonTemplet.StartDateUTC;
			}
		}
		else
		{
			int seasonID = FindPvpSeasonID(NKCSynchronizedTime.GetServerUTCTime());
			NKMPvpRankSeasonTemplet seasonTemplet = GetSeasonTemplet(seasonID);
			if (seasonTemplet != null)
			{
				return seasonTemplet.StartDate;
			}
		}
		return default(DateTime);
	}

	private DateTime GetSeasonEndDate()
	{
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(ServiceTime.Recent);
			if (nKMLeaguePvpRankSeasonTemplet != null)
			{
				return nKMLeaguePvpRankSeasonTemplet.EndDateUTC;
			}
		}
		else
		{
			int seasonID = FindPvpSeasonID(NKCSynchronizedTime.GetServerUTCTime());
			NKMPvpRankSeasonTemplet seasonTemplet = GetSeasonTemplet(seasonID);
			if (seasonTemplet != null)
			{
				return seasonTemplet.EndDate;
			}
		}
		return default(DateTime);
	}

	private void UpdateWeekPeriod()
	{
		int seasonID = FindPvpSeasonID(NKCSynchronizedTime.GetServerUTCTime());
		NKMPvpRankSeasonTemplet seasonTemplet = GetSeasonTemplet(seasonID);
		if (seasonTemplet != null)
		{
			if (!NKCPVPManager.IsRewardWeek(seasonTemplet, NKCPVPManager.WeekCalcStartDateUtc))
			{
				m_lbLeftBottomPeriod.text = "-";
				NKCUtil.SetGameobjectActive(m_objWeeklyRewardLock, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objWeeklyRewardLock, bValue: false);
				m_lbLeftBottomPeriod.text = NKCUtilString.GetRemainTimeStringForGauntletWeekly();
			}
		}
	}

	private void Update()
	{
		if (m_fPrevUpdateTime + 1f < Time.time)
		{
			if (m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB == NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_WEEK)
			{
				UpdateWeekPeriod();
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objWeeklyRewardLock, bValue: false);
			}
			m_fPrevUpdateTime = Time.time;
		}
	}

	public void Open(NKC_GAUNTLET_LOBBY_TAB lobbyTab, PvpState pvpData, NKMPvpRankSeasonTemplet curSeasonTemplet)
	{
		if (curSeasonTemplet == null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			return;
		}
		m_bOpenSeason = false;
		m_bOpenWeek = false;
		m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB = NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_SEASON;
		m_NKC_GAUNTLET_LOBBY_TAB = lobbyTab;
		int seasonID = curSeasonTemplet.SeasonID;
		int rankGroup = curSeasonTemplet.RankGroup;
		if (!NKCPVPManager.dicPvpRankTemplet.ContainsKey(rankGroup))
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			return;
		}
		m_lstNKMPvpRankTempletSorted = NKCPVPManager.dicPvpRankTemplet[rankGroup].Values.OrderByDescending((NKMPvpRankTemplet e) => e.LeagueTier).ToList();
		NKCUtil.SetGameobjectActive(m_ctglWeek, bValue: true);
		NKCUtil.SetGameobjectActive(m_ctglRank, lobbyTab == NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK);
		switch (lobbyTab)
		{
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK:
			m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_PVP_RANK;
			NKCUtil.SetLabelText(m_lbLeftCenter, NKCUtilString.GET_STRING_GAUNTLET_RANK_GAME);
			m_imgTierBG.sprite = SpriteTierBG_Rank;
			m_imgTierBG_Help.sprite = SpriteTierBG_Rank;
			NKCUtil.SetLabelText(m_lbRightHelpDesc, GetDesc(NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_HELP));
			if (NKMPvpRankSeasonRewardTempletManager.PvpRankSeasonRewardTemplets.TryGetValue(curSeasonTemplet.SeasonRewardGroupId, out m_lstNKMPvpRankSeasonRewardTemplet))
			{
				m_lstNKMPvpRankSeasonRewardTemplet = m_lstNKMPvpRankSeasonRewardTemplet.OrderBy((NKMPvpRankSeasonRewardTemplet e) => e.MinRank).ToList();
			}
			if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.RANK_SEASON_REWARD) || m_lstNKMPvpRankSeasonRewardTemplet == null)
			{
				NKCUtil.SetGameobjectActive(m_ctglRank, bValue: false);
			}
			break;
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC:
			m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_ASYNC_PVP;
			NKCUtil.SetLabelText(m_lbLeftCenter, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_GAME);
			m_imgTierBG.sprite = SpriteTierBG_Async;
			m_imgTierBG_Help.sprite = SpriteTierBG_Async;
			NKCUtil.SetLabelText(m_lbRightHelpDesc, GetDesc(NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_HELP));
			break;
		default:
			Debug.LogError($"Gauntlet Info Lobby Tab ?? : {lobbyTab}");
			NKCUtil.SetLabelText(m_lbLeftCenter, "");
			m_imgTierBG.sprite = null;
			break;
		}
		if (seasonID != pvpData.SeasonID)
		{
			m_MyRankTemplet = NKCPVPManager.GetRankTempletByRankGroupScore(rankGroup, NKCUtil.GetScoreBySeason(seasonID, pvpData.SeasonID, pvpData.Score, m_NKM_GAME_TYPE));
		}
		else
		{
			m_MyRankTemplet = NKCPVPManager.GetRankTempletByRankGroupTier(rankGroup, pvpData.LeagueTierID);
		}
		UIOpened();
		if (!m_bLoopScrollInit)
		{
			m_lvsrSeason.PrepareCells();
			m_lvsrWeekly.PrepareCells();
			m_lvsrRank.PrepareCells();
			m_bLoopScrollInit = true;
		}
		m_ctglSeason.Select(bSelect: false, bForce: true);
		m_ctglSeason.Select(bSelect: true);
	}

	public void Open(NKC_GAUNTLET_LOBBY_TAB lobbyTab, PvpState pvpData, NKMLeaguePvpRankSeasonTemplet curSeasonTemplet)
	{
		if (curSeasonTemplet == null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			return;
		}
		m_bOpenSeason = false;
		m_bOpenWeek = false;
		m_NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB = NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_SEASON;
		m_NKC_GAUNTLET_LOBBY_TAB = lobbyTab;
		int seasonId = curSeasonTemplet.SeasonId;
		_ = curSeasonTemplet.RankGroup.Key;
		if (curSeasonTemplet.RankGroup.List.Count <= 0)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			return;
		}
		m_lstNKMLeaguePvpRankTempletSorted = curSeasonTemplet.RankGroup.List.OrderByDescending((NKMLeaguePvpRankTemplet e) => e.LeagueTier).ToList();
		NKCUtil.SetGameobjectActive(m_ctglWeek, bValue: false);
		NKCUtil.SetGameobjectActive(m_ctglRank, bValue: false);
		if (lobbyTab == NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE)
		{
			m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_PVP_LEAGUE;
			NKCUtil.SetLabelText(m_lbLeftCenter, NKCUtilString.GET_STRING_GAUNTLET_LEAGUE_TITLE);
			m_imgTierBG.sprite = SpriteTierBG_League;
			m_imgTierBG_Help.sprite = SpriteTierBG_League;
			NKCUtil.SetLabelText(m_lbRightHelpDesc, GetDesc(NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_HELP));
		}
		else
		{
			Debug.LogError($"Gauntlet Info Lobby Tab ?? : {lobbyTab}");
			NKCUtil.SetLabelText(m_lbLeftCenter, "");
			m_imgTierBG.sprite = null;
		}
		if (seasonId != pvpData.SeasonID)
		{
			m_MyLeagueRankTemplet = curSeasonTemplet.RankGroup.GetByScore(NKCPVPManager.GetResetScore(pvpData.SeasonID, pvpData.Score, m_NKM_GAME_TYPE));
		}
		else
		{
			m_MyLeagueRankTemplet = curSeasonTemplet.RankGroup.GetByTier(pvpData.LeagueTierID);
		}
		UIOpened();
		if (!m_bLoopScrollInit)
		{
			m_lvsrSeason.PrepareCells();
			m_lvsrWeekly.PrepareCells();
			m_bLoopScrollInit = true;
		}
		m_ctglSeason.Select(bSelect: false, bForce: true);
		m_ctglSeason.Select(bSelect: true);
	}

	private int FindPvpSeasonID(DateTime now)
	{
		return m_NKC_GAUNTLET_LOBBY_TAB switch
		{
			NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK => NKCUtil.FindPVPSeasonIDForRank(now), 
			NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC => NKCUtil.FindPVPSeasonIDForAsync(now), 
			_ => 0, 
		};
	}

	private NKMPvpRankSeasonTemplet GetSeasonTemplet(int seasonID)
	{
		return m_NKC_GAUNTLET_LOBBY_TAB switch
		{
			NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK => NKCPVPManager.GetPvpRankSeasonTemplet(seasonID), 
			NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC => NKCPVPManager.GetPvpAsyncSeasonTemplet(seasonID), 
			_ => null, 
		};
	}

	private string GetDesc(NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB gauideTab)
	{
		switch (m_NKC_GAUNTLET_LOBBY_TAB)
		{
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK:
			switch (gauideTab)
			{
			case NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_SEASON:
				return NKCUtilString.GET_STRING_GAUNTLET_SEASON_LEAUGE_DESC;
			case NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_WEEK:
				return NKCUtilString.GET_STRING_GAUNTLET_WEEK_LEAGUE_DESC;
			case NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_HELP:
				return NKCUtilString.GET_STRING_GAUNTLET_RANK_HELP_DESC;
			}
			break;
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC:
			switch (gauideTab)
			{
			case NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_SEASON:
				return NKCUtilString.GET_STRING_GAUNTLET_ASYNC_SEASON_LEAGUE_DESC;
			case NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_WEEK:
				return NKCUtilString.GET_STRING_GAUNTLET_ASYNC_WEEK_LEAGUE_DESC;
			case NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_HELP:
				return NKCUtilString.GET_STRING_GAUNTLET_ASYNC_HELP_DESC;
			}
			break;
		case NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE:
			switch (gauideTab)
			{
			case NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_SEASON:
				return NKCUtilString.GET_STRING_GAUNTLET_LEAGUE_SEASON_LEAGUE_DESC;
			case NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_WEEK:
				return "";
			case NKC_POPUP_GAUNTLET_LEAUGE_GUIDE_TAB.NPGLGT_HELP:
				return NKCUtilString.GET_STRING_GAUNTLET_LEAGUE_HELP_DESC;
			}
			break;
		}
		return string.Empty;
	}
}
