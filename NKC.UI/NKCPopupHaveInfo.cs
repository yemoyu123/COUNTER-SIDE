using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupHaveInfo : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_UNIT_HAVE";

	private static NKCPopupHaveInfo m_Instance;

	public LoopScrollRect m_loopScrollRect;

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnOk;

	public NKCDeckViewUnitSlot m_pfbUnitSlot;

	private List<NKMUnitData> m_lstUnitData = new List<NKMUnitData>();

	private List<NKCDeckViewUnitSlot> m_lstVisibleSlot = new List<NKCDeckViewUnitSlot>();

	private Stack<NKCDeckViewUnitSlot> m_stkUnitSlotPool = new Stack<NKCDeckViewUnitSlot>();

	public static NKCPopupHaveInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupHaveInfo>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_UNIT_HAVE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupHaveInfo>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

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
		m_lstUnitData = null;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_btnOk.PointerClick.RemoveAllListeners();
		m_btnOk.PointerClick.AddListener(base.Close);
		NKCUtil.SetHotkey(m_btnOk, HotkeyEventType.Confirm);
		m_loopScrollRect.dOnGetObject += GetObject;
		m_loopScrollRect.dOnReturnObject += ReturnObject;
		m_loopScrollRect.dOnProvideData += ProvideData;
		NKCUtil.SetScrollHotKey(m_loopScrollRect);
	}

	public void Open(int unitID)
	{
		m_lstUnitData = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitListByUnitID(unitID);
		if (m_lstUnitData.Count == 0)
		{
			Close();
			Debug.LogWarning("보유중인 유닛이 없을 경우 여기 들어오면 안됨");
			return;
		}
		m_lstUnitData.Sort(CompBreakLevel);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loopScrollRect.TotalCount = m_lstUnitData.Count;
		if (m_loopScrollRect.content.childCount == 0)
		{
			m_loopScrollRect.PrepareCells();
		}
		m_loopScrollRect.RefreshCells();
		m_loopScrollRect.SetIndexPosition(0);
		UIOpened();
	}

	public int CompBreakLevel(NKMUnitData lUnitData, NKMUnitData rUnitData)
	{
		if (lUnitData.m_LimitBreakLevel == rUnitData.m_LimitBreakLevel)
		{
			return rUnitData.m_UnitLevel.CompareTo(lUnitData.m_UnitLevel);
		}
		return rUnitData.m_LimitBreakLevel.CompareTo(lUnitData.m_LimitBreakLevel);
	}

	public RectTransform GetObject(int index)
	{
		if (m_stkUnitSlotPool.Count > 0)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_stkUnitSlotPool.Pop();
			NKCUtil.SetGameobjectActive(nKCDeckViewUnitSlot, bValue: true);
			nKCDeckViewUnitSlot.transform.localScale = Vector3.one;
			m_lstVisibleSlot.Add(nKCDeckViewUnitSlot);
			return nKCDeckViewUnitSlot.GetComponent<RectTransform>();
		}
		NKCDeckViewUnitSlot nKCDeckViewUnitSlot2 = Object.Instantiate(m_pfbUnitSlot);
		nKCDeckViewUnitSlot2.Init(0, bEnableDrag: false);
		NKCUtil.SetGameobjectActive(nKCDeckViewUnitSlot2, bValue: true);
		nKCDeckViewUnitSlot2.transform.localScale = Vector3.one;
		m_lstVisibleSlot.Add(nKCDeckViewUnitSlot2);
		return nKCDeckViewUnitSlot2.GetComponent<RectTransform>();
	}

	public void ReturnObject(Transform tr)
	{
		NKCDeckViewUnitSlot component = tr.GetComponent<NKCDeckViewUnitSlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		tr.SetParent(base.transform);
		if (component != null)
		{
			m_lstVisibleSlot.Remove(component);
			m_stkUnitSlotPool.Push(component);
		}
	}

	public void ProvideData(Transform tr, int idx)
	{
		NKCDeckViewUnitSlot component = tr.GetComponent<NKCDeckViewUnitSlot>();
		if (!(component == null) && m_lstUnitData.Count > idx)
		{
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.SetData(m_lstUnitData[idx], bEnableButton: false);
		}
	}
}
