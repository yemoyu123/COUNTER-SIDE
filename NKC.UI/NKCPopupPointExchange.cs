using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupPointExchange : NKCUIBase
{
	public const string UI_ASSET_BUNDLE_NAME = "ui_event_md_popup_single_pe";

	public const string UI_ASSET_NAME = "UI_EVENT_MD_POPUP_SINGLE_PE";

	private static NKCPopupPointExchange m_Instance;

	public GameObject m_objRoot;

	public Transform m_UIParent;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnInfomation;

	public NKCUIEventBarMissionGroupList m_comMissionGroupList;

	private NKCUIPointExchange m_pointExchangeUI;

	private NKCAssetInstanceData m_assetInstanceData;

	private int m_missionTabId;

	public static NKCPopupPointExchange Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupPointExchange>("ui_event_md_popup_single_pe", "UI_EVENT_MD_POPUP_SINGLE_PE", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanUpInstance).GetInstance<NKCPopupPointExchange>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override string MenuName => "Point Exchange";

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanUpInstance()
	{
		m_Instance?.Release();
		m_Instance = null;
	}

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, OnClickClose);
		NKCUtil.SetButtonClickDelegate(m_csbtnInfomation, OnClickInfomation);
		NKMPointExchangeTemplet byTime = NKMPointExchangeTemplet.GetByTime(NKCSynchronizedTime.ServiceTime);
		string text = "";
		if (byTime != null)
		{
			text = byTime.PrefabId;
			m_missionTabId = byTime.MissionTabId;
		}
		if (m_pointExchangeUI == null)
		{
			NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(text, text);
			m_pointExchangeUI = OpenInstanceByAssetName<NKCUIPointExchange>(nKMAssetName.m_BundleName, nKMAssetName.m_AssetName, m_UIParent);
			if (m_pointExchangeUI != null)
			{
				m_pointExchangeUI.Init(OnClickClose, OnClickInfomation);
				m_pointExchangeUI.ResetUI();
			}
		}
		m_comMissionGroupList?.Init("ui_event_md_popup_single_pe", "POPUP_PE_MISSION_LIST_SLOT");
		base.gameObject.SetActive(value: false);
	}

	public override void CloseInternal()
	{
		foreach (NKCUIPointExchangeTransition item in NKCUIManager.GetOpenedUIsByType<NKCUIPointExchangeTransition>())
		{
			if (item.IsOpen)
			{
				item.Close();
			}
		}
		m_pointExchangeUI?.RevertMusic();
		base.gameObject.SetActive(value: false);
	}

	public override void OnBackButton()
	{
		OnClickClose();
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		m_pointExchangeUI?.ResetUI();
		m_comMissionGroupList?.CloseImmediately();
		UIOpened();
	}

	public void RefreshPoint()
	{
		m_pointExchangeUI?.RefreshPoint();
	}

	public void RefreshProduct()
	{
		if (m_pointExchangeUI != null)
		{
			m_pointExchangeUI.RefreshPoint();
			m_pointExchangeUI.RefreshScrollRect();
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		if (m_pointExchangeUI != null)
		{
			m_pointExchangeUI.ResetUI();
		}
	}

	public void RefreshMission()
	{
		if (!(m_comMissionGroupList == null) && m_comMissionGroupList.IsOpened())
		{
			m_comMissionGroupList.Refresh();
		}
	}

	public void PlayMusic()
	{
		m_pointExchangeUI?.PlayMusic();
	}

	private T OpenInstanceByAssetName<T>(string BundleName, string AssetName, Transform parent) where T : MonoBehaviour
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(BundleName, AssetName, bAsync: false, parent);
		if (nKCAssetInstanceData != null && nKCAssetInstanceData.m_Instant != null)
		{
			GameObject instant = nKCAssetInstanceData.m_Instant;
			T component = instant.GetComponent<T>();
			if (component == null)
			{
				Object.Destroy(instant);
				NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
				return null;
			}
			m_assetInstanceData = nKCAssetInstanceData;
			return component;
		}
		Debug.LogWarning("prefab is null - " + BundleName + "/" + AssetName);
		return null;
	}

	private bool CanClose()
	{
		if (m_comMissionGroupList != null && !m_comMissionGroupList.IsClosed())
		{
			m_comMissionGroupList.Close();
			return false;
		}
		return true;
	}

	private void OnClickInfomation()
	{
		if (!(m_comMissionGroupList == null))
		{
			if (m_comMissionGroupList.IsOpened())
			{
				m_comMissionGroupList.Close();
			}
			else
			{
				m_comMissionGroupList.Open(NKCUIEventBarMissionGroupList.MissionType.MissionTabId, m_missionTabId);
			}
		}
	}

	private void OnClickClose()
	{
		if (CanClose())
		{
			Close();
		}
	}

	private void Release()
	{
		m_pointExchangeUI = null;
		NKCAssetResourceManager.CloseInstance(m_assetInstanceData);
		m_assetInstanceData = null;
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		RefreshPoint();
	}
}
