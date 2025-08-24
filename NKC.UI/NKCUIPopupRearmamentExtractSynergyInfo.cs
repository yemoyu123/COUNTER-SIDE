using System;
using System.Collections.Generic;
using System.Linq;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupRearmamentExtractSynergyInfo : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_rearm";

	private const string UI_ASSET_NAME = "AB_UI_POPUP_REARM_RECORD_SYNERGY";

	private static NKCUIPopupRearmamentExtractSynergyInfo m_Instance;

	[Header("시너지 증가 확률")]
	public Text m_lbAwakePercent;

	public Text m_lbSSRPercent;

	public Text m_lbSRPercent;

	[Header("스크롤")]
	public LoopVerticalScrollRect m_LoopScroll;

	[Header("슬롯")]
	public NKCUIRearmamentExtractSynergySlot m_pfbSynergySlot;

	[Header("버튼")]
	public NKCUIComStateButton m_csbtnOK;

	[Header("BG")]
	public EventTrigger m_evtPanel;

	private List<Tuple<int, MiscItemUnit>> m_lstSynergyItems;

	private List<NKCUIRearmamentExtractSynergySlot> m_lstSynergySlots = new List<NKCUIRearmamentExtractSynergySlot>();

	public static NKCUIPopupRearmamentExtractSynergyInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupRearmamentExtractSynergyInfo>("ab_ui_rearm", "AB_UI_POPUP_REARM_RECORD_SYNERGY", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupRearmamentExtractSynergyInfo>();
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

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

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
		NKCUtil.SetButtonClickDelegate(m_csbtnOK, base.Close);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		if (m_evtPanel != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnEventPanelClick);
			m_evtPanel.triggers.Add(entry);
		}
		if (m_LoopScroll != null)
		{
			m_LoopScroll.dOnGetObject += GetSlot;
			m_LoopScroll.dOnReturnObject += ReturnSlot;
			m_LoopScroll.dOnProvideData += ProvideSlot;
			m_LoopScroll.ContentConstraintCount = 1;
			m_LoopScroll.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopScroll);
		}
	}

	private void Clear()
	{
		for (int i = 0; i < m_lstSynergySlots.Count; i++)
		{
			UnityEngine.Object.Destroy(m_lstSynergySlots[i].gameObject);
		}
		m_lstSynergySlots.Clear();
	}

	private void OnEventPanelClick(BaseEventData e)
	{
		Close();
	}

	public void Open()
	{
		NKCUtil.SetLabelText(m_lbAwakePercent, $"{NKMCommonConst.ExtractBonusRatePercent_Awaken}%");
		NKCUtil.SetLabelText(m_lbSSRPercent, $"{NKMCommonConst.ExtractBonusRatePercent_SSR}%");
		NKCUtil.SetLabelText(m_lbSRPercent, $"{NKMCommonConst.ExtractBonusRatePercent_SR}%");
		m_lstSynergyItems = NKMUnitExtractBonuseTemplet.Instance.Datas.ToList();
		m_LoopScroll.TotalCount = m_lstSynergyItems.Count;
		m_LoopScroll.PrepareCells();
		m_LoopScroll.RefreshCells(bForce: true);
		UIOpened();
	}

	private RectTransform GetSlot(int index)
	{
		NKCUIRearmamentExtractSynergySlot nKCUIRearmamentExtractSynergySlot = UnityEngine.Object.Instantiate(m_pfbSynergySlot);
		nKCUIRearmamentExtractSynergySlot.transform.localScale = Vector3.one;
		m_lstSynergySlots.Add(nKCUIRearmamentExtractSynergySlot);
		return nKCUIRearmamentExtractSynergySlot.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		go.GetComponent<NKCUIRearmamentExtractSynergySlot>();
		NKCUtil.SetGameobjectActive(go, bValue: false);
	}

	private void ProvideSlot(Transform tr, int idx)
	{
		NKCUIRearmamentExtractSynergySlot component = tr.GetComponent<NKCUIRearmamentExtractSynergySlot>();
		if (component != null)
		{
			if (m_lstSynergyItems.Count <= idx || idx < 0)
			{
				Debug.LogError($"m_lstUnitSlot - 잘못된 인덱스 입니다, {idx}");
				return;
			}
			tr.SetParent(m_LoopScroll.content);
			MiscItemUnit item = m_lstSynergyItems[idx].Item2;
			component.SetData(item.ItemId, (int)item.Count, m_lstSynergyItems[idx].Item1);
			NKCUtil.SetGameobjectActive(tr.gameObject, bValue: true);
		}
	}
}
