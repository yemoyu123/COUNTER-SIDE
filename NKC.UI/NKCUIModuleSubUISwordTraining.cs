using System.Collections;
using System.Collections.Generic;
using Cs.Logging;
using NKC.UI.Component;
using NKC.UI.Module;
using NKM;
using NKM.Event;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIModuleSubUISwordTraining : NKCUIModuleSubUIBase
{
	public NKCUIComStateButton m_btnRule;

	public NKCUIComStateButton m_btnScoreReward;

	public NKCUIComStateButton m_btnStart;

	public GameObject m_objReddot;

	public Animator m_Ani;

	public NKCPopupSwordTraining m_SwordTraining;

	public NKCComText m_lbEventTime;

	public NKCComTMPUIText m_lbScore;

	public NKCComTMPUIText m_lbRemainTime;

	private NKCPopupImage m_NKCPopupImage;

	private NKCPopupScoreReward m_RewardPopup;

	private NKMMiniGameTemplet m_MiniGameTemplet;

	public override void Init()
	{
		base.Init();
		NKCUtil.SetButtonClickDelegate(m_btnRule, OnClickRule);
		NKCUtil.SetButtonClickDelegate(m_btnScoreReward, OnClickReward);
		NKCUtil.SetButtonClickDelegate(m_btnStart, OnClickStart);
		m_SwordTraining.InitUI(OnExitGame);
	}

	public override void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
		ModuleID = templet.Key;
		int intValue = NKCUtil.GetIntValue(templet.m_Option, "MiniGameTempletID", 0);
		m_MiniGameTemplet = NKMMiniGameManager.GetTemplet(intValue);
		if (m_MiniGameTemplet == null)
		{
			Log.Error($"NKMMiniGameTemplet is null - MiniGame ID : {intValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCUIModuleSubUISwordTraining.cs", 53);
			return;
		}
		NKCPacketSender.Send_NKMPacket_MINI_GAME_INFO_REQ(NKM_MINI_GAME_TYPE.SWORD_TRAINING, m_MiniGameTemplet.Key);
		NKCUtil.SetLabelText(m_lbEventTime, string.Format("{0} ~ {1} {2} (UTC+{3})", m_MiniGameTemplet.IntervalTemplet.GetStartDate().ToString("yyyy-mm-dd"), m_MiniGameTemplet.IntervalTemplet.GetEndDate().ToString("yyyy-mm-dd"), m_MiniGameTemplet.IntervalTemplet.GetEndDate().ToString("hh-mm"), NKCSynchronizedTime.ServiceTimeOffet.TotalHours));
		Refresh();
	}

	public override void Refresh()
	{
		if (m_MiniGameTemplet != null)
		{
			int num = 0;
			NKMMiniGameData miniGameData = NKCScenManager.CurrentUserData().GetMiniGameData(NKM_MINI_GAME_TYPE.SWORD_TRAINING, m_MiniGameTemplet.Key);
			if (miniGameData != null)
			{
				num = (int)miniGameData.score;
			}
			NKCUtil.SetLabelText(m_lbScore, num.ToString());
			if (NKMScoreRewardTemplet.Groups.ContainsKey(m_MiniGameTemplet.m_ScoreRewardGroupID))
			{
				List<NKMScoreRewardTemplet> lstRewardTemplet = NKMScoreRewardTemplet.Groups[m_MiniGameTemplet.m_ScoreRewardGroupID];
				NKCUtil.SetGameobjectActive(m_objReddot, CanGetAnyReward(lstRewardTemplet, num));
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
	}

	public override bool OnBackButton()
	{
		if (m_SwordTraining.gameObject.activeInHierarchy)
		{
			return true;
		}
		return false;
	}

	private bool CanGetAnyReward(List<NKMScoreRewardTemplet> lstRewardTemplet, long bestScore)
	{
		for (int i = 0; i < lstRewardTemplet.Count; i++)
		{
			if (lstRewardTemplet[i].m_Score <= bestScore)
			{
				if (!NKCScenManager.CurrentUserData().GetIsMiniGameReceviedID(lstRewardTemplet[i].m_ScoreRewardID))
				{
					return true;
				}
				continue;
			}
			return false;
		}
		return false;
	}

	private void OnExitGame()
	{
		NKCPacketSender.Send_NKMPacket_MINI_GAME_INFO_REQ(NKM_MINI_GAME_TYPE.SWORD_TRAINING, m_MiniGameTemplet.Key);
		m_SwordTraining?.CleanUp();
		NKCUIModuleHome.PlayBGMMusic();
		m_Ani.SetTrigger("02_to_01");
	}

	private void OnClickRule()
	{
		if (m_NKCPopupImage == null)
		{
			m_NKCPopupImage = NKCPopupImage.OpenInstance("UI_SINGLE_SWORDTRAINING", "UI_SINGLE_POPUP_SWORDTRAINING_RULE");
		}
		m_NKCPopupImage.Open(NKCStringTable.GetString(m_MiniGameTemplet.m_BannerTitle), NKCStringTable.GetString(m_MiniGameTemplet.m_BannerDesc));
	}

	private void OnClickReward()
	{
		if (m_RewardPopup == null)
		{
			m_RewardPopup = NKCPopupScoreReward.MakeInstance("UI_SINGLE_SWORDTRAINING", "UI_SINGLE_POPUP_SWORDTRAINING_REWARD");
		}
		int myBestScore = 0;
		NKMMiniGameData miniGameData = NKCScenManager.CurrentUserData().GetMiniGameData(NKM_MINI_GAME_TYPE.SWORD_TRAINING, m_MiniGameTemplet.Key);
		if (miniGameData != null)
		{
			myBestScore = (int)miniGameData.score;
		}
		m_RewardPopup.Open(m_MiniGameTemplet.m_ScoreRewardGroupID, myBestScore, "", IsReceivedPointReward, OnClickRewardSlot, OnClickReceiveAll, bIsSwordGame: true);
	}

	private bool IsReceivedPointReward(int rewardId)
	{
		return NKCScenManager.CurrentUserData().GetIsMiniGameReceviedID(rewardId);
	}

	private void OnClickStart()
	{
		StartCoroutine(OnStart());
	}

	private void OnClickRewardSlot(int rewardId)
	{
		NKCPacketSender.Send_NKMPacket_MINI_GAME_REWARD_REQ(m_MiniGameTemplet.Key, rewardId);
	}

	private void OnClickReceiveAll(int rewardId)
	{
		NKCPacketSender.Send_NKMPacket_MINI_GAME_REWARD_ALL_REQ(m_MiniGameTemplet.Key);
	}

	private IEnumerator OnStart()
	{
		yield return new WaitForSeconds(0.1f);
		m_Ani.SetTrigger("01_to_02");
		yield return null;
		while (!(m_Ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
		{
			yield return null;
		}
		yield return null;
		m_Ani.SetTrigger("02_IDLE");
		m_SwordTraining.Open(m_MiniGameTemplet.Key);
	}
}
