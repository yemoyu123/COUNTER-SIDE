using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCPopupMoldSort : MonoBehaviour
{
	public delegate void OnSortOption(NKCMoldSortSystem.eSortOption lstSortOptions);

	[Header("정렬 방식 선택")]
	public NKCUIRectMove m_rmSortTypeMenu;

	public NKCUIComToggle m_cTglSortCraftable;

	public NKCUIComToggle m_cTglSortTypeTier;

	public NKCUIComToggle m_cTglSortTypeRarity;

	private OnSortOption dOnSortOption;

	private Dictionary<NKCMoldSortSystem.eSortOption, NKCUIComToggle> m_dicSortOption = new Dictionary<NKCMoldSortSystem.eSortOption, NKCUIComToggle>();

	private bool m_bDescending;

	private bool m_bInitComplete;

	private void Init()
	{
		m_dicSortOption.Clear();
		if (m_cTglSortCraftable != null)
		{
			m_dicSortOption.Add(NKCMoldSortSystem.eSortOption.Craftable_High, m_cTglSortCraftable);
			m_dicSortOption.Add(NKCMoldSortSystem.eSortOption.Craftable_Low, m_cTglSortCraftable);
			m_cTglSortCraftable.OnValueChanged.RemoveAllListeners();
			m_cTglSortCraftable.OnValueChanged.AddListener(OnSortCraftable);
		}
		if (m_cTglSortTypeTier != null)
		{
			m_dicSortOption.Add(NKCMoldSortSystem.eSortOption.Tier_High, m_cTglSortTypeTier);
			m_dicSortOption.Add(NKCMoldSortSystem.eSortOption.Tier_Low, m_cTglSortTypeTier);
			m_cTglSortTypeTier.OnValueChanged.RemoveAllListeners();
			m_cTglSortTypeTier.OnValueChanged.AddListener(OnSortTier);
		}
		if (m_cTglSortTypeRarity != null)
		{
			m_dicSortOption.Add(NKCMoldSortSystem.eSortOption.Rarity_High, m_cTglSortTypeRarity);
			m_dicSortOption.Add(NKCMoldSortSystem.eSortOption.Rarity_Low, m_cTglSortTypeRarity);
			m_cTglSortTypeRarity.OnValueChanged.RemoveAllListeners();
			m_cTglSortTypeRarity.OnValueChanged.AddListener(OnSortRarity);
		}
		m_bInitComplete = true;
	}

	public void OpenMoldSortMenu(NKCMoldSortSystem.eSortOption selectedSortOption, OnSortOption onSortOption, bool bDescending, bool bValue, List<string> lstSort)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		NKCUtil.SetGameobjectActive(m_cTglSortCraftable.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_cTglSortTypeTier.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_cTglSortTypeTier.gameObject, bValue: false);
		if (lstSort.Count > 0)
		{
			for (int i = 0; i < lstSort.Count; i++)
			{
				if (lstSort[i].Contains("ST_Makeable"))
				{
					NKCUtil.SetGameobjectActive(m_cTglSortCraftable.gameObject, bValue: true);
				}
				else if (lstSort[i].Contains("ST_Tier"))
				{
					NKCUtil.SetGameobjectActive(m_cTglSortTypeTier.gameObject, bValue: true);
				}
				else if (lstSort[i].Contains("ST_Grade"))
				{
					NKCUtil.SetGameobjectActive(m_cTglSortTypeRarity.gameObject, bValue: true);
				}
			}
		}
		if (bValue)
		{
			dOnSortOption = onSortOption;
			m_bDescending = bDescending;
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			if (m_dicSortOption.ContainsKey(selectedSortOption) && m_dicSortOption[selectedSortOption] != null)
			{
				m_dicSortOption[selectedSortOption].Select(bSelect: true, bForce: true);
			}
			else
			{
				ResetSortMenu();
			}
			StartRectMove(bValue);
		}
		else
		{
			Close();
		}
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void StartRectMove(bool bOpen, bool bAnimate = true)
	{
		if (bAnimate)
		{
			if (bOpen)
			{
				m_rmSortTypeMenu.gameObject.SetActive(value: true);
				m_rmSortTypeMenu.Transit("Open");
			}
			else
			{
				m_rmSortTypeMenu.Transit("Close", delegate
				{
					m_rmSortTypeMenu.gameObject.SetActive(value: false);
				});
			}
		}
		else
		{
			m_rmSortTypeMenu.gameObject.SetActive(bOpen);
			m_rmSortTypeMenu.Set(bOpen ? "Open" : "Close");
		}
	}

	private void ResetSortMenu()
	{
		NKCUIComToggle[] componentsInChildren = base.transform.GetComponentsInChildren<NKCUIComToggle>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Select(bSelect: false, bForce: true, bImmediate: true);
		}
	}

	public void OnSortCraftable(bool bSelect)
	{
		m_cTglSortCraftable.Select(bSelect, bForce: true, bImmediate: true);
		if (bSelect)
		{
			if (m_bDescending)
			{
				OnSort(NKCMoldSortSystem.eSortOption.Craftable_High);
			}
			else
			{
				OnSort(NKCMoldSortSystem.eSortOption.Craftable_Low);
			}
		}
	}

	public void OnSortTier(bool bSelect)
	{
		m_cTglSortTypeTier.Select(bSelect, bForce: true, bImmediate: true);
		if (bSelect)
		{
			if (m_bDescending)
			{
				OnSort(NKCMoldSortSystem.eSortOption.Tier_High);
			}
			else
			{
				OnSort(NKCMoldSortSystem.eSortOption.Tier_Low);
			}
		}
	}

	public void OnSortRarity(bool bSelect)
	{
		m_cTglSortTypeRarity.Select(bSelect, bForce: true, bImmediate: true);
		if (bSelect)
		{
			if (m_bDescending)
			{
				OnSort(NKCMoldSortSystem.eSortOption.Rarity_High);
			}
			else
			{
				OnSort(NKCMoldSortSystem.eSortOption.Rarity_Low);
			}
		}
	}

	public void OnSort(NKCMoldSortSystem.eSortOption sortOption)
	{
		dOnSortOption(sortOption);
		Close();
	}
}
