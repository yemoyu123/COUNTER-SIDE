using NKC.Templet;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyEventShortCut : NKCUILobbyMenuButtonBase
{
	public delegate bool DotEnableConditionFunction(NKMUserData userData);

	public delegate void OnButton(NKCLobbyIconTemplet templet);

	public GameObject m_objReddot;

	public NKCUIComStateButton m_csbtnButton;

	public Image m_imgIcon;

	public Text m_lbDesc;

	private DotEnableConditionFunction dDotEnableConditionFunction;

	private OnButton dOnButton;

	private UnlockInfo m_unlockInfo;

	private NKCLobbyIconTemplet m_templet;

	public void Init(DotEnableConditionFunction conditionFunc, OnButton onButton, NKCLobbyIconTemplet templet = null)
	{
		dDotEnableConditionFunction = conditionFunc;
		dOnButton = onButton;
		m_templet = templet;
		if (m_templet != null)
		{
			m_unlockInfo = new UnlockInfo(m_templet.m_UnlockReqType, m_templet.m_UnlockReqValue);
			if (!string.IsNullOrEmpty(m_templet.m_IconName))
			{
				NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
				NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LOBBY_SPRITE", m_templet.m_IconName));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_imgIcon, bValue: false);
			}
			if (!string.IsNullOrEmpty(m_templet.m_Desc))
			{
				NKCUtil.SetGameobjectActive(m_lbDesc, bValue: true);
				NKCUtil.SetLabelText(m_lbDesc, m_templet.GetDesc());
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lbDesc, bValue: false);
			}
		}
		m_csbtnButton.PointerClick.RemoveAllListeners();
		m_csbtnButton.PointerClick.AddListener(OnBtn);
	}

	protected override void UpdateLock()
	{
		m_bLocked = !NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in m_unlockInfo);
		NKCUtil.SetLabelText(m_lbLock, NKCContentManager.MakeUnlockConditionString(in m_unlockInfo, bSimple: true));
		NKCUtil.SetGameobjectActive(m_objLock, m_bLocked);
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		bool flag = dDotEnableConditionFunction != null && dDotEnableConditionFunction(userData);
		SetNotify(flag);
		NKCUtil.SetGameobjectActive(m_objReddot, flag);
	}

	private void OnBtn()
	{
		if (m_bLocked)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCContentManager.MakeUnlockConditionString(in m_unlockInfo, bSimple: false));
		}
		else if (dOnButton != null)
		{
			dOnButton(m_templet);
		}
	}
}
