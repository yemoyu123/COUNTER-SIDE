using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupMessageServer : NKCUIBase
{
	public enum eMessageStyle
	{
		Slide,
		FadeInOut,
		Typing
	}

	private struct MsgData
	{
		public eMessageStyle style { get; private set; }

		public string msg { get; private set; }

		public int loopCnt { get; private set; }

		public MsgData(eMessageStyle newStyle, string newMsg, int cnt)
		{
			style = newStyle;
			msg = newMsg;
			loopCnt = cnt;
		}
	}

	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_MESSAGE_SERVER";

	private static NKCUIPopupMessageServer m_Instance;

	public CanvasGroup m_CanvasGroup;

	public NKCUIComStateButton m_NKM_UI_POPUP_MESSAGE_BUTTON_CLOSE;

	public Image m_NKM_UI_POPUP_MESSAGE_BG2;

	public Text m_NKM_UI_POPUP_MESSAGE_TEXT;

	public float m_fMoveSpeed = 1f;

	private Vector3 m_textOriginPos = Vector3.zero;

	private Vector3 m_textPos = Vector3.zero;

	private float m_fMaskWidth;

	private int m_iLoopCnt;

	private bool m_bPlaying;

	private Queue<MsgData> m_MessageQueue = new Queue<MsgData>();

	private float m_iWidthOffset;

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public static NKCUIPopupMessageServer Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupMessageServer>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_MESSAGE_SERVER", NKCUIManager.eUIBaseRect.UIOverlay, CleanupInstance).GetInstance<NKCUIPopupMessageServer>();
				m_Instance.Init();
				m_Instance.transform.SetParent(NKCUIManager.rectFrontCanvas, worldPositionStays: false);
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

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

	public override eMenutype eUIType => eMenutype.Overlay;

	public override string MenuName => "Message";

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
		if (m_NKM_UI_POPUP_MESSAGE_BUTTON_CLOSE != null)
		{
			m_NKM_UI_POPUP_MESSAGE_BUTTON_CLOSE.PointerClick.RemoveAllListeners();
			m_NKM_UI_POPUP_MESSAGE_BUTTON_CLOSE.PointerClick.AddListener(base.Close);
		}
		if (m_NKM_UI_POPUP_MESSAGE_TEXT != null)
		{
			m_textOriginPos = m_NKM_UI_POPUP_MESSAGE_TEXT.rectTransform.localPosition;
		}
	}

	public void Open(eMessageStyle style, string message, int loopCnt = 1)
	{
		if (m_bPlaying)
		{
			m_MessageQueue.Enqueue(new MsgData(style, message, loopCnt));
			return;
		}
		m_bPlaying = true;
		SetData(style, message, loopCnt);
		if (!base.IsOpen)
		{
			UIOpened();
		}
	}

	private void SetData(eMessageStyle style, string message, int loopCnt)
	{
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_MESSAGE_TEXT, message);
		StopAllCoroutines();
		m_fMaskWidth = m_NKM_UI_POPUP_MESSAGE_TEXT.preferredWidth;
		m_iLoopCnt = loopCnt;
		SetStartPosition(style);
	}

	private void SetStartPosition(eMessageStyle style)
	{
		m_textPos = m_textOriginPos;
		if (style == eMessageStyle.Slide)
		{
			m_iWidthOffset = (float)Screen.width * 0.6f;
			m_textPos.x += m_iWidthOffset;
		}
		m_NKM_UI_POPUP_MESSAGE_TEXT.rectTransform.localPosition = m_textPos;
	}

	private void Update()
	{
		if (m_iLoopCnt > 0)
		{
			m_textPos.x -= m_fMoveSpeed;
			m_NKM_UI_POPUP_MESSAGE_TEXT.rectTransform.localPosition = m_textPos;
			if (m_textPos.x < m_iWidthOffset && Mathf.Abs(m_textPos.x) >= m_iWidthOffset)
			{
				m_textPos.x = m_textOriginPos.x + m_iWidthOffset;
				m_iLoopCnt--;
			}
		}
		else if (m_iLoopCnt == 0 && !StartNextMessage())
		{
			Close();
		}
	}

	private bool StartNextMessage()
	{
		if (m_MessageQueue.Count == 0)
		{
			return false;
		}
		MsgData msgData = m_MessageQueue.Dequeue();
		SetData(msgData.style, msgData.msg, msgData.loopCnt);
		return true;
	}

	public override void CloseInternal()
	{
		if (!StartNextMessage())
		{
			m_bPlaying = false;
			base.gameObject.SetActive(value: false);
		}
	}
}
