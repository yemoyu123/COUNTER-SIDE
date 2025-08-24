using ClientPacket.Common;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

[RequireComponent(typeof(NKCUISlot))]
public class NKCUISlotProfile : MonoBehaviour
{
	public delegate void OnClick(NKCUISlotProfile slot, int frameID);

	public Image m_imgBorder;

	private NKCUISlot m_slot;

	private OnClick dOnClick;

	private NKCUISlot Slot
	{
		get
		{
			if (m_slot == null)
			{
				m_slot = GetComponent<NKCUISlot>();
				m_slot.SetOnClick(OnClickSlot);
			}
			return m_slot;
		}
	}

	public int FrameID { get; private set; }

	public void Init()
	{
		Slot.Init();
	}

	public void SetProfiledata(NKMCommonProfile profile, OnClick onClick)
	{
		Slot.SetUnitData(profile.mainUnitId, 1, profile.mainUnitSkinId, bShowName: false, bShowLevel: false, bEnableLayoutElement: false, OnClickSlot);
		Slot.SetMaxLevelTacticFX(profile.mainUnitTacticLevel == 6);
		NKCUtil.SetGameobjectActive(Slot.m_imgUpperRightIcon, bValue: false);
		dOnClick = onClick;
		SetProfileFrame(profile.frameId);
	}

	public void SetProfiledata(int unitID, int skinID, int frameID, OnClick onClick, bool bTacticMaxLvFX = false)
	{
		Slot.SetUnitData(unitID, 1, skinID, bShowName: false, bShowLevel: false, bEnableLayoutElement: false, OnClickSlot);
		Slot.SetMaxLevelTacticFX(bTacticMaxLvFX);
		NKCUtil.SetGameobjectActive(Slot.m_imgUpperRightIcon, bValue: false);
		dOnClick = onClick;
		SetProfileFrame(frameID);
	}

	public void SetProfiledata(NKMUserProfileData cNKMUserProfileData, OnClick onClick, bool bHasMaxTacticLevelUnit = false)
	{
		Slot.SetUnitData(cNKMUserProfileData.commonProfile.mainUnitId, 1, cNKMUserProfileData.commonProfile.mainUnitSkinId, bShowName: false, bShowLevel: false, bEnableLayoutElement: false, OnClickSlot);
		Slot.SetMaxLevelTacticFX(bHasMaxTacticLevelUnit);
		NKCUtil.SetGameobjectActive(Slot.m_imgUpperRightIcon, bValue: false);
		dOnClick = onClick;
		SetProfileFrame(cNKMUserProfileData.commonProfile.frameId);
	}

	public void SetSelected(bool value)
	{
		Slot.SetSelected(value);
	}

	private void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		dOnClick?.Invoke(this, FrameID);
	}

	private void SetProfileFrame(int frameID)
	{
		if (frameID != 0)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(frameID);
			if (itemMiscTempletByID.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME)
			{
				Debug.LogError("Not a Frame Item!! ItemID : " + frameID);
				NKCUtil.SetGameobjectActive(m_imgBorder, bValue: false);
				return;
			}
			NKCUtil.SetImageSprite(m_imgBorder, NKCResourceUtility.GetOrLoadMiscItemIcon(itemMiscTempletByID));
			NKCUtil.SetGameobjectActive(m_imgBorder, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_imgBorder, bValue: false);
		}
		FrameID = frameID;
	}
}
