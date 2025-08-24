using NKC.UI.Component;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIPopupRearmamentConfirmSkillInfo : MonoBehaviour
{
	public Image m_SkillIcon;

	public Text m_SkillType;

	public Text m_SkillName;

	public Text m_SkillLevel;

	public NKCComTMPUIText m_SkillDesc;

	public void SetData(NKMUnitSkillTemplet skillTemplet)
	{
		if (skillTemplet != null)
		{
			NKCUtil.SetImageSprite(m_SkillIcon, NKCUtil.GetSkillIconSprite(skillTemplet));
			NKCUtil.SetLabelText(m_SkillType, NKCUtilString.GetSkillTypeName(skillTemplet.m_NKM_SKILL_TYPE));
			NKCUtil.SetLabelTextColor(m_SkillType, NKCUtil.GetSkillTypeColor(skillTemplet.m_NKM_SKILL_TYPE));
			NKCUtil.SetLabelText(m_SkillName, skillTemplet.GetSkillName());
			NKCUtil.SetLabelText(m_SkillLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, skillTemplet.m_Level));
			NKCUtil.SetLabelText(m_SkillDesc, skillTemplet.GetSkillDesc());
		}
	}
}
