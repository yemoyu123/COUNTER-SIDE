using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Option;

public class NKCUIGameOptionMenuButton : MonoBehaviour
{
	public delegate void OnSelected();

	public NKCUIComToggle m_Toggle;

	public Image m_Icon;

	public Text m_Text;

	public Text m_SubText;

	private Color m_OriginalIconColor;

	private Color m_OriginalTextColor;

	private Color m_OriginalSubTextColor;

	private Color m_SelectedIconColor;

	private Color m_SelectedTextColor;

	private Color m_SelectedSubTextColor;

	private OnSelected dOnSelected;

	public void Init(Color selectedIconColor, Color selectedTextColor, Color selectedSubTextColor, OnSelected onSelected = null)
	{
		m_OriginalIconColor = m_Icon.color;
		m_OriginalTextColor = m_Text.color;
		m_OriginalSubTextColor = m_SubText.color;
		m_SelectedIconColor = selectedIconColor;
		m_SelectedTextColor = selectedTextColor;
		m_SelectedSubTextColor = selectedSubTextColor;
		dOnSelected = onSelected;
		m_Toggle.OnValueChanged.AddListener(Select);
	}

	private void Select(bool bSelect)
	{
		if (bSelect)
		{
			m_Icon.color = m_SelectedIconColor;
			m_Text.color = m_SelectedTextColor;
			m_SubText.color = m_SelectedSubTextColor;
			dOnSelected();
		}
		else
		{
			m_Icon.color = m_OriginalIconColor;
			m_Text.color = m_OriginalTextColor;
			m_SubText.color = m_OriginalSubTextColor;
		}
	}
}
