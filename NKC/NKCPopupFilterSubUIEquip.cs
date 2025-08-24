using System.Collections.Generic;
using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCPopupFilterSubUIEquip : MonoBehaviour
{
	public delegate void OnFilterOptionChange(NKCEquipSortSystem equipListOption, NKCEquipSortSystem.eFilterOption filterOption);

	[Header("StatType")]
	public GameObject m_objStatType;

	public NKCPopupFilterSubUIEquipStatSlot m_btnStat_01;

	public NKCPopupFilterSubUIEquipStatSlot m_btnStat_02;

	public NKCPopupFilterSubUIEquipStatSlot m_btnStat_03;

	public NKCPopupFilterSubUIEquipStatSlot m_btnStatSet;

	[Header("UnitType")]
	public GameObject m_objUnitType;

	public NKCUIComToggle m_tglCounter;

	public NKCUIComToggle m_tglSoldier;

	public NKCUIComToggle m_tglMechanic;

	[Header("EquipType")]
	public GameObject m_objEquipType;

	public NKCUIComToggle m_tglWeapon;

	public NKCUIComToggle m_tglArmor;

	public NKCUIComToggle m_tglAcc;

	public NKCUIComToggle m_tglEnchant;

	[Header("Tier")]
	public GameObject m_objTier;

	public NKCUIComToggle m_tglTier_7;

	public NKCUIComToggle m_tglTier_6;

	public NKCUIComToggle m_tglTier_5;

	public NKCUIComToggle m_tglTier_4;

	public NKCUIComToggle m_tglTier_3;

	public NKCUIComToggle m_tglTier_2;

	public NKCUIComToggle m_tglTier_1;

	[Header("Rarity")]
	public GameObject m_objRarity;

	public NKCUIComToggle m_tglRarity_SSR;

	public NKCUIComToggle m_tglRarity_SR;

	public NKCUIComToggle m_tglRarity_R;

	public NKCUIComToggle m_tglRarity_N;

	[Header("SetOptionPart")]
	public GameObject m_objSetOptionPart;

	public NKCUIComToggle m_tglSetOption_Part2;

	public NKCUIComToggle m_tglSetOption_Part3;

	public NKCUIComToggle m_tglSetOption_Part4;

	[Header("SetOptionPart")]
	public GameObject m_objSetOptionType;

	public NKCUIComToggle m_tglSetOption_Attack;

	public NKCUIComToggle m_tglSetOption_Defence;

	public NKCUIComToggle m_tglSetOption_Etc;

	[Header("Equipped")]
	public GameObject m_objEquipped;

	public NKCUIComToggle m_tglEquipped;

	public NKCUIComToggle m_tglUnUsed;

	[Header("Locked")]
	public GameObject m_objLocked;

	public NKCUIComToggle m_tglLocked;

	public NKCUIComToggle m_tglUnlocked;

	[Header("Have")]
	public GameObject m_objHave;

	public NKCUIComToggle m_tglHave;

	public NKCUIComToggle m_tglNotHave;

	[Header("전용장비")]
	public GameObject m_objPrivate;

	public NKCUIComToggle m_tglPrivate;

	public NKCUIComToggle m_tglPrivateAwaken;

	public NKCUIComToggle m_tglNonPrivate;

	public NKCUIComToggle m_tglRelic;

	private RectTransform m_RectTransform;

	private Dictionary<NKCEquipSortSystem.eFilterOption, NKCPopupFilterSubUIEquipStatSlot> m_dicFilterStatBtn = new Dictionary<NKCEquipSortSystem.eFilterOption, NKCPopupFilterSubUIEquipStatSlot>();

	private Dictionary<NKCEquipSortSystem.eFilterOption, NKCUIComToggle> m_dicFilterTgl = new Dictionary<NKCEquipSortSystem.eFilterOption, NKCUIComToggle>();

	private OnFilterOptionChange dOnFilterOptionChange;

	private NKCEquipSortSystem m_ssActive;

	private List<NKM_STAT_TYPE> m_lstSelectedType = new List<NKM_STAT_TYPE>
	{
		NKM_STAT_TYPE.NST_RANDOM,
		NKM_STAT_TYPE.NST_RANDOM,
		NKM_STAT_TYPE.NST_RANDOM
	};

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
		m_dicFilterTgl.Clear();
		m_dicFilterStatBtn.Clear();
		if (m_objStatType != null)
		{
			SetButtonListner(m_btnStat_01, NKCEquipSortSystem.eFilterOption.Equip_Stat_01, isSetOptionSlot: false, 0);
			SetButtonListner(m_btnStat_02, NKCEquipSortSystem.eFilterOption.Equip_Stat_02, isSetOptionSlot: false, 1);
			SetButtonListner(m_btnStat_03, NKCEquipSortSystem.eFilterOption.Equip_Stat_Potential, isSetOptionSlot: false, 2);
			SetButtonListner(m_btnStatSet, NKCEquipSortSystem.eFilterOption.Equip_Stat_SetOption, isSetOptionSlot: true, 3);
		}
		SetToggleListner(m_tglCounter, NKCEquipSortSystem.eFilterOption.Equip_Counter);
		SetToggleListner(m_tglSoldier, NKCEquipSortSystem.eFilterOption.Equip_Soldier);
		SetToggleListner(m_tglMechanic, NKCEquipSortSystem.eFilterOption.Equip_Mechanic);
		SetToggleListner(m_tglWeapon, NKCEquipSortSystem.eFilterOption.Equip_Weapon);
		SetToggleListner(m_tglArmor, NKCEquipSortSystem.eFilterOption.Equip_Armor);
		SetToggleListner(m_tglAcc, NKCEquipSortSystem.eFilterOption.Equip_Acc);
		SetToggleListner(m_tglEnchant, NKCEquipSortSystem.eFilterOption.Equip_Enchant);
		SetToggleListner(m_tglTier_7, NKCEquipSortSystem.eFilterOption.Equip_Tier_7);
		SetToggleListner(m_tglTier_6, NKCEquipSortSystem.eFilterOption.Equip_Tier_6);
		SetToggleListner(m_tglTier_5, NKCEquipSortSystem.eFilterOption.Equip_Tier_5);
		SetToggleListner(m_tglTier_4, NKCEquipSortSystem.eFilterOption.Equip_Tier_4);
		SetToggleListner(m_tglTier_3, NKCEquipSortSystem.eFilterOption.Equip_Tier_3);
		SetToggleListner(m_tglTier_2, NKCEquipSortSystem.eFilterOption.Equip_Tier_2);
		SetToggleListner(m_tglTier_1, NKCEquipSortSystem.eFilterOption.Equip_Tier_1);
		SetToggleListner(m_tglRarity_SSR, NKCEquipSortSystem.eFilterOption.Equip_Rarity_SSR);
		SetToggleListner(m_tglRarity_SR, NKCEquipSortSystem.eFilterOption.Equip_Rarity_SR);
		SetToggleListner(m_tglRarity_R, NKCEquipSortSystem.eFilterOption.Equip_Rarity_R);
		SetToggleListner(m_tglRarity_N, NKCEquipSortSystem.eFilterOption.Equip_Rarity_N);
		SetToggleListner(m_tglSetOption_Part2, NKCEquipSortSystem.eFilterOption.Equip_Set_Part_2);
		SetToggleListner(m_tglSetOption_Part3, NKCEquipSortSystem.eFilterOption.Equip_Set_Part_3);
		SetToggleListner(m_tglSetOption_Part4, NKCEquipSortSystem.eFilterOption.Equip_Set_Part_4);
		SetToggleListner(m_tglSetOption_Attack, NKCEquipSortSystem.eFilterOption.Equip_Set_Effect_Red);
		SetToggleListner(m_tglSetOption_Defence, NKCEquipSortSystem.eFilterOption.Equip_Set_Effect_Blue);
		SetToggleListner(m_tglSetOption_Etc, NKCEquipSortSystem.eFilterOption.Equip_Set_Effect_Yellow);
		SetToggleListner(m_tglEquipped, NKCEquipSortSystem.eFilterOption.Equip_Equipped);
		SetToggleListner(m_tglUnUsed, NKCEquipSortSystem.eFilterOption.Equip_Unused);
		SetToggleListner(m_tglLocked, NKCEquipSortSystem.eFilterOption.Equip_Locked);
		SetToggleListner(m_tglUnlocked, NKCEquipSortSystem.eFilterOption.Equip_Unlocked);
		SetToggleListner(m_tglHave, NKCEquipSortSystem.eFilterOption.Equip_Have);
		SetToggleListner(m_tglNotHave, NKCEquipSortSystem.eFilterOption.Equip_NotHave);
		SetToggleListner(m_tglPrivate, NKCEquipSortSystem.eFilterOption.Equip_Private);
		SetToggleListner(m_tglPrivateAwaken, NKCEquipSortSystem.eFilterOption.Equip_Private_Awaken);
		SetToggleListner(m_tglNonPrivate, NKCEquipSortSystem.eFilterOption.Equip_Non_Private);
		SetToggleListner(m_tglRelic, NKCEquipSortSystem.eFilterOption.Equip_Relic);
		m_bInitComplete = true;
	}

	private void SetButtonListner(NKCPopupFilterSubUIEquipStatSlot statSlot, NKCEquipSortSystem.eFilterOption filterOption, bool isSetOptionSlot, int idx)
	{
		if (!(statSlot != null))
		{
			return;
		}
		if (!m_dicFilterStatBtn.ContainsKey(filterOption))
		{
			m_dicFilterStatBtn[filterOption] = statSlot;
		}
		if (isSetOptionSlot)
		{
			statSlot.SetData(0);
		}
		else
		{
			statSlot.SetData(NKM_STAT_TYPE.NST_RANDOM);
		}
		NKCUIComStateButton button = statSlot.GetButton();
		if (button != null)
		{
			button.PointerClick.RemoveAllListeners();
			button.PointerClick.AddListener(delegate
			{
				OpenStatPopup(isSetOptionSlot, filterOption, idx);
			});
		}
	}

	private void SetToggleListner(NKCUIComToggle toggle, NKCEquipSortSystem.eFilterOption filterOption)
	{
		if (toggle != null)
		{
			m_dicFilterTgl.Add(filterOption, toggle);
			toggle.OnValueChanged.RemoveAllListeners();
			toggle.OnValueChanged.AddListener(delegate(bool value)
			{
				OnFilterButton(value, filterOption);
			});
		}
	}

	public void OpenFilterPopup(NKCEquipSortSystem ssActive, HashSet<NKCEquipSortSystem.eFilterCategory> setFilterCategory, OnFilterOptionChange onFilterOptionChange, bool bEnableEnchantModuleFilter = false)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		if (NKCPopupFilterSubUIEquipStat.IsInstanceOpen())
		{
			NKCPopupFilterSubUIEquipStat.Instance.Close();
		}
		m_ssActive = ssActive;
		dOnFilterOptionChange = onFilterOptionChange;
		SetFilter(ssActive.m_EquipListOptions, bEnableEnchantModuleFilter);
		SetStatFilter(ssActive.m_EquipListOptions);
		NKCUtil.SetGameobjectActive(m_objEquipped, setFilterCategory.Contains(NKCEquipSortSystem.eFilterCategory.Equipped));
		NKCUtil.SetGameobjectActive(m_objEquipType, setFilterCategory.Contains(NKCEquipSortSystem.eFilterCategory.EquipType));
		NKCUtil.SetGameobjectActive(m_objHave, setFilterCategory.Contains(NKCEquipSortSystem.eFilterCategory.Have));
		NKCUtil.SetGameobjectActive(m_objLocked, setFilterCategory.Contains(NKCEquipSortSystem.eFilterCategory.Locked));
		NKCUtil.SetGameobjectActive(m_objRarity, setFilterCategory.Contains(NKCEquipSortSystem.eFilterCategory.Rarity));
		NKCUtil.SetGameobjectActive(m_objSetOptionPart, setFilterCategory.Contains(NKCEquipSortSystem.eFilterCategory.SetOptionPart));
		NKCUtil.SetGameobjectActive(m_objSetOptionType, setFilterCategory.Contains(NKCEquipSortSystem.eFilterCategory.SetOptionType));
		NKCUtil.SetGameobjectActive(m_objTier, setFilterCategory.Contains(NKCEquipSortSystem.eFilterCategory.Tier));
		NKCUtil.SetGameobjectActive(m_objUnitType, setFilterCategory.Contains(NKCEquipSortSystem.eFilterCategory.UnitType));
		NKCUtil.SetGameobjectActive(m_objStatType, setFilterCategory.Contains(NKCEquipSortSystem.eFilterCategory.StatType));
		NKCUtil.SetGameobjectActive(m_objPrivate, setFilterCategory.Contains(NKCEquipSortSystem.eFilterCategory.PrivateEquip));
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	private void SetFilter(NKCEquipSortSystem.EquipListOptions equipListOption, bool bEnableEnchant = false)
	{
		m_bReset = true;
		foreach (KeyValuePair<NKCEquipSortSystem.eFilterOption, NKCUIComToggle> item in m_dicFilterTgl)
		{
			if (equipListOption.setFilterOption.Contains(item.Key))
			{
				m_dicFilterTgl[item.Key].Select(bSelect: true);
			}
			else
			{
				m_dicFilterTgl[item.Key].Select(bSelect: false);
			}
		}
		if (m_dicFilterTgl.ContainsKey(NKCEquipSortSystem.eFilterOption.Equip_Enchant))
		{
			if (!bEnableEnchant)
			{
				m_dicFilterTgl[NKCEquipSortSystem.eFilterOption.Equip_Enchant].Lock();
			}
			else
			{
				m_dicFilterTgl[NKCEquipSortSystem.eFilterOption.Equip_Enchant].UnLock();
			}
		}
		m_bReset = false;
	}

	private void SetStatFilter(NKCEquipSortSystem.EquipListOptions equipListOption)
	{
		m_bReset = true;
		if (m_objStatType != null)
		{
			m_btnStat_01.SetData(equipListOption.FilterStatType_01, equipListOption.FilterStatType_01 != NKM_STAT_TYPE.NST_RANDOM);
			m_btnStat_02.SetData(equipListOption.FilterStatType_02, equipListOption.FilterStatType_02 != NKM_STAT_TYPE.NST_RANDOM);
			m_btnStat_03.SetData(equipListOption.FilterStatType_Potential, equipListOption.FilterStatType_Potential != NKM_STAT_TYPE.NST_RANDOM);
			if (equipListOption.FilterSetOptionID <= 0)
			{
				m_btnStatSet.SetData(null);
			}
			else
			{
				m_btnStatSet.SetData(equipListOption.FilterSetOptionID, bSelected: true);
			}
		}
		m_bReset = false;
	}

	private void OnFilterButton(bool bSelect, NKCEquipSortSystem.eFilterOption filterOption)
	{
		if (!m_dicFilterTgl.ContainsKey(filterOption))
		{
			return;
		}
		NKCUIComToggle nKCUIComToggle = m_dicFilterTgl[filterOption];
		if (!(nKCUIComToggle != null))
		{
			return;
		}
		nKCUIComToggle.Select(bSelect, bForce: true, bImmediate: true);
		if (!m_bReset)
		{
			if (m_ssActive.FilterSet == null)
			{
				m_ssActive.FilterSet = new HashSet<NKCEquipSortSystem.eFilterOption>();
			}
			if (m_ssActive.FilterSet.Contains(filterOption))
			{
				m_ssActive.FilterSet.Remove(filterOption);
			}
			else
			{
				m_ssActive.FilterSet.Add(filterOption);
			}
			dOnFilterOptionChange?.Invoke(m_ssActive, filterOption);
		}
	}

	private void OnStatFilterOptionChanged(NKM_STAT_TYPE statType, int setOptionID, int selectedSlotIdx)
	{
		if (NKCPopupFilterSubUIEquipStat.IsInstanceOpen())
		{
			NKCPopupFilterSubUIEquipStat.Instance.Close();
		}
		NKCEquipSortSystem.eFilterOption eFilterOption = NKCEquipSortSystem.eFilterOption.Nothing;
		switch (selectedSlotIdx)
		{
		case 0:
			if (m_ssActive.FilterStatType_01 != NKM_STAT_TYPE.NST_RANDOM)
			{
				m_ssActive.FilterStatType_01 = NKM_STAT_TYPE.NST_RANDOM;
			}
			else
			{
				m_ssActive.FilterStatType_01 = statType;
			}
			m_dicFilterStatBtn[NKCEquipSortSystem.eFilterOption.Equip_Stat_01].SetData(m_ssActive.FilterStatType_01, m_ssActive.FilterStatType_01 != NKM_STAT_TYPE.NST_RANDOM);
			eFilterOption = NKCEquipSortSystem.eFilterOption.Equip_Stat_01;
			break;
		case 1:
			if (m_ssActive.FilterStatType_02 != NKM_STAT_TYPE.NST_RANDOM)
			{
				m_ssActive.FilterStatType_02 = NKM_STAT_TYPE.NST_RANDOM;
			}
			else
			{
				m_ssActive.FilterStatType_02 = statType;
			}
			m_dicFilterStatBtn[NKCEquipSortSystem.eFilterOption.Equip_Stat_02].SetData(m_ssActive.FilterStatType_02, m_ssActive.FilterStatType_02 != NKM_STAT_TYPE.NST_RANDOM);
			eFilterOption = NKCEquipSortSystem.eFilterOption.Equip_Stat_02;
			break;
		case 2:
			if (m_ssActive.FilterStatType_Potential != NKM_STAT_TYPE.NST_RANDOM)
			{
				m_ssActive.FilterStatType_Potential = NKM_STAT_TYPE.NST_RANDOM;
			}
			else
			{
				m_ssActive.FilterStatType_Potential = statType;
			}
			m_dicFilterStatBtn[NKCEquipSortSystem.eFilterOption.Equip_Stat_Potential].SetData(m_ssActive.FilterStatType_Potential, m_ssActive.FilterStatType_Potential != NKM_STAT_TYPE.NST_RANDOM);
			eFilterOption = NKCEquipSortSystem.eFilterOption.Equip_Stat_Potential;
			break;
		case 3:
			if (m_ssActive.FilterStatType_SetOptionID > 0)
			{
				m_ssActive.FilterStatType_SetOptionID = 0;
			}
			else
			{
				m_ssActive.FilterStatType_SetOptionID = setOptionID;
			}
			m_dicFilterStatBtn[NKCEquipSortSystem.eFilterOption.Equip_Stat_SetOption].SetData(m_ssActive.FilterStatType_SetOptionID, m_ssActive.FilterStatType_SetOptionID > 0);
			eFilterOption = NKCEquipSortSystem.eFilterOption.Equip_Stat_SetOption;
			break;
		}
		if (!m_ssActive.FilterSet.Contains(eFilterOption))
		{
			m_ssActive.FilterSet.Add(eFilterOption);
		}
		else if (m_ssActive.FilterSet.Contains(eFilterOption))
		{
			m_ssActive.FilterSet.Remove(eFilterOption);
		}
		dOnFilterOptionChange?.Invoke(m_ssActive, eFilterOption);
	}

	private void OpenStatPopup(bool bIsSetOptionSlot, NKCEquipSortSystem.eFilterOption filterOption, int selectedSlotIdx)
	{
		if (m_ssActive.FilterSet.Contains(filterOption) && m_dicFilterStatBtn.ContainsKey(filterOption) && m_dicFilterStatBtn[filterOption].GetButton().m_bSelect)
		{
			OnStatFilterOptionChanged(NKM_STAT_TYPE.NST_RANDOM, 0, selectedSlotIdx);
			return;
		}
		if (NKCPopupFilterSubUIEquipStat.IsInstanceOpen())
		{
			NKCPopupFilterSubUIEquipStat.Instance.Close();
		}
		List<NKM_STAT_TYPE> list = new List<NKM_STAT_TYPE>
		{
			NKM_STAT_TYPE.NST_RANDOM,
			NKM_STAT_TYPE.NST_RANDOM
		};
		if (selectedSlotIdx < 2)
		{
			if (m_ssActive.FilterStatType_01 != NKM_STAT_TYPE.NST_RANDOM)
			{
				list[0] = m_ssActive.FilterStatType_01;
			}
			if (m_ssActive.FilterStatType_02 != NKM_STAT_TYPE.NST_RANDOM)
			{
				list[1] = m_ssActive.FilterStatType_02;
			}
		}
		NKCPopupFilterSubUIEquipStat.Instance.Open(bIsSetOptionSlot, list, OnStatFilterOptionChanged, selectedSlotIdx);
	}

	private bool IsAnyStatSlotSelected()
	{
		if (m_ssActive.FilterStatType_01 != NKM_STAT_TYPE.NST_RANDOM)
		{
			return true;
		}
		if (m_ssActive.FilterStatType_02 != NKM_STAT_TYPE.NST_RANDOM)
		{
			return true;
		}
		if (m_ssActive.FilterStatType_Potential != NKM_STAT_TYPE.NST_RANDOM)
		{
			return true;
		}
		if (m_ssActive.FilterStatType_SetOptionID > 0)
		{
			return true;
		}
		return false;
	}

	public void ResetFilter()
	{
		m_bReset = true;
		NKCUIComToggle[] componentsInChildren = base.transform.GetComponentsInChildren<NKCUIComToggle>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Select(bSelect: false);
		}
		if (m_objStatType != null)
		{
			for (int j = 34; j <= 37; j++)
			{
				NKCPopupFilterSubUIEquipStatSlot nKCPopupFilterSubUIEquipStatSlot = m_dicFilterStatBtn[(NKCEquipSortSystem.eFilterOption)j];
				if (nKCPopupFilterSubUIEquipStatSlot.IsSetOptionSlot)
				{
					nKCPopupFilterSubUIEquipStatSlot.SetData(0);
				}
				else
				{
					nKCPopupFilterSubUIEquipStatSlot.SetData(NKM_STAT_TYPE.NST_RANDOM);
				}
			}
		}
		m_ssActive.FilterStatType_SetOptionID = 0;
		m_ssActive.FilterStatType_01 = NKM_STAT_TYPE.NST_RANDOM;
		m_ssActive.FilterStatType_02 = NKM_STAT_TYPE.NST_RANDOM;
		m_ssActive.FilterStatType_Potential = NKM_STAT_TYPE.NST_RANDOM;
		if (NKCPopupFilterSubUIEquipStat.IsInstanceOpen())
		{
			NKCPopupFilterSubUIEquipStat.Instance.Close();
		}
		m_bReset = false;
	}
}
