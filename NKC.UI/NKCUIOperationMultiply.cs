using NKC.UI.Guide;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationMultiply : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	public delegate void OnCountUpdated(int count);

	public GameObject m_objUnlock;

	public GameObject m_objLock;

	public NKCUIItemCostSlot m_NKCUIItemCostSlot;

	public Text m_lbCount;

	public NKCUIComStateButton m_csbtnUp;

	public NKCUIComStateButton m_csbtnDown;

	public NKCUIComStateButton m_btnInfo;

	public NKCUIComStateButton m_btnClose;

	private UnityAction m_dOnClickEventClose;

	private OnCountUpdated m_dOnCountUpdated;

	private int m_MultiplyCostItemID;

	private int m_MultiplyCostItemCount = 1;

	private int m_DungeonCostItemID;

	private int m_DungeonCostItemCount = 1;

	private int m_currentCount;

	private int m_MaxCount = 99;

	private int m_MinCount = 2;

	private bool m_bWasHold;

	public void Init(OnCountUpdated onCountUpdated = null, UnityAction closeEvent = null)
	{
		m_dOnCountUpdated = onCountUpdated;
		m_dOnClickEventClose = closeEvent;
		m_csbtnUp.PointerClick.RemoveAllListeners();
		m_csbtnUp.PointerClick.AddListener(OnClickUp);
		m_csbtnUp.dOnPointerHoldPress = OnClickUp;
		m_csbtnUp.SetHotkey(HotkeyEventType.Plus);
		m_csbtnDown.PointerClick.RemoveAllListeners();
		m_csbtnDown.PointerClick.AddListener(OnClickDown);
		m_csbtnDown.dOnPointerHoldPress = OnHoldDown;
		m_csbtnDown.SetHotkey(HotkeyEventType.Minus);
		if (m_btnInfo != null)
		{
			m_btnInfo.PointerClick.RemoveAllListeners();
			m_btnInfo.PointerClick.AddListener(OnClickInfo);
		}
		if (m_btnClose != null)
		{
			m_btnClose.PointerClick.RemoveAllListeners();
			m_btnClose.PointerClick.AddListener(OnClickClose);
		}
	}

	private void OnClickDown()
	{
		m_currentCount--;
		if (!m_bWasHold && m_currentCount < m_MinCount)
		{
			m_currentCount = m_MaxCount;
		}
		m_bWasHold = false;
		OnValueChanged(bInvokeCallback: true);
	}

	private void OnClickUp()
	{
		m_currentCount++;
		OnValueChanged(bInvokeCallback: true);
	}

	private void OnHoldDown()
	{
		m_bWasHold = true;
		m_currentCount--;
		OnValueChanged(bInvokeCallback: true);
	}

	private void OnClickInfo()
	{
		NKCUIPopUpGuide.Instance.Open("ARTICLE_SYSTEM_SUPPORT_BATTLE", 1);
	}

	private void OnClickClose()
	{
		if (m_dOnClickEventClose != null)
		{
			m_dOnClickEventClose();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void SetLockUI(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objUnlock, !bSet);
		NKCUtil.SetGameobjectActive(m_objLock, bSet);
	}

	private void OnValueChanged(bool bInvokeCallback)
	{
		m_currentCount = Mathf.Clamp(m_currentCount, m_MinCount, GetMaxCount());
		if (m_NKCUIItemCostSlot != null)
		{
			m_NKCUIItemCostSlot.SetData(m_MultiplyCostItemID, m_MultiplyCostItemCount * m_currentCount, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_MultiplyCostItemID));
		}
		NKCUtil.SetLabelText(m_lbCount, m_currentCount.ToString());
		if (bInvokeCallback)
		{
			m_dOnCountUpdated?.Invoke(m_currentCount);
		}
	}

	private int GetMaxCount()
	{
		int num = 99;
		if (m_DungeonCostItemID != 0 && m_DungeonCostItemCount != 0)
		{
			num = (int)(NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_DungeonCostItemID) / m_DungeonCostItemCount);
		}
		int num2 = 99;
		if (m_MultiplyCostItemID != 0 && m_MultiplyCostItemCount != 0)
		{
			num2 = (int)(NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_MultiplyCostItemID) / m_MultiplyCostItemCount);
		}
		return Mathf.Min(99, m_MaxCount, num, num2);
	}

	public void SetData(int multiplyCostItemID, int multiplyCostItemCount, int dungeonCostItemID, int dungeonCostItemCount, int currCount, int minCount = 2, int maxCount = 99)
	{
		m_MultiplyCostItemID = multiplyCostItemID;
		m_MultiplyCostItemCount = multiplyCostItemCount;
		m_DungeonCostItemID = dungeonCostItemID;
		m_DungeonCostItemCount = dungeonCostItemCount;
		m_MaxCount = maxCount;
		m_MinCount = minCount;
		m_currentCount = currCount;
		OnValueChanged(bInvokeCallback: false);
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (eventData.scrollDelta.y < 0f)
		{
			m_currentCount--;
		}
		else if (eventData.scrollDelta.y > 0f)
		{
			m_currentCount++;
		}
		OnValueChanged(bInvokeCallback: true);
	}
}
