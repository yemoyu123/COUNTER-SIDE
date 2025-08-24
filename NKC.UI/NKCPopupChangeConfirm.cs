using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupChangeConfirm : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_CHANGE_CONFIRM";

	private static NKCPopupChangeConfirm m_Instance;

	public Text m_NKM_UI_POPUP_CHANGE_CONFIRM_TOP_TEXT;

	public Text m_NKM_UI_POPUP_CHANGE_CONFIRM_TEXT_BEFORE;

	public Text m_NKM_UI_POPUP_CHANGE_CONFIRM_TEXT_AFTER;

	public Text m_NKM_UI_POPUP_CHANGE_CONFIRM_TEXT;

	public NKCUIComStateButton m_NKM_UI_POPUP_OK_CANCEL_BOX_OK;

	public NKCUIComStateButton m_NKM_UI_POPUP_OK_CANCEL_BOX_CANCEL;

	public static NKCPopupChangeConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupChangeConfirm>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_CHANGE_CONFIRM", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupChangeConfirm>();
				if (m_Instance != null)
				{
					m_Instance.InitUI();
				}
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "변경 확인 팝업";

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void InitUI()
	{
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public void Open(string title, string before, string after, string desc, UnityAction ok, UnityAction cancel = null)
	{
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CHANGE_CONFIRM_TOP_TEXT, title);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CHANGE_CONFIRM_TEXT_BEFORE, before);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CHANGE_CONFIRM_TEXT_AFTER, after);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CHANGE_CONFIRM_TEXT, desc);
		m_NKM_UI_POPUP_OK_CANCEL_BOX_OK.PointerClick.RemoveAllListeners();
		if (ok != null)
		{
			m_NKM_UI_POPUP_OK_CANCEL_BOX_OK.PointerClick.AddListener(ok);
		}
		m_NKM_UI_POPUP_OK_CANCEL_BOX_OK.PointerClick.AddListener(CheckInstanceAndClose);
		NKCUtil.SetHotkey(m_NKM_UI_POPUP_OK_CANCEL_BOX_OK, HotkeyEventType.Confirm);
		m_NKM_UI_POPUP_OK_CANCEL_BOX_CANCEL.PointerClick.RemoveAllListeners();
		if (cancel != null)
		{
			m_NKM_UI_POPUP_OK_CANCEL_BOX_CANCEL.PointerClick.AddListener(cancel);
		}
		m_NKM_UI_POPUP_OK_CANCEL_BOX_CANCEL.PointerClick.AddListener(CheckInstanceAndClose);
		if (!base.gameObject.activeSelf)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		}
		UIOpened();
	}
}
