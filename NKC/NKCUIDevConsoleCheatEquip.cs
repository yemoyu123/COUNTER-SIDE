using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleCheatEquip : NKCUIDevConsoleContentBase
{
	public Text m_NKM_UI_DEV_CONSOLE_CHEAT_EQUIP_NAME_LABEL;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_EQUIP_SEARCH_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DEV_CONSOLE_CHEAT_EQUIP_ADD_BUTTON;

	public Dropdown m_ddEnhanceLevel;

	public NKCUIComToggle m_toggleUseCustomOption;

	public GameObject m_objOption;

	public Dropdown m_ddOption1;

	public IReadOnlyList<NKMEquipRandomStatTemplet> m_lstOption1;

	public InputField m_ifOptionPrecision1;

	public Dropdown m_ddOption2;

	public IReadOnlyList<NKMEquipRandomStatTemplet> m_lstOption2;

	public InputField m_ifOptionPrecision2;

	public NKCUIComToggle m_toggleUseCustomPotentialOption;

	public GameObject m_objPotentialOption;

	public Dropdown m_ddPotentialOption;

	public Dropdown m_ddPotentialOption2;

	public IReadOnlyList<NKMPotentialOptionTemplet> m_lstPotentialOption;

	public IReadOnlyList<NKMPotentialOptionTemplet> m_lstPotentialOption2;

	public GameObject m_objPotentialSocket1;

	public InputField m_ifPotentialSocketPrecision1;

	public GameObject m_objPotentialSocket2;

	public InputField m_ifPotentialSocketPrecision2;

	public GameObject m_objPotentialSocket3;

	public InputField m_ifPotentialSocketPrecision3;

	public Dropdown m_ddSetOption;

	public InputField m_ifSearch;
}
