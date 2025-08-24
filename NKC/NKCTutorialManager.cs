using System;
using System.Collections.Generic;
using ClientPacket.Guild;
using ClientPacket.Warfare;
using ClientPacket.WorldMap;
using NKC.UI.Option;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public static class NKCTutorialManager
{
	private static Dictionary<int, NKCTutorialReqTemplet> m_dicReqTemplet = new Dictionary<int, NKCTutorialReqTemplet>();

	private static TutorialPoint lastPoint = TutorialPoint.None;

	public static bool CheckTutoGameCondAtLogin(NKMUserData userData)
	{
		if (userData == null)
		{
			return false;
		}
		if ((userData.m_eAuthLevel == NKM_USER_AUTH_LEVEL.NORMAL_USER || userData.m_eAuthLevel == NKM_USER_AUTH_LEVEL.NORMAL_ADMIN) && userData.UserLevel == 1)
		{
			return true;
		}
		return false;
	}

	public static bool IsTutorialDungeon(int dungeonID)
	{
		return NKMDungeonManager.IsTutorialDungeon(dungeonID);
	}

	public static bool IsPrologueDungeon(int dungeonID)
	{
		if ((uint)(dungeonID - 1004) <= 2u)
		{
			return true;
		}
		return false;
	}

	public static bool CanGiveupDungeon(int dungeonID)
	{
		if ((uint)(dungeonID - 1004) <= 3u)
		{
			return NKCScenManager.CurrentUserData().CheckDungeonClear(dungeonID);
		}
		return true;
	}

	public static bool TutorialRequired(TutorialStep step)
	{
		foreach (KeyValuePair<int, NKCTutorialReqTemplet> item in m_dicReqTemplet)
		{
			NKCTutorialReqTemplet value = item.Value;
			if (step == value.Step)
			{
				return CheckTutorial(value);
			}
		}
		return false;
	}

	public static void TutorialRequiredByLastPoint()
	{
		if (lastPoint != TutorialPoint.None)
		{
			TutorialRequired(lastPoint);
		}
	}

	public static TutorialStep TutorialRequired(TutorialPoint point, bool play = true)
	{
		lastPoint = TutorialPoint.None;
		foreach (KeyValuePair<int, NKCTutorialReqTemplet> item in m_dicReqTemplet)
		{
			NKCTutorialReqTemplet value = item.Value;
			if (value.EventPoint == point && CheckTutorial(value))
			{
				if (play)
				{
					PlayTutorial(value.EventID);
				}
				lastPoint = point;
				return value.Step;
			}
		}
		return TutorialStep.None;
	}

	public static bool TutorialCompleted(TutorialStep step)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return true;
		}
		return NKMTutorialManager.IsTutorialCompleted(step, myUserData);
	}

	public static bool EveryTutorialCompleted()
	{
		if (TutorialCompleted(TutorialStep.FactoryTuning))
		{
			return TutorialCompleted(TutorialStep.RaidEvent);
		}
		return false;
	}

	public static bool IsCloseDailyContents()
	{
		if (!TutorialCompleted(TutorialStep.Achieventment))
		{
			return !NKCContentManager.IsContentsUnlocked(ContentsType.LOBBY_SUBMENU);
		}
		return false;
	}

	public static void CompleteTutorial(TutorialStep step)
	{
		CompleteTutorial((int)step);
	}

	public static void CompleteTutorial(int missionID)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null && myUserData.m_MissionData.GetCompletedMissionData(missionID) == null)
		{
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionID);
			if (missionTemplet != null)
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(missionTemplet);
			}
		}
	}

	public static void PlayTutorial(int eventID)
	{
		Debug.Log("Tutorial Playing : " + eventID);
		if (NKCUIGameOption.IsInstanceOpen)
		{
			NKCUIGameOption.Instance.Close();
		}
		if (NKCGameEventManager.GetCurrentEventID() != eventID)
		{
			NKCGameEventManager.PlayGameEvent(eventID, isPauseEvent: false, null);
		}
		NKCMMPManager.OnPlayTutorial(eventID);
	}

	public static void PlayTutorial(TutorialStep step)
	{
		Debug.Log("Tutorial Playing : " + step);
		PlayTutorial((int)step);
	}

	public static bool LoadFromLua()
	{
		m_dicReqTemplet = NKMTempletLoader.LoadDictionary("ab_script", "LUA_TUTORIAL_REQ_TEMPLET", "m_TutorialReqTable", NKCTutorialReqTemplet.LoadFromLUA);
		return true;
	}

	public static bool CheckTutorial(NKCTutorialReqTemplet templet)
	{
		if (TutorialCompleted(templet.Step))
		{
			return false;
		}
		foreach (KeyValuePair<TutorialReq, string> item in templet.dicReq)
		{
			if (!CheckReq(templet.EventID, templet.Step, item.Key, item.Value))
			{
				return false;
			}
		}
		return true;
	}

	private static bool CheckReq(int eventID, TutorialStep step, TutorialReq req, string value)
	{
		if (req == TutorialReq.None)
		{
			return true;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		if (myUserData.m_eAuthLevel == NKM_USER_AUTH_LEVEL.SUPER_ADMIN)
		{
			return false;
		}
		switch (req)
		{
		case TutorialReq.EventClear:
		{
			TutorialStep result6;
			bool num3 = Enum.TryParse<TutorialStep>(value, out result6);
			int result7;
			bool flag3 = int.TryParse(value, out result7);
			if (num3)
			{
				if (!TutorialCompleted(result6))
				{
					return false;
				}
				break;
			}
			if (flag3)
			{
				if (!TutorialCompleted((TutorialStep)result7))
				{
					return false;
				}
				break;
			}
			return false;
		}
		case TutorialReq.DungeonClear:
			if (!myUserData.CheckDungeonClear(value))
			{
				return false;
			}
			break;
		case TutorialReq.WarfarePlay:
		{
			WarfareGameData warfareGameData4 = NKCScenManager.GetScenManager().WarfareGameData;
			if (warfareGameData4 == null)
			{
				return false;
			}
			NKMWarfareTemplet nKMWarfareTemplet3 = NKMWarfareTemplet.Find(warfareGameData4.warfareTempletID);
			if (nKMWarfareTemplet3 == null)
			{
				return false;
			}
			if (nKMWarfareTemplet3.m_WarfareStrID != value)
			{
				return false;
			}
			break;
		}
		case TutorialReq.WarfareClear:
			if (!myUserData.CheckWarfareClear(value))
			{
				return false;
			}
			break;
		case TutorialReq.WarfareUseSupply:
		{
			WarfareGameData warfareGameData2 = NKCScenManager.GetScenManager().WarfareGameData;
			if (warfareGameData2 == null)
			{
				return false;
			}
			NKMWarfareTemplet nKMWarfareTemplet2 = NKMWarfareTemplet.Find(warfareGameData2.warfareTempletID);
			if (nKMWarfareTemplet2 == null)
			{
				return false;
			}
			if (nKMWarfareTemplet2.m_WarfareStrID != value)
			{
				return false;
			}
			if (warfareGameData2.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
			{
				return false;
			}
			bool flag2 = false;
			foreach (WarfareUnitData value2 in warfareGameData2.warfareTeamDataA.warfareUnitDataByUIDMap.Values)
			{
				if (value2.hp > 0f && value2.supply < 2)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				return false;
			}
			break;
		}
		case TutorialReq.WarfareSupplyTile:
		{
			WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
			if (warfareGameData == null)
			{
				return false;
			}
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
			if (nKMWarfareTemplet == null)
			{
				return false;
			}
			if (nKMWarfareTemplet.m_WarfareStrID != value)
			{
				return false;
			}
			if (warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
			{
				return false;
			}
			bool flag = false;
			foreach (WarfareUnitData value3 in warfareGameData.warfareTeamDataA.warfareUnitDataByUIDMap.Values)
			{
				if (value3.hp != 0f)
				{
					WarfareTileData tileData = warfareGameData.GetTileData(value3.tileIndex);
					if (tileData != null && tileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_RESUPPLY)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
			break;
		}
		case TutorialReq.WarfareSquadCount:
		{
			WarfareGameData warfareGameData3 = NKCScenManager.GetScenManager().WarfareGameData;
			if (warfareGameData3 == null)
			{
				return false;
			}
			if (warfareGameData3.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
			{
				return false;
			}
			if (!int.TryParse(value, out var result4))
			{
				return false;
			}
			if (warfareGameData3.warfareTeamDataA.warfareUnitDataByUIDMap.Count < result4)
			{
				CompleteTutorial(step);
				return false;
			}
			break;
		}
		case TutorialReq.UnlockDeckCount:
			if (myUserData.m_ArmyData.GetUnlockedDeckCount(NKM_DECK_TYPE.NDT_NORMAL) == myUserData.m_ArmyData.GetMaxDeckCount(NKM_DECK_TYPE.NDT_NORMAL))
			{
				CompleteTutorial(step);
				return false;
			}
			break;
		case TutorialReq.DeckSlotEmpty:
		{
			string[] array = value.Split(',');
			List<int> list = new List<int>();
			for (int i = 0; i < array.Length; i++)
			{
				if (int.TryParse(array[i], out var result2))
				{
					list.Add(result2);
				}
			}
			if (list.Count < 1)
			{
				return false;
			}
			NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(NKM_DECK_TYPE.NDT_NORMAL, list[0]);
			if (deckData == null)
			{
				return false;
			}
			for (int j = 1; j < list.Count; j++)
			{
				int num2 = list[j];
				if (num2 < 0 || num2 >= deckData.m_listDeckUnitUID.Count)
				{
					return false;
				}
				if (deckData.m_listDeckUnitUID[num2] != 0L)
				{
					CompleteTutorial(step);
					return false;
				}
			}
			break;
		}
		case TutorialReq.WorldMapCityLevel:
		{
			if (!int.TryParse(value, out var result5))
			{
				return false;
			}
			foreach (NKMWorldMapCityData value4 in myUserData.m_WorldmapData.worldMapCityDataMap.Values)
			{
				if (value4.level >= result5)
				{
					return true;
				}
			}
			return false;
		}
		case TutorialReq.FierceCond:
		{
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			if (nKCFierceBattleSupportDataMgr != null)
			{
				NKCFierceBattleSupportDataMgr.FIERCE_STATUS status = nKCFierceBattleSupportDataMgr.GetStatus();
				if (int.TryParse(value, out var result3))
				{
					switch (status)
					{
					case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE:
						if (result3 == 1)
						{
							return true;
						}
						break;
					case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_REWARD:
					case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_COMPLETE:
						if (result3 == 2)
						{
							return true;
						}
						break;
					}
					return false;
				}
			}
			return false;
		}
		case TutorialReq.GuildDungeonCond:
		{
			int result;
			bool num = int.TryParse(value, out result);
			GuildDungeonState guildDungeonState = NKCGuildCoopManager.m_GuildDungeonState;
			if (num)
			{
				switch (guildDungeonState)
				{
				case GuildDungeonState.Invalid:
				case GuildDungeonState.SeasonOut:
				case GuildDungeonState.SessionOut:
					if (result == 0)
					{
						return true;
					}
					break;
				case GuildDungeonState.PlayableGuildDungeon:
					if (result == 1)
					{
						return true;
					}
					break;
				}
				return false;
			}
			return false;
		}
		}
		return true;
	}
}
