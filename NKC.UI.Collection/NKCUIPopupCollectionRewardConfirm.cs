using System.Collections.Generic;
using UnityEngine;

namespace NKC.UI.Collection;

public class NKCUIPopupCollectionRewardConfirm : NKCUIBase
{
	public delegate void OnButton();

	private const string ASSET_BUNDLE_NAME = "ab_ui_collection";

	private const string UI_ASSET_NAME = "AB_UI_COLLECTION_POPUP_REWARD";

	private static NKCUIPopupCollectionRewardConfirm m_Instance;

	public Transform m_slotParent;

	public NKCUIComStateButton m_OKButton;

	public NKCUIComStateButton m_CancelButton;

	private OnButton m_dOnOKButton;

	private NKCUISlot[] m_slot;

	public static NKCUIPopupCollectionRewardConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupCollectionRewardConfirm>("ab_ui_collection", "AB_UI_COLLECTION_POPUP_REWARD", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupCollectionRewardConfirm>();
				m_Instance?.Init();
			}
			return m_Instance;
		}
	}

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

	public override string MenuName => "Collection Misc Reward Confirm";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_OKButton, OnClickOK);
		NKCUtil.SetHotkey(m_OKButton, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_CancelButton, OnClickCancel);
		m_slot = m_slotParent.GetComponentsInChildren<NKCUISlot>();
		NKCUISlot[] slot = m_slot;
		for (int i = 0; i < slot.Length; i++)
		{
			slot[i].Init();
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open(List<NKCUISlot.SlotData> slotDataList, OnButton dOnOK = null)
	{
		if (slotDataList == null)
		{
			return;
		}
		int num = m_slot.Length;
		for (int i = 0; i < num; i++)
		{
			if (i < slotDataList.Count)
			{
				m_slot[i].SetData(slotDataList[i]);
				m_slot[i].SetActive(bSet: true);
			}
			else
			{
				m_slot[i].SetActive(bSet: false);
			}
		}
		m_dOnOKButton = dOnOK;
		UIOpened();
	}

	private void OnClickOK()
	{
		Close();
		if (m_dOnOKButton != null)
		{
			m_dOnOKButton();
		}
	}

	private void OnClickCancel()
	{
		Close();
	}
}
