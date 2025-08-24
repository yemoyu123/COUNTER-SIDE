using System;
using NKC.Patcher;
using NKC.UI;
using UnityEngine;

namespace NKC;

public class NKCPopupSelectServer : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_LOGIN_SELECT";

	private const string UI_ASSET_NAME = "AB_UI_LOGIN_SELECT_SERVER";

	private static NKCPopupSelectServer m_Instance;

	private RectTransform m_rectTransform;

	public NKCUIComToggleGroup m_ctglGroup;

	public NKCUIComToggle m_ctglKorea;

	public NKCUIComToggle m_ctglGlobal;

	public NKCUIComStateButton m_csbtnOk;

	public NKCUIComStateButton m_csbtnClose;

	private NKCConnectionInfo.LOGIN_SERVER_TYPE m_selectedServerType;

	public Action m_onClosed;

	private bool m_bShowCloseMenu;

	private bool m_bMoveToPatcher;

	public static NKCPopupSelectServer Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupSelectServer>("AB_UI_LOGIN_SELECT", "AB_UI_LOGIN_SELECT_SERVER", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupSelectServer>();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "SelectServer";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void Open(bool bShowCloseMenu, bool bMoveToPatcher, Action onClosed = null)
	{
		m_selectedServerType = NKCConnectionInfo.LOGIN_SERVER_TYPE.None;
		m_onClosed = onClosed;
		m_bShowCloseMenu = bShowCloseMenu;
		m_bMoveToPatcher = bMoveToPatcher;
		InitUI();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
	}

	private void InitUI()
	{
		NKCUtil.SetGameobjectActive(m_csbtnClose, m_bShowCloseMenu);
		if (m_bShowCloseMenu)
		{
			m_csbtnClose.PointerClick.RemoveAllListeners();
			m_csbtnClose.PointerClick.AddListener(delegate
			{
				OnClickClose();
			});
		}
		else
		{
			m_rectTransform = GetComponent<RectTransform>();
			m_rectTransform.localPosition = new Vector3(0f, 0f, 0f);
		}
		NKCUtil.SetGameobjectActive(m_ctglGroup, bValue: true);
		m_ctglGroup.SetAllToggleUnselected();
		NKCUtil.SetGameobjectActive(m_ctglKorea, NKCConnectionInfo.HasLoginServerInfo(NKCConnectionInfo.LOGIN_SERVER_TYPE.Korea));
		m_ctglKorea.OnValueChanged.RemoveAllListeners();
		m_ctglKorea.OnValueChanged.AddListener(delegate
		{
			OnClickServer(NKCConnectionInfo.LOGIN_SERVER_TYPE.Korea);
		});
		NKCUtil.SetGameobjectActive(m_ctglGlobal, NKCConnectionInfo.HasLoginServerInfo(NKCConnectionInfo.LOGIN_SERVER_TYPE.Global));
		m_ctglGlobal.OnValueChanged.RemoveAllListeners();
		m_ctglGlobal.OnValueChanged.AddListener(delegate
		{
			OnClickServer(NKCConnectionInfo.LOGIN_SERVER_TYPE.Global);
		});
		m_csbtnOk.PointerClick.RemoveAllListeners();
		m_csbtnOk.PointerClick.AddListener(delegate
		{
			OnClickOk();
		});
	}

	private void OnClickServer(NKCConnectionInfo.LOGIN_SERVER_TYPE type)
	{
		m_selectedServerType = type;
	}

	private void OnClickClose()
	{
		m_onClosed?.Invoke();
		Close();
	}

	private void OnClickOk()
	{
		if (m_selectedServerType == NKCConnectionInfo.LOGIN_SERVER_TYPE.None)
		{
			return;
		}
		if (m_selectedServerType == NKCConnectionInfo.CurrentLoginServerType)
		{
			OnClickClose();
			return;
		}
		NKCConnectionInfo.CurrentLoginServerType = m_selectedServerType;
		NKCConnectionInfo.SaveCurrentLoginServerType();
		if (NKCPatchDownloader.Instance != null)
		{
			NKCPatchDownloader.Instance.VersionCheckStatus = NKCPatchDownloader.VersionStatus.Unchecked;
		}
		OnClickClose();
		NKCCollectionManager.SetReloadCollectionData();
		if (m_bMoveToPatcher)
		{
			NKCScenManager.GetScenManager().MoveToPatchScene();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
