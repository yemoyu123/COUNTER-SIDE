using System;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Core.Util;
using Cs.Logging;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMMissionTabTemplet : INKMTemplet, INKMTempletEx
{
	private NKMIntervalTemplet intervalTemplet = NKMIntervalTemplet.Invalid;

	public int m_tabID;

	public NKM_MISSION_TYPE m_MissionType;

	private string m_OpenTag;

	public string m_MissionTabDesc = string.Empty;

	public string m_MissionTabIconName = string.Empty;

	public string m_MainUnitStrID = string.Empty;

	public string m_SlotBannerName = string.Empty;

	public bool m_LobbyIconDisplayBool;

	public string m_LobbyIconName = string.Empty;

	public string m_LobbyIconDesc = string.Empty;

	public bool m_MissionTotalPointBool;

	public int m_MissionTotalPointID;

	public int m_NewbieDate;

	public ReturningUserType m_ReturningUserType;

	public int m_MissionPoolID;

	public int m_MissionDisplayCount;

	public int m_MissionRefreshFreeCount;

	public int m_MissionRefreshReqItemID;

	public int m_MissionRefreshReqItemValue;

	public string intervalId;

	public string m_MissionBannerImage;

	public int m_OrderList;

	public bool m_Visible = true;

	public bool m_VisibleWhenLocked;

	public List<UnlockInfo> m_UnlockInfo = new List<UnlockInfo>();

	public int m_firstMissionID;

	public int m_completeMissionID;

	public int Key => m_tabID;

	public DateTime m_startTime => intervalTemplet.StartDate;

	public DateTime m_endTime => intervalTemplet.EndDate;

	public DateTime m_startTimeUtc => ServiceTime.ToUtcTime(intervalTemplet.StartDate);

	public DateTime m_endTimeUtc => ServiceTime.ToUtcTime(intervalTemplet.EndDate);

	public bool HasDateLimit => intervalTemplet.IsValid;

	public bool IsReturningMission => m_ReturningUserType != ReturningUserType.None;

	public bool IsNewbieMission => m_NewbieDate > 0;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public static NKMMissionTabTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 840))
		{
			return null;
		}
		NKMMissionTabTemplet nKMMissionTabTemplet = new NKMMissionTabTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_TabID", ref nKMMissionTabTemplet.m_tabID) ? 1u : 0u)) & (cNKMLua.GetData("m_MissionTab", ref nKMMissionTabTemplet.m_MissionType) ? 1 : 0);
		cNKMLua.GetData("m_OpenTag", ref nKMMissionTabTemplet.m_OpenTag);
		cNKMLua.GetData("m_MissionTabDesc", ref nKMMissionTabTemplet.m_MissionTabDesc);
		cNKMLua.GetData("m_MissionTabIconName", ref nKMMissionTabTemplet.m_MissionTabIconName);
		cNKMLua.GetData("m_MainUnitStrID", ref nKMMissionTabTemplet.m_MainUnitStrID);
		cNKMLua.GetData("m_SlotBannerName", ref nKMMissionTabTemplet.m_SlotBannerName);
		cNKMLua.GetData("m_LobbyIconDisplayBool", ref nKMMissionTabTemplet.m_LobbyIconDisplayBool);
		cNKMLua.GetData("m_LobbyIconName", ref nKMMissionTabTemplet.m_LobbyIconName);
		cNKMLua.GetData("m_LobbyIconDesc", ref nKMMissionTabTemplet.m_LobbyIconDesc);
		cNKMLua.GetData("m_MissionTotalPointBool", ref nKMMissionTabTemplet.m_MissionTotalPointBool);
		cNKMLua.GetData("m_MissionTotalPointID", ref nKMMissionTabTemplet.m_MissionTotalPointID);
		cNKMLua.GetData("m_Visible", ref nKMMissionTabTemplet.m_Visible);
		cNKMLua.GetData("m_VisibleWhenLocked", ref nKMMissionTabTemplet.m_VisibleWhenLocked);
		cNKMLua.GetData("m_firstMissionID", ref nKMMissionTabTemplet.m_firstMissionID);
		cNKMLua.GetData("m_completeMissionID", ref nKMMissionTabTemplet.m_completeMissionID);
		cNKMLua.GetData("m_NewbieDate", ref nKMMissionTabTemplet.m_NewbieDate);
		cNKMLua.GetData("m_ReturningUserType", ref nKMMissionTabTemplet.m_ReturningUserType);
		cNKMLua.GetData("m_MissionPoolID", ref nKMMissionTabTemplet.m_MissionPoolID);
		cNKMLua.GetData("m_MissionDisplayCount", ref nKMMissionTabTemplet.m_MissionDisplayCount);
		cNKMLua.GetData("m_MissionRefreshFreeCount", ref nKMMissionTabTemplet.m_MissionRefreshFreeCount);
		cNKMLua.GetData("m_MissionRefreshReqItemID", ref nKMMissionTabTemplet.m_MissionRefreshReqItemID);
		cNKMLua.GetData("m_MissionRefreshReqItemValue", ref nKMMissionTabTemplet.m_MissionRefreshReqItemValue);
		cNKMLua.GetData("m_DateStrID", ref nKMMissionTabTemplet.intervalId);
		cNKMLua.GetData("m_MissionBannerImage", ref nKMMissionTabTemplet.m_MissionBannerImage);
		nKMMissionTabTemplet.m_UnlockInfo = UnlockInfo.LoadFromLua2(cNKMLua);
		cNKMLua.GetData("m_OrderList", ref nKMMissionTabTemplet.m_OrderList);
		if (num == 0)
		{
			Log.Error($"NKMMissionTabTemplet Load - {nKMMissionTabTemplet.m_tabID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 884);
			return null;
		}
		return nKMMissionTabTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void JoinIntervalTemplet()
	{
		if (!string.IsNullOrEmpty(intervalId))
		{
			intervalTemplet = NKMIntervalTemplet.Find(intervalId);
			if (intervalTemplet == null)
			{
				intervalTemplet = NKMIntervalTemplet.Unuseable;
				Log.ErrorAndExit($"[Mission:{Key}] \ufffd߸\ufffd\ufffd\ufffd interval id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 905);
			}
			else if (intervalTemplet.IsRepeatDate)
			{
				Log.ErrorAndExit($"[Mission:{Key}] \ufffdݺ\ufffd \ufffdⰣ\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd \ufffdҰ\ufffd. id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMissionManager.cs", 911);
			}
		}
	}

	public void Validate()
	{
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}

	public string GetDesc()
	{
		return NKCStringTable.GetString(m_MissionTabDesc);
	}

	public string GetLobbyIconDesc()
	{
		return NKCStringTable.GetString(m_LobbyIconDesc);
	}
}
