using System.Collections.Generic;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionMiscItemSlot : MonoBehaviour
{
	public delegate void ClickSlot(int index, int itemId, RectTransform slotRT);

	public RectTransform m_rectTransform;

	public NKCUISlot m_iconSlot;

	public TMP_Text m_IdText;

	public Text m_itemName;

	public GameObject m_notGet;

	public GameObject m_selected;

	public GameObject m_redDot;

	public NKCUIComStateButton m_slotButton;

	private int m_index;

	private int m_itemId;

	private NKCAssetInstanceData m_InstanceData;

	private ClickSlot m_dOnClickSlot;

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_slotButton, OnClickSlot);
	}

	public void SetData(int index, int itemId, NKMCollectionV2MiscTemplet templet, NKCUICollectionGeneral.CollectionType collectionType, ClickSlot dOnClickSlot)
	{
		m_index = index;
		m_itemId = itemId;
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemId);
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(itemId, 1L);
		m_iconSlot?.SetData(data);
		NKCUtil.SetLabelText(m_IdText, itemId.ToString());
		NKCUtil.SetLabelText(m_itemName, (itemMiscTempletByID != null) ? itemMiscTempletByID.GetItemName() : "");
		m_dOnClickSlot = dOnClickSlot;
		bool flag = true;
		switch (collectionType)
		{
		case NKCUICollectionGeneral.CollectionType.CT_EMBLEM:
		{
			List<NKMItemMiscData> list = NKCScenManager.CurrentUserData()?.m_InventoryData.GetEmblemData();
			flag = list == null || list.Find((NKMItemMiscData e) => e.ItemID == itemId) == null;
			break;
		}
		case NKCUICollectionGeneral.CollectionType.CT_FRAME:
		case NKCUICollectionGeneral.CollectionType.CT_BACKGROUND:
		{
			NKMInventoryData nKMInventoryData = NKCScenManager.CurrentUserData()?.m_InventoryData;
			flag = nKMInventoryData == null || nKMInventoryData.GetCountMiscItem(itemId) <= 0;
			break;
		}
		}
		bool flag2 = false;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			flag2 = nKMUserData.m_InventoryData.GetMiscCollectionData(itemId)?.IsRewardComplete() ?? false;
		}
		if (templet != null && templet.DefaultCollection)
		{
			flag = false;
			flag2 = true;
		}
		NKCUtil.SetGameobjectActive(m_notGet, flag);
		NKCUtil.SetGameobjectActive(m_redDot, !flag && !flag2);
	}

	public void SetSelected(bool value)
	{
		NKCUtil.SetGameobjectActive(m_selected, value);
	}

	public static NKCUICollectionMiscItemSlot GetNewInstance(Transform parent, bool bMentoringSlot = false)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_collection", "AB_UI_COLLECTION_ICON_SLOT");
		NKCUICollectionMiscItemSlot nKCUICollectionMiscItemSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUICollectionMiscItemSlot>();
		if (nKCUICollectionMiscItemSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUICollectionMiscItemSlot Prefab null!");
			return null;
		}
		nKCUICollectionMiscItemSlot.m_InstanceData = nKCAssetInstanceData;
		nKCUICollectionMiscItemSlot.Init();
		if (parent != null)
		{
			nKCUICollectionMiscItemSlot.transform.SetParent(parent);
		}
		nKCUICollectionMiscItemSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUICollectionMiscItemSlot.gameObject.SetActive(value: false);
		return nKCUICollectionMiscItemSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	private void OnClickSlot()
	{
		if (m_dOnClickSlot != null)
		{
			m_dOnClickSlot(m_index, m_itemId, m_rectTransform);
		}
	}
}
