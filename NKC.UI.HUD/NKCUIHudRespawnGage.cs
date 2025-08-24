using System.Collections.Generic;
using System.Text;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.HUD;

public class NKCUIHudRespawnGage : MonoBehaviour
{
	public Animator m_AnimatorGageCost;

	public Image m_imgCostBgCharge;

	public Text m_lbGageCost;

	public NKCUIHudRespawnCostAddEvent m_pfbCostAdd;

	private List<NKCUIHudRespawnCostAddEvent> m_lstCostAdd = new List<NKCUIHudRespawnCostAddEvent>();

	private int m_BeforeRespawnCost;

	private NKMTrackingFloat m_RespawnCostGage = new NKMTrackingFloat();

	private NKMTrackingFloat m_RespawnCostGageAssist = new NKMTrackingFloat();

	private StringBuilder m_StringBuilder = new StringBuilder();

	[Header("\ufffdȾ\ufffd\ufffd°\ufffd \ufffdϴ\ufffd \ufffd\ufffd\ufffd\u0735\ufffd")]
	public Image m_imgRespawnMovePoint;

	public void SetData()
	{
		m_RespawnCostGage.SetNowValue(0f);
		m_RespawnCostGageAssist.SetNowValue(0f);
		m_StringBuilder.Remove(0, m_StringBuilder.Length);
		m_StringBuilder.AppendFormat("{0}", m_BeforeRespawnCost);
		m_lbGageCost.text = m_StringBuilder.ToString();
	}

	public void SetRespawnCostNowValue(float fValue)
	{
		m_RespawnCostGage.Init();
		m_RespawnCostGage.SetNowValue(fValue);
		m_RespawnCostGageAssist.SetNowValue(fValue);
	}

	public void SetSupply(int supplyCount)
	{
		if (supplyCount == 0)
		{
			Color color = m_imgCostBgCharge.color;
			color.r = 0.7f;
			color.g = 0f;
			color.b = 0.7f;
			m_imgCostBgCharge.color = color;
			color = m_lbGageCost.color;
			color.r = 0.7f;
			color.g = 0.7f;
			color.b = 0.7f;
			m_lbGageCost.color = color;
		}
		else
		{
			Color color2 = m_imgCostBgCharge.color;
			color2.r = 1f;
			color2.g = 0f;
			color2.b = 1f;
			m_imgCostBgCharge.color = color2;
			color2 = m_lbGageCost.color;
			color2.r = 1f;
			color2.g = 1f;
			color2.b = 1f;
			m_lbGageCost.color = color2;
		}
	}

	public void UpdateGage(float fDeltaTime)
	{
		m_RespawnCostGage.Update(fDeltaTime);
		m_RespawnCostGageAssist.Update(fDeltaTime);
		float nowValue = m_RespawnCostGage.GetNowValue();
		int num = Mathf.FloorToInt(nowValue);
		if (m_BeforeRespawnCost < num && m_AnimatorGageCost.gameObject.activeInHierarchy)
		{
			m_AnimatorGageCost.Play("FULL", -1, 0f);
		}
		if (m_BeforeRespawnCost != num)
		{
			m_BeforeRespawnCost = num;
			m_StringBuilder.Remove(0, m_StringBuilder.Length);
			int num2 = num;
			if (num2 < 0)
			{
				num2 = 0;
			}
			m_StringBuilder.AppendFormat("{0}", num2);
			m_lbGageCost.text = m_StringBuilder.ToString();
		}
		NKCUtil.SetImageFillAmount(m_imgCostBgCharge, nowValue - (float)num);
	}

	public void SetRespawnCost(float fRespawnCost)
	{
		if (fRespawnCost < m_RespawnCostGage.GetNowValue())
		{
			m_RespawnCostGage.SetTracking(fRespawnCost, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		else if (m_RespawnCostGage.GetTargetValue() != fRespawnCost || !m_RespawnCostGage.IsTracking())
		{
			m_RespawnCostGage.SetTracking(fRespawnCost, 1f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
	}

	public float GetRespawnCostGage()
	{
		return m_RespawnCostGage.GetNowValue();
	}

	public void SetRespawnCostAssist(float fRespawnCost)
	{
		if (fRespawnCost < m_RespawnCostGageAssist.GetNowValue())
		{
			m_RespawnCostGageAssist.SetTracking(fRespawnCost, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		else if (m_RespawnCostGageAssist.GetTargetValue() != fRespawnCost || !m_RespawnCostGageAssist.IsTracking())
		{
			m_RespawnCostGageAssist.SetTracking(fRespawnCost, 1f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
	}

	public float GetRespawnCostGageAssist()
	{
		return m_RespawnCostGageAssist.GetNowValue();
	}

	public void PlayRespawnAddEvent(float value)
	{
		GetAddEvent().Play(value);
	}

	private NKCUIHudRespawnCostAddEvent GetAddEvent()
	{
		foreach (NKCUIHudRespawnCostAddEvent item in m_lstCostAdd)
		{
			if (item != null && item.Idle)
			{
				return item;
			}
		}
		return MakeAddEvent();
	}

	private NKCUIHudRespawnCostAddEvent MakeAddEvent()
	{
		NKCUIHudRespawnCostAddEvent nKCUIHudRespawnCostAddEvent = Object.Instantiate(m_pfbCostAdd, base.transform);
		if (nKCUIHudRespawnCostAddEvent == null)
		{
			return null;
		}
		nKCUIHudRespawnCostAddEvent.transform.localRotation = Quaternion.identity;
		nKCUIHudRespawnCostAddEvent.transform.localScale = Vector3.one;
		nKCUIHudRespawnCostAddEvent.transform.SetAsLastSibling();
		m_lstCostAdd.Add(nKCUIHudRespawnCostAddEvent);
		NKCUtil.SetGameobjectActive(nKCUIHudRespawnCostAddEvent, bValue: false);
		return nKCUIHudRespawnCostAddEvent;
	}
}
