using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCPopupShopCustomPackageSubstitude : NKCUIBase
{
	public delegate void OnClose();

	public struct SubstituteData
	{
		public NKCUISlot.SlotData Before;

		public NKCUISlot.SlotData After;
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_shop";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_SHOP_BUY_PACKAGE_SUBSTITUDE";

	private static NKCPopupShopCustomPackageSubstitude m_Instance;

	public NKCPopupShopCustomPackageSubstitudeSlot m_pfbSlot;

	public LoopScrollRect m_srContent;

	public NKCUIComStateButton m_csbtnCancel;

	public NKCUIComStateButton m_csbtnConfirm;

	private OnClose dOnClose;

	private List<SubstituteData> m_lstSubstituteData;

	public static NKCPopupShopCustomPackageSubstitude Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupShopCustomPackageSubstitude>("ab_ui_nkm_ui_shop", "NKM_UI_POPUP_SHOP_BUY_PACKAGE_SUBSTITUDE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupShopCustomPackageSubstitude>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void Init()
	{
		m_srContent.dOnGetObject += GetObject;
		m_srContent.dOnReturnObject += ReturnObject;
		m_srContent.dOnProvideData += ProvideData;
		m_srContent.PrepareCells();
		NKCUtil.SetScrollHotKey(m_srContent);
		NKCUtil.SetButtonClickDelegate(m_csbtnCancel, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnConfirm, OnOK);
		NKCUtil.SetHotkey(m_csbtnConfirm, HotkeyEventType.Confirm);
	}

	public void Open(List<NKCShopManager.ShopRewardSubstituteData> lstSubstituteReward, OnClose onClose)
	{
		if (lstSubstituteReward == null || lstSubstituteReward.Count == 0)
		{
			base.gameObject.SetActive(value: false);
			dOnClose?.Invoke();
			return;
		}
		m_lstSubstituteData = new List<SubstituteData>();
		for (int i = 0; i < lstSubstituteReward.Count; i++)
		{
			NKCUISlot.SlotData before = NKCUISlot.SlotData.MakeRewardTypeData(lstSubstituteReward[i].Before);
			NKCUISlot.SlotData after = NKCUISlot.SlotData.MakeRewardTypeData(lstSubstituteReward[i].After);
			m_lstSubstituteData.Add(new SubstituteData
			{
				Before = before,
				After = after
			});
		}
		dOnClose = onClose;
		UIOpened();
		m_srContent.TotalCount = m_lstSubstituteData.Count;
		m_srContent.SetIndexPosition(0);
	}

	private void OnOK()
	{
		Close();
		dOnClose?.Invoke();
		dOnClose = null;
		m_lstSubstituteData = null;
	}

	private RectTransform GetObject(int index)
	{
		NKCPopupShopCustomPackageSubstitudeSlot nKCPopupShopCustomPackageSubstitudeSlot = Object.Instantiate(m_pfbSlot);
		nKCPopupShopCustomPackageSubstitudeSlot.Init();
		return nKCPopupShopCustomPackageSubstitudeSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		if (tr != null)
		{
			tr.SetParent(null);
		}
		tr.gameObject.SetActive(value: false);
		Object.Destroy(tr.gameObject);
	}

	private void ProvideData(Transform transform, int idx)
	{
		if (idx >= m_lstSubstituteData.Count || idx < 0)
		{
			transform.gameObject.SetActive(value: false);
			return;
		}
		NKCPopupShopCustomPackageSubstitudeSlot component = transform.GetComponent<NKCPopupShopCustomPackageSubstitudeSlot>();
		if (component != null)
		{
			component.SetData(m_lstSubstituteData[idx].Before, m_lstSubstituteData[idx].After);
		}
		else
		{
			transform.gameObject.SetActive(value: false);
		}
	}
}
