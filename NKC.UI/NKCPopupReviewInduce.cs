using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupReviewInduce : NKCUIBase
{
	private enum eOpenType
	{
		OK,
		OKCancel
	}

	public delegate void OnButton();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_REVIEW_INDUCE";

	private static NKCPopupReviewInduce m_Instance;

	private eOpenType m_Type;

	[Header("BG")]
	public EventTrigger m_etBG;

	[Header("OK/Cancel Box")]
	public NKCUIComButton m_cbtnOKCancel_OK;

	public NKCUIComButton m_cbtnOKCancel_Cancel;

	public Text m_lbBtnOKCancel_OK;

	public Text m_lbBtnOKCancel_Cancel;

	private OnButton dOnOKButton;

	private OnButton dOnCancelButton;

	private bool m_bInitComplete;

	public static NKCPopupReviewInduce Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUISlotListViewer>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_REVIEW_INDUCE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupReviewInduce>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override string MenuName => "리뷰 확인/취소 팝업";

	public override eMenutype eUIType => eMenutype.Popup;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void OnBackButton()
	{
		switch (m_Type)
		{
		case eOpenType.OK:
			OnOK();
			break;
		case eOpenType.OKCancel:
			OnCancel();
			break;
		}
	}

	private void InitUI()
	{
		if (m_cbtnOKCancel_OK != null)
		{
			m_cbtnOKCancel_OK.PointerClick.RemoveAllListeners();
			m_cbtnOKCancel_OK.PointerClick.AddListener(OnOK);
		}
		if (m_cbtnOKCancel_Cancel != null)
		{
			m_cbtnOKCancel_Cancel.PointerClick.RemoveAllListeners();
			m_cbtnOKCancel_Cancel.PointerClick.AddListener(OnCancel);
		}
		base.gameObject.SetActive(value: false);
		m_bInitComplete = true;
	}

	public void OpenOKCancel(OnButton onOkButton, OnButton onCancelButton = null)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			OnCancel();
		});
		if (m_etBG != null)
		{
			m_etBG.triggers.Clear();
			m_etBG.triggers.Add(entry);
		}
		dOnOKButton = onOkButton;
		dOnCancelButton = onCancelButton;
		base.gameObject.SetActive(value: true);
		m_Type = eOpenType.OKCancel;
		UIOpened();
	}

	public void ClosePopupBox()
	{
		Close();
	}

	public void OnOK()
	{
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

	public void SetOnTop(bool bOverDevConsle = false)
	{
		Transform transform = GameObject.Find("NKM_SCEN_UI_FRONT")?.transform;
		if (transform != null)
		{
			base.gameObject.transform.SetParent(transform);
			if (bOverDevConsle)
			{
				base.gameObject.transform.SetAsLastSibling();
			}
			else
			{
				base.gameObject.transform.SetSiblingIndex(transform.childCount - 2);
			}
		}
	}
}
