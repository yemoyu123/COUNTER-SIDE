using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletLGSlot : MonoBehaviour
{
	public GameObject m_objMyLeague;

	public GameObject m_objDemoteImpossible;

	public GameObject m_objScore;

	public NKCUILeagueTier m_NKCUILeagueTier;

	public Text m_lbTier;

	public Text m_lbScore;

	public List<NKCUISlot> m_lstNKCUISlotReward;

	[Header("랭크")]
	public GameObject m_objRank;

	public GameObject m_obj1STCrown;

	public GameObject m_obj2NDCrown;

	public GameObject m_obj3RDCrown;

	public Text m_lbRank;

	private NKCAssetInstanceData m_InstanceData;

	public static NKCPopupGauntletLGSlot GetNewInstance(Transform parent, string bundleName = "AB_UI_NKM_UI_GAUNTLET", string assetName = "NKM_UI_GAUNTLET_POPUP_LEAGUEINFO_SLOT")
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		NKCPopupGauntletLGSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCPopupGauntletLGSlot>();
		if (component == null)
		{
			Debug.LogError("NKCPopupGauntletLGSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.transform.localScale = new Vector3(1f, 1f, 1f);
		component.Init();
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	public void Init()
	{
		int num = 0;
		for (num = 0; num < m_lstNKCUISlotReward.Count; num++)
		{
			m_lstNKCUISlotReward[num].Init();
		}
	}

	public void SetUI(bool bSeason, NKMPvpRankTemplet cNKMPvpRankTemplet, bool bMyLeague)
	{
		if (cNKMPvpRankTemplet == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRank, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUILeagueTier, bValue: true);
		m_NKCUILeagueTier.SetUI(cNKMPvpRankTemplet);
		m_lbTier.text = cNKMPvpRankTemplet.GetLeagueName();
		NKCUtil.SetGameobjectActive(m_objScore, bValue: true);
		m_lbScore.text = cNKMPvpRankTemplet.LeaguePointReq + "+";
		int i = 0;
		int num = 0;
		if (bSeason)
		{
			if (cNKMPvpRankTemplet.RewardSeason.Count > 0)
			{
				for (num = 0; num < cNKMPvpRankTemplet.RewardSeason.Count; num++)
				{
					if (i < m_lstNKCUISlotReward.Count)
					{
						NKCUtil.SetGameobjectActive(m_lstNKCUISlotReward[i], bValue: true);
						m_lstNKCUISlotReward[i].SetData(NKCUISlot.SlotData.MakeRewardTypeData(cNKMPvpRankTemplet.RewardSeason[num]));
					}
					i++;
				}
			}
		}
		else if (cNKMPvpRankTemplet.RewardWeekly.Count > 0)
		{
			for (num = 0; num < cNKMPvpRankTemplet.RewardWeekly.Count; num++)
			{
				if (i < m_lstNKCUISlotReward.Count)
				{
					NKCUtil.SetGameobjectActive(m_lstNKCUISlotReward[i], bValue: true);
					m_lstNKCUISlotReward[i].SetData(NKCUISlot.SlotData.MakeRewardTypeData(cNKMPvpRankTemplet.RewardWeekly[num]));
				}
				i++;
			}
		}
		for (; i < m_lstNKCUISlotReward.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstNKCUISlotReward[i], bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objMyLeague, bMyLeague);
		NKCUtil.SetGameobjectActive(m_objDemoteImpossible, !cNKMPvpRankTemplet.LeagueDemote);
	}

	public void SetUI(bool bSeason, NKMLeaguePvpRankTemplet cNKMLeaguePvpRankTemplet, bool bMyLeague)
	{
		if (cNKMLeaguePvpRankTemplet == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRank, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUILeagueTier, bValue: true);
		m_NKCUILeagueTier.SetUI(cNKMLeaguePvpRankTemplet.LeagueTierIcon, cNKMLeaguePvpRankTemplet.LeagueTierIconNumber);
		m_lbTier.text = NKCStringTable.GetString(cNKMLeaguePvpRankTemplet.LeagueName);
		NKCUtil.SetGameobjectActive(m_objScore, bValue: true);
		m_lbScore.text = cNKMLeaguePvpRankTemplet.LeaguePointReq + "+";
		int i = 0;
		int num = 0;
		if (bSeason)
		{
			if (cNKMLeaguePvpRankTemplet.RewardSeason.Count > 0)
			{
				for (num = 0; num < cNKMLeaguePvpRankTemplet.RewardSeason.Count; num++)
				{
					if (i < m_lstNKCUISlotReward.Count)
					{
						NKCUtil.SetGameobjectActive(m_lstNKCUISlotReward[i], bValue: true);
						m_lstNKCUISlotReward[i].SetData(NKCUISlot.SlotData.MakeRewardTypeData(cNKMLeaguePvpRankTemplet.RewardSeason[num]));
					}
					i++;
				}
			}
		}
		else if (cNKMLeaguePvpRankTemplet.RewardWeekly.Count > 0)
		{
			for (num = 0; num < cNKMLeaguePvpRankTemplet.RewardWeekly.Count; num++)
			{
				if (i < m_lstNKCUISlotReward.Count)
				{
					NKCUtil.SetGameobjectActive(m_lstNKCUISlotReward[i], bValue: true);
					m_lstNKCUISlotReward[i].SetData(NKCUISlot.SlotData.MakeRewardTypeData(cNKMLeaguePvpRankTemplet.RewardWeekly[num]));
				}
				i++;
			}
		}
		for (; i < m_lstNKCUISlotReward.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstNKCUISlotReward[i], bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objMyLeague, bMyLeague);
		NKCUtil.SetGameobjectActive(m_objDemoteImpossible, !cNKMLeaguePvpRankTemplet.LeagueDemote);
	}

	public void SetUI(NKMPvpRankSeasonRewardTemplet rankSeasonRewardTemplet)
	{
		if (rankSeasonRewardTemplet == null || !rankSeasonRewardTemplet.EnableByTag)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRank, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKCUILeagueTier, bValue: false);
		NKCUtil.SetGameobjectActive(m_objScore, bValue: false);
		if (rankSeasonRewardTemplet.MinRank == rankSeasonRewardTemplet.MaxRank)
		{
			NKCUtil.SetGameobjectActive(m_obj1STCrown, rankSeasonRewardTemplet.MinRank == 1);
			NKCUtil.SetGameobjectActive(m_obj2NDCrown, rankSeasonRewardTemplet.MinRank == 2);
			NKCUtil.SetGameobjectActive(m_obj3RDCrown, rankSeasonRewardTemplet.MinRank == 3);
			NKCUtil.SetLabelText(m_lbTier, string.Format(NKCStringTable.GetString("SI_DP_RANK_ONE_PARAM"), rankSeasonRewardTemplet.MinRank));
			NKCUtil.SetLabelText(m_lbRank, $"{rankSeasonRewardTemplet.MinRank}{NKCUtilString.GetRankNumber(rankSeasonRewardTemplet.MinRank, bUpper: true)}");
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_obj1STCrown, bValue: false);
			NKCUtil.SetGameobjectActive(m_obj2NDCrown, bValue: false);
			NKCUtil.SetGameobjectActive(m_obj3RDCrown, bValue: false);
			string text = string.Format(NKCStringTable.GetString("SI_DP_RANK_ONE_PARAM"), rankSeasonRewardTemplet.MinRank);
			string text2 = string.Format(NKCStringTable.GetString("SI_DP_RANK_ONE_PARAM"), rankSeasonRewardTemplet.MaxRank);
			NKCUtil.SetLabelText(m_lbTier, text + "~" + text2);
			NKCUtil.SetLabelText(m_lbRank, $"TOP {rankSeasonRewardTemplet.MaxRank}");
		}
		int i = 0;
		if (rankSeasonRewardTemplet.RewardList.Count > 0)
		{
			for (int j = 0; j < rankSeasonRewardTemplet.RewardList.Count; j++)
			{
				if (i < m_lstNKCUISlotReward.Count)
				{
					NKCUtil.SetGameobjectActive(m_lstNKCUISlotReward[i], bValue: true);
					m_lstNKCUISlotReward[i].SetData(NKCUISlot.SlotData.MakeRewardTypeData(rankSeasonRewardTemplet.RewardList[j]));
				}
				i++;
			}
		}
		for (; i < m_lstNKCUISlotReward.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstNKCUISlotReward[i], bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objMyLeague, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDemoteImpossible, bValue: false);
	}

	public void SetData(NKMLeaguePvpRankSeasonRewardTemplet templet)
	{
		if (templet == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRank, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKCUILeagueTier, bValue: false);
		NKCUtil.SetGameobjectActive(m_objScore, bValue: false);
		if (templet.MinRank == templet.MaxRank)
		{
			NKCUtil.SetGameobjectActive(m_obj1STCrown, templet.MinRank == 1);
			NKCUtil.SetGameobjectActive(m_obj2NDCrown, templet.MinRank == 2);
			NKCUtil.SetGameobjectActive(m_obj3RDCrown, templet.MinRank == 3);
			NKCUtil.SetLabelText(m_lbTier, string.Format(NKCStringTable.GetString("SI_DP_RANK_ONE_PARAM"), templet.MinRank));
			NKCUtil.SetLabelText(m_lbRank, $"{templet.MinRank}{NKCUtilString.GetRankNumber(templet.MinRank, bUpper: true)}");
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_obj1STCrown, bValue: false);
			NKCUtil.SetGameobjectActive(m_obj2NDCrown, bValue: false);
			NKCUtil.SetGameobjectActive(m_obj3RDCrown, bValue: false);
			string text = string.Format(NKCStringTable.GetString("SI_DP_RANK_ONE_PARAM"), templet.MinRank);
			string text2 = string.Format(NKCStringTable.GetString("SI_DP_RANK_ONE_PARAM"), templet.MaxRank);
			NKCUtil.SetLabelText(m_lbTier, text + "~" + text2);
			NKCUtil.SetLabelText(m_lbRank, $"TOP {templet.MaxRank}");
		}
		int i = 0;
		if (templet.Rewards.Count > 0)
		{
			for (int j = 0; j < templet.Rewards.Count; j++)
			{
				if (i < m_lstNKCUISlotReward.Count)
				{
					NKCUtil.SetGameobjectActive(m_lstNKCUISlotReward[i], bValue: true);
					m_lstNKCUISlotReward[i].SetData(NKCUISlot.SlotData.MakeRewardTypeData(templet.Rewards[j]));
				}
				i++;
			}
		}
		for (; i < m_lstNKCUISlotReward.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstNKCUISlotReward[i], bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objMyLeague, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDemoteImpossible, bValue: false);
	}
}
