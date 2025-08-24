using System;
using System.Collections.Generic;
using UnityEngine;

namespace NKC.UI;

public class NKCUITacticUpdateLevelSlot : MonoBehaviour
{
	[Serializable]
	public struct LEVELUP_OBJECT
	{
		public GameObject objOFF;

		public GameObject objON;

		public GameObject objActive;
	}

	public GameObject m_objLevelMax;

	public GameObject m_objFinishFX;

	public List<LEVELUP_OBJECT> m_lstLevelSlot;

	public void SetLevel(int setLevel, bool bShowActive = false)
	{
		if (setLevel != 6)
		{
			int num = 0;
			foreach (LEVELUP_OBJECT item in m_lstLevelSlot)
			{
				if (!bShowActive)
				{
					NKCUtil.SetGameobjectActive(item.objActive, bValue: false);
					NKCUtil.SetGameobjectActive(item.objOFF, num >= setLevel);
					NKCUtil.SetGameobjectActive(item.objON, num < setLevel);
				}
				else
				{
					NKCUtil.SetGameobjectActive(item.objActive, num == setLevel);
					NKCUtil.SetGameobjectActive(item.objON, num < setLevel);
					NKCUtil.SetGameobjectActive(item.objOFF, num > setLevel);
				}
				num++;
			}
		}
		else
		{
			foreach (LEVELUP_OBJECT item2 in m_lstLevelSlot)
			{
				NKCUtil.SetGameobjectActive(item2.objActive, bValue: false);
				NKCUtil.SetGameobjectActive(item2.objOFF, bValue: false);
				NKCUtil.SetGameobjectActive(item2.objON, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objFinishFX, bShowActive && setLevel == 6);
		NKCUtil.SetGameobjectActive(m_objLevelMax, setLevel == 6);
	}

	public void SetActiveLevel(int iCurLevel, int iActiveLevel)
	{
		for (int i = 0; i < m_lstLevelSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstLevelSlot[i].objON, i < iCurLevel);
			NKCUtil.SetGameobjectActive(m_lstLevelSlot[i].objActive, i >= iCurLevel && i < iActiveLevel);
			NKCUtil.SetGameobjectActive(m_lstLevelSlot[i].objOFF, i >= iActiveLevel);
		}
	}
}
