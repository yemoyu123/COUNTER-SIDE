using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSkip : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	public delegate void OnCountUpdated(int count);

	public GameObject m_objUnlock;

	public GameObject m_objLock;

	public NKCUIItemCostSlot m_NKCUIItemCostSlot;

	public Text m_lbCount;

	public NKCUIComStateButton m_csbtnUp;

	public NKCUIComStateButton m_csbtnDown;

	public NKCUIComStateButton m_csbtnInfo;

	public NKCUIComStateButton m_csbtnClose;

	public Text m_lbUseDeckType;

	private UnityAction m_dOnClickEventClose;

	private OnCountUpdated m_dOnCountUpdated;

	private int m_SkipCostItemID;

	private int m_SkipCostItemCount = 1;

	private int m_DungeonCostItemID;

	private int m_DungeonCostItemCount = 1;

	private int m_currentCount;

	public const int MAX_COUNT_MULTIPLY_AND_SKIP = 99;

	private int m_MaxCount = 99;

	private int m_MinCount = 1;

	private bool m_bWasHold;

	public int CurrentCount => m_currentCount;

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
		if (m_csbtnInfo != null)
		{
			m_csbtnInfo.PointerClick.RemoveAllListeners();
			m_csbtnInfo.PointerClick.AddListener(OnClickInfo);
		}
		if (m_csbtnClose != null)
		{
			m_csbtnClose.PointerClick.RemoveAllListeners();
			m_csbtnClose.PointerClick.AddListener(OnClickClose);
		}
	}

	public void SetData(NKMStageTempletV2 stageTemplet, int currCount, bool bShowUseDeckTypeNotice)
	{
		int a = 99;
		int eternium = stageTemplet?.m_StageReqItemCount ?? 1;
		int num = stageTemplet?.m_StageReqItemID ?? 0;
		if (stageTemplet != null)
		{
			if (stageTemplet.m_StageReqItemID == 2)
			{
				if (stageTemplet.WarfareTemplet != null)
				{
					NKCCompanyBuff.SetDiscountOfEterniumInEnteringWarfare(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
				}
				else if (stageTemplet.DungeonTempletBase != null)
				{
					NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
				}
				else if (stageTemplet.PhaseTemplet != null)
				{
					NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
				}
			}
			if (stageTemplet.EnterLimit > 0)
			{
				int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(stageTemplet.Key);
				a = stageTemplet.EnterLimit - statePlayCnt;
			}
			if (stageTemplet.IsUsingEventDeck())
			{
				NKCUtil.SetLabelText(m_lbUseDeckType, NKCStringTable.GetString("SI_PF_OPERATION_SKIP_EVENTDECK_INFO"));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbUseDeckType, NKCStringTable.GetString("SI_PF_OPERATION_SKIP_INFO"));
			}
		}
		int b = 1;
		if (eternium > 0)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null)
			{
				b = (int)myUserData.m_InventoryData.GetCountMiscItem(num) / eternium;
			}
		}
		a = Mathf.Min(a, b);
		SetData(NKMCommonConst.SkipCostMiscItemId, NKMCommonConst.SkipCostMiscItemCount, num, eternium, currCount, 1, a);
		if (bShowUseDeckTypeNotice)
		{
			NKCUtil.SetGameobjectActive(m_lbUseDeckType, stageTemplet != null);
		}
	}

	public void SetData(int skipCostItemID, int skipCostItemCount, int dungeonCostItemID, int dungeonCostItemCount, int currCount, int minCount = 1, int maxCount = 99)
	{
		m_SkipCostItemID = skipCostItemID;
		m_SkipCostItemCount = skipCostItemCount;
		m_DungeonCostItemID = dungeonCostItemID;
		m_DungeonCostItemCount = dungeonCostItemCount;
		m_MaxCount = maxCount;
		m_MinCount = minCount;
		m_currentCount = currCount;
		OnValueChanged(bInvokeCallback: false);
		NKCUtil.SetGameobjectActive(m_lbUseDeckType, bValue: false);
	}

	public void Close()
	{
		OnClickClose();
	}

	private void SetLockUI(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objUnlock, !bSet);
		NKCUtil.SetGameobjectActive(m_objLock, bSet);
	}

	private int GetMaxCount()
	{
		int num = 99;
		if (m_DungeonCostItemID != 0 && m_DungeonCostItemCount != 0)
		{
			num = (int)(NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_DungeonCostItemID) / m_DungeonCostItemCount);
		}
		int num2 = 99;
		if (m_SkipCostItemID != 0 && m_SkipCostItemCount != 0)
		{
			num2 = (int)(NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_SkipCostItemID) / m_SkipCostItemCount);
		}
		return Mathf.Min(99, m_MaxCount, num, num2);
	}

	private void OnClickUp()
	{
		m_currentCount++;
		OnValueChanged(bInvokeCallback: true);
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

	private void OnHoldDown()
	{
		m_bWasHold = true;
		m_currentCount--;
		OnValueChanged(bInvokeCallback: true);
	}

	private void OnValueChanged(bool bInvokeCallback)
	{
		m_currentCount = Mathf.Clamp(m_currentCount, m_MinCount, GetMaxCount());
		if (m_NKCUIItemCostSlot != null)
		{
			m_NKCUIItemCostSlot.SetData(m_SkipCostItemID, m_SkipCostItemCount * m_currentCount, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_SkipCostItemID));
		}
		NKCUtil.SetLabelText(m_lbCount, m_currentCount.ToString());
		if (bInvokeCallback)
		{
			m_dOnCountUpdated?.Invoke(m_currentCount);
		}
	}

	private void OnClickInfo()
	{
		NKCUIPopUpGuide.Instance.Open("ARTICLE_SYSTEM_SUPPORT_BATTLE", 2);
	}

	private void OnClickClose()
	{
		if (m_dOnClickEventClose != null)
		{
			m_dOnClickEventClose();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
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
