using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleCheatUserInfoController : MonoBehaviour
{
	public delegate void OnConfirmCallBack();

	public NKCUIComStateButton m_ConfirmButton;

	public NKCUIComStateButton m_ConfirmButton2;

	public InputField m_CountInputField;
}
