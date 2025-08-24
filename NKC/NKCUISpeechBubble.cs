using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUISpeechBubble : MonoBehaviour
{
	public Text m_lbText;

	public Image m_imgBubble;

	public void SetText(string text)
	{
		NKCUtil.SetLabelText(m_lbText, text);
	}
}
