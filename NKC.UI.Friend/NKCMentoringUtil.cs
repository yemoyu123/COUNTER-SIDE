using ClientPacket.Common;
using NKM;
using NKM.Templet;

namespace NKC.UI.Friend;

public static class NKCMentoringUtil
{
	public enum MENTORING_STATS
	{
		MS_NONE,
		MS_MAINTENANCE,
		MS_SEASON_ID_REQ,
		MS_ACTIVE
	}

	public static NKMMentoringTemplet GetCurrentTempet()
	{
		return NKMMentoringTemplet.Find(NKCScenManager.CurrentUserData().MentoringData.SeasonId);
	}

	public static MentoringIdentity GetMentoringIdentity(NKMUserData userData)
	{
		if (userData != null && userData.UserLevel > NKMMentoringConst.MentorAddLimitLevel && NKCScenManager.CurrentUserData().MentoringData.MyMentor == null)
		{
			return MentoringIdentity.Mentor;
		}
		return MentoringIdentity.Mentee;
	}

	public static int GetMenteeMissionMaxCount()
	{
		return GetMenteeMissionMaxCount(GetCurrentTempet());
	}

	public static int GetMenteeMissionMaxCount(NKMMentoringTemplet templet)
	{
		int result = 0;
		if (templet != null)
		{
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(templet.AllClearMissionId);
			if (missionTemplet != null)
			{
				result = missionTemplet.m_MissionCond.value1.Count + 1;
			}
		}
		return result;
	}

	public static MENTORING_STATS CheckMentoringSeason()
	{
		if (NKCScenManager.CurrentUserData().MentoringData.SeasonId == 0)
		{
			return MENTORING_STATS.MS_NONE;
		}
		NKMMentoringTemplet currentTempet = GetCurrentTempet();
		if (currentTempet != null)
		{
			NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(currentTempet.MissionTabId);
			if (missionTabTemplet != null)
			{
				if (missionTabTemplet.m_endTime <= NKCSynchronizedTime.ServiceTime)
				{
					if (missionTabTemplet.m_endTime.AddMinutes(NKMMentoringConst.MentoringSeasonInitMinutes) <= NKCSynchronizedTime.ServiceTime)
					{
						return MENTORING_STATS.MS_MAINTENANCE;
					}
					NKCPacketSender.Send_NKMPacket_MENTORING_SEASON_ID_REQ();
					return MENTORING_STATS.MS_SEASON_ID_REQ;
				}
				if (missionTabTemplet.m_startTime.AddMinutes(NKMMentoringConst.MentoringSeasonInitMinutes) >= NKCSynchronizedTime.ServiceTime)
				{
					return MENTORING_STATS.MS_MAINTENANCE;
				}
				return MENTORING_STATS.MS_ACTIVE;
			}
		}
		return MENTORING_STATS.MS_NONE;
	}

	public static bool IsCanReceiveMenteeMissionReward(NKMUserData userData)
	{
		if (userData == null)
		{
			return false;
		}
		if (GetMentoringIdentity(userData) == MentoringIdentity.Mentor)
		{
			return false;
		}
		NKMMentoringTemplet currentTempet = GetCurrentTempet();
		if (currentTempet == null)
		{
			return false;
		}
		if (userData.MentoringData.MyMentor != null && !userData.MentoringData.bMenteeGraduate)
		{
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(currentTempet.AllClearMissionId);
			if (missionTemplet != null)
			{
				foreach (int item in missionTemplet.m_MissionCond.value1)
				{
					NKMMissionTemplet missionTemplet2 = NKMMissionManager.GetMissionTemplet(item);
					if (missionTemplet2 != null)
					{
						NKMMissionManager.MissionState missionState = NKMMissionManager.GetMissionState(missionTemplet2);
						if (missionState == NKMMissionManager.MissionState.CAN_COMPLETE || missionState == NKMMissionManager.MissionState.REPEAT_CAN_COMPLETE)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public static bool IsDontHaveMentor(NKMUserData userData)
	{
		if (userData == null)
		{
			return false;
		}
		if (GetMentoringIdentity(userData) == MentoringIdentity.Mentor)
		{
			return false;
		}
		return userData.MentoringData.MyMentor == null;
	}
}
