using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Game;
using Cs.Core.Util;
using NKC.UI.Component;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIModuleSubUITournamentTryout : MonoBehaviour
{
	public NKCComTMPUIText m_lbDesc;

	public NKCUIModuleSubUITournamentTryoutSlot m_pfbSlot;

	public LoopScrollRect m_loop;

	public GameObject m_objEmpty;

	public Text m_lbEmpty;

	public Text m_lbPlayTime;

	public NKCComTMPUIText m_lbRemainTime;

	public NKCUIComStateButton m_btnFinalUserList;

	public NKCUIComStateButton m_btnCheckAll;

	public GameObject m_objFailNotice;

	private List<NKMTournamentPlayInfo> m_lstHistory = new List<NKMTournamentPlayInfo>();

	private List<NKCUIModuleSubUITournamentTryoutSlot> m_lstVisibleSlot = new List<NKCUIModuleSubUITournamentTryoutSlot>();

	private Stack<NKCUIModuleSubUITournamentTryoutSlot> m_stkSlot = new Stack<NKCUIModuleSubUITournamentTryoutSlot>();

	private int m_RefreshIntervalMin = 5;

	private DateTime m_dNextRefreshTIme = DateTime.MinValue;

	private bool m_bNeedUpdateTime = true;

	private HashSet<int> m_hsShowFxTargetIndex = new HashSet<int>();

	private bool m_bShowUserList = true;

	private float m_fDeltaTime;

	public void InitUI()
	{
		m_btnCheckAll.PointerClick.RemoveAllListeners();
		m_btnCheckAll.PointerClick.AddListener(OnClickCheckAll);
		m_btnCheckAll.m_bGetCallbackWhileLocked = true;
		m_btnFinalUserList.PointerClick.RemoveAllListeners();
		m_btnFinalUserList.PointerClick.AddListener(OnClickFinalUserList);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		m_RefreshIntervalMin = NKMCommonConst.TournamentQualifyCoolTime / 60;
		m_hsShowFxTargetIndex = new HashSet<int>();
		m_bShowUserList = true;
	}

	public void Open()
	{
		m_fDeltaTime = 0f;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (NKCTournamentManager.m_TournamentTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbPlayTime, string.Format("{0} ~ {1}", NKCTournamentManager.m_TournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.Tryout).ToString("yyyy-MM-dd HH:mm"), NKCTournamentManager.m_TournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.Tryout).ToString("yyyy-MM-dd HH:mm")));
			NKCUtil.SetLabelText(m_lbDesc, NKCTournamentManager.m_TournamentTemplet.GetTournamentTryouyDesc());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbPlayTime, "");
			NKCUtil.SetLabelText(m_lbDesc, "");
		}
		m_lstHistory = NKCTournamentManager.GetTryoutHistory();
		m_bNeedUpdateTime = m_lstHistory.Count > NKCTournamentManager.GetTryoutCheckIndex() + 1 && NKCTournamentManager.m_TournamentApply;
		RefreshShowFxTarget();
		if (m_bNeedUpdateTime)
		{
			m_dNextRefreshTIme = new DateTime(ServiceTime.Now.Year, ServiceTime.Now.Month, ServiceTime.Now.Day, ServiceTime.Now.Hour, ServiceTime.Now.Minute / m_RefreshIntervalMin * m_RefreshIntervalMin + m_RefreshIntervalMin, 0);
		}
		RefreshList();
		RefreshTime();
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private RectTransform GetObject(int idx)
	{
		NKCUIModuleSubUITournamentTryoutSlot nKCUIModuleSubUITournamentTryoutSlot = null;
		nKCUIModuleSubUITournamentTryoutSlot = ((m_stkSlot.Count <= 0) ? UnityEngine.Object.Instantiate(m_pfbSlot, m_loop.content) : m_stkSlot.Pop());
		m_lstVisibleSlot.Add(nKCUIModuleSubUITournamentTryoutSlot);
		return nKCUIModuleSubUITournamentTryoutSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIModuleSubUITournamentTryoutSlot component = tr.GetComponent<NKCUIModuleSubUITournamentTryoutSlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		m_stkSlot.Push(component);
		m_lstVisibleSlot.Remove(component);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIModuleSubUITournamentTryoutSlot component = tr.GetComponent<NKCUIModuleSubUITournamentTryoutSlot>();
		if (m_lstHistory != null && m_lstHistory.Count > 0 && m_lstHistory[idx].history != null)
		{
			NKCUtil.SetGameobjectActive(component, bValue: true);
			new DateTime(m_lstHistory[idx].history.RegdateTick);
			component.SetData(m_lstHistory[idx], idx + 1, NKCTournamentManager.GetTryoutCheckIndex() >= idx, NeedShowFx(idx), OnClickSlotResult);
		}
	}

	public void Refresh()
	{
		RefreshTime();
	}

	public void RefreshList()
	{
		m_loop.TotalCount = GetCurrentListCount();
		m_loop.SetIndexPosition(GetCurrentListCount() - 1);
		NKCUtil.SetGameobjectActive(m_objEmpty, m_loop.TotalCount == 0);
		if (m_objEmpty.activeSelf)
		{
			if (ServiceTime.Now < NKCTournamentManager.m_TournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.Tryout))
			{
				NKCUtil.SetLabelText(m_lbEmpty, NKCUtilString.GET_STRING_TOURNAMENT_QUALIFY_EMPTY);
			}
			else if (!NKCTournamentManager.m_TournamentApply)
			{
				NKCUtil.SetLabelText(m_lbEmpty, NKCUtilString.GET_STRING_TOURNAMENT_QUALIFY_NO_ENTER);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbEmpty, NKCUtilString.GET_STRING_TOURNAMENT_QUALIFY_EMPTY);
			}
		}
		RefreshShowFxTarget();
		NKCUtil.SetGameobjectActive(m_objFailNotice, !m_bNeedUpdateTime && m_lstHistory.Count > 0 && m_lstHistory[m_lstHistory.Count - 1].history.Result == PVP_RESULT.LOSE);
		if (GetCurrentListCount() - 1 == NKCTournamentManager.GetTryoutCheckIndex() || m_lstHistory.Count == 0)
		{
			m_btnCheckAll.Lock();
		}
		else
		{
			m_btnCheckAll.UnLock();
		}
	}

	private void RefreshShowFxTarget()
	{
		m_hsShowFxTargetIndex.Clear();
		for (int i = 0; i < m_lstHistory.Count; i++)
		{
			if (i > NKCTournamentManager.GetTryoutCheckIndex())
			{
				SetShowFxTargetIndex(i);
			}
		}
	}

	private int GetCurrentListCount()
	{
		int num = 0;
		for (int i = 0; i < m_lstHistory.Count; i++)
		{
			DateTime dateTime = new DateTime(m_lstHistory[i].history.RegdateTick);
			if (ServiceTime.Now > dateTime.ToLocalTime())
			{
				int tryoutCheckIndex = NKCTournamentManager.GetTryoutCheckIndex();
				if (i > tryoutCheckIndex + 1)
				{
					break;
				}
				num++;
			}
		}
		return num;
	}

	public void RefreshTime()
	{
		if (m_bNeedUpdateTime && m_dNextRefreshTIme <= ServiceTime.Now)
		{
			m_dNextRefreshTIme = m_dNextRefreshTIme.AddMinutes(m_RefreshIntervalMin);
			RefreshList();
		}
		if (NKCTournamentManager.GetTryoutCheckIndex() + 1 >= m_lstHistory.Count || ServiceTime.Now > NKCTournamentManager.m_TournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.Tryout))
		{
			m_bNeedUpdateTime = false;
			NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: false);
			if (m_bShowUserList && ServiceTime.Now > NKCTournamentManager.m_TournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.Tryout))
			{
				m_bShowUserList = false;
				NKCUtil.SetGameobjectActive(m_btnFinalUserList, ServiceTime.Now > NKCTournamentManager.m_TournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.Tryout));
				OnClickFinalUserList();
			}
		}
		else
		{
			if (!m_lbRemainTime.gameObject.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: true);
			}
			NKCUtil.SetLabelText(m_lbRemainTime, string.Format(NKCUtilString.GET_STRING_TOURNAMENT_QUALIFY_COUNTDOWN, NKCUtilString.GetRemainTimeString(m_dNextRefreshTIme - ServiceTime.Now, 2)));
			NKCUtil.SetGameobjectActive(m_btnFinalUserList, ServiceTime.Now > NKCTournamentManager.m_TournamentTemplet.GetTournamentStateEndDate(NKMTournamentState.Tryout));
		}
	}

	public void Update()
	{
		if (m_bNeedUpdateTime && m_lbRemainTime.gameObject.activeSelf)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				RefreshTime();
			}
		}
	}

	private void OnClickSlotResult(int idx)
	{
		NKCTournamentManager.SetTryoutCheckIndex(idx);
		RefreshList();
	}

	private void OnClickCheckAll()
	{
		if (m_btnCheckAll.m_bLock)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < m_lstHistory.Count; i++)
		{
			DateTime dateTime = new DateTime(m_lstHistory[i].history.RegdateTick);
			if (ServiceTime.Now >= dateTime.ToLocalTime() && num < i)
			{
				num = i;
			}
		}
		NKCTournamentManager.SetTryoutCheckIndex(num);
		RefreshList();
	}

	private void OnClickFinalUserList()
	{
		NKCPopupTournamentFinalUserList.Instance.Open();
	}

	public void SetShowFxTargetIndex(int idx)
	{
		if (!m_hsShowFxTargetIndex.Contains(idx))
		{
			m_hsShowFxTargetIndex.Add(idx);
		}
	}

	public void RemoveShowFxTargetIdx(int idx)
	{
		if (m_hsShowFxTargetIndex.Contains(idx))
		{
			m_hsShowFxTargetIndex.Remove(idx);
		}
	}

	public bool NeedShowFx(int idx)
	{
		return m_hsShowFxTargetIndex.Contains(idx);
	}
}
