using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupShipModuleOption : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_ship_info";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_SHIP_MODULE_OPTION";

	private static NKCPopupShipModuleOption m_Instance;

	public NKCPopupEquipOptionListSlot m_pfbSlot;

	public ScrollRect m_srOption_01;

	public Transform m_trOption_01;

	public ScrollRect m_srOption_02;

	public Transform m_trOption_02;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_csbtn_Probability;

	public GameObject m_objProbability;

	public EventTrigger m_etBG;

	private List<GameObject> m_lstSlots = new List<GameObject>();

	public static NKCPopupShipModuleOption Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupShipModuleOption>("ab_ui_nkm_ui_ship_info", "NKM_UI_POPUP_SHIP_MODULE_OPTION", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupShipModuleOption>();
				m_Instance.Init();
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

	public override string MenuName => "";

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
		foreach (GameObject lstSlot in m_lstSlots)
		{
			Object.Destroy(lstSlot);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_btnOk, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtn_Probability, OnClickProbability);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_etBG.triggers.Add(entry);
	}

	public void Open(NKM_UNIT_STYLE_TYPE styleType, NKM_UNIT_GRADE grade, int moduleIndex)
	{
		NKMShipCommandModuleTemplet nKMShipCommandModuleTemplet = NKMShipManager.GetNKMShipCommandModuleTemplet(styleType, grade, moduleIndex + 1);
		IReadOnlyList<NKMCommandModulePassiveTemplet> passiveListsByGroupId = NKMShipModuleGroupTemplet.GetPassiveListsByGroupId(nKMShipCommandModuleTemplet.Slot1Id);
		IReadOnlyList<NKMCommandModulePassiveTemplet> passiveListsByGroupId2 = NKMShipModuleGroupTemplet.GetPassiveListsByGroupId(nKMShipCommandModuleTemplet.Slot2Id);
		if (passiveListsByGroupId == null || passiveListsByGroupId2 == null)
		{
			return;
		}
		List<NKMCommandModuleRandomStatTemplet> list = new List<NKMCommandModuleRandomStatTemplet>();
		List<NKMCommandModuleRandomStatTemplet> list2 = new List<NKMCommandModuleRandomStatTemplet>();
		List<int> list3 = new List<int>();
		for (int i = 0; i < passiveListsByGroupId.Count; i++)
		{
			if (!list3.Contains(passiveListsByGroupId[i].StatGroupId))
			{
				list3.Add(passiveListsByGroupId[i].StatGroupId);
			}
		}
		List<int> list4 = new List<int>();
		for (int j = 0; j < passiveListsByGroupId2.Count; j++)
		{
			if (!list4.Contains(passiveListsByGroupId2[j].StatGroupId))
			{
				list4.Add(passiveListsByGroupId2[j].StatGroupId);
			}
		}
		for (int k = 0; k < list3.Count; k++)
		{
			IReadOnlyList<NKMCommandModuleRandomStatTemplet> lstStatTemplet_01 = NKMShipModuleGroupTemplet.GetStatListsByGroupId(list3[k]);
			int l;
			for (l = 0; l < lstStatTemplet_01.Count; l++)
			{
				if (list.Find((NKMCommandModuleRandomStatTemplet x) => x.StatType == lstStatTemplet_01[l].StatType) == null)
				{
					list.Add(lstStatTemplet_01[l]);
				}
			}
		}
		for (int num = 0; num < list4.Count; num++)
		{
			IReadOnlyList<NKMCommandModuleRandomStatTemplet> lstStatTemplet_2 = NKMShipModuleGroupTemplet.GetStatListsByGroupId(list4[num]);
			int j2;
			for (j2 = 0; j2 < lstStatTemplet_2.Count; j2++)
			{
				if (list2.Find((NKMCommandModuleRandomStatTemplet x) => x.StatType == lstStatTemplet_2[j2].StatType) == null)
				{
					list2.Add(lstStatTemplet_2[j2]);
				}
			}
		}
		SetData(0, list);
		SetData(1, list2);
		NKCUtil.SetGameobjectActive(m_objProbability, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.OPEN_TAG_RATE_INFO));
		UIOpened();
	}

	private void SetData(int Idx, List<NKMCommandModuleRandomStatTemplet> lstStat)
	{
		foreach (NKMCommandModuleRandomStatTemplet item in lstStat)
		{
			NKCPopupEquipOptionListSlot nKCPopupEquipOptionListSlot = Object.Instantiate(m_pfbSlot);
			if (nKCPopupEquipOptionListSlot != null)
			{
				if (Idx == 0)
				{
					nKCPopupEquipOptionListSlot.transform.SetParent(m_trOption_01, worldPositionStays: false);
				}
				else
				{
					nKCPopupEquipOptionListSlot.transform.SetParent(m_trOption_02, worldPositionStays: false);
				}
				if (NKCUtilString.IsNameReversedIfNegative(item.StatType) && item.MaxStatValue < 0f)
				{
					nKCPopupEquipOptionListSlot.SetData(NKCUtilString.GetStatShortName(item.StatType, bNegative: true), $"{NKCUtilString.GetShipModuleStatValue(item.StatType, item.MaxStatValue)} ~ {NKCUtilString.GetShipModuleStatValue(item.StatType, item.MinStatValue)}");
				}
				else
				{
					nKCPopupEquipOptionListSlot.SetData(NKCUtilString.GetStatShortName(item.StatType), $"{NKCUtilString.GetShipModuleStatValue(item.StatType, item.MinStatValue)} ~ {NKCUtilString.GetShipModuleStatValue(item.StatType, item.MaxStatValue)}");
				}
				m_lstSlots.Add(nKCPopupEquipOptionListSlot.gameObject);
			}
		}
	}

	private void OnClickProbability()
	{
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.KOR))
		{
			Application.OpenURL(NKCUtilString.GET_STRING_RATE_WEB_NOTICE_URL_KOR);
		}
		else if (NKMContentsVersionManager.HasCountryTag(CountryTagType.JPN))
		{
			Application.OpenURL(NKCUtilString.GET_STRING_RATE_WEB_NOTICE_URL_JPN);
		}
		else
		{
			Application.OpenURL(NKCUtilString.GET_STRING_RATE_WEB_NOTICE_URL_GLOBAL);
		}
	}
}
