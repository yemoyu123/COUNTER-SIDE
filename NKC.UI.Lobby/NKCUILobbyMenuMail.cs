using System;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuMail : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_csbtnMenu;

	public GameObject m_objNotifyRoot;

	public Text m_lbMailCount;

	public void Init()
	{
		if (m_csbtnMenu != null)
		{
			m_csbtnMenu.PointerClick.RemoveAllListeners();
			m_csbtnMenu.PointerClick.AddListener(OnButton);
		}
		NKCMailManager.dOnMailFlagChange = (NKCMailManager.OnMailFlagChange)Delegate.Combine(NKCMailManager.dOnMailFlagChange, new NKCMailManager.OnMailFlagChange(OnMailFlagChange));
	}

	private void OnMailFlagChange(bool bHasNewMail)
	{
		_UpdateData();
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		_UpdateData();
	}

	private void _UpdateData()
	{
		int totalMailCount = NKCMailManager.GetTotalMailCount();
		bool flag = totalMailCount > 0;
		NKCUtil.SetGameobjectActive(m_objNotifyRoot, flag);
		if (flag)
		{
			NKCUtil.SetLabelText(m_lbMailCount, totalMailCount.ToString());
		}
		SetNotify(flag);
	}

	private void OnButton()
	{
		NKCUIMail.Instance.Open();
	}

	public override void CleanUp()
	{
		base.CleanUp();
		NKCMailManager.dOnMailFlagChange = (NKCMailManager.OnMailFlagChange)Delegate.Remove(NKCMailManager.dOnMailFlagChange, new NKCMailManager.OnMailFlagChange(OnMailFlagChange));
	}
}
