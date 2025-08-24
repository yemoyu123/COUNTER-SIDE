using System;
using System.Collections.Generic;
using ClientPacket.Item;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupForgeCraft : NKCUIBase, IScrollHandler, IEventSystemHandler
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_FACTORY";

	private const string UI_ASSET_NAME = "NKM_UI_FACTORY_CRAFT_POPUP";

	private static NKCPopupForgeCraft m_Instance;

	private readonly List<int> RESOURCE_LIST = new List<int> { 1, 2, 101 };

	public GameObject m_goToAnimate;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUISlot m_AB_ICON_SLOT;

	public Text m_NKM_UI_POPUP_CRAFT_NAME;

	public Text m_NKM_UI_POPUP_CRAFT_COUNT;

	public Text m_NKM_UI_POPUP_CRAFT_TIME_TEXT;

	public Text m_STACK_TEXT;

	public NKCUIItemCostSlot m_firstCostSlot;

	public List<NKCUIItemCostSlot> m_lst_Material;

	public EventTrigger m_etBG;

	public NKCUIComStateButton m_btnMinus;

	public NKCUIComStateButton m_btnMinus10;

	public NKCUIComStateButton m_btnMinus100;

	public NKCUIComStateButton m_btnPlus;

	public NKCUIComStateButton m_btnPlus10;

	public NKCUIComStateButton m_btnPlus100;

	public NKCUIComButton m_btnMax;

	public NKCUIComButton m_btnCancel;

	public NKCUIComButton m_btnCraft;

	public NKCUIComButton m_btnClose;

	private int m_CurrCountToMake = 1;

	private NKMMoldItemData m_cNKMMoldItemData;

	public Text m_NKM_UI_FACTORY_CRAFT_POPUP_TOP_TEXT;

	public static NKCPopupForgeCraft Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupForgeCraft>("AB_UI_NKM_UI_FACTORY", "NKM_UI_FACTORY_CRAFT_POPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupForgeCraft>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_FORGE_CRAFT_POPUP;

	public override List<int> UpsideMenuShowResourceList => RESOURCE_LIST;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.ResourceOnly;

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

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(m_goToAnimate);
		if (m_AB_ICON_SLOT != null)
		{
			m_AB_ICON_SLOT.Init();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			OnCloseBtn();
		});
		m_etBG.triggers.Clear();
		m_etBG.triggers.Add(entry);
		NKCUtil.SetBindFunction(m_btnMinus, delegate
		{
			OnClickMinus(1);
		});
		NKCUtil.SetBindFunction(m_btnMinus10, delegate
		{
			OnClickMinus(10);
		});
		NKCUtil.SetBindFunction(m_btnMinus100, delegate
		{
			OnClickMinus(100);
		});
		m_btnMinus.dOnPointerHoldPress = delegate
		{
			OnHoldMinus(1);
		};
		m_btnMinus10.dOnPointerHoldPress = delegate
		{
			OnHoldMinus(10);
		};
		m_btnMinus100.dOnPointerHoldPress = delegate
		{
			OnHoldMinus(100);
		};
		NKCUtil.SetHotkey(m_btnMinus, HotkeyEventType.Minus);
		NKCUtil.SetBindFunction(m_btnPlus, delegate
		{
			OnClickPlus(1);
		});
		NKCUtil.SetBindFunction(m_btnPlus10, delegate
		{
			OnClickPlus(10);
		});
		NKCUtil.SetBindFunction(m_btnPlus100, delegate
		{
			OnClickPlus(100);
		});
		m_btnPlus.dOnPointerHoldPress = delegate
		{
			OnClickPlus(1);
		};
		m_btnPlus10.dOnPointerHoldPress = delegate
		{
			OnClickPlus(10);
		};
		m_btnPlus100.dOnPointerHoldPress = delegate
		{
			OnClickPlus(100);
		};
		NKCUtil.SetHotkey(m_btnPlus, HotkeyEventType.Plus);
		m_btnMax.PointerClick.RemoveAllListeners();
		m_btnMax.PointerClick.AddListener(OnClickMax);
		m_btnCraft.PointerClick.RemoveAllListeners();
		m_btnCraft.PointerClick.AddListener(OnClickMake);
		NKCUtil.SetHotkey(m_btnCraft, HotkeyEventType.Confirm);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(OnCloseBtn);
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(OnCloseBtn);
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMMoldItemData cNKMMoldItemData)
	{
		if (cNKMMoldItemData != null)
		{
			base.gameObject.SetActive(value: true);
			m_NKCUIOpenAnimator.PlayOpenAni();
			m_CurrCountToMake = 1;
			SetUI(cNKMMoldItemData);
			UIOpened();
		}
	}

	private void SetTimeUI()
	{
		if (m_cNKMMoldItemData != null)
		{
			int num = m_CurrCountToMake;
			if (num <= 0)
			{
				num = 1;
			}
			int num2 = 0;
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(m_cNKMMoldItemData.m_MoldID);
			if (itemMoldTempletByID != null)
			{
				num2 = itemMoldTempletByID.m_Time * num;
			}
			TimeSpan timeSpan = new TimeSpan(num2 / 60, num2 % 60, 0);
			m_NKM_UI_POPUP_CRAFT_TIME_TEXT.text = NKCUtilString.GetTimeSpanString(timeSpan);
		}
	}

	private void SetMaterialUI()
	{
		if (m_cNKMMoldItemData == null)
		{
			return;
		}
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(m_cNKMMoldItemData.m_MoldID);
		if (itemMoldTempletByID == null)
		{
			return;
		}
		int num = m_CurrCountToMake;
		if (num < 1)
		{
			num = 1;
		}
		if (itemMoldTempletByID.m_MaterialList.Count > 0)
		{
			NKCUtil.SetGameobjectActive(m_firstCostSlot, bValue: true);
			NKMItemMoldMaterialData nKMItemMoldMaterialData = itemMoldTempletByID.m_MaterialList[0];
			long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(nKMItemMoldMaterialData.m_MaterialID);
			int credit = itemMoldTempletByID.m_MaterialList[0].m_MaterialValue * num;
			bool flag = itemMoldTempletByID.m_MaterialList[0].m_MaterialID == 1 && NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT);
			if (flag)
			{
				NKCCompanyBuff.SetDiscountOfCreditInCraft(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit);
			}
			m_firstCostSlot.SetData(nKMItemMoldMaterialData.m_MaterialID, credit, countMiscItem, bShowTooltip: false, bShowBG: false, flag);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_firstCostSlot, bValue: false);
		}
		for (int i = 0; i < m_lst_Material.Count; i++)
		{
			int num2 = i + 1;
			if (num2 >= itemMoldTempletByID.m_MaterialList.Count)
			{
				m_lst_Material[i].SetData(0, 0, 0L);
				continue;
			}
			long countMiscItem2 = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemMoldTempletByID.m_MaterialList[num2].m_MaterialID);
			int credit2 = itemMoldTempletByID.m_MaterialList[num2].m_MaterialValue * num;
			bool flag2 = itemMoldTempletByID.m_MaterialList[num2].m_MaterialID == 1 && NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT);
			if (flag2)
			{
				NKCCompanyBuff.SetDiscountOfCreditInCraft(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit2);
			}
			m_lst_Material[i].SetData(itemMoldTempletByID.m_MaterialList[num2].m_MaterialID, credit2, countMiscItem2, bShowTooltip: true, bShowBG: true, flag2);
		}
	}

	private void SetUI(NKMMoldItemData cNKMMoldItemData)
	{
		if (cNKMMoldItemData == null)
		{
			return;
		}
		m_cNKMMoldItemData = cNKMMoldItemData;
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(cNKMMoldItemData.m_MoldID);
		if (itemMoldTempletByID != null)
		{
			m_AB_ICON_SLOT.SetData(NKCUISlot.SlotData.MakeMoldItemData(itemMoldTempletByID.m_MoldID, cNKMMoldItemData.m_Count));
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CRAFT_COUNT, !itemMoldTempletByID.m_bPermanent);
			if (!itemMoldTempletByID.m_bPermanent)
			{
				NKCUtil.SetLabelText(m_NKM_UI_POPUP_CRAFT_COUNT, string.Format(NKCUtilString.GET_STRING_ITEM_COUNT_ONE_PARAM, cNKMMoldItemData.m_Count));
			}
			NKCUtil.SetLabelText(m_NKM_UI_FACTORY_CRAFT_POPUP_TOP_TEXT, NKCUtilString.GET_STRING_FORGE_CRAFT_POPUP_TITLE);
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_CRAFT_NAME, itemMoldTempletByID.GetItemName());
			SetTimeUI();
			SetMaterialUI();
			SetCountUI();
		}
	}

	public void OnClickMinus(int iNum)
	{
		OnMinus(bAllowJumpToMax: true, iNum);
	}

	public void OnHoldMinus(int iNum)
	{
		OnMinus(bAllowJumpToMax: false, iNum);
	}

	public void OnMinus(bool bAllowJumpToMax, int iNum)
	{
		if (m_CurrCountToMake == 1 && bAllowJumpToMax)
		{
			m_CurrCountToMake = GetMaxCount();
		}
		else if (m_CurrCountToMake >= 2)
		{
			m_CurrCountToMake -= iNum;
		}
		m_CurrCountToMake = Mathf.Max(0, m_CurrCountToMake);
		SetTimeUI();
		SetCountUI();
		SetMaterialUI();
	}

	public void OnClickPlus(int iNum)
	{
		m_CurrCountToMake += iNum;
		m_CurrCountToMake = Mathf.Clamp(m_CurrCountToMake, 1, GetMaxCount());
		SetTimeUI();
		SetCountUI();
		SetMaterialUI();
	}

	private int GetMaxCount()
	{
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(m_cNKMMoldItemData.m_MoldID);
		long num = 0L;
		num = ((!NKMItemManager.IsStackMoldItem(itemMoldTempletByID)) ? ((itemMoldTempletByID != null && !itemMoldTempletByID.m_bPermanent) ? m_cNKMMoldItemData.m_Count : 999) : NKMItemManager.GetRemainResetCountStack(itemMoldTempletByID));
		int equipCreatableCount = NKCUtil.GetEquipCreatableCount(m_cNKMMoldItemData, NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData);
		return Mathf.Min((int)num, equipCreatableCount);
	}

	private void SetCountUI()
	{
		m_STACK_TEXT.text = m_CurrCountToMake.ToString();
	}

	public void OnClickMax()
	{
		m_CurrCountToMake = GetMaxCount();
		SetTimeUI();
		SetCountUI();
		SetMaterialUI();
	}

	public void OnClickMake()
	{
		if (m_CurrCountToMake < 1 || m_cNKMMoldItemData == null)
		{
			return;
		}
		NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(m_cNKMMoldItemData.m_MoldID);
		if (itemMoldTempletByID != null)
		{
			for (int i = 0; i < itemMoldTempletByID.m_MaterialList.Count; i++)
			{
				long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemMoldTempletByID.m_MaterialList[i].m_MaterialID);
				int credit = itemMoldTempletByID.m_MaterialList[i].m_MaterialValue;
				if (itemMoldTempletByID.m_MaterialList[i].m_MaterialID == 1 && NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT))
				{
					NKCCompanyBuff.SetDiscountOfCreditInCraft(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit);
				}
				if (credit * m_CurrCountToMake > countMiscItem)
				{
					NKCShopManager.OpenItemLackPopup(itemMoldTempletByID.m_MaterialList[i].m_MaterialID, itemMoldTempletByID.m_MaterialList[i].m_MaterialValue * m_CurrCountToMake);
					return;
				}
			}
		}
		if (itemMoldTempletByID != null && itemMoldTempletByID.IsEquipMold)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				NKMInventoryData inventoryData = nKMUserData.m_InventoryData;
				if (inventoryData != null)
				{
					int maxItemEqipCount = inventoryData.m_MaxItemEqipCount;
					if (inventoryData.GetCountEquipItemTypes() >= maxItemEqipCount)
					{
						int count = 1;
						int resultCount;
						bool flag = !NKCAdManager.IsAdRewardInventory(NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP) || !NKMInventoryManager.CanExpandInventoryByAd(NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP, nKMUserData, count, out resultCount);
						if (!NKMInventoryManager.CanExpandInventory(NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP, nKMUserData, count, out resultCount) && flag)
						{
							NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_CANNOT_EXPAND_INVENTORY));
							return;
						}
						string expandDesc = NKCUtilString.GetExpandDesc(NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP, isFullMsg: true);
						NKCPopupInventoryAdd.SliderInfo sliderInfo = new NKCPopupInventoryAdd.SliderInfo
						{
							increaseCount = 5,
							maxCount = 2000,
							currentCount = maxItemEqipCount,
							inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP
						};
						NKCPopupInventoryAdd.Instance.Open(NKCUtilString.GET_STRING_INVENTORY_EQUIP, expandDesc, sliderInfo, 50, 101, delegate(int value)
						{
							NKCPacketSender.Send_NKMPacket_INVENTORY_EXPAND_REQ(NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP, value);
						}, showResource: true);
						return;
					}
				}
			}
		}
		NKMPacket_CRAFT_INSTANT_REQ nKMPacket_CRAFT_INSTANT_REQ = new NKMPacket_CRAFT_INSTANT_REQ();
		nKMPacket_CRAFT_INSTANT_REQ.moldId = m_cNKMMoldItemData.m_MoldID;
		nKMPacket_CRAFT_INSTANT_REQ.moldCount = m_CurrCountToMake;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_CRAFT_INSTANT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public void CloseForgeCraftPopup()
	{
		Close();
	}

	public void OnCloseBtn()
	{
		Close();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (eventData.scrollDelta.y < 0f)
		{
			OnMinus(bAllowJumpToMax: false, 1);
		}
		else if (eventData.scrollDelta.y > 0f)
		{
			OnClickPlus(1);
		}
	}
}
