using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;

namespace NKC.UI;

public class NKCUIShipInfoSkillPanel : MonoBehaviour
{
	public List<NKCUIShipSkillSlot> m_lstSkillSlot;

	private UnityAction dOnCallSkillInfo;

	public void Init(UnityAction callback = null)
	{
		dOnCallSkillInfo = callback;
		foreach (NKCUIShipSkillSlot item in m_lstSkillSlot)
		{
			item.Init(OnSelectSkill);
		}
	}

	public void SetData(NKMUnitTempletBase cNKMShipTemplet)
	{
		if (cNKMShipTemplet != null)
		{
			int num = 0;
			for (int i = 0; i < cNKMShipTemplet.GetSkillCount(); i++)
			{
				NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(cNKMShipTemplet, i);
				if (shipSkillTempletByIndex != null && num < m_lstSkillSlot.Count && shipSkillTempletByIndex.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_PASSIVE)
				{
					NKCUtil.SetGameobjectActive(m_lstSkillSlot[num], bValue: true);
					m_lstSkillSlot[num].SetData(shipSkillTempletByIndex);
					num++;
				}
			}
			for (int j = 0; j < cNKMShipTemplet.GetSkillCount(); j++)
			{
				NKMShipSkillTemplet shipSkillTempletByIndex2 = NKMShipSkillManager.GetShipSkillTempletByIndex(cNKMShipTemplet, j);
				if (shipSkillTempletByIndex2 != null && num < m_lstSkillSlot.Count && shipSkillTempletByIndex2.m_NKM_SKILL_TYPE != NKM_SKILL_TYPE.NST_PASSIVE)
				{
					NKCUtil.SetGameobjectActive(m_lstSkillSlot[num], bValue: true);
					m_lstSkillSlot[num].SetData(shipSkillTempletByIndex2);
					num++;
				}
			}
			for (int k = num; k < m_lstSkillSlot.Count; k++)
			{
				NKCUtil.SetGameobjectActive(m_lstSkillSlot[k], bValue: false);
			}
			return;
		}
		foreach (NKCUIShipSkillSlot item in m_lstSkillSlot)
		{
			NKCUtil.SetGameobjectActive(item, bValue: false);
		}
	}

	public void OnSelectSkill()
	{
		dOnCallSkillInfo?.Invoke();
	}
}
