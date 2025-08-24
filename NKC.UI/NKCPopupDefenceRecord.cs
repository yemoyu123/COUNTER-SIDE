using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI;

public class NKCPopupDefenceRecord : NKCUIBase
{
	public delegate void OnClose();

	private const string BUNDLE_NAME = "AB_UI_NKM_UI_RESULT";

	private const string ASSET_NAME = "NKM_UI_WARFARE_RESULT_DEF_RECORD";

	private static NKCPopupDefenceRecord m_Instance;

	public EventTrigger m_eventTrigger;

	public TMP_Text m_lbBestScore;

	public TMP_Text m_lbCurScore;

	public GameObject m_objNewRecord;

	private OnClose dOnClose;

	public static NKCPopupDefenceRecord Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupDefenceRecord>("AB_UI_NKM_UI_RESULT", "NKM_UI_WARFARE_RESULT_DEF_RECORD", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupDefenceRecord>();
				m_Instance.Init();
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

	public void Init()
	{
		if (m_eventTrigger != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnTouchBG);
			m_eventTrigger.triggers.Clear();
			m_eventTrigger.triggers.Add(entry);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (dOnClose != null)
		{
			dOnClose();
		}
	}

	public void Open(long gameScore, long bestScore, OnClose onClose)
	{
		NKCUtil.SetLabelText(m_lbBestScore, string.Format(NKCUtilString.GET_STRING_DEFENCE_SCORE_DESC, bestScore));
		NKCUtil.SetLabelText(m_lbCurScore, string.Format(NKCUtilString.GET_STRING_DEFENCE_SCORE_DESC, gameScore));
		NKCUtil.SetGameobjectActive(m_objNewRecord, gameScore >= NKCDefenceDungeonManager.m_BestClearScore);
		dOnClose = onClose;
		UIOpened();
	}

	private void OnTouchBG(BaseEventData eventData)
	{
		Close();
	}
}
