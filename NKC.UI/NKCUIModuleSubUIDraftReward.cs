using System.Collections.Generic;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIModuleSubUIDraftReward : NKCUIBase
{
	public TMP_Text m_lbDesc;

	[Header("Ƽ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public NKCUILeagueTier m_NKCUILeagueTierMy;

	public TMP_Text m_lbScore;

	public TMP_Text m_lbTier;

	public Text m_lbTierRewardDesc;

	public List<NKCUISlot> m_lstTierRewardSlot;

	public GameObject m_objTierReward;

	public GameObject m_objRank_1;

	public GameObject m_objRank_2;

	public GameObject m_objRank_3;

	public GameObject m_objRank_Other;

	public Text m_lbRank;

	public Text m_lbRankRewardDesc;

	public List<NKCUISlot> m_lstRankRewardSlot;

	public GameObject m_objRankReward;

	public NKCUIComStateButton m_btnOK;

	private PvpState m_LeagueData;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public static NKCUIModuleSubUIDraftReward OpenInstance(string bundleName, string assetName)
	{
		NKCUIModuleSubUIDraftReward instance = NKCUIManager.OpenNewInstance<NKCUIModuleSubUIDraftReward>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIFrontPopup, null).GetInstance<NKCUIModuleSubUIDraftReward>();
		if ((object)instance != null)
		{
			instance.InitUI();
			return instance;
		}
		return instance;
	}

	private void InitUI()
	{
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(base.Close);
		for (int i = 0; i < m_lstRankRewardSlot.Count; i++)
		{
			m_lstRankRewardSlot[i].Init();
		}
		for (int j = 0; j < m_lstTierRewardSlot.Count; j++)
		{
			m_lstTierRewardSlot[j].Init();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(int seasonID)
	{
		NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(seasonID);
		if (nKMLeaguePvpRankSeasonTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_LeagueData = NKCScenManager.CurrentUserData().m_LeagueData;
		NKCUtil.SetLabelText(m_lbDesc, nKMLeaguePvpRankSeasonTemplet.GetSeasonStrId());
		NKMLeaguePvpRankTemplet.FindByScore(seasonID, m_LeagueData.Score, out var templet);
		if (templet != null)
		{
			m_NKCUILeagueTierMy.SetUI(templet.LeagueTierIcon, templet.LeagueTierIconNumber);
			NKCUtil.SetLabelText(m_lbScore, m_LeagueData.Score.ToString());
			NKCUtil.SetLabelText(m_lbTier, NKCStringTable.GetString(templet.LeagueName));
			for (int i = 0; i < m_lstTierRewardSlot.Count; i++)
			{
				if (i < templet.RewardSeason.Count)
				{
					NKCUtil.SetGameobjectActive(m_lstTierRewardSlot[i], bValue: true);
					m_lstTierRewardSlot[i].SetData(NKCUISlot.SlotData.MakeRewardTypeData(templet.RewardSeason[i]));
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstTierRewardSlot[i], bValue: false);
				}
			}
		}
		NKMLeaguePvpRankSeasonRewardTemplet leaguePvpSeasonRewardTemplet = NKCPVPManager.GetLeaguePvpSeasonRewardTemplet(nKMLeaguePvpRankSeasonTemplet.RankSeasonRewardGroup, m_LeagueData.Rank);
		NKCUtil.SetGameobjectActive(m_objRankReward, leaguePvpSeasonRewardTemplet != null);
		if (leaguePvpSeasonRewardTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objRank_1, m_LeagueData.Rank == 1);
			NKCUtil.SetGameobjectActive(m_objRank_2, m_LeagueData.Rank == 2);
			NKCUtil.SetGameobjectActive(m_objRank_3, m_LeagueData.Rank == 3);
			NKCUtil.SetGameobjectActive(m_objRank_Other, m_LeagueData.Rank >= 4);
			NKCUtil.SetLabelText(m_lbRank, m_LeagueData.Rank.ToString());
			for (int j = 0; j < m_lstRankRewardSlot.Count; j++)
			{
				if (j < leaguePvpSeasonRewardTemplet.Rewards.Count)
				{
					NKCUtil.SetGameobjectActive(m_lstRankRewardSlot[j], bValue: true);
					m_lstRankRewardSlot[j].SetData(NKCUISlot.SlotData.MakeRewardTypeData(leaguePvpSeasonRewardTemplet.Rewards[j]));
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstRankRewardSlot[j], bValue: false);
				}
			}
		}
		UIOpened();
	}
}
