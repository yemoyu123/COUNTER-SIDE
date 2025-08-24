using System.Collections.Generic;
using NKC.UI.Collection;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;

namespace NKC.UI;

public class NKCUIUnitSelectListSlotAssist : MonoBehaviour
{
	public delegate void dOnClick(long uid);

	public NKCUIUnitSelectListSlot m_UnitSlot;

	public GameObject m_objUnitPower;

	public NKCComTMPUIText m_ComUnitPower;

	public GameObject m_objEmpty;

	public GameObject m_objSelected;

	public NKCUIComStateButton m_csbtnSlot;

	private UnityAction m_btnCallBack;

	private long m_SupportUserUID;

	private dOnClick m_dClick;

	private List<NKMEquipItemData> m_reserveEquipItemData = new List<NKMEquipItemData>();

	public long SupportUserUID => m_SupportUserUID;

	public void Init(UnityAction callBack = null)
	{
		m_UnitSlot.Init();
		m_UnitSlot.SetEmpty(bEnableLayoutElement: true, OnSlotSelected);
		NKCUtil.SetBindFunction(m_csbtnSlot, OnClickBtn);
		m_btnCallBack = callBack;
	}

	public void SetData(NKMUnitData unitData, long supportUserUID = 0L)
	{
		if (unitData == null)
		{
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSelected, bValue: false);
			return;
		}
		m_SupportUserUID = supportUserUID;
		m_reserveEquipItemData.Clear();
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSelected, bValue: false);
		m_UnitSlot.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		m_UnitSlot.SetData(unitData, NKMDeckIndex.None, bEnableLayoutElement: false, OnSlotSelected);
		NKCUtil.SetLabelText(m_ComUnitPower, unitData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData).ToString("N0"));
	}

	public void SetData(NKMUnitData unitData, NKMEquipmentSet equipSet, long userUID, dOnClick onClick)
	{
		if (unitData == null)
		{
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSelected, bValue: false);
			return;
		}
		m_dClick = onClick;
		m_SupportUserUID = userUID;
		m_reserveEquipItemData.Clear();
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSelected, bValue: false);
		m_UnitSlot.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		m_UnitSlot.SetDataForDummyUnit(unitData, NKMDeckIndex.None, bEnableLayoutElement: false, OnSlotSelected);
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(unitData.m_UnitUID);
		if (unitFromUID != null)
		{
			m_UnitSlot.SetTouchHoldEvent(OpenUnitInfo);
		}
		else
		{
			m_UnitSlot.SetTouchHoldEvent(OpenUnitCollcetionInfo);
		}
		NKCUtil.SetLabelText(m_ComUnitPower, unitData.CalculateUnitOperationPower(equipSet).ToString("N0"));
		SetEquipData(equipSet);
	}

	public void SetEquipData(NKMEquipmentSet equipSet)
	{
		m_UnitSlot.SetEquipData(equipSet);
		m_reserveEquipItemData.Clear();
		m_reserveEquipItemData.Add(equipSet.Weapon);
		m_reserveEquipItemData.Add(equipSet.Defence);
		m_reserveEquipItemData.Add(equipSet.Accessory);
		m_reserveEquipItemData.Add(equipSet.Accessory2);
	}

	public void SetCalculateOperatorPower(int operatorPower)
	{
		NKCUtil.SetLabelText(m_ComUnitPower, operatorPower.ToString("N0"));
	}

	private void OpenUnitInfo(NKMUnitData unitData)
	{
		if (unitData != null)
		{
			NKCScenManager.GetScenManager().GET_NKC_SCEN_UNIT_LIST().SetOpenReserve(NKC_SCEN_UNIT_LIST.eUIOpenReserve.UnitInfo, unitData.m_UnitUID, bForce: true);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_UNIT_LIST, bForce: false);
		}
	}

	private void OpenUnitCollcetionInfo(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return;
		}
		List<NKMEquipItemData> list = new List<NKMEquipItemData>();
		if (m_reserveEquipItemData != null && m_reserveEquipItemData.Count > 0)
		{
			list = m_reserveEquipItemData;
		}
		else
		{
			NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
			if (inventoryData != null)
			{
				list.Add(inventoryData.GetItemEquip(unitData.GetEquipItemWeaponUid()));
				list.Add(inventoryData.GetItemEquip(unitData.GetEquipItemDefenceUid()));
				list.Add(inventoryData.GetItemEquip(unitData.GetEquipItemAccessoryUid()));
				list.Add(inventoryData.GetItemEquip(unitData.GetEquipItemAccessory2Uid()));
			}
		}
		NKCUICollectionUnitInfoV2.CheckInstanceAndOpen(unitData, null, list, NKCUICollectionUnitInfoV2.eCollectionState.CS_STATUS, isGauntlet: false, NKCUIUpsideMenu.eMode.BackButtonOnly, bWillCloseUnderPopupOnOpen: true, bDummyUnit: true);
	}

	private void OnSlotSelected(NKMUnitData selectedUnit, NKMUnitTempletBase unitTempletBase, NKMDeckIndex selectedUnitDeckIndex, NKCUnitSortSystem.eUnitState unitSlotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		if (selectedUnit != null)
		{
			m_dClick?.Invoke(m_SupportUserUID);
		}
	}

	private void OnClickBtn()
	{
		if (m_btnCallBack != null)
		{
			m_btnCallBack();
		}
		else
		{
			m_dClick?.Invoke(m_SupportUserUID);
		}
	}

	public void SetSelect(bool bSelected)
	{
		NKCUtil.SetGameobjectActive(m_objSelected, bSelected);
	}
}
