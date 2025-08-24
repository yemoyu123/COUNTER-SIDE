using System;
using System.Collections.Generic;
using NKC.Office;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCPopupSort : MonoBehaviour
{
	[Serializable]
	public struct CustomSortMenu
	{
		public NKCUIComToggle m_cTglSortTypeCustom;

		public Text m_lbCustomOffText;

		public Text m_lbCustomOnText;

		public Text m_lbCustomPressText;
	}

	public delegate void OnSortOption(List<NKCUnitSortSystem.eSortOption> lstSortOptions);

	[Header("정렬 방식 선택")]
	public NKCUIRectMove m_rmSortTypeMenu;

	public NKCUIComToggle m_cTglSortTypeIdx;

	public NKCUIComToggle m_cTglSortTypeScoutProgress;

	public NKCUIComToggle m_cTglSortTypeLevel;

	public NKCUIComToggle m_cTglSortTypeRarity;

	public NKCUIComToggle m_cTglSortTypeSetOption;

	public NKCUIComToggle m_cTglSortTypeCost;

	public NKCUIComToggle m_cTglSortTypeUID;

	public NKCUIComToggle m_cTglSortTypePower;

	public NKCUIComToggle m_cTglSortTypeAttack;

	public NKCUIComToggle m_cTglSortTypeHP;

	public NKCUIComToggle m_cTglSortTypeDefense;

	public NKCUIComToggle m_cTglSortTypeCritical;

	public NKCUIComToggle m_cTglSortTypeHit;

	public NKCUIComToggle m_cTglSortTypeEvade;

	public NKCUIComToggle m_cTglSortTypeReduceSkillCool;

	public NKCUIComToggle m_cTglSortTypePlayerLevel;

	public NKCUIComToggle m_cTglSortTypeLoginTime;

	public NKCUIComToggle m_cTglSortTypeGuildGrade;

	public NKCUIComToggle m_cTglSortTypeGuildWeeklyPoint;

	public NKCUIComToggle m_cTglSortTypeGuildTotalPoint;

	public NKCUIComToggle m_cTglSortTypeLimitBreakPossible;

	public NKCUIComToggle m_cTglSortTypeTranscendence;

	public NKCUIComToggle m_cTglSortTypeLoyalty;

	public NKCUIComToggle m_cTglSortTypeSquadDungeon;

	public NKCUIComToggle m_cTglSortTypeSquadGauntlet;

	public NKCUIComToggle m_cTglSortTypeDeployStatus;

	[Header("커스텀 정렬 메뉴 텍스트")]
	public CustomSortMenu[] m_arrayCustomSortMenu;

	private OnSortOption dOnSortOption;

	private Dictionary<NKCUnitSortSystem.eSortOption, NKCUIComToggle> m_dicSortOption = new Dictionary<NKCUnitSortSystem.eSortOption, NKCUIComToggle>();

	private NKM_UNIT_TYPE m_eUnitType;

	private bool m_bIsCollection;

	private bool m_bDescending;

	private bool m_bInitComplete;

	public bool m_bUseDefaultSortAdd = true;

	private Dictionary<NKCUnitSortSystem.eSortCategory, NKCUIComToggle> m_dicToggle = new Dictionary<NKCUnitSortSystem.eSortCategory, NKCUIComToggle>();

	private Dictionary<NKCUnitSortSystem.eSortOption, List<NKCUnitSortSystem.eSortOption>> m_dicSortOptionDetails = new Dictionary<NKCUnitSortSystem.eSortOption, List<NKCUnitSortSystem.eSortOption>>();

	private void Init()
	{
		_ = m_bInitComplete;
		m_dicSortOption.Clear();
		if (m_arrayCustomSortMenu != null)
		{
			for (int i = 0; i < m_arrayCustomSortMenu.Length; i++)
			{
				switch (i)
				{
				case 0:
					AddSortOption(NKCUnitSortSystem.eSortCategory.Custom1, m_arrayCustomSortMenu[i].m_cTglSortTypeCustom);
					continue;
				case 1:
					AddSortOption(NKCUnitSortSystem.eSortCategory.Custom2, m_arrayCustomSortMenu[i].m_cTglSortTypeCustom);
					continue;
				case 2:
					AddSortOption(NKCUnitSortSystem.eSortCategory.Custom3, m_arrayCustomSortMenu[i].m_cTglSortTypeCustom);
					continue;
				}
				break;
			}
		}
		AddSortOption(NKCUnitSortSystem.eSortCategory.IDX, m_cTglSortTypeIdx);
		AddSortOption(NKCUnitSortSystem.eSortCategory.Level, m_cTglSortTypeLevel);
		AddSortOption(NKCUnitSortSystem.eSortCategory.Rarity, m_cTglSortTypeRarity);
		AddSortOption(NKCUnitSortSystem.eSortCategory.SetOption, m_cTglSortTypeSetOption);
		AddSortOption(NKCUnitSortSystem.eSortCategory.UnitSummonCost, m_cTglSortTypeCost);
		AddSortOption(NKCUnitSortSystem.eSortCategory.UID, m_cTglSortTypeUID);
		AddSortOption(NKCUnitSortSystem.eSortCategory.UnitPower, m_cTglSortTypePower);
		AddSortOption(NKCUnitSortSystem.eSortCategory.UnitAttack, m_cTglSortTypeAttack);
		AddSortOption(NKCUnitSortSystem.eSortCategory.UnitHealth, m_cTglSortTypeHP);
		AddSortOption(NKCUnitSortSystem.eSortCategory.UnitDefense, m_cTglSortTypeDefense);
		AddSortOption(NKCUnitSortSystem.eSortCategory.UnitCrit, m_cTglSortTypeCritical);
		AddSortOption(NKCUnitSortSystem.eSortCategory.UnitHit, m_cTglSortTypeHit);
		AddSortOption(NKCUnitSortSystem.eSortCategory.UnitEvade, m_cTglSortTypeEvade);
		AddSortOption(NKCUnitSortSystem.eSortCategory.UnitReduceSkillCool, m_cTglSortTypeReduceSkillCool);
		AddSortOption(NKCUnitSortSystem.eSortCategory.PlayerLevel, m_cTglSortTypePlayerLevel);
		AddSortOption(NKCUnitSortSystem.eSortCategory.LoginTime, m_cTglSortTypeLoginTime);
		AddSortOption(NKCUnitSortSystem.eSortCategory.ScoutProgress, m_cTglSortTypeScoutProgress);
		AddSortOption(NKCUnitSortSystem.eSortCategory.GuildGrade, m_cTglSortTypeGuildGrade);
		AddSortOption(NKCUnitSortSystem.eSortCategory.GuildWeeklyPoint, m_cTglSortTypeGuildWeeklyPoint);
		AddSortOption(NKCUnitSortSystem.eSortCategory.GuildTotalPoint, m_cTglSortTypeGuildTotalPoint);
		AddSortOption(NKCUnitSortSystem.eSortCategory.LimitBreakPossible, m_cTglSortTypeLimitBreakPossible);
		AddSortOption(NKCUnitSortSystem.eSortCategory.Transcendence, m_cTglSortTypeTranscendence);
		AddSortOption(NKCUnitSortSystem.eSortCategory.UnitLoyalty, m_cTglSortTypeLoyalty);
		AddSortOption(NKCUnitSortSystem.eSortCategory.Squad_Dungeon, m_cTglSortTypeSquadDungeon);
		AddSortOption(NKCUnitSortSystem.eSortCategory.Squad_Gauntlet, m_cTglSortTypeSquadGauntlet);
		AddSortOption(NKCUnitSortSystem.eSortCategory.Deploy_Status, m_cTglSortTypeDeployStatus);
		NKCOfficeManager.LoadTemplets();
		m_bInitComplete = true;
	}

	private void AddSortOption(NKCUnitSortSystem.eSortCategory sortCategory, NKCUIComToggle tgl)
	{
		if (tgl != null)
		{
			tgl.m_DataInt = (int)sortCategory;
			m_dicSortOption.Add(NKCUnitSortSystem.GetSortOptionByCategory(sortCategory, bDescending: true), tgl);
			m_dicSortOption.Add(NKCUnitSortSystem.GetSortOptionByCategory(sortCategory, bDescending: false), tgl);
			m_dicToggle.Add(sortCategory, tgl);
			tgl.OnValueChanged.RemoveAllListeners();
			tgl.OnValueChangedWithData = OnTglSortOption;
		}
	}

	private void OnTglSortOption(bool value, int data)
	{
		if (value)
		{
			NKCUnitSortSystem.eSortOption sortOptionByCategory = NKCUnitSortSystem.GetSortOptionByCategory((NKCUnitSortSystem.eSortCategory)data, m_bDescending);
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

	public void AddSortOptionDetail(NKCUnitSortSystem.eSortOption sortOption, List<NKCUnitSortSystem.eSortOption> lstDetail)
	{
		m_dicSortOptionDetails.Add(sortOption, lstDetail);
	}

	public void OpenSortMenu(NKM_UNIT_TYPE targetType, NKCUnitSortSystem.eSortOption selectedSortOption, OnSortOption onSortOption, bool bIsCollection, bool bDescending, bool bOpen)
	{
		OpenSortMenu(MakeDefaultSortSet(targetType, bIsCollection), selectedSortOption, onSortOption, bOpen, targetType, bIsCollection, null);
	}

	public static HashSet<NKCUnitSortSystem.eSortCategory> MakeGlobalBanSortSet()
	{
		return new HashSet<NKCUnitSortSystem.eSortCategory>
		{
			NKCUnitSortSystem.eSortCategory.IDX,
			NKCUnitSortSystem.eSortCategory.UnitPower,
			NKCUnitSortSystem.eSortCategory.Rarity,
			NKCUnitSortSystem.eSortCategory.UnitAttack,
			NKCUnitSortSystem.eSortCategory.UnitHealth,
			NKCUnitSortSystem.eSortCategory.UnitSummonCost,
			NKCUnitSortSystem.eSortCategory.UnitDefense,
			NKCUnitSortSystem.eSortCategory.UnitHit,
			NKCUnitSortSystem.eSortCategory.UnitEvade
		};
	}

	public static HashSet<NKCUnitSortSystem.eSortCategory> MakeDefaultSortSet(NKM_UNIT_TYPE targetType, bool bIsCollection)
	{
		HashSet<NKCUnitSortSystem.eSortCategory> hashSet = new HashSet<NKCUnitSortSystem.eSortCategory>();
		if (bIsCollection)
		{
			hashSet.Add(NKCUnitSortSystem.eSortCategory.IDX);
		}
		else
		{
			hashSet.Add(NKCUnitSortSystem.eSortCategory.Level);
			hashSet.Add(NKCUnitSortSystem.eSortCategory.UID);
			hashSet.Add(NKCUnitSortSystem.eSortCategory.UnitPower);
			hashSet.Add(NKCUnitSortSystem.eSortCategory.UnitLoyalty);
			hashSet.Add(NKCUnitSortSystem.eSortCategory.Squad_Dungeon);
			hashSet.Add(NKCUnitSortSystem.eSortCategory.Deploy_Status);
		}
		hashSet.Add(NKCUnitSortSystem.eSortCategory.Rarity);
		hashSet.Add(NKCUnitSortSystem.eSortCategory.UnitAttack);
		hashSet.Add(NKCUnitSortSystem.eSortCategory.UnitHealth);
		if (targetType == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			hashSet.Add(NKCUnitSortSystem.eSortCategory.UnitSummonCost);
			hashSet.Add(NKCUnitSortSystem.eSortCategory.UnitDefense);
			hashSet.Add(NKCUnitSortSystem.eSortCategory.UnitHit);
			hashSet.Add(NKCUnitSortSystem.eSortCategory.UnitEvade);
		}
		return hashSet;
	}

	public static HashSet<NKCOperatorSortSystem.eSortCategory> MakeDefaultOprSortSet(NKM_UNIT_TYPE targetType, bool bIsCollection)
	{
		HashSet<NKCOperatorSortSystem.eSortCategory> hashSet = new HashSet<NKCOperatorSortSystem.eSortCategory>();
		if (bIsCollection)
		{
			hashSet.Add(NKCOperatorSortSystem.eSortCategory.IDX);
		}
		else
		{
			hashSet.Add(NKCOperatorSortSystem.eSortCategory.Level);
			hashSet.Add(NKCOperatorSortSystem.eSortCategory.UID);
			hashSet.Add(NKCOperatorSortSystem.eSortCategory.UnitPower);
		}
		hashSet.Add(NKCOperatorSortSystem.eSortCategory.Rarity);
		hashSet.Add(NKCOperatorSortSystem.eSortCategory.UnitAttack);
		hashSet.Add(NKCOperatorSortSystem.eSortCategory.UnitHealth);
		if (targetType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			hashSet.Add(NKCOperatorSortSystem.eSortCategory.UnitPower);
			hashSet.Add(NKCOperatorSortSystem.eSortCategory.UnitAttack);
			hashSet.Add(NKCOperatorSortSystem.eSortCategory.UnitDefense);
			hashSet.Add(NKCOperatorSortSystem.eSortCategory.UnitHealth);
			hashSet.Add(NKCOperatorSortSystem.eSortCategory.UnitReduceSkillCool);
		}
		return hashSet;
	}

	public void OpenGuildMemberSortMenu(HashSet<NKCUnitSortSystem.eSortCategory> setCategory, NKCUnitSortSystem.eSortOption selectedSortOption, OnSortOption onSortOption, bool bOpen)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		if (bOpen)
		{
			dOnSortOption = onSortOption;
			foreach (KeyValuePair<NKCUnitSortSystem.eSortCategory, NKCUIComToggle> item in m_dicToggle)
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

	public void OpenSortMenu(HashSet<NKCUnitSortSystem.eSortCategory> setCategory, NKCUnitSortSystem.eSortOption selectedSortOption, OnSortOption onSortOption, bool bOpen, NKM_UNIT_TYPE targetType, bool bIsCollection, List<string> customSortName)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		m_eUnitType = targetType;
		m_bIsCollection = bIsCollection;
		if (bOpen)
		{
			dOnSortOption = onSortOption;
			m_bDescending = NKCUnitSortSystem.IsDescending(selectedSortOption);
			foreach (KeyValuePair<NKCUnitSortSystem.eSortCategory, NKCUIComToggle> item in m_dicToggle)
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
			if (customSortName != null && m_arrayCustomSortMenu != null)
			{
				int count = customSortName.Count;
				for (int i = 0; i < count && i < m_arrayCustomSortMenu.Length; i++)
				{
					m_arrayCustomSortMenu[i].m_lbCustomOffText.text = customSortName[i];
					m_arrayCustomSortMenu[i].m_lbCustomOnText.text = customSortName[i];
					m_arrayCustomSortMenu[i].m_lbCustomPressText.text = customSortName[i];
				}
			}
		}
		else
		{
			Close();
		}
	}

	public void OpenSquadSortMenu(NKCUnitSortSystem.eSortOption selectedSortOption, OnSortOption onSortOption, bool bValue)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		if (bValue)
		{
			dOnSortOption = onSortOption;
			NKCUtil.SetGameobjectActive(m_cTglSortTypeIdx, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeLevel, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeRarity, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeCost, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeUID, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypePower, bValue: true);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeAttack, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeHP, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeDefense, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeCritical, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeHit, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeEvade, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypePlayerLevel, bValue: true);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeLoginTime, bValue: true);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeSetOption, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeLimitBreakPossible, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeLoyalty, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeSquadDungeon, bValue: false);
			NKCUtil.SetGameobjectActive(m_cTglSortTypeSquadGauntlet, bValue: false);
			if (m_arrayCustomSortMenu != null)
			{
				int num = m_arrayCustomSortMenu.Length;
				for (int i = 0; i < num; i++)
				{
					NKCUtil.SetGameobjectActive(m_arrayCustomSortMenu[i].m_cTglSortTypeCustom, bValue: false);
				}
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

	public void OnSort(List<NKCUnitSortSystem.eSortOption> sortList)
	{
		if (m_bUseDefaultSortAdd)
		{
			sortList = NKCUnitSortSystem.AddDefaultSortOptions(sortList, m_eUnitType, m_bIsCollection);
		}
		dOnSortOption(sortList);
		Close();
	}

	public void OnSort(NKCUnitSortSystem.eSortOption sortOption)
	{
		List<NKCUnitSortSystem.eSortOption> list = new List<NKCUnitSortSystem.eSortOption>();
		list.Add(sortOption);
		if (m_bUseDefaultSortAdd)
		{
			list = NKCUnitSortSystem.AddDefaultSortOptions(list, m_eUnitType, m_bIsCollection);
		}
		dOnSortOption(list);
		Close();
	}
}
