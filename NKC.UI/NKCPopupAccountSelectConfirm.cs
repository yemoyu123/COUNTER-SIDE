using ClientPacket.Account;
using Cs.Logging;
using UnityEngine.Events;

namespace NKC.UI;

public class NKCPopupAccountSelectConfirm : NKCUIBase
{
	private const string DEBUG_HEADER = "[SteamLink][SelectConfirm]";

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_ACCOUNT_LINK";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_ACCOUNT_SELECT_CONFIRM";

	private static NKCPopupAccountSelectConfirm m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCPopupAccountSelectSlot m_selectedSlot;

	public NKCUIComStateButton m_ok;

	public NKCUIComStateButton m_cancel;

	public static NKCPopupAccountSelectConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupAccountSelectConfirm>("AB_UI_NKM_UI_ACCOUNT_LINK", "NKM_UI_POPUP_ACCOUNT_SELECT_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupAccountSelectConfirm>();
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

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_selectedSlot.InitData();
		m_cancel?.PointerClick.RemoveAllListeners();
		m_cancel?.PointerClick.AddListener(OnClickClose);
	}

	public void OnClickClose()
	{
		Log.Debug("[SteamLink][SelectConfirm] OnClickClose", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountSelectConfirm.cs", 67);
		Close();
	}

	public void Open(NKMAccountLinkUserProfile selectedUserProfile, UnityAction onClickConfirm)
	{
		Log.Debug("[SteamLink][SelectConfirm] Open", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountSelectConfirm.cs", 73);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_selectedSlot?.SetData(selectedUserProfile, null);
		m_ok?.PointerClick.RemoveAllListeners();
		m_ok?.PointerClick.AddListener(onClickConfirm);
		UIOpened();
	}

	public void OpenSuccess(NKMAccountLinkUserProfile selectedUserProfile, UnityAction onClickConfirm)
	{
		Log.Debug("[SteamLink][SelectConfirm] OpenSuccess", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountSelectConfirm.cs", 87);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_selectedSlot?.SetData(selectedUserProfile, null);
		m_ok?.PointerClick.RemoveAllListeners();
		m_ok?.PointerClick.AddListener(onClickConfirm);
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
		Log.Debug("[SteamLink][SelectConfirm] CloseInternal", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountSelectConfirm.cs", 109);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
