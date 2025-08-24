using NKC.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIFadeInOut
{
	public delegate void FadeCallBack();

	private static bool m_bOpen;

	private static GameObject m_NKM_FADE_IN_OUT;

	private static CanvasRenderer m_CanvasRenderer;

	private static Image m_Image;

	private static bool m_bFadeIn;

	private static bool m_bFadeOut;

	private static float m_fTotalTime;

	private static float m_fElapsedTime;

	private static FadeCallBack m_CallBack;

	private static bool m_bReservedTimeOutForFadeOut;

	private static bool m_bTimeOutForFadeOut;

	private static float m_fTimeOutForFadeOut;

	public static void InitUI()
	{
		m_NKM_FADE_IN_OUT = NKCUIManager.OpenUI("NKM_FADE_IN_OUT");
		m_CanvasRenderer = m_NKM_FADE_IN_OUT.GetComponent<CanvasRenderer>();
		m_Image = m_NKM_FADE_IN_OUT.GetComponent<Image>();
		if (m_NKM_FADE_IN_OUT.activeSelf)
		{
			m_NKM_FADE_IN_OUT.SetActive(value: false);
		}
	}

	private static void InitFade(float fTime)
	{
		if (m_NKM_FADE_IN_OUT != null && !m_NKM_FADE_IN_OUT.activeSelf)
		{
			m_NKM_FADE_IN_OUT.SetActive(value: true);
		}
		m_fTotalTime = fTime;
		m_fElapsedTime = 0f;
		m_bReservedTimeOutForFadeOut = false;
		m_bTimeOutForFadeOut = false;
		m_fTimeOutForFadeOut = 0f;
	}

	public static void Finish()
	{
		if (m_bFadeIn)
		{
			if (m_NKM_FADE_IN_OUT != null && m_NKM_FADE_IN_OUT.activeSelf)
			{
				m_NKM_FADE_IN_OUT.SetActive(value: false);
			}
			if (m_Image != null)
			{
				NKCUtil.SetImageColor(m_Image, new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 1f));
			}
			m_bTimeOutForFadeOut = false;
			m_bReservedTimeOutForFadeOut = false;
			m_fTimeOutForFadeOut = 0f;
		}
		else if (m_bFadeOut)
		{
			if (m_NKM_FADE_IN_OUT != null && !m_NKM_FADE_IN_OUT.activeSelf)
			{
				m_NKM_FADE_IN_OUT.SetActive(value: true);
			}
			if (m_Image != null)
			{
				NKCUtil.SetImageColor(m_Image, new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 1f));
			}
			if (m_bReservedTimeOutForFadeOut)
			{
				m_bReservedTimeOutForFadeOut = false;
				m_bTimeOutForFadeOut = true;
			}
		}
		m_bFadeOut = false;
		m_bFadeIn = false;
		m_CallBack = null;
	}

	public static void Close(bool bDoCallBack = false)
	{
		if (m_NKM_FADE_IN_OUT != null && m_NKM_FADE_IN_OUT.activeSelf)
		{
			m_NKM_FADE_IN_OUT.SetActive(value: false);
		}
		if (m_Image != null)
		{
			NKCUtil.SetImageColor(m_Image, new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 1f));
		}
		m_bFadeOut = false;
		m_bFadeIn = false;
		if (bDoCallBack && m_CallBack != null)
		{
			m_CallBack();
		}
		m_CallBack = null;
		m_bReservedTimeOutForFadeOut = false;
		m_fTimeOutForFadeOut = 0f;
	}

	public static bool IsFinshed()
	{
		if (!m_bFadeOut && !m_bFadeIn)
		{
			return true;
		}
		return false;
	}

	public static void FadeIn(float fTime, FadeCallBack fadeCallBack = null, bool bWhite = false)
	{
		if (bWhite)
		{
			if (m_Image != null)
			{
				m_Image.color = Color.white;
			}
		}
		else if (m_Image != null)
		{
			m_Image.color = Color.black;
		}
		if (m_Image != null)
		{
			NKCUtil.SetImageColor(m_Image, new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 1f));
		}
		InitFade(fTime);
		m_bFadeIn = true;
		m_bFadeOut = false;
		m_CallBack = fadeCallBack;
	}

	public static void FadeOut(float fTime, FadeCallBack fadeCallBack = null, bool bWhite = false, float fTimeOutForFadeOut = -1f)
	{
		if (bWhite)
		{
			if (m_Image != null)
			{
				m_Image.color = Color.white;
			}
		}
		else if (m_Image != null)
		{
			m_Image.color = Color.black;
		}
		if (m_Image != null)
		{
			NKCUtil.SetImageColor(m_Image, new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 0f));
		}
		InitFade(fTime);
		m_bFadeOut = true;
		m_bFadeIn = false;
		if (fTimeOutForFadeOut > 0f)
		{
			m_bReservedTimeOutForFadeOut = true;
			m_fTimeOutForFadeOut = fTimeOutForFadeOut;
		}
		m_CallBack = fadeCallBack;
	}

	public static void Update(float deltaTime)
	{
		if (deltaTime > 0.04f)
		{
			deltaTime = 0.04f;
		}
		if (m_bFadeIn)
		{
			if (m_fElapsedTime > m_fTotalTime)
			{
				m_bFadeIn = false;
				if (m_NKM_FADE_IN_OUT != null && m_NKM_FADE_IN_OUT.activeSelf)
				{
					m_NKM_FADE_IN_OUT.SetActive(value: false);
				}
				if (m_CallBack != null)
				{
					m_CallBack();
				}
				m_CallBack = null;
			}
			float num = (m_fTotalTime - m_fElapsedTime) / m_fTotalTime;
			if (num < 0f)
			{
				num = 0f;
			}
			if (num > 1f)
			{
				num = 1f;
			}
			if (m_Image != null)
			{
				NKCUtil.SetImageColor(m_Image, new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, num));
			}
			m_fElapsedTime += deltaTime;
		}
		else if (m_bFadeOut)
		{
			m_fElapsedTime += deltaTime;
			float num2 = m_fElapsedTime / m_fTotalTime;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (m_Image != null)
			{
				NKCUtil.SetImageColor(m_Image, new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, num2));
			}
			if (m_fElapsedTime > m_fTotalTime)
			{
				m_bFadeOut = false;
				if (m_bReservedTimeOutForFadeOut)
				{
					m_bReservedTimeOutForFadeOut = false;
					m_bTimeOutForFadeOut = true;
				}
				if (m_CallBack != null)
				{
					m_CallBack();
				}
				m_CallBack = null;
			}
		}
		if (m_bTimeOutForFadeOut)
		{
			m_fTimeOutForFadeOut -= deltaTime;
			if (m_fTimeOutForFadeOut <= 0f)
			{
				m_bTimeOutForFadeOut = false;
				Close();
			}
		}
	}
}
