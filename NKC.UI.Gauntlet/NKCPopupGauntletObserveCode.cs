using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletObserveCode : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_gauntlet";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_PRIVATE_ROOM_OBSERVECODE_POPUP";

	private static NKCPopupGauntletObserveCode m_Instance;

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnOk;

	public EventTrigger m_evtBG;

	public InputField m_inputCode;

	private static float m_reqTime;

	private const float m_reqCoolTime = 10f;

	public static NKCPopupGauntletObserveCode Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGauntletObserveCode>("ab_ui_nkm_ui_gauntlet", "NKM_UI_GAUNTLET_PRIVATE_ROOM_OBSERVECODE_POPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupGauntletObserveCode>();
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

	public override string MenuName => "Gautlet Observe Code";

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

	private void InitUI()
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_evtBG.triggers.Add(entry);
		NKCUtil.SetButtonClickDelegate(m_btnClose, base.Close);
		NKCUtil.SetButtonClickDelegate(m_btnOk, OnClickOK);
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		m_inputCode.text = "";
		m_btnOk.SetLock(m_reqTime > 0f);
		UIOpened();
	}

	private void Update()
	{
		if (m_reqTime > 0f && m_reqTime < Time.time)
		{
			m_reqTime = 0f;
			m_btnOk.SetLock(value: false);
		}
	}

	private void OnClickOK()
	{
		if (!string.IsNullOrEmpty(m_inputCode.text))
		{
			if (m_reqTime > 0f || m_btnOk.m_bLock)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_REQUEST_COOLDOWN_TIME), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				return;
			}
			m_reqTime = Time.time + 10f;
			NKCPacketSender.Send_NKMPacket_PVP_ROOM_ACCEPT_CODE_REQ(m_inputCode.text);
			m_btnOk.SetLock(value: true);
		}
	}
}
