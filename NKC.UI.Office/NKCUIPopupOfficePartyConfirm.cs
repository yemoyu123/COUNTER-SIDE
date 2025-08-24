using System.Collections.Generic;
using ClientPacket.Office;
using NKM;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIPopupOfficePartyConfirm : NKCUIBase
{
	public delegate void OnComfirm(int roomID);

	private const string ASSET_BUNDLE_NAME = "ab_ui_office";

	private const string UI_ASSET_NAME = "AB_UI_POPUP_OFFICE_PARTY_READY";

	private static NKCUIPopupOfficePartyConfirm m_Instance;

	private OnComfirm dOnComfirm;

	private int m_roomID;

	public Text m_lbDesc;

	public GameObject m_objCount;

	public Text m_lbCount;

	public NKCUIComResourceButton m_csbtnConfirm;

	public NKCUIComStateButton m_csbtnCancel;

	public NKCUIComStateButton m_csbtnClose;

	public List<NKCUISlot> m_lstSlotReward;

	public static NKCUIPopupOfficePartyConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupOfficePartyConfirm>("ab_ui_office", "AB_UI_POPUP_OFFICE_PARTY_READY", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupOfficePartyConfirm>();
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

	public override string MenuName => string.Empty;

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
		base.gameObject.SetActive(value: false);
	}

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnCancel, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnConfirm, OnBtnComfirm);
		NKCUtil.SetHotkey(m_csbtnConfirm, HotkeyEventType.Confirm);
		foreach (NKCUISlot item in m_lstSlotReward)
		{
			item.Init();
		}
	}

	public void Open(int roomID, OnComfirm onComfirm)
	{
		NKMOfficeRoom officeRoom = NKCScenManager.CurrentUserData().OfficeData.GetOfficeRoom(roomID);
		if (officeRoom == null)
		{
			Debug.LogError("OfficeRoomData null!");
			return;
		}
		m_roomID = roomID;
		dOnComfirm = onComfirm;
		SetRequiredResource();
		SetReward(officeRoom);
		UIOpened();
	}

	private void SetReward(NKMOfficeRoom room)
	{
		NKMOfficeGradeTemplet nKMOfficeGradeTemplet = NKMOfficeGradeTemplet.Find(room.grade);
		if (nKMOfficeGradeTemplet == null)
		{
			Debug.LogError("GradeTemplet null!");
			NKCUtil.SetLabelText(m_lbDesc, "");
			for (int i = 0; i < m_lstSlotReward.Count; i++)
			{
				NKCUtil.SetGameobjectActive(m_lstSlotReward[i], bValue: false);
			}
		}
		else
		{
			string text = ((!string.IsNullOrEmpty(room.name)) ? room.name : NKCUIOfficeMapFront.GetDefaultRoomName(room.id));
			NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString("SI_PF_OFFICE_PARTY_0", text, nKMOfficeGradeTemplet.PartyRewardLoyalty / 100));
			NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeRewardTypeData(nKMOfficeGradeTemplet.PartyRewardType, nKMOfficeGradeTemplet.PartyRewardId, nKMOfficeGradeTemplet.PartyRewardValueMin);
			List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
			list.Add(item);
			NKCUISlot.SetSlotListData(m_lstSlotReward, list, false, false, true, null, default(NKCUISlot.SlotClickType));
		}
	}

	private void SetRequiredResource()
	{
		if (NKMCommonConst.Office?.PartyUseItem != null)
		{
			m_csbtnConfirm.OnShow(bShow: true);
			m_csbtnConfirm.SetData(NKMCommonConst.Office.PartyUseItem.m_ItemMiscID, 1);
			NKCUtil.SetGameobjectActive(m_objCount, bValue: true);
			NKCUtil.SetLabelText(m_lbCount, NKCUtilString.GET_STRING_TOOLTIP_QUANTITY_ONE_PARAM, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(NKMCommonConst.Office.PartyUseItem.m_ItemMiscID));
		}
		else
		{
			Debug.LogError("Party required item not set!");
			m_csbtnConfirm.OnShow(bShow: false);
		}
	}

	private void OnBtnComfirm()
	{
		Close();
		dOnComfirm?.Invoke(m_roomID);
	}
}
