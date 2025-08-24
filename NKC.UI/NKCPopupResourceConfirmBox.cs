using System.Collections.Generic;
using NKM;
using NKM.Contract2;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupResourceConfirmBox : NKCUIBase
{
	public delegate void OnButton();

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_RESOURCE_USE_CONFIRM";

	private static NKCPopupResourceConfirmBox m_Instance;

	private OnButton dOnOKButton;

	private OnButton dOnCancelButton;

	public Text m_lbTitle;

	public Text m_lbMessage;

	public GameObject m_objCountLimit;

	public Text m_lbCountLimit;

	public GameObject m_objItemCostSlot;

	public NKCUIItemCostSlot m_ItemCostSlot;

	public GameObject m_objInvenSlot;

	public List<NKCUISlot> m_lstInvenSlot;

	public NKCUIComButton m_cbtnOK;

	public NKCUIComButton m_cbtnCancel;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private bool m_bResourceEnough;

	private int m_iNeedResourceID;

	private int m_iNeedResourceCount;

	private bool m_bShowResource;

	[Header("채용 포인트")]
	public GameObject m_CONTRACT_POINT;

	public Image m_CONTRACT_POINT_ICON;

	public Text m_CONTRACT_POINT_TEXT;

	[Header("일본 법무 대응")]
	public GameObject m_JPN_POLICY;

	private int m_ContractID;

	private int m_ContractCnt;

	public static NKCPopupResourceConfirmBox Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupResourceConfirmBox>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_RESOURCE_USE_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupResourceConfirmBox>();
				m_Instance?.Init();
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

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode
	{
		get
		{
			if (m_bShowResource)
			{
				return NKCUIUpsideMenu.eMode.ResourceOnly;
			}
			return base.eUpsideMenuMode;
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void Init()
	{
		foreach (NKCUISlot item in m_lstInvenSlot)
		{
			item?.Init();
		}
		NKCUtil.SetHotkey(m_cbtnOK, HotkeyEventType.Confirm);
	}

	public void OpenWithLeftCount(string Title, string Content, int itemID, int requiredCount, int leftCount, int maxCount, OnButton onOkButton, OnButton onCancelButton = null)
	{
		NKCUtil.SetGameobjectActive(m_objCountLimit, bValue: true);
		NKCUtil.SetLabelText(m_lbCountLimit, string.Format(NKCUtilString.GET_STRING_REMAIN_COUNT_TWO_PARAM, leftCount, maxCount));
		m_lbCountLimit.color = Color.white;
		OpenInternal(Title, Content, itemID, requiredCount, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemID), onOkButton, onCancelButton);
	}

	public void Open(string Title, string Content, int itemID, int requiredCount, long curItemCount, OnButton onOkButton, OnButton onCancelButton = null, bool showResource = false)
	{
		m_bShowResource = showResource;
		NKCUtil.SetGameobjectActive(m_objCountLimit, bValue: false);
		OpenInternal(Title, Content, itemID, requiredCount, curItemCount, onOkButton, onCancelButton);
	}

	public void Open(string Title, string Content, int itemID, int requiredCount, OnButton onOkButton, OnButton onCancelButton = null, bool showResource = false)
	{
		m_bShowResource = showResource;
		NKCUtil.SetGameobjectActive(m_objCountLimit, bValue: false);
		OpenInternal(Title, Content, itemID, requiredCount, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemID), onOkButton, onCancelButton);
	}

	public void OpenForContract(string Title, string Content, int itemID, int requiredCount, OnButton onOkButton, OnButton onCancelButton = null, int contractID = 0, int contractCnt = 0)
	{
		m_ContractID = contractID;
		m_ContractCnt = contractCnt;
		NKCUtil.SetGameobjectActive(m_objCountLimit, bValue: false);
		OpenInternal(Title, Content, itemID, requiredCount, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemID), onOkButton, onCancelButton);
		NKCUtil.SetGameobjectActive(m_objItemCostSlot, itemID > 0 && requiredCount > 0);
		m_ContractID = 0;
		m_ContractCnt = 0;
	}

	private void OpenInternal(string Title, string Content, int itemID, int requiredCount, long curItemCount, OnButton onOkButton, OnButton onCancelButton)
	{
		if (m_NKCUIOpenAnimator == null)
		{
			m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		}
		NKCUtil.SetGameobjectActive(m_objInvenSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCountLimit, bValue: false);
		NKCScenManager.CurrentUserData();
		NKCUtil.SetGameobjectActive(m_objItemCostSlot, bValue: true);
		m_ItemCostSlot.SetData(itemID, requiredCount, curItemCount);
		m_bResourceEnough = requiredCount <= curItemCount;
		if (!m_bResourceEnough)
		{
			m_iNeedResourceID = itemID;
			m_iNeedResourceCount = requiredCount - (int)curItemCount;
		}
		NKCUtil.SetLabelText(m_lbTitle, Title);
		NKCUtil.SetLabelText(m_lbMessage, Content);
		NKCUtil.SetGameobjectActive(m_CONTRACT_POINT, bValue: false);
		if (m_ContractID != 0 && m_ContractCnt > 0)
		{
			ContractTempletV2 contractTempletV = ContractTempletV2.Find(m_ContractID);
			if (contractTempletV != null)
			{
				SetContractPointUI(contractTempletV.m_ResultRewards, m_ContractCnt, !contractTempletV.MissionCountIgnore);
			}
			else
			{
				CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(m_ContractID);
				if (customPickupContractTemplet != null)
				{
					SetContractPointUI(customPickupContractTemplet.ResultRewards, m_ContractCnt, !customPickupContractTemplet.MissionCountIgnore);
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_JPN_POLICY, NKCUtil.IsJPNPolicyRelatedItem(itemID));
		dOnOKButton = onOkButton;
		dOnCancelButton = onCancelButton;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	private void SetContractPointUI(List<RewardUnit> lstResultReward, int contractCnt, bool bShowContractPoint)
	{
		if (lstResultReward != null && lstResultReward.Count > 0)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(lstResultReward[0].ItemID);
			if (itemMiscTempletByID != null)
			{
				Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
				NKCUtil.SetImageSprite(m_CONTRACT_POINT_ICON, orLoadMiscItemSmallIcon);
				NKCUtil.SetLabelText(m_CONTRACT_POINT_TEXT, string.Format(NKCUtilString.GET_STRING_POPUP_RESOURCE_CONFIRM_REWARD_DESC_02, itemMiscTempletByID.GetItemName(), lstResultReward[0].Count * contractCnt));
				NKCUtil.SetGameobjectActive(m_CONTRACT_POINT, bShowContractPoint);
			}
		}
	}

	public void OpenForSelection(NKMItemMiscTemplet itemMiscTemplet, int targetID, long targetCount, OnButton onOkButton, OnButton onCancelButton = null, bool showResource = false, int setItemID = 0)
	{
		NKCUtil.SetGameobjectActive(m_objCountLimit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objItemCostSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objInvenSlot, bValue: true);
		NKCUtil.SetGameobjectActive(m_CONTRACT_POINT, bValue: false);
		m_bResourceEnough = true;
		NKCUtil.SetLabelText(m_lbTitle, itemMiscTemplet.GetItemName());
		NKCUISlot.SlotData data = new NKCUISlot.SlotData();
		switch (itemMiscTemplet.m_ItemMiscType)
		{
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT:
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_OPERATOR:
			NKCUtil.SetLabelText(m_lbMessage, NKCUtilString.GET_STRING_CHOICE_UNIT_RECHECK);
			data = NKCUISlot.SlotData.MakeUnitData(targetID, 1);
			break;
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP:
			NKCUtil.SetLabelText(m_lbMessage, NKCUtilString.GET_STRING_CHOICE_SHIP_RECHECK);
			data = NKCUISlot.SlotData.MakeUnitData(targetID, 1);
			break;
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_EQUIP:
			NKCUtil.SetLabelText(m_lbMessage, NKCUtilString.GET_STRING_CHOICE_EQUIP_RECHECK);
			data = NKCUISlot.SlotData.MakeEquipData(targetID, (int)targetCount, setItemID);
			break;
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_MISC:
			NKCUtil.SetLabelText(m_lbMessage, NKCUtilString.GET_STRING_CHOICE_MISC_RECHECK);
			data = NKCUISlot.SlotData.MakeMiscItemData(targetID, targetCount);
			break;
		case NKM_ITEM_MISC_TYPE.IMT_CHOICE_SKIN:
			NKCUtil.SetLabelText(m_lbMessage, NKCUtilString.GET_STRING_CHOICE_SKIN_RECHECK);
			data = NKCUISlot.SlotData.MakeSkinData(targetID);
			break;
		}
		NKCUtil.SetGameobjectActive(m_lstInvenSlot[0], bValue: true);
		m_lstInvenSlot[0].SetData(data);
		for (int i = 1; i < m_lstInvenSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstInvenSlot[i], bValue: false);
		}
		dOnOKButton = onOkButton;
		dOnCancelButton = onCancelButton;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_NKCUIOpenAnimator == null)
		{
			m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	public void OpenItemSlotList(string title, string content, List<NKCUISlot.SlotData> lstSlot, OnButton onOkButton, OnButton onCancelButton = null, bool mustShowNum = false)
	{
		if (lstSlot == null)
		{
			Debug.LogError("lstSlot Null!");
			return;
		}
		NKCUtil.SetGameobjectActive(m_objCountLimit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objItemCostSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objInvenSlot, bValue: true);
		NKCUtil.SetGameobjectActive(m_CONTRACT_POINT, bValue: false);
		m_bResourceEnough = true;
		NKCUtil.SetLabelText(m_lbTitle, title);
		NKCUtil.SetLabelText(m_lbMessage, content);
		for (int i = 0; i < m_lstInvenSlot.Count; i++)
		{
			if (i < lstSlot.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstInvenSlot[i], bValue: true);
				if (mustShowNum)
				{
					m_lstInvenSlot[i].SetData(lstSlot[i], bShowName: false, bShowNumber: true, bEnableLayoutElement: true, null);
				}
				else
				{
					m_lstInvenSlot[i].SetData(lstSlot[i]);
				}
				m_lstInvenSlot[i].SetOnClickAction(default(NKCUISlot.SlotClickType));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstInvenSlot[i], bValue: false);
			}
		}
		dOnOKButton = onOkButton;
		dOnCancelButton = onCancelButton;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_NKCUIOpenAnimator == null)
		{
			m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	public void OpenForConfirm(string title, string content, OnButton onOkButton, OnButton onCancelButton = null, bool showResource = false)
	{
		m_bShowResource = showResource;
		m_bResourceEnough = true;
		NKCUtil.SetGameobjectActive(m_objCountLimit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objItemCostSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objInvenSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_CONTRACT_POINT, bValue: false);
		NKCUtil.SetLabelText(m_lbTitle, title);
		NKCUtil.SetLabelText(m_lbMessage, content);
		dOnOKButton = onOkButton;
		dOnCancelButton = onCancelButton;
		if (m_NKCUIOpenAnimator == null)
		{
			m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	public void OpenWithText(string title, string content, string text, OnButton onOkButton, OnButton onCancelButton = null, bool showResource = false)
	{
		m_bShowResource = showResource;
		m_bResourceEnough = true;
		NKCUtil.SetGameobjectActive(m_objCountLimit, bValue: true);
		NKCUtil.SetGameobjectActive(m_objItemCostSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objInvenSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_CONTRACT_POINT, bValue: false);
		NKCUtil.SetLabelText(m_lbTitle, title);
		NKCUtil.SetLabelText(m_lbMessage, content);
		dOnOKButton = onOkButton;
		dOnCancelButton = onCancelButton;
		NKCUtil.SetLabelText(m_lbCountLimit, text);
		m_lbCountLimit.color = Color.white;
		if (m_NKCUIOpenAnimator == null)
		{
			m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public void OnOK()
	{
		if (!m_bResourceEnough)
		{
			Close();
			NKCShopManager.OpenItemLackPopup(m_iNeedResourceID, m_iNeedResourceCount);
			return;
		}
		Close();
		if (dOnOKButton != null)
		{
			dOnOKButton();
		}
	}

	public void OnCancel()
	{
		Close();
		if (dOnCancelButton != null)
		{
			dOnCancelButton();
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}
}
