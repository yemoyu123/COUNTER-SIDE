using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIGameHUDMultiplyReward : MonoBehaviour
{
	public Text m_txt;

	public void SetMultiply(int multiply)
	{
		if (multiply > 1)
		{
			NKCUtil.SetLabelText(m_txt, NKCUtilString.GET_MULTIPLY_REWARD_ONE_PARAM, multiply);
		}
	}
}
