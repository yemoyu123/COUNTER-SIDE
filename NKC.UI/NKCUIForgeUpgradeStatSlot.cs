using System;
using System.Collections.Generic;
using System.Linq;
using NKC.UI.Tooltip;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeUpgradeStatSlot : MonoBehaviour
{
	private enum STAT_SLOT_TYPE
	{
		MAIN,
		SUB_01,
		SUB_02,
		SET
	}

	public Text m_lbOptionType;

	public Text m_lbOptionName;

	public Text m_lbPrevStat;

	public GameObject m_objArrow;

	public Text m_lbNextStat;

	public NKCUIComStateButton m_btnInfo;

	public NKCUIComStateButton m_btnWarning;

	public NKCUIComStateButton m_btnDetail;

	private NKMEquipItemData m_PrevEquipData;

	private int m_IDX;

	private NKCPopupEquipOptionList m_NKCPopupEquipOption;

	private NKCPopupEquipSetOptionList m_NKCPopupEquipSetOption;

	private NKCPopupEquipOptionList NKCPopupEquipOption
	{
		get
		{
			if (m_NKCPopupEquipOption == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupEquipOptionList>("AB_UI_NKM_UI_FACTORY", "NKM_UI_FACTORY_EQUIP_OPTION_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				m_NKCPopupEquipOption = loadedUIData.GetInstance<NKCPopupEquipOptionList>();
				if (m_NKCPopupEquipOption != null)
				{
					m_NKCPopupEquipOption.InitUI();
				}
			}
			return m_NKCPopupEquipOption;
		}
	}

	private NKCPopupEquipSetOptionList NKCPopupEquipSetOption
	{
		get
		{
			if (m_NKCPopupEquipSetOption == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupEquipSetOptionList>("AB_UI_NKM_UI_FACTORY", "NKM_UI_FACTORY_EQUIP_SET_LIST_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				m_NKCPopupEquipSetOption = loadedUIData.GetInstance<NKCPopupEquipSetOptionList>();
				if (m_NKCPopupEquipSetOption != null)
				{
					m_NKCPopupEquipSetOption.InitUI();
				}
			}
			return m_NKCPopupEquipSetOption;
		}
	}

	public void InitUI()
	{
		m_btnInfo.PointerDown.RemoveAllListeners();
		m_btnInfo.PointerDown.AddListener(OnPressInfoTooltip);
		m_btnInfo.PointerUp.RemoveAllListeners();
		m_btnInfo.PointerUp.AddListener(delegate
		{
			NKCUITooltip.Instance.Close();
		});
		m_btnWarning.PointerDown.RemoveAllListeners();
		m_btnWarning.PointerDown.AddListener(OnPressWarningTooltip);
		m_btnWarning.PointerUp.RemoveAllListeners();
		m_btnWarning.PointerUp.AddListener(delegate
		{
			NKCUITooltip.Instance.Close();
		});
		m_btnDetail.PointerClick.RemoveAllListeners();
		m_btnDetail.PointerClick.AddListener(OnClickDetail);
	}

	public void SetData(int idx, NKMEquipItemData prevEquipData, NKMEquipItemData nextEquipData)
	{
		m_PrevEquipData = prevEquipData;
		m_IDX = idx;
		NKCUtil.SetGameobjectActive(m_btnWarning, idx == 0);
		NKCUtil.SetGameobjectActive(m_btnInfo, idx != 0 && idx < prevEquipData.m_Stat.Count && prevEquipData.m_Stat[idx].type != NKM_STAT_TYPE.NST_RANDOM);
		NKCUtil.SetGameobjectActive(m_btnDetail, idx >= prevEquipData.m_Stat.Count || prevEquipData.m_Stat[idx].type == NKM_STAT_TYPE.NST_RANDOM);
		NKCUtil.SetLabelText(m_lbOptionType, GetOptionType(idx));
		if (idx == prevEquipData.m_Stat.Count)
		{
			NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(prevEquipData.m_SetOptionId);
			if (equipSetOptionTemplet != null)
			{
				NKCUtil.SetLabelText(m_lbOptionName, NKCStringTable.GetString(equipSetOptionTemplet.m_EquipSetName));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbOptionName, NKCUtilString.GET_STRING_FACTORY_UPGRADE_OPTION_SUCCESSION);
			}
			NKCUtil.SetLabelText(m_lbPrevStat, "");
			NKCUtil.SetGameobjectActive(m_objArrow, bValue: false);
			NKCUtil.SetLabelText(m_lbNextStat, "");
			return;
		}
		if (prevEquipData.m_Stat[idx].type == NKM_STAT_TYPE.NST_RANDOM)
		{
			NKCUtil.SetLabelText(m_lbOptionName, NKCUtilString.GET_STRING_FACTORY_UPGRADE_OPTION_SUCCESSION);
			NKCUtil.SetLabelText(m_lbPrevStat, "");
			NKCUtil.SetGameobjectActive(m_objArrow, bValue: false);
			NKCUtil.SetLabelText(m_lbNextStat, "");
			return;
		}
		bool flag = prevEquipData.m_Stat[idx].stat_value < 0f;
		bool flag2 = NKCUtilString.IsNameReversedIfNegative(prevEquipData.m_Stat[idx].type) && flag;
		NKCUtil.SetLabelText(m_lbOptionName, NKCUtilString.GetStatShortName(prevEquipData.m_Stat[idx].type, flag));
		NKCUtil.SetGameobjectActive(m_objArrow, bValue: true);
		if (prevEquipData.m_Stat[idx].stat_value != 0f)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(nextEquipData.m_ItemEquipID);
			IReadOnlyList<NKMEquipRandomStatTemplet> readOnlyList = null;
			if (idx == 1)
			{
				readOnlyList = NKMEquipTuningManager.GetEquipRandomStatGroupList(equipTemplet.m_StatGroupID);
			}
			else if (idx == 2)
			{
				readOnlyList = NKMEquipTuningManager.GetEquipRandomStatGroupList(equipTemplet.m_StatGroupID_2);
			}
			NKMEquipRandomStatTemplet nKMEquipRandomStatTemplet = null;
			if (readOnlyList != null && readOnlyList.Count > 0)
			{
				nKMEquipRandomStatTemplet = readOnlyList.ToList().Find((NKMEquipRandomStatTemplet x) => x.m_StatType == prevEquipData.m_Stat[idx].type);
			}
			if ((nKMEquipRandomStatTemplet != null && NKCUIForgeTuning.IsPercentStat(nKMEquipRandomStatTemplet)) || NKMUnitStatManager.IsPercentStat(prevEquipData.m_Stat[idx].type))
			{
				float num = prevEquipData.m_Stat[idx].stat_value;
				if (flag2)
				{
					num = Mathf.Abs(num);
				}
				decimal num2 = new decimal(num + (float)prevEquipData.m_EnchantLevel * prevEquipData.m_Stat[idx].stat_level_value);
				num2 = Math.Round(num2 * 1000m) / 1000m;
				NKCUtil.SetLabelText(m_lbPrevStat, $"{num2:P1}");
				if (readOnlyList == null)
				{
					float num3 = nextEquipData.m_Stat[idx].stat_value;
					if (flag2)
					{
						num3 = Mathf.Abs(num3);
					}
					NKCUtil.SetLabelText(m_lbNextStat, $"{Math.Round(num3 * 1000f) / 1000.0:P1}");
					return;
				}
				{
					foreach (NKMEquipRandomStatTemplet item in readOnlyList)
					{
						if (item.m_StatType == prevEquipData.m_Stat[idx].type)
						{
							NKCUtil.SetLabelText(msg: $"{Math.Round(new decimal(item.m_MaxStatValue) * 1000m) / 1000m:P1}", label: m_lbNextStat);
							break;
						}
					}
					return;
				}
			}
			float num4 = prevEquipData.m_Stat[idx].stat_value + (float)prevEquipData.m_EnchantLevel * prevEquipData.m_Stat[idx].stat_level_value;
			if (flag2)
			{
				num4 = Mathf.Abs(num4);
			}
			NKCUtil.SetLabelText(m_lbPrevStat, $"{num4:+#;-#;''}");
			if (readOnlyList != null)
			{
				foreach (NKMEquipRandomStatTemplet item2 in readOnlyList)
				{
					if (item2.m_StatType == prevEquipData.m_Stat[idx].type)
					{
						float num5 = item2.m_MaxStatValue;
						if (flag2)
						{
							num5 = Mathf.Abs(num5);
						}
						NKCUtil.SetLabelText(m_lbNextStat, $"{num5:+#;-#;''}");
						break;
					}
				}
				return;
			}
			float num6 = (flag2 ? Mathf.Abs(nextEquipData.m_Stat[idx].stat_value) : nextEquipData.m_Stat[idx].stat_value);
			NKCUtil.SetLabelText(m_lbNextStat, $"{num6:+#;-#;''}");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbPrevStat, "");
			NKCUtil.SetGameobjectActive(m_objArrow, bValue: false);
			NKCUtil.SetLabelText(m_lbNextStat, "");
		}
	}

	private string GetOptionType(int idx)
	{
		return idx switch
		{
			0 => NKCUtilString.GET_STRING_EQUIP_OPTION_MAIN, 
			1 => NKCUtilString.GET_STRING_EQUIP_OPTION_1, 
			2 => NKCUtilString.GET_STRING_EQUIP_OPTION_2, 
			3 => NKCUtilString.GET_STRING_EQUIP_OPTION_SET, 
			_ => "", 
		};
	}

	private void OnPressInfoTooltip(PointerEventData e)
	{
		NKCUITooltip.Instance.Open(NKCUISlot.eSlotMode.Etc, "", NKCUtilString.GET_STRING_FACTORY_UPGRADE_MAIN_SUB_TOOLTIP, e.position);
	}

	private void OnPressWarningTooltip(PointerEventData e)
	{
		NKCUITooltip.Instance.Open(NKCUISlot.eSlotMode.Etc, "", NKCUtilString.GET_STRING_FACTORY_UPGRADE_MAIN_OPTION_TOOLTIP, e.position);
	}

	private void OnClickDetail()
	{
		if (m_IDX < 3)
		{
			if (NKCPopupEquipOption != null)
			{
				NKCPopupEquipOption.Open(m_PrevEquipData, m_IDX, string.Empty);
			}
		}
		else if (NKCPopupEquipSetOption != null)
		{
			NKCPopupEquipSetOption.Open(m_PrevEquipData, string.Empty);
		}
	}
}
