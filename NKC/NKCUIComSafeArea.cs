using System.Collections.Generic;
using UnityEngine;

namespace NKC;

[DisallowMultipleComponent]
public class NKCUIComSafeArea : MonoBehaviour
{
	private RectTransform m_RectTransform;

	private static bool m_sbCalculatedSafeArea = false;

	private static float m_sfSafeAreaLeft = 0f;

	private static float m_sfSafeAreaRight = 0f;

	private static float m_sfSafeAreaUp = 0f;

	private static float m_sfSafeAreaBottom = 0f;

	private static float m_sfSafeAreaScale = 1f;

	private static float m_sfSafeAreaLeftPrev = 0f;

	private static float m_sfSafeAreaRightPrev = 0f;

	private static float m_sfSafeAreaUpPrev = 0f;

	private static float m_sfSafeAreaBottomPrev = 0f;

	public bool m_bUseLeft;

	public bool m_bUseRight;

	public bool m_bUseTop;

	public bool m_bUseBottom;

	public bool m_bUseScale;

	public bool m_bUseRectSide;

	public bool m_bAdjustRectSidePos;

	public float m_fAddRectSide;

	public bool m_bUseRectHeight;

	public bool m_bAdjustRectHeightPos;

	public float m_fAddRectHeight;

	private Vector2 m_vOriginalSize = Vector2.one;

	private bool m_bSizeChanged;

	private bool m_bInit;

	private bool m_bInitUI;

	private static Dictionary<string, float> s_dicDeviceSafeArea = new Dictionary<string, float>();

	private static bool s_bCompleteAddDevice = false;

	public static void RevertCalculatedSafeArea()
	{
		m_sbCalculatedSafeArea = false;
	}

	public bool CheckInit()
	{
		return m_bInit;
	}

	private static bool IsZlongAndroidDevice()
	{
		if (NKCDefineManager.DEFINE_ZLONG() && NKCDefineManager.DEFINE_ANDROID())
		{
			return !NKCDefineManager.DEFINE_UNITY_EDITOR();
		}
		return false;
	}

	private void Awake()
	{
		m_RectTransform = base.gameObject.GetComponent<RectTransform>();
		if (!s_bCompleteAddDevice && IsZlongAndroidDevice())
		{
			s_bCompleteAddDevice = true;
			s_dicDeviceSafeArea.Add("OPPO R15", 80f);
			s_dicDeviceSafeArea.Add("OPPO CPH1903", 54f);
		}
	}

	public static void InitSafeArea()
	{
		if (!m_sbCalculatedSafeArea)
		{
			m_sbCalculatedSafeArea = true;
			m_sfSafeAreaLeftPrev = m_sfSafeAreaLeft;
			m_sfSafeAreaRightPrev = m_sfSafeAreaRight;
			m_sfSafeAreaUpPrev = m_sfSafeAreaUp;
			m_sfSafeAreaBottomPrev = m_sfSafeAreaBottom;
			m_sfSafeAreaLeft = Screen.safeArea.x;
			m_sfSafeAreaRight = (float)Screen.width - (Screen.safeArea.x + Screen.safeArea.width);
			m_sfSafeAreaUp = (float)Screen.height - (Screen.safeArea.y + Screen.safeArea.height);
			m_sfSafeAreaBottom = Screen.safeArea.y;
			float width = Screen.safeArea.width;
			float height = Screen.safeArea.height;
			Vector2 vector = new Vector2
			{
				x = width / (float)Screen.currentResolution.width,
				y = height / (float)Screen.currentResolution.height
			};
			if (vector.x > vector.y)
			{
				vector.x = vector.y;
			}
			else
			{
				vector.y = vector.x;
			}
			m_sfSafeAreaScale = vector.x;
		}
	}

	private static Vector2 GetSafeAreaPos(Vector2 vec2, bool left, bool right, bool top, bool bottom, bool bWidth, bool bHeight)
	{
		InitSafeArea();
		if (left)
		{
			vec2.x += m_sfSafeAreaLeft;
		}
		if (right)
		{
			vec2.x -= m_sfSafeAreaRight;
		}
		if (top)
		{
			vec2.y -= m_sfSafeAreaUp;
		}
		if (bottom)
		{
			vec2.y += m_sfSafeAreaBottom;
		}
		if (bWidth)
		{
			vec2.x += (m_sfSafeAreaLeft - m_sfSafeAreaRight) * 0.5f;
		}
		if (bHeight)
		{
			vec2.y += (m_sfSafeAreaUp + m_sfSafeAreaBottom) * 0.5f;
		}
		return vec2;
	}

	public void SetInit()
	{
		m_bInit = true;
		m_bInitUI = true;
	}

	public Vector2 GetSafeAreaPos(Vector2 vec2)
	{
		return GetSafeAreaPos(vec2, m_bUseLeft, m_bUseRight, m_bUseTop, m_bUseBottom, m_bAdjustRectSidePos, m_bAdjustRectHeightPos);
	}

	public static Vector2 GetSafeAreaScale(Vector2 vec2)
	{
		InitSafeArea();
		vec2.x = m_sfSafeAreaScale;
		vec2.y = m_sfSafeAreaScale;
		return vec2;
	}

	public Vector3 GetSafeAreaScale()
	{
		return new Vector3(m_sfSafeAreaScale, m_sfSafeAreaScale, 1f);
	}

	public float GetSafeAreaWidth(float width)
	{
		if (m_bUseRectSide)
		{
			width -= m_sfSafeAreaLeft + m_sfSafeAreaRight + m_fAddRectSide;
			m_RectTransform.SetWidth(width);
		}
		return width;
	}

	public float GetSafeAreaHeight(float height)
	{
		if (m_bUseRectHeight && m_sfSafeAreaBottom > 0f)
		{
			height -= m_sfSafeAreaBottom + m_fAddRectHeight;
			m_RectTransform.SetHeight(height);
		}
		return height;
	}

	private void Start()
	{
		SetSafeAreaBase();
	}

	public void SetSafeAreaBase()
	{
		if (!m_bInit && IsSafeAreaRequired())
		{
			m_bInit = true;
			SetSafeArea();
		}
	}

	public void SetSafeAreaUI()
	{
		if (!m_bInitUI && IsSafeAreaRequired())
		{
			m_bInitUI = true;
			m_bInit = true;
			SetSafeArea();
		}
	}

	public void Rollback()
	{
		if (m_bInit)
		{
			if (m_RectTransform == null)
			{
				return;
			}
			Vector2 anchoredPosition = m_RectTransform.anchoredPosition;
			if (m_bUseLeft)
			{
				anchoredPosition.x -= m_sfSafeAreaLeftPrev;
			}
			if (m_bUseRight)
			{
				anchoredPosition.x += m_sfSafeAreaRightPrev;
			}
			if (m_bUseTop)
			{
				anchoredPosition.y += m_sfSafeAreaUpPrev;
			}
			if (m_bUseBottom)
			{
				anchoredPosition.y -= m_sfSafeAreaBottomPrev;
			}
			if (m_bAdjustRectSidePos)
			{
				anchoredPosition.x -= (m_sfSafeAreaLeftPrev - m_sfSafeAreaRightPrev) * 0.5f;
			}
			if (m_bAdjustRectHeightPos)
			{
				anchoredPosition.y -= (m_sfSafeAreaUpPrev + m_sfSafeAreaBottomPrev) * 0.5f;
			}
			m_RectTransform.anchoredPosition = anchoredPosition;
			if (m_bUseScale)
			{
				Vector3 localScale = m_RectTransform.localScale;
				localScale.x = 1f;
				localScale.y = 1f;
				m_RectTransform.localScale = localScale;
			}
			if (m_bUseRectHeight)
			{
				float height = m_RectTransform.GetHeight();
				height += m_sfSafeAreaBottomPrev + m_fAddRectHeight;
				m_RectTransform.SetHeight(height);
			}
			if (m_bUseRectSide)
			{
				float width = m_RectTransform.GetWidth();
				width += m_sfSafeAreaLeftPrev + m_sfSafeAreaRightPrev + m_fAddRectSide;
				m_RectTransform.SetWidth(width);
			}
			m_bSizeChanged = false;
		}
		m_bInit = false;
	}

	public void SetSafeArea()
	{
		if (IsSafeAreaRequired() && !(m_RectTransform == null))
		{
			Vector3 localScale = m_RectTransform.localScale;
			Vector2 vec = new Vector2(localScale.x, localScale.y);
			if (m_bUseScale)
			{
				vec = GetSafeAreaScale(vec);
			}
			localScale.x = vec.x;
			localScale.y = vec.y;
			m_RectTransform.localScale = localScale;
			vec = m_RectTransform.anchoredPosition;
			vec = GetSafeAreaPos(vec);
			if (!m_bSizeChanged)
			{
				m_bSizeChanged = true;
				m_vOriginalSize.x = m_RectTransform.GetWidth();
				m_vOriginalSize.y = m_RectTransform.GetHeight();
			}
			float safeAreaWidth = GetSafeAreaWidth(m_vOriginalSize.x);
			m_RectTransform.SetWidth(safeAreaWidth);
			float safeAreaHeight = GetSafeAreaHeight(m_vOriginalSize.y);
			m_RectTransform.SetHeight(safeAreaHeight);
			m_RectTransform.anchoredPosition = vec;
		}
	}

	private static bool IsSafeAreaRequired()
	{
		if (Screen.safeArea.width == (float)Screen.width)
		{
			return Screen.safeArea.height != (float)Screen.height;
		}
		return true;
	}
}
