using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKCUIOperatorTacticalSkillCombo : MonoBehaviour
{
	public List<NKCGameHudComboSlot> m_lstComboSlot;

	public void SetData(NKMOperator operatorData)
	{
		SetData(operatorData.id);
	}

	public void SetData(int operatorID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorID);
		if (unitTempletBase == null || unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			return;
		}
		NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(unitTempletBase.m_lstSkillStrID[0]);
		if (skillTemplet == null || skillTemplet.m_OperSkillType != OperatorSkillType.m_Tactical)
		{
			return;
		}
		NKMTacticalCommandTemplet tacticalCommandTempletByStrID = NKMTacticalCommandManager.GetTacticalCommandTempletByStrID(skillTemplet.m_OperSkillTarget);
		if (tacticalCommandTempletByStrID.m_listComboType == null || tacticalCommandTempletByStrID.m_listComboType.Count <= 0)
		{
			return;
		}
		List<NKMTacticalCombo> listComboType = tacticalCommandTempletByStrID.m_listComboType;
		for (int i = 0; i < m_lstComboSlot.Count; i++)
		{
			if (listComboType.Count <= i)
			{
				NKCUtil.SetGameobjectActive(m_lstComboSlot[i].gameObject, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(m_lstComboSlot[i].gameObject, bValue: true);
			m_lstComboSlot[i].SetUI(listComboType[i], i < listComboType.Count - 1);
		}
	}
}
