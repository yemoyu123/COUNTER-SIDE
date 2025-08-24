using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDevConsoleCheatUnit : NKCUIDevConsoleContentBase
{
	public Text m_lbUnitName;

	public NKCUIComStateButton m_csbtnSearch;

	public InputField m_ifSearch;

	[Header("Common")]
	public InputField m_ifLevel;

	public GameObject m_objLimitBreakLevel;

	public Dropdown m_ddLimitBreakLevel;

	[Header("유닛")]
	public GameObject m_objUnit;

	public InputField m_ifLoyalty;

	public NKCUIComToggle m_tglPermanentContract;

	public Dropdown m_ddTacticUpdateLevel;

	public Dropdown m_ddSkillNormal;

	public Dropdown m_ddSkillPassive;

	public Dropdown m_ddSkillSpecial;

	public Dropdown m_ddSkillUltimate;

	public Dropdown m_ddSkillLeader;

	[Header("함선")]
	public GameObject m_objShip;

	[Header("오퍼레이터")]
	public GameObject m_objOperator;

	public Text m_lbOperatorMainSkillName;

	public Dropdown m_ddOperatorMainSkillLevel;

	public Dropdown m_ddOperatorSubSkillLevel;

	public Dropdown m_ddOperatorSubSkill;

	private List<NKMOperatorSkillTemplet> m_lstOperatorSubSkill = new List<NKMOperatorSkillTemplet>();

	[Header("Buttons")]
	public NKCUIComStateButton m_btnAdd;

	public NKCUIComStateButton m_btnAddEnhanced;

	public NKCUIComStateButton m_btnAllAddUnit;

	public NKCUIComStateButton m_btnAllAddUnitEnhanced;

	public NKCUIComStateButton m_btnAllAddShip;

	public NKCUIComStateButton m_btnAllAddOperator;

	private bool CheckUnitListFilter(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return false;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase == null)
		{
			return false;
		}
		if (!unitTempletBase.m_bContractable && unitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return false;
		}
		if (m_ifSearch != null && !string.IsNullOrEmpty(m_ifSearch.text))
		{
			string text = m_ifSearch.text;
			bool flag = false;
			if (int.TryParse(text, out var result) && unitTempletBase.m_UnitID == result)
			{
				flag = true;
			}
			if (unitTempletBase.m_UnitStrID.ToLower().Contains(text.ToLower()) || unitTempletBase.GetUnitName().Contains(text))
			{
				flag = true;
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}
}
