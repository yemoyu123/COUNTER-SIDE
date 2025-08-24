using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleContentTest : NKCUIDevConsoleContentBase2
{
	[Header("KILLCOUNT")]
	public NKCUIComStateButton m_USER_KILLCOUNT_RESET;

	public NKCUIComStateButton m_SERVER_KILLCOUNT_RESET;

	public NKCUIComStateButton m_KILLCOUNT_REWARD_RESET;

	public InputField m_USER_KILLCOUNT;

	public InputField m_SERVER_KILLCOUNT;

	public InputField m_USER_REWARD_STEP;

	public InputField m_SERVER_REWARD_STEP;

	public InputField m_KILLCOUNT_ID;

	public NKCUIComStateButton m_USER_KILLCOUNT_APPLY;

	public NKCUIComStateButton m_SERVER_KILLCOUNT_APPLY;

	public NKCUIComStateButton m_KILLCOUNT_REWARD_STEP_APPLY;

	[Header("Raid Point")]
	public NKCUIComStateButton m_RaidPointChange;

	public InputField m_TargetPoint;

	public NKCUIComToggle m_ResetRewardPoint;
}
