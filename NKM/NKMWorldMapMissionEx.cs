using ClientPacket.WorldMap;

namespace NKM;

public static class NKMWorldMapMissionEx
{
	public static NKM_ERROR_CODE CanCancelMission(this NKMWorldMapMission data, int missionID)
	{
		if (data.currentMissionID == 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_DOING;
		}
		if (NKMWorldMapManager.GetMissionTemplet(missionID) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_MISSION_ID;
		}
		if (!data.ValidMissionID(missionID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_MISSION_ID;
		}
		if (data.currentMissionID != missionID)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_MISSION_ID;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanStartMission(this NKMWorldMapMission data, NKMUserData userData, int missionID, NKMDeckIndex deckIndex, NKMUnitData leaderUnit)
	{
		if (data.currentMissionID != 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_DOING;
		}
		NKMWorldMapMissionTemplet missionTemplet = NKMWorldMapManager.GetMissionTemplet(missionID);
		if (missionTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_MISSION_ID;
		}
		if (leaderUnit.m_UnitLevel < missionTemplet.m_ReqManagerLevel)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_LEADER_LEVEL_LOW;
		}
		if (!data.stMissionIDList.Contains(missionID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_MISSION_ID;
		}
		if (!data.ValidMissionID(missionID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_INVALID_MISSION_ID;
		}
		if (!NKMWorldMapManager.IsMissionLeaderOnly(missionTemplet.m_eMissionType))
		{
			if (deckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_NONE)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_INVALID_DECK;
			}
			if (userData.m_ArmyData.GetDeckData(deckIndex) == null)
			{
				return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_INVALID_DECK;
			}
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMMain.IsValidDeck(userData.m_ArmyData, deckIndex);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static void Reset(this NKMWorldMapMission data, int[] mission_list)
	{
		data.currentMissionID = 0;
		data.completeTime = 0L;
		data.stMissionIDList.Clear();
		foreach (int item in mission_list)
		{
			data.stMissionIDList.Add(item);
		}
	}

	public static void Reset(this NKMWorldMapMission data)
	{
		data.currentMissionID = 0;
		data.completeTime = 0L;
	}

	public static bool ValidMissionID(this NKMWorldMapMission data, int mission_id)
	{
		foreach (int stMissionID in data.stMissionIDList)
		{
			if (stMissionID == mission_id)
			{
				return true;
			}
		}
		return false;
	}
}
