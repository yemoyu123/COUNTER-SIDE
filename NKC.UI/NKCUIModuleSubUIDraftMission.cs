using System.Collections;
using System.Collections.Generic;
using NKC.UI.Gauntlet;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIModuleSubUIDraftMission : MonoBehaviour
{
	public CanvasGroup m_canvasGroup;

	public LoopScrollRect m_LoopScrollRect;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnComplete;

	public Animator m_animator;

	public string m_outroAnimation;

	private Coroutine m_coroutine;

	private List<NKMMissionTemplet> m_lstMissionTemplet = new List<NKMMissionTemplet>();

	private Stack<NKCUIGauntletEventRewardSlot> m_stkSlot = new Stack<NKCUIGauntletEventRewardSlot>();

	public void InitUI()
	{
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.dOnGetObject += GetObject;
			m_LoopScrollRect.dOnReturnObject += ReturnObject;
			m_LoopScrollRect.dOnProvideData += ProvideData;
			m_LoopScrollRect.ContentConstraintCount = 1;
			m_LoopScrollRect.PrepareCells();
			m_LoopScrollRect.TotalCount = 0;
			m_LoopScrollRect.RefreshCells();
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnComplete, OnClickComplete);
	}

	public void Open(List<NKMMissionTemplet> lstMissionTemplet)
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
		m_lstMissionTemplet.Clear();
		if (lstMissionTemplet.Count > 0)
		{
			m_lstMissionTemplet = lstMissionTemplet;
		}
		m_LoopScrollRect.TotalCount = m_lstMissionTemplet.Count;
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
		bool flag = false;
		NKMUserData userData = NKCScenManager.CurrentUserData();
		for (int i = 0; i < m_lstMissionTemplet.Count; i++)
		{
			NKMMissionData missionData = NKMMissionManager.GetMissionData(m_lstMissionTemplet[i]);
			flag |= NKMMissionManager.CanComplete(m_lstMissionTemplet[i], userData, missionData) == NKM_ERROR_CODE.NEC_OK;
		}
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

	private RectTransform GetObject(int idx)
	{
		NKCUIGauntletEventRewardSlot nKCUIGauntletEventRewardSlot = null;
		return ((m_stkSlot.Count <= 0) ? NKCUIGauntletEventRewardSlot.GetNewInstance(m_LoopScrollRect.content, "AB_UI_EVENT_MD", "UI_POPUP_MISSION_LIST_SLOT") : m_stkSlot.Pop())?.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIGauntletEventRewardSlot component = tr.GetComponent<NKCUIGauntletEventRewardSlot>();
		m_stkSlot.Push(component);
		NKCUtil.SetGameobjectActive(component, bValue: false);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIGauntletEventRewardSlot component = tr.GetComponent<NKCUIGauntletEventRewardSlot>();
		if (idx >= m_lstMissionTemplet.Count)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(component, bValue: true);
		component.SetData(m_lstMissionTemplet[idx]);
	}

	private void OnClickComplete()
	{
		NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_ALL_REQ(m_lstMissionTemplet[0].m_MissionTabId);
	}

	public void OnCloseInstance()
	{
		m_coroutine = null;
		m_lstMissionTemplet?.Clear();
		m_lstMissionTemplet = null;
		m_LoopScrollRect?.ClearCells();
	}
}
