using System.Collections.Generic;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Unit;

namespace NKM;

public class NKMEventRespawn : NKMUnitStateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public string m_UnitStrID = "";

	public NKMEventPosData m_EventPosData;

	public bool m_bUseMasterLevel = true;

	public bool m_bUseMasterData;

	public byte m_MaxCount = 1;

	public bool m_bMaxCountReRespawn;

	public float m_ReduceHPRate;

	public string m_RespawnState = string.Empty;

	public Dictionary<int, int> m_dicSummonSkin;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public void DeepCopyFromSource(NKMEventRespawn source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTime = source.m_fEventTime;
		m_bStateEndTime = source.m_bStateEndTime;
		m_UnitStrID = source.m_UnitStrID;
		m_EventPosData.DeepCopy(source.m_EventPosData);
		m_bUseMasterLevel = source.m_bUseMasterLevel;
		m_bUseMasterData = source.m_bUseMasterData;
		m_MaxCount = source.m_MaxCount;
		m_bMaxCountReRespawn = source.m_bMaxCountReRespawn;
		m_ReduceHPRate = source.m_ReduceHPRate;
		m_RespawnState = source.m_RespawnState;
		if (source.m_dicSummonSkin != null)
		{
			m_dicSummonSkin = new Dictionary<int, int>(source.m_dicSummonSkin);
		}
		else
		{
			m_dicSummonSkin = null;
		}
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_UnitStrID", ref m_UnitStrID);
		m_EventPosData.m_MoveBase = NKMEventPosData.MoveBase.EVENT_RESPAWN_POS;
		m_EventPosData.m_MoveOffset = NKMEventPosData.MoveOffset.MY_LOOK_DIR;
		bool rbValue = false;
		if (cNKMLua.GetData("m_bShipSkillPos", ref rbValue))
		{
			m_EventPosData.m_MoveBase = NKMEventPosData.MoveBase.SHIP_SKILL_POS;
		}
		m_EventPosData.LoadFromLua(cNKMLua);
		cNKMLua.GetData("m_bUseMasterLevel", ref m_bUseMasterLevel);
		cNKMLua.GetData("m_bUseMasterData", ref m_bUseMasterData);
		cNKMLua.GetData("m_MaxCount", ref m_MaxCount);
		cNKMLua.GetData("m_bMaxCountReRespawn", ref m_bMaxCountReRespawn);
		cNKMLua.GetData("m_ReduceHPRate", ref m_ReduceHPRate);
		cNKMLua.GetData("m_RespawnState", ref m_RespawnState);
		if (cNKMLua.OpenTable("m_dicSummonedUnitSkin"))
		{
			m_dicSummonSkin = new Dictionary<int, int>();
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				if (cNKMLua.GetData(1, out var rValue, 0) && cNKMLua.GetData(2, out var rValue2, 0))
				{
					m_dicSummonSkin.Add(rValue, rValue2);
				}
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		return true;
	}

	public void ValidateSummon(NKMUnitTempletBase templet)
	{
		string name = GetType().Name;
		if (templet.m_bMonster)
		{
			return;
		}
		NKMUnitTemplet summonTemplet;
		if (string.IsNullOrEmpty(m_UnitStrID))
		{
			NKMTempletError.Add("[EventCondition] " + templet.DebugName + " 유닛의 " + name + "의 m_UnitStrID가 비어있습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 1710);
		}
		else if (!NKMUnitManager.m_dicNKMUnitTempletStrID.TryGetValue(m_UnitStrID, out summonTemplet))
		{
			NKMTempletError.Add("[EventCondition] " + templet.DebugName + " 유닛의 " + name + "에있는 유닛아이디 (" + m_UnitStrID + ")가 잘못되었습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 1716);
		}
		else
		{
			if (summonTemplet.m_UnitTempletBase.m_bMonster)
			{
				return;
			}
			ValidateMasterSkillID(summonTemplet.m_listHitCriticalFeedBack);
			ValidateMasterSkillID(summonTemplet.m_listPhaseChangeData);
			ValidateMasterSkillID(summonTemplet.m_listReflectionBuffData);
			ValidateMasterSkillID(summonTemplet.m_listStaticBuffData);
			ValidateMasterSkillID(summonTemplet.m_listHitFeedBack);
			ValidateMasterSkillID(summonTemplet.m_listAccumStateChangePack);
			ValidateMasterSkillID(summonTemplet.m_listBuffUnitDieEvent);
			ValidateMasterSkillID(summonTemplet.m_listStartStateData);
			ValidateMasterSkillID(summonTemplet.m_listStandStateData);
			ValidateMasterSkillID(summonTemplet.m_listRunStateData);
			ValidateMasterSkillID(summonTemplet.m_listAttackStateData);
			ValidateMasterSkillID(summonTemplet.m_listAirAttackStateData);
			ValidateMasterSkillID(summonTemplet.m_listSkillStateData);
			ValidateMasterSkillID(summonTemplet.m_listAirSkillStateData);
			ValidateMasterSkillID(summonTemplet.m_listHyperSkillStateData);
			ValidateMasterSkillID(summonTemplet.m_listAirHyperSkillStateData);
			ValidateMasterSkillID(summonTemplet.m_listHitEvadeFeedBack);
			ValidateMasterSkillID(summonTemplet.m_listKillFeedBack);
			foreach (NKMUnitState value in summonTemplet.m_dicNKMUnitState.Values)
			{
				ValidateSummonState(value);
			}
		}
		void ValidateMasterSkillID(IEnumerable<IEventConditionOwner> lstEvent)
		{
			foreach (IEventConditionOwner item in lstEvent)
			{
				item.Validate(summonTemplet, templet);
				item.ValidateMasterSkillId(templet, summonTemplet.m_UnitTempletBase);
			}
		}
		void ValidateSummonState(NKMUnitState state)
		{
			if (state != null)
			{
				ValidateMasterSkillID(state.m_listNKMEventSpeed);
				ValidateMasterSkillID(state.m_listNKMEventSpeedX);
				ValidateMasterSkillID(state.m_listNKMEventSpeedY);
				ValidateMasterSkillID(state.m_listNKMEventMove);
				ValidateMasterSkillID(state.m_listNKMEventAttack);
				ValidateMasterSkillID(state.m_listNKMEventStopTime);
				ValidateMasterSkillID(state.m_listNKMEventInvincible);
				ValidateMasterSkillID(state.m_listNKMEventInvincibleGlobal);
				ValidateMasterSkillID(state.m_listNKMEventSuperArmor);
				ValidateMasterSkillID(state.m_listNKMEventSound);
				ValidateMasterSkillID(state.m_listNKMEventColor);
				ValidateMasterSkillID(state.m_listNKMEventCameraCrash);
				ValidateMasterSkillID(state.m_listNKMEventCameraMove);
				ValidateMasterSkillID(state.m_listNKMEventFadeWorld);
				ValidateMasterSkillID(state.m_listNKMEventDissolve);
				ValidateMasterSkillID(state.m_listNKMEventMotionBlur);
				ValidateMasterSkillID(state.m_listNKMEventEffect);
				ValidateMasterSkillID(state.m_listNKMEventHyperSkillCutIn);
				ValidateMasterSkillID(state.m_listNKMEventDamageEffect);
				ValidateMasterSkillID(state.m_listNKMEventDEStateChange);
				ValidateMasterSkillID(state.m_listNKMEventGameSpeed);
				ValidateMasterSkillID(state.m_listNKMEventBuff);
				ValidateMasterSkillID(state.m_listNKMEventStatus);
				ValidateMasterSkillID(state.m_listNKMEventRespawn);
				ValidateMasterSkillID(state.m_listNKMEventDie);
				ValidateMasterSkillID(state.m_listNKMEventChangeState);
				ValidateMasterSkillID(state.m_listNKMEventAgro);
				ValidateMasterSkillID(state.m_listNKMEventHeal);
				ValidateMasterSkillID(state.m_listNKMEventStun);
				ValidateMasterSkillID(state.m_listNKMEventCost);
				ValidateMasterSkillID(state.m_listNKMEventDefence);
				ValidateMasterSkillID(state.m_listNKMEventDispel);
				ValidateMasterSkillID(state.m_listNKMEventChangeCooltime);
				ValidateMasterSkillID(state.m_listNKMEventCatchEnd);
				ValidateMasterSkillID(state.m_listNKMEventFindTarget);
				ValidateMasterSkillID(state.m_listNKMEventTrigger);
				ValidateMasterSkillID(state.m_listNKMEventChangeRemainTime);
				ValidateMasterSkillID(state.m_listNKMEventVariable);
				ValidateMasterSkillID(state.m_listNKMEventConsume);
				ValidateMasterSkillID(state.m_listNKMEventSkillCutIn);
				ValidateMasterSkillID(state.m_listNKMEventTriggerBranch);
				ValidateMasterSkillID(state.m_listNKMEventReaction);
				if (state.m_NKMEventUnitChange != null)
				{
					state.m_NKMEventUnitChange.Validate(summonTemplet, templet);
					state.m_NKMEventUnitChange.ValidateMasterSkillId(templet, summonTemplet.m_UnitTempletBase);
				}
			}
		}
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.ApplyEventRespawn(this, cNKMUnit);
	}
}
