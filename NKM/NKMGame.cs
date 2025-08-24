using System;
using System.Collections.Generic;
using Cs.Logging;
using Cs.Math;
using NKM.Game;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Unit;

namespace NKM;

public abstract class NKMGame
{
	private struct TimeStopEventInfo
	{
		public float fStopTime;

		public NKM_STOP_TIME_INDEX stopTimeIndex;

		public short callUnitUID;

		public bool bStopSelf;

		public bool bStopSummonee;
	}

	public delegate bool OnUnitBatch(NKMUnit targetUnit, NKMUnit finderUnit, float distance);

	public const float DefaultInitialGameTime = 180f;

	public const byte MAX_RESPAWN_COST = 10;

	public const byte PVE_RESPAWN_COST = 4;

	public const byte PVP_RESPAWN_COST = 7;

	public const byte ASYNC_PVP_RESPAWN_COST = 10;

	public const float RESPAWN_COST_Add_ADJUST_VALUE = 0.3f;

	public const float GAME_PLAY_WAIT_TIME = 4f;

	public const float GAME_FINISH_WAIT_TIME = 8f;

	public const float GAME_DOUBLE_COST_TIME = 60f;

	public const float FULL_DECK_RESPAWN_COST_ADD = 0f;

	public const float FULL_DECK_COOL_TIME_REDUCE = 0f;

	public const float VALID_LAND_LEVEL_2_HP_RATE = 0.6f;

	public const float VALID_LAND_LEVEL_3_HP_RATE = 0.3f;

	public readonly float[] SHIP_ASSULT_RESPAWN_COST_ADD = new float[3];

	public readonly float[] SHIP_HEAVY_RESPAWN_COST_ADD = new float[3];

	public readonly float[] SHIP_CRUISER_RESPAWN_COST_ADD = new float[3];

	public readonly float[] SHIP_SPECIAL_RESPAWN_COST_ADD = new float[3];

	public readonly float[] SHIP_ASSULT_COOLTIME_REDUCE_ADD = new float[3];

	public readonly float[] SHIP_HEAVY_COOLTIME_REDUCE_ADD = new float[3];

	public readonly float[] SHIP_CRUISER_COOLTIME_REDUCE_ADD = new float[3];

	public readonly float[] SHIP_SPECIAL_COOLTIME_REDUCE_ADD = new float[3];

	public NKMGameRecord m_GameRecord = new NKMGameRecord();

	protected NKM_GAME_CLASS_TYPE m_NKM_GAME_CLASS_TYPE;

	protected NKMObjectPool m_ObjectPool;

	protected NKMGameRuntimeData m_NKMGameRuntimeData = new NKMGameRuntimeData();

	protected NKMMapTemplet m_NKMMapTemplet;

	protected NKMDungeonTemplet m_NKMDungeonTemplet;

	protected HashSet<long> m_TeamALiveUnitUID = new HashSet<long>();

	protected HashSet<long> m_TeamBLiveUnitUID = new HashSet<long>();

	protected List<short> m_listDieGameUnitUID = new List<short>();

	protected Dictionary<short, NKMUnit> m_dicNKMUnitPool = new Dictionary<short, NKMUnit>();

	protected Dictionary<short, NKMUnit> m_dicNKMUnit = new Dictionary<short, NKMUnit>();

	protected List<NKMUnit> m_listNKMUnit = new List<NKMUnit>();

	protected LinkedList<NKMDamageInst> m_linklistReAttack = new LinkedList<NKMDamageInst>();

	protected const float m_SyncFlushTimeMax = 0.4f;

	protected float m_fDeltaTime;

	protected NKMTrackingFloat m_GameSpeed = new NKMTrackingFloat();

	protected int m_ShipHPValidLandLevelTeamA;

	protected int m_ShipHPValidLandLevelTeamB;

	protected float m_fRespawnValidLandTeamA;

	protected float m_fRespawnValidLandTeamB;

	protected float m_fRespawnCostAddTeamA;

	protected float m_fRespawnCostAddTeamB;

	protected float m_fRespawnCostFullDeckAddTeamA;

	protected float m_fRespawnCostFullDeckAddTeamB;

	protected float m_fRespawnCostAsyncPVPAddTeamB;

	protected float m_fCoolTimeReduceFullDeckTeamA;

	protected float m_fCoolTimeReduceFullDeckTeamB;

	protected float m_fCoolTimeReduceTeamA;

	protected float m_fCoolTimeReduceTeamB;

	protected float m_fPlayWaitTimeOrg;

	protected float m_fPlayWaitTime;

	protected float m_fFinishWaitTime;

	public float m_AbsoluteGameTime;

	protected float m_fWorldStopTime;

	protected float m_fLastWorldStopFinishedATime;

	protected HashSet<int> m_hsEnabledBattleConditions = new HashSet<int>();

	protected NKMGameDevModeData m_GameDevModeData = new NKMGameDevModeData();

	private const string UNIT_DIE_PREFIX = "UNITDIE_";

	private const string DECK_DIE_PREFIX = "DECKDIE_";

	protected Dictionary<string, int> m_EventStatusTag = new Dictionary<string, int>();

	protected List<NKMDungeonEventData> m_listDungeonEventDataTeamA = new List<NKMDungeonEventData>();

	protected List<NKMDungeonEventData> m_listDungeonEventDataTeamB = new List<NKMDungeonEventData>();

	protected List<NKMDynamicRespawnUnitReserve> m_listNKMGameUnitDynamicRespawnData = new List<NKMDynamicRespawnUnitReserve>();

	protected List<NKMGameUnitRespawnData> m_listNKMGameUnitRespawnData = new List<NKMGameUnitRespawnData>();

	private Queue<TimeStopEventInfo> m_qEventTimeStop = new Queue<TimeStopEventInfo>();

	protected NKMGameData m_NKMGameData { get; set; }

	protected bool IsServerGame()
	{
		if (m_NKM_GAME_CLASS_TYPE != NKM_GAME_CLASS_TYPE.NGCT_GAME_SERVER)
		{
			return m_NKM_GAME_CLASS_TYPE == NKM_GAME_CLASS_TYPE.NGCT_GAME_SERVER_LOCAL;
		}
		return true;
	}

	public NKMObjectPool GetObjectPool()
	{
		return m_ObjectPool;
	}

	public NKMDungeonTemplet GetDungeonTemplet()
	{
		return m_NKMDungeonTemplet;
	}

	public float GetCoolTimeReduceTeamA()
	{
		return m_fCoolTimeReduceTeamA + m_fCoolTimeReduceFullDeckTeamA;
	}

	public float GetCoolTimeReduceTeamB()
	{
		return m_fCoolTimeReduceTeamB + m_fCoolTimeReduceFullDeckTeamB;
	}

	private void SetWorldStopTime(float fWorldStopTime)
	{
		m_fWorldStopTime = fWorldStopTime;
	}

	public float GetWorldStopTime()
	{
		return m_fWorldStopTime;
	}

	public NKMGameDevModeData GetGameDevModeData()
	{
		return m_GameDevModeData;
	}

	public NKMGameData GetGameData()
	{
		return m_NKMGameData;
	}

	public NKMGameRuntimeData GetGameRuntimeData()
	{
		return m_NKMGameRuntimeData;
	}

	public NKMMapTemplet GetMapTemplet()
	{
		return m_NKMMapTemplet;
	}

	public virtual void AddKillCount(NKMUnitDataGame unitDataGame, short victimUnitUid)
	{
	}

	public virtual void PostKill(NKMUnitData attacker, NKMUnitData defender)
	{
	}

	public void AddDieUnitRespawnTag(string tag)
	{
		AddEventTag("UNITDIE_" + tag, 1);
	}

	public int GetDieUnitRespawnTag(string tag)
	{
		return GetEventTag("UNITDIE_" + tag);
	}

	public void AddDieDeckRespawnTag(string tag)
	{
		AddEventTag("DECKDIE_" + tag, 1);
	}

	public int GetDieDeckRespawnTag(string tag)
	{
		return GetEventTag("DECKDIE_" + tag);
	}

	public void SetEventTag(string tag, int value)
	{
		m_EventStatusTag[tag] = value;
	}

	public void AddEventTag(string tag, int value)
	{
		if (!m_EventStatusTag.ContainsKey(tag))
		{
			m_EventStatusTag.Add(tag, value);
		}
		else
		{
			m_EventStatusTag[tag] += value;
		}
	}

	public int GetEventTag(string tag)
	{
		if (m_EventStatusTag.ContainsKey(tag))
		{
			return m_EventStatusTag[tag];
		}
		return 0;
	}

	protected virtual void IsThreadSafe()
	{
	}

	public static bool IsPVE(NKM_GAME_TYPE type)
	{
		switch (type)
		{
		case NKM_GAME_TYPE.NGT_DEV:
		case NKM_GAME_TYPE.NGT_PRACTICE:
		case NKM_GAME_TYPE.NGT_DUNGEON:
		case NKM_GAME_TYPE.NGT_WARFARE:
		case NKM_GAME_TYPE.NGT_DIVE:
		case NKM_GAME_TYPE.NGT_TUTORIAL:
		case NKM_GAME_TYPE.NGT_RAID:
		case NKM_GAME_TYPE.NGT_RAID_SOLO:
		case NKM_GAME_TYPE.NGT_SHADOW_PALACE:
		case NKM_GAME_TYPE.NGT_FIERCE:
		case NKM_GAME_TYPE.NGT_PHASE:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_ARENA:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS:
		case NKM_GAME_TYPE.NGT_TRIM:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE:
		case NKM_GAME_TYPE.NGT_PVE_DEFENCE:
			return true;
		default:
			return false;
		}
	}

	public static bool IsPVP(NKM_GAME_TYPE type)
	{
		switch (type)
		{
		case NKM_GAME_TYPE.NGT_PVP_RANK:
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
		case NKM_GAME_TYPE.NGT_PVP_EVENT:
		case NKM_GAME_TYPE.NGT_PVE_SIMULATED:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
			return true;
		default:
			return false;
		}
	}

	public bool IsPVE(bool bUseDevOption)
	{
		if (bUseDevOption && GetGameDevModeData().m_bDevForcePvp)
		{
			return false;
		}
		return IsPVE(GetGameData().m_NKM_GAME_TYPE);
	}

	public bool IsPVP(bool bUseDevOption)
	{
		if (bUseDevOption && GetGameDevModeData().m_bDevForcePvp)
		{
			return true;
		}
		return IsPVP(GetGameData().m_NKM_GAME_TYPE);
	}

	public static bool ApplyUpBanByGameType(NKMGameData gameData, NKM_TEAM_TYPE teamType)
	{
		if (gameData == null)
		{
			return false;
		}
		switch (gameData.GetGameType())
		{
		case NKM_GAME_TYPE.NGT_PVP_RANK:
		case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_EVENT:
			return true;
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
			if (IsBTeamStaticFunc(teamType))
			{
				return true;
			}
			break;
		}
		return false;
	}

	public static bool IsGuildDungeon(NKM_GAME_TYPE type)
	{
		if (type - 16 <= NKM_GAME_TYPE.NGT_DEV || type == NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE)
		{
			return true;
		}
		return false;
	}

	public static bool IsPVPSync(NKM_GAME_TYPE type)
	{
		if (type != NKM_GAME_TYPE.NGT_PVP_RANK && type != NKM_GAME_TYPE.NGT_PVP_PRIVATE && type != NKM_GAME_TYPE.NGT_PVP_LEAGUE && type != NKM_GAME_TYPE.NGT_PVP_EVENT)
		{
			return type == NKM_GAME_TYPE.NGT_PVP_UNLIMITED;
		}
		return true;
	}

	public NKMGame()
	{
		m_NKM_GAME_CLASS_TYPE = NKM_GAME_CLASS_TYPE.NGCT_GAME;
	}

	public virtual void Init()
	{
		m_NKMGameData = null;
		m_NKMMapTemplet = null;
		m_NKMDungeonTemplet = null;
		m_listDieGameUnitUID.Clear();
		InitUnit();
		foreach (NKMDamageInst item in m_linklistReAttack)
		{
			if (item != null)
			{
				m_ObjectPool.CloseObj(item);
			}
		}
		m_linklistReAttack.Clear();
		m_fDeltaTime = 0f;
		m_GameSpeed.SetNowValue(1f);
		m_fPlayWaitTimeOrg = 4f;
		m_fPlayWaitTime = m_fPlayWaitTimeOrg;
		m_fFinishWaitTime = 8f;
		m_AbsoluteGameTime = 0f;
		m_fWorldStopTime = 0f;
		m_ShipHPValidLandLevelTeamA = 0;
		m_ShipHPValidLandLevelTeamB = 0;
		m_fRespawnValidLandTeamA = 0.4f;
		m_fRespawnValidLandTeamB = 0.4f;
		m_fRespawnCostAddTeamA = 0f;
		m_fRespawnCostAddTeamB = 0f;
		m_fRespawnCostFullDeckAddTeamA = 0f;
		m_fRespawnCostFullDeckAddTeamB = 0f;
		m_fCoolTimeReduceFullDeckTeamA = 0f;
		m_fCoolTimeReduceFullDeckTeamB = 0f;
		m_fCoolTimeReduceTeamA = 0f;
		m_fCoolTimeReduceTeamB = 0f;
		m_EventStatusTag.Clear();
		for (int i = 0; i < m_listNKMGameUnitRespawnData.Count; i++)
		{
			m_ObjectPool.CloseObj(m_listNKMGameUnitRespawnData[i]);
		}
		m_listNKMGameUnitRespawnData.Clear();
		m_hsEnabledBattleConditions.Clear();
	}

	public virtual void InitUnit()
	{
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnitPool.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMUnit value = enumerator.Current.Value;
			if (value != null)
			{
				GetObjectPool().CloseObj(value);
			}
		}
		m_dicNKMUnitPool.Clear();
		for (int i = 0; i < m_listNKMUnit.Count; i++)
		{
			NKMUnit nKMUnit = m_listNKMUnit[i];
			if (nKMUnit != null)
			{
				GetObjectPool().CloseObj(nKMUnit);
			}
		}
		m_listNKMUnit.Clear();
		m_dicNKMUnit.Clear();
	}

	protected void InitDungeonEventData()
	{
		if (m_NKMDungeonTemplet != null)
		{
			m_listDungeonEventDataTeamA.Clear();
			m_listDungeonEventDataTeamB.Clear();
			for (int i = 0; i < m_NKMDungeonTemplet.m_listDungeonEventTempletTeamA.Count; i++)
			{
				NKMDungeonEventData cNKMDungeonEventData = new NKMDungeonEventData();
				cNKMDungeonEventData.m_DungeonEventTemplet = m_NKMDungeonTemplet.m_listDungeonEventTempletTeamA[i];
				ProcessEventDataCache(ref cNKMDungeonEventData);
				m_listDungeonEventDataTeamA.Add(cNKMDungeonEventData);
			}
			for (int j = 0; j < m_NKMDungeonTemplet.m_listDungeonEventTempletTeamB.Count; j++)
			{
				NKMDungeonEventData cNKMDungeonEventData2 = new NKMDungeonEventData();
				cNKMDungeonEventData2.m_DungeonEventTemplet = m_NKMDungeonTemplet.m_listDungeonEventTempletTeamB[j];
				ProcessEventDataCache(ref cNKMDungeonEventData2);
				m_listDungeonEventDataTeamB.Add(cNKMDungeonEventData2);
			}
		}
	}

	private void ProcessEventDataCache(ref NKMDungeonEventData cNKMDungeonEventData)
	{
		if (cNKMDungeonEventData.m_DungeonEventTemplet == null)
		{
			return;
		}
		switch (cNKMDungeonEventData.m_DungeonEventTemplet.m_EventCondition)
		{
		case NKM_EVENT_START_CONDITION_TYPE.UNIT_STATE_START:
		case NKM_EVENT_START_CONDITION_TYPE.UNIT_STATE_END:
		case NKM_EVENT_START_CONDITION_TYPE.TARGET_ALLY_UNIT_HP_RATE_LESS:
		case NKM_EVENT_START_CONDITION_TYPE.TARGET_ENEMY_UNIT_HP_RATE_LESS:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMDungeonEventData.m_DungeonEventTemplet.m_EventConditionValue1);
			if (unitTempletBase != null)
			{
				cNKMDungeonEventData.EventConditionCache1 = unitTempletBase.m_UnitID;
				break;
			}
			Log.ErrorAndExit("DungeonStrID " + m_NKMDungeonTemplet.m_DungeonTempletBase.m_DungeonStrID + ", " + $"EventCondition : {cNKMDungeonEventData.m_DungeonEventTemplet.m_EventCondition}, " + $"EventID {cNKMDungeonEventData.m_DungeonEventTemplet.m_EventID}, Unit Not Found", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGame.cs", 2285);
			break;
		}
		case NKM_EVENT_START_CONDITION_TYPE.NONE:
		case NKM_EVENT_START_CONDITION_TYPE.ENEMY_BOSS_HP_RATE_LESS:
		case NKM_EVENT_START_CONDITION_TYPE.HAVE_SUMMON_COST:
			break;
		}
	}

	protected virtual void SetDefaultTacticalCommand()
	{
	}

	protected void AddTacticalCommand(NKMGameTeamData cNKMGameTeamData)
	{
		if (cNKMGameTeamData == null || cNKMGameTeamData.m_Operator == null)
		{
			return;
		}
		NKMOperatorSkill mainSkill = cNKMGameTeamData.m_Operator.mainSkill;
		if (mainSkill == null)
		{
			return;
		}
		NKMOperatorSkillTemplet nKMOperatorSkillTemplet = NKMTempletContainer<NKMOperatorSkillTemplet>.Find(mainSkill.id);
		if (nKMOperatorSkillTemplet == null || nKMOperatorSkillTemplet.m_OperSkillType != OperatorSkillType.m_Tactical)
		{
			return;
		}
		NKMTacticalCommandTemplet tacticalCommandTempletByStrID = NKMTacticalCommandManager.GetTacticalCommandTempletByStrID(nKMOperatorSkillTemplet.m_OperSkillTarget);
		if (tacticalCommandTempletByStrID != null)
		{
			NKMTacticalCommandData nKMTacticalCommandData = new NKMTacticalCommandData();
			nKMTacticalCommandData.m_TCID = tacticalCommandTempletByStrID.m_TCID;
			nKMTacticalCommandData.m_Level = mainSkill.level;
			if (m_NKMGameData.IsPVP() && m_NKMGameData.IsBanOperator(cNKMGameTeamData.m_Operator.id))
			{
				int banOperatorLevel = m_NKMGameData.GetBanOperatorLevel(cNKMGameTeamData.m_Operator.id);
				nKMTacticalCommandData.m_fCoolTimeNow = Math.Min(NKMUnitStatManager.m_OperatorTacticalCommandPerBanLevel * (float)banOperatorLevel, NKMUnitStatManager.m_MaxOperatorTacticalCommandPerBanLevel);
			}
			cNKMGameTeamData.m_listTacticalCommandData.Add(nKMTacticalCommandData);
		}
	}

	public virtual void StartGame(bool bIntrude)
	{
		NKM_GAME_STATE nKM_GAME_STATE = m_NKMGameRuntimeData.m_NKM_GAME_STATE;
		if (nKM_GAME_STATE == NKM_GAME_STATE.NGS_INVALID || nKM_GAME_STATE == NKM_GAME_STATE.NGS_STOP || nKM_GAME_STATE == NKM_GAME_STATE.NGS_LOBBY_MATCHING || nKM_GAME_STATE == NKM_GAME_STATE.NGS_LOBBY_GAME_SETTING)
		{
			SetGameState(NKM_GAME_STATE.NGS_START);
		}
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			m_fPlayWaitTime = 0.3f;
			GetGameRuntimeData().m_NKMGameRuntimeTeamDataB.m_bAIDisable = true;
		}
		if (m_NKMDungeonTemplet != null && !m_NKMDungeonTemplet.m_bCanUseAuto)
		{
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_bAutoRespawn = false;
		}
		if (GetGameData().m_NKMGameTeamDataA.IsFullDeck())
		{
			m_fRespawnCostFullDeckAddTeamA = 0f;
			m_fCoolTimeReduceFullDeckTeamA = 0f;
		}
		switch (GetGameData().GetGameType())
		{
		default:
			if (GetGameData().m_NKMGameTeamDataB.IsFullDeck())
			{
				m_fRespawnCostFullDeckAddTeamB = 0f;
				m_fCoolTimeReduceFullDeckTeamB = 0f;
			}
			break;
		case NKM_GAME_TYPE.NGT_DUNGEON:
		case NKM_GAME_TYPE.NGT_TUTORIAL:
		case NKM_GAME_TYPE.NGT_PHASE:
		case NKM_GAME_TYPE.NGT_TRIM:
			break;
		}
		if (m_NKMGameData.m_NKMGameTeamDataA.m_MainShip != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_NKMGameData.m_NKMGameTeamDataA.m_MainShip.m_UnitID);
			m_fRespawnValidLandTeamA = GetValidLandFactor(0, GetGameData().IsPVP(), NKM_TEAM_TYPE.NTT_A1);
			m_fRespawnCostAddTeamA = GetCostAddFactorByShip(0, unitTempletBase.m_NKM_UNIT_STYLE_TYPE);
			m_fCoolTimeReduceTeamA = GetCoolTimeReduceFactorByShip(0, unitTempletBase.m_NKM_UNIT_STYLE_TYPE);
		}
		if (m_NKMGameData.m_NKMGameTeamDataB.m_MainShip != null)
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(m_NKMGameData.m_NKMGameTeamDataB.m_MainShip.m_UnitID);
			m_fRespawnValidLandTeamB = GetValidLandFactor(0, GetGameData().IsPVP(), NKM_TEAM_TYPE.NTT_B1);
			m_fRespawnCostAddTeamB = GetCostAddFactorByShip(0, unitTempletBase2.m_NKM_UNIT_STYLE_TYPE);
			m_fCoolTimeReduceTeamB = GetCoolTimeReduceFactorByShip(0, unitTempletBase2.m_NKM_UNIT_STYLE_TYPE);
		}
		if (IsPVE(m_NKMGameData.m_NKM_GAME_TYPE))
		{
			GetGameRuntimeData().m_NKMGameRuntimeTeamDataB.m_NKM_GAME_AUTO_SKILL_TYPE = NKM_GAME_AUTO_SKILL_TYPE.NGST_AUTO;
		}
		m_NKMGameData.InitRespawnLimitCount();
	}

	public virtual void EndGame()
	{
	}

	public void SetGameSpeed(float fSpeed, float fTrackingTime)
	{
		m_GameSpeed.SetNowValue(fSpeed);
		m_GameSpeed.SetTracking(1f, fTrackingTime, TRACKING_DATA_TYPE.TDT_FASTER);
	}

	protected void CreateGameUnitUID()
	{
		CreateGameUnitUID(m_NKMGameData.m_NKMGameTeamDataA);
		CreateGameUnitUID(m_NKMGameData.m_NKMGameTeamDataB);
	}

	protected void CreateGameUnitUID(NKMGameTeamData cNKMGameTeamData)
	{
		CreateGameUnitUID(cNKMGameTeamData.m_MainShip);
		for (int i = 0; i < cNKMGameTeamData.m_listUnitData.Count; i++)
		{
			CreateGameUnitUID(cNKMGameTeamData.m_listUnitData[i]);
		}
		for (int j = 0; j < cNKMGameTeamData.m_listAssistUnitData.Count; j++)
		{
			CreateGameUnitUID(cNKMGameTeamData.m_listAssistUnitData[j]);
		}
		for (int k = 0; k < cNKMGameTeamData.m_listEvevtUnitData.Count; k++)
		{
			CreateGameUnitUID(cNKMGameTeamData.m_listEvevtUnitData[k]);
		}
		for (int l = 0; l < cNKMGameTeamData.m_listEnvUnitData.Count; l++)
		{
			CreateGameUnitUID(cNKMGameTeamData.m_listEnvUnitData[l]);
		}
		for (int m = 0; m < cNKMGameTeamData.m_listOperatorUnitData.Count; m++)
		{
			CreateGameUnitUID(cNKMGameTeamData.m_listOperatorUnitData[m]);
		}
	}

	protected bool CreateGameUnitUID(NKMUnitData cNKMUnitData)
	{
		if (cNKMUnitData == null)
		{
			return false;
		}
		NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(cNKMUnitData.m_UnitID);
		if (unitTemplet == null)
		{
			Log.Error($"Invalid Unit Data. unit id : {cNKMUnitData.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGame.cs", 2503);
			return false;
		}
		if (cNKMUnitData.m_listGameUnitUID.Count > 0)
		{
			Log.Error("FATAL. GameUnitUid List Not Empty", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGame.cs", 2509);
			throw new Exception("GameUnitUid List Not Empty");
		}
		for (int i = 0; i < unitTemplet.m_StatTemplet.m_RespawnCount; i++)
		{
			cNKMUnitData.m_listGameUnitUID.Add(m_NKMGameData.GetGameUnitUID());
			cNKMUnitData.m_listNearTargetRange.Add(unitTemplet.GetNearTargetRandomRange());
		}
		return true;
	}

	protected void CreateDynaminRespawnPoolUnit(bool bAsync)
	{
		CreateDynaminRespawnPoolUnit(m_NKMGameData.m_NKMGameTeamDataA, NKM_TEAM_TYPE.NTT_A1, bAsync);
		CreateDynaminRespawnPoolUnit(m_NKMGameData.m_NKMGameTeamDataB, NKM_TEAM_TYPE.NTT_B1, bAsync);
	}

	protected void CreateDynaminRespawnPoolUnit(NKMGameTeamData cNKMGameTeamData, NKM_TEAM_TYPE eNKM_TEAM_TYPE, bool bAsync)
	{
		for (int i = 0; i < cNKMGameTeamData.m_listDynamicRespawnUnitData.Count; i++)
		{
			NKMDynamicRespawnUnitData nKMDynamicRespawnUnitData = cNKMGameTeamData.m_listDynamicRespawnUnitData[i];
			if (m_NKM_GAME_CLASS_TYPE == NKM_GAME_CLASS_TYPE.NGCT_GAME_CLIENT)
			{
				if (nKMDynamicRespawnUnitData != null && !nKMDynamicRespawnUnitData.m_bLoadedClient)
				{
					CreatePoolUnit(cNKMGameTeamData, cNKMGameTeamData.m_listDynamicRespawnUnitData[i].m_NKMUnitData, cNKMGameTeamData.m_listDynamicRespawnUnitData[i].m_MasterGameUnitUID, eNKM_TEAM_TYPE, bAsync);
					nKMDynamicRespawnUnitData.m_bLoadedClient = true;
				}
			}
			else if (nKMDynamicRespawnUnitData != null && !nKMDynamicRespawnUnitData.m_bLoadedServer)
			{
				CreatePoolUnit(cNKMGameTeamData, cNKMGameTeamData.m_listDynamicRespawnUnitData[i].m_NKMUnitData, cNKMGameTeamData.m_listDynamicRespawnUnitData[i].m_MasterGameUnitUID, eNKM_TEAM_TYPE, bAsync);
				nKMDynamicRespawnUnitData.m_bLoadedServer = true;
			}
		}
	}

	protected void CreatePoolUnit(bool bAsync)
	{
		CreatePoolUnit(m_NKMGameData.m_NKMGameTeamDataA, NKM_TEAM_TYPE.NTT_A1, bAsync);
		CreatePoolUnit(m_NKMGameData.m_NKMGameTeamDataB, NKM_TEAM_TYPE.NTT_B1, bAsync);
	}

	protected void CreatePoolUnit(NKMGameTeamData cNKMGameTeamData, NKM_TEAM_TYPE eNKM_TEAM_TYPE, bool bAsync)
	{
		CreatePoolUnit(cNKMGameTeamData, cNKMGameTeamData.m_MainShip, 0, eNKM_TEAM_TYPE, bAsync);
		for (int i = 0; i < cNKMGameTeamData.m_listUnitData.Count; i++)
		{
			CreatePoolUnit(cNKMGameTeamData, cNKMGameTeamData.m_listUnitData[i], 0, eNKM_TEAM_TYPE, bAsync);
		}
		for (int j = 0; j < cNKMGameTeamData.m_listAssistUnitData.Count; j++)
		{
			CreatePoolUnit(cNKMGameTeamData, cNKMGameTeamData.m_listAssistUnitData[j], 0, eNKM_TEAM_TYPE, bAsync);
		}
		for (int k = 0; k < cNKMGameTeamData.m_listEvevtUnitData.Count; k++)
		{
			CreatePoolUnit(cNKMGameTeamData, cNKMGameTeamData.m_listEvevtUnitData[k], 0, eNKM_TEAM_TYPE, bAsync);
		}
		for (int l = 0; l < cNKMGameTeamData.m_listEnvUnitData.Count; l++)
		{
			CreatePoolUnit(cNKMGameTeamData, cNKMGameTeamData.m_listEnvUnitData[l], 0, eNKM_TEAM_TYPE, bAsync);
		}
		for (int m = 0; m < cNKMGameTeamData.m_listOperatorUnitData.Count; m++)
		{
			CreatePoolUnit(cNKMGameTeamData, cNKMGameTeamData.m_listOperatorUnitData[m], 0, eNKM_TEAM_TYPE, bAsync);
		}
	}

	protected bool CreatePoolUnit(NKMGameTeamData cNKMGameTeamData, NKMUnitData cNKMUnitData, short masterGameUnitUID, NKM_TEAM_TYPE eNKM_TEAM_TYPE, bool bAsync)
	{
		if (cNKMUnitData == null)
		{
			return false;
		}
		NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(cNKMUnitData.m_UnitID);
		if (unitTemplet == null)
		{
			return false;
		}
		for (int i = 0; i < cNKMUnitData.m_listGameUnitUID.Count; i++)
		{
			NKMUnit nKMUnit = CreateNewUnitObj();
			if (nKMUnit == null)
			{
				return false;
			}
			if (m_dicNKMUnit.ContainsKey(cNKMUnitData.m_listGameUnitUID[i]))
			{
				return false;
			}
			if (m_dicNKMUnitPool.ContainsKey(cNKMUnitData.m_listGameUnitUID[i]))
			{
				return false;
			}
			nKMUnit.SetBoss(IsBoss(cNKMUnitData.m_listGameUnitUID[i]));
			if (i == 0)
			{
				nKMUnit.LoadUnit(this, cNKMUnitData, masterGameUnitUID, cNKMUnitData.m_listGameUnitUID[i], cNKMUnitData.m_listNearTargetRange[i], eNKM_TEAM_TYPE, bSub: false, bAsync);
			}
			else
			{
				nKMUnit.LoadUnit(this, cNKMUnitData, masterGameUnitUID, cNKMUnitData.m_listGameUnitUID[i], cNKMUnitData.m_listNearTargetRange[i], eNKM_TEAM_TYPE, bSub: true, bAsync);
			}
			if (!bAsync)
			{
				nKMUnit.LoadUnitComplete();
			}
			if (m_dicNKMUnitPool.ContainsKey(nKMUnit.GetUnitDataGame().m_GameUnitUID))
			{
				return false;
			}
			m_dicNKMUnitPool.Add(nKMUnit.GetUnitDataGame().m_GameUnitUID, nKMUnit);
			if (cNKMGameTeamData == null || !IsServerGame())
			{
				continue;
			}
			foreach (NKMBuffUnitDieEvent item in unitTemplet.m_listBuffUnitDieEvent)
			{
				foreach (NKMEventRespawn item2 in item.m_listNKMEventRespawn)
				{
					CreatePoolUnitDynamicRespawn(cNKMGameTeamData, nKMUnit, cNKMUnitData, item2, NKM_SKILL_TYPE.NST_INVALID);
				}
			}
			foreach (KeyValuePair<short, NKMUnitState> item3 in unitTemplet.m_dicNKMUnitStateID)
			{
				foreach (NKMEventRespawn item4 in item3.Value.m_listNKMEventRespawn)
				{
					CreatePoolUnitDynamicRespawn(cNKMGameTeamData, nKMUnit, cNKMUnitData, item4, item3.Value.m_NKM_SKILL_TYPE);
				}
				if (item3.Value.m_NKMEventUnitChange == null || nKMUnit.GetUnitChangeRespawnPool(item3.Value.m_NKMEventUnitChange.m_UnitStrID) != 0)
				{
					continue;
				}
				NKMDynamicRespawnUnitData nKMDynamicRespawnUnitData = new NKMDynamicRespawnUnitData();
				if (item3.Value.m_NKMEventUnitChange.m_dicSummonSkin != null && !item3.Value.m_NKMEventUnitChange.m_dicSummonSkin.TryGetValue(cNKMUnitData.m_SkinID, out nKMDynamicRespawnUnitData.m_NKMUnitData.m_SkinID))
				{
					if (cNKMUnitData.m_SkinID == 0)
					{
						nKMDynamicRespawnUnitData.m_NKMUnitData.m_SkinID = 0;
					}
					else if (!item3.Value.m_NKMEventUnitChange.m_dicSummonSkin.TryGetValue(0, out nKMDynamicRespawnUnitData.m_NKMUnitData.m_SkinID))
					{
						nKMDynamicRespawnUnitData.m_NKMUnitData.m_SkinID = 0;
					}
				}
				if (nKMUnit.GetMasterUnitGameUID() == 0)
				{
					nKMDynamicRespawnUnitData.m_MasterGameUnitUID = nKMUnit.GetUnitDataGame().m_GameUnitUID;
				}
				else
				{
					nKMDynamicRespawnUnitData.m_MasterGameUnitUID = nKMUnit.GetMasterUnitGameUID();
				}
				nKMUnit.GetUnitDataGame().m_bChangeUnit = true;
				nKMDynamicRespawnUnitData.m_NKMUnitData.m_UnitID = NKMUnitManager.GetUnitID(item3.Value.m_NKMEventUnitChange.m_UnitStrID);
				nKMDynamicRespawnUnitData.m_NKMUnitData.FillDataFromRespawnOrigin(cNKMUnitData, bSimple: false);
				short gameUnitUID = m_NKMGameData.GetGameUnitUID();
				nKMDynamicRespawnUnitData.m_NKMUnitData.m_listGameUnitUID.Add(gameUnitUID);
				NKMUnitTemplet unitTemplet2 = nKMDynamicRespawnUnitData.m_NKMUnitData.GetUnitTemplet();
				if (unitTemplet2 != null)
				{
					nKMDynamicRespawnUnitData.m_NKMUnitData.m_listNearTargetRange.Add(unitTemplet2.GetNearTargetRandomRange());
				}
				cNKMUnitData.m_listGameUnitUIDChange.Add(gameUnitUID);
				nKMUnit.AddUnitChangeRespawnPool(item3.Value.m_NKMEventUnitChange.m_UnitStrID, gameUnitUID);
				cNKMGameTeamData.m_listDynamicRespawnUnitData.Add(nKMDynamicRespawnUnitData);
			}
			if (unitTemplet.m_dicTriggerSet == null)
			{
				continue;
			}
			foreach (NKMUnitTriggerSet value in unitTemplet.m_dicTriggerSet.Values)
			{
				foreach (NKMEventRespawn @event in value.GetEvents<NKMEventRespawn>())
				{
					CreatePoolUnitDynamicRespawn(cNKMGameTeamData, nKMUnit, cNKMUnitData, @event, NKM_SKILL_TYPE.NST_INVALID);
				}
			}
		}
		return true;
	}

	protected bool CreatePoolUnitDynamicRespawn(NKMGameTeamData cNKMGameTeamData, NKMUnit cUnit, NKMUnitData cNKMUnitData, NKMEventRespawn cNKMEventRespawn, NKM_SKILL_TYPE eNKM_SKILL_TYPE)
	{
		NKMDynamicRespawnUnitData nKMDynamicRespawnUnitData = new NKMDynamicRespawnUnitData();
		if (cUnit.GetMasterUnitGameUID() == 0)
		{
			nKMDynamicRespawnUnitData.m_MasterGameUnitUID = cUnit.GetUnitDataGame().m_GameUnitUID;
		}
		else
		{
			nKMDynamicRespawnUnitData.m_MasterGameUnitUID = cUnit.GetMasterUnitGameUID();
		}
		nKMDynamicRespawnUnitData.m_NKMUnitData.m_UnitID = NKMUnitManager.GetUnitID(cNKMEventRespawn.m_UnitStrID);
		nKMDynamicRespawnUnitData.m_NKMUnitData.m_LimitBreakLevel = 0;
		nKMDynamicRespawnUnitData.m_NKMUnitData.m_UnitLevel = 1;
		nKMDynamicRespawnUnitData.m_NKMUnitData.m_bSummonUnit = true;
		if (cNKMEventRespawn.m_bUseMasterLevel)
		{
			nKMDynamicRespawnUnitData.m_NKMUnitData.m_UnitLevel = cNKMUnitData.m_UnitLevel;
		}
		if (cNKMEventRespawn.m_bUseMasterData)
		{
			nKMDynamicRespawnUnitData.m_NKMUnitData.FillDataFromRespawnOrigin(cNKMUnitData, bSimple: false);
		}
		NKMUnitTemplet unitTemplet = cNKMUnitData.GetUnitTemplet();
		if (unitTemplet == null)
		{
			Log.Error($"UnitTemplet is null. UserUid:{cNKMUnitData.m_UserUID}, UnitId:{cNKMUnitData.m_UnitID}, UnitUid:{cNKMUnitData.m_UnitUID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGame.cs", 2752);
			return false;
		}
		if (unitTemplet.m_UnitTempletBase == null)
		{
			Log.Error($"UnitBaseTemplet is null. UserUid:{cNKMUnitData.m_UserUID}, UnitId:{cNKMUnitData.m_UnitID}, UnitUid:{cNKMUnitData.m_UnitUID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGame.cs", 2759);
			return false;
		}
		if (cNKMEventRespawn.m_dicSummonSkin != null && !cNKMEventRespawn.m_dicSummonSkin.TryGetValue(cNKMUnitData.m_SkinID, out nKMDynamicRespawnUnitData.m_NKMUnitData.m_SkinID))
		{
			if (cNKMUnitData.m_SkinID == 0)
			{
				nKMDynamicRespawnUnitData.m_NKMUnitData.m_SkinID = 0;
			}
			else if (!cNKMEventRespawn.m_dicSummonSkin.TryGetValue(0, out nKMDynamicRespawnUnitData.m_NKMUnitData.m_SkinID))
			{
				nKMDynamicRespawnUnitData.m_NKMUnitData.m_SkinID = 0;
			}
		}
		NKMUnitTemplet unitTemplet2 = nKMDynamicRespawnUnitData.m_NKMUnitData.GetUnitTemplet();
		for (int i = 0; i < cNKMEventRespawn.m_MaxCount; i++)
		{
			short gameUnitUID = m_NKMGameData.GetGameUnitUID();
			nKMDynamicRespawnUnitData.m_NKMUnitData.m_listGameUnitUID.Add(gameUnitUID);
			if (unitTemplet2 != null)
			{
				nKMDynamicRespawnUnitData.m_NKMUnitData.m_listNearTargetRange.Add(unitTemplet2.GetNearTargetRandomRange());
			}
			cUnit.AddDynamicRespawnPool(cNKMEventRespawn, gameUnitUID);
		}
		cNKMGameTeamData.m_listDynamicRespawnUnitData.Add(nKMDynamicRespawnUnitData);
		return true;
	}

	protected virtual NKMUnit CreateNewUnitObj()
	{
		if (IsServerGame())
		{
			return new NKMUnitServer();
		}
		return null;
	}

	public bool CheckBossDungeon()
	{
		return GetGameData()?.m_bBossDungeon ?? false;
	}

	protected void LoadCompleteUnit()
	{
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnitPool.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Value?.LoadUnitComplete();
		}
	}

	public virtual List<NKMUnit> GetUnitChain()
	{
		return m_listNKMUnit;
	}

	public virtual NKMUnit GetUnitByUnitID(int unitID, bool bChain = true, bool bPool = false)
	{
		if (bChain)
		{
			foreach (KeyValuePair<short, NKMUnit> item in m_dicNKMUnit)
			{
				NKMUnit value = item.Value;
				if (value.GetUnitData().m_UnitID == unitID)
				{
					return value;
				}
				if (value.GetUnitTemplet().m_UnitTempletBase.IsSameBaseUnit(unitID))
				{
					return value;
				}
			}
		}
		if (bPool)
		{
			foreach (KeyValuePair<short, NKMUnit> item2 in m_dicNKMUnitPool)
			{
				NKMUnit value2 = item2.Value;
				if (value2.GetUnitData().m_UnitID == unitID)
				{
					return value2;
				}
				if (value2.GetUnitTemplet().m_UnitTempletBase.IsSameBaseUnit(unitID))
				{
					return value2;
				}
			}
		}
		return null;
	}

	public virtual NKMUnit GetUnitByUnitID(int unitID, NKM_TEAM_TYPE team, bool bChain = true, bool bPool = false)
	{
		if (bChain)
		{
			foreach (KeyValuePair<short, NKMUnit> item in m_dicNKMUnit)
			{
				NKMUnit value = item.Value;
				if (value.IsAlly(team))
				{
					if (value.GetUnitData().m_UnitID == unitID)
					{
						return value;
					}
					if (value.GetUnitTemplet().m_UnitTempletBase.IsSameBaseUnit(unitID))
					{
						return value;
					}
				}
			}
		}
		if (bPool)
		{
			foreach (KeyValuePair<short, NKMUnit> item2 in m_dicNKMUnitPool)
			{
				NKMUnit value2 = item2.Value;
				if (value2.IsAlly(team))
				{
					if (value2.GetUnitData().m_UnitID == unitID)
					{
						return value2;
					}
					if (value2.GetUnitTemplet().m_UnitTempletBase.IsSameBaseUnit(unitID))
					{
						return value2;
					}
				}
			}
		}
		return null;
	}

	public virtual NKMUnit GetEnemyUnitByUnitID(int unitID, NKM_TEAM_TYPE myTeam, bool bChain = true, bool bPool = false)
	{
		if (bChain)
		{
			foreach (KeyValuePair<short, NKMUnit> item in m_dicNKMUnit)
			{
				NKMUnit value = item.Value;
				if (IsEnemy(value.GetTeam(), myTeam))
				{
					if (value.GetUnitData().m_UnitID == unitID)
					{
						return value;
					}
					if (value.GetUnitTemplet().m_UnitTempletBase.IsSameBaseUnit(unitID))
					{
						return value;
					}
				}
			}
		}
		if (bPool)
		{
			foreach (KeyValuePair<short, NKMUnit> item2 in m_dicNKMUnitPool)
			{
				NKMUnit value2 = item2.Value;
				if (IsEnemy(value2.GetTeam(), myTeam))
				{
					if (value2.GetUnitData().m_UnitID == unitID)
					{
						return value2;
					}
					if (value2.GetUnitTemplet().m_UnitTempletBase.IsSameBaseUnit(unitID))
					{
						return value2;
					}
				}
			}
		}
		return null;
	}

	public void GetUnitByUnitID(List<NKMUnit> listUnit, int unitID, bool bChain = true, bool bPool = false)
	{
		if (bChain)
		{
			foreach (KeyValuePair<short, NKMUnit> item in m_dicNKMUnit)
			{
				NKMUnit value = item.Value;
				if (value.GetUnitData().m_UnitID == unitID)
				{
					listUnit.Add(value);
				}
			}
		}
		if (!bPool)
		{
			return;
		}
		foreach (KeyValuePair<short, NKMUnit> item2 in m_dicNKMUnitPool)
		{
			NKMUnit value2 = item2.Value;
			if (value2.GetUnitData().m_UnitID == unitID)
			{
				listUnit.Add(value2);
			}
		}
	}

	public virtual NKMUnit GetUnit(short GameUnitUID, bool bChain = true, bool bPool = false)
	{
		if (bChain && m_dicNKMUnit.ContainsKey(GameUnitUID))
		{
			return m_dicNKMUnit[GameUnitUID];
		}
		if (bPool && m_dicNKMUnitPool.ContainsKey(GameUnitUID))
		{
			return m_dicNKMUnitPool[GameUnitUID];
		}
		return null;
	}

	public virtual List<NKMUnit> GetTargetingUnitList(short targetedGameUnitUID)
	{
		List<NKMUnit> list = new List<NKMUnit>();
		NKMUnit unit = GetUnit(targetedGameUnitUID);
		if (unit == null)
		{
			return list;
		}
		foreach (KeyValuePair<short, NKMUnit> item in m_dicNKMUnit)
		{
			NKMUnit value = item.Value;
			if (value != unit && value.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DYING && value.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE && value.GetUnitSyncData().m_TargetUID == targetedGameUnitUID)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public virtual NKMDamageEffect GetDamageEffect(short DEUID)
	{
		return null;
	}

	public virtual NKMDamageEffectManager GetDEManager()
	{
		return null;
	}

	public bool IsGameUnitAllDie(long unitUID)
	{
		NKMUnitData unitDataByUnitUID = m_NKMGameData.GetUnitDataByUnitUID(unitUID);
		if (unitDataByUnitUID != null)
		{
			return IsGameUnitAllDie(unitDataByUnitUID, -1);
		}
		return false;
	}

	public NKM_ERROR_CODE IsGameUnitAllInBattle(long unitUID)
	{
		bool flag = false;
		int num = 0;
		NKMUnitData unitDataByUnitUID = m_NKMGameData.GetUnitDataByUnitUID(unitUID);
		if (unitDataByUnitUID != null)
		{
			for (int i = 0; i < unitDataByUnitUID.m_listGameUnitUID.Count; i++)
			{
				short gameUnitUID = unitDataByUnitUID.m_listGameUnitUID[i];
				NKMUnit unit = GetUnit(gameUnitUID);
				if (unit != null && unit.GetUnitStateNow() != null)
				{
					if (unit.GetUnitStateNow().m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_START)
					{
						return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RE_RESPAWN_UNIT_START;
					}
					if (unit.GetUnitStateNow().m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_SKILL || unit.GetUnitStateNow().m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER)
					{
						return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RE_RESPAWN_UNIT_SKILL;
					}
					if (unit.IsDying())
					{
						flag = true;
					}
					else
					{
						num++;
					}
				}
			}
			for (int j = 0; j < unitDataByUnitUID.m_listGameUnitUIDChange.Count; j++)
			{
				short gameUnitUID2 = unitDataByUnitUID.m_listGameUnitUIDChange[j];
				NKMUnit unit2 = GetUnit(gameUnitUID2);
				if (unit2 != null && unit2.GetUnitStateNow() != null)
				{
					if (unit2.IsDying())
					{
						flag = true;
					}
					else
					{
						num++;
					}
				}
			}
			if (flag && num <= 1)
			{
				return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RE_RESPAWN_UNIT_DYING;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public bool IsGameUnitAllDie(NKMUnitData cNKMUnitData, short dieCallerGameUnitUID = -1)
	{
		for (int i = 0; i < cNKMUnitData.m_listGameUnitUID.Count; i++)
		{
			short num = cNKMUnitData.m_listGameUnitUID[i];
			if (dieCallerGameUnitUID == num)
			{
				continue;
			}
			if (!m_dicNKMUnitPool.ContainsKey(num))
			{
				return false;
			}
			for (int j = 0; j < m_listNKMGameUnitDynamicRespawnData.Count; j++)
			{
				if (m_listNKMGameUnitDynamicRespawnData[j].m_GameUnitUID == num)
				{
					return false;
				}
			}
		}
		for (int k = 0; k < cNKMUnitData.m_listGameUnitUIDChange.Count; k++)
		{
			short num2 = cNKMUnitData.m_listGameUnitUIDChange[k];
			if (dieCallerGameUnitUID == num2)
			{
				continue;
			}
			if (!m_dicNKMUnitPool.ContainsKey(num2))
			{
				return false;
			}
			for (int l = 0; l < m_listNKMGameUnitDynamicRespawnData.Count; l++)
			{
				if (m_listNKMGameUnitDynamicRespawnData[l].m_GameUnitUID == num2)
				{
					return false;
				}
			}
		}
		return true;
	}

	protected virtual void ProcecssGameTime()
	{
		NKMProfiler.BeginSample("NKCGameClient.ProcecssGameTime");
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY)
		{
			_ = m_NKMGameRuntimeData.m_fRemainGameTime;
			if (m_NKMGameRuntimeData.m_fRemainGameTime > 0f)
			{
				m_NKMGameRuntimeData.m_fRemainGameTime -= m_fDeltaTime;
				if (m_NKMGameRuntimeData.m_fRemainGameTime <= 0f)
				{
					m_NKMGameRuntimeData.m_fRemainGameTime = 0f;
				}
			}
		}
		NKMProfiler.EndSample();
	}

	protected virtual bool ProcecssValidLand(NKM_TEAM_TYPE myTeam)
	{
		bool result = false;
		if (m_NKMGameData.m_NKMGameTeamDataA.m_MainShip != null && m_NKMGameData.m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID.Count == 0)
		{
			Log.Error("Check the respawn count or unit id.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGame.cs", 3151);
			return false;
		}
		bool bPvP = m_NKMGameData.IsPVP();
		NKMUnit liveBossUnit = GetLiveBossUnit(NKM_TEAM_TYPE.NTT_A1);
		NKMUnit liveBossUnit2 = GetLiveBossUnit(NKM_TEAM_TYPE.NTT_B1);
		if (liveBossUnit != null && liveBossUnit2 != null)
		{
			int num = CalculateCurrentValidLandLevel(NKM_TEAM_TYPE.NTT_B1);
			if (num > m_ShipHPValidLandLevelTeamA)
			{
				m_ShipHPValidLandLevelTeamA = num;
				m_fRespawnCostAddTeamA = GetCostAddFactorByShip(num, liveBossUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
				m_fCoolTimeReduceTeamA = GetCoolTimeReduceFactorByShip(num, liveBossUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
				if (IsATeam(myTeam))
				{
					result = true;
				}
			}
			int num2 = CalculateCurrentValidLandLevel(NKM_TEAM_TYPE.NTT_A1);
			if (num2 > m_ShipHPValidLandLevelTeamB)
			{
				m_ShipHPValidLandLevelTeamB = num2;
				m_fRespawnCostAddTeamB = GetCostAddFactorByShip(num2, liveBossUnit2.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
				m_fCoolTimeReduceTeamB = GetCoolTimeReduceFactorByShip(num2, liveBossUnit2.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
				if (IsBTeam(myTeam))
				{
					result = true;
				}
			}
		}
		m_fRespawnValidLandTeamA = GetValidLandFactor(m_ShipHPValidLandLevelTeamA, bPvP, NKM_TEAM_TYPE.NTT_A1);
		m_fRespawnValidLandTeamB = GetValidLandFactor(m_ShipHPValidLandLevelTeamB, bPvP, NKM_TEAM_TYPE.NTT_B1);
		return result;
	}

	protected int GetCurrentValidLandLevel(NKM_TEAM_TYPE teamType)
	{
		if (IsATeam(teamType))
		{
			return m_ShipHPValidLandLevelTeamA;
		}
		if (IsBTeam(teamType))
		{
			return m_ShipHPValidLandLevelTeamB;
		}
		return 0;
	}

	protected int CalculateCurrentValidLandLevel(NKM_TEAM_TYPE teamType)
	{
		NKMUnit liveBossUnit = GetLiveBossUnit(teamType);
		if (liveBossUnit == null)
		{
			return 0;
		}
		float hPRate = liveBossUnit.GetHPRate();
		if (0.3f > hPRate)
		{
			return 2;
		}
		if (0.6f > hPRate)
		{
			return 1;
		}
		return 0;
	}

	protected float GetValidLandFactor(int level, bool bPvP, NKM_TEAM_TYPE teamType)
	{
		float num = 0f;
		float num2 = 1f;
		float validLandFactorByBossHPLevel = GetValidLandFactorByBossHPLevel(level, bPvP);
		foreach (NKMUnit value in m_dicNKMUnit.Values)
		{
			if (value.IsDyingOrDie())
			{
				continue;
			}
			if (value.IsAlly(teamType))
			{
				if (value.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_PRESS_ZONE))
				{
					float mapFactor = GetMapTemplet().GetMapFactor(value.GetUnitSyncData().m_PosX, IsATeam(teamType));
					if (num < mapFactor)
					{
						num = mapFactor;
					}
				}
			}
			else if (value.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_BLOCK_ZONE))
			{
				float mapFactor2 = GetMapTemplet().GetMapFactor(value.GetUnitSyncData().m_PosX, IsATeam(teamType));
				if (num2 > mapFactor2)
				{
					num2 = mapFactor2;
				}
			}
		}
		if (num > 0.8f)
		{
			num = 0.8f;
		}
		if (num2 < 0.2f)
		{
			num2 = 0.2f;
		}
		num = NKMMathf.Max(num, validLandFactorByBossHPLevel);
		return NKMMathf.Min(num2, num);
	}

	protected float GetValidLandFactorByBossHPLevel(int level, bool bPvP)
	{
		if (GetDungeonType() == NKM_DUNGEON_TYPE.NDT_WAVE)
		{
			level = 2;
		}
		if (m_NKMDungeonTemplet != null && m_NKMDungeonTemplet.m_listValidLand != null)
		{
			return m_NKMDungeonTemplet.m_listValidLand[level];
		}
		if (bPvP)
		{
			return NKMCommonConst.VALID_LAND_PVP[level];
		}
		return NKMCommonConst.VALID_LAND_PVE[level];
	}

	protected float GetCostAddFactorByShip(int level, NKM_UNIT_STYLE_TYPE eNKM_UNIT_STYLE_TYPE)
	{
		return eNKM_UNIT_STYLE_TYPE switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT => SHIP_ASSULT_RESPAWN_COST_ADD[level], 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY => SHIP_HEAVY_RESPAWN_COST_ADD[level], 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER => SHIP_CRUISER_RESPAWN_COST_ADD[level], 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL => SHIP_SPECIAL_RESPAWN_COST_ADD[level], 
			_ => 0f, 
		};
	}

	protected float GetCoolTimeReduceFactorByShip(int level, NKM_UNIT_STYLE_TYPE eNKM_UNIT_STYLE_TYPE)
	{
		return eNKM_UNIT_STYLE_TYPE switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT => SHIP_ASSULT_COOLTIME_REDUCE_ADD[level], 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY => SHIP_HEAVY_COOLTIME_REDUCE_ADD[level], 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER => SHIP_CRUISER_COOLTIME_REDUCE_ADD[level], 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL => SHIP_SPECIAL_COOLTIME_REDUCE_ADD[level], 
			_ => 0f, 
		};
	}

	public NKMUnit GetLiveBossUnit(NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		NKMGameTeamData nKMGameTeamData = null;
		switch (eNKM_TEAM_TYPE)
		{
		case NKM_TEAM_TYPE.NTT_A1:
			nKMGameTeamData = m_NKMGameData.m_NKMGameTeamDataA;
			break;
		case NKM_TEAM_TYPE.NTT_B1:
			nKMGameTeamData = m_NKMGameData.m_NKMGameTeamDataB;
			break;
		}
		if (nKMGameTeamData == null || nKMGameTeamData.m_MainShip == null)
		{
			return null;
		}
		for (int i = 0; i < nKMGameTeamData.m_MainShip.m_listGameUnitUID.Count; i++)
		{
			short gameUnitUID = nKMGameTeamData.m_MainShip.m_listGameUnitUID[i];
			NKMUnit unit = GetUnit(gameUnitUID);
			if (unit != null)
			{
				return unit;
			}
		}
		return null;
	}

	public bool IsLiveEnemyStructureUnit(NKM_TEAM_TYPE eMyTeam, List<NKMUnit> finderSortUnitList)
	{
		if (GetLiveBossUnit(!IsATeam(eMyTeam)) != null)
		{
			return true;
		}
		for (int i = 0; i < finderSortUnitList.Count; i++)
		{
			NKMUnit nKMUnit = finderSortUnitList[i];
			if (nKMUnit != null && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DYING && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE && nKMUnit.WillBeTargetted() && IsEnemy(eMyTeam, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE) && nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_TOWER)
			{
				return true;
			}
		}
		return false;
	}

	public NKMUnit GetLiveMyBossUnit(NKM_TEAM_TYPE eMyTeam)
	{
		return GetLiveBossUnit(IsATeam(eMyTeam));
	}

	public NKMUnit GetLiveEnemyBossUnit(NKM_TEAM_TYPE eMyTeam)
	{
		return GetLiveBossUnit(!IsATeam(eMyTeam));
	}

	public NKMUnit GetLiveBossUnit(bool bTeamA)
	{
		NKMGameTeamData nKMGameTeamData = null;
		nKMGameTeamData = ((!bTeamA) ? m_NKMGameData.m_NKMGameTeamDataB : m_NKMGameData.m_NKMGameTeamDataA);
		if (nKMGameTeamData.m_MainShip == null)
		{
			return null;
		}
		for (int i = 0; i < nKMGameTeamData.m_MainShip.m_listGameUnitUID.Count; i++)
		{
			short gameUnitUID = nKMGameTeamData.m_MainShip.m_listGameUnitUID[i];
			NKMUnit unit = GetUnit(gameUnitUID);
			if (unit != null && unit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY)
			{
				return unit;
			}
		}
		return null;
	}

	public NKMUnit GetLiveUnit(bool bTeamA)
	{
		foreach (NKMUnit item in m_listNKMUnit)
		{
			if (item.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY && item.WillBeTargetted() && IsATeam(item.GetUnitDataGame().m_NKM_TEAM_TYPE_ORG) == bTeamA)
			{
				return item;
			}
		}
		return null;
	}

	public NKMUnit GetLiveUnit(bool bTeamA, bool bAirUnit)
	{
		foreach (NKMUnit item in m_listNKMUnit)
		{
			if (item.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY && item.WillBeTargetted() && IsATeam(item.GetUnitDataGame().m_NKM_TEAM_TYPE_ORG) == bTeamA && item.IsAirUnit() == bAirUnit)
			{
				return item;
			}
		}
		return null;
	}

	public float GetLiveShipHPRate(NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		NKMGameTeamData nKMGameTeamData = null;
		switch (eNKM_TEAM_TYPE)
		{
		case NKM_TEAM_TYPE.NTT_A1:
			nKMGameTeamData = m_NKMGameData.m_NKMGameTeamDataA;
			break;
		case NKM_TEAM_TYPE.NTT_B1:
			nKMGameTeamData = m_NKMGameData.m_NKMGameTeamDataB;
			break;
		}
		if (nKMGameTeamData == null || nKMGameTeamData.m_MainShip == null)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < nKMGameTeamData.m_MainShip.m_listGameUnitUID.Count; i++)
		{
			short gameUnitUID = nKMGameTeamData.m_MainShip.m_listGameUnitUID[i];
			NKMUnit unit = GetUnit(gameUnitUID);
			if (unit != null)
			{
				num += unit.GetUnitSyncData().GetHP();
				num2 += unit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP);
			}
		}
		if (num2 <= 0f)
		{
			return 0f;
		}
		return num / num2;
	}

	public virtual float CaculateFiercePointByDamage(NKMUnit target)
	{
		return 0f;
	}

	public virtual float CaculateTrimPointByDamage(NKMUnit target)
	{
		return 0f;
	}

	protected virtual void ProcecssRespawnCost()
	{
		if (GetWorldStopTime() <= 0f)
		{
			float num = m_fDeltaTime * 0.3f;
			float num2 = m_fDeltaTime * 0.3f;
			num -= num * m_NKMGameData.m_fRespawnCostMinusPercentForTeamA;
			if (m_NKMDungeonTemplet != null)
			{
				num *= m_NKMDungeonTemplet.m_fCostSpeedRateA;
				num2 *= m_NKMDungeonTemplet.m_fCostSpeedRateB;
			}
			num += Math.Max(m_fDeltaTime * m_NKMGameData.fExtraRespawnCostAddForA, -1f);
			num2 += Math.Max(m_fDeltaTime * m_NKMGameData.fExtraRespawnCostAddForB, -1f);
			switch (GetDungeonType())
			{
			case NKM_DUNGEON_TYPE.NDT_WAVE:
			case NKM_DUNGEON_TYPE.NDT_RAID:
			case NKM_DUNGEON_TYPE.NDT_SOLO_RAID:
				m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost += num;
				m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost += num2;
				break;
			case NKM_DUNGEON_TYPE.NDT_DAMAGE_ACCRUE:
				m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost += num * 2f;
				m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost += num2;
				break;
			default:
				if (m_NKMGameRuntimeData.m_fRemainGameTime <= GetGameData().m_fDoubleCostTime)
				{
					m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost += num * 1.5f;
					m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost += num2 * 1.5f;
				}
				else
				{
					m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost += num;
					m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost += num2;
				}
				break;
			}
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost += num * (m_fRespawnCostAddTeamA + m_fRespawnCostFullDeckAddTeamA);
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost += num2 * (m_fRespawnCostAddTeamB + m_fRespawnCostFullDeckAddTeamB);
		}
		if (m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost > 10f)
		{
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost = 10f;
		}
		if (m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost > 10f)
		{
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost = 10f;
		}
		if (GetWorldStopTime() <= 0f)
		{
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCostAssist += m_fDeltaTime * 0.3f * 0.5f;
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCostAssist += m_fDeltaTime * 0.3f * 0.5f;
		}
		if (m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCostAssist > 10f)
		{
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCostAssist = 10f;
		}
		if (m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCostAssist > 10f)
		{
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCostAssist = 10f;
		}
	}

	public int GetLiveUnitCount(NKM_TEAM_TYPE teamType)
	{
		switch (teamType)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			return m_TeamALiveUnitUID.Count;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			return m_TeamBLiveUnitUID.Count;
		default:
			return 0;
		}
	}

	protected bool IsGameUsingDoubleCostTime()
	{
		switch (GetGameData().GetGameType())
		{
		case NKM_GAME_TYPE.NGT_PVP_RANK:
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
			return NKMPvpCommonConst.Instance.UseDoubleCostTimeRank;
		case NKM_GAME_TYPE.NGT_PVP_EVENT:
			return NKMPvpCommonConst.Instance.UseDoubleCostTimeArcade;
		default:
		{
			NKM_DUNGEON_TYPE dungeonType = GetDungeonType();
			if ((uint)(dungeonType - 3) <= 3u)
			{
				return false;
			}
			return true;
		}
		}
	}

	protected virtual bool CheckRespawnCountMax(NKM_TEAM_TYPE teamType)
	{
		if (IsATeam(teamType) && GetDungeonTemplet() != null && GetDungeonTemplet().m_DungeonTempletBase.m_RespawnCountMaxSameTime > 0 && GetLiveUnitCount(teamType) >= GetDungeonTemplet().m_DungeonTempletBase.m_RespawnCountMaxSameTime)
		{
			return true;
		}
		return false;
	}

	protected void CalcLiveUnitCount()
	{
		m_TeamALiveUnitUID.Clear();
		for (int i = 0; i < GetGameData().m_NKMGameTeamDataA.m_listUnitData.Count; i++)
		{
			NKMUnitData nKMUnitData = GetGameData().m_NKMGameTeamDataA.m_listUnitData[i];
			if (nKMUnitData == null)
			{
				continue;
			}
			for (int j = 0; j < nKMUnitData.m_listGameUnitUID.Count; j++)
			{
				if (GetUnit(nKMUnitData.m_listGameUnitUID[j]) != null)
				{
					if (!m_TeamALiveUnitUID.Contains(nKMUnitData.m_UnitUID))
					{
						m_TeamALiveUnitUID.Add(nKMUnitData.m_UnitUID);
					}
					break;
				}
			}
			foreach (short item in nKMUnitData.m_listGameUnitUIDChange)
			{
				if (GetUnit(item) != null)
				{
					m_TeamALiveUnitUID.Add(nKMUnitData.m_UnitUID);
					break;
				}
			}
		}
		m_TeamBLiveUnitUID.Clear();
		for (int k = 0; k < GetGameData().m_NKMGameTeamDataB.m_listUnitData.Count; k++)
		{
			NKMUnitData nKMUnitData2 = GetGameData().m_NKMGameTeamDataB.m_listUnitData[k];
			if (nKMUnitData2 == null)
			{
				continue;
			}
			for (int l = 0; l < nKMUnitData2.m_listGameUnitUID.Count; l++)
			{
				if (GetUnit(nKMUnitData2.m_listGameUnitUID[l]) != null)
				{
					if (!m_TeamBLiveUnitUID.Contains(nKMUnitData2.m_UnitUID))
					{
						m_TeamBLiveUnitUID.Add(nKMUnitData2.m_UnitUID);
					}
					break;
				}
			}
			foreach (short item2 in nKMUnitData2.m_listGameUnitUIDChange)
			{
				if (GetUnit(item2) != null)
				{
					m_TeamBLiveUnitUID.Add(nKMUnitData2.m_UnitUID);
					break;
				}
			}
		}
		AddRespawnUnitsToLiveUnit();
	}

	public void AddRespawnUnitsToLiveUnit()
	{
		for (int i = 0; i < m_listNKMGameUnitRespawnData.Count; i++)
		{
			NKMGameUnitRespawnData nKMGameUnitRespawnData = m_listNKMGameUnitRespawnData[i];
			if (nKMGameUnitRespawnData == null)
			{
				continue;
			}
			if (IsATeam(nKMGameUnitRespawnData.m_eNKM_TEAM_TYPE))
			{
				if (!m_TeamALiveUnitUID.Contains(nKMGameUnitRespawnData.m_UnitUID))
				{
					m_TeamALiveUnitUID.Add(nKMGameUnitRespawnData.m_UnitUID);
				}
			}
			else if (!m_TeamBLiveUnitUID.Contains(nKMGameUnitRespawnData.m_UnitUID))
			{
				m_TeamBLiveUnitUID.Add(nKMGameUnitRespawnData.m_UnitUID);
			}
		}
	}

	protected virtual void ProcessUnit()
	{
		m_listDieGameUnitUID.Clear();
		bool flag = false;
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnit.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMUnit value = enumerator.Current.Value;
			value.Update(m_fDeltaTime);
			if (!(m_NKMGameRuntimeData.m_fShipDamage > 0f))
			{
				continue;
			}
			long unitUID = value.GetUnitData().m_UnitUID;
			if (m_NKMGameData.GetAnyTeamMainShipDataByUnitUID(unitUID) == null)
			{
				continue;
			}
			float hP = value.GetUnitSyncData().GetHP();
			if (!(hP > 0f))
			{
				continue;
			}
			hP -= m_NKMGameRuntimeData.m_fShipDamage * m_fDeltaTime;
			if (hP <= 0f)
			{
				if (m_NKM_GAME_CLASS_TYPE == NKM_GAME_CLASS_TYPE.NGCT_GAME_CLIENT)
				{
					hP = 1f;
				}
				flag = true;
			}
			value.GetUnitSyncData().SetHP(hP);
		}
		if (flag)
		{
			m_NKMGameRuntimeData.m_fShipDamage = 0f;
		}
		enumerator = m_dicNKMUnit.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMUnit value2 = enumerator.Current.Value;
			value2.Update2();
			if (value2.IsDie() && m_NKMGameData.GetAnyTeamMainShipDataByUnitUID(value2.GetUnitData().m_UnitUID) == null)
			{
				m_listDieGameUnitUID.Add(value2.GetUnitSyncData().m_GameUnitUID);
			}
		}
		for (int i = 0; i < m_listDieGameUnitUID.Count; i++)
		{
			short key = m_listDieGameUnitUID[i];
			NKMUnit nKMUnit = m_dicNKMUnit[key];
			m_dicNKMUnitPool.Add(key, nKMUnit);
			m_dicNKMUnit.Remove(key);
			m_listNKMUnit.Remove(nKMUnit);
			nKMUnit.SetDie();
		}
		m_listDieGameUnitUID.Clear();
		CalcLiveUnitCount();
	}

	protected virtual void ProcessTacticalCommand()
	{
		if (GetGameData().m_NKMGameTeamDataA != null)
		{
			for (int i = 0; i < GetGameData().m_NKMGameTeamDataA.m_listTacticalCommandData.Count; i++)
			{
				NKMTacticalCommandData nKMTacticalCommandData = GetGameData().m_NKMGameTeamDataA.m_listTacticalCommandData[i];
				if (nKMTacticalCommandData != null && GetWorldStopTime() <= 0f)
				{
					nKMTacticalCommandData.Update(m_fDeltaTime);
				}
			}
		}
		if (GetGameData().m_NKMGameTeamDataB == null)
		{
			return;
		}
		for (int j = 0; j < GetGameData().m_NKMGameTeamDataB.m_listTacticalCommandData.Count; j++)
		{
			NKMTacticalCommandData nKMTacticalCommandData2 = GetGameData().m_NKMGameTeamDataB.m_listTacticalCommandData[j];
			if (nKMTacticalCommandData2 != null && GetWorldStopTime() <= 0f)
			{
				nKMTacticalCommandData2.Update(m_fDeltaTime);
			}
		}
	}

	protected void ProcessReAttack()
	{
		NKMProfiler.BeginSample("NKMGame.ProcessReAttack");
		LinkedListNode<NKMDamageInst> linkedListNode = m_linklistReAttack.First;
		while (linkedListNode != null)
		{
			NKMDamageInst value = linkedListNode.Value;
			if (value != null && value.m_fReAttackGap > 0f)
			{
				value.m_fReAttackGap -= m_fDeltaTime;
				if (value.m_fReAttackGap <= 0f)
				{
					value.m_ReAttackCount++;
					value.m_fReAttackGap = value.m_Templet.m_fReAttackGap;
					NKMUnit unit = GetUnit(value.m_DefenderUID);
					if (unit != null)
					{
						unit.DamageReact(value, bBuffDamage: false);
						if (value.m_ReActResult == NKM_REACT_TYPE.NRT_NO)
						{
							m_ObjectPool.CloseObj(value);
							LinkedListNode<NKMDamageInst> next = linkedListNode.Next;
							m_linklistReAttack.Remove(linkedListNode);
							linkedListNode = next;
							continue;
						}
					}
					if (value.m_ReAttackCount >= value.m_Templet.m_ReAttackCount)
					{
						m_ObjectPool.CloseObj(value);
						LinkedListNode<NKMDamageInst> next2 = linkedListNode.Next;
						m_linklistReAttack.Remove(linkedListNode);
						linkedListNode = next2;
						continue;
					}
				}
			}
			linkedListNode = linkedListNode.Next;
		}
		NKMProfiler.EndSample();
	}

	public NKMUnit FindTarget(NKMUnit finderUnit, List<NKMUnit> finderSortUnitList, NKMFindTargetData cNKMFindTargetData, NKM_TEAM_TYPE eFinderTeam, float fFinderPosX, float fFinderSizeX, bool bFinderRight)
	{
		if (cNKMFindTargetData == null)
		{
			return null;
		}
		NKMUnit nKMUnit = null;
		bool flag;
		bool bEnemy;
		bool bLand;
		bool bAir;
		bool bStructureOnly;
		switch (cNKMFindTargetData.m_FindTargetType)
		{
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR_FIRST:
			flag = true;
			bEnemy = true;
			bLand = true;
			bAir = true;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
			flag = true;
			bEnemy = true;
			bLand = true;
			bAir = false;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND_RANGER_SUPPORTER_SNIPER_FIRST:
			flag = true;
			bEnemy = true;
			bLand = true;
			bAir = false;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
			flag = true;
			bEnemy = true;
			bLand = false;
			bAir = true;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_BOSS_LAST:
			flag = false;
			bEnemy = true;
			bLand = true;
			bAir = true;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND_BOSS_LAST:
			flag = false;
			bEnemy = true;
			bLand = true;
			bAir = false;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR_BOSS_LAST:
			flag = false;
			bEnemy = true;
			bLand = false;
			bAir = true;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY:
			if (GetLiveEnemyBossUnit(eFinderTeam) != null)
			{
				flag = true;
				bEnemy = true;
				bLand = false;
				bAir = true;
				bStructureOnly = true;
			}
			else
			{
				flag = true;
				bEnemy = true;
				bLand = true;
				bAir = true;
				bStructureOnly = false;
			}
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS:
			if (IsLiveEnemyStructureUnit(eFinderTeam, finderSortUnitList))
			{
				flag = true;
				bEnemy = true;
				bLand = true;
				bAir = true;
				bStructureOnly = true;
			}
			else
			{
				flag = true;
				bEnemy = true;
				bLand = true;
				bAir = true;
				bStructureOnly = false;
			}
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
			if (IsLiveEnemyStructureUnit(eFinderTeam, finderSortUnitList))
			{
				flag = true;
				bEnemy = true;
				bLand = true;
				bAir = false;
				bStructureOnly = true;
			}
			else
			{
				flag = true;
				bEnemy = true;
				bLand = true;
				bAir = false;
				bStructureOnly = false;
			}
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
			if (IsLiveEnemyStructureUnit(eFinderTeam, finderSortUnitList))
			{
				flag = true;
				bEnemy = true;
				bLand = false;
				bAir = true;
				bStructureOnly = true;
			}
			else
			{
				flag = true;
				bEnemy = true;
				bLand = false;
				bAir = true;
				bStructureOnly = false;
			}
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM:
			flag = true;
			bEnemy = false;
			bLand = true;
			bAir = true;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LAND:
			flag = true;
			bEnemy = false;
			bLand = true;
			bAir = false;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_AIR:
			flag = true;
			bEnemy = false;
			bLand = false;
			bAir = true;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP:
			flag = true;
			bEnemy = false;
			bLand = true;
			bAir = true;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_LAND:
			flag = true;
			bEnemy = false;
			bLand = true;
			bAir = false;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_AIR:
			flag = true;
			bEnemy = false;
			bLand = false;
			bAir = true;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM:
			flag = false;
			bEnemy = false;
			bLand = true;
			bAir = true;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_AIR:
			flag = false;
			bEnemy = false;
			bLand = false;
			bAir = true;
			bStructureOnly = false;
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_MY_TEAM_LAND:
			flag = false;
			bEnemy = false;
			bLand = true;
			bAir = false;
			bStructureOnly = false;
			break;
		default:
			return null;
		}
		if (flag)
		{
			return FindNearUnit(finderUnit, finderSortUnitList, cNKMFindTargetData, eFinderTeam, fFinderPosX, fFinderSizeX, bFinderRight, bEnemy, bLand, bAir, bStructureOnly);
		}
		return FindFarUnit(finderUnit, finderSortUnitList, cNKMFindTargetData, eFinderTeam, fFinderPosX, fFinderSizeX, bFinderRight, bEnemy, bLand, bAir, bStructureOnly);
	}

	protected NKMUnit FindNearUnit(NKMUnit finderUnit, List<NKMUnit> finderSortUnitList, NKMFindTargetData cNKMFindTargetData, NKM_TEAM_TYPE eFinderTeam, float fFinderPosX, float fFinderSizeX, bool bFinderRight, bool bEnemy, bool bLand, bool bAir, bool bStructureOnly)
	{
		if (finderSortUnitList == null)
		{
			return null;
		}
		NKMUnit nKMUnit = null;
		bool flag = cNKMFindTargetData.m_hsFindTargetRolePriority != null && cNKMFindTargetData.m_hsFindTargetRolePriority.Count > 0;
		bool flag2 = cNKMFindTargetData.m_hsFindTargetStylePriority != null && cNKMFindTargetData.m_hsFindTargetStylePriority.Count > 0;
		bool flag3 = false;
		for (int i = 0; i < finderSortUnitList.Count; i++)
		{
			NKMUnit nKMUnit2 = finderSortUnitList[i];
			if (nKMUnit2 == null)
			{
				continue;
			}
			NKMUnitTempletBase unitTempletBase = nKMUnit2.GetUnitTempletBase();
			if (nKMUnit2.GetDist(fFinderPosX, fFinderSizeX, cNKMFindTargetData.m_bUseUnitSize) >= cNKMFindTargetData.m_fSeeRange || nKMUnit2.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || nKMUnit2.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE || !nKMUnit2.WillBeTargetted() || bEnemy != IsEnemy(eFinderTeam, nKMUnit2.GetUnitDataGame().m_NKM_TEAM_TYPE) || ((flag3 || cNKMFindTargetData.m_bPriorityOnly) && ((flag && !cNKMFindTargetData.m_hsFindTargetRolePriority.Contains(unitTempletBase.m_NKM_UNIT_ROLE_TYPE)) || (flag2 && !unitTempletBase.HasUnitStyleType(cNKMFindTargetData.m_hsFindTargetStylePriority)))) || (bEnemy && (finderUnit == null || !finderUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_DETECTER)) && nKMUnit2.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_CLOCKING)) || (nKMUnit2.GetUnitStateNow() != null && nKMUnit2.GetUnitStateNow().m_bForceNoTargeted))
			{
				continue;
			}
			bool flag4 = false;
			if (IsBoss(nKMUnit2.GetUnitDataGame().m_GameUnitUID))
			{
				flag4 = true;
			}
			if ((!flag4 && ((!bAir && nKMUnit2.IsAirUnit()) || (!bLand && !nKMUnit2.IsAirUnit()) || (bStructureOnly && unitTempletBase.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_TOWER) || cNKMFindTargetData.m_FindTargetType == NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY)) || (flag4 && !cNKMFindTargetData.m_bCanTargetBoss) || (cNKMFindTargetData.m_bNoBackTarget && ((bFinderRight && nKMUnit2.GetUnitSyncData().m_PosX < fFinderPosX) || (!bFinderRight && nKMUnit2.GetUnitSyncData().m_PosX > fFinderPosX))) || (cNKMFindTargetData.m_bNoFrontTarget && ((bFinderRight && nKMUnit2.GetUnitSyncData().m_PosX > fFinderPosX) || (!bFinderRight && nKMUnit2.GetUnitSyncData().m_PosX < fFinderPosX))))
			{
				continue;
			}
			bool flag5 = false;
			switch (cNKMFindTargetData.m_FindTargetType)
			{
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_AIR:
				if (nKMUnit == null)
				{
					nKMUnit = nKMUnit2;
				}
				else if (nKMUnit2.GetUnitSyncData().GetHP() < nKMUnit.GetUnitSyncData().GetHP())
				{
					nKMUnit = nKMUnit2;
				}
				break;
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR_FIRST:
				if (nKMUnit == null)
				{
					nKMUnit = nKMUnit2;
				}
				else if (!nKMUnit.IsAirUnit() && nKMUnit2.IsAirUnit())
				{
					nKMUnit = nKMUnit2;
				}
				break;
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND_RANGER_SUPPORTER_SNIPER_FIRST:
			{
				if (nKMUnit == null)
				{
					nKMUnit = nKMUnit2;
					break;
				}
				NKMUnitTempletBase unitTempletBase2 = nKMUnit.GetUnitTempletBase();
				if (unitTempletBase2.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_RANGER && unitTempletBase2.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_SNIPER && unitTempletBase2.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER && (unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_RANGER || unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_SNIPER || unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER))
				{
					nKMUnit = nKMUnit2;
				}
				break;
			}
			default:
				nKMUnit = nKMUnit2;
				flag5 = true;
				break;
			}
			if (nKMUnit != null && (flag || flag2))
			{
				NKMUnitTempletBase unitTempletBase3 = nKMUnit.GetUnitTempletBase();
				bool num = !flag || cNKMFindTargetData.m_hsFindTargetRolePriority.Contains(unitTempletBase3.m_NKM_UNIT_ROLE_TYPE);
				bool flag6 = !flag2 || unitTempletBase3.HasUnitStyleType(cNKMFindTargetData.m_hsFindTargetStylePriority);
				flag3 = num && flag6;
			}
			if (flag5 && (flag3 || (!flag && !flag2)))
			{
				break;
			}
		}
		return nKMUnit;
	}

	protected NKMUnit FindFarUnit(NKMUnit finderUnit, List<NKMUnit> finderSortUnitList, NKMFindTargetData cNKMFindTargetData, NKM_TEAM_TYPE eFinderTeam, float fFinderPosX, float fFinderSizeX, bool bFinderRight, bool bEnemy, bool bLand, bool bAir, bool bStructureOnly)
	{
		NKMUnit nKMUnit = null;
		bool flag = cNKMFindTargetData.m_hsFindTargetRolePriority != null && cNKMFindTargetData.m_hsFindTargetRolePriority.Count > 0;
		bool flag2 = cNKMFindTargetData.m_hsFindTargetStylePriority != null && cNKMFindTargetData.m_hsFindTargetStylePriority.Count > 0;
		if (finderSortUnitList != null)
		{
			bool flag3 = false;
			for (int num = finderSortUnitList.Count - 1; num >= 0; num--)
			{
				NKMUnit nKMUnit2 = finderSortUnitList[num];
				if (nKMUnit2 != null)
				{
					NKMUnitTempletBase unitTempletBase = nKMUnit2.GetUnitTempletBase();
					if (!(nKMUnit2.GetDist(fFinderPosX, fFinderSizeX, cNKMFindTargetData.m_bUseUnitSize) >= cNKMFindTargetData.m_fSeeRange) && nKMUnit2.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DYING && nKMUnit2.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE && nKMUnit2.WillBeTargetted() && bEnemy == IsEnemy(eFinderTeam, nKMUnit2.GetUnitDataGame().m_NKM_TEAM_TYPE) && ((!flag3 && !cNKMFindTargetData.m_bPriorityOnly) || ((!flag || cNKMFindTargetData.m_hsFindTargetRolePriority.Contains(unitTempletBase.m_NKM_UNIT_ROLE_TYPE)) && (!flag2 || unitTempletBase.HasUnitStyleType(cNKMFindTargetData.m_hsFindTargetStylePriority)))) && (!bEnemy || (finderUnit != null && finderUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_DETECTER)) || !nKMUnit2.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_CLOCKING)) && (nKMUnit2.GetUnitStateNow() == null || !nKMUnit2.GetUnitStateNow().m_bForceNoTargeted))
					{
						bool flag4 = false;
						if (IsBoss(nKMUnit2.GetUnitDataGame().m_GameUnitUID))
						{
							flag4 = true;
						}
						if ((flag4 || ((bAir || !nKMUnit2.IsAirUnit()) && (bLand || nKMUnit2.IsAirUnit()) && (!bStructureOnly || unitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_TOWER))) && (!flag4 || cNKMFindTargetData.m_bCanTargetBoss) && (!cNKMFindTargetData.m_bNoBackTarget || ((!bFinderRight || !(nKMUnit2.GetUnitSyncData().m_PosX < fFinderPosX)) && (bFinderRight || !(nKMUnit2.GetUnitSyncData().m_PosX > fFinderPosX)))) && (!cNKMFindTargetData.m_bNoFrontTarget || ((!bFinderRight || !(nKMUnit2.GetUnitSyncData().m_PosX > fFinderPosX)) && (bFinderRight || !(nKMUnit2.GetUnitSyncData().m_PosX < fFinderPosX)))))
						{
							bool flag5 = false;
							NKM_FIND_TARGET_TYPE findTargetType = cNKMFindTargetData.m_FindTargetType;
							if ((uint)(findTargetType - 10) <= 2u)
							{
								if (nKMUnit == null)
								{
									nKMUnit = nKMUnit2;
								}
								else if (nKMUnit.IsBoss() && !nKMUnit2.IsBoss())
								{
									nKMUnit = nKMUnit2;
								}
							}
							else
							{
								nKMUnit = nKMUnit2;
								flag5 = true;
							}
							if (nKMUnit != null && (flag || flag2))
							{
								NKMUnitTempletBase unitTempletBase2 = nKMUnit.GetUnitTempletBase();
								bool num2 = !flag || cNKMFindTargetData.m_hsFindTargetRolePriority.Contains(unitTempletBase2.m_NKM_UNIT_ROLE_TYPE);
								bool flag6 = !flag2 || unitTempletBase2.HasUnitStyleType(cNKMFindTargetData.m_hsFindTargetStylePriority);
								flag3 = num2 && flag6;
							}
							if (flag5 && (flag3 || (!flag && !flag2)))
							{
								break;
							}
						}
					}
				}
			}
		}
		return nKMUnit;
	}

	public bool DamageCheck(NKMDamageInst cNKMDamageInst, NKMEventAttack cNKMEventAttack, bool bDieAttack = false)
	{
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY)
		{
			return false;
		}
		bool flag = false;
		if (cNKMDamageInst == null || cNKMDamageInst.m_Templet == null)
		{
			return false;
		}
		switch (cNKMDamageInst.m_AttackerType)
		{
		case NKM_REACTOR_TYPE.NRT_GAME_UNIT:
			flag = UnitToUnit(cNKMDamageInst, cNKMEventAttack);
			break;
		case NKM_REACTOR_TYPE.NRT_DAMAGE_EFFECT:
			flag = EffectToUnit(cNKMDamageInst, cNKMEventAttack, bDieAttack);
			break;
		}
		if (flag)
		{
			ProcessExtraHit(cNKMDamageInst, cNKMEventAttack);
		}
		return flag;
	}

	private void ProcessExtraHit(NKMDamageInst cNKMDamageInst, NKMEventAttack cNKMEventAttack)
	{
		if (cNKMDamageInst.m_Templet.m_ExtraHitDamageTemplet == null)
		{
			return;
		}
		NKMUnit unit = GetUnit(cNKMDamageInst.m_AttackerGameUnitUID);
		if (unit == null)
		{
			return;
		}
		int count = cNKMDamageInst.m_listHitUnit.Count;
		if (cNKMDamageInst.m_Templet.m_ExtraHitCountRange.m_Min > count || count > cNKMDamageInst.m_Templet.m_ExtraHitCountRange.m_Max)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			NKMUnit unit2 = GetUnit(cNKMDamageInst.m_listHitUnit[i]);
			if (unit2 != null)
			{
				ProcessDamageTemplet(cNKMDamageInst.m_Templet.m_ExtraHitDamageTemplet, unit, unit2, bUseAttackerStat: true, buffDamage: false, cNKMDamageInst.m_AttackerUnitSkillTemplet, cNKMDamageInst.m_Templet.m_ExtraHitAttribute);
			}
		}
	}

	protected bool UnitToUnit(NKMDamageInst cNKMDamageInst, NKMEventAttack cNKMEventAttack)
	{
		bool flag = false;
		NKMUnit unit = GetUnit(cNKMDamageInst.m_AttackerGameUnitUID);
		NKMUnit nKMUnit = null;
		if (unit == null)
		{
			return false;
		}
		cNKMDamageInst.m_EventAttack = cNKMEventAttack;
		float num = 0f;
		bool bSplashHit = cNKMEventAttack.m_AttackUnitCount > 1;
		if (cNKMEventAttack.m_AttackTargetUnit)
		{
			if (cNKMEventAttack.m_AttackUnitCount == 1 && cNKMDamageInst.m_AttackCount >= 1)
			{
				return flag;
			}
			if (cNKMEventAttack.m_AttackUnitCountOnly && cNKMDamageInst.m_AttackCount >= cNKMEventAttack.m_AttackUnitCount)
			{
				return flag;
			}
			nKMUnit = unit.GetTargetUnit(bDying: true);
			flag = HitCheck(cNKMDamageInst, cNKMEventAttack, unit, nKMUnit, num, bSplashHit);
			if (flag && nKMUnit != null)
			{
				if (nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_DEFENDER)
				{
					num += NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE;
					float statFinal = nKMUnit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_DEFENDER_PROTECT_RATE);
					num += NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE * statFinal;
				}
				if (num > NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE_MAX)
				{
					num = NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE_MAX;
				}
			}
		}
		List<NKMUnit> sortUnitListByNearDist = unit.GetSortUnitListByNearDist();
		if (sortUnitListByNearDist.Count > 0)
		{
			for (int i = 0; i < sortUnitListByNearDist.Count; i++)
			{
				if (cNKMEventAttack.m_AttackUnitCount == 1 && cNKMDamageInst.m_AttackCount >= 1)
				{
					break;
				}
				if (cNKMEventAttack.m_AttackUnitCountOnly && cNKMDamageInst.m_AttackCount >= cNKMEventAttack.m_AttackUnitCount)
				{
					break;
				}
				nKMUnit = sortUnitListByNearDist[i];
				if (nKMUnit == null)
				{
					continue;
				}
				if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE)
				{
					break;
				}
				if (HitCheck(cNKMDamageInst, cNKMEventAttack, unit, nKMUnit, num, bSplashHit))
				{
					if (nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_DEFENDER)
					{
						num += NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE;
						float statFinal2 = nKMUnit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_DEFENDER_PROTECT_RATE);
						num += NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE * statFinal2;
					}
					if (num > NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE_MAX)
					{
						num = NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE_MAX;
					}
					flag = true;
				}
			}
		}
		return flag;
	}

	protected bool EffectToUnit(NKMDamageInst cNKMDamageInst, NKMEventAttack cNKMEventAttack, bool bDieAttack = false)
	{
		bool flag = false;
		NKMDamageEffect damageEffect = GetDamageEffect(cNKMDamageInst.m_AttackerEffectUID);
		if (damageEffect == null)
		{
			return false;
		}
		NKMUnit masterUnit = damageEffect.GetMasterUnit();
		NKMUnit nKMUnit = null;
		if (damageEffect.IsEnd())
		{
			return false;
		}
		if (damageEffect.GetDEData() == null)
		{
			return false;
		}
		if (damageEffect.GetTemplet() == null)
		{
			return false;
		}
		if (masterUnit == null)
		{
			return false;
		}
		cNKMDamageInst.m_EventAttack = cNKMEventAttack;
		float num = 0f;
		bool bSplashHit = (cNKMEventAttack.m_AttackUnitCount != 1 || !cNKMEventAttack.m_AttackUnitCountOnly) && damageEffect.GetTemplet().m_DamageCountMax != 1;
		NKMUnit targetUnit = damageEffect.GetTargetUnit();
		if (cNKMEventAttack.m_AttackUnitCount == 1 && cNKMEventAttack.m_AttackTargetUnit && targetUnit != null && targetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			nKMUnit = targetUnit;
			bool num2 = HitCheck(cNKMDamageInst, cNKMEventAttack, damageEffect, masterUnit, nKMUnit, num, bSplashHit);
			if (num2)
			{
				if (nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_DEFENDER)
				{
					num += NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE;
					float statFinal = nKMUnit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_DEFENDER_PROTECT_RATE);
					num += NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE * statFinal;
				}
				if (num > NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE_MAX)
				{
					num = NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE_MAX;
				}
			}
			return num2;
		}
		if (cNKMEventAttack.m_AttackTargetUnit)
		{
			nKMUnit = damageEffect.GetTargetUnit();
			if (nKMUnit != null)
			{
				flag = HitCheck(cNKMDamageInst, cNKMEventAttack, damageEffect, masterUnit, nKMUnit, num, bSplashHit);
				if (flag)
				{
					if (nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_DEFENDER)
					{
						num += NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE;
						float statFinal2 = nKMUnit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_DEFENDER_PROTECT_RATE);
						num += NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE * statFinal2;
					}
					if (num > NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE_MAX)
					{
						num = NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE_MAX;
					}
					flag = true;
				}
			}
		}
		if (!bDieAttack && damageEffect.GetDEData().m_DamageCountNow >= damageEffect.GetTemplet().m_DamageCountMax)
		{
			return flag;
		}
		List<NKMUnit> sortUnitListByNearDist = damageEffect.GetSortUnitListByNearDist();
		if (sortUnitListByNearDist.Count > 0)
		{
			for (int i = 0; i < sortUnitListByNearDist.Count; i++)
			{
				if (cNKMEventAttack.m_AttackUnitCount == 1 && cNKMDamageInst.m_AttackCount >= 1)
				{
					break;
				}
				if (cNKMEventAttack.m_AttackUnitCountOnly && cNKMDamageInst.m_AttackCount >= cNKMEventAttack.m_AttackUnitCount)
				{
					break;
				}
				if (damageEffect.IsEnd())
				{
					break;
				}
				if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE)
				{
					break;
				}
				if (!bDieAttack && damageEffect.GetDEData().m_DamageCountNow >= damageEffect.GetTemplet().m_DamageCountMax)
				{
					break;
				}
				nKMUnit = sortUnitListByNearDist[i];
				if (nKMUnit != null && HitCheck(cNKMDamageInst, cNKMEventAttack, damageEffect, masterUnit, nKMUnit, num, bSplashHit))
				{
					if (nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE == NKM_UNIT_ROLE_TYPE.NURT_DEFENDER)
					{
						num += NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE;
						float statFinal3 = nKMUnit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_DEFENDER_PROTECT_RATE);
						num += NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE * statFinal3;
					}
					if (num > NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE_MAX)
					{
						num = NKMUnitStatManager.m_fDEFENDER_PROTECT_RATE_MAX;
					}
					flag = true;
				}
			}
		}
		return flag;
	}

	protected bool CollisionCheck(float fAtkBeforeX, float fAtkNowX, float fAtkSizeX, bool bAtkRight, NKMEventAttack cNKMEventAttack, float fAddRange, float fDefNowX, float fDefSizeX)
	{
		if (fAtkBeforeX.IsNearlyEqual(-1f))
		{
			fAtkBeforeX = fAtkNowX;
		}
		float num = 0f;
		float num2 = 0f;
		if (fAtkBeforeX <= fAtkNowX)
		{
			num = fAtkBeforeX;
			num2 = fAtkNowX;
		}
		else
		{
			num = fAtkNowX;
			num2 = fAtkBeforeX;
		}
		if (bAtkRight)
		{
			if (cNKMEventAttack.m_fRangeMax + fAddRange > 0f)
			{
				num2 += fAtkSizeX * 0.5f;
			}
			if (cNKMEventAttack.m_fRangeMin < 0f)
			{
				num -= fAtkSizeX * 0.5f;
			}
			num += cNKMEventAttack.m_fRangeMin;
			num2 += cNKMEventAttack.m_fRangeMax + fAddRange;
		}
		else
		{
			if (cNKMEventAttack.m_fRangeMax + fAddRange > 0f)
			{
				num -= fAtkSizeX * 0.5f;
			}
			if (cNKMEventAttack.m_fRangeMin < 0f)
			{
				num2 += fAtkSizeX * 0.5f;
			}
			num -= cNKMEventAttack.m_fRangeMax + fAddRange;
			num2 -= cNKMEventAttack.m_fRangeMin;
		}
		if (fDefNowX >= num && fDefNowX <= num2)
		{
			return true;
		}
		float num3 = num - fDefNowX;
		if (num3 >= 0f && num3 < fDefSizeX * 0.5f)
		{
			return true;
		}
		float num4 = fDefNowX - num2;
		if (num4 >= 0f && num4 < fDefSizeX * 0.5f)
		{
			return true;
		}
		return false;
	}

	protected bool HitCheck(NKMDamageInst cNKMDamageInst, NKMEventAttack cNKMEventAttack, NKMUnit pAttacker, NKMUnit pDefender, float fDefenderDamageReduce, bool bSplashHit)
	{
		bool flag = true;
		if (pAttacker == null || pDefender == null)
		{
			return false;
		}
		if (!pDefender.WillInteractWithGameUnits())
		{
			return false;
		}
		if (!pDefender.CheckEventCondition(cNKMEventAttack.m_ConditionTarget, pAttacker))
		{
			return false;
		}
		if (pDefender.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			return false;
		}
		switch (cNKMEventAttack.m_NKM_DAMAGE_TARGET_TYPE)
		{
		case NKM_DAMAGE_TARGET_TYPE.NDTT_ENEMY:
			if (!IsEnemy(pAttacker.GetUnitDataGame().m_NKM_TEAM_TYPE, pDefender.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				return false;
			}
			break;
		case NKM_DAMAGE_TARGET_TYPE.NDTT_MY_TEAM:
			if (IsEnemy(pAttacker.GetUnitDataGame().m_NKM_TEAM_TYPE, pDefender.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				return false;
			}
			break;
		case NKM_DAMAGE_TARGET_TYPE.NDTT_MY_TEAM_NOT_SELF:
			if (IsEnemy(pAttacker.GetUnitDataGame().m_NKM_TEAM_TYPE, pDefender.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				return false;
			}
			if (pAttacker == pDefender)
			{
				return false;
			}
			break;
		case NKM_DAMAGE_TARGET_TYPE.NDTT_ALL_NOT_SELF:
			if (pAttacker == pDefender)
			{
				return false;
			}
			break;
		case NKM_DAMAGE_TARGET_TYPE.NDTT_SELF_ONLY:
			if (pAttacker != pDefender)
			{
				return false;
			}
			break;
		}
		for (int i = 0; i < cNKMDamageInst.m_listHitUnit.Count; i++)
		{
			if (cNKMDamageInst.m_listHitUnit[i] == pDefender.GetUnitDataGame().m_GameUnitUID)
			{
				return false;
			}
		}
		NKMUnitTempletBase unitTempletBase = pDefender.GetUnitTemplet().m_UnitTempletBase;
		if (!unitTempletBase.IsAllowUnitStyleType(cNKMEventAttack.m_listAllowStyle, cNKMEventAttack.m_listIgnoreStyle))
		{
			return false;
		}
		if (!unitTempletBase.IsAllowUnitRoleType(cNKMEventAttack.m_listAllowRole, cNKMEventAttack.m_listIgnoreRole))
		{
			return false;
		}
		int respawnCost = GetRespawnCost(pDefender.GetUnitTemplet().m_StatTemplet, bLeader: false, pDefender.GetUnitDataGame().m_NKM_TEAM_TYPE);
		if (cNKMEventAttack.m_TargetCostLess != -1 && cNKMEventAttack.m_TargetCostLess <= respawnCost)
		{
			return false;
		}
		if (cNKMEventAttack.m_TargetCostOver != -1 && cNKMEventAttack.m_TargetCostOver >= respawnCost)
		{
			return false;
		}
		bool flag2 = false;
		if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			flag2 = true;
		}
		if (pDefender.IsBoss())
		{
			flag2 = true;
		}
		if (!flag2)
		{
			if (!cNKMEventAttack.m_bHitAir && pDefender.IsAirUnit())
			{
				return false;
			}
			if (!cNKMEventAttack.m_bHitLand && !pDefender.IsAirUnit())
			{
				return false;
			}
		}
		if (cNKMEventAttack.m_bHitSummonOnly && !pDefender.IsSummonUnit())
		{
			return false;
		}
		if (!cNKMEventAttack.m_bHitAwakenUnit && unitTempletBase.m_bAwaken)
		{
			return false;
		}
		if (!cNKMEventAttack.m_bHitNormalUnit && !unitTempletBase.m_bAwaken)
		{
			return false;
		}
		if (!cNKMEventAttack.m_bHitBossUnit && flag2)
		{
			return false;
		}
		bool flag3 = false;
		if (cNKMDamageInst.m_EventAttack.m_AttackUnitCount + pAttacker.GetUnitFrameData().m_AddAttackUnitCount <= cNKMDamageInst.m_AttackCount)
		{
			flag3 = true;
		}
		if (!CollisionCheck(pAttacker.GetUnitFrameData().m_PosXBefore, pAttacker.GetUnitSyncData().m_PosX, pAttacker.GetUnitTemplet().m_UnitSizeX, pAttacker.GetUnitSyncData().m_bRight, cNKMEventAttack, pAttacker.GetUnitFrameData().m_fAddAttackRange, pDefender.GetUnitSyncData().m_PosX, pDefender.GetUnitTemplet().m_UnitSizeX))
		{
			return false;
		}
		cNKMDamageInst.m_DefenderUID = pDefender.GetUnitDataGame().m_GameUnitUID;
		cNKMDamageInst.m_ReActResult = cNKMDamageInst.m_Templet.m_ReActType;
		cNKMDamageInst.m_AttackerPosX = pAttacker.GetUnitSyncData().m_PosX;
		cNKMDamageInst.m_AttackerPosZ = pAttacker.GetUnitSyncData().m_PosZ;
		cNKMDamageInst.m_AttackerPosJumpY = pAttacker.GetUnitSyncData().m_JumpYPos;
		cNKMDamageInst.m_bAttackerRight = pAttacker.GetUnitSyncData().m_bRight;
		cNKMDamageInst.m_bAttackerAwaken = pAttacker.GetUnitTemplet().m_UnitTempletBase.m_bAwaken;
		cNKMDamageInst.m_AttackerAddAttackUnitCount = pAttacker.GetUnitFrameData().m_AddAttackUnitCount;
		cNKMDamageInst.m_bEvade = NKMUnitStatManager.GetEvade(pAttacker, pDefender, bBuffDamage: false, pDefender.GetHPRate(), cNKMEventAttack);
		pDefender.DamageReact(cNKMDamageInst, bBuffDamage: false);
		if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_NO)
		{
			return false;
		}
		flag = true;
		bool flag4 = true;
		for (int j = 0; j < cNKMDamageInst.m_listHitUnit.Count; j++)
		{
			if (cNKMDamageInst.m_listHitUnit[j] == cNKMDamageInst.m_DefenderUID)
			{
				flag4 = false;
				break;
			}
		}
		if (flag4)
		{
			cNKMDamageInst.m_listHitUnit.Add(cNKMDamageInst.m_DefenderUID);
			cNKMDamageInst.m_AttackCount++;
			cNKMDamageInst.m_AttackCount += (int)(pDefender.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_ATTACK_COUNT_REDUCE) + 0.1f);
		}
		pAttacker.AttackResult(cNKMDamageInst);
		if (cNKMDamageInst.m_Templet.m_ReAttackCount > 1)
		{
			cNKMDamageInst.m_bReAttackCountOver = flag3;
			cNKMDamageInst.m_ReAttackCount = 1;
			cNKMDamageInst.m_fReAttackGap = cNKMDamageInst.m_Templet.m_fReAttackGap;
			NKMDamageInst nKMDamageInst = (NKMDamageInst)m_ObjectPool.OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMDamageInst);
			nKMDamageInst.DeepCopyFromSource(cNKMDamageInst);
			m_linklistReAttack.AddLast(nKMDamageInst);
		}
		if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_INVINCIBLE)
		{
			return flag;
		}
		if (IsServerGame())
		{
			float dist = pAttacker.GetDist(pDefender, bUseUnitSize: true);
			NKM_DAMAGE_RESULT_TYPE eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL;
			bool bInstaKill = false;
			float num;
			if (GetGameData().m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PRACTICE && GetGameRuntimeData().m_bPracticeFixedDamage)
			{
				num = NKMUnitStatManager.GetAttackFactorDamage(cNKMDamageInst.m_Templet.m_DamageTempletBase, cNKMDamageInst.m_AttackerUnitSkillTemplet, flag2);
			}
			else if (!flag3 && pDefender.WillInstaKilled(cNKMDamageInst.m_Templet))
			{
				num = pDefender.GetNowHP() * 2f;
				pDefender.AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_INSTA_KILL);
				bInstaKill = true;
			}
			else
			{
				num = NKMUnitStatManager.GetFinalDamage(IsPVP(bUseDevOption: true), pAttacker.GetUnitFrameData().m_StatData, pDefender.GetUnitFrameData().m_StatData, pAttacker.GetUnitData(), pAttacker, pDefender, cNKMDamageInst.m_Templet, cNKMDamageInst.m_AttackerUnitSkillTemplet, flag3, bBuffDamage: false, cNKMDamageInst.m_bEvade, out eNKM_DAMAGE_RESULT_TYPE, fDefenderDamageReduce, dist, flag2, pAttacker.GetHPRate(), cNKMEventAttack.m_bTrueDamage, bSplashHit, cNKMEventAttack.m_bForceCritical, cNKMEventAttack.m_bNoCritical);
			}
			if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE)
			{
				num *= 0.1f;
			}
			num = pDefender.GetModifiedDMGAfterEventDEF(pAttacker.GetUnitSyncData().m_PosX, num);
			if (!cNKMDamageInst.m_Templet.IsZeroDamage())
			{
				pDefender.SetHitFeedBack();
				pDefender.OnReactionEvent(NKMUnitReaction.ReactionEventType.HIT, pAttacker, 1);
				pAttacker.OnReactionEvent(NKMUnitReaction.ReactionEventType.ATTACK, pDefender, 1);
				if (eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_CRITICAL)
				{
					pAttacker.SetHitCriticalFeedBack();
					pAttacker.OnReactionEvent(NKMUnitReaction.ReactionEventType.ATTACK_CRITICAL, pDefender, 1);
					pDefender.OnReactionEvent(NKMUnitReaction.ReactionEventType.HIT_CRITICAL, pAttacker, 1);
				}
				if (cNKMDamageInst.m_bEvade)
				{
					pDefender.SetHitEvadeFeedBack();
					pAttacker.OnReactionEvent(NKMUnitReaction.ReactionEventType.ATTACK_MISS, pDefender, 1);
					pDefender.OnReactionEvent(NKMUnitReaction.ReactionEventType.HIT_EVADE, pAttacker, 1);
				}
			}
			if (cNKMDamageInst.m_ReActResult != NKM_REACT_TYPE.NRT_REVENGE && pDefender.WillKilledByDamage(num))
			{
				pAttacker.Kill(cNKMDamageInst.m_Templet.m_NKMKillFeedBack, pDefender.GetUnitDataGame().m_GameUnitUID);
			}
			pDefender.AddDamage(flag3, num, eNKM_DAMAGE_RESULT_TYPE, pAttacker.GetUnitDataGame().m_GameUnitUID, pAttacker.GetUnitDataGame().m_NKM_TEAM_TYPE, bPushSyncData: false, bNoRedirect: false, bInstaKill);
			if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE)
			{
				if (pDefender.GetUnitFrameData().m_fDamageThisFrame >= pDefender.GetUnitSyncData().GetHP())
				{
					pDefender.GetUnitFrameData().m_fDamageThisFrame = pDefender.GetUnitSyncData().GetHP() - 1f;
				}
			}
			else
			{
				if (cNKMDamageInst.m_Templet.CanApplyStun(pDefender.GetUnitTemplet().m_UnitTempletBase))
				{
					pDefender.ApplyStatusTime(NKM_UNIT_STATUS_EFFECT.NUSE_STUN, cNKMDamageInst.m_Templet.m_fStunTime, pAttacker);
				}
				if (cNKMDamageInst.m_Templet.CanApplyCooltimeDamage(pDefender.GetUnitTemplet().m_UnitTempletBase))
				{
					pDefender.AddDamage(flag3, cNKMDamageInst.m_Templet.m_fCoolTimeDamage, NKM_DAMAGE_RESULT_TYPE.NDRT_COOL_TIME, pAttacker.GetUnitDataGame().m_GameUnitUID, pAttacker.GetUnitDataGame().m_NKM_TEAM_TYPE);
				}
				if (cNKMDamageInst.m_Templet.CanApplyStatus(pDefender.GetUnitTemplet().m_UnitTempletBase))
				{
					pDefender.ApplyStatusTime(cNKMDamageInst.m_Templet.m_ApplyStatusEffect, cNKMDamageInst.m_Templet.m_fApplyStatusTime, pAttacker);
				}
				if (cNKMEventAttack.m_fGetAgroTime > 0f)
				{
					pAttacker.SetAgro(pDefender, cNKMEventAttack.m_fGetAgroTime);
				}
			}
			ProcessHitBuff(cNKMDamageInst, pAttacker, pDefender);
			ProcessHitTrigger(cNKMDamageInst, pAttacker, pDefender);
		}
		ProcessHitEvent(cNKMDamageInst, pAttacker, pDefender, bFromDE: false);
		return flag;
	}

	protected bool HitCheck(NKMDamageInst cNKMDamageInst, NKMEventAttack cNKMEventAttack, NKMDamageEffect pAttackerEffect, NKMUnit pAttackerUnit, NKMUnit pDefender, float fDefenderDamageReduce, bool bSplashHit)
	{
		bool flag = true;
		if (pAttackerEffect == null || pDefender == null)
		{
			return false;
		}
		if (pAttackerEffect.IsEnd())
		{
			return false;
		}
		if (pDefender.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			return false;
		}
		if (!pDefender.WillInteractWithGameUnits())
		{
			return false;
		}
		switch (cNKMEventAttack.m_NKM_DAMAGE_TARGET_TYPE)
		{
		case NKM_DAMAGE_TARGET_TYPE.NDTT_ENEMY:
			if (!IsEnemy(pAttackerEffect.GetDEData().m_NKM_TEAM_TYPE, pDefender.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				return false;
			}
			break;
		case NKM_DAMAGE_TARGET_TYPE.NDTT_MY_TEAM:
			if (IsEnemy(pAttackerEffect.GetDEData().m_NKM_TEAM_TYPE, pDefender.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				return false;
			}
			break;
		case NKM_DAMAGE_TARGET_TYPE.NDTT_MY_TEAM_NOT_SELF:
			if (IsEnemy(pAttackerEffect.GetDEData().m_NKM_TEAM_TYPE, pDefender.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				return false;
			}
			if (pAttackerEffect.GetDEData().m_MasterGameUnitUID == pDefender.GetUnitDataGame().m_GameUnitUID)
			{
				return false;
			}
			break;
		case NKM_DAMAGE_TARGET_TYPE.NDTT_ALL_NOT_SELF:
			if (pAttackerEffect.GetDEData().m_MasterGameUnitUID == pDefender.GetUnitDataGame().m_GameUnitUID)
			{
				return false;
			}
			break;
		case NKM_DAMAGE_TARGET_TYPE.NDTT_SELF_ONLY:
			if (pAttackerEffect.GetDEData().m_MasterGameUnitUID != pDefender.GetUnitDataGame().m_GameUnitUID)
			{
				return false;
			}
			break;
		}
		if (pDefender.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || pDefender.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			return false;
		}
		for (int i = 0; i < cNKMDamageInst.m_listHitUnit.Count; i++)
		{
			if (cNKMDamageInst.m_listHitUnit[i] == pDefender.GetUnitDataGame().m_GameUnitUID)
			{
				return false;
			}
		}
		NKMUnitTempletBase unitTempletBase = pDefender.GetUnitTemplet().m_UnitTempletBase;
		if (!unitTempletBase.IsAllowUnitStyleType(cNKMEventAttack.m_listAllowStyle, cNKMEventAttack.m_listIgnoreStyle))
		{
			return false;
		}
		if (!unitTempletBase.IsAllowUnitRoleType(cNKMEventAttack.m_listAllowRole, cNKMEventAttack.m_listIgnoreRole))
		{
			return false;
		}
		int respawnCost = GetRespawnCost(pDefender.GetUnitTemplet().m_StatTemplet, bLeader: false, pDefender.GetUnitDataGame().m_NKM_TEAM_TYPE);
		if (cNKMEventAttack.m_TargetCostLess != -1 && cNKMEventAttack.m_TargetCostLess <= respawnCost)
		{
			return false;
		}
		if (cNKMEventAttack.m_TargetCostOver != -1 && cNKMEventAttack.m_TargetCostOver >= respawnCost)
		{
			return false;
		}
		bool flag2 = false;
		if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			flag2 = true;
		}
		if (pDefender.IsBoss())
		{
			flag2 = true;
		}
		if (!flag2)
		{
			if (!cNKMEventAttack.m_bHitAir && pDefender.IsAirUnit())
			{
				return false;
			}
			if (!cNKMEventAttack.m_bHitLand && !pDefender.IsAirUnit())
			{
				return false;
			}
		}
		if (cNKMEventAttack.m_bHitSummonOnly && !pDefender.IsSummonUnit())
		{
			return false;
		}
		if (!cNKMEventAttack.m_bHitAwakenUnit && unitTempletBase.m_bAwaken)
		{
			return false;
		}
		if (!cNKMEventAttack.m_bHitNormalUnit && !unitTempletBase.m_bAwaken)
		{
			return false;
		}
		if (!cNKMEventAttack.m_bHitBossUnit && flag2)
		{
			return false;
		}
		byte b = 0;
		b = (byte)((pAttackerEffect.GetMasterUnit() != null) ? pAttackerEffect.GetMasterUnit().GetUnitFrameData().m_AddAttackUnitCount : 0);
		bool flag3 = false;
		if (cNKMDamageInst.m_EventAttack.m_AttackUnitCount + b <= cNKMDamageInst.m_AttackCount)
		{
			flag3 = true;
		}
		if (!pDefender.CheckEventCondition(cNKMEventAttack.m_ConditionTarget, pAttackerUnit))
		{
			return false;
		}
		if (!CollisionCheck(pAttackerEffect.GetDEData().m_PosXBefore, pAttackerEffect.GetDEData().m_PosX, pAttackerEffect.GetTemplet().m_fEffectSize, pAttackerEffect.GetDEData().m_bRight, cNKMEventAttack, 0f, pDefender.GetUnitSyncData().m_PosX, pDefender.GetUnitTemplet().m_UnitSizeX))
		{
			return false;
		}
		cNKMDamageInst.m_DefenderUID = pDefender.GetUnitDataGame().m_GameUnitUID;
		cNKMDamageInst.m_ReActResult = cNKMDamageInst.m_Templet.m_ReActType;
		if (!pAttackerEffect.GetTemplet().m_bDamageSpeedDependMaster || pAttackerUnit == null)
		{
			cNKMDamageInst.m_AttackerPosX = pAttackerEffect.GetDEData().m_PosX;
			cNKMDamageInst.m_AttackerPosZ = pAttackerEffect.GetDEData().m_PosZ;
			cNKMDamageInst.m_AttackerPosJumpY = pAttackerEffect.GetDEData().m_JumpYPos;
			cNKMDamageInst.m_bAttackerRight = pAttackerEffect.GetDEData().m_bRight;
			if (pAttackerEffect.GetMasterUnit() != null)
			{
				cNKMDamageInst.m_bAttackerAwaken = pAttackerEffect.GetMasterUnit().GetUnitTemplet().m_UnitTempletBase.m_bAwaken;
				cNKMDamageInst.m_AttackerAddAttackUnitCount = pAttackerEffect.GetMasterUnit().GetUnitFrameData().m_AddAttackUnitCount;
			}
			else
			{
				cNKMDamageInst.m_AttackerAddAttackUnitCount = 0;
			}
		}
		else
		{
			cNKMDamageInst.m_AttackerPosX = pAttackerUnit.GetUnitSyncData().m_PosX;
			cNKMDamageInst.m_AttackerPosZ = pAttackerUnit.GetUnitSyncData().m_PosZ;
			cNKMDamageInst.m_AttackerPosJumpY = pAttackerUnit.GetUnitSyncData().m_JumpYPos;
			cNKMDamageInst.m_bAttackerRight = pAttackerUnit.GetUnitSyncData().m_bRight;
			cNKMDamageInst.m_bAttackerAwaken = pAttackerUnit.GetUnitTemplet().m_UnitTempletBase.m_bAwaken;
			cNKMDamageInst.m_bEvade = NKMUnitStatManager.GetEvade(pAttackerUnit, pDefender, bBuffDamage: false, pDefender.GetHPRate(), cNKMEventAttack);
			cNKMDamageInst.m_AttackerAddAttackUnitCount = pAttackerUnit.GetUnitFrameData().m_AddAttackUnitCount;
		}
		if (pAttackerUnit == null)
		{
			cNKMDamageInst.m_bEvade = false;
		}
		else
		{
			cNKMDamageInst.m_bEvade = NKMUnitStatManager.GetEvade(pAttackerUnit, pDefender, bBuffDamage: false, pDefender.GetHPRate(), cNKMEventAttack);
		}
		pDefender.DamageReact(cNKMDamageInst, bBuffDamage: false);
		if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_NO)
		{
			return false;
		}
		flag = true;
		bool flag4 = true;
		for (int j = 0; j < cNKMDamageInst.m_listHitUnit.Count; j++)
		{
			if (cNKMDamageInst.m_listHitUnit[j] == cNKMDamageInst.m_DefenderUID)
			{
				flag4 = false;
				break;
			}
		}
		if (flag4)
		{
			cNKMDamageInst.m_listHitUnit.Add(cNKMDamageInst.m_DefenderUID);
			cNKMDamageInst.m_AttackCount++;
			cNKMDamageInst.m_AttackCount += (int)(pDefender.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_ATTACK_COUNT_REDUCE) + 0.1f);
		}
		pAttackerEffect.AttackResult(cNKMDamageInst, pDefender);
		if (cNKMDamageInst.m_Templet.m_ReAttackCount > 1)
		{
			cNKMDamageInst.m_bReAttackCountOver = flag3;
			cNKMDamageInst.m_ReAttackCount = 1;
			cNKMDamageInst.m_fReAttackGap = cNKMDamageInst.m_Templet.m_fReAttackGap;
			NKMDamageInst nKMDamageInst = (NKMDamageInst)m_ObjectPool.OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMDamageInst);
			nKMDamageInst.DeepCopyFromSource(cNKMDamageInst);
			m_linklistReAttack.AddLast(nKMDamageInst);
		}
		if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_INVINCIBLE)
		{
			return flag;
		}
		if (IsServerGame())
		{
			float fAtkHPRate = 1f;
			float dist = pDefender.GetDist(pAttackerUnit, bUseUnitSize: true);
			if (pAttackerUnit != null)
			{
				fAtkHPRate = pAttackerUnit.GetHPRate();
			}
			NKM_DAMAGE_RESULT_TYPE eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL;
			bool bInstaKill = false;
			float num;
			if (GetGameData().m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PRACTICE && GetGameRuntimeData().m_bPracticeFixedDamage)
			{
				num = NKMUnitStatManager.GetAttackFactorDamage(cNKMDamageInst.m_Templet.m_DamageTempletBase, cNKMDamageInst.m_AttackerUnitSkillTemplet, flag2);
			}
			else if (!flag3 && pDefender.WillInstaKilled(cNKMDamageInst.m_Templet))
			{
				num = pDefender.GetNowHP() * 2f;
				pDefender.AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_INSTA_KILL);
				bInstaKill = true;
			}
			else
			{
				num = NKMUnitStatManager.GetFinalDamage(IsPVP(bUseDevOption: true), pAttackerEffect.GetDEData().m_StatData, pDefender.GetUnitFrameData().m_StatData, pAttackerEffect.GetDEData().m_UnitData, pAttackerEffect.GetMasterUnit(), pDefender, cNKMDamageInst.m_Templet, cNKMDamageInst.m_AttackerUnitSkillTemplet, flag3, bBuffDamage: false, cNKMDamageInst.m_bEvade, out eNKM_DAMAGE_RESULT_TYPE, fDefenderDamageReduce, dist, flag2, fAtkHPRate, cNKMEventAttack.m_bTrueDamage, bSplashHit, cNKMEventAttack.m_bForceCritical, cNKMEventAttack.m_bNoCritical);
			}
			if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE)
			{
				num *= 0.1f;
			}
			num = ((!pAttackerEffect.GetTemplet().m_bDamageSpeedDependMaster || pAttackerUnit == null) ? pDefender.GetModifiedDMGAfterEventDEF(pAttackerEffect.GetDEData().m_PosX, num) : pDefender.GetModifiedDMGAfterEventDEF(pAttackerUnit.GetUnitSyncData().m_PosX, num));
			if (!cNKMDamageInst.m_Templet.IsZeroDamage())
			{
				pDefender.SetHitFeedBack();
				pDefender.OnReactionEvent(NKMUnitReaction.ReactionEventType.HIT, pAttackerUnit, 1);
				pAttackerUnit.OnReactionEvent(NKMUnitReaction.ReactionEventType.ATTACK, pDefender, 1);
				if (eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_CRITICAL && pAttackerUnit != null)
				{
					pAttackerUnit.SetHitCriticalFeedBack();
					pAttackerUnit.OnReactionEvent(NKMUnitReaction.ReactionEventType.ATTACK_CRITICAL, pDefender, 1);
					pDefender.OnReactionEvent(NKMUnitReaction.ReactionEventType.HIT_CRITICAL, pAttackerUnit, 1);
				}
				if (cNKMDamageInst.m_bEvade)
				{
					pDefender.SetHitEvadeFeedBack();
					pAttackerUnit.OnReactionEvent(NKMUnitReaction.ReactionEventType.ATTACK_MISS, pDefender, 1);
					pDefender.OnReactionEvent(NKMUnitReaction.ReactionEventType.HIT_EVADE, pAttackerUnit, 1);
				}
			}
			if (cNKMDamageInst.m_ReActResult != NKM_REACT_TYPE.NRT_REVENGE && pDefender.WillKilledByDamage(num))
			{
				pAttackerUnit?.Kill(cNKMDamageInst.m_Templet.m_NKMKillFeedBack, pDefender.GetUnitDataGame().m_GameUnitUID);
			}
			pDefender.AddDamage(flag3, num, eNKM_DAMAGE_RESULT_TYPE, pAttackerEffect.GetDEData().m_MasterGameUnitUID, pAttackerEffect.GetDEData().m_NKM_TEAM_TYPE, bPushSyncData: false, bNoRedirect: false, bInstaKill);
			if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE)
			{
				if (pDefender.GetUnitFrameData().m_fDamageThisFrame >= pDefender.GetUnitSyncData().GetHP())
				{
					pDefender.GetUnitFrameData().m_fDamageThisFrame = pDefender.GetUnitSyncData().GetHP() - 1f;
				}
			}
			else
			{
				if (cNKMDamageInst.m_Templet.CanApplyStun(pDefender.GetUnitTemplet().m_UnitTempletBase))
				{
					pDefender.ApplyStatusTime(NKM_UNIT_STATUS_EFFECT.NUSE_STUN, cNKMDamageInst.m_Templet.m_fStunTime, pAttackerUnit);
				}
				if (cNKMDamageInst.m_Templet.CanApplyCooltimeDamage(pDefender.GetUnitTemplet().m_UnitTempletBase))
				{
					pDefender.AddDamage(flag3, cNKMDamageInst.m_Templet.m_fCoolTimeDamage, NKM_DAMAGE_RESULT_TYPE.NDRT_COOL_TIME, pAttackerEffect.GetDEData().m_MasterGameUnitUID, pAttackerEffect.GetDEData().m_NKM_TEAM_TYPE);
				}
				if (cNKMDamageInst.m_Templet.CanApplyStatus(pDefender.GetUnitTemplet().m_UnitTempletBase))
				{
					pDefender.ApplyStatusTime(cNKMDamageInst.m_Templet.m_ApplyStatusEffect, cNKMDamageInst.m_Templet.m_fApplyStatusTime, pAttackerUnit);
				}
				if (pAttackerUnit != null && cNKMEventAttack.m_fGetAgroTime > 0f)
				{
					pAttackerUnit.SetAgro(pDefender, cNKMEventAttack.m_fGetAgroTime);
				}
			}
			ProcessHitBuff(cNKMDamageInst, pAttackerUnit, pDefender);
			ProcessHitTrigger(cNKMDamageInst, pAttackerUnit, pDefender);
		}
		ProcessHitEvent(cNKMDamageInst, pAttackerUnit, pDefender, bFromDE: true);
		return flag;
	}

	public bool ProcessDamageTemplet(NKMDamageTemplet cNKMDamageTemplet, NKMUnit pAttacker, NKMUnit pDefender, bool bUseAttackerStat = true, bool buffDamage = false, NKMUnitSkillTemplet attackerSkillTemplet = null, NKMDamageAttribute damageAttribute = null)
	{
		if (cNKMDamageTemplet == null)
		{
			return false;
		}
		if (pAttacker == null)
		{
			return false;
		}
		if (pDefender == null)
		{
			return false;
		}
		NKMDamageInst nKMDamageInst = (NKMDamageInst)m_ObjectPool.OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMDamageInst);
		nKMDamageInst.m_Templet = cNKMDamageTemplet;
		nKMDamageInst.m_DefenderUID = pDefender.GetUnitDataGame().m_GameUnitUID;
		nKMDamageInst.m_ReActResult = nKMDamageInst.m_Templet.m_ReActType;
		nKMDamageInst.m_AttackerPosX = pAttacker.GetUnitSyncData().m_PosX;
		nKMDamageInst.m_AttackerPosZ = pAttacker.GetUnitSyncData().m_PosZ;
		nKMDamageInst.m_AttackerPosJumpY = pAttacker.GetUnitSyncData().m_JumpYPos;
		nKMDamageInst.m_bAttackerRight = pAttacker.GetUnitSyncData().m_bRight;
		nKMDamageInst.m_bAttackerAwaken = pAttacker.GetUnitTemplet().m_UnitTempletBase.m_bAwaken;
		nKMDamageInst.m_AttackerAddAttackUnitCount = pAttacker.GetUnitFrameData().m_AddAttackUnitCount;
		nKMDamageInst.m_AttackerUnitSkillTemplet = attackerSkillTemplet;
		nKMDamageInst.m_bEvade = false;
		pDefender.DamageReact(nKMDamageInst, bBuffDamage: false);
		if (nKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_NO)
		{
			m_ObjectPool.CloseObj(nKMDamageInst);
			return false;
		}
		bool flag = true;
		for (int i = 0; i < nKMDamageInst.m_listHitUnit.Count; i++)
		{
			if (nKMDamageInst.m_listHitUnit[i] == nKMDamageInst.m_DefenderUID)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			nKMDamageInst.m_listHitUnit.Add(nKMDamageInst.m_DefenderUID);
			nKMDamageInst.m_AttackCount++;
			nKMDamageInst.m_AttackCount += (int)(pDefender.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_ATTACK_COUNT_REDUCE) + 0.1f);
		}
		pAttacker.AttackResult(nKMDamageInst);
		if (nKMDamageInst.m_Templet.m_ReAttackCount > 1)
		{
			nKMDamageInst.m_bReAttackCountOver = false;
			nKMDamageInst.m_ReAttackCount = 1;
			nKMDamageInst.m_fReAttackGap = nKMDamageInst.m_Templet.m_fReAttackGap;
			NKMDamageInst nKMDamageInst2 = (NKMDamageInst)m_ObjectPool.OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMDamageInst);
			nKMDamageInst2.DeepCopyFromSource(nKMDamageInst);
			m_linklistReAttack.AddLast(nKMDamageInst2);
		}
		if (nKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_INVINCIBLE && !pDefender.IsAlly(pAttacker))
		{
			m_ObjectPool.CloseObj(nKMDamageInst);
			return true;
		}
		if (IsServerGame())
		{
			float dist = pDefender.GetDist(pAttacker, bUseUnitSize: true);
			NKM_DAMAGE_RESULT_TYPE eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL;
			bool bInstaKill = false;
			bool bBoss = false;
			if (pDefender.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				bBoss = true;
			}
			if (pDefender.IsBoss())
			{
				bBoss = true;
			}
			float num;
			if (GetGameData().m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PRACTICE && GetGameRuntimeData().m_bPracticeFixedDamage)
			{
				num = NKMUnitStatManager.GetAttackFactorDamage(nKMDamageInst.m_Templet.m_DamageTempletBase, nKMDamageInst.m_AttackerUnitSkillTemplet, bBoss);
			}
			else if (pDefender.WillInstaKilled(nKMDamageInst.m_Templet))
			{
				num = pDefender.GetNowHP() * 2f;
				pDefender.AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_INSTA_KILL);
				bInstaKill = true;
			}
			else
			{
				num = NKMUnitStatManager.GetFinalDamage(IsPVP(bUseDevOption: true), pAttacker.GetUnitFrameData().m_StatData, pDefender.GetUnitFrameData().m_StatData, pAttacker.GetUnitData(), pAttacker, pDefender, nKMDamageInst.m_Templet, attackerSkillTemplet, bAttackCountOver: false, buffDamage, nKMDamageInst.m_bEvade, out eNKM_DAMAGE_RESULT_TYPE, 0f, dist, bBoss, pAttacker.GetHPRate(), bSplashHit: false, damageAttribute);
			}
			if (nKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE)
			{
				num *= 0.1f;
			}
			num = pDefender.GetModifiedDMGAfterEventDEF(pAttacker.GetUnitSyncData().m_PosX, num);
			if (!nKMDamageInst.m_Templet.IsZeroDamage())
			{
				pDefender.SetHitFeedBack();
				pDefender.OnReactionEvent(NKMUnitReaction.ReactionEventType.HIT, pAttacker, 1);
				pAttacker.OnReactionEvent(NKMUnitReaction.ReactionEventType.ATTACK, pDefender, 1);
				if (eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_CRITICAL)
				{
					pAttacker.SetHitCriticalFeedBack();
					pAttacker.OnReactionEvent(NKMUnitReaction.ReactionEventType.ATTACK_CRITICAL, pDefender, 1);
					pDefender.OnReactionEvent(NKMUnitReaction.ReactionEventType.HIT_CRITICAL, pAttacker, 1);
				}
				if (nKMDamageInst.m_bEvade)
				{
					pDefender.SetHitEvadeFeedBack();
					pAttacker.OnReactionEvent(NKMUnitReaction.ReactionEventType.ATTACK_MISS, pDefender, 1);
					pDefender.OnReactionEvent(NKMUnitReaction.ReactionEventType.HIT_EVADE, pAttacker, 1);
				}
			}
			if (nKMDamageInst.m_ReActResult != NKM_REACT_TYPE.NRT_REVENGE && pDefender.WillKilledByDamage(num))
			{
				pAttacker.Kill(nKMDamageInst.m_Templet.m_NKMKillFeedBack, pDefender.GetUnitDataGame().m_GameUnitUID);
			}
			pDefender.AddDamage(bAttackCountOver: false, num, eNKM_DAMAGE_RESULT_TYPE, pAttacker.GetUnitDataGame().m_GameUnitUID, pAttacker.GetUnitDataGame().m_NKM_TEAM_TYPE, bPushSyncData: false, bNoRedirect: false, bInstaKill);
			if (nKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE)
			{
				if (pDefender.GetUnitFrameData().m_fDamageThisFrame >= pDefender.GetUnitSyncData().GetHP())
				{
					pDefender.GetUnitFrameData().m_fDamageThisFrame = pDefender.GetUnitSyncData().GetHP() - 1f;
				}
			}
			else
			{
				if (nKMDamageInst.m_Templet.CanApplyStun(pDefender.GetUnitTemplet().m_UnitTempletBase))
				{
					pDefender.ApplyStatusTime(NKM_UNIT_STATUS_EFFECT.NUSE_STUN, nKMDamageInst.m_Templet.m_fStunTime, pAttacker);
				}
				if (nKMDamageInst.m_Templet.CanApplyCooltimeDamage(pDefender.GetUnitTemplet().m_UnitTempletBase))
				{
					pDefender.AddDamage(bAttackCountOver: false, nKMDamageInst.m_Templet.m_fCoolTimeDamage, NKM_DAMAGE_RESULT_TYPE.NDRT_COOL_TIME, pAttacker.GetUnitDataGame().m_GameUnitUID, pAttacker.GetUnitDataGame().m_NKM_TEAM_TYPE);
				}
				if (nKMDamageInst.m_Templet.CanApplyStatus(pDefender.GetUnitTemplet().m_UnitTempletBase))
				{
					pDefender.ApplyStatusTime(nKMDamageInst.m_Templet.m_ApplyStatusEffect, nKMDamageInst.m_Templet.m_fApplyStatusTime, pAttacker);
				}
			}
			ProcessHitBuff(nKMDamageInst, pAttacker, pDefender);
			ProcessHitTrigger(nKMDamageInst, pAttacker, pDefender);
		}
		ProcessHitEvent(nKMDamageInst, pAttacker, pDefender, bFromDE: false);
		m_ObjectPool.CloseObj(nKMDamageInst);
		return true;
	}

	public void ProcessHitBuff(NKMDamageInst cNKMDamageInst, NKMUnit pAttackerUnit, NKMUnit pDefender)
	{
		if (pAttackerUnit == null || pDefender == null)
		{
			return;
		}
		NKMDamageTemplet templet = cNKMDamageInst.m_Templet;
		bool flag = cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE;
		if (!flag && templet.m_DeleteBuffCount > 0)
		{
			int num = templet.m_DeleteBuffCount;
			if (templet.m_bCanDispelStatus)
			{
				num -= pDefender.DispelStatusTime(bDebuff: false, num);
			}
			pDefender.DispelRandomBuff(num, bDispelDebuff: false);
			pDefender.AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_DISPEL);
		}
		if (cNKMDamageInst.m_Templet.m_DeleteConfuseBuff)
		{
			pDefender.DeleteStatusBuff(NKM_UNIT_STATUS_EFFECT.NUSE_CONFUSE, bForceRemove: false, bAffectOnly: true);
		}
		if (!string.IsNullOrEmpty(cNKMDamageInst.m_Templet.m_AttackerBuff))
		{
			ProcessAttackerBuff(cNKMDamageInst, pAttackerUnit, cNKMDamageInst.m_Templet.m_AttackerBuff, cNKMDamageInst.m_Templet.m_AttackerBuffStatBaseLevel, cNKMDamageInst.m_Templet.m_AttackerBuffStatAddLVBySkillLV, cNKMDamageInst.m_Templet.m_AttackerBuffTimeBaseLevel, cNKMDamageInst.m_Templet.m_AttackerBuffTimeAddLVBySkillLV, bRemove: false, 1);
		}
		for (int i = 0; i < cNKMDamageInst.m_Templet.m_listAttackerHitBuff.Count; i++)
		{
			if (pAttackerUnit.CheckEventCondition(cNKMDamageInst.m_Templet.m_listAttackerHitBuff[i].m_Condition))
			{
				ProcessAttackerBuff(cNKMDamageInst, pAttackerUnit, cNKMDamageInst.m_Templet.m_listAttackerHitBuff[i].m_HitBuff, cNKMDamageInst.m_Templet.m_listAttackerHitBuff[i].m_HitBuffStatBaseLevel, cNKMDamageInst.m_Templet.m_listAttackerHitBuff[i].m_HitBuffStatAddLVBySkillLV, cNKMDamageInst.m_Templet.m_listAttackerHitBuff[i].m_HitBuffTimeBaseLevel, cNKMDamageInst.m_Templet.m_listAttackerHitBuff[i].m_HitBuffTimeAddLVBySkillLV, cNKMDamageInst.m_Templet.m_listAttackerHitBuff[i].m_bRemove, cNKMDamageInst.m_Templet.m_listAttackerHitBuff[i].m_HitBuffOverlap);
			}
		}
		if (flag)
		{
			return;
		}
		if (!string.IsNullOrEmpty(cNKMDamageInst.m_Templet.m_DefenderBuff))
		{
			ProcessDefenderBuff(cNKMDamageInst, pAttackerUnit, pDefender, cNKMDamageInst.m_Templet.m_DefenderBuff, cNKMDamageInst.m_Templet.m_DefenderBuffStatBaseLevel, cNKMDamageInst.m_Templet.m_DefenderBuffStatAddLVBySkillLV, cNKMDamageInst.m_Templet.m_DefenderBuffTimeBaseLevel, cNKMDamageInst.m_Templet.m_DefenderBuffTimeAddLVBySkillLV, bRemove: false, 1);
		}
		for (int j = 0; j < cNKMDamageInst.m_Templet.m_listDefenderHitBuff.Count; j++)
		{
			if (pAttackerUnit.CheckEventCondition(cNKMDamageInst.m_Templet.m_listDefenderHitBuff[j].m_Condition))
			{
				ProcessDefenderBuff(cNKMDamageInst, pAttackerUnit, pDefender, cNKMDamageInst.m_Templet.m_listDefenderHitBuff[j].m_HitBuff, cNKMDamageInst.m_Templet.m_listDefenderHitBuff[j].m_HitBuffStatBaseLevel, cNKMDamageInst.m_Templet.m_listDefenderHitBuff[j].m_HitBuffStatAddLVBySkillLV, cNKMDamageInst.m_Templet.m_listDefenderHitBuff[j].m_HitBuffTimeBaseLevel, cNKMDamageInst.m_Templet.m_listDefenderHitBuff[j].m_HitBuffTimeAddLVBySkillLV, cNKMDamageInst.m_Templet.m_listDefenderHitBuff[j].m_bRemove, cNKMDamageInst.m_Templet.m_listDefenderHitBuff[j].m_HitBuffOverlap);
			}
		}
		if (!pAttackerUnit.WillInteractWithGameUnits() || cNKMDamageInst.m_Templet.IsZeroDamage())
		{
			return;
		}
		for (int k = 0; k < pDefender.GetUnitTemplet().m_listReflectionBuffData.Count; k++)
		{
			NKMStaticBuffData nKMStaticBuffData = pDefender.GetUnitTemplet().m_listReflectionBuffData[k];
			if (pDefender.CheckEventCondition(nKMStaticBuffData.m_Condition))
			{
				pAttackerUnit.AddBuffByStrID(nKMStaticBuffData.m_BuffStrID, nKMStaticBuffData.m_BuffStatLevel, nKMStaticBuffData.m_BuffTimeLevel, pDefender.GetUnitDataGame().m_GameUnitUID, bUseMasterStat: true, bRangeSon: false);
			}
		}
		if (pDefender.GetUnitStateNow() == null)
		{
			return;
		}
		for (int l = 0; l < pDefender.GetUnitStateNow().m_listNKMEventBuff.Count; l++)
		{
			NKMEventBuff nKMEventBuff = pDefender.GetUnitStateNow().m_listNKMEventBuff[l];
			if (nKMEventBuff == null || !nKMEventBuff.m_bReflection || !pDefender.CheckEventCondition(nKMEventBuff.m_Condition))
			{
				continue;
			}
			bool flag2 = false;
			if (pDefender.EventTimer(nKMEventBuff.m_bAnimTime, nKMEventBuff.m_fEventTime, bOneTime: true) && !nKMEventBuff.m_bStateEndTime)
			{
				flag2 = true;
			}
			if (!flag2)
			{
				continue;
			}
			int num2 = nKMEventBuff.m_BuffStatLevel;
			int num3 = nKMEventBuff.m_BuffTimeLevel;
			NKMUnitTemplet unitTemplet = pDefender.GetUnitData().GetUnitTemplet();
			if (unitTemplet != null && unitTemplet.m_UnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
			{
				NKMUnitSkillTemplet unitSkillTempletByType = pDefender.GetUnitData().GetUnitSkillTempletByType(pDefender.GetUnitStateNow().m_NKM_SKILL_TYPE);
				if (unitSkillTempletByType != null && unitSkillTempletByType.m_Level > 0)
				{
					if (nKMEventBuff.m_BuffStatLevelPerSkillLevel > 0)
					{
						num2 += (unitSkillTempletByType.m_Level - 1) * nKMEventBuff.m_BuffStatLevelPerSkillLevel;
					}
					if (nKMEventBuff.m_BuffTimeLevelPerSkillLevel > 0)
					{
						num3 += (unitSkillTempletByType.m_Level - 1) * nKMEventBuff.m_BuffTimeLevelPerSkillLevel;
					}
				}
			}
			pAttackerUnit.AddBuffByStrID(nKMEventBuff.m_BuffStrID, (byte)num2, (byte)num3, pDefender.GetUnitDataGame().m_GameUnitUID, bUseMasterStat: true, bRangeSon: false, bStateEndRemove: false, nKMEventBuff.m_Overlap);
		}
	}

	private void ProcessAttackerBuff(NKMDamageInst cNKMDamageInst, NKMUnit cAttackerUnit, string hitBuff, byte hitBuffBaseLevel, byte hitBuffAddLVBySkillLV, byte hitBuffTimeBaseLevel, byte hitBuffTimeAddLVBySkillLV, bool bRemove, int overlap)
	{
		NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(hitBuff);
		if (buffTempletByStrID == null)
		{
			return;
		}
		if (bRemove)
		{
			cAttackerUnit?.DeleteBuff(hitBuff);
		}
		else
		{
			if (cNKMDamageInst.m_AtkBuffCount >= buffTempletByStrID.m_RangeSonCount || cAttackerUnit == null)
			{
				return;
			}
			byte b = 0;
			byte b2 = 0;
			NKMUnitState unitState = cAttackerUnit.GetUnitState(cAttackerUnit.GetUnitSyncData().m_StateID);
			if (unitState != null && (unitState.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER || unitState.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_SKILL))
			{
				NKMUnitSkillTemplet stateSkill = cAttackerUnit.GetStateSkill(unitState);
				if (stateSkill != null)
				{
					b = (byte)(hitBuffAddLVBySkillLV * (stateSkill.m_Level - 1));
					b2 = (byte)(hitBuffTimeAddLVBySkillLV * (stateSkill.m_Level - 1));
				}
			}
			cAttackerUnit.AddBuffByID(buffTempletByStrID.m_BuffID, (byte)(hitBuffBaseLevel + b), (byte)(hitBuffTimeBaseLevel + b2), cAttackerUnit.GetUnitDataGame().m_GameUnitUID, bUseMasterStat: true, bRangeSon: false, bStateEndRemove: false, overlap);
			cNKMDamageInst.m_AtkBuffCount++;
		}
	}

	private void ProcessDefenderBuff(NKMDamageInst cNKMDamageInst, NKMUnit cAttackerUnit, NKMUnit cDefenderUnit, string hitBuff, byte hitBuffBaseLevel, byte hitBuffAddLVBySkillLV, byte hitBuffTimeBaseLevel, byte hitBuffTimeAddLVBySkillLV, bool bRemove, int overlap)
	{
		NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(hitBuff);
		if (buffTempletByStrID == null)
		{
			return;
		}
		if (bRemove)
		{
			if (cDefenderUnit != null && cAttackerUnit != null)
			{
				bool bFromEnemy = !cDefenderUnit.IsAlly(cAttackerUnit);
				cDefenderUnit.DeleteBuff(hitBuff, bFromEnemy);
			}
		}
		else
		{
			if (cNKMDamageInst.m_DefBuffCount >= buffTempletByStrID.m_RangeSonCount || cAttackerUnit == null)
			{
				return;
			}
			byte b = 0;
			byte b2 = 0;
			NKMUnitState unitState = cAttackerUnit.GetUnitState(cAttackerUnit.GetUnitSyncData().m_StateID);
			if (unitState != null && (unitState.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER || unitState.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_SKILL))
			{
				NKMUnitSkillTemplet stateSkill = cAttackerUnit.GetStateSkill(unitState);
				if (stateSkill != null)
				{
					b = (byte)(hitBuffAddLVBySkillLV * (stateSkill.m_Level - 1));
					b2 = (byte)(hitBuffTimeAddLVBySkillLV * (stateSkill.m_Level - 1));
				}
			}
			cDefenderUnit.AddBuffByID(buffTempletByStrID.m_BuffID, (byte)(hitBuffBaseLevel + b), (byte)(hitBuffTimeBaseLevel + b2), cAttackerUnit.GetUnitDataGame().m_GameUnitUID, bUseMasterStat: true, bRangeSon: false, bStateEndRemove: false, overlap);
			cNKMDamageInst.m_DefBuffCount++;
		}
	}

	public void ProcessHitTrigger(NKMDamageInst cNKMDamageInst, NKMUnit pAttacker, NKMUnit pDefender)
	{
		if (!string.IsNullOrEmpty(cNKMDamageInst.m_Templet.m_HitTrigger))
		{
			int triggerID = pAttacker.GetUnitTemplet().GetTriggerID(cNKMDamageInst.m_Templet.m_HitTrigger);
			pDefender.InvokeTrigger(pAttacker, triggerID);
		}
	}

	public void ProcessHitEvent(NKMDamageInst cNKMDamageInst, NKMUnit pAttackerUnit, NKMUnit pDefender, bool bFromDE)
	{
		if (pAttackerUnit == null || cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE || cNKMDamageInst.m_Templet.m_EventMove == null)
		{
			return;
		}
		bool flag = true;
		if (cNKMDamageInst.m_Templet.m_CrashSuperArmorLevel != NKM_SUPER_ARMOR_LEVEL.NSAL_INVALID && IsEnemy(pAttackerUnit.GetUnitDataGame().m_NKM_TEAM_TYPE, pDefender.GetUnitDataGame().m_NKM_TEAM_TYPE))
		{
			if (pDefender.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NO_DAMAGE_BACK_SPEED))
			{
				flag = false;
			}
			NKM_SUPER_ARMOR_LEVEL currentSuperArmorLevel = pDefender.GetUnitFrameData().CurrentSuperArmorLevel;
			if (currentSuperArmorLevel != NKM_SUPER_ARMOR_LEVEL.NSAL_NO && currentSuperArmorLevel >= cNKMDamageInst.m_Templet.m_CrashSuperArmorLevel)
			{
				flag = false;
			}
			if (pDefender.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_BACK_RESIST) >= 1f)
			{
				flag = false;
			}
			if (pDefender.GetUnitTemplet().m_fDamageBackFactor == 0f)
			{
				flag = false;
			}
		}
		if (pDefender.GetUnitTemplet().m_UnitTempletBase.IsShip())
		{
			flag = false;
		}
		if (pDefender.IsBoss())
		{
			flag = false;
		}
		if (flag)
		{
			pDefender.ProcessEventMove(cNKMDamageInst.m_Templet.m_EventMove, bUseEventTimer: false, bStateEnd: false, bFromDE, pAttackerUnit);
		}
	}

	public void SetStopTime(float fStopTime, NKM_STOP_TIME_INDEX stopTimeIndex)
	{
		SetStopTime(-1, fStopTime, bStopSelf: true, bStopSummonee: true, stopTimeIndex);
	}

	public void SetStopTime(short callUnitUID, float fStopTime, bool bStopSelf, bool bStopSummonee, NKM_STOP_TIME_INDEX stopTimeIndex)
	{
		TimeStopEventInfo item = new TimeStopEventInfo
		{
			fStopTime = fStopTime,
			stopTimeIndex = stopTimeIndex,
			bStopSelf = bStopSelf,
			callUnitUID = callUnitUID,
			bStopSummonee = bStopSummonee
		};
		m_qEventTimeStop.Enqueue(item);
	}

	protected void ProcessStopTime()
	{
		if (!(m_fWorldStopTime > 0f) && m_qEventTimeStop.Count > 0)
		{
			TimeStopEventInfo timeStopEventInfo = m_qEventTimeStop.Dequeue();
			if (timeStopEventInfo.callUnitUID == -1)
			{
				ApplyStopTime(timeStopEventInfo.fStopTime, timeStopEventInfo.stopTimeIndex);
			}
			else
			{
				ApplyStopTime(timeStopEventInfo.callUnitUID, timeStopEventInfo.fStopTime, timeStopEventInfo.bStopSelf, timeStopEventInfo.bStopSummonee, timeStopEventInfo.stopTimeIndex);
			}
			SetWorldStopTime(timeStopEventInfo.fStopTime);
		}
	}

	private void ApplyStopTime(float fStopTime, NKM_STOP_TIME_INDEX stopTimeIndex)
	{
		for (int i = 0; i < m_listNKMUnit.Count; i++)
		{
			m_listNKMUnit[i]?.SetStopTime(fStopTime, stopTimeIndex);
		}
	}

	private void ApplyStopTime(short callUnitUID, float fStopTime, bool bMyStop, bool bSummoneeStop, NKM_STOP_TIME_INDEX stopTimeIndex)
	{
		NKMUnit unit = GetUnit(callUnitUID);
		if (unit == null)
		{
			return;
		}
		for (int i = 0; i < m_listNKMUnit.Count; i++)
		{
			NKMUnit nKMUnit = m_listNKMUnit[i];
			if (nKMUnit != null && (bMyStop || unit != nKMUnit) && (bSummoneeStop || nKMUnit.GetMasterUnit(bPool: false) != unit))
			{
				nKMUnit.SetStopTime(fStopTime, stopTimeIndex);
			}
		}
	}

	public void SetStopReserveTime(short callUnitUID, float fStopReserveTime, bool bEnemyOnly)
	{
		NKMUnit unit = GetUnit(callUnitUID);
		if (unit == null)
		{
			return;
		}
		for (int i = 0; i < m_listNKMUnit.Count; i++)
		{
			NKMUnit nKMUnit = m_listNKMUnit[i];
			if (!bEnemyOnly || IsEnemy(unit.GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				nKMUnit.SetStopReserveTime(fStopReserveTime);
			}
		}
	}

	public bool HasBattleCondition(int id)
	{
		return m_NKMGameData.m_BattleConditionIDs.ContainsKey(id);
	}

	public bool HasBattleCondition(NKMBattleConditionTemplet bcTemplet)
	{
		if (bcTemplet == null)
		{
			return false;
		}
		return m_NKMGameData.m_BattleConditionIDs.ContainsKey(bcTemplet.BattleCondID);
	}

	protected void ProcessDelayedBattleConditions()
	{
		foreach (KeyValuePair<int, int> battleConditionID in m_NKMGameData.m_BattleConditionIDs)
		{
			NKMBattleConditionTemplet templetByID = NKMBattleConditionManager.GetTempletByID(battleConditionID.Key);
			if (templetByID != null && !IsBattleConditionActivated(templetByID) && templetByID.ActiveTimeLeft >= m_NKMGameRuntimeData.m_fRemainGameTime)
			{
				ActivateDelayedBattleConditions(templetByID, battleConditionID.Value);
			}
		}
	}

	public bool IsBattleConditionActivated(NKMBattleConditionTemplet bcTemplet)
	{
		if (bcTemplet == null)
		{
			return false;
		}
		if (bcTemplet.ActiveTimeLeft <= 0f)
		{
			return true;
		}
		return m_hsEnabledBattleConditions.Contains(bcTemplet.BattleCondID);
	}

	protected virtual void ActivateDelayedBattleConditions(NKMBattleConditionTemplet bcTemplet, int level)
	{
		m_hsEnabledBattleConditions.Add(bcTemplet.BattleCondID);
	}

	public float GetZScaleFactor(float fPosZ)
	{
		return 1.05f - (fPosZ - GetMapTemplet().m_fMinZ) * 0.001f;
	}

	public void AllKill(NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnit.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMUnit value = enumerator.Current.Value;
			if (value.GetUnitDataGame().m_NKM_TEAM_TYPE == eNKM_TEAM_TYPE && !IsBoss(value.GetUnitSyncData().m_GameUnitUID))
			{
				value.GetUnitSyncData().SetHP(0f);
				value.SetDying(bForce: true);
			}
		}
	}

	public void PracticeBossStateChange(string stateName)
	{
		if (GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE || GetGameData().m_NKMGameTeamDataB.m_MainShip == null)
		{
			return;
		}
		for (int i = 0; i < GetGameData().m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID.Count; i++)
		{
			short gameUnitUID = GetGameData().m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID[i];
			NKMUnit unit = GetUnit(gameUnitUID, bChain: true, bPool: true);
			if (unit != null)
			{
				unit.StateChange(stateName);
				break;
			}
		}
	}

	public void PracticeHealEnable(bool value)
	{
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			GetGameRuntimeData().m_bPracticeHeal = value;
		}
	}

	public void PracticeFixedDamageEnable(bool value)
	{
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			GetGameRuntimeData().m_bPracticeFixedDamage = value;
		}
	}

	public void DEV_HPReset(NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnit.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMUnit value = enumerator.Current.Value;
			if (value.GetUnitDataGame().m_NKM_TEAM_TYPE == eNKM_TEAM_TYPE)
			{
				value.GetUnitSyncData().SetHP(value.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP));
			}
		}
	}

	public void DEV_SkillCoolTimeReset(NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnit.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMUnit value = enumerator.Current.Value;
			if (value.GetUnitDataGame().m_NKM_TEAM_TYPE != eNKM_TEAM_TYPE)
			{
				continue;
			}
			for (int i = 0; i < value.GetUnitTemplet().m_listSkillStateData.Count; i++)
			{
				if (value.GetUnitTemplet().m_listSkillStateData[i] != null)
				{
					value.StateCoolTimeReset(value.GetUnitTemplet().m_listSkillStateData[i].m_StateName);
				}
			}
			for (int j = 0; j < value.GetUnitTemplet().m_listAirSkillStateData.Count; j++)
			{
				if (value.GetUnitTemplet().m_listAirSkillStateData[j] != null)
				{
					value.StateCoolTimeReset(value.GetUnitTemplet().m_listAirSkillStateData[j].m_StateName);
				}
			}
			if (!IsBoss(value.GetUnitSyncData().m_GameUnitUID))
			{
				continue;
			}
			for (int k = 0; k < value.GetUnitTemplet().m_UnitTempletBase.GetSkillCount(); k++)
			{
				NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(value.GetUnitTemplet().m_UnitTempletBase, k);
				if (shipSkillTempletByIndex != null && shipSkillTempletByIndex.m_UnitStateName.Length > 1)
				{
					value.StateCoolTimeReset(shipSkillTempletByIndex.m_UnitStateName);
				}
			}
		}
	}

	public void DEV_HyperSkillCoolTimeReset(NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnit.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMUnit value = enumerator.Current.Value;
			if (value.GetUnitDataGame().m_NKM_TEAM_TYPE != eNKM_TEAM_TYPE)
			{
				continue;
			}
			for (int i = 0; i < value.GetUnitTemplet().m_listHyperSkillStateData.Count; i++)
			{
				if (value.GetUnitTemplet().m_listHyperSkillStateData[i] != null)
				{
					value.StateCoolTimeReset(value.GetUnitTemplet().m_listHyperSkillStateData[i].m_StateName);
				}
			}
			for (int j = 0; j < value.GetUnitTemplet().m_listAirHyperSkillStateData.Count; j++)
			{
				if (value.GetUnitTemplet().m_listAirHyperSkillStateData[j] != null)
				{
					value.StateCoolTimeReset(value.GetUnitTemplet().m_listAirHyperSkillStateData[j].m_StateName);
				}
			}
			if (!IsBoss(value.GetUnitSyncData().m_GameUnitUID))
			{
				continue;
			}
			for (int k = 0; k < value.GetUnitTemplet().m_UnitTempletBase.GetSkillCount(); k++)
			{
				NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(value.GetUnitTemplet().m_UnitTempletBase, k);
				if (shipSkillTempletByIndex != null && shipSkillTempletByIndex.m_UnitStateName.Length > 1)
				{
					value.StateCoolTimeReset(shipSkillTempletByIndex.m_UnitStateName);
				}
			}
		}
	}

	public bool IsBoss(short gameUnitUID)
	{
		if (GetGameData().m_NKMGameTeamDataA.m_MainShip != null)
		{
			for (int i = 0; i < GetGameData().m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID.Count; i++)
			{
				if (gameUnitUID == GetGameData().m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID[i])
				{
					return true;
				}
			}
		}
		if (GetGameData().m_NKMGameTeamDataB.m_MainShip != null)
		{
			for (int j = 0; j < GetGameData().m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID.Count; j++)
			{
				if (gameUnitUID == GetGameData().m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID[j])
				{
					return true;
				}
			}
		}
		return false;
	}

	public NKM_DUNGEON_TYPE GetDungeonType()
	{
		if (m_NKMGameData != null && m_NKMGameData.IsPVE() && m_NKMDungeonTemplet != null && m_NKMDungeonTemplet.m_DungeonTempletBase != null)
		{
			return m_NKMDungeonTemplet.m_DungeonTempletBase.m_DungeonType;
		}
		return NKM_DUNGEON_TYPE.NDT_INVALID;
	}

	public float GetHyperBeginRatio(NKM_TEAM_TYPE team)
	{
		float result = -1f;
		if (m_NKMDungeonTemplet != null)
		{
			switch (team)
			{
			case NKM_TEAM_TYPE.NTT_A1:
			case NKM_TEAM_TYPE.NTT_A2:
				result = m_NKMDungeonTemplet.m_fAllyHyperCooltimeStartRatio;
				break;
			case NKM_TEAM_TYPE.NTT_B1:
			case NKM_TEAM_TYPE.NTT_B2:
				result = m_NKMDungeonTemplet.m_fEnemyHyperCooltimeStartRatio;
				break;
			}
		}
		return result;
	}

	public float GetShipSkillBeginRatio(NKM_TEAM_TYPE team)
	{
		if (IsATeam(team) && m_NKMGameData.m_TeamASupply > 0)
		{
			return 1f;
		}
		return 0f;
	}

	protected float GetRespawnPosX(bool bTeamA, float fFactor = -1f)
	{
		float num = (GetGameData().IsPVP() ? NKMCommonConst.PVP_SUMMON_MIN_POS : NKMCommonConst.PVE_SUMMON_MIN_POS);
		if (bTeamA)
		{
			if (fFactor.IsNearlyEqual(-1f))
			{
				return m_NKMMapTemplet.m_fMinX + num;
			}
			float num2 = m_NKMMapTemplet.m_fMaxX - m_NKMMapTemplet.m_fMinX;
			return NKMMathf.Max(m_NKMMapTemplet.m_fMinX + num2 * fFactor, m_NKMMapTemplet.m_fMinX + num);
		}
		if (fFactor.IsNearlyEqual(-1f))
		{
			return m_NKMMapTemplet.m_fMaxX - num;
		}
		float num3 = m_NKMMapTemplet.m_fMaxX - m_NKMMapTemplet.m_fMinX;
		return NKMMathf.Min(m_NKMMapTemplet.m_fMaxX - num3 * fFactor, m_NKMMapTemplet.m_fMaxX - num);
	}

	public bool IsEnemy(NKM_TEAM_TYPE eMyTeam, NKM_TEAM_TYPE eTargetTeam)
	{
		if (eMyTeam == eTargetTeam)
		{
			return false;
		}
		switch (eMyTeam)
		{
		case NKM_TEAM_TYPE.NTT_A1:
			if (eTargetTeam == NKM_TEAM_TYPE.NTT_A2)
			{
				return false;
			}
			break;
		case NKM_TEAM_TYPE.NTT_A2:
			if (eTargetTeam == NKM_TEAM_TYPE.NTT_A1)
			{
				return false;
			}
			break;
		case NKM_TEAM_TYPE.NTT_B1:
			if (eTargetTeam == NKM_TEAM_TYPE.NTT_B2)
			{
				return false;
			}
			break;
		case NKM_TEAM_TYPE.NTT_B2:
			if (eTargetTeam == NKM_TEAM_TYPE.NTT_B1)
			{
				return false;
			}
			break;
		}
		return true;
	}

	public bool IsAlly(NKM_TEAM_TYPE eMyTeam, NKM_TEAM_TYPE eTargetTeam)
	{
		switch (eMyTeam)
		{
		case NKM_TEAM_TYPE.NTT_A1:
			if (eTargetTeam == NKM_TEAM_TYPE.NTT_A2)
			{
				return true;
			}
			break;
		case NKM_TEAM_TYPE.NTT_A2:
			if (eTargetTeam == NKM_TEAM_TYPE.NTT_A1)
			{
				return true;
			}
			break;
		case NKM_TEAM_TYPE.NTT_B1:
			if (eTargetTeam == NKM_TEAM_TYPE.NTT_B2)
			{
				return true;
			}
			break;
		case NKM_TEAM_TYPE.NTT_B2:
			if (eTargetTeam == NKM_TEAM_TYPE.NTT_B1)
			{
				return true;
			}
			break;
		}
		return false;
	}

	public bool IsSameTeam(NKM_TEAM_TYPE eMyTeam, NKM_TEAM_TYPE eTargetTeam)
	{
		return IsSameTeamStaticFunc(eMyTeam, eTargetTeam);
	}

	public static bool IsSameTeamStaticFunc(NKM_TEAM_TYPE eMyTeam, NKM_TEAM_TYPE eTargetTeam)
	{
		switch (eMyTeam)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			if (eTargetTeam != NKM_TEAM_TYPE.NTT_A1)
			{
				return eTargetTeam == NKM_TEAM_TYPE.NTT_A2;
			}
			return true;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			if (eTargetTeam != NKM_TEAM_TYPE.NTT_B1)
			{
				return eTargetTeam == NKM_TEAM_TYPE.NTT_B2;
			}
			return true;
		case NKM_TEAM_TYPE.NTT_C:
			return eTargetTeam == NKM_TEAM_TYPE.NTT_C;
		default:
			return false;
		}
	}

	public bool IsReversePosTeam(NKM_TEAM_TYPE eTeam)
	{
		if (eTeam == NKM_TEAM_TYPE.NTT_B1 || eTeam == NKM_TEAM_TYPE.NTT_B2 || eTeam == NKM_TEAM_TYPE.NTT_C)
		{
			return true;
		}
		return false;
	}

	public bool IsATeam(NKM_TEAM_TYPE eTeam)
	{
		return IsATeamStaticFunc(eTeam);
	}

	public static bool IsATeamStaticFunc(NKM_TEAM_TYPE eTeam)
	{
		if (eTeam == NKM_TEAM_TYPE.NTT_A1 || eTeam == NKM_TEAM_TYPE.NTT_A2)
		{
			return true;
		}
		return false;
	}

	public bool IsBTeam(NKM_TEAM_TYPE eTeam)
	{
		return IsBTeamStaticFunc(eTeam);
	}

	public static bool IsBTeamStaticFunc(NKM_TEAM_TYPE eTeam)
	{
		if (eTeam == NKM_TEAM_TYPE.NTT_B1 || eTeam == NKM_TEAM_TYPE.NTT_B2)
		{
			return true;
		}
		return false;
	}

	public void GiveUp()
	{
		m_NKMGameRuntimeData.m_bGiveUp = true;
		m_NKMGameRuntimeData.m_WinTeam = NKM_TEAM_TYPE.NTT_B1;
		SetGameState(NKM_GAME_STATE.NGS_FINISH);
		m_NKMGameRuntimeData.m_bPause = false;
	}

	public void RestartGame()
	{
		m_NKMGameRuntimeData.m_bRestart = true;
		GiveUp();
	}

	protected virtual bool SetGameState(NKM_GAME_STATE eNKM_GAME_STATE)
	{
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_END)
		{
			return false;
		}
		m_NKMGameRuntimeData.m_NKM_GAME_STATE = eNKM_GAME_STATE;
		return true;
	}

	public static bool CanUseAutoRespawnOnlyDungeon(NKMUserData cNKMUserData, int dungeonID)
	{
		switch (dungeonID)
		{
		case 0:
			return false;
		case 20001:
		case 20002:
		case 20003:
		case 20004:
		case 20005:
			return false;
		default:
			return true;
		}
	}

	public static bool CanUseAutoRespawnOnlyWarfare(NKMUserData userData, int warfareID)
	{
		if (warfareID == 0)
		{
			return false;
		}
		return true;
	}

	public bool CanUseAutoRespawn(NKMUserData cNKMUserData)
	{
		switch (GetGameData().GetGameType())
		{
		case NKM_GAME_TYPE.NGT_TUTORIAL:
			return false;
		case NKM_GAME_TYPE.NGT_WARFARE:
			return CanUseAutoRespawnOnlyWarfare(cNKMUserData, GetGameData().m_WarfareID);
		case NKM_GAME_TYPE.NGT_DUNGEON:
		case NKM_GAME_TYPE.NGT_PHASE:
		case NKM_GAME_TYPE.NGT_TRIM:
			return CanUseAutoRespawnOnlyDungeon(cNKMUserData, GetGameData().m_DungeonID);
		default:
			return true;
		}
	}

	public bool IsPVP()
	{
		return GetGameData().IsPVP();
	}

	public bool IsPVE()
	{
		return GetGameData().IsPVE();
	}

	public int GetRespawnCost(NKMUnitStatTemplet cNKMUnitStatTemplet, bool bLeader, NKM_TEAM_TYPE teamType)
	{
		if (IsPVE() || GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_ASYNC_PVP || (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVP_STRATEGY && IsATeam(teamType)) || (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE && IsATeam(teamType)) || GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC)
		{
			return cNKMUnitStatTemplet.GetRespawnCost(bPVP: false, bLeader, null, null);
		}
		return cNKMUnitStatTemplet.GetRespawnCost(GetGameData().IsPVP(), bLeader, GetGameData().m_dicNKMBanData, GetGameData().m_dicNKMUpData);
	}

	public bool IsBanUnit(int unitID)
	{
		return GetGameData().IsBanUnit(unitID);
	}

	public bool IsUpUnit(int unitID)
	{
		return GetGameData().IsUpUnit(unitID);
	}

	public bool IsBanShip(int shipGroupId)
	{
		return GetGameData().IsBanShip(shipGroupId);
	}

	public int GetBanShipLevel(int shipGroupId)
	{
		return GetGameData().GetBanShipLevel(shipGroupId);
	}

	public void EventDieForReRespawn(long unitUID)
	{
		NKMUnitData unitDataByUnitUID = GetGameData().GetUnitDataByUnitUID(unitUID);
		if (unitDataByUnitUID != null)
		{
			for (int i = 0; i < unitDataByUnitUID.m_listGameUnitUID.Count; i++)
			{
				EventDieForReRespawnInner(unitDataByUnitUID.m_listGameUnitUID[i]);
			}
			for (int j = 0; j < unitDataByUnitUID.m_listGameUnitUIDChange.Count; j++)
			{
				EventDieForReRespawnInner(unitDataByUnitUID.m_listGameUnitUIDChange[j]);
			}
		}
	}

	public void EventDieForReRespawnInner(short gameUnitUID)
	{
		NKMUnit unit = GetUnit(gameUnitUID);
		if (unit != null)
		{
			unit.AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_RE_RESPAWN_EFFECT);
			unit.EventDie(bImmediate: true, bCheckAllDie: false);
			unit.PushSyncData();
			m_dicNKMUnitPool.Add(gameUnitUID, unit);
			m_dicNKMUnit.Remove(gameUnitUID);
			m_listNKMUnit.Remove(unit);
			unit.SetDie();
		}
	}

	protected float GetRollbackTime(NKMUnitTemplet targetUnitTemplet)
	{
		if (!NKMCommonConst.USE_ROLLBACK)
		{
			return 0f;
		}
		if (targetUnitTemplet == null)
		{
			return 0f;
		}
		if (GetWorldStopTime() > 0f)
		{
			return 0f;
		}
		float num = Math.Min(targetUnitTemplet.m_fMaxRollbackTime, NKMCommonConst.SUMMON_UNIT_NOEVENT_TIME);
		float num2 = m_AbsoluteGameTime - num;
		if (m_fLastWorldStopFinishedATime > num2)
		{
			return m_AbsoluteGameTime - m_fLastWorldStopFinishedATime;
		}
		return num;
	}

	public bool IsInDynamicRespawnUnitReserve(short gameUnitUID)
	{
		for (int i = 0; i < m_listNKMGameUnitDynamicRespawnData.Count; i++)
		{
			NKMDynamicRespawnUnitReserve nKMDynamicRespawnUnitReserve = m_listNKMGameUnitDynamicRespawnData[i];
			if (nKMDynamicRespawnUnitReserve != null && nKMDynamicRespawnUnitReserve.m_GameUnitUID == gameUnitUID)
			{
				return true;
			}
		}
		return false;
	}

	public virtual void ChangeRemainGameTime(float newTime, bool delta, bool bShowEffect)
	{
	}

	public void UnitBatch(NKMUnit finderUnit, float rangeMin, float rangeMax, OnUnitBatch onBatch)
	{
		if (onBatch == null)
		{
			return;
		}
		bool bRight = finderUnit.GetUnitSyncData().m_bRight;
		float posX = finderUnit.GetUnitSyncData().m_PosX;
		foreach (NKMUnit item in finderUnit.GetSortUnitListByNearDist())
		{
			if (item != null && !item.IsDyingOrDie() && item.WillInteractWithGameUnits())
			{
				float num = (bRight ? (item.GetUnitSyncData().m_PosX - posX) : (posX - item.GetUnitSyncData().m_PosX));
				if (!(num < rangeMin) && !(num > rangeMax) && onBatch(item, finderUnit, num))
				{
					break;
				}
			}
		}
	}
}
