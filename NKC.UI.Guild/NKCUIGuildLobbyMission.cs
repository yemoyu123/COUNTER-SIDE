using System;
using System.Collections.Generic;
using Cs.Logging;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildLobbyMission : MonoBehaviour
{
	public enum MissionUIType
	{
		User,
		Union
	}

	public float WEEKLY_REFRESH_WAITING_MINUTE = 5f;

	public NKCUIMissionAchieveSlot m_pfbSlot;

	public NKCUIComToggle m_tglUser;

	public NKCUIComToggle m_tglUnion;

	public GameObject m_objRefreshCount;

	public Image m_imgRefreshCost;

	public Text m_lbRefreshCost;

	public LoopScrollRect m_loop;

	public Transform m_trSlotParent;

	public NKCUIComStateButton m_btnCompleteAll;

	public Text m_lbCompleteAll;

	public Text m_lbNextResetTime;

	public GameObject m_objDisabled;

	public GameObject m_objResetWaiting;

	private Stack<NKCUIMissionAchieveSlot> m_stkSlot = new Stack<NKCUIMissionAchieveSlot>();

	private List<NKMMissionTemplet> m_lstUserMissions = new List<NKMMissionTemplet>();

	private List<NKMMissionTemplet> m_lstUnionMissions = new List<NKMMissionTemplet>();

	private MissionUIType m_CurrentMissionUIType;

	private float m_fDeltaTime;

	public void InitUI()
	{
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loop);
		NKCUtil.SetBindFunction(m_btnCompleteAll, OnClickCompleteAll);
		NKCUtil.SetToggleValueChangedDelegate(m_tglUser, OnUserToggle);
		NKCUtil.SetToggleValueChangedDelegate(m_tglUnion, OnUnionToggle);
	}

	private RectTransform GetObject(int idx)
	{
		NKCUIMissionAchieveSlot nKCUIMissionAchieveSlot = null;
		nKCUIMissionAchieveSlot = ((m_stkSlot.Count <= 0) ? UnityEngine.Object.Instantiate(m_pfbSlot, m_trSlotParent) : m_stkSlot.Pop());
		nKCUIMissionAchieveSlot.Init();
		return nKCUIMissionAchieveSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		tr.SetParent(base.transform);
		m_stkSlot.Push(component);
	}

	private void ProvideData(Transform tr, int idx)
	{
		tr.GetComponent<NKCUIMissionAchieveSlot>().SetData(m_lstUserMissions[idx], OnClickMove, OnClickComplete, OnClickRefresh);
	}

	public void SetData()
	{
		BuildMissionTemplets();
		if (m_CurrentMissionUIType == MissionUIType.User)
		{
			m_tglUser.Select(bSelect: true, bForce: true, bImmediate: true);
			OnUserToggle(bValue: true);
		}
		else
		{
			m_tglUnion.Select(bSelect: true, bForce: true, bImmediate: true);
			OnUnionToggle(bValue: true);
		}
		SetNextResetTime();
	}

	private void BuildMissionTemplets()
	{
		if (m_lstUserMissions.Count == 0 || !NKCScenManager.CurrentUserData().m_MissionData.WaitingForRandomMissionRefresh())
		{
			m_lstUserMissions = NKMMissionManager.GetGuildUserMissionTemplets();
		}
		m_lstUnionMissions = new List<NKMMissionTemplet>();
	}

	private void SetRefreshCount()
	{
		if (m_CurrentMissionUIType == MissionUIType.User)
		{
			NKCUtil.SetGameobjectActive(m_objRefreshCount, bValue: true);
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData == null)
			{
				return;
			}
			NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(NKM_MISSION_TYPE.GUILD);
			if (missionTabTemplet == null)
			{
				Log.Error("GuildTabTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/NKCUIGuildLobbyMission.cs", 132);
				return;
			}
			int randomMissionRefreshCount = nKMUserData.m_MissionData.GetRandomMissionRefreshCount(missionTabTemplet.m_tabID);
			if (randomMissionRefreshCount > 0)
			{
				NKCUtil.SetGameobjectActive(m_imgRefreshCost, bValue: false);
				NKCUtil.SetLabelText(m_lbRefreshCost, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_POPUP_DONATION_REFRESH_FREE_TEXT, randomMissionRefreshCount));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_imgRefreshCost, bValue: true);
				NKCUtil.SetImageSprite(m_imgRefreshCost, NKCResourceUtility.GetOrLoadMiscItemIcon(missionTabTemplet.m_MissionRefreshReqItemID));
				NKCUtil.SetLabelText(m_lbRefreshCost, missionTabTemplet.m_MissionRefreshReqItemValue.ToString());
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRefreshCount, bValue: false);
		}
	}

	private void SetButtonState()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		if (m_CurrentMissionUIType == MissionUIType.User)
		{
			NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(NKM_MISSION_TYPE.GUILD);
			if (missionTabTemplet == null)
			{
				Log.Error("GuildTabTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/NKCUIGuildLobbyMission.cs", 168);
			}
			else if (!nKMUserData.m_MissionData.CheckCompletableMission(nKMUserData, missionTabTemplet.m_tabID))
			{
				m_btnCompleteAll.Lock();
				NKCUtil.SetLabelTextColor(m_lbCompleteAll, NKCUtil.GetColor("#212122"));
			}
			else
			{
				m_btnCompleteAll.UnLock();
				NKCUtil.SetLabelTextColor(m_lbCompleteAll, NKCUtil.GetColor("#582817"));
			}
		}
		else
		{
			m_btnCompleteAll.Lock();
			NKCUtil.SetLabelTextColor(m_lbCompleteAll, NKCUtil.GetColor("#212122"));
		}
	}

	private void SetNextResetTime()
	{
		DateTime resetTime = NKMTime.GetResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Week);
		if ((NKCSynchronizedTime.GetServerUTCTime() - resetTime).TotalMinutes < (double)WEEKLY_REFRESH_WAITING_MINUTE)
		{
			NKCUtil.SetGameobjectActive(m_objResetWaiting, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objResetWaiting, bValue: false);
		}
		NKCUtil.SetLabelText(m_lbNextResetTime, NKCUtilString.GetResetTimeString(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Week, 3));
	}

	private bool WaitingForWeeklyRefresh()
	{
		return NKCScenManager.CurrentUserData().m_MissionData.WaitingForRandomMissionRefresh();
	}

	private void OnClickCompleteAll()
	{
		if (WaitingForWeeklyRefresh())
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_CONSORTIUM_MISSION_REFRESH_PROGRESS_TITLE_TEXT, NKCUtilString.GET_STRING_CONSORTIUM_MISSION_REFRESH_PROGRESS_DESC_TEXT, SetData);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_ALL_REQ(NKMMissionManager.GetMissionTabTemplet(NKM_MISSION_TYPE.GUILD).m_tabID);
		}
	}

	public void OnClickMove(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (WaitingForWeeklyRefresh())
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_CONSORTIUM_MISSION_REFRESH_PROGRESS_TITLE_TEXT, NKCUtilString.GET_STRING_CONSORTIUM_MISSION_REFRESH_PROGRESS_DESC_TEXT, SetData);
		}
		else
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
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				if (NKMMissionManager.IsMissionTabExpired(missionTabTemplet, nKMUserData))
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, SetData, NKCUtilString.GET_STRING_CONFIRM);
				}
				else
				{
					NKCContentManager.MoveToShortCut(nKMMissionTemplet.m_ShortCutType, nKMMissionTemplet.m_ShortCut);
				}
			}
		}
	}

	public void OnClickComplete(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (WaitingForWeeklyRefresh())
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_CONSORTIUM_MISSION_REFRESH_PROGRESS_TITLE_TEXT, NKCUtilString.GET_STRING_CONSORTIUM_MISSION_REFRESH_PROGRESS_DESC_TEXT, SetData);
		}
		else if (!(cNKCUIMissionAchieveSlot == null))
		{
			NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
			if (nKMMissionTemplet != null)
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(nKMMissionTemplet);
			}
		}
	}

	private void OnClickRefresh(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (WaitingForWeeklyRefresh())
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_CONSORTIUM_MISSION_REFRESH_PROGRESS_TITLE_TEXT, NKCUtilString.GET_STRING_CONSORTIUM_MISSION_REFRESH_PROGRESS_DESC_TEXT, SetData);
			return;
		}
		NKMMissionTabTemplet tabTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet().m_TabTemplet;
		if (tabTemplet == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		if (nKMUserData.m_MissionData.GetRandomMissionRefreshCount(tabTemplet.m_tabID) > 0)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_POPUP_MISSION_REFRESH_CONFIRM_TITLE, NKCUtilString.GET_STRING_CONSORTIUM_POPUP_MISSION_REFRESH_CONFIRM_BODY, delegate
			{
				OnConfirmRefresh(cNKCUIMissionAchieveSlot);
			});
		}
		else
		{
			NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_POPUP_MISSION_REFRESH_CONFIRM_TITLE, NKCUtilString.GET_STRING_CONSORTIUM_POPUP_MISSION_REFRESH_CONFIRM_BODY, tabTemplet.m_MissionRefreshReqItemID, tabTemplet.m_MissionRefreshReqItemValue, delegate
			{
				OnConfirmRefresh(cNKCUIMissionAchieveSlot);
			});
		}
	}

	private void OnConfirmRefresh(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
		if (nKMMissionTemplet != null)
		{
			NKCPacketSender.Send_NKMPacket_RANDOM_MISSION_CHANGE_REQ(nKMMissionTemplet.m_MissionTabId, nKMMissionTemplet.m_MissionID);
		}
	}

	private void OnUserToggle(bool bValue)
	{
		if (bValue)
		{
			m_CurrentMissionUIType = MissionUIType.User;
			m_loop.TotalCount = m_lstUserMissions.Count;
			m_loop.SetIndexPosition(0);
			SetRefreshCount();
			SetButtonState();
			NKCUtil.SetGameobjectActive(m_objDisabled, m_loop.TotalCount == 0);
		}
	}

	private void OnUnionToggle(bool bValue)
	{
		if (bValue)
		{
			m_CurrentMissionUIType = MissionUIType.Union;
			m_loop.TotalCount = 0;
			m_loop.SetIndexPosition(0);
			SetRefreshCount();
			SetButtonState();
			NKCUtil.SetGameobjectActive(m_objDisabled, m_loop.TotalCount == 0);
		}
	}

	private void Update()
	{
		m_fDeltaTime += Time.deltaTime;
		if (m_fDeltaTime > 1f)
		{
			m_fDeltaTime -= 1f;
			SetNextResetTime();
		}
	}
}
