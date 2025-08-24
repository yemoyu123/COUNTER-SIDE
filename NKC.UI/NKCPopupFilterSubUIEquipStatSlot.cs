using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupFilterSubUIEquipStatSlot : MonoBehaviour
{
	public NKCUIComStateButton m_btn;

	public Image m_imgIconOff;

	public Text m_lbNameOff;

	public Image m_imgIconOn;

	public Text m_lbNameOn;

	public Image m_imgIconLock;

	public Text m_lbNameLock;

	private NKM_STAT_TYPE m_StatType = NKM_STAT_TYPE.NST_RANDOM;

	private int m_SetOptionID;

	private bool m_bIsSetOptionSlot;

	public bool IsSetOptionSlot => m_bIsSetOptionSlot;

	public NKCUIComStateButton GetButton()
	{
		return m_btn;
	}

	public NKM_STAT_TYPE GetStatType()
	{
		return m_StatType;
	}

	public int GetSetOptionID()
	{
		return m_SetOptionID;
	}

	public void SetData(NKM_STAT_TYPE statType, bool bSelected = false)
	{
		m_btn.Select(bSelected, bForce: true, bImmediate: true);
		if (statType != NKM_STAT_TYPE.NST_RANDOM)
		{
			NKCStatInfoTemplet nKCStatInfoTemplet = NKMTempletContainer<NKCStatInfoTemplet>.Find((NKCStatInfoTemplet x) => x.StatType == statType);
			if (string.IsNullOrEmpty(nKCStatInfoTemplet.Filter_Name))
			{
				NKCUtil.SetLabelText(m_lbNameOff, NKCStringTable.GetString(nKCStatInfoTemplet.Stat_Name));
				NKCUtil.SetLabelText(m_lbNameOn, NKCStringTable.GetString(nKCStatInfoTemplet.Stat_Name));
				NKCUtil.SetLabelText(m_lbNameLock, NKCStringTable.GetString(nKCStatInfoTemplet.Stat_Name));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbNameOff, NKCStringTable.GetString(nKCStatInfoTemplet.Filter_Name));
				NKCUtil.SetLabelText(m_lbNameOn, NKCStringTable.GetString(nKCStatInfoTemplet.Filter_Name));
				NKCUtil.SetLabelText(m_lbNameLock, NKCStringTable.GetString(nKCStatInfoTemplet.Filter_Name));
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbNameOff, NKCUtilString.GET_STRING_FILTER_EQUIP_OPTION_SEARCH);
			NKCUtil.SetLabelText(m_lbNameOn, NKCUtilString.GET_STRING_FILTER_EQUIP_OPTION_SEARCH);
			NKCUtil.SetLabelText(m_lbNameLock, NKCUtilString.GET_STRING_FILTER_EQUIP_OPTION_SEARCH);
		}
		NKCUtil.SetGameobjectActive(m_imgIconOff, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgIconOn, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgIconLock, bValue: false);
		m_StatType = statType;
		m_SetOptionID = 0;
		m_bIsSetOptionSlot = false;
	}

	public void SetData(int setOptionID, bool bSelected = false)
	{
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(setOptionID);
		SetData(equipSetOptionTemplet, bSelected);
	}

	public void SetData(NKMItemEquipSetOptionTemplet setOptionTemplet, bool bSelected = false)
	{
		m_btn.Select(bSelected, bForce: true, bImmediate: true);
		if (setOptionTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_imgIconOff, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgIconOn, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgIconLock, bValue: true);
			NKCUtil.SetLabelText(m_lbNameOff, NKCStringTable.GetString(setOptionTemplet.m_EquipSetName));
			NKCUtil.SetLabelText(m_lbNameOn, NKCStringTable.GetString(setOptionTemplet.m_EquipSetName));
			NKCUtil.SetLabelText(m_lbNameLock, NKCStringTable.GetString(setOptionTemplet.m_EquipSetName));
			NKCUtil.SetImageSprite(m_imgIconOff, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_EQUIP_SET_ICON", setOptionTemplet.m_EquipSetIcon));
			NKCUtil.SetImageSprite(m_imgIconOn, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_EQUIP_SET_ICON", setOptionTemplet.m_EquipSetIcon));
			NKCUtil.SetImageSprite(m_imgIconLock, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_EQUIP_SET_ICON", setOptionTemplet.m_EquipSetIcon));
			m_SetOptionID = setOptionTemplet.m_EquipSetID;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_imgIconOff, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgIconOn, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgIconLock, bValue: false);
			NKCUtil.SetLabelText(m_lbNameOff, NKCUtilString.GET_STRING_SORT_SETOPTION);
			NKCUtil.SetLabelText(m_lbNameOn, NKCUtilString.GET_STRING_SORT_SETOPTION);
			NKCUtil.SetLabelText(m_lbNameLock, NKCUtilString.GET_STRING_SORT_SETOPTION);
			m_SetOptionID = 0;
		}
		m_StatType = NKM_STAT_TYPE.NST_RANDOM;
		m_bIsSetOptionSlot = true;
	}
}
