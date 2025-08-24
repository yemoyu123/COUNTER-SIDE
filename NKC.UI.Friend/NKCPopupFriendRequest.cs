using ClientPacket.Community;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI.Friend;

public class NKCPopupFriendRequest : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FRIEND_REQUEST";

	private static NKCPopupFriendRequest m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUISlot m_slot;

	public Text m_txtLevel;

	public Text m_txtName;

	public Text m_txtFCode;

	public Text m_txtLastConnectTime;

	public Text m_txtPower;

	public Text m_txtDesc;

	[Header("Button")]
	public NKCUIComStateButton m_btnInfo;

	public NKCUIComButton m_btnOk;

	public NKCUIComButton m_btnCancel;

	private UnityAction dOnClose;

	private long m_guestFriendCode;

	public static NKCPopupFriendRequest Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFriendRequest>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_FRIEND_REQUEST", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFriendRequest>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "친구 요청";

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

	private void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		m_slot.Init();
		m_btnInfo.PointerClick.RemoveAllListeners();
		m_btnInfo.PointerClick.AddListener(OnFriendInfo);
		m_btnOk.PointerClick.RemoveAllListeners();
		m_btnOk.PointerClick.AddListener(OnOk);
		NKCUtil.SetHotkey(m_btnOk, HotkeyEventType.Confirm);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(OnCancel);
	}

	public void Open(WarfareSupporterListData data, UnityAction onClose)
	{
		if (data == null)
		{
			Debug.LogError("WarfareSupporterListData is null");
			return;
		}
		dOnClose = onClose;
		m_guestFriendCode = data.commonProfile.friendCode;
		m_slot.SetUnitData(data.commonProfile.mainUnitId, 0, data.commonProfile.mainUnitSkinId, bShowName: false, bShowLevel: false, bEnableLayoutElement: false, null);
		m_slot.SetMaxLevelTacticFX(data.commonProfile.mainUnitTacticLevel == 6);
		NKCUtil.SetLabelText(m_txtLevel, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, data.commonProfile.level);
		NKCUtil.SetLabelText(m_txtName, data.commonProfile.nickname);
		NKCUtil.SetLabelText(m_txtFCode, NKCUtilString.GetFriendCode(data.commonProfile.friendCode));
		NKCUtil.SetLabelText(m_txtLastConnectTime, NKCUtilString.GetLastTimeString(data.lastLoginDate));
		NKCUtil.SetLabelText(m_txtDesc, data.message);
		NKCUtil.SetLabelText(m_txtPower, data.deckData.CalculateOperationPower().ToString());
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

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (dOnClose != null)
		{
			dOnClose();
			dOnClose = null;
		}
	}

	private void OnFriendInfo()
	{
		NKCPacketSender.Send_NKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ(m_guestFriendCode);
	}

	private void OnOk()
	{
		NKCPacketSender.Send_NKMPacket_FRIEND_REQUEST_REQ(m_guestFriendCode);
		Close();
	}

	private void OnCancel()
	{
		Close();
	}
}
