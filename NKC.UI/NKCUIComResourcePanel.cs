using System;
using ClientPacket.Office;
using NKC.UI.Tooltip;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComResourcePanel : MonoBehaviour, NKCUIManager.IInventoryChangeObserver
{
	public enum ResourceType
	{
		MISC,
		INTERIOR,
		EQUIP
	}

	[Serializable]
	public struct ResourceInfo
	{
		public delegate void OnSetSprite(int itemId, ref Image targetUI);

		public delegate void OnUpdateCount(int itemId, ref Text targetUI, ref NKCUISlot.SlotData slotData);

		public ResourceType resourceType;

		public int itemId;

		public Image iconImageUI;

		public Text countTextUI;

		public NKCUIComStateButton csbtnButton;

		public NKCUISlot.SlotData slotData;

		public OnSetSprite onSetSprite;

		public OnUpdateCount onUpdateCount;

		public void SetSprite()
		{
			if (onSetSprite != null)
			{
				onSetSprite(itemId, ref iconImageUI);
			}
		}

		public void UpdateCount()
		{
			if (onUpdateCount != null)
			{
				onUpdateCount(itemId, ref countTextUI, ref slotData);
			}
		}

		public void Release()
		{
			iconImageUI = null;
			countTextUI = null;
			csbtnButton = null;
			slotData = null;
			onSetSprite = null;
			onUpdateCount = null;
		}
	}

	public ResourceInfo[] resourceInfo;

	public bool showToolTip;

	public int Handle { get; set; }

	public void OnInventoryChange(NKMItemMiscData itemData)
	{
		RefreshCount(ResourceType.MISC, itemData.ItemID);
	}

	public void OnInteriorInventoryUpdate(NKMInteriorData interiorData, bool bAdded)
	{
		RefreshCount(ResourceType.INTERIOR, interiorData.itemId);
	}

	public void OnEquipChange(NKMUserData.eChangeNotifyType eType, long equipUID, NKMEquipItemData equipData)
	{
		RefreshCount(ResourceType.EQUIP, equipData.m_ItemEquipID);
	}

	private void Awake()
	{
		if (resourceInfo == null)
		{
			return;
		}
		int num = resourceInfo.Length;
		for (int i = 0; i < num; i++)
		{
			switch (resourceInfo[i].resourceType)
			{
			case ResourceType.MISC:
				resourceInfo[i].onSetSprite = SetSpriteMiscType;
				resourceInfo[i].onUpdateCount = UpdateCountMiscType;
				break;
			case ResourceType.INTERIOR:
				resourceInfo[i].onSetSprite = SetSpriteMiscType;
				resourceInfo[i].onUpdateCount = UpdateCountInteriorType;
				break;
			case ResourceType.EQUIP:
				resourceInfo[i].onSetSprite = SetSpriteEquipType;
				resourceInfo[i].onUpdateCount = UpdateCountEquipType;
				break;
			}
			resourceInfo[i].SetSprite();
			resourceInfo[i].UpdateCount();
			resourceInfo[i].csbtnButton?.PointerDown.RemoveAllListeners();
			if (showToolTip)
			{
				int index = i;
				resourceInfo[i].csbtnButton?.PointerDown.AddListener(delegate(PointerEventData eventData)
				{
					OnSlotDown(resourceInfo[index].slotData, eventData);
				});
			}
		}
	}

	private void SetSpriteMiscType(int itemId, ref Image targetUI)
	{
		Sprite orLoadMiscItemIcon = NKCResourceUtility.GetOrLoadMiscItemIcon(NKMItemMiscTemplet.Find(itemId));
		NKCUtil.SetImageSprite(targetUI, orLoadMiscItemIcon);
	}

	private void SetSpriteEquipType(int itemId, ref Image targetUI)
	{
		Sprite orLoadEquipIcon = NKCResourceUtility.GetOrLoadEquipIcon(NKMItemManager.GetEquipTemplet(itemId));
		NKCUtil.SetImageSprite(targetUI, orLoadEquipIcon);
	}

	private void UpdateCountMiscType(int itemId, ref Text targetUI, ref NKCUISlot.SlotData slotData)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(itemId);
			NKCUtil.SetLabelText(targetUI, countMiscItem.ToString());
			if (slotData == null)
			{
				slotData = NKCUISlot.SlotData.MakeMiscItemData(itemId, countMiscItem);
			}
		}
	}

	private void UpdateCountInteriorType(int itemId, ref Text targetUI, ref NKCUISlot.SlotData slotData)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			long interiorCount = nKMUserData.OfficeData.GetInteriorCount(itemId);
			NKCUtil.SetLabelText(targetUI, interiorCount.ToString());
			if (slotData == null)
			{
				slotData = NKCUISlot.SlotData.MakeMiscItemData(itemId, interiorCount);
			}
		}
	}

	private void UpdateCountEquipType(int itemId, ref Text targetUI, ref NKCUISlot.SlotData slotData)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			long num = nKMUserData.m_InventoryData.GetSameKindEquipCount(itemId);
			NKCUtil.SetLabelText(targetUI, num.ToString());
			if (slotData == null)
			{
				slotData = NKCUISlot.SlotData.MakeEquipData(itemId, 1);
			}
		}
	}

	private void RefreshCount(ResourceType resourceType, int itemId)
	{
		if (resourceInfo == null)
		{
			return;
		}
		int num = resourceInfo.Length;
		for (int i = 0; i < num; i++)
		{
			if (resourceInfo[i].resourceType == resourceType && resourceInfo[i].itemId == itemId)
			{
				resourceInfo[i].UpdateCount();
			}
		}
	}

	private void RefreshTotalCount()
	{
		if (resourceInfo != null)
		{
			int num = resourceInfo.Length;
			for (int i = 0; i < num; i++)
			{
				resourceInfo[i].UpdateCount();
			}
		}
	}

	private void OnSlotDown(NKCUISlot.SlotData slotData, PointerEventData eventData)
	{
		NKCUITooltip.Instance.Open(slotData, eventData.position);
	}

	private void OnEnable()
	{
		NKCUIManager.RegisterInventoryObserver(this);
		RefreshTotalCount();
	}

	private void OnDisable()
	{
		NKCUIManager.UnregisterInventoryObserver(this);
	}

	private void OnDestroy()
	{
		int num = resourceInfo.Length;
		for (int i = 0; i < num; i++)
		{
			resourceInfo[i].Release();
		}
	}
}
