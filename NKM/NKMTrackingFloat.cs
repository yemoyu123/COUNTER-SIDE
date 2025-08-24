using System;

namespace NKM;

public class NKMTrackingFloat
{
	private float m_NowValue;

	private float m_BeforeValue;

	private float m_StartValue;

	private float m_TargetValue;

	private float m_fTime;

	private float m_fDeltaTime;

	private bool m_bTrackingLastFrame;

	private TRACKING_DATA_TYPE m_eTrackingType;

	private float m_fIntensity;

	public NKMTrackingFloat()
	{
		Init();
	}

	public void Init()
	{
		m_NowValue = 0f;
		m_BeforeValue = 0f;
		m_StartValue = 0f;
		m_TargetValue = 0f;
		m_fTime = 0f;
		m_fDeltaTime = 0f;
		m_eTrackingType = TRACKING_DATA_TYPE.TDT_NORMAL;
		m_fIntensity = 3f;
	}

	public void SetNowValue(float NowValue)
	{
		m_NowValue = NowValue;
		m_TargetValue = NowValue;
		m_fDeltaTime = m_fTime;
	}

	public float GetNowValue()
	{
		return m_NowValue;
	}

	public float GetDelta()
	{
		return m_NowValue - m_BeforeValue;
	}

	public float GetBeforeValue()
	{
		return m_BeforeValue;
	}

	public float GetTargetValue()
	{
		return m_TargetValue;
	}

	public bool IsTracking()
	{
		if (!(m_fDeltaTime < m_fTime))
		{
			return m_bTrackingLastFrame;
		}
		return true;
	}

	public void SetIntensity(float fValue)
	{
		m_fIntensity = fValue;
	}

	public void Update(float deltaTime)
	{
		NKMProfiler.BeginSample("NKMTrackingFloat.Update");
		if (m_fDeltaTime < m_fTime)
		{
			m_fDeltaTime += deltaTime;
			float num = TrackRatio(m_eTrackingType, m_fDeltaTime, m_fTime, m_fIntensity);
			m_BeforeValue = m_NowValue;
			m_NowValue = m_StartValue + (m_TargetValue - m_StartValue) * num;
			if (m_fDeltaTime >= m_fTime)
			{
				m_fDeltaTime = m_fTime;
				m_NowValue = m_TargetValue;
				m_BeforeValue = m_TargetValue;
				m_bTrackingLastFrame = true;
			}
		}
		else
		{
			m_bTrackingLastFrame = false;
		}
		NKMProfiler.EndSample();
	}

	public static float TrackRatio(TRACKING_DATA_TYPE type, float deltaTime, float endTime, float Intensity = 3f)
	{
		float num = deltaTime / endTime;
		switch (type)
		{
		case TRACKING_DATA_TYPE.TDT_FASTER:
			return (float)Math.Pow(num, Intensity);
		case TRACKING_DATA_TYPE.TDT_SLOWER:
			return 1f - (float)Math.Pow(1f - num, 3.0);
		case TRACKING_DATA_TYPE.TDT_SIN:
			return (float)Math.Sin(NKMUtil.NKMToRadian(deltaTime * 50f));
		case TRACKING_DATA_TYPE.TDT_SIN_PLUS:
			return Math.Abs((float)Math.Sin(NKMUtil.NKMToRadian(deltaTime * 100f))) * 0.5f;
		case TRACKING_DATA_TYPE.TDT_SIN_PLUS_FAST:
			return Math.Abs((float)Math.Sin(NKMUtil.NKMToRadian(deltaTime * 100f)));
		case TRACKING_DATA_TYPE.TDT_SIN_PLUS_FAST2:
			return Math.Abs((float)Math.Sin(NKMUtil.NKMToRadian(deltaTime * 200f)));
		case TRACKING_DATA_TYPE.TDT_SIN_PLUS_FAST4:
			return Math.Abs((float)Math.Sin(NKMUtil.NKMToRadian(deltaTime * 400f)));
		case TRACKING_DATA_TYPE.TDT_BACK_OUT:
			return (num -= 1f) * num * (2.70158f * num + 1.70158f) + 1f;
		case TRACKING_DATA_TYPE.TDT_BOUNCE_OUT:
			if ((double)num < 0.36363636363636365)
			{
				return 7.5625f * num * num;
			}
			if ((double)num < 0.7272727272727273)
			{
				return 7.5625f * (num -= 0.54545456f) * num + 0.75f;
			}
			if ((double)num < 0.9090909090909091)
			{
				return 7.5625f * (num -= 0.8181818f) * num + 0.9375f;
			}
			return 7.5625f * (num -= 21f / 22f) * num + 63f / 64f;
		default:
			return num;
		}
	}

	public void StopTracking()
	{
		m_fDeltaTime = m_fTime;
	}

	public void SetTracking(float targetVal, float fTime, TRACKING_DATA_TYPE eTrackingType)
	{
		m_StartValue = m_NowValue;
		m_TargetValue = targetVal;
		m_fTime = fTime;
		m_fDeltaTime = 0f;
		m_eTrackingType = eTrackingType;
	}

	public float GetTime()
	{
		return m_fTime;
	}

	public float GetDeltaTime()
	{
		return m_fDeltaTime;
	}

	public TRACKING_DATA_TYPE GetTrackingType()
	{
		return m_eTrackingType;
	}
}
