using UnityEngine;

namespace NKC.UI.Shop;

public class NKCPopupShopCustomPackageSubstitudeSlot : MonoBehaviour
{
	public NKCUISlot m_slotBefore;

	public NKCUISlot m_slotAfter;

	public void Init()
	{
		m_slotBefore?.Init();
		m_slotAfter?.Init();
	}

	public void SetData(NKCUISlot.SlotData before, NKCUISlot.SlotData after)
	{
		if (m_slotBefore != null)
		{
			m_slotBefore.SetData(before);
			m_slotBefore.SetOnClickAction(default(NKCUISlot.SlotClickType));
		}
		if (m_slotAfter != null)
		{
			m_slotAfter.SetData(after);
			m_slotAfter.SetOnClickAction(default(NKCUISlot.SlotClickType));
		}
	}
}
