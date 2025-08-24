using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUIPopupCollectionAchievement : NKCUIBase
{
	private static string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_collection";

	private static string UI_ASSET_NAME = "NKM_UI_POPUP_COLLECTION_ACHIEVEMENT";

	private static NKCUIPopupCollectionAchievement m_Instance;

	public LoopScrollRect m_LoopScrollRect;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnCompleteAll;

	public EventTrigger m_eventBG;

	private int m_iUnitId;

	private List<NKMUnitMissionStepTemplet> m_MissionStepTempletList;

	private List<NKMMissionManager.MissionStateData> m_missionStateDataList = new List<NKMMissionManager.MissionStateData>();

	public static NKCUIPopupCollectionAchievement Instance
	{
		get
		{
			if (m_Instance == null)
			{
				if (NKCCollectionManager.IsCollectionV2Active)
				{
					ASSET_BUNDLE_NAME = "ab_ui_collection";
					UI_ASSET_NAME = "AB_UI_COLLECTION_POPUP_ACHIEVEMENT";
				}
				else
				{
					ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_collection";
					UI_ASSET_NAME = "NKM_UI_POPUP_COLLECTION_ACHIEVEMENT";
				}
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupCollectionAchievement>(ASSET_BUNDLE_NAME, UI_ASSET_NAME, NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupCollectionAchievement>();
				m_Instance?.Init();
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

	public override string MenuName => "유닛 도감 미션";

	private static void CleanupInstance()
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

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnCompleteAll, OnClickCompleteAll);
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.dOnGetObject += GetMissionSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnMissionSlot;
			m_LoopScrollRect.dOnProvideData += ProvideMissionData;
			m_LoopScrollRect.ContentConstraintCount = 1;
			m_LoopScrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopScrollRect);
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

	public void Open(int unitId)
	{
		m_iUnitId = unitId;
		m_MissionStepTempletList = NKCUnitMissionManager.GetUnitMissionStepTempletList(unitId);
		base.gameObject.SetActive(value: true);
		Refresh();
		if (!base.IsOpen)
		{
			UIOpened();
		}
	}

	public void Refresh()
	{
		if (m_MissionStepTempletList == null)
		{
			return;
		}
		m_missionStateDataList.Clear();
		bool flag = false;
		int count = m_MissionStepTempletList.Count;
		for (int i = 0; i < count; i++)
		{
			NKMMissionManager.MissionStateData missionState = NKCUnitMissionManager.GetMissionState(m_iUnitId, m_MissionStepTempletList[i]);
			m_missionStateDataList.Add(missionState);
			if (missionState.state == NKMMissionManager.MissionState.CAN_COMPLETE)
			{
				flag = true;
			}
		}
		m_csbtnCompleteAll?.SetLock(!flag);
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.TotalCount = count;
			m_LoopScrollRect.SetIndexPosition(0);
		}
	}

	private RectTransform GetMissionSlot(int index)
	{
		return NKCUICollectionAchievementSlot.GetNewInstance(null)?.GetComponent<RectTransform>();
	}

	private void ReturnMissionSlot(Transform tr)
	{
		NKCUICollectionAchievementSlot component = tr.GetComponent<NKCUICollectionAchievementSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	private void ProvideMissionData(Transform tr, int index)
	{
		NKCUICollectionAchievementSlot component = tr.GetComponent<NKCUICollectionAchievementSlot>();
		if (!(component == null) && m_MissionStepTempletList != null && m_MissionStepTempletList.Count > index)
		{
			component.SetData(m_iUnitId, m_MissionStepTempletList[index], m_missionStateDataList[index], OnMissionComplete);
		}
	}

	private void OnMissionComplete(int unitId, int missionId, int stepId)
	{
		NKCPacketSender.Send_NKMPacket_UNIT_MISSION_REWARD_REQ(unitId, missionId, stepId);
	}

	private void OnClickCompleteAll()
	{
		NKCPacketSender.Send_NKMPacket_UNIT_MISSION_REWARD_ALL_REQ(m_iUnitId);
	}

	private void Release()
	{
		m_MissionStepTempletList = null;
		m_missionStateDataList?.Clear();
		m_missionStateDataList = null;
	}
}
