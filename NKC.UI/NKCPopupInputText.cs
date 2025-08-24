using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupInputText : NKCUIBase
{
	public delegate void OnButton(string str);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_INPUT_TEXT";

	private static NKCPopupInputText m_Instance;

	public Text m_lbTitle;

	public Text m_lbContent;

	public NKCComText m_lbInputKeyword;

	public GameObject m_objInputKeyword;

	[Header("BG")]
	public EventTrigger m_etBG;

	public InputField m_IFText;

	[Header("OK/Cancel Box")]
	public NKCUIComStateButton m_cbtnOKCancel_OK;

	public NKCUIComStateButton m_cbtnOKCancel_Cancel;

	public Text m_lbBtnOKCancel_OK;

	public Text m_lbBtnOKCancel_Cancel;

	private OnButton dOnOKButton;

	private OnButton dOnCancelButton;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public static NKCPopupInputText Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupInputText>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_INPUT_TEXT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupInputText>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override string MenuName => "확인/취소 팝업";

	public override eMenutype eUIType => eMenutype.Popup;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public static bool isOpen()
	{
		if (m_Instance != null)
		{
			return m_Instance.IsOpen;
		}
		return false;
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void OnBackButton()
	{
		OnCancel();
	}

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
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
		if (m_IFText != null)
		{
			m_IFText.onEndEdit.RemoveAllListeners();
			m_IFText.onEndEdit.AddListener(OnTextChanged);
		}
		NKCUtil.SetHotkey(m_cbtnOKCancel_OK, HotkeyEventType.Confirm);
		base.gameObject.SetActive(value: false);
	}

	public void Update()
	{
		if (base.IsOpen && m_NKCUIOpenAnimator != null)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public void OpenOKCancelBoxbyStringID(string TitleID, string ContentID, string GuideID, OnButton onOkButton, OnButton onCancelButton = null, int MaxCharCount = 0)
	{
		m_Instance.OpenOKCancel(NKCStringTable.GetString(TitleID), NKCStringTable.GetString(ContentID), "", NKCStringTable.GetString(GuideID), onOkButton, onCancelButton, "", "", MaxCharCount);
	}

	public void OpenOKCancelBox(string Title, string Content, string InputKeyword, string Guide, OnButton onOkButton, OnButton onCancelButton = null, bool bDev = false, int MaxCharCount = 0)
	{
		m_Instance.OpenOKCancel(Title, Content, InputKeyword, Guide, onOkButton, onCancelButton, "", "", MaxCharCount);
		if (bDev)
		{
			Transform component = m_Instance.GetComponent<Transform>();
			if ((bool)component)
			{
				Transform parent = GameObject.Find("NKM_SCEN_UI_FRONT")?.transform;
				component.SetParent(parent, worldPositionStays: false);
				component.localPosition = Vector3.zero;
				component.localScale = Vector3.one;
			}
		}
	}

	public void OpenOKCancelBox(string Title, string Content, string InputKeyword, OnButton onOkButton, OnButton onCancelButton, string OKButtonStr, string CancelButtonStr = "", int MaxCharCount = 0)
	{
		m_Instance.OpenOKCancel(Title, Content, InputKeyword, "", onOkButton, onCancelButton, OKButtonStr, CancelButtonStr, MaxCharCount);
	}

	public void OpenOKCancelBox(string Title, string Content, string InputKeyword, string Guide, OnButton onOkButton, OnButton onCancelButton = null, int MaxCharCount = 0)
	{
		m_Instance.OpenOKCancel(Title, Content, InputKeyword, Guide, onOkButton, onCancelButton, "", "", MaxCharCount);
	}

	public void OpenOKCancel(string Title, string Content, string InputKeyword, string Guide, OnButton onOkButton, OnButton onCancelButton = null, string OKButtonStr = "", string CancelButtonStr = "", int MaxCharCount = 0)
	{
		if (m_NKCUIOpenAnimator != null)
		{
			m_NKCUIOpenAnimator.PlayOpenAni();
		}
		m_IFText.text = string.Empty;
		m_IFText.placeholder.GetComponent<Text>().text = Guide;
		m_IFText.characterLimit = MaxCharCount;
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
		NKCUtil.SetGameobjectActive(m_objInputKeyword, !string.IsNullOrEmpty(InputKeyword));
		m_lbInputKeyword.text = InputKeyword;
		base.gameObject.SetActive(value: true);
		if (string.IsNullOrEmpty(OKButtonStr))
		{
			m_lbBtnOKCancel_OK.text = NKCUtilString.GET_STRING_CONFIRM_BY_ALL_SEARCH();
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
			m_lbBtnOKCancel_Cancel.text = OKButtonStr;
		}
		UIOpened();
	}

	public void ClosePopupBox()
	{
		m_Instance.Close();
	}

	private void OnTextChanged(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			m_IFText.text = string.Empty;
		}
		else
		{
			m_IFText.text = NKCFilterManager.CheckBadChat(str);
		}
		if (NKCInputManager.IsChatSubmitEnter())
		{
			if (!m_cbtnOKCancel_OK.m_bLock)
			{
				OnOK();
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	public void OnOK()
	{
		Close();
		if (!string.IsNullOrEmpty(m_IFText.text))
		{
			dOnOKButton?.Invoke(m_IFText.text);
		}
	}

	public void OnCancel()
	{
		Close();
		dOnCancelButton?.Invoke(m_IFText.text);
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public static void SetOnTop(bool bOverDevConsle = false)
	{
		if (!(m_Instance != null))
		{
			return;
		}
		Transform transform = GameObject.Find("NKM_SCEN_UI_FRONT")?.transform;
		if (transform != null)
		{
			m_Instance.transform.SetParent(transform);
			if (bOverDevConsle)
			{
				m_Instance.transform.SetAsLastSibling();
			}
			else
			{
				m_Instance.transform.SetSiblingIndex(transform.childCount - 2);
			}
		}
	}
}
