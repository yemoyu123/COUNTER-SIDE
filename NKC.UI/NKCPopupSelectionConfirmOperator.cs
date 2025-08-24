using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupSelectionConfirmOperator : MonoBehaviour
{
	public NKCUIOperatorSelectListSlot m_slot;

	[Header("스탯")]
	public Text m_lbHP;

	public Text m_lbAtt;

	public Text m_lbDef;

	public Text m_lbSkillCoolReduce;

	[Header("스킬")]
	public NKCUIOperatorSkill m_skillMain;

	public NKCUIOperatorSkill m_skillSub;

	public void SetData(int unitID, int subSkillID, int opLevel, int mainSkillLv, int subSkillLv)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase != null)
		{
			m_slot.SetOperatorData(unitTempletBase, opLevel, bEnableLayoutElement: false, null);
			m_slot.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
			SetOperatorStat(unitID, opLevel);
			UpdateSkillInfo(unitID, subSkillID, mainSkillLv, subSkillLv);
		}
	}

	private void SetOperatorStat(int unitID, int unitLv)
	{
		NKCUtil.SetLabelText(m_lbAtt, NKCOperatorUtil.GetStatPercentageString(unitID, unitLv, NKM_STAT_TYPE.NST_ATK) ?? "");
		NKCUtil.SetLabelText(m_lbDef, NKCOperatorUtil.GetStatPercentageString(unitID, unitLv, NKM_STAT_TYPE.NST_DEF) ?? "");
		NKCUtil.SetLabelText(m_lbHP, NKCOperatorUtil.GetStatPercentageString(unitID, unitLv, NKM_STAT_TYPE.NST_HP) ?? "");
		NKCUtil.SetLabelText(m_lbSkillCoolReduce, NKCOperatorUtil.GetStatPercentageString(unitID, unitLv, NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE) ?? "");
	}

	private void UpdateSkillInfo(int unitID, int subSkillID, int mainSkillLv, int subSkillLv)
	{
		NKMOperatorSkillTemplet mainSkill = NKCOperatorUtil.GetMainSkill(unitID);
		m_skillMain.SetData(mainSkill, mainSkillLv);
		NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(subSkillID);
		if (skillTemplet != null)
		{
			m_skillSub.SetData(skillTemplet, subSkillLv);
		}
		NKCUtil.SetGameobjectActive(m_skillSub, skillTemplet != null);
	}
}
