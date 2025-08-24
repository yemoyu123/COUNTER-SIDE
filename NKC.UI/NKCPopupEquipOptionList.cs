using System;
using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEquipOptionList : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_FACTORY";

	public const string UI_ASSET_NAME = "NKM_UI_FACTORY_EQUIP_OPTION_POPUP";

	[Header("옵션 1,2")]
	public GameObject m_NKM_UI_FACTORY_EQUIP_OPTION_POPUP_LIST01;

	public GameObject m_NKM_UI_FACTORY_EQUIP_OPTION_POPUP_LIST02;

	[Space]
	public Transform m_trNKM_UI_UNIT_INFO_POPUP_LIST_Content_01;

	public Transform m_trNKM_UI_UNIT_INFO_POPUP_LIST_Content_02;

	[Header("팝업 설명")]
	public Text m_lbDesc;

	[Space]
	public Text m_lbOptionRate1;

	public Text m_lbOptionRate2;

	[Header("버튼들")]
	public GameObject m_objProbability;

	public NKCUIComStateButton m_csbtn_Probability;

	public NKCUIComStateButton m_NKM_UI_POPUP_OK_BOX_OK;

	public NKCUIComStateButton m_NKM_UI_FACTORY_EQUIP_OPTION_POPUP_CANCEL_BUTTON;

	[Header("슬롯")]
	public NKCPopupEquipOptionListSlot m_pbfNKCPopupEquipOptionListSlot;

	private List<GameObject> m_lstSlots = new List<GameObject>();

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "옵션 목록";

	public void InitUI()
	{
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_OK_BOX_OK, base.Close);
		NKCUtil.SetHotkey(m_NKM_UI_POPUP_OK_BOX_OK, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_EQUIP_OPTION_POPUP_CANCEL_BUTTON, base.Close);
		NKCUtil.SetBindFunction(m_csbtn_Probability, OnClickProbability);
	}

	public void Open(NKMEquipItemData equipData, int ChangeableOptionCnt, string desc)
	{
		if (equipData == null || ChangeableOptionCnt == 0)
		{
			Close();
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			Close();
			return;
		}
		SetData(0, NKMEquipTuningManager.GetEquipRandomStatGroupList(equipTemplet.m_StatGroupID));
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_EQUIP_OPTION_POPUP_LIST01, ChangeableOptionCnt != 2);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_EQUIP_OPTION_POPUP_LIST02, ChangeableOptionCnt >= 2);
		if (ChangeableOptionCnt >= 2)
		{
			SetData(1, NKMEquipTuningManager.GetEquipRandomStatGroupList(equipTemplet.m_StatGroupID_2));
		}
		NKCUtil.SetGameobjectActive(m_lbDesc, !string.IsNullOrEmpty(desc));
		NKCUtil.SetLabelText(m_lbDesc, desc);
		ProbabilityUI(equipTemplet);
		UIOpened();
	}

	public void Open(long equipUID, int ChangeableOptionCnt, string desc)
	{
		if (equipUID != 0L && ChangeableOptionCnt != 0)
		{
			NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(equipUID);
			if (itemEquip != null)
			{
				Open(itemEquip, ChangeableOptionCnt, desc);
			}
		}
	}

	private void SetData(int Idx, IReadOnlyList<NKMEquipRandomStatTemplet> lstStat)
	{
		foreach (NKMEquipRandomStatTemplet item in lstStat)
		{
			NKCPopupEquipOptionListSlot nKCPopupEquipOptionListSlot = UnityEngine.Object.Instantiate(m_pbfNKCPopupEquipOptionListSlot);
			if (!(nKCPopupEquipOptionListSlot != null))
			{
				continue;
			}
			if (Idx == 0)
			{
				nKCPopupEquipOptionListSlot.transform.SetParent(m_trNKM_UI_UNIT_INFO_POPUP_LIST_Content_01, worldPositionStays: false);
			}
			else
			{
				nKCPopupEquipOptionListSlot.transform.SetParent(m_trNKM_UI_UNIT_INFO_POPUP_LIST_Content_02, worldPositionStays: false);
			}
			if (NKCUIForgeTuning.IsPercentStat(item))
			{
				decimal num = new decimal(item.m_MinStatValue);
				num = Math.Round(num * 1000m) / 1000m;
				decimal num2 = new decimal(item.m_MaxStatValue);
				num2 = Math.Round(num2 * 1000m) / 1000m;
				if (NKCUtilString.IsNameReversedIfNegative(item.m_StatType) && num2 < 0m)
				{
					nKCPopupEquipOptionListSlot.SetData(NKCUtilString.GetStatShortName(item.m_StatType, bNegative: true), $"{-num2:P1}~{-num:P1}");
				}
				else
				{
					nKCPopupEquipOptionListSlot.SetData(NKCUtilString.GetStatShortName(item.m_StatType), $"{num:P1}~{num2:P1}");
				}
			}
			else if (NKCUtilString.IsNameReversedIfNegative(item.m_StatType) && item.m_MaxStatValue < 0f)
			{
				nKCPopupEquipOptionListSlot.SetData(NKCUtilString.GetStatShortName(item.m_StatType, bNegative: true), $"{Mathf.Abs(item.m_MaxStatValue)}~{Mathf.Abs(item.m_MinStatValue)}");
			}
			else
			{
				nKCPopupEquipOptionListSlot.SetData(NKCUtilString.GetStatShortName(item.m_StatType), $"{item.m_MinStatValue}~{item.m_MaxStatValue}");
			}
			m_lstSlots.Add(nKCPopupEquipOptionListSlot.gameObject);
		}
	}

	public override void CloseInternal()
	{
		foreach (GameObject lstSlot in m_lstSlots)
		{
			UnityEngine.Object.Destroy(lstSlot);
		}
		base.gameObject.SetActive(value: false);
	}

	private void ProbabilityUI(NKMEquipTemplet _equipTemplet)
	{
		bool bValue = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.OPEN_TAG_RATE_INFO);
		NKCUtil.SetGameobjectActive(m_objProbability, bValue);
		NKCUtil.SetGameobjectActive(m_lbOptionRate1.gameObject, bValue);
		NKCUtil.SetGameobjectActive(m_lbOptionRate2.gameObject, bValue);
		if (_equipTemplet != null)
		{
			IReadOnlyList<NKMEquipRandomStatTemplet> equipRandomStatGroupList = NKMEquipTuningManager.GetEquipRandomStatGroupList(_equipTemplet.m_StatGroupID);
			if (equipRandomStatGroupList != null)
			{
				float num = 100f / (float)(equipRandomStatGroupList.Count - 1);
				NKCUtil.SetLabelText(m_lbOptionRate1, string.Format(NKCUtilString.GET_STRING_EQUIP_SET_RATE_INFO, num));
			}
			IReadOnlyList<NKMEquipRandomStatTemplet> equipRandomStatGroupList2 = NKMEquipTuningManager.GetEquipRandomStatGroupList(_equipTemplet.m_StatGroupID_2);
			if (equipRandomStatGroupList2 != null)
			{
				float num2 = 100f / (float)(equipRandomStatGroupList2.Count - 1);
				NKCUtil.SetLabelText(m_lbOptionRate2, string.Format(NKCUtilString.GET_STRING_EQUIP_SET_RATE_INFO, num2));
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
