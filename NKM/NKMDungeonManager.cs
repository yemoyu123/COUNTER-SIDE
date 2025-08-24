using System;
using System.Collections.Generic;
using System.Text;
using ClientPacket.Common;
using Cs.Logging;
using Cs.Protocol;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKM;

public sealed class NKMDungeonManager
{
	private static Dictionary<long, NKMDungeonRespawnUnitTemplet> m_dicNKMDungeonRespawnUnitTemplet = new Dictionary<long, NKMDungeonRespawnUnitTemplet>();

	public static Dictionary<int, NKMDungeonTemplet> m_dicNKMDungeonTempletByID = new Dictionary<int, NKMDungeonTemplet>();

	public static Dictionary<string, NKMDungeonTemplet> m_dicNKMDungeonTempletByStrID = new Dictionary<string, NKMDungeonTemplet>();

	public static Dictionary<int, NKMDungeonEventDeckTemplet> m_dicNKMDungeonEventDeckTemplet = null;

	private static string m_DungeonTempletBaseFileName = "";

	private static string m_DungeonTempletFolderName = "";

	public static long CalculateDungeonRespawnUnitTempletUID(int dungeonID, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE dungeonRespawnUnitTempletType, int waveID, int respawnUnitCount)
	{
		return (long)dungeonID * 100000L + (long)dungeonRespawnUnitTempletType * 10000L + waveID * 100 + respawnUnitCount;
	}

	public static long AddNKMDungeonRespawnUnitTemplet(NKMDungeonRespawnUnitTemplet cNKMDungeonRespawnUnitTemplet, string dungeonName, int dungeonID, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE dungeonRespawnUnitTempletType, int waveID, int respawnUnitCount)
	{
		if (respawnUnitCount >= 100)
		{
			Log.ErrorAndExit($"[NKMDungeonRespawnUnitTemplet] RespawnUnitCount is Too Big - DungeonID[{dungeonID}:{dungeonName}] DungeonRespawnUnitTempletType[{dungeonRespawnUnitTempletType}] WaveID[{waveID}] RespawnUnitCount[{respawnUnitCount}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 1149);
			return 0L;
		}
		if (waveID >= 100)
		{
			Log.ErrorAndExit($"[NKMDungeonRespawnUnitTemplet] WaveID is Too Big - DungeonID[{dungeonID}:{dungeonName}] DungeonRespawnUnitTempletType[{dungeonRespawnUnitTempletType}] WaveID[{waveID}] RespawnUnitCount[{respawnUnitCount}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 1155);
			return 0L;
		}
		long num = CalculateDungeonRespawnUnitTempletUID(dungeonID, dungeonRespawnUnitTempletType, waveID, respawnUnitCount);
		if (m_dicNKMDungeonRespawnUnitTemplet.ContainsKey(num))
		{
			Log.ErrorAndExit($"[NKMDungeonRespawnUnitTemplet] Duplicate DungeonRespawnUnitTemplet UID[{num}]  DungeonID[{dungeonID}:{dungeonName}] dungeonRespawnUnitTempletType[{dungeonRespawnUnitTempletType}] respawnUnitCount[{respawnUnitCount}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 1162);
			return 0L;
		}
		m_dicNKMDungeonRespawnUnitTemplet.Add(num, cNKMDungeonRespawnUnitTemplet);
		return num;
	}

	public static NKMDungeonRespawnUnitTemplet GetNKMDungeonRespawnUnitTemplet(long UID)
	{
		if (!m_dicNKMDungeonRespawnUnitTemplet.ContainsKey(UID))
		{
			return null;
		}
		return m_dicNKMDungeonRespawnUnitTemplet[UID];
	}

	public static bool LoadFromLUA(string dungeonTempletBaseFileName, string dungeonTempletFolderName, bool bFullLoad)
	{
		m_dicNKMDungeonTempletByID.Clear();
		m_dicNKMDungeonTempletByStrID.Clear();
		m_DungeonTempletFolderName = dungeonTempletFolderName;
		LoadFromLUA_LUA_DUNGEON_TEMPLET_BASE(dungeonTempletBaseFileName);
		IEnumerator<NKMDungeonTempletBase> enumerator = NKMTempletContainer<NKMDungeonTempletBase>.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMDungeonTemplet nKMDungeonTemplet = new NKMDungeonTemplet
			{
				m_DungeonTempletBase = enumerator.Current
			};
			if (bFullLoad)
			{
				LoadFromLUA_LUA_DUNGEON_TEMPLET(nKMDungeonTemplet);
			}
			m_dicNKMDungeonTempletByID.Add(enumerator.Current.Key, nKMDungeonTemplet);
			m_dicNKMDungeonTempletByStrID.Add(enumerator.Current.m_DungeonStrID, nKMDungeonTemplet);
		}
		return true;
	}

	private static void LoadFromLUA_LUA_DUNGEON_TEMPLET_BASE(string dungeonTempletBaseFileName)
	{
		m_DungeonTempletBaseFileName = dungeonTempletBaseFileName;
		NKMTempletContainer<NKMDungeonTempletBase>.Load("AB_SCRIPT_DUNGEON_TEMPLET", m_DungeonTempletBaseFileName, "m_dicNKMDungeonTempletByStrID", NKMDungeonTempletBase.LoadFromLUA, (NKMDungeonTempletBase e) => e.m_DungeonStrID);
	}

	private static void LoadFromLUA_LUA_DUNGEON_TEMPLET(NKMDungeonTemplet cNKMDungeonTemplet)
	{
		if (cNKMDungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
		{
			return;
		}
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT_DUNGEON_TEMPLET_ALL", m_DungeonTempletFolderName + cNKMDungeonTemplet.m_DungeonTempletBase.m_DungeonTempletFileName))
		{
			if (nKMLua.OpenTable("NKMDungeonTemplet"))
			{
				cNKMDungeonTemplet.LoadFromLUA(nKMLua, cNKMDungeonTemplet.m_DungeonTempletBase);
				nKMLua.CloseTable();
			}
			else
			{
				Log.ErrorAndExit($"[DungeonTemplet] 데이터 로드 실패 m_DungeonID : {cNKMDungeonTemplet.m_DungeonTempletBase.m_DungeonID}, m_DungeonType : {cNKMDungeonTemplet.m_DungeonTempletBase.m_DungeonType}, m_DungeonTempletFileName : {cNKMDungeonTemplet.m_DungeonTempletBase.m_DungeonTempletFileName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 1234);
			}
		}
		nKMLua.LuaClose();
	}

	public static bool LoadFromLUA_EventDeckInfo()
	{
		m_dicNKMDungeonEventDeckTemplet = NKMTempletLoader.LoadDictionary("ab_script", "LUA_EVENTDECK_TEMPLET", "EVENTDECK_TEMPLET", NKMDungeonEventDeckTemplet.LoadFromLUA);
		return m_dicNKMDungeonEventDeckTemplet != null;
	}

	public static NKMDungeonEventDeckTemplet GetEventDeckTemplet(int id)
	{
		if (m_dicNKMDungeonEventDeckTemplet.TryGetValue(id, out var value))
		{
			return value;
		}
		return null;
	}

	public static bool IsEventDeckUnitStyleRight(NKMDungeonEventDeckTemplet.SLOT_TYPE slotType, NKM_UNIT_STYLE_TYPE styleType)
	{
		switch (slotType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
			return false;
		default:
			return true;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_COUNTER:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_SOLDIER:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_MECHANIC:
			return styleType == GetUnitStyleTypeFromEventDeckType(slotType);
		}
	}

	public static NKM_UNIT_STYLE_TYPE GetUnitStyleTypeFromEventDeckType(NKMDungeonEventDeckTemplet.SLOT_TYPE slotType)
	{
		return slotType switch
		{
			NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_COUNTER => NKM_UNIT_STYLE_TYPE.NUST_COUNTER, 
			NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_MECHANIC => NKM_UNIT_STYLE_TYPE.NUST_MECHANIC, 
			NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_SOLDIER => NKM_UNIT_STYLE_TYPE.NUST_SOLDIER, 
			_ => NKM_UNIT_STYLE_TYPE.NUST_INVALID, 
		};
	}

	public static int GetDungeonID(string dungeonStrID)
	{
		if (dungeonStrID == null)
		{
			return 0;
		}
		return GetDungeonTempletBase(dungeonStrID)?.m_DungeonID ?? 0;
	}

	public static List<string> GetTotalDungeonStrID()
	{
		List<string> list = new List<string>();
		Dictionary<int, NKMDungeonTemplet>.Enumerator enumerator = m_dicNKMDungeonTempletByID.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMDungeonTemplet value = enumerator.Current.Value;
			list.Add(value.m_DungeonTempletBase.m_DungeonStrID);
		}
		return list;
	}

	public static List<string> GetTotalDungeonStrIDExpectCutscene()
	{
		List<string> list = new List<string>();
		Dictionary<int, NKMDungeonTemplet>.Enumerator enumerator = m_dicNKMDungeonTempletByID.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMDungeonTemplet value = enumerator.Current.Value;
			if (value.m_DungeonTempletBase.m_DungeonType != NKM_DUNGEON_TYPE.NDT_CUTSCENE)
			{
				list.Add(value.m_DungeonTempletBase.m_DungeonStrID);
			}
		}
		return list;
	}

	public static int GetDungeonLevel(int dungeonID)
	{
		return GetDungeonTempletBase(dungeonID)?.m_DungeonLevel ?? 0;
	}

	public static NKMDungeonTempletBase GetDungeonTempletBase(int dungeonID)
	{
		if (m_dicNKMDungeonTempletByID.ContainsKey(dungeonID))
		{
			return m_dicNKMDungeonTempletByID[dungeonID].m_DungeonTempletBase;
		}
		return null;
	}

	public static NKMDungeonTempletBase GetDungeonTempletBase(string dungeonStrID)
	{
		if (m_dicNKMDungeonTempletByStrID.ContainsKey(dungeonStrID))
		{
			return m_dicNKMDungeonTempletByStrID[dungeonStrID].m_DungeonTempletBase;
		}
		return null;
	}

	public static NKMDungeonTemplet GetDungeonTemplet(int dungeonID)
	{
		if (m_dicNKMDungeonTempletByID.ContainsKey(dungeonID))
		{
			NKMDungeonTemplet nKMDungeonTemplet = m_dicNKMDungeonTempletByID[dungeonID];
			if (!nKMDungeonTemplet.m_bLoaded)
			{
				LoadFromLUA_LUA_DUNGEON_TEMPLET(nKMDungeonTemplet);
			}
			return nKMDungeonTemplet;
		}
		return null;
	}

	public static NKMDungeonTemplet GetDungeonTemplet(string dungeonStrID)
	{
		if (m_dicNKMDungeonTempletByStrID.ContainsKey(dungeonStrID))
		{
			NKMDungeonTemplet nKMDungeonTemplet = m_dicNKMDungeonTempletByStrID[dungeonStrID];
			if (!nKMDungeonTemplet.m_bLoaded)
			{
				LoadFromLUA_LUA_DUNGEON_TEMPLET(nKMDungeonTemplet);
			}
			return nKMDungeonTemplet;
		}
		return null;
	}

	public static bool MakeGameTeamData(NKMGameData cNKMGameData, NKMGameRuntimeData cNKMGameRuntimeData)
	{
		NKMDungeonTemplet dungeonTemplet = GetDungeonTemplet(cNKMGameData.m_DungeonID);
		if (dungeonTemplet == null)
		{
			Log.Error("No Exist NKMDungeonTemplet dungeonID: " + cNKMGameData.m_DungeonID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 1417);
			return false;
		}
		if (cNKMGameData.m_TeamASupply > 0)
		{
			cNKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost = 4f;
			if (cNKMGameData.m_NKMGameTeamDataA.m_MainShip != null)
			{
				NKMUnitTemplet unitTemplet = cNKMGameData.m_NKMGameTeamDataA.m_MainShip.GetUnitTemplet();
				if (unitTemplet != null)
				{
					cNKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost += unitTemplet.m_UnitTempletBase.m_StarGradeMax;
				}
			}
		}
		else
		{
			cNKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost = 0f;
		}
		cNKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost = dungeonTemplet.m_fStartCost;
		NKMMapTemplet mapTempletByStrID = NKMMapManager.GetMapTempletByStrID(dungeonTemplet.m_DungeonTempletBase.m_DungeonMapStrID);
		if (mapTempletByStrID == null)
		{
			Log.Error("No Exist cNKMMapTemplet m_DungeonMapStrID: " + dungeonTemplet.m_DungeonTempletBase.m_DungeonMapStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 1443);
			return false;
		}
		cNKMGameData.m_MapID = mapTempletByStrID.m_MapID;
		cNKMGameData.m_fDoubleCostTime = dungeonTemplet.m_DungeonTempletBase.m_fDoubleCostTime;
		if (dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_WAVE)
		{
			cNKMGameData.m_NKMGameTeamDataB.m_MainShip = null;
			cNKMGameData.m_NKMGameTeamDataB.m_LeaderUnitUID = 0L;
			cNKMGameData.m_NKMGameTeamDataB.m_UserLevel = GetFixedTeamBUnitLevel(dungeonTemplet.m_BossUnitLevel, cNKMGameData);
			cNKMGameData.m_NKMGameTeamDataB.m_UserNickname = "";
		}
		else
		{
			NKMUnitData nKMUnitData = new NKMUnitData();
			nKMUnitData.m_UnitUID = NpcUid.Get();
			NKMUnitTemplet unitTemplet2 = NKMUnitManager.GetUnitTemplet(dungeonTemplet.m_BossUnitStrID);
			if (unitTemplet2 != null)
			{
				nKMUnitData.m_UnitID = unitTemplet2.m_UnitTempletBase.m_UnitID;
				cNKMGameData.m_NKMGameTeamDataB.m_UserNickname = "";
			}
			nKMUnitData.m_UnitLevel = GetFixedTeamBUnitLevel(dungeonTemplet.m_BossUnitLevel, cNKMGameData);
			nKMUnitData.SetDungeonRespawnUnitTemplet(dungeonTemplet.m_BossRespawnUnitTemplet);
			cNKMGameData.m_NKMGameTeamDataB.m_MainShip = nKMUnitData;
			cNKMGameData.m_NKMGameTeamDataB.m_LeaderUnitUID = nKMUnitData.m_UnitUID;
			cNKMGameData.m_NKMGameTeamDataB.m_UserLevel = GetFixedTeamBUnitLevel(dungeonTemplet.m_BossUnitLevel, cNKMGameData);
		}
		cNKMGameData.m_NKMGameTeamDataB.m_listUnitData.Clear();
		for (int i = 0; i < dungeonTemplet.m_listDungeonDeck.Count; i++)
		{
			if (i == 8)
			{
				Log.Error("cNKMDungeonTemplet.m_listDungeonDeck over 8 : " + dungeonTemplet.m_listDungeonDeck.Count, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 1485);
				break;
			}
			NKMDungeonRespawnUnitTemplet nKMDungeonRespawnUnitTemplet = dungeonTemplet.m_listDungeonDeck[i];
			if (nKMDungeonRespawnUnitTemplet == null)
			{
				Log.Error("cNKMDungeonUnitTemplet null dungeonID: " + cNKMGameData.m_DungeonID + " i: " + i, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 1492);
			}
			else
			{
				AddNewRespawnUnitTempletToUnitDataList(cNKMGameData, nKMDungeonRespawnUnitTemplet, -1, ref cNKMGameData.m_NKMGameTeamDataB.m_listUnitData);
			}
		}
		cNKMGameData.m_NKMGameTeamDataA.m_listEvevtUnitData.Clear();
		for (int j = 0; j < dungeonTemplet.m_listDungeonUnitRespawnA.Count; j++)
		{
			NKMDungeonRespawnUnitTemplet nKMDungeonRespawnUnitTemplet2 = dungeonTemplet.m_listDungeonUnitRespawnA[j];
			if (nKMDungeonRespawnUnitTemplet2 != null)
			{
				AddNewRespawnUnitTempletToUnitDataList(cNKMGameData, nKMDungeonRespawnUnitTemplet2, -1, ref cNKMGameData.m_NKMGameTeamDataA.m_listEvevtUnitData);
			}
		}
		cNKMGameData.m_NKMGameTeamDataB.m_listEvevtUnitData.Clear();
		foreach (NKMDungeonRespawnUnitTemplet item in dungeonTemplet.m_listDungeonUnitRespawnB)
		{
			AddNewRespawnUnitTempletToUnitDataList(cNKMGameData, item, -1, ref cNKMGameData.m_NKMGameTeamDataB.m_listEvevtUnitData);
		}
		for (int k = 0; k < dungeonTemplet.m_listDungeonWave.Count; k++)
		{
			foreach (NKMDungeonRespawnUnitTemplet item2 in dungeonTemplet.m_listDungeonWave[k].m_listDungeonUnitRespawnA)
			{
				int level = item2.m_UnitLevel + k * item2.m_UnitLevelBonusPerWave;
				AddNewRespawnUnitTempletToUnitDataList(cNKMGameData, item2, level, ref cNKMGameData.m_NKMGameTeamDataA.m_listEvevtUnitData);
			}
			foreach (NKMDungeonRespawnUnitTemplet item3 in dungeonTemplet.m_listDungeonWave[k].m_listDungeonUnitRespawnB)
			{
				int level2 = item3.m_UnitLevel + k * item3.m_UnitLevelBonusPerWave;
				AddNewRespawnUnitTempletToUnitDataList(cNKMGameData, item3, level2, ref cNKMGameData.m_NKMGameTeamDataB.m_listEvevtUnitData);
			}
		}
		foreach (int key in cNKMGameData.m_BattleConditionIDs.Keys)
		{
			NKMBattleConditionTemplet templetByID = NKMBattleConditionManager.GetTempletByID(key);
			if (templetByID != null)
			{
				MakeEnviromentUnitData(templetByID.AllyBCondUnitStrIDList, cNKMGameData, cNKMGameData.m_NKMGameTeamDataA);
				MakeEnviromentUnitData(templetByID.EnemyBCondUnitStrIDList, cNKMGameData, cNKMGameData.m_NKMGameTeamDataB);
			}
		}
		return true;
	}

	public static void AddNewRespawnUnitTempletToUnitDataList(NKMGameData cNKMGameData, NKMDungeonRespawnUnitTemplet cNKMDungeonUnitTemplet, int level, ref List<NKMUnitData> unitDataList)
	{
		if (cNKMDungeonUnitTemplet == null)
		{
			return;
		}
		if (level == -1)
		{
			level = cNKMDungeonUnitTemplet.m_UnitLevel;
		}
		NKMUnitData nKMUnitData = new NKMUnitData();
		nKMUnitData.m_UnitUID = NpcUid.Get();
		nKMUnitData.m_UnitID = NKMUnitManager.GetUnitID(cNKMDungeonUnitTemplet.m_UnitStrID);
		nKMUnitData.m_SkinID = cNKMDungeonUnitTemplet.m_SkinID;
		nKMUnitData.m_UnitLevel = GetFixedTeamBUnitLevel(level, cNKMGameData);
		nKMUnitData.m_LimitBreakLevel = cNKMDungeonUnitTemplet.m_UnitLimitBreakLevel;
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(nKMUnitData.m_UnitID);
		nKMUnitData.tacticLevel = Math.Min(cNKMDungeonUnitTemplet.m_TacticUpdateLevel, 6);
		if (cNKMDungeonUnitTemplet.m_SkillLevel > 0)
		{
			nKMUnitData.FillSkillLevelByUnitID(nKMUnitData.m_UnitID);
			if (nKMUnitTempletBase != null)
			{
				int skillCount = nKMUnitTempletBase.GetSkillCount();
				for (int i = 0; i < skillCount; i++)
				{
					string skillStrID = nKMUnitTempletBase.GetSkillStrID(i);
					nKMUnitData.m_aUnitSkillLevel[i] = Math.Min(NKMUnitSkillManager.GetMaxSkillLevel(skillStrID), cNKMDungeonUnitTemplet.m_SkillLevel);
				}
			}
		}
		if (cNKMDungeonUnitTemplet.m_bReactor)
		{
			NKMUnitReactorTemplet nKMUnitReactorTemplet = NKMUnitReactorTemplet.Find(nKMUnitTempletBase.m_ReactorId);
			if (nKMUnitReactorTemplet != null)
			{
				nKMUnitData.reactorLevel = nKMUnitReactorTemplet.GetMaxReactorLevel();
			}
		}
		nKMUnitData.SetDungeonRespawnUnitTemplet(cNKMDungeonUnitTemplet);
		unitDataList.Add(nKMUnitData);
	}

	private static void MakeEnviromentUnitData(ICollection<string> unitStrIDList, NKMGameData cNKMGameData, NKMGameTeamData cNKMGameTeamData)
	{
		if (cNKMGameData == null || cNKMGameTeamData == null)
		{
			return;
		}
		foreach (string unitStrID in unitStrIDList)
		{
			NKMUnitData nKMUnitData = new NKMUnitData();
			nKMUnitData.m_UnitUID = NpcUid.Get();
			nKMUnitData.m_UnitID = NKMUnitManager.GetUnitID(unitStrID);
			cNKMGameTeamData.m_listEnvUnitData.Add(nKMUnitData);
		}
	}

	public static void MakeOperatorUnitData(NKMGameTeamData cNKMGameTeamData)
	{
		if (cNKMGameTeamData == null)
		{
			return;
		}
		cNKMGameTeamData.m_listOperatorUnitData.Clear();
		NKMOperator nKMOperator = cNKMGameTeamData.m_Operator;
		if (nKMOperator == null)
		{
			return;
		}
		NKMTacticalCommandTemplet tacticalCommandTemplet = nKMOperator.GetTacticalCommandTemplet();
		if (tacticalCommandTemplet == null || string.IsNullOrEmpty(tacticalCommandTemplet.m_UnitStrID))
		{
			return;
		}
		int unitID = NKMUnitManager.GetUnitID(tacticalCommandTemplet.m_UnitStrID);
		if (unitID == 0)
		{
			Log.Error("Operator Unit " + tacticalCommandTemplet.m_UnitStrID + " not found!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 1639);
			return;
		}
		NKMUnitData nKMUnitData = MakeUnitDataFromID(unitID, NpcUid.Get(), nKMOperator.level, 13, 0);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(nKMUnitData);
		int level = nKMOperator.mainSkill.level;
		for (int i = 0; i < nKMUnitData.m_aUnitSkillLevel.Length; i++)
		{
			int maxSkillLevel = NKMUnitSkillManager.GetMaxSkillLevel(unitTempletBase.GetSkillStrID(i));
			nKMUnitData.m_aUnitSkillLevel[i] = ((level > maxSkillLevel) ? maxSkillLevel : level);
		}
		cNKMGameTeamData.m_listOperatorUnitData.Add(nKMUnitData);
	}

	public static NKM_ERROR_CODE CheckCommon(NKMUserData cNKMUserData, NKMDungeonTempletBase dungeonTempletBase)
	{
		if (cNKMUserData.m_UserLevel < dungeonTempletBase.m_DGLimitUserLevel)
		{
			return NKM_ERROR_CODE.NEC_FAIL_REQUIRE_MORE_USER_LEVEL;
		}
		if (!cNKMUserData.m_ArmyData.CanGetMoreUnit(1))
		{
			return NKM_ERROR_CODE.NEC_FAIL_ARMY_FULL;
		}
		if (!cNKMUserData.m_ArmyData.CanGetMoreShip(0))
		{
			return NKM_ERROR_CODE.NEC_FAIL_SHIP_FULL;
		}
		if (!cNKMUserData.m_InventoryData.CanGetMoreEquipItem(1))
		{
			return NKM_ERROR_CODE.NEC_FAIL_EQUIP_ITEM_FULL;
		}
		if (!cNKMUserData.m_ArmyData.CanGetMoreOperator(0))
		{
			return NKM_ERROR_CODE.NEC_FAIL_OPERATOR_FULL;
		}
		if (!cNKMUserData.m_ArmyData.CanGetMoreTrophy(1))
		{
			return NKM_ERROR_CODE.NEC_FAIL_TROPHY_FULL;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CheckEventSlot(NKMArmyData armyData, NKMDungeonEventDeckTemplet eventDeckTemplet, NKMDungeonEventDeckTemplet.EventDeckSlot eventDeckSlotData, long targetUnitUID, NKM_UNIT_TYPE unitType)
	{
		switch (eventDeckSlotData.m_eType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
			if (unitType == NKM_UNIT_TYPE.NUT_SHIP)
			{
				return NKM_ERROR_CODE.NEC_FAIL_DECK_NO_SHIP;
			}
			if (targetUnitUID != 0L)
			{
				return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_SLOT_UNREQUIRED_DATA;
			}
			return NKM_ERROR_CODE.NEC_OK;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
			if (targetUnitUID != 0L)
			{
				return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_SLOT_UNREQUIRED_DATA;
			}
			return NKM_ERROR_CODE.NEC_OK;
		default:
			if (targetUnitUID == 0L)
			{
				if (eventDeckSlotData.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST)
				{
					return NKM_ERROR_CODE.NEC_OK;
				}
				return unitType switch
				{
					NKM_UNIT_TYPE.NUT_SHIP => NKM_ERROR_CODE.NEC_FAIL_DECK_NO_SHIP, 
					NKM_UNIT_TYPE.NUT_OPERATOR => NKM_ERROR_CODE.NEC_OK, 
					_ => NKM_ERROR_CODE.NEC_FAIL_DECK_UNIT_INVALID, 
				};
			}
			if (unitType == NKM_UNIT_TYPE.NUT_OPERATOR)
			{
				NKMOperator operatorFromUId = armyData.GetOperatorFromUId(targetUnitUID);
				if (operatorFromUId == null)
				{
					return NKM_ERROR_CODE.NEC_FAIL_OPERATOR_INVALID_UNIT_UID;
				}
				NKMDungeonEventDeckTemplet.SLOT_TYPE eType = eventDeckSlotData.m_eType;
				if ((uint)(eType - 2) <= 1u && eventDeckSlotData.m_ID != operatorFromUId.id)
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_SLOT_DIFFRENT_UNIT;
				}
				if (NKMUnitManager.GetUnitTempletBase(operatorFromUId) == null)
				{
					return NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_BASE_TEMPLET_NULL;
				}
			}
			else
			{
				NKMUnitData nKMUnitData;
				switch (unitType)
				{
				case NKM_UNIT_TYPE.NUT_NORMAL:
					nKMUnitData = armyData.GetUnitOrTrophyFromUID(targetUnitUID);
					break;
				case NKM_UNIT_TYPE.NUT_SHIP:
					nKMUnitData = armyData.GetShipFromUID(targetUnitUID);
					break;
				default:
					return NKM_ERROR_CODE.NEC_FAIL_EVENT_SLOT_UNIT_TYPE_INVALID;
				}
				if (nKMUnitData == null)
				{
					return NKM_ERROR_CODE.NEC_FAIL_DECK_UNIT_INVALID;
				}
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(nKMUnitData);
				if (unitTempletBase == null)
				{
					return NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_BASE_TEMPLET_NULL;
				}
				NKMDungeonEventDeckTemplet.SLOT_TYPE eType = eventDeckSlotData.m_eType;
				if ((uint)(eType - 2) <= 1u && !unitTempletBase.IsSameBaseUnit(eventDeckSlotData.m_ID))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_SLOT_DIFFRENT_UNIT;
				}
				if (!IsEventDeckUnitStyleRight(eventDeckSlotData.m_eType, unitTempletBase.m_NKM_UNIT_STYLE_TYPE))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_SLOT_DIFFRENT_UNIT;
				}
			}
			return NKM_ERROR_CODE.NEC_OK;
		}
	}

	public static NKM_ERROR_CODE IsValidEventDeck(NKMArmyData armyData, NKMDungeonEventDeckTemplet eventDeckTemplet, NKMDeckCondition deckCondition, NKMEventDeckData eventDeckData)
	{
		if (eventDeckTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_EVENT_DECK_TEMPLET_NULL;
		}
		if (eventDeckData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_NOT_EXIST;
		}
		Dictionary<int, long> dicUnit = eventDeckData.m_dicUnit;
		long shipUID = eventDeckData.m_ShipUID;
		NKM_ERROR_CODE nKM_ERROR_CODE = CheckEventSlot(armyData, eventDeckTemplet, eventDeckTemplet.ShipSlot, shipUID, NKM_UNIT_TYPE.NUT_SHIP);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		if (NKMOpenTagManager.IsOpened("OPERATOR"))
		{
			NKM_ERROR_CODE nKM_ERROR_CODE2 = CheckEventSlot(armyData, eventDeckTemplet, eventDeckTemplet.OperatorSlot, eventDeckData.m_OperatorUID, NKM_UNIT_TYPE.NUT_OPERATOR);
			if (nKM_ERROR_CODE2 != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE2;
			}
		}
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<int> hashSet2 = new HashSet<int>();
		for (int i = 0; i < 8; i++)
		{
			NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = eventDeckTemplet.GetUnitSlot(i);
			if (!dicUnit.TryGetValue(i, out var value))
			{
				value = 0L;
			}
			NKM_ERROR_CODE nKM_ERROR_CODE3 = CheckEventSlot(armyData, eventDeckTemplet, unitSlot, value, NKM_UNIT_TYPE.NUT_NORMAL);
			if (nKM_ERROR_CODE3 != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE3;
			}
			switch (unitSlot.m_eType)
			{
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
				if (NKMUnitManager.CheckContainsBaseUnit(hashSet, unitSlot.m_ID))
				{
					return NKM_ERROR_CODE.NEC_FAIL_DECK_DUPLICATE_UNIT;
				}
				hashSet.Add(unitSlot.m_ID);
				break;
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_COUNTER:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_SOLDIER:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_MECHANIC:
			{
				NKMUnitData unitFromUID = armyData.GetUnitFromUID(value);
				if (NKMUnitManager.CheckContainsBaseUnit(hashSet, unitFromUID.m_UnitID))
				{
					return NKM_ERROR_CODE.NEC_FAIL_DECK_DUPLICATE_UNIT;
				}
				if (unitFromUID.IsSeized)
				{
					return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED;
				}
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID);
				if (unitTempletBase == null)
				{
					return NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_BASE_TEMPLET_NULL;
				}
				if (!IsEventDeckUnitStyleRight(unitSlot.m_eType, unitTempletBase.m_NKM_UNIT_STYLE_TYPE))
				{
					return NKM_ERROR_CODE.NEC_FAIL_EVENTDECK_SLOT_DIFFRENT_UNIT;
				}
				hashSet.Add(unitFromUID.m_UnitID);
				hashSet2.Add(unitFromUID.m_UnitID);
				break;
			}
			}
		}
		if (eventDeckTemplet.HasRandomUnitSlot())
		{
			HashSet<int> hashSet3 = new HashSet<int>();
			foreach (NKMDungeonEventDeckTemplet.EventDeckSlot item in eventDeckTemplet.m_lstUnitSlot)
			{
				if (item.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
				{
					List<int> connectedUnitList = item.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_A1);
					List<int> connectedUnitList2 = item.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_B1);
					if (connectedUnitList != null)
					{
						hashSet3.UnionWith(connectedUnitList);
					}
					if (connectedUnitList2 != null)
					{
						hashSet3.UnionWith(connectedUnitList2);
					}
				}
			}
			foreach (int item2 in hashSet3)
			{
				if (NKMUnitManager.CheckContainsBaseUnit(hashSet2, item2))
				{
					return NKM_ERROR_CODE.NEC_FAIL_DECK_DUPLICATE_UNIT;
				}
			}
		}
		return deckCondition?.CheckEventDeckCondition(armyData, eventDeckTemplet, eventDeckData) ?? NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE IsValidDeckCondition(NKMArmyData armyData, NKMDeckCondition deckCondition, NKMDeckIndex deckIndex)
	{
		if (deckCondition == null)
		{
			return NKM_ERROR_CODE.NEC_OK;
		}
		NKMDeckData deckData = armyData.GetDeckData(deckIndex);
		if (deckData == null)
		{
			return NKM_ERROR_CODE.NEC_DB_FAIL_DECK_DATA;
		}
		return deckCondition.CheckDeckCondition(armyData, deckData);
	}

	public static NKMUnitData MakeUnitDataFromID(int unitID, long unitUid, int level, int limitBreakLevel, int skinID, int tacticLevel = 0, int reactorLevel = -1, int skillLevel = -1)
	{
		NKMUnitData nKMUnitData = new NKMUnitData();
		nKMUnitData.m_UnitID = unitID;
		nKMUnitData.m_UnitUID = unitUid;
		nKMUnitData.m_UnitLevel = level;
		nKMUnitData.m_SkinID = skinID;
		nKMUnitData.tacticLevel = tacticLevel;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (limitBreakLevel == -1)
		{
			nKMUnitData.m_LimitBreakLevel = (short)NKMUnitLimitBreakManager.GetMaxLimitBreakLevelByUnitLevel(unitTempletBase, level);
		}
		else
		{
			nKMUnitData.m_LimitBreakLevel = (short)limitBreakLevel;
		}
		if (unitTempletBase.IsReactorUnit)
		{
			if (reactorLevel < 0)
			{
				if (level >= 110)
				{
					NKMUnitReactorTemplet nKMUnitReactorTemplet = NKMUnitReactorTemplet.Find(unitTempletBase.m_ReactorId);
					if (nKMUnitReactorTemplet != null)
					{
						nKMUnitData.reactorLevel = nKMUnitReactorTemplet.GetMaxReactorLevel();
					}
				}
				else
				{
					nKMUnitData.reactorLevel = 0;
				}
			}
			else
			{
				nKMUnitData.reactorLevel = reactorLevel;
			}
		}
		if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			int skillCount = unitTempletBase.GetSkillCount();
			for (int i = 0; i < skillCount; i++)
			{
				NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(unitTempletBase.GetSkillStrID(i), 1);
				if (skillTemplet != null)
				{
					int num = NKMUnitSkillManager.GetMaxSkillLevelFromLimitBreakLevel(skillTemplet.m_ID, nKMUnitData.m_LimitBreakLevel);
					if (skillLevel > 0)
					{
						num = Math.Min(skillLevel, num);
					}
					if (NKMUnitSkillManager.GetSkillTemplet(skillTemplet.m_ID, num) != null)
					{
						nKMUnitData.m_aUnitSkillLevel[i] = num;
					}
					else
					{
						nKMUnitData.m_aUnitSkillLevel[i] = 1;
					}
				}
			}
			for (int j = 0; j < nKMUnitData.m_listStatEXP.Count; j++)
			{
				nKMUnitData.m_listStatEXP[j] = NKMEnhanceManager.CalculateMaxEXP(nKMUnitData, (NKM_STAT_TYPE)j);
			}
		}
		return nKMUnitData;
	}

	public static NKMUnitData MakeUnitData(NKMAsyncUnitData asyncUnitData)
	{
		if (asyncUnitData == null)
		{
			return null;
		}
		return MakeUnitDataFromID(asyncUnitData.unitId, asyncUnitData.unitUid, asyncUnitData.unitLevel, asyncUnitData.limitBreakLevel, asyncUnitData.skinId, asyncUnitData.tacticLevel, asyncUnitData.reactorLevel);
	}

	public static NKMUnitData MakeUnitData(NKMAsyncUnitData asyncUnitData, long overrideUnidUid)
	{
		if (asyncUnitData == null)
		{
			return null;
		}
		return MakeUnitDataFromID(asyncUnitData.unitId, overrideUnidUid, asyncUnitData.unitLevel, asyncUnitData.limitBreakLevel, asyncUnitData.skinId, asyncUnitData.tacticLevel, asyncUnitData.reactorLevel);
	}

	public static NKMUnitData MakeUnitData(NKMDungeonEventDeckTemplet.EventDeckSlot eventDeckSlot)
	{
		return MakeUnitDataFromID(eventDeckSlot.m_ID, NpcUid.Get(), eventDeckSlot.m_Level, -1, eventDeckSlot.m_SkinID, eventDeckSlot.m_TacticLevel);
	}

	public static NKMOperator MakeOperatorDataFromID(int unitID, long unitUid, int level, int subSkillID)
	{
		NKMOperator nKMOperator = new NKMOperator();
		nKMOperator.uid = unitUid;
		nKMOperator.id = unitID;
		nKMOperator.level = level;
		nKMOperator.mainSkill = new NKMOperatorSkill();
		nKMOperator.subSkill = new NKMOperatorSkill();
		NKMOperatorSkillTemplet nKMOperatorSkillTemplet = NKMTempletContainer<NKMOperatorSkillTemplet>.Find(NKMUnitManager.GetUnitTempletBase(unitID).m_lstSkillStrID[0]);
		if (nKMOperatorSkillTemplet != null)
		{
			nKMOperator.mainSkill.id = nKMOperatorSkillTemplet.m_OperSkillID;
			nKMOperator.mainSkill.level = (byte)nKMOperatorSkillTemplet.m_MaxSkillLevel;
		}
		NKMOperatorSkillTemplet nKMOperatorSkillTemplet2 = NKMTempletContainer<NKMOperatorSkillTemplet>.Find(subSkillID);
		if (nKMOperatorSkillTemplet2 != null)
		{
			nKMOperator.subSkill.id = subSkillID;
			nKMOperator.subSkill.level = (byte)nKMOperatorSkillTemplet2.m_MaxSkillLevel;
		}
		return nKMOperator;
	}

	public static NKMUnitData MakeEventDeckUnit(NKMArmyData cNKMArmyData, NKMDungeonEventDeckTemplet eventDeckTemplet, NKMDeckCondition deckCondition, NKMDungeonEventDeckTemplet.EventDeckSlot eventDeckSlot, long unitUid, NKM_UNIT_TYPE unitType, NKM_TEAM_TYPE eTeam, HashSet<int> hsExcludeRandomUnit = null, bool bSkipRandomSlot = false)
	{
		switch (eventDeckSlot.m_eType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
			return null;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
		{
			if (bSkipRandomSlot)
			{
				return null;
			}
			int unit = NKMUnitListTemplet.Find(eventDeckSlot.m_ID).GetUnit(eTeam, hsExcludeRandomUnit);
			if (unit <= 0)
			{
				Log.Error($"Could not pick random unit from list! NKMUnitListTemplet ID {eventDeckSlot.m_ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 2098);
				return null;
			}
			return MakeUnitDataFromID(unit, NpcUid.Get(), eventDeckSlot.m_Level, -1, eventDeckSlot.m_SkinID, eventDeckSlot.m_TacticLevel);
		}
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
			return MakeUnitDataFromID(eventDeckSlot.m_ID, NpcUid.Get(), eventDeckSlot.m_Level, -1, eventDeckSlot.m_SkinID, eventDeckSlot.m_TacticLevel);
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_COUNTER:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_SOLDIER:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_MECHANIC:
		{
			if (eventDeckSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST && unitUid == 0L)
			{
				return MakeUnitDataFromID(eventDeckSlot.m_ID, NpcUid.Get(), eventDeckSlot.m_Level, -1, eventDeckSlot.m_SkinID, eventDeckSlot.m_TacticLevel);
			}
			NKMUnitData nKMUnitData = ((unitType != NKM_UNIT_TYPE.NUT_SHIP) ? cNKMArmyData.GetUnitFromUID(unitUid) : cNKMArmyData.GetShipFromUID(unitUid));
			if (nKMUnitData != null)
			{
				NKMUnitData nKMUnitData2 = new NKMUnitData();
				nKMUnitData2.DeepCopyFrom(nKMUnitData);
				if (deckCondition != null)
				{
					NKMDeckCondition.GameCondition gameCondition = deckCondition.GetGameCondition(NKMDeckCondition.GAME_CONDITION.LEVEL_CAP);
					if (gameCondition != null && nKMUnitData2.m_UnitLevel > gameCondition.Value)
					{
						nKMUnitData2.m_UnitLevel = gameCondition.Value;
					}
					if (deckCondition.GetGameCondition(NKMDeckCondition.GAME_CONDITION.FORCE_REARM_TO_BASIC) != null)
					{
						NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(nKMUnitData);
						if (unitTempletBase != null && unitTempletBase.IsRearmUnit)
						{
							nKMUnitData2.m_UnitID = unitTempletBase.m_BaseUnitID;
							NKMUnitTempletBase baseUnit = unitTempletBase.BaseUnit;
							for (int i = 0; i < baseUnit.GetSkillCount(); i++)
							{
								int skillLevelByIndex = nKMUnitData.GetSkillLevelByIndex(i);
								int maxSkillLevel = NKMUnitSkillManager.GetMaxSkillLevel(baseUnit.GetSkillStrID(i));
								if (skillLevelByIndex > maxSkillLevel)
								{
									nKMUnitData2.m_aUnitSkillLevel[i] = maxSkillLevel;
								}
							}
						}
					}
				}
				if (eventDeckSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST && eventDeckSlot.m_SkinID != 0)
				{
					nKMUnitData2.m_SkinID = eventDeckSlot.m_SkinID;
				}
				if (eventDeckSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED)
				{
					nKMUnitData2.tacticLevel = eventDeckSlot.m_TacticLevel;
				}
				if (eventDeckSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST || eventDeckSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE)
				{
					nKMUnitData2.tacticLevel = ((eventDeckSlot.m_TacticLevel > nKMUnitData.tacticLevel) ? eventDeckSlot.m_TacticLevel : nKMUnitData.tacticLevel);
				}
				return nKMUnitData2;
			}
			return null;
		}
		default:
			return null;
		}
	}

	public static NKMOperator MakeEventDeckOperator(NKMArmyData cNKMArmyData, NKMDungeonEventDeckTemplet eventDeckTemplet, NKMDeckCondition deckCondition, NKMDungeonEventDeckTemplet.EventDeckSlot eventDeckSlot, long unitUid, NKM_TEAM_TYPE eTeam)
	{
		switch (eventDeckSlot.m_eType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
		{
			int unit = NKMUnitListTemplet.Find(eventDeckSlot.m_ID).GetUnit(eTeam);
			if (unit <= 0)
			{
				Log.Error($"Operator random unit list not found! NKMUnitListTemplet ID {eventDeckSlot.m_ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 2225);
				return null;
			}
			return MakeOperatorDataFromID(unit, NpcUid.Get(), eventDeckSlot.m_Level, eventDeckTemplet.OperatorSubSkillID);
		}
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
			return null;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
			return MakeOperatorDataFromID(eventDeckSlot.m_ID, NpcUid.Get(), eventDeckSlot.m_Level, eventDeckTemplet.OperatorSubSkillID);
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_COUNTER:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_SOLDIER:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_MECHANIC:
		{
			if (eventDeckSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST && unitUid == 0L)
			{
				return MakeOperatorDataFromID(eventDeckSlot.m_ID, NpcUid.Get(), eventDeckSlot.m_Level, eventDeckTemplet.OperatorSubSkillID);
			}
			NKMOperator operatorFromUId = cNKMArmyData.GetOperatorFromUId(unitUid);
			if (operatorFromUId != null)
			{
				NKMOperator nKMOperator = new NKMOperator();
				nKMOperator.DeepCopyFrom(operatorFromUId);
				if (deckCondition != null)
				{
					NKMDeckCondition.GameCondition gameCondition = deckCondition.GetGameCondition(NKMDeckCondition.GAME_CONDITION.LEVEL_CAP);
					if (gameCondition != null && nKMOperator.level > gameCondition.Value)
					{
						nKMOperator.level = gameCondition.Value;
					}
				}
				return nKMOperator;
			}
			return null;
		}
		default:
			return null;
		}
	}

	public static bool IsEventDeckSelectRequired(NKMStageTempletV2 stageTemplet, bool bOperatorEnabled)
	{
		STAGE_TYPE sTAGE_TYPE = stageTemplet.m_STAGE_TYPE;
		if (sTAGE_TYPE != STAGE_TYPE.ST_DUNGEON && sTAGE_TYPE == STAGE_TYPE.ST_PHASE)
		{
			if (stageTemplet.PhaseTemplet != null)
			{
				return IsEventDeckSelectRequired(stageTemplet.PhaseTemplet.EventDeckTemplet, bOperatorEnabled);
			}
		}
		else if (stageTemplet.DungeonTempletBase != null)
		{
			return IsEventDeckSelectRequired(stageTemplet.DungeonTempletBase.EventDeckTemplet, bOperatorEnabled);
		}
		return false;
	}

	public static bool IsEventDeckSelectRequired(NKMDungeonEventDeckTemplet eventDeckTemplet, bool bOperatorEnabled)
	{
		NKMDungeonEventDeckTemplet.SLOT_TYPE eType;
		foreach (NKMDungeonEventDeckTemplet.EventDeckSlot item in eventDeckTemplet.m_lstUnitSlot)
		{
			eType = item.m_eType;
			if (eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
			{
				return true;
			}
		}
		eType = eventDeckTemplet.ShipSlot.m_eType;
		if (eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
		{
			return true;
		}
		if (bOperatorEnabled)
		{
			eType = eventDeckTemplet.OperatorSlot.m_eType;
			if (eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
			{
				return true;
			}
		}
		return false;
	}

	public static NKMUnitData MakeDeckShipData(NKMArmyData armyData, NKMDeckData deckData, NKMGameData gameData)
	{
		NKMUnitData nKMUnitData = null;
		NKMUnitData shipFromUID = armyData.GetShipFromUID(deckData.m_ShipUID);
		if (shipFromUID != null)
		{
			nKMUnitData = new NKMUnitData();
			nKMUnitData.DeepCopyFrom(shipFromUID);
		}
		return nKMUnitData;
	}

	public static NKMOperator MakeDeckOperatorData(NKMArmyData armyData, NKMDeckData deckData, NKMGameData gameData)
	{
		NKMOperator operatorFromUId = armyData.GetOperatorFromUId(deckData.m_OperatorUID);
		if (operatorFromUId == null)
		{
			return null;
		}
		NKMOperator nKMOperator = new NKMOperator();
		nKMOperator.DeepCopyFrom(operatorFromUId);
		return nKMOperator;
	}

	public static List<GameUnitData> MakeDeckUnitDataList(NKMArmyData armyData, NKMDeckData deckData, NKMInventoryData inventoryData)
	{
		List<GameUnitData> list = new List<GameUnitData>();
		for (int i = 0; i < deckData.m_listDeckUnitUID.Count; i++)
		{
			if (deckData.m_listDeckUnitUID[i] == 0L)
			{
				continue;
			}
			GameUnitData gameUnitData = new GameUnitData();
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(deckData.m_listDeckUnitUID[i]);
			if (unitFromUID != null)
			{
				NKMUnitData nKMUnitData = new NKMUnitData();
				nKMUnitData.DeepCopyFrom(unitFromUID);
				gameUnitData.unit = nKMUnitData;
			}
			for (int j = 0; j < 4; j++)
			{
				long equipUid = gameUnitData.unit.GetEquipUid((ITEM_EQUIP_POSITION)j);
				if (equipUid > 0)
				{
					NKMEquipItemData itemEquip = inventoryData.GetItemEquip(equipUid);
					if (itemEquip != null)
					{
						NKMEquipItemData nKMEquipItemData = new NKMEquipItemData();
						nKMEquipItemData.DeepCopyFrom(itemEquip);
						gameUnitData.equip_item_list.Add(nKMEquipItemData);
					}
				}
			}
			list.Add(gameUnitData);
		}
		return list;
	}

	public static NKMOperator MakeEventDeckOperatorData(NKMArmyData cNKMArmyData, NKMDungeonEventDeckTemplet eventDeckTemplet, NKMDeckCondition deckCondition, NKMEventDeckData eventDeckData, NKM_TEAM_TYPE eTeam = NKM_TEAM_TYPE.NTT_A1, bool bSkipRandomSlot = false)
	{
		if (!NKMOpenTagManager.IsOpened("OPERATOR"))
		{
			return null;
		}
		if (bSkipRandomSlot && eventDeckTemplet.OperatorSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
		{
			return null;
		}
		return MakeEventDeckOperator(cNKMArmyData, eventDeckTemplet, deckCondition, eventDeckTemplet.OperatorSlot, eventDeckData.m_OperatorUID, eTeam);
	}

	public static NKMUnitData MakeEventDeckShipData(NKMArmyData cNKMArmyData, NKMDungeonEventDeckTemplet eventDeckTemplet, NKMDeckCondition deckCondition, NKMEventDeckData eventDeckData, NKM_TEAM_TYPE eTeam = NKM_TEAM_TYPE.NTT_A1, bool bSkipRandomSlot = false)
	{
		if (bSkipRandomSlot && eventDeckTemplet.ShipSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
		{
			return null;
		}
		return MakeEventDeckUnit(cNKMArmyData, eventDeckTemplet, deckCondition, eventDeckTemplet.ShipSlot, eventDeckData.m_ShipUID, NKM_UNIT_TYPE.NUT_SHIP, eTeam);
	}

	public static List<GameUnitData> MakeEventDeckUnitDataList(NKMArmyData cNKMArmyData, NKMDungeonEventDeckTemplet eventDeckTemplet, NKMDeckCondition deckCondition, NKMEventDeckData eventDeckData, NKMInventoryData inventoryData, NKM_TEAM_TYPE eTeam = NKM_TEAM_TYPE.NTT_A1, bool bSkipRandomSlot = false)
	{
		HashSet<int> allFixedUnitID = eventDeckTemplet.GetAllFixedUnitID();
		List<GameUnitData> list = new List<GameUnitData>();
		for (int i = 0; i < 8; i++)
		{
			NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = eventDeckTemplet.GetUnitSlot(i);
			if (bSkipRandomSlot && unitSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
			{
				continue;
			}
			long unitUID = eventDeckData.GetUnitUID(i);
			NKMUnitData nKMUnitData = MakeEventDeckUnit(cNKMArmyData, eventDeckTemplet, deckCondition, unitSlot, unitUID, NKM_UNIT_TYPE.NUT_NORMAL, eTeam, allFixedUnitID);
			if (nKMUnitData == null)
			{
				continue;
			}
			GameUnitData gameUnitData = new GameUnitData();
			gameUnitData.unit = nKMUnitData;
			allFixedUnitID.Add(nKMUnitData.m_UnitID);
			for (int j = 0; j < 4; j++)
			{
				long equipUid = gameUnitData.unit.GetEquipUid((ITEM_EQUIP_POSITION)j);
				if (equipUid > 0)
				{
					NKMEquipItemData itemEquip = inventoryData.GetItemEquip(equipUid);
					if (itemEquip != null)
					{
						NKMEquipItemData nKMEquipItemData = new NKMEquipItemData();
						nKMEquipItemData.DeepCopyFrom(itemEquip);
						gameUnitData.equip_item_list.Add(nKMEquipItemData);
					}
				}
			}
			list.Add(gameUnitData);
		}
		return list;
	}

	public static bool IsTutorialDungeon(int dungeonID)
	{
		if ((uint)(dungeonID - 1004) <= 3u || (uint)(dungeonID - 20001) <= 4u)
		{
			return true;
		}
		return false;
	}

	public static float GetBossHp(int dungeonID, int fixedBossLevel)
	{
		NKMDungeonTemplet dungeonTemplet = GetDungeonTemplet(dungeonID);
		if (dungeonTemplet == null)
		{
			return 0f;
		}
		NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(dungeonTemplet.m_BossUnitStrID);
		if (unitTemplet == null)
		{
			return 0f;
		}
		return NKMUnitStatManager.MakeFinalStat(new NKMUnitData
		{
			m_UnitID = unitTemplet.m_UnitTempletBase.m_UnitID,
			m_UnitLevel = ((fixedBossLevel > 0) ? fixedBossLevel : dungeonTemplet.m_BossUnitLevel)
		}, null, null).GetStatFinal(NKM_STAT_TYPE.NST_HP);
	}

	private static int GetFixedTeamBUnitLevel(int baseLevel, NKMGameData gameData)
	{
		if (gameData.m_TeamBLevelFix > 0)
		{
			return gameData.m_TeamBLevelFix;
		}
		return baseLevel + gameData.m_TeamBLevelAdd;
	}

	public static void CheckValidCutScen()
	{
		Dictionary<int, NKMDungeonTemplet>.Enumerator enumerator = m_dicNKMDungeonTempletByID.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMDungeonTempletBase dungeonTempletBase = enumerator.Current.Value.m_DungeonTempletBase;
			if (dungeonTempletBase.m_CutScenStrIDBefore != "" && NKCCutScenManager.GetCutScenTemple(dungeonTempletBase.m_CutScenStrIDBefore) == null)
			{
				Log.Error("NKMDungeonTempletBase can't find m_CutScenStrIDBefore : " + dungeonTempletBase.m_CutScenStrIDBefore, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMDungeonManagerEx.cs", 226);
			}
			if (dungeonTempletBase.m_CutScenStrIDAfter != "" && NKCCutScenManager.GetCutScenTemple(dungeonTempletBase.m_CutScenStrIDAfter) == null)
			{
				Log.Error("NKMDungeonTempletBase can't find m_CutScenStrIDAfter : " + dungeonTempletBase.m_CutScenStrIDAfter, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMDungeonManagerEx.cs", 236);
			}
		}
		NKCCutScenManager.ClearCacheData();
	}

	public static string GetDungeonStrID(int dungeonID)
	{
		NKMDungeonTempletBase dungeonTempletBase = GetDungeonTempletBase(dungeonID);
		if (dungeonTempletBase != null)
		{
			return dungeonTempletBase.m_DungeonStrID;
		}
		return "";
	}

	public static int GetCurrentMissionValue(NKMGame game, DUNGEON_GAME_MISSION_TYPE missionType)
	{
		NKMGameData gameData = game.GetGameData();
		NKMGameRuntimeData gameRuntimeData = game.GetGameRuntimeData();
		if (gameData == null || gameRuntimeData == null)
		{
			return 0;
		}
		int num = 0;
		switch (missionType)
		{
		case DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR:
			if (gameRuntimeData.m_WinTeam == NKM_TEAM_TYPE.NTT_A1 || gameRuntimeData.m_WinTeam == NKM_TEAM_TYPE.NTT_A2)
			{
				num++;
			}
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_TIME:
			num = (int)gameRuntimeData.GetGamePlayTime();
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_COST:
			num = (int)gameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fUsedRespawnCost;
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_RESPAWN:
			num = gameRuntimeData.m_NKMGameRuntimeTeamDataA.m_respawn_count;
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_SOLDIER:
			num = DeckUnitTypeCount(gameData, NKM_UNIT_STYLE_TYPE.NUST_SOLDIER);
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_MECHANIC:
			num = DeckUnitTypeCount(gameData, NKM_UNIT_STYLE_TYPE.NUST_MECHANIC);
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_COUNTER:
			num = DeckUnitTypeCount(gameData, NKM_UNIT_STYLE_TYPE.NUST_COUNTER);
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_DEFENDER:
			num = DeckUnitTypeCount(gameData, NKM_UNIT_ROLE_TYPE.NURT_DEFENDER);
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_STRIKER:
			num = DeckUnitTypeCount(gameData, NKM_UNIT_ROLE_TYPE.NURT_STRIKER);
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_RANGER:
			num = DeckUnitTypeCount(gameData, NKM_UNIT_ROLE_TYPE.NURT_RANGER);
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_SNIPER:
			num = DeckUnitTypeCount(gameData, NKM_UNIT_ROLE_TYPE.NURT_SNIPER);
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_TOWER:
			num = DeckUnitTypeCount(gameData, NKM_UNIT_ROLE_TYPE.NURT_TOWER);
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_SIEGE:
			num = DeckUnitTypeCount(gameData, NKM_UNIT_ROLE_TYPE.NURT_SIEGE);
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_DECKCOUNT_SUPPORTER:
			num = DeckUnitTypeCount(gameData, NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER);
			break;
		case DUNGEON_GAME_MISSION_TYPE.DGMT_SHIP_HP_DAMAGE:
		{
			float statFinal = NKMUnitStatManager.MakeFinalStat(gameData.m_NKMGameTeamDataA.m_MainShip, null, null).GetStatFinal(NKM_STAT_TYPE.NST_HP);
			float num2;
			if (NKCPhaseManager.IsPhaseOnGoing() && NKCPhaseManager.IsCurrentPhaseDungeon(gameData.m_DungeonID))
			{
				num2 = statFinal;
			}
			else
			{
				num2 = gameData.m_NKMGameTeamDataA.m_fInitHP;
				if (num2 == 0f)
				{
					num2 = statFinal;
				}
			}
			float num3 = num2 / statFinal;
			float num4 = NKCScenManager.GetScenManager().GetGameClient().GetLiveShipHPRate(NKM_TEAM_TYPE.NTT_A1);
			if (num4 == 0f)
			{
				num4 = num3;
			}
			num = Mathf.FloorToInt((num3 - num4) * 100f);
			break;
		}
		case DUNGEON_GAME_MISSION_TYPE.DGMT_TEAM_A_KILL_COUNT:
			num = (int)NKCKillCountManager.CurrentStageKillCount;
			break;
		}
		return num;
	}

	private static int DeckUnitTypeCount(NKMGameData gameData, NKM_UNIT_STYLE_TYPE unitType)
	{
		if (gameData == null)
		{
			return 0;
		}
		int num = 0;
		foreach (NKMUnitData listUnitDatum in gameData.m_NKMGameTeamDataA.m_listUnitData)
		{
			if (NKMUnitManager.GetUnitTempletBase(listUnitDatum.m_UnitID).m_NKM_UNIT_STYLE_TYPE == unitType)
			{
				num++;
			}
		}
		return num;
	}

	private static int DeckUnitTypeCount(NKMGameData gameData, NKM_UNIT_ROLE_TYPE unitRole)
	{
		if (gameData == null)
		{
			return 0;
		}
		int num = 0;
		foreach (NKMUnitData listUnitDatum in gameData.m_NKMGameTeamDataA.m_listUnitData)
		{
			if (NKMUnitManager.GetUnitTempletBase(listUnitDatum.m_UnitID).m_NKM_UNIT_ROLE_TYPE == unitRole)
			{
				num++;
			}
		}
		return num;
	}

	public static NKM_ERROR_CODE CanCounterCaseUnlock(NKMUserData userData, NKMDungeonTemplet dungeonTemplet)
	{
		NKMStageTempletV2 stageTemplet = dungeonTemplet.m_DungeonTempletBase.StageTemplet;
		if (stageTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_LOCKED_EPISODE;
		}
		if (!NKMEpisodeMgr.CheckEpisodeMission(userData, stageTemplet))
		{
			return NKM_ERROR_CODE.NEC_FAIL_LOCKED_EPISODE;
		}
		if (userData.CheckUnlockedCounterCase(dungeonTemplet.m_DungeonTempletBase.m_DungeonID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_COUNTERCASE_ALREADY_UNLOCKED;
		}
		if (userData.GetInformation() < stageTemplet.m_StageReqItemCount)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_INFORMATION;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static Dictionary<string, NKCEnemyData> GetEnemyUnits(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return new Dictionary<string, NKCEnemyData>();
		}
		return stageTemplet.m_STAGE_TYPE switch
		{
			STAGE_TYPE.ST_WARFARE => GetEnemyUnits(stageTemplet.WarfareTemplet), 
			STAGE_TYPE.ST_DUNGEON => GetEnemyUnits(stageTemplet.DungeonTempletBase), 
			STAGE_TYPE.ST_PHASE => GetEnemyUnits(stageTemplet.PhaseTemplet), 
			_ => new Dictionary<string, NKCEnemyData>(), 
		};
	}

	public static Dictionary<string, NKCEnemyData> GetEnemyUnits(NKMWarfareTemplet cNKMWarfareTemplet)
	{
		Dictionary<string, NKCEnemyData> dicEnemyUnitIDs = new Dictionary<string, NKCEnemyData>();
		if (cNKMWarfareTemplet == null)
		{
			return dicEnemyUnitIDs;
		}
		NKMWarfareMapTemplet mapTemplet = cNKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return dicEnemyUnitIDs;
		}
		foreach (string dungeonStrID in mapTemplet.GetDungeonStrIDList())
		{
			NKMDungeonTemplet dungeonTemplet = GetDungeonTemplet(dungeonStrID);
			if (dungeonTemplet != null)
			{
				bool isWarfareBossDungeon = dungeonTemplet.m_DungeonTempletBase.m_DungeonStrID == mapTemplet.GetFlagDungeonStrID();
				AddEnemyUnits(dungeonTemplet, ref dicEnemyUnitIDs, isWarfareBossDungeon);
			}
		}
		return dicEnemyUnitIDs;
	}

	public static Dictionary<string, NKCEnemyData> GetEnemyUnits(NKMDungeonTempletBase cNKMDungeonTempletBase)
	{
		Dictionary<string, NKCEnemyData> dicEnemyUnitIDs = new Dictionary<string, NKCEnemyData>();
		if (cNKMDungeonTempletBase == null)
		{
			return dicEnemyUnitIDs;
		}
		NKMDungeonTemplet dungeonTemplet = GetDungeonTemplet(cNKMDungeonTempletBase.m_DungeonStrID);
		if (dungeonTemplet == null)
		{
			return dicEnemyUnitIDs;
		}
		AddEnemyUnits(dungeonTemplet, ref dicEnemyUnitIDs, isWarfareBossDungeon: false);
		return dicEnemyUnitIDs;
	}

	public static Dictionary<string, NKCEnemyData> GetEnemyUnits(NKMPhaseTemplet phaseTemplet)
	{
		Dictionary<string, NKCEnemyData> dicEnemyUnitIDs = new Dictionary<string, NKCEnemyData>();
		if (phaseTemplet == null || phaseTemplet.PhaseList == null)
		{
			return dicEnemyUnitIDs;
		}
		for (int i = 0; i < phaseTemplet.PhaseList.List.Count; i++)
		{
			NKMDungeonTemplet dungeonTemplet = GetDungeonTemplet(phaseTemplet.PhaseList.List[i].Dungeon.m_DungeonID);
			if (dungeonTemplet != null)
			{
				AddEnemyUnits(dungeonTemplet, ref dicEnemyUnitIDs, isWarfareBossDungeon: false);
			}
		}
		return dicEnemyUnitIDs;
	}

	private static void AddEnemyUnits(NKMDungeonTemplet dungeonTemplet, ref Dictionary<string, NKCEnemyData> dicEnemyUnitIDs, bool isWarfareBossDungeon)
	{
		if (dungeonTemplet == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(dungeonTemplet.m_BossUnitStrID))
		{
			string key = ((!string.IsNullOrEmpty(dungeonTemplet.m_BossUnitChangeName)) ? dungeonTemplet.m_BossUnitChangeName : dungeonTemplet.m_BossUnitStrID);
			NKCEnemyData value = null;
			if (dicEnemyUnitIDs.TryGetValue(key, out value))
			{
				if (value.m_Level < dungeonTemplet.m_BossUnitLevel)
				{
					dicEnemyUnitIDs[key].m_Level = dungeonTemplet.m_BossUnitLevel;
				}
				value.m_NKM_BOSS_TYPE = ((!isWarfareBossDungeon) ? NKM_BOSS_TYPE.NBT_DUNGEON_BOSS : NKM_BOSS_TYPE.NBT_WARFARE_BOSS);
			}
			else
			{
				value = new NKCEnemyData();
				value.m_UnitStrID = dungeonTemplet.m_BossUnitStrID;
				value.m_ChangeUnitName = dungeonTemplet.m_BossUnitChangeName;
				value.m_Level = dungeonTemplet.m_BossUnitLevel;
				value.m_NKM_BOSS_TYPE = ((!isWarfareBossDungeon) ? NKM_BOSS_TYPE.NBT_DUNGEON_BOSS : NKM_BOSS_TYPE.NBT_WARFARE_BOSS);
				dicEnemyUnitIDs.Add(key, value);
			}
		}
		if (dungeonTemplet.m_BossRespawnUnitTemplet != null)
		{
			NKCEnemyData nKCEnemyData = AddEnemyUnits(ref dicEnemyUnitIDs, dungeonTemplet.m_BossRespawnUnitTemplet);
			if (nKCEnemyData != null)
			{
				nKCEnemyData.m_NKM_BOSS_TYPE = ((!isWarfareBossDungeon) ? NKM_BOSS_TYPE.NBT_DUNGEON_BOSS : NKM_BOSS_TYPE.NBT_WARFARE_BOSS);
			}
		}
		if (dungeonTemplet.m_listDungeonDeck != null)
		{
			for (int i = 0; i < dungeonTemplet.m_listDungeonDeck.Count; i++)
			{
				NKMDungeonRespawnUnitTemplet cNKMDungeonRespawnUnitTemplet = dungeonTemplet.m_listDungeonDeck[i];
				AddEnemyUnits(ref dicEnemyUnitIDs, cNKMDungeonRespawnUnitTemplet);
			}
		}
		if (dungeonTemplet.m_listDungeonWave != null)
		{
			for (int j = 0; j < dungeonTemplet.m_listDungeonWave.Count; j++)
			{
				NKMDungeonWaveTemplet nKMDungeonWaveTemplet = dungeonTemplet.m_listDungeonWave[j];
				if (nKMDungeonWaveTemplet != null)
				{
					for (int k = 0; k < nKMDungeonWaveTemplet.m_listDungeonUnitRespawnB.Count; k++)
					{
						NKMDungeonRespawnUnitTemplet cNKMDungeonRespawnUnitTemplet2 = nKMDungeonWaveTemplet.m_listDungeonUnitRespawnB[k];
						AddEnemyUnits(ref dicEnemyUnitIDs, cNKMDungeonRespawnUnitTemplet2);
					}
				}
			}
		}
		if (dungeonTemplet.m_listDungeonUnitRespawnB != null)
		{
			for (int l = 0; l < dungeonTemplet.m_listDungeonUnitRespawnB.Count; l++)
			{
				NKMDungeonRespawnUnitTemplet cNKMDungeonRespawnUnitTemplet3 = dungeonTemplet.m_listDungeonUnitRespawnB[l];
				AddEnemyUnits(ref dicEnemyUnitIDs, cNKMDungeonRespawnUnitTemplet3);
			}
		}
	}

	private static NKCEnemyData AddEnemyUnits(ref Dictionary<string, NKCEnemyData> dicEnemyUnitIDs, NKMDungeonRespawnUnitTemplet cNKMDungeonRespawnUnitTemplet)
	{
		if (dicEnemyUnitIDs == null || cNKMDungeonRespawnUnitTemplet == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(cNKMDungeonRespawnUnitTemplet.m_UnitStrID))
		{
			return null;
		}
		if (dicEnemyUnitIDs.TryGetValue(cNKMDungeonRespawnUnitTemplet.StrKey, out var value))
		{
			if (value.m_Level < cNKMDungeonRespawnUnitTemplet.m_UnitLevel)
			{
				dicEnemyUnitIDs[cNKMDungeonRespawnUnitTemplet.StrKey].m_Level = cNKMDungeonRespawnUnitTemplet.m_UnitLevel;
				dicEnemyUnitIDs[cNKMDungeonRespawnUnitTemplet.StrKey].m_SkinID = cNKMDungeonRespawnUnitTemplet.m_SkinID;
			}
		}
		else
		{
			value = new NKCEnemyData();
			value.m_UnitStrID = cNKMDungeonRespawnUnitTemplet.m_UnitStrID;
			value.m_Level = cNKMDungeonRespawnUnitTemplet.m_UnitLevel;
			value.m_SkinID = cNKMDungeonRespawnUnitTemplet.m_SkinID;
			value.m_ChangeUnitName = cNKMDungeonRespawnUnitTemplet.m_ChangeUnitName;
			dicEnemyUnitIDs.Add(cNKMDungeonRespawnUnitTemplet.StrKey, value);
		}
		return value;
	}

	public static NKMEventDeckData LoadDungeonDeck(NKMStageTempletV2 stageTemplet)
	{
		string curEventDeckKey = GetCurEventDeckKey(stageTemplet.DungeonTempletBase, stageTemplet);
		return LoadDungeonDeck(stageTemplet.GetEventDeckTemplet(), curEventDeckKey);
	}

	public static NKMEventDeckData LoadDungeonDeck(NKMDungeonEventDeckTemplet targetEventDeckTemplet, string eventDeckKey)
	{
		if (string.IsNullOrEmpty(eventDeckKey))
		{
			return null;
		}
		if (!PlayerPrefs.HasKey(eventDeckKey))
		{
			return null;
		}
		NKMEventDeckData nKMEventDeckData = new NKMEventDeckData();
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		string text = PlayerPrefs.GetString(eventDeckKey);
		string[] array = text.Split('&');
		foreach (string text2 in array)
		{
			int num = text2.IndexOf('/');
			if (num < 0)
			{
				break;
			}
			int.TryParse(text2.Substring(0, num), out var result);
			long.TryParse(text2.Substring(num + 1, text2.Length - (num + 1)), out var result2);
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(result2);
			if (targetEventDeckTemplet.IsUnitFitInSlot(targetEventDeckTemplet.GetUnitSlot(result), unitFromUID))
			{
				nKMEventDeckData.m_dicUnit.Add(result, result2);
			}
		}
		int num2 = text.IndexOf('_') + 1;
		if (num2 > 0)
		{
			int num3 = text.IndexOf('o');
			int num4 = text.IndexOf('l');
			int num5 = text.Length - num2;
			if (num3 > 0)
			{
				num5 -= text.Length - num3;
			}
			else if (num4 > 0)
			{
				num5 -= text.Length - num4;
			}
			long.TryParse(text.Substring(num2, num5), out var result3);
			NKMUnitData shipFromUID = armyData.GetShipFromUID(result3);
			if (targetEventDeckTemplet.IsUnitFitInSlot(targetEventDeckTemplet.ShipSlot, shipFromUID))
			{
				nKMEventDeckData.m_ShipUID = result3;
			}
		}
		int num6 = text.IndexOf('|') + 1;
		if (num6 > 0)
		{
			int num7 = text.IndexOf('l');
			int num8 = text.Length - num6;
			if (num7 > 0)
			{
				num8 -= text.Length - num7;
			}
			long.TryParse(text.Substring(num6, num8), out var result4);
			NKMOperator operatorFromUId = armyData.GetOperatorFromUId(result4);
			if (targetEventDeckTemplet.IsOperatorFitInSlot(operatorFromUId))
			{
				nKMEventDeckData.m_OperatorUID = result4;
			}
		}
		int num9 = text.IndexOf('^') + 1;
		if (num9 > 0)
		{
			int length = text.Length - num9;
			string text3 = text.Substring(num9, length);
			long num10 = NKCScenManager.CurrentUserData()?.m_UserUID ?? 0;
			long result5 = 0L;
			if (text3.Length > 1)
			{
				long.TryParse(text3.Substring(1), out result5);
			}
			if (result5 == num10 && int.TryParse(text3.Substring(0, 1), out var result6))
			{
				nKMEventDeckData.m_LeaderIndex = result6;
			}
		}
		return nKMEventDeckData;
	}

	public static string GetFierceEventDeckKey(NKMFierceBossGroupTemplet templet)
	{
		if (templet != null)
		{
			return string.Format($"NKM_PREPARE_EVENT_DECK_F_{templet.FierceBossGroupID}");
		}
		return "";
	}

	public static string GetCurEventDeckKey(NKMDungeonTempletBase dungeonTempletBase, NKMStageTempletV2 stageTemplet)
	{
		if (dungeonTempletBase != null)
		{
			return string.Format($"NKM_PREPARE_EVENT_DECK_{dungeonTempletBase.m_DungeonID}");
		}
		if (stageTemplet != null && stageTemplet.PhaseTemplet != null)
		{
			return string.Format($"NKM_PREPARE_EVENT_DECK_{stageTemplet.PhaseTemplet.Id}");
		}
		return "";
	}

	public static bool HasSavedDungeonDeck(NKMStageTempletV2 stageTemplet)
	{
		string curEventDeckKey = GetCurEventDeckKey(stageTemplet.DungeonTempletBase, stageTemplet);
		if (string.IsNullOrEmpty(curEventDeckKey))
		{
			return false;
		}
		return PlayerPrefs.HasKey(curEventDeckKey);
	}

	public static void SaveDungeonDeck(NKMEventDeckData eventDeckData, string eventDeckKey)
	{
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		StringBuilder stringBuilder = new StringBuilder();
		if (string.IsNullOrEmpty(eventDeckKey))
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			if (eventDeckData.m_dicUnit.ContainsKey(i))
			{
				stringBuilder.Append($"{i}/{eventDeckData.m_dicUnit[i]}&");
			}
		}
		if (eventDeckData.m_ShipUID != 0L && armyData.IsHaveShipFromUID(eventDeckData.m_ShipUID))
		{
			stringBuilder.Append($"s_{eventDeckData.m_ShipUID}");
		}
		if (eventDeckData.m_OperatorUID != 0L && armyData.IsHaveOperatorFromUID(eventDeckData.m_OperatorUID))
		{
			stringBuilder.Append($"o|{eventDeckData.m_OperatorUID}");
		}
		if (eventDeckData.m_LeaderIndex != 0)
		{
			long num = NKCScenManager.CurrentUserData()?.m_UserUID ?? 0;
			stringBuilder.Append($"l^{eventDeckData.m_LeaderIndex}{num}");
		}
		PlayerPrefs.SetString(eventDeckKey, stringBuilder.ToString());
	}

	public static bool IsRestartAllowed(NKM_GAME_TYPE game_type)
	{
		switch (game_type)
		{
		case NKM_GAME_TYPE.NGT_FIERCE:
		case NKM_GAME_TYPE.NGT_PVE_DEFENCE:
			return true;
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE:
			if (NKMOpenTagManager.IsOpened("GUILD_DUNGEON_GIVEUP"))
			{
				return false;
			}
			return true;
		default:
			return false;
		}
	}
}
