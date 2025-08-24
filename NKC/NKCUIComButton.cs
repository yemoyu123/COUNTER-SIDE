using System.Collections.Generic;
using NKC.UI;
using NKM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NKC;

public class NKCUIComButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler
{
	public delegate void OnPointerHolding();

	public HotkeyEventType m_HotkeyEventType;

	public NKCUnityEvent PointerDown = new NKCUnityEvent();

	public UnityEvent PointerUp;

	public UnityEvent PointerClick;

	private float m_fOrgSizeX = 1f;

	private float m_fOrgSizeY = 1f;

	public float m_fTouchSize = 0.9f;

	public float m_fSelectSize = 1.05f;

	public float m_fTrackingTime = 0.1f;

	public bool m_bLock;

	public bool m_bSelect;

	public bool m_bSelectByClick = true;

	public GameObject m_ButtonBG_On;

	public GameObject m_ButtonBG_Off;

	public GameObject m_ButtonBG_Lock;

	public GameObject m_ButtonBG_UnLock;

	private GameObject m_objTempRaycaster;

	public int m_DataInt;

	private RectTransform m_RectTransform;

	private NKMTrackingFloat m_Scale = new NKMTrackingFloat();

	public string m_SoundForPointClick = "FX_UI_BUTTON_SELECT";

	private NKCUIComButton m_LinkButton;

	protected NKCUIComStateButtonBase.ButtonState m_eCurrentState;

	public float m_fDelayHold = 0.3f;

	public OnPointerHolding dOnPointerHolding;

	private float m_fHoldingTime;

	private Vector2 m_touchPos = Vector2.zero;

	private NKCUIBase m_uiRoot;

	private void Start()
	{
		m_uiRoot = NKCUIManager.FindRootUIBase(base.transform);
	}

	public void SetLinkButton(NKCUIComButton cNKCUIComButton)
	{
		m_LinkButton = cNKCUIComButton;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		m_Scale.SetNowValue(1f);
		m_Scale.SetTracking(m_fTouchSize, m_fTrackingTime, TRACKING_DATA_TYPE.TDT_SLOWER);
		SetButtonState(NKCUIComStateButtonBase.ButtonState.Pressed);
		if (m_fTouchSize < 1f)
		{
			MakeTempRayCaster();
		}
		m_fHoldingTime = 0f;
		m_touchPos = eventData.position;
		if (PointerDown != null)
		{
			PointerDown.Invoke(eventData);
		}
		if (m_LinkButton != null)
		{
			m_LinkButton.OnPointerDown(eventData);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!m_bSelect)
		{
			m_Scale.SetTracking(1f, m_fTrackingTime, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		else
		{
			m_Scale.SetTracking(m_fSelectSize, m_fTrackingTime, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		if (PointerUp != null)
		{
			PointerUp.Invoke();
		}
		if (m_LinkButton != null)
		{
			m_LinkButton.OnPointerUp(eventData);
		}
		ClearTempRayCaster();
		ClearHolding();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (m_eCurrentState == NKCUIComStateButtonBase.ButtonState.Pressed)
		{
			if (m_bSelectByClick)
			{
				Select(bSelect: true);
			}
			else
			{
				SetButtonState(m_bSelect ? NKCUIComStateButtonBase.ButtonState.Selected : NKCUIComStateButtonBase.ButtonState.Normal);
			}
			if (!string.IsNullOrEmpty(m_SoundForPointClick))
			{
				NKCSoundManager.PlaySound(m_SoundForPointClick, 1f, 0f, 0f);
			}
			if (PointerClick != null)
			{
				PointerClick.Invoke();
			}
			if (m_LinkButton != null)
			{
				m_LinkButton.OnPointerClick(eventData);
			}
		}
		else if (m_eCurrentState == NKCUIComStateButtonBase.ButtonState.Locked)
		{
			if (PointerClick != null)
			{
				PointerClick.Invoke();
			}
			if (m_LinkButton != null)
			{
				m_LinkButton.OnPointerClick(eventData);
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (m_eCurrentState == NKCUIComStateButtonBase.ButtonState.Pressed)
		{
			SetButtonState(m_bSelect ? NKCUIComStateButtonBase.ButtonState.Selected : NKCUIComStateButtonBase.ButtonState.Normal);
		}
		ClearHolding();
	}

	private void StartHolding(Vector2 touchPos)
	{
		if (dOnPointerHolding != null)
		{
			NKCUIHoldLoading.Instance.Open(touchPos);
		}
	}

	private void OnHoldEvent()
	{
		if (m_eCurrentState == NKCUIComStateButtonBase.ButtonState.Pressed)
		{
			SetButtonState(m_bSelect ? NKCUIComStateButtonBase.ButtonState.Selected : NKCUIComStateButtonBase.ButtonState.Normal);
		}
		dOnPointerHolding?.Invoke();
		ClearHolding();
	}

	private void ClearHolding()
	{
		if (NKCUIHoldLoading.IsOpen)
		{
			NKCUIHoldLoading.Instance.Close();
		}
		m_fHoldingTime = 0f;
	}

	public void UpdateOrgSize()
	{
		m_RectTransform = base.gameObject.GetComponentInChildren<RectTransform>();
		m_fOrgSizeX = m_RectTransform.localScale.x;
		m_fOrgSizeY = m_RectTransform.localScale.y;
		m_Scale.SetNowValue(1f);
	}

	private void Awake()
	{
		Select(m_bSelect, bForce: true);
		UpdateOrgSize();
		UpdateScale();
	}

	private void OnDisable()
	{
		m_Scale.StopTracking();
		if (m_RectTransform != null)
		{
			m_RectTransform.localScale = new Vector3(m_fOrgSizeX, m_fOrgSizeY, 1f);
		}
	}

	public void Update()
	{
		m_Scale.Update(Time.deltaTime);
		UpdateScale();
		if (m_eCurrentState == NKCUIComStateButtonBase.ButtonState.Pressed)
		{
			if (dOnPointerHolding == null)
			{
				return;
			}
			if (Input.touchCount > 1)
			{
				ClearHolding();
				return;
			}
			if (m_fHoldingTime < m_fDelayHold)
			{
				m_fHoldingTime += Time.deltaTime;
				return;
			}
			if (!NKCUIHoldLoading.IsOpen)
			{
				StartHolding(m_touchPos);
				return;
			}
			if (!NKCUIHoldLoading.Instance.IsPlaying())
			{
				OnHoldEvent();
			}
		}
		if (m_bLock || m_HotkeyEventType == HotkeyEventType.None || !NKCUIManager.CanUIProcessHotkey(m_uiRoot))
		{
			return;
		}
		if (NKCInputManager.CheckHotKeyEvent(m_HotkeyEventType))
		{
			Vector3 vector = NKCCamera.GetSubUICamera().WorldToScreenPoint(base.transform.GetComponent<RectTransform>().GetCenterWorldPos());
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = new Vector2(vector.x, vector.y);
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, list);
			if (list.Count > 0 && list[0].gameObject.transform.IsChildOf(base.transform))
			{
				NKCInputManager.ConsumeHotKeyEvent(m_HotkeyEventType);
				if (PointerClick != null)
				{
					PointerClick.Invoke();
				}
				if (m_LinkButton != null)
				{
					m_LinkButton.OnPointerClick(pointerEventData);
				}
			}
		}
		if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.ShowHotkey))
		{
			NKCUIComHotkeyDisplay.OpenInstance(base.transform, m_HotkeyEventType);
		}
	}

	private void UpdateScale()
	{
		if (m_Scale.IsTracking() && m_RectTransform != null)
		{
			Vector3 localScale = m_RectTransform.localScale;
			localScale.Set(m_fOrgSizeX * m_Scale.GetNowValue(), m_fOrgSizeY * m_Scale.GetNowValue(), 1f);
			m_RectTransform.localScale = localScale;
		}
	}

	public void Select(bool bSelect, bool bForce = false, bool bImmediate = false)
	{
		if (m_bLock || (m_bSelect == bSelect && !bForce))
		{
			return;
		}
		if (!bSelect)
		{
			if (bImmediate)
			{
				m_Scale.StopTracking();
				if (m_RectTransform != null)
				{
					m_RectTransform.localScale = new Vector3(m_fOrgSizeX, m_fOrgSizeY, 1f);
				}
			}
			else
			{
				m_Scale.SetTracking(1f, m_fTrackingTime, TRACKING_DATA_TYPE.TDT_SLOWER);
			}
			if (m_ButtonBG_On != null && m_ButtonBG_On.activeSelf)
			{
				m_ButtonBG_On.SetActive(value: false);
			}
			if (m_ButtonBG_Off != null && !m_ButtonBG_Off.activeSelf)
			{
				m_ButtonBG_Off.SetActive(value: true);
			}
			SetButtonState(NKCUIComStateButtonBase.ButtonState.Normal);
		}
		else
		{
			if (bImmediate)
			{
				m_Scale.StopTracking();
				if (m_RectTransform != null)
				{
					m_RectTransform.localScale = new Vector3(m_fOrgSizeX * m_fSelectSize, m_fOrgSizeY * m_fSelectSize, 1f);
				}
			}
			else
			{
				m_Scale.SetTracking(m_fSelectSize, m_fTrackingTime, TRACKING_DATA_TYPE.TDT_SLOWER);
			}
			if (m_ButtonBG_On != null && !m_ButtonBG_On.activeSelf)
			{
				m_ButtonBG_On.SetActive(value: true);
			}
			if (m_ButtonBG_Off != null && m_ButtonBG_Off.activeSelf)
			{
				m_ButtonBG_Off.SetActive(value: false);
			}
			SetButtonState(NKCUIComStateButtonBase.ButtonState.Selected);
		}
		m_bSelect = bSelect;
	}

	public void Lock()
	{
		if (!m_bLock)
		{
			Select(bSelect: false);
			if (m_ButtonBG_On != null && m_ButtonBG_On.activeSelf)
			{
				m_ButtonBG_On.SetActive(value: false);
			}
			if (m_ButtonBG_Off != null && m_ButtonBG_Off.activeSelf)
			{
				m_ButtonBG_Off.SetActive(value: false);
			}
			if (m_ButtonBG_UnLock != null && m_ButtonBG_UnLock.activeSelf)
			{
				m_ButtonBG_UnLock.SetActive(value: false);
			}
			if (m_ButtonBG_Lock != null && !m_ButtonBG_Lock.activeSelf)
			{
				m_ButtonBG_Lock.SetActive(value: true);
			}
			m_bLock = true;
			SetButtonState(NKCUIComStateButtonBase.ButtonState.Locked);
		}
	}

	public void UnLock()
	{
		if (m_bLock)
		{
			if (m_ButtonBG_UnLock != null && !m_ButtonBG_UnLock.activeSelf)
			{
				m_ButtonBG_UnLock.SetActive(value: true);
			}
			if (m_ButtonBG_Lock != null && m_ButtonBG_Lock.activeSelf)
			{
				m_ButtonBG_Lock.SetActive(value: false);
			}
			m_bLock = false;
			SetButtonState(NKCUIComStateButtonBase.ButtonState.Normal);
			Select(m_bSelect, bForce: true);
		}
	}

	private void MakeTempRayCaster()
	{
		if (m_objTempRaycaster == null)
		{
			RectTransform component = GetComponent<RectTransform>();
			if (component != null)
			{
				m_objTempRaycaster = new GameObject("TempRaycaster");
				RectTransform rectTransform = m_objTempRaycaster.AddComponent<RectTransform>();
				rectTransform.SetParent(base.transform, worldPositionStays: false);
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.offsetMin = component.GetSize() * -0.2f;
				rectTransform.offsetMax = component.GetSize() * 0.2f;
				m_objTempRaycaster.AddComponent<NKCUIComRaycastTarget>().raycastTarget = true;
			}
		}
	}

	private void ClearTempRayCaster()
	{
		if (m_objTempRaycaster != null)
		{
			Object.Destroy(m_objTempRaycaster);
			m_objTempRaycaster = null;
		}
	}

	private void SetButtonState(NKCUIComStateButtonBase.ButtonState state)
	{
		m_eCurrentState = state;
	}
}
