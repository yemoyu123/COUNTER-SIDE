using System.Collections.Generic;
using ClientPacket.Event;
using NKC.UI.Event;
using NKM.Event;
using UnityEngine;

namespace NKC.UI.Module;

[RequireComponent(typeof(NKCUIEventSubUIBase))]
public class NKCUIModuleEventAdapter : NKCUIModuleSubUIBase
{
	public class EventModuleDataBingo : NKCUIModuleHome.EventModuleMessageDataBase
	{
		public List<NKMBingoTile> bingoList;

		public bool bRandom;
	}

	private NKMEventTabTemplet m_eventTabTemplet;

	private NKCUIEventSubUIBase m_EventSubUI;

	private NKCUIEventSubUIBase EventSubUI
	{
		get
		{
			if (m_EventSubUI != null)
			{
				return m_EventSubUI;
			}
			if (!TryGetComponent<NKCUIEventSubUIBase>(out m_EventSubUI))
			{
				return null;
			}
			return m_EventSubUI;
		}
	}

	public override void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
		if (!(EventSubUI == null) && templet != null && templet.m_Option != null)
		{
			int intValue = NKCUtil.GetIntValue(templet.m_Option, "EventTabID", 0);
			m_eventTabTemplet = NKMEventTabTemplet.Find(intValue);
			if (m_eventTabTemplet != null)
			{
				EventSubUI.Open(m_eventTabTemplet);
			}
		}
	}

	public override void OnOpen(NKMEventTabTemplet eventTabTemplet)
	{
		if (!(EventSubUI == null))
		{
			m_eventTabTemplet = eventTabTemplet;
			EventSubUI.Open(m_eventTabTemplet);
		}
	}

	public override void OnClose()
	{
		if (!(EventSubUI == null))
		{
			EventSubUI.Close();
		}
	}

	public override void Init()
	{
		if (!(EventSubUI == null))
		{
			EventSubUI.Init();
		}
	}

	public override void Refresh()
	{
		if (!(EventSubUI == null))
		{
			EventSubUI.Refresh();
		}
	}

	public override void UnHide()
	{
		if (!(EventSubUI == null))
		{
			EventSubUI.UnHide();
		}
	}

	public override void Hide()
	{
		if (!(EventSubUI == null))
		{
			EventSubUI.Hide();
		}
	}

	public override bool OnBackButton()
	{
		if (EventSubUI == null)
		{
			return false;
		}
		return EventSubUI.OnBackButton();
	}

	public override void PassData(NKCUIModuleHome.EventModuleMessageDataBase data)
	{
		if (m_eventTabTemplet != null && m_eventTabTemplet.m_EventID == data.eventID && !(EventSubUI == null) && data is EventModuleDataBingo eventModuleDataBingo)
		{
			if (EventSubUI is NKCUIEventSubUIBingo)
			{
				(EventSubUI as NKCUIEventSubUIBingo).MarkBingo(eventModuleDataBingo.bingoList, eventModuleDataBingo.bRandom);
			}
			else if (EventSubUI is NKCUIEventSubUIBingoV2)
			{
				(EventSubUI as NKCUIEventSubUIBingoV2).MarkBingo(eventModuleDataBingo.bingoList, eventModuleDataBingo.bRandom);
			}
		}
	}
}
