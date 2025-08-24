using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.HUD;

public class NKCUIGameUnitSkillCooltime : MonoBehaviour
{
	[Header("\ufffd\ufffdų \ufffd\ufffdŸ\ufffd\ufffd")]
	public GameObject m_objSkillCoolRoot;

	public Image m_imgSkillCool;

	public GameObject m_objSkillCoolFx;

	public Animator m_animatorSkillCoolFx;

	public Image m_imgSkillCoolFxBlur;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffdŸ\ufffd\ufffd")]
	public GameObject m_objHyperCoolRoot;

	public Image m_imgHyperCool;

	public GameObject m_objHyperCoolFx;

	public Animator m_animatorHyperCoolFx;

	public Image m_imgHyperCoolFxBlur;

	public void SetSkillCoolVisible(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objSkillCoolRoot, value);
		NKCUtil.SetGameobjectActive(m_objSkillCoolFx, bValue: false);
	}

	public void SetHyperCoolVisible(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objHyperCoolRoot, value);
		NKCUtil.SetGameobjectActive(m_objHyperCoolFx, bValue: false);
	}

	public void SetUnit(NKMUnitTemplet unitTemplet, NKMUnitData unitData)
	{
		SetSkillCoolVisible(HaveUnlockedSkill(unitTemplet, unitData));
		SetHyperCoolVisible(HaveUnlockedHyper(unitTemplet, unitData));
		if (unitTemplet.m_UnitTempletBase.StopDefaultCoolTime)
		{
			NKCUtil.SetImageColor(m_imgSkillCool, NKCUtil.GetColor("#FFB830"));
			NKCUtil.SetImageColor(m_imgSkillCoolFxBlur, NKCUtil.GetColor("#FFB830"));
			NKCUtil.SetImageColor(m_imgHyperCool, NKCUtil.GetColor("#FF7F1B"));
			NKCUtil.SetImageColor(m_imgHyperCoolFxBlur, NKCUtil.GetColor("#FF7F1B"));
		}
		else
		{
			NKCUtil.SetImageColor(m_imgSkillCool, NKCUtil.GetColor("#008FFF"));
			NKCUtil.SetImageColor(m_imgSkillCoolFxBlur, NKCUtil.GetColor("#008FFF"));
			NKCUtil.SetImageColor(m_imgHyperCool, NKCUtil.GetColor("#9900FF"));
			NKCUtil.SetImageColor(m_imgHyperCoolFxBlur, NKCUtil.GetColor("#9900FF"));
		}
		NKCUtil.SetImageFillAmount(m_imgSkillCool, 0f);
		NKCUtil.SetImageFillAmount(m_imgHyperCool, 0f);
	}

	public void SetCooltime(float fSkillCoolRate, float fHyperCoolRate)
	{
		SetSkillCooltime(fSkillCoolRate);
		SetHyperCooltime(fHyperCoolRate);
	}

	public void SetSkillCooltime(float fSkillCoolNow, float fSkillCoolMax)
	{
		SetSkillCooltime(fSkillCoolNow / fSkillCoolMax);
	}

	public void SetSkillCooltime(float skillRateNow)
	{
		if (!m_objSkillCoolRoot.activeSelf)
		{
			return;
		}
		float num = 1f - skillRateNow;
		if (m_imgSkillCool.fillAmount < 1f && num >= 1f)
		{
			NKCUtil.SetGameobjectActive(m_objSkillCoolFx, bValue: true);
			if (m_objSkillCoolFx.activeInHierarchy)
			{
				m_animatorSkillCoolFx.Play("FULL");
			}
		}
		else if (num < 1f)
		{
			NKCUtil.SetGameobjectActive(m_objSkillCoolFx, bValue: false);
		}
		m_imgSkillCool.fillAmount = num;
	}

	public void SetHyperCooltime(float fHyperSkillCoolNow, float fHyperSkillMax)
	{
		SetHyperCooltime(fHyperSkillCoolNow / fHyperSkillMax);
	}

	public void SetHyperCooltime(float hyperRateNow)
	{
		if (!m_objHyperCoolRoot.activeSelf)
		{
			return;
		}
		float num = 1f - hyperRateNow;
		if (m_imgHyperCool.fillAmount < 1f && num >= 1f)
		{
			NKCUtil.SetGameobjectActive(m_objHyperCoolFx, bValue: true);
			if (m_objHyperCoolFx.activeInHierarchy)
			{
				m_animatorHyperCoolFx.Play("FULL");
			}
		}
		else if (num < 1f)
		{
			NKCUtil.SetGameobjectActive(m_objHyperCoolFx, bValue: false);
		}
		m_imgHyperCool.fillAmount = num;
	}

	private bool HaveUnlockedSkill(NKMUnitTemplet unitTemplet, NKMUnitData unitData)
	{
		if (unitTemplet == null)
		{
			return false;
		}
		if (unitData == null)
		{
			return false;
		}
		if (unitTemplet.m_listSkillStateData.Count <= 0)
		{
			return false;
		}
		if (unitTemplet.m_listSkillStateData[0] == null)
		{
			return false;
		}
		NKMUnitState unitState = unitTemplet.GetUnitState(unitTemplet.m_listSkillStateData[0].m_StateName);
		if (unitState == null)
		{
			return false;
		}
		return unitData.IsUnitSkillUnlockedByType(unitState.m_NKM_SKILL_TYPE);
	}

	private bool HaveUnlockedHyper(NKMUnitTemplet unitTemplet, NKMUnitData unitData)
	{
		if (unitTemplet == null)
		{
			return false;
		}
		if (unitData == null)
		{
			return false;
		}
		if (unitTemplet.m_listHyperSkillStateData.Count <= 0)
		{
			return false;
		}
		if (unitTemplet.m_listHyperSkillStateData[0] == null)
		{
			return false;
		}
		NKMUnitState unitState = unitTemplet.GetUnitState(unitTemplet.m_listHyperSkillStateData[0].m_StateName);
		if (unitState == null)
		{
			return false;
		}
		return unitData.IsUnitSkillUnlockedByType(unitState.m_NKM_SKILL_TYPE);
	}
}
