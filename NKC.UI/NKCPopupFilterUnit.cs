using System.Collections.Generic;
using NKM;
using NKM.Templet;

namespace NKC.UI;

public class NKCPopupFilterUnit : NKCUIBase
{
	public enum FILTER_OPEN_TYPE
	{
		NONE,
		NORMAL,
		COLLECTION,
		SELECTION,
		ALLUNIT_DEV
	}

	public delegate void OnFilterSetChange(HashSet<NKCUnitSortSystem.eFilterOption> setFilterOption);

	public enum FILTER_TYPE
	{
		NONE,
		UNIT,
		OPERATOR,
		SHIP
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_selection";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FILTER_UNIT";

	private static NKCPopupFilterUnit m_Instance;

	public NKCUIComStateButton m_btnBackground;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnReset;

	public NKCPopupFilterSubUIUnit m_cNKCPopupFilterSubUIUnit;

	private OnFilterSetChange dOnFilterSetChange;

	private HashSet<NKCUnitSortSystem.eFilterOption> m_setFilterOption;

	private bool m_bInitComplete;

	public static NKCPopupFilterUnit Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFilterUnit>("ab_ui_nkm_ui_popup_selection", "NKM_UI_POPUP_FILTER_UNIT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFilterUnit>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Invalid;

	public override string MenuName => "필 터";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		if (m_btnBackground != null)
		{
			m_btnBackground.PointerClick.RemoveAllListeners();
			m_btnBackground.PointerClick.AddListener(OnClickOk);
		}
		if (m_btnOk != null)
		{
			m_btnOk.PointerClick.RemoveAllListeners();
			m_btnOk.PointerClick.AddListener(OnClickOk);
			NKCUtil.SetHotkey(m_btnOk, HotkeyEventType.Confirm);
		}
		if (m_btnReset != null)
		{
			m_btnReset.PointerClick.RemoveAllListeners();
			m_btnReset.PointerClick.AddListener(OnClickReset);
		}
		m_bInitComplete = true;
	}

	public void Open(HashSet<NKCUnitSortSystem.eFilterOption> setFilterOption, OnFilterSetChange onFilterSetChange, FILTER_TYPE targetType, FILTER_OPEN_TYPE filterOpenType)
	{
		switch (targetType)
		{
		case FILTER_TYPE.UNIT:
			Open(MakeDefaultFilterOption(NKM_UNIT_TYPE.NUT_NORMAL, filterOpenType), setFilterOption, onFilterSetChange, targetType);
			break;
		case FILTER_TYPE.SHIP:
			Open(MakeDefaultFilterOption(NKM_UNIT_TYPE.NUT_SHIP, filterOpenType), setFilterOption, onFilterSetChange, targetType);
			break;
		case FILTER_TYPE.OPERATOR:
			Open(MakeDefaultFilterOption(NKM_UNIT_TYPE.NUT_OPERATOR, filterOpenType), setFilterOption, onFilterSetChange, targetType);
			break;
		}
	}

	public void Open(HashSet<NKCUnitSortSystem.eFilterCategory> setFilterCategory, HashSet<NKCUnitSortSystem.eFilterOption> setCurrentFilterOption, OnFilterSetChange onFilterSetChange, FILTER_TYPE targetType, bool bShowTrophyFilter = false)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		dOnFilterSetChange = onFilterSetChange;
		if (setCurrentFilterOption != null)
		{
			m_setFilterOption = setCurrentFilterOption;
		}
		else
		{
			m_setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		}
		m_cNKCPopupFilterSubUIUnit.OpenFilterPopup(m_setFilterOption, setFilterCategory, OnSelectFilterOption, bShowTrophyFilter);
		UIOpened();
	}

	public void OnSelectFilterOption(NKCUnitSortSystem.eFilterOption selectOption)
	{
		if (m_setFilterOption == null)
		{
			m_setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		}
		if (m_setFilterOption.Contains(selectOption))
		{
			m_setFilterOption.Remove(selectOption);
		}
		else
		{
			m_setFilterOption.Add(selectOption);
		}
		dOnFilterSetChange?.Invoke(m_setFilterOption);
	}

	public void OnClickOk()
	{
		Close();
	}

	public void OnClickReset()
	{
		m_cNKCPopupFilterSubUIUnit.ResetFilter();
		bool num = m_setFilterOption.Contains(NKCUnitSortSystem.eFilterOption.Collection_HasAchieve);
		m_setFilterOption.Clear();
		if (num)
		{
			m_setFilterOption.Add(NKCUnitSortSystem.eFilterOption.Collection_HasAchieve);
		}
		dOnFilterSetChange?.Invoke(m_setFilterOption);
	}

	public static HashSet<NKCUnitSortSystem.eFilterCategory> MakeGlobalBanSortOption()
	{
		return new HashSet<NKCUnitSortSystem.eFilterCategory>
		{
			NKCUnitSortSystem.eFilterCategory.UnitType,
			NKCUnitSortSystem.eFilterCategory.UnitRole,
			NKCUnitSortSystem.eFilterCategory.UnitTargetType,
			NKCUnitSortSystem.eFilterCategory.Cost,
			NKCUnitSortSystem.eFilterCategory.Rarity
		};
	}

	public static HashSet<NKCUnitSortSystem.eFilterCategory> MakeDefaultFilterOption(NKM_UNIT_TYPE unitType, FILTER_OPEN_TYPE filterOpenType)
	{
		HashSet<NKCUnitSortSystem.eFilterCategory> hashSet = new HashSet<NKCUnitSortSystem.eFilterCategory>();
		if (filterOpenType == FILTER_OPEN_TYPE.ALLUNIT_DEV)
		{
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.MonsterType);
			filterOpenType = FILTER_OPEN_TYPE.NORMAL;
		}
		switch (unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.UnitType);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.UnitRole);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.UnitMoveType);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.UnitTargetType);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.Cost);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.Rarity);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.SpecialType);
			if (NKMOpenTagManager.IsOpened("UNIT_SOURCE_TYPE"))
			{
				hashSet.Add(NKCUnitSortSystem.eFilterCategory.SourceType);
			}
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.ShipType);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.Rarity);
			break;
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.Rarity);
			break;
		}
		switch (filterOpenType)
		{
		case FILTER_OPEN_TYPE.NORMAL:
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.Level);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.Decked);
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.Locked);
			break;
		case FILTER_OPEN_TYPE.SELECTION:
			hashSet.Add(NKCUnitSortSystem.eFilterCategory.Have);
			break;
		}
		return hashSet;
	}

	public static HashSet<NKCOperatorSortSystem.eFilterCategory> MakeOprDefaultFilterOption(NKM_UNIT_TYPE unitType, FILTER_OPEN_TYPE filterOpenType)
	{
		HashSet<NKCOperatorSortSystem.eFilterCategory> hashSet = new HashSet<NKCOperatorSortSystem.eFilterCategory>();
		if ((uint)(unitType - 2) > 1u && unitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			hashSet.Add(NKCOperatorSortSystem.eFilterCategory.Rarity);
		}
		switch (filterOpenType)
		{
		case FILTER_OPEN_TYPE.NORMAL:
			hashSet.Add(NKCOperatorSortSystem.eFilterCategory.Level);
			hashSet.Add(NKCOperatorSortSystem.eFilterCategory.Decked);
			hashSet.Add(NKCOperatorSortSystem.eFilterCategory.Locked);
			break;
		case FILTER_OPEN_TYPE.SELECTION:
			hashSet.Add(NKCOperatorSortSystem.eFilterCategory.Have);
			break;
		}
		return hashSet;
	}
}
