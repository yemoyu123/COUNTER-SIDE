using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.HUD;

public class NKCUIMainHPGage : MonoBehaviour
{
	public NKCUIMainGageBuff m_NKCUIMainGageBuff;

	private float m_fCurrentHPRate;

	private float m_fCurrentBarrierRate;

	private NKMTrackingFloat m_InnerSize = new NKMTrackingFloat();

	private NKMTrackingFloat m_InnerAlpha = new NKMTrackingFloat();

	public Image m_imgHPGage;

	public Image m_imgHPGageInner;

	public Image m_imgShieldGage;

	public RectTransform m_rtLine1;

	public RectTransform m_rtLine2;

	public Animator m_animFX;

	public GameObject m_objRageGageEffect;

	public NKCUIMainGageBuff GetMainGageBuff()
	{
		return m_NKCUIMainGageBuff;
	}

	public void InitUI()
	{
		if (m_NKCUIMainGageBuff != null)
		{
			m_NKCUIMainGageBuff.InitUI();
		}
	}

	public void InitData()
	{
		m_InnerSize.SetNowValue(0f);
		m_InnerAlpha.SetNowValue(0f);
		NKCUtil.SetImageFillAmount(m_imgHPGage, 0f);
		NKCUtil.SetImageFillAmount(m_imgHPGageInner, 0f);
		NKCUtil.SetGameobjectActive(m_objRageGageEffect, bValue: false);
	}

	public void SetMainGageVisible(bool bSet)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bSet);
	}

	public bool IsVisibleMainGage()
	{
		return base.gameObject.activeSelf;
	}

	public void UpdateGage(float fDeltaTime)
	{
		m_InnerSize.Update(fDeltaTime);
		m_InnerAlpha.Update(fDeltaTime);
		if (m_imgHPGageInner != null)
		{
			m_imgHPGageInner.fillAmount = m_InnerSize.GetNowValue();
			if (m_InnerAlpha.IsTracking())
			{
				Color color = m_imgHPGageInner.color;
				color.a = m_InnerAlpha.GetNowValue();
				m_imgHPGageInner.color = color;
			}
		}
	}

	public void SetMainGage(NKMUnit unit)
	{
		if (unit != null)
		{
			float hP = unit.GetUnitSyncData().GetHP();
			float statFinal = unit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP);
			float totalBarrierHP = GetTotalBarrierHP(unit);
			SetMainGage(hP, statFinal, totalBarrierHP);
		}
	}

	private float GetTotalBarrierHP(NKMUnit unit)
	{
		float num = 0f;
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in unit.GetUnitFrameData().m_dicBuffData)
		{
			NKMBuffData value = dicBuffDatum.Value;
			if (value != null && value.m_NKMBuffTemplet.m_fBarrierHP != -1f && value.m_BuffSyncData.m_bAffect)
			{
				num += value.m_fBarrierHP;
			}
		}
		return num;
	}

	public void SetMainGage(float fHP, float fMaxHP, float fBarrierHP)
	{
		float num = Mathf.Max(fMaxHP, fHP + fBarrierHP);
		float num2 = ((num != 0f) ? (fHP / num) : 0f);
		float num3 = 0f;
		if (fBarrierHP > 0f)
		{
			num3 = ((num != 0f) ? ((fBarrierHP + fHP) / num) : 0f);
		}
		if (m_fCurrentHPRate == num2 && m_fCurrentBarrierRate == num3)
		{
			return;
		}
		if (m_animFX != null && m_animFX.gameObject.activeInHierarchy)
		{
			if (m_fCurrentHPRate < 0.999f && num2 >= 0.999f)
			{
				m_animFX.Play("FULL", -1);
			}
			else if (m_fCurrentHPRate > 0f && num2 <= 0f)
			{
				m_animFX.Play("BASE", -1);
			}
		}
		if (m_fCurrentHPRate != num2)
		{
			m_InnerAlpha.SetNowValue(0.8f);
			m_InnerAlpha.SetTracking(0f, 1.7f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		m_fCurrentHPRate = num2;
		m_fCurrentBarrierRate = num3;
		m_imgShieldGage.fillAmount = num3;
		m_imgHPGage.fillAmount = num2;
		m_InnerSize.SetTracking(num2, 2f, TRACKING_DATA_TYPE.TDT_SLOWER);
		RecalculateLinePos(fMaxHP, num);
	}

	private void RecalculateLinePos(float originalMaxHP, float currentMaxHP)
	{
		float num = ((originalMaxHP != 0f) ? (currentMaxHP / originalMaxHP) : 1f);
		if (m_rtLine1 != null)
		{
			float num2 = 0.6f / num;
			m_rtLine1.anchorMin = new Vector2(1f - num2, 0.5f);
			m_rtLine1.anchorMax = new Vector2(1f - num2, 0.5f);
			m_rtLine1.anchoredPosition = Vector2.zero;
		}
		if (m_rtLine2 != null)
		{
			float num3 = 0.3f / num;
			m_rtLine2.anchorMin = new Vector2(1f - num3, 0.5f);
			m_rtLine2.anchorMax = new Vector2(1f - num3, 0.5f);
			m_rtLine2.anchoredPosition = Vector2.zero;
		}
	}

	public void SetRageMode(bool bOn)
	{
		NKCUtil.SetGameobjectActive(m_objRageGageEffect, bOn);
	}
}
