using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupRearmamentExtractConfirm : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_rearm";

	private const string UI_ASSET_NAME = "AB_UI_POPUP_REARM_RECORD_CONFIRM";

	private static NKCUIPopupRearmamentExtractConfirm m_Instance;

	public Text m_lbResultText;

	public RectTransform m_rtTacticsInfo;

	private List<NKCUISlot> m_lstExtractItem = new List<NKCUISlot>();

	public NKCUISlot m_SynergySlot;

	public Text m_lbSynergyBonus;

	public NKCUIComButton m_csbtnOK;

	public NKCUIComButton m_csbtnCancel;

	public List<GameObject> m_lstSynergyON;

	public List<GameObject> m_lstSynergyOFF;

	public int m_iMiscRewardItemCode = 913;

	public int m_iDisableMiscRewardItemCode = 913;

	private List<long> m_lstSelectedUnitsUID;

	public static NKCUIPopupRearmamentExtractConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupRearmamentExtractConfirm>("ab_ui_rearm", "AB_UI_POPUP_REARM_RECORD_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupRearmamentExtractConfirm>();
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

	public override string MenuName => NKCUtilString.GET_STRING_REARM_CONFIRM_POPUP_TITLE;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.BackButtonOnly;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnCancel, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnOK, OnClickOK);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
	}

	private void Clear()
	{
		for (int i = 0; i < m_lstExtractItem.Count; i++)
		{
			Object.Destroy(m_lstExtractItem[i]);
		}
		m_lstExtractItem.Clear();
	}

	public void Open(List<NKCUISlot.SlotData> lstExtractItemData, List<long> lstSelectedUnitsUID, bool bActiveSynergyBouns = false)
	{
		m_lstSelectedUnitsUID = lstSelectedUnitsUID;
		NKCUtil.SetLabelText(m_lbResultText, string.Format(NKCUtilString.GET_STRING_REARM_EXTRACT_CONFIRM_POPUP_DESC, lstSelectedUnitsUID.Count));
		if (bActiveSynergyBouns)
		{
			int synergyIncreasePercentage = NKCRearmamentUtil.GetSynergyIncreasePercentage(m_lstSelectedUnitsUID);
			NKCUtil.SetLabelText(m_lbSynergyBonus, string.Format(NKCUtilString.GET_STRING_REARM_EXTRACT_CONFIRM_POPUP_SYNERGY_BONUS, synergyIncreasePercentage));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbSynergyBonus, NKCUtilString.GET_STRING_REARM_EXTRACT_NOT_ACTIVE_SYNERGY_BOUNS);
		}
		foreach (GameObject item in m_lstSynergyON)
		{
			NKCUtil.SetGameobjectActive(item, bActiveSynergyBouns);
		}
		foreach (GameObject item2 in m_lstSynergyOFF)
		{
			NKCUtil.SetGameobjectActive(item2, !bActiveSynergyBouns);
		}
		foreach (NKCUISlot.SlotData lstExtractItemDatum in lstExtractItemData)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(lstExtractItemDatum.ID);
			if (itemMiscTempletByID == null)
			{
				continue;
			}
			if (itemMiscTempletByID.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_PIECE)
			{
				Debug.LogError("NKCUIPopupRearmamentExtractConfirm::Open() - Can not support imt_piece type");
			}
			else
			{
				if (!(m_rtTacticsInfo != null))
				{
					continue;
				}
				NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_rtTacticsInfo);
				if (null != newInstance)
				{
					NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_MISC, lstExtractItemDatum.ID, (int)lstExtractItemDatum.Count);
					if (slotData != null)
					{
						newInstance.SetData(slotData);
						NKCUtil.SetGameobjectActive(newInstance.gameObject, bValue: true);
					}
					m_lstExtractItem.Add(newInstance);
				}
			}
		}
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(bActiveSynergyBouns ? m_iMiscRewardItemCode : m_iDisableMiscRewardItemCode, 1L);
		m_SynergySlot.SetData(data);
		UIOpened();
	}

	private void OnClickOK()
	{
		NKCPacketSender.Send_NKMPacket_EXTRACT_UNIT_REQ(m_lstSelectedUnitsUID);
	}
}
