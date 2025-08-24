using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupResourceTextConfirmBox : NKCUIBase
{
	public delegate void OnButton();

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_RESOURCE_USE_CONFIRM_TXT";

	private static NKCPopupResourceTextConfirmBox m_Instance;

	private OnButton dOnOKButton;

	private OnButton dOnCancelButton;

	public Text m_lbTitle;

	public Text m_lbMessage;

	public GameObject m_objPoint;

	public Text m_txtPoint;

	public NKCUIComButton m_cbtnOK;

	public NKCUIComButton m_cbtnCancel;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public static NKCPopupResourceTextConfirmBox Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupResourceTextConfirmBox>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_RESOURCE_USE_CONFIRM_TXT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupResourceTextConfirmBox>();
				m_Instance?.Init();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => string.Empty;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void Init()
	{
		if (m_cbtnOK != null)
		{
			m_cbtnOK.PointerClick.RemoveAllListeners();
			m_cbtnOK.PointerClick.AddListener(OnOK);
			NKCUtil.SetHotkey(m_cbtnOK, HotkeyEventType.Confirm);
		}
		if (m_cbtnCancel != null)
		{
			m_cbtnCancel.PointerClick.RemoveAllListeners();
			m_cbtnCancel.PointerClick.AddListener(OnCancel);
		}
	}

	public void Open(string Title, string Content, string strPoint, OnButton onOkButton, OnButton onCancelButton = null)
	{
		if (m_NKCUIOpenAnimator == null)
		{
			m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		}
		NKCUtil.SetLabelText(m_lbTitle, Title);
		NKCUtil.SetLabelText(m_lbMessage, Content);
		NKCUtil.SetGameobjectActive(m_objPoint, bValue: true);
		NKCUtil.SetLabelText(m_txtPoint, strPoint);
		dOnOKButton = onOkButton;
		dOnCancelButton = onCancelButton;
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public void OnOK()
	{
		Close();
		if (dOnOKButton != null)
		{
			dOnOKButton();
		}
	}

	public void OnCancel()
	{
		Close();
		if (dOnCancelButton != null)
		{
			dOnCancelButton();
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}
}
