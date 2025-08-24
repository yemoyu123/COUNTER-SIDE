using System.Collections.Generic;
using NKC.UI.Shop;
using NKM;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupSelectionConfirm : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_selection";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_SELECTION_CONFIRM";

	private static NKCPopupSelectionConfirm m_Instance;

	public Text m_lbTitle;

	public NKCPopupSelectionConfirmUnit m_unit;

	public NKCPopupSelectionConfirmShip m_ship;

	public NKCPopupSelectionConfirmEquip m_equip;

	public NKCPopupMiscUseCountContents m_misc;

	public NKCPopupSelectionConfirmOperator m_operator;

	public NKCPopupSelectionConfirmSkin m_skin;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnCancel;

	private NKMItemMiscTemplet m_NKMItemMiscTemplet;

	private int m_targetID;

	private long m_targetCount;

	private int m_setOptionID;

	private int m_subSkillID;

	private List<NKM_STAT_TYPE> m_lstStatType = new List<NKM_STAT_TYPE>();

	private int m_PotentialOptionKey;

	private int m_PotentialOptionKey2;

	public static NKCPopupSelectionConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupSelectionConfirm>("ab_ui_nkm_ui_popup_selection", "NKM_UI_POPUP_SELECTION_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupSelectionConfirm>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	private int UseCount
	{
		get
		{
			if (m_NKMItemMiscTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CHOICE_MISC)
			{
				return (int)m_misc.m_useCount;
			}
			return 1;
		}
	}

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
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void InitUI()
	{
		m_btnOk.PointerClick.RemoveAllListeners();
		m_btnOk.PointerClick.AddListener(OnClickOk);
		NKCUtil.SetHotkey(m_btnOk, HotkeyEventType.Confirm);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(base.Close);
		m_misc.Init();
		m_skin.Init();
	}

	public void Open(NKMItemMiscTemplet itemMiscTemplet, int id, long count = 1L, int setItemID = 0, int subSkillID = 0, List<NKM_STAT_TYPE> lstStatType = null, int potentialOptionKey = 0, int potentialOptionKey2 = 0)
	{
		if (itemMiscTemplet == null)
		{
			return;
		}
		m_NKMItemMiscTemplet = itemMiscTemplet;
		m_targetID = id;
		m_targetCount = count;
		m_setOptionID = setItemID;
		m_subSkillID = subSkillID;
		m_lstStatType = lstStatType;
		m_PotentialOptionKey = potentialOptionKey;
		m_PotentialOptionKey2 = potentialOptionKey2;
		NKCUtil.SetGameobjectActive(m_unit, itemMiscTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT);
		NKCUtil.SetGameobjectActive(m_ship, itemMiscTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP);
		NKCUtil.SetGameobjectActive(m_equip, itemMiscTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CHOICE_EQUIP);
		NKCUtil.SetGameobjectActive(m_misc, itemMiscTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CHOICE_MISC);
		NKCUtil.SetGameobjectActive(m_operator, itemMiscTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CHOICE_OPERATOR);
		NKCUtil.SetGameobjectActive(m_skin, itemMiscTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CHOICE_SKIN);
		switch (itemMiscTemplet.m_ItemMiscType)
		{
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT:
		{
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_CHOICE_UNIT_CONFIRM);
			int unitLv = 1;
			short unitLimitBreakLv = 0;
			int reactorLv = 0;
			int unitSkillLv = 1;
			int unitTacticLv = 0;
			if (m_NKMItemMiscTemplet.m_CustomBoxId > 0)
			{
				NKMCustomBoxTemplet nKMCustomBoxTemplet2 = NKMCustomBoxTemplet.Find(m_NKMItemMiscTemplet.m_CustomBoxId);
				if (nKMCustomBoxTemplet2 != null)
				{
					if (nKMCustomBoxTemplet2.Level > 0)
					{
						unitLv = nKMCustomBoxTemplet2.Level;
					}
					if (nKMCustomBoxTemplet2.LimitBreak > 0)
					{
						unitLimitBreakLv = (short)nKMCustomBoxTemplet2.LimitBreak;
					}
					if (nKMCustomBoxTemplet2.ReactorLevel > 0)
					{
						reactorLv = nKMCustomBoxTemplet2.ReactorLevel;
					}
					if (nKMCustomBoxTemplet2.SkillLevel > 0)
					{
						unitSkillLv = nKMCustomBoxTemplet2.SkillLevel;
					}
					if (nKMCustomBoxTemplet2.TacticUpdate > 0)
					{
						unitTacticLv = nKMCustomBoxTemplet2.TacticUpdate;
					}
				}
			}
			m_unit.SetData(m_targetID, unitLv, unitLimitBreakLv, unitTacticLv, reactorLv, unitSkillLv);
			break;
		}
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP:
		{
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_CHOICE_SHIP_CONFIRM);
			int shipLv = 1;
			short limitBreakLv = 0;
			if (m_NKMItemMiscTemplet.m_CustomBoxId > 0)
			{
				NKMCustomBoxTemplet nKMCustomBoxTemplet3 = NKMCustomBoxTemplet.Find(m_NKMItemMiscTemplet.m_CustomBoxId);
				if (nKMCustomBoxTemplet3 != null)
				{
					if (nKMCustomBoxTemplet3.Level > 0)
					{
						shipLv = nKMCustomBoxTemplet3.Level;
					}
					if (nKMCustomBoxTemplet3.LimitBreak > 0)
					{
						limitBreakLv = (short)nKMCustomBoxTemplet3.LimitBreak;
					}
				}
			}
			m_ship.SetData(m_targetID, shipLv, limitBreakLv);
			break;
		}
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_EQUIP:
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_CHOICE_EQUIP_CONFIRM);
			m_equip.SetData(m_targetID, setItemID, lstStatType, new List<int> { m_PotentialOptionKey, m_PotentialOptionKey2 });
			break;
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_MISC:
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_CHOICE_MISC_CONFIRM);
			m_misc.SetData(NKCPopupMiscUseCount.USE_ITEM_TYPE.Common, itemMiscTemplet.m_ItemMiscID, NKCUISlot.SlotData.MakeMiscItemData(m_targetID, m_targetCount));
			break;
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_OPERATOR:
		{
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_CHOICE_UNIT_CONFIRM);
			int opLevel = 1;
			int subSkillLv = 1;
			int mainSkillLv = 1;
			if (m_NKMItemMiscTemplet.m_CustomBoxId > 0)
			{
				NKMCustomBoxTemplet nKMCustomBoxTemplet = NKMCustomBoxTemplet.Find(m_NKMItemMiscTemplet.m_CustomBoxId);
				if (nKMCustomBoxTemplet != null)
				{
					if (nKMCustomBoxTemplet.Level > 0)
					{
						opLevel = nKMCustomBoxTemplet.Level;
					}
					if (nKMCustomBoxTemplet.TacticUpdate > 0)
					{
						mainSkillLv = nKMCustomBoxTemplet.TacticUpdate;
					}
					if (nKMCustomBoxTemplet.SkillLevel > 0)
					{
						subSkillLv = nKMCustomBoxTemplet.SkillLevel;
					}
				}
			}
			m_operator.SetData(m_targetID, subSkillID, opLevel, mainSkillLv, subSkillLv);
			break;
		}
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_SKIN:
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_CHOICE_SKIN_CONFIRM);
			m_skin.SetData(m_targetID, itemMiscTemplet.m_ItemMiscID);
			break;
		default:
			return;
		}
		UIOpened();
	}

	public void OnClickOk()
	{
		Close();
		NKCPopupResourceConfirmBox.Instance.OpenForSelection(m_NKMItemMiscTemplet, m_targetID, m_targetCount * UseCount, OnFinalConfirm, null, showResource: false, m_setOptionID);
	}

	private void OnFinalConfirm()
	{
		if (NKCShopManager.MakeSubstituteItem(NKCRandomBoxManager.GetRandomBoxItemTempletList(m_NKMItemMiscTemplet.m_RewardGroupID).Find((NKMRandomBoxItemTemplet x) => x.m_RewardID == m_targetID), UseCount, out var data))
		{
			List<NKCShopManager.ShopRewardSubstituteData> list = new List<NKCShopManager.ShopRewardSubstituteData>();
			list.Add(data);
			NKCPopupShopCustomPackageSubstitude.Instance.Open(list, SendPacket);
		}
		else
		{
			SendPacket();
		}
	}

	private void SendPacket()
	{
		NKCPacketSender.Send_NKMPacket_CHOICE_ITEM_USE_REQ(m_NKMItemMiscTemplet.m_ItemMiscID, m_targetID, UseCount, m_setOptionID, m_subSkillID, m_lstStatType, m_PotentialOptionKey, m_PotentialOptionKey2);
	}
}
