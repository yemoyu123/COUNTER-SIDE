using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupFilterSubUIOperatorToken : MonoBehaviour
{
	public delegate void OnFilterOptionChange(NKCOperatorTokenSortSystem ssActive);

	[Header("\ufffdش\ufffd \ufffd\ufffd\ufffd\ufffd\ufffdտ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffdϴ°\u0378\ufffd \ufffd\ufffd\ufffd\ufffd")]
	[Header("Passive Skill")]
	public GameObject m_objPassiveSkill;

	public NKCPopupFilterSubUIOperatorPassiveSlot m_passiveSlot;

	[Header("Rarity")]
	public GameObject m_objRare;

	public NKCUIComToggle m_tglRare_SSR;

	public NKCUIComToggle m_tglRare_SR;

	public NKCUIComToggle m_tglRare_R;

	public NKCUIComToggle m_tglRare_N;

	[Header("Passive Skill")]
	public GameObject m_objSubFilter;

	public NKCPopupFilterSubUIOperatorPassive m_subFilter;

	private RectTransform m_RectTransform;

	private Dictionary<NKCOperatorTokenSortSystem.eFilterOption, NKCPopupFilterSubUIOperatorPassiveSlot> m_dicFilterPassiveSlot = new Dictionary<NKCOperatorTokenSortSystem.eFilterOption, NKCPopupFilterSubUIOperatorPassiveSlot>();

	private Dictionary<NKCOperatorTokenSortSystem.eFilterOption, NKCUIComToggle> m_dicFilterBtn = new Dictionary<NKCOperatorTokenSortSystem.eFilterOption, NKCUIComToggle>();

	private OnFilterOptionChange dOnFilterOptionChange;

	private NKCOperatorTokenSortSystem m_ssActive;

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

	public bool IsSubfilterOpened
	{
		get
		{
			if (m_objSubFilter != null)
			{
				return m_objSubFilter.activeSelf;
			}
			return false;
		}
	}

	public void CloseSubFilter()
	{
		m_subFilter.Close();
	}

	private void Init()
	{
		m_dicFilterBtn.Clear();
		SetToggleListner(m_passiveSlot, NKCOperatorTokenSortSystem.eFilterOption.PassiveSkill);
		SetToggleListner(m_tglRare_SSR, NKCOperatorTokenSortSystem.eFilterOption.Rarily_SSR);
		SetToggleListner(m_tglRare_SR, NKCOperatorTokenSortSystem.eFilterOption.Rarily_SR);
		SetToggleListner(m_tglRare_R, NKCOperatorTokenSortSystem.eFilterOption.Rarily_R);
		SetToggleListner(m_tglRare_N, NKCOperatorTokenSortSystem.eFilterOption.Rarily_N);
		m_bInitComplete = true;
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(m_objSubFilter, bValue: false);
	}

	private void SetToggleListner(NKCUIComToggle toggle, NKCOperatorTokenSortSystem.eFilterOption filterOption)
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

	private void SetToggleListner(NKCPopupFilterSubUIOperatorPassiveSlot slot, NKCOperatorTokenSortSystem.eFilterOption filterOption)
	{
		if (!(slot != null))
		{
			return;
		}
		m_dicFilterPassiveSlot.Add(filterOption, slot);
		slot.SetData(null);
		NKCUIComStateButton button = slot.GetButton();
		if (button != null)
		{
			button.PointerClick.RemoveAllListeners();
			button.PointerClick.AddListener(delegate
			{
				OpenPassiveSkillPopup();
			});
		}
	}

	public void OpenFilterPopup(NKCOperatorTokenSortSystem ssActive, OnFilterOptionChange onFilterOptionChange, NKCOperatorSortSystem.FILTER_OPEN_TYPE filterOpenType)
	{
		OpenFilterPopup(ssActive, NKCOperatorTokenSortSystem.setDefaultOperatorFilterCategory, onFilterOptionChange);
	}

	public void OpenFilterPopup(NKCOperatorTokenSortSystem ssActive, HashSet<NKCOperatorTokenSortSystem.eFilterCategory> setFilterCategory, OnFilterOptionChange onFilterOptionChange)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		m_ssActive = ssActive;
		dOnFilterOptionChange = onFilterOptionChange;
		SetFilter(m_ssActive.FilterSet);
		NKCUtil.SetGameobjectActive(m_objRare, setFilterCategory.Contains(NKCOperatorTokenSortSystem.eFilterCategory.Rarity));
		NKCUtil.SetGameobjectActive(m_objPassiveSkill, setFilterCategory.Contains(NKCOperatorTokenSortSystem.eFilterCategory.PassiveSkill));
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void OpenPassiveSkillPopup()
	{
		GameObject objSubFilter = m_objSubFilter;
		if ((object)objSubFilter != null && !objSubFilter.activeSelf)
		{
			m_subFilter.Open(m_ssActive.Options.passiveSkillID, OnFilterButton, OperatorSkillType.m_Passive);
		}
		else
		{
			m_subFilter.Close();
		}
	}

	private void SetFilter(HashSet<NKCOperatorTokenSortSystem.eFilterOption> setFilterOption)
	{
		ResetFilterSlot();
		m_bReset = true;
		foreach (NKCOperatorTokenSortSystem.eFilterOption item in setFilterOption)
		{
			if (m_dicFilterBtn.ContainsKey(item) && m_dicFilterBtn[item] != null)
			{
				m_dicFilterBtn[item].Select(bSelect: true);
			}
		}
		if (setFilterOption.Contains(NKCOperatorTokenSortSystem.eFilterOption.PassiveSkill))
		{
			m_dicFilterPassiveSlot[NKCOperatorTokenSortSystem.eFilterOption.PassiveSkill].SetData(m_ssActive.m_PassiveSkillID, m_ssActive.m_PassiveSkillID != 0);
		}
		m_bReset = false;
	}

	private void OnFilterButton(bool bSelect, NKCOperatorTokenSortSystem.eFilterOption filterOption)
	{
		if (!m_dicFilterBtn.ContainsKey(filterOption))
		{
			return;
		}
		NKCUIComToggle nKCUIComToggle = m_dicFilterBtn[filterOption];
		if (!(nKCUIComToggle != null))
		{
			return;
		}
		nKCUIComToggle.Select(bSelect, bForce: true, bImmediate: true);
		if (!m_bReset)
		{
			if (m_ssActive.FilterSet.Contains(filterOption))
			{
				m_ssActive.FilterSet.Remove(filterOption);
			}
			else
			{
				m_ssActive.FilterSet.Add(filterOption);
			}
			dOnFilterOptionChange?.Invoke(m_ssActive);
		}
	}

	private void OnFilterButton(int selectedSkillID)
	{
		if (!m_dicFilterPassiveSlot.ContainsKey(NKCOperatorTokenSortSystem.eFilterOption.PassiveSkill))
		{
			return;
		}
		if (m_ssActive.FilterSet.Contains(NKCOperatorTokenSortSystem.eFilterOption.PassiveSkill))
		{
			if (m_ssActive.m_PassiveSkillID == selectedSkillID)
			{
				m_ssActive.FilterSet.Remove(NKCOperatorTokenSortSystem.eFilterOption.PassiveSkill);
				m_ssActive.m_PassiveSkillID = 0;
			}
			else
			{
				m_ssActive.m_PassiveSkillID = selectedSkillID;
			}
		}
		else
		{
			m_ssActive.FilterSet.Add(NKCOperatorTokenSortSystem.eFilterOption.PassiveSkill);
			m_ssActive.m_PassiveSkillID = selectedSkillID;
		}
		m_passiveSlot.SetData(m_ssActive.m_PassiveSkillID, m_ssActive.m_PassiveSkillID != 0);
		if (!m_bReset)
		{
			dOnFilterOptionChange?.Invoke(m_ssActive);
		}
	}

	public void ResetFilterSlot()
	{
		m_bReset = true;
		NKCUIComToggle[] componentsInChildren = base.transform.GetComponentsInChildren<NKCUIComToggle>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Select(bSelect: false);
		}
		m_passiveSlot.SetData(0);
		m_bReset = false;
	}
}
