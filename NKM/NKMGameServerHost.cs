using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Game;
using Cs.Logging;
using Cs.Math;
using Cs.Protocol;
using NKM.Game;
using NKM.Templet;

namespace NKM;

public abstract class NKMGameServerHost : NKMGame
{
	private NKMGameSyncDataPack m_NKMGameSyncDataPack = new NKMGameSyncDataPack();

	private float m_SyncFlushTime;

	private NKMDamageEffectManager m_DEManager = new NKMDamageEffectManager();

	protected bool m_bTeamAFirstRespawn;

	protected bool m_bTeamBFirstRespawn;

	private bool m_bUseStateChangeEvent;

	private Dictionary<long, int> m_dicRespawnUnitUID_ByDeckUsed = new Dictionary<long, int>();

	protected NKMReservedTacticalCommand m_NKMReservedTacticalCommandTeamA = new NKMReservedTacticalCommand();

	protected NKMReservedTacticalCommand m_NKMReservedTacticalCommandTeamB = new NKMReservedTacticalCommand();

	protected bool m_bUseTurtlingPrevent;

	protected bool m_bTurtleWarningSentA;

	protected bool m_bTurtleWarningSentB;

	public NKMGameServerHost()
	{
		m_NKM_GAME_CLASS_TYPE = NKM_GAME_CLASS_TYPE.NGCT_GAME_SERVER;
		m_ObjectPool = new NKMObjectPool();
		Init();
		m_DEManager.Init(this);
	}

	public override void Init()
	{
		base.Init();
		m_listNKMGameUnitDynamicRespawnData.Clear();
		m_NKMGameSyncDataPack = new NKMGameSyncDataPack();
		m_SyncFlushTime = 0f;
		m_DEManager.Init(this);
		if (m_dicRespawnUnitUID_ByDeckUsed != null)
		{
			m_dicRespawnUnitUID_ByDeckUsed.Clear();
		}
		if (m_NKMReservedTacticalCommandTeamA != null)
		{
			m_NKMReservedTacticalCommandTeamA.Invalidate();
		}
		if (m_NKMReservedTacticalCommandTeamB != null)
		{
			m_NKMReservedTacticalCommandTeamB.Invalidate();
		}
		m_bTeamAFirstRespawn = false;
		m_bTeamBFirstRespawn = false;
	}

	public void SetOperatorBanSkillLevel()
	{
		if (!IsPVP())
		{
			return;
		}
		if (base.m_NKMGameData.m_NKMGameTeamDataA.m_Operator != null)
		{
			NKMOperator nKMOperator = base.m_NKMGameData.m_NKMGameTeamDataA.m_Operator;
			if (base.m_NKMGameData.IsBanOperator(nKMOperator.id))
			{
				int banOperatorLevel = base.m_NKMGameData.GetBanOperatorLevel(nKMOperator.id);
				byte level = nKMOperator.mainSkill.level;
				nKMOperator.mainSkill.level = (byte)Math.Max(level - banOperatorLevel * NKMUnitStatManager.m_OperatorSkillLevelPerBanLevel, NKMUnitStatManager.m_MinOperatorSkillLevelPerBanLevel);
				level = nKMOperator.subSkill.level;
				nKMOperator.subSkill.level = (byte)Math.Max(level - banOperatorLevel * NKMUnitStatManager.m_OperatorSkillLevelPerBanLevel, NKMUnitStatManager.m_MinOperatorSkillLevelPerBanLevel);
			}
		}
		if (base.m_NKMGameData.m_NKMGameTeamDataB.m_Operator != null)
		{
			NKMOperator nKMOperator2 = base.m_NKMGameData.m_NKMGameTeamDataB.m_Operator;
			if (base.m_NKMGameData.IsBanOperator(nKMOperator2.id))
			{
				int banOperatorLevel2 = base.m_NKMGameData.GetBanOperatorLevel(nKMOperator2.id);
				byte level2 = nKMOperator2.mainSkill.level;
				nKMOperator2.mainSkill.level = (byte)Math.Max(level2 - banOperatorLevel2 * NKMUnitStatManager.m_OperatorSkillLevelPerBanLevel, NKMUnitStatManager.m_MinOperatorSkillLevelPerBanLevel);
				level2 = nKMOperator2.subSkill.level;
				nKMOperator2.subSkill.level = (byte)Math.Max(level2 - banOperatorLevel2 * NKMUnitStatManager.m_OperatorSkillLevelPerBanLevel, NKMUnitStatManager.m_MinOperatorSkillLevelPerBanLevel);
			}
		}
	}

	public virtual void SetGameData(NKMGameData cNKMGameData)
	{
		Init();
		base.m_NKMGameData = cNKMGameData;
		if (base.m_NKMGameData.m_DungeonID > 0)
		{
			m_NKMDungeonTemplet = NKMDungeonManager.GetDungeonTemplet(base.m_NKMGameData.m_DungeonID);
		}
		else
		{
			m_NKMDungeonTemplet = null;
		}
		if (m_NKMDungeonTemplet != null)
		{
			GetGameDevModeData().m_bDevForcePvp = m_NKMDungeonTemplet.m_bDevForcePVP;
		}
		SetOperatorBanSkillLevel();
		SetDefaultTacticalCommand();
		InitDungeonEventData();
		m_bUseStateChangeEvent = false;
		for (int i = 0; i < m_listDungeonEventDataTeamA.Count; i++)
		{
			NKMDungeonEventData nKMDungeonEventData = m_listDungeonEventDataTeamA[i];
			if (nKMDungeonEventData.m_DungeonEventTemplet.m_EventCondition == NKM_EVENT_START_CONDITION_TYPE.UNIT_STATE_START || nKMDungeonEventData.m_DungeonEventTemplet.m_EventCondition == NKM_EVENT_START_CONDITION_TYPE.UNIT_STATE_END)
			{
				m_bUseStateChangeEvent = true;
				break;
			}
		}
		if (!m_bUseStateChangeEvent)
		{
			for (int j = 0; j < m_listDungeonEventDataTeamB.Count; j++)
			{
				NKMDungeonEventData nKMDungeonEventData2 = m_listDungeonEventDataTeamB[j];
				if (nKMDungeonEventData2.m_DungeonEventTemplet.m_EventCondition == NKM_EVENT_START_CONDITION_TYPE.UNIT_STATE_START || nKMDungeonEventData2.m_DungeonEventTemplet.m_EventCondition == NKM_EVENT_START_CONDITION_TYPE.UNIT_STATE_END)
				{
					m_bUseStateChangeEvent = true;
					break;
				}
			}
		}
		SetMap();
		SetRemainGameTime();
		SetUnit();
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PREVENTION_TURTLING) && NKMCommonConst.PVP_AFK_APPLY_MODE.Contains(cNKMGameData.m_NKM_GAME_TYPE))
		{
			m_bUseTurtlingPrevent = true;
		}
		if (!IsGameUsingDoubleCostTime())
		{
			base.m_NKMGameData.m_fDoubleCostTime = -100f;
		}
	}

	protected override void SetDefaultTacticalCommand()
	{
		base.SetDefaultTacticalCommand();
		if (GetGameData() != null)
		{
			AddTacticalCommand(GetGameData().m_NKMGameTeamDataA);
			AddTacticalCommand(GetGameData().m_NKMGameTeamDataB);
		}
	}

	public virtual void SetGameRuntimeData(NKMGameRuntimeData cNKMRuntimeGameData)
	{
		m_NKMGameRuntimeData = cNKMRuntimeGameData;
	}

	public virtual float GetInitialGameTimeSec()
	{
		return 180f;
	}

	private void SetRemainGameTime()
	{
		if (base.m_NKMGameData == null || m_NKMGameRuntimeData == null)
		{
			return;
		}
		float initialGameTimeSec = GetInitialGameTimeSec();
		if (base.m_NKMGameData.IsPVE())
		{
			if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
			{
				m_NKMGameRuntimeData.m_fRemainGameTime = 999999f;
				return;
			}
			switch (GetDungeonType())
			{
			case NKM_DUNGEON_TYPE.NDT_DAMAGE_ACCRUE:
			{
				NKMDungeonTemplet dungeonTemplet3 = NKMDungeonManager.GetDungeonTemplet(GetGameData().m_DungeonID);
				m_NKMGameRuntimeData.m_fRemainGameTime = dungeonTemplet3?.m_DungeonTempletBase.m_fGameTime ?? 60f;
				break;
			}
			case NKM_DUNGEON_TYPE.NDT_WAVE:
			{
				NKMDungeonTemplet dungeonTemplet2 = NKMDungeonManager.GetDungeonTemplet(GetGameData().m_DungeonID);
				NKMDungeonWaveTemplet nKMDungeonWaveTemplet = null;
				nKMDungeonWaveTemplet = dungeonTemplet2?.GetWaveTemplet(1);
				m_NKMGameRuntimeData.m_fRemainGameTime = ((dungeonTemplet2 != null && nKMDungeonWaveTemplet != null) ? nKMDungeonWaveTemplet.m_fNextWavetime : initialGameTimeSec);
				break;
			}
			default:
			{
				NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(GetGameData().m_DungeonID);
				m_NKMGameRuntimeData.m_fRemainGameTime = dungeonTemplet?.m_DungeonTempletBase.m_fGameTime ?? initialGameTimeSec;
				break;
			}
			}
		}
		else
		{
			m_NKMGameRuntimeData.m_fRemainGameTime = initialGameTimeSec;
		}
	}

	public void SetMap()
	{
		m_NKMMapTemplet = NKMMapManager.GetMapTempletByID(base.m_NKMGameData.m_MapID);
	}

	public void SetUnit()
	{
		CreateGameUnitUID();
		CreatePoolUnit(bAsync: true);
		CreateDynaminRespawnPoolUnit(bAsync: true);
		float num = 1f;
		float num2 = 1f;
		if (IsPVP())
		{
			if (base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip != null && NKMGame.ApplyUpBanByGameType(base.m_NKMGameData, NKM_TEAM_TYPE.NTT_A1))
			{
				NKMUnitTemplet unitTemplet = base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip.GetUnitTemplet();
				if (unitTemplet == null)
				{
					Log.Error($"Invalid Ship Templet Data ShipId:{base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 349);
					return;
				}
				if (base.m_NKMGameData.IsBanShip(unitTemplet.m_UnitTempletBase.m_ShipGroupID))
				{
					int banShipLevel = base.m_NKMGameData.GetBanShipLevel(unitTemplet.m_UnitTempletBase.m_ShipGroupID);
					num = 1f - NKMUnitStatManager.m_fPercentPerBanLevel * (float)banShipLevel;
					if (num < 1f - NKMUnitStatManager.m_fMaxPercentPerBanLevel)
					{
						num = 1f - NKMUnitStatManager.m_fMaxPercentPerBanLevel;
					}
				}
			}
			if (base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip != null && NKMGame.ApplyUpBanByGameType(base.m_NKMGameData, NKM_TEAM_TYPE.NTT_B1))
			{
				NKMUnitTemplet unitTemplet2 = base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip.GetUnitTemplet();
				if (unitTemplet2 == null)
				{
					Log.Error($"Invalid Ship Templet Data ShipId:{base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 368);
					return;
				}
				if (base.m_NKMGameData.IsBanShip(unitTemplet2.m_UnitTempletBase.m_ShipGroupID))
				{
					int banShipLevel2 = base.m_NKMGameData.GetBanShipLevel(unitTemplet2.m_UnitTempletBase.m_ShipGroupID);
					num2 = 1f - NKMUnitStatManager.m_fPercentPerBanLevel * (float)banShipLevel2;
					if (num2 < 1f - NKMUnitStatManager.m_fMaxPercentPerBanLevel)
					{
						num2 = 1f - NKMUnitStatManager.m_fMaxPercentPerBanLevel;
					}
				}
			}
		}
		if (IsPVP())
		{
			if (base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip != null)
			{
				RespawnUnit(base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip, GetShipRespawnPosX(bTeamA: true), GetShipRespawnPosZ(), num, bInitHPRate: true);
			}
			if (base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip != null)
			{
				RespawnUnit(base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip, GetShipRespawnPosX(bTeamA: false), GetShipRespawnPosZ(), num2, bInitHPRate: true);
			}
		}
		else
		{
			if (base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip != null)
			{
				RespawnUnit(base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip, GetShipRespawnPosX(bTeamA: true), GetShipRespawnPosZ(), base.m_NKMGameData.m_NKMGameTeamDataA.m_fInitHP);
			}
			if (base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip != null)
			{
				if (m_NKMDungeonTemplet != null)
				{
					RespawnUnit(base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip, GetShipRespawnPosX(bTeamA: false), GetShipRespawnPosZ(m_NKMDungeonTemplet.m_fBossPosZ), base.m_NKMGameData.m_NKMGameTeamDataB.m_fInitHP);
				}
				else
				{
					RespawnUnit(base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip, GetShipRespawnPosX(bTeamA: false), GetShipRespawnPosZ());
				}
			}
		}
		foreach (NKMUnitData listEnvUnitDatum in base.m_NKMGameData.m_NKMGameTeamDataA.m_listEnvUnitData)
		{
			RespawnUnit(listEnvUnitDatum, GetShipRespawnPosX(bTeamA: true), GetShipRespawnPosZ());
		}
		foreach (NKMUnitData listEnvUnitDatum2 in base.m_NKMGameData.m_NKMGameTeamDataB.m_listEnvUnitData)
		{
			RespawnUnit(listEnvUnitDatum2, GetShipRespawnPosX(bTeamA: false), GetShipRespawnPosZ());
		}
	}

	public NKMUnit DynamicRespawnUnitReserve(bool bMaxCountReRespawn, short gameUnitUID, float x, float z, float fJumpYPos = 0f, bool bUseRight = false, bool bRight = true, float fHPRate = 0f, string respawnState = null, float rollbackTime = 0f)
	{
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY)
		{
			return null;
		}
		if (m_dicNKMUnitPool.ContainsKey(gameUnitUID))
		{
			NKMUnit nKMUnit = m_dicNKMUnitPool[gameUnitUID];
			if (nKMUnit != null)
			{
				NKMDynamicRespawnUnitReserve item = new NKMDynamicRespawnUnitReserve
				{
					m_GameUnitUID = gameUnitUID,
					m_PosX = x,
					m_PosZ = z,
					m_fJumpYPos = fJumpYPos,
					m_bUseRight = bUseRight,
					m_bRight = bRight,
					m_fHPRate = fHPRate,
					m_RespawnState = respawnState,
					m_fRollbackTime = rollbackTime
				};
				m_listNKMGameUnitDynamicRespawnData.Add(item);
			}
			return nKMUnit;
		}
		if (bMaxCountReRespawn)
		{
			NKMUnit unit = GetUnit(gameUnitUID);
			if (unit != null)
			{
				unit.ProcessUnitDyingBuff();
				unit.RespawnUnit(x, z, fJumpYPos, bUseRight, bRight, fHPRate, bInitHPRate: true, rollbackTime);
				AddStaticBuff(unit);
				unit.SetConserveHPRate();
				unit.CheckAndCalculateBuffStat();
				ProcessRespawnCostReturn(unit);
				if (!string.IsNullOrEmpty(respawnState) && unit.GetUnitState(respawnState) != null)
				{
					unit.StateChange(respawnState, bForceChange: true, bImmediate: true);
				}
			}
			return unit;
		}
		return null;
	}

	public bool DynamicRespawnUnit(short gameUnitUID, float x, float z, float fJumpYPos = 0f, bool bUseRight = false, bool bRight = true, float fHPRate = 0f, string respawnState = "", float rollbackTime = 0f)
	{
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY)
		{
			return false;
		}
		if (m_dicNKMUnitPool.ContainsKey(gameUnitUID))
		{
			NKMUnit nKMUnit = m_dicNKMUnitPool[gameUnitUID];
			if (nKMUnit != null)
			{
				m_dicNKMUnitPool.Remove(gameUnitUID);
				m_dicNKMUnit.Add(nKMUnit.GetUnitDataGame().m_GameUnitUID, nKMUnit);
				m_listNKMUnit.Add(nKMUnit);
				nKMUnit.RespawnUnit(x, z, fJumpYPos, bUseRight, bRight, fHPRate, bInitHPRate: true, rollbackTime);
				AddStaticBuff(nKMUnit);
				nKMUnit.SetConserveHPRate();
				nKMUnit.CheckAndCalculateBuffStat();
				ProcessRespawnCostReturn(nKMUnit);
				if (!string.IsNullOrEmpty(respawnState) && nKMUnit.GetUnitState(respawnState) != null)
				{
					nKMUnit.StateChange(respawnState, bForceChange: true, bImmediate: true);
				}
			}
			return true;
		}
		return false;
	}

	public bool RespawnUnit(NKMUnitData cNKMUnitData, float x, float z, float fInitHP = 0f, bool bInitHPRate = false, float rollbackTime = 0f)
	{
		for (int i = 0; i < cNKMUnitData.m_listGameUnitUID.Count; i++)
		{
			short key = cNKMUnitData.m_listGameUnitUID[i];
			if (!m_dicNKMUnitPool.ContainsKey(key))
			{
				continue;
			}
			NKMUnit nKMUnit = m_dicNKMUnitPool[key];
			if (nKMUnit != null)
			{
				NKMUnitTemplet unitTemplet = nKMUnit.GetUnitTemplet();
				if (unitTemplet != null)
				{
					m_dicNKMUnitPool.Remove(key);
					m_dicNKMUnit.Add(nKMUnit.GetUnitDataGame().m_GameUnitUID, nKMUnit);
					m_listNKMUnit.Add(nKMUnit);
					if (!unitTemplet.m_fForceRespawnXpos.IsNearlyEqual(-1f))
					{
						x = GetRespawnPosX(IsATeam(nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE), unitTemplet.m_fForceRespawnXpos);
					}
					if (!unitTemplet.m_fForceRespawnZposMin.IsNearlyEqual(-1f) || !unitTemplet.m_fForceRespawnZposMax.IsNearlyEqual(-1f))
					{
						z = GetRespawnPosZ(unitTemplet.m_fForceRespawnZposMin, unitTemplet.m_fForceRespawnZposMax);
					}
					nKMUnit.RespawnUnit(x, z, 0f, bUseRight: false, bRight: true, fInitHP, bInitHPRate, rollbackTime);
					AddStaticBuff(nKMUnit);
					nKMUnit.CheckAndCalculateBuffStat();
					ProcessRespawnCostReturn(nKMUnit);
				}
				if (m_bUseStateChangeEvent)
				{
					nKMUnit.ClearAllStateChangeEvent();
					nKMUnit.AddStateChangeEvent(OnUnitStateChangeEvent);
				}
			}
			return true;
		}
		return false;
	}

	private void ProcessRespawnCostReturn(NKMUnit cNKMUnit)
	{
		if (cNKMUnit == null || cNKMUnit.m_usedRespawnCost <= 0)
		{
			return;
		}
		NKM_TEAM_TYPE nKM_TEAM_TYPE_ORG = cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE_ORG;
		NKMGameRuntimeTeamData myRuntimeTeamData = GetGameRuntimeData().GetMyRuntimeTeamData(nKM_TEAM_TYPE_ORG);
		if (!GetGameData().GetTeamData(nKM_TEAM_TYPE_ORG).IsAssistUnit(cNKMUnit.GetUnitData().m_UnitUID))
		{
			float statFinal = cNKMUnit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_COST_RETURN_RATE);
			if (!(statFinal <= 0f))
			{
				float num = (float)cNKMUnit.m_usedRespawnCost * statFinal;
				myRuntimeTeamData.m_fRespawnCost += num;
				cNKMUnit.m_usedRespawnCost = 0;
				SyncGameEvent(NKM_GAME_EVENT_TYPE.NGET_COST_RETURN, nKM_TEAM_TYPE_ORG, 0, num);
			}
		}
	}

	private void AddStaticBuff(NKMUnit cNKMUnit)
	{
		cNKMUnit.InitAndApplyStaticBuff();
		foreach (NKMUnit item in GetUnitChain())
		{
			item.ApplyStaticBuffToSummonedUnit(cNKMUnit);
		}
		NKMUnitData unitData = cNKMUnit.GetUnitData();
		if (unitData.IsDungeonUnit())
		{
			for (int i = 0; i < unitData.m_DungeonRespawnUnitTemplet.m_listStaticBuffData.Count; i++)
			{
				NKMStaticBuffData nKMStaticBuffData = unitData.m_DungeonRespawnUnitTemplet.m_listStaticBuffData[i];
				if (cNKMUnit.CheckEventCondition(nKMStaticBuffData.m_Condition))
				{
					cNKMUnit.AddBuffByStrID(nKMStaticBuffData.m_BuffStrID, nKMStaticBuffData.m_BuffStatLevel, nKMStaticBuffData.m_BuffTimeLevel, cNKMUnit.GetUnitDataGame().m_GameUnitUID, bUseMasterStat: false, bRangeSon: false);
				}
			}
		}
		foreach (KeyValuePair<int, int> battleConditionID in base.m_NKMGameData.m_BattleConditionIDs)
		{
			int key = battleConditionID.Key;
			int value = battleConditionID.Value;
			NKMBattleConditionTemplet templetByID = NKMBattleConditionManager.GetTempletByID(key);
			if (IsBattleConditionActivated(templetByID))
			{
				if (base.m_NKMGameData.IsPVP() && base.m_NKMGameData.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC)
				{
					AddBuffByBattleCondition(templetByID, cNKMUnit, 1, cNKMUnit.GetTeam());
				}
				else
				{
					AddBuffByBattleCondition(templetByID, cNKMUnit, value);
				}
			}
		}
		AddStaticBuffByOperator(cNKMUnit, base.m_NKMGameData.m_NKMGameTeamDataA);
		AddStaticBuffByOperator(cNKMUnit, base.m_NKMGameData.m_NKMGameTeamDataB);
		NKM_DUNGEON_TYPE dungeonType = GetDungeonType();
		if ((dungeonType == NKM_DUNGEON_TYPE.NDT_RAID || dungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID) && cNKMUnit.IsATeam())
		{
			for (int j = 0; j < base.m_NKMGameData.m_lstTeamABuffStrIDListForRaid.Count; j++)
			{
				string buffStrID = base.m_NKMGameData.m_lstTeamABuffStrIDListForRaid[j];
				cNKMUnit.AddBuffByStrID(buffStrID, 1, 1, cNKMUnit.GetUnitDataGame().m_GameUnitUID, bUseMasterStat: false, bRangeSon: false);
			}
		}
		AddStaticBuffByBanUnit(cNKMUnit);
	}

	private void AddStaticBuffByOperator(NKMUnit cNKMUnitTarget, NKMGameTeamData cNKMGameTeamData)
	{
		if (cNKMGameTeamData != null && cNKMGameTeamData != null && cNKMGameTeamData.m_Operator != null && cNKMGameTeamData.m_Operator.subSkill != null)
		{
			NKMBattleConditionTemplet battleCondTemplet = cNKMGameTeamData.m_Operator.subSkill.GetBattleCondTemplet();
			AddBuffByBattleCondition(battleCondTemplet, cNKMUnitTarget, cNKMGameTeamData.m_Operator.subSkill.level, cNKMGameTeamData.m_eNKM_TEAM_TYPE);
		}
	}

	private void AddStaticBuffByBanUnit(NKMUnit cNKMUnitTarget)
	{
		NKMUnitData unitData = cNKMUnitTarget.GetUnitData();
		if (!IsPVP())
		{
			return;
		}
		switch (GetGameData().m_NKM_GAME_TYPE)
		{
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
			if (cNKMUnitTarget.IsATeam())
			{
				return;
			}
			break;
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
			return;
		}
		if (!IsBanUnit(unitData.m_UnitID))
		{
			return;
		}
		int banUnitLevel = base.m_NKMGameData.GetBanUnitLevel(unitData.m_UnitID);
		foreach (string banUnitBuffStrID in base.m_NKMGameData.m_BanUnitBuffStrIDs)
		{
			cNKMUnitTarget.AddBuffByStrID(banUnitBuffStrID, (byte)banUnitLevel, 1, cNKMUnitTarget.GetUnitDataGame().m_GameUnitUID, bUseMasterStat: false, bRangeSon: false);
		}
	}

	private void AddBuffByBattleCondition(NKMBattleConditionTemplet cNKMBattleConditionTemplet, NKMUnit cNKMUnit, int buffLevel = 1, NKM_TEAM_TYPE usingTeamType = NKM_TEAM_TYPE.NTT_A1)
	{
		if (!IsCorrectStaticBuffTarget(cNKMBattleConditionTemplet, cNKMUnit))
		{
			return;
		}
		NKMUnitDataGame unitDataGame = cNKMUnit.GetUnitDataGame();
		Dictionary<string, int> buffList = cNKMBattleConditionTemplet.GetBuffList(unitDataGame.m_NKM_TEAM_TYPE, usingTeamType);
		if (buffList == null)
		{
			return;
		}
		foreach (KeyValuePair<string, int> item in buffList)
		{
			if (buffLevel < item.Value)
			{
				buffLevel = item.Value;
			}
			cNKMUnit.AddBuffByStrID(item.Key, (byte)buffLevel, (byte)buffLevel, cNKMUnit.GetUnitDataGame().m_GameUnitUID, bUseMasterStat: false, bRangeSon: false);
		}
	}

	protected override void ActivateDelayedBattleConditions(NKMBattleConditionTemplet bcTemplet, int level)
	{
		base.ActivateDelayedBattleConditions(bcTemplet, level);
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnit.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMUnit value = enumerator.Current.Value;
			if (base.m_NKMGameData.IsPVP() && base.m_NKMGameData.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC)
			{
				AddBuffByBattleCondition(bcTemplet, value, level, value.GetTeam());
			}
			else
			{
				AddBuffByBattleCondition(bcTemplet, value, level);
			}
		}
	}

	private bool IsCorrectStaticBuffTarget(NKMBattleConditionTemplet cNKMBattleConditionTemplet, NKMUnit cNKMUnit)
	{
		if (cNKMBattleConditionTemplet == null)
		{
			return false;
		}
		NKMUnitTempletBase unitTempletBase = cNKMUnit.GetUnitTemplet().m_UnitTempletBase;
		switch (unitTempletBase.m_NKM_UNIT_TYPE)
		{
		case NKM_UNIT_TYPE.NUT_SYSTEM:
		case NKM_UNIT_TYPE.NUT_NORMAL:
		{
			int num = (int)(0u | ((cNKMBattleConditionTemplet.AffectCOUNTER && unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_COUNTER)) ? 1u : 0u) | ((cNKMBattleConditionTemplet.AffectSOLDIER && unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_SOLDIER)) ? 1u : 0u) | ((cNKMBattleConditionTemplet.AffectMECHANIC && unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_MECHANIC)) ? 1u : 0u)) | ((cNKMBattleConditionTemplet.AffectCORRUPT && unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED)) ? 1 : 0);
			bool flag = cNKMBattleConditionTemplet.AffectUnitRole == NKM_UNIT_ROLE_TYPE.NURT_INVALID || unitTempletBase.m_NKM_UNIT_ROLE_TYPE == cNKMBattleConditionTemplet.AffectUnitRole;
			bool flag2 = (cNKMUnit.IsAirUnit() ? cNKMBattleConditionTemplet.HitAir : cNKMBattleConditionTemplet.HitLand);
			bool flag3 = true;
			if (!cNKMBattleConditionTemplet.AffectNormal && !unitTempletBase.m_bAwaken)
			{
				flag3 = false;
			}
			bool flag4 = true;
			if (!cNKMBattleConditionTemplet.AffectAwaken && unitTempletBase.m_bAwaken)
			{
				flag4 = false;
			}
			bool result = (byte)((uint)num & (flag ? 1u : 0u) & (flag2 ? 1u : 0u) & (flag3 ? 1u : 0u) & (flag4 ? 1u : 0u)) != 0;
			if (cNKMBattleConditionTemplet.AffectTeamUpID.Contains(unitTempletBase.TeamUp))
			{
				result = true;
			}
			if (cNKMBattleConditionTemplet.hashAffectUnitID.Contains(unitTempletBase.m_UnitID))
			{
				result = true;
			}
			if (cNKMBattleConditionTemplet.hashIgnoreUnitID.Contains(unitTempletBase.m_UnitID))
			{
				result = false;
			}
			if (!NKMEventConditionV2.CheckTempletMacro(cNKMBattleConditionTemplet.m_EventCondition, this, cNKMUnit))
			{
				result = false;
			}
			return result;
		}
		case NKM_UNIT_TYPE.NUT_SHIP:
			if (cNKMBattleConditionTemplet.AffectTeamUpID != null && cNKMBattleConditionTemplet.AffectTeamUpID.Count > 0 && !cNKMBattleConditionTemplet.AffectTeamUpID.Contains(unitTempletBase.TeamUp))
			{
				return false;
			}
			if (!NKMEventConditionV2.CheckTempletMacro(cNKMBattleConditionTemplet.m_EventCondition, this, cNKMUnit))
			{
				return false;
			}
			return cNKMBattleConditionTemplet.AffectSHIP;
		default:
			return false;
		}
	}

	public virtual void Update(float deltaTime)
	{
		if (m_NKMGameRuntimeData.m_bPause)
		{
			return;
		}
		m_fDeltaTime = deltaTime * m_GameSpeed.GetNowValue();
		m_GameSpeed.Update(deltaTime);
		m_ObjectPool.Update();
		switch (m_NKMGameRuntimeData.m_NKM_GAME_STATE)
		{
		case NKM_GAME_STATE.NGS_START:
		case NKM_GAME_STATE.NGS_FINISH:
			m_AbsoluteGameTime += m_fDeltaTime;
			m_NKMGameRuntimeData.m_GameTime += m_fDeltaTime;
			ProcessGameState();
			ProcessUnit();
			SyncDataPackFlush();
			break;
		case NKM_GAME_STATE.NGS_PLAY:
			m_AbsoluteGameTime += m_fDeltaTime;
			if (m_fWorldStopTime <= 0f)
			{
				m_NKMGameRuntimeData.m_GameTime += m_fDeltaTime;
				ProcecssGameTime();
			}
			else
			{
				m_fWorldStopTime -= m_fDeltaTime;
				if (m_fWorldStopTime < 0f)
				{
					m_fWorldStopTime = 0f;
					m_fLastWorldStopFinishedATime = m_AbsoluteGameTime;
				}
			}
			ProcessStopTime();
			ProcecssWin();
			ProcecssValidLand(NKM_TEAM_TYPE.NTT_A1);
			ProcecssRespawnCost();
			ProcessTacticalCommand();
			ProcessDungeonEvent();
			ProcessUnit();
			if (m_fWorldStopTime <= 0f)
			{
				ProcessReAttack();
			}
			m_DEManager.Update(m_fDeltaTime);
			ProcessRespawn();
			ProcessDynamicRespawn();
			ProcessDelayedBattleConditions();
			SyncDataPackFlush();
			break;
		}
	}

	public abstract void ProcessGameState();

	protected void ProcessTacticalCommandTeam(NKMTacticalCommandData cNKMTacticalCommandData, NKMGameTeamData cNKMGameTeamData)
	{
		if (cNKMTacticalCommandData != null && cNKMGameTeamData != null)
		{
			if (cNKMTacticalCommandData.m_ComboCount > 0 && cNKMTacticalCommandData.m_fComboResetCoolTimeNow <= 0f)
			{
				cNKMTacticalCommandData.m_ComboCount = 0;
				SyncGameEvent(NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_FAIL, cNKMGameTeamData.m_eNKM_TEAM_TYPE, cNKMTacticalCommandData.m_TCID, (int)cNKMTacticalCommandData.m_ComboCount);
			}
			if (!cNKMTacticalCommandData.m_bCoolTimeOn && cNKMTacticalCommandData.m_fCoolTimeNow <= 0f)
			{
				cNKMTacticalCommandData.m_bCoolTimeOn = true;
				SyncGameEvent(NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_COOL_TIME_ON, cNKMGameTeamData.m_eNKM_TEAM_TYPE, cNKMTacticalCommandData.m_TCID, 1f);
			}
		}
	}

	protected override void ProcessTacticalCommand()
	{
		base.ProcessTacticalCommand();
		if (GetGameData().m_NKMGameTeamDataA != null)
		{
			for (int i = 0; i < GetGameData().m_NKMGameTeamDataA.m_listTacticalCommandData.Count; i++)
			{
				NKMTacticalCommandData nKMTacticalCommandData = GetGameData().m_NKMGameTeamDataA.m_listTacticalCommandData[i];
				if (nKMTacticalCommandData != null)
				{
					ProcessTacticalCommandTeam(nKMTacticalCommandData, GetGameData().m_NKMGameTeamDataA);
				}
			}
			if (GetWorldStopTime() <= 0f)
			{
				m_NKMReservedTacticalCommandTeamA.Update(m_fDeltaTime);
				if (m_NKMReservedTacticalCommandTeamA.CheckApplyTiming())
				{
					UseReservedTacticalCommand(m_NKMReservedTacticalCommandTeamA);
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
			if (nKMTacticalCommandData2 != null)
			{
				ProcessTacticalCommandTeam(nKMTacticalCommandData2, GetGameData().m_NKMGameTeamDataB);
			}
		}
		if (GetWorldStopTime() <= 0f)
		{
			m_NKMReservedTacticalCommandTeamB.Update(m_fDeltaTime);
			if (m_NKMReservedTacticalCommandTeamB.CheckApplyTiming())
			{
				UseReservedTacticalCommand(m_NKMReservedTacticalCommandTeamB);
			}
		}
	}

	private void UseReservedTacticalCommand(NKMReservedTacticalCommand cNKMReservedTacticalCommand)
	{
		if (cNKMReservedTacticalCommand != null)
		{
			UseTacticalCommand_RealAffect(cNKMReservedTacticalCommand.GetNKMTacticalCommandTemplet(), cNKMReservedTacticalCommand.GetNKMTacticalCommandData(), cNKMReservedTacticalCommand.Get_NKM_TEAM_TYPE());
			NKMTacticalCommandTemplet nKMTacticalCommandTemplet = cNKMReservedTacticalCommand.GetNKMTacticalCommandTemplet();
			NKMTacticalCommandData nKMTacticalCommandData = cNKMReservedTacticalCommand.GetNKMTacticalCommandData();
			if (nKMTacticalCommandTemplet != null && nKMTacticalCommandData != null)
			{
				GetGameData().GetTeamData(cNKMReservedTacticalCommand.Get_NKM_TEAM_TYPE());
				nKMTacticalCommandData.SetActiveTime(nKMTacticalCommandTemplet);
				SyncGameEvent(NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_SKILL_REAL_APPLY_AFTER_SUCCESS, cNKMReservedTacticalCommand.Get_NKM_TEAM_TYPE(), cNKMReservedTacticalCommand.GetNKMTacticalCommandTemplet().m_TCID, (int)cNKMReservedTacticalCommand.GetNKMTacticalCommandData().m_ComboCount);
			}
			cNKMReservedTacticalCommand.Invalidate();
		}
	}

	protected void ProcessWinForCommon()
	{
		if (CheckGameOver(base.m_NKMGameData.m_NKMGameTeamDataA, base.m_NKMGameData.m_NKMGameTeamDataB) || ProcessTimeOut())
		{
			SyncGameStateChange(m_NKMGameRuntimeData.m_NKM_GAME_STATE, m_NKMGameRuntimeData.m_WinTeam);
		}
	}

	protected void ProcessWinForWave()
	{
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY && CheckPossibleNextWave(base.m_NKMGameData.m_NKMGameTeamDataB))
		{
			NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(GetGameData().m_DungeonID);
			if (dungeonTemplet != null)
			{
				int nextWave = dungeonTemplet.GetNextWave(m_NKMGameRuntimeData.m_WaveID);
				if (!dungeonTemplet.CheckValidWave(nextWave))
				{
					SetGameState(NKM_GAME_STATE.NGS_FINISH);
					m_NKMGameRuntimeData.m_WinTeam = NKM_TEAM_TYPE.NTT_A1;
					SyncGameStateChange(m_NKMGameRuntimeData.m_NKM_GAME_STATE, m_NKMGameRuntimeData.m_WinTeam, m_NKMGameRuntimeData.m_WaveID);
				}
			}
		}
		else if (CheckGameOver(base.m_NKMGameData.m_NKMGameTeamDataA, base.m_NKMGameData.m_NKMGameTeamDataB))
		{
			SyncGameStateChange(m_NKMGameRuntimeData.m_NKM_GAME_STATE, m_NKMGameRuntimeData.m_WinTeam, m_NKMGameRuntimeData.m_WaveID);
		}
	}

	protected void ProcessWinForRaid()
	{
		if (CheckGameOver(base.m_NKMGameData.m_NKMGameTeamDataA, base.m_NKMGameData.m_NKMGameTeamDataB))
		{
			SyncGameStateChange(m_NKMGameRuntimeData.m_NKM_GAME_STATE, m_NKMGameRuntimeData.m_WinTeam);
			return;
		}
		bool flag = true;
		if (base.m_NKMGameData.m_NKMGameTeamDataA.m_DeckData.GetListUnitDeckTombCount() < base.m_NKMGameData.m_NKMGameTeamDataA.m_listUnitData.Count)
		{
			flag = false;
		}
		if (flag)
		{
			for (int i = 0; i < m_listNKMGameUnitRespawnData.Count; i++)
			{
				NKMGameUnitRespawnData nKMGameUnitRespawnData = m_listNKMGameUnitRespawnData[i];
				if (base.m_NKMGameData.m_NKMGameTeamDataA.GetUnitDataByUnitUID(nKMGameUnitRespawnData.m_UnitUID) != null)
				{
					flag = false;
					break;
				}
			}
		}
		NKMUnit nKMUnit = null;
		if (flag)
		{
			for (int j = 0; j < m_listNKMGameUnitDynamicRespawnData.Count; j++)
			{
				NKMDynamicRespawnUnitReserve nKMDynamicRespawnUnitReserve = m_listNKMGameUnitDynamicRespawnData[j];
				nKMUnit = GetUnit(nKMDynamicRespawnUnitReserve.m_GameUnitUID, bChain: true, bPool: true);
				if (nKMUnit != null && nKMUnit.IsATeam())
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			List<NKMUnit> unitChain = GetUnitChain();
			for (int k = 0; k < unitChain.Count; k++)
			{
				nKMUnit = unitChain[k];
				if (nKMUnit != null && nKMUnit.IsATeam() && !nKMUnit.IsBoss())
				{
					flag = false;
					break;
				}
			}
		}
		if (flag || ProcessTimeOut())
		{
			SetGameState(NKM_GAME_STATE.NGS_FINISH);
			m_NKMGameRuntimeData.m_WinTeam = NKM_TEAM_TYPE.NTT_B1;
			SyncGameStateChange(m_NKMGameRuntimeData.m_NKM_GAME_STATE, m_NKMGameRuntimeData.m_WinTeam);
		}
	}

	protected virtual void ProcecssWin()
	{
		switch (GetDungeonType())
		{
		case NKM_DUNGEON_TYPE.NDT_WAVE:
			ProcessWinForWave();
			break;
		case NKM_DUNGEON_TYPE.NDT_RAID:
		case NKM_DUNGEON_TYPE.NDT_SOLO_RAID:
			ProcessWinForRaid();
			break;
		default:
			ProcessWinForCommon();
			break;
		}
	}

	protected virtual bool ProcessTimeOut()
	{
		if (GetGameData() == null)
		{
			return false;
		}
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_DEV)
		{
			return false;
		}
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			return false;
		}
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY)
		{
			return false;
		}
		if (base.m_NKMGameData.IsPVE() && !base.m_NKMGameData.m_bLocal && m_NKMGameRuntimeData.m_fRemainGameTime <= 0f)
		{
			SetGameState(NKM_GAME_STATE.NGS_FINISH);
			m_NKMGameRuntimeData.m_WinTeam = NKM_TEAM_TYPE.NTT_B1;
			return true;
		}
		return false;
	}

	public int GetUnitDieCount(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return -1;
		}
		int num = 0;
		for (int i = 0; i < unitData.m_listGameUnitUID.Count; i++)
		{
			short gameUnitUID = unitData.m_listGameUnitUID[i];
			NKMUnit unit = GetUnit(gameUnitUID);
			if (unit != null && (unit.IsDie() || unit.IsDying()))
			{
				num++;
			}
		}
		return num;
	}

	protected virtual bool CheckGameOver(NKMGameTeamData cNKMGameTeamDataA, NKMGameTeamData cNKMGameTeamDataB)
	{
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_END || m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH)
		{
			return false;
		}
		int unitDieCount = GetUnitDieCount(cNKMGameTeamDataA.m_MainShip);
		int unitDieCount2 = GetUnitDieCount(cNKMGameTeamDataB.m_MainShip);
		bool flag = false;
		bool flag2 = false;
		if (unitDieCount != -1 && unitDieCount == cNKMGameTeamDataA.m_MainShip.m_listGameUnitUID.Count)
		{
			flag = true;
		}
		if (unitDieCount2 != -1 && unitDieCount2 == cNKMGameTeamDataB.m_MainShip.m_listGameUnitUID.Count)
		{
			flag2 = true;
		}
		if (!flag && !flag2 && base.m_NKMGameData.IsPVP() && !base.m_NKMGameData.m_bLocal && m_NKMGameRuntimeData.m_fRemainGameTime <= 0f)
		{
			NKMUnit liveBossUnit = GetLiveBossUnit(bTeamA: true);
			NKMUnit liveBossUnit2 = GetLiveBossUnit(bTeamA: false);
			if (liveBossUnit != null && liveBossUnit2 != null)
			{
				if (liveBossUnit.GetHPRate() > liveBossUnit2.GetHPRate())
				{
					flag2 = true;
				}
				else if (liveBossUnit.GetHPRate() < liveBossUnit2.GetHPRate())
				{
					flag = true;
				}
				else
				{
					flag = true;
					flag2 = true;
				}
			}
		}
		if (flag || flag2)
		{
			SetGameState(NKM_GAME_STATE.NGS_FINISH);
			if (flag && flag2)
			{
				m_NKMGameRuntimeData.m_WinTeam = NKM_TEAM_TYPE.NTT_DRAW;
			}
			else
			{
				if (flag)
				{
					m_NKMGameRuntimeData.m_WinTeam = NKM_TEAM_TYPE.NTT_B1;
				}
				else if (flag2)
				{
					m_NKMGameRuntimeData.m_WinTeam = NKM_TEAM_TYPE.NTT_A1;
				}
				Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnit.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NKMUnit value = enumerator.Current.Value;
					if (IsEnemy(m_NKMGameRuntimeData.m_WinTeam, value.GetUnitDataGame().m_NKM_TEAM_TYPE))
					{
						value.GetUnitSyncData().SetHP(0f);
					}
				}
			}
			return true;
		}
		return false;
	}

	protected override void ProcecssRespawnCost()
	{
		base.ProcecssRespawnCost();
		if (m_NKMDungeonTemplet != null && !m_NKMDungeonTemplet.m_bCanUseAuto)
		{
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_bAutoRespawn = false;
		}
		if (m_bUseTurtlingPrevent)
		{
			float gamePlayTime = m_NKMGameRuntimeData.GetGamePlayTime();
			if (!m_bTeamAFirstRespawn && m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost >= 10f)
			{
				if (!m_bTurtleWarningSentA && gamePlayTime >= NKMCommonConst.PVP_AFK_WARNING_TIME)
				{
					SyncGameEvent(NKM_GAME_EVENT_TYPE.NGET_AUTO_RESPAWN_WARNING, NKM_TEAM_TYPE.NTT_A1, 0);
					m_bTurtleWarningSentA = true;
				}
				if (gamePlayTime >= NKMCommonConst.PVP_AFK_AUTO_TIME)
				{
					m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_bAutoRespawn = true;
					SyncGameEvent(NKM_GAME_EVENT_TYPE.NGET_AUTO_RESPAWN_SET, NKM_TEAM_TYPE.NTT_A1, 1);
					m_bTeamAFirstRespawn = true;
				}
			}
			if (!m_bTeamBFirstRespawn && m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost >= 10f)
			{
				if (!m_bTurtleWarningSentB && gamePlayTime >= NKMCommonConst.PVP_AFK_WARNING_TIME)
				{
					SyncGameEvent(NKM_GAME_EVENT_TYPE.NGET_AUTO_RESPAWN_WARNING, NKM_TEAM_TYPE.NTT_B1, 0);
					m_bTurtleWarningSentB = true;
				}
				if (gamePlayTime >= NKMCommonConst.PVP_AFK_AUTO_TIME)
				{
					m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_bAutoRespawn = true;
					SyncGameEvent(NKM_GAME_EVENT_TYPE.NGET_AUTO_RESPAWN_SET, NKM_TEAM_TYPE.NTT_B1, 1);
					m_bTeamBFirstRespawn = true;
				}
			}
		}
		if (m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_bAutoRespawn)
		{
			ProcessAutoRespawn(base.m_NKMGameData.m_NKMGameTeamDataA, m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA);
			ProcessAutoRespawnAssist(base.m_NKMGameData.m_NKMGameTeamDataA, m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA);
		}
		if (!NKMGame.IsPVP(base.m_NKMGameData.m_NKM_GAME_TYPE) && base.m_NKMGameData.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_DEV)
		{
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_bAutoRespawn = true;
		}
		if (m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_bAutoRespawn)
		{
			ProcessAutoRespawn(base.m_NKMGameData.m_NKMGameTeamDataB, m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB);
			ProcessAutoRespawnAssist(base.m_NKMGameData.m_NKMGameTeamDataB, m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB);
		}
	}

	private void GetNextAutoRespawnIndex(NKMGameTeamData cNKMGameTeamData)
	{
		IsThreadSafe();
		float gamePlayTime = m_NKMGameRuntimeData.GetGamePlayTime();
		for (int i = 0; i < cNKMGameTeamData.m_DeckData.GetListUnitDeckCount(); i++)
		{
			cNKMGameTeamData.m_DeckData.SetAutoRespawnIndex((cNKMGameTeamData.m_DeckData.GetAutoRespawnIndex() + 1) % cNKMGameTeamData.m_DeckData.GetListUnitDeckCount());
			long listUnitDeck = cNKMGameTeamData.m_DeckData.GetListUnitDeck(cNKMGameTeamData.m_DeckData.GetAutoRespawnIndex());
			NKMUnitData unitDataByUnitUID = cNKMGameTeamData.GetUnitDataByUnitUID(listUnitDeck);
			if (unitDataByUnitUID != null && IsGameUnitAllDie(listUnitDeck) && CheckRespawnable(unitDataByUnitUID, cNKMGameTeamData.m_eNKM_TEAM_TYPE, gamePlayTime))
			{
				break;
			}
		}
	}

	private void GetNextAutoRespawnIndexAssist(NKMGameTeamData cNKMGameTeamData)
	{
		IsThreadSafe();
		if (cNKMGameTeamData.m_listAssistUnitData.Count == 0 || cNKMGameTeamData.m_DeckData.GetAutoRespawnIndexAssist() == -1)
		{
			return;
		}
		for (int i = 0; i < cNKMGameTeamData.m_listAssistUnitData.Count; i++)
		{
			int num = cNKMGameTeamData.m_DeckData.GetAutoRespawnIndexAssist() + 1;
			if (cNKMGameTeamData.m_listAssistUnitData.Count <= num)
			{
				num = -1;
			}
			cNKMGameTeamData.m_DeckData.SetAutoRespawnIndexAssist(num);
			if (num != -1)
			{
				NKMUnitData assistUnitDataByIndex = cNKMGameTeamData.GetAssistUnitDataByIndex(cNKMGameTeamData.m_DeckData.GetAutoRespawnIndexAssist());
				if (assistUnitDataByIndex != null && assistUnitDataByIndex.m_UnitUID != 0L)
				{
					break;
				}
				continue;
			}
			break;
		}
	}

	private void ProcessAutoRespawn(NKMGameTeamData cNKMGameTeamData, NKMGameRuntimeTeamData cNKMGameRuntimeTeamData)
	{
		IsThreadSafe();
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY && !CheckRespawnCountMax(cNKMGameTeamData.m_eNKM_TEAM_TYPE))
		{
			long listUnitDeck = cNKMGameTeamData.m_DeckData.GetListUnitDeck(cNKMGameTeamData.m_DeckData.GetAutoRespawnIndex());
			NKMUnitData unitDataByUnitUID = cNKMGameTeamData.GetUnitDataByUnitUID(listUnitDeck);
			if (unitDataByUnitUID != null && IsGameUnitAllDie(listUnitDeck) && CheckRespawnable(unitDataByUnitUID, cNKMGameTeamData.m_eNKM_TEAM_TYPE, m_NKMGameRuntimeData.GetGamePlayTime()))
			{
				RespawnUnit(bCheckOnly: false, 0f, 0f, cNKMGameTeamData, cNKMGameRuntimeTeamData, listUnitDeck, -1f, bAutoRespawn: true);
				return;
			}
			GetNextAutoRespawnIndex(cNKMGameTeamData);
			RespawnUnit(bCheckOnly: false, 0f, 0f, cNKMGameTeamData, cNKMGameRuntimeTeamData, listUnitDeck, -1f, bAutoRespawn: true);
		}
	}

	private void ProcessAutoRespawnAssist(NKMGameTeamData cNKMGameTeamData, NKMGameRuntimeTeamData cNKMGameRuntimeTeamData)
	{
		IsThreadSafe();
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY && cNKMGameTeamData.m_DeckData.GetAutoRespawnIndexAssist() >= 0)
		{
			NKMUnitData assistUnitDataByIndex = cNKMGameTeamData.GetAssistUnitDataByIndex(cNKMGameTeamData.m_DeckData.GetAutoRespawnIndexAssist());
			if (assistUnitDataByIndex != null && assistUnitDataByIndex.m_UnitUID != 0L && IsGameUnitAllDie(assistUnitDataByIndex.m_UnitUID))
			{
				RespawnUnit(bCheckOnly: false, 0f, 0f, cNKMGameTeamData, cNKMGameRuntimeTeamData, assistUnitDataByIndex.m_UnitUID, -1f, bAutoRespawn: true);
			}
		}
	}

	private void ProcessDungeonEvent()
	{
		ProcessDungeonEvent(m_listDungeonEventDataTeamA, NKM_TEAM_TYPE.NTT_A1);
		ProcessDungeonEvent(m_listDungeonEventDataTeamB, NKM_TEAM_TYPE.NTT_B1);
		ProcessEventRespawn();
		ProcessDungeonWaveEvent(base.m_NKMGameData.m_NKMGameTeamDataB);
	}

	protected bool CheckPossibleNextWave(NKMGameTeamData cNKMGameTeamData)
	{
		if (m_NKMGameRuntimeData.m_fRemainGameTime <= 0f)
		{
			return true;
		}
		NKMUnit nKMUnit = null;
		for (int i = 0; i < cNKMGameTeamData.m_listEvevtUnitData.Count; i++)
		{
			NKMUnitData cNKMUnitData = cNKMGameTeamData.m_listEvevtUnitData[i];
			if (cNKMUnitData == null || cNKMUnitData.m_DungeonRespawnUnitTemplet == null || (cNKMUnitData.m_DungeonRespawnUnitTemplet.m_WaveID != 0 && cNKMUnitData.m_DungeonRespawnUnitTemplet.m_WaveID != m_NKMGameRuntimeData.m_WaveID))
			{
				continue;
			}
			if (cNKMUnitData.m_fLastRespawnTime <= 0f)
			{
				return false;
			}
			if (m_listNKMGameUnitRespawnData.Any((NKMGameUnitRespawnData e) => e.m_UnitUID == cNKMUnitData.m_UnitUID))
			{
				return false;
			}
			for (int num = 0; num < cNKMUnitData.m_listGameUnitUID.Count; num++)
			{
				short gameUnitUID = cNKMUnitData.m_listGameUnitUID[num];
				nKMUnit = GetUnit(gameUnitUID);
				if (nKMUnit != null && !nKMUnit.IsDie() && !nKMUnit.IsDying())
				{
					return false;
				}
			}
		}
		List<NKMUnit> unitChain = GetUnitChain();
		for (int num2 = 0; num2 < unitChain.Count; num2++)
		{
			nKMUnit = unitChain[num2];
			if (nKMUnit != null && nKMUnit.IsBTeam() && !nKMUnit.IsDyingOrDie())
			{
				return false;
			}
		}
		return true;
	}

	protected virtual void ProcessDungeonWaveEvent(NKMGameTeamData cNKMGameTeamData)
	{
		if (GetDungeonType() != NKM_DUNGEON_TYPE.NDT_WAVE || m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY || !CheckPossibleNextWave(cNKMGameTeamData))
		{
			return;
		}
		NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(GetGameData().m_DungeonID);
		if (dungeonTemplet == null)
		{
			return;
		}
		int nextWave = dungeonTemplet.GetNextWave(m_NKMGameRuntimeData.m_WaveID);
		if (dungeonTemplet.CheckValidWave(nextWave))
		{
			m_NKMGameRuntimeData.m_WaveID = nextWave;
			m_NKMGameRuntimeData.m_PrevWaveEndTime = m_NKMGameRuntimeData.m_GameTime;
			NKMDungeonWaveTemplet waveTemplet = dungeonTemplet.GetWaveTemplet(nextWave);
			if (waveTemplet != null)
			{
				ChangeRemainGameTime(waveTemplet.m_fNextWavetime, delta: false, bSendPacket: false);
			}
			SyncGameStateChange(m_NKMGameRuntimeData.m_NKM_GAME_STATE, m_NKMGameRuntimeData.m_WinTeam, m_NKMGameRuntimeData.m_WaveID);
		}
	}

	private void ProcessDungeonEvent(List<NKMDungeonEventData> listNKMDungeonEventData, NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		for (int i = 0; i < listNKMDungeonEventData.Count; i++)
		{
			NKMDungeonEventData nKMDungeonEventData = listNKMDungeonEventData[i];
			if (nKMDungeonEventData == null)
			{
				continue;
			}
			if (nKMDungeonEventData.m_bEvokeReserved)
			{
				if (nKMDungeonEventData.m_fEventExecuteReserveTime > 0f)
				{
					if (nKMDungeonEventData.m_fEventExecuteReserveTime <= m_NKMGameRuntimeData.m_GameTime)
					{
						ExcuteDungeonEvent(nKMDungeonEventData, eNKM_TEAM_TYPE);
						nKMDungeonEventData.m_bEvokeReserved = false;
					}
				}
				else
				{
					ExcuteDungeonEvent(nKMDungeonEventData, eNKM_TEAM_TYPE);
					nKMDungeonEventData.m_bEvokeReserved = false;
				}
			}
			else if (nKMDungeonEventData.m_DungeonEventTemplet != null && CheckDungeonEventCondition(nKMDungeonEventData, eNKM_TEAM_TYPE) && DungeonEventTimer(nKMDungeonEventData.m_DungeonEventTemplet.m_NKMDungeonEventTiming, eNKM_TEAM_TYPE, nKMDungeonEventData.m_fEventLastStartTime, nKMDungeonEventData.m_fEventLastEndTime))
			{
				if (nKMDungeonEventData.m_DungeonEventTemplet.m_fEventDelay > 0f)
				{
					nKMDungeonEventData.m_fEventExecuteReserveTime = m_NKMGameRuntimeData.m_GameTime + nKMDungeonEventData.m_DungeonEventTemplet.m_fEventDelay;
					nKMDungeonEventData.m_bEvokeReserved = true;
				}
				else
				{
					ExcuteDungeonEvent(nKMDungeonEventData, eNKM_TEAM_TYPE);
				}
			}
		}
	}

	private bool CheckDungeonEventCondition(NKMDungeonEventData cNKMDungeonEventData, NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		switch (cNKMDungeonEventData.m_DungeonEventTemplet.m_EventCondition)
		{
		case NKM_EVENT_START_CONDITION_TYPE.NONE:
			return true;
		case NKM_EVENT_START_CONDITION_TYPE.UNIT_STATE_START:
		case NKM_EVENT_START_CONDITION_TYPE.UNIT_STATE_END:
			return false;
		case NKM_EVENT_START_CONDITION_TYPE.ENEMY_BOSS_HP_RATE_LESS:
		{
			float num2 = ((!IsATeam(eNKM_TEAM_TYPE)) ? GetLiveShipHPRate(NKM_TEAM_TYPE.NTT_A1) : GetLiveShipHPRate(NKM_TEAM_TYPE.NTT_B1));
			return num2 * 100f < (float)cNKMDungeonEventData.m_DungeonEventTemplet.m_EventConditionNumValue;
		}
		case NKM_EVENT_START_CONDITION_TYPE.HAVE_SUMMON_COST:
		{
			float num = ((!IsATeam(eNKM_TEAM_TYPE)) ? m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost : m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost);
			return num >= (float)cNKMDungeonEventData.m_DungeonEventTemplet.m_EventConditionNumValue;
		}
		case NKM_EVENT_START_CONDITION_TYPE.TARGET_ALLY_UNIT_HP_RATE_LESS:
			if (IsSameTeam(eNKM_TEAM_TYPE, NKM_TEAM_TYPE.NTT_A1))
			{
				return CheckLiveUnitHPRate(cNKMDungeonEventData.EventConditionCache1, cNKMDungeonEventData.m_DungeonEventTemplet.m_EventConditionNumValue, NKM_TEAM_TYPE.NTT_A1);
			}
			if (IsSameTeam(eNKM_TEAM_TYPE, NKM_TEAM_TYPE.NTT_B1))
			{
				return CheckLiveUnitHPRate(cNKMDungeonEventData.EventConditionCache1, cNKMDungeonEventData.m_DungeonEventTemplet.m_EventConditionNumValue, NKM_TEAM_TYPE.NTT_B1);
			}
			return false;
		case NKM_EVENT_START_CONDITION_TYPE.TARGET_ENEMY_UNIT_HP_RATE_LESS:
			if (IsSameTeam(eNKM_TEAM_TYPE, NKM_TEAM_TYPE.NTT_A1))
			{
				return CheckLiveUnitHPRate(cNKMDungeonEventData.EventConditionCache1, cNKMDungeonEventData.m_DungeonEventTemplet.m_EventConditionNumValue, NKM_TEAM_TYPE.NTT_B1);
			}
			if (IsSameTeam(eNKM_TEAM_TYPE, NKM_TEAM_TYPE.NTT_B1))
			{
				return CheckLiveUnitHPRate(cNKMDungeonEventData.EventConditionCache1, cNKMDungeonEventData.m_DungeonEventTemplet.m_EventConditionNumValue, NKM_TEAM_TYPE.NTT_A1);
			}
			return false;
		default:
			return false;
		}
	}

	private bool CheckLiveUnitHPRate(int unitID, int intHpRate, NKM_TEAM_TYPE team)
	{
		float num = (float)intHpRate / 100f;
		foreach (NKMUnit item in m_listNKMUnit)
		{
			if (item.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY && item.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_ENV && IsSameTeam(item.GetUnitDataGame().m_NKM_TEAM_TYPE, team) && item.GetUnitData().m_UnitID == unitID && item.GetHPRate() < num)
			{
				return true;
			}
		}
		return false;
	}

	private void OnUnitStateChangeEvent(NKM_UNIT_STATE_CHANGE_TYPE stateChangeType, NKMUnit unit, NKMUnitState unitState)
	{
		if (unit == null || unitState == null)
		{
			return;
		}
		NKM_TEAM_TYPE nKM_TEAM_TYPE = unit.GetUnitDataGame().m_NKM_TEAM_TYPE;
		List<NKMDungeonEventData> list = null;
		if (IsATeam(nKM_TEAM_TYPE))
		{
			list = m_listDungeonEventDataTeamA;
		}
		else if (IsBTeam(nKM_TEAM_TYPE))
		{
			list = m_listDungeonEventDataTeamB;
		}
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			NKMDungeonEventData nKMDungeonEventData = list[i];
			if (nKMDungeonEventData.m_bEvokeReserved || nKMDungeonEventData.EventConditionCache1 != unit.GetUnitData().m_UnitID)
			{
				continue;
			}
			NKMDungeonEventTemplet dungeonEventTemplet = nKMDungeonEventData.m_DungeonEventTemplet;
			if (dungeonEventTemplet == null)
			{
				continue;
			}
			if (stateChangeType != NKM_UNIT_STATE_CHANGE_TYPE.NUSCT_START)
			{
				if (stateChangeType != NKM_UNIT_STATE_CHANGE_TYPE.NUSCT_END || dungeonEventTemplet.m_EventCondition != NKM_EVENT_START_CONDITION_TYPE.UNIT_STATE_END)
				{
					continue;
				}
			}
			else if (dungeonEventTemplet.m_EventCondition != NKM_EVENT_START_CONDITION_TYPE.UNIT_STATE_START)
			{
				continue;
			}
			if (!(unitState.m_StateName != dungeonEventTemplet.m_EventConditionValue2) && DungeonEventTimer(dungeonEventTemplet.m_NKMDungeonEventTiming, nKM_TEAM_TYPE, nKMDungeonEventData.m_fEventLastStartTime, nKMDungeonEventData.m_fEventLastEndTime))
			{
				nKMDungeonEventData.m_bEvokeReserved = true;
				if (nKMDungeonEventData.m_DungeonEventTemplet.m_fEventDelay > 0f)
				{
					nKMDungeonEventData.m_fEventExecuteReserveTime = m_NKMGameRuntimeData.m_GameTime + nKMDungeonEventData.m_DungeonEventTemplet.m_fEventDelay;
				}
			}
		}
	}

	protected virtual void ExcuteDungeonEvent(NKMDungeonEventData cNKMDungeonEventData, NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		cNKMDungeonEventData.m_fEventLastStartTime = m_NKMGameRuntimeData.m_GameTime;
		cNKMDungeonEventData.m_fEventLastEndTime = m_NKMGameRuntimeData.m_GameTime;
		cNKMDungeonEventData.m_fEventExecuteReserveTime = 0f;
		bool flag = false;
		bool bRecordToGameData = NKMDungeonEventTemplet.IsPermanent(cNKMDungeonEventData.m_DungeonEventTemplet.m_dungeonEventType);
		switch (cNKMDungeonEventData.m_DungeonEventTemplet.m_dungeonEventType)
		{
		case NKM_EVENT_ACTION_TYPE.GAME_EVENT:
		case NKM_EVENT_ACTION_TYPE.UNLOCK_TUTORIAL_GAME_RE_RESPAWN:
		case NKM_EVENT_ACTION_TYPE.CHANGE_BGM:
		case NKM_EVENT_ACTION_TYPE.CHANGE_BGM_TRACK:
		case NKM_EVENT_ACTION_TYPE.HUD_ALERT:
		case NKM_EVENT_ACTION_TYPE.POPUP_MESSAGE:
		case NKM_EVENT_ACTION_TYPE.MAP_ANIMATION:
		case NKM_EVENT_ACTION_TYPE.CHANGE_CAMERA:
			flag = true;
			break;
		case NKM_EVENT_ACTION_TYPE.SET_ENEMY_BOSS_HP_RATE:
			((!IsATeam(eNKM_TEAM_TYPE)) ? GetLiveBossUnit(bTeamA: true) : GetLiveBossUnit(bTeamA: false))?.SetHPRate((float)cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue / 100f);
			break;
		case NKM_EVENT_ACTION_TYPE.SET_UNIT_HYPER_FULL:
			flag = true;
			if (cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue != 0)
			{
				NKMUnit unitByUnitID = GetUnitByUnitID(cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue, bChain: true, bPool: true);
				if (unitByUnitID != null)
				{
					List<NKMAttackStateData> listHyperSkillStateData = unitByUnitID.GetUnitTemplet().m_listHyperSkillStateData;
					for (int i = 0; i < listHyperSkillStateData.Count; i++)
					{
						unitByUnitID.SetStateCoolTime(listHyperSkillStateData[i].m_StateName, bMax: false, 0f);
					}
				}
				break;
			}
			foreach (KeyValuePair<short, NKMUnit> item in m_dicNKMUnit)
			{
				NKMUnit value2 = item.Value;
				List<NKMAttackStateData> listHyperSkillStateData2 = value2.GetUnitTemplet().m_listHyperSkillStateData;
				for (int j = 0; j < listHyperSkillStateData2.Count; j++)
				{
					value2.SetStateCoolTime(listHyperSkillStateData2[j].m_StateName, bMax: false, 0f);
				}
			}
			foreach (KeyValuePair<short, NKMUnit> item2 in m_dicNKMUnitPool)
			{
				NKMUnit value3 = item2.Value;
				List<NKMAttackStateData> listHyperSkillStateData3 = value3.GetUnitTemplet().m_listHyperSkillStateData;
				for (int k = 0; k < listHyperSkillStateData3.Count; k++)
				{
					value3.SetStateCoolTime(listHyperSkillStateData3[k].m_StateName, bMax: false, 0f);
				}
			}
			break;
		case NKM_EVENT_ACTION_TYPE.SET_UNIT_STATE:
		case NKM_EVENT_ACTION_TYPE.SET_ENEMY_UNIT_STATE:
		{
			NKM_TEAM_TYPE nKM_TEAM_TYPE = eNKM_TEAM_TYPE;
			if (cNKMDungeonEventData.m_DungeonEventTemplet.m_dungeonEventType == NKM_EVENT_ACTION_TYPE.SET_ENEMY_UNIT_STATE)
			{
				nKM_TEAM_TYPE = ((!IsSameTeam(eNKM_TEAM_TYPE, NKM_TEAM_TYPE.NTT_A1)) ? NKM_TEAM_TYPE.NTT_A1 : NKM_TEAM_TYPE.NTT_B1);
			}
			if (cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue != 0)
			{
				NKMUnit unitByUnitID2 = GetUnitByUnitID(cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue, nKM_TEAM_TYPE);
				if (unitByUnitID2 != null && !unitByUnitID2.IsDying() && !unitByUnitID2.IsDie() && unitByUnitID2.GetUnitState(cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionStrValue) != null)
				{
					unitByUnitID2.StateChange(cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionStrValue, bForceChange: true, bImmediate: true);
				}
				break;
			}
			foreach (KeyValuePair<short, NKMUnit> item3 in m_dicNKMUnit)
			{
				NKMUnit value4 = item3.Value;
				if (value4.IsAlly(nKM_TEAM_TYPE) && !value4.IsBoss() && value4 != null && !value4.IsDying() && !value4.IsDie() && value4.GetUnitState(cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionStrValue) != null)
				{
					value4.StateChange(cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionStrValue, bForceChange: true, bImmediate: true);
				}
			}
			break;
		}
		case NKM_EVENT_ACTION_TYPE.KILL_ALL_TAGGED_UNIT:
		{
			Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnit.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NKMUnit value = enumerator.Current.Value;
				if (value.GetUnitData().m_DungeonRespawnUnitTemplet != null && value.GetUnitData().m_DungeonRespawnUnitTemplet.m_EventRespawnUnitTag == cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionStrValue)
				{
					value.GetUnitSyncData().SetHP(0f);
					value.SetDying(bForce: true);
				}
			}
			break;
		}
		case NKM_EVENT_ACTION_TYPE.ADD_EVENTTAG:
			AddEventTag(cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionStrValue, cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue);
			break;
		case NKM_EVENT_ACTION_TYPE.SET_EVENTTAG:
			SetEventTag(cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionStrValue, cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue);
			break;
		case NKM_EVENT_ACTION_TYPE.NEAT_RESPAWN_COST_A_TEAM:
			flag = true;
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost = cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue;
			if (m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost > 10f)
			{
				m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost = 10f;
			}
			if (m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost < 0f)
			{
				m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost = 0f;
			}
			break;
		case NKM_EVENT_ACTION_TYPE.NEAT_RESPAWN_COST_B_TEAM:
			flag = true;
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost = cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue;
			if (m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost > 10f)
			{
				m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost = 10f;
			}
			if (m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost < 0f)
			{
				m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost = 0f;
			}
			break;
		case NKM_EVENT_ACTION_TYPE.ADD_TEAM_A_EXTRA_RESPAWN_COST:
			base.m_NKMGameData.fExtraRespawnCostAddForA += (float)cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue * 0.01f;
			flag = true;
			break;
		case NKM_EVENT_ACTION_TYPE.ADD_TEAM_B_EXTRA_RESPAWN_COST:
			base.m_NKMGameData.fExtraRespawnCostAddForB += (float)cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue * 0.01f;
			flag = true;
			break;
		case NKM_EVENT_ACTION_TYPE.FORCE_WIN:
			ForceWin(eNKM_TEAM_TYPE, cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue == 0);
			break;
		case NKM_EVENT_ACTION_TYPE.FORCE_LOSE:
			if (IsATeam(eNKM_TEAM_TYPE))
			{
				ForceWin(NKM_TEAM_TYPE.NTT_B1, cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue == 0);
			}
			else
			{
				ForceWin(NKM_TEAM_TYPE.NTT_A1, cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue == 0);
			}
			break;
		case NKM_EVENT_ACTION_TYPE.SET_GAME_TIME:
			ChangeRemainGameTime(cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue, delta: false, bSendPacket: true);
			break;
		case NKM_EVENT_ACTION_TYPE.DELTA_GAME_TIME:
			ChangeRemainGameTime(cNKMDungeonEventData.m_DungeonEventTemplet.m_EventActionValue, delta: true, bSendPacket: true);
			break;
		}
		if (flag)
		{
			if (cNKMDungeonEventData.m_DungeonEventTemplet.m_bPause)
			{
				ForceSyncDataPackFlushThisFrame();
			}
			SyncDungeonEvent(cNKMDungeonEventData.m_DungeonEventTemplet, eNKM_TEAM_TYPE, bRecordToGameData);
		}
	}

	protected void ForceWin(NKM_TEAM_TYPE winTeam, bool bKillBoss)
	{
		if (GetDungeonType() == NKM_DUNGEON_TYPE.NDT_WAVE)
		{
			if (winTeam == NKM_TEAM_TYPE.NTT_A1)
			{
				m_NKMGameRuntimeData.m_WaveID = 0;
				AllKill(NKM_TEAM_TYPE.NTT_B1);
			}
			else
			{
				m_NKMGameRuntimeData.m_WaveID = 0;
				m_NKMGameRuntimeData.m_fRemainGameTime = 0f;
			}
		}
		else if (bKillBoss)
		{
			GetLiveBossUnit(winTeam != NKM_TEAM_TYPE.NTT_A1)?.SetHPRate(0f);
		}
		else
		{
			SetGameState(NKM_GAME_STATE.NGS_FINISH);
			m_NKMGameRuntimeData.m_WinTeam = winTeam;
			SyncGameStateChange(m_NKMGameRuntimeData.m_NKM_GAME_STATE, m_NKMGameRuntimeData.m_WinTeam);
		}
	}

	private void ProcessEventRespawn()
	{
		ProcessEventRespawn(base.m_NKMGameData.m_NKMGameTeamDataA);
		ProcessEventRespawn(base.m_NKMGameData.m_NKMGameTeamDataB);
	}

	protected bool CheckRespawnable(NKMUnitData cNKMUnitData, NKM_TEAM_TYPE teamType, float gamePlayTime)
	{
		NKMDungeonRespawnUnitTemplet dungeonRespawnUnitTemplet = cNKMUnitData.m_DungeonRespawnUnitTemplet;
		if (dungeonRespawnUnitTemplet == null)
		{
			return true;
		}
		return DungeonEventTimer(dungeonRespawnUnitTemplet.m_NKMDungeonEventTiming, teamType, cNKMUnitData.m_fLastRespawnTime, cNKMUnitData.m_fLastDieTime);
	}

	protected bool DungeonEventTimer(NKMDungeonEventTiming cNKMDungeonEventTiming, NKM_TEAM_TYPE eNKM_TEAM_TYPE, float fLastEventTime, float fLastEventEndTime)
	{
		if (m_NKMGameRuntimeData.m_PrevWaveEndTime > 0f)
		{
			if (!cNKMDungeonEventTiming.EventTimeCheck(m_NKMGameRuntimeData.m_GameTime - m_NKMGameRuntimeData.m_PrevWaveEndTime))
			{
				return false;
			}
		}
		else if (!cNKMDungeonEventTiming.EventTimeCheck(m_NKMGameRuntimeData.GetGamePlayTime()))
		{
			return false;
		}
		if (cNKMDungeonEventTiming.m_fEventBossHPLess > 0f && GetLiveShipHPRate(eNKM_TEAM_TYPE) >= cNKMDungeonEventTiming.m_fEventBossHPLess)
		{
			return false;
		}
		if (cNKMDungeonEventTiming.m_fEventBossHPUpper > 0f && GetLiveShipHPRate(eNKM_TEAM_TYPE) <= cNKMDungeonEventTiming.m_fEventBossHPUpper)
		{
			return false;
		}
		NKMUnit liveBossUnit = GetLiveBossUnit(eNKM_TEAM_TYPE);
		if (liveBossUnit != null && cNKMDungeonEventTiming.m_fEventBossHPLess > 0f && cNKMDungeonEventTiming.m_fEventIgnoreBossInitHPLess && cNKMDungeonEventTiming.m_NKM_DUNGEON_EVENT_TYPE == NKM_DUNGEON_EVENT_TYPE.NDET_ONE_TIME && liveBossUnit.GetUnitFrameData().m_fInitHP <= liveBossUnit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP) * cNKMDungeonEventTiming.m_fEventBossHPLess)
		{
			return false;
		}
		if (cNKMDungeonEventTiming.m_EventDieUnitTagCount > 0 && cNKMDungeonEventTiming.m_EventDieUnitTagCount > GetDieUnitRespawnTag(cNKMDungeonEventTiming.m_EventDieUnitTag))
		{
			return false;
		}
		if (cNKMDungeonEventTiming.m_EventDieDeckTagCount > 0 && cNKMDungeonEventTiming.m_EventDieDeckTagCount > GetDieDeckRespawnTag(cNKMDungeonEventTiming.m_EventDieDeckTag))
		{
			return false;
		}
		if (cNKMDungeonEventTiming.m_EventTagCount > 0 && cNKMDungeonEventTiming.m_EventTagCount > GetEventTag(cNKMDungeonEventTiming.m_EventTag))
		{
			return false;
		}
		if (cNKMDungeonEventTiming.m_EventLiveDeckTag.Length > 0 && GetDieDeckRespawnTag(cNKMDungeonEventTiming.m_EventLiveDeckTag) > 0)
		{
			return false;
		}
		if (base.m_NKMGameData.IsPVE())
		{
			if (cNKMDungeonEventTiming.m_EventDieWarfareDungeonTag.Length > 1 && CheckWarfareDungeonExist(cNKMDungeonEventTiming.m_EventDieWarfareDungeonTag))
			{
				return false;
			}
			if (cNKMDungeonEventTiming.m_EventLiveWarfareDungeonTag.Length > 1 && !CheckWarfareDungeonExist(cNKMDungeonEventTiming.m_EventLiveWarfareDungeonTag))
			{
				return false;
			}
		}
		switch (cNKMDungeonEventTiming.m_NKM_DUNGEON_EVENT_TYPE)
		{
		case NKM_DUNGEON_EVENT_TYPE.NDET_ONE_TIME:
			if (fLastEventTime >= 0f)
			{
				return false;
			}
			break;
		case NKM_DUNGEON_EVENT_TYPE.NDET_TIME:
			if (m_NKMGameRuntimeData.m_GameTime - fLastEventTime < cNKMDungeonEventTiming.m_fEventTimeGap && fLastEventTime > 0f)
			{
				return false;
			}
			break;
		case NKM_DUNGEON_EVENT_TYPE.NDET_DIE_AFTER_TIME:
			if (m_NKMGameRuntimeData.m_GameTime - fLastEventEndTime < cNKMDungeonEventTiming.m_fEventTimeGap && fLastEventTime > 0f)
			{
				return false;
			}
			break;
		}
		return true;
	}

	protected virtual bool CheckWarfareDungeonExist(string dungeonTag)
	{
		return false;
	}

	private void ProcessEventRespawn(NKMGameTeamData cNKMGameTeamData)
	{
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY)
		{
			return;
		}
		for (int i = 0; i < cNKMGameTeamData.m_listEvevtUnitData.Count; i++)
		{
			NKMUnitData nKMUnitData = cNKMGameTeamData.m_listEvevtUnitData[i];
			if (nKMUnitData == null || nKMUnitData.m_DungeonRespawnUnitTemplet == null || (nKMUnitData.m_DungeonRespawnUnitTemplet.m_WaveID != 0 && nKMUnitData.m_DungeonRespawnUnitTemplet.m_WaveID != m_NKMGameRuntimeData.m_WaveID))
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < m_listNKMGameUnitRespawnData.Count; j++)
			{
				if (m_listNKMGameUnitRespawnData[j].m_UnitUID == nKMUnitData.m_UnitUID)
				{
					flag = true;
					break;
				}
			}
			if (flag || !IsGameUnitAllDie(nKMUnitData.m_UnitUID) || !DungeonEventTimer(nKMUnitData.m_DungeonRespawnUnitTemplet.m_NKMDungeonEventTiming, cNKMGameTeamData.m_eNKM_TEAM_TYPE, nKMUnitData.m_fLastRespawnTime, nKMUnitData.m_fLastDieTime))
			{
				continue;
			}
			NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(nKMUnitData.m_UnitID);
			if (unitTemplet == null)
			{
				continue;
			}
			for (int k = 0; k < unitTemplet.m_StatTemplet.m_RespawnCount; k++)
			{
				NKMGameUnitRespawnData nKMGameUnitRespawnData = (NKMGameUnitRespawnData)m_ObjectPool.OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMGameUnitRespawnData);
				nKMGameUnitRespawnData.m_UnitUID = nKMUnitData.m_UnitUID;
				nKMGameUnitRespawnData.m_fRespawnCoolTime = nKMUnitData.m_DungeonRespawnUnitTemplet.m_fRespawnCoolTime + unitTemplet.m_fRespawnCoolTime * (float)k;
				nKMGameUnitRespawnData.m_fRollbackTime = 0f;
				if (IsATeam(cNKMGameTeamData.m_eNKM_TEAM_TYPE))
				{
					nKMGameUnitRespawnData.m_fRespawnPosX = m_NKMMapTemplet.m_fMinX + (m_NKMMapTemplet.m_fMaxX - m_NKMMapTemplet.m_fMinX) * nKMUnitData.m_DungeonRespawnUnitTemplet.m_NKMDungeonEventTiming.m_fEventPos;
				}
				else
				{
					nKMGameUnitRespawnData.m_fRespawnPosX = m_NKMMapTemplet.m_fMaxX - (m_NKMMapTemplet.m_fMaxX - m_NKMMapTemplet.m_fMinX) * nKMUnitData.m_DungeonRespawnUnitTemplet.m_NKMDungeonEventTiming.m_fEventPos;
				}
				m_listNKMGameUnitRespawnData.Add(nKMGameUnitRespawnData);
			}
			nKMUnitData.m_fLastRespawnTime = m_NKMGameRuntimeData.m_GameTime;
		}
	}

	private bool RespawnUnit(bool bCheckOnly, float fBaseCoolTime, float rollbackTime, NKMGameTeamData cNKMGameTeamData, NKMGameRuntimeTeamData cNKMGameRuntimeTeamData, long unitUID, float fRespawnPosX = -1f, bool bAutoRespawn = false)
	{
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY)
		{
			return false;
		}
		NKMUnitData unitDataByUnitUID = cNKMGameTeamData.GetUnitDataByUnitUID(unitUID);
		if (unitDataByUnitUID == null)
		{
			return false;
		}
		NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(unitDataByUnitUID.m_UnitID);
		if (unitTemplet == null)
		{
			return false;
		}
		if (!CheckRespawnable(unitDataByUnitUID, cNKMGameTeamData.m_eNKM_TEAM_TYPE, m_NKMGameRuntimeData.GetGamePlayTime()))
		{
			return false;
		}
		for (int i = 0; i < m_listNKMGameUnitRespawnData.Count; i++)
		{
			if (m_listNKMGameUnitRespawnData[i].m_UnitUID == unitUID)
			{
				return false;
			}
		}
		int num = ((cNKMGameTeamData.m_LeaderUnitUID != unitUID) ? GetRespawnCost(unitTemplet.m_StatTemplet, bLeader: false, cNKMGameTeamData.m_eNKM_TEAM_TYPE) : Math.Max(0, GetRespawnCost(unitTemplet.m_StatTemplet, bLeader: true, cNKMGameTeamData.m_eNKM_TEAM_TYPE)));
		if (!bCheckOnly && !IsGameUnitAllDie(unitUID))
		{
			return false;
		}
		if (cNKMGameTeamData.IsOperatorUnit(unitUID))
		{
			if (bCheckOnly)
			{
				return true;
			}
			ProcessUnitRespawnCount(unitUID, unitDataByUnitUID, unitTemplet, cNKMGameTeamData, fRespawnPosX, fBaseCoolTime, rollbackTime);
			return true;
		}
		if (cNKMGameTeamData.IsAssistUnit(unitUID))
		{
			if (cNKMGameRuntimeTeamData.m_fRespawnCostAssist < (float)num)
			{
				return false;
			}
			if (bCheckOnly)
			{
				return true;
			}
			ProcessUnitRespawnCount(unitUID, unitDataByUnitUID, unitTemplet, cNKMGameTeamData, fRespawnPosX, fBaseCoolTime, rollbackTime);
			if (bAutoRespawn)
			{
				GetNextAutoRespawnIndexAssist(cNKMGameTeamData);
			}
			cNKMGameRuntimeTeamData.m_fRespawnCostAssist = 0f;
			SyncDeckChangeAssist(cNKMGameTeamData.m_eNKM_TEAM_TYPE, rollbackTime, cNKMGameTeamData.m_DeckData.GetAutoRespawnIndexAssist());
			if (m_dicRespawnUnitUID_ByDeckUsed != null && !m_dicRespawnUnitUID_ByDeckUsed.ContainsKey(unitUID))
			{
				m_dicRespawnUnitUID_ByDeckUsed.Add(unitUID, num);
			}
			return true;
		}
		if (cNKMGameRuntimeTeamData.m_fRespawnCost < (float)num)
		{
			return false;
		}
		if (bCheckOnly)
		{
			return true;
		}
		ProcessUnitRespawnCount(unitUID, unitDataByUnitUID, unitTemplet, cNKMGameTeamData, fRespawnPosX, fBaseCoolTime, rollbackTime);
		if (bAutoRespawn)
		{
			GetNextAutoRespawnIndex(cNKMGameTeamData);
		}
		AddRespawnUnitsToLiveUnit();
		UseDeck(cNKMGameTeamData, unitUID, rollbackTime);
		cNKMGameRuntimeTeamData.m_fRespawnCost -= num;
		cNKMGameRuntimeTeamData.m_fUsedRespawnCost += num;
		cNKMGameRuntimeTeamData.m_respawn_count++;
		if (unitDataByUnitUID.m_listGameUnitUID.Count > 0)
		{
			short gameUnitUID = unitDataByUnitUID.m_listGameUnitUID[0];
			NKMUnit unit = GetUnit(gameUnitUID, bChain: true, bPool: true);
			if (unit != null)
			{
				unit.m_usedRespawnCost = num;
			}
		}
		if (m_dicRespawnUnitUID_ByDeckUsed != null && !m_dicRespawnUnitUID_ByDeckUsed.ContainsKey(unitUID))
		{
			m_dicRespawnUnitUID_ByDeckUsed.Add(unitUID, num);
		}
		return true;
	}

	private void ProcessUnitRespawnCount(long unitUID, NKMUnitData cNKMUnitData, NKMUnitTemplet cNKMUnitTemplet, NKMGameTeamData cNKMGameTeamData, float fRespawnPosX, float fBaseCoolTime, float rollbackTime = 0f)
	{
		for (int i = 0; i < cNKMUnitTemplet.m_StatTemplet.m_RespawnCount; i++)
		{
			NKMGameUnitRespawnData nKMGameUnitRespawnData = (NKMGameUnitRespawnData)m_ObjectPool.OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMGameUnitRespawnData);
			nKMGameUnitRespawnData.m_UnitUID = unitUID;
			nKMGameUnitRespawnData.m_fRespawnPosX = fRespawnPosX;
			nKMGameUnitRespawnData.m_eNKM_TEAM_TYPE = cNKMGameTeamData.m_eNKM_TEAM_TYPE;
			float num = cNKMUnitTemplet.m_fRespawnCoolTime * (float)i - rollbackTime;
			if (num >= 0f)
			{
				nKMGameUnitRespawnData.m_fRespawnCoolTime = fBaseCoolTime + num;
				nKMGameUnitRespawnData.m_fRollbackTime = 0f;
			}
			else
			{
				nKMGameUnitRespawnData.m_fRespawnCoolTime = fBaseCoolTime;
				nKMGameUnitRespawnData.m_fRollbackTime = 0f - num;
			}
			if (fRespawnPosX.IsNearlyEqual(-1f) && cNKMUnitData.m_DungeonRespawnUnitTemplet != null)
			{
				if (IsATeam(cNKMGameTeamData.m_eNKM_TEAM_TYPE))
				{
					nKMGameUnitRespawnData.m_fRespawnPosX = m_NKMMapTemplet.m_fMinX + (m_NKMMapTemplet.m_fMaxX - m_NKMMapTemplet.m_fMinX) * cNKMUnitData.m_DungeonRespawnUnitTemplet.m_NKMDungeonEventTiming.m_fEventPos;
				}
				else
				{
					nKMGameUnitRespawnData.m_fRespawnPosX = m_NKMMapTemplet.m_fMaxX - (m_NKMMapTemplet.m_fMaxX - m_NKMMapTemplet.m_fMinX) * cNKMUnitData.m_DungeonRespawnUnitTemplet.m_NKMDungeonEventTiming.m_fEventPos;
				}
			}
			else
			{
				nKMGameUnitRespawnData.m_fRespawnPosX = fRespawnPosX;
			}
			m_listNKMGameUnitRespawnData.Add(nKMGameUnitRespawnData);
		}
	}

	protected void AddComboByDeckUnitRespawn(NKMGameTeamData cNKMGameTeamData, NKMGameRuntimeTeamData cNKMGameRuntimeTeamData, int unitID, int respawnCost)
	{
		NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(unitID);
		if (cNKMGameTeamData == null || unitTemplet == null || unitTemplet.m_UnitTempletBase == null)
		{
			return;
		}
		NKMTacticalCommandData tC_Combo = cNKMGameTeamData.GetTC_Combo();
		if (tC_Combo == null || tC_Combo.m_fCoolTimeNow > 0f)
		{
			return;
		}
		NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(tC_Combo.m_TCID);
		if (tacticalCommandTempletByID == null || tC_Combo.m_ComboCount >= tacticalCommandTempletByID.m_listComboType.Count)
		{
			return;
		}
		NKMTacticalCombo nKMTacticalCombo = tacticalCommandTempletByID.m_listComboType[tC_Combo.m_ComboCount];
		if (nKMTacticalCombo != null && nKMTacticalCombo.CheckCond(unitTemplet.m_UnitTempletBase, respawnCost))
		{
			tC_Combo.AddComboCount();
			if (tacticalCommandTempletByID.m_listComboType.Count <= tC_Combo.m_ComboCount)
			{
				UseTacticalCommand(tacticalCommandTempletByID, tC_Combo, cNKMGameRuntimeTeamData, cNKMGameTeamData.m_eNKM_TEAM_TYPE, NKMCommonConst.OPERATOR_SKILL_DELAY_START_TIME);
				return;
			}
			tC_Combo.m_fComboResetCoolTimeNow = tacticalCommandTempletByID.m_fComboResetCoolTime;
			SyncGameEvent(NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_SUCCESS, cNKMGameTeamData.m_eNKM_TEAM_TYPE, tacticalCommandTempletByID.m_TCID, (int)tC_Combo.m_ComboCount);
		}
	}

	protected long UseDeck(NKMGameTeamData cNKMGameTeamData, long unitUID, float rollbackTime)
	{
		int num = 0;
		long deckTombAddUnitUID = -1L;
		cNKMGameTeamData.m_DeckData.DecreaseRespawnLimitCount(unitUID);
		bool flag = true;
		if (GetDungeonTemplet() != null && !GetDungeonTemplet().m_DungeonTempletBase.m_bDeckReuse)
		{
			flag = false;
		}
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_RAID || GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_RAID_SOLO || !cNKMGameTeamData.m_DeckData.IsRespawnLimitCountLeft(unitUID) || !flag)
		{
			cNKMGameTeamData.m_DeckData.AddListUnitDeckTomb(unitUID);
			deckTombAddUnitUID = unitUID;
		}
		else
		{
			cNKMGameTeamData.m_DeckData.AddListUnitDeckUsed(unitUID);
		}
		for (num = 0; num < cNKMGameTeamData.m_DeckData.GetListUnitDeckCount(); num++)
		{
			if (cNKMGameTeamData.m_DeckData.GetListUnitDeck(num) == unitUID)
			{
				cNKMGameTeamData.m_DeckData.SetListUnitDeck(num, 0L);
				break;
			}
		}
		long num2 = 0L;
		if (cNKMGameTeamData.m_DeckData.GetNextDeck() != 0L)
		{
			num2 = cNKMGameTeamData.m_DeckData.GetNextDeck();
			cNKMGameTeamData.m_DeckData.SetListUnitDeck(num, num2);
			cNKMGameTeamData.m_DeckData.SetNextDeck(0L);
		}
		int num3 = 0;
		int num4 = -1;
		for (num3 = 0; num3 < cNKMGameTeamData.m_DeckData.GetListUnitDeckUsedCount(); num3++)
		{
			long listUnitDeckUsed = cNKMGameTeamData.m_DeckData.GetListUnitDeckUsed(num3);
			if (listUnitDeckUsed > 0 && IsGameUnitAllDie(listUnitDeckUsed))
			{
				num4 = num3;
				break;
			}
			if (listUnitDeckUsed > 0)
			{
				num4 = num3;
				break;
			}
		}
		if (num4 >= 0)
		{
			long listUnitDeckUsed2 = cNKMGameTeamData.m_DeckData.GetListUnitDeckUsed(num4);
			if (cNKMGameTeamData.m_DeckData.GetListUnitDeck(num) == 0L)
			{
				cNKMGameTeamData.m_DeckData.SetListUnitDeck(num, listUnitDeckUsed2);
				num2 = listUnitDeckUsed2;
			}
			else
			{
				cNKMGameTeamData.m_DeckData.SetNextDeck(listUnitDeckUsed2);
			}
			cNKMGameTeamData.m_DeckData.RemoveAtListUnitDeckUsed(num4);
		}
		if (num2 == unitUID)
		{
			unitUID = -1L;
			num3 = -1;
		}
		SyncDeckChange(cNKMGameTeamData.m_eNKM_TEAM_TYPE, rollbackTime, num, num2, unitUID, num3, deckTombAddUnitUID, cNKMGameTeamData.m_DeckData.GetAutoRespawnIndex(), cNKMGameTeamData.m_DeckData.GetNextDeck());
		return num2;
	}

	private void ProcessRespawn()
	{
		ProcessRespawn(base.m_NKMGameData.m_NKMGameTeamDataA, bTeamA: true);
		ProcessRespawn(base.m_NKMGameData.m_NKMGameTeamDataB, bTeamA: false);
	}

	private void ProcessRespawn(NKMGameTeamData cNKMGameTeamData, bool bTeamA)
	{
		if (GetWorldStopTime() > 0f || (GetDungeonTemplet() != null && GetDungeonTemplet().m_bNoEnemyRespawnBeforeUserFirstRespawn && !bTeamA && !m_bTeamAFirstRespawn))
		{
			return;
		}
		for (int i = 0; i < m_listNKMGameUnitRespawnData.Count; i++)
		{
			NKMGameUnitRespawnData nKMGameUnitRespawnData = m_listNKMGameUnitRespawnData[i];
			if (nKMGameUnitRespawnData == null)
			{
				continue;
			}
			nKMGameUnitRespawnData.m_fRespawnCoolTime -= m_fDeltaTime;
			if (!(nKMGameUnitRespawnData.m_fRespawnCoolTime < 0f))
			{
				continue;
			}
			nKMGameUnitRespawnData.m_fRespawnCoolTime = 0f;
			NKMUnitData unitDataByUnitUID = cNKMGameTeamData.GetUnitDataByUnitUID(nKMGameUnitRespawnData.m_UnitUID);
			if (unitDataByUnitUID == null)
			{
				continue;
			}
			float num = nKMGameUnitRespawnData.m_fRespawnPosX;
			if (num.IsNearlyEqual(-1f))
			{
				num = GetRespawnPosX(bTeamA);
			}
			if (RespawnUnit(unitDataByUnitUID, num, GetRespawnPosZ(), 0f, bInitHPRate: false, nKMGameUnitRespawnData.m_fRollbackTime))
			{
				if (bTeamA)
				{
					m_bTeamAFirstRespawn = true;
				}
				else
				{
					m_bTeamBFirstRespawn = true;
				}
				ProcessRespawnCombo(unitDataByUnitUID, cNKMGameTeamData);
				m_ObjectPool.CloseObj(nKMGameUnitRespawnData);
				m_listNKMGameUnitRespawnData.RemoveAt(i);
				break;
			}
		}
	}

	protected void ProcessRespawnCombo(NKMUnitData cNewUnitData, NKMGameTeamData cNKMGameTeamData)
	{
		if (cNewUnitData != null && cNKMGameTeamData != null && m_dicRespawnUnitUID_ByDeckUsed != null)
		{
			NKMGameRuntimeTeamData myRuntimeTeamData = m_NKMGameRuntimeData.GetMyRuntimeTeamData(cNKMGameTeamData.m_eNKM_TEAM_TYPE);
			if (myRuntimeTeamData != null && m_dicRespawnUnitUID_ByDeckUsed.TryGetValue(cNewUnitData.m_UnitUID, out var value))
			{
				AddComboByDeckUnitRespawn(cNKMGameTeamData, myRuntimeTeamData, cNewUnitData.m_UnitID, value);
				m_dicRespawnUnitUID_ByDeckUsed.Remove(cNewUnitData.m_UnitUID);
			}
		}
	}

	private void ProcessDynamicRespawn()
	{
		for (int i = 0; i < m_listNKMGameUnitDynamicRespawnData.Count; i++)
		{
			NKMDynamicRespawnUnitReserve nKMDynamicRespawnUnitReserve = m_listNKMGameUnitDynamicRespawnData[i];
			DynamicRespawnUnit(nKMDynamicRespawnUnitReserve.m_GameUnitUID, nKMDynamicRespawnUnitReserve.m_PosX, nKMDynamicRespawnUnitReserve.m_PosZ, nKMDynamicRespawnUnitReserve.m_fJumpYPos, nKMDynamicRespawnUnitReserve.m_bUseRight, nKMDynamicRespawnUnitReserve.m_bRight, nKMDynamicRespawnUnitReserve.m_fHPRate, nKMDynamicRespawnUnitReserve.m_RespawnState, nKMDynamicRespawnUnitReserve.m_fRollbackTime);
		}
		m_listNKMGameUnitDynamicRespawnData.Clear();
	}

	private float GetShipRespawnPosX(bool bTeamA)
	{
		if (bTeamA)
		{
			return m_NKMMapTemplet.m_fMinX;
		}
		return m_NKMMapTemplet.m_fMaxX;
	}

	private float GetShipRespawnPosZ(float fRate = 1f)
	{
		return m_NKMMapTemplet.m_fMinZ + (m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ) * fRate;
	}

	public float GetRespawnPosZ(float fFactorMin = -1f, float fFactorMax = -1f)
	{
		if (fFactorMin.IsNearlyEqual(-1f) && fFactorMax.IsNearlyEqual(-1f))
		{
			return NKMRandom.Range(m_NKMMapTemplet.m_fMinZ, m_NKMMapTemplet.m_fMaxZ);
		}
		if (fFactorMin.IsNearlyEqual(-1f))
		{
			fFactorMin = 0f;
		}
		else if (fFactorMin < 0f)
		{
			fFactorMin = 0f;
		}
		else if (fFactorMin > 1f)
		{
			fFactorMin = 1f;
		}
		if (fFactorMax.IsNearlyEqual(-1f))
		{
			fFactorMax = 1f;
		}
		else if (fFactorMax < 0f)
		{
			fFactorMax = 0f;
		}
		else if (fFactorMax > 1f)
		{
			fFactorMax = 1f;
		}
		if (fFactorMin > fFactorMax)
		{
			fFactorMin = fFactorMax;
			Log.Error("GetRespawnPosZ fFactorMin > fFactorMax ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 2657);
		}
		float num = NKMRandom.Range(fFactorMin, fFactorMax);
		float num2 = m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ;
		return m_NKMMapTemplet.m_fMinZ + num2 * num;
	}

	protected virtual NKMGameSyncData_Base GetCurrentSyncData(float rollbackTime = 0f)
	{
		bool bWillUseRollback = rollbackTime > 0f;
		NKMGameSyncData_Base nKMGameSyncData_Base = m_NKMGameSyncDataPack.m_listGameSyncData.FindLast((NKMGameSyncData_Base x) => x.IsRollbackPacket == bWillUseRollback);
		if (nKMGameSyncData_Base != null && nKMGameSyncData_Base.m_fAbsoluteGameTime < m_AbsoluteGameTime - rollbackTime)
		{
			nKMGameSyncData_Base = null;
		}
		if (nKMGameSyncData_Base == null)
		{
			nKMGameSyncData_Base = new NKMGameSyncData_Base();
			m_NKMGameSyncDataPack.m_listGameSyncData.Add(nKMGameSyncData_Base);
		}
		nKMGameSyncData_Base.m_fGameTime = m_NKMGameRuntimeData.m_GameTime - rollbackTime;
		nKMGameSyncData_Base.m_fAbsoluteGameTime = m_AbsoluteGameTime - rollbackTime;
		nKMGameSyncData_Base.m_fRemainGameTime = m_NKMGameRuntimeData.m_fRemainGameTime;
		nKMGameSyncData_Base.m_fRespawnCostA1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost;
		nKMGameSyncData_Base.m_fRespawnCostB1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost;
		nKMGameSyncData_Base.m_fRespawnCostAssistA1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCostAssist;
		nKMGameSyncData_Base.m_fRespawnCostAssistB1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCostAssist;
		nKMGameSyncData_Base.m_fUsedRespawnCostA1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fUsedRespawnCost;
		nKMGameSyncData_Base.m_fUsedRespawnCostB1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fUsedRespawnCost;
		nKMGameSyncData_Base.m_fShipDamage = m_NKMGameRuntimeData.m_fShipDamage;
		nKMGameSyncData_Base.m_NKM_GAME_SPEED_TYPE = m_NKMGameRuntimeData.m_NKM_GAME_SPEED_TYPE;
		nKMGameSyncData_Base.m_NKM_GAME_AUTO_SKILL_TYPE_A = m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_NKM_GAME_AUTO_SKILL_TYPE;
		nKMGameSyncData_Base.m_NKM_GAME_AUTO_SKILL_TYPE_B = m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_B1).m_NKM_GAME_AUTO_SKILL_TYPE;
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_FIERCE)
		{
			if (nKMGameSyncData_Base.m_NKMGameSyncData_GamePoint == null)
			{
				nKMGameSyncData_Base.m_NKMGameSyncData_GamePoint = new NKMGameSyncData_GamePoint();
			}
			nKMGameSyncData_Base.m_NKMGameSyncData_GamePoint.m_fGamePoint = (int)m_GameRecord.TotalFiercePoint;
		}
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_TRIM)
		{
			if (nKMGameSyncData_Base.m_NKMGameSyncData_GamePoint == null)
			{
				nKMGameSyncData_Base.m_NKMGameSyncData_GamePoint = new NKMGameSyncData_GamePoint();
			}
			nKMGameSyncData_Base.m_NKMGameSyncData_GamePoint.m_fGamePoint = (int)m_GameRecord.TotalTrimPoint;
		}
		return nKMGameSyncData_Base;
	}

	private void SyncDeckChange(NKM_TEAM_TYPE eNKM_TEAM_TYPE, float rollbackTime, int deckIndex, long unitUID, long deckUsedAddUnitUID, int deckUsedRemoveIndex, long deckTombAddUnitUID, int autoRespawnIndex, long nextDeckUnitUID)
	{
		NKMGameSyncData_Base currentSyncData = GetCurrentSyncData(rollbackTime);
		NKMGameSyncData_Deck item = new NKMGameSyncData_Deck
		{
			m_NKM_TEAM_TYPE = eNKM_TEAM_TYPE,
			m_UnitDeckIndex = (sbyte)deckIndex,
			m_UnitDeckUID = unitUID,
			m_DeckUsedAddUnitUID = deckUsedAddUnitUID,
			m_DeckUsedRemoveIndex = (sbyte)deckUsedRemoveIndex,
			m_DeckTombAddUnitUID = deckTombAddUnitUID,
			m_AutoRespawnIndex = (sbyte)autoRespawnIndex,
			m_NextDeckUnitUID = nextDeckUnitUID
		};
		currentSyncData.m_NKMGameSyncData_Deck.Add(item);
	}

	private void SyncDeckChangeAssist(NKM_TEAM_TYPE eNKM_TEAM_TYPE, float rollbackTime, int autoRespawnIndexAssist)
	{
		NKMGameSyncData_Base currentSyncData = GetCurrentSyncData(rollbackTime);
		NKMGameSyncData_DeckAssist item = new NKMGameSyncData_DeckAssist
		{
			m_NKM_TEAM_TYPE = eNKM_TEAM_TYPE,
			m_AutoRespawnIndexAssist = (sbyte)autoRespawnIndexAssist
		};
		currentSyncData.m_NKMGameSyncData_DeckAssist.Add(item);
	}

	public void SyncDieUnit(short gameUnitUID = -1)
	{
		NKMGameSyncData_Base currentSyncData = GetCurrentSyncData();
		NKMGameSyncData_DieUnit nKMGameSyncData_DieUnit = null;
		if (gameUnitUID == -1)
		{
			nKMGameSyncData_DieUnit = new NKMGameSyncData_DieUnit();
			foreach (KeyValuePair<short, NKMUnit> item in m_dicNKMUnitPool)
			{
				if (!nKMGameSyncData_DieUnit.m_DieGameUnitUID.Contains(item.Key))
				{
					nKMGameSyncData_DieUnit.m_DieGameUnitUID.Add(item.Key);
				}
			}
		}
		else
		{
			nKMGameSyncData_DieUnit = ((currentSyncData.m_NKMGameSyncData_DieUnit.Count <= 0) ? new NKMGameSyncData_DieUnit() : currentSyncData.m_NKMGameSyncData_DieUnit[currentSyncData.m_NKMGameSyncData_DieUnit.Count - 1]);
			if (!nKMGameSyncData_DieUnit.m_DieGameUnitUID.Contains(gameUnitUID))
			{
				nKMGameSyncData_DieUnit.m_DieGameUnitUID.Add(gameUnitUID);
			}
		}
		currentSyncData.m_NKMGameSyncData_DieUnit.Add(nKMGameSyncData_DieUnit);
	}

	public void SyncUnitSyncHalfData(NKMUnit cUnit, NKMUnitSyncData cNKMUnitSyncData)
	{
		cNKMUnitSyncData.m_usSpeedX = NKMUtil.FloatToHalf(cUnit.GetUnitFrameData().m_fSpeedX);
		cNKMUnitSyncData.m_usSpeedY = NKMUtil.FloatToHalf(cUnit.GetUnitFrameData().m_fSpeedY);
		cNKMUnitSyncData.m_usSpeedZ = NKMUtil.FloatToHalf(cUnit.GetUnitFrameData().m_fSpeedZ);
		cNKMUnitSyncData.m_bDamageSpeedXNegative = cUnit.GetUnitFrameData().m_fDamageSpeedX < 0f;
		cNKMUnitSyncData.m_usDamageSpeedX = NKMUtil.FloatToHalf(Math.Abs(cUnit.GetUnitFrameData().m_fDamageSpeedX));
		cNKMUnitSyncData.m_usDamageSpeedZ = NKMUtil.FloatToHalf(cUnit.GetUnitFrameData().m_fDamageSpeedZ);
		cNKMUnitSyncData.m_usDamageSpeedJumpY = NKMUtil.FloatToHalf(cUnit.GetUnitFrameData().m_fDamageSpeedJumpY);
		cNKMUnitSyncData.m_usDamageSpeedKeepTimeX = NKMUtil.FloatToHalf(cUnit.GetUnitFrameData().m_fDamageSpeedKeepTimeX);
		cNKMUnitSyncData.m_usDamageSpeedKeepTimeZ = NKMUtil.FloatToHalf(cUnit.GetUnitFrameData().m_fDamageSpeedKeepTimeZ);
		cNKMUnitSyncData.m_usDamageSpeedKeepTimeJumpY = NKMUtil.FloatToHalf(cUnit.GetUnitFrameData().m_fDamageSpeedKeepTimeJumpY);
		cNKMUnitSyncData.m_usSkillCoolTime = 0;
		cNKMUnitSyncData.m_usHyperSkillCoolTime = 0;
		if (cUnit.GetUnitTemplet().m_listSkillStateData.Count > 0 && cUnit.GetUnitTemplet().m_listSkillStateData[0] != null)
		{
			float stateCoolTime = cUnit.GetStateCoolTime(cUnit.GetUnitTemplet().m_listSkillStateData[0].m_StateName);
			cNKMUnitSyncData.m_usSkillCoolTime = NKMUtil.FloatToHalf(stateCoolTime);
		}
		if (cUnit.GetUnitTemplet().m_listHyperSkillStateData.Count > 0 && cUnit.GetUnitTemplet().m_listHyperSkillStateData[0] != null)
		{
			float stateCoolTime2 = cUnit.GetStateCoolTime(cUnit.GetUnitTemplet().m_listHyperSkillStateData[0].m_StateName);
			cNKMUnitSyncData.m_usHyperSkillCoolTime = NKMUtil.FloatToHalf(stateCoolTime2);
		}
	}

	public void SyncUnitSync(NKMUnit cUnit, NKMGameSyncDataPack cNKMGameSyncDataPack = null)
	{
		if (cNKMGameSyncDataPack == null)
		{
			cNKMGameSyncDataPack = m_NKMGameSyncDataPack;
		}
		float syncRollbackTime = cUnit.GetSyncRollbackTime();
		bool bWillUseRollback = syncRollbackTime > 0f;
		NKMGameSyncData_Base nKMGameSyncData_Base = cNKMGameSyncDataPack.m_listGameSyncData.FindLast((NKMGameSyncData_Base x) => x.IsRollbackPacket == bWillUseRollback);
		if (nKMGameSyncData_Base != null && nKMGameSyncData_Base.m_fAbsoluteGameTime < m_AbsoluteGameTime - cUnit.GetSyncRollbackTime())
		{
			nKMGameSyncData_Base = null;
		}
		if (nKMGameSyncData_Base == null)
		{
			nKMGameSyncData_Base = new NKMGameSyncData_Base();
			m_NKMGameSyncDataPack.m_listGameSyncData.Add(nKMGameSyncData_Base);
		}
		nKMGameSyncData_Base.IsRollbackPacket = bWillUseRollback;
		nKMGameSyncData_Base.m_fGameTime = m_NKMGameRuntimeData.m_GameTime - syncRollbackTime;
		nKMGameSyncData_Base.m_fAbsoluteGameTime = m_AbsoluteGameTime - syncRollbackTime;
		nKMGameSyncData_Base.m_fRemainGameTime = m_NKMGameRuntimeData.m_fRemainGameTime;
		nKMGameSyncData_Base.m_fRespawnCostA1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost;
		nKMGameSyncData_Base.m_fRespawnCostB1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost;
		nKMGameSyncData_Base.m_fRespawnCostAssistA1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCostAssist;
		nKMGameSyncData_Base.m_fRespawnCostAssistB1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCostAssist;
		nKMGameSyncData_Base.m_fUsedRespawnCostA1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fUsedRespawnCost;
		nKMGameSyncData_Base.m_fUsedRespawnCostB1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fUsedRespawnCost;
		nKMGameSyncData_Base.m_fShipDamage = m_NKMGameRuntimeData.m_fShipDamage;
		nKMGameSyncData_Base.m_NKM_GAME_SPEED_TYPE = m_NKMGameRuntimeData.m_NKM_GAME_SPEED_TYPE;
		nKMGameSyncData_Base.m_NKM_GAME_AUTO_SKILL_TYPE_A = m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_NKM_GAME_AUTO_SKILL_TYPE;
		nKMGameSyncData_Base.m_NKM_GAME_AUTO_SKILL_TYPE_B = m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_B1).m_NKM_GAME_AUTO_SKILL_TYPE;
		NKMGameSyncData_Unit nKMGameSyncData_Unit = new NKMGameSyncData_Unit();
		nKMGameSyncData_Unit.m_NKMGameUnitSyncData = new NKMUnitSyncData();
		nKMGameSyncData_Unit.m_NKMGameUnitSyncData.DeepCopyFrom(cUnit.GetUnitSyncData());
		SyncUnitSyncHalfData(cUnit, nKMGameSyncData_Unit.m_NKMGameUnitSyncData);
		nKMGameSyncData_Base.m_NKMGameSyncData_Unit.Add(nKMGameSyncData_Unit);
		cUnit.GetUnitSyncData().m_bRespawnThisFrame = false;
		foreach (KeyValuePair<short, NKMBuffSyncData> dicBuffDatum in cUnit.GetUnitSyncData().m_dicBuffData)
		{
			dicBuffDatum.Value.m_bNew = false;
		}
		cUnit.GetUnitSyncData().m_listNKM_UNIT_EVENT_MARK.Clear();
		cUnit.GetUnitSyncData().m_listStatusTimeData.Clear();
		cUnit.GetUnitSyncData().m_listInvokedTrigger.Clear();
		cUnit.GetUnitSyncData().m_listUpdatedReaction.Clear();
	}

	public void SyncUnitSimpleSync(NKMUnit cUnit)
	{
		NKMGameSyncData_Base currentSyncData = GetCurrentSyncData();
		NKMGameSyncDataSimple_Unit nKMGameSyncDataSimple_Unit = new NKMGameSyncDataSimple_Unit();
		nKMGameSyncDataSimple_Unit.m_GameUnitUID = cUnit.GetUnitSyncData().m_GameUnitUID;
		nKMGameSyncDataSimple_Unit.m_bRight = cUnit.GetUnitSyncData().m_bRight;
		nKMGameSyncDataSimple_Unit.m_TargetUID = cUnit.GetUnitSyncData().m_TargetUID;
		nKMGameSyncDataSimple_Unit.m_SubTargetUID = cUnit.GetUnitSyncData().m_SubTargetUID;
		nKMGameSyncDataSimple_Unit.m_listNKM_UNIT_EVENT_MARK.Clear();
		for (int i = 0; i < cUnit.GetUnitSyncData().m_listNKM_UNIT_EVENT_MARK.Count; i++)
		{
			nKMGameSyncDataSimple_Unit.m_listNKM_UNIT_EVENT_MARK.Add(cUnit.GetUnitSyncData().m_listNKM_UNIT_EVENT_MARK[i]);
		}
		nKMGameSyncDataSimple_Unit.m_listInvokedTrigger.Clear();
		nKMGameSyncDataSimple_Unit.m_listInvokedTrigger.AddRange(cUnit.GetUnitSyncData().m_listInvokedTrigger);
		nKMGameSyncDataSimple_Unit.m_listUpdatedReaction.Clear();
		nKMGameSyncDataSimple_Unit.m_listUpdatedReaction.AddRange(cUnit.GetUnitSyncData().m_listUpdatedReaction);
		nKMGameSyncDataSimple_Unit.m_dicEventVariables.Clear();
		foreach (KeyValuePair<string, int> dicEventVariable in cUnit.GetUnitSyncData().m_dicEventVariables)
		{
			nKMGameSyncDataSimple_Unit.m_dicEventVariables.Add(dicEventVariable.Key, dicEventVariable.Value);
		}
		cUnit.GetUnitSyncData().m_bRespawnThisFrame = false;
		nKMGameSyncDataSimple_Unit.m_dicBuffData.Clear();
		foreach (KeyValuePair<short, NKMBuffSyncData> dicBuffDatum in cUnit.GetUnitSyncData().m_dicBuffData)
		{
			NKMBuffSyncData nKMBuffSyncData = new NKMBuffSyncData();
			nKMBuffSyncData.DeepCopyFrom(dicBuffDatum.Value);
			dicBuffDatum.Value.m_bNew = false;
			nKMGameSyncDataSimple_Unit.m_dicBuffData.Add(nKMBuffSyncData.m_BuffID, nKMBuffSyncData);
		}
		nKMGameSyncDataSimple_Unit.m_listStatusTimeData.Clear();
		foreach (NKMUnitStatusTimeSyncData listStatusTimeDatum in cUnit.GetUnitSyncData().m_listStatusTimeData)
		{
			nKMGameSyncDataSimple_Unit.m_listStatusTimeData.Add(listStatusTimeDatum);
		}
		cUnit.GetUnitSyncData().m_listStatusTimeData.Clear();
		currentSyncData.m_NKMGameSyncDataSimple_Unit.Add(nKMGameSyncDataSimple_Unit);
		cUnit.GetUnitSyncData().m_listNKM_UNIT_EVENT_MARK.Clear();
		cUnit.GetUnitSyncData().m_listInvokedTrigger.Clear();
		cUnit.GetUnitSyncData().m_listUpdatedReaction.Clear();
	}

	public void SyncShipSkillSync(NKMUnit cUnit)
	{
		NKMGameSyncData_Base currentSyncData = GetCurrentSyncData();
		NKMGameSyncData_ShipSkill nKMGameSyncData_ShipSkill = new NKMGameSyncData_ShipSkill
		{
			m_NKMGameUnitSyncData = new NKMUnitSyncData()
		};
		nKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData.DeepCopyFrom(cUnit.GetUnitSyncData());
		nKMGameSyncData_ShipSkill.m_ShipSkillID = cUnit.GetUnitFrameData().m_ShipSkillTemplet.m_ShipSkillID;
		nKMGameSyncData_ShipSkill.m_SkillPosX = cUnit.GetUnitFrameData().m_fShipSkillPosX;
		currentSyncData.m_NKMGameSyncData_ShipSkill.Add(nKMGameSyncData_ShipSkill);
		cUnit.GetUnitSyncData().m_bRespawnThisFrame = false;
		foreach (KeyValuePair<short, NKMBuffSyncData> dicBuffDatum in cUnit.GetUnitSyncData().m_dicBuffData)
		{
			dicBuffDatum.Value.m_bNew = false;
		}
		cUnit.GetUnitSyncData().m_listNKM_UNIT_EVENT_MARK.Clear();
		cUnit.GetUnitSyncData().m_listInvokedTrigger.Clear();
		cUnit.GetUnitSyncData().m_listUpdatedReaction.Clear();
	}

	protected void SyncGameStateChange(NKM_GAME_STATE eNKM_GAME_STATE, NKM_TEAM_TYPE eWinTeam, int waveID = 0)
	{
		NKMGameSyncData_Base currentSyncData = GetCurrentSyncData();
		NKMGameSyncData_GameState item = new NKMGameSyncData_GameState
		{
			m_NKM_GAME_STATE = eNKM_GAME_STATE,
			m_WinTeam = eWinTeam,
			m_WaveID = waveID
		};
		currentSyncData.m_NKMGameSyncData_GameState.Add(item);
	}

	protected void SyncDungeonEvent(NKMDungeonEventTemplet dungeonEventTemplet, NKM_TEAM_TYPE eTeam, bool bRecordToGameData)
	{
		NKMGameSyncData_Base currentSyncData = GetCurrentSyncData();
		NKMGameSyncData_DungeonEvent item = new NKMGameSyncData_DungeonEvent
		{
			m_eEventActionType = dungeonEventTemplet.m_dungeonEventType,
			m_EventID = dungeonEventTemplet.m_EventID,
			m_iEventActionValue = dungeonEventTemplet.m_EventActionValue,
			m_strEventActionValue = dungeonEventTemplet.m_EventActionStrValue,
			m_bPause = dungeonEventTemplet.m_bPause,
			m_eTeam = eTeam
		};
		currentSyncData.m_NKMGameSyncData_DungeonEvent.Add(item);
		if (bRecordToGameData)
		{
			if (GetGameRuntimeData().m_lstPermanentDungeonEvent == null)
			{
				GetGameRuntimeData().m_lstPermanentDungeonEvent = new List<NKMGameSyncData_DungeonEvent>();
			}
			GetGameRuntimeData().m_lstPermanentDungeonEvent.Add(item);
		}
	}

	protected void SendDungeonEvent(NKM_TEAM_TYPE eTeam, bool bRecordToGameData, NKM_EVENT_ACTION_TYPE eventActionType, int actionValue, string strValue, int eventID = 0, bool bPause = false)
	{
		NKMGameSyncData_Base currentSyncData = GetCurrentSyncData();
		NKMGameSyncData_DungeonEvent item = new NKMGameSyncData_DungeonEvent
		{
			m_eEventActionType = eventActionType,
			m_EventID = eventID,
			m_iEventActionValue = actionValue,
			m_strEventActionValue = strValue,
			m_bPause = bPause,
			m_eTeam = eTeam
		};
		currentSyncData.m_NKMGameSyncData_DungeonEvent.Add(item);
		if (bRecordToGameData)
		{
			if (GetGameRuntimeData().m_lstPermanentDungeonEvent == null)
			{
				GetGameRuntimeData().m_lstPermanentDungeonEvent = new List<NKMGameSyncData_DungeonEvent>();
			}
			GetGameRuntimeData().m_lstPermanentDungeonEvent.Add(item);
		}
	}

	protected void SyncGameEvent(NKM_GAME_EVENT_TYPE eNKM_GAME_EVENT_TYPE, NKM_TEAM_TYPE eNKM_TEAM_TYPE, int eventID, float fValue = 0f)
	{
		NKMGameSyncData_Base currentSyncData = GetCurrentSyncData();
		NKMGameSyncData_GameEvent item = new NKMGameSyncData_GameEvent
		{
			m_NKM_GAME_EVENT_TYPE = eNKM_GAME_EVENT_TYPE,
			m_NKM_TEAM_TYPE = eNKM_TEAM_TYPE,
			m_EventID = eventID,
			m_fValue = fValue
		};
		currentSyncData.m_NKMGameSyncData_GameEvent.Add(item);
	}

	protected void ForceSyncDataPackFlushThisFrame()
	{
		m_SyncFlushTime = 0f;
	}

	private void SyncDataPackFlush()
	{
		m_SyncFlushTime -= m_fDeltaTime;
		if (m_SyncFlushTime <= 0f)
		{
			m_SyncFlushTime = 0.4f;
			if (m_NKMGameSyncDataPack.m_listGameSyncData.Count == 0)
			{
				NKMGameSyncData_Base nKMGameSyncData_Base = new NKMGameSyncData_Base();
				nKMGameSyncData_Base.m_fGameTime = m_NKMGameRuntimeData.m_GameTime;
				nKMGameSyncData_Base.m_fAbsoluteGameTime = m_AbsoluteGameTime;
				nKMGameSyncData_Base.m_fRemainGameTime = m_NKMGameRuntimeData.m_fRemainGameTime;
				nKMGameSyncData_Base.m_fShipDamage = m_NKMGameRuntimeData.m_fShipDamage;
				nKMGameSyncData_Base.m_fRespawnCostA1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCost;
				nKMGameSyncData_Base.m_fRespawnCostB1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCost;
				nKMGameSyncData_Base.m_fRespawnCostAssistA1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fRespawnCostAssist;
				nKMGameSyncData_Base.m_fRespawnCostAssistB1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fRespawnCostAssist;
				nKMGameSyncData_Base.m_fUsedRespawnCostA1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fUsedRespawnCost;
				nKMGameSyncData_Base.m_fUsedRespawnCostB1 = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_fUsedRespawnCost;
				nKMGameSyncData_Base.m_NKM_GAME_SPEED_TYPE = m_NKMGameRuntimeData.m_NKM_GAME_SPEED_TYPE;
				nKMGameSyncData_Base.m_NKM_GAME_AUTO_SKILL_TYPE_A = m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_NKM_GAME_AUTO_SKILL_TYPE;
				nKMGameSyncData_Base.m_NKM_GAME_AUTO_SKILL_TYPE_B = m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_B1).m_NKM_GAME_AUTO_SKILL_TYPE;
				m_NKMGameSyncDataPack.m_listGameSyncData.Add(nKMGameSyncData_Base);
			}
			NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT nKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT = new NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT();
			nKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT.gameTime = m_NKMGameRuntimeData.m_GameTime;
			nKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT.absoluteGameTime = m_AbsoluteGameTime;
			nKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT.gameSyncDataPack = m_NKMGameSyncDataPack;
			SendSyncDataPackFlush(nKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT);
			m_NKMGameSyncDataPack = new NKMGameSyncDataPack();
		}
	}

	public virtual void SendSyncDataPackFlush(NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT cPacket_NPT_GAME_SYNC_DATA_PACK_NOT)
	{
	}

	public NKMPacket_GAME_INTRUDE_START_NOT MakeFullSyncData()
	{
		NKMPacket_GAME_INTRUDE_START_NOT nKMPacket_GAME_INTRUDE_START_NOT = new NKMPacket_GAME_INTRUDE_START_NOT();
		nKMPacket_GAME_INTRUDE_START_NOT.gameTime = m_NKMGameRuntimeData.m_GameTime;
		nKMPacket_GAME_INTRUDE_START_NOT.absoluteGameTime = m_AbsoluteGameTime;
		nKMPacket_GAME_INTRUDE_START_NOT.gameSyncDataPack = new NKMGameSyncDataPack();
		nKMPacket_GAME_INTRUDE_START_NOT.gameTeamDeckDataA = new NKMGameTeamDeckData();
		nKMPacket_GAME_INTRUDE_START_NOT.gameTeamDeckDataA.DeepCopyFrom(GetGameData().m_NKMGameTeamDataA.m_DeckData);
		nKMPacket_GAME_INTRUDE_START_NOT.gameTeamDeckDataB = new NKMGameTeamDeckData();
		nKMPacket_GAME_INTRUDE_START_NOT.gameTeamDeckDataB.DeepCopyFrom(GetGameData().m_NKMGameTeamDataB.m_DeckData);
		nKMPacket_GAME_INTRUDE_START_NOT.usedRespawnCost = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fUsedRespawnCost;
		nKMPacket_GAME_INTRUDE_START_NOT.respawnCount = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_respawn_count;
		if (base.m_NKMGameData.m_NKMGameTeamDataA != null && base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip != null && base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID.Count == 1)
		{
			nKMPacket_GAME_INTRUDE_START_NOT.mainShipAStateCoolTimeMap = GetUnit(base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID[0]).GetDicStateCoolTime();
		}
		if (base.m_NKMGameData.m_NKMGameTeamDataB != null && base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip != null && base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID.Count == 1)
		{
			nKMPacket_GAME_INTRUDE_START_NOT.mainShipBStateCoolTimeMap = GetUnit(base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID[0]).GetDicStateCoolTime();
		}
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnit.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMUnit value = enumerator.Current.Value;
			SyncUnitSync(value, nKMPacket_GAME_INTRUDE_START_NOT.gameSyncDataPack);
		}
		return nKMPacket_GAME_INTRUDE_START_NOT;
	}

	public override NKMDamageEffectManager GetDEManager()
	{
		return m_DEManager;
	}

	public override NKMDamageEffect GetDamageEffect(short DEUID)
	{
		return m_DEManager.GetDamageEffect(DEUID);
	}

	public virtual NKM_ERROR_CODE OnRecv(NKMPacket_GAME_PAUSE_REQ cNKMPacket_GAME_PAUSE_REQ)
	{
		if ((m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH || m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_END) && cNKMPacket_GAME_PAUSE_REQ.isPause)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GAME_IS_PAUSE;
		}
		m_NKMGameRuntimeData.m_bPause = cNKMPacket_GAME_PAUSE_REQ.isPause;
		return NKM_ERROR_CODE.NEC_OK;
	}

	public virtual NKM_ERROR_CODE OnRecv(NKMPacket_GAME_RESPAWN_REQ cPacket_GAME_RESPAWN_REQ, NKM_TEAM_TYPE eNKM_TEAM_TYPE, ref long respawnUnitUID)
	{
		respawnUnitUID = cPacket_GAME_RESPAWN_REQ.unitUID;
		NKMGameTeamData nKMGameTeamData = null;
		if (IsATeam(eNKM_TEAM_TYPE))
		{
			nKMGameTeamData = base.m_NKMGameData.m_NKMGameTeamDataA;
		}
		if (GetGameData().IsPVP() && GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_ASYNC_PVP)
		{
			return NKM_ERROR_CODE.NEC_FAIL_ASYNC_PVP_MANUAL_PLAY_DISABLE;
		}
		if (IsBTeam(eNKM_TEAM_TYPE))
		{
			nKMGameTeamData = base.m_NKMGameData.m_NKMGameTeamDataB;
		}
		if (nKMGameTeamData == null)
		{
			return NKMError.Build(NKM_ERROR_CODE.NEC_FAIL_ACCOUNT_INVALID_ID, -1L, base.m_NKMGameData.m_GameUID, $"teamType:{eNKM_TEAM_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3216);
		}
		if (nKMGameTeamData.m_DeckData == null)
		{
			return NKMError.Build(NKM_ERROR_CODE.NEC_FAIL_DECK_DATA_INVALID, -1L, base.m_NKMGameData.m_GameUID, $"teamType:{eNKM_TEAM_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3221);
		}
		NKMGameRuntimeTeamData nKMGameRuntimeTeamData = null;
		if (IsATeam(eNKM_TEAM_TYPE))
		{
			nKMGameRuntimeTeamData = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA;
		}
		if (IsBTeam(eNKM_TEAM_TYPE))
		{
			nKMGameRuntimeTeamData = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB;
		}
		NKMUnitData nKMUnitData = null;
		NKMUnitTemplet nKMUnitTemplet = null;
		bool assistUnit = cPacket_GAME_RESPAWN_REQ.assistUnit;
		if (assistUnit)
		{
			nKMUnitData = nKMGameTeamData.GetAssistUnitDataByIndex(nKMGameTeamData.m_DeckData.GetAutoRespawnIndexAssist());
			if (nKMUnitData != null)
			{
				GetNextAutoRespawnIndexAssist(nKMGameTeamData);
			}
		}
		else
		{
			for (int i = 0; i < nKMGameTeamData.m_DeckData.GetListUnitDeckCount(); i++)
			{
				long listUnitDeck = nKMGameTeamData.m_DeckData.GetListUnitDeck(i);
				if (cPacket_GAME_RESPAWN_REQ.unitUID == listUnitDeck)
				{
					nKMUnitData = nKMGameTeamData.GetUnitDataByUnitUID(listUnitDeck);
					if (nKMUnitData != null)
					{
						break;
					}
				}
			}
		}
		if (nKMUnitData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_UNIT_NULL;
		}
		respawnUnitUID = nKMUnitData.m_UnitUID;
		nKMUnitTemplet = NKMUnitManager.GetUnitTemplet(nKMUnitData.m_UnitID);
		if (nKMUnitTemplet == null)
		{
			Log.Error($"Can not found unittemplet. unitId:{nKMUnitData.m_UnitID}, unitUid:{nKMUnitData.m_UnitUID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3268);
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_UNIT_TEMPLET_NULL;
		}
		NKMUnitStatTemplet statTemplet = nKMUnitTemplet.m_StatTemplet;
		float fRespawnValidLandTeamA = m_fRespawnValidLandTeamA;
		bool flag = false;
		if (nKMUnitTemplet.m_UnitTempletBase.RespawnFreePos)
		{
			flag = true;
		}
		if (GetDungeonTemplet() != null && GetDungeonTemplet().m_bRespawnFreePos)
		{
			flag = true;
		}
		fRespawnValidLandTeamA = (flag ? 0.8f : ((!IsATeam(eNKM_TEAM_TYPE)) ? m_fRespawnValidLandTeamB : m_fRespawnValidLandTeamA));
		int num = ((nKMGameTeamData.m_LeaderUnitUID != nKMUnitData.m_UnitUID || assistUnit) ? GetRespawnCost(statTemplet, bLeader: false, nKMGameTeamData.m_eNKM_TEAM_TYPE) : Math.Max(0, GetRespawnCost(statTemplet, bLeader: true, nKMGameTeamData.m_eNKM_TEAM_TYPE)));
		float minOffset = (GetGameData().IsPVP() ? NKMCommonConst.PVP_SUMMON_MIN_POS : NKMCommonConst.PVE_SUMMON_MIN_POS);
		cPacket_GAME_RESPAWN_REQ.respawnPosX = m_NKMMapTemplet.GetNearLandX(cPacket_GAME_RESPAWN_REQ.respawnPosX, IsATeam(eNKM_TEAM_TYPE), fRespawnValidLandTeamA, minOffset);
		if (!assistUnit)
		{
			if ((float)num > nKMGameRuntimeTeamData.m_fRespawnCost)
			{
				return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_NO_RESPAWN_COST;
			}
		}
		else if ((float)num > nKMGameRuntimeTeamData.m_fRespawnCostAssist)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_NO_RESPAWN_COST;
		}
		if (IsRespawnUnitWait(nKMUnitData.m_UnitUID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_UNIT_LIVE;
		}
		if (!IsGameUnitAllDie(nKMUnitData.m_UnitUID))
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = IsGameUnitAllInBattle(nKMUnitData.m_UnitUID);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE;
			}
			if (!RespawnUnit(bCheckOnly: true, 0f, 0f, nKMGameTeamData, nKMGameRuntimeTeamData, nKMUnitData.m_UnitUID, cPacket_GAME_RESPAWN_REQ.respawnPosX))
			{
				return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_NO_GAME_STATE;
			}
			EventDieForReRespawn(nKMUnitData.m_UnitUID);
			if (!RespawnUnit(bCheckOnly: false, 0.01f, 0f, nKMGameTeamData, nKMGameRuntimeTeamData, nKMUnitData.m_UnitUID, cPacket_GAME_RESPAWN_REQ.respawnPosX))
			{
				return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_NO_GAME_STATE;
			}
		}
		else
		{
			float rollbackTime = GetRollbackTime(nKMUnitTemplet);
			if (CheckRespawnCountMax(eNKM_TEAM_TYPE))
			{
				return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_MAX_UNIT_COUNT_SAME_TIME;
			}
			if (!RespawnUnit(bCheckOnly: false, 0f, rollbackTime, nKMGameTeamData, nKMGameRuntimeTeamData, nKMUnitData.m_UnitUID, cPacket_GAME_RESPAWN_REQ.respawnPosX))
			{
				return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_NO_GAME_STATE;
			}
		}
		respawnUnitUID = nKMUnitData.m_UnitUID;
		return NKM_ERROR_CODE.NEC_OK;
	}

	public virtual NKM_ERROR_CODE OnRecv(NKMPacket_GAME_UNIT_RETREAT_REQ cNKMPacket_GAME_UNIT_RETREAT_REQ, NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		NKMGameTeamData nKMGameTeamData = null;
		if (IsATeam(eNKM_TEAM_TYPE))
		{
			nKMGameTeamData = base.m_NKMGameData.m_NKMGameTeamDataA;
		}
		if (GetGameData().IsPVP() && GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_ASYNC_PVP)
		{
			return NKM_ERROR_CODE.NEC_FAIL_ASYNC_PVP_MANUAL_PLAY_DISABLE;
		}
		if (IsBTeam(eNKM_TEAM_TYPE))
		{
			nKMGameTeamData = base.m_NKMGameData.m_NKMGameTeamDataB;
		}
		if (nKMGameTeamData == null)
		{
			return NKMError.Build(NKM_ERROR_CODE.NEC_FAIL_ACCOUNT_INVALID_ID, -1L, base.m_NKMGameData.m_GameUID, $"teamType:{eNKM_TEAM_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3390);
		}
		if (nKMGameTeamData.m_DeckData == null)
		{
			return NKMError.Build(NKM_ERROR_CODE.NEC_FAIL_DECK_DATA_INVALID, -1L, base.m_NKMGameData.m_GameUID, $"teamType:{eNKM_TEAM_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3393);
		}
		if (IsATeam(eNKM_TEAM_TYPE))
		{
			_ = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA;
		}
		if (IsBTeam(eNKM_TEAM_TYPE))
		{
			_ = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB;
		}
		NKMUnitData unitDataByUnitUID = nKMGameTeamData.GetUnitDataByUnitUID(cNKMPacket_GAME_UNIT_RETREAT_REQ.unitUID);
		if (unitDataByUnitUID == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_NO_GAME_STATE;
		}
		if (!IsGameUnitAllDie(unitDataByUnitUID.m_UnitUID))
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = IsGameUnitAllInBattle(unitDataByUnitUID.m_UnitUID);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				return nKM_ERROR_CODE;
			}
			EventDieForReRespawn(unitDataByUnitUID.m_UnitUID);
			if (IsPVE() && NKMUnitManager.GetUnitTemplet(unitDataByUnitUID.m_UnitID) == null)
			{
				Log.Error($"Can not found unittemplet. unitId:{unitDataByUnitUID.m_UnitID}, unitUid:{unitDataByUnitUID.m_UnitUID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3421);
				return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_UNIT_TEMPLET_NULL;
			}
			return NKM_ERROR_CODE.NEC_OK;
		}
		return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_NO_GAME_STATE;
	}

	protected bool IsRespawnUnitWait(long unitUID)
	{
		for (int i = 0; i < m_listNKMGameUnitRespawnData.Count; i++)
		{
			NKMGameUnitRespawnData nKMGameUnitRespawnData = m_listNKMGameUnitRespawnData[i];
			if (nKMGameUnitRespawnData != null && nKMGameUnitRespawnData.m_UnitUID == unitUID)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanUseTacticalCommand(NKMGameRuntimeTeamData cNKMGameRuntimeTeamData, NKMTacticalCommandTemplet cNKMTacticalCommandTemplet, NKMTacticalCommandData cNKMTacticalCommandData)
	{
		if (cNKMTacticalCommandData.m_fCoolTimeNow > 0f)
		{
			return false;
		}
		if (cNKMGameRuntimeTeamData.m_fRespawnCost < cNKMTacticalCommandTemplet.GetNeedCost(cNKMTacticalCommandData))
		{
			return false;
		}
		return true;
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_TACTICAL_COMMAND_REQ cNKMPacket_GAME_TACTICAL_COMMAND_REQ, NKMPacket_GAME_TACTICAL_COMMAND_ACK cNKMPacket_GAME_TACTICAL_COMMAND_ACK, NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		NKMGameTeamData nKMGameTeamData = null;
		NKMGameRuntimeTeamData nKMGameRuntimeTeamData = null;
		if (IsATeam(eNKM_TEAM_TYPE))
		{
			nKMGameTeamData = GetGameData().m_NKMGameTeamDataA;
			nKMGameRuntimeTeamData = GetGameRuntimeData().m_NKMGameRuntimeTeamDataA;
		}
		if (IsBTeam(eNKM_TEAM_TYPE))
		{
			nKMGameTeamData = GetGameData().m_NKMGameTeamDataB;
			nKMGameRuntimeTeamData = GetGameRuntimeData().m_NKMGameRuntimeTeamDataB;
		}
		if (nKMGameTeamData == null || nKMGameRuntimeTeamData == null)
		{
			Log.Error($"Invalid TeamType. {eNKM_TEAM_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3489);
			return NKM_ERROR_CODE.NED_FAIL_INVALID_TEAM_TYPE;
		}
		NKMTacticalCommandData nKMTacticalCommandData = null;
		for (int i = 0; i < nKMGameTeamData.m_listTacticalCommandData.Count; i++)
		{
			NKMTacticalCommandData nKMTacticalCommandData2 = nKMGameTeamData.m_listTacticalCommandData[i];
			if (nKMTacticalCommandData2 != null && nKMTacticalCommandData2.m_TCID == cNKMPacket_GAME_TACTICAL_COMMAND_REQ.TCID)
			{
				nKMTacticalCommandData = nKMTacticalCommandData2;
				break;
			}
		}
		if (nKMTacticalCommandData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_TACTICAL_COMMAND_INVALID_TC;
		}
		cNKMPacket_GAME_TACTICAL_COMMAND_ACK.cTacticalCommandData.DeepCopyFromSource(nKMTacticalCommandData);
		NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(nKMTacticalCommandData.m_TCID);
		if (tacticalCommandTempletByID == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_TACTICAL_COMMAND_INVALID_TC;
		}
		if (!CanUseTacticalCommand(nKMGameRuntimeTeamData, tacticalCommandTempletByID, nKMTacticalCommandData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_TACTICAL_COMMAND_NO_COST;
		}
		UseTacticalCommand(tacticalCommandTempletByID, nKMTacticalCommandData, nKMGameRuntimeTeamData, eNKM_TEAM_TYPE);
		cNKMPacket_GAME_TACTICAL_COMMAND_ACK.cTacticalCommandData.DeepCopyFromSource(nKMTacticalCommandData);
		return NKM_ERROR_CODE.NEC_OK;
	}

	protected void AddCost(NKM_TEAM_TYPE eNKM_TEAM_TYPE, float fAddCost)
	{
		NKMGameRuntimeTeamData nKMGameRuntimeTeamData = null;
		if (IsATeam(eNKM_TEAM_TYPE))
		{
			nKMGameRuntimeTeamData = GetGameRuntimeData().m_NKMGameRuntimeTeamDataA;
		}
		else if (IsBTeam(eNKM_TEAM_TYPE))
		{
			nKMGameRuntimeTeamData = GetGameRuntimeData().m_NKMGameRuntimeTeamDataB;
		}
		nKMGameRuntimeTeamData.m_fRespawnCost += fAddCost;
		if (nKMGameRuntimeTeamData.m_fRespawnCost > 10f)
		{
			nKMGameRuntimeTeamData.m_fRespawnCost = 10f;
		}
		if (nKMGameRuntimeTeamData.m_fRespawnCost < 0f)
		{
			nKMGameRuntimeTeamData.m_fRespawnCost = 0f;
		}
	}

	private void UseTacticalCommand_RealAffect(NKMTacticalCommandTemplet cNKMTacticalCommandTemplet, NKMTacticalCommandData cNKMTacticalCommandData, NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		if (cNKMTacticalCommandData == null || cNKMTacticalCommandTemplet == null)
		{
			return;
		}
		if (cNKMTacticalCommandTemplet.m_NKM_TACTICAL_COMMAND_TYPE == NKM_TACTICAL_COMMAND_TYPE.NTCT_COMBO)
		{
			SetStopTime(NKMCommonConst.OPERATOR_SKILL_STOP_TIME, NKM_STOP_TIME_INDEX.NSTI_OPERATOR_SKILL);
		}
		bool bPvP = IsPVP();
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnit.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMUnit value = enumerator.Current.Value;
			if (IsEnemy(eNKM_TEAM_TYPE, value.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				if (!cNKMTacticalCommandTemplet.CheckEnemyTargetBuffExist(bPvP) || (IsBoss(value.GetUnitDataGame().m_GameUnitUID) && !cNKMTacticalCommandTemplet.m_bTargetBossEnemy))
				{
					continue;
				}
				List<string> enemyTeamBuffStrList = cNKMTacticalCommandTemplet.GetEnemyTeamBuffStrList(bPvP);
				if (enemyTeamBuffStrList != null)
				{
					for (int i = 0; i < enemyTeamBuffStrList.Count; i++)
					{
						value.AddBuffByStrID(enemyTeamBuffStrList[i], cNKMTacticalCommandData.m_Level, cNKMTacticalCommandData.m_Level, value.GetUnitDataGame().m_GameUnitUID, bUseMasterStat: false, bRangeSon: false);
					}
				}
			}
			if (IsSameTeam(eNKM_TEAM_TYPE, value.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				if (!cNKMTacticalCommandTemplet.CheckMyTeamTargetBuffExist(bPvP) || (IsBoss(value.GetUnitDataGame().m_GameUnitUID) && !cNKMTacticalCommandTemplet.m_bTargetBossMyTeam))
				{
					continue;
				}
				List<string> myTeamBuffStrList = cNKMTacticalCommandTemplet.GetMyTeamBuffStrList(bPvP);
				if (myTeamBuffStrList != null)
				{
					for (int j = 0; j < myTeamBuffStrList.Count; j++)
					{
						value.AddBuffByStrID(myTeamBuffStrList[j], cNKMTacticalCommandData.m_Level, cNKMTacticalCommandData.m_Level, value.GetUnitDataGame().m_GameUnitUID, bUseMasterStat: false, bRangeSon: false);
					}
				}
			}
			if (IsEnemy(eNKM_TEAM_TYPE, value.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				value.AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_TACTICAL_COMMAND_ENEMY_EFFECT);
			}
			else
			{
				value.AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_TACTICAL_COMMAND_MYTEAM_EFFECT);
			}
		}
		if (cNKMTacticalCommandTemplet.m_fCostPump > 0f)
		{
			float fAddCost = cNKMTacticalCommandTemplet.m_fCostPump + (float)(cNKMTacticalCommandData.m_Level - 1) * cNKMTacticalCommandTemplet.m_fCostPumpPerLevel;
			AddCost(eNKM_TEAM_TYPE, fAddCost);
		}
		if (string.IsNullOrEmpty(cNKMTacticalCommandTemplet.m_UnitStrID))
		{
			return;
		}
		NKMGameTeamData teamData = base.m_NKMGameData.GetTeamData(eNKM_TEAM_TYPE);
		NKMGameRuntimeTeamData myRuntimeTeamData = m_NKMGameRuntimeData.GetMyRuntimeTeamData(eNKM_TEAM_TYPE);
		bool bTeamA = eNKM_TEAM_TYPE.IsAteam();
		foreach (NKMUnitData listOperatorUnitDatum in teamData.m_listOperatorUnitData)
		{
			if (listOperatorUnitDatum != null)
			{
				if (!IsGameUnitAllDie(listOperatorUnitDatum.m_UnitUID))
				{
					EventDieForReRespawn(listOperatorUnitDatum.m_UnitUID);
					RespawnUnit(bCheckOnly: false, 0f, 0f, teamData, myRuntimeTeamData, listOperatorUnitDatum.m_UnitUID, GetShipRespawnPosX(bTeamA));
				}
				else
				{
					RespawnUnit(bCheckOnly: false, 0f, 0f, teamData, myRuntimeTeamData, listOperatorUnitDatum.m_UnitUID, GetShipRespawnPosX(bTeamA));
				}
			}
		}
	}

	private void UseTacticalCommand(NKMTacticalCommandTemplet cNKMTacticalCommandTemplet, NKMTacticalCommandData cNKMTacticalCommandData, NKMGameRuntimeTeamData cNKMGameRuntimeTeamData, NKM_TEAM_TYPE eNKM_TEAM_TYPE, float fReservedAffectTime = 0f)
	{
		if (cNKMTacticalCommandData == null || cNKMTacticalCommandTemplet == null || cNKMGameRuntimeTeamData == null)
		{
			return;
		}
		if (fReservedAffectTime <= 0f)
		{
			UseTacticalCommand_RealAffect(cNKMTacticalCommandTemplet, cNKMTacticalCommandData, eNKM_TEAM_TYPE);
		}
		else
		{
			switch (eNKM_TEAM_TYPE)
			{
			case NKM_TEAM_TYPE.NTT_A1:
				m_NKMReservedTacticalCommandTeamA.SetNewData(fReservedAffectTime, cNKMTacticalCommandTemplet, cNKMTacticalCommandData, eNKM_TEAM_TYPE);
				break;
			case NKM_TEAM_TYPE.NTT_B1:
				m_NKMReservedTacticalCommandTeamB.SetNewData(fReservedAffectTime, cNKMTacticalCommandTemplet, cNKMTacticalCommandData, eNKM_TEAM_TYPE);
				break;
			default:
				Log.Error("UseTacticalCommand, Not support team type found, teamType " + eNKM_TEAM_TYPE, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3688);
				break;
			}
		}
		float needCost = cNKMTacticalCommandTemplet.GetNeedCost(cNKMTacticalCommandData);
		cNKMGameRuntimeTeamData.m_fRespawnCost -= needCost;
		cNKMGameRuntimeTeamData.m_fUsedRespawnCost += needCost;
		float num = cNKMTacticalCommandTemplet.m_fCoolTime;
		if (base.m_NKMGameData.IsPVP())
		{
			int operatorId = 0;
			switch (eNKM_TEAM_TYPE)
			{
			case NKM_TEAM_TYPE.NTT_A1:
				operatorId = base.m_NKMGameData.m_NKMGameTeamDataA.m_Operator.id;
				break;
			case NKM_TEAM_TYPE.NTT_B1:
				operatorId = base.m_NKMGameData.m_NKMGameTeamDataB.m_Operator.id;
				break;
			}
			if (base.m_NKMGameData.IsBanOperator(operatorId))
			{
				int banOperatorLevel = base.m_NKMGameData.GetBanOperatorLevel(operatorId);
				float num2 = Math.Min(NKMUnitStatManager.m_fPercentPerBanLevel * (float)banOperatorLevel, NKMUnitStatManager.m_fMaxPercentPerBanLevel);
				num += num * num2 + Math.Min(NKMUnitStatManager.m_OperatorTacticalCommandPerBanLevel * (float)banOperatorLevel, NKMUnitStatManager.m_MaxOperatorTacticalCommandPerBanLevel);
			}
		}
		cNKMTacticalCommandData.m_fCoolTimeNow = num;
		cNKMTacticalCommandData.m_bCoolTimeOn = false;
		cNKMTacticalCommandData.m_UseCount++;
		cNKMTacticalCommandData.m_fComboResetCoolTimeNow = 0f;
		cNKMTacticalCommandData.m_ComboCount = 0;
		if (cNKMTacticalCommandTemplet.m_NKM_TACTICAL_COMMAND_TYPE == NKM_TACTICAL_COMMAND_TYPE.NTCT_ACTIVE)
		{
			SyncGameEvent(NKM_GAME_EVENT_TYPE.NGET_TACTICAL_COMMAND, eNKM_TEAM_TYPE, cNKMTacticalCommandTemplet.m_TCID);
		}
		else if (cNKMTacticalCommandTemplet.m_NKM_TACTICAL_COMMAND_TYPE == NKM_TACTICAL_COMMAND_TYPE.NTCT_COMBO)
		{
			SyncGameEvent(NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_SKILL_SUCCESS, eNKM_TEAM_TYPE, cNKMTacticalCommandTemplet.m_TCID, (int)cNKMTacticalCommandData.m_ComboCount);
		}
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_SHIP_SKILL_REQ cPacket_GAME_SHIP_SKILL_REQ, NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		if (IsATeam(eNKM_TEAM_TYPE))
		{
			_ = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA;
		}
		if (IsBTeam(eNKM_TEAM_TYPE))
		{
			_ = m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB;
		}
		NKMUnit unit = GetUnit(cPacket_GAME_SHIP_SKILL_REQ.gameUnitUID);
		if (unit == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_SHIP_SKILL_ACK_NO_UNIT;
		}
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVP_EVENT && GetGameData().m_bForcedAuto)
		{
			Log.Error($"Invalid GameType. Cannot use ship skill. gameType:{GetGameData().GetGameType()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3761);
			return NKM_ERROR_CODE.NEC_FAIL_EVENT_PVP_MANUAL_PLAY_DISABLE;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = unit.CanUseShipSkill(cPacket_GAME_SHIP_SKILL_REQ.shipSkillID);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		if (NKMShipSkillManager.GetShipSkillTempletByID(cPacket_GAME_SHIP_SKILL_REQ.shipSkillID) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_SHIP_SKILL_ACK_NO_SHIP_SKILL;
		}
		unit.UseShipSkill(cPacket_GAME_SHIP_SKILL_REQ.shipSkillID, cPacket_GAME_SHIP_SKILL_REQ.skillPosX);
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_CHECK_DIE_UNIT_REQ cNKMPacket_GAME_CHECK_DIE_UNIT_REQ)
	{
		SyncDieUnit(-1);
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_AUTO_RESPAWN_REQ cPacket_GAME_AUTO_RESPAWN_REQ, NKM_TEAM_TYPE eNKM_TEAM_TYPE, NKMUserData cSenderUserData)
	{
		if (m_NKMDungeonTemplet != null && !m_NKMDungeonTemplet.m_bCanUseAuto && cPacket_GAME_AUTO_RESPAWN_REQ.isAutoRespawn)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_AUTO_CAN_NOT_USE;
		}
		if (!CanUseAutoRespawn(cSenderUserData) && cPacket_GAME_AUTO_RESPAWN_REQ.isAutoRespawn)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_AUTO_CAN_NOT_USE;
		}
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVP_EVENT && GetGameData().m_bForcedAuto)
		{
			Log.Error($"cannot auto respawn. gameMode:{GetGameData().GetGameType()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3804);
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_AUTO_CAN_NOT_USE;
		}
		if (IsATeam(eNKM_TEAM_TYPE))
		{
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_bAutoRespawn = cPacket_GAME_AUTO_RESPAWN_REQ.isAutoRespawn;
		}
		else
		{
			if (!IsBTeam(eNKM_TEAM_TYPE))
			{
				Log.Error($"Invalid TeamData. {eNKM_TEAM_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3818);
				return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_AUTO_CAN_NOT_USE;
			}
			m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataB.m_bAutoRespawn = cPacket_GAME_AUTO_RESPAWN_REQ.isAutoRespawn;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_SPEED_2X_REQ cNKMPacket_GAME_SPEED_2X_REQ, NKMUserData cSenderUserData)
	{
		if (NKMGame.IsPVPSync(GetGameData().GetGameType()))
		{
			return NKMError.Build(NKM_ERROR_CODE.NEC_FAIL_GAME_SPEED_2X_NOT_SUPPORT_IN_PVP, -1L, GetGameData().m_GameUID, "", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3830);
		}
		GetGameRuntimeData().m_NKM_GAME_SPEED_TYPE = cNKMPacket_GAME_SPEED_2X_REQ.gameSpeedType;
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_AUTO_SKILL_CHANGE_REQ cNKMPacket_GAME_AUTO_SKILL_CHANGE_REQ, NKM_TEAM_TYPE eNKM_TEAM_TYPE, NKMUserData cSenderUserData)
	{
		NKMGameRuntimeData gameRuntimeData = GetGameRuntimeData();
		if (gameRuntimeData == null)
		{
			Log.Error($"GameRuntimeData is null. UserUid:{cSenderUserData.m_UserUID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3843);
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_AUTO_CAN_NOT_USE;
		}
		NKMGameRuntimeTeamData myRuntimeTeamData = gameRuntimeData.GetMyRuntimeTeamData(eNKM_TEAM_TYPE);
		if (myRuntimeTeamData == null)
		{
			Log.Error($"MyRunTimeTeamData is null. UserUid:{cSenderUserData.m_UserUID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3850);
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_AUTO_CAN_NOT_USE;
		}
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVP_EVENT && GetGameData().m_bForcedAuto)
		{
			Log.Error($"Invalid GameType. Cannot change auto skill option. UserUid:{cSenderUserData.m_UserUID} gameType:{GetGameData().GetGameType()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3858);
			return NKM_ERROR_CODE.NEC_FAIL_EVENT_PVP_MANUAL_PLAY_DISABLE;
		}
		myRuntimeTeamData.m_NKM_GAME_AUTO_SKILL_TYPE = cNKMPacket_GAME_AUTO_SKILL_CHANGE_REQ.gameAutoSkillType;
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE OnRecv(NKMPacket_GAME_USE_UNIT_SKILL_REQ cNKMPacket_GAME_USE_UNIT_SKILL_REQ, NKM_TEAM_TYPE eNKM_TEAM_TYPE, out byte skillStateID, NKMUserData cSenderUserData)
	{
		bool bHyper = false;
		skillStateID = 0;
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVP_EVENT && GetGameData().m_bForcedAuto)
		{
			Log.Error($"Invalid GameType. Cannot use unit skill. gameType:{GetGameData().GetGameType()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3875);
			return NKM_ERROR_CODE.NEC_FAIL_EVENT_PVP_MANUAL_PLAY_DISABLE;
		}
		NKMUnit unit = GetUnit(cNKMPacket_GAME_USE_UNIT_SKILL_REQ.gameUnitUID);
		if (unit != null && !unit.IsDyingOrDie())
		{
			if (unit.GetUnitDataGame().m_NKM_TEAM_TYPE == eNKM_TEAM_TYPE)
			{
				if (unit.CanUseManualSkill(bUse: true, out bHyper, out skillStateID))
				{
					return NKM_ERROR_CODE.NEC_OK;
				}
				return NKM_ERROR_CODE.NEC_FAIL_USE_UNIT_SKILL_CANT_USE_SKILL;
			}
			return NKMError.Build(NKM_ERROR_CODE.NEC_FAIL_USE_UNIT_SKILL_CANT_USE_SKILL, -1L, GetGameData().m_GameUID, $"m_GameUnitUID: {cNKMPacket_GAME_USE_UNIT_SKILL_REQ.gameUnitUID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGameServerHost.cs", 3891);
		}
		return NKM_ERROR_CODE.NEC_FAIL_USE_UNIT_SKILL_CANT_FIND_UNIT;
	}

	public virtual NKM_ERROR_CODE OnRecv(NKMPacket_GAME_DEV_RESPAWN_REQ cNKMPacket_GAME_DEV_RESPAWN_REQ, ref NKMPacket_GAME_DEV_RESPAWN_ACK cNKMPacket_GAME_DEV_RESPAWN_ACK, NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		NKMUnitData nKMUnitData;
		if (cNKMPacket_GAME_DEV_RESPAWN_REQ.unitLevel == 1)
		{
			nKMUnitData = new NKMUnitData();
			nKMUnitData.m_UnitUID = NpcUid.Get();
			nKMUnitData.m_UnitID = cNKMPacket_GAME_DEV_RESPAWN_REQ.unitID;
			nKMUnitData.m_UnitLevel = cNKMPacket_GAME_DEV_RESPAWN_REQ.unitLevel;
			int unitSkillCount = nKMUnitData.GetUnitSkillCount();
			for (int i = 0; i < 5; i++)
			{
				if (i < unitSkillCount)
				{
					nKMUnitData.m_aUnitSkillLevel[i] = 1;
				}
				else
				{
					nKMUnitData.m_aUnitSkillLevel[i] = 0;
				}
			}
			nKMUnitData.m_LimitBreakLevel = 0;
		}
		else
		{
			nKMUnitData = NKMDungeonManager.MakeUnitDataFromID(cNKMPacket_GAME_DEV_RESPAWN_REQ.unitID, NpcUid.Get(), cNKMPacket_GAME_DEV_RESPAWN_REQ.unitLevel, -1, 0);
		}
		float fRespawnValidLandTeamA = m_fRespawnValidLandTeamA;
		fRespawnValidLandTeamA = ((!IsATeam(eNKM_TEAM_TYPE)) ? m_fRespawnValidLandTeamB : m_fRespawnValidLandTeamA);
		float respawnPosX = cNKMPacket_GAME_DEV_RESPAWN_REQ.respawnPosX;
		if (respawnPosX.IsNearlyEqual(-1f))
		{
			respawnPosX = GetRespawnPosX(IsATeam(eNKM_TEAM_TYPE));
		}
		float minOffset = (GetGameData().IsPVP() ? NKMCommonConst.PVP_SUMMON_MIN_POS : NKMCommonConst.PVE_SUMMON_MIN_POS);
		respawnPosX = m_NKMMapTemplet.GetNearLandX(respawnPosX, IsATeam(eNKM_TEAM_TYPE), fRespawnValidLandTeamA, minOffset);
		if (!DevRespawnUnit(ref cNKMPacket_GAME_DEV_RESPAWN_ACK, nKMUnitData, respawnPosX, GetRespawnPosZ(), IsATeam(eNKM_TEAM_TYPE)))
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_NO_GAME_STATE;
		}
		cNKMPacket_GAME_DEV_RESPAWN_ACK.unitData = new NKMUnitData();
		cNKMPacket_GAME_DEV_RESPAWN_ACK.unitData.DeepCopyFrom(nKMUnitData);
		cNKMPacket_GAME_DEV_RESPAWN_ACK.teamType = eNKM_TEAM_TYPE;
		return NKM_ERROR_CODE.NEC_OK;
	}

	public bool DevRespawnUnit(ref NKMPacket_GAME_DEV_RESPAWN_ACK cNKMPacket_GAME_DEV_RESPAWN_ACK, NKMUnitData cNKMUnitData, float x, float z, bool bTeamA)
	{
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY && base.m_NKMGameData.GetGameType() != NKM_GAME_TYPE.NGT_DEV)
		{
			return false;
		}
		CreateGameUnitUID(cNKMUnitData);
		if (bTeamA)
		{
			CreatePoolUnit(base.m_NKMGameData.m_NKMGameTeamDataA, cNKMUnitData, 0, NKM_TEAM_TYPE.NTT_A1, bAsync: false);
		}
		else
		{
			CreatePoolUnit(base.m_NKMGameData.m_NKMGameTeamDataB, cNKMUnitData, 0, NKM_TEAM_TYPE.NTT_B1, bAsync: false);
		}
		CreateDynaminRespawnPoolUnit(bAsync: false);
		cNKMPacket_GAME_DEV_RESPAWN_ACK.dynamicRespawnUnitDataTeamA = base.m_NKMGameData.m_NKMGameTeamDataA.m_listDynamicRespawnUnitData;
		cNKMPacket_GAME_DEV_RESPAWN_ACK.dynamicRespawnUnitDataTeamB = base.m_NKMGameData.m_NKMGameTeamDataB.m_listDynamicRespawnUnitData;
		return RespawnUnit(cNKMUnitData, x, z);
	}

	public virtual NKM_ERROR_CODE OnRecv(NKMPacket_GAME_DEV_COOL_TIME_RESET_REQ cNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ)
	{
		if (!GetGameData().m_bLocal)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GAME_IS_NOT_LOCAL;
		}
		if (cNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ.isSkill)
		{
			DEV_SkillCoolTimeReset(cNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ.teamType);
		}
		else
		{
			DEV_HyperSkillCoolTimeReset(cNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ.teamType);
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public override void ChangeRemainGameTime(float timeSeconds, bool delta, bool bSendPacket)
	{
		float num;
		if (delta)
		{
			num = timeSeconds;
			m_NKMGameRuntimeData.m_fRemainGameTime += timeSeconds;
		}
		else
		{
			num = timeSeconds - m_NKMGameRuntimeData.m_fRemainGameTime;
			m_NKMGameRuntimeData.m_fRemainGameTime = timeSeconds;
		}
		if (bSendPacket)
		{
			SendDungeonEvent(NKM_TEAM_TYPE.NTT_A1, bRecordToGameData: false, NKM_EVENT_ACTION_TYPE.DELTA_GAME_TIME, (int)(num * 100f), null);
		}
	}

	public void BroadcastReactionEvent(NKMUnitReaction.ReactionEventType eventType, NKMUnit invoker, int count = 1, params float[] param)
	{
		foreach (KeyValuePair<short, NKMUnit> item in m_dicNKMUnit)
		{
			if (item.Key != invoker.GetUnitGameUID())
			{
				item.Value.OnReactionEvent(eventType, invoker, count, param);
			}
		}
	}
}
