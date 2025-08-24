using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCUIDeckViewOperator : NKCUIInstantiatable
{
	public RectTransform m_rectAnchor;

	public float m_fSelectedScale = 1f;

	public float m_fDeselectedScale = 0.7f;

	private NKMTrackingFloat m_ScaleX = new NKMTrackingFloat();

	private NKMTrackingFloat m_ScaleY = new NKMTrackingFloat();

	private bool m_bTrackingStarted;

	public void Init()
	{
		m_ScaleX.SetNowValue(m_fSelectedScale);
		m_ScaleY.SetNowValue(m_fSelectedScale);
	}

	public void Close()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Update()
	{
		m_ScaleX.Update(Time.deltaTime);
		m_ScaleY.Update(Time.deltaTime);
		if (!m_ScaleX.IsTracking() && m_bTrackingStarted)
		{
			m_bTrackingStarted = false;
		}
		Vector2 vector = m_rectAnchor.localScale;
		vector.Set(m_ScaleX.GetNowValue(), m_ScaleY.GetNowValue());
		m_rectAnchor.localScale = vector;
	}

	public void Enable()
	{
		m_ScaleX.SetTracking(m_fSelectedScale, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_ScaleY.SetTracking(m_fSelectedScale, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_bTrackingStarted = true;
	}

	public void Disable()
	{
		m_ScaleX.SetTracking(m_fDeselectedScale, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_ScaleY.SetTracking(m_fDeselectedScale, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_bTrackingStarted = true;
	}
}
