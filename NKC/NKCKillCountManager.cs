using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using NKM.Templet;

namespace NKC;

public static class NKCKillCountManager
{
	private static List<NKMServerKillCountData> serverKillCountDataList;

	private static List<NKMKillCountData> killCountDataList;

	public static long CurrentStageKillCount { get; set; }

	public static void SetServerKillCountData(List<NKMServerKillCountData> serverKillCountData)
	{
		serverKillCountDataList = serverKillCountData;
	}

	public static void SetKillCountData(List<NKMKillCountData> killCountData)
	{
		killCountDataList = killCountData;
	}

	public static NKMServerKillCountData GetKillCountServerData(int eventId)
	{
		if (serverKillCountDataList == null)
		{
			return null;
		}
		return serverKillCountDataList.Find((NKMServerKillCountData e) => e.killCountId == eventId);
	}

	public static NKMKillCountData GetKillCountData(int eventId)
	{
		if (killCountDataList == null)
		{
			return null;
		}
		return killCountDataList.Find((NKMKillCountData e) => e.killCountId == eventId);
	}

	public static void UpdateKillCountData(NKMKillCountData killCountData)
	{
		if (killCountDataList != null && killCountData != null)
		{
			int num = killCountDataList.FindIndex((NKMKillCountData e) => e.killCountId == killCountData.killCountId);
			if (num < 0 || num >= killCountDataList.Count)
			{
				killCountDataList.Add(killCountData);
			}
			else
			{
				killCountDataList[num] = killCountData;
			}
		}
	}

	public static bool IsKillCountDungeon(NKMGameData gameData)
	{
		NKMStageTempletV2 nKMStageTempletV;
		switch (gameData.GetGameType())
		{
		case NKM_GAME_TYPE.NGT_PVE_DEFENCE:
			return true;
		case NKM_GAME_TYPE.NGT_PHASE:
			nKMStageTempletV = NKCPhaseManager.GetStageTemplet();
			break;
		default:
			nKMStageTempletV = NKMDungeonManager.GetDungeonTempletBase(gameData.m_DungeonID)?.StageTemplet;
			break;
		}
		if (nKMStageTempletV == null)
		{
			return false;
		}
		return nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_KILLCOUNT;
	}
}
