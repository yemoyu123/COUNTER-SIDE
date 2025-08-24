using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleContentPVP : NKCUIDevConsoleContentBase2
{
	[Header("PVP")]
	public InputField m_NKM_UI_DEV_CONSLOE_CHEAT_PVP_SCORE_INPUT_FIELD;

	public NKCUIComStateButton m_PVP_RANK_SCORE_BUTTON;

	public NKCUIComStateButton m_PVP_ASYNC_SCORE_BUTTON;

	public NKCUIComStateButton m_PVP_LEAGUE_SCORE_BUTTON;

	public Text m_lbCurStateName;

	public NKCUIComStateButton m_btnNextInterval;

	public NKCUIComStateButton m_btnResetInterval;

	public NKCUIComStateButton m_btnResetTryoutCheckCount;
}
