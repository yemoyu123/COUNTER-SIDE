using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuFriends : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_csbtnMenu;

	public GameObject m_objNotify;

	public Text m_lbFriendCount;

	public Text m_lbFriendReqCount;

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
		bool flag = NKCAlarmManager.CheckFriendNotify(userData);
		NKCUtil.SetGameobjectActive(m_objNotify, flag);
		NKCUtil.SetLabelText(m_lbFriendCount, NKCFriendManager.FriendList.Count.ToString());
		NKCUtil.SetLabelText(m_lbFriendReqCount, NKCFriendManager.ReceivedREQList.Count.ToString());
		SetNotify(flag);
	}

	private void OnButton()
	{
		if (m_bLocked)
		{
			NKCContentManager.ShowLockedMessagePopup(m_ContentsType);
		}
		else
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_FRIEND);
		}
	}
}
