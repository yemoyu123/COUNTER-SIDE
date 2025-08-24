using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIGameHudDevMenu : MonoBehaviour
{
	[Header("인게임 컨트롤 버튼")]
	public NKCUIComButton m_NUF_GAME_DEV_ALL_KILL;

	public NKCUIComButton m_NUF_GAME_DEV_ALL_KILL_ENEMY;

	public NKCUIComButton m_NUF_GAME_DEV_MENU;

	public NKCUIComButton m_NUF_GAME_DEV_AI_STOP_TEAM_A_BTN;

	public NKCUIComButton m_NUF_GAME_DEV_AI_STOP_TEAM_B_BTN;

	public NKCUIComButton m_NUF_GAME_DEV_FRAME_MOVE;

	public NKCUIComButton m_NUF_GAME_DEV_SKILL_NORMAL;

	public NKCUIComButton m_NUF_GAME_DEV_SKILL_NORMAL_ENEMY;

	public NKCUIComButton m_NUF_GAME_DEV_SKILL_SPECIAL;

	public NKCUIComButton m_NUF_GAME_DEV_SKILL_SPECIAL_ENEMY;

	public NKCUIComButton m_NUF_GAME_DEV_OPTION;

	public NKCUIComButton m_Force_Operator;

	[Header("메뉴 선택 버튼")]
	public NKCUIComToggle m_NUF_GAME_DEV_MENU_WINDOW_UNIT;

	public NKCUIComToggle m_NUF_GAME_DEV_MENU_WINDOW_SHIP;

	public NKCUIComToggle m_NUF_GAME_DEV_MENU_WINDOW_DUNGEON;

	public NKCUIComToggle m_NUF_GAME_DEV_MENU_WINDOW_OPER;

	public NKCUIComToggleGroup m_NUF_GAME_DEV_MENU_WINDOW_BTN_GROUP_ToggleGroup;

	[Header("상세 메뉴 페이지들")]
	public GameObject m_NUF_GAME_DEV_MENU_WINDOW;

	public GameObject m_goUnitRoot;

	public GameObject m_goShipRoot;

	public GameObject m_goDungeonRoot;

	public GameObject m_goOperatorRoot;

	[Header("유닛 페이지")]
	public NKCUIComToggleGroup m_tgDeckUnitList;

	public List<NKCUIGameHudDevUnit> m_listNKCUIGameHudDevDeckUnit = new List<NKCUIGameHudDevUnit>();

	public List<NKCUIGameHudDevUnit> m_listNKCUIGameHudDevDeckUnitEnemy = new List<NKCUIGameHudDevUnit>();

	public InputField m_ifUnitNameSearch;

	public NKCUIComToggle m_tglUnitNameSearch;

	public InputField m_ifUnitStriIDSearch;

	public NKCUIComToggle m_tghUnitStridSearch;

	public NKCUIComToggle m_tglUnitMonster;

	public NKCUIComToggle m_tglMonsterAutoRespawn;

	public NKCUIComButton m_btnUnitForceSpawn;

	public NKCUIComButton m_btnEnemyForceSpawn;

	public InputField m_ifTacticLevel;

	public NKCUIComStateButton m_btnMaxLevel;

	public NKCUIComToggle m_tglSkin;

	public NKCUIComStateButton m_btnDeckSave;

	public NKCUIComStateButton m_btnDeckLoad;

	[Header("던전 페이지")]
	public Dropdown m_NUF_GAME_DEV_MENU_WINDOW_DUNGEON_LIST_Dropdown;

	public NKCUIComStateButton m_NUF_GAME_DEV_MENU_WINDOW_DUNGEON_LIST_RELOAD_Btn;

	public Dropdown m_NUF_GAME_DEV_MENU_WINDOW_MAP_LIST_Dropdown;

	public NKCUIComStateButton m_NUF_GAME_DEV_MENU_WINDOW_MAP_LIST_RELOAD_Btn;

	[Header("함선 페이지")]
	public NKCUIComButton m_btnShipChange;

	public NKCUIComToggle m_tglShipTeam;

	public InputField m_ifShipNameSearch;

	public NKCUIComToggle m_tglShipNameSearch;

	public InputField m_ifShipStrIDSearch;

	public NKCUIComToggle m_tglShipStrIDSearch;

	[Header("오퍼레이터")]
	public InputField m_ifMainSkillLevel;

	public Dropdown m_dropdownOperSubSkill;

	public InputField m_ifSubSkillLevel;

	public NKCUIComButton m_btnOperChange;

	[Header("기타 버튼")]
	public Text m_lbSelectedUnit;

	public NKCUIComButton m_btnExit;

	public NKCUIComButton m_btnReset;

	public NKCUIComButton m_btnMoveToPatcher;

	private int m_unitLevel = 1;
}
