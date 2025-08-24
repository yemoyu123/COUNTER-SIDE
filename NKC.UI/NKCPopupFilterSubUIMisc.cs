using System.Collections.Generic;
using NKC.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupFilterSubUIMisc : MonoBehaviour
{
	public delegate void OnFilterOptionChange(NKCMiscSortSystem.eFilterOption filterOption, int selectedThemeID);

	public delegate void OnFilterOptionChangeTitleCategory(int titleCategoryKey);

	[Header("해당 프리팹에서 사용하는것만 연결")]
	[Header("Interior Target")]
	public GameObject m_objInteriorTarget;

	public NKCUIComToggle m_tglFloor;

	public NKCUIComToggle m_tglTile;

	public NKCUIComToggle m_tglWall;

	public NKCUIComToggle m_tglBackground;

	[Header("Interior Category")]
	public GameObject m_objInteriorCategory;

	public NKCUIComToggle m_tglDeco;

	public NKCUIComToggle m_tglFurniture;

	[Header("Interior CanPlace")]
	public GameObject m_objInteriorCanPlace;

	public NKCUIComToggle m_tglCanPlace;

	public NKCUIComToggle m_tglCanNotPlace;

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

	[Header("Theme")]
	public GameObject m_objTheme;

	public NKCUIComToggle m_tglTheme;

	private int m_currentSelectedTheme;

	[Header("Interaction")]
	public GameObject m_objInteraction;

	public NKCUIComToggle m_tglInteraction_Have;

	public NKCUIComToggle m_tglInteraction_NotHave;

	[Header("Title")]
	public GameObject m_objTitleCategory;

	public Transform m_trTitleCategory;

	public NKCPopupFilterSubUIMiscSlot m_prefabCategorySlot;

	private List<NKCPopupFilterSubUIMiscSlot> m_lstCategorySlot = new List<NKCPopupFilterSubUIMiscSlot>();

	private Dictionary<NKCMiscSortSystem.eFilterOption, NKCUIComToggle> m_dicFilterBtn = new Dictionary<NKCMiscSortSystem.eFilterOption, NKCUIComToggle>();

	private OnFilterOptionChange dOnFilterOptionChange;

	private OnFilterOptionChangeTitleCategory dOnFilterOptionChangeTitleCategory;

	private bool m_bInitComplete;

	private bool m_bReset;

	private void Init()
	{
		m_dicFilterBtn.Clear();
		SetToggleListner(m_tglFloor, NKCMiscSortSystem.eFilterOption.InteriorTarget_Floor);
		SetToggleListner(m_tglTile, NKCMiscSortSystem.eFilterOption.InteriorTarget_Tile);
		SetToggleListner(m_tglWall, NKCMiscSortSystem.eFilterOption.InteriorTarget_Wall);
		SetToggleListner(m_tglBackground, NKCMiscSortSystem.eFilterOption.InteriorTarget_Background);
		SetToggleListner(m_tglDeco, NKCMiscSortSystem.eFilterOption.InteriorCategory_DECO);
		SetToggleListner(m_tglFurniture, NKCMiscSortSystem.eFilterOption.InteriorCategory_FURNITURE);
		SetToggleListner(m_tglCanPlace, NKCMiscSortSystem.eFilterOption.InteriorCanPlace);
		SetToggleListner(m_tglCanNotPlace, NKCMiscSortSystem.eFilterOption.InteriorCannotPlace);
		SetToggleListner(m_tglHave, NKCMiscSortSystem.eFilterOption.Have);
		SetToggleListner(m_tglNotHave, NKCMiscSortSystem.eFilterOption.NotHave);
		SetToggleListner(m_tglRare_SSR, NKCMiscSortSystem.eFilterOption.Tier_SSR);
		SetToggleListner(m_tglRare_SR, NKCMiscSortSystem.eFilterOption.Tier_SR);
		SetToggleListner(m_tglRare_R, NKCMiscSortSystem.eFilterOption.Tier_R);
		SetToggleListner(m_tglRare_N, NKCMiscSortSystem.eFilterOption.Tier_N);
		m_dicFilterBtn.Add(NKCMiscSortSystem.eFilterOption.Theme, m_tglTheme);
		NKCUtil.SetToggleValueChangedDelegate(m_tglTheme, OnTglTheme);
		ClearTitleCategorySlot();
		foreach (NKCUserTitleCategoryTemplet value in NKMTempletContainer<NKCUserTitleCategoryTemplet>.Values)
		{
			if (value != null)
			{
				NKCPopupFilterSubUIMiscSlot nKCPopupFilterSubUIMiscSlot = Object.Instantiate(m_prefabCategorySlot, m_trTitleCategory);
				if (null != nKCPopupFilterSubUIMiscSlot)
				{
					nKCPopupFilterSubUIMiscSlot.SetData(value.TitleCategoryID, NKCStringTable.GetString(value.CategoryString));
					SetToggleListner(nKCPopupFilterSubUIMiscSlot.m_cTgl, value.TitleCategoryID);
					m_lstCategorySlot.Add(nKCPopupFilterSubUIMiscSlot);
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objTitleCategory.gameObject, m_lstCategorySlot.Count > 0);
		m_bInitComplete = true;
	}

	private void SetToggleListner(NKCUIComToggle toggle, NKCMiscSortSystem.eFilterOption filterOption)
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

	private void SetToggleListner(NKCUIComToggle toggle, int filterOptionTitleCategoryID)
	{
		if (toggle != null)
		{
			toggle.OnValueChanged.RemoveAllListeners();
			toggle.OnValueChanged.AddListener(delegate(bool value)
			{
				OnFilterButton(value, filterOptionTitleCategoryID);
			});
		}
	}

	public void OpenFilterPopup(HashSet<NKCMiscSortSystem.eFilterOption> setFilterOption, HashSet<NKCMiscSortSystem.eFilterCategory> setFilterCategory, OnFilterOptionChange onFilterOptionChange, int currentSelectedTheme, HashSet<int> setFilterOptionTitleCategory, OnFilterOptionChangeTitleCategory onFilterOptionChangeTitleCategory = null)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		m_currentSelectedTheme = currentSelectedTheme;
		dOnFilterOptionChange = onFilterOptionChange;
		dOnFilterOptionChangeTitleCategory = onFilterOptionChangeTitleCategory;
		if (setFilterCategory == null)
		{
			setFilterCategory = new HashSet<NKCMiscSortSystem.eFilterCategory>();
		}
		SetFilter(setFilterOption);
		NKCUtil.SetGameobjectActive(m_objInteriorTarget, setFilterCategory.Contains(NKCMiscSortSystem.eFilterCategory.InteriorTarget));
		NKCUtil.SetGameobjectActive(m_objInteriorCategory, setFilterCategory.Contains(NKCMiscSortSystem.eFilterCategory.InteriorCategory));
		NKCUtil.SetGameobjectActive(m_objInteriorCanPlace, setFilterCategory.Contains(NKCMiscSortSystem.eFilterCategory.InteriorCanPlace));
		NKCUtil.SetGameobjectActive(m_objInteraction, setFilterCategory.Contains(NKCMiscSortSystem.eFilterCategory.Interaction));
		NKCUtil.SetGameobjectActive(m_objHave, setFilterCategory.Contains(NKCMiscSortSystem.eFilterCategory.Have));
		NKCUtil.SetGameobjectActive(m_objRare, setFilterCategory.Contains(NKCMiscSortSystem.eFilterCategory.Tier));
		NKCUtil.SetGameobjectActive(m_objTheme, setFilterCategory.Contains(NKCMiscSortSystem.eFilterCategory.Theme));
		foreach (NKCPopupFilterSubUIMiscSlot item in m_lstCategorySlot)
		{
			item.m_cTgl.Select(setFilterOptionTitleCategory.Contains(item.TitleCategoryID), bForce: true, bImmediate: true);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	private void SetFilter(HashSet<NKCMiscSortSystem.eFilterOption> setFilterOption)
	{
		ResetFilter(resetSelectedTheme: false);
		m_bReset = true;
		foreach (NKCMiscSortSystem.eFilterOption item in setFilterOption)
		{
			if (m_dicFilterBtn.ContainsKey(item) && m_dicFilterBtn[item] != null)
			{
				m_dicFilterBtn[item].Select(bSelect: true);
			}
		}
		SetThemeButton();
		m_bReset = false;
	}

	private void OnFilterButton(bool bSelect, NKCMiscSortSystem.eFilterOption filterOption)
	{
		if (!m_dicFilterBtn.ContainsKey(filterOption))
		{
			return;
		}
		NKCUIComToggle nKCUIComToggle = m_dicFilterBtn[filterOption];
		if (nKCUIComToggle != null)
		{
			nKCUIComToggle.Select(bSelect, bForce: true, bImmediate: true);
			if (m_bReset)
			{
				return;
			}
			dOnFilterOptionChange?.Invoke(filterOption, m_currentSelectedTheme);
		}
		SetThemeButton();
	}

	private void OnFilterButton(bool bSelect, int filterOptionTitleCategoryID)
	{
		foreach (NKCPopupFilterSubUIMiscSlot item in m_lstCategorySlot)
		{
			if (!(null == item.m_cTgl) && filterOptionTitleCategoryID == item.TitleCategoryID)
			{
				item.m_cTgl.Select(bSelect, bForce: true, bImmediate: true);
				if (m_bReset)
				{
					break;
				}
				dOnFilterOptionChangeTitleCategory?.Invoke(filterOptionTitleCategoryID);
			}
		}
	}

	public void ResetFilter(bool resetSelectedTheme)
	{
		if (resetSelectedTheme)
		{
			m_currentSelectedTheme = 0;
		}
		m_bReset = true;
		NKCUIComToggle[] componentsInChildren = base.transform.GetComponentsInChildren<NKCUIComToggle>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Select(bSelect: false);
		}
		SetThemeButton();
		m_bReset = false;
	}

	private void OnTglTheme(bool value)
	{
		if (!m_bReset)
		{
			if (value)
			{
				NKCPopupFilterTheme.Instance.Open(OnSelectTheme, m_currentSelectedTheme);
				m_tglTheme.Select(bSelect: false, bForce: true);
			}
			else
			{
				OnSelectTheme(0);
			}
		}
	}

	private void OnSelectTheme(int themeID)
	{
		m_currentSelectedTheme = themeID;
		OnFilterButton(themeID != 0, NKCMiscSortSystem.eFilterOption.Theme);
	}

	private void SetThemeButton()
	{
		if (m_currentSelectedTheme == 0)
		{
			m_tglTheme.Select(bSelect: false, bForce: true);
			m_tglTheme.SetTitleText(NKCStringTable.GetString("SI_PF_FILTER_THEME_OPTION_SELECT"));
		}
		else
		{
			NKCThemeGroupTemplet nKCThemeGroupTemplet = NKCThemeGroupTemplet.Find(m_currentSelectedTheme);
			m_tglTheme.SetTitleText(NKCStringTable.GetString(nKCThemeGroupTemplet.GroupStringKey));
			m_tglTheme.Select(bSelect: true, bForce: true);
		}
	}

	public void ClearTitleCategorySlot()
	{
		for (int i = 0; i < m_lstCategorySlot.Count; i++)
		{
			Object.Destroy(m_lstCategorySlot[i]);
			m_lstCategorySlot[i] = null;
		}
		m_lstCategorySlot.Clear();
	}
}
