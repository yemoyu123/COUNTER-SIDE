using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEmblemChangeConfirm : NKCUIBase
{
	public delegate void dOnClickOK(int id);

	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_FRIEND";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_EMBLEM_CONFIRM";

	public Text m_lbTitle;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnCancel;

	public EventTrigger m_etBG;

	public NKCPopupEmblemBigSlot m_NKCPopupEmblemBigSlotSrc;

	public NKCPopupEmblemBigSlot m_NKCPopupEmblemBigSlotDest;

	public GameObject m_objArrow;

	private int m_DestEmblemID = -1;

	private dOnClickOK m_dOnClickOK;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupEmblemChangeConfirm";

	public void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetBindFunction(m_csbtnOK, OnClickOK);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_csbtnCancel, OnCloseBtn);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			OnCloseBtn();
		});
		m_etBG.triggers.Add(entry);
	}

	private void OnClickOK()
	{
		if (m_DestEmblemID != -1 && m_dOnClickOK != null)
		{
			m_dOnClickOK(m_DestEmblemID);
		}
		m_dOnClickOK = null;
		Close();
	}

	public void Open(int srcEmblemID, int destEmblemID, dOnClickOK _dOnClickOK = null)
	{
		NKCUtil.SetGameobjectActive(m_NKCPopupEmblemBigSlotDest, bValue: true);
		if (destEmblemID == 0)
		{
			NKCUtil.SetGameobjectActive(m_NKCPopupEmblemBigSlotSrc, bValue: true);
			NKCUtil.SetGameobjectActive(m_objArrow, bValue: true);
			m_NKCPopupEmblemBigSlotSrc.SetEmblemData(srcEmblemID);
			m_NKCPopupEmblemBigSlotDest.SetEmblemEmpty(NKCUtilString.GET_STRING_EMBLEM_EQUIPPED_EMBLEM_UNEQUIP);
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_EMBLEM_EQUIPPED_EMBLEM_UNEQUIP_CONFIRM);
		}
		else if (srcEmblemID == 0 || srcEmblemID == -1)
		{
			NKCUtil.SetGameobjectActive(m_NKCPopupEmblemBigSlotSrc, bValue: false);
			NKCUtil.SetGameobjectActive(m_objArrow, bValue: false);
			m_NKCPopupEmblemBigSlotDest.SetEmblemData(destEmblemID);
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_EMBLEM_EQUIP_CONFIRM);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKCPopupEmblemBigSlotSrc, bValue: true);
			NKCUtil.SetGameobjectActive(m_objArrow, bValue: true);
			m_NKCPopupEmblemBigSlotSrc.SetEmblemData(srcEmblemID);
			m_NKCPopupEmblemBigSlotDest.SetEmblemData(destEmblemID);
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_EMBLEM_EQUIPPED_EMBLEM_CHANGE_CONFIRM);
		}
		m_DestEmblemID = destEmblemID;
		m_dOnClickOK = _dOnClickOK;
		UIOpened();
	}

	public void CloseEmblemChangeConfirmPopup()
	{
		Close();
	}

	public void OnCloseBtn()
	{
		Close();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnDestroy()
	{
	}
}
