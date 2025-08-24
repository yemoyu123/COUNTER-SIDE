using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMAttendanceManager
{
	private static Dictionary<int, NKMAttendanceTabTemplet> templets = null;

	private static readonly Dictionary<int, Dictionary<int, NKMAttendanceRewardTemplet>> rewards = new Dictionary<int, Dictionary<int, NKMAttendanceRewardTemplet>>();

	private static DateTime TodayAttendanceDate;

	private static bool m_bContentBlocked = false;

	private static List<int> m_lstAttendanceKey = new List<int>();

	private static DateTime m_tNextKeySettingTime = default(DateTime);

	public static IEnumerable<NKMAttendanceTabTemplet> Values => templets.Values;

	public static bool IsAttendanceBlocked => m_bContentBlocked;

	public static bool LoadFromLua()
	{
		bool flag = true;
		templets = NKMTempletLoader.LoadDictionary("AB_SCRIPT", "LUA_ATTENDANCE_TAB_TEMPLET", "ATTENDANCE_TAB_TEMPLET", NKMAttendanceTabTemplet.LoadFromLUA);
		if (templets == null)
		{
			NKMTempletError.Add("AttendanceTemplet Load Failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAttendanceManager.cs", 219);
			return false;
		}
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_ATTENDANCE_REWARD_TEMPLET") && nKMLua.OpenTable("ATTENDANCE_REWARD_TEMPLET"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				if (NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAttendanceManager.cs", 231))
				{
					NKMAttendanceRewardTemplet nKMAttendanceRewardTemplet = new NKMAttendanceRewardTemplet();
					flag &= nKMAttendanceRewardTemplet.LoadFromLUA(nKMLua);
					if (!rewards.ContainsKey(nKMAttendanceRewardTemplet.RewardGroup))
					{
						rewards.Add(nKMAttendanceRewardTemplet.RewardGroup, new Dictionary<int, NKMAttendanceRewardTemplet>());
					}
					rewards[nKMAttendanceRewardTemplet.RewardGroup][nKMAttendanceRewardTemplet.LoginDate] = nKMAttendanceRewardTemplet;
				}
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		foreach (KeyValuePair<int, NKMAttendanceTabTemplet> templet in templets)
		{
			if (!rewards.ContainsKey(templet.Value.RewardGroup))
			{
				NKMTempletError.Add($"RewardGroup \ufffd\ufffd \ufffdش\ufffd\ufffdϴ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd - {templet.Value.RewardGroup}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAttendanceManager.cs", 255);
				return false;
			}
			if (rewards[templet.Value.RewardGroup].Count != templet.Value.MaxAttCount)
			{
				NKMTempletError.Add($"RewardGroup \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd TabTemplet\ufffd\ufffd \ufffd\ufffd\ufffdǵ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdٸ\ufffd - {templet.Value.RewardGroup}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAttendanceManager.cs", 261);
				return false;
			}
			templet.Value.Join();
		}
		return true;
	}

	public static NKMAttendanceTabTemplet GetAttendanceTabTamplet(int idx)
	{
		templets.TryGetValue(idx, out var value);
		return value;
	}

	public static Dictionary<int, NKMAttendanceRewardTemplet> GetAttendanceRewardTemplet(int rewardGroupID)
	{
		rewards.TryGetValue(rewardGroupID, out var value);
		return value;
	}

	public static void Validate()
	{
		foreach (NKMAttendanceRewardTemplet item in rewards.Values.SelectMany((Dictionary<int, NKMAttendanceRewardTemplet> e) => e.Values))
		{
			item.Validate();
		}
		foreach (NKMAttendanceTabTemplet value in templets.Values)
		{
			value.Validate();
		}
	}

	public static void PostJoin()
	{
		foreach (KeyValuePair<int, NKMAttendanceTabTemplet> templet in templets)
		{
			templet.Value.PostJoin();
		}
	}

	public static DateTime GetTodayResetDate(DateTime date)
	{
		return NKMTime.GetResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Day);
	}

	public static void Init(DateTime date)
	{
		TodayAttendanceDate = NKMTime.GetResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Day);
		m_tNextKeySettingTime = default(DateTime);
		m_bContentBlocked = false;
	}

	public static bool CheckNeedAttendance(NKMAttendanceData attendanceData, DateTime now, int idx = 0)
	{
		if (m_bContentBlocked)
		{
			return false;
		}
		if (attendanceData == null || attendanceData.AttList.Count == 0)
		{
			return true;
		}
		if (idx != 0 && attendanceData.AttList.Find((NKMAttendance x) => x.IDX == idx) == null)
		{
			return true;
		}
		if (idx == 0)
		{
			attendanceData = AddNeedAttendanceKeyByTemplet(attendanceData);
		}
		foreach (NKMAttendance att in attendanceData.AttList)
		{
			if ((idx == 0 || att.IDX == idx) && CheckNeedAttendance(att, now))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckNeedAttendance(NKMAttendance attendance, DateTime now)
	{
		if (m_bContentBlocked)
		{
			return false;
		}
		if (attendance == null)
		{
			return true;
		}
		if (!templets.ContainsKey(attendance.IDX))
		{
			Log.Error($"IDX {attendance.IDX} 가 존재하지 않음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMAttendanceManagerEx.cs", 88);
			return false;
		}
		if (attendance.EventEndDate < now)
		{
			return false;
		}
		if (attendance.Count == 0)
		{
			return true;
		}
		if (NKMTime.GetNextResetTime(NKCScenManager.CurrentUserData().m_AttendanceData.LastUpdateDate, NKMTime.TimePeriod.Day) < now)
		{
			NKMAttendanceTabTemplet attendanceTabTamplet = GetAttendanceTabTamplet(attendance.IDX);
			if (attendanceTabTamplet != null && attendance.Count < attendanceTabTamplet.MaxAttCount)
			{
				return true;
			}
		}
		if (!rewards.ContainsKey(templets[attendance.IDX].RewardGroup))
		{
			Log.Error($"RewardGroup {templets[attendance.IDX].RewardGroup} 가 존재하지 않음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMAttendanceManagerEx.cs", 110);
			return false;
		}
		return false;
	}

	public static void ResetNeedAttendanceKey()
	{
		m_lstAttendanceKey = new List<int>();
	}

	public static List<int> GetNeedAttendanceKey()
	{
		return m_lstAttendanceKey;
	}

	public static NKMAttendanceData AddNeedAttendanceKeyByTemplet(NKMAttendanceData attData)
	{
		if (m_tNextKeySettingTime > NKCSynchronizedTime.GetServerUTCTime() && attData != null && attData.LastUpdateDate.Ticks > 0)
		{
			return attData;
		}
		m_tNextKeySettingTime = NKMTime.GetResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Day);
		if (attData == null)
		{
			attData = new NKMAttendanceData();
		}
		foreach (NKMAttendanceTabTemplet attendanceTemplet in Values)
		{
			if ((!NKMContentsVersionManager.HasDFChangeTagType(DataFormatChangeTagType.OPEN_TAG_ATTENDANCE) || attendanceTemplet.EnableByTag) && NKCSynchronizedTime.IsEventTime(attendanceTemplet.StartDateUtc, attendanceTemplet.EndDateUtc) && attData.AttList.Find((NKMAttendance e) => e.IDX == attendanceTemplet.IDX) == null && attendanceTemplet.EventType != NKM_ATTENDANCE_EVENT_TYPE.NEW && attendanceTemplet.EventType != NKM_ATTENDANCE_EVENT_TYPE.RETURN)
			{
				NKMAttendance nKMAttendance = new NKMAttendance();
				nKMAttendance.IDX = attendanceTemplet.IDX;
				nKMAttendance.Count = 0;
				nKMAttendance.EventEndDate = attendanceTemplet.EndDateUtc;
				attData.AttList.Add(nKMAttendance);
			}
		}
		m_lstAttendanceKey = new List<int>();
		for (int num = 0; num < attData.AttList.Count; num++)
		{
			if (CheckNeedAttendance(attData.AttList[num], NKCSynchronizedTime.GetServerUTCTime()))
			{
				m_lstAttendanceKey.Add(attData.AttList[num].IDX);
			}
		}
		return attData;
	}

	public static void SetContentBlock()
	{
		m_bContentBlocked = true;
	}
}
