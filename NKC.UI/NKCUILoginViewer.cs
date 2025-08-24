using System;
using System.Collections.Generic;
using Cs.Logging;
using NKC.Localization;
using NKC.Patcher;
using NKC.Publisher;
using NKC.Templet;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILoginViewer : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_login_loc";

	private const string UI_ASSET_NAME = "NUF_LOGIN_PREFAB";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public GameObject m_NUF_LOGIN_DEV_SERVER_LIST;

	public GameObject m_NUF_LOGIN_DEV_LOGIN_SIGN_BUTTON;

	private Dropdown m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN;

	public NKCUIComButton m_comBtnServer1STSave;

	public NKCUIComButton m_comBtnServer1STLoad;

	public NKCUIComButton m_comBtnServer2NDSave;

	public NKCUIComButton m_comBtnServer2NDLoad;

	public NKCUIComButton m_comBtnServer3RDSave;

	public NKCUIComButton m_comBtnServer3RDLoad;

	public InputField m_IFServerAddress;

	public InputField m_IFID;

	public InputField m_IFPassword;

	public NKCUIComButton m_comBtnLogin;

	public NKCUIComButton m_comBtnChangeAccount;

	public NKCUIComButton m_comBtnPlaySingle;

	public NKCUIComButton m_comBtnCutscen;

	public NKCUIComButton m_comBtnVoiceList;

	public NKCUIComButton m_comBtnLanguageSelect;

	public NKCUIComButton m_comBtnMoveToPatch;

	public NKCUIComButton m_comBtnPatchSkipTest;

	public GameObject m_NUF_LOGIN_SCREEN;

	public GameObject m_objBsideLogo;

	public GameObject m_objNexonLogo;

	public GameObject m_objZlongLogo;

	public NKCUIComStateButton m_csbtnPcQR;

	public GameObject m_objMessage;

	private NKCUILoginViewerMsg m_NKCUILoginViewerMsg;

	private NKCAssetInstanceData m_NKCAssetInstanceDataMessage;

	public const string NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING = "NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING";

	public const string NKM_LOCAL_SAVE_KEY_LOGIN_PASSWORD_STRING = "NKM_LOCAL_SAVE_KEY_LOGIN_PASSWORD_STRING";

	private string NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED = "NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED";

	private string NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_1ST = "NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_1ST";

	private string NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_2ND = "NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_2ND";

	private string NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_3RD = "NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_3RD";

	public NKCUIComStateButton NUF_LOGIN_NOTICE;

	public GameObject m_objNUF_LOGIN_DEV_LOGIN;

	public Text m_lbNUF_LOGIN_UPDATE_Text;

	[Obsolete]
	public GameObject m_NUF_LOGIN_DEV_SINGLE;

	public GameObject m_NUF_LOGIN_DEV_CUTSCEN;

	public GameObject m_NUF_LOGIN_DEV_VOICE_LIST;

	public NKCUIComStateButton m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL;

	public Text m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL_TEXT;

	private NKM_USER_AUTH_LEVEL m_NKM_USER_AUTH_LEVEL = NKM_USER_AUTH_LEVEL.NORMAL_USER;

	public GameObject m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON;

	public Image m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON_BG;

	public Text m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON_TEXT;

	public Text m_lbVersion;

	private float m_fUpdateTime;

	private const float LOGIN_PANEL_SHOW_UP_TIME = 2.5f;

	private bool m_bShowUpLoginPanel;

	private const float LOGIN_TOUCH_DELAY_TIME = 60f;

	private float m_loginTouchDelay;

	private NKCAssetResourceData m_BackResource;

	private GameObject m_objBack;

	private List<NKCAssetInstanceData> m_instancedPrefabList = new List<NKCAssetInstanceData>();

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public static bool IsInstanceOpen
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsUIOpen;
			}
			return false;
		}
	}

	public static bool IsInstanceLoaded
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsLoadComplete;
			}
			return false;
		}
	}

	public override string MenuName => NKCUtilString.GET_STRING_LOGIN;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public NKM_USER_AUTH_LEVEL AuthLevel => m_NKM_USER_AUTH_LEVEL;

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUILoginViewer>("ab_ui_login_loc", "NUF_LOGIN_PREFAB", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUILoginViewer GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUILoginViewer>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public override void OnBackButton()
	{
		OpenQuitApplicationPopup();
	}

	public void UpdateLoginMsgUI()
	{
		if (m_NKCUILoginViewerMsg != null)
		{
			m_NKCUILoginViewerMsg.ForceUpdateMsg();
		}
	}

	public NKCUILoginViewer GetLoginDevMenu()
	{
		return this;
	}

	private void OnDestroy()
	{
		if (m_NKCAssetInstanceDataMessage != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataMessage);
		}
		m_NKCAssetInstanceDataMessage = null;
		m_NKCUILoginViewerMsg = null;
	}

	private void LoadBackgroundTemplet()
	{
		if (m_objBack != null)
		{
			return;
		}
		if (m_BackResource == null)
		{
			NKCLoginBackgroundTemplet currentBackgroundTemplet = NKCLoginBackgroundTemplet.GetCurrentBackgroundTemplet();
			if (currentBackgroundTemplet != null && NKCAssetResourceManager.IsBundleExists(currentBackgroundTemplet.BundleName, currentBackgroundTemplet.AssetName))
			{
				m_BackResource = NKCAssetResourceManager.OpenResource<GameObject>(currentBackgroundTemplet.BundleName, currentBackgroundTemplet.AssetName);
			}
			if (m_BackResource == null || m_BackResource.GetAsset<GameObject>() == null)
			{
				if (m_BackResource != null)
				{
					NKCAssetResourceManager.CloseResource(m_BackResource);
				}
				m_BackResource = NKCAssetResourceManager.OpenResource<GameObject>("AB_LOGIN_SCREEN_NKM_LS_202001_OPEN", "AB_LOGIN_SCREEN_NKM_LS_202001_OPEN");
			}
		}
		m_objBack = UnityEngine.Object.Instantiate(m_BackResource.GetAsset<GameObject>(), NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIMidCanvas));
		m_objBack.transform.localPosition = Vector3.zero;
	}

	private void LoadPrefabList()
	{
		ReleaseEnabledPrefabs();
		foreach (NKCLoginBackgroundTemplet enabledPrefab in NKCLoginBackgroundTemplet.GetEnabledPrefabList())
		{
			NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(enabledPrefab.BundleName, enabledPrefab.AssetName, bAsync: false, base.transform);
			if (nKCAssetInstanceData == null)
			{
				Log.Error($"[NKCUILoginViewer] LoadPrefab failed  id[{enabledPrefab.ID}] asset[{enabledPrefab.AssetName}/{enabledPrefab.BundleName}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginViewer.cs", 233);
			}
			else
			{
				m_instancedPrefabList.Add(nKCAssetInstanceData);
			}
		}
	}

	private void ReleaseEnabledPrefabs()
	{
		foreach (NKCAssetInstanceData instancedPrefab in m_instancedPrefabList)
		{
			instancedPrefab.Close();
		}
		m_instancedPrefabList.Clear();
	}

	private void RescaleBack()
	{
		if (!(m_objBack == null))
		{
			RectTransform component = m_objBack.GetComponent<RectTransform>();
			if (component != null)
			{
				NKCCamera.RescaleRectToCameraFrustrum(component, NKCCamera.GetCamera(), Vector2.zero, NKCScenManager.GetScenManager().Get_SCEN_LOGIN().GetCameraDistance());
			}
		}
	}

	public void InitUI()
	{
		if (m_comBtnServer1STSave != null)
		{
			m_comBtnServer1STSave.PointerClick.RemoveAllListeners();
			m_comBtnServer1STSave.PointerClick.AddListener(OnSaveServer1ST);
		}
		if (m_comBtnServer1STLoad != null)
		{
			m_comBtnServer1STLoad.PointerClick.RemoveAllListeners();
			m_comBtnServer1STLoad.PointerClick.AddListener(OnLoadServer1ST);
		}
		if (m_comBtnServer2NDSave != null)
		{
			m_comBtnServer2NDSave.PointerClick.RemoveAllListeners();
			m_comBtnServer2NDSave.PointerClick.AddListener(OnSaveServer2ND);
		}
		if (m_comBtnServer2NDLoad != null)
		{
			m_comBtnServer2NDLoad.PointerClick.RemoveAllListeners();
			m_comBtnServer2NDLoad.PointerClick.AddListener(OnLoadServer2ND);
		}
		if (m_comBtnServer3RDSave != null)
		{
			m_comBtnServer3RDSave.PointerClick.RemoveAllListeners();
			m_comBtnServer3RDSave.PointerClick.AddListener(OnSaveServer3RD);
		}
		if (m_comBtnServer3RDLoad != null)
		{
			m_comBtnServer3RDLoad.PointerClick.RemoveAllListeners();
			m_comBtnServer3RDLoad.PointerClick.AddListener(OnLoadServer3RD);
		}
		if (m_comBtnLogin != null)
		{
			m_comBtnLogin.PointerClick.RemoveAllListeners();
			m_comBtnLogin.PointerClick.AddListener(OnLogin);
		}
		if (NUF_LOGIN_NOTICE != null)
		{
			if (NKCPublisherModule.IsZlongPublished())
			{
				NKCUtil.SetGameobjectActive(NUF_LOGIN_NOTICE.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(NUF_LOGIN_NOTICE.gameObject, bValue: true);
				NUF_LOGIN_NOTICE.PointerClick.RemoveAllListeners();
				NUF_LOGIN_NOTICE.PointerClick.AddListener(OnClickNoticeButton);
			}
			else
			{
				NKCUtil.SetGameobjectActive(NUF_LOGIN_NOTICE.gameObject, bValue: false);
			}
		}
		if (NKCPublisherModule.IsPublisherNoneType())
		{
			if (m_comBtnChangeAccount != null)
			{
				m_comBtnChangeAccount.PointerClick.RemoveAllListeners();
				m_comBtnChangeAccount.PointerClick.AddListener(OnChangeAccountDev);
			}
		}
		else
		{
			NKCUtil.SetImageSprite(m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON_BG, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			NKCUtil.SetLabelText(m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON_TEXT, NKCUtilString.GET_STRING_TOY_LOGIN_NO_AUTH);
			NKCUtil.SetLabelTextColor(m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON_TEXT, Color.black);
		}
		NKCUtil.SetButtonClickDelegate(m_comBtnPlaySingle, OnPlaySingle);
		if (m_comBtnCutscen != null)
		{
			m_comBtnCutscen.PointerClick.RemoveAllListeners();
			m_comBtnCutscen.PointerClick.AddListener(OnCutScenSim);
		}
		if (m_comBtnVoiceList != null)
		{
			m_comBtnVoiceList.PointerClick.RemoveAllListeners();
			m_comBtnVoiceList.PointerClick.AddListener(OnVoiceList);
		}
		if (m_comBtnLanguageSelect != null)
		{
			m_comBtnLanguageSelect.PointerClick.RemoveAllListeners();
			m_comBtnLanguageSelect.PointerClick.AddListener(OnLanguageSelect);
		}
		NKCUtil.SetButtonClickDelegate(m_comBtnMoveToPatch, OnMoveToPatch);
		NKCUtil.SetButtonClickDelegate(m_comBtnPatchSkipTest, OnPatchSkipTest);
		NKCUtil.SetGameobjectActive(m_comBtnPatchSkipTest, NKCDefineManager.DEFINE_PATCH_SKIP());
		if (m_NUF_LOGIN_SCREEN != null)
		{
			EventTrigger eventTrigger = m_NUF_LOGIN_SCREEN.GetComponent<EventTrigger>();
			if (eventTrigger == null)
			{
				eventTrigger = m_NUF_LOGIN_SCREEN.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				TouchLogin();
			});
			eventTrigger.triggers.Clear();
			eventTrigger.triggers.Add(entry);
		}
		if (m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL != null)
		{
			m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL.PointerClick.RemoveAllListeners();
			m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL.PointerClick.AddListener(OnClickAuthLevelChange);
		}
		string text = $"{NKCUtilString.GetAppVersionText()} {NKCConnectionInfo.s_ServerType}";
		if (!string.IsNullOrEmpty(NKCUtil.PatchVersion))
		{
			string[] array = NKCUtil.PatchVersion.Split('_');
			if (array.Length != 0)
			{
				text = text + " A." + array[array.Length - 1];
			}
		}
		if (!string.IsNullOrEmpty(NKCUtil.PatchVersionEA))
		{
			string[] array2 = NKCUtil.PatchVersionEA.Split('_');
			if (array2.Length != 0)
			{
				text = text + " E." + array2[array2.Length - 1];
			}
		}
		NKCUtil.SetLabelText(m_lbVersion, text);
		LoadBackgroundTemplet();
		LoadPrefabList();
		base.gameObject.SetActive(value: false);
		if (m_NUF_LOGIN_DEV_SERVER_LIST != null)
		{
			m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN = m_NUF_LOGIN_DEV_SERVER_LIST.transform.GetComponentInChildren<Dropdown>();
			if (m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN != null)
			{
				m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN.onValueChanged.AddListener(delegate
				{
					OnServerListDropDown(m_NUF_LOGIN_DEV_SERVER_LIST_DROPDOWN);
				});
			}
		}
		if (!NKCDefineManager.DEFINE_SERVICE())
		{
			m_NUF_LOGIN_DEV_SERVER_LIST.SetActive(!NKCDefineManager.DEFINE_SELECT_SERVER());
		}
		else
		{
			m_NUF_LOGIN_DEV_SERVER_LIST.SetActive(value: false);
			m_NUF_LOGIN_DEV_CUTSCEN.SetActive(value: false);
			m_NUF_LOGIN_DEV_VOICE_LIST.SetActive(value: false);
			NKCUtil.SetGameobjectActive(m_comBtnLanguageSelect, bValue: false);
			NKCUtil.SetGameobjectActive(m_comBtnMoveToPatch, bValue: false);
			NKCUtil.SetGameobjectActive(m_comBtnPatchSkipTest, bValue: false);
		}
		if (m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL != null)
		{
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL.gameObject, bValue: false);
		}
		if (NKCPublisherModule.IsPCBuild())
		{
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON, !NKCPublisherModule.IsPublisherNoneType());
		}
		if (!(m_csbtnPcQR != null))
		{
			return;
		}
		if (NKCPublisherModule.PublisherType == NKCPublisherModule.ePublisherType.Zlong)
		{
			if (NKCPublisherModule.Auth.CheckNeedToCheckEnableQR_AfterPubLogin())
			{
				NKCUtil.SetGameobjectActive(m_csbtnPcQR, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_csbtnPcQR, NKCPublisherModule.Auth.CheckEnableQR_Login());
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnPcQR, bValue: false);
		}
		m_csbtnPcQR.PointerClick.RemoveAllListeners();
		m_csbtnPcQR.PointerClick.AddListener(OnClickPcQR);
	}

	private void OnClickPcQR()
	{
		NKMPopUpBox.OpenWaitBox();
		NKCPublisherModule.Auth.QR_Login(OnCompleteQR_Login);
	}

	private void OnCompleteQR_Login(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		NKCPublisherModule.CheckError(resultCode, additionalError);
	}

	private void OnClickAuthLevelChange()
	{
		int num = Enum.GetNames(typeof(NKM_USER_AUTH_LEVEL)).Length;
		if ((int)m_NKM_USER_AUTH_LEVEL >= (int)(byte)num)
		{
			m_NKM_USER_AUTH_LEVEL = NKM_USER_AUTH_LEVEL.NORMAL_USER;
		}
		else
		{
			m_NKM_USER_AUTH_LEVEL++;
		}
		ApplyNowAuthLevelToText();
	}

	private void OnClickNoticeButton()
	{
		NKCPublisherModule.Notice.OpenNotice(null);
	}

	public void Open()
	{
		RescaleBack();
		base.gameObject.SetActive(value: true);
		m_fUpdateTime = 0f;
		LoadLastEditServerAddress();
		Log.Debug("[Login] Open PublisherModule[" + NKCPublisherModule.PublisherType.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginViewer.cs", 547);
		if (NKCPublisherModule.IsPublisherNoneType())
		{
			if (PlayerPrefs.HasKey("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING"))
			{
				m_objNUF_LOGIN_DEV_LOGIN.SetActive(value: false);
				m_bShowUpLoginPanel = false;
				NKCUtil.SetGameobjectActive(m_lbNUF_LOGIN_UPDATE_Text, bValue: true);
				Log.Debug("[Login] HasKey[" + "NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING".ToString() + "] Value[" + PlayerPrefs.GetString("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING") + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginViewer.cs", 557);
			}
			else
			{
				m_objNUF_LOGIN_DEV_LOGIN.SetActive(value: true);
				m_bShowUpLoginPanel = true;
				NKCUtil.SetGameobjectActive(m_lbNUF_LOGIN_UPDATE_Text, bValue: false);
				Log.Debug("[Login] NUF_LOGIN_DEV_LOGIN active", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginViewer.cs", 567);
				Log.Debug("[Login] HasKey[" + "NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING".ToString() + "] Value[" + PlayerPrefs.GetString("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING") + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginViewer.cs", 568);
			}
		}
		else
		{
			m_objNUF_LOGIN_DEV_LOGIN.SetActive(value: false);
			m_bShowUpLoginPanel = false;
		}
		if (NKCPublisherModule.IsPublisherNoneType())
		{
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON, bValue: true);
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_CUTSCEN, bValue: true);
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_VOICE_LIST, bValue: true);
			NKCUtil.SetGameobjectActive(m_comBtnLanguageSelect, bValue: true);
			NKCUtil.SetGameobjectActive(m_comBtnMoveToPatch, bValue: true);
			NKCUtil.SetGameobjectActive(m_comBtnPatchSkipTest, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_CUTSCEN, bValue: false);
			NKCUtil.SetGameobjectActive(m_NUF_LOGIN_DEV_VOICE_LIST, bValue: false);
			NKCUtil.SetGameobjectActive(m_comBtnLanguageSelect, bValue: false);
			NKCUtil.SetGameobjectActive(m_comBtnMoveToPatch, bValue: false);
			NKCUtil.SetGameobjectActive(m_comBtnPatchSkipTest, bValue: false);
		}
		if (NKCDefineManager.DEFINE_USE_CHEAT())
		{
			NKCUtil.SetGameobjectActive(m_comBtnPlaySingle, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_comBtnPlaySingle, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objBsideLogo, !NKCPublisherModule.IsZlongPublished());
		NKCUtil.SetGameobjectActive(m_objNexonLogo, NKCPublisherModule.IsNexonPublished());
		NKCUtil.SetGameobjectActive(m_objZlongLogo, NKCPublisherModule.IsZlongPublished());
		SetLoginMessageUI();
		UIOpened();
	}

	private void SetLoginMessageUI()
	{
		if (NKCPublisherModule.PublisherType == NKCPublisherModule.ePublisherType.Zlong && m_objMessage != null && m_NKCUILoginViewerMsg == null)
		{
			if (m_NKCAssetInstanceDataMessage != null)
			{
				NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataMessage);
			}
			m_NKCAssetInstanceDataMessage = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_LOGIN", "NUF_LOGIN_MESSAGE");
			if (m_NKCAssetInstanceDataMessage != null && m_NKCAssetInstanceDataMessage.m_Instant != null)
			{
				NKCUILoginViewerMsg componentInChildren = m_NKCAssetInstanceDataMessage.m_Instant.GetComponentInChildren<NKCUILoginViewerMsg>();
				m_NKCUILoginViewerMsg = componentInChildren;
				m_NKCAssetInstanceDataMessage.m_Instant.transform.SetParent(m_objMessage.transform, worldPositionStays: false);
				m_NKCAssetInstanceDataMessage.m_Instant.transform.localPosition = new Vector3(m_NKCAssetInstanceDataMessage.m_Instant.transform.localPosition.x, m_NKCAssetInstanceDataMessage.m_Instant.transform.localPosition.y, 0f);
			}
			if (m_NKCUILoginViewerMsg != null)
			{
				m_NKCUILoginViewerMsg.ForceUpdateMsg();
			}
		}
	}

	public override void Hide()
	{
		base.Hide();
		NKCUtil.SetGameobjectActive(m_objBack, bValue: false);
	}

	public override void UnHide()
	{
		base.UnHide();
		NKCUtil.SetGameobjectActive(m_objBack, bValue: true);
	}

	public override void OnCloseInstance()
	{
		if (m_objBack != null)
		{
			UnityEngine.Object.Destroy(m_objBack);
			m_objBack = null;
		}
		if (m_BackResource != null)
		{
			NKCAssetResourceManager.CloseResource(m_BackResource);
			m_BackResource = null;
		}
	}

	private void ApplyNowAuthLevelToText()
	{
		if (m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL_TEXT != null)
		{
			m_NUF_LOGIN_DEV_LOGIN_AUTH_LEVEL_TEXT.text = m_NKM_USER_AUTH_LEVEL.ToString();
		}
	}

	private void LoadLastEditServerAddress()
	{
		if (PlayerPrefs.HasKey(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED))
		{
			string text = PlayerPrefs.GetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED);
			m_IFServerAddress.text = text;
		}
		else
		{
			m_IFServerAddress.text = "192.168.0.201";
			PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED, m_IFServerAddress.text);
		}
	}

	public void OnEndEditServerAddress()
	{
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED, m_IFServerAddress.text);
	}

	public void SaveIDPass()
	{
		Log.Debug($"[Login] SaveIDPass ShowUpLoginPanel[{m_bShowUpLoginPanel}]  ID[{m_IFID.text}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginViewer.cs", 717);
		if (m_bShowUpLoginPanel)
		{
			PlayerPrefs.SetString("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING", m_IFID.text);
			PlayerPrefs.SetString("NKM_LOCAL_SAVE_KEY_LOGIN_PASSWORD_STRING", m_IFPassword.text);
		}
	}

	private void OnServerListDropDown(Dropdown change)
	{
		string text = m_IFServerAddress.text;
		switch (change.captionText.text)
		{
		case "Dev":
			text = "studiobsidedev.com";
			break;
		case "Next":
			text = "Build02:32000";
			break;
		case "Stage":
			text = "52.231.15.96";
			break;
		case "Review":
			text = "52.141.21.24";
			break;
		case "SeaStage":
			text = "DataManager:52000";
			break;
		case "SeaLive":
			text = "DataManager:32000";
			break;
		case "SeaQa":
			text = "DataManager:42000";
			break;
		case "TaiwanQa":
			text = "build02";
			break;
		case "TaiwanStage":
			text = "DataManager";
			break;
		case "Vietnam":
			text = "192.168.0.145";
			break;
		case "JapanQa":
			text = "Build02:5310";
			break;
		case "JapanStage":
			text = "Build02:6310";
			break;
		case "JapanLive":
			text = "Build02:7310";
			break;
		case "devChina":
			text = "DataManager:8100";
			break;
		case "ChinaQa":
			text = "DataManager:7100";
			break;
		case "ChinaStage":
			text = "DataManager:9100";
			break;
		}
		m_IFServerAddress.text = text;
	}

	private void LoadConnectionAddressFromUI()
	{
		Log.Debug("[Login] LoadConnectionAddressFromUI m_IFServerAddress[" + m_IFServerAddress.text + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginViewer.cs", 819);
		string text = m_IFServerAddress.text;
		int num = 22000;
		string[] array = m_IFServerAddress.text.Split(':');
		text = array[0];
		if (array.Length > 1)
		{
			num = Convert.ToInt32(array[1]);
		}
		NKCConnectionInfo.SetLoginServerInfo(NKCConnectionInfo.LOGIN_SERVER_TYPE.Default, text, num);
		Log.Debug($"[Login] NKCConnectionInfo s_ServiceIP[{text}] s_ServicePort[{num}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginViewer.cs", 832);
	}

	public string GetCurrentServerAddress()
	{
		return NKCConnectionInfo.ServiceIP;
	}

	public int GetCurrentServerPort()
	{
		return NKCConnectionInfo.ServicePort;
	}

	private void InitConnect()
	{
		NKCConnectLogin connectLogin = NKCScenManager.GetScenManager().GetConnectLogin();
		connectLogin.SetRemoteAddress(GetCurrentServerAddress(), GetCurrentServerPort());
		connectLogin.ResetConnection();
	}

	public void TouchLogin()
	{
		if (!m_bShowUpLoginPanel)
		{
			OnLogin();
		}
	}

	private void OnLogin()
	{
		Log.Debug(string.Format("[Agreement] OnLogin Opened[{0}]", NKMContentsVersionManager.HasTag("CHECK_AGREEMENT_NOTICE")), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginViewer.cs", 865);
		if (NKMContentsVersionManager.HasTag("CHECK_AGREEMENT_NOTICE") && !NKCUIAgreementNotice.IsAgreementChecked())
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUIAgreementNotice.PopupMessage, NKCPopupMessage.eMessagePosition.Middle, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (m_loginTouchDelay > 0f)
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_UNDER_MAINTENANCE);
			NKCScenManager.GetScenManager().Get_SCEN_LOGIN()?.UpdateLoginMsgUI();
		}
		else
		{
			NKCScenManager.GetScenManager().Get_SCEN_LOGIN().TryLogin();
		}
	}

	private void OnChangeAccountDev()
	{
		m_bShowUpLoginPanel = true;
		if (!m_objNUF_LOGIN_DEV_LOGIN.activeSelf)
		{
			m_objNUF_LOGIN_DEV_LOGIN.SetActive(value: true);
		}
		NKCUtil.SetGameobjectActive(m_lbNUF_LOGIN_UPDATE_Text, bValue: false);
		if (PlayerPrefs.HasKey("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING"))
		{
			string text = PlayerPrefs.GetString("NKM_LOCAL_SAVE_KEY_LOGIN_ID_STRING");
			string text2 = PlayerPrefs.GetString("NKM_LOCAL_SAVE_KEY_LOGIN_PASSWORD_STRING");
			m_IFID.text = text;
			m_IFPassword.text = text2;
		}
	}

	public void SetLoginTouchDelay()
	{
		if (!NKCDefineManager.DEFINE_UNITY_EDITOR() && !NKCDefineManager.DEFINE_PC_EXTRA_DOWNLOAD_IN_EXE_FOLDER() && (NKCDefineManager.DEFINE_SB_GB() || NKCDefineManager.DEFINE_USE_TOUCH_DELAY()))
		{
			m_loginTouchDelay = 60f;
		}
	}

	private void OnPlaySingle()
	{
		if (NKCDefineManager.DEFINE_USE_CHEAT())
		{
			NKCPacketSender.Send_NKMPacket_DEV_GAME_LOAD_REQ("NKM_DUNGEON_TEST");
		}
	}

	public void OnCutScenSim()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_SIM);
	}

	public void OnVoiceList()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_VOICE_LIST);
	}

	public void OnLanguageSelect()
	{
		NKCUIPopupLanguageSelect.Instance.Open(NKCLocalization.GetSelectLanguageSet(), OnChangeLanguage);
	}

	private void OnMoveToPatch()
	{
		NKCScenManager.GetScenManager().ShowBundleUpdate(bCallFromTutorial: false);
	}

	private void OnPatchSkipTest()
	{
		NKCPatchUtility.DeleteTutorialClearedStatus();
		NKCPatchUtility.ReservePatchSkipTest();
		Application.Quit();
	}

	private void OnChangeLanguage(NKM_NATIONAL_CODE eNKM_NATIONAL_CODE)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && eNKM_NATIONAL_CODE != NKM_NATIONAL_CODE.NNC_END)
		{
			gameOptionData.NKM_NATIONAL_CODE = eNKM_NATIONAL_CODE;
			NKCGameOptionData.SaveOnlyLang(eNKM_NATIONAL_CODE);
			Application.Quit();
		}
	}

	public void OnGoogleLogin()
	{
	}

	public void OnCollection()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_COLLECTION);
	}

	private void OnSaveServer1ST()
	{
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_1ST, m_IFServerAddress.text);
	}

	private void OnLoadServer1ST()
	{
		if (PlayerPrefs.HasKey(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_1ST))
		{
			string text = PlayerPrefs.GetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_1ST);
			m_IFServerAddress.text = text;
		}
		else
		{
			m_IFServerAddress.text = "192.168.0.201";
		}
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED, m_IFServerAddress.text);
	}

	private void OnSaveServer2ND()
	{
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_2ND, m_IFServerAddress.text);
	}

	private void OnLoadServer2ND()
	{
		if (PlayerPrefs.HasKey(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_2ND))
		{
			string text = PlayerPrefs.GetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_2ND);
			m_IFServerAddress.text = text;
		}
		else
		{
			m_IFServerAddress.text = "";
		}
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED, m_IFServerAddress.text);
	}

	private void OnSaveServer3RD()
	{
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_3RD, m_IFServerAddress.text);
	}

	private void OnLoadServer3RD()
	{
		if (PlayerPrefs.HasKey(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_3RD))
		{
			string text = PlayerPrefs.GetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_3RD);
			m_IFServerAddress.text = text;
		}
		else
		{
			m_IFServerAddress.text = "";
		}
		PlayerPrefs.SetString(NKM_LOCAL_SAVE_KEY_LOGIN_SERVER_LAST_EDITED, m_IFServerAddress.text);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(m_objBack, bValue: false);
		base.gameObject.SetActive(value: false);
	}

	public void Update()
	{
		m_fUpdateTime += Time.deltaTime;
		if (m_loginTouchDelay > 0f)
		{
			m_loginTouchDelay -= Time.deltaTime;
		}
		if (!m_objNUF_LOGIN_DEV_LOGIN.activeSelf && m_bShowUpLoginPanel && m_fUpdateTime > 2.5f)
		{
			m_objNUF_LOGIN_DEV_LOGIN.SetActive(value: true);
		}
	}

	public void AfterLoginToPublisherCompleted(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		NKMPopUpBox.CloseWaitBox();
		Debug.Log("AfterLoginToPublisherCompleted");
		NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.AfterSyncAccountComplete);
		if (NKCPublisherModule.IsPCBuild() || NKCPublisherModule.IsPublisherNoneType())
		{
			return;
		}
		bool loginToPublisherCompleted = NKCPublisherModule.Auth.LoginToPublisherCompleted;
		if (loginToPublisherCompleted)
		{
			Debug.Log("bSyncAccountActive true");
			NKCUtil.SetImageSprite(m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON_BG, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_LOGIN_YELLOW));
			m_comBtnChangeAccount.PointerClick.RemoveAllListeners();
			m_comBtnChangeAccount.PointerClick.AddListener(OnChangeAccountPublisher);
			if (NKCPublisherModule.Auth.CheckNeedToCheckEnableQR_AfterPubLogin())
			{
				NKCUtil.SetGameobjectActive(m_csbtnPcQR, NKCPublisherModule.Auth.CheckEnableQR_Login());
			}
		}
		else
		{
			Debug.Log("bSyncAccountActive false");
			NKCUtil.SetImageSprite(m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON_BG, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			m_comBtnChangeAccount.PointerClick.RemoveAllListeners();
		}
		NKCUtil.SetLabelText(m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON_TEXT, loginToPublisherCompleted ? NKCUtilString.GET_STRING_TOY_LOGIN_CHANGE_ACCOUNT : NKCUtilString.GET_STRING_TOY_LOGIN_NO_AUTH);
		NKCUtil.SetLabelTextColor(m_NUF_LOGIN_DEV_CHANGE_ACCOUNT_BUTTON_TEXT, Color.black);
	}

	private void OnChangeAccountPublisher()
	{
		NKCPublisherModule.Auth.ChangeAccount(AfterLoginToPublisherCompleted, syncAccount: false);
	}

	private void OpenQuitApplicationPopup()
	{
		if (NKCPublisherModule.Auth.CheckExitCallFirst())
		{
			NKCPublisherModule.Auth.Exit(OnCompleteExitFirst);
		}
		else
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_LOBBY_CHECK_QUIT_GAME, OnQuit);
		}
	}

	private void OnQuit()
	{
		NKMPopUpBox.OpenWaitBox();
		NKCPublisherModule.Auth.Exit(OnCompleteExit);
	}

	private void OnCompleteExit(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (NKCPublisherModule.CheckError(resultCode, additionalError))
		{
			Application.Quit();
		}
	}

	private void OnCompleteExitFirst(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (NKCPublisherModule.CheckError(resultCode, additionalError))
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_LOBBY_CHECK_QUIT_GAME, delegate
			{
				Application.Quit();
			});
		}
	}
}
