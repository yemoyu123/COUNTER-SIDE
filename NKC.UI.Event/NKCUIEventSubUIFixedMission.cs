using System.Collections.Generic;
using Cs.Logging;
using NKC.Templet;
using NKM;
using NKM.Event;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventSubUIFixedMission : NKCUIEventSubUIBase
{
	[Header("일괄 완료 버튼")]
	public NKCUIComStateButton m_csbtnCompleteAll;

	[Header("이벤트 상점 이동 버튼")]
	public NKCUIComStateButton m_csbtnEventShop;

	[Header("미션 슬롯 프리팹")]
	public List<NKCUIMissionAchieveSlot> m_lstMissionSlot;

	[Header("미션 스크롤렉트")]
	public ScrollRect m_srMission;

	[Header("초기화가 되는 경우 남은시간")]
	public GameObject m_objResetRemainTime;

	public Text m_lbResetRemainTime;

	private NKCEventMissionTemplet m_EventMissionTemplet;

	private List<NKMMissionTemplet> m_lstCurrentList = new List<NKMMissionTemplet>();

	private float m_fDeltaTime;

	public override void Init()
	{
		base.Init();
		RectTransform component = GetComponent<RectTransform>();
		if (component != null)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(component);
		}
		NKCUtil.SetScrollHotKey(m_srMission);
		NKCUtil.SetGameobjectActive(m_csbtnCompleteAll, bValue: true);
		NKCUtil.SetButtonClickDelegate(m_csbtnEventShop, OnMovetoShop);
		NKCUtil.SetButtonClickDelegate(m_csbtnEventShortcut, base.OnMoveShortcut);
		NKCUtil.SetButtonClickDelegate(m_csbtnCompleteAll, OnCompleteAll);
		for (int i = 0; i < m_lstMissionSlot.Count; i++)
		{
			m_lstMissionSlot[i].Init();
		}
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		m_tabTemplet = tabTemplet;
		if (m_tabTemplet != null)
		{
			m_EventMissionTemplet = NKCEventMissionTemplet.Find(m_tabTemplet.Key);
			if (m_EventMissionTemplet != null)
			{
				SetDateLimit();
				NKCUtil.SetGameobjectActive(m_csbtnEventShop, m_EventMissionTemplet.m_ShortcutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE);
				SelectTab(m_tabTemplet.Key);
			}
		}
	}

	public override void Refresh()
	{
		SelectTab(m_tabTemplet.Key);
	}

	private void SelectTab(int tabID)
	{
		BuildMissionTempletList(tabID);
		SetMissionData();
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMUserMissionData missionData = NKCScenManager.GetScenManager().GetMyUserData().m_MissionData;
		if (missionData != null)
		{
			if (!missionData.CheckCompletableMission(myUserData, tabID))
			{
				m_csbtnCompleteAll?.Lock();
			}
			else
			{
				m_csbtnCompleteAll?.UnLock();
			}
		}
	}

	private void OnMovetoShop()
	{
		NKCContentManager.MoveToShortCut(m_EventMissionTemplet.m_ShortcutType, m_EventMissionTemplet.m_ShortcutParam);
	}

	private void OnCompleteAll()
	{
		NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_ALL_REQ(m_tabTemplet.Key);
	}

	public void OnClickMove(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (cNKCUIMissionAchieveSlot == null)
		{
			return;
		}
		NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
		if (nKMMissionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(nKMMissionTemplet.m_MissionTabId);
		if (missionTabTemplet != null)
		{
			if (NKMMissionManager.IsMissionTabExpired(missionTabTemplet, NKCScenManager.CurrentUserData()))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, Refresh, NKCUtilString.GET_STRING_CONFIRM);
			}
			else
			{
				NKCContentManager.MoveToShortCut(nKMMissionTemplet.m_ShortCutType, nKMMissionTemplet.m_ShortCut);
			}
		}
	}

	public void OnClickComplete(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (cNKCUIMissionAchieveSlot == null)
		{
			return;
		}
		NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
		if (nKMMissionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(nKMMissionTemplet.m_MissionTabId);
		if (missionTabTemplet == null)
		{
			return;
		}
		if (m_tabTemplet.m_EventType == NKM_EVENT_TYPE.ONTIME)
		{
			NKMMissionData missionData = NKMMissionManager.GetMissionData(nKMMissionTemplet);
			if (missionData != null && NKMMissionManager.CheckCanReset(nKMMissionTemplet.m_ResetInterval, missionData))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, Refresh, NKCUtilString.GET_STRING_CONFIRM);
				return;
			}
		}
		if (NKMMissionManager.IsMissionTabExpired(missionTabTemplet, NKCScenManager.CurrentUserData()))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, Refresh, NKCUtilString.GET_STRING_CONFIRM);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(nKMMissionTemplet);
		}
	}

	private void BuildMissionTempletList(int tabID)
	{
		m_lstCurrentList.Clear();
		List<NKMMissionTemplet> missionTempletListByType = NKMMissionManager.GetMissionTempletListByType(tabID);
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null || NKMMissionManager.GetMissionTabTemplet(tabID) == null)
		{
			return;
		}
		for (int i = 0; i < missionTempletListByType.Count; i++)
		{
			NKMMissionTemplet nKMMissionTemplet = missionTempletListByType[i];
			if (nKMMissionTemplet == null)
			{
				continue;
			}
			if (nKMMissionTemplet.m_MissionRequire != 0)
			{
				NKMMissionData missionData = myUserData.m_MissionData.GetMissionData(nKMMissionTemplet);
				if (missionData == null)
				{
					continue;
				}
				if (missionData.mission_id == nKMMissionTemplet.m_MissionID)
				{
					m_lstCurrentList.Add(nKMMissionTemplet);
					continue;
				}
				missionData = myUserData.m_MissionData.GetMissionDataByMissionId(nKMMissionTemplet.m_MissionRequire);
				if (missionData == null)
				{
					continue;
				}
				if (missionData.IsComplete && missionData.mission_id == nKMMissionTemplet.m_MissionRequire)
				{
					m_lstCurrentList.Add(nKMMissionTemplet);
					continue;
				}
				if (missionData.mission_id <= nKMMissionTemplet.m_MissionRequire)
				{
					continue;
				}
			}
			m_lstCurrentList.Add(nKMMissionTemplet);
		}
		if (m_lstCurrentList.Count != m_lstMissionSlot.Count)
		{
			Log.Error($"MissionCount error! - TempletCount : {m_lstCurrentList.Count}, SlotCount : {m_lstMissionSlot.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Event/NKCUIEventSubUIFixedMission.cs", 238);
		}
		m_lstCurrentList.Sort(Comparer);
	}

	private int Comparer(NKMMissionTemplet x, NKMMissionTemplet y)
	{
		if (x.m_MissionPoolID != y.m_MissionPoolID)
		{
			return x.m_MissionPoolID.CompareTo(y.m_MissionPoolID);
		}
		return x.m_MissionID.CompareTo(y.m_MissionID);
	}

	private void SetMissionData()
	{
		for (int i = 0; i < m_lstMissionSlot.Count; i++)
		{
			if (i < m_lstCurrentList.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstMissionSlot[i], bValue: true);
				NKMMissionTemplet cNKMMissionTemplet = m_lstCurrentList[i];
				m_lstMissionSlot[i].m_bShowRewardName = true;
				m_lstMissionSlot[i].SetData(cNKMMissionTemplet, OnClickMove, OnClickComplete);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstMissionSlot[i], bValue: false);
			}
		}
	}

	private void SetRemainTime()
	{
		Refresh();
	}

	private void Update()
	{
		if (!(m_objResetRemainTime == null) && m_objResetRemainTime.activeSelf)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRemainTime();
			}
		}
	}
}
