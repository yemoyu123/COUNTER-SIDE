namespace NKM;

public class NKMTrackingVector3
{
	private NKMVector3 m_NowValue;

	private NKMVector3 m_BeforeValue;

	private NKMVector3 m_StartValue;

	private NKMVector3 m_TargetValue;

	private float m_fTime;

	private float m_fDeltaTime;

	private TRACKING_DATA_TYPE m_eTrackingType;

	private bool m_bPause;

	public NKMTrackingVector3()
	{
		m_NowValue = default(NKMVector3);
		m_BeforeValue = default(NKMVector3);
		m_StartValue = default(NKMVector3);
		m_TargetValue = default(NKMVector3);
		m_fTime = 0f;
		m_fDeltaTime = 0f;
		m_eTrackingType = TRACKING_DATA_TYPE.TDT_NORMAL;
		m_bPause = false;
	}

	public void SetPause(bool bSet)
	{
		m_bPause = bSet;
	}

	public bool GetPause()
	{
		return m_bPause;
	}

	public void SetNowValue(float fX, float fY, float fZ)
	{
		m_NowValue.x = fX;
		m_NowValue.y = fY;
		m_NowValue.z = fZ;
		m_TargetValue = m_NowValue;
		m_fDeltaTime = m_fTime;
	}

	public NKMVector3 GetNowValue()
	{
		return m_NowValue;
	}

	public float GetNowValueX()
	{
		return m_NowValue.x;
	}

	public float GetNowValueY()
	{
		return m_NowValue.y;
	}

	public float GetNowValueZ()
	{
		return m_NowValue.z;
	}

	public NKMVector3 GetDelta()
	{
		return m_NowValue - m_BeforeValue;
	}

	public NKMVector3 GetBeforeValue()
	{
		return m_BeforeValue;
	}

	public NKMVector3 GetTargetValue()
	{
		return m_TargetValue;
	}

	public bool IsTracking()
	{
		return m_fDeltaTime < m_fTime;
	}

	public void Update(float deltaTime)
	{
		if (m_fDeltaTime < m_fTime && !m_bPause)
		{
			m_fDeltaTime += deltaTime;
			float num = NKMTrackingFloat.TrackRatio(m_eTrackingType, m_fDeltaTime, m_fTime);
			m_BeforeValue = m_NowValue;
			m_NowValue.x = m_StartValue.x + (m_TargetValue.x - m_StartValue.x) * num;
			m_NowValue.y = m_StartValue.y + (m_TargetValue.y - m_StartValue.y) * num;
			m_NowValue.z = m_StartValue.z + (m_TargetValue.z - m_StartValue.z) * num;
			if (m_fDeltaTime >= m_fTime)
			{
				m_fDeltaTime = m_fTime;
				m_NowValue = m_TargetValue;
				m_BeforeValue = m_TargetValue;
			}
		}
	}

	public void StopTracking()
	{
		m_fDeltaTime = m_fTime;
	}

	public void SetTracking(NKMVector3 targetVal, float fTime, TRACKING_DATA_TYPE eTrackingType)
	{
		SetTracking(targetVal.x, targetVal.y, targetVal.z, fTime, eTrackingType);
	}

	public void SetTracking(float fX, float fY, float fZ, float fTime, TRACKING_DATA_TYPE eTrackingType)
	{
		m_bPause = false;
		m_StartValue = m_NowValue;
		m_TargetValue.Set(fX, fY, fZ);
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
