using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCComGroupColor : MonoBehaviour
{
	private Image[] m_ImageArray;

	private Color[] m_ImageColorOrgArray;

	private SpriteRenderer[] m_SpriteRendererArray;

	private Color[] m_SpriteRendererColorOrgArray;

	private NKMTrackingFloat m_fR = new NKMTrackingFloat();

	private NKMTrackingFloat m_fG = new NKMTrackingFloat();

	private NKMTrackingFloat m_fB = new NKMTrackingFloat();

	private NKMTrackingFloat m_fA = new NKMTrackingFloat();

	private Color m_ColorTemp;

	private void Awake()
	{
		InitImage();
		InitSpriteRenderer();
		SetColor(1f, 1f, 1f, 1f);
	}

	private void Start()
	{
	}

	private void InitImage()
	{
		if (m_ImageArray == null)
		{
			m_ImageArray = base.gameObject.GetComponentsInChildren<Image>(includeInactive: true);
			m_ImageColorOrgArray = new Color[m_ImageArray.Length];
			for (int i = 0; i < m_ImageArray.Length; i++)
			{
				m_ImageColorOrgArray[i] = m_ImageArray[i].color;
			}
		}
	}

	private void InitSpriteRenderer()
	{
		if (m_SpriteRendererArray == null)
		{
			m_SpriteRendererArray = base.gameObject.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
			m_SpriteRendererColorOrgArray = new Color[m_SpriteRendererArray.Length];
			for (int i = 0; i < m_SpriteRendererArray.Length; i++)
			{
				m_SpriteRendererColorOrgArray[i] = m_SpriteRendererArray[i].color;
			}
		}
	}

	private void Update()
	{
		m_fR.Update(Time.deltaTime);
		m_fG.Update(Time.deltaTime);
		m_fB.Update(Time.deltaTime);
		m_fA.Update(Time.deltaTime);
		if (m_fR.IsTracking() || m_fG.IsTracking() || m_fB.IsTracking() || m_fA.IsTracking())
		{
			UpdateColor();
		}
	}

	public void SetColor(Color color, float fTime = 0f)
	{
		SetColor(color.r, color.g, color.b, color.a, fTime);
	}

	public void SetColor(float fR = -1f, float fG = -1f, float fB = -1f, float fA = -1f, float fTime = 0f)
	{
		bool flag = false;
		if (fR != -1f && m_fR.GetNowValue() != fR)
		{
			if (fTime == 0f)
			{
				m_fR.SetNowValue(fR);
			}
			else
			{
				m_fR.SetTracking(fR, fTime, TRACKING_DATA_TYPE.TDT_NORMAL);
			}
			flag = true;
		}
		if (fG != -1f && m_fG.GetNowValue() != fG)
		{
			if (fTime == 0f)
			{
				m_fG.SetNowValue(fG);
			}
			else
			{
				m_fG.SetTracking(fG, fTime, TRACKING_DATA_TYPE.TDT_NORMAL);
			}
			flag = true;
		}
		if (fB != -1f && m_fB.GetNowValue() != fB)
		{
			if (fTime == 0f)
			{
				m_fB.SetNowValue(fB);
			}
			else
			{
				m_fB.SetTracking(fB, fTime, TRACKING_DATA_TYPE.TDT_NORMAL);
			}
			flag = true;
		}
		if (fA != -1f && m_fA.GetNowValue() != fA)
		{
			if (fTime == 0f)
			{
				m_fA.SetNowValue(fA);
			}
			else
			{
				m_fA.SetTracking(fA, fTime, TRACKING_DATA_TYPE.TDT_NORMAL);
			}
			flag = true;
		}
		if (flag)
		{
			UpdateColor();
		}
	}

	private void UpdateColor()
	{
		if (m_ImageArray != null)
		{
			for (int i = 0; i < m_ImageArray.Length; i++)
			{
				m_ColorTemp.r = m_ImageColorOrgArray[i].r * m_fR.GetNowValue();
				m_ColorTemp.g = m_ImageColorOrgArray[i].g * m_fG.GetNowValue();
				m_ColorTemp.b = m_ImageColorOrgArray[i].b * m_fB.GetNowValue();
				m_ColorTemp.a = m_ImageColorOrgArray[i].a * m_fA.GetNowValue();
				m_ImageArray[i].color = m_ColorTemp;
			}
		}
		if (m_SpriteRendererArray != null)
		{
			for (int j = 0; j < m_SpriteRendererArray.Length; j++)
			{
				m_ColorTemp.r = m_SpriteRendererColorOrgArray[j].r * m_fR.GetNowValue();
				m_ColorTemp.g = m_SpriteRendererColorOrgArray[j].g * m_fG.GetNowValue();
				m_ColorTemp.b = m_SpriteRendererColorOrgArray[j].b * m_fB.GetNowValue();
				m_ColorTemp.a = m_SpriteRendererColorOrgArray[j].a * m_fA.GetNowValue();
				m_SpriteRendererArray[j].color = m_ColorTemp;
			}
		}
	}
}
