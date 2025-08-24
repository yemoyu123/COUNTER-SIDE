using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupProfileFrameChange : NKCUIBase
{
	public delegate void OnSelectFrame(int frameID);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_BORDER_CHANGE";

	private static NKCUIPopupProfileFrameChange m_Instance;

	public NKCUISlotProfile m_pfbSlot;

	public LoopScrollRect m_LoopScrollRect;

	public NKCUIComStateButton m_csbtnCancel;

	public NKCUIComStateButton m_csbtnOK;

	private OnSelectFrame dOnSelectFrame;

	private int m_selectedFrame;

	private int m_ProfileUnitID;

	private int m_ProfileSkinID;

	private Stack<NKCUISlotProfile> m_stkSlot = new Stack<NKCUISlotProfile>();

	private NKCUISlotProfile m_slotSelected;

	public Transform m_rtSlotPool;

	private List<int> m_lstFrameItemID;

	public static NKCUIPopupProfileFrameChange Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupProfileFrameChange>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_BORDER_CHANGE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupProfileFrameChange>();
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

	public override string MenuName => NKCStringTable.GetString("SI_PF_PROFILE_BORDER_CHANGE");

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
		NKCUtil.SetBindFunction(m_csbtnCancel, base.Close);
		NKCUtil.SetBindFunction(m_csbtnOK, OnBtnOK);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		m_LoopScrollRect.dOnGetObject += GetSlot;
		m_LoopScrollRect.dOnReturnObject += ReturnSlot;
		m_LoopScrollRect.dOnProvideData += ProvideSlotData;
		m_LoopScrollRect.PrepareCells();
		NKCUtil.SetScrollHotKey(m_LoopScrollRect);
	}

	public override void OnCloseInstance()
	{
		base.OnCloseInstance();
		m_lstFrameItemID?.Clear();
		m_lstFrameItemID = null;
	}

	public void Open(NKMUserProfileData profileData, OnSelectFrame onSelectFrame)
	{
		Open(profileData.commonProfile.mainUnitId, profileData.commonProfile.mainUnitSkinId, profileData.commonProfile.frameId, onSelectFrame);
	}

	public void Open(NKMCommonProfile commonProfile, OnSelectFrame onSelectFrame)
	{
		Open(commonProfile.mainUnitId, commonProfile.mainUnitSkinId, commonProfile.frameId, onSelectFrame);
	}

	public void Open(int profileUnitID, int profileSkinID, int currentFrameID, OnSelectFrame onSelectFrame)
	{
		if (NKCContentManager.IsContentsUnlocked(ContentsType.PROFILE_FRAME))
		{
			m_ProfileUnitID = profileUnitID;
			m_ProfileSkinID = profileSkinID;
			m_selectedFrame = currentFrameID;
			dOnSelectFrame = onSelectFrame;
			UIOpened();
			m_lstFrameItemID = GetFrameIDList();
			m_LoopScrollRect.TotalCount = m_lstFrameItemID.Count;
			m_LoopScrollRect.SetIndexPosition(0);
		}
	}

	private void OnSelectSlot(NKCUISlotProfile selectedSlot, int selectedFrame)
	{
		m_slotSelected?.SetSelected(value: false);
		m_slotSelected = selectedSlot;
		selectedSlot?.SetSelected(value: true);
		m_selectedFrame = selectedFrame;
	}

	private List<int> GetFrameIDList()
	{
		List<int> list = new List<int>();
		list.Add(0);
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		foreach (NKMItemMiscTemplet value in NKMItemMiscTemplet.Values)
		{
			if (value.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME && inventoryData.GetCountMiscItem(value.Key) > 0)
			{
				list.Add(value.Key);
			}
		}
		return list;
	}

	private void OnBtnOK()
	{
		Close();
		dOnSelectFrame?.Invoke(m_selectedFrame);
	}

	private RectTransform GetSlot(int index)
	{
		if (m_stkSlot.Count > 0)
		{
			return m_stkSlot.Pop().GetComponent<RectTransform>();
		}
		NKCUISlotProfile nKCUISlotProfile = Object.Instantiate(m_pfbSlot);
		nKCUISlotProfile.Init();
		NKCUtil.SetGameobjectActive(nKCUISlotProfile, bValue: true);
		nKCUISlotProfile.transform.localScale = Vector3.one;
		nKCUISlotProfile.transform.SetParent(m_LoopScrollRect.content);
		return nKCUISlotProfile.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		go.SetParent(m_rtSlotPool);
		NKCUISlotProfile component = go.GetComponent<NKCUISlotProfile>();
		if (component != null)
		{
			m_stkSlot.Push(component);
		}
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		if (m_lstFrameItemID == null)
		{
			tr.gameObject.SetActive(value: false);
			return;
		}
		if (m_lstFrameItemID.Count < idx || idx < 0)
		{
			tr.gameObject.SetActive(value: false);
			return;
		}
		int num = m_lstFrameItemID[idx];
		NKCUISlotProfile component = tr.GetComponent<NKCUISlotProfile>();
		if (component != null)
		{
			component.SetProfiledata(m_ProfileUnitID, m_ProfileSkinID, num, OnSelectSlot);
			bool flag = num == m_selectedFrame;
			component.SetSelected(flag);
			if (flag)
			{
				m_slotSelected = component;
			}
		}
	}
}
