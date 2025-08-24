using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuLeaderBoard : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_csbtnMenu;

	public GameObject m_objNotify;

	public void Init(ContentsType contentsType = ContentsType.None)
	{
		if (m_csbtnMenu != null)
		{
			m_csbtnMenu.PointerClick.RemoveAllListeners();
			m_csbtnMenu.PointerClick.AddListener(OnButton);
		}
		m_ContentsType = contentsType;
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		bool flag = NKCAlarmManager.CheckleaderBoardNotify(userData);
		NKCUtil.SetGameobjectActive(m_objNotify, flag);
		SetNotify(flag);
	}

	protected override void SetNotify(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objNotify, value);
		base.SetNotify(value);
	}

	private void OnButton()
	{
		if (m_bLocked)
		{
			NKCContentManager.ShowLockedMessagePopup(m_ContentsType);
		}
		else
		{
			NKCUILeaderBoard.Instance.Open();
		}
	}
}
