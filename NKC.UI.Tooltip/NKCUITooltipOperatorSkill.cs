using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Tooltip;

public class NKCUITooltipOperatorSkill : NKCUITooltipBase
{
	public Image SkillIcon;

	public Text m_SkillLv;

	public Text m_SkillName;

	public Text m_SkillType;

	public GameObject m_SkillCool;

	public Text m_SkillCoolTime;

	public override void Init()
	{
	}

	public override void SetData(NKCUITooltip.Data data)
	{
		if (!(data is NKCUITooltip.OperatorSkillData operatorSkillData))
		{
			return;
		}
		NKCUtil.SetImageSprite(SkillIcon, NKCUtil.GetSkillIconSprite(operatorSkillData.skillTemplet));
		NKCUtil.SetLabelText(m_SkillLv, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, operatorSkillData.skillLevel));
		NKCUtil.SetLabelText(m_SkillName, NKCStringTable.GetString(operatorSkillData.skillTemplet.m_OperSkillNameStrID));
		string text = "";
		if (operatorSkillData.skillTemplet.m_OperSkillType == OperatorSkillType.m_Tactical)
		{
			text = NKCUtilString.GET_STRING_OPERATOR_TOOLTIP_ACTIVE_SKILL_TITLE;
			NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(operatorSkillData.skillTemplet.m_OperSkillID);
			if (tacticalCommandTempletByID != null)
			{
				NKCUtil.SetLabelText(m_SkillCoolTime, string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_SECONDS"), (int)tacticalCommandTempletByID.m_fCoolTime));
			}
			NKCUtil.SetGameobjectActive(m_SkillCoolTime, tacticalCommandTempletByID != null);
		}
		else
		{
			text = NKCUtilString.GET_STRING_OPERATOR_TOOLTIP_PASSIVE_SKILL_TITLE;
		}
		NKCUtil.SetGameobjectActive(m_SkillCool, operatorSkillData.skillTemplet.m_OperSkillType == OperatorSkillType.m_Tactical);
		NKCUtil.SetLabelText(m_SkillType, text);
	}
}
