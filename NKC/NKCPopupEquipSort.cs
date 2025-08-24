using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCPopupEquipSort : MonoBehaviour
{
	[Serializable]
	public struct CustomSortMenu
	{
		public NKCUIComToggle m_cTglSortTypeCustom;

		public Text m_lbCustomOffText;

		public Text m_lbCustomOnText;

		public Text m_lbCustomPressText;
	}

	public enum SORT_OPEN_TYPE
	{
		NORMAL,
		CRAFT,
		SELECTION,
		OPERATION_POWER,
		OPTION_WEIGHT
	}

	public delegate void OnSortOption(List<NKCEquipSortSystem.eSortOption> lstSortOptions);

	[Header("정렬 방식 선택")]
	public NKCUIRectMove m_rmSortTypeMenu;

	public NKCUIComToggle m_cTglSortCraftable;

	public NKCUIComToggle m_cTglSortTypeEnhance;

	public NKCUIComToggle m_cTglSortTypeTier;

	public NKCUIComToggle m_cTglSortTypeRarity;

	public NKCUIComToggle m_cTglSortTypeUID;

	public NKCUIComToggle m_cTglSortTypeSetOption;

	[Header("커스텀 정렬 메뉴 텍스트")]
	public CustomSortMenu[] m_arrayCustomSortMenu;

	private OnSortOption dOnSortOption;

	private Dictionary<NKCEquipSortSystem.eSortCategory, NKCUIComToggle> m_dicToggle = new Dictionary<NKCEquipSortSystem.eSortCategory, NKCUIComToggle>();

	private Dictionary<NKCEquipSortSystem.eSortOption, List<NKCEquipSortSystem.eSortOption>> m_dicSortOptionDetails = new Dictionary<NKCEquipSortSystem.eSortOption, List<NKCEquipSortSystem.eSortOption>>();

	private Dictionary<NKCEquipSortSystem.eSortOption, NKCUIComToggle> m_dicSortOption = new Dictionary<NKCEquipSortSystem.eSortOption, NKCUIComToggle>();

	private bool m_bDescending;

	private bool m_bInitComplete;

	private void Init()
	{
		m_dicSortOption.Clear();
		m_dicToggle.Clear();
		m_dicSortOptionDetails.Clear();
		AddSortOption(NKCEquipSortSystem.eSortCategory.Enhance, m_cTglSortTypeEnhance);
		AddSortOption(NKCEquipSortSystem.eSortCategory.Tier, m_cTglSortTypeTier);
		AddSortOption(NKCEquipSortSystem.eSortCategory.Rarity, m_cTglSortTypeRarity);
		AddSortOption(NKCEquipSortSystem.eSortCategory.UID, m_cTglSortTypeUID);
		AddSortOption(NKCEquipSortSystem.eSortCategory.SetOption, m_cTglSortTypeSetOption);
		if (m_arrayCustomSortMenu != null)
		{
			for (int i = 0; i < m_arrayCustomSortMenu.Length; i++)
			{
				switch (i)
				{
				case 0:
					AddSortOption(NKCEquipSortSystem.eSortCategory.Custom1, m_arrayCustomSortMenu[i].m_cTglSortTypeCustom);
					continue;
				case 1:
					AddSortOption(NKCEquipSortSystem.eSortCategory.Custom2, m_arrayCustomSortMenu[i].m_cTglSortTypeCustom);
					continue;
				case 2:
					AddSortOption(NKCEquipSortSystem.eSortCategory.Custom3, m_arrayCustomSortMenu[i].m_cTglSortTypeCustom);
					continue;
				}
				break;
			}
		}
		m_bInitComplete = true;
	}

	private void AddSortOption(NKCEquipSortSystem.eSortCategory sortCategory, NKCUIComToggle tgl)
	{
		if (tgl != null)
		{
			tgl.m_DataInt = (int)sortCategory;
			m_dicSortOption.Add(NKCEquipSortSystem.GetSortOptionByCategory(sortCategory, bDescending: true), tgl);
			m_dicSortOption.Add(NKCEquipSortSystem.GetSortOptionByCategory(sortCategory, bDescending: false), tgl);
			m_dicToggle.Add(sortCategory, tgl);
			tgl.OnValueChanged.RemoveAllListeners();
			tgl.OnValueChangedWithData = OnTglSortOption;
		}
	}

	private void OnTglSortOption(bool value, int data)
	{
		if (value)
		{
			NKCEquipSortSystem.eSortOption sortOptionByCategory = NKCEquipSortSystem.GetSortOptionByCategory((NKCEquipSortSystem.eSortCategory)data, m_bDescending);
			if (m_dicSortOptionDetails.TryGetValue(sortOptionByCategory, out var value2))
			{
				OnSort(value2);
			}
			else
			{
				OnSort(sortOptionByCategory);
			}
		}
	}

	public void OpenEquipSortMenu(HashSet<NKCEquipSortSystem.eSortCategory> setSortCategory, NKCEquipSortSystem.eSortOption selectedSortOption, OnSortOption onSortOption, bool bDescending, bool bValue)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		NKCUtil.SetGameobjectActive(m_cTglSortCraftable, setSortCategory.Contains(NKCEquipSortSystem.eSortCategory.Craftable));
		NKCUtil.SetGameobjectActive(m_cTglSortTypeEnhance, setSortCategory.Contains(NKCEquipSortSystem.eSortCategory.Enhance));
		NKCUtil.SetGameobjectActive(m_cTglSortTypeTier, setSortCategory.Contains(NKCEquipSortSystem.eSortCategory.Tier));
		NKCUtil.SetGameobjectActive(m_cTglSortTypeRarity, setSortCategory.Contains(NKCEquipSortSystem.eSortCategory.Rarity));
		NKCUtil.SetGameobjectActive(m_cTglSortTypeUID, setSortCategory.Contains(NKCEquipSortSystem.eSortCategory.UID));
		NKCUtil.SetGameobjectActive(m_cTglSortTypeSetOption, setSortCategory.Contains(NKCEquipSortSystem.eSortCategory.SetOption));
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

	public void OpenEquipSortMenu(NKCEquipSortSystem.eSortOption selectedSortOption, OnSortOption onSortOption, bool bDescending, bool bValue, SORT_OPEN_TYPE openType = SORT_OPEN_TYPE.NORMAL)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		NKCUtil.SetGameobjectActive(m_cTglSortCraftable, openType == SORT_OPEN_TYPE.CRAFT);
		NKCUtil.SetGameobjectActive(m_cTglSortTypeEnhance, openType == SORT_OPEN_TYPE.NORMAL);
		NKCUtil.SetGameobjectActive(m_cTglSortTypeUID, openType == SORT_OPEN_TYPE.NORMAL);
		NKCUtil.SetGameobjectActive(m_cTglSortTypeSetOption, openType == SORT_OPEN_TYPE.NORMAL);
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

	public void OnSort(List<NKCEquipSortSystem.eSortOption> lstSortOption)
	{
		lstSortOption = NKCEquipSortSystem.AddDefaultSortOptions(lstSortOption);
		dOnSortOption(lstSortOption);
		Close();
	}

	public void OnSort(NKCEquipSortSystem.eSortOption sortOption)
	{
		List<NKCEquipSortSystem.eSortOption> list = new List<NKCEquipSortSystem.eSortOption>();
		list.Add(sortOption);
		list = NKCEquipSortSystem.AddDefaultSortOptions(list);
		dOnSortOption(list);
		Close();
	}

	public List<NKCEquipSortSystem.eSortOption> ChangeAscend(List<NKCEquipSortSystem.eSortOption> targetList)
	{
		if (targetList == null || targetList.Count == 0)
		{
			return targetList;
		}
		switch (targetList[0])
		{
		case NKCEquipSortSystem.eSortOption.Enhance_High:
			targetList[0] = NKCEquipSortSystem.eSortOption.Enhance_Low;
			break;
		case NKCEquipSortSystem.eSortOption.Enhance_Low:
			targetList[0] = NKCEquipSortSystem.eSortOption.Enhance_High;
			break;
		case NKCEquipSortSystem.eSortOption.Tier_High:
			targetList[0] = NKCEquipSortSystem.eSortOption.Tier_Low;
			break;
		case NKCEquipSortSystem.eSortOption.Tier_Low:
			targetList[0] = NKCEquipSortSystem.eSortOption.Tier_High;
			break;
		case NKCEquipSortSystem.eSortOption.Rarity_High:
			targetList[0] = NKCEquipSortSystem.eSortOption.Rarity_Low;
			break;
		case NKCEquipSortSystem.eSortOption.Rarity_Low:
			targetList[0] = NKCEquipSortSystem.eSortOption.Rarity_High;
			break;
		case NKCEquipSortSystem.eSortOption.UID_First:
			targetList[0] = NKCEquipSortSystem.eSortOption.UID_Last;
			break;
		case NKCEquipSortSystem.eSortOption.UID_Last:
			targetList[0] = NKCEquipSortSystem.eSortOption.UID_First;
			break;
		case NKCEquipSortSystem.eSortOption.SetOption_High:
			targetList[0] = NKCEquipSortSystem.eSortOption.SetOption_Low;
			break;
		case NKCEquipSortSystem.eSortOption.SetOption_Low:
			targetList[0] = NKCEquipSortSystem.eSortOption.SetOption_High;
			break;
		case NKCEquipSortSystem.eSortOption.Equipped_First:
			targetList[0] = NKCEquipSortSystem.eSortOption.Equipped_Last;
			if (targetList[1] == NKCEquipSortSystem.eSortOption.Enhance_High)
			{
				targetList[1] = NKCEquipSortSystem.eSortOption.Enhance_Low;
			}
			break;
		case NKCEquipSortSystem.eSortOption.Equipped_Last:
			targetList[0] = NKCEquipSortSystem.eSortOption.Equipped_First;
			if (targetList[1] == NKCEquipSortSystem.eSortOption.Enhance_Low)
			{
				targetList[1] = NKCEquipSortSystem.eSortOption.Enhance_High;
			}
			break;
		default:
			targetList.RemoveAt(0);
			ChangeAscend(targetList);
			break;
		}
		return targetList;
	}
}
