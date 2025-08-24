using NKC.PacketHandler;
using NKC.Publisher;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupOKCancel : NKCUIBase
{
	private enum eOpenType
	{
		OK,
		OKCancel
	}

	public delegate void OnButton();

	private eOpenType m_Type;

	public Text m_lbTitle;

	public Text m_lbContent;

	public Text m_lbTimerText;

	private static NKCPopupOKCancel m_Popup;

	[Header("BG")]
	public EventTrigger m_etBG;

	public GameObject m_objNpcIllust;

	[Header("OK Box")]
	public GameObject m_objRootOkBox;

	public NKCUIComButton m_cbtnOK_OK;

	public Text m_lbBtnOK_OK;

	[Header("OK/Cancel Box")]
	public GameObject m_objRootOkCancelBox;

	public NKCUIComButton m_cbtnOKCancel_OK;

	public NKCUIComButton m_cbtnOKCancel_Cancel;

	public NKCUIComButton m_cbtnOKCancel_Red;

	public Text m_lbBtnOKCancel_OK;

	public Text m_lbBtnOKCancel_Cancel;

	public Text m_lbBtnOKCancel_Red;

	private OnButton dOnOKButton;

	private OnButton dOnCancelButton;

	private float m_endTimer;

	private string m_endTimerTextStringID = "";

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private bool m_bInitComplete;

	public override string MenuName => "확인/취소 팝업";

	public override eMenutype eUIType => eMenutype.Popup;

	public static bool isOpen()
	{
		if (m_Popup != null)
		{
			return m_Popup.IsOpen;
		}
		return false;
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

	public static void InitUI()
	{
		NKCPopupOKCancel nKCPopupOKCancel = (m_Popup = NKCUIManager.OpenUI<NKCPopupOKCancel>("NKM_UI_POPUP_BOX"));
		nKCPopupOKCancel.m_NKCUIOpenAnimator = new NKCUIOpenAnimator(nKCPopupOKCancel.gameObject);
		if (nKCPopupOKCancel.m_cbtnOK_OK != null)
		{
			nKCPopupOKCancel.m_cbtnOK_OK.PointerClick.RemoveAllListeners();
			nKCPopupOKCancel.m_cbtnOK_OK.PointerClick.AddListener(nKCPopupOKCancel.OnOK);
			NKCUtil.SetHotkey(nKCPopupOKCancel.m_cbtnOK_OK, HotkeyEventType.Confirm);
		}
		if (nKCPopupOKCancel.m_cbtnOKCancel_OK != null)
		{
			nKCPopupOKCancel.m_cbtnOKCancel_OK.PointerClick.RemoveAllListeners();
			nKCPopupOKCancel.m_cbtnOKCancel_OK.PointerClick.AddListener(nKCPopupOKCancel.OnOK);
			NKCUtil.SetHotkey(nKCPopupOKCancel.m_cbtnOKCancel_OK, HotkeyEventType.Confirm);
		}
		if (nKCPopupOKCancel.m_cbtnOKCancel_Cancel != null)
		{
			nKCPopupOKCancel.m_cbtnOKCancel_Cancel.PointerClick.RemoveAllListeners();
			nKCPopupOKCancel.m_cbtnOKCancel_Cancel.PointerClick.AddListener(nKCPopupOKCancel.OnCancel);
		}
		if (nKCPopupOKCancel.m_cbtnOKCancel_Red != null)
		{
			nKCPopupOKCancel.m_cbtnOKCancel_Red.PointerClick.RemoveAllListeners();
			nKCPopupOKCancel.m_cbtnOKCancel_Red.PointerClick.AddListener(nKCPopupOKCancel.OnOK);
			NKCUtil.SetHotkey(nKCPopupOKCancel.m_cbtnOKCancel_Red, HotkeyEventType.Confirm);
		}
		nKCPopupOKCancel.gameObject.SetActive(value: false);
		nKCPopupOKCancel.m_endTimer = 0f;
		if (nKCPopupOKCancel.m_lbTimerText != null)
		{
			NKCUtil.SetGameobjectActive(nKCPopupOKCancel.m_lbTimerText.gameObject, bValue: false);
		}
		nKCPopupOKCancel.m_bInitComplete = true;
	}

	public static void OpenOKBoxByStringID(string TitleID, string ContentID, OnButton onOkButton = null)
	{
		OpenOKBox(NKCStringTable.GetString(TitleID), NKCStringTable.GetString(ContentID), onOkButton);
	}

	public static void OpenOKBox(string Title, string Content, OnButton onOkButton = null, string OKButtonStr = "")
	{
		m_Popup.OpenOK(Title, Content, onOkButton, OKButtonStr);
	}

	public static void OpenOKBox(NKM_ERROR_CODE errorCode, OnButton onOkButton = null, string OKButtonStr = "")
	{
		m_Popup.OpenOK(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(errorCode), onOkButton, OKButtonStr);
	}

	public static void OpenOKBox(NKC_PUBLISHER_RESULT_CODE errorCode, string additionalError, OnButton onOkButton = null, string OKButtonStr = "")
	{
		m_Popup.OpenOK(NKCUtilString.GET_STRING_ERROR, NKCPublisherModule.GetErrorMessage(errorCode, additionalError), onOkButton, OKButtonStr);
	}

	public void OpenOK(string Title, string Content, OnButton onOkButton = null, string OKButtonStr = "", bool bShowNpcIllust = true)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		if (m_bOpen)
		{
			Close();
		}
		if (m_NKCUIOpenAnimator != null)
		{
			m_NKCUIOpenAnimator.PlayOpenAni();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			OnOK();
		});
		if (m_etBG != null)
		{
			m_etBG.triggers.Clear();
			m_etBG.triggers.Add(entry);
		}
		dOnOKButton = onOkButton;
		m_lbTitle.text = Title;
		m_lbContent.text = Content;
		NKCUtil.SetLabelText(m_lbTimerText, "");
		if (Content != null)
		{
			Debug.Log("OK Popup Content : " + Content);
		}
		m_objRootOkBox.SetActive(value: true);
		m_objRootOkCancelBox.SetActive(value: false);
		NKCUtil.SetGameobjectActive(m_objNpcIllust, bShowNpcIllust);
		base.gameObject.SetActive(value: true);
		if (string.IsNullOrEmpty(OKButtonStr))
		{
			m_lbBtnOK_OK.text = NKCUtilString.GET_STRING_CONFIRM_BY_ALL_SEARCH();
		}
		else
		{
			m_lbBtnOK_OK.text = OKButtonStr;
		}
		m_Type = eOpenType.OK;
		UIOpened();
	}

	public void Update()
	{
		if (!base.IsOpen)
		{
			return;
		}
		if (m_NKCUIOpenAnimator != null)
		{
			m_NKCUIOpenAnimator.Update();
		}
		if (!(m_endTimer > 0f) || !(m_lbTimerText != null))
		{
			return;
		}
		int num = (int)m_endTimer;
		m_endTimer -= Time.deltaTime;
		if (m_endTimer <= 0f)
		{
			if (dOnCancelButton != null)
			{
				dOnCancelButton();
			}
			m_endTimer = 0f;
			Close();
		}
		if (num != (int)m_endTimer)
		{
			if (!string.IsNullOrEmpty(m_endTimerTextStringID))
			{
				m_lbTimerText.text = string.Format(NKCStringTable.GetString(m_endTimerTextStringID), 1 + (int)m_endTimer);
			}
			else
			{
				m_lbTimerText.text = string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_SECONDS"), 1 + (int)m_endTimer);
			}
			if (!m_lbTimerText.gameObject.activeInHierarchy)
			{
				NKCUtil.SetGameobjectActive(m_lbTimerText.gameObject, bValue: true);
			}
		}
	}

	public static void OpenOKCancelBoxbyStringID(string TitleID, string ContentID, OnButton onOkButton, OnButton onCancelButton = null)
	{
		m_Popup.OpenOKCancel(NKCStringTable.GetString(TitleID), NKCStringTable.GetString(ContentID), onOkButton, onCancelButton);
	}

	public static void OpenOKTimerBox(string title, string content, OnButton onOkButton, float endTime, string OKButtonStr, string endTimerStringID)
	{
		m_Popup.OpenOK(title, content, onOkButton, OKButtonStr);
		m_Popup.m_endTimer = endTime;
		m_Popup.m_endTimerTextStringID = endTimerStringID;
	}

	public static void OpenOKCancelTimerBox(string Title, string Content, OnButton onOkButton, OnButton onCancelButton, float endTime, string endTimerStringID, string OKButtonStr, string CancelButtonStr)
	{
		m_Popup.OpenOKCancel(Title, Content, onOkButton, onCancelButton, OKButtonStr, CancelButtonStr);
		m_Popup.m_endTimer = endTime;
		m_Popup.m_endTimerTextStringID = endTimerStringID;
	}

	public static void OpenOKCancelBox(string Title, string Content, OnButton onOkButton, OnButton onCancelButton = null, bool bDev = false)
	{
		m_Popup.OpenOKCancel(Title, Content, onOkButton, onCancelButton);
		if (bDev)
		{
			Transform component = m_Popup.GetComponent<Transform>();
			if ((bool)component)
			{
				Transform parent = GameObject.Find("NKM_SCEN_UI_FRONT")?.transform;
				component.SetParent(parent, worldPositionStays: false);
				component.localPosition = Vector3.zero;
				component.localScale = Vector3.one;
			}
		}
	}

	public static void OpenOKCancelBox(string Title, string Content, OnButton onOkButton, OnButton onCancelButton, string OKButtonStr, string CancelButtonStr = "", bool bUseRed = false)
	{
		m_Popup.OpenOKCancel(Title, Content, onOkButton, onCancelButton, OKButtonStr, CancelButtonStr, bUseRed);
	}

	public void OpenOKCancel(string Title, string Content, OnButton onOkButton, OnButton onCancelButton = null, string OKButtonStr = "", string CancelButtonStr = "", bool bUseRedButton = false, bool bShowNpcIllust = true)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		if (m_bOpen)
		{
			Close();
		}
		Debug.Log("### OpenOKCancel");
		if (m_NKCUIOpenAnimator != null)
		{
			m_NKCUIOpenAnimator.PlayOpenAni();
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
		m_lbTitle.text = Title;
		m_lbContent.text = Content;
		NKCUtil.SetLabelText(m_lbTimerText, "");
		m_objRootOkBox.SetActive(value: false);
		m_objRootOkCancelBox.SetActive(value: true);
		NKCUtil.SetGameobjectActive(m_cbtnOKCancel_OK, !bUseRedButton);
		NKCUtil.SetGameobjectActive(m_cbtnOKCancel_Red, bUseRedButton);
		NKCUtil.SetGameobjectActive(m_objNpcIllust, bShowNpcIllust);
		base.gameObject.SetActive(value: true);
		if (string.IsNullOrEmpty(OKButtonStr))
		{
			if (bUseRedButton)
			{
				m_lbBtnOKCancel_Red.text = NKCUtilString.GET_STRING_CONFIRM_BY_ALL_SEARCH();
			}
			else
			{
				m_lbBtnOKCancel_OK.text = NKCUtilString.GET_STRING_CONFIRM_BY_ALL_SEARCH();
			}
		}
		else if (bUseRedButton)
		{
			m_lbBtnOKCancel_Red.text = OKButtonStr;
		}
		else
		{
			m_lbBtnOKCancel_OK.text = OKButtonStr;
		}
		if (string.IsNullOrEmpty(CancelButtonStr))
		{
			m_lbBtnOKCancel_Cancel.text = NKCUtilString.GET_STRING_CANCEL_BY_ALL_SEARCH();
		}
		else
		{
			m_lbBtnOKCancel_Cancel.text = CancelButtonStr;
		}
		m_Type = eOpenType.OKCancel;
		UIOpened();
	}

	public static void ClosePopupBox()
	{
		Debug.Log("### ClosePopupBox");
		m_Popup.Close();
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
		Debug.Log("### CloseInternal");
		m_endTimer = 0f;
		base.gameObject.SetActive(value: false);
	}

	public static void SetOnTop(bool bOverDevConsle = false)
	{
		if (!(m_Popup != null))
		{
			return;
		}
		Transform transform = GameObject.Find("NKM_SCEN_UI_FRONT")?.transform;
		if (transform != null)
		{
			m_Popup.transform.SetParent(transform);
			if (bOverDevConsle)
			{
				m_Popup.transform.SetAsLastSibling();
			}
			else
			{
				m_Popup.transform.SetSiblingIndex(transform.childCount - 2);
			}
		}
	}
}
