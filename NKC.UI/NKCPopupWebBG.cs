using Cs.Logging;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupWebBG : NKCUIBase
{
	public delegate void OnCloseWeb();

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_WEB_BG";

	private static NKCPopupWebBG m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public static int m_topBarHeight = 100;

	public NKCUIComStateButton m_BtnClose;

	public NKCUIComStateButton m_BtnBG;

	public GameObject m_TopBarObject;

	public GameObject m_WebViewAreaObject;

	private OnCloseWeb m_dOnCloseWeb;

	public static NKCPopupWebBG Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupWebBG>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_WEB_BG", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupWebBG>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupWebBG";

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
		m_BtnClose.PointerClick.AddListener(OnClickClose);
		m_BtnBG.PointerClick.RemoveAllListeners();
		m_BtnBG.PointerClick.AddListener(OnClickClose);
		NKCUtil.SetGameobjectActive(m_WebViewAreaObject, bValue: false);
		if (NKCDefineManager.DEFINE_UNITY_EDITOR())
		{
			NKCUtil.SetGameobjectActive(m_WebViewAreaObject, bValue: true);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void OnClickClose()
	{
		Log.Debug("PopupWebBG - OnClickClose", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupWebBG.cs", 75);
		Close();
	}

	public void Open(int marginX, int marginYTop, int marginYBottom, int nxpWebwidth, int nxpWebheight, OnCloseWeb dOnCloseWeb)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_dOnCloseWeb = dOnCloseWeb;
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		Log.Debug($"PopupWebBG - screenSize[{Screen.width}, {Screen.height}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupWebBG.cs", 90);
		Log.Debug($"PopupWebBG - safescreenSize[{Screen.safeArea.width}, {Screen.safeArea.height}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupWebBG.cs", 91);
		Log.Debug($"PopupWebBG - stretchedscreenSize[{component.GetWidth()}, {component.GetHeight()}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupWebBG.cs", 92);
		Log.Debug($"PopupWebBG - WebSize[{nxpWebwidth}, {nxpWebheight}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupWebBG.cs", 93);
		Log.Debug($"PopupWebBG - marginX[{marginX}], marginYTop[{marginYTop}], marginYBottom[{marginYBottom}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupWebBG.cs", 94);
		float num = component.GetHeight() / (float)Screen.height;
		if (m_WebViewAreaObject != null)
		{
			RectTransform component2 = m_WebViewAreaObject.GetComponent<RectTransform>();
			component2.offsetMin = new Vector2(marginX, marginYBottom);
			component2.offsetMax = new Vector2(-marginX, -marginYTop);
		}
		if (m_TopBarObject != null)
		{
			RectTransform component3 = m_TopBarObject.GetComponent<RectTransform>();
			component3.offsetMin = new Vector2(marginX, component3.offsetMin.y);
			component3.offsetMax = new Vector2(-marginX, component3.offsetMax.y);
			component3.SetHeight(m_topBarHeight);
			component3.anchoredPosition = new Vector2(component3.anchoredPosition.x, 0f - (float)marginYTop * num);
		}
		UIOpened();
	}

	public static void CalculateMarginSizeForNXPWeb(out int marginX, out int marginYTop, out int marginYBottom, out int webWidth, out int webHeight)
	{
		int height = Screen.height;
		int num = 177;
		int num2 = 10;
		webHeight = height - m_topBarHeight - num2 * 2;
		int num3 = webHeight * num / 100;
		if (Screen.width < num3)
		{
			webHeight = Screen.width / num * 100;
			num3 = Screen.width;
		}
		webWidth = num3;
		int num4 = (Screen.height - m_topBarHeight - webHeight) / 2;
		marginX = (Screen.width - webWidth) / 2;
		marginYTop = m_topBarHeight + num4;
		marginYBottom = num4;
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
		Log.Debug("PopupWebBG - CloseInternal", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupWebBG.cs", 164);
		m_dOnCloseWeb?.Invoke();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
