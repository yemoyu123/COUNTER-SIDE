using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComSkillGauge : MonoBehaviour
{
	private const string SKILL_COLOR_NORMAL = "#008FFF";

	private const string SKILL_COLOR_FURY = "#FFB830";

	private const string HYPER_COLOR_NORMAL = "#9900FF";

	private const string HYPER_COLOR_FURY = "#FF7F1B";

	public GameObject m_UNIT_SKILL_GAGE;

	public Image m_UNIT_GAGE_SKILL_BAR_Image;

	public Animator m_UNIT_SKILL_GAGE_FX_Animator;

	public GameObject m_UNIT_HYPER_SKILL_GAGE;

	public Image m_UNIT_GAGE_HYPER_SKILL_BAR_Image;

	public Animator m_UNIT_HYPER_SKILL_GAGE_FX_Animator;

	public void SetSkillType(bool bIsFury = false)
	{
		if (bIsFury)
		{
			NKCUtil.SetImageColor(m_UNIT_GAGE_SKILL_BAR_Image, NKCUtil.GetColor("#FFB830"));
			NKCUtil.SetImageColor(m_UNIT_GAGE_HYPER_SKILL_BAR_Image, NKCUtil.GetColor("#FF7F1B"));
		}
		else
		{
			NKCUtil.SetImageColor(m_UNIT_GAGE_SKILL_BAR_Image, NKCUtil.GetColor("#008FFF"));
			NKCUtil.SetImageColor(m_UNIT_GAGE_HYPER_SKILL_BAR_Image, NKCUtil.GetColor("#9900FF"));
		}
	}

	public GameObject GetSkillGauge()
	{
		return m_UNIT_SKILL_GAGE;
	}

	public void SetActiveSkillGauge(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_UNIT_SKILL_GAGE, bActive);
	}

	public void SetSkillCoolTime(float fSkillCollTimeRate)
	{
		if (base.gameObject.activeSelf)
		{
			if (m_UNIT_GAGE_SKILL_BAR_Image.fillAmount < 0.999f && fSkillCollTimeRate >= 0.999f)
			{
				m_UNIT_SKILL_GAGE_FX_Animator.Play("FULL", -1);
			}
			else if (m_UNIT_GAGE_SKILL_BAR_Image.fillAmount >= 0.999f && fSkillCollTimeRate < 0.999f)
			{
				m_UNIT_SKILL_GAGE_FX_Animator.Play("BASE", -1);
			}
		}
		m_UNIT_GAGE_SKILL_BAR_Image.fillAmount = fSkillCollTimeRate;
	}

	public GameObject GetHyperGauge()
	{
		return m_UNIT_HYPER_SKILL_GAGE;
	}

	public void SetActiveHyperGauge(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_UNIT_HYPER_SKILL_GAGE, bActive);
	}

	public void SetHyperCoolTime(float fHyperSkillCollTimeRate)
	{
		if (base.gameObject.activeSelf)
		{
			if (m_UNIT_GAGE_HYPER_SKILL_BAR_Image.fillAmount < 0.999f && fHyperSkillCollTimeRate >= 0.999f)
			{
				m_UNIT_HYPER_SKILL_GAGE_FX_Animator.Play("FULL", -1);
			}
			else if (m_UNIT_GAGE_HYPER_SKILL_BAR_Image.fillAmount >= 0.999f && fHyperSkillCollTimeRate < 0.999f)
			{
				m_UNIT_HYPER_SKILL_GAGE_FX_Animator.Play("BASE", -1);
			}
		}
		m_UNIT_GAGE_HYPER_SKILL_BAR_Image.fillAmount = fHyperSkillCollTimeRate;
	}
}
