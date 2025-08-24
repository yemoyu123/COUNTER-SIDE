using System.Collections.Generic;
using Cs.Logging;
using NKC.UI.Result;
using NKC.UI.Tooltip;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIItemCostSlot : MonoBehaviour
{
	public Image m_BG;

	public Image m_ICON;

	public Text m_COUNT;

	public GameObject m_REQUIRED;

	public NKCUIComStateButton m_AB_ICON_COST_SLOT;

	public GameObject m_objEvent;

	public bool m_ShowPopup;

	public bool m_bShowReqCount = true;

	private NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE m_EQUIP_BOX_BOTTOM_MENU_TYPE = NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_ENFORCE_AND_EQUIP;

	private const int DISPLAY_LIMIT_VALUE = 100000;

	private NKCUISlot.SlotData slotData;

	public int ItemID { get; private set; }

	public void Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE type)
	{
		m_EQUIP_BOX_BOTTOM_MENU_TYPE = type;
	}

	public void SetData(int itemID, int ReqCnt, long CurCnt, bool bShowTooltip = true, bool bShowBG = true, bool bShowEvent = false)
	{
		ItemID = itemID;
		NKCUtil.SetGameobjectActive(m_ICON.gameObject, itemID != 0);
		NKCUtil.SetGameobjectActive(m_COUNT.gameObject, itemID != 0 && (ReqCnt > 0 || CurCnt > 0));
		NKCUtil.SetGameobjectActive(m_REQUIRED, itemID != 0 && ReqCnt > CurCnt);
		NKCUtil.SetGameobjectActive(m_BG, bShowBG);
		NKCUtil.SetGameobjectActive(m_objEvent, bShowEvent);
		if (itemID == 0)
		{
			if (bShowBG)
			{
				Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_inven_icon_common", "AB_INVEN_ICON_COSTBG_EMPTY");
				if (orLoadAssetResource != null)
				{
					NKCUtil.SetImageSprite(m_BG, orLoadAssetResource);
				}
			}
			m_AB_ICON_COST_SLOT.PointerClick.RemoveAllListeners();
			slotData = null;
			return;
		}
		if (bShowBG)
		{
			Sprite orLoadAssetResource2 = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_inven_icon_common", "AB_INVEN_ICON_COSTBG");
			if (orLoadAssetResource2 != null)
			{
				NKCUtil.SetImageSprite(m_BG, orLoadAssetResource2);
			}
		}
		Sprite orLoadMiscItemIcon = NKCResourceUtility.GetOrLoadMiscItemIcon(NKMItemManager.GetItemMiscTempletByID(itemID));
		NKCUtil.SetImageSprite(m_ICON, orLoadMiscItemIcon);
		SetCount(ReqCnt, CurCnt);
		slotData = NKCUISlot.SlotData.MakeMiscItemData(itemID, ReqCnt);
		if (bShowTooltip)
		{
			if (!m_ShowPopup)
			{
				m_AB_ICON_COST_SLOT.PointerDown.RemoveAllListeners();
				m_AB_ICON_COST_SLOT.PointerDown.AddListener(OnClicked);
			}
			else
			{
				m_AB_ICON_COST_SLOT.PointerClick.RemoveAllListeners();
				m_AB_ICON_COST_SLOT.PointerClick.AddListener(OnClickedPopUpSlot);
			}
		}
	}

	public void SetCount(int ReqCnt, long CurCnt)
	{
		string msg;
		switch (ItemID)
		{
		case 1:
			msg = ((ReqCnt <= CurCnt) ? $"{ReqCnt:#,###}" : $"<color=#ff0000ff>{ReqCnt:#,###}</color>");
			break;
		case 2:
		case 3:
		case 101:
			msg = ((ReqCnt <= CurCnt) ? $"{ReqCnt}" : $"<color=#ff0000ff>{ReqCnt}</color>");
			break;
		default:
			msg = ((!m_bShowReqCount) ? ((ReqCnt <= CurCnt) ? $"{ReqCnt}" : $"<color=#ff0000ff>{ReqCnt}</color>") : ((ReqCnt <= CurCnt) ? (((int)CurCnt <= 100000) ? $"{CurCnt}/{ReqCnt}" : $"*/{ReqCnt}") : $"<color=#ff0000ff>{CurCnt}</color>/{ReqCnt}"));
			break;
		}
		NKCUtil.SetLabelText(m_COUNT, msg);
	}

	private void OpenItemBox()
	{
		if (slotData == null)
		{
			return;
		}
		switch (slotData.eType)
		{
		default:
			NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData);
			break;
		case NKCUISlot.eSlotMode.Unit:
		case NKCUISlot.eSlotMode.UnitCount:
		case NKCUISlot.eSlotMode.Emoticon:
			NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData);
			break;
		case NKCUISlot.eSlotMode.Equip:
			OpenEquipBox(slotData);
			break;
		case NKCUISlot.eSlotMode.Skin:
			Debug.LogWarning("Skin Popup under construction");
			break;
		case NKCUISlot.eSlotMode.Mold:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(slotData.ID);
			if (itemMoldTempletByID != null && itemMoldTempletByID.IsEquipMold && NKMItemManager.m_dicRandomMoldBox.ContainsKey(itemMoldTempletByID.m_RewardGroupID))
			{
				Dictionary<NKM_REWARD_TYPE, List<int>> dictionary = NKMItemManager.m_dicRandomMoldBox[itemMoldTempletByID.m_RewardGroupID];
				if (dictionary != null && dictionary.Count > 0)
				{
					NKCUISlotListViewer.GetNewInstance().OpenEquipMoldRewardList(dictionary, itemMoldTempletByID.GetItemName(), NKCUtilString.GET_STRING_FORGE_CRAFT_MOLD_DESC);
				}
			}
			break;
		}
		}
	}

	private void OpenEquipBox(NKCUISlot.SlotData slotData)
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(slotData.UID);
		if (itemEquip == null)
		{
			itemEquip = NKCEquipSortSystem.MakeTempEquipData(slotData.ID, slotData.GroupID);
			NKCPopupItemEquipBox.Open(itemEquip, NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
		else if (NKCUIWarfareResult.IsInstanceOpen)
		{
			NKCPopupItemEquipBox.Open(itemEquip, NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
		else if (itemEquip.m_OwnerUnitUID > 0)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet == null)
			{
				return;
			}
			NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(itemEquip.m_OwnerUnitUID);
			if (unitFromUID != null)
			{
				NKM_ERROR_CODE nKM_ERROR_CODE = equipTemplet.CanUnEquipByUnit(NKCScenManager.GetScenManager().GetMyUserData(), unitFromUID);
				if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
				{
					NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE.ToString());
				}
				else
				{
					NKCPopupItemEquipBox.Open(itemEquip, m_EQUIP_BOX_BOTTOM_MENU_TYPE);
				}
			}
		}
		else
		{
			NKCPopupItemEquipBox.Open(itemEquip, m_EQUIP_BOX_BOTTOM_MENU_TYPE);
		}
	}

	public void OnClicked(PointerEventData eventData)
	{
		if (slotData != null)
		{
			NKCUITooltip.Instance.Open(slotData, eventData.position);
		}
	}

	public void OnClickedPopUpSlot()
	{
		if (slotData != null)
		{
			OpenItemBox();
		}
	}

	public static void SetDataList(IList<NKCUIItemCostSlot> slots, IReadOnlyCollection<MiscItemUnit> items, bool bShowTooltip = true, bool bShowBG = true, bool bShowEvent = false)
	{
		int i = 0;
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		foreach (MiscItemUnit item in items)
		{
			if (i < slots.Count)
			{
				NKCUIItemCostSlot nKCUIItemCostSlot = slots[i];
				NKCUtil.SetGameobjectActive(nKCUIItemCostSlot, bValue: true);
				nKCUIItemCostSlot.SetData(item.ItemId, item.Count32, inventoryData.GetCountMiscItem(item.ItemId), bShowTooltip, bShowBG, bShowEvent);
				i++;
				continue;
			}
			Log.Error("Not enough slot count for data List!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIItemCostSlot.cs", 263);
			return;
		}
		for (; i < slots.Count; i++)
		{
			NKCUtil.SetGameobjectActive(slots[i], bValue: false);
		}
	}
}
