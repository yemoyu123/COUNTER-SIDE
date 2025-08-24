using System;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperatorDeckSelectSlot : NKCUIUnitSelectListSlotBase
{
	[Serializable]
	public struct Skill
	{
		public GameObject m_Obj;

		public GameObject m_MAX;

		public GameObject m_ENHANCE;

		public GameObject m_IMPLANT;

		public NKCUIOperatorSkill m_SkillInfo;
	}

	private NKCAssetInstanceData m_InstanceData;

	public List<Skill> m_lstSkill;

	public Image m_ImgBGGradation;

	public GameObject m_objSkillCombo;

	public NKCUIOperatorTacticalSkillCombo m_TacticalSkillCombo;

	private long m_curOperatorUID;

	protected override NKCResourceUtility.eUnitResourceType UseResourceType => NKCResourceUtility.eUnitResourceType.INVEN_ICON;

	public long OperatorUID => m_curOperatorUID;

	public static NKCUIOperatorDeckSelectSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_operator_deck", "NKM_UI_OPERATOR_DECK_SELECT_SLOT");
		NKCUIOperatorDeckSelectSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIOperatorDeckSelectSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIOperatorDeckSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.GetComponent<RectTransform>().localScale = Vector3.one;
			component.Init();
		}
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public override void SetData(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, int officeID = 0)
	{
		base.SetData(cNKMUnitData, deckIndex, bEnableLayoutElement, onSelectThisSlot);
		if (cNKMUnitData != null)
		{
			NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(cNKMUnitData.m_UnitUID);
			if (operatorData != null)
			{
				UpdateSkillEnhanceUI(operatorData);
			}
			UpdateBackgroundGradationByGrade(cNKMUnitData.m_UnitID);
		}
	}

	public override void SetEmpty(bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, OnSelectThisOperatorSlot onSelectThisOperatorSlot = null)
	{
		base.SetEmpty(bEnableLayoutElement, onSelectThisSlot, onSelectThisOperatorSlot);
		NKCUtil.SetGameobjectActive(m_objSkillCombo, bValue: false);
	}

	public override void SetData(NKMOperator operatorData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisOperatorSlot onSelectThisSlot)
	{
		base.SetData(operatorData, deckIndex, bEnableLayoutElement, onSelectThisSlot);
		ProcessBanUIForOperator();
		if (operatorData != null)
		{
			UpdateSkillEnhanceUI(operatorData);
			UpdateBackgroundGradationByGrade(operatorData.id);
			m_TacticalSkillCombo.SetData(operatorData.id);
		}
		NKCUtil.SetGameobjectActive(m_objSkillCombo, operatorData != null);
	}

	protected override void OnClick()
	{
		if (dOnSelectThisOperatorSlot != null)
		{
			dOnSelectThisOperatorSlot(m_OperatorData, m_NKMUnitTempletBase, m_DeckIndex, m_eUnitSlotState, m_eUnitSelectState);
		}
	}

	public override void SetSlotState(NKCUnitSortSystem.eUnitState eUnitSlotState)
	{
		base.SetSlotState(eUnitSlotState);
		if (eUnitSlotState == NKCUnitSortSystem.eUnitState.DUPLICATE)
		{
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: true);
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_DECK_UNIT_STATE_DUPLICATE);
		}
	}

	public void SetData(NKMOperator mainOperator, NKMOperator curOperatorData, OnSelectThisOperatorSlot onSelectThisSlot)
	{
		m_curOperatorUID = 0L;
		base.SetData(curOperatorData, NKMDeckIndex.None, bEnableLayoutElement: true, onSelectThisSlot);
		NKCUtil.SetGameobjectActive(m_objSkillCombo, bValue: false);
		if (curOperatorData == null)
		{
			return;
		}
		m_curOperatorUID = curOperatorData.uid;
		NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(m_curOperatorUID);
		if (operatorData != null)
		{
			bool bEnhanceMain = NKCOperatorUtil.IsCanEnhanceMainSkill(mainOperator, curOperatorData);
			bool flag = NKCOperatorUtil.IsCanEnhanceSubSkill(mainOperator, curOperatorData);
			bool bImplantSub = false;
			if (!flag)
			{
				NKMOperator operatorData2 = NKCOperatorUtil.GetOperatorData(mainOperator.uid);
				if (operatorData2 != null)
				{
					bImplantSub = operatorData2.subSkill.id != operatorData.subSkill.id;
				}
			}
			UpdateSkillEnhanceUI(operatorData, bEnhanceMain, flag, bImplantSub);
		}
		UpdateBackgroundGradationByGrade(curOperatorData.id);
	}

	private void UpdateSkillEnhanceUI(NKMOperator operatorData, bool bEnhanceMain = false, bool bEnhanceSub = false, bool bImplantSub = false)
	{
		for (int i = 0; i < m_lstSkill.Count; i++)
		{
			int num = 0;
			bool flag = false;
			if (i == 0)
			{
				num = operatorData.mainSkill.level;
				flag = NKCOperatorUtil.IsMaximumSkillLevel(operatorData.mainSkill.id, num);
				NKCUtil.SetGameobjectActive(m_lstSkill[i].m_ENHANCE, bEnhanceMain);
				NKCUtil.SetGameobjectActive(m_lstSkill[i].m_IMPLANT, bValue: false);
				m_lstSkill[i].m_SkillInfo.SetData(operatorData.mainSkill.id, operatorData.mainSkill.level);
			}
			else
			{
				num = operatorData.subSkill.level;
				flag = NKCOperatorUtil.IsMaximumSkillLevel(operatorData.subSkill.id, num);
				NKCUtil.SetGameobjectActive(m_lstSkill[i].m_ENHANCE, bEnhanceSub);
				NKCUtil.SetGameobjectActive(m_lstSkill[i].m_IMPLANT, bImplantSub);
				m_lstSkill[i].m_SkillInfo.SetData(operatorData.subSkill.id, operatorData.subSkill.level);
			}
			NKCUtil.SetGameobjectActive(m_lstSkill[i].m_Obj, bValue: true);
			NKCUtil.SetGameobjectActive(m_lstSkill[i].m_MAX, flag);
		}
	}

	private void UpdateBackgroundGradationByGrade(int unitID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		Color color = Color.clear;
		if (unitTempletBase != null)
		{
			if (unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR)
			{
				color = NKCUtil.GetColor("#FFAE14");
			}
			else if (unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SR)
			{
				color = NKCUtil.GetColor("#C414FF");
			}
			else if (unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_R)
			{
				color = NKCUtil.GetColor("#1366FF");
			}
			else if (unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_N)
			{
				color = NKCUtil.GetColor("#767676");
			}
		}
		NKCUtil.SetImageColor(m_ImgBGGradation, color);
	}
}
