using System;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupMiscUseCountContents : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	public Text m_lbItemName;

	public GameObject m_objItemInventoryCountParent;

	public Text m_lbSlotItemInventoryCountDesc;

	public Text m_lbSlotItemInventoryCount;

	public Text m_lbItemDesc;

	public NKCUISlot m_NKCUIItemSlot;

	public GameObject m_objUseItemCount;

	public Text m_lbUseItemInventoryCountDesc;

	public Text m_lbUseItemInventoryCount;

	public GameObject m_objCount;

	public NKCUIComStateButton m_btnCountMinus;

	public NKCUIComStateButton m_btnCountPlus;

	public NKCUIComStateButton m_btnCountMax;

	public Text m_lbCount;

	[Header("기간제 아이템")]
	public GameObject m_objTimeInterval;

	public Text m_lbTimeLeft;

	private NKMItemMiscTemplet m_useMiscTemplet;

	private NKCUISlot.SlotData m_currentItemData;

	private const int MAX_USE_COUNT = 10000;

	private const int PACKAGE_MAX_USE_COUNT = 100;

	public const float PRESS_GAP_MAX = 0.35f;

	public const float PRESS_GAP_MIN = 0.01f;

	public const float DAMPING = 0.8f;

	private float m_fDelay = 0.5f;

	private float m_fHoldTime;

	private int m_iChangeValue;

	private bool m_bPress;

	private bool m_bWasHold;

	public long m_useCount { get; private set; }

	public long m_maxCount { get; private set; }

	public void Init()
	{
		m_btnCountMinus?.PointerDown.RemoveAllListeners();
		m_btnCountMinus?.PointerDown.AddListener(OnMinusDown);
		m_btnCountMinus?.PointerUp.RemoveAllListeners();
		m_btnCountMinus?.PointerUp.AddListener(OnButtonUp);
		NKCUtil.SetHotkey(m_btnCountMinus, HotkeyEventType.Minus, null, bUpDownEvent: true);
		m_btnCountPlus?.PointerDown.RemoveAllListeners();
		m_btnCountPlus?.PointerDown.AddListener(OnPlusDown);
		m_btnCountPlus?.PointerUp.RemoveAllListeners();
		m_btnCountPlus?.PointerUp.AddListener(OnButtonUp);
		NKCUtil.SetHotkey(m_btnCountPlus, HotkeyEventType.Plus, null, bUpDownEvent: true);
		m_btnCountMax?.PointerClick.RemoveAllListeners();
		m_btnCountMax?.PointerClick.AddListener(OnButtonMax);
		m_NKCUIItemSlot?.Init();
		NKCUtil.SetBindFunction(m_btnCountMinus, delegate
		{
			OnChangeCount(bPlus: false);
		});
		NKCUtil.SetBindFunction(m_btnCountPlus, delegate
		{
			OnChangeCount();
		});
	}

	public void SetData(NKCPopupMiscUseCount.USE_ITEM_TYPE openType, int useItemID, NKCUISlot.SlotData slotData)
	{
		if (slotData == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_useMiscTemplet = NKMItemManager.GetItemMiscTempletByID(useItemID);
		if (m_useMiscTemplet != null)
		{
			switch (openType)
			{
			case NKCPopupMiscUseCount.USE_ITEM_TYPE.Common:
				NKCUtil.SetLabelText(m_lbSlotItemInventoryCountDesc, NKCStringTable.GetString("SI_DP_POPUP_USE_COUNT_TEXT_MISC_HAVE"));
				NKCUtil.SetLabelText(m_lbUseItemInventoryCountDesc, NKCStringTable.GetString("SI_DP_POPUP_USE_COUNT_HAVE_TEXT_MISC_CHOICE"));
				break;
			case NKCPopupMiscUseCount.USE_ITEM_TYPE.DailyTicket:
				NKCUtil.SetLabelText(m_lbSlotItemInventoryCountDesc, NKCStringTable.GetString("SI_DP_POPUP_USE_COUNT_TEXT_MISC_REMAIN_COUNT"));
				NKCUtil.SetLabelText(m_lbUseItemInventoryCountDesc, NKCStringTable.GetString("SI_DP_POPUP_USE_COUNT_HAVE_TEXT_MISC_DAILY_TICKET"));
				break;
			}
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			m_currentItemData = slotData;
			m_NKCUIItemSlot.SetData(m_currentItemData, bShowName: false, m_useMiscTemplet.IsChoiceItem(), bEnableLayoutElement: false, null);
			m_NKCUIItemSlot.SetOnClickAction(NKCUISlot.SlotClickType.RatioList, NKCUISlot.SlotClickType.BoxList);
			long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(useItemID);
			m_useCount = 1L;
			if (m_useMiscTemplet.IsPackageItem)
			{
				m_maxCount = Math.Min(countMiscItem, 100L);
			}
			else
			{
				m_maxCount = Math.Min(countMiscItem, 10000L);
			}
			NKCUtil.SetGameobjectActive(m_objUseItemCount, openType == NKCPopupMiscUseCount.USE_ITEM_TYPE.DailyTicket || m_useMiscTemplet.IsChoiceItem());
			if (m_objUseItemCount.activeSelf)
			{
				NKCUtil.SetLabelText(m_lbUseItemInventoryCount, countMiscItem.ToString("#,##0"));
			}
			SetTextFromSlotData(m_currentItemData);
			UpdateCountInfo();
			SetIntervalItem(slotData);
		}
	}

	private void SetTextFromSlotData(NKCUISlot.SlotData data)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(data.ID);
		if (itemMiscTempletByID != null)
		{
			NKCUtil.SetLabelText(m_lbItemName, itemMiscTempletByID.GetItemName());
			NKCUtil.SetLabelText(m_lbItemDesc, itemMiscTempletByID.GetItemDesc());
			long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(data.ID);
			NKCUtil.SetLabelText(m_lbSlotItemInventoryCount, countMiscItem.ToString("#,##0"));
		}
	}

	public void Update()
	{
		OnUpdateButtonHold();
	}

	private void UpdateCountInfo()
	{
		if (m_useMiscTemplet.IsChoiceItem())
		{
			m_NKCUIItemSlot.SetSlotItemCount(m_currentItemData.Count * m_useCount);
		}
		NKCUtil.SetLabelText(m_lbCount, m_useCount.ToString("N0"));
	}

	private void SetIntervalItem(NKCUISlot.SlotData slotData)
	{
		bool flag = false;
		if (slotData != null && slotData.eType == NKCUISlot.eSlotMode.ItemMisc)
		{
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(slotData.ID);
			flag = nKMItemMiscTemplet?.IsTimeIntervalItem ?? false;
			if (flag)
			{
				string timeSpanStringDHM = NKCUtilString.GetTimeSpanStringDHM(nKMItemMiscTemplet.GetIntervalTimeSpanLeft());
				NKCUtil.SetLabelText(m_lbTimeLeft, timeSpanStringDHM);
			}
		}
		NKCUtil.SetGameobjectActive(m_objTimeInterval, flag);
	}

	public void OnChangeCount(bool bPlus = true)
	{
		if (m_bWasHold)
		{
			m_bWasHold = false;
			return;
		}
		if (!bPlus && m_useCount == 1)
		{
			if (m_maxCount > 0)
			{
				m_useCount = m_maxCount;
				UpdateCountInfo();
			}
			OnButtonUp();
			return;
		}
		m_useCount += (bPlus ? 1 : (-1));
		if (!bPlus && m_useCount <= 1)
		{
			m_useCount = 1L;
		}
		if (bPlus && m_useCount >= m_maxCount)
		{
			m_useCount = m_maxCount;
		}
		UpdateCountInfo();
	}

	private void OnMinusDown(PointerEventData eventData)
	{
		m_iChangeValue = -1;
		m_bPress = true;
		m_fDelay = 0.35f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
	}

	private void OnPlusDown(PointerEventData eventData)
	{
		m_iChangeValue = 1;
		m_bPress = true;
		m_fDelay = 0.35f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
	}

	private void OnButtonUp()
	{
		m_iChangeValue = 0;
		m_fDelay = 0.35f;
		m_bPress = false;
	}

	private void OnButtonMax()
	{
		m_iChangeValue = 0;
		m_fDelay = 0.35f;
		m_bPress = false;
		m_useCount = m_maxCount;
		UpdateCountInfo();
	}

	private void OnUpdateButtonHold()
	{
		if (!m_bPress)
		{
			return;
		}
		m_fHoldTime += Time.deltaTime;
		if (m_fHoldTime >= m_fDelay)
		{
			m_fHoldTime = 0f;
			m_fDelay *= 0.8f;
			int num = ((!(m_fDelay < 0.01f)) ? 1 : 5);
			m_fDelay = Mathf.Clamp(m_fDelay, 0.01f, 0.35f);
			m_useCount += m_iChangeValue * num;
			m_bWasHold = true;
			if (m_iChangeValue < 0 && m_useCount < 1)
			{
				m_useCount = 1L;
				m_bPress = false;
			}
			if (m_iChangeValue > 0 && m_useCount > m_maxCount)
			{
				m_useCount = m_maxCount;
				m_bPress = false;
			}
			UpdateCountInfo();
		}
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (eventData.scrollDelta.y < 0f)
		{
			m_useCount--;
		}
		else if (eventData.scrollDelta.y > 0f)
		{
			m_useCount++;
		}
		if (m_maxCount < m_useCount)
		{
			m_useCount = m_maxCount;
		}
		if (m_useCount < 1)
		{
			m_useCount = 1L;
		}
		UpdateCountInfo();
	}
}
