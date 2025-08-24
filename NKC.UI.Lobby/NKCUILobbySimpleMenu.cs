using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Lobby;

public class NKCUILobbySimpleMenu : NKCUILobbyMenuButtonBase
{
	public delegate bool DotEnableConditionFunction(NKMUserData userData);

	public delegate void OnButton();

	public GameObject m_objReddot;

	public NKCUIComStateButton m_csbtnButton;

	private DotEnableConditionFunction dDotEnableConditionFunction;

	private OnButton dOnButton;

	public void Init(DotEnableConditionFunction conditionFunc, OnButton onButton, ContentsType contentsType = ContentsType.None)
	{
		dDotEnableConditionFunction = conditionFunc;
		dOnButton = onButton;
		m_ContentsType = contentsType;
		m_csbtnButton.PointerClick.RemoveAllListeners();
		m_csbtnButton.PointerClick.AddListener(OnBtn);
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		bool notify = dDotEnableConditionFunction != null && dDotEnableConditionFunction(userData);
		SetNotify(notify);
	}

	protected override void UpdateLock()
	{
		m_bLocked = !NKCContentManager.IsContentsUnlocked(m_ContentsType);
		if (m_csbtnButton != null)
		{
			m_csbtnButton.SetLock(m_bLocked);
		}
	}

	protected override void SetNotify(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objReddot, value);
		base.SetNotify(value);
	}

	private void OnBtn()
	{
		if (m_bLocked)
		{
			NKCContentManager.ShowLockedMessagePopup(m_ContentsType);
		}
		else if (dOnButton != null)
		{
			dOnButton();
		}
	}
}
