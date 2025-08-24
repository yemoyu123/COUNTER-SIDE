using NKM;
using NKM.Templet;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletNewSeasonAlarm : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	public const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_RANK_NEWSEASON_POPUP";

	public EventTrigger m_etBG;

	public Text m_lbTitle;

	public NKCUILeagueTier m_NKCUILeagueTier;

	public Text m_lbScore;

	public Text m_lbTierText;

	public NKCUILeagueTier m_NKCUILeagueTierNew;

	public Text m_lbScoreNew;

	public Text m_lbTierTextNew;

	public NKCUIComStateButton m_csbtnOK;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private static PvpState m_PrevNKMPVPData = new PvpState();

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupGauntletNewSeasonAlarm";

	public static void SetPrevNKMPVPData(PvpState cNKMPVPData)
	{
		m_PrevNKMPVPData = cNKMPVPData;
	}

	public void InitUI()
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			OnCloseBtn();
		});
		m_etBG.triggers.Add(entry);
		NKCUtil.SetBindFunction(m_csbtnOK, OnClickOK);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickOK()
	{
		Close();
	}

	public void Open(bool bRank)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		PvpState pvpState;
		NKMPvpRankSeasonTemplet nKMPvpRankSeasonTemplet;
		NKMPvpRankSeasonTemplet nKMPvpRankSeasonTemplet2;
		if (bRank)
		{
			pvpState = nKMUserData.m_PvpData;
			nKMPvpRankSeasonTemplet = NKCPVPManager.GetPvpRankSeasonTemplet(NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime()));
			nKMPvpRankSeasonTemplet2 = NKCPVPManager.GetPvpRankSeasonTemplet(m_PrevNKMPVPData.SeasonID);
		}
		else
		{
			pvpState = nKMUserData.m_AsyncData;
			nKMPvpRankSeasonTemplet = NKCPVPManager.GetPvpAsyncSeasonTemplet(NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime()));
			nKMPvpRankSeasonTemplet2 = NKCPVPManager.GetPvpAsyncSeasonTemplet(m_PrevNKMPVPData.SeasonID);
		}
		if (nKMPvpRankSeasonTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbTitle, string.Format(NKCStringTable.GetString("SI_DP_SEASON_POPUP_NEW_SEASON_TITLE"), nKMPvpRankSeasonTemplet.GetSeasonStrID()));
		}
		if (pvpState != null)
		{
			int score = m_PrevNKMPVPData.Score;
			NKMPvpRankTemplet rankTempletByRankGroupTier = NKCPVPManager.GetRankTempletByRankGroupTier(nKMPvpRankSeasonTemplet2.RankGroup, m_PrevNKMPVPData.LeagueTierID);
			if (rankTempletByRankGroupTier != null)
			{
				m_NKCUILeagueTier.SetUI(rankTempletByRankGroupTier);
				m_lbTierText.text = rankTempletByRankGroupTier.GetLeagueName();
			}
			m_lbScore.text = score.ToString();
			int score2 = pvpState.Score;
			NKMPvpRankTemplet rankTempletByRankGroupTier2 = NKCPVPManager.GetRankTempletByRankGroupTier(nKMPvpRankSeasonTemplet.RankGroup, pvpState.LeagueTierID);
			if (rankTempletByRankGroupTier2 != null)
			{
				m_NKCUILeagueTierNew.SetUI(rankTempletByRankGroupTier2);
				m_lbTierTextNew.text = rankTempletByRankGroupTier2.GetLeagueName();
			}
			m_lbScoreNew.text = score2.ToString();
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	public void OpenForLeague()
	{
		PvpState leagueData = NKCScenManager.CurrentUserData().m_LeagueData;
		NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(NKCUtil.FindPVPSeasonIDForLeague(NKCSynchronizedTime.GetServerUTCTime()));
		NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet2 = NKMLeaguePvpRankSeasonTemplet.Find(m_PrevNKMPVPData.SeasonID);
		if (nKMLeaguePvpRankSeasonTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbTitle, string.Format(NKCStringTable.GetString("SI_DP_SEASON_POPUP_NEW_SEASON_TITLE"), nKMLeaguePvpRankSeasonTemplet.GetSeasonStrId()));
		}
		if (leagueData != null)
		{
			int score = m_PrevNKMPVPData.Score;
			m_NKCUILeagueTier.SetUI(NKCPVPManager.GetTierIconByTier(NKM_GAME_TYPE.NGT_PVP_LEAGUE, nKMLeaguePvpRankSeasonTemplet2.SeasonId, m_PrevNKMPVPData.LeagueTierID), NKCPVPManager.GetTierNumberByTier(NKM_GAME_TYPE.NGT_PVP_LEAGUE, nKMLeaguePvpRankSeasonTemplet2.SeasonId, m_PrevNKMPVPData.LeagueTierID));
			m_lbTierText.text = NKCPVPManager.GetLeagueNameByTier(NKM_GAME_TYPE.NGT_PVP_LEAGUE, nKMLeaguePvpRankSeasonTemplet2.SeasonId, m_PrevNKMPVPData.LeagueTierID);
			m_lbScore.text = score.ToString();
			int score2 = leagueData.Score;
			m_NKCUILeagueTierNew.SetUI(NKCPVPManager.GetTierIconByTier(NKM_GAME_TYPE.NGT_PVP_LEAGUE, nKMLeaguePvpRankSeasonTemplet.SeasonId, leagueData.LeagueTierID), NKCPVPManager.GetTierNumberByTier(NKM_GAME_TYPE.NGT_PVP_LEAGUE, nKMLeaguePvpRankSeasonTemplet.SeasonId, leagueData.LeagueTierID));
			m_lbTierTextNew.text = NKCPVPManager.GetLeagueNameByTier(NKM_GAME_TYPE.NGT_PVP_LEAGUE, leagueData.SeasonID, leagueData.LeagueTierID);
			m_lbScoreNew.text = score2.ToString();
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	private void Update()
	{
		m_NKCUIOpenAnimator.Update();
	}

	public void CloseGauntletNewSeasonAlarm()
	{
		Close();
	}

	public void OnCloseBtn()
	{
		Close();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
