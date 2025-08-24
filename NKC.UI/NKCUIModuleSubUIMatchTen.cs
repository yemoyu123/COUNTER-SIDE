using System.Collections;
using System.Collections.Generic;
using Cs.Logging;
using NKC.UI.Component;
using NKC.UI.Module;
using NKM.Event;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIModuleSubUIMatchTen : NKCUIModuleSubUIBase
{
	public NKCUIComStateButton m_btnRule;

	public NKCUIComStateButton m_btnScoreReward;

	public NKCUIComStateButton m_btnStart;

	public GameObject m_objReddot;

	public Animator m_Ani;

	public NKCPopupMatchTen m_MatchTen;

	public NKCComText m_lbEventTime;

	public NKCComTMPUIText m_lbScore;

	public NKCComTMPUIText m_lbRemainTime;

	private NKMMatchTenTemplet m_NKMMatchTenTemplet;

	private NKCPopupImage m_NKCPopupImage;

	private NKCPopupScoreReward m_RewardPopup;

	public override void Init()
	{
		base.Init();
		NKCUtil.SetButtonClickDelegate(m_btnRule, OnClickRule);
		NKCUtil.SetButtonClickDelegate(m_btnScoreReward, OnClickReward);
		NKCUtil.SetButtonClickDelegate(m_btnStart, OnClickStart);
		m_MatchTen.InitUI(OnGameEnd, OnCloseGame);
	}

	public override void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
		ModuleID = templet.Key;
		int intValue = NKCUtil.GetIntValue(templet.m_Option, "MatchTenTempletID", 0);
		m_NKMMatchTenTemplet = NKMMatchTenTemplet.Find(intValue);
		if (m_NKMMatchTenTemplet == null)
		{
			Log.Error($"NKMMatchTenTemplet is null - matchTenTempletID : {intValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCUIModuleSubUIMatchTen.cs", 52);
			return;
		}
		NKCMatchTenManager.SetBoardSize(m_NKMMatchTenTemplet.m_BoardSizeX, m_NKMMatchTenTemplet.m_BoardSizeY);
		NKCMatchTenManager.SetPerfectScoreValue(m_NKMMatchTenTemplet.m_PerfectScoreValue);
		m_MatchTen.PresetData();
		NKCUtil.SetLabelText(m_lbEventTime, string.Format("{0} ~ {1} {2} (UTC+{3})", m_NKMMatchTenTemplet.IntervalTemplet.GetStartDate().ToString("yyyy-mm-dd"), m_NKMMatchTenTemplet.IntervalTemplet.GetEndDate().ToString("yyyy-mm-dd"), m_NKMMatchTenTemplet.IntervalTemplet.GetEndDate().ToString("hh-mm"), NKCSynchronizedTime.ServiceTimeOffet.TotalHours));
		Refresh();
	}

	public override bool OnBackButton()
	{
		if (m_MatchTen.gameObject.activeInHierarchy)
		{
			m_MatchTen.OnClickBackButton();
			return true;
		}
		return false;
	}

	public override void OnClose()
	{
		StopAllCoroutines();
		m_RewardPopup = null;
		m_NKCPopupImage = null;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void Refresh()
	{
		NKCMatchTenManager.SetTotalTimeSec(m_NKMMatchTenTemplet.m_PlayTimeSec);
		NKCUtil.SetLabelText(m_lbScore, NKCMatchTenManager.GetBestScore().ToString());
		NKCUtil.SetLabelText(m_lbRemainTime, ((float)NKCMatchTenManager.GetBestRemainTime() / 100f).ToString("F2"));
		if (NKMScoreRewardTemplet.Groups.ContainsKey(m_NKMMatchTenTemplet.m_ScoreRewardGroupID))
		{
			List<NKMScoreRewardTemplet> lstRewardTemplet = NKMScoreRewardTemplet.Groups[m_NKMMatchTenTemplet.m_ScoreRewardGroupID];
			NKCUtil.SetGameobjectActive(m_objReddot, CanGetAnyReward(lstRewardTemplet));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objReddot, bValue: false);
		}
		if (m_RewardPopup != null && m_RewardPopup.isActiveAndEnabled)
		{
			m_RewardPopup.Refresh();
		}
	}

	private bool CanGetAnyReward(List<NKMScoreRewardTemplet> lstRewardTemplet)
	{
		for (int i = 0; i < lstRewardTemplet.Count; i++)
		{
			if (lstRewardTemplet[i].m_Score <= NKCMatchTenManager.GetBestScore())
			{
				if (!NKCMatchTenManager.IsRewardReceived(lstRewardTemplet[i].m_ScoreRewardID))
				{
					return true;
				}
				continue;
			}
			return false;
		}
		return false;
	}

	public override void PassData(NKCUIModuleHome.EventModuleMessageDataBase passData)
	{
	}

	private void OnClickStart()
	{
		m_MatchTen.SetBlind();
		StartCoroutine(OnStart());
	}

	private IEnumerator OnStart()
	{
		NKCMatchTenManager.SetBoard();
		m_MatchTen.SetBoardData(bResetIconIdx: true);
		NKCUtil.SetGameobjectActive(m_MatchTen.m_trSlotParent, bValue: false);
		yield return new WaitForSeconds(0.1f);
		m_Ani.SetTrigger("01_to_02");
		yield return null;
		while (!(m_Ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
		{
			yield return null;
		}
		yield return null;
		m_Ani.SetTrigger("02_IDLE");
		m_MatchTen.Open();
	}

	private void OnClickRule()
	{
		if (m_NKCPopupImage == null)
		{
			m_NKCPopupImage = NKCPopupImage.OpenInstance("UI_SINGLE_MATCHTEN", "UI_SINGLE_POPUP_MATCHTEN_RULE");
		}
		m_NKCPopupImage.Open(m_NKMMatchTenTemplet.GetRuleTitle(), m_NKMMatchTenTemplet.GetRuleDesc());
	}

	private void OnClickReward()
	{
		if (m_RewardPopup == null)
		{
			m_RewardPopup = NKCPopupScoreReward.MakeInstance("UI_SINGLE_MATCHTEN", "UI_SINGLE_POPUP_MATCHTEN_REWARD");
		}
		m_RewardPopup.Open(m_NKMMatchTenTemplet.m_ScoreRewardGroupID, NKCMatchTenManager.GetBestScore(), "", (int score) => NKCMatchTenManager.IsRewardReceived(score), OnClickRewardSlot, OnClickReceiveAll);
	}

	private void OnClickRewardSlot(int rewardId)
	{
		NKCPacketSender.Send_NKMPACKET_EVENT_TEN_REWARD_REQ(rewardId);
	}

	private void OnClickReceiveAll(int rewardId)
	{
		NKCPacketSender.Send_NKMPACKET_EVENT_TEN_REWARD_ALL_REQ();
	}

	private void OnGameEnd(int score, int remainTime)
	{
		if (NKCUIManager.IsAnyPopupOpened())
		{
			NKCUIManager.CloseAllPopup();
		}
		int bestScore = NKCMatchTenManager.GetBestScore();
		int bestRemainTime = NKCMatchTenManager.GetBestRemainTime();
		NKCMatchTenManager.SetMyScore(score, remainTime);
		NKCPacketSender.Send_NKMPACKET_EVENT_TEN_RESULT_REQ(score, remainTime);
		NKCPopupMatchTenResult.Instance.Open(bestScore, bestRemainTime, score, remainTime, ProcessRestart, OnCloseGame);
	}

	private void OnCloseGame()
	{
		m_Ani.SetTrigger("02_to_01");
		Refresh();
	}

	private void ProcessRestart()
	{
		NKCMatchTenManager.SetBoard();
		m_MatchTen.Open(bResetIconIdx: true);
	}
}
