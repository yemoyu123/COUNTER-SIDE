using System;
using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupUnitInfoDetail : NKCUIBase
{
	public enum UnitInfoDetailType
	{
		normal,
		lab,
		gauntlet,
		gauntlet_collection_v2
	}

	public delegate void OnClickOK(NKCUISlot slot);

	public delegate void OnClose();

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_UNIT_INFO";

	private const string ASSET_BUNDLE_NAME_COLLECTION_V2 = "AB_UI_COLLECTION";

	private const string UI_ASSET_NAME = "NKM_UI_UNIT_INFO_POPUP";

	private const string UI_ASSET_NAME_LAB = "NKM_UI_UNIT_INFO_POPUP_LAB";

	private const string UI_ASSET_NAME_OTHER = "NKM_UI_UNIT_INFO_POPUP_OTHER";

	private const string UI_ASSET_NAME_COLLECTION_V2 = "AB_UI_COLLECTION_POPUP_STATS_GUNTLET";

	private static UnitInfoDetailType m_UnitInfoDetailType;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	private static NKCPopupUnitInfoDetail m_Instance;

	public GameObject m_NKM_UI_UNIT_INFO_POPUP_STAT_LIST_Content;

	public GameObject m_objLine;

	public NKCUIComButton m_btnClose;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private List<NKCUIUnitInfoDetailStatSlot> m_lstNKCUIUnitInfoDetailStatSlot = new List<NKCUIUnitInfoDetailStatSlot>();

	private List<NKM_STAT_TYPE> m_lstAllStatType;

	private OnClose m_dOnClose;

	public static NKCPopupUnitInfoDetail Instance
	{
		get
		{
			if (m_Instance == null)
			{
				switch (m_UnitInfoDetailType)
				{
				case UnitInfoDetailType.gauntlet:
					m_loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupUnitInfoDetail>("AB_UI_NKM_UI_UNIT_INFO", "NKM_UI_UNIT_INFO_POPUP_OTHER", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance);
					break;
				case UnitInfoDetailType.gauntlet_collection_v2:
					m_loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupUnitInfoDetail>("AB_UI_COLLECTION", "AB_UI_COLLECTION_POPUP_STATS_GUNTLET", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance);
					break;
				case UnitInfoDetailType.lab:
					m_loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupUnitInfoDetail>("AB_UI_NKM_UI_UNIT_INFO", "NKM_UI_UNIT_INFO_POPUP_LAB", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance);
					break;
				default:
					m_loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupUnitInfoDetail>("AB_UI_NKM_UI_UNIT_INFO", "NKM_UI_UNIT_INFO_POPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance);
					break;
				}
				m_Instance = m_loadedUIData.GetInstance<NKCPopupUnitInfoDetail>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_UNIT_INFO_DETAIL;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public static void InstanceOpen(NKMUnitData unitData, UnitInfoDetailType unitInfoDetailType = UnitInfoDetailType.normal, List<NKMEquipItemData> listNKMEquipItemData = null, OnClose dOnClose = null)
	{
		if (m_Instance != null && m_UnitInfoDetailType != unitInfoDetailType)
		{
			m_loadedUIData.CloseInstance();
			m_loadedUIData = null;
		}
		m_UnitInfoDetailType = unitInfoDetailType;
		Instance.Open(unitData, listNKMEquipItemData, dOnClose);
	}

	private static void CleanupInstance()
	{
		m_Instance.CleanUp();
		m_Instance = null;
	}

	private void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (m_lstAllStatType == null)
		{
			m_lstAllStatType = new List<NKM_STAT_TYPE>();
			foreach (NKM_STAT_TYPE value in Enum.GetValues(typeof(NKM_STAT_TYPE)))
			{
				m_lstAllStatType.Add(value);
			}
		}
		NKCUtil.SetButtonClickDelegate(m_btnClose, base.Close);
	}

	private void Open(NKMUnitData unitData, List<NKMEquipItemData> listNKMEquipItemData = null, OnClose dOnClose = null)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetData(unitData, listNKMEquipItemData);
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_dOnClose = dOnClose;
		if (!base.IsOpen)
		{
			UIOpened();
		}
	}

	public void SetData(NKMUnitData unitData, List<NKMEquipItemData> listNKMEquipItemData = null)
	{
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		if (unitStatTemplet == null)
		{
			return;
		}
		bool bPvP = false;
		NKMStatData nKMStatData = new NKMStatData();
		nKMStatData.Init();
		nKMStatData.MakeBaseStat(null, bPvP, unitData, unitStatTemplet.m_StatData);
		if (listNKMEquipItemData == null)
		{
			nKMStatData.MakeBaseBonusFactor(unitData, NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.EquipItems, null, null);
		}
		else
		{
			NKMInventoryData nKMInventoryData = new NKMInventoryData();
			nKMInventoryData.AddItemEquip(listNKMEquipItemData);
			nKMStatData.MakeBaseBonusFactor(unitData, nKMInventoryData.EquipItems, null, null);
		}
		HashSet<NKM_STAT_TYPE> unitBonusStatList = nKMStatData.GetUnitBonusStatList();
		unitBonusStatList.RemoveWhere(NKMUnitStatManager.IsMainFactorStat);
		for (int i = m_lstNKCUIUnitInfoDetailStatSlot.Count; i < unitBonusStatList.Count; i++)
		{
			if (m_UnitInfoDetailType == UnitInfoDetailType.gauntlet_collection_v2)
			{
				m_lstNKCUIUnitInfoDetailStatSlot.Add(NKCUIUnitInfoDetailStatSlot.GetNewInstance("AB_UI_COLLECTION", "AB_UI_COLLECTION_INFO_STATS_GUNTLET_SLOT", m_NKM_UI_UNIT_INFO_POPUP_STAT_LIST_Content.transform));
			}
			else
			{
				m_lstNKCUIUnitInfoDetailStatSlot.Add(NKCUIUnitInfoDetailStatSlot.GetNewInstance("AB_UI_NKM_UI_UNIT_INFO", "NKM_UI_UNIT_INFO_POPUP_STAT_LIST_SLOT", m_NKM_UI_UNIT_INFO_POPUP_STAT_LIST_Content.transform));
			}
		}
		int num = 0;
		int num2 = 0;
		for (int j = 0; j < m_lstAllStatType.Count; j++)
		{
			if (num2 >= unitBonusStatList.Count)
			{
				break;
			}
			if (unitBonusStatList.Contains(m_lstAllStatType[j]))
			{
				m_lstNKCUIUnitInfoDetailStatSlot[num2].SetData(m_lstAllStatType[j], nKMStatData);
				m_lstNKCUIUnitInfoDetailStatSlot[num2].transform.SetSiblingIndex(num2);
				num2++;
				if (m_lstAllStatType[j] <= NKM_STAT_TYPE.NST_EVADE)
				{
					num++;
				}
			}
		}
		m_objLine.transform.SetSiblingIndex(num);
		for (int k = unitBonusStatList.Count; k < m_lstNKCUIUnitInfoDetailStatSlot.Count; k++)
		{
			NKCUtil.SetGameobjectActive(m_lstNKCUIUnitInfoDetailStatSlot[k], bValue: false);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (m_dOnClose != null)
		{
			m_dOnClose();
		}
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public void CleanUp()
	{
		foreach (NKCUIUnitInfoDetailStatSlot item in m_Instance.m_lstNKCUIUnitInfoDetailStatSlot)
		{
			item.Clear();
		}
	}
}
