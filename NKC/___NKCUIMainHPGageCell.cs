using System.Text;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class ___NKCUIMainHPGageCell
{
	private GameObject m_NKM_UI_HUD_MAIN_GAGE;

	private GameObject m_NKM_UI_HUD_MAIN_GAGE_CELL;

	private Image m_NKM_UI_HUD_MAIN_GAGE_CELL_Image;

	private GameObject m_NKM_UI_HUD_MAIN_GAGE_CELL_INNER;

	private Image m_NKM_UI_HUD_MAIN_GAGE_CELL_INNER_Image;

	private GameObject m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX;

	private RectTransform m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX_RectTransform;

	private Animator m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX_Animator;

	private StringBuilder m_StringBuilder = new StringBuilder();

	public NKMTrackingFloat m_InnerSize = new NKMTrackingFloat();

	public NKMTrackingFloat m_InnerAlpha = new NKMTrackingFloat();

	private bool m_bGreen = true;

	private bool m_bLong = true;

	public ___NKCUIMainHPGageCell(GameObject cNKM_UI_HUD_MAIN_GAGE, bool bGreen, int index, bool bLong = false)
	{
		m_bGreen = bGreen;
		m_bLong = bLong;
		m_NKM_UI_HUD_MAIN_GAGE = cNKM_UI_HUD_MAIN_GAGE;
		m_StringBuilder.Remove(0, m_StringBuilder.Length);
		if (bLong)
		{
			if (bGreen)
			{
				m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_GREEN_MOD/NKM_UI_HUD_MAIN_GAGE_GREEN_CELL{0}", index);
			}
			else
			{
				m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_RED_MOD/NKM_UI_HUD_MAIN_GAGE_RED_CELL{0}", index);
			}
		}
		else if (bGreen)
		{
			m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_GREEN/NKM_UI_HUD_MAIN_GAGE_GREEN_CELL{0}", index);
		}
		else
		{
			m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_RED/NKM_UI_HUD_MAIN_GAGE_RED_CELL{0}", index);
		}
		m_NKM_UI_HUD_MAIN_GAGE_CELL = m_NKM_UI_HUD_MAIN_GAGE.transform.Find(m_StringBuilder.ToString()).gameObject;
		m_NKM_UI_HUD_MAIN_GAGE_CELL_Image = m_NKM_UI_HUD_MAIN_GAGE_CELL.GetComponent<Image>();
		m_StringBuilder.Remove(0, m_StringBuilder.Length);
		if (bLong)
		{
			if (bGreen)
			{
				m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_GREEN_MOD/NKM_UI_HUD_MAIN_GAGE_GREEN_CELL{0}_INNER", index);
			}
			else
			{
				m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_RED_MOD/NKM_UI_HUD_MAIN_GAGE_RED_CELL{0}_INNER", index);
			}
		}
		else if (bGreen)
		{
			m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_GREEN/NKM_UI_HUD_MAIN_GAGE_GREEN_CELL{0}_INNER", index);
		}
		else
		{
			m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_RED/NKM_UI_HUD_MAIN_GAGE_RED_CELL{0}_INNER", index);
		}
		m_NKM_UI_HUD_MAIN_GAGE_CELL_INNER = m_NKM_UI_HUD_MAIN_GAGE.transform.Find(m_StringBuilder.ToString()).gameObject;
		m_NKM_UI_HUD_MAIN_GAGE_CELL_INNER_Image = m_NKM_UI_HUD_MAIN_GAGE_CELL_INNER.GetComponent<Image>();
		m_StringBuilder.Remove(0, m_StringBuilder.Length);
		if (bLong)
		{
			if (bGreen)
			{
				m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_GREEN_MOD/NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX/NKM_UI_HUD_MAIN_GAGE_GREEN_CELL{0}_FX", index);
			}
			else
			{
				m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_RED_MOD/NKM_UI_HUD_MAIN_GAGE_RED_CELL_FX/NKM_UI_HUD_MAIN_GAGE_RED_CELL{0}_FX", index);
			}
		}
		else if (bGreen)
		{
			m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_GREEN/NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX/NKM_UI_HUD_MAIN_GAGE_GREEN_CELL{0}_FX", index);
		}
		else
		{
			m_StringBuilder.AppendFormat("NKM_UI_HUD_MAIN_GAGE_RED/NKM_UI_HUD_MAIN_GAGE_RED_CELL_FX/NKM_UI_HUD_MAIN_GAGE_RED_CELL{0}_FX", index);
		}
		m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX = m_NKM_UI_HUD_MAIN_GAGE.transform.Find(m_StringBuilder.ToString()).gameObject;
		m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX_RectTransform = m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX.GetComponent<RectTransform>();
		m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX_Animator = m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX.GetComponent<Animator>();
	}

	public void Init()
	{
		m_NKM_UI_HUD_MAIN_GAGE_CELL_Image.fillAmount = 0f;
		m_NKM_UI_HUD_MAIN_GAGE_CELL_INNER_Image.fillAmount = 0f;
		m_InnerSize.SetNowValue(0f);
		m_InnerAlpha.SetNowValue(1f);
	}

	public void SetMainGage(float fHPRate)
	{
		if (m_NKM_UI_HUD_MAIN_GAGE_CELL_Image.fillAmount == fHPRate)
		{
			return;
		}
		if (m_NKM_UI_HUD_MAIN_GAGE_CELL_Image.fillAmount < 0.999f && fHPRate >= 0.999f)
		{
			if (m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX_Animator.gameObject.activeInHierarchy)
			{
				m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX_Animator.Play("FULL", -1);
			}
		}
		else if (m_NKM_UI_HUD_MAIN_GAGE_CELL_Image.fillAmount > 0f && fHPRate <= 0f && m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX_Animator.gameObject.activeInHierarchy)
		{
			m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX_Animator.Play("BASE", -1);
		}
		if (m_NKM_UI_HUD_MAIN_GAGE_CELL_Image.fillAmount != fHPRate)
		{
			m_InnerAlpha.SetNowValue(0.8f);
			m_InnerAlpha.SetTracking(0f, 1.7f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		Vector3 localScale = m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX_RectTransform.localScale;
		if (!m_bLong)
		{
			if (m_bGreen)
			{
				localScale.x = fHPRate;
			}
			else
			{
				localScale.x = 0f - fHPRate;
			}
		}
		else if (!m_bGreen)
		{
			localScale.x = fHPRate;
		}
		else
		{
			localScale.x = 0f - fHPRate;
		}
		m_NKM_UI_HUD_MAIN_GAGE_GREEN_CELL_FX_RectTransform.localScale = localScale;
		m_NKM_UI_HUD_MAIN_GAGE_CELL_Image.fillAmount = fHPRate;
		m_InnerSize.SetTracking(fHPRate, 2f, TRACKING_DATA_TYPE.TDT_SLOWER);
	}

	public void Update(float fDeltaTime)
	{
		m_InnerSize.Update(fDeltaTime);
		m_InnerAlpha.Update(fDeltaTime);
		m_NKM_UI_HUD_MAIN_GAGE_CELL_INNER_Image.fillAmount = m_InnerSize.GetNowValue();
		if (m_InnerAlpha.IsTracking())
		{
			Color color = m_NKM_UI_HUD_MAIN_GAGE_CELL_INNER_Image.color;
			color.a = m_InnerAlpha.GetNowValue();
			m_NKM_UI_HUD_MAIN_GAGE_CELL_INNER_Image.color = color;
		}
	}
}
