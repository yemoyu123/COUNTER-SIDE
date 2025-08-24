namespace NKM;

public class NKMTrackingVector2
{
	private NKMVector2 m_NowValue;

	private NKMVector2 m_BeforeValue;

	private NKMVector2 m_StartValue;

	private NKMVector2 m_TargetValue;

	private float m_fTime;

	private float m_fDeltaTime;

	private TRACKING_DATA_TYPE m_eTrackingType;

	public NKMTrackingVector2()
	{
		m_NowValue = default(NKMVector2);
		m_BeforeValue = default(NKMVector2);
		m_StartValue = default(NKMVector2);
		m_TargetValue = default(NKMVector2);
		m_fTime = 0f;
		m_fDeltaTime = 0f;
		m_eTrackingType = TRACKING_DATA_TYPE.TDT_NORMAL;
	}

	public void SetNowValue(NKMVector2 NowValue)
	{
		m_NowValue = NowValue;
		m_TargetValue = NowValue;
		m_fDeltaTime = m_fTime;
	}

	public NKMVector2 GetNowValue()
	{
		return m_NowValue;
	}

	public NKMVector2 GetDelta()
	{
		return m_NowValue - m_BeforeValue;
	}

	public NKMVector2 GetBeforeValue()
	{
		return m_BeforeValue;
	}

	public NKMVector2 GetTargetValue()
	{
		return m_TargetValue;
	}

	public bool IsTracking()
	{
		return m_fDeltaTime < m_fTime;
	}

	public void Update(float deltaTime)
	{
		if (m_fDeltaTime < m_fTime)
		{
			m_fDeltaTime += deltaTime;
			float num = NKMTrackingFloat.TrackRatio(m_eTrackingType, m_fDeltaTime, m_fTime);
			m_BeforeValue = m_NowValue;
			m_NowValue.x = m_StartValue.x + (m_TargetValue.x - m_StartValue.x) * num;
			m_NowValue.y = m_StartValue.y + (m_TargetValue.y - m_StartValue.y) * num;
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

	public void SetTracking(NKMVector2 targetVal, float fTime, TRACKING_DATA_TYPE eTrackingType)
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
