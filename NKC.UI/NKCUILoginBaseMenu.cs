using System.Collections.Generic;
using Cs.Logging;
using NKC.Publisher;
using NKC.Templet;
using NKC.UI.Component;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI;

public class NKCUILoginBaseMenu : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_login_base";

	private const string UI_ASSET_NAME = "NUF_LOGIN_BASE_MENU";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public GameObject m_NUF_LOGIN_SCREEN;

	public NKCUIComStateButton m_comBtnChangeAccount;

	public NKCUIComStateButton m_comBtnSelectServer;

	public GameObject m_objSelectedServerStatus;

	public NKCComTMPUIText m_lbSelectedServerStatus;

	public NKCUIComStateButton m_csbtnPcQR;

	public GameObject m_NUF_LOGIN_MESSAGE;

	private NKCUILoginViewerMsg m_NKCUILoginViewerMsg;

	private NKCAssetInstanceData m_NKCAssetInstanceDataMessage;

	public NKCUIComStateButton NUF_LOGIN_NOTICE;

	private float m_fUpdateTime;

	private const float LOGIN_TOUCH_DELAY_TIME = 60f;

	private float m_loginTouchDelay;

	private NKCAssetResourceData m_BackResource;

	private GameObject m_objBack;

	private List<NKCAssetInstanceData> m_instancedPrefabList = new List<NKCAssetInstanceData>();

	public Transform m_LoginPrefabBase;

	public NKCUILoginDevMenu m_LoginDevMenu;

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

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUILoginBaseMenu>("ab_ui_login_base", "NUF_LOGIN_BASE_MENU", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public static NKCUILoginBaseMenu GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUILoginBaseMenu>();
		}
		return null;
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
				m_BackResource = NKCAssetResourceManager.OpenResource<GameObject>("AB_LOGIN_SCREEN_NKM_LS_VER_6", "AB_LOGIN_SCREEN_NKM_LS_VER_6");
			}
		}
		m_objBack = Object.Instantiate(m_BackResource.GetAsset<GameObject>(), NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIMidCanvas));
		m_objBack.transform.localPosition = Vector3.zero;
	}

	private void LoadPrefabList()
	{
		ReleaseEnabledPrefabs();
		foreach (NKCLoginBackgroundTemplet enabledPrefab in NKCLoginBackgroundTemplet.GetEnabledPrefabList())
		{
			NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(enabledPrefab.BundleName, enabledPrefab.AssetName, bAsync: false, m_LoginPrefabBase);
			if (nKCAssetInstanceData == null)
			{
				Log.Error($"[NKCUILoginViewer] LoadPrefab failed  id[{enabledPrefab.ID}] asset[{enabledPrefab.AssetName}/{enabledPrefab.BundleName}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginBaseMenu.cs", 165);
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

	public NKCUILoginDevMenu GetLoginDevMenu()
	{
		return m_LoginDevMenu;
	}

	public void InitUI()
	{
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
		if (m_comBtnChangeAccount != null)
		{
			if (NKCPublisherModule.IsPublisherNoneType())
			{
				m_comBtnChangeAccount.PointerClick.RemoveAllListeners();
				NKCUtil.SetGameobjectActive(m_comBtnChangeAccount, bValue: true);
			}
			else if (NKCPublisherModule.IsPCBuild())
			{
				NKCUtil.SetGameobjectActive(m_comBtnChangeAccount, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_comBtnChangeAccount, NKCPublisherModule.Auth.LoginToPublisherCompleted);
			}
		}
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
		LoadBackgroundTemplet();
		LoadPrefabList();
		base.gameObject.SetActive(value: false);
		if (m_csbtnPcQR != null)
		{
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
		if (m_comBtnSelectServer != null)
		{
			if (NKCDefineManager.DEFINE_SELECT_SERVER() && NKCConnectionInfo.GetLoginServerCount() > 1)
			{
				NKCUtil.SetGameobjectActive(m_comBtnSelectServer, bValue: true);
				NKCUtil.SetButtonClickDelegate(m_comBtnSelectServer, OnClickSelectServer);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_comBtnSelectServer, bValue: false);
			}
		}
		if (m_objSelectedServerStatus != null && m_lbSelectedServerStatus != null)
		{
			if (NKCDefineManager.DEFINE_SELECT_SERVER())
			{
				NKCUtil.SetGameobjectActive(m_objSelectedServerStatus, bValue: true);
				NKCUtil.SetLabelText(m_lbSelectedServerStatus, NKCConnectionInfo.GetCurrentLoginServerString("SI_PF_LOGIN_SELECTSERVER_STATUS"));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objSelectedServerStatus, bValue: false);
			}
		}
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

	private void OnClickNoticeButton()
	{
		NKCPublisherModule.Notice.OpenNotice(null);
	}

	public void Open()
	{
		RescaleBack();
		base.gameObject.SetActive(value: true);
		m_fUpdateTime = 0f;
		Log.Debug("[Login] Open PublisherModule[" + NKCPublisherModule.PublisherType.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginBaseMenu.cs", 350);
		SetLoginMessageUI();
		SetMouseCursor();
		UIOpened();
	}

	private void SetLoginMessageUI()
	{
		if (NKCPublisherModule.PublisherType == NKCPublisherModule.ePublisherType.Zlong && m_NUF_LOGIN_MESSAGE != null && m_NKCUILoginViewerMsg == null)
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
				m_NKCAssetInstanceDataMessage.m_Instant.transform.SetParent(m_NUF_LOGIN_MESSAGE.transform, worldPositionStays: false);
				m_NKCAssetInstanceDataMessage.m_Instant.transform.localPosition = new Vector3(m_NKCAssetInstanceDataMessage.m_Instant.transform.localPosition.x, m_NKCAssetInstanceDataMessage.m_Instant.transform.localPosition.y, 0f);
			}
			if (m_NKCUILoginViewerMsg != null)
			{
				m_NKCUILoginViewerMsg.ForceUpdateMsg();
			}
		}
	}

	private void SetMouseCursor()
	{
		NKCScenManager.GetScenManager().GetComponent<NKCCursor>()?.SetMouseCursor();
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
			Object.Destroy(m_objBack);
			m_objBack = null;
		}
		if (m_BackResource != null)
		{
			NKCAssetResourceManager.CloseResource(m_BackResource);
			m_BackResource = null;
		}
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
		OnLogin();
	}

	public void OnLogin()
	{
		Log.Debug(string.Format("[Agreement] OnLogin Opened[{0}]", NKMContentsVersionManager.HasTag("CHECK_AGREEMENT_NOTICE")), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUILoginBaseMenu.cs", 464);
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

	public void SetLoginTouchDelay()
	{
		if (!NKCDefineManager.DEFINE_UNITY_EDITOR() && !NKCDefineManager.DEFINE_PC_EXTRA_DOWNLOAD_IN_EXE_FOLDER() && (NKCDefineManager.DEFINE_SB_GB() || NKCDefineManager.DEFINE_USE_TOUCH_DELAY()))
		{
			m_loginTouchDelay = 60f;
		}
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
		if (NKCPublisherModule.Auth.LoginToPublisherCompleted)
		{
			Debug.Log("bSyncAccountActive true");
			NKCUtil.SetGameobjectActive(m_comBtnChangeAccount, bValue: true);
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
			NKCUtil.SetGameobjectActive(m_comBtnChangeAccount, bValue: false);
			m_comBtnChangeAccount.PointerClick.RemoveAllListeners();
		}
	}

	private void OnChangeAccountPublisher()
	{
		NKCPublisherModule.Auth.ChangeAccount(AfterLoginToPublisherCompleted, syncAccount: false);
	}

	private void OnClickSelectServer()
	{
		NKCPopupSelectServer.Instance.Open(bShowCloseMenu: true, bMoveToPatcher: true);
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
