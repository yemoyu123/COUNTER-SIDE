using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIReactorSkillInfo : MonoBehaviour
{
	public Image m_imgReactorLevelIcon;

	public Image m_imgInvenIcon;

	public Image m_imgSkillIcon;

	public Text m_lbSkillTitle;

	public NKCComTMPUIText m_lbSkillDesc;

	public GameObject m_bojLock;

	public void SetData(NKMReactorSkillTemplet skillTemplet, bool bLock, int TargetUnitID = 0)
	{
		if (skillTemplet == null)
		{
			return;
		}
		int id = ((TargetUnitID == 0) ? skillTemplet.ReactorId : TargetUnitID);
		if (string.IsNullOrEmpty(skillTemplet.BaseSkillStrId))
		{
			NKCUtil.SetImageSprite(m_imgInvenIcon, NKCResourceUtility.GetRewardInvenIcon(NKM_REWARD_TYPE.RT_UNIT, id));
			NKCUtil.SetGameobjectActive(m_imgInvenIcon, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgSkillIcon, bValue: false);
			NKCUtil.SetLabelText(m_lbSkillTitle, NKCUtilString.GET_STRING_UNIT_REACTOR_SKILL_TITLE_STAT);
		}
		else
		{
			int maxSkillLevel = NKMUnitSkillManager.GetMaxSkillLevel(skillTemplet.BaseSkillStrId);
			NKMUnitSkillTemplet skillTemplet2 = NKMUnitSkillManager.GetSkillTemplet(skillTemplet.BaseSkillStrId, maxSkillLevel);
			if (skillTemplet2 != null)
			{
				NKCUtil.SetImageSprite(m_imgSkillIcon, NKCUtil.GetSkillIconSprite(skillTemplet2));
				string skillTypeName = NKCUtilString.GetSkillTypeName(skillTemplet2.m_NKM_SKILL_TYPE);
				NKCUtil.SetLabelText(m_lbSkillTitle, string.Format(NKCUtilString.GET_STRING_UNIT_REACTOR_SKILL_TITLE_SKILL, skillTypeName, skillTemplet2.GetSkillName()));
			}
			NKCUtil.SetImageSprite(m_imgInvenIcon, NKCResourceUtility.GetRewardInvenIcon(NKM_REWARD_TYPE.RT_UNIT, id));
			NKCUtil.SetGameobjectActive(m_imgInvenIcon, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgSkillIcon, bValue: true);
		}
		NKCUtil.SetLabelText(m_lbSkillDesc, NKCStringTable.GetString(skillTemplet.SkillDesc));
		NKCUtil.SetGameobjectActive(m_bojLock, bLock);
	}
}
