using System.Collections.Generic;
using System.Linq;
using ClientPacket.Pvp;
using Cs.Core.Util;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public static class NKCEventPvpMgr
{
	private static NKMEventDeckData m_eventDeckData = null;

	private static List<EventPvpReward> m_eventPvpRewardInfo = null;

	private static string m_lobbyArtBundleName = "AB_UI_NKM_UI_GAUNTLET_EVENTMATCH_RES";

	private static string m_eventDeckArtBundleName = "AB_UI_NKM_UI_GAUNTLET_EVENTMATCH_THUMBNAIL";

	private static string m_contantTag = "UNLOCK_PVP_EVENTMATCH";

	public static NKMEventDeckData EventDeckData
	{
		get
		{
			return m_eventDeckData;
		}
		set
		{
			m_eventDeckData = value;
		}
	}

	public static List<EventPvpReward> EventPvpRewardInfo
	{
		get
		{
			return m_eventPvpRewardInfo;
		}
		set
		{
			m_eventPvpRewardInfo = value;
		}
	}

	public static NKMEventPvpSeasonTemplet GetEventPvpSeasonTemplet()
	{
		return NKMEventPvpSeasonTemplet.Values.FirstOrDefault((NKMEventPvpSeasonTemplet e) => NKMIntervalTemplet.Find(e.IntervalStrId)?.IsValidTime(ServiceTime.Recent) ?? false);
	}

	public static bool CanAccessEventPvp()
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet == null || !eventPvpSeasonTemplet.EnableByTag)
		{
			return false;
		}
		return true;
	}

	public static bool IsTournamentPractice()
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet == null || !eventPvpSeasonTemplet.EnableByTag)
		{
			return false;
		}
		if (eventPvpSeasonTemplet.EnableTournamentBan && eventPvpSeasonTemplet.MappingTournamentId == NKCTournamentManager.m_TournamentInfo.tournamentId)
		{
			return true;
		}
		return false;
	}

	public static bool IsEventPvpTime()
	{
		return true;
	}

	public static bool IsDeteminedSlotType(NKMDungeonEventDeckTemplet.SLOT_TYPE slotType)
	{
		if (slotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED || slotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST || slotType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC)
		{
			return true;
		}
		return false;
	}

	public static bool CanGetReward()
	{
		if (m_eventPvpRewardInfo == null)
		{
			return false;
		}
		foreach (EventPvpReward item in m_eventPvpRewardInfo)
		{
			NKMEventPvpRewardTemplet nKMEventPvpRewardTemplet = NKMEventPvpRewardTemplet.FindStep(item.groupId, item.step);
			if (nKMEventPvpRewardTemplet != null && !item.isReward && nKMEventPvpRewardTemplet.PlayTimes <= item.playCount)
			{
				return true;
			}
		}
		return false;
	}

	public static Sprite GetLobbySeasonArt()
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet != null)
		{
			NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(m_lobbyArtBundleName, eventPvpSeasonTemplet.LobbyArtResource);
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>(nKMAssetName.m_BundleName, nKMAssetName.m_AssetName);
		}
		return null;
	}

	public static Sprite GetEventDeckSeasonArt()
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet != null)
		{
			NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(m_eventDeckArtBundleName, eventPvpSeasonTemplet.EventDeckThumbnail);
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>(nKMAssetName.m_BundleName, nKMAssetName.m_AssetName);
		}
		return null;
	}

	public static string GetEventMatchIntervalString()
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet != null)
		{
			NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(eventPvpSeasonTemplet.IntervalStrId);
			if (nKMIntervalTemplet != null)
			{
				return NKCUtilString.GetTimeIntervalString(nKMIntervalTemplet.StartDate, nKMIntervalTemplet.EndDate, NKMTime.INTERVAL_FROM_UTC);
			}
		}
		return "";
	}
}
