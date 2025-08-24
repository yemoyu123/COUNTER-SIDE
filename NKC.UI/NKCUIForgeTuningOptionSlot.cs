using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeTuningOptionSlot : MonoBehaviour
{
	[Header("옵션 on/off")]
	public GameObject m_objNKM_UI_FACTORY_TUNING_OPTION_SLOT_OFF;

	public GameObject m_objNKM_UI_FACTORY_TUNING_OPTION_SLOT_ON;

	[Header("정밀화 on/off")]
	public GameObject m_objAdjustment_rate_off;

	public GameObject m_objAdjustment_rate_on;

	[Header("옵션 변경 on/off")]
	public GameObject m_objOption_Change_off;

	public GameObject m_objOption_Change_on;

	[Header("슬라이더 설정")]
	public Slider m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_off;

	public Slider m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_on;

	public Slider m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_NEW_off;

	public Slider m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_NEW_on;

	[Header("최대치 슬라이더")]
	public GameObject m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_MAX_off;

	public GameObject m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_MAX_on;

	[Header("최대치 MAX 표시")]
	public GameObject m_NKM_UI_FACTORY_ADJUSTMENT_RATE_MAX_off;

	public GameObject m_NKM_UI_FACTORY_ADJUSTMENT_RATE_MAX_on;

	[Header("옵션 변경")]
	public NKCUIForgeTuningStatSlot m_statSlot_Option_Change_Before_off;

	public NKCUIForgeTuningStatSlot m_statSlot_Option_Change_Before_on;

	public NKCUIForgeTuningStatSlot m_statSlot_Option_Change_After_off;

	public NKCUIForgeTuningStatSlot m_statSlot_Option_Change_After_on;

	public Text m_txtOption_Number_off;

	public Text m_txtOption_Number_on;

	[Header("옵션")]
	public Text m_txtOPTION_TEXT_off;

	public Text m_txtOPTION_TEXT_on;

	public Text m_txtOPTION_PRECISION_NUMBER_TEXT_off;

	public Text m_txtOPTION_PRECISION_NUMBER_TEXT_on;

	public GameObject m_objOption_None_text_off;

	public GameObject m_objOption_None_text_on;

	public Text m_OPTION_None_Text_off;

	public Text m_OPTION_None_Text_on;

	public void SetData(NKCUIForgeTuning.NKC_TUNING_TAB newState, int idx, NKMEquipItemData equipData)
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipData.m_ItemEquipID);
		if (equipTemplet == null || equipData.m_Stat.Count <= idx + 1)
		{
			return;
		}
		bool bValue = true;
		if (newState == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_PRECISION)
		{
			bValue = false;
		}
		bool flag = false;
		switch (idx)
		{
		case 0:
			flag = NKMEquipTuningManager.IsChangeableStatGroup(equipTemplet.m_StatGroupID);
			break;
		case 1:
			flag = NKMEquipTuningManager.IsChangeableStatGroup(equipTemplet.m_StatGroupID_2);
			break;
		}
		string msg;
		if (equipTemplet.IsPrivateEquip() && !flag)
		{
			msg = NKCUtilString.GET_STRING_TUNING_OPTION_SLOT_EXCLUSIVE;
			if (newState == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_OPTION_CHANGE)
			{
				NKCUtil.SetLabelText(m_OPTION_None_Text_off, NKCUtilString.GET_STRING_TUNING_OPTIN_CAN_NOT_CHANGE);
				NKCUtil.SetGameobjectActive(m_objOption_None_text_off, bValue: true);
			}
		}
		else
		{
			msg = string.Format(NKCUtilString.GET_STRING_TUNING_OPTION_SLOT_OPTION + " " + (idx + 1));
		}
		NKCUtil.SetLabelText(m_txtOption_Number_off, msg);
		NKCUtil.SetLabelText(m_txtOption_Number_on, msg);
		switch (newState)
		{
		case NKCUIForgeTuning.NKC_TUNING_TAB.NTT_PRECISION:
		{
			bool bValue2 = false;
			int num = 0;
			switch (idx)
			{
			case 0:
				num = equipData.m_Precision;
				break;
			case 1:
				num = equipData.m_Precision2;
				break;
			}
			if (num >= 100)
			{
				bValue2 = true;
			}
			m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_off.value = (float)num / 100f;
			m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_on.value = (float)num / 100f;
			m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_NEW_off.value = 0f;
			m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_NEW_on.value = 0f;
			bool flag2 = false;
			switch (idx)
			{
			case 0:
				flag2 = equipTemplet.m_StatGroupID != 0;
				break;
			case 1:
				flag2 = equipTemplet.m_StatGroupID_2 != 0;
				break;
			}
			if (!flag2)
			{
				NKCUtil.SetLabelText(m_OPTION_None_Text_off, NKCUtilString.GET_STRING_TUNING_OPTIN_NONE);
				NKCUtil.SetLabelText(m_OPTION_None_Text_on, NKCUtilString.GET_STRING_TUNING_OPTIN_NONE);
				NKCUtil.SetGameobjectActive(m_objOption_None_text_off, bValue: true);
				NKCUtil.SetGameobjectActive(m_objOption_None_text_on, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objOption_None_text_off, bValue: false);
				NKCUtil.SetGameobjectActive(m_objOption_None_text_on, bValue: false);
			}
			string tuningOptionStatString = NKCUIForgeTuning.GetTuningOptionStatString(equipData, idx + 1);
			NKCUtil.SetLabelText(m_txtOPTION_TEXT_off, tuningOptionStatString);
			NKCUtil.SetLabelText(m_txtOPTION_TEXT_on, tuningOptionStatString);
			NKCUtil.SetLabelText(m_txtOPTION_PRECISION_NUMBER_TEXT_off, $"{num}%");
			NKCUtil.SetLabelText(m_txtOPTION_PRECISION_NUMBER_TEXT_on, $"{num}%");
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_MAX_off, bValue2);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_MAX_on, bValue2);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ADJUSTMENT_RATE_MAX_off, bValue2);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_ADJUSTMENT_RATE_MAX_on, bValue2);
			break;
		}
		case NKCUIForgeTuning.NKC_TUNING_TAB.NTT_OPTION_CHANGE:
			if (!flag)
			{
				NKCUtil.SetLabelText(m_OPTION_None_Text_off, NKCUtilString.GET_STRING_TUNING_OPTIN_CAN_NOT_CHANGE);
				NKCUtil.SetLabelText(m_OPTION_None_Text_on, NKCUtilString.GET_STRING_TUNING_OPTIN_CAN_NOT_CHANGE);
				NKCUtil.SetGameobjectActive(m_objOption_None_text_off, bValue: true);
				NKCUtil.SetGameobjectActive(m_objOption_None_text_on, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objOption_None_text_off, bValue: false);
				NKCUtil.SetGameobjectActive(m_objOption_None_text_on, bValue: false);
			}
			m_statSlot_Option_Change_Before_off?.SetData(bBefore: true, equipData, idx);
			m_statSlot_Option_Change_Before_on?.SetData(bBefore: true, equipData, idx);
			m_statSlot_Option_Change_After_off?.SetData(bBefore: false, equipData, idx);
			m_statSlot_Option_Change_After_on?.SetData(bBefore: false, equipData, idx);
			break;
		}
		NKCUtil.SetGameobjectActive(m_txtOPTION_TEXT_off.gameObject, newState == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_PRECISION);
		NKCUtil.SetGameobjectActive(m_txtOPTION_TEXT_on.gameObject, newState == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_PRECISION);
		NKCUtil.SetGameobjectActive(m_objAdjustment_rate_off, newState == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_PRECISION);
		NKCUtil.SetGameobjectActive(m_objAdjustment_rate_on, newState == NKCUIForgeTuning.NKC_TUNING_TAB.NTT_PRECISION);
		NKCUtil.SetGameobjectActive(m_objOption_Change_off, bValue);
		NKCUtil.SetGameobjectActive(m_objOption_Change_on, bValue);
	}

	public void SetPrecisionRate(NKMEquipItemData equipData, int idx)
	{
		if (equipData == null)
		{
			return;
		}
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(equipData.m_ItemUid);
		if (itemEquip == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		if (idx == 0)
		{
			num = itemEquip.m_Precision;
			num2 = equipData.m_Precision;
		}
		else
		{
			num = itemEquip.m_Precision2;
			num2 = equipData.m_Precision2;
		}
		if (num < 100)
		{
			if (num >= num2)
			{
				m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_off.value = (float)num / 100f;
				m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_on.value = (float)num / 100f;
				m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_NEW_off.value = (float)num2 / 100f;
				m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_NEW_on.value = (float)num2 / 100f;
			}
			else
			{
				m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_off.value = (float)num / 100f;
				m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_on.value = (float)num / 100f;
				m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_NEW_off.value = 0f;
				m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_NEW_on.value = 0f;
			}
		}
	}

	public string GetStatText(bool before)
	{
		if (before)
		{
			return m_statSlot_Option_Change_Before_on.m_STAT_TEXT.text;
		}
		return m_statSlot_Option_Change_After_on.m_STAT_TEXT.text;
	}

	public void ClearPrecisionRate()
	{
		m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_off.value = 0f;
		m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_on.value = 0f;
		m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_NEW_off.value = 0f;
		m_NKM_UI_FACTORY_ADJUSTMENT_RATE_SLIDER_NEW_on.value = 0f;
	}

	public void ClearUI(bool bForce = false)
	{
		if (bForce)
		{
			NKCUtil.SetLabelText(m_txtOption_Number_off, "-");
			NKCUtil.SetLabelText(m_txtOption_Number_on, "-");
			NKCUtil.SetLabelText(m_OPTION_None_Text_off, NKCUtilString.GET_STRING_TUNING_OPTIN_NONE);
			NKCUtil.SetLabelText(m_OPTION_None_Text_on, NKCUtilString.GET_STRING_TUNING_OPTIN_NONE);
		}
		NKCUtil.SetGameobjectActive(m_txtOPTION_TEXT_off.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_txtOPTION_TEXT_on.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_objAdjustment_rate_off, bValue: false);
		NKCUtil.SetGameobjectActive(m_objAdjustment_rate_on, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOption_Change_off, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOption_Change_on, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOption_None_text_off, bValue: true);
		NKCUtil.SetGameobjectActive(m_objOption_None_text_on, bValue: true);
		NKCUtil.SetGameobjectActive(m_objNKM_UI_FACTORY_TUNING_OPTION_SLOT_OFF, bValue: true);
		NKCUtil.SetGameobjectActive(m_objNKM_UI_FACTORY_TUNING_OPTION_SLOT_ON, bValue: false);
	}
}
