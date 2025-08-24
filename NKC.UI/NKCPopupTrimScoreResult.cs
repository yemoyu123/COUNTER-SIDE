using ClientPacket.Mode;
using Cs.Logging;
using NKC.UI.Trim;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupTrimScoreResult : NKCUIBase
{
	public delegate void OnClose();

	private const string BUNDLE_NAME = "ab_ui_trim";

	private const string ASSET_NAME = "AB_UI_TRIM_RECORD_POPUP";

	private static NKCPopupTrimScoreResult m_Instance;

	public Text m_lbDungeonName;

	public Text m_lbTrimLevel;

	public Text m_lbTotalScore;

	public Text m_lbBestScore;

	public EventTrigger m_eventTrigger;

	public NKCUITrimScoreSlot[] m_trimScoreSlot;

	public GameObject m_objNewTag;

	private OnClose m_dOnClose;

	public static NKCPopupTrimScoreResult Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupTrimScoreResult>("ab_ui_trim", "AB_UI_TRIM_RECORD_POPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupTrimScoreResult>();
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

	public override string MenuName => "TRIM SCORE RESULT";

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

	private void Init()
	{
		if (m_eventTrigger != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnTouchBG);
			m_eventTrigger.triggers.Clear();
			m_eventTrigger.triggers.Add(entry);
		}
		base.gameObject.SetActive(value: false);
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		if (m_dOnClose != null)
		{
			m_dOnClose();
		}
	}

	public void Open(TrimModeState trimModeState, int totalScore, int bestScore, OnClose onClose = null)
	{
		if (trimModeState == null)
		{
			return;
		}
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(trimModeState.trimId);
		string text = null;
		if (nKMTrimTemplet != null)
		{
			text = NKCStringTable.GetString(nKMTrimTemplet.TirmGroupName);
		}
		else
		{
			Log.Error($"PopupTrimScoreResult - Wrong TrimId (TrimModeState.trimId: {trimModeState.trimId})", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Trim/NKCPopupTrimScoreResult.cs", 98);
			text = " - ";
		}
		NKCUtil.SetLabelText(m_lbDungeonName, text);
		NKCUtil.SetLabelText(m_lbTrimLevel, trimModeState.trimLevel.ToString());
		NKCUtil.SetLabelText(m_lbTotalScore, $"{totalScore.ToString():#,0}");
		NKCUtil.SetLabelText(m_lbBestScore, $"{bestScore.ToString():#,0}");
		NKCUtil.SetGameobjectActive(m_objNewTag, totalScore >= bestScore);
		if (m_trimScoreSlot != null)
		{
			int num = 0;
			int num2 = m_trimScoreSlot.Length;
			int count = trimModeState.stageList.Count;
			for (int i = 0; i < num2; i++)
			{
				if (i < count)
				{
					m_trimScoreSlot[i].SetActive(value: true);
					m_trimScoreSlot[i].SetData(trimModeState.stageList[i]);
					num++;
				}
			}
			if (num < num2)
			{
				m_trimScoreSlot[num].SetActive(value: true);
				m_trimScoreSlot[num].SetData(trimModeState.lastClearStage);
				num++;
			}
			for (int j = num; j < num2; j++)
			{
				m_trimScoreSlot[j].SetActive(value: false);
			}
		}
		m_dOnClose = onClose;
		base.gameObject.SetActive(value: true);
		UIOpened();
	}

	private void OnTouchBG(BaseEventData eventData)
	{
		Close();
	}
}
