using ClientPacket.Common;
using DG.Tweening;
using NKC.UI.Event;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIComKillCountTotal : MonoBehaviour
{
	public Text m_lbCurrentRewardStep;

	public Text m_lbServerCurrentKillCnt;

	public Text m_lbServerRewardKillCnt;

	public NKCUISlot m_serverRewardSlot;

	public Image m_KillCountGauge;

	public GameObject m_objComplet;

	public GameObject m_objRefreshEffect;

	public GameObject m_objSlotRewardOn;

	public NKCUIComStateButton m_csbtnServerReward;

	public NKCUIComStateButton m_csbtnServerRewardComplete;

	public NKCUIComStateButton m_csbtnServerRewardProgress;

	private int m_iEventId;

	private int m_iMaxServerStep;

	private const int StepMin = 1;

	public void Init()
	{
		m_serverRewardSlot?.Init();
		NKCUtil.SetButtonClickDelegate(m_csbtnServerReward, OnClickViewServerReward);
		NKCUtil.SetButtonClickDelegate(m_csbtnServerRewardProgress, OnClickProgress);
		NKCUtil.SetButtonClickDelegate(m_csbtnServerRewardComplete, OnClickComplete);
	}

	public void SetData(int eventId)
	{
		m_iEventId = eventId;
		m_iMaxServerStep = 0;
		long num = 0L;
		int num2 = 0;
		NKMServerKillCountData killCountServerData = NKCKillCountManager.GetKillCountServerData(eventId);
		if (killCountServerData != null)
		{
			num = killCountServerData.killCount;
		}
		NKMKillCountData killCountData = NKCKillCountManager.GetKillCountData(eventId);
		if (killCountData != null)
		{
			num2 = killCountData.serverCompleteStep;
		}
		NKCUtil.SetLabelText(m_lbServerCurrentKillCnt, $"{num:#,0}");
		NKMKillCountTemplet nKMKillCountTemplet = NKMKillCountTemplet.Find(eventId);
		if (nKMKillCountTemplet != null)
		{
			m_iMaxServerStep = nKMKillCountTemplet.GetMaxServerStep();
			NKCUtil.SetLabelText(m_lbCurrentRewardStep, string.Format(NKCUtilString.GET_STRING_KILLCOUNT_SERVER_REWARD_CURRENT_STEP, Mathf.Min(num2 + 1, m_iMaxServerStep)));
			NKMKillCountStepTemplet result = null;
			nKMKillCountTemplet.TryGetServerStep(num2 + 1, out result);
			NKMKillCountStepTemplet result2 = null;
			nKMKillCountTemplet.TryGetServerStep(num2, out result2);
			float num3 = 0f;
			if (result != null)
			{
				int killCount = result.KillCount;
				int num4 = result2?.KillCount ?? 0;
				NKCUtil.SetLabelText(m_lbServerRewardKillCnt, $"{killCount:#,0}");
				num3 = (float)(num - num4) / (float)(killCount - num4);
				NKCUtil.SetImageFillAmount(m_KillCountGauge, num3);
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(result.RewardInfo);
				m_serverRewardSlot.SetData(data);
			}
			else
			{
				NKMKillCountStepTemplet result3 = null;
				nKMKillCountTemplet.TryGetServerStep(num2 - 1, out result3);
				int num5 = result3?.KillCount ?? 0;
				int num6 = result2?.KillCount ?? 0;
				NKCUtil.SetLabelText(m_lbServerRewardKillCnt, $"{num6:#,0}");
				num3 = (float)(num - num5) / (float)(num6 - num5);
				NKCUtil.SetImageFillAmount(m_KillCountGauge, num3);
				NKCUISlot.SlotData data2 = NKCUISlot.SlotData.MakeRewardTypeData(result2.RewardInfo);
				m_serverRewardSlot.SetData(data2);
			}
			NKCUtil.SetGameobjectActive(m_csbtnServerRewardProgress.gameObject, num3 < 1f);
			NKCUtil.SetGameobjectActive(m_csbtnServerRewardComplete.gameObject, num3 >= 1f);
			if (num2 >= m_iMaxServerStep)
			{
				NKCUtil.SetGameobjectActive(m_csbtnServerRewardProgress.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnServerRewardComplete.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_objSlotRewardOn, bValue: false);
				NKCUtil.SetGameobjectActive(m_objComplet, bValue: true);
				m_serverRewardSlot.SetDisable(disable: true);
				m_serverRewardSlot.SetEventGet(bActive: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objSlotRewardOn, num3 >= 1f);
				NKCUtil.SetGameobjectActive(m_objComplet, bValue: false);
				m_serverRewardSlot.SetDisable(disable: false);
				m_serverRewardSlot.SetEventGet(bActive: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objRefreshEffect, bValue: false);
		if (NKCUIEventSubUIHorizon.RewardGet)
		{
			NKCUtil.SetGameobjectActive(m_objRefreshEffect, bValue: true);
			m_objRefreshEffect.GetComponentInChildren<DOTweenAnimation>()?.DORestart();
			NKCUIEventSubUIHorizon.RewardGet = false;
		}
	}

	private void OnClickProgress()
	{
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_OPERATION, "EC_EVENT");
	}

	private void OnClickComplete()
	{
		NKMKillCountData killCountData = NKCKillCountManager.GetKillCountData(m_iEventId);
		int num = 0;
		if (killCountData != null)
		{
			num = killCountData.serverCompleteStep;
		}
		if (m_iMaxServerStep <= num)
		{
			return;
		}
		NKMKillCountTemplet nKMKillCountTemplet = NKMKillCountTemplet.Find(m_iEventId);
		if (nKMKillCountTemplet == null)
		{
			return;
		}
		if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), nKMKillCountTemplet.UnlockInfo))
		{
			string text = "";
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(nKMKillCountTemplet.UnlockInfo.reqValue);
			text = ((nKMKillCountTemplet.UnlockInfo.eReqType != STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_STAGE || nKMStageTempletV == null) ? NKCContentManager.MakeUnlockConditionString(nKMKillCountTemplet.UnlockInfo, bSimple: false) : (nKMStageTempletV.GetDungeonName() + " " + NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_STAGE")));
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(text, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		NKMKillCountStepTemplet result = null;
		nKMKillCountTemplet.TryGetServerStep(num + 1, out result);
		long num2 = 0L;
		NKMServerKillCountData killCountServerData = NKCKillCountManager.GetKillCountServerData(m_iEventId);
		if (killCountServerData != null)
		{
			num2 = killCountServerData.killCount;
		}
		if (result != null && result.KillCount <= num2)
		{
			NKCPacketSender.Send_NKMPacket_KILL_COUNT_SERVER_REWARD_REQ(m_iEventId, num + 1);
		}
	}

	private void OnClickViewServerReward()
	{
		NKCPopupEventKillCountReward.Instance.Open(m_iEventId);
	}
}
