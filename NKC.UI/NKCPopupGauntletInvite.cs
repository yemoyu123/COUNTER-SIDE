using ClientPacket.Common;
using ClientPacket.Pvp;
using NKC.UI.Gauntlet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupGauntletInvite : NKCUIBase
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

	public NKCUIGauntletFriendSlot m_gauntletFriendSlot;

	public NKCUIGauntletPrivateRoomCustomOption m_customOption;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	private static NKCPopupGauntletInvite m_Instance;

	[Header("BG")]
	public EventTrigger m_etBG;

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

	private static NKCPopupGauntletInvite m_Popup
	{
		get
		{
			if (m_Instance == null)
			{
				m_loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupGauntletInvite>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_POPUP_GAUNTLET_INVITE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance);
				if (m_loadedUIData != null)
				{
					m_Instance = m_loadedUIData.GetInstance<NKCPopupGauntletInvite>();
				}
				InitUI();
			}
			return m_Instance;
		}
	}

	public static bool isOpen()
	{
		if (m_Popup != null)
		{
			return m_Popup.IsOpen;
		}
		return false;
	}

	public static void CheckInstanceAndClose()
	{
		m_loadedUIData?.CloseInstance();
		m_loadedUIData = null;
	}

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

	public static void InitUI()
	{
		m_Instance.m_NKCUIOpenAnimator = new NKCUIOpenAnimator(m_Instance.gameObject);
		if (m_Instance.m_cbtnOK_OK != null)
		{
			m_Instance.m_cbtnOK_OK.PointerClick.RemoveAllListeners();
			m_Instance.m_cbtnOK_OK.PointerClick.AddListener(m_Instance.OnOK);
			NKCUtil.SetHotkey(m_Instance.m_cbtnOK_OK, HotkeyEventType.Confirm);
		}
		if (m_Instance.m_cbtnOKCancel_OK != null)
		{
			m_Instance.m_cbtnOKCancel_OK.PointerClick.RemoveAllListeners();
			m_Instance.m_cbtnOKCancel_OK.PointerClick.AddListener(m_Instance.OnOK);
			NKCUtil.SetHotkey(m_Instance.m_cbtnOKCancel_OK, HotkeyEventType.Confirm);
		}
		if (m_Instance.m_cbtnOKCancel_Cancel != null)
		{
			m_Instance.m_cbtnOKCancel_Cancel.PointerClick.RemoveAllListeners();
			m_Instance.m_cbtnOKCancel_Cancel.PointerClick.AddListener(m_Instance.OnCancel);
		}
		if (m_Instance.m_cbtnOKCancel_Red != null)
		{
			m_Instance.m_cbtnOKCancel_Red.PointerClick.RemoveAllListeners();
			m_Instance.m_cbtnOKCancel_Red.PointerClick.AddListener(m_Instance.OnOK);
			NKCUtil.SetHotkey(m_Instance.m_cbtnOKCancel_Red, HotkeyEventType.Confirm);
		}
		m_Instance.gameObject.SetActive(value: false);
		m_Instance.m_endTimer = 0f;
		if (m_Instance.m_lbTimerText != null)
		{
			NKCUtil.SetGameobjectActive(m_Instance.m_lbTimerText.gameObject, bValue: false);
		}
		m_Instance.m_customOption?.Init();
		m_Instance.m_customOption.ProhibitToggle = true;
		m_Instance.m_bInitComplete = true;
	}

	private void OpenOK(string Title, string Content, OnButton onOkButton = null, string OKButtonStr = "")
	{
		if (m_bOpen)
		{
			Close();
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

	public static void OpenOKTimerBox(string title, string content, OnButton onOkButton, float endTime, string OKButtonStr, string endTimerStringID, FriendListData friendListData, NKMPrivateGameConfig privateGameConfig)
	{
		m_Popup.OpenOK(title, content, onOkButton, OKButtonStr);
		m_Popup.m_endTimer = endTime;
		m_Popup.m_endTimerTextStringID = endTimerStringID;
		m_Popup.m_customOption.SetOption(privateGameConfig);
		m_Popup.m_gauntletFriendSlot.SetUI(friendListData, showTimeAndButtons: false);
	}

	public static void OpenOKTimerBox(string title, string content, OnButton onOkButton, float endTime, string OKButtonStr, string endTimerStringID, NKMUserProfileData userProfileData)
	{
		m_Popup.OpenOK(title, content, onOkButton, OKButtonStr);
		m_Popup.m_endTimer = endTime;
		m_Popup.m_endTimerTextStringID = endTimerStringID;
		m_Popup.m_gauntletFriendSlot.SetUI(userProfileData);
	}

	public static void OpenOKCancelTimerBox(string Title, string Content, OnButton onOkButton, OnButton onCancelButton, float endTime, string endTimerStringID, string OKButtonStr, string CancelButtonStr, NKMUserProfileData userProfileData, NKMPrivateGameConfig privateGameConfig)
	{
		m_Popup.OpenOKCancel(Title, Content, onOkButton, onCancelButton, OKButtonStr, CancelButtonStr);
		m_Popup.m_endTimer = endTime;
		m_Popup.m_endTimerTextStringID = endTimerStringID;
		m_Popup.m_customOption.SetOption(privateGameConfig);
		m_Popup.m_gauntletFriendSlot.SetUI(userProfileData);
	}

	private void OpenOKCancel(string Title, string Content, OnButton onOkButton, OnButton onCancelButton = null, string OKButtonStr = "", string CancelButtonStr = "", bool bUseRedButton = false)
	{
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
		if (!(m_Instance == null))
		{
			if (isOpen())
			{
				m_Popup.Close();
			}
			CheckInstanceAndClose();
		}
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
