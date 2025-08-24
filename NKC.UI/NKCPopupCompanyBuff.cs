using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupCompanyBuff : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_EVENTBUFF";

	private static NKCPopupCompanyBuff m_Instance;

	public GameObject m_objBG;

	public NKCUIComStateButton m_btnClose;

	public LoopScrollFlexibleRect m_loopScoll;

	public Transform m_trContent;

	private List<NKMCompanyBuffData> m_lstBuff = new List<NKMCompanyBuffData>();

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public static NKCPopupCompanyBuff Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupCompanyBuff>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_EVENTBUFF", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupCompanyBuff>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback = new EventTrigger.TriggerEvent();
		entry.callback.AddListener(OnClickBG);
		EventTrigger eventTrigger = m_objBG.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = m_objBG.AddComponent<EventTrigger>();
		}
		eventTrigger.triggers.Add(entry);
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_loopScoll.dOnGetObject += GetObject;
		m_loopScoll.dOnReturnObject += ReturnObject;
		m_loopScoll.dOnProvideData += ProvideData;
		m_loopScoll.ContentConstraintCount = 1;
		m_loopScoll.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loopScoll);
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
	}

	public void Open()
	{
		RefreshList(bOpen: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private int Compare(NKMCompanyBuffData buffA, NKMCompanyBuffData buffB)
	{
		return buffA.ExpireTicks.CompareTo(buffB.ExpireTicks);
	}

	private void RefreshList(bool bOpen = false)
	{
		IReadOnlyList<NKMCompanyBuffData> companyBuffDataList = NKCScenManager.CurrentUserData().m_companyBuffDataList;
		m_lstBuff = new List<NKMCompanyBuffData>();
		m_lstBuff.AddRange(companyBuffDataList);
		m_lstBuff.Sort(Compare);
		if (!bOpen && m_lstBuff.Count == 0)
		{
			Close();
			return;
		}
		m_loopScoll.TotalCount = m_lstBuff.Count;
		m_loopScoll.RefreshCells(bForce: true);
	}

	private void OnClickBG(BaseEventData eventData)
	{
		Close();
	}

	public void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	private RectTransform GetObject(int index)
	{
		NKCPopupCompanyBuffSlot newInstance = NKCPopupCompanyBuffSlot.GetNewInstance(m_trContent);
		if (newInstance == null)
		{
			return null;
		}
		newInstance.gameObject.transform.localPosition = Vector3.zero;
		newInstance.gameObject.transform.localScale = Vector3.one;
		return newInstance.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCPopupCompanyBuffSlot component = tr.GetComponent<NKCPopupCompanyBuffSlot>();
		tr.SetParent(base.transform);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	private void ProvideData(Transform transform, int idx)
	{
		if (idx < 0)
		{
			NKCUtil.SetGameobjectActive(transform, bValue: false);
			return;
		}
		if (idx >= m_lstBuff.Count)
		{
			NKCUtil.SetGameobjectActive(transform, bValue: false);
			return;
		}
		NKCPopupCompanyBuffSlot component = transform.GetComponent<NKCPopupCompanyBuffSlot>();
		if (component != null)
		{
			component.SetData(m_lstBuff[idx], RefreshList);
		}
	}
}
