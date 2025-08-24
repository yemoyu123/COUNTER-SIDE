using System.Collections.Generic;
using NKM;
using NKM.Event;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Module;

public class NKCUIModuleSubUIMission : NKCUIModuleSubUIBase
{
	[Header("\ufffd\u033c\ufffd \ufffd\ufffdũ\ufffdѷ\ufffdƮ")]
	public LoopScrollRect m_scrollRect;

	[Header("\ufffdϰ\ufffd \ufffdϷ\ufffd \ufffd\ufffdư")]
	public NKCUIComStateButton m_csbtnCompleteAll;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\u033c\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public NKCUIMissionAchieveSlot m_SpecialMissionSlot;

	[Space]
	public GameObject m_objAllMissionCompleted;

	public GameObject m_objSpecialMissionCompleted;

	public bool m_hideCompletedMission;

	public bool m_hideLockedMission;

	private string m_missionSlotBundleName;

	private string m_missionSlotAssetName;

	private NKMEventCollectionIndexTemplet m_collectionIndexTemplet;

	private List<NKMMissionTemplet> m_missionTempletList = new List<NKMMissionTemplet>();

	private List<NKMMissionTemplet> m_missionTempletShowList = new List<NKMMissionTemplet>();

	private List<int> m_completeEnableTabIdList = new List<int>();

	private bool m_bScrollRectInit;

	public override void Init()
	{
		base.Init();
		NKCUtil.SetButtonClickDelegate(m_csbtnCompleteAll, OnCompleteAll);
		m_SpecialMissionSlot?.Init();
	}

	public override void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
		base.OnOpen(templet);
		m_collectionIndexTemplet = templet;
		if (templet == null)
		{
			return;
		}
		m_missionSlotBundleName = templet.EventPrefabID_BundleName;
		m_missionSlotAssetName = templet.EventMissionSlotPrefabId;
		if (!m_bScrollRectInit && m_scrollRect != null)
		{
			m_scrollRect.dOnGetObject += GetSlot;
			m_scrollRect.dOnReturnObject += ReturnSlot;
			m_scrollRect.dOnProvideData += ProvideData;
			m_scrollRect.ContentConstraintCount = 1;
			m_scrollRect.TotalCount = 0;
			m_scrollRect.PrepareCells();
			m_bScrollRectInit = true;
		}
		m_missionTempletList.Clear();
		foreach (int missionTabId in templet.MissionTabIds)
		{
			List<NKMMissionTemplet> missionTempletListByType = NKMMissionManager.GetMissionTempletListByType(missionTabId);
			if (missionTempletListByType != null)
			{
				m_missionTempletList.AddRange(missionTempletListByType);
			}
		}
		Refresh();
	}

	public override void Refresh()
	{
		base.Refresh();
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		m_missionTempletShowList.Clear();
		foreach (NKMMissionTemplet missionTemplet in m_missionTempletList)
		{
			if (missionTemplet == null)
			{
				continue;
			}
			if (m_hideCompletedMission)
			{
				NKMMissionManager.MissionStateData missionStateData = NKMMissionManager.GetMissionStateData(missionTemplet);
				if (missionStateData.state == NKMMissionManager.MissionState.COMPLETED || missionStateData.state == NKMMissionManager.MissionState.REPEAT_COMPLETED)
				{
					continue;
				}
			}
			if (m_hideLockedMission && missionTemplet.m_MissionRequire != 0)
			{
				NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
				NKMMissionData nKMMissionData = myUserData?.m_MissionData.GetMissionData(missionTemplet);
				if (nKMMissionData == null)
				{
					continue;
				}
				if (nKMMissionData.mission_id == missionTemplet.m_MissionID)
				{
					m_missionTempletShowList.Add(missionTemplet);
					continue;
				}
				nKMMissionData = myUserData?.m_MissionData.GetMissionDataByMissionId(missionTemplet.m_MissionRequire);
				if (nKMMissionData == null)
				{
					continue;
				}
				if (nKMMissionData.IsComplete && nKMMissionData.mission_id == missionTemplet.m_MissionRequire)
				{
					m_missionTempletShowList.Add(missionTemplet);
					continue;
				}
				if (nKMMissionData.mission_id <= missionTemplet.m_MissionRequire)
				{
					continue;
				}
			}
			m_missionTempletShowList.Add(missionTemplet);
		}
		m_missionTempletShowList.Sort(NKMMissionManager.Comparer);
		NKCUtil.SetGameobjectActive(m_objAllMissionCompleted, m_missionTempletShowList.Count <= 0);
		if (m_scrollRect != null)
		{
			m_scrollRect.TotalCount = m_missionTempletShowList.Count;
			m_scrollRect.SetIndexPosition(0);
		}
		UpdateCompleteAllState();
		int specialMissionData = ((m_collectionIndexTemplet != null) ? m_collectionIndexTemplet.EventMissionAllClearTabId : 0);
		SetSpecialMissionData(specialMissionData);
	}

	public override void OnClose()
	{
		base.OnClose();
		m_missionSlotBundleName = null;
		m_missionSlotAssetName = null;
		m_collectionIndexTemplet = null;
		m_missionTempletList.Clear();
		m_missionTempletShowList.Clear();
		m_completeEnableTabIdList.Clear();
	}

	private void UpdateCompleteAllState()
	{
		m_completeEnableTabIdList.Clear();
		m_csbtnCompleteAll?.Lock();
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMUserMissionData missionData = NKCScenManager.GetScenManager().GetMyUserData().m_MissionData;
		if (missionData == null || m_collectionIndexTemplet == null)
		{
			return;
		}
		foreach (int missionTabId in m_collectionIndexTemplet.MissionTabIds)
		{
			if (missionData.CheckCompletableMission(myUserData, missionTabId))
			{
				m_csbtnCompleteAll?.UnLock();
				m_completeEnableTabIdList.Add(missionTabId);
			}
		}
	}

	private void SetSpecialMissionData(int specialMissionTabID)
	{
		if (m_SpecialMissionSlot == null)
		{
			return;
		}
		if (specialMissionTabID <= 0)
		{
			NKCUtil.SetGameobjectActive(m_SpecialMissionSlot, bValue: false);
			return;
		}
		List<NKMMissionTemplet> missionTempletListByType = NKMMissionManager.GetMissionTempletListByType(specialMissionTabID);
		NKMMissionTemplet nKMMissionTemplet = null;
		NKMMissionTemplet nKMMissionTemplet2 = null;
		NKMMissionTemplet nKMMissionTemplet3 = null;
		bool flag = false;
		foreach (NKMMissionTemplet item in missionTempletListByType)
		{
			switch (NKMMissionManager.GetMissionStateData(item).state)
			{
			case NKMMissionManager.MissionState.CAN_COMPLETE:
			case NKMMissionManager.MissionState.REPEAT_CAN_COMPLETE:
			case NKMMissionManager.MissionState.ONGOING:
				nKMMissionTemplet = item;
				flag = true;
				break;
			case NKMMissionManager.MissionState.REPEAT_COMPLETED:
			case NKMMissionManager.MissionState.COMPLETED:
				nKMMissionTemplet2 = item;
				break;
			case NKMMissionManager.MissionState.LOCKED:
				if (nKMMissionTemplet3 == null)
				{
					nKMMissionTemplet3 = item;
				}
				break;
			}
			if (flag)
			{
				break;
			}
		}
		bool bValue = false;
		if (nKMMissionTemplet == null)
		{
			if (nKMMissionTemplet2 != null)
			{
				nKMMissionTemplet = nKMMissionTemplet2;
				bValue = true;
			}
			else
			{
				if (nKMMissionTemplet3 == null)
				{
					NKCUtil.SetGameobjectActive(m_SpecialMissionSlot, bValue: false);
					return;
				}
				nKMMissionTemplet = nKMMissionTemplet3;
			}
		}
		m_SpecialMissionSlot?.SetData(nKMMissionTemplet, OnClickMove, OnClickComplete);
		NKCUtil.SetGameobjectActive(m_SpecialMissionSlot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objSpecialMissionCompleted, bValue);
	}

	private void OnClickMove(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
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

	private void OnClickComplete(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
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
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(nKMMissionTemplet);
			}
		}
	}

	private RectTransform GetSlot(int index)
	{
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(m_missionSlotBundleName, m_missionSlotAssetName);
		return NKCUIMissionAchieveSlot.GetNewInstance(null, nKMAssetName.m_BundleName, nKMAssetName.m_AssetName)?.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform tr)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
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

	private void ProvideData(Transform tr, int index)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
		if (!(component == null) && m_missionTempletShowList.Count > index)
		{
			component.SetData(m_missionTempletShowList[index], OnClickMove, OnClickComplete);
		}
	}

	private void OnCompleteAll()
	{
		foreach (int completeEnableTabId in m_completeEnableTabIdList)
		{
			NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_ALL_REQ(completeEnableTabId);
		}
	}
}
