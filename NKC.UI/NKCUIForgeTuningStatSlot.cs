using System;
using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeTuningStatSlot : MonoBehaviour
{
	public Text m_NKM_UI_FACTORY_TUNING_STAT_TEXT;

	public GameObject m_STAT;

	public Text m_STAT_TEXT;

	public GameObject m_NKM_UI_FACTORY_TUNING_PRECISION_NONE;

	public GameObject m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_NONE;

	public void Clear(int type)
	{
		if (type == 1)
		{
			m_NKM_UI_FACTORY_TUNING_STAT_TEXT.text = NKCUtilString.GET_STRING_FORGE_TUNING_STAT_BASE;
		}
		if (type == 2)
		{
			m_NKM_UI_FACTORY_TUNING_STAT_TEXT.text = NKCUtilString.GET_STRING_FORGE_TUNING_STAT_RESULT;
		}
		m_STAT_TEXT.text = "0";
	}

	public void SetData(bool bBefore, NKMEquipItemData equipItemData, int idx = 0)
	{
		if (equipItemData == null)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipItemData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return;
		}
		int statGroupID = ((idx == 0) ? equipTemplet.m_StatGroupID : equipTemplet.m_StatGroupID_2);
		IReadOnlyList<NKMEquipRandomStatTemplet> equipRandomStatGroupList = NKMEquipTuningManager.GetEquipRandomStatGroupList(statGroupID);
		EQUIP_ITEM_STAT eQUIP_ITEM_STAT = null;
		bool bPercentStat = false;
		int num = 0;
		foreach (EQUIP_ITEM_STAT item in equipItemData.m_Stat)
		{
			if (num == idx + 1)
			{
				if (equipRandomStatGroupList != null)
				{
					foreach (NKMEquipRandomStatTemplet item2 in equipRandomStatGroupList)
					{
						if (item2.m_StatType == item.type)
						{
							bPercentStat = NKCUIForgeTuning.IsPercentStat(item2);
						}
					}
				}
				eQUIP_ITEM_STAT = item;
				break;
			}
			num++;
		}
		if (eQUIP_ITEM_STAT == null)
		{
			return;
		}
		if (bBefore)
		{
			NKCUtil.SetLabelText(m_STAT_TEXT, NKCUIForgeTuning.GetTuningOptionStatString(eQUIP_ITEM_STAT, equipItemData, bPercentStat));
			NKCUtil.SetLabelText(m_NKM_UI_FACTORY_TUNING_STAT_TEXT, NKCUtilString.GET_STRING_FORGE_TUNING_STAT_CURRENT);
			if (equipItemData.m_Stat.Count <= 1)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_NONE, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_PRECISION_NONE, bValue: false);
				NKCUtil.SetGameobjectActive(m_STAT, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_NONE, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_PRECISION_NONE, bValue: false);
				NKCUtil.SetGameobjectActive(m_STAT, bValue: true);
			}
			return;
		}
		NKCUtil.SetLabelText(m_NKM_UI_FACTORY_TUNING_STAT_TEXT, NKCUtilString.GET_STRING_FORGE_TUNING_STAT_CHANGE);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_PRECISION_NONE, bValue: false);
		bool flag = false;
		NKMEquipTuningCandidate tuiningData = NKCScenManager.GetScenManager().GetMyUserData().GetTuiningData();
		if (tuiningData != null && tuiningData.equipUid == equipItemData.m_ItemUid)
		{
			NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(tuiningData.equipUid);
			NKM_STAT_TYPE nKM_STAT_TYPE = ((idx == 0) ? tuiningData.option1 : tuiningData.option2);
			if (itemEquip != null && nKM_STAT_TYPE != NKM_STAT_TYPE.NST_RANDOM && NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID) != null)
			{
				flag = true;
				int precision = ((idx == 1) ? equipItemData.m_Precision2 : equipItemData.m_Precision);
				float value = 0f;
				NKMEquipRandomStatTemplet equipRandomStat = NKMEquipTuningManager.GetEquipRandomStat(statGroupID, nKM_STAT_TYPE);
				if (equipRandomStat != null)
				{
					value = equipRandomStat.CalcResultStat(precision);
				}
				NKCUtil.SetGameobjectActive(m_STAT, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_NONE, bValue: false);
				if (NKCUIForgeTuning.IsPercentStat(equipRandomStat))
				{
					decimal num2 = new decimal(value);
					num2 = Math.Round(num2 * 1000m) / 1000m;
					NKCUtil.SetLabelText(m_STAT_TEXT, NKCUtilString.GetStatShortString("<color=#ffdb00>{0} {1:P1}</color>", nKM_STAT_TYPE, num2));
				}
				else
				{
					NKCUtil.SetLabelText(m_STAT_TEXT, NKCUtilString.GetStatShortString("<color=#ffdb00>{0} {1:+#;-#;''}</color>", nKM_STAT_TYPE, value));
				}
			}
		}
		if (!flag)
		{
			NKCUtil.SetGameobjectActive(m_STAT, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_NONE, bValue: true);
		}
	}
}
