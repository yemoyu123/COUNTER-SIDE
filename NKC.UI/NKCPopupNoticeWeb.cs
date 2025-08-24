using NKC.Publisher;
using UnityEngine;
using UnityEngine.EventSystems;
using ZenFulcrum.EmbeddedBrowser;

namespace NKC.UI;

public class NKCPopupNoticeWeb : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_WEB_NOTICE";

	private static NKCPopupNoticeWeb m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComStateButton m_BtnClose;

	public NKCUIComStateButton m_csbtnHome;

	public EventTrigger m_etBG;

	public GameObject m_objWebView;

	private string m_HomeUrl = "";

	private NKCPublisherModule.OnComplete dOnWindowClosed;

	private Browser m_Browser;

	public static NKCPopupNoticeWeb Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupNoticeWeb>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_WEB_NOTICE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupNoticeWeb>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupWebNotice";

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

	private void SetUrl(string url)
	{
		if (m_Browser == null)
		{
			m_Browser = m_objWebView.AddComponent<Browser>();
			m_Browser.Resize(1312, 656);
			m_objWebView.AddComponent<PointerUIGUI>();
			m_objWebView.AddComponent<CursorRendererOS>();
		}
		if (!(m_Browser == null))
		{
			m_Browser.Url = url;
			m_HomeUrl = url;
		}
	}

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		m_BtnClose.PointerClick.RemoveAllListeners();
		m_BtnClose.PointerClick.AddListener(base.Close);
		m_csbtnHome.PointerClick.RemoveAllListeners();
		m_csbtnHome.PointerClick.AddListener(OnClickHome);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_etBG.triggers.Add(entry);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickHome()
	{
		SetUrl(m_HomeUrl);
	}

	public void Open(string url, NKCPublisherModule.OnComplete onWindowClosed, bool bPatcher = false)
	{
		if (bPatcher)
		{
			InitUI();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetUrl(url);
		m_NKCUIOpenAnimator.PlayOpenAni();
		dOnWindowClosed = onWindowClosed;
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
		dOnWindowClosed?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		(NKCScenManager.GetScenManager()?.GetComponent<NKCCursor>())?.SetMouseCursor();
	}
}
