using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeUpgradeSlot : MonoBehaviour
{
	public delegate void OnClickUpgradeSlot(NKCUIForgeUpgradeSlot slot, NKC_EQUIP_UPGRADE_STATE state);

	public NKCUIComStateButton m_btn;

	public NKCUISlotEquip m_slotEquip;

	public Text m_lbEquipName;

	public Text m_lbEquipType;

	public GameObject m_objUpgradable;

	public GameObject m_objNeedEnhance;

	public GameObject m_objNotHave;

	public GameObject m_objSelected;

	private NKC_EQUIP_UPGRADE_STATE m_EquipUpgradeState;

	private NKMItemEquipUpgradeTemplet m_UpgradeTemplet;

	public void SetData(NKMItemEquipUpgradeTemplet upgradeTemplet, OnClickUpgradeSlot onClickUpgardeSlot)
	{
		m_UpgradeTemplet = upgradeTemplet;
		NKCUtil.SetGameobjectActive(m_objSelected, bValue: false);
		NKMEquipTemplet upgradeEquipTemplet = upgradeTemplet.UpgradeEquipTemplet;
		if (upgradeEquipTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetLabelText(m_lbEquipName, upgradeEquipTemplet.GetItemName());
		if (NKMUnitManager.GetUnitTempletBase(upgradeEquipTemplet.GetPrivateUnitID()) != null)
		{
			NKCUtil.SetLabelText(m_lbEquipType, NKCUtilString.GetEquipPositionStringByUnitStyle(upgradeEquipTemplet));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEquipType, NKCUtilString.GetEquipPositionStringByUnitStyle(upgradeEquipTemplet, skipPrivateUnit: true));
		}
		m_slotEquip.SetData(NKCEquipSortSystem.MakeTempEquipData(upgradeEquipTemplet.m_ItemEquipID));
		List<NKMEquipItemData> lstCoreEquipData = new List<NKMEquipItemData>();
		m_EquipUpgradeState = NKMItemManager.GetSetUpgradeSlotState(upgradeTemplet, ref lstCoreEquipData);
		SetUpgradeSlotState(m_EquipUpgradeState);
		m_btn.PointerClick.RemoveAllListeners();
		m_btn.PointerClick.AddListener(delegate
		{
			onClickUpgardeSlot(this, m_EquipUpgradeState);
		});
	}

	public void SetSelected(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objSelected, bValue);
	}

	public void SetUpgradeSlotState(NKC_EQUIP_UPGRADE_STATE state)
	{
		NKCUtil.SetGameobjectActive(m_objUpgradable, state == NKC_EQUIP_UPGRADE_STATE.UPGRADABLE);
		NKCUtil.SetGameobjectActive(m_objNeedEnhance, state == NKC_EQUIP_UPGRADE_STATE.NEED_ENHANCE || state == NKC_EQUIP_UPGRADE_STATE.NEED_PRECISION);
		NKCUtil.SetGameobjectActive(m_objNotHave, state == NKC_EQUIP_UPGRADE_STATE.NOT_HAVE);
	}

	public NKMItemEquipUpgradeTemplet GetUpgradeTemplet()
	{
		return m_UpgradeTemplet;
	}

	public NKC_EQUIP_UPGRADE_STATE GetUpgradeState()
	{
		return m_EquipUpgradeState;
	}
}
