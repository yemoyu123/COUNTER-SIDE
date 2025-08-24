using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCPopupFilterSubUIMold : MonoBehaviour
{
	public delegate void OnFilterOptionChange(NKCMoldSortSystem.eFilterOption filterOption);

	[Header("MOLD_PARTS")]
	public GameObject m_objMoldParts;

	public NKCUIComToggle m_tglAll;

	public NKCUIComToggle m_tglWeapon;

	public NKCUIComToggle m_tglDefence;

	public NKCUIComToggle m_tglAcc;

	[Header("MOLD_TIER")]
	public GameObject m_objMoldTier;

	public NKCUIComToggle m_tglTier1;

	public NKCUIComToggle m_tglTier2;

	public NKCUIComToggle m_tglTier3;

	public NKCUIComToggle m_tglTier4;

	public NKCUIComToggle m_tglTier5;

	public NKCUIComToggle m_tglTier6;

	public NKCUIComToggle m_tglTier7;

	[Header("MOLD_TYPE")]
	public GameObject m_objMoldType;

	public NKCUIComToggle m_tglNormal;

	public NKCUIComToggle m_tglRaid;

	public NKCUIComToggle m_tglEtc;

	[Header("ENABLE_TYPE")]
	public GameObject m_objEnableType;

	public NKCUIComToggle m_tglEnable;

	public NKCUIComToggle m_tglDisable;

	[Header("유닛 타입")]
	public GameObject m_objUnitType;

	public NKCUIComToggle m_tglTypeCounter;

	public NKCUIComToggle m_tglTypeSoldier;

	public NKCUIComToggle m_tglTypeMechanic;

	public NKCUIComToggle m_tglTypeEtc;

	[Header("희귀도")]
	public GameObject m_objGrade;

	public NKCUIComToggle m_tglSSR;

	public NKCUIComToggle m_tglSR;

	public NKCUIComToggle m_tglR;

	public NKCUIComToggle m_tglN;

	[Header("MISC_ENABLE_TYPE")]
	public GameObject m_objMiscEnableType;

	public NKCUIComToggle m_tglMiscEnable;

	public NKCUIComToggle m_tglMiscDisable;

	private RectTransform m_RectTransform;

	private Dictionary<NKCMoldSortSystem.eFilterOption, NKCUIComToggle> m_dicFilterBtn = new Dictionary<NKCMoldSortSystem.eFilterOption, NKCUIComToggle>();

	private OnFilterOptionChange dOnFilterOptionChange;

	private bool m_bInitComplete;

	private bool m_bReset;

	public RectTransform RectTransform
	{
		get
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	private void Init()
	{
		m_dicFilterBtn.Clear();
		SetToggleListner(m_tglAll, NKCMoldSortSystem.eFilterOption.Mold_Parts_All);
		SetToggleListner(m_tglWeapon, NKCMoldSortSystem.eFilterOption.Mold_Parts_Weapon);
		SetToggleListner(m_tglDefence, NKCMoldSortSystem.eFilterOption.Mold_Parts_Defence);
		SetToggleListner(m_tglAcc, NKCMoldSortSystem.eFilterOption.Mold_Parts_Acc);
		SetToggleListner(m_tglTier1, NKCMoldSortSystem.eFilterOption.Mold_Tier_1);
		SetToggleListner(m_tglTier2, NKCMoldSortSystem.eFilterOption.Mold_Tier_2);
		SetToggleListner(m_tglTier3, NKCMoldSortSystem.eFilterOption.Mold_Tier_3);
		SetToggleListner(m_tglTier4, NKCMoldSortSystem.eFilterOption.Mold_Tier_4);
		SetToggleListner(m_tglTier5, NKCMoldSortSystem.eFilterOption.Mold_Tier_5);
		SetToggleListner(m_tglTier6, NKCMoldSortSystem.eFilterOption.Mold_Tier_6);
		SetToggleListner(m_tglTier7, NKCMoldSortSystem.eFilterOption.Mold_Tier_7);
		SetToggleListner(m_tglNormal, NKCMoldSortSystem.eFilterOption.Mold_Type_Normal);
		SetToggleListner(m_tglRaid, NKCMoldSortSystem.eFilterOption.Mold_Type_Raid);
		SetToggleListner(m_tglEtc, NKCMoldSortSystem.eFilterOption.Mold_Type_Etc);
		SetToggleListner(m_tglEnable, NKCMoldSortSystem.eFilterOption.Mold_Status_Enable);
		SetToggleListner(m_tglDisable, NKCMoldSortSystem.eFilterOption.Mold_Status_Disable);
		SetToggleListner(m_tglTypeCounter, NKCMoldSortSystem.eFilterOption.Mold_Unit_Counter);
		SetToggleListner(m_tglTypeSoldier, NKCMoldSortSystem.eFilterOption.Mold_Unit_Soldier);
		SetToggleListner(m_tglTypeMechanic, NKCMoldSortSystem.eFilterOption.Mold_Unit_Mechanic);
		SetToggleListner(m_tglTypeEtc, NKCMoldSortSystem.eFilterOption.Mold_Unit_Etc);
		SetToggleListner(m_tglSSR, NKCMoldSortSystem.eFilterOption.Mold_Grade_SSR);
		SetToggleListner(m_tglSR, NKCMoldSortSystem.eFilterOption.Mold_Grade_SR);
		SetToggleListner(m_tglR, NKCMoldSortSystem.eFilterOption.Mold_Grade_R);
		SetToggleListner(m_tglN, NKCMoldSortSystem.eFilterOption.Mold_Grade_N);
		m_bInitComplete = true;
	}

	private void SetToggleListner(NKCUIComToggle toggle, NKCMoldSortSystem.eFilterOption filterOption)
	{
		if (toggle != null)
		{
			m_dicFilterBtn.Add(filterOption, toggle);
			toggle.OnValueChanged.RemoveAllListeners();
			toggle.OnValueChanged.AddListener(delegate(bool value)
			{
				OnFilterButton(value, filterOption);
			});
		}
	}

	public void OpenFilterPopup(HashSet<NKCMoldSortSystem.eFilterOption> setFilterOption, OnFilterOptionChange onFilterOptionChange, List<string> lstFilter, bool bSelection = false)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		dOnFilterOptionChange = onFilterOptionChange;
		NKCUtil.SetGameobjectActive(m_objMoldParts, lstFilter.Contains("FT_EquipPosition"));
		NKCUtil.SetGameobjectActive(m_objMoldTier, lstFilter.Contains("FT_Tier"));
		NKCUtil.SetGameobjectActive(m_objMoldType, lstFilter.Contains("FT_ContentType"));
		NKCUtil.SetGameobjectActive(m_objEnableType, lstFilter.Contains("FT_Makeable"));
		NKCUtil.SetGameobjectActive(m_objUnitType, lstFilter.Contains("FT_UnitType"));
		NKCUtil.SetGameobjectActive(m_objGrade, lstFilter.Contains("FT_Grade"));
		NKCUtil.SetGameobjectActive(m_objMiscEnableType, bValue: false);
		SetFilter(setFilterOption);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	private void SetFilter(HashSet<NKCMoldSortSystem.eFilterOption> setFilterOption)
	{
		ResetFilter();
		m_bReset = true;
		foreach (NKCMoldSortSystem.eFilterOption item in setFilterOption)
		{
			if (m_dicFilterBtn.ContainsKey(item) && m_dicFilterBtn[item] != null)
			{
				m_dicFilterBtn[item].Select(bSelect: true);
			}
		}
		m_bReset = false;
	}

	private void OnFilterButton(bool bSelect, NKCMoldSortSystem.eFilterOption filterOption)
	{
		if (!m_dicFilterBtn.ContainsKey(filterOption))
		{
			return;
		}
		NKCUIComToggle nKCUIComToggle = m_dicFilterBtn[filterOption];
		if (nKCUIComToggle != null)
		{
			nKCUIComToggle.Select(bSelect, bForce: true, bImmediate: true);
			if (!m_bReset)
			{
				dOnFilterOptionChange?.Invoke(filterOption);
			}
		}
	}

	public void ResetFilter()
	{
		m_bReset = true;
		NKCUIComToggle[] componentsInChildren = base.transform.GetComponentsInChildren<NKCUIComToggle>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Select(bSelect: false);
		}
		m_bReset = false;
	}

	public void ResetMoldPartFilter(NKCMoldSortSystem.eFilterOption selectOption)
	{
		switch (selectOption)
		{
		case NKCMoldSortSystem.eFilterOption.Mold_Status_Enable:
			m_tglDisable.Select(bSelect: false, bForce: true);
			break;
		case NKCMoldSortSystem.eFilterOption.Mold_Status_Disable:
			m_tglEnable.Select(bSelect: false, bForce: true);
			break;
		}
	}
}
