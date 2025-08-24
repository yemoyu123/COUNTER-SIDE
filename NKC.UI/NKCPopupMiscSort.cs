using System.Collections.Generic;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupMiscSort : MonoBehaviour
{
	public enum SORT_OPEN_TYPE
	{
		Normal,
		Interior
	}

	public delegate void OnSortOption(List<NKCMiscSortSystem.eSortOption> lstSortOptions);

	[Header("정렬 방식 선택")]
	public NKCUIRectMove m_rmSortTypeMenu;

	public NKCUIComToggle m_cTglSortTypeID;

	public NKCUIComToggle m_cTglSortTypePoint;

	public NKCUIComToggle m_cTglSortTypeRarity;

	public NKCUIComToggle m_cTglSortTypeRegDate;

	public NKCUIComToggle m_cTglSortTypeCanPlace;

	private OnSortOption dOnSortOption;

	private Dictionary<NKCMiscSortSystem.eSortOption, NKCUIComToggle> m_dicSortOption = new Dictionary<NKCMiscSortSystem.eSortOption, NKCUIComToggle>();

	private Dictionary<NKCMiscSortSystem.eSortCategory, NKCUIComToggle> m_dicToggle = new Dictionary<NKCMiscSortSystem.eSortCategory, NKCUIComToggle>();

	private Dictionary<NKCMiscSortSystem.eSortOption, List<NKCMiscSortSystem.eSortOption>> m_dicSortOptionDetails = new Dictionary<NKCMiscSortSystem.eSortOption, List<NKCMiscSortSystem.eSortOption>>();

	private bool m_bDescending;

	private bool m_bInitComplete;

	public void Init()
	{
		m_dicSortOption.Clear();
		AddSortOption(NKCMiscSortSystem.eSortCategory.ID, m_cTglSortTypeID);
		AddSortOption(NKCMiscSortSystem.eSortCategory.Point, m_cTglSortTypePoint);
		AddSortOption(NKCMiscSortSystem.eSortCategory.Rarity, m_cTglSortTypeRarity);
		AddSortOption(NKCMiscSortSystem.eSortCategory.RegDate, m_cTglSortTypeRegDate);
		AddSortOption(NKCMiscSortSystem.eSortCategory.CanPlace, m_cTglSortTypeCanPlace);
		m_bInitComplete = true;
	}

	private void AddSortOption(NKCMiscSortSystem.eSortCategory sortCategory, NKCUIComToggle tgl)
	{
		if (tgl != null)
		{
			tgl.m_DataInt = (int)sortCategory;
			m_dicSortOption.Add(NKCMiscSortSystem.GetSortOptionByCategory(sortCategory, bDescending: true), tgl);
			m_dicSortOption.Add(NKCMiscSortSystem.GetSortOptionByCategory(sortCategory, bDescending: false), tgl);
			m_dicToggle.Add(sortCategory, tgl);
			tgl.OnValueChanged.RemoveAllListeners();
			tgl.OnValueChangedWithData = OnTglSortOption;
		}
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void OpenSortMenu(HashSet<NKCMiscSortSystem.eSortCategory> setCategory, NKCMiscSortSystem.eSortOption selectedSortOption, OnSortOption onSortOption, bool bOpen)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		if (bOpen)
		{
			dOnSortOption = onSortOption;
			m_bDescending = NKCMiscSortSystem.IsDescending(selectedSortOption);
			foreach (KeyValuePair<NKCMiscSortSystem.eSortCategory, NKCUIComToggle> item in m_dicToggle)
			{
				NKCUtil.SetGameobjectActive(item.Value, setCategory.Contains(item.Key));
			}
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			if (m_dicSortOption.ContainsKey(selectedSortOption) && m_dicSortOption[selectedSortOption] != null)
			{
				m_dicSortOption[selectedSortOption].Select(bSelect: true, bForce: true);
			}
			else
			{
				ResetSortMenu();
			}
			StartRectMove(bOpen);
		}
		else
		{
			Close();
		}
	}

	public void StartRectMove(bool bOpen, bool bAnimate = true)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
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
			if (componentsInChildren[i].gameObject.activeSelf)
			{
				componentsInChildren[i].Select(bSelect: false, bForce: true, bImmediate: true);
			}
		}
	}

	private void OnTglSortOption(bool value, int data)
	{
		if (value)
		{
			NKCMiscSortSystem.eSortOption sortOptionByCategory = NKCMiscSortSystem.GetSortOptionByCategory((NKCMiscSortSystem.eSortCategory)data, m_bDescending);
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

	public void OnSort(List<NKCMiscSortSystem.eSortOption> sortList)
	{
		dOnSortOption(sortList);
		Close();
	}

	public void OnSort(NKCMiscSortSystem.eSortOption sortOption)
	{
		List<NKCMiscSortSystem.eSortOption> list = new List<NKCMiscSortSystem.eSortOption>();
		list.Add(sortOption);
		dOnSortOption(list);
		Close();
	}
}
