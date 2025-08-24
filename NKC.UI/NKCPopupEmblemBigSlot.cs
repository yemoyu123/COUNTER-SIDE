using ClientPacket.Common;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEmblemBigSlot : MonoBehaviour
{
	public NKCUISlot m_NKCUISlot;

	public NKCUISlotProfile m_NKCUISlotProfile;

	public NKCUISlotTitle m_NKCUISlotTitle;

	public Text m_lbEmblemName;

	public Text m_lbEmblemDesc;

	public void SetEmblemData(int miscItemID)
	{
		NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKCUISlotProfile, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUISlotTitle, bValue: false);
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(miscItemID);
		if (itemMiscTempletByID != null)
		{
			m_NKCUISlot.SetMiscItemData(miscItemID, 1L, bShowName: false, bShowCount: false, bEnableLayoutElement: true, null);
			NKCUtil.SetLabelText(m_lbEmblemName, itemMiscTempletByID.GetItemName());
			NKCUtil.SetLabelText(m_lbEmblemDesc, itemMiscTempletByID.GetItemDesc());
		}
	}

	public void SetProfileData(NKMUserProfileData profileData)
	{
		NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUISlotProfile, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKCUISlotTitle, bValue: true);
		if (profileData != null)
		{
			m_NKCUISlotProfile?.SetProfiledata(profileData, null);
			m_NKCUISlotTitle?.SetData(profileData.commonProfile.titleId, showEmpty: true, showLock: true);
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(profileData.commonProfile.mainUnitId);
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelText(m_lbEmblemName, unitTempletBase.GetUnitName());
			NKCUtil.SetLabelText(m_lbEmblemDesc, unitTempletBase.GetUnitDesc());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEmblemName, "");
			NKCUtil.SetLabelText(m_lbEmblemDesc, "");
		}
	}

	public void SetProfileData(int unitId, int skinId, int frameId, bool bTacticMaxLvFX = false)
	{
		NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUISlotProfile, bValue: true);
		m_NKCUISlotProfile?.SetProfiledata(unitId, skinId, frameId, null, bTacticMaxLvFX);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitId);
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelText(m_lbEmblemName, unitTempletBase.GetUnitName());
			NKCUtil.SetLabelText(m_lbEmblemDesc, unitTempletBase.GetUnitDesc());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEmblemName, "");
			NKCUtil.SetLabelText(m_lbEmblemDesc, "");
		}
	}

	public void SetFrameData(int unitId, int skinId, int frameId, bool bIsMaxTacticLevel = false)
	{
		NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUISlotProfile, bValue: true);
		m_NKCUISlotProfile?.SetProfiledata(unitId, skinId, frameId, null, bIsMaxTacticLevel);
		NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(frameId);
		if (nKMItemMiscTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbEmblemName, nKMItemMiscTemplet.GetItemName());
			NKCUtil.SetLabelText(m_lbEmblemDesc, nKMItemMiscTemplet.GetItemDesc());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEmblemName, "");
			NKCUtil.SetLabelText(m_lbEmblemDesc, "");
		}
	}

	public void SetTitleData(int titleId)
	{
		m_NKCUISlotTitle?.SetData(titleId, showEmpty: true, showLock: true);
		NKMTitleTemplet nKMTitleTemplet = NKMTitleTemplet.Find(titleId);
		if (nKMTitleTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbEmblemName, nKMTitleTemplet.GetTitleName());
			NKCUtil.SetLabelText(m_lbEmblemDesc, nKMTitleTemplet.GetTitleDesc());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEmblemName, "");
			NKCUtil.SetLabelText(m_lbEmblemDesc, "");
		}
	}

	public void SetEmblemEmpty(string desc = "")
	{
		NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKCUISlotProfile, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUISlotTitle, bValue: false);
		m_NKCUISlot.SetEmpty();
		NKCUtil.SetLabelText(m_lbEmblemName, "");
		NKCUtil.SetLabelText(m_lbEmblemDesc, desc);
	}
}
