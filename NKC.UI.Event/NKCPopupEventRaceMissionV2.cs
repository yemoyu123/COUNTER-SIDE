using System.Collections.Generic;
using NKC.Templet;
using NKM;
using NKM.Templet;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventRaceMissionV2 : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ui_single_race";

	private const string UI_ASSET_NAME = "UI_POPUP_SINGLE_RACE_MISSION";

	private static NKCPopupEventRaceMissionV2 m_Instance;

	public ScrollRect m_scrollRect;

	public EventTrigger m_eventTriggerBg;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnAllComplete;

	private List<NKCUIMissionAchieveSlot> m_missionSlotList = new List<NKCUIMissionAchieveSlot>();

	private int m_iEventRaceTempletKey;

	private List<NKMMissionTemplet> m_lstCompleteMission = new List<NKMMissionTemplet>();

	public static NKCPopupEventRaceMissionV2 Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventRaceMissionV2>("ui_single_race", "UI_POPUP_SINGLE_RACE_MISSION", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventRaceMissionV2>();
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

	public override string MenuName => "RACE MISSION";

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
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnAllComplete, OnClickAllComplete);
		NKCUtil.SetScrollHotKey(m_scrollRect);
		m_missionSlotList.Clear();
		if (m_scrollRect != null)
		{
			int childCount = m_scrollRect.content.childCount;
			for (int num = 0; num < childCount; num++)
			{
				NKCUIMissionAchieveSlot component = m_scrollRect.content.GetChild(num).GetComponent<NKCUIMissionAchieveSlot>();
				component.Init();
				m_missionSlotList.Add(component);
			}
		}
		base.gameObject.SetActive(value: false);
	}

	public void Open(int raceKey)
	{
		base.gameObject.SetActive(value: true);
		SetMissionData(raceKey);
		m_iEventRaceTempletKey = raceKey;
		UIOpened();
	}

	public bool GetMissionRedDotState(int raceKey)
	{
		SetMissionData(raceKey);
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		int count = m_missionSlotList.Count;
		for (int i = 0; i < count && m_missionSlotList[i].IsActive(); i++)
		{
			NKMMissionTemplet nKMMissionTemplet = m_missionSlotList[i].GetNKMMissionTemplet();
			NKMMissionData nKMMissionData = m_missionSlotList[i].GetNKMMissionData();
			if (NKMMissionManager.CanComplete(nKMMissionTemplet, myUserData, nKMMissionData) == NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
		}
		return false;
	}

	private void OnClickMissionMove(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (!(cNKCUIMissionAchieveSlot == null))
		{
			NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
			if (nKMMissionTemplet != null)
			{
				NKCContentManager.MoveToShortCut(nKMMissionTemplet.m_ShortCutType, nKMMissionTemplet.m_ShortCut);
			}
		}
	}

	private void OnClickMissionComplete(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (!(cNKCUIMissionAchieveSlot == null))
		{
			NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
			if (nKMMissionTemplet != null)
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(nKMMissionTemplet);
			}
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		m_scrollRect = null;
		m_eventTriggerBg = null;
		m_csbtnClose = null;
		if (m_missionSlotList != null)
		{
			m_missionSlotList.Clear();
			m_missionSlotList = null;
		}
	}

	private void SetMissionData(int raceKey)
	{
		NKMEventRaceTemplet nKMEventRaceTemplet = NKMEventRaceTemplet.Find(raceKey);
		if (nKMEventRaceTemplet == null || nKMEventRaceTemplet.RaceMissionID.Count <= 0)
		{
			return;
		}
		NKCEventMissionTemplet nKCEventMissionTemplet = NKCEventMissionTemplet.Find(nKMEventRaceTemplet.RaceMissionID[0]);
		if (nKCEventMissionTemplet == null)
		{
			return;
		}
		m_lstCompleteMission.Clear();
		m_csbtnAllComplete.Lock();
		int count = nKCEventMissionTemplet.m_lstMissionTab.Count;
		for (int i = 0; i < count; i++)
		{
			List<NKMMissionTemplet> missionTempletListByType = NKMMissionManager.GetMissionTempletListByType(nKCEventMissionTemplet.m_lstMissionTab[i]);
			missionTempletListByType.Sort(NKMMissionManager.Comparer);
			int count2 = m_missionSlotList.Count;
			int count3 = missionTempletListByType.Count;
			for (int j = 0; j < count2; j++)
			{
				if (count3 <= j)
				{
					m_missionSlotList[j].SetActive(bSet: false);
					continue;
				}
				m_missionSlotList[j].SetData(missionTempletListByType[j], OnClickMissionMove, OnClickMissionComplete);
				m_missionSlotList[j].SetActive(bSet: true);
			}
			if (count2 < count3)
			{
				for (int k = count2; k < count3; k++)
				{
					NKCUIMissionAchieveSlot newInstance = NKCUIMissionAchieveSlot.GetNewInstance(m_scrollRect.content, "ui_single_race", "UI_POPUP_SINGLE_RACE_MISSION_SLOT");
					if (newInstance != null)
					{
						m_missionSlotList.Add(newInstance);
						newInstance.Init();
						m_missionSlotList[k].SetData(missionTempletListByType[k], OnClickMissionMove, OnClickMissionComplete);
						newInstance.SetData(missionTempletListByType[k], OnClickMissionMove, OnClickMissionComplete);
					}
				}
			}
			foreach (NKMMissionTemplet item in missionTempletListByType)
			{
				if (item != null && NKMMissionManager.GetMissionStateData(item).IsMissionCanClear)
				{
					m_lstCompleteMission.Add(item);
				}
			}
		}
		if (m_lstCompleteMission.Count > 0)
		{
			m_csbtnAllComplete.UnLock();
		}
	}

	private void OnClickAllComplete()
	{
		if (m_lstCompleteMission.Count > 0)
		{
			NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_ALL_REQ(m_lstCompleteMission[0].m_MissionTabId);
		}
	}

	public override void OnMissionUpdated()
	{
		SetMissionData(m_iEventRaceTempletKey);
	}
}
