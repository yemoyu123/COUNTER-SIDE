using System;
using System.Collections.Generic;
using NKC.UI.Component;
using NKC.UI.Event;
using NKM;
using NKM.Event;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.UI.Lobby;

public class NKCUILobbyEventPanel : MonoBehaviour
{
	private DateTime m_tFirstEventEndDateUTC;

	[Header("이벤트")]
	public GameObject m_objEvent;

	public NKCUILobbyMenuEvent m_pfbEventButton;

	public NKCUIComDragSelectablePanel m_EventSlidePanel;

	public GameObject m_objRedDot;

	private List<NKMEventTabTemplet> m_lstEventTabTemplet = new List<NKMEventTabTemplet>();

	private Stack<NKCUILobbyMenuEvent> m_stkEventObjects = new Stack<NKCUILobbyMenuEvent>();

	private float m_fDelatTime;

	public void Init()
	{
		if (m_EventSlidePanel != null)
		{
			m_EventSlidePanel.Init(rotation: true);
			m_EventSlidePanel.dOnGetObject += GetObject;
			m_EventSlidePanel.dOnReturnObject += ReturnObject;
			m_EventSlidePanel.dOnProvideData += ProvideData;
		}
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
	}

	public void SetData(NKMUserData userData)
	{
		SetEventList();
		if (NKCContentManager.IsContentsUnlocked(ContentsType.LOBBY_EVENT) && m_lstEventTabTemplet.Count > 0 && m_EventSlidePanel != null)
		{
			NKCUtil.SetGameobjectActive(m_objEvent, bValue: true);
			m_EventSlidePanel.TotalCount = m_lstEventTabTemplet.Count;
			m_EventSlidePanel.SetIndex(0);
			CheckReddot();
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objEvent, bValue: false);
		}
	}

	public void CheckReddot(bool checkShowBannerOnly = true)
	{
		NKCUtil.SetGameobjectActive(m_objRedDot, NKMEventManager.CheckRedDot(checkShowBannerOnly));
	}

	private RectTransform GetObject()
	{
		if (m_stkEventObjects.Count > 0)
		{
			NKCUILobbyMenuEvent nKCUILobbyMenuEvent = m_stkEventObjects.Pop();
			NKCUtil.SetGameobjectActive(nKCUILobbyMenuEvent, bValue: false);
			return nKCUILobbyMenuEvent.GetComponent<RectTransform>();
		}
		NKCUILobbyMenuEvent nKCUILobbyMenuEvent2 = UnityEngine.Object.Instantiate(m_pfbEventButton);
		NKCUtil.SetGameobjectActive(nKCUILobbyMenuEvent2, bValue: false);
		return nKCUILobbyMenuEvent2.GetComponent<RectTransform>();
	}

	private void ReturnObject(RectTransform rect)
	{
		NKCUILobbyMenuEvent component = rect.GetComponent<NKCUILobbyMenuEvent>();
		if (component != null)
		{
			m_stkEventObjects.Push(component);
		}
		NKCUtil.SetGameobjectActive(rect, bValue: false);
		rect.parent = base.transform;
	}

	private void ProvideData(RectTransform rect, int idx)
	{
		NKCUILobbyMenuEvent component = rect.GetComponent<NKCUILobbyMenuEvent>();
		if (component != null)
		{
			if (idx >= 0 && idx < m_lstEventTabTemplet.Count)
			{
				NKCUtil.SetGameobjectActive(component, bValue: true);
				rect.SetParent(m_EventSlidePanel.transform);
				component.SetData(m_lstEventTabTemplet[idx], OnBtnBanner);
			}
			else
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
		}
	}

	private void OnBtnBanner(NKMEventTabTemplet tabTemplet)
	{
		NKCUIEvent.Instance.Open(tabTemplet);
	}

	private void SetEventList()
	{
		m_lstEventTabTemplet.Clear();
		m_tFirstEventEndDateUTC = default(DateTime);
		foreach (NKMEventTabTemplet value in NKMTempletContainer<NKMEventTabTemplet>.Values)
		{
			if (value.IsAvailable && value.ShowEventBanner())
			{
				m_lstEventTabTemplet.Add(value);
				if (m_tFirstEventEndDateUTC.Ticks == 0L || m_tFirstEventEndDateUTC > value.EventDateEndUtc)
				{
					m_tFirstEventEndDateUTC = value.EventDateEndUtc;
				}
			}
		}
		m_lstEventTabTemplet.Sort(CompEventTabTemplet);
	}

	private int CompEventTabTemplet(NKMEventTabTemplet lItem, NKMEventTabTemplet rItem)
	{
		return lItem.m_OrderList.CompareTo(rItem.m_OrderList);
	}

	private void Update()
	{
		if (m_tFirstEventEndDateUTC.Ticks <= 0)
		{
			return;
		}
		m_fDelatTime += Time.deltaTime;
		if (m_fDelatTime > 1f)
		{
			m_fDelatTime -= 1f;
			if (m_tFirstEventEndDateUTC < NKCSynchronizedTime.GetServerUTCTime())
			{
				SetData(NKCScenManager.CurrentUserData());
			}
		}
	}
}
