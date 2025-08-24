using System.Collections;
using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletEventReward : MonoBehaviour
{
	public CanvasGroup m_canvasGroup;

	public LoopScrollRect m_LoopScrollRect;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnComplete;

	public Animator m_animator;

	public string m_outroAnimation;

	private Coroutine m_coroutine;

	private List<NKMEventPvpRewardTemplet> m_eventPvpRewardTempletList = new List<NKMEventPvpRewardTemplet>();

	public void Init()
	{
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.dOnGetObject += GetPresetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnPresetSlot;
			m_LoopScrollRect.dOnProvideData += ProvidePresetData;
			m_LoopScrollRect.ContentConstraintCount = 1;
			m_LoopScrollRect.PrepareCells();
			m_LoopScrollRect.TotalCount = 0;
			m_LoopScrollRect.RefreshCells();
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnComplete, OnClickComplete);
	}

	public void Open()
	{
		if (m_canvasGroup != null)
		{
			m_canvasGroup.blocksRaycasts = true;
		}
		if (m_coroutine != null)
		{
			StopCoroutine(m_coroutine);
			m_coroutine = null;
			base.gameObject.SetActive(value: false);
		}
		base.gameObject.SetActive(value: true);
		m_eventPvpRewardTempletList.Clear();
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet != null)
		{
			foreach (List<NKMEventPvpRewardTemplet> value in eventPvpSeasonTemplet.EventPvpRewardTemplets.Values)
			{
				m_eventPvpRewardTempletList.AddRange(value);
			}
		}
		m_LoopScrollRect.TotalCount = m_eventPvpRewardTempletList.Count;
		m_LoopScrollRect.SetIndexPosition(0);
		SetCompleteButtonState();
	}

	public void Refresh()
	{
		m_LoopScrollRect.RefreshCells();
		SetCompleteButtonState();
	}

	public bool IsOpened()
	{
		if (base.gameObject.activeSelf)
		{
			return m_coroutine == null;
		}
		return false;
	}

	public void CloseImmediately()
	{
		if (m_coroutine != null)
		{
			StopCoroutine(m_coroutine);
			m_coroutine = null;
		}
		base.gameObject.SetActive(value: false);
	}

	public bool IsClosed()
	{
		if (m_coroutine == null)
		{
			return !base.gameObject.activeSelf;
		}
		return true;
	}

	public void Close()
	{
		if (base.gameObject.activeSelf && m_coroutine == null)
		{
			m_coroutine = StartCoroutine(Outro());
		}
	}

	private void SetCompleteButtonState()
	{
		m_csbtnComplete.SetLock(value: true);
		if (NKCEventPvpMgr.EventPvpRewardInfo == null)
		{
			m_csbtnComplete.SetLock(value: true);
			return;
		}
		bool flag = NKCEventPvpMgr.CanGetReward();
		m_csbtnComplete.SetLock(!flag);
	}

	private IEnumerator Outro()
	{
		if (m_canvasGroup != null)
		{
			m_canvasGroup.blocksRaycasts = false;
		}
		if (m_animator != null)
		{
			m_animator.Play(m_outroAnimation, -1, 0f);
			if (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f)
			{
				yield return null;
			}
			while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				yield return null;
			}
		}
		base.gameObject.SetActive(value: false);
		m_coroutine = null;
	}

	private void OnClickComplete()
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet != null)
		{
			NKCPacketSender.Send_NKMPacket_EVENT_PVP_REWARD_REQ(eventPvpSeasonTemplet.SeasonId);
		}
	}

	private RectTransform GetPresetSlot(int index)
	{
		return NKCUIGauntletEventRewardSlot.GetNewInstance(null, "ab_ui_nkm_ui_gauntlet", "POPUP_GAUNTLET_EVENTMATCH_REWARD_LIST_SLOT")?.GetComponent<RectTransform>();
	}

	private void ReturnPresetSlot(Transform tr)
	{
		NKCUIGauntletEventRewardSlot component = tr.GetComponent<NKCUIGauntletEventRewardSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	private void ProvidePresetData(Transform tr, int index)
	{
		NKCUIGauntletEventRewardSlot component = tr.GetComponent<NKCUIGauntletEventRewardSlot>();
		if (!(component == null))
		{
			if (index < m_eventPvpRewardTempletList.Count)
			{
				NKCUtil.SetGameobjectActive(component, bValue: true);
				component.SetData(m_eventPvpRewardTempletList[index]);
			}
			else
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
				component.SetEmpty();
			}
		}
	}

	public void OnCloseInstance()
	{
		m_coroutine = null;
		m_eventPvpRewardTempletList?.Clear();
		m_eventPvpRewardTempletList = null;
		m_LoopScrollRect?.ClearCells();
	}
}
