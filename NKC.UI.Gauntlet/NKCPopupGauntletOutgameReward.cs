using System.Collections.Generic;
using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletOutgameReward : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	public const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_RANK_REWARD_POPUP";

	public Text m_lbTitle;

	public NKCUILeagueTier m_NKCUILeagueTier;

	public Text m_lbScore;

	public Text m_lbTierText;

	public Text m_lbSeasonRemainTime;

	public Transform m_ParentOfSlots;

	public NKCUIComStateButton m_csbtnOK;

	public GameObject m_objChangeScoreMessage;

	[Header("랭킹 보상")]
	public GameObject m_objRankingRoot;

	public GameObject m_objRank1st;

	public GameObject m_objRank2nd;

	public GameObject m_objRank3rd;

	public GameObject m_objRankCommon;

	public Text m_lbRank;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private List<NKCUISlot> m_lstNKCUISlot = new List<NKCUISlot>();

	private NKMRewardData m_RankRewardData;

	private bool m_bSeason;

	private bool m_bRank;

	private bool m_bLeague;

	private static PvpState m_NKMPVPData = new PvpState();

	private static PvpState m_prevSeasonPVPData = null;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupGauntletOutgameReward";

	public static void SetNKMPVPData(PvpState cNKMPVPData)
	{
		m_NKMPVPData = cNKMPVPData;
	}

	public static void SetPrevSeasonPVPData(PvpState prevPVPData)
	{
		m_prevSeasonPVPData = prevPVPData;
	}

	public void SetRankRewardData(NKMRewardData rankRewardData)
	{
		m_RankRewardData = rankRewardData;
	}

	public void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetBindFunction(m_csbtnOK, OnClickOK);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
	}

	private void OnClickOK()
	{
		Close();
	}

	public void Open(bool bWeeklyReward, NKMRewardData cNKMRewardData, bool bRank, bool bChangeScore = false, bool isSeasonRankReward = false)
	{
		m_bSeason = !bWeeklyReward;
		m_bRank = bRank;
		m_bLeague = false;
		NKCUtil.SetGameobjectActive(m_objRankingRoot, bValue: false);
		if (m_NKMPVPData != null)
		{
			int score = m_NKMPVPData.Score;
			NKMPvpRankTemplet nKMPvpRankTemplet = ((!bRank) ? NKCPVPManager.GetAsyncPvpRankTempletByTier(m_NKMPVPData.SeasonID, m_NKMPVPData.LeagueTierID) : NKCPVPManager.GetPvpRankTempletByTier(m_NKMPVPData.SeasonID, m_NKMPVPData.LeagueTierID));
			if (nKMPvpRankTemplet != null)
			{
				m_NKCUILeagueTier.SetUI(nKMPvpRankTemplet);
				m_lbTierText.text = nKMPvpRankTemplet.GetLeagueName();
			}
			m_lbScore.text = score.ToString();
		}
		if (bWeeklyReward)
		{
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_GAUNTLET_WEEKLY_REWARD);
			NKMPvpRankSeasonTemplet nKMPvpRankSeasonTemplet = ((!bRank) ? NKCPVPManager.GetPvpAsyncSeasonTemplet(NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime())) : NKCPVPManager.GetPvpRankSeasonTemplet(NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime())));
			if (nKMPvpRankSeasonTemplet != null)
			{
				if (!NKCSynchronizedTime.IsFinished(nKMPvpRankSeasonTemplet.EndDate))
				{
					m_lbSeasonRemainTime.text = string.Format(NKCStringTable.GetString("SI_DP_SEASON_TIME_UP_TO_END_FULL_TEXT_ONE_PARAM"), NKCUtilString.GetTimeString(nKMPvpRankSeasonTemplet.EndDate));
				}
				else
				{
					m_lbSeasonRemainTime.text = NKCStringTable.GetString("SI_DP_SEASON_TIME_CLOSING_FULL_TEXT");
				}
			}
		}
		else
		{
			NKCSoundManager.PlaySound("FX_UI_PVP_RESULT_PROMOTE", 1f, 0f, 0f);
			NKCUtil.SetLabelText(m_lbSeasonRemainTime, "");
			if (isSeasonRankReward)
			{
				NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_PF_GAUNTLET_RANK_SEASON_RANK_REWARD"));
				NKCUtil.SetGameobjectActive(m_objRankingRoot, bValue: true);
				NKMPvpRankSeasonTemplet nKMPvpRankSeasonTemplet2 = ((m_prevSeasonPVPData != null) ? NKCPVPManager.GetPvpRankSeasonTemplet(m_prevSeasonPVPData.SeasonID) : null);
				NKMPvpRankSeasonRewardTemplet nKMPvpRankSeasonRewardTemplet = ((nKMPvpRankSeasonTemplet2 != null) ? NKMPvpRankSeasonRewardTempletManager.GetPvpSeasonReward(nKMPvpRankSeasonTemplet2.SeasonRewardGroupId, m_prevSeasonPVPData.Rank) : null);
				if (nKMPvpRankSeasonRewardTemplet != null)
				{
					NKCUtil.SetGameobjectActive(m_objRank1st, nKMPvpRankSeasonRewardTemplet.MinRank == 1 && nKMPvpRankSeasonRewardTemplet.MaxRank == 1);
					NKCUtil.SetGameobjectActive(m_objRank2nd, nKMPvpRankSeasonRewardTemplet.MinRank == 2 && nKMPvpRankSeasonRewardTemplet.MaxRank == 2);
					NKCUtil.SetGameobjectActive(m_objRank3rd, nKMPvpRankSeasonRewardTemplet.MinRank == 3 && nKMPvpRankSeasonRewardTemplet.MaxRank == 3);
					NKCUtil.SetGameobjectActive(m_objRankCommon, nKMPvpRankSeasonRewardTemplet.MinRank != nKMPvpRankSeasonRewardTemplet.MaxRank);
				}
				else
				{
					if (m_prevSeasonPVPData != null)
					{
						Log.Error($"NKMPvpRankSeasonRewardTemplet is null (SeasonID: {m_prevSeasonPVPData.SeasonID})", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCPopupGauntletOutgameReward.cs", 167);
					}
					else
					{
						Log.Error("Previous season PVP data is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCPopupGauntletOutgameReward.cs", 169);
					}
					NKCUtil.SetGameobjectActive(m_objRank1st, bValue: false);
					NKCUtil.SetGameobjectActive(m_objRank2nd, bValue: false);
					NKCUtil.SetGameobjectActive(m_objRank3rd, bValue: false);
					NKCUtil.SetGameobjectActive(m_objRankCommon, bValue: true);
				}
				string msg = ((m_prevSeasonPVPData.Rank > 0) ? m_prevSeasonPVPData.Rank.ToString() : "");
				NKCUtil.SetLabelText(m_lbRank, msg);
				NKCUtil.SetLabelText(m_lbScore, "");
				NKCUtil.SetLabelText(m_lbTierText, "");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_GAUNTLET_SEASON_REWARD);
			}
		}
		List<NKCUISlot.SlotData> list = null;
		if (cNKMRewardData != null)
		{
			list = NKCUISlot.MakeSlotDataListFromReward(cNKMRewardData);
		}
		if (list != null)
		{
			int num = list.Count - m_lstNKCUISlot.Count;
			for (int i = 0; i < num; i++)
			{
				NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_ParentOfSlots);
				newInstance.transform.localScale = Vector3.one;
				m_lstNKCUISlot.Add(newInstance);
			}
			for (int j = 0; j < list.Count; j++)
			{
				NKCUtil.SetGameobjectActive(m_lstNKCUISlot[j], bValue: true);
				m_lstNKCUISlot[j].SetData(list[j]);
			}
			for (int k = list.Count; k < m_lstNKCUISlot.Count; k++)
			{
				NKCUtil.SetGameobjectActive(m_lstNKCUISlot[k], bValue: false);
			}
		}
		else
		{
			for (int l = 0; l < m_lstNKCUISlot.Count; l++)
			{
				NKCUtil.SetGameobjectActive(m_lstNKCUISlot[l], bValue: false);
			}
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
		NKCUtil.SetGameobjectActive(m_objChangeScoreMessage, bChangeScore);
		UIOpened();
	}

	public void OpenForLeague(NKMRewardData cNKMRewardData)
	{
		m_bSeason = true;
		m_bRank = false;
		m_bLeague = true;
		NKCUtil.SetGameobjectActive(m_objRankingRoot, bValue: false);
		if (m_NKMPVPData != null)
		{
			int score = m_NKMPVPData.Score;
			m_NKCUILeagueTier.SetUI(NKCPVPManager.GetTierIconByTier(NKM_GAME_TYPE.NGT_PVP_LEAGUE, m_NKMPVPData.SeasonID, m_NKMPVPData.LeagueTierID), NKCPVPManager.GetTierNumberByTier(NKM_GAME_TYPE.NGT_PVP_LEAGUE, m_NKMPVPData.SeasonID, m_NKMPVPData.LeagueTierID));
			m_lbTierText.text = NKCPVPManager.GetLeagueNameByTier(NKM_GAME_TYPE.NGT_PVP_LEAGUE, m_NKMPVPData.SeasonID, m_NKMPVPData.LeagueTierID);
			m_lbScore.text = score.ToString();
		}
		NKCSoundManager.PlaySound("FX_UI_PVP_RESULT_PROMOTE", 1f, 0f, 0f);
		NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_GAUNTLET_SEASON_REWARD);
		NKCUtil.SetLabelText(m_lbSeasonRemainTime, "");
		List<NKCUISlot.SlotData> list = null;
		if (cNKMRewardData != null)
		{
			list = NKCUISlot.MakeSlotDataListFromReward(cNKMRewardData);
		}
		if (list != null)
		{
			int num = list.Count - m_lstNKCUISlot.Count;
			for (int i = 0; i < num; i++)
			{
				NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_ParentOfSlots);
				newInstance.transform.localScale = Vector3.one;
				m_lstNKCUISlot.Add(newInstance);
			}
			for (int j = 0; j < list.Count; j++)
			{
				NKCUtil.SetGameobjectActive(m_lstNKCUISlot[j], bValue: true);
				m_lstNKCUISlot[j].SetData(list[j]);
			}
			for (int k = list.Count; k < m_lstNKCUISlot.Count; k++)
			{
				NKCUtil.SetGameobjectActive(m_lstNKCUISlot[k], bValue: false);
			}
		}
		else
		{
			for (int l = 0; l < m_lstNKCUISlot.Count; l++)
			{
				NKCUtil.SetGameobjectActive(m_lstNKCUISlot[l], bValue: false);
			}
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	private void Update()
	{
		m_NKCUIOpenAnimator.Update();
	}

	public void CloseGauntletOutgameRewardPopup()
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
		if (!m_bSeason)
		{
			return;
		}
		if (m_RankRewardData != null && m_RankRewardData.HasAnyReward())
		{
			Open(!m_bSeason, m_RankRewardData, m_bRank, bChangeScore: false, isSeasonRankReward: true);
			m_RankRewardData = null;
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			if (m_bLeague)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().NKCPopupGauntletNewSeasonAlarm.OpenForLeague();
			}
			else
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().NKCPopupGauntletNewSeasonAlarm.Open(m_bRank);
			}
		}
	}
}
