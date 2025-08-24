using Cs.Core.Util;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public abstract class NKCUIGauntletLobbyRightSideBase : MonoBehaviour
{
	public CanvasGroup m_cgMyInfo;

	public NKCUILeagueTier m_NKCUILeagueTierMy;

	public Text m_lbScore;

	public Text m_lbTier;

	public Text m_lbLeagueRank;

	public GameObject m_objNoRecord;

	public Text m_lbWinCount;

	public Text m_lbMaxWinStreak;

	public Text m_lbMaxScore;

	public Text m_lbMaxLeagueTierId;

	public Text m_lbSeasonWinCount;

	public Text m_lbSeasonWinPercent;

	public NKCUIComStateButton m_csbtnLeagueGuide;

	public NKCUIComStateButton m_csbtnBattleHistory;

	public NKCUIComStateButton m_csbtnEmoticonSetting;

	[Header("전투 환경")]
	public GameObject m_objBattleCond;

	public Image m_imgBattleCond;

	public Text m_lbBattleCondTitle;

	public Text m_lbBattleCondDesc;

	private NKCPopupGauntletLeagueGuide m_NKCPopupGauntletLeagueGuide;

	private NKM_GAME_TYPE m_NKM_GAME_TYPE;

	public virtual void InitUI()
	{
		if (m_csbtnLeagueGuide != null)
		{
			m_csbtnLeagueGuide.PointerClick.RemoveAllListeners();
			m_csbtnLeagueGuide.PointerClick.AddListener(OnClickLeagueGuide);
		}
		if (m_csbtnBattleHistory != null)
		{
			m_csbtnBattleHistory.PointerClick.RemoveAllListeners();
			m_csbtnBattleHistory.PointerClick.AddListener(OnClickBattleRecord);
		}
		if (m_csbtnEmoticonSetting != null)
		{
			m_csbtnEmoticonSetting.PointerClick.RemoveAllListeners();
			m_csbtnEmoticonSetting.PointerClick.AddListener(OnClickEmoticonSetting);
		}
	}

	public virtual void UpdateNowSeasonPVPInfoUI(NKM_GAME_TYPE gameType)
	{
		if (NKCScenManager.GetScenManager().GetMyUserData() == null)
		{
			return;
		}
		int num = 0;
		m_NKM_GAME_TYPE = gameType;
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
			num = NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime());
			break;
		case NKM_GAME_TYPE.NGT_PVP_RANK:
			num = NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime());
			break;
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
			num = NKCUtil.FindPVPSeasonIDForLeague(NKCSynchronizedTime.GetServerUTCTime());
			break;
		case NKM_GAME_TYPE.NGT_PVP_EVENT:
			return;
		}
		PvpState pvpData = GetPvpData();
		if (pvpData == null)
		{
			return;
		}
		if (pvpData.SeasonID != num)
		{
			m_cgMyInfo.alpha = 0f;
			NKCUtil.SetGameobjectActive(m_objNoRecord, bValue: true);
			NKCUtil.SetLabelText(m_lbLeagueRank, NKCUtilString.GET_STRING_GAUNTLET_RANK_NO_JOIN);
			NKCUtil.SetLabelText(m_lbWinCount, "-");
			NKCUtil.SetLabelText(m_lbMaxWinStreak, "-");
			NKCUtil.SetLabelText(m_lbMaxScore, "-");
			NKCUtil.SetLabelText(m_lbMaxLeagueTierId, "-");
			NKCUtil.SetLabelText(m_lbSeasonWinCount, "-");
			NKCUtil.SetLabelText(m_lbSeasonWinPercent, "-");
			int resetScore = NKCPVPManager.GetResetScore(pvpData.SeasonID, pvpData.Score, gameType);
			NKCUtil.SetLabelText(m_lbScore, resetScore.ToString());
			SetMyLeagueTier(NKCPVPManager.GetTierIconByScore(gameType, num, resetScore), NKCPVPManager.GetTierNumberByScore(gameType, num, resetScore));
			NKCUtil.SetLabelText(m_lbTier, GetLeagueNameByScore(num, pvpData));
			return;
		}
		SetMyLeagueTier(NKCPVPManager.GetTierIconByTier(gameType, num, pvpData.LeagueTierID), NKCPVPManager.GetTierNumberByTier(gameType, num, pvpData.LeagueTierID));
		NKCUtil.SetLabelText(m_lbTier, GetLeagueNameByTier(num, pvpData));
		m_cgMyInfo.alpha = 1f;
		NKCUtil.SetLabelText(m_lbScore, pvpData.Score.ToString());
		NKCUtil.SetGameobjectActive(m_objNoRecord, bValue: false);
		NKCUtil.SetLabelText(m_lbLeagueRank, string.Format(NKCUtilString.GET_STRING_TOTAL_RANK_ONE_PARAM, pvpData.Rank));
		NKCUtil.SetLabelText(m_lbWinCount, pvpData.WinCount.ToString());
		NKCUtil.SetLabelText(m_lbMaxWinStreak, pvpData.MaxWinStreak.ToString());
		NKCUtil.SetLabelText(m_lbMaxScore, pvpData.MaxScore.ToString());
		NKCUtil.SetLabelText(m_lbSeasonWinCount, pvpData.SeasonWinCount.ToString());
		if (pvpData.SeasonWinCount == 0)
		{
			NKCUtil.SetLabelText(m_lbSeasonWinPercent, "-");
		}
		else
		{
			float num2 = (float)pvpData.SeasonWinCount / (float)pvpData.SeasonPlayCount;
			NKCUtil.SetLabelText(m_lbSeasonWinPercent, num2.ToString("P2"));
		}
		NKCUtil.SetLabelText(m_lbMaxLeagueTierId, NKCPVPManager.GetLeagueNameByTier(gameType, num, pvpData.LeagueTierID));
	}

	private void SetMyLeagueTier(LEAGUE_TIER_ICON leagueTierIcon, int leagueTierNum)
	{
		m_NKCUILeagueTierMy.SetUI(leagueTierIcon, leagueTierNum);
	}

	public void UpdateBattleCondition()
	{
		NKMPvpRankSeasonTemplet seasonTemplet = GetSeasonTemplet();
		if (seasonTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_objBattleCond, bValue: false);
		}
		else if (!string.IsNullOrEmpty(seasonTemplet.SeasonBattleCondition))
		{
			NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(seasonTemplet.SeasonBattleCondition);
			if (templetByStrID != null)
			{
				NKCUtil.SetGameobjectActive(m_objBattleCond, bValue: true);
				NKCUtil.SetImageSprite(m_imgBattleCond, NKCUtil.GetSpriteBattleConditionICon(templetByStrID));
				NKCUtil.SetLabelText(m_lbBattleCondTitle, templetByStrID.BattleCondName_Translated);
				NKCUtil.SetLabelText(m_lbBattleCondDesc, templetByStrID.BattleCondDesc_Translated);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objBattleCond, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objBattleCond, bValue: false);
		}
	}

	private void OnClickLeagueGuide()
	{
		if (m_NKCPopupGauntletLeagueGuide == null)
		{
			m_NKCPopupGauntletLeagueGuide = NKCPopupGauntletLeagueGuide.OpenInstance();
		}
		PvpState pvpData = GetPvpData();
		if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_ASYNC_PVP || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_RANK)
		{
			NKMPvpRankSeasonTemplet seasonTemplet = GetSeasonTemplet();
			m_NKCPopupGauntletLeagueGuide.Open(NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().GetCurrentLobbyTab(), pvpData, seasonTemplet);
		}
		else if (m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE || m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			NKMLeaguePvpRankSeasonTemplet curSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(ServiceTime.Recent);
			m_NKCPopupGauntletLeagueGuide.Open(NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().GetCurrentLobbyTab(), pvpData, curSeasonTemplet);
		}
	}

	private void OnClickBattleRecord()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY()?.OpenBattleRecord(m_NKM_GAME_TYPE);
	}

	private void OnClickEmoticonSetting()
	{
		if (NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			NKCPopupEmoticonSetting.Instance.Open();
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetWaitForEmoticon(bValue: true);
			NKCPacketSender.Send_NKMPacket_EMOTICON_DATA_REQ();
		}
	}

	protected abstract PvpState GetPvpData();

	protected abstract NKMPvpRankSeasonTemplet GetSeasonTemplet();

	protected abstract string GetLeagueNameByScore(int seasonID, PvpState pvpData);

	protected abstract string GetLeagueNameByTier(int seasonID, PvpState pvpData);
}
