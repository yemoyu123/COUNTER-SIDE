using NKC.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIUnitRecommendEquipListSlot : MonoBehaviour
{
	public NKCUISlot m_slotWeapon;

	public NKCUISlot m_slotArmor;

	public NKCUISlot m_slotAcc1;

	public NKCUISlot m_slotAcc2;

	public void Init()
	{
		m_slotWeapon.Init();
		m_slotArmor.Init();
		m_slotAcc1.Init();
		m_slotAcc2.Init();
	}

	public void SetData(NKCEquipRecommendListTemplet templet)
	{
		m_slotWeapon.SetData(NKCUISlot.SlotData.MakeEquipData(templet.m_WeaponSlot, 0));
		m_slotWeapon.SetOnClickAction(NKCUISlot.SlotClickType.ItemBox);
		m_slotArmor.SetData(NKCUISlot.SlotData.MakeEquipData(templet.m_DefenceSlot, 0));
		m_slotArmor.SetOnClickAction(NKCUISlot.SlotClickType.ItemBox);
		m_slotAcc1.SetData(NKCUISlot.SlotData.MakeEquipData(templet.m_ACCSlot_1, 0));
		m_slotAcc1.SetOnClickAction(NKCUISlot.SlotClickType.ItemBox);
		m_slotAcc2.SetData(NKCUISlot.SlotData.MakeEquipData(templet.m_ACCSlot_2, 0));
		m_slotAcc2.SetOnClickAction(NKCUISlot.SlotClickType.ItemBox);
	}

	public void OnClickSlot(NKCUISlot.SlotData slotData, bool bLock)
	{
		NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData);
	}
}
