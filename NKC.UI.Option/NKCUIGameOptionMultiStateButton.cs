using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Option;

public class NKCUIGameOptionMultiStateButton : MonoBehaviour
{
	public delegate void OnClicked();

	private int m_Min;

	private int m_Max;

	private string[] m_TextTemplet;

	private int m_Value;

	public NKCUIComStateButton m_Button;

	public Text m_Text;

	private OnClicked dOnClicked;

	public void Init(int min, int max, int value, string[] textTemplet = null, OnClicked onClicked = null)
	{
		m_Min = min;
		m_Max = max;
		m_TextTemplet = textTemplet;
		dOnClicked = onClicked;
		m_Value = value;
		m_Button?.PointerClick.AddListener(OnClickButton);
		UpdateButtonText();
	}

	public int GetValue()
	{
		return m_Value;
	}

	public void ChangeValue(int value)
	{
		m_Value = value;
		UpdateButtonText();
	}

	private void UpdateButtonText()
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

	private void OnClickButton()
	{
		int value = m_Value;
		value++;
		if (value > m_Max)
		{
			value = m_Min;
		}
		ChangeValue(value);
		if (dOnClicked != null)
		{
			dOnClicked();
		}
	}
}
