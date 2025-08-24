using System.Collections.Generic;
using Cs.Logging;
using Cs.Math;
using NKM.Game;

namespace NKM;

public class NKMUnitServer : NKMUnit
{
	private NKMGameServerHost m_NKMGameServerHost;

	public NKMUnitServer()
	{
		m_NKM_UNIT_CLASS_TYPE = NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER;
	}

	public override bool LoadUnit(NKMGame cNKMGame, NKMUnitData cNKMUnitData, short masterGameUnitUID, short gameUnitUID, float fNearTargetRange, NKM_TEAM_TYPE eNKM_TEAM_TYPE, bool bSub, bool bAsync)
	{
		if (!base.LoadUnit(cNKMGame, cNKMUnitData, masterGameUnitUID, gameUnitUID, fNearTargetRange, eNKM_TEAM_TYPE, bSub, bAsync))
		{
			return false;
		}
		m_NKMGameServerHost = (NKMGameServerHost)cNKMGame;
		return true;
	}

	public override bool SetDie(bool bCheckAllDie = true)
	{
		bool num = base.SetDie(bCheckAllDie);
		if (num && m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			m_NKMGameServerHost.SyncDieUnit(GetUnitDataGame().m_GameUnitUID);
		}
		return num;
	}

	public override void SetDying(bool bForce = false, bool bUnitChange = false)
	{
		if (!IsDyingOrDie())
		{
			m_NKMGame.m_GameRecord.AddPlayTime(this, m_NKMGame.GetGameRuntimeData().m_GameTime - m_RespawnTime);
			m_NKMGame.m_GameRecord.AddDieCount(this);
			if (!bUnitChange)
			{
				BroadcastReactionEvent(NKMUnitReaction.ReactionEventType.UNIT_DEAD, this, 1);
				OnReactionEvent(NKMUnitReaction.ReactionEventType.DEAD, this, 1);
			}
		}
		base.SetDying(bForce, bUnitChange);
	}

	public override void RespawnUnit(float fPosX, float fPosZ, float fJumpYPos, bool bUseRight = false, bool bRight = true, float fInitHP = 0f, bool bInitHPRate = false, float rollbackTime = 0f)
	{
		base.RespawnUnit(fPosX, fPosZ, fJumpYPos, bUseRight, bRight, fInitHP, bInitHPRate, rollbackTime);
		StateChangeToSTART(bForceChange: true, bImmediate: true);
		m_NKMGame.m_GameRecord.AddSummonCount(this);
		m_RespawnTime = m_NKMGame.GetGameRuntimeData().m_GameTime;
		BroadcastReactionEvent(NKMUnitReaction.ReactionEventType.RESPAWN, this, 1);
	}

	protected override void StateStart()
	{
		base.StateStart();
		if (m_fSyncRollbackTime > 0f)
		{
			ProcessRollbackEvents();
		}
	}

	public override void OnGameEnd()
	{
		if (!IsDyingOrDie())
		{
			m_NKMGame.m_GameRecord.AddPlayTime(this, m_NKMGame.GetGameRuntimeData().m_GameTime - m_RespawnTime);
		}
	}

	public override void PushSyncData()
	{
		if (m_UnitFrameData.m_bSyncShipSkill)
		{
			m_NKMGameServerHost.SyncShipSkillSync(this);
			m_UnitFrameData.m_bSyncShipSkill = false;
		}
		else
		{
			m_NKMGameServerHost.SyncUnitSync(this);
		}
		base.PushSyncData();
	}

	protected override void PushSimpleSyncData()
	{
		m_NKMGameServerHost.SyncUnitSimpleSync(this);
		base.PushSyncData();
	}

	protected void ProcessRollbackEvents()
	{
		if (m_UnitStateNow != null)
		{
			ProcessEventSpeedAndEventMoveRollback(m_fSyncRollbackTime);
			if (m_NKMGame.GetDungeonTemplet() == null || !m_NKMGame.GetDungeonTemplet().m_bNoTimeStop)
			{
				ProcessEventRollback(m_UnitStateNow.m_listNKMEventStopTime);
			}
			ProcessEventRollback(m_UnitStateNow.m_listNKMEventInvincibleGlobal);
			ProcessEventDamageEffectRollback();
			if (m_linklistDamageEffect != null && m_linklistDamageEffect.First != null)
			{
				ProcessEventRollback(m_UnitStateNow.m_listNKMEventDEStateChange);
			}
			ProcessEventBuffRollBack(m_fSyncRollbackTime);
			ProcessEventRollback(m_UnitStateNow.m_listNKMEventStatus);
			ProcessEventRespawnRollback(m_fSyncRollbackTime);
			ProcessEventRollback(m_UnitStateNow.m_listNKMEventAgro);
			ProcessEventRollback(m_UnitStateNow.m_listNKMEventHeal);
			ProcessEventRollback(m_UnitStateNow.m_listNKMEventCost);
			ProcessEventRollback(m_UnitStateNow.m_listNKMEventDispel);
			ProcessEventRollback(m_UnitStateNow.m_listNKMEventStun);
			ProcessEventRollback(m_UnitStateNow.m_listNKMEventChangeCooltime);
		}
	}

	private void ProcessEventRespawnRollback(float rollbackTIme)
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventRespawn.Count; i++)
		{
			NKMEventRespawn nKMEventRespawn = m_UnitStateNow.m_listNKMEventRespawn[i];
			if (nKMEventRespawn != null && !nKMEventRespawn.m_bStateEndTime && RollbackEventTimer(nKMEventRespawn.m_bAnimTime, nKMEventRespawn.m_fEventTime) && CheckEventCondition(nKMEventRespawn.m_Condition))
			{
				float eventStateTime = GetEventStateTime(nKMEventRespawn.m_bAnimTime, nKMEventRespawn.m_fEventTime);
				float rollbackTime = rollbackTIme - eventStateTime;
				if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
				{
					ApplyEventRespawn(nKMEventRespawn, this, rollbackTime);
				}
			}
		}
	}

	protected void ProcessEventSpeedAndEventMoveRollback(float rollbackTime)
	{
		if (m_UnitStateNow == null || m_UnitStateNow.m_bNoMove || m_UnitTemplet.m_bNoMove || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOMOVE))
		{
			return;
		}
		int num = m_UnitStateNow.m_listNKMEventSpeed.Count + m_UnitStateNow.m_listNKMEventSpeedX.Count + m_UnitStateNow.m_listNKMEventSpeedY.Count + m_UnitStateNow.m_listNKMEventMove.Count;
		if (num == 0)
		{
			return;
		}
		List<INKMUnitStateEventRollback> list = new List<INKMUnitStateEventRollback>(num);
		list.AddRange(m_UnitStateNow.m_listNKMEventSpeed);
		list.AddRange(m_UnitStateNow.m_listNKMEventSpeedX);
		list.AddRange(m_UnitStateNow.m_listNKMEventSpeedY);
		list.AddRange(m_UnitStateNow.m_listNKMEventMove);
		list.Sort(delegate(INKMUnitStateEventRollback x, INKMUnitStateEventRollback y)
		{
			int num3 = x.EventStartTime.CompareTo(y.EventStartTime);
			if (num3 != 0)
			{
				return num3;
			}
			bool value = x is NKMEventMove;
			return (y is NKMEventMove).CompareTo(value);
		});
		for (int num2 = 0; num2 < list.Count; num2++)
		{
			INKMUnitStateEventRollback iNKMUnitStateEventRollback = list[num2];
			if (!iNKMUnitStateEventRollback.bStateEnd && RollbackEventTimer(iNKMUnitStateEventRollback.bAnimTime, iNKMUnitStateEventRollback.EventStartTime) && CheckEventCondition(iNKMUnitStateEventRollback.Condition))
			{
				iNKMUnitStateEventRollback.ApplyEventRollback(m_NKMGame, this, rollbackTime);
			}
		}
	}

	private void ProcessEventDamageEffectRollback()
	{
		if (m_UnitStateNow.m_listNKMEventDamageEffect.Count == 0)
		{
			return;
		}
		NKMUnitSkillTemplet stateSkill = GetStateSkill(m_UnitStateNow);
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventDamageEffect.Count; i++)
		{
			NKMEventDamageEffect nKMEventDamageEffect = m_UnitStateNow.m_listNKMEventDamageEffect[i];
			if (nKMEventDamageEffect != null && !nKMEventDamageEffect.m_bStateEndTime && RollbackEventTimer(nKMEventDamageEffect.m_bAnimTime, nKMEventDamageEffect.m_fEventTime) && CheckEventCondition(nKMEventDamageEffect.m_Condition) && (!nKMEventDamageEffect.m_bIgnoreNoTarget || m_TargetUnit != null))
			{
				ApplyEventDamageEffect(nKMEventDamageEffect, stateSkill, m_UnitSyncData.m_PosX, m_UnitSyncData.m_JumpYPos, m_UnitSyncData.m_PosZ);
			}
		}
	}

	private void ProcessEventBuffRollBack(float rollbackTime)
	{
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventBuff.Count; i++)
		{
			NKMEventBuff nKMEventBuff = m_UnitStateNow.m_listNKMEventBuff[i];
			if (nKMEventBuff != null && !nKMEventBuff.m_bReflection && !nKMEventBuff.m_bStateEndTime && RollbackEventTimer(nKMEventBuff.m_bAnimTime, nKMEventBuff.m_fEventTime) && CheckEventCondition(nKMEventBuff.m_Condition))
			{
				nKMEventBuff.ApplyEvent(m_NKMGame, this);
			}
		}
	}

	private void ProcessEventRollback<T>(List<T> lstEvent) where T : INKMUnitStateEventRollback
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < lstEvent.Count; i++)
		{
			INKMUnitStateEventRollback iNKMUnitStateEventRollback = lstEvent[i];
			if (iNKMUnitStateEventRollback != null && !iNKMUnitStateEventRollback.bStateEnd && RollbackEventTimer(iNKMUnitStateEventRollback.bAnimTime, iNKMUnitStateEventRollback.EventStartTime) && CheckEventCondition(iNKMUnitStateEventRollback.Condition))
			{
				iNKMUnitStateEventRollback.ApplyEventRollback(m_NKMGame, this, m_fSyncRollbackTime);
			}
		}
	}

	protected override void ProcessEventRespawn(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventRespawn.Count; i++)
		{
			NKMEventRespawn nKMEventRespawn = m_UnitStateNow.m_listNKMEventRespawn[i];
			if (nKMEventRespawn != null && CheckEventCondition(nKMEventRespawn.m_Condition))
			{
				bool flag = false;
				if (nKMEventRespawn.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventRespawn.m_bAnimTime, nKMEventRespawn.m_fEventTime, bOneTime: true) && !nKMEventRespawn.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag && m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
				{
					ApplyEventRespawn(nKMEventRespawn, this);
				}
			}
		}
	}

	public override bool ApplyEventRespawn(NKMEventRespawn cNKMEventRespawn, NKMUnit invoker, float rollbackTime = 0f)
	{
		if (GetUnitFrameData().m_bNotCastSummon && !IsBoss())
		{
			return false;
		}
		short num = GetDynamicRespawnPool(cNKMEventRespawn);
		bool bMaxCountReRespawn = false;
		if (num <= 0 && cNKMEventRespawn.m_bMaxCountReRespawn)
		{
			bMaxCountReRespawn = true;
			num = GetDynamicRespawnPoolReRespawn(cNKMEventRespawn);
		}
		(NKMEventPosData.EventPosExtraUnitType, NKMUnit) tuple = (NKMEventPosData.EventPosExtraUnitType.SUMMON_INVOKER, invoker);
		float eventPosX = GetEventPosX(cNKMEventRespawn.m_EventPosData, IsATeam(), tuple);
		float respawnPosZ = m_NKMGameServerHost.GetRespawnPosZ();
		NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(cNKMEventRespawn.m_UnitStrID);
		if (unitTemplet == null)
		{
			Log.Error("Can not found UnitTemplet. UnitStrId:" + cNKMEventRespawn.m_UnitStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitServer.cs", 469);
			return false;
		}
		if (!unitTemplet.m_fForceRespawnZposMin.IsNearlyEqual(-1f) || !unitTemplet.m_fForceRespawnZposMax.IsNearlyEqual(-1f))
		{
			respawnPosZ = m_NKMGameServerHost.GetRespawnPosZ(unitTemplet.m_fForceRespawnZposMin, unitTemplet.m_fForceRespawnZposMax);
		}
		if (m_NKMGameServerHost.DynamicRespawnUnitReserve(bMaxCountReRespawn, num, eventPosX, respawnPosZ, 0f, bUseRight: false, bRight: true, 0f, cNKMEventRespawn.m_RespawnState, rollbackTime) != null)
		{
			AddDamage(bAttackCountOver: false, GetMaxHP() * cNKMEventRespawn.m_ReduceHPRate, NKM_DAMAGE_RESULT_TYPE.NDRT_NO_MARK, GetUnitDataGame().m_GameUnitUID, GetUnitDataGame().m_NKM_TEAM_TYPE);
		}
		return true;
	}

	protected override void ProcessEventUnitChange(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		if (m_UnitStateNow.m_NKMEventUnitChange != null)
		{
			if (!CheckEventCondition(m_UnitStateNow.m_NKMEventUnitChange.m_Condition))
			{
				return;
			}
			bool flag = false;
			if (m_UnitStateNow.m_NKMEventUnitChange.m_bStateEndTime && bStateEnd)
			{
				flag = true;
			}
			else if (EventTimer(m_UnitStateNow.m_NKMEventUnitChange.m_bAnimTime, m_UnitStateNow.m_NKMEventUnitChange.m_fEventTime, bOneTime: true) && !m_UnitStateNow.m_NKMEventUnitChange.m_bStateEndTime)
			{
				flag = true;
			}
			if (flag && m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
			{
				short unitChangeRespawnPool = GetUnitChangeRespawnPool(m_UnitStateNow.m_NKMEventUnitChange.m_UnitStrID);
				m_NKMGameServerHost.DynamicRespawnUnitReserve(bMaxCountReRespawn: false, unitChangeRespawnPool, GetUnitSyncData().m_PosX, GetUnitSyncData().m_PosZ, GetUnitSyncData().m_JumpYPos, bUseRight: true, GetUnitSyncData().m_bRight, GetHPRate())?.GetUnitData().SetDungeonRespawnUnitTemplet(GetUnitData().m_DungeonRespawnUnitTemplet);
			}
		}
		base.ProcessEventUnitChange(bStateEnd);
	}

	public override void InvokeTrigger(string name)
	{
		int triggerID = m_UnitTemplet.GetTriggerID(name);
		InvokeTrigger(triggerID);
	}

	public override void InvokeTrigger(int id)
	{
		if (m_UnitTemplet.GetTriggerSet(id) != null)
		{
			AddInvokedTrigger(this, id);
			RegisterInvokedTrigger(id, this);
		}
	}

	public override void InvokeTrigger(NKMUnit ownerUnit, int triggerID)
	{
		if (ownerUnit.GetUnitTemplet().GetTriggerSet(triggerID) != null)
		{
			AddInvokedTrigger(ownerUnit, triggerID);
			RegisterInvokedTrigger(triggerID, ownerUnit);
		}
	}

	public override void BroadcastReactionEvent(NKMUnitReaction.ReactionEventType eventType, NKMUnit invoker, int count = 1, params float[] param)
	{
		if (invoker.WillInteractWithGameUnits())
		{
			m_NKMGameServerHost.BroadcastReactionEvent(eventType, invoker, count, param);
		}
	}
}
