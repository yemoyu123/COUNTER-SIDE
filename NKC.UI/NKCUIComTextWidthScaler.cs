using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

[RequireComponent(typeof(Text))]
public class NKCUIComTextWidthScaler : MonoBehaviour
{
	private Text m_Text;

	private string LastText;

	private void Start()
	{
		m_Text = GetComponent<Text>();
		if (m_Text == null)
		{
			base.enabled = false;
			return;
		}
		LastText = m_Text.text;
		NKCUtil.SetLabelWidthScale(ref m_Text);
	}

	private void Update()
	{
		if (m_Text != null && LastText != m_Text.text)
		{
			NKCUtil.SetLabelWidthScale(ref m_Text);
		}
	}
}
