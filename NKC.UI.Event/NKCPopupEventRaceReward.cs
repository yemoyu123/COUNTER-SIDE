using NKM.Templet;
using UnityEngine.EventSystems;

namespace NKC.UI.Event;

public class NKCPopupEventRaceReward : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT_PF_RACE";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_EVENT_RACE_REWARD";

	private static NKCPopupEventRaceReward m_Instance;

	public NKCUISlot m_raceWinRewardIcon;

	public NKCUISlot m_raceLoseRewardIcon;

	public NKCUISlot m_teamWinRewardIcon;

	public NKCUISlot m_teamLoseRewardIcon;

	public EventTrigger m_eventTriggerBg;

	public NKCUIComStateButton m_csbtnClose;

	private bool dataSet;

	public static NKCPopupEventRaceReward Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventRaceReward>("AB_UI_NKM_UI_EVENT_PF_RACE", "NKM_UI_POPUP_EVENT_RACE_REWARD", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventRaceReward>();
				m_Instance.InitUI();
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

	public override string MenuName => "RACE REWARD";

	public override eMenutype eUIType => eMenutype.Popup;

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

	public void InitUI()
	{
		m_raceWinRewardIcon.Init();
		m_raceLoseRewardIcon.Init();
		m_teamWinRewardIcon.Init();
		m_teamLoseRewardIcon.Init();
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		if (m_eventTriggerBg != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_eventTriggerBg.triggers.Add(entry);
		}
	}

	public void Open(int eventId)
	{
		NKMEventRaceTemplet nKMEventRaceTemplet = NKMEventRaceTemplet.Find(eventId);
		if (!dataSet && nKMEventRaceTemplet != null)
		{
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_MISC, nKMEventRaceTemplet.RaceItemId, 0);
			m_raceWinRewardIcon?.SetData(data);
			dataSet = true;
		}
		base.gameObject.SetActive(value: true);
		UIOpened();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		m_raceWinRewardIcon = null;
		m_raceLoseRewardIcon = null;
		m_teamWinRewardIcon = null;
		m_teamLoseRewardIcon = null;
		m_eventTriggerBg = null;
		m_csbtnClose = null;
	}
}
