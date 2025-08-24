using NKC.UI.Event;
using NKM.Event;

namespace NKC.UI.Module;

public class NKCUIModuleSubUIBar : NKCUIModuleSubUIBase
{
	private NKCUIEventSubUI m_eventSubUI;

	private NKCUIEventSubUIBar m_eventSubUIBar;

	public NKCUIEventSubUIBar EventSubUIBar => m_eventSubUIBar;

	public override void Init()
	{
		base.Init();
		m_eventSubUI = GetComponent<NKCUIEventSubUI>();
		m_eventSubUI?.Init();
		m_eventSubUIBar = GetComponent<NKCUIEventSubUIBar>();
	}

	public override void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
		if (!(m_eventSubUIBar == null))
		{
			m_eventSubUIBar.Open(templet);
		}
	}

	public override void OnOpen(NKMEventTabTemplet eventTabTemplet)
	{
		if (!(m_eventSubUIBar == null))
		{
			m_eventSubUIBar.Open(eventTabTemplet);
		}
	}

	public override void Refresh()
	{
		m_eventSubUIBar?.Refresh();
	}

	public override void OnClose()
	{
		m_eventSubUIBar?.Close();
	}

	public override void Hide()
	{
		m_eventSubUIBar?.Hide();
	}

	public override bool OnBackButton()
	{
		if (m_eventSubUIBar == null)
		{
			return false;
		}
		return m_eventSubUIBar.OnClickClose();
	}
}
