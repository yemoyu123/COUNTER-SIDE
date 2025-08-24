using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupMiscUseCount : NKCUIBase
{
	public enum USE_ITEM_TYPE
	{
		Common,
		DailyTicket
	}

	public delegate void OnButton(int useItemId, int useItemCount);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_MISC_USE_COUNT";

	private static NKCPopupMiscUseCount m_Instance;

	public Text m_lbTitle;

	public NKCPopupMiscUseCountContents m_contents;

	[Header("하단 버튼")]
	public NKCUIComStateButton m_btnCancel;

	public NKCUIComStateButton m_btnAction;

	public Text m_txtAction;

	private OnButton dOnActionButton;

	private NKCUISlot.SlotData m_slotData;

	public static NKCPopupMiscUseCount Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupMiscUseCount>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_MISC_USE_COUNT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupMiscUseCount>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override string MenuName => "";

	public override eMenutype eUIType => eMenutype.Popup;

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

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitUI()
	{
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(base.Close);
		m_btnAction.PointerClick.RemoveAllListeners();
		m_btnAction.PointerClick.AddListener(OnAction);
		NKCUtil.SetHotkey(m_btnAction, HotkeyEventType.Confirm);
		m_contents?.Init();
	}

	public void Open(USE_ITEM_TYPE useItemType, int useItemId, NKCUISlot.SlotData slotData, OnButton onButton = null)
	{
		if (slotData == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		switch (useItemType)
		{
		case USE_ITEM_TYPE.Common:
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_DP_POPUP_USE_COUNT_TOP_TEXT_MISC"));
			break;
		case USE_ITEM_TYPE.DailyTicket:
		{
			string itemName = NKMItemManager.GetItemMiscTempletByID(slotData.ID).GetItemName();
			NKCUtil.SetLabelText(m_lbTitle, string.Format(NKCStringTable.GetString("SI_DP_POPUP_USE_COUNT_TOP_TEXT_MISC_DAILY_TICKET"), itemName));
			break;
		}
		}
		m_slotData = slotData;
		m_contents.SetData(useItemType, useItemId, m_slotData);
		dOnActionButton = onButton;
		UIOpened();
	}

	private void OnAction()
	{
		Close();
		dOnActionButton?.Invoke(m_slotData.ID, (int)m_contents.m_useCount);
	}
}
