namespace NKC.UI.Warfare;

public class NKCPopupWarfareInfo : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_warfare";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_WARFARE_INFOPOPUP";

	private static NKCPopupWarfareInfo m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComStateButton m_BtnClose;

	public static NKCPopupWarfareInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupWarfareInfo>("ab_ui_nkm_ui_warfare", "NKM_UI_POPUP_WARFARE_INFOPOPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupWarfareInfo>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "WarfareInfo";

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
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open()
	{
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
		Close();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
