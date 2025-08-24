using System;
using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUINews : NKCUIBase
{
	private const string UI_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_news";

	private const string UI_ASSET_NAME = "NKM_UI_NEWS";

	private static NKCUINews m_Instance;

	[Header("탑 메뉴")]
	public NKCUIComToggle m_tglTopMenuNews;

	public NKCUIComToggle m_tglTopMenuNotice;

	public NKCUIComStateButton m_btnClose;

	[Header("좌측 슬롯")]
	public LoopVerticalScrollRect m_loopScrollRectSlot;

	public Transform m_trSlotParent;

	public NKCUINewsSlot m_pfbNewsSlot;

	[Header("우측 화면")]
	public NKCUINewsSubUI m_subUI;

	[Header("하단 바")]
	public NKCUIComStateButton m_btnTodayClose;

	public NKCUIComStateButton m_btnShortCut;

	[Header("배경 버튼")]
	public NKCUIComStateButton m_btnBG;

	private eNewsFilterType m_eCurrentFilterType;

	private List<NKCUINewsSlot> m_lstNewsSlot = new List<NKCUINewsSlot>();

	private Stack<NKCUINewsSlot> m_stkIdleNewsSlot = new Stack<NKCUINewsSlot>();

	private List<NKCNewsTemplet> m_lstNewsData = new List<NKCNewsTemplet>();

	private List<NKCNewsTemplet> m_lstNoticeData = new List<NKCNewsTemplet>();

	private int m_currentSelectSlotKey;

	private int m_reservedSlotKey = -1;

	private bool m_bInitComplete;

	private Action m_CloseCallback;

	public static NKCUINews Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUINews>("ab_ui_nkm_ui_news", "NKM_UI_NEWS", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUINews>();
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

	public override string MenuName => NKCUtilString.GET_STRING_NEWS;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		Transform transform = base.transform.Find("AB_UI_NKM_UI_NEWS_BG2/AB_UI_NKM_UI_NEWS_CONTENT");
		m_tglTopMenuNews = transform.Find("AB_UI_NKM_UI_NEWS_TOGGLE_TOP_MENU/AB_UI_NKM_UI_NEWS_TAP_TOP_MENU_NEWS").GetComponent<NKCUIComToggle>();
		m_tglTopMenuNews.OnValueChanged.RemoveAllListeners();
		m_tglTopMenuNews.OnValueChanged.AddListener(OnTglNews);
		m_tglTopMenuNotice = transform.Find("AB_UI_NKM_UI_NEWS_TOGGLE_TOP_MENU/AB_UI_NKM_UI_NEWS_TAP_TOP_MENU_NOTICE").GetComponent<NKCUIComToggle>();
		m_tglTopMenuNotice.OnValueChanged.RemoveAllListeners();
		m_tglTopMenuNotice.OnValueChanged.AddListener(OnTglNotice);
		m_btnClose = transform.Find("AB_UI_NKM_UI_NEWS_BUTTON_CLOSE").GetComponent<NKCUIComStateButton>();
		m_loopScrollRectSlot = transform.Find("AB_UI_NKM_UI_NEWS_CONTENT_SCROLL_VIEW_BANNER").GetComponent<LoopVerticalScrollRect>();
		m_trSlotParent = transform.Find("AB_UI_NKM_UI_NEWS_CONTENT_SCROLL_VIEW_BANNER/AB_UI_NKM_UI_NEWS_CONTENT_VIEWPORT_BANNER");
		m_btnTodayClose = transform.Find("AB_UI_NKM_UI_NEWS_BUTTON_TODAYCLOSE").GetComponent<NKCUIComStateButton>();
		m_btnShortCut = transform.Find("AB_UI_NKM_UI_NEWS_BUTTON_SHORTCUT").GetComponent<NKCUIComStateButton>();
		m_btnShortCut.PointerClick.RemoveAllListeners();
		m_btnShortCut.PointerClick.AddListener(OnClickShortCut);
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_btnTodayClose.PointerClick.RemoveAllListeners();
		m_btnTodayClose.PointerClick.AddListener(OnClickTodayClose);
		m_eCurrentFilterType = eNewsFilterType.NONE;
		m_loopScrollRectSlot.dOnGetObject += GetObject;
		m_loopScrollRectSlot.dOnReturnObject += ReturnObject;
		m_loopScrollRectSlot.dOnProvideData += ProvideData;
		m_loopScrollRectSlot.ContentConstraintCount = 1;
		m_loopScrollRectSlot.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loopScrollRectSlot);
		m_btnBG.PointerClick.RemoveAllListeners();
		m_btnBG.PointerClick.AddListener(base.Close);
		m_bInitComplete = true;
	}

	public void SetDataAndOpen(bool bForceRefresh = false, eNewsFilterType reservedFilterType = eNewsFilterType.NOTICE, int reservedSlotKey = -1)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		m_reservedSlotKey = reservedSlotKey;
		if (bForceRefresh || m_lstNewsData.Count == 0)
		{
			m_eCurrentFilterType = eNewsFilterType.NONE;
			NKCNewsManager.SortByFilterType(NKCSynchronizedTime.GetServerUTCTime(), out m_lstNewsData, out m_lstNoticeData);
			if (m_lstNewsData.Count == 0 && m_lstNoticeData.Count > 0)
			{
				reservedFilterType = eNewsFilterType.NOTICE;
			}
			else if (m_lstNewsData.Count > 0 && m_lstNoticeData.Count == 0)
			{
				reservedFilterType = eNewsFilterType.NEWS;
			}
		}
		UIOpened();
		OnClickTopMenu(reservedFilterType);
	}

	public override void CloseInternal()
	{
		m_eCurrentFilterType = eNewsFilterType.NONE;
		m_currentSelectSlotKey = -1;
		m_reservedSlotKey = -1;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (m_CloseCallback != null)
		{
			m_CloseCallback();
			m_CloseCallback = null;
		}
	}

	private void ShowSubUI(int slotKey)
	{
		NKCNewsTemplet newsTemplet = NKCNewsManager.GetNewsTemplet(slotKey);
		m_subUI.SetData(m_eCurrentFilterType, newsTemplet);
		if (newsTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_btnShortCut.gameObject, newsTemplet.m_ShortCutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_btnShortCut.gameObject, bValue: false);
		}
	}

	private int GetFirstSlotKey()
	{
		if (m_reservedSlotKey < 0)
		{
			if (m_eCurrentFilterType == eNewsFilterType.NEWS && m_lstNewsData.Count > 0)
			{
				return m_lstNewsData[0].Idx;
			}
			if (m_eCurrentFilterType == eNewsFilterType.NOTICE && m_lstNoticeData.Count > 0)
			{
				return m_lstNoticeData[0].Idx;
			}
			return -1;
		}
		return m_reservedSlotKey;
	}

	public void SetCloseCallback(Action closeCallback)
	{
		m_CloseCallback = closeCallback;
	}

	private void OnClickTopMenu(eNewsFilterType filterType)
	{
		if (m_eCurrentFilterType != filterType)
		{
			m_eCurrentFilterType = filterType;
			if (m_eCurrentFilterType == eNewsFilterType.NEWS)
			{
				m_loopScrollRectSlot.TotalCount = m_lstNewsData.Count;
				m_tglTopMenuNews.Select(bSelect: true, bForce: true, bImmediate: true);
			}
			else if (m_eCurrentFilterType == eNewsFilterType.NOTICE)
			{
				m_loopScrollRectSlot.TotalCount = m_lstNoticeData.Count;
				m_tglTopMenuNotice.Select(bSelect: true, bForce: true, bImmediate: true);
			}
			m_loopScrollRectSlot.RefreshCells();
			if (m_loopScrollRectSlot.TotalCount > 0)
			{
				int firstSlotKey = GetFirstSlotKey();
				m_reservedSlotKey = -1;
				OnClickSlot(firstSlotKey);
			}
			else
			{
				OnClickSlot(-1);
			}
		}
	}

	private void OnTglNews(bool bSelect)
	{
		OnClickTopMenu(eNewsFilterType.NEWS);
	}

	private void OnTglNotice(bool bSelect)
	{
		OnClickTopMenu(eNewsFilterType.NOTICE);
	}

	private void OnClickSlot(int key)
	{
		if (m_currentSelectSlotKey != key)
		{
			for (int i = 0; i < m_lstNewsSlot.Count; i++)
			{
				m_lstNewsSlot[i].Select(m_lstNewsSlot[i].GetSlotKey() == key);
			}
			m_currentSelectSlotKey = key;
			ShowSubUI(m_currentSelectSlotKey);
		}
	}

	private void OnClickTodayClose()
	{
		PlayerPrefs.SetString(NKCNewsManager.GetPreferenceString(NKCNewsManager.NKM_LOCAL_SAVE_NEXT_NEWS_POPUP_SHOW_TIME), NKMTime.GetNextResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Day).Ticks.ToString());
		Close();
	}

	private void OnClickShortCut()
	{
		NKCNewsTemplet newsTemplet = NKCNewsManager.GetNewsTemplet(m_currentSelectSlotKey);
		if (newsTemplet != null)
		{
			NKCContentManager.MoveToShortCut(newsTemplet.m_ShortCutType, newsTemplet.m_ShortCut);
			Close();
		}
	}

	private RectTransform GetObject(int index)
	{
		if (m_stkIdleNewsSlot.Count > 0)
		{
			NKCUINewsSlot nKCUINewsSlot = m_stkIdleNewsSlot.Pop();
			NKCUtil.SetGameobjectActive(nKCUINewsSlot, bValue: true);
			m_lstNewsSlot.Add(nKCUINewsSlot);
			return nKCUINewsSlot.GetComponent<RectTransform>();
		}
		NKCUINewsSlot nKCUINewsSlot2 = UnityEngine.Object.Instantiate(m_pfbNewsSlot);
		nKCUINewsSlot2.InitUI();
		nKCUINewsSlot2.transform.localScale = Vector3.one;
		nKCUINewsSlot2.transform.localPosition = Vector3.zero;
		nKCUINewsSlot2.gameObject.AddComponent<CanvasGroup>();
		NKCUtil.SetGameobjectActive(nKCUINewsSlot2, bValue: true);
		m_lstNewsSlot.Add(nKCUINewsSlot2);
		return nKCUINewsSlot2.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(base.transform);
		NKCUINewsSlot component = go.GetComponent<NKCUINewsSlot>();
		if (m_lstNewsSlot.Contains(component))
		{
			m_lstNewsSlot.Remove(component);
		}
		if (!m_stkIdleNewsSlot.Contains(component))
		{
			m_stkIdleNewsSlot.Push(component);
		}
	}

	private void ProvideData(Transform transform, int idx)
	{
		NKCUINewsSlot component = transform.GetComponent<NKCUINewsSlot>();
		if (m_eCurrentFilterType == eNewsFilterType.NEWS)
		{
			if (m_lstNewsData.Count > idx)
			{
				component.SetData(m_lstNewsData[idx], OnClickSlot, SetDataAndOpen);
			}
			else
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
		}
		else if (m_eCurrentFilterType == eNewsFilterType.NOTICE)
		{
			if (m_lstNoticeData.Count > idx)
			{
				component.SetData(m_lstNoticeData[idx], OnClickSlot, SetDataAndOpen);
			}
			else
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
		}
	}
}
