using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Tooltip;

public class NKCUITooltipSkillUnit : NKCUITooltipBase
{
	public NKCUISkillSlot m_slot;

	public Text m_type;

	public Text m_name;

	public GameObject m_objCooltime;

	public Image m_imgSkillCooldown;

	public Text m_cooltime;

	public GameObject m_objAttackCount;

	public Text m_attackCount;

	public GameObject m_objCondUnlock;

	public List<GameObject> m_listCondUnlock;

	public override void Init()
	{
		m_slot.Init(null);
	}

	public override void SetData(NKCUITooltip.Data data)
	{
		if (!(data is NKCUITooltip.UnitSkillData { UnitSkillTemplet: var unitSkillTemplet } unitSkillData))
		{
			Debug.LogError("Tooltip UnitSkillData is null");
			return;
		}
		bool bIsHyper = unitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER;
		m_slot.SetData(unitSkillTemplet, bIsHyper);
		m_type.text = NKCUtilString.GetSkillTypeName(unitSkillTemplet.m_NKM_SKILL_TYPE);
		m_type.color = NKCUtil.GetSkillTypeColor(unitSkillTemplet.m_NKM_SKILL_TYPE);
		m_name.text = unitSkillTemplet.GetSkillName();
		bool flag = unitSkillTemplet.m_fCooltimeSecond > 0f;
		NKCUtil.SetGameobjectActive(m_objCooltime, flag);
		if (flag)
		{
			if (unitSkillData.IsFury)
			{
				NKCUtil.SetImageSprite(m_imgSkillCooldown, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_GAUNTLET_SMALL"));
				NKCUtil.SetLabelText(m_cooltime, string.Format(NKCUtilString.GET_STRING_COUNT_ONE_PARAM, unitSkillTemplet.m_fCooltimeSecond));
			}
			else
			{
				NKCUtil.SetImageSprite(m_imgSkillCooldown, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_TIME"));
				NKCUtil.SetLabelText(m_cooltime, string.Format(NKCUtilString.GET_STRING_TIME_SECOND_ONE_PARAM, unitSkillTemplet.m_fCooltimeSecond));
			}
		}
		bool flag2 = unitSkillTemplet.m_AttackCount > 0;
		NKCUtil.SetGameobjectActive(m_objAttackCount, flag2);
		if (flag2)
		{
			NKCUtil.SetLabelText(m_attackCount, string.Format(NKCUtilString.GET_STRING_SKILL_ATTACK_COUNT_ONE_PARAM, unitSkillTemplet.m_AttackCount));
		}
		bool flag3 = NKMUnitSkillManager.IsLockedSkill(unitSkillTemplet.m_ID, unitSkillData.UnitLimitBreakLevel);
		NKCUtil.SetGameobjectActive(m_objCondUnlock, flag3);
		if (flag3)
		{
			NKCUtil.SetSkillUnlockStarRank(m_listCondUnlock, unitSkillTemplet, unitSkillData.UnitStarGradeMax);
		}
	}
}
