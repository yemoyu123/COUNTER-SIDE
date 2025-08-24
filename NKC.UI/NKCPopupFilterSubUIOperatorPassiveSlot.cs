using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupFilterSubUIOperatorPassiveSlot : MonoBehaviour
{
	public NKCUIComStateButton m_btn;

	public Image m_imgIconOff;

	public Text m_lbNameOff;

	public Image m_imgIconOn;

	public Text m_lbNameOn;

	private NKMOperatorSkillTemplet m_PassiveSkillTemplet;

	public NKCUIComStateButton GetButton()
	{
		return m_btn;
	}

	public NKMOperatorSkillTemplet GetPassiveTemplet()
	{
		return m_PassiveSkillTemplet;
	}

	public void SetData(NKMOperatorSkillTemplet skillTemplet, bool bSelected = false)
	{
		m_PassiveSkillTemplet = skillTemplet;
		m_btn.Select(bSelected, bForce: true, bImmediate: true);
		if (skillTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_imgIconOff, bValue: false);
			NKCUtil.SetLabelText(m_lbNameOff, NKCStringTable.GetString("SI_PF_SORT_OPR_PASSIVE_SKILL_OPTION"));
			return;
		}
		NKCUtil.SetGameobjectActive(m_imgIconOff, bValue: true);
		NKCUtil.SetImageSprite(m_imgIconOn, NKCUtil.GetSkillIconSprite(skillTemplet));
		NKCUtil.SetImageSprite(m_imgIconOff, NKCUtil.GetSkillIconSprite(skillTemplet));
		NKCUtil.SetLabelText(m_lbNameOn, NKCStringTable.GetString(skillTemplet.m_OperSkillNameStrID));
		NKCUtil.SetLabelText(m_lbNameOff, NKCStringTable.GetString(skillTemplet.m_OperSkillNameStrID));
	}

	public void SetData(int skillID, bool bSelected = false)
	{
		if (skillID > 0)
		{
			NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(skillID);
			SetData(skillTemplet, bSelected);
		}
		else
		{
			SetData(null);
		}
	}
}
