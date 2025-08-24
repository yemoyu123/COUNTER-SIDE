using System.Collections.Generic;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupSelectionConfirmUnit : MonoBehaviour
{
	public NKCUIUnitSelectListSlot m_slot;

	[Header("타입")]
	public GameObject m_objTypeRoot;

	public Image m_imgBattleType;

	public Text m_lbBattleType;

	public Image m_imgAttackType;

	public Text m_lbAttackType;

	[Header("스탯")]
	public GameObject m_objStatRoot;

	public Text m_lbHP;

	public Text m_lbAtt;

	public Text m_lbDef;

	public Text m_lbCrit;

	public Text m_lbHit;

	public Text m_lbEvd;

	[Header("스킬")]
	public GameObject m_objSkillRoot;

	public List<NKCUISkillSlot> m_lstSkillSlot = new List<NKCUISkillSlot>(4);

	public void SetData(int unitID, int unitLv = 1, short unitLimitBreakLv = 0, int _unitTacticLv = 0, int reactorLv = 0, int unitSkillLv = 1)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase == null)
		{
			return;
		}
		NKMUnitData item = NKMUnitManager.CreateUnitData(unitID, unitID, unitLv, unitLimitBreakLv, unitSkillLv, reactorLv, _unitTacticLv, 0, fromContract: false).unitData;
		m_slot.SetData(item, NKMDeckIndex.None, bEnableLayoutElement: false, null);
		m_slot.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		if (unitTempletBase.IsTrophy)
		{
			NKCUtil.SetGameobjectActive(m_objTypeRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objStatRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSkillRoot, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objTypeRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objStatRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objSkillRoot, bValue: true);
		if (m_imgBattleType != null)
		{
			m_imgBattleType.sprite = NKCUtil.GetMoveTypeImg(unitTempletBase.m_bAirUnit);
		}
		if (m_lbBattleType != null)
		{
			m_lbBattleType.text = NKCUtilString.GetMoveTypeText(unitTempletBase.m_bAirUnit);
		}
		if (m_imgAttackType != null)
		{
			m_imgAttackType.sprite = NKCResourceUtility.GetOrLoadUnitAttackTypeIcon(unitTempletBase);
		}
		if (m_lbAttackType != null)
		{
			m_lbAttackType.text = NKCUtilString.GetAtkTypeText(unitTempletBase);
		}
		SetUnitStat(unitID, unitLv, unitLimitBreakLv);
		int skillCount = unitTempletBase.GetSkillCount();
		for (int i = 0; i < m_lstSkillSlot.Count; i++)
		{
			if (i < skillCount)
			{
				NKCUtil.SetGameobjectActive(m_lstSkillSlot[i], bValue: true);
				string skillStrID = unitTempletBase.GetSkillStrID(i);
				int skillLevel = item.GetSkillLevel(skillStrID);
				NKMUnitSkillTemplet unitSkillTemplet = NKMUnitSkillManager.GetUnitSkillTemplet(skillStrID, skillLevel);
				if (unitSkillTemplet != null)
				{
					m_lstSkillSlot[i].SetData(unitSkillTemplet, unitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER);
					bool value = NKMUnitSkillManager.IsLockedSkill(unitSkillTemplet.m_ID, item.m_LimitBreakLevel);
					m_lstSkillSlot[i].LockSkill(value);
					int unitStarGradeMax = unitTempletBase.m_StarGradeMax;
					NKCUIComButton component = m_lstSkillSlot[i].GetComponent<NKCUIComButton>();
					component.PointerDown.RemoveAllListeners();
					component.PointerDown.AddListener(delegate(PointerEventData e)
					{
						OnPointDownSkill(unitSkillTemplet, unitStarGradeMax, e);
					});
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstSkillSlot[i], bValue: false);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstSkillSlot[i], bValue: false);
			}
		}
	}

	private void SetUnitStat(int unitID, int unitLv = 1, short unitLimitBreakLv = 0)
	{
		NKMUnitData unitData = NKCUtil.MakeDummyUnit(unitID, unitLv, unitLimitBreakLv);
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitID);
		NKMStatData nKMStatData = new NKMStatData();
		nKMStatData.Init();
		nKMStatData.MakeBaseStat(null, bPvP: false, unitData, unitStatTemplet.m_StatData);
		NKCUtil.SetLabelText(m_lbHP, ((int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_HP)).ToString());
		NKCUtil.SetLabelText(m_lbAtt, ((int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_ATK)).ToString());
		NKCUtil.SetLabelText(m_lbDef, ((int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_DEF)).ToString());
		NKCUtil.SetLabelText(m_lbCrit, ((int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_CRITICAL)).ToString());
		NKCUtil.SetLabelText(m_lbHit, ((int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_HIT)).ToString());
		NKCUtil.SetLabelText(m_lbEvd, ((int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_EVADE)).ToString());
	}

	private void OnPointDownSkill(NKMUnitSkillTemplet unitSkillTemplet, int unitStarGradeMax, PointerEventData eventData)
	{
		if (unitSkillTemplet != null)
		{
			NKCUITooltip.Instance.Open(unitSkillTemplet, eventData.position, unitStarGradeMax);
		}
	}
}
