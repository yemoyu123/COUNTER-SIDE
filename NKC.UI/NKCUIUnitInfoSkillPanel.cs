using System.Collections.Generic;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitInfoSkillPanel : MonoBehaviour
{
	[Header("왼쪽 버튼")]
	public List<Text> m_lstSkillTypeName;

	public List<NKCUISkillSlot> m_lstSkillSlot;

	public GameObject m_goHyperSkillEffect;

	[Header("도감 리뉴얼 스킬 오브젝트")]
	public List<TMP_Text> m_lstSkillTypeNameTMPro;

	public List<GameObject> m_lstSkillSlotRoot;

	private bool m_bOpenPopupWhenSelected;

	private string m_unitName;

	private int m_unitStarGradeMax;

	private int m_unitLimitBreakLevel;

	private NKMUnitTempletBase m_CurUnitTempletBase;

	private NKMUnitData m_CurUnitData;

	public void SetOpenPopupWhenSelected()
	{
		m_bOpenPopupWhenSelected = true;
	}

	public void Init()
	{
		foreach (NKCUISkillSlot item in m_lstSkillSlot)
		{
			item.Init(SelectSkill);
		}
	}

	public void OpenSkillInfoPopup()
	{
		NKCPopupSkillFullInfo.UnitInstance.OpenForUnit(m_CurUnitData, m_unitName, m_unitStarGradeMax, m_unitLimitBreakLevel, m_CurUnitTempletBase.IsRearmUnit);
	}

	public void SetData(NKMUnitData unitData, bool bDisplayEmptySlot = false)
	{
		NKMUnitTempletBase nKMUnitTempletBase = (m_CurUnitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID));
		m_unitName = nKMUnitTempletBase.GetUnitName();
		m_unitStarGradeMax = nKMUnitTempletBase.m_StarGradeMax;
		m_unitLimitBreakLevel = unitData.m_LimitBreakLevel;
		List<NKMUnitSkillTemplet> unitAllSkillTempletList = NKMUnitSkillManager.GetUnitAllSkillTempletList(unitData);
		bool bValue = false;
		int num = 1;
		for (int i = 0; i < m_lstSkillSlot.Count; i++)
		{
			if (unitAllSkillTempletList.Count <= i)
			{
				continue;
			}
			bool value = NKMUnitSkillManager.IsLockedSkill(unitAllSkillTempletList[i].m_ID, unitData.m_LimitBreakLevel);
			NKMUnitSkillTemplet nKMUnitSkillTemplet = unitAllSkillTempletList[i];
			if (nKMUnitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_LEADER)
			{
				bValue = true;
				m_lstSkillSlot[0].SetData(nKMUnitSkillTemplet, bIsHyper: false);
				m_lstSkillSlot[0].LockSkill(value);
				NKCUtil.SetLabelText(m_lstSkillTypeName[0], NKCUtilString.GetSkillTypeName(nKMUnitSkillTemplet.m_NKM_SKILL_TYPE));
				if (m_lstSkillTypeNameTMPro != null && m_lstSkillTypeNameTMPro.Count > 0)
				{
					NKCUtil.SetLabelText(m_lstSkillTypeNameTMPro[0], NKCUtilString.GetSkillTypeName(nKMUnitSkillTemplet.m_NKM_SKILL_TYPE));
				}
				continue;
			}
			m_lstSkillSlot[num].SetData(nKMUnitSkillTemplet, nKMUnitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER);
			m_lstSkillSlot[num].LockSkill(value);
			NKCUtil.SetGameobjectActive(m_lstSkillSlot[num], bValue: true);
			NKCUtil.SetLabelText(m_lstSkillTypeName[num], NKCUtilString.GetSkillTypeName(nKMUnitSkillTemplet.m_NKM_SKILL_TYPE));
			if (m_lstSkillTypeNameTMPro != null && m_lstSkillTypeNameTMPro.Count > num)
			{
				NKCUtil.SetLabelText(m_lstSkillTypeNameTMPro[num], NKCUtilString.GetSkillTypeName(nKMUnitSkillTemplet.m_NKM_SKILL_TYPE));
			}
			if (m_lstSkillSlotRoot != null && m_lstSkillSlotRoot.Count > num)
			{
				NKCUtil.SetGameobjectActive(m_lstSkillSlotRoot[num], bValue: true);
			}
			num++;
		}
		for (int j = num; j < m_lstSkillSlot.Count; j++)
		{
			NKCUtil.SetGameobjectActive(m_lstSkillSlot[j], bValue: false);
			if (m_lstSkillSlotRoot != null && m_lstSkillSlotRoot.Count > j)
			{
				NKCUtil.SetGameobjectActive(m_lstSkillSlotRoot[j], bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_lstSkillSlot[0], bValue);
		NKCUtil.SetGameobjectActive(m_lstSkillTypeName[0], bValue);
		if (m_lstSkillSlotRoot != null && m_lstSkillSlotRoot.Count > 0)
		{
			NKCUtil.SetGameobjectActive(m_lstSkillSlotRoot[0], bValue);
		}
		m_CurUnitData = unitData;
	}

	public void SelectSkill(NKMUnitSkillTemplet skillTemplet)
	{
		if (skillTemplet != null && m_bOpenPopupWhenSelected)
		{
			OpenSkillInfoPopup();
		}
	}
}
