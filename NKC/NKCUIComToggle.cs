using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC;

public class NKCUIComToggle : NKCUIComStateButtonBase
{
	public delegate void ValueChangedWithData(bool value, int data);

	public NKCUnityEvent PointerDown = new NKCUnityEvent();

	public Toggle.ToggleEvent OnValueChanged;

	public ValueChangedWithData OnValueChangedWithData;

	public NKCUIComToggleGroup m_ToggleGroup;

	private bool m_bReverseSeqCallbackCall;

	public bool m_bChecked => m_bSelect;

	public void SetbReverseSeqCallbackCall(bool bSet)
	{
		m_bReverseSeqCallbackCall = bSet;
	}

	protected override void OnPointerDownEvent(PointerEventData eventData)
	{
		if (PointerDown != null)
		{
			PointerDown.Invoke(eventData);
		}
	}

	protected override void OnPointerUpEvent(PointerEventData eventData)
	{
	}

	protected override void OnPointerClickEvent(PointerEventData eventData)
	{
		if (m_bSelect && m_ToggleGroup != null && !m_ToggleGroup.m_bAllowSwitchOff)
		{
			SetButtonState(ButtonState.Selected);
		}
		else if (m_eCurrentState != ButtonState.Locked || m_bGetCallbackWhileLocked)
		{
			Select(!m_bSelect);
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (m_bSelect && m_ToggleGroup != null && !m_ToggleGroup.m_bAllowSwitchOff)
		{
			SetButtonState(ButtonState.Selected);
		}
		else if (m_eCurrentState == ButtonState.Pressed)
		{
			if (!string.IsNullOrEmpty(m_SoundForPointClick))
			{
				NKCSoundManager.PlaySound(m_SoundForPointClick, 1f, 0f, 0f);
			}
			Select(!m_bSelect);
		}
		else if (m_eCurrentState == ButtonState.Locked && m_bGetCallbackWhileLocked)
		{
			OnValueChanged?.Invoke(m_bSelect);
			OnValueChangedWithData?.Invoke(m_bSelect, m_DataInt);
		}
	}

	private void Awake()
	{
		if (m_ToggleGroup != null)
		{
			m_ToggleGroup.RegisterToggle(this);
		}
		Select(m_bSelect, bForce: true);
		UpdateOrgSize();
		UpdateScale();
	}

	public void SetToggleGroup(NKCUIComToggleGroup group)
	{
		if (m_ToggleGroup != null)
		{
			m_ToggleGroup.DeregisterToggle(this);
		}
		m_ToggleGroup = group;
		if (m_ToggleGroup != null)
		{
			m_ToggleGroup.RegisterToggle(this);
		}
	}

	public override bool Select(bool bSelect, bool bForce = false, bool bImmediate = false)
	{
		if (base.Select(bSelect, bForce, bImmediate))
		{
			if (m_bReverseSeqCallbackCall)
			{
				if (bSelect && m_ToggleGroup != null)
				{
					m_ToggleGroup.OnCheckOneToggle(this);
				}
				if (!bForce)
				{
					OnValueChanged?.Invoke(m_bSelect);
					OnValueChangedWithData?.Invoke(m_bSelect, m_DataInt);
				}
			}
			else
			{
				if (!bForce)
				{
					OnValueChanged?.Invoke(m_bSelect);
					OnValueChangedWithData?.Invoke(m_bSelect, m_DataInt);
				}
				if (bSelect && m_ToggleGroup != null)
				{
					m_ToggleGroup.OnCheckOneToggle(this);
				}
			}
			return true;
		}
		return false;
	}
}
