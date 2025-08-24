using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Trim;

public class NKCUITrimToolTip : NKCUIBase
{
	private enum PivotType
	{
		None,
		RightUp,
		RightDown,
		LeftUp,
		LeftDown
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_TRIM";

	private const string UI_ASSET_NAME = "AB_UI_TRIM_TOOLTIP";

	private static NKCUITrimToolTip m_Instance;

	public RectTransform m_rectTransform;

	public RectTransform m_rtPanel;

	public LayoutElement m_layoutElement;

	public Text m_lbText;

	public GameObject m_objLeftTail;

	public GameObject m_objRightTail;

	public GameObject m_objLeftUpTail;

	public GameObject m_objRightUpTail;

	public float m_touchOffset;

	public static NKCUITrimToolTip Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUITrimToolTip>("AB_UI_TRIM", "AB_UI_TRIM_TOOLTIP", NKCUIManager.eUIBaseRect.UIOverlay, CleanupInstance).GetInstance<NKCUITrimToolTip>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override string MenuName => "Trim ToolTip";

	public override eMenutype eUIType => eMenutype.Overlay;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
	}

	private void Update()
	{
		Camera subUICamera = NKCCamera.GetSubUICamera();
		CanvasScaler uIFrontCanvasScaler = NKCUIManager.GetUIFrontCanvasScaler();
		RectTransform rectTransform = m_lbText.rectTransform;
		Vector3 position = rectTransform.position;
		float num = uIFrontCanvasScaler.referenceResolution.x / uIFrontCanvasScaler.referenceResolution.y;
		float num2 = Mathf.Lerp(subUICamera.aspect / num, 1f, uIFrontCanvasScaler.matchWidthOrHeight);
		float num3 = position.x + m_rtPanel.rect.width * 0.5f * num2;
		float num4 = position.x - m_rtPanel.rect.width * 0.5f * num2;
		float num5 = subUICamera.orthographicSize * subUICamera.aspect;
		float num6 = num5 * 0.05f;
		if (m_layoutElement != null)
		{
			if (num5 < num3)
			{
				float num7 = num3 - num5 + num6;
				m_layoutElement.preferredWidth = rectTransform.rect.width - num7;
			}
			else if (0f - num5 > num4)
			{
				float num8 = 0f - num5 - num4 + num6;
				m_layoutElement.preferredWidth = rectTransform.rect.width - num8;
			}
		}
		if (!Input.anyKey)
		{
			Close();
		}
	}

	public void Open(string message, Vector2? touchPos, int fontSize = 30, float preferredWidth = 800f)
	{
		if (!base.IsOpen)
		{
			NKCUtil.SetLabelText(m_lbText, message);
			m_lbText.fontSize = fontSize;
			if (m_lbText.preferredWidth >= preferredWidth)
			{
				m_layoutElement.preferredWidth = -1f;
			}
			else
			{
				m_layoutElement.preferredWidth = preferredWidth;
			}
			UIOpened();
			SetPosition(touchPos);
		}
	}

	private void SetPosition(Vector2? touchPos)
	{
		PivotType pivotType = VectorToPivotType(touchPos);
		Vector3 zero = Vector3.zero;
		if (touchPos.HasValue)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(m_rectTransform, touchPos.Value, NKCCamera.GetSubUICamera(), out var localPoint);
			zero.x = localPoint.x;
			zero.y = localPoint.y;
			zero.z = 0f;
		}
		Vector3 localPosition;
		switch (pivotType)
		{
		default:
			m_rtPanel.pivot = new Vector2(0.5f, 0.5f);
			localPosition = Vector3.zero;
			NKCUtil.SetGameobjectActive(m_objLeftTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRightTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLeftUpTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRightUpTail, bValue: false);
			break;
		case PivotType.RightUp:
			m_rtPanel.pivot = new Vector2(1f, 1f);
			localPosition = zero + new Vector3(0f, -1f, 0f) * m_touchOffset;
			NKCUtil.SetGameobjectActive(m_objLeftTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRightTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLeftUpTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRightUpTail, bValue: true);
			break;
		case PivotType.RightDown:
			m_rtPanel.pivot = new Vector2(1f, 0f);
			localPosition = zero + new Vector3(0f, 1f, 0f) * m_touchOffset;
			NKCUtil.SetGameobjectActive(m_objLeftTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRightTail, bValue: true);
			NKCUtil.SetGameobjectActive(m_objLeftUpTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRightUpTail, bValue: false);
			break;
		case PivotType.LeftUp:
			m_rtPanel.pivot = new Vector2(0f, 1f);
			localPosition = zero + new Vector3(0f, -1f, 0f) * m_touchOffset;
			NKCUtil.SetGameobjectActive(m_objLeftTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRightTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLeftUpTail, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRightUpTail, bValue: false);
			break;
		case PivotType.LeftDown:
			m_rtPanel.pivot = new Vector2(0f, 0f);
			localPosition = zero + new Vector3(0f, 1f, 0f) * m_touchOffset;
			NKCUtil.SetGameobjectActive(m_objLeftTail, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRightTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLeftUpTail, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRightUpTail, bValue: false);
			break;
		}
		m_rtPanel.localPosition = localPosition;
	}

	private PivotType VectorToPivotType(Vector2? touchPos)
	{
		if (!touchPos.HasValue)
		{
			return PivotType.None;
		}
		Vector2 value = touchPos.Value;
		float num = (float)Screen.width * 0.5f;
		float num2 = (float)Screen.height * 0.5f;
		if (value.x > num)
		{
			if (value.y > num2)
			{
				return PivotType.RightUp;
			}
			return PivotType.RightDown;
		}
		if (value.y > num2)
		{
			return PivotType.LeftUp;
		}
		return PivotType.LeftDown;
	}
}
