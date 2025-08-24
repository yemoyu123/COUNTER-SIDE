using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Option;

public class NKCUIGameOptionSliderWithButton : MonoBehaviour
{
	private enum ButtonType
	{
		Plus,
		Minus,
		Change
	}

	public delegate void OnChanged();

	private int m_Min;

	private int m_Max;

	private string[] m_TextTemplet;

	private int m_Value;

	private bool m_bDisabled;

	public Slider m_Slider;

	public Image m_SliderHandle;

	public Color m_OriginalHandleColor;

	public Color m_DisabledHandleColor;

	public NKCUIComStateButton m_PlusButton;

	public NKCUIComStateButton m_MinusButton;

	public NKCUIComStateButton m_ChangeButton;

	public Text m_Text;

	private OnChanged dOnChanged;

	private int m_WarningValue;

	private string m_WarningTilte;

	private string m_WarningDesc;

	private int m_lastValue;

	public void Init(int min, int max, int value, string[] textTemplet = null, OnChanged onChanged = null)
	{
		m_Min = min;
		m_Max = max;
		m_TextTemplet = textTemplet;
		dOnChanged = onChanged;
		m_Value = value;
		m_WarningValue = max + 1;
		m_lastValue = value;
		m_Slider.maxValue = max;
		m_Slider.minValue = min;
		m_Slider.value = value;
		m_Slider.onValueChanged.AddListener(OnValueChangedSlider);
		m_PlusButton?.PointerClick.AddListener(delegate
		{
			OnClickButton(ButtonType.Plus);
		});
		m_MinusButton?.PointerClick.AddListener(delegate
		{
			OnClickButton(ButtonType.Minus);
		});
		m_ChangeButton?.PointerClick.AddListener(delegate
		{
			OnClickButton(ButtonType.Change);
		});
		NKCUtil.SetLabelText(m_Text, value.ToString());
	}

	public bool isDisabled()
	{
		return m_bDisabled;
	}

	public int GetValue()
	{
		return m_Value;
	}

	public void ChangeValue(int value)
	{
		if (value < m_Min)
		{
			m_Value = m_Min;
		}
		else if (value > m_Max)
		{
			m_Value = m_Max;
		}
		else
		{
			m_Value = value;
		}
		m_lastValue = m_Value;
		m_Slider.value = m_Value;
		UpdateButtonText();
		if (dOnChanged != null)
		{
			dOnChanged();
		}
	}

	private void ChangeValueWithWarning(int value)
	{
		if (value != m_lastValue && value >= m_WarningValue)
		{
			ShowWarning(value);
		}
		else
		{
			ChangeValue(value);
		}
	}

	public void SetMax(int value)
	{
		m_Max = value;
		if (m_Value > value)
		{
			m_Value = value;
		}
		m_Slider.maxValue = value;
		ChangeValue(m_Value);
	}

	public void SetDisabled(bool disabled, string text = "")
	{
		m_bDisabled = disabled;
		m_Slider.interactable = !disabled;
		if (disabled)
		{
			m_Text.text = text;
			if (m_SliderHandle != null)
			{
				m_SliderHandle.color = m_DisabledHandleColor;
			}
		}
		else if (m_SliderHandle != null)
		{
			m_SliderHandle.color = m_OriginalHandleColor;
		}
	}

	public void UpdateButtonText()
	{
		if (m_TextTemplet != null)
		{
			m_Text.text = m_TextTemplet[m_Value];
		}
		else
		{
			m_Text.text = m_Value.ToString();
		}
	}

	private void OnValueChangedSlider(float value)
	{
		if (!m_bDisabled)
		{
			ChangeValueWithWarning((int)value);
		}
	}

	private void OnClickButton(ButtonType buttonType)
	{
		if (m_bDisabled)
		{
			m_bDisabled = false;
			m_Slider.interactable = true;
			m_SliderHandle.color = m_OriginalHandleColor;
		}
		switch (buttonType)
		{
		case ButtonType.Plus:
			if (++m_Value > m_Max)
			{
				m_Value = m_Max;
			}
			ChangeValueWithWarning(m_Value);
			break;
		case ButtonType.Minus:
			ChangeValueWithWarning(--m_Value);
			break;
		case ButtonType.Change:
		{
			int value = m_Value;
			value++;
			if (value > m_Max)
			{
				value = m_Min;
			}
			ChangeValueWithWarning(value);
			break;
		}
		}
	}

	public void SetWarningPopup(int warningValue, string warningTitle, string warningDesc)
	{
		m_WarningValue = warningValue;
		m_WarningTilte = warningTitle;
		m_WarningDesc = warningDesc;
	}

	private void ShowWarning(int value)
	{
		NKCPopupOKCancel.OpenOKCancelBox(m_WarningTilte, m_WarningDesc, delegate
		{
			ChangeValue(value);
		}, delegate
		{
			m_Value = m_lastValue;
			ChangeValue(m_Value);
		});
	}
}
