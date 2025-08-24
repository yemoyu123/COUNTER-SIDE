using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Tooltip;

public class NKCUITooltipItem : NKCUITooltipBase
{
	public NKCUISlot m_slot;

	public Text m_type;

	public Text m_name;

	public Text m_amount;

	public Text m_lbPrivateEquip;

	public override void Init()
	{
		m_slot.Init();
	}

	public override void SetData(NKCUITooltip.Data data)
	{
		if (!(data is NKCUITooltip.ItemData itemData))
		{
			Debug.LogError("Tooltip Item Data is null");
			return;
		}
		m_slot.SetData(itemData.Slot, bShowName: false, bShowNumber: false, bEnableLayoutElement: false, null);
		m_type.text = NKCUtilString.GetSlotModeTypeString(itemData.Slot.eType, itemData.Slot.ID);
		m_name.text = NKCUISlot.GetName(itemData.Slot.eType, itemData.Slot.ID);
		if (ShowAmount(itemData.Slot.eType, itemData.Slot.ID, out var strAmount))
		{
			m_amount.text = strAmount;
		}
		else
		{
			m_amount.text = "";
		}
		if (itemData.Slot.eType == NKCUISlot.eSlotMode.Equip || itemData.Slot.eType == NKCUISlot.eSlotMode.EquipCount)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemData.Slot.ID);
			if (equipTemplet != null)
			{
				NKCUtil.SetGameobjectActive(m_lbPrivateEquip, bValue: true);
				NKCUtil.SetLabelText(m_lbPrivateEquip, NKCUtilString.GetEquipPositionStringByUnitStyle(equipTemplet));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lbPrivateEquip, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbPrivateEquip, bValue: false);
		}
	}

	private bool ShowAmount(NKCUISlot.eSlotMode type, int id, out string strAmount)
	{
		strAmount = "";
		long num = 0L;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		NKMInventoryData inventoryData = myUserData.m_InventoryData;
		if (inventoryData == null)
		{
			return false;
		}
		switch (type)
		{
		case NKCUISlot.eSlotMode.ItemMisc:
		{
			NKMItemMiscTemplet itemMiscTempletByID2 = NKMItemManager.GetItemMiscTempletByID(id);
			if (itemMiscTempletByID2 == null)
			{
				return false;
			}
			switch (itemMiscTempletByID2.m_ItemMiscType)
			{
			case NKM_ITEM_MISC_TYPE.IMT_INTERIOR:
				num = NKCScenManager.CurrentUserData().OfficeData.GetInteriorCount(id);
				strAmount = string.Format(NKCUtilString.GET_STRING_TOOLTIP_QUANTITY_ONE_PARAM, num);
				return true;
			default:
				num = inventoryData.GetCountMiscItem(id);
				strAmount = string.Format(NKCUtilString.GET_STRING_TOOLTIP_QUANTITY_ONE_PARAM, num);
				return true;
			case NKM_ITEM_MISC_TYPE.IMT_VIEW:
				break;
			}
			break;
		}
		case NKCUISlot.eSlotMode.Mold:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(id);
			if (itemMoldTempletByID == null)
			{
				return false;
			}
			if (!NKMItemManager.m_dicRandomMoldBox.TryGetValue(itemMoldTempletByID.m_RewardGroupID, out var value))
			{
				break;
			}
			if (value == null || value.Count != 1)
			{
				return false;
			}
			foreach (KeyValuePair<NKM_REWARD_TYPE, List<int>> item in value)
			{
				if (item.Value == null || item.Value.Count != 1)
				{
					return false;
				}
				int num2 = item.Value[0];
				NKCRandomMoldBoxTemplet randomMoldBoxTemplet = NKMItemManager.GetRandomMoldBoxTemplet(num2);
				if (randomMoldBoxTemplet == null)
				{
					return false;
				}
				if (randomMoldBoxTemplet.m_reward_type != NKM_REWARD_TYPE.RT_MISC)
				{
					return false;
				}
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(num2);
				if (itemMiscTempletByID == null)
				{
					return false;
				}
				if (itemMiscTempletByID.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_VIEW)
				{
					num = inventoryData.GetCountMiscItem(num2);
					strAmount = itemMiscTempletByID.GetItemName() + " " + string.Format(NKCUtilString.GET_STRING_TOOLTIP_QUANTITY_ONE_PARAM, num);
					return true;
				}
			}
			break;
		}
		}
		return false;
	}
}
