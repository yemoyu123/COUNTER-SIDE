using Cs.Logging;
using NKC.Publisher;

namespace NKC.UI;

public class NKCPopupSelectLoginType : NKCUIBase
{
	public NKCUIComStateButton m_Facebook;

	public NKCUIComStateButton m_Google;

	public NKCUIComStateButton m_Twitter;

	public NKCUIComStateButton m_Apple;

	public NKCUIComStateButton m_Guest;

	public NKCUIComStateButton m_Close;

	public NKCUIComStateButton m_Back;

	private const string m_assetBundleName = "AB_UI_LOGIN_SELECT";

	private const string m_prefabName = "AB_UI_LOGIN_SOCIAL_POPUP";

	private NKCPublisherModule.OnComplete m_onComplete;

	private bool m_isAddMappingState;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	private static NKCPopupSelectLoginType _instance;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "Select Login Type";

	public static NKCPopupSelectLoginType Instance
	{
		get
		{
			if (_instance == null)
			{
				m_loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupSelectLoginType>("AB_UI_LOGIN_SELECT", "AB_UI_LOGIN_SOCIAL_POPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance);
				if (m_loadedUIData != null)
				{
					_instance = m_loadedUIData.GetInstance<NKCPopupSelectLoginType>();
				}
				_instance.InitUI();
				NKCUtil.SetGameobjectActive(_instance.gameObject, bValue: false);
			}
			return _instance;
		}
	}

	public override void OnBackButton()
	{
		OnClickBack();
	}

	public static void CheckInstanceAndClose()
	{
		m_loadedUIData?.CloseInstance();
		m_loadedUIData = null;
	}

	private static void CleanupInstance()
	{
		_instance = null;
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void InitUI()
	{
		m_Facebook?.PointerClick.RemoveAllListeners();
		m_Facebook?.PointerClick.AddListener(OnClickFacebookLogin);
		m_Google?.PointerClick.RemoveAllListeners();
		m_Google?.PointerClick.AddListener(OnClickGoogleLogin);
		m_Twitter?.PointerClick.RemoveAllListeners();
		m_Twitter?.PointerClick.AddListener(OnClickTwitterLogin);
		m_Guest?.PointerClick.RemoveAllListeners();
		m_Guest?.PointerClick.AddListener(OnClickGuestLogin);
		m_Apple?.PointerClick.RemoveAllListeners();
		m_Apple?.PointerClick.AddListener(OnClickAppleLogin);
		m_Close?.PointerClick.RemoveAllListeners();
		m_Close?.PointerClick.AddListener(ClosePopup);
		m_Back?.PointerClick.RemoveAllListeners();
		m_Back?.PointerClick.AddListener(OnClickBack);
	}

	public void Open(NKCPublisherModule.OnComplete dOnComplete, bool isAddMappingState, bool hideTwitterButton)
	{
		if (m_bOpen)
		{
			Close();
		}
		Log.Debug($"[GBLogin] PopupSelectLoginType mapping[{isAddMappingState}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupSelectLoginType.cs", 99);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
		m_onComplete = dOnComplete;
		m_isAddMappingState = isAddMappingState;
		NKCUtil.SetGameobjectActive(m_Guest.gameObject, !m_isAddMappingState);
		if (m_Twitter != null)
		{
			NKCUtil.SetGameobjectActive(m_Twitter.gameObject, !hideTwitterButton);
		}
		UpdateUI();
	}

	public static bool isOpen()
	{
		if (Instance != null)
		{
			return Instance.IsOpen;
		}
		return false;
	}

	public void ClosePopup()
	{
		if (!(_instance == null))
		{
			if (isOpen())
			{
				Instance.Close();
			}
			CheckInstanceAndClose();
		}
	}

	public void OnClickBack()
	{
		ClosePopup();
	}

	public void UpdateUI()
	{
	}

	public void OnClickGuestLogin()
	{
		DoRequest("GUEST");
	}

	public void OnClickGoogleLogin()
	{
		DoRequest("GOOGLE");
	}

	public void OnClickTwitterLogin()
	{
		DoRequest("TWITTER");
	}

	public void OnClickFacebookLogin()
	{
		DoRequest("FACEBOOK");
	}

	public void OnClickAppleLogin()
	{
		DoRequest("APPLEID");
	}

	private void DoRequest(string providerName)
	{
		ClosePopup();
		if (m_isAddMappingState)
		{
			NKCPublisherModule.Auth.AddMapping(providerName, m_onComplete);
		}
		else
		{
			NKCPublisherModule.Auth.LoginToPublisherBy(providerName, m_onComplete);
		}
	}
}
