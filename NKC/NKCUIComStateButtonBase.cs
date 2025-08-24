using System.Collections.Generic;
using NKC.UI;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NKC;

public abstract class NKCUIComStateButtonBase : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler
{
	public enum ButtonState
	{
		Normal,
		Pressed,
		Selected,
		Locked
	}

	public enum PressedEffectType
	{
		Single,
		Additional,
		Effect
	}

	public delegate void OnPointerHolding();

	public delegate void OnPointerHoldPress();

	protected ButtonState m_eCurrentState;

	private float m_fOrgSizeX = 1f;

	private float m_fOrgSizeY = 1f;

	public float m_fTouchSize = 0.9f;

	public float m_fSelectSize = 1f;

	public float m_fTrackingTime = 0.1f;

	public HotkeyEventType m_HotkeyEventType;

	public bool m_bUseHotkeyUpDownEvent;

	public bool m_bLock;

	public bool m_bGetCallbackWhileLocked;

	[FormerlySerializedAs("m_bChecked")]
	public bool m_bSelect;

	public bool m_bSelectByClick;

	[FormerlySerializedAs("m_ButtonBG_Off")]
	public GameObject m_ButtonBG_Normal;

	[FormerlySerializedAs("m_ButtonBG_On")]
	public GameObject m_ButtonBG_Selected;

	[FormerlySerializedAs("m_ButtonBG_Lock")]
	public GameObject m_ButtonBG_Locked;

	public GameObject m_ButtonBG_Pressed;

	public PressedEffectType m_ePressedEffectIsAdditional;

	public int m_DataInt;

	protected RectTransform m_RectTransform;

	private NKMTrackingFloat m_Scale = new NKMTrackingFloat();

	public string m_SoundForPointClick = "FX_UI_BUTTON_SELECT";

	public Text[] m_aTitle;

	public Image[] m_aImage;

	private GameObject m_objTempRaycaster;

	public float m_fDelayHold = 0.3f;

	public OnPointerHolding dOnPointerHolding;

	[Header("버튼 홀드 이벤트 호출 속도 조절")]
	public float m_fPressGapMax = 0.4f;

	public float m_fPressGapMin = 0.01f;

	public float m_fDamping = 0.8f;

	public int m_iFastRepeatValue = 1;

	public OnPointerHoldPress dOnPointerHoldPress;

	private float m_fHoldingTime;

	private float m_fPressGap;

	private Vector2 m_touchPos = Vector2.zero;

	private NKCUIBase m_uiRoot;

	public bool IsSelected => m_bSelect;

	protected abstract void OnPointerDownEvent(PointerEventData eventData);

	protected abstract void OnPointerUpEvent(PointerEventData eventData);

	protected abstract void OnPointerClickEvent(PointerEventData eventData);

	protected virtual void OnPointerExitEvent(PointerEventData eventData)
	{
	}

	private void Start()
	{
		if (m_uiRoot == null)
		{
			m_uiRoot = NKCUIManager.FindRootUIBase(base.transform);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		m_Scale.SetNowValue(1f);
		m_Scale.SetTracking(m_fTouchSize, m_fTrackingTime, TRACKING_DATA_TYPE.TDT_SLOWER);
		SetButtonState(ButtonState.Pressed);
		if (m_fTouchSize < 1f)
		{
			MakeTempRayCaster();
		}
		m_fHoldingTime = 0f;
		m_fPressGap = m_fPressGapMax;
		m_touchPos = eventData.position;
		OnPointerDownEvent(eventData);
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
		OnPointerUpEvent(eventData);
		ClearTempRayCaster();
		ClearHolding();
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (m_eCurrentState == ButtonState.Pressed)
		{
			if (m_bSelectByClick)
			{
				Select(bSelect: true);
			}
			else
			{
				SetButtonState(m_bSelect ? ButtonState.Selected : ButtonState.Normal);
			}
			if (!string.IsNullOrEmpty(m_SoundForPointClick))
			{
				NKCSoundManager.PlaySound(m_SoundForPointClick, 1f, 0f, 0f);
			}
			OnPointerClickEvent(eventData);
		}
		else if (m_eCurrentState == ButtonState.Locked && m_bGetCallbackWhileLocked)
		{
			OnPointerClickEvent(eventData);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (m_eCurrentState == ButtonState.Pressed)
		{
			SetButtonState(m_bSelect ? ButtonState.Selected : ButtonState.Normal);
		}
		ClearHolding();
		OnPointerExitEvent(eventData);
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
		if (m_eCurrentState == ButtonState.Pressed)
		{
			SetButtonState(m_bSelect ? ButtonState.Selected : ButtonState.Normal);
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

	public void Update()
	{
		m_Scale.Update(Time.deltaTime);
		UpdateScale();
		if (m_eCurrentState == ButtonState.Pressed)
		{
			if (dOnPointerHolding != null)
			{
				if (Input.touchCount > 1)
				{
					ClearHolding();
				}
				else if (m_fHoldingTime < m_fDelayHold)
				{
					m_fHoldingTime += Time.deltaTime;
				}
				else if (!NKCUIHoldLoading.IsOpen)
				{
					StartHolding(m_touchPos);
				}
				else if (!NKCUIHoldLoading.Instance.IsPlaying())
				{
					OnHoldEvent();
				}
				return;
			}
			if (dOnPointerHoldPress != null)
			{
				if (Input.touchCount > 1)
				{
					m_fHoldingTime = 0f;
				}
				else
				{
					ProcessHold();
				}
				return;
			}
		}
		if ((m_bLock && !m_bGetCallbackWhileLocked) || m_HotkeyEventType == HotkeyEventType.None || !NKCUIManager.CanUIProcessHotkey(m_uiRoot))
		{
			return;
		}
		PointerEventData raycastEvent2;
		if (NKCInputManager.CheckHotKeyEvent(m_HotkeyEventType))
		{
			m_fHoldingTime = 0f;
			m_fPressGap = m_fPressGapMax;
			if (m_bUseHotkeyUpDownEvent && CanCastRaycast(out var raycastEvent))
			{
				OnPointerDownEvent(raycastEvent);
			}
		}
		else if (NKCInputManager.CheckHotKeyUp(m_HotkeyEventType) && m_bUseHotkeyUpDownEvent && CanCastRaycast(out raycastEvent2))
		{
			OnPointerUpEvent(raycastEvent2);
		}
		if (NKCInputManager.CheckHotKeyEvent(m_HotkeyEventType))
		{
			if (CanCastRaycast(out var raycastEvent3))
			{
				NKCInputManager.ConsumeHotKeyEvent(m_HotkeyEventType);
				OnPointerClickEvent(raycastEvent3);
			}
		}
		else if (NKCInputManager.IsHotkeyPressed(m_HotkeyEventType) && dOnPointerHoldPress != null)
		{
			ProcessHold();
			return;
		}
		if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.ShowHotkey))
		{
			NKCUIComHotkeyDisplay.OpenInstance(base.transform, m_HotkeyEventType);
		}
	}

	private void ProcessHold()
	{
		m_fHoldingTime += Time.deltaTime;
		if (m_fHoldingTime > m_fPressGap)
		{
			m_fPressGap *= m_fDamping;
			if (m_iFastRepeatValue < 1)
			{
				m_iFastRepeatValue = 1;
			}
			int num = ((!(m_fPressGap < m_fPressGapMin)) ? 1 : m_iFastRepeatValue);
			m_fPressGap = Mathf.Clamp(m_fPressGap, m_fPressGapMin, m_fPressGapMax);
			m_fHoldingTime = 0f;
			for (int i = 0; i < num; i++)
			{
				dOnPointerHoldPress();
			}
		}
	}

	public bool CanCastRaycast(out PointerEventData raycastEvent)
	{
		Vector3 vector = NKCCamera.GetSubUICamera().WorldToScreenPoint(base.transform.GetComponent<RectTransform>().GetCenterWorldPos());
		raycastEvent = new PointerEventData(EventSystem.current);
		raycastEvent.position = new Vector2(vector.x, vector.y);
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(raycastEvent, list);
		if (list.Count > 0)
		{
			return list[0].gameObject.transform.IsChildOf(base.transform);
		}
		return false;
	}

	private void OnDisable()
	{
		ClearHolding();
		SetButtonState(m_bSelect ? ButtonState.Selected : ButtonState.Normal);
		m_Scale.StopTracking();
		if (m_RectTransform != null)
		{
			m_RectTransform.localScale = new Vector3(m_fOrgSizeX, m_fOrgSizeY, 1f);
		}
	}

	protected void UpdateScale()
	{
		if (m_Scale.IsTracking() && m_RectTransform != null)
		{
			Vector3 localScale = m_RectTransform.localScale;
			localScale.Set(m_fOrgSizeX * m_Scale.GetNowValue(), m_fOrgSizeY * m_Scale.GetNowValue(), 1f);
			m_RectTransform.localScale = localScale;
		}
	}

	public virtual bool Select(bool bSelect, bool bForce = false, bool bImmediate = false)
	{
		if (m_bLock)
		{
			return false;
		}
		if (m_bSelect != bSelect || bForce)
		{
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
				SetButtonState(ButtonState.Normal);
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
				SetButtonState(ButtonState.Selected);
			}
			m_bSelect = bSelect;
			return true;
		}
		return false;
	}

	public void SetLock(bool value, bool bForce = false)
	{
		if (value)
		{
			Lock(bForce);
		}
		else
		{
			UnLock(bForce);
		}
	}

	public void Lock(bool bForce = false)
	{
		if (!m_bLock || bForce)
		{
			Select(bSelect: false);
			m_bLock = true;
			SetButtonState(ButtonState.Locked);
		}
	}

	public void UnLock(bool bForce = false)
	{
		if (m_bLock || bForce)
		{
			m_bLock = false;
			SetButtonState(ButtonState.Normal);
			Select(m_bSelect, bForce: true);
		}
	}

	public ButtonState GetButtonState()
	{
		return m_eCurrentState;
	}

	protected void SetButtonState(ButtonState state)
	{
		m_eCurrentState = state;
		if (m_bLock)
		{
			m_eCurrentState = ButtonState.Locked;
		}
		switch (m_eCurrentState)
		{
		case ButtonState.Normal:
			SetObject(ButtonState.Normal);
			break;
		case ButtonState.Pressed:
			if (m_ButtonBG_Pressed == null)
			{
				SetObject(m_bSelect ? ButtonState.Selected : ButtonState.Normal);
			}
			else
			{
				SetObject(ButtonState.Pressed);
			}
			break;
		case ButtonState.Selected:
			if (m_ButtonBG_Selected == null)
			{
				SetObject(ButtonState.Normal);
			}
			else
			{
				SetObject(ButtonState.Selected);
			}
			break;
		case ButtonState.Locked:
			if (m_ButtonBG_Locked == null)
			{
				SetObject(ButtonState.Normal);
			}
			else
			{
				SetObject(ButtonState.Locked);
			}
			break;
		}
	}

	private void SetObject(ButtonState state)
	{
		switch (m_ePressedEffectIsAdditional)
		{
		case PressedEffectType.Single:
			NKCUtil.SetGameobjectActive(m_ButtonBG_Normal, state == ButtonState.Normal);
			NKCUtil.SetGameobjectActive(m_ButtonBG_Pressed, state == ButtonState.Pressed);
			NKCUtil.SetGameobjectActive(m_ButtonBG_Selected, state == ButtonState.Selected);
			NKCUtil.SetGameobjectActive(m_ButtonBG_Locked, state == ButtonState.Locked);
			break;
		case PressedEffectType.Additional:
			if (state == ButtonState.Pressed)
			{
				if (!m_bLock)
				{
					NKCUtil.SetGameobjectActive(m_ButtonBG_Pressed, bValue: true);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_ButtonBG_Normal, state == ButtonState.Normal);
				NKCUtil.SetGameobjectActive(m_ButtonBG_Pressed, bValue: false);
				NKCUtil.SetGameobjectActive(m_ButtonBG_Selected, state == ButtonState.Selected);
				NKCUtil.SetGameobjectActive(m_ButtonBG_Locked, state == ButtonState.Locked);
			}
			break;
		case PressedEffectType.Effect:
			if (state == ButtonState.Pressed)
			{
				if (!m_bLock)
				{
					NKCUtil.SetGameobjectActive(m_ButtonBG_Pressed, bValue: false);
					NKCUtil.SetGameobjectActive(m_ButtonBG_Pressed, bValue: true);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_ButtonBG_Normal, state == ButtonState.Normal);
				NKCUtil.SetGameobjectActive(m_ButtonBG_Selected, state == ButtonState.Selected);
				NKCUtil.SetGameobjectActive(m_ButtonBG_Locked, state == ButtonState.Locked);
			}
			break;
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

	public void SetTitleText(string text)
	{
		if (m_aTitle != null)
		{
			for (int i = 0; i < m_aTitle.Length; i++)
			{
				NKCUtil.SetLabelText(m_aTitle[i], text);
			}
		}
	}

	public void SetImage(Sprite spr)
	{
		if (m_aImage != null)
		{
			for (int i = 0; i < m_aImage.Length; i++)
			{
				NKCUtil.SetImageSprite(m_aImage[i], spr, bDisableIfSpriteNull: true);
			}
		}
	}

	public void SetHotkey(HotkeyEventType hotkey, NKCUIBase uiBase = null, bool bUpDownEvent = false)
	{
		m_HotkeyEventType = hotkey;
		if (uiBase != null)
		{
			m_uiRoot = uiBase;
		}
		m_bUseHotkeyUpDownEvent = bUpDownEvent;
	}
}
