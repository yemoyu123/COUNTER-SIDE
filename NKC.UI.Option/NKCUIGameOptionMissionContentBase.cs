using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Option;

public class NKCUIGameOptionMissionContentBase : MonoBehaviour
{
	public GameObject m_Condition;

	public Image m_ConditionImage_1;

	public Image m_ConditionImage_2;

	public Image m_ConditionImage_3;

	public GameObject m_Button;

	public NKCUIComStateButton m_GiveUpButton;

	public NKCUIComStateButton m_LeaveButton;

	public Text m_NAME_TEXT;

	public Text m_NAME_SUB_TEXT;

	public Text m_LV_COUNT;

	public Text m_RANDOM_SET_COUNT;
}
