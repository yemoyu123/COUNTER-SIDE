using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.UI;

public class NKCUISelectionEquipDetailSlot : MonoBehaviour
{
	public delegate void OnTglSlot(NKCUISelectionEquipDetail.OPTION_LIST_TYPE m_OptionListType, NKM_STAT_TYPE statType, int setOptionId, int potentialOptionKey);

	public NKCUIComToggle m_tglSlot;

	private NKCUISelectionEquipDetail.OPTION_LIST_TYPE m_OptionListType;

	private NKM_STAT_TYPE m_StatType = NKM_STAT_TYPE.NST_RANDOM;

	private int m_SetOptionID;

	private int m_potentialOptionKey;

	private OnTglSlot m_dOnTglSlot;

	public void InitUI(OnTglSlot dOnTglSlot, NKCUIComToggleGroup tglGroup)
	{
		m_tglSlot.m_ToggleGroup = tglGroup;
		m_tglSlot.OnValueChanged.RemoveAllListeners();
		m_tglSlot.OnValueChanged.AddListener(OnTgl);
		m_dOnTglSlot = dOnTglSlot;
	}

	public void SetData(NKCUISelectionEquipDetail.OPTION_LIST_TYPE optionListType, NKM_STAT_TYPE statType, bool bSelected = false)
	{
		m_tglSlot.SetImage(null);
		if (statType != NKM_STAT_TYPE.NST_RANDOM)
		{
			NKCStatInfoTemplet nKCStatInfoTemplet = NKMTempletContainer<NKCStatInfoTemplet>.Find((NKCStatInfoTemplet x) => x.StatType == statType);
			if (string.IsNullOrEmpty(nKCStatInfoTemplet.Filter_Name))
			{
				m_tglSlot.SetTitleText(NKCStringTable.GetString(nKCStatInfoTemplet.Stat_Name));
			}
			else
			{
				m_tglSlot.SetTitleText(NKCStringTable.GetString(nKCStatInfoTemplet.Filter_Name));
			}
		}
		else
		{
			m_tglSlot.SetTitleText(NKCUtilString.GET_STRING_FILTER_EQUIP_OPTION_SEARCH);
		}
		m_OptionListType = optionListType;
		m_StatType = statType;
		m_SetOptionID = 0;
		m_potentialOptionKey = 0;
		m_tglSlot.Select(bSelected, bForce: true, bImmediate: true);
	}

	public void SetData(NKMPotentialOptionTemplet potentialOptionTemplet, int potentialIndex, bool bSelected = false)
	{
		m_tglSlot.SetImage(null);
		if (potentialOptionTemplet.StatType != NKM_STAT_TYPE.NST_RANDOM)
		{
			NKCStatInfoTemplet nKCStatInfoTemplet = NKMTempletContainer<NKCStatInfoTemplet>.Find((NKCStatInfoTemplet x) => x.StatType == potentialOptionTemplet.StatType);
			if (string.IsNullOrEmpty(nKCStatInfoTemplet.Filter_Name))
			{
				m_tglSlot.SetTitleText(NKCStringTable.GetString(nKCStatInfoTemplet.Stat_Name));
			}
			else
			{
				m_tglSlot.SetTitleText(NKCStringTable.GetString(nKCStatInfoTemplet.Filter_Name));
			}
		}
		else
		{
			m_tglSlot.SetTitleText(NKCUtilString.GET_STRING_FILTER_EQUIP_OPTION_SEARCH);
		}
		m_OptionListType = NKCUISelectionEquipDetail.OPTION_LIST_TYPE.POTENTIAL;
		m_StatType = potentialOptionTemplet.StatType;
		m_SetOptionID = potentialIndex;
		m_potentialOptionKey = potentialOptionTemplet.optionKey;
		m_tglSlot.Select(bSelected, bForce: true, bImmediate: true);
	}

	public void SetData(int setOptionId, bool bSelected = false)
	{
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(setOptionId);
		SetData(equipSetOptionTemplet, bSelected);
	}

	public void SetData(NKMItemEquipSetOptionTemplet setOptionTemplet, bool bSelected = false)
	{
		if (setOptionTemplet != null)
		{
			m_tglSlot.SetImage(NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_EQUIP_SET_ICON", setOptionTemplet.m_EquipSetIcon));
			m_tglSlot.SetTitleText(NKCStringTable.GetString(setOptionTemplet.m_EquipSetName));
			m_SetOptionID = setOptionTemplet.m_EquipSetID;
		}
		else
		{
			m_tglSlot.SetImage(null);
			m_tglSlot.SetTitleText(NKCUtilString.GET_STRING_SORT_SETOPTION);
			m_SetOptionID = 0;
		}
		m_OptionListType = NKCUISelectionEquipDetail.OPTION_LIST_TYPE.SETOPTION;
		m_StatType = NKM_STAT_TYPE.NST_RANDOM;
		m_potentialOptionKey = 0;
		m_tglSlot.Select(bSelected, bForce: true, bImmediate: true);
	}

	private void OnTgl(bool bValue)
	{
		if (bValue)
		{
			m_dOnTglSlot?.Invoke(m_OptionListType, m_StatType, m_SetOptionID, m_potentialOptionKey);
		}
	}

	public void SetLock(bool bLock)
	{
		m_tglSlot.SetLock(bLock);
	}
}
