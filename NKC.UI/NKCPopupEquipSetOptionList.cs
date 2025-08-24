using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEquipSetOptionList : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_FACTORY";

	public const string UI_ASSET_NAME = "NKM_UI_FACTORY_EQUIP_SET_LIST_POPUP";

	public Text m_NKM_UI_FACTORY_EQUIP_SET_LIST_POPUP_TOP_TEXT;

	public Text m_lbDesc;

	public Text m_lbRateDesc;

	public RectTransform m_NKM_UI_FACTORY_EQUIP_SET_LIST_POPUP_Content;

	[Header("버튼들")]
	public NKCUIComStateButton m_NKM_UI_POPUP_OK_BOX_OK;

	public NKCUIComStateButton NKM_UI_FACTORY_EQUIP_SET_LIST_POPUP_CANCEL_BUTTON;

	[Header("슬롯")]
	public NKCPopupEquipSetOptionListSlot m_pbfNKCPopupEquipSetOptionListSlot;

	private List<GameObject> m_lstSlots = new List<GameObject>();

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "세트 옵션 목록";

	public void InitUI()
	{
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_OK_BOX_OK, base.Close);
		NKCUtil.SetHotkey(m_NKM_UI_POPUP_OK_BOX_OK, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(NKM_UI_FACTORY_EQUIP_SET_LIST_POPUP_CANCEL_BUTTON, base.Close);
	}

	public void Open(long equipUID, string desc)
	{
		NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(equipUID);
		if (equipUID != 0L && itemEquip != null)
		{
			Open(itemEquip, desc);
		}
	}

	public void Open(NKMEquipItemData equipData, string desc)
	{
		NKCUtil.SetGameobjectActive(m_lbDesc, !string.IsNullOrEmpty(desc));
		NKCUtil.SetLabelText(m_lbDesc, desc);
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipData.m_ItemEquipID);
		if (equipTemplet != null && equipTemplet.SetGroupList != null && equipTemplet.SetGroupList.Count > 0)
		{
			m_NKM_UI_FACTORY_EQUIP_SET_LIST_POPUP_Content.anchoredPosition = new Vector2(m_NKM_UI_FACTORY_EQUIP_SET_LIST_POPUP_Content.anchoredPosition.x, 0f);
			List<int> list = new List<int>();
			list.AddRange(equipTemplet.SetGroupList);
			list.Sort();
			float num = 100f / (float)(equipTemplet.SetGroupList.Count - 1);
			NKCUtil.SetLabelText(m_lbRateDesc, string.Format(NKCUtilString.GET_STRING_EQUIP_SET_RATE_INFO, num));
			NKCUtil.SetGameobjectActive(m_lbRateDesc.gameObject, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.OPEN_TAG_RATE_INFO));
			SetData(list);
			UIOpened();
		}
	}

	private void SetData(IReadOnlyList<int> lstSetOption)
	{
		foreach (int item in lstSetOption)
		{
			NKCPopupEquipSetOptionListSlot nKCPopupEquipSetOptionListSlot = Object.Instantiate(m_pbfNKCPopupEquipSetOptionListSlot);
			if (nKCPopupEquipSetOptionListSlot != null)
			{
				nKCPopupEquipSetOptionListSlot.SetData(item);
				RectTransform component = nKCPopupEquipSetOptionListSlot.GetComponent<RectTransform>();
				if (component != null)
				{
					component.localScale = Vector2.one;
					component.SetParent(m_NKM_UI_FACTORY_EQUIP_SET_LIST_POPUP_Content);
				}
				m_lstSlots.Add(nKCPopupEquipSetOptionListSlot.gameObject);
			}
		}
	}

	public override void CloseInternal()
	{
		foreach (GameObject lstSlot in m_lstSlots)
		{
			NKCUtil.SetGameobjectActive(lstSlot.gameObject, bValue: false);
			Object.Destroy(lstSlot);
		}
		m_lstSlots.Clear();
		base.gameObject.SetActive(value: false);
	}
}
