using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleLogFileScrollItem : MonoBehaviour
{
	public Text m_Text;

	public NKCUIComStateButton m_Button;

	public void SetData(string fullPathForFilename)
	{
		m_Text.text = Path.GetFileName(fullPathForFilename);
	}
}
