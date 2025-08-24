using NKC.UI.Component;
using NKM.Event;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventSubUIHorizon : NKCUIEventSubUIBase
{
	public Text m_lbEventDesc;

	public NKCUIComKillCountTotal m_KillCountTotal;

	public NKCUIComKillCountPrivate m_KillCountPrivate;

	public static bool RewardGet;

	public override void Init()
	{
		base.Init();
		m_KillCountTotal.Init();
		m_KillCountPrivate.Init();
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		RewardGet = false;
		if (tabTemplet != null)
		{
			m_tabTemplet = tabTemplet;
			if (NKCStringTable.CheckExistString(tabTemplet.m_EventHelpDesc))
			{
				NKCUtil.SetLabelText(m_lbEventDesc, NKCStringTable.GetString(tabTemplet.m_EventHelpDesc));
			}
			m_KillCountTotal.SetData(tabTemplet.m_EventID);
			m_KillCountPrivate.SetData(tabTemplet.m_EventID);
		}
	}

	public override void Refresh()
	{
		if (m_tabTemplet != null)
		{
			m_KillCountTotal.SetData(m_tabTemplet.m_EventID);
			m_KillCountPrivate.SetData(m_tabTemplet.m_EventID);
		}
	}
}
