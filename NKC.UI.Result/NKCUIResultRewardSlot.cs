using NKM;
using UnityEngine;

namespace NKC.UI.Result;

public class NKCUIResultRewardSlot : MonoBehaviour
{
	public NKCUISlot m_NKCUISlot;

	public GameObject m_objEffect;

	private int idx;

	public int GetIdx()
	{
		return idx;
	}

	public void Init()
	{
		m_NKCUISlot.Init();
	}

	public void SetData(NKCUISlot.SlotData data, int index)
	{
		idx = index;
		m_NKCUISlot.SetData(data, bEnableLayoutElement: true, OnClickItem);
		m_NKCUISlot.SetBonusRate(data.BonusRate);
	}

	public void SetEffectEnable(bool bEnable)
	{
		NKCUtil.SetGameobjectActive(m_objEffect, bEnable);
	}

	public void OnClickItem(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (slotData == null)
		{
			return;
		}
		switch (slotData.eType)
		{
		default:
			NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData, null, singleOpenOnly: false, bShowCount: false, showDropInfo: false);
			break;
		case NKCUISlot.eSlotMode.Equip:
		case NKCUISlot.eSlotMode.EquipCount:
		{
			NKMEquipItemData nKMEquipItemData = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(slotData.UID);
			if (nKMEquipItemData == null)
			{
				nKMEquipItemData = NKCEquipSortSystem.MakeTempEquipData(slotData.ID, slotData.GroupID);
			}
			NKCPopupItemEquipBox.Open(nKMEquipItemData, NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
			break;
		}
		case NKCUISlot.eSlotMode.Skin:
			Debug.LogWarning("Skin Popup under construction");
			break;
		}
	}

	public void SetFirstRewardMark(bool bEnable)
	{
		m_NKCUISlot.SetFirstGetMark(bEnable);
	}

	public void SetOnetimeRewardMark(bool bEnable)
	{
		m_NKCUISlot.SetOnetimeMark(bEnable);
	}

	public void SetFirstAllClearRewardMark(bool bEnable)
	{
		m_NKCUISlot.SetFirstAllClearMark(bEnable);
	}

	public void SetSelect(bool bSelect)
	{
		m_NKCUISlot.SetSelected(bSelect);
	}

	public void SetText(string text)
	{
		m_NKCUISlot.SetAdditionalText(text);
	}

	private void OnDisable()
	{
		SetEffectEnable(bEnable: false);
	}
}
