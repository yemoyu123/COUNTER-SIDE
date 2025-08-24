using ClientPacket.Account;
using Cs.Logging;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupServiceTransferUserData : NKCUIBase
{
	private const string DEBUG_HEADER = "[ServiceTransfer][UserData]";

	private const string ASSET_BUNDLE_NAME = "AB_UI_SERVICE_TRANSFER";

	private const string UI_ASSET_NAME = "AB_UI_SERVICE_TRANSFER_INFO_CONFIRM";

	private static NKCPopupServiceTransferUserData m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComStateButton m_Ok;

	public NKCUIComStateButton m_Cancel;

	public Text m_lbCreditCount;

	public Text m_lbEterniumCount;

	public Text m_lbCashCount;

	public Text m_lbMedalCount;

	public Text m_lbNickName;

	public Text m_lbLevel;

	public Text m_lbFriendCode;

	public static NKCPopupServiceTransferUserData Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupServiceTransferUserData>("AB_UI_SERVICE_TRANSFER", "AB_UI_SERVICE_TRANSFER_INFO_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupServiceTransferUserData>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "ServiceTransfer";

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
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(NKMAccountLinkUserProfile userProfile)
	{
		Log.Debug("[ServiceTransfer][UserData] Open", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCPopupServiceTransferUserData.cs", 69);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_Ok?.PointerClick.RemoveAllListeners();
		m_Cancel?.PointerClick.RemoveAllListeners();
		m_Ok?.PointerClick.AddListener(OnClickSendConfirm);
		m_Cancel?.PointerClick.AddListener(NKCServiceTransferMgr.CancelServiceTransferProcess);
		NKCUtil.SetLabelText(m_lbCreditCount, userProfile.creditCount.ToString());
		NKCUtil.SetLabelText(m_lbEterniumCount, userProfile.eterniumCount.ToString());
		NKCUtil.SetLabelText(m_lbCashCount, userProfile.cashCount.ToString());
		NKCUtil.SetLabelText(m_lbMedalCount, userProfile.medalCount.ToString());
		NKCUtil.SetLabelText(m_lbNickName, userProfile.commonProfile.nickname.ToString());
		NKCUtil.SetLabelText(m_lbLevel, "Lv." + userProfile.commonProfile.level);
		NKCUtil.SetLabelText(m_lbFriendCode, NKCUtilString.GetFriendCode(userProfile.commonProfile.friendCode));
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
		Log.Debug("[ServiceTransfer][UserData] CloseInternal", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCPopupServiceTransferUserData.cs", 100);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void OnClickSendConfirm()
	{
		NKCServiceTransferMgr.Send_NKMPacket_SERVICE_TRANSFER_CONFIRM_REQ();
	}
}
