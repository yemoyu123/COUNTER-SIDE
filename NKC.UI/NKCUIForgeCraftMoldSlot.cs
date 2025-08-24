using System;
using System.Collections.Generic;
using NKC.UI.Component;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeCraftMoldSlot : MonoBehaviour
{
	public delegate void OnClickMoldSlot(NKMMoldItemData cNKMMoldItemData);

	public NKCUISlot m_AB_ICON_SLOT;

	public Text m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_NAME;

	public Text m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_COUNT;

	public Text m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_TIME;

	public Text m_lbLimit;

	public NKCUIItemCostSlot m_firstCostSlot;

	public List<NKCUIItemCostSlot> m_lstCostSlot;

	public GameObject m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BLACK;

	public NKCUIComButton m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BUTTON;

	public Image m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BUTTON_img;

	public Text m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BUTTON_text;

	private OnClickMoldSlot m_dOnClickMoldSlot;

	private NKMMoldItemData m_cNKMMoldItemData;

	public GameObject m_objStackCount;

	public NKCUIComStateButton m_csbtnStack;

	public NKCComTMPUIText m_lbStackCount;

	private const string IMPOSSIBLE_TEXT_COLOR = "#222222";

	private NKCAssetInstanceData m_InstanceData;

	public int MoldID => m_cNKMMoldItemData?.m_MoldID ?? 0;

	public static NKCUIForgeCraftMoldSlot GetNewInstance(Transform parent, OnClickMoldSlot dOnClickMoldSlot = null)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_FACTORY", "NKM_UI_FACTORY_CRAFT_MOLD_SLOT");
		if (nKCAssetInstanceData == null || nKCAssetInstanceData.m_Instant == null)
		{
			Debug.LogError("NKCUIForgeCraftMoldSlot Prefab null!");
			return null;
		}
		NKCUIForgeCraftMoldSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIForgeCraftMoldSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIForgeCraftMoldSlot null!");
			return null;
		}
		component.m_dOnClickMoldSlot = dOnClickMoldSlot;
		component.transform.SetParent(parent, worldPositionStays: false);
		component.m_InstanceData = nKCAssetInstanceData;
		component.m_AB_ICON_SLOT.Init();
		component.m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BUTTON.PointerClick.RemoveAllListeners();
		component.m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BUTTON.PointerClick.AddListener(component.OnClickCraftMoldSlot);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void OnClickCraftMoldSlot()
	{
		if (!(m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BUTTON_text.color == NKCUtil.GetColor("#222222")) && m_dOnClickMoldSlot != null)
		{
			m_dOnClickMoldSlot(m_cNKMMoldItemData);
		}
	}

	public void SetData(int index)
	{
		if (!NKCUIForgeCraftMold.HasInstance)
		{
			return;
		}
		NKMMoldItemData cNKMMoldItemData = NKCUIForgeCraftMold.Instance.GetSortedMoldItemData(index);
		m_cNKMMoldItemData = cNKMMoldItemData;
		bool flag = true;
		if (cNKMMoldItemData == null)
		{
			return;
		}
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(cNKMMoldItemData.m_MoldID);
		if (itemMoldTempletByID == null)
		{
			Debug.LogError($"Mold templet not found : id {cNKMMoldItemData.m_MoldID}");
			return;
		}
		m_AB_ICON_SLOT.SetData(NKCUISlot.SlotData.MakeMoldItemData(cNKMMoldItemData.m_MoldID, cNKMMoldItemData.m_Count), bShowName: false, bShowNumber: false, bEnableLayoutElement: true, null);
		if (itemMoldTempletByID.IsEquipMold)
		{
			m_AB_ICON_SLOT.SetOnClickAction(NKCUISlot.SlotClickType.MoldList);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BLACK, cNKMMoldItemData.m_Count <= 0 && !itemMoldTempletByID.m_bPermanent);
		}
		else
		{
			m_AB_ICON_SLOT.SetOnClickAction(default(NKCUISlot.SlotClickType));
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BLACK, bValue: false);
		}
		if (cNKMMoldItemData.m_Count == 0L && !itemMoldTempletByID.m_bPermanent)
		{
			flag = false;
		}
		if (itemMoldTempletByID.m_bPermanent)
		{
			m_AB_ICON_SLOT.SetHaveCount(0L);
		}
		else
		{
			m_AB_ICON_SLOT.SetHaveCount(cNKMMoldItemData.m_Count);
		}
		m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_NAME.text = itemMoldTempletByID.GetItemName();
		TimeSpan timeSpan = new TimeSpan(itemMoldTempletByID.m_Time / 60, itemMoldTempletByID.m_Time % 60, 0);
		m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_TIME.text = NKCUtilString.GetTimeSpanString(timeSpan);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			if (itemMoldTempletByID.m_MaterialList.Count > 0)
			{
				NKCUtil.SetGameobjectActive(m_firstCostSlot, bValue: true);
				NKMItemMoldMaterialData nKMItemMoldMaterialData = itemMoldTempletByID.m_MaterialList[0];
				long materialCount = GetMaterialCount(nKMUserData, nKMItemMoldMaterialData.m_MaterialType, nKMItemMoldMaterialData.m_MaterialID);
				int credit = nKMItemMoldMaterialData.m_MaterialValue;
				bool bShowEvent = nKMItemMoldMaterialData.m_MaterialID == 1 && NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT);
				if (nKMItemMoldMaterialData.m_MaterialID == 1)
				{
					NKCCompanyBuff.SetDiscountOfCreditInCraft(nKMUserData.m_companyBuffDataList, ref credit);
				}
				m_firstCostSlot.SetData(nKMItemMoldMaterialData.m_MaterialID, credit, materialCount, bShowTooltip: false, bShowBG: false, bShowEvent);
				if (materialCount < credit)
				{
					flag = false;
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_firstCostSlot, bValue: false);
			}
			for (int i = 0; i < m_lstCostSlot.Count; i++)
			{
				int num = i + 1;
				if (num >= itemMoldTempletByID.m_MaterialList.Count)
				{
					m_lstCostSlot[i].SetData(0, 0, 0L);
					continue;
				}
				long materialCount2 = GetMaterialCount(nKMUserData, itemMoldTempletByID.m_MaterialList[num].m_MaterialType, itemMoldTempletByID.m_MaterialList[num].m_MaterialID);
				int credit2 = itemMoldTempletByID.m_MaterialList[num].m_MaterialValue;
				bool bShowEvent2 = itemMoldTempletByID.m_MaterialList[num].m_MaterialID == 1 && NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT);
				if (itemMoldTempletByID.m_MaterialList[num].m_MaterialID == 1)
				{
					NKCCompanyBuff.SetDiscountOfCreditInCraft(nKMUserData.m_companyBuffDataList, ref credit2);
				}
				m_lstCostSlot[i].SetData(itemMoldTempletByID.m_MaterialList[num].m_MaterialID, credit2, materialCount2, bShowTooltip: true, bShowBG: true, bShowEvent2);
				if (materialCount2 < credit2)
				{
					flag = false;
				}
			}
		}
		NKMResetCounterGroupTemplet nKMResetCounterGroupTemplet = NKMResetCounterGroupTemplet.Find(itemMoldTempletByID.m_ResetGroupId);
		if (nKMResetCounterGroupTemplet != null && nKMResetCounterGroupTemplet.IsValid())
		{
			NKCUtil.SetGameobjectActive(m_lbLimit, bValue: true);
			NKCUtil.SetLabelText(m_lbLimit, NKCUtilString.GetMoldResetCount(itemMoldTempletByID));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbLimit, bValue: false);
		}
		int num2 = 0;
		if (NKMItemManager.IsStackMoldItem(itemMoldTempletByID))
		{
			NKCUtil.SetGameobjectActive(m_lbLimit, bValue: false);
			num2 = NKMItemManager.GetRemainResetCountStack(itemMoldTempletByID);
			m_csbtnStack.PointerDown.AddListener(delegate(PointerEventData eventData)
			{
				OnSlotDown(cNKMMoldItemData.m_MoldID, eventData);
			});
		}
		if (num2 > 9999)
		{
			NKCUtil.SetLabelText(m_lbStackCount, "9999+");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbStackCount, num2.ToString());
		}
		NKCUtil.SetGameobjectActive(m_objStackCount, NKMItemManager.IsStackMoldItem(itemMoldTempletByID));
		if (NKMItemManager.IsStackMoldItem(itemMoldTempletByID))
		{
			if (num2 == 0)
			{
				flag = false;
			}
		}
		else if (NKMItemManager.GetRemainResetCount(itemMoldTempletByID.m_ResetGroupId) == 0)
		{
			flag = false;
		}
		if (flag)
		{
			m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BUTTON_img.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_POPUP_BLUEBUTTON");
			m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BUTTON_text.color = NKCUtil.GetColor("#FFFFFF");
		}
		else
		{
			m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BUTTON_img.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_POPUP_BUTTON_02");
			m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BUTTON_text.color = NKCUtil.GetColor("#222222");
		}
	}

	private void OnSlotDown(int moldID, PointerEventData eventData)
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(moldID);
		if (itemMoldTempletByID != null)
		{
			string text = "";
			switch (itemMoldTempletByID.m_StackType)
			{
			case COUNT_RESET_TYPE.DAY:
				text = string.Format(NKCStringTable.GetString("SI_PF_FACTORY_CRAFT_MOLD_STACK_DAY"), itemMoldTempletByID.m_StackCount);
				break;
			case COUNT_RESET_TYPE.WEEK:
				text = string.Format(NKCStringTable.GetString("SI_PF_FACTORY_CRAFT_MOLD_STACK_WEEK"), itemMoldTempletByID.m_StackCount);
				break;
			case COUNT_RESET_TYPE.MONTH:
				text = string.Format(NKCStringTable.GetString("SI_PF_FACTORY_CRAFT_MOLD_STACK_MONTH"), itemMoldTempletByID.m_StackCount);
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				NKCUITooltip.TextData textData = new NKCUITooltip.TextData(text);
				NKCUITooltip.Instance.Open(textData, eventData.position);
			}
		}
	}

	public long GetMaterialCount(NKMUserData cNKMUserData, NKM_REWARD_TYPE type, int id)
	{
		if (cNKMUserData == null)
		{
			return 0L;
		}
		if (type == NKM_REWARD_TYPE.RT_MISC)
		{
			return cNKMUserData.m_InventoryData.GetCountMiscItem(id);
		}
		Debug.Log("not supported material");
		return 0L;
	}

	public RectTransform GetButtonRect()
	{
		return m_NKM_UI_FACTORY_CRAFT_MOLD_SLOT_BUTTON.GetComponent<RectTransform>();
	}
}
