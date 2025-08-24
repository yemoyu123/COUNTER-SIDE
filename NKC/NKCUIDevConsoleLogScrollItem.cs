using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleLogScrollItem : MonoBehaviour
{
	public Text m_Text;

	public void SetData(string text)
	{
		m_Text.text = text;
	}
}
