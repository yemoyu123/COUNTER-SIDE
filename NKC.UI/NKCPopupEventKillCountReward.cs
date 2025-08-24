using ClientPacket.Common;
using NKC.UI.Component;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI;

public class NKCPopupEventKillCountReward : NKCUIBase
{
	public const string UI_ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT_PF_COUNT";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_EVENT_PF_COUNT_REWARD_INFO";

	private static NKCPopupEventKillCountReward m_Instance;

	public Transform m_slotContents;

	public NKCUIComStateButton m_csbtnClose;

	public EventTrigger m_eventBG;

	private int m_iEventId;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public static NKCPopupEventKillCountReward Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventKillCountReward>("AB_UI_NKM_UI_EVENT_PF_COUNT", "NKM_UI_POPUP_EVENT_PF_COUNT_REWARD_INFO", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanUpInstance).GetInstance<NKCPopupEventKillCountReward>();
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

	public override string MenuName => "Killcount Reward";

	public override eMenutype eUIType => eMenutype.Popup;

	private static void CleanUpInstance()
	{
		m_Instance.Release();
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public void InitUI()
	{
		if (m_NKCUIOpenAnimator == null)
		{
			m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		int childCount = m_slotContents.childCount;
		for (int i = 0; i < childCount; i++)
		{
			m_slotContents.GetChild(i).GetComponent<NKCUIComKillCountRewardSlot>()?.Init();
		}
		if (m_eventBG != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				Close();
			});
			m_eventBG.triggers.Clear();
			m_eventBG.triggers.Add(entry);
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open(int eventId)
	{
		m_iEventId = eventId;
		SetData();
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	public void SetData()
	{
		NKMKillCountTemplet nKMKillCountTemplet = NKMKillCountTemplet.Find(m_iEventId);
		if (nKMKillCountTemplet == null || !(m_slotContents != null))
		{
			return;
		}
		long currentServerKillCount = 0L;
		NKMServerKillCountData killCountServerData = NKCKillCountManager.GetKillCountServerData(m_iEventId);
		if (killCountServerData != null)
		{
			currentServerKillCount = killCountServerData.killCount;
		}
		NKMKillCountData killCountData = NKCKillCountManager.GetKillCountData(m_iEventId);
		int maxServerStep = nKMKillCountTemplet.GetMaxServerStep();
		int childCount = m_slotContents.childCount;
		int num = maxServerStep - childCount;
		for (int i = 0; i < num; i++)
		{
			NKCUIComKillCountRewardSlot component = Object.Instantiate(m_slotContents.GetChild(0).gameObject, m_slotContents).GetComponent<NKCUIComKillCountRewardSlot>();
			if (component != null)
			{
				component.Init();
			}
		}
		childCount = m_slotContents.childCount;
		for (int j = 0; j < childCount; j++)
		{
			if (j >= maxServerStep)
			{
				NKCUtil.SetGameobjectActive(m_slotContents.gameObject, bValue: false);
				continue;
			}
			NKCUIComKillCountRewardSlot component2 = m_slotContents.GetChild(j).GetComponent<NKCUIComKillCountRewardSlot>();
			NKMKillCountStepTemplet result = null;
			nKMKillCountTemplet.TryGetServerStep(j + 1, out result);
			if (component2 == null || result == null)
			{
				NKCUtil.SetGameobjectActive(component2.gameObject, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(component2.gameObject, bValue: true);
			component2.SetData(m_iEventId, result, killCountData, currentServerKillCount);
		}
	}

	public void Release()
	{
		m_NKCUIOpenAnimator = null;
		m_eventBG = null;
	}
}
