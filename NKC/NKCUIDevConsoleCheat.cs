using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleCheat : NKCUIDevConsoleContentBase
{
	private enum DevConsoleCheatGroup
	{
		None = -1,
		UserInfo,
		Unit,
		Item,
		WorldMap,
		Guild,
		Max
	}

	private NKCUIDevConsoleContentBase[] m_Contents = new NKCUIDevConsoleContentBase[5];

	[Header("button")]
	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_WIN_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_LOSE_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_USER_INFO_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_UNIT_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_ITEM_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_WORLDMAP_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_GUILD_BUTTON;

	[Header("button more")]
	public GameObject m_BUTTON_MORE;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_MOO_JUCK;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_ACCOUNT_RESET;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSLOE_CHEAT_WARFARE_EXPIRED;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSLOE_CHEAT_DIVE_EXPIRED;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSLOE_CHEAT_PURCHASE_RESET;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSLOE_CHEAT_INVENTORY_RESET;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSLOE_CHEAT_PLZ_GAUNTLET_POINT;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSLOE_CHEAT_PVP_REWARD_WEEK;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSLOE_CHEAT_PVP_REWARD_SEASON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_WARFARE_UNBREAKABLE_MODE;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_MARQUEE_MESSAGE;

	[Header("PVP")]
	public InputField m_NKM_UI_DEV_CONSLOE_CHEAT_PVP_SCORE_INPUT_FIELD;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSLOE_CHEAT_PVP_SCORE_BUTTON;

	public NKCUIComStateButton m_btnPvpAsyncScoreButton;

	public NKCUIComStateButton m_btnPvpLeagueScoreButton;

	[Header("emoticon")]
	public InputField m_NKM_UI_DEV_CONSLOE_CHEAT_EMOTICON_ID_INPUT_FIELD;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSLOE_CHEAT_EMOTICON_ADD_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSLOE_CHEAT_EMOTICON_LIST_BUTTON;

	[Space]
	public NKCUIComToggle m_tgSkipEffect;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_DISCONNECT_TEST;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CONTRACT_EFFECT;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_AWAKEN_CONTRACT_EFFECT;

	[Header("reset")]
	public NKCUIComStateButton m_btnResetUnit;

	public NKCUIComStateButton m_btnResetShip;

	public NKCUIComStateButton m_btnResetMisc;

	public NKCUIComStateButton m_btnResetEquip;

	[Header("Contents")]
	public NKCUIDevConsoleContentBase m_NKM_UI_DEV_CONSOLE_CHEAT_USER_INFO;

	public NKCUIDevConsoleContentBase m_NKM_UI_DEV_CONSOLE_CHEAT_UNIT;

	public NKCUIDevConsoleContentBase m_NKM_UI_DEV_CONSOLE_CHEAT_ITEM;

	public NKCUIDevConsoleContentBase m_NKM_UI_DEV_CONSOLE_CHEAT_WORLDMAP;

	public NKCUIDevConsoleContentBase m_NKM_UI_DEV_CONSOLE_CHEAT_GUILD;

	public Text m_NKM_UI_DEV_CONSOLE_CHEAT_MOO_JUCK_text;

	public Text m_NKM_UI_DEV_CONSOLE_CHEAT_WARFARE_UNBREAKABLE_MODE_text;

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

	[Header("Etc")]
	public Text m_NKM_UI_DEV_CONTETNS_TAG;

	public NKCUIComStateButton m_btnShopBuyAll;

	[Header("Raid Point")]
	public NKCUIComStateButton m_RaidPointChange;

	public InputField m_TargetPoint;

	public NKCUIComToggle m_ResetRewardPoint;

	private bool isMooJuckMode;

	private bool isWarfareUnbreakableMode;
}
