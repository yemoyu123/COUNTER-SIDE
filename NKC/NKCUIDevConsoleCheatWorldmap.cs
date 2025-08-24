using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleCheatWorldmap : NKCUIDevConsoleContentBase
{
	[Serializable]
	public class CityMenu
	{
		public int CityID;

		public NKCUIComStateButton CityButton;

		public Text CityName;
	}

	public List<CityMenu> m_cityMenuList;

	public GameObject m_objCityInfo;

	public Text m_txtName;

	public Text m_txtLeader;

	public Text m_txtMission;

	public NKCUIComStateButton m_btnMission;

	public Text m_txtEvent;

	public NKCUIComStateButton m_btnEvent;

	public Text m_txtBuildingPoint;

	public InputField m_ifTargetBuilding;

	public InputField m_ifTargetEvent;

	public NKCUIComStateButton m_btnTargetBuild;

	public NKCUIComStateButton m_btnTargetUpgrade;

	public NKCUIComStateButton m_btnTargetRemove;

	public Dropdown m_ddDungeonType;

	public Dropdown m_ddDungeonRank;

	public NKCUIComStateButton m_btnDungeonCreate;

	public NKCUIComStateButton m_btnDungeonCreateByID;

	public NKCUIComStateButton m_btnDungeonExpire;

	public NKCUIComStateButton m_btnLvUp;

	public NKCUIComStateButton m_btnLvDown;
}
