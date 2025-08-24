using Cs.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupAccountCodeInput : NKCUIBase
{
	private const string DEBUG_HEADER = "[SteamLink][CodeInput]";

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_ACCOUNT_LINK";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_ACCOUNT_CODE_INPUT";

	private static NKCPopupAccountCodeInput m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComStateButton m_ok;

	public NKCUIComStateButton m_cancel;

	public Text m_titleText;

	public Text m_descriptionText;

	public InputField m_inputField;

	private bool m_isStartingProcess = true;

	public static NKCPopupAccountCodeInput Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupAccountCodeInput>("AB_UI_NKM_UI_ACCOUNT_LINK", "NKM_UI_POPUP_ACCOUNT_CODE_INPUT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupAccountCodeInput>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "AccountLink";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void OnBackButton()
	{
		if (m_isStartingProcess)
		{
			Close();
		}
		else
		{
			OnClickClose();
		}
	}

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(bool isStartingProcess)
	{
		Log.Debug("[SteamLink][CodeInput] Open", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountCodeInput.cs", 76);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_cancel?.PointerClick.RemoveAllListeners();
		if (m_inputField != null)
		{
			m_inputField.text = "";
			m_inputField.inputType = InputField.InputType.AutoCorrect;
			if (isStartingProcess)
			{
				m_inputField.keyboardType = TouchScreenKeyboardType.NumberPad;
				m_inputField.contentType = InputField.ContentType.IntegerNumber;
			}
			else
			{
				m_inputField.keyboardType = TouchScreenKeyboardType.Default;
				m_inputField.contentType = InputField.ContentType.Alphanumeric;
			}
		}
		m_ok?.PointerClick.RemoveAllListeners();
		if (isStartingProcess)
		{
			m_ok?.PointerClick.AddListener(TrySendPublisherCode);
			m_cancel?.PointerClick.AddListener(base.Close);
			NKCUtil.SetLabelText(m_titleText, NKCStringTable.GetString("SI_PF_STEAMLINK_MEMBERSHIP_TITLE"));
			NKCUtil.SetLabelText(m_descriptionText, NKCStringTable.GetString("SI_PF_STEAMLINK_MEMBERSHIP_DESC"));
		}
		else
		{
			m_ok?.PointerClick.AddListener(TrySendPrivateLinkCode);
			m_cancel?.PointerClick.AddListener(OnClickClose);
			NKCUtil.SetLabelText(m_titleText, NKCStringTable.GetString("SI_PF_STEAMLINK_CODE_ENTER_TITLE"));
			NKCUtil.SetLabelText(m_descriptionText, NKCStringTable.GetString("SI_PF_STEAMLINK_CODE_ENTER_DESC"));
		}
		UIOpened();
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public override void CloseInternal()
	{
		Log.Debug("[SteamLink][CodeInput] CloseInternal", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountCodeInput.cs", 132);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void OnClickClose()
	{
		Log.Debug("[SteamLink][CodeInput] OnClickClose", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountCodeInput.cs", 139);
		NKCAccountLinkMgr.CheckForCancelProcess();
	}

	public void TrySendPublisherCode()
	{
		string text = "";
		if (m_inputField != null)
		{
			text = m_inputField.text;
		}
		if (!string.IsNullOrEmpty(text))
		{
			NKCAccountLinkMgr.Send_NKMPacket_BSIDE_ACCOUNT_LINK_REQ(text);
		}
	}

	public void TrySendPrivateLinkCode()
	{
		string text = "";
		if (m_inputField != null)
		{
			text = m_inputField.text;
		}
		if (!string.IsNullOrEmpty(text))
		{
			NKCAccountLinkMgr.Send_NKMPacket_BSIDE_ACCOUNT_LINK_CODE_REQ(text);
		}
	}
}
