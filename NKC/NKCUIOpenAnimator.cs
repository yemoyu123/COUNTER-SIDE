using NKM;
using UnityEngine;

namespace NKC;

public class NKCUIOpenAnimator
{
	private GameObject m_go;

	private RectTransform m_rt;

	private CanvasGroup m_canvas;

	private NKMTrackingFloat m_trackAlpha = new NKMTrackingFloat();

	private NKMTrackingFloat m_trackScale = new NKMTrackingFloat();

	public NKCUIOpenAnimator(GameObject go)
	{
		m_go = go;
		m_rt = m_go.GetComponent<RectTransform>();
		m_canvas = m_go.GetComponent<CanvasGroup>();
	}

	public void PlayOpenAni()
	{
		m_trackAlpha.SetNowValue(0f);
		if (m_canvas != null)
		{
			m_canvas.alpha = m_trackAlpha.GetNowValue();
		}
		m_trackScale.SetNowValue(1.1f);
		m_rt.localScale = Vector3.one * m_trackScale.GetNowValue();
		m_trackAlpha.SetTracking(1f, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_trackScale.SetTracking(1f, 0.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
	}

	public void Update()
	{
		float num = Time.deltaTime;
		if (NKCScenManager.GetScenManager() != null && num > NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f)
		{
			num = NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f;
		}
		m_trackAlpha.Update(num);
		m_trackScale.Update(num);
		if (m_trackAlpha.IsTracking() && m_canvas != null)
		{
			m_canvas.alpha = m_trackAlpha.GetNowValue();
		}
		if (m_trackScale.IsTracking())
		{
			m_rt.localScale = new Vector3(m_trackScale.GetNowValue(), m_trackScale.GetNowValue(), m_trackScale.GetNowValue());
		}
	}
}
