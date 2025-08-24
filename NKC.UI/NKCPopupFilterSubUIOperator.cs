using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupFilterSubUIOperator : MonoBehaviour
{
	public delegate void OnFilterOptionChange(NKCOperatorSortSystem ssActive);

	[Header("해당 프리팹에서 사용하는것만 연결")]
	[Header("Passive Skill")]
	public GameObject m_objPassiveSkill;

	public NKCPopupFilterSubUIOperatorPassiveSlot m_passiveSlot;

	[Header("In Collection")]
	public GameObject m_objCollected;

	public NKCUIComToggle m_tglCollected;

	public NKCUIComToggle m_tglNotCollected;

	[Header("Have")]
	public GameObject m_objHave;

	public NKCUIComToggle m_tglHave;

	public NKCUIComToggle m_tglNotHave;

	[Header("Rarity")]
	public GameObject m_objRare;

	public NKCUIComToggle m_tglRare_SSR;

	public NKCUIComToggle m_tglRare_SR;

	public NKCUIComToggle m_tglRare_R;

	public NKCUIComToggle m_tglRare_N;

	[Header("Level")]
	public GameObject m_objLevel;

	public NKCUIComToggle m_tglLevel_1;

	public NKCUIComToggle m_tglLevel_Other;

	public NKCUIComToggle m_tglLevel_Max;

	[Header("Deck")]
	public GameObject m_objDeck;

	public NKCUIComToggle m_tglDecked;

	public NKCUIComToggle m_tglWait;

	[Header("Lock")]
	public GameObject m_objLock;

	public NKCUIComToggle m_tglLocked;

	public NKCUIComToggle m_tglUnlocked;

	[Header("Passive Skill")]
	public GameObject m_objSubFilter;

	public NKCPopupFilterSubUIOperatorPassive m_subFilter;

	private RectTransform m_RectTransform;

	private Dictionary<NKCOperatorSortSystem.eFilterOption, NKCPopupFilterSubUIOperatorPassiveSlot> m_dicFilterPassiveSlot = new Dictionary<NKCOperatorSortSystem.eFilterOption, NKCPopupFilterSubUIOperatorPassiveSlot>();

	private Dictionary<NKCOperatorSortSystem.eFilterOption, NKCUIComToggle> m_dicFilterBtn = new Dictionary<NKCOperatorSortSystem.eFilterOption, NKCUIComToggle>();

	private OnFilterOptionChange dOnFilterOptionChange;

	private NKCOperatorSortSystem m_ssActive;

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
		SetToggleListner(m_passiveSlot, NKCOperatorSortSystem.eFilterOption.PassiveSkill);
		SetToggleListner(m_tglHave, NKCOperatorSortSystem.eFilterOption.Have);
		SetToggleListner(m_tglNotHave, NKCOperatorSortSystem.eFilterOption.NotHave);
		SetToggleListner(m_tglRare_SSR, NKCOperatorSortSystem.eFilterOption.Rarily_SSR);
		SetToggleListner(m_tglRare_SR, NKCOperatorSortSystem.eFilterOption.Rarily_SR);
		SetToggleListner(m_tglRare_R, NKCOperatorSortSystem.eFilterOption.Rarily_R);
		SetToggleListner(m_tglRare_N, NKCOperatorSortSystem.eFilterOption.Rarily_N);
		SetToggleListner(m_tglLevel_1, NKCOperatorSortSystem.eFilterOption.Level_1);
		SetToggleListner(m_tglLevel_Other, NKCOperatorSortSystem.eFilterOption.Level_other);
		SetToggleListner(m_tglLevel_Max, NKCOperatorSortSystem.eFilterOption.Level_Max);
		SetToggleListner(m_tglDecked, NKCOperatorSortSystem.eFilterOption.Decked);
		SetToggleListner(m_tglWait, NKCOperatorSortSystem.eFilterOption.NotDecked);
		SetToggleListner(m_tglLocked, NKCOperatorSortSystem.eFilterOption.Locked);
		SetToggleListner(m_tglUnlocked, NKCOperatorSortSystem.eFilterOption.Unlocked);
		SetToggleListner(m_tglCollected, NKCOperatorSortSystem.eFilterOption.Collected);
		SetToggleListner(m_tglNotCollected, NKCOperatorSortSystem.eFilterOption.NotCollected);
		m_bInitComplete = true;
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(m_objSubFilter, bValue: false);
	}

	private void SetToggleListner(NKCUIComToggle toggle, NKCOperatorSortSystem.eFilterOption filterOption)
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

	private void SetToggleListner(NKCPopupFilterSubUIOperatorPassiveSlot slot, NKCOperatorSortSystem.eFilterOption filterOption)
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

	public void OpenFilterPopup(NKCOperatorSortSystem ssActive, OnFilterOptionChange onFilterOptionChange, NKCOperatorSortSystem.FILTER_OPEN_TYPE filterOpenType)
	{
		OpenFilterPopup(ssActive, NKCOperatorSortSystem.MakeDefaultFilterCategory(filterOpenType), onFilterOptionChange);
	}

	public void OpenFilterPopup(NKCOperatorSortSystem ssActive, HashSet<NKCOperatorSortSystem.eFilterCategory> setFilterCategory, OnFilterOptionChange onFilterOptionChange)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		m_ssActive = ssActive;
		dOnFilterOptionChange = onFilterOptionChange;
		SetFilter(m_ssActive.FilterSet);
		NKCUtil.SetGameobjectActive(m_objHave, setFilterCategory.Contains(NKCOperatorSortSystem.eFilterCategory.Have));
		NKCUtil.SetGameobjectActive(m_objRare, setFilterCategory.Contains(NKCOperatorSortSystem.eFilterCategory.Rarity));
		NKCUtil.SetGameobjectActive(m_objLevel, setFilterCategory.Contains(NKCOperatorSortSystem.eFilterCategory.Level));
		NKCUtil.SetGameobjectActive(m_objDeck, setFilterCategory.Contains(NKCOperatorSortSystem.eFilterCategory.Decked));
		NKCUtil.SetGameobjectActive(m_objLock, setFilterCategory.Contains(NKCOperatorSortSystem.eFilterCategory.Locked));
		NKCUtil.SetGameobjectActive(m_objCollected, setFilterCategory.Contains(NKCOperatorSortSystem.eFilterCategory.Collected));
		NKCUtil.SetGameobjectActive(m_objPassiveSkill, setFilterCategory.Contains(NKCOperatorSortSystem.eFilterCategory.PassiveSkill));
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

	private void SetFilter(HashSet<NKCOperatorSortSystem.eFilterOption> setFilterOption)
	{
		ResetFilterSlot();
		m_bReset = true;
		foreach (NKCOperatorSortSystem.eFilterOption item in setFilterOption)
		{
			if (m_dicFilterBtn.ContainsKey(item) && m_dicFilterBtn[item] != null)
			{
				m_dicFilterBtn[item].Select(bSelect: true);
			}
		}
		if (setFilterOption.Contains(NKCOperatorSortSystem.eFilterOption.PassiveSkill))
		{
			m_dicFilterPassiveSlot[NKCOperatorSortSystem.eFilterOption.PassiveSkill].SetData(m_ssActive.m_PassiveSkillID, m_ssActive.m_PassiveSkillID != 0);
		}
		m_bReset = false;
	}

	private void OnFilterButton(bool bSelect, NKCOperatorSortSystem.eFilterOption filterOption)
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
		if (!m_dicFilterPassiveSlot.ContainsKey(NKCOperatorSortSystem.eFilterOption.PassiveSkill))
		{
			return;
		}
		if (m_ssActive.FilterSet.Contains(NKCOperatorSortSystem.eFilterOption.PassiveSkill))
		{
			if (m_ssActive.m_PassiveSkillID == selectedSkillID)
			{
				m_ssActive.FilterSet.Remove(NKCOperatorSortSystem.eFilterOption.PassiveSkill);
				m_ssActive.m_PassiveSkillID = 0;
			}
			else
			{
				m_ssActive.m_PassiveSkillID = selectedSkillID;
			}
		}
		else
		{
			m_ssActive.FilterSet.Add(NKCOperatorSortSystem.eFilterOption.PassiveSkill);
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
