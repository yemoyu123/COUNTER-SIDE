using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupCoupon : NKCUIBase
{
	public delegate void OnClickOK(string code);

	private OnClickOK m_OnClickOK;

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_COUPON_BOX";

	private static NKCPopupCoupon m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComStateButton m_BtnClose;

	public NKCUIComStateButton m_BtnOk;

	public EventTrigger m_etBG;

	public InputField m_IFCouponCode;

	public static NKCPopupCoupon Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupCoupon>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_COUPON_BOX", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupCoupon>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupCoupon";

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
		m_BtnClose.PointerClick.RemoveAllListeners();
		m_BtnClose.PointerClick.AddListener(base.Close);
		m_BtnOk.PointerClick.RemoveAllListeners();
		m_BtnOk.PointerClick.AddListener(OK);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_etBG.triggers.Add(entry);
		m_IFCouponCode.onEndEdit.RemoveAllListeners();
		m_IFCouponCode.onEndEdit.AddListener(OnEndEditCoupon);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(OnClickOK onClickOK)
	{
		m_OnClickOK = onClickOK;
		m_IFCouponCode.text = "";
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
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

	public void OK()
	{
		if (m_OnClickOK != null)
		{
			m_OnClickOK(m_IFCouponCode.text);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnEndEditCoupon(string input)
	{
		if (NKCInputManager.IsChatSubmitEnter())
		{
			if (!m_BtnOk.m_bLock)
			{
				OK();
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}
