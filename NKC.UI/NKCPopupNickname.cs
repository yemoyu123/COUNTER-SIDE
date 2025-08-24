using Cs.Logging;
using NKC.PacketHandler;
using NKM;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupNickname : NKCUIBase
{
	public delegate void OnButton();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_NICKNAME_BOX";

	private static NKCPopupNickname m_Instance;

	public InputField m_InputField;

	public NKCUIComStateButton m_btnOK;

	public NKCUIComStateButton m_btnCancel;

	public Text m_textDesc1;

	public Text m_textDesc2;

	private OnButton m_dOnClose;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public static NKCPopupNickname Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupNickname>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_NICKNAME_BOX", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupNickname>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "Nickname";

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

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

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		m_btnOK?.PointerClick.RemoveAllListeners();
		m_btnOK?.PointerClick.AddListener(OnOK);
		m_btnCancel?.PointerClick.RemoveAllListeners();
		m_btnCancel?.PointerClick.AddListener(base.Close);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.NICKNAME_LIMIT_ENG))
		{
			m_InputField.contentType = InputField.ContentType.Alphanumeric;
			NKCUtil.SetLabelText(m_textDesc1, NKCStringTable.GetString("SI_PF_NICKNAME_CHANGE_DESC_1_GLOBAL"));
		}
		m_InputField.onValueChanged.AddListener(OnNickNameValueChanged);
		m_InputField.onEndEdit.RemoveAllListeners();
		m_InputField.onEndEdit.AddListener(OnEndEditNickName);
		NKCUtil.SetHotkey(m_btnOK, HotkeyEventType.Confirm);
	}

	public void Open(OnButton onClose = null)
	{
		base.gameObject.SetActive(value: true);
		m_InputField.text = "";
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_dOnClose = onClose;
		UIOpened();
	}

	public void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public void OnOK()
	{
		if (!NKM_USER_COMMON.CheckNickName(m_InputField.text))
		{
			if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.NICKNAME_LIMIT_ENG))
			{
				NKCPacketHandlers.Check_NKM_ERROR_CODE(NKM_ERROR_CODE.NEC_FAIL_ACCOUNT_INVALID_NICKNAME_LENGTH_GLOBAL);
			}
			else
			{
				NKCPacketHandlers.Check_NKM_ERROR_CODE(NKM_ERROR_CODE.NEC_FAIL_ACCOUNT_INVALID_NICKNAME_LENGTH);
			}
			return;
		}
		if (!NKCFilterManager.CheckNickNameFilter(m_InputField.text))
		{
			NKCPacketHandlers.Check_NKM_ERROR_CODE(NKM_ERROR_CODE.NEC_FAIL_ACCOUNT_INVALID_NICKNAME_FILTER);
			return;
		}
		for (int i = 0; i < m_InputField.text.Length; i++)
		{
			if (char.IsWhiteSpace(m_InputField.text[i]))
			{
				NKCPacketHandlers.Check_NKM_ERROR_CODE(NKM_ERROR_CODE.NEC_FAIL_ACCOUNT_INVALID_NICKNAME_FILTER);
				return;
			}
		}
		if (string.Equals(NKCScenManager.CurrentUserData().m_UserNickName, m_InputField.text))
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCPacketHandlers.GetErrorMessage(NKM_ERROR_CODE.NEC_FAIL_ACCOUNT_INVALID_NICKNAME_SAME), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(510);
			_ = $"1/{countMiscItem}";
			string itemName = NKMItemManager.GetItemMiscTempletByID(510).GetItemName();
			NKCPopupResourceConfirmBox.Instance.Open(m_InputField.text, string.Format(NKCUtilString.GET_STRING_NICKNAME_CHANGE_RECHECK_ONE_PARAM, itemName), 510, 1, delegate
			{
				OnChangeNickname(m_InputField.text);
			});
		}
	}

	private void OnChangeNickname(string nickname)
	{
		NKMPopUpBox.OpenWaitBox();
		NKCPacketSender.Send_NKMPacket_CHANGE_NICKNAME_REQ(nickname);
	}

	private void OnNickNameValueChanged(string input)
	{
		Log.Debug("NickName : " + input, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupNickname.cs", 167);
	}

	private void OnEndEditNickName(string input)
	{
		if (NKCInputManager.IsChatSubmitEnter())
		{
			if (!m_btnOK.m_bLock)
			{
				OnOK();
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	public void OnClose()
	{
		Close();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		m_dOnClose?.Invoke();
	}
}
