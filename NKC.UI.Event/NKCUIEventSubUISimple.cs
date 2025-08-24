using NKM;
using NKM.Event;
using UnityEngine.EventSystems;

namespace NKC.UI.Event;

public class NKCUIEventSubUISimple : NKCUIEventSubUIBase
{
	public NKM_SHORTCUT_TYPE m_ShortcutType;

	public string m_ShortcutParam;

	public override void Init()
	{
		base.Init();
		EventTrigger component = GetComponent<EventTrigger>();
		if (component != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnTouch);
			component.triggers.Clear();
			component.triggers.Add(entry);
		}
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		m_tabTemplet = tabTemplet;
		if (m_tabTemplet != null)
		{
			m_ShortcutType = m_tabTemplet.m_ShortCutType;
			m_ShortcutParam = m_tabTemplet.m_ShortCut;
			SetDateLimit();
		}
	}

	public override void Refresh()
	{
	}

	private void OnTouch(BaseEventData baseEventData)
	{
		if (m_ShortcutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE && CheckEventTime())
		{
			NKCContentManager.MoveToShortCut(m_ShortcutType, m_ShortcutParam);
		}
	}
}
