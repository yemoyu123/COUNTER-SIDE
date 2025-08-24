using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupSelectionConfirmEquip : MonoBehaviour
{
	public NKCUIInvenEquipSlot m_slot;

	public void SetData(int equipID, int setItemID, List<NKM_STAT_TYPE> lstStatType, List<int> lstPotentialOptionKeys)
	{
		if (NKMItemManager.GetEquipTemplet(equipID) == null)
		{
			return;
		}
		NKMEquipItemData nKMEquipItemData = NKCEquipSortSystem.MakeTempEquipData(equipID, setItemID);
		if (lstPotentialOptionKeys.Count > 0)
		{
			nKMEquipItemData.potentialOptions = new List<NKMPotentialOption>();
			for (int i = 0; i < lstPotentialOptionKeys.Count; i++)
			{
				if (lstPotentialOptionKeys[i] > 0)
				{
					NKMPotentialOptionTemplet nKMPotentialOptionTemplet = NKMPotentialOptionTemplet.Find(lstPotentialOptionKeys[i]);
					if (nKMPotentialOptionTemplet != null)
					{
						NKMPotentialOption nKMPotentialOption = new NKMPotentialOption();
						nKMPotentialOption.statType = nKMPotentialOptionTemplet.StatType;
						nKMEquipItemData.potentialOptions.Add(nKMPotentialOption);
					}
				}
			}
		}
		if (nKMEquipItemData.m_Stat != null && nKMEquipItemData.m_Stat.Count > 2 && lstStatType.Count > 1)
		{
			nKMEquipItemData.m_Stat[1].type = lstStatType[0];
			nKMEquipItemData.m_Stat[2].type = lstStatType[1];
		}
		m_slot.SetData(nKMEquipItemData);
	}
}
