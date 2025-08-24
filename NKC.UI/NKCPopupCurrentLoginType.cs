using System.Collections.Generic;
using Cs.Logging;
using NKC.PacketHandler;
using NKC.Publisher;

namespace NKC.UI;

public class NKCPopupCurrentLoginType : NKCUIBase
{
	public NKCUIComStateButton m_Facebook;

	public NKCUIComStateButton m_Google;

	public NKCUIComStateButton m_Twitter;

	public NKCUIComStateButton m_Apple;

	public NKCUIComStateButton m_Guest;

	public NKCUIComStateButton m_Logout;

	public NKCUIComStateButton m_Close;

	public NKCUIComStateButton m_Back;

	public NKCUIComStateButton m_Withdraw;

	private const string m_assetBundleName = "AB_UI_LOGIN_SELECT";

	private const string m_prefabName = "AB_UI_LOGIN_SELECT_CURRENT";

	private NKCPublisherModule.OnComplete m_onComplete;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	private static NKCPopupCurrentLoginType _instance;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "Current Login Type";

	public static NKCPopupCurrentLoginType Instance
	{
		get
		{
			if (_instance == null)
			{
				m_loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupCurrentLoginType>("AB_UI_LOGIN_SELECT", "AB_UI_LOGIN_SELECT_CURRENT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance);
				if (m_loadedUIData != null)
				{
					_instance = m_loadedUIData.GetInstance<NKCPopupCurrentLoginType>();
				}
				_instance.InitUI();
				NKCUtil.SetGameobjectActive(_instance.gameObject, bValue: false);
			}
			return _instance;
		}
	}

	public override void OnBackButton()
	{
		ClosePopup();
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
		m_Facebook?.Lock();
		m_Google?.Lock();
		m_Twitter?.Lock();
		m_Guest?.Lock();
		m_Apple?.Lock();
		NKCUtil.SetGameobjectActive(m_Facebook, bValue: false);
		NKCUtil.SetGameobjectActive(m_Google, bValue: false);
		NKCUtil.SetGameobjectActive(m_Twitter, bValue: false);
		NKCUtil.SetGameobjectActive(m_Guest, bValue: false);
		NKCUtil.SetGameobjectActive(m_Apple, bValue: false);
		m_Logout?.PointerClick.RemoveAllListeners();
		m_Logout?.PointerClick.AddListener(OnClickLogout);
		m_Close?.PointerClick.RemoveAllListeners();
		m_Close?.PointerClick.AddListener(ClosePopup);
		m_Back?.PointerClick.RemoveAllListeners();
		m_Back?.PointerClick.AddListener(OnClickBack);
		NKCUtil.SetGameobjectActive(m_Withdraw, NKCDefineManager.DEFINE_SELECT_SERVER());
		m_Withdraw?.PointerClick.RemoveAllListeners();
		m_Withdraw?.PointerClick.AddListener(OnClickWithdraw);
	}

	public void Open(NKCPublisherModule.OnComplete dOnComplete, string providerName)
	{
		if (m_bOpen)
		{
			Close();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
		m_onComplete = dOnComplete;
		UpdateProviderBanner(providerName);
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

	public void OnClickLogout()
	{
		NKCPublisherModule.Auth.Logout(NKCPacketHandlersLobby.OnLogoutComplete);
		ClosePopup();
	}

	private void OnClickWithdraw()
	{
		if (NKCPublisherModule.Auth.IsGuest())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_OPTION_DROPOUT_WARNING_INSTANT, delegate
			{
				NKCUIAccountWithdrawCheckPopup.Instance.OpenUI();
			});
		}
		else
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_OPTION_DROPOUT_WARNING, delegate
			{
				NKCUIAccountWithdrawCheckPopup.Instance.OpenUI();
			});
		}
	}

	public void UpdateProviderBanner(string providerName)
	{
		NKCUtil.SetGameobjectActive(m_Facebook, providerName.ToLower().Equals("facebook"));
		NKCUtil.SetGameobjectActive(m_Google, providerName.ToLower().Equals("google"));
		NKCUtil.SetGameobjectActive(m_Twitter, providerName.ToLower().Equals("twitter"));
		NKCUtil.SetGameobjectActive(m_Guest, providerName.ToLower().Equals("guest"));
		NKCUtil.SetGameobjectActive(m_Apple, providerName.ToLower().Equals("appleid"));
	}

	public void UpdateProviderProfile(Dictionary<string, object> providerProfileInformation)
	{
		foreach (KeyValuePair<string, object> item in providerProfileInformation)
		{
			Log.Debug("[GameBase] authproviderInfo [" + item.Key + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Popup/NKCPopupCurrentLoginType.cs", 183);
		}
	}
}
