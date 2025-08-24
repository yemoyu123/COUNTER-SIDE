using System;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperatorSelectListSlot : NKCUIUnitSelectListSlotBase
{
	[Serializable]
	public struct SkillInfo
	{
		public GameObject m_Object;

		public GameObject m_Max;

		public NKCUIOperatorSkill m_SkillInfo;
	}

	public NKCAssetInstanceData m_Instance;

	[Header("오퍼레이터")]
	public NKCUIComButton m_NKM_UI_OPERATOR_CARD_SLOT;

	public GameObject m_NKM_UI_OPERATOR_CARD_LEVEL_BG;

	public Image m_NKM_UI_OPERATOR_CARD_LEVEL_GAUGE;

	public GameObject m_objContractGainUnit;

	public GameObject m_NKM_UI_OPERATOR_CARD_SKILL;

	public List<SkillInfo> m_lstSkill;

	public Text m_NKM_UI_OPERATOR_CARD_TITLE_TEXT;

	[Header("격전 지원")]
	public Text m_NKM_UI_UNIT_SELECT_LIST_FIERCE_BATTLE_TEXT;

	[Header("보조 스킬")]
	public GameObject m_ObjPassiveSkill;

	public Image m_ImgPassiveSkill;

	public Text m_txtPassiveSkillName;

	[Header("도감")]
	public GameObject m_NKM_UI_OPERATOR_CARD_SLOT_COLLECTION_EMPLOYEE;

	public Text m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EMPLOYEE_TEXT;

	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_DISABLE;

	private NKMOperator m_CurOperator;

	public static NKCUIOperatorSelectListSlot GetNewInstance(Transform parent = null)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_operator_card", "NKM_UI_OPERATOR_CARD_SLOT");
		NKCUIOperatorSelectListSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIOperatorSelectListSlot>();
		if (component == null)
		{
			Debug.LogError("NKM_UI_OPERATOR_CARD_SLOT Prefab null!");
			return null;
		}
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.GetComponent<RectTransform>().localScale = Vector3.one;
			component.Init();
		}
		component.m_Instance = nKCAssetInstanceData;
		component.gameObject.SetActive(value: false);
		return component;
	}

	public override void SetData(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, int officeID = 0)
	{
	}

	public override void SetData(NKMOperator operatorData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisOperatorSlot onSelectThisSlot)
	{
		base.SetData(operatorData, deckIndex, bEnableLayoutElement, onSelectThisSlot);
		if (operatorData == null)
		{
			return;
		}
		m_CurOperator = operatorData;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_CurOperator.id);
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelText(m_NKM_UI_OPERATOR_CARD_TITLE_TEXT, unitTempletBase.GetUnitTitle());
		}
		for (int i = 0; i < m_lstSkill.Count; i++)
		{
			int num = 1;
			if (operatorData == null)
			{
				NKCUtil.SetGameobjectActive(m_lstSkill[i].m_Object, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(m_lstSkill[i].m_Object, bValue: true);
			NKMOperatorSkillTemplet nKMOperatorSkillTemplet = null;
			if (i == 0)
			{
				num = m_CurOperator.mainSkill.level;
				nKMOperatorSkillTemplet = NKCOperatorUtil.GetSkillTemplet(m_CurOperator.mainSkill.id);
				m_lstSkill[i].m_SkillInfo.SetData(m_CurOperator.mainSkill.id, m_CurOperator.mainSkill.level);
			}
			else
			{
				num = m_CurOperator.subSkill.level;
				nKMOperatorSkillTemplet = NKCOperatorUtil.GetSkillTemplet(m_CurOperator.subSkill.id);
				m_lstSkill[i].m_SkillInfo.SetData(m_CurOperator.subSkill.id, m_CurOperator.subSkill.level);
			}
			if (nKMOperatorSkillTemplet != null && nKMOperatorSkillTemplet.m_MaxSkillLevel == num)
			{
				NKCUtil.SetGameobjectActive(m_lstSkill[i].m_Max, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstSkill[i].m_Max, bValue: false);
			}
		}
		if (m_NKM_UI_OPERATOR_CARD_LEVEL_GAUGE != null)
		{
			if (NKMCommonConst.OperatorConstTemplet.unitMaximumLevel == m_CurOperator.level)
			{
				m_NKM_UI_OPERATOR_CARD_LEVEL_GAUGE.fillAmount = 1f;
			}
			else
			{
				m_NKM_UI_OPERATOR_CARD_LEVEL_GAUGE.fillAmount = NKCExpManager.GetOperatorNextLevelExpProgress(m_CurOperator);
			}
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_CARD_LEVEL_BG, bValue: true);
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_DISABLE, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_CARD_SKILL, bValue: true);
	}

	private void Init()
	{
		if (m_NKM_UI_OPERATOR_CARD_SLOT != null)
		{
			m_NKM_UI_OPERATOR_CARD_SLOT.PointerClick.RemoveAllListeners();
			m_NKM_UI_OPERATOR_CARD_SLOT.PointerClick.AddListener(OnClick);
		}
	}

	public void Clear()
	{
		if (m_Instance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_Instance);
		}
	}

	public override void SetDataForCollection(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, OnSelectThisSlot onSelectThisSlot, bool bEnable = false)
	{
	}

	public override void SetDataForCollection(NKMOperator operatorData, NKMDeckIndex deckIndex, OnSelectThisOperatorSlot onSelectThisSlot, bool bEnable = false)
	{
		base.SetData(operatorData, deckIndex, bEnableLayoutElement: true, onSelectThisSlot);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelText(m_NKM_UI_OPERATOR_CARD_TITLE_TEXT, unitTempletBase.GetUnitTitle());
		}
		NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(operatorData.id);
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_CARD_SLOT_COLLECTION_EMPLOYEE, unitTemplet != null && !unitTemplet.m_bExclude);
		NKCUtil.SetLabelText(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EMPLOYEE_TEXT, NKCCollectionManager.GetEmployeeNumber(operatorData.id));
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_DISABLE, !bEnable);
	}

	public override void SetDataForContractSelection(NKMOperator cNKMOperData)
	{
		base.SetData(cNKMOperData, NKMDeckIndex.None, bEnableLayoutElement: true, null);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMOperData.id);
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelText(m_NKM_UI_OPERATOR_CARD_TITLE_TEXT, unitTempletBase.GetUnitTitle());
		}
		NKCUtil.SetLabelText(m_lbLevel, 1.ToString());
		NKCUtil.SetGameobjectActive(m_objMaxExp, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_CARD_LEVEL_BG, bValue: false);
		if (m_NKM_UI_OPERATOR_CARD_LEVEL_GAUGE != null)
		{
			m_NKM_UI_OPERATOR_CARD_LEVEL_GAUGE.fillAmount = 0f;
		}
		foreach (SkillInfo item in m_lstSkill)
		{
			NKCUtil.SetGameobjectActive(item.m_Object, bValue: false);
		}
	}

	public override void SetData(NKMUnitTempletBase templetBase, int levelToDisplay, int skinID, bool bEnableLayoutElement, OnSelectThisOperatorSlot onSelectThisSlot)
	{
		base.SetData(templetBase, levelToDisplay, skinID, bEnableLayoutElement, onSelectThisSlot);
		if (templetBase != null)
		{
			NKCUtil.SetLabelText(m_NKM_UI_OPERATOR_CARD_TITLE_TEXT, templetBase.GetUnitTitle());
		}
		m_CurOperator = null;
		for (int i = 0; i < m_lstSkill.Count; i++)
		{
			int num = 1;
			if (m_CurOperator == null)
			{
				NKCUtil.SetGameobjectActive(m_lstSkill[i].m_Object, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(m_lstSkill[i].m_Object, bValue: true);
			NKMOperatorSkillTemplet nKMOperatorSkillTemplet = null;
			if (i == 0)
			{
				num = m_CurOperator.mainSkill.level;
				nKMOperatorSkillTemplet = NKCOperatorUtil.GetSkillTemplet(m_CurOperator.mainSkill.id);
				m_lstSkill[i].m_SkillInfo.SetData(m_CurOperator.mainSkill.id, m_CurOperator.mainSkill.level);
			}
			else
			{
				num = m_CurOperator.subSkill.level;
				nKMOperatorSkillTemplet = NKCOperatorUtil.GetSkillTemplet(m_CurOperator.subSkill.id);
				m_lstSkill[i].m_SkillInfo.SetData(m_CurOperator.subSkill.id, m_CurOperator.subSkill.level);
			}
			if (nKMOperatorSkillTemplet != null && nKMOperatorSkillTemplet.m_MaxSkillLevel == num)
			{
				NKCUtil.SetGameobjectActive(m_lstSkill[i].m_Max, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstSkill[i].m_Max, bValue: false);
			}
		}
		if (m_NKM_UI_OPERATOR_CARD_LEVEL_GAUGE != null)
		{
			m_NKM_UI_OPERATOR_CARD_LEVEL_GAUGE.fillAmount = 0f;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_CARD_LEVEL_BG, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_DISABLE, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
	}

	public override void SetDataForRearm(NKMUnitData unitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, bool bShowEqup = true, bool bShowLevel = false, bool bUnable = false)
	{
	}

	public override void SetDataForBan(NKMOperator operData, bool bEnableLayoutElement, OnSelectThisOperatorSlot onSelectThisSlot)
	{
		SetData(operData, NKMDeckIndex.None, bEnableLayoutElement, onSelectThisSlot);
		NKCUtil.SetGameobjectActive(m_objContractGainUnit, bValue: false);
		ProcessBanUI();
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_CARD_SKILL, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL, bValue: false);
		SetCityLeaderMark(value: false);
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
	}

	private void ProcessBanUI()
	{
		if (m_NKMUnitTempletBase != null)
		{
			if (m_bEnableShowBan && NKCBanManager.IsBanOperator(m_NKMUnitTempletBase.m_UnitID, m_eBanDataType))
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
				int operBanLevel = NKCBanManager.GetOperBanLevel(m_NKMUnitTempletBase.m_UnitID, m_eBanDataType);
				NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, operBanLevel));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
			}
		}
	}

	protected override void OnClick()
	{
		if (dOnSelectThisOperatorSlot != null)
		{
			dOnSelectThisOperatorSlot(m_OperatorData, m_NKMUnitTempletBase, m_DeckIndex, m_eUnitSlotState, m_eUnitSelectState);
		}
	}

	public override void SetLock(bool bLocked, bool bBig = false)
	{
		base.SetLock(bLocked, bBig);
		SetPassiveSkillInfo(bLocked && bBig);
	}

	public override void SetDelete(bool bSet)
	{
		base.SetDelete(bSet);
		SetPassiveSkillInfo(bSet);
	}

	public override void SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState eUnitSelectState)
	{
		base.SetSlotSelectState(eUnitSelectState);
		SetPassiveSkillInfo(eUnitSelectState == NKCUIUnitSelectList.eUnitSlotSelectState.DELETE);
	}

	private void SetPassiveSkillInfo(bool bActive)
	{
		if (bActive && m_CurOperator != null)
		{
			NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(m_CurOperator.subSkill.id);
			if (skillTemplet != null)
			{
				NKCUtil.SetImageSprite(m_ImgPassiveSkill, NKCUtil.GetSkillIconSprite(skillTemplet));
				NKCUtil.SetLabelText(m_txtPassiveSkillName, NKCStringTable.GetString(skillTemplet.m_OperSkillNameStrID));
			}
		}
		NKCUtil.SetGameobjectActive(m_ObjPassiveSkill, bActive);
	}

	public override void SetContractedUnitMark(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objContractGainUnit, value);
	}
}
