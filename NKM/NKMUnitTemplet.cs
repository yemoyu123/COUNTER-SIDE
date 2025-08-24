using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Game;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Unit;

namespace NKM;

public sealed class NKMUnitTemplet
{
	public class RespawnEffectData
	{
		public NKMEventConditionV2 Condition;

		public bool m_RespawnEffectFullMap;

		public bool m_RespawnEffectUseUnitSize = true;

		public NKMMinMaxFloat m_RespawnEffectRange = new NKMMinMaxFloat();

		public NKMEventConditionV2 m_RespawnEffectCondition;

		public float m_RespawnOffset;

		public float m_RespawnEffectColorR = -1f;

		public float m_RespawnEffectColorG = -1f;

		public float m_RespawnEffectColorB = -1f;

		public float m_RespawnEffectColorA = -1f;

		public float m_RespawnAffectColorR = -1f;

		public float m_RespawnAffectColorG = -1f;

		public float m_RespawnAffectColorB = -1f;

		public bool HasRespawnEffectRange
		{
			get
			{
				if (!m_RespawnEffectFullMap)
				{
					return m_RespawnEffectRange.m_Max != m_RespawnEffectRange.m_Min;
				}
				return true;
			}
		}

		public bool HasRespawnOffset => m_RespawnOffset != 0f;

		public RespawnEffectData Clone()
		{
			return new RespawnEffectData
			{
				Condition = NKMEventConditionV2.Clone(Condition),
				m_RespawnEffectFullMap = m_RespawnEffectFullMap,
				m_RespawnEffectUseUnitSize = m_RespawnEffectUseUnitSize,
				m_RespawnEffectRange = NKMMinMaxFloat.Clone(m_RespawnEffectRange),
				m_RespawnEffectCondition = NKMEventConditionV2.Clone(m_RespawnEffectCondition),
				m_RespawnOffset = m_RespawnOffset,
				m_RespawnEffectColorR = m_RespawnEffectColorR,
				m_RespawnEffectColorG = m_RespawnEffectColorG,
				m_RespawnEffectColorB = m_RespawnEffectColorB,
				m_RespawnEffectColorA = m_RespawnEffectColorA,
				m_RespawnAffectColorR = m_RespawnAffectColorR,
				m_RespawnAffectColorG = m_RespawnAffectColorG,
				m_RespawnAffectColorB = m_RespawnAffectColorB
			};
		}

		public static RespawnEffectData LoadFromLua(NKMLua cNKMLua)
		{
			RespawnEffectData respawnEffectData = new RespawnEffectData();
			respawnEffectData.Condition = NKMEventConditionV2.LoadFromLUA(cNKMLua);
			respawnEffectData.m_RespawnEffectRange.LoadFromLua(cNKMLua, "m_RespawnEffectRange");
			int num = (int)(0u | (respawnEffectData.m_RespawnEffectRange.HasValue() ? 1u : 0u) | (cNKMLua.GetData("m_RespawnEffectUseUnitSize", ref respawnEffectData.m_RespawnEffectUseUnitSize) ? 1u : 0u) | (cNKMLua.GetData("m_RespawnEffectFullMap", ref respawnEffectData.m_RespawnEffectFullMap) ? 1u : 0u)) | (cNKMLua.GetData("m_RespawnOffset", ref respawnEffectData.m_RespawnOffset) ? 1 : 0);
			respawnEffectData.m_RespawnEffectCondition = NKMEventConditionV2.LoadFromLUA(cNKMLua, "m_RespawnEffectCondition");
			if (((uint)num | (cNKMLua.GetData("m_RespawnEffectColorR", ref respawnEffectData.m_RespawnEffectColorR) ? 1u : 0u) | (cNKMLua.GetData("m_RespawnEffectColorG", ref respawnEffectData.m_RespawnEffectColorG) ? 1u : 0u) | (cNKMLua.GetData("m_RespawnEffectColorB", ref respawnEffectData.m_RespawnEffectColorB) ? 1u : 0u) | (cNKMLua.GetData("m_RespawnEffectColorA", ref respawnEffectData.m_RespawnEffectColorA) ? 1u : 0u) | (cNKMLua.GetData("m_RespawnAffectColorR", ref respawnEffectData.m_RespawnAffectColorR) ? 1u : 0u) | (cNKMLua.GetData("m_RespawnAffectColorG", ref respawnEffectData.m_RespawnAffectColorG) ? 1u : 0u) | (cNKMLua.GetData("m_RespawnAffectColorB", ref respawnEffectData.m_RespawnAffectColorB) ? 1u : 0u)) != 0)
			{
				return respawnEffectData;
			}
			return null;
		}
	}

	public delegate bool ValidateFunc<T>(T target);

	public delegate string ValidateErrorMsgFactory<T>(T target);

	public NKMUnitTempletBase m_UnitTempletBase;

	public NKMUnitStatTemplet m_StatTemplet;

	public float m_SpriteScale = 1f;

	public float m_SpriteOffsetX;

	public float m_SpriteOffsetY;

	public float m_fForceRespawnXpos = -1f;

	public float m_fForceRespawnZposMin = -1f;

	public float m_fForceRespawnZposMax = -1f;

	public float m_UnitSizeX;

	public float m_UnitSizeY;

	public NKC_TEAM_COLOR_TYPE m_NKC_TEAM_COLOR_TYPE = NKC_TEAM_COLOR_TYPE.NTCT_FULL;

	public float m_fShadowOffsetX;

	public float m_fShadowOffsetY;

	public float m_fShadowScaleX = 1f;

	public float m_fShadowScaleY = 1f;

	public float m_fShadowRotateX;

	public float m_fShadowRotateZ;

	public float m_fBuffEffectScaleFactor = 1f;

	public bool m_bShowGage = true;

	public bool m_bGageSmall = true;

	public float m_fGageOffsetX;

	public float m_fGageOffsetY = 290f;

	public float m_fRespawnCoolTime = 1f;

	public float m_fDieCompleteTime = 2f;

	public bool m_bDieDeActive = true;

	public bool m_bUseMotionBlur;

	public bool m_bNoDamageState;

	public bool m_bNoDamageDownState;

	public bool m_bNoDamageStopTime;

	public bool m_Invincible;

	public NKM_SUPER_ARMOR_LEVEL m_SuperArmorLevel;

	public float m_ColorR = 1f;

	public float m_ColorG = 1f;

	public float m_ColorB = 1f;

	public float m_fAirHigh;

	public bool m_bNoMove;

	public bool m_bNoRun;

	public float m_SpeedRun;

	public float m_SpeedJump;

	public float m_fReloadAccel = 3000f;

	public float m_fGAccel = 2000f;

	public float m_fMaxGSpeed = -3000f;

	public float m_fDamageBackFactor = 1f;

	public float m_fDamageUpFactor = 1f;

	public bool m_bNoMapLimit;

	public bool m_SeeTarget = true;

	public bool m_bSeeMoreEnemy;

	public NKMFindTargetData m_TargetFindData = new NKMFindTargetData();

	public float m_SeeRangeMax;

	public NKMFindTargetData m_SubTargetFindData;

	public float m_TargetNearRange = 1f;

	public float m_fPatrolRange;

	public int m_PenetrateDefence;

	public string m_StateChangeSilence = "";

	public string m_StateChangeStun = "";

	public string m_StateChangeSleep = "";

	public string m_StateChangeConfuse = "";

	public float m_MapPosOverStatePos;

	public string m_MapPosOverState = "";

	public HashSet<NKM_UNIT_STATUS_EFFECT> m_listFixedStatusEffect = new HashSet<NKM_UNIT_STATUS_EFFECT>();

	public HashSet<NKM_UNIT_STATUS_EFFECT> m_listFixedStatusImmune = new HashSet<NKM_UNIT_STATUS_EFFECT>();

	public List<NKMAccumStateChangePack> m_listAccumStateChangePack = new List<NKMAccumStateChangePack>();

	public List<NKMHitFeedBack> m_listHitFeedBack = new List<NKMHitFeedBack>();

	public List<NKMHitFeedBack> m_listHitEvadeFeedBack = new List<NKMHitFeedBack>();

	public List<NKMHitFeedBack> m_listHitCriticalFeedBack = new List<NKMHitFeedBack>();

	public List<NKMKillFeedBack> m_listKillFeedBack = new List<NKMKillFeedBack>();

	public List<NKMBuffUnitDieEvent> m_listBuffUnitDieEvent = new List<NKMBuffUnitDieEvent>();

	public List<NKMStaticBuffData> m_listReflectionBuffData = new List<NKMStaticBuffData>();

	public List<NKMStaticBuffData> m_listStaticBuffData = new List<NKMStaticBuffData>();

	public List<NKMPhaseChangeData> m_listPhaseChangeData = new List<NKMPhaseChangeData>();

	public List<NKMCommonStateData> m_listStartStateData = new List<NKMCommonStateData>();

	public List<NKMCommonStateData> m_listStandStateData = new List<NKMCommonStateData>();

	public List<NKMCommonStateData> m_listRunStateData = new List<NKMCommonStateData>();

	public List<NKMAttackStateData> m_listAttackStateData = new List<NKMAttackStateData>();

	public List<NKMAttackStateData> m_listAirAttackStateData = new List<NKMAttackStateData>();

	public List<NKMAttackStateData> m_listSkillStateData = new List<NKMAttackStateData>();

	public List<NKMAttackStateData> m_listAirSkillStateData = new List<NKMAttackStateData>();

	public List<NKMAttackStateData> m_listHyperSkillStateData = new List<NKMAttackStateData>();

	public List<NKMAttackStateData> m_listAirHyperSkillStateData = new List<NKMAttackStateData>();

	public List<NKMAttackStateData> m_listPassiveAttackStateData = new List<NKMAttackStateData>();

	public List<NKMAttackStateData> m_listAirPassiveAttackStateData = new List<NKMAttackStateData>();

	public Dictionary<int, NKMUnitTriggerSet> m_dicTriggerSet;

	public Dictionary<string, int> m_dicTriggerSetID;

	public List<NKMTriggerRepeatData> m_listTriggerRepeatData;

	public Dictionary<string, NKMEventConditionV2> m_dicEventConditonV2Macro;

	public Dictionary<int, NKMUnitReaction> m_dicReaction;

	public Dictionary<string, int> m_dicReactionID;

	public List<RespawnEffectData> m_lstRespawnEffectData;

	public Dictionary<int, NKM_UNIT_ATTACK_STATE_TYPE> m_dicAttackStateType = new Dictionary<int, NKM_UNIT_ATTACK_STATE_TYPE>();

	public string m_DyingState = "USN_DAMAGE_DOWN";

	public Dictionary<string, NKMUnitState> m_dicNKMUnitState = new Dictionary<string, NKMUnitState>();

	public Dictionary<short, NKMUnitState> m_dicNKMUnitStateID = new Dictionary<short, NKMUnitState>();

	private static Action<NKMUnitTemplet, string> stateErrorCallback = DefaultStateErrorHandler;

	public float m_fMaxRollbackTime;

	public NKM_UNIT_ATTACK_STATE_TYPE GetAttackStateType(int stateID)
	{
		if (m_dicAttackStateType.TryGetValue(stateID, out var value))
		{
			return value;
		}
		return NKM_UNIT_ATTACK_STATE_TYPE.INVALID;
	}

	public NKMUnitTemplet()
	{
		m_TargetFindData.m_bUseUnitSize = NKMCommonConst.FIND_TARGET_USE_UNITSIZE;
	}

	private void ValidateList<T>(List<T> lstEvent, ValidateFunc<T> validateFunc, ValidateErrorMsgFactory<T> errorMsgFactory)
	{
		if (lstEvent == null)
		{
			return;
		}
		foreach (T item in lstEvent)
		{
			if (!validateFunc(item))
			{
				Log.ErrorAndExit(errorMsgFactory(item), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 251);
			}
		}
	}

	private void ValidateConditionOwner(IEnumerable<IEventConditionOwner> lstConditionOwner)
	{
		if (lstConditionOwner == null)
		{
			return;
		}
		foreach (IEventConditionOwner item in lstConditionOwner)
		{
			item.ValidateSkillId(m_UnitTempletBase);
			item.Validate(this, null);
		}
	}

	private void ValidateConditionOwner(IEventConditionOwner conditionOwner)
	{
		if (conditionOwner != null)
		{
			conditionOwner.ValidateSkillId(m_UnitTempletBase);
			conditionOwner.Validate(this, null);
		}
	}

	public void Validate()
	{
		if (m_listStaticBuffData != null)
		{
			foreach (NKMStaticBuffData listStaticBuffDatum in m_listStaticBuffData)
			{
				if (!listStaticBuffDatum.Validate())
				{
					Log.ErrorAndExit("[UnitTemplet] m_listStaticBuffData is invalid. UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "], StaticBuffDataStrID [" + listStaticBuffDatum.m_BuffStrID + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 286);
				}
			}
		}
		if (m_listReflectionBuffData != null)
		{
			foreach (NKMStaticBuffData listReflectionBuffDatum in m_listReflectionBuffData)
			{
				if (!listReflectionBuffDatum.Validate())
				{
					Log.ErrorAndExit("[UnitTemplet] m_listReflectionBuffData is invalid. UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "], ReflectionBuffDataStrID [" + listReflectionBuffDatum.m_BuffStrID + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 297);
				}
			}
		}
		ValidateList(m_listHitFeedBack, (NKMHitFeedBack ev) => ev.Validate(), (NKMHitFeedBack ev) => "[UnitTemplet] m_listHitFeedBack is invalid. UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "], BuffStrID [" + ev.m_BuffStrID + "]");
		ValidateList(m_listHitCriticalFeedBack, (NKMHitFeedBack ev) => ev.Validate(), (NKMHitFeedBack ev) => "[UnitTemplet] m_listHitCriticalFeedBack is invalid. UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "], BuffStrID [" + ev.m_BuffStrID + "]");
		ValidateList(m_listHitEvadeFeedBack, (NKMHitFeedBack ev) => ev.Validate(), (NKMHitFeedBack ev) => "[UnitTemplet] m_listHitEvadeFeedBack is invalid. UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "], BuffStrID [" + ev.m_BuffStrID + "]");
		ValidateList(m_listKillFeedBack, (NKMKillFeedBack ev) => ev.Validate(), (NKMKillFeedBack ev) => "[UnitTemplet] m_listKillFeedBack is invalid. UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "], m_BuffName [" + ev.m_BuffName + "]");
		if (m_dicNKMUnitState != null)
		{
			foreach (NKMUnitState value in m_dicNKMUnitState.Values)
			{
				value.Validate(this);
			}
		}
		if (GetUnitState("USN_DAMAGE_STUN") == null)
		{
			Log.ErrorAndExit("[UnitTemplet] UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "] Has no stun state USN_DAMAGE_STUN", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 329);
		}
		if (GetUnitState("USN_DAMAGE_SLEEP") == null)
		{
			Log.ErrorAndExit("[UnitTemplet] UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "] Has no sleep state USN_DAMAGE_SLEEP", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 333);
		}
		if (GetUnitState("USN_DAMAGE_FEAR") == null)
		{
			Log.ErrorAndExit("[UnitTemplet] UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "] Has no fear state USN_DAMAGE_FEAR", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 337);
		}
		if (GetUnitState("USN_DAMAGE_FEAR_NOMOVE") == null)
		{
			Log.ErrorAndExit("[UnitTemplet] UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "] Has no fear state USN_DAMAGE_FEAR_NOMOVE", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 341);
		}
		if (GetUnitState("USN_DAMAGE_FREEZE") == null)
		{
			Log.ErrorAndExit("[UnitTemplet] UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "] Has no freeze state USN_DAMAGE_FREEZE", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 345);
		}
		if (GetUnitState("USN_DAMAGE_HOLD") == null)
		{
			Log.ErrorAndExit("[UnitTemplet] UnitStrID [" + m_UnitTempletBase.m_UnitStrID + "] Has no hold state USN_DAMAGE_HOLD", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 349);
		}
		if (m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			foreach (string item in m_UnitTempletBase.m_lstSkillStrID)
			{
				NKMShipSkillTemplet shipSkillTempletByStrID = NKMShipSkillManager.GetShipSkillTempletByStrID(item);
				if (shipSkillTempletByStrID == null)
				{
					Log.ErrorAndExit("[UnitTemplet] 전함 스킬 아이디가 올바르지 않음. UnitStrID:" + m_UnitTempletBase.m_UnitStrID + " skillStrID:" + item, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 361);
				}
				else if (!shipSkillTempletByStrID.m_NKM_SKILL_TYPE.IsStatHoldType() && GetUnitState(shipSkillTempletByStrID.m_UnitStateName) == null)
				{
					Log.ErrorAndExit("[UnitTemplet] 전함 스킬에 따른 유닛 상태 정보가 존재하지 않음 UnitStrID:" + m_UnitTempletBase.m_UnitStrID + " skillStrID:" + item + " skillUnitStateName:" + shipSkillTempletByStrID.m_UnitStateName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 366);
				}
			}
		}
		ValidateConditionOwner(m_listHitCriticalFeedBack);
		ValidateConditionOwner(m_listPhaseChangeData);
		ValidateConditionOwner(m_listReflectionBuffData);
		ValidateConditionOwner(m_listStaticBuffData);
		ValidateConditionOwner(m_listHitFeedBack);
		ValidateConditionOwner(m_listAccumStateChangePack);
		ValidateConditionOwner(m_listBuffUnitDieEvent);
		ValidateConditionOwner(m_listStartStateData);
		ValidateConditionOwner(m_listStandStateData);
		ValidateConditionOwner(m_listRunStateData);
		ValidateConditionOwner(m_listAttackStateData);
		ValidateConditionOwner(m_listAirAttackStateData);
		ValidateConditionOwner(m_listSkillStateData);
		ValidateConditionOwner(m_listAirSkillStateData);
		ValidateConditionOwner(m_listHyperSkillStateData);
		ValidateConditionOwner(m_listAirHyperSkillStateData);
		ValidateConditionOwner(m_listPassiveAttackStateData);
		ValidateConditionOwner(m_listAirPassiveAttackStateData);
		ValidateConditionOwner(m_listHitEvadeFeedBack);
		ValidateConditionOwner(m_listKillFeedBack);
		ValidateConditionOwner(m_listTriggerRepeatData);
		foreach (NKMUnitState value2 in m_dicNKMUnitState.Values)
		{
			ValidateState(value2);
		}
		foreach (NKMEventRespawn item2 in m_dicNKMUnitState.Values.SelectMany((NKMUnitState e) => e.m_listNKMEventRespawn))
		{
			item2.ValidateSummon(m_UnitTempletBase);
		}
		void ValidateState(NKMUnitState state)
		{
			if (state != null)
			{
				ValidateConditionOwner(state.m_listNKMEventSpeed);
				ValidateConditionOwner(state.m_listNKMEventSpeedX);
				ValidateConditionOwner(state.m_listNKMEventSpeedY);
				ValidateConditionOwner(state.m_listNKMEventMove);
				ValidateConditionOwner(state.m_listNKMEventAttack);
				ValidateConditionOwner(state.m_listNKMEventStopTime);
				ValidateConditionOwner(state.m_listNKMEventInvincible);
				ValidateConditionOwner(state.m_listNKMEventInvincibleGlobal);
				ValidateConditionOwner(state.m_listNKMEventSuperArmor);
				ValidateConditionOwner(state.m_listNKMEventSound);
				ValidateConditionOwner(state.m_listNKMEventColor);
				ValidateConditionOwner(state.m_listNKMEventCameraCrash);
				ValidateConditionOwner(state.m_listNKMEventCameraMove);
				ValidateConditionOwner(state.m_listNKMEventFadeWorld);
				ValidateConditionOwner(state.m_listNKMEventDissolve);
				ValidateConditionOwner(state.m_listNKMEventMotionBlur);
				ValidateConditionOwner(state.m_listNKMEventEffect);
				ValidateConditionOwner(state.m_listNKMEventHyperSkillCutIn);
				ValidateConditionOwner(state.m_listNKMEventDamageEffect);
				ValidateConditionOwner(state.m_listNKMEventDEStateChange);
				ValidateConditionOwner(state.m_listNKMEventGameSpeed);
				ValidateConditionOwner(state.m_listNKMEventBuff);
				ValidateConditionOwner(state.m_listNKMEventStatus);
				ValidateConditionOwner(state.m_listNKMEventRespawn);
				ValidateConditionOwner(state.m_listNKMEventDie);
				ValidateConditionOwner(state.m_listNKMEventChangeState);
				ValidateConditionOwner(state.m_listNKMEventAgro);
				ValidateConditionOwner(state.m_listNKMEventHeal);
				ValidateConditionOwner(state.m_listNKMEventStun);
				ValidateConditionOwner(state.m_listNKMEventCost);
				ValidateConditionOwner(state.m_listNKMEventDefence);
				ValidateConditionOwner(state.m_listNKMEventDispel);
				ValidateConditionOwner(state.m_listNKMEventChangeCooltime);
				ValidateConditionOwner(state.m_listNKMEventCatchEnd);
				ValidateConditionOwner(state.m_listNKMEventFindTarget);
				ValidateConditionOwner(state.m_listNKMEventTrigger);
				ValidateConditionOwner(state.m_listNKMEventChangeRemainTime);
				ValidateConditionOwner(state.m_listNKMEventVariable);
				ValidateConditionOwner(state.m_NKMEventUnitChange);
				ValidateConditionOwner(state.m_listNKMEventConsume);
				ValidateConditionOwner(state.m_listNKMEventSkillCutIn);
				ValidateConditionOwner(state.m_listNKMEventTriggerBranch);
				ValidateConditionOwner(state.m_listNKMEventReaction);
			}
		}
	}

	public bool LoadFromLUA(NKMLua cNKMLua, NKMUnitTempletBase cNKMUnitTempletBase)
	{
		m_UnitTempletBase = cNKMUnitTempletBase;
		m_StatTemplet = NKMUnitManager.GetUnitStatTemplet(m_UnitTempletBase.m_UnitID);
		m_TargetFindData.m_FindTargetType = cNKMUnitTempletBase.m_NKM_FIND_TARGET_TYPE;
		return LoadFromLUA(cNKMLua, cNKMUnitTempletBase.m_UnitStrID);
	}

	public bool LoadFromLUA(NKMLua cNKMLua, string unitStrId)
	{
		try
		{
			cNKMLua.GetData("m_SpriteScale", ref m_SpriteScale);
			cNKMLua.GetData("m_SpriteOffsetX", ref m_SpriteOffsetX);
			cNKMLua.GetData("m_SpriteOffsetY", ref m_SpriteOffsetY);
			cNKMLua.GetData("m_fForceRespawnXpos", ref m_fForceRespawnXpos);
			cNKMLua.GetData("m_fForceRespawnZposMin", ref m_fForceRespawnZposMin);
			cNKMLua.GetData("m_fForceRespawnZposMax", ref m_fForceRespawnZposMax);
			cNKMLua.GetData("m_UnitSizeX", ref m_UnitSizeX);
			cNKMLua.GetData("m_UnitSizeY", ref m_UnitSizeY);
			cNKMLua.GetData("m_NKC_TEAM_COLOR_TYPE", ref m_NKC_TEAM_COLOR_TYPE);
			cNKMLua.GetData("m_fShadowOffsetX", ref m_fShadowOffsetX);
			cNKMLua.GetData("m_fShadowOffsetY", ref m_fShadowOffsetY);
			cNKMLua.GetData("m_fShadowScaleX", ref m_fShadowScaleX);
			cNKMLua.GetData("m_fShadowScaleY", ref m_fShadowScaleY);
			cNKMLua.GetData("m_fShadowRotateX", ref m_fShadowRotateX);
			cNKMLua.GetData("m_fShadowRotateZ", ref m_fShadowRotateZ);
			cNKMLua.GetData("m_fBuffEffectScaleFactor", ref m_fBuffEffectScaleFactor);
			cNKMLua.GetData("m_bShowGage", ref m_bShowGage);
			cNKMLua.GetData("m_bGageSmall", ref m_bGageSmall);
			cNKMLua.GetData("m_fGageOffsetX", ref m_fGageOffsetX);
			cNKMLua.GetData("m_fGageOffsetY", ref m_fGageOffsetY);
			cNKMLua.GetData("m_fRespawnCoolTime", ref m_fRespawnCoolTime);
			cNKMLua.GetData("m_fDieCompleteTime", ref m_fDieCompleteTime);
			cNKMLua.GetData("m_bDieDeActive", ref m_bDieDeActive);
			cNKMLua.GetData("m_bUseMotionBlur", ref m_bUseMotionBlur);
			cNKMLua.GetData("m_bNoDamageState", ref m_bNoDamageState);
			cNKMLua.GetData("m_bNoDamageDownState", ref m_bNoDamageDownState);
			cNKMLua.GetData("m_bNoDamageStopTime", ref m_bNoDamageStopTime);
			cNKMLua.GetData("m_Invincible", ref m_Invincible);
			cNKMLua.GetData("m_SuperArmorLevel", ref m_SuperArmorLevel);
			cNKMLua.GetData("m_ColorR", ref m_ColorR);
			cNKMLua.GetData("m_ColorG", ref m_ColorG);
			cNKMLua.GetData("m_ColorB", ref m_ColorB);
			cNKMLua.GetData("m_fAirHigh", ref m_fAirHigh);
			cNKMLua.GetData("m_bNoMove", ref m_bNoMove);
			cNKMLua.GetData("m_bNoRun", ref m_bNoRun);
			cNKMLua.GetData("m_SpeedRun", ref m_SpeedRun);
			cNKMLua.GetData("m_SpeedJump", ref m_SpeedJump);
			cNKMLua.GetData("m_fReloadAccel", ref m_fReloadAccel);
			cNKMLua.GetData("m_fGAccel", ref m_fGAccel);
			cNKMLua.GetData("m_fMaxGSpeed", ref m_fMaxGSpeed);
			cNKMLua.GetData("m_fDamageBackFactor", ref m_fDamageBackFactor);
			cNKMLua.GetData("m_fDamageUpFactor", ref m_fDamageUpFactor);
			cNKMLua.GetData("m_bNoMapLimit", ref m_bNoMapLimit);
			cNKMLua.GetData("m_SeeTarget", ref m_SeeTarget);
			cNKMLua.GetData("m_bSeeMoreEnemy", ref m_bSeeMoreEnemy);
			cNKMLua.GetData("m_SeeRange", ref m_TargetFindData.m_fSeeRange);
			cNKMLua.GetData("m_SeeRangeMax", ref m_SeeRangeMax);
			cNKMLua.GetData("m_FindTargetTime", ref m_TargetFindData.m_fFindTargetTime);
			cNKMLua.GetData("m_bTargetNoChange", ref m_TargetFindData.m_bTargetNoChange);
			cNKMLua.GetData("m_bNoBackTarget", ref m_TargetFindData.m_bNoBackTarget);
			cNKMLua.GetDataListEnum("m_hsFindTargetRolePriority", m_TargetFindData.m_hsFindTargetRolePriority);
			cNKMLua.GetDataListEnum("m_hsFindTargetStylePriority", m_TargetFindData.m_hsFindTargetStylePriority);
			cNKMLua.GetData("m_TargetNearRange", ref m_TargetNearRange);
			if (m_TargetNearRange <= 0f)
			{
				Log.Error(unitStrId + " m_TargetNearRange must over zero", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 545);
			}
			cNKMLua.GetData("m_fPatrolRange", ref m_fPatrolRange);
			cNKMLua.GetData("m_PenetrateDefence", ref m_PenetrateDefence);
			cNKMLua.GetData("m_StateChangeSilence", ref m_StateChangeSilence);
			cNKMLua.GetData("m_StateChangeStun", ref m_StateChangeStun);
			cNKMLua.GetData("m_StateChangeSleep", ref m_StateChangeSleep);
			cNKMLua.GetData("m_StateChangeConfuse", ref m_StateChangeConfuse);
			cNKMLua.GetData("m_MapPosOverStatePos", ref m_MapPosOverStatePos);
			cNKMLua.GetData("m_MapPosOverState", ref m_MapPosOverState);
			NKMUnitTriggerSet.LoadFromLua(cNKMLua, ref m_dicTriggerSet, ref m_dicTriggerSetID);
			NKMEventConditionV2.LoadMacroFromLua(cNKMLua, "m_listConditionMacro", ref m_dicEventConditonV2Macro);
			NKMUnitReaction.LoadFromLua(cNKMLua, ref m_dicReaction, ref m_dicReactionID);
			cNKMLua.GetDataListEnum("m_listFixedStatusEffect", m_listFixedStatusEffect, bClearList: false);
			cNKMLua.GetDataListEnum("m_listFixedStatusImmune", m_listFixedStatusImmune, bClearList: false);
			HashSet<NKM_UNIT_STATUS_EFFECT> hashSet = new HashSet<NKM_UNIT_STATUS_EFFECT>();
			HashSet<NKM_UNIT_STATUS_EFFECT> hashSet2 = new HashSet<NKM_UNIT_STATUS_EFFECT>();
			cNKMLua.GetDataListEnum("m_listFixedStatusEffectRemove", hashSet);
			cNKMLua.GetDataListEnum("m_listFixedStatusImmuneRemove", hashSet2);
			m_listFixedStatusEffect.ExceptWith(hashSet);
			m_listFixedStatusImmune.ExceptWith(hashSet2);
			m_lstRespawnEffectData = NKMUtil.LoadListFromLua(cNKMLua, "m_listRespawnEffect", RespawnEffectData.LoadFromLua, bNullIfEmpty: true);
			RespawnEffectData respawnEffectData = RespawnEffectData.LoadFromLua(cNKMLua);
			if (respawnEffectData != null)
			{
				if (m_lstRespawnEffectData == null)
				{
					m_lstRespawnEffectData = new List<RespawnEffectData>();
				}
				m_lstRespawnEffectData.Add(respawnEffectData);
			}
			LowerCompability("m_bNoHeal", NKM_UNIT_STATUS_EFFECT.NUSE_NOHEAL, m_listFixedStatusEffect);
			LowerCompability("m_bImmuneSleep", NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP, m_listFixedStatusImmune);
			LowerCompability("m_bImmuneStun", NKM_UNIT_STATUS_EFFECT.NUSE_STUN, m_listFixedStatusImmune);
			LowerCompability("m_bMetalSlime", NKM_UNIT_STATUS_EFFECT.NUSE_IRONWALL, m_listFixedStatusEffect);
			LowerCompability("m_bNoDamageBackSpeed", NKM_UNIT_STATUS_EFFECT.NUSE_NO_DAMAGE_BACK_SPEED, m_listFixedStatusEffect);
			Dictionary<string, NKM_UNIT_ATTACK_STATE_TYPE> dicAttackStateType = new Dictionary<string, NKM_UNIT_ATTACK_STATE_TYPE>();
			if (cNKMLua.OpenTable("m_listAccumStateChangePack"))
			{
				int num = 1;
				while (cNKMLua.OpenTable(num))
				{
					NKMAccumStateChangePack nKMAccumStateChangePack = null;
					if (m_listAccumStateChangePack.Count < num)
					{
						nKMAccumStateChangePack = new NKMAccumStateChangePack();
						m_listAccumStateChangePack.Add(nKMAccumStateChangePack);
					}
					else
					{
						nKMAccumStateChangePack = m_listAccumStateChangePack[num - 1];
					}
					nKMAccumStateChangePack.LoadFromLUA(cNKMLua);
					num++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
			}
			if (cNKMLua.OpenTable("m_listHitFeedBack"))
			{
				int num2 = 1;
				while (cNKMLua.OpenTable(num2))
				{
					NKMHitFeedBack nKMHitFeedBack = null;
					if (m_listHitFeedBack.Count < num2)
					{
						nKMHitFeedBack = new NKMHitFeedBack();
						m_listHitFeedBack.Add(nKMHitFeedBack);
					}
					else
					{
						nKMHitFeedBack = m_listHitFeedBack[num2 - 1];
					}
					nKMHitFeedBack.LoadFromLUA(cNKMLua);
					num2++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
			}
			if (cNKMLua.OpenTable("m_listHitCriticalFeedBack"))
			{
				int num3 = 1;
				while (cNKMLua.OpenTable(num3))
				{
					NKMHitFeedBack nKMHitFeedBack2 = null;
					if (m_listHitCriticalFeedBack.Count < num3)
					{
						nKMHitFeedBack2 = new NKMHitFeedBack();
						m_listHitCriticalFeedBack.Add(nKMHitFeedBack2);
					}
					else
					{
						nKMHitFeedBack2 = m_listHitCriticalFeedBack[num3 - 1];
					}
					nKMHitFeedBack2.LoadFromLUA(cNKMLua);
					num3++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
			}
			if (cNKMLua.OpenTable("m_listHitEvadeFeedBack"))
			{
				int num4 = 1;
				while (cNKMLua.OpenTable(num4))
				{
					NKMHitFeedBack nKMHitFeedBack3 = null;
					if (m_listHitEvadeFeedBack.Count < num4)
					{
						nKMHitFeedBack3 = new NKMHitFeedBack();
						m_listHitEvadeFeedBack.Add(nKMHitFeedBack3);
					}
					else
					{
						nKMHitFeedBack3 = m_listHitEvadeFeedBack[num4 - 1];
					}
					nKMHitFeedBack3.LoadFromLUA(cNKMLua);
					num4++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
			}
			if (cNKMLua.OpenTable("m_listKillFeedBack"))
			{
				int num5 = 1;
				while (cNKMLua.OpenTable(num5))
				{
					NKMKillFeedBack nKMKillFeedBack = null;
					if (m_listKillFeedBack.Count < num5)
					{
						nKMKillFeedBack = new NKMKillFeedBack();
						m_listKillFeedBack.Add(nKMKillFeedBack);
					}
					else
					{
						nKMKillFeedBack = m_listKillFeedBack[num5 - 1];
					}
					nKMKillFeedBack.LoadFromLUA(cNKMLua);
					num5++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
			}
			if (cNKMLua.OpenTable("m_listBuffUnitDieEvent"))
			{
				int num6 = 1;
				while (cNKMLua.OpenTable(num6))
				{
					NKMBuffUnitDieEvent nKMBuffUnitDieEvent = null;
					if (m_listBuffUnitDieEvent.Count < num6)
					{
						nKMBuffUnitDieEvent = new NKMBuffUnitDieEvent();
						m_listBuffUnitDieEvent.Add(nKMBuffUnitDieEvent);
					}
					else
					{
						nKMBuffUnitDieEvent = m_listBuffUnitDieEvent[num6 - 1];
					}
					nKMBuffUnitDieEvent.LoadFromLUA(cNKMLua);
					num6++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
			}
			if (cNKMLua.OpenTable("m_listReflectionBuffData"))
			{
				int num7 = 1;
				while (cNKMLua.OpenTable(num7))
				{
					NKMStaticBuffData nKMStaticBuffData = null;
					if (m_listReflectionBuffData.Count < num7)
					{
						nKMStaticBuffData = new NKMStaticBuffData();
						m_listReflectionBuffData.Add(nKMStaticBuffData);
					}
					else
					{
						nKMStaticBuffData = m_listReflectionBuffData[num7 - 1];
					}
					nKMStaticBuffData.LoadFromLUA(cNKMLua);
					num7++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
			}
			if (cNKMLua.OpenTable("m_listStaticBuffData"))
			{
				int num8 = 1;
				while (cNKMLua.OpenTable(num8))
				{
					NKMStaticBuffData nKMStaticBuffData2 = null;
					if (m_listStaticBuffData.Count < num8)
					{
						nKMStaticBuffData2 = new NKMStaticBuffData();
						m_listStaticBuffData.Add(nKMStaticBuffData2);
					}
					else
					{
						nKMStaticBuffData2 = m_listStaticBuffData[num8 - 1];
					}
					nKMStaticBuffData2.LoadFromLUA(cNKMLua);
					num8++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
			}
			LoadNullListTable<NKMTriggerRepeatData>("m_listTriggerRepeatData", delegate(NKMLua lua)
			{
				NKMTriggerRepeatData nKMTriggerRepeatData = new NKMTriggerRepeatData();
				nKMTriggerRepeatData.LoadFromLUA(lua);
				return nKMTriggerRepeatData;
			}, ref m_listTriggerRepeatData);
			if (cNKMLua.OpenTable("m_listPhaseChangeData"))
			{
				int num9 = 1;
				while (cNKMLua.OpenTable(num9))
				{
					NKMPhaseChangeData nKMPhaseChangeData = null;
					if (m_listPhaseChangeData.Count < num9)
					{
						nKMPhaseChangeData = new NKMPhaseChangeData();
						m_listPhaseChangeData.Add(nKMPhaseChangeData);
					}
					else
					{
						nKMPhaseChangeData = m_listPhaseChangeData[num9 - 1];
					}
					nKMPhaseChangeData.LoadFromLUA(cNKMLua);
					num9++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
			}
			OpenCommonStateData("m_listStartStateData", ref m_listStartStateData);
			OpenCommonStateData("m_listStandStateData", ref m_listStandStateData);
			OpenCommonStateData("m_listRunStateData", ref m_listRunStateData);
			OpenAttackStateData(NKM_UNIT_ATTACK_STATE_TYPE.ATTACK, "m_listAttackStateData", ref m_listAttackStateData);
			OpenAttackStateData(NKM_UNIT_ATTACK_STATE_TYPE.AIR_ATTACK, "m_listAirAttackStateData", ref m_listAirAttackStateData);
			OpenAttackStateData(NKM_UNIT_ATTACK_STATE_TYPE.SKILL, "m_listSkillStateData", ref m_listSkillStateData);
			OpenAttackStateData(NKM_UNIT_ATTACK_STATE_TYPE.AIR_SKILL, "m_listAirSkillStateData", ref m_listAirSkillStateData);
			OpenAttackStateData(NKM_UNIT_ATTACK_STATE_TYPE.HYPER, "m_listHyperSkillStateData", ref m_listHyperSkillStateData);
			OpenAttackStateData(NKM_UNIT_ATTACK_STATE_TYPE.AIR_HYPER, "m_listAirHyperSkillStateData", ref m_listAirHyperSkillStateData);
			OpenAttackStateData(NKM_UNIT_ATTACK_STATE_TYPE.PASSIVE, "m_listPassiveAttackStateData", ref m_listPassiveAttackStateData);
			OpenAttackStateData(NKM_UNIT_ATTACK_STATE_TYPE.AIR_PASSIVE, "m_listAirPassiveAttackStateData", ref m_listAirPassiveAttackStateData);
			AddAttackState(NKM_UNIT_ATTACK_STATE_TYPE.ATTACK, "m_AttackStateData", ref m_listAttackStateData);
			AddAttackState(NKM_UNIT_ATTACK_STATE_TYPE.AIR_ATTACK, "m_AirAttackStateData", ref m_listAirAttackStateData);
			AddAttackState(NKM_UNIT_ATTACK_STATE_TYPE.SKILL, "m_SkillStateData", ref m_listSkillStateData);
			AddAttackState(NKM_UNIT_ATTACK_STATE_TYPE.AIR_SKILL, "m_AirSkillStateData", ref m_listAirSkillStateData);
			AddAttackState(NKM_UNIT_ATTACK_STATE_TYPE.HYPER, "m_HyperSkillStateData", ref m_listHyperSkillStateData);
			AddAttackState(NKM_UNIT_ATTACK_STATE_TYPE.AIR_HYPER, "m_AirHyperSkillStateData", ref m_listAirHyperSkillStateData);
			AddAttackState(NKM_UNIT_ATTACK_STATE_TYPE.PASSIVE, "m_PassiveAttackStateData", ref m_listPassiveAttackStateData);
			AddAttackState(NKM_UNIT_ATTACK_STATE_TYPE.AIR_PASSIVE, "m_AirPassiveAttackStateData", ref m_listAirPassiveAttackStateData);
			cNKMLua.GetData("m_DyingState", ref m_DyingState);
			if (cNKMLua.OpenTable("m_dicNKMUnitState"))
			{
				int num10 = 1;
				while (cNKMLua.OpenTable(num10))
				{
					NKMUnitState nKMUnitState = null;
					string rValue = "";
					cNKMLua.GetData("m_StateName", ref rValue);
					nKMUnitState = ((!m_dicNKMUnitState.ContainsKey(rValue)) ? new NKMUnitState() : m_dicNKMUnitState[rValue]);
					nKMUnitState.LoadFromLUA(cNKMLua, unitStrId);
					if (!m_dicNKMUnitState.ContainsKey(nKMUnitState.m_StateName))
					{
						m_dicNKMUnitState.Add(nKMUnitState.m_StateName, nKMUnitState);
					}
					num10++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
				num10 = 1;
				int num11 = m_dicNKMUnitStateID.Count + 10;
				m_dicNKMUnitStateID.Clear();
				m_dicAttackStateType.Clear();
				Dictionary<string, NKMUnitState>.Enumerator enumerator = m_dicNKMUnitState.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NKMUnitState value = enumerator.Current.Value;
					if (value.m_StateID == 0)
					{
						value.m_StateID = (byte)(num11 + num10);
					}
					if (!m_dicNKMUnitStateID.ContainsKey(value.m_StateID))
					{
						m_dicNKMUnitStateID.Add(value.m_StateID, value);
					}
					else
					{
						Log.Error("m_dicNKMUnitStateID Duplicate: " + m_UnitTempletBase.m_UnitStrID + " : " + value.m_StateName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 1008);
					}
					if (dicAttackStateType.ContainsKey(value.m_StateName))
					{
						m_dicAttackStateType.Add(value.m_StateID, dicAttackStateType[value.m_StateName]);
					}
					num10++;
				}
			}
			NKMFindTargetData.LoadFromLUA(cNKMLua, "m_SubTargetFindData", out m_SubTargetFindData);
			if (NKMCommonConst.USE_ROLLBACK && !m_UnitTempletBase.m_bMonster && !m_UnitTempletBase.IsShip())
			{
				NKMUnitState unitState = GetUnitState("USN_START");
				if (unitState != null)
				{
					if (unitState.m_bRun)
					{
						Log.Error("유닛 " + m_UnitTempletBase.m_UnitStrID + " 스타트 스테이트 USN_START : m_bRun = true", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 1030);
					}
					m_fMaxRollbackTime = unitState.CalcaluateMaxRollbackTime(m_UnitTempletBase.m_UnitStrID);
				}
				else
				{
					m_fMaxRollbackTime = 0f;
				}
			}
			return true;
			void AddAttackState(NKM_UNIT_ATTACK_STATE_TYPE stateType, string tableName, ref List<NKMAttackStateData> lstAttackStateData)
			{
				if (cNKMLua.OpenTable(tableName))
				{
					NKMAttackStateData nKMAttackStateData = new NKMAttackStateData();
					nKMAttackStateData.LoadFromLUA(cNKMLua, m_TargetNearRange);
					dicAttackStateType[nKMAttackStateData.m_StateName] = stateType;
					lstAttackStateData.Add(nKMAttackStateData);
					cNKMLua.CloseTable();
				}
			}
			void OpenAttackStateData(NKM_UNIT_ATTACK_STATE_TYPE stateType, string tableName, ref List<NKMAttackStateData> lstAttackStateData)
			{
				if (cNKMLua.OpenTable(tableName))
				{
					int num12 = 1;
					while (cNKMLua.OpenTable(num12))
					{
						NKMAttackStateData nKMAttackStateData = null;
						if (lstAttackStateData.Count < num12)
						{
							nKMAttackStateData = new NKMAttackStateData();
							lstAttackStateData.Add(nKMAttackStateData);
						}
						else
						{
							nKMAttackStateData = lstAttackStateData[num12 - 1];
						}
						nKMAttackStateData.LoadFromLUA(cNKMLua, m_TargetNearRange);
						num12++;
						cNKMLua.CloseTable();
					}
					cNKMLua.CloseTable();
				}
				foreach (NKMAttackStateData lstAttackStateDatum in lstAttackStateData)
				{
					dicAttackStateType[lstAttackStateDatum.m_StateName] = stateType;
				}
			}
		}
		catch (Exception ex)
		{
			NKMTempletError.Add("[UnitTemplet] skill state 로딩 오류. exception:" + ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 1045);
			return false;
		}
		void LoadNullListTable<T>(string tableName, Func<NKMLua, T> elementFactory, ref List<T> listContainer) where T : new()
		{
			if (cNKMLua.OpenTable(tableName))
			{
				int num12 = 1;
				listContainer = new List<T>();
				while (cNKMLua.OpenTable(num12))
				{
					T item = elementFactory(cNKMLua);
					listContainer.Add(item);
					num12++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
			}
			else
			{
				listContainer = null;
			}
		}
		void LowerCompability(string name, NKM_UNIT_STATUS_EFFECT type, HashSet<NKM_UNIT_STATUS_EFFECT> hsTarget)
		{
			bool rbValue = false;
			if (cNKMLua.GetData(name, ref rbValue))
			{
				if (rbValue)
				{
					hsTarget.Add(type);
				}
				else
				{
					hsTarget.Remove(type);
				}
			}
		}
		void OpenCommonStateData(string tableName, ref List<NKMCommonStateData> lstCommonStateData)
		{
			if (cNKMLua.OpenTable(tableName))
			{
				int num12 = 1;
				while (cNKMLua.OpenTable(num12))
				{
					NKMCommonStateData nKMCommonStateData = null;
					if (lstCommonStateData.Count < num12)
					{
						nKMCommonStateData = new NKMCommonStateData();
						lstCommonStateData.Add(nKMCommonStateData);
					}
					else
					{
						nKMCommonStateData = lstCommonStateData[num12 - 1];
					}
					nKMCommonStateData.LoadFromLUA(cNKMLua);
					num12++;
					cNKMLua.CloseTable();
				}
				cNKMLua.CloseTable();
			}
		}
	}

	public void DeepCopyFromSource(NKMUnitTemplet source)
	{
		m_SpriteScale = source.m_SpriteScale;
		m_SpriteOffsetX = source.m_SpriteOffsetX;
		m_SpriteOffsetY = source.m_SpriteOffsetY;
		m_fForceRespawnXpos = source.m_fForceRespawnXpos;
		m_fForceRespawnZposMin = source.m_fForceRespawnZposMin;
		m_fForceRespawnZposMax = source.m_fForceRespawnZposMax;
		m_UnitSizeX = source.m_UnitSizeX;
		m_UnitSizeY = source.m_UnitSizeY;
		m_NKC_TEAM_COLOR_TYPE = source.m_NKC_TEAM_COLOR_TYPE;
		m_fShadowOffsetX = source.m_fShadowOffsetX;
		m_fShadowOffsetY = source.m_fShadowOffsetY;
		m_fShadowScaleX = source.m_fShadowScaleX;
		m_fShadowScaleY = source.m_fShadowScaleY;
		m_fShadowRotateX = source.m_fShadowRotateX;
		m_fShadowRotateZ = source.m_fShadowRotateZ;
		m_fBuffEffectScaleFactor = source.m_fBuffEffectScaleFactor;
		m_bShowGage = source.m_bShowGage;
		m_bGageSmall = source.m_bGageSmall;
		m_fGageOffsetX = source.m_fGageOffsetX;
		m_fGageOffsetY = source.m_fGageOffsetY;
		m_fRespawnCoolTime = source.m_fRespawnCoolTime;
		m_fDieCompleteTime = source.m_fDieCompleteTime;
		m_bDieDeActive = source.m_bDieDeActive;
		m_bUseMotionBlur = source.m_bUseMotionBlur;
		m_bNoDamageState = source.m_bNoDamageState;
		m_bNoDamageDownState = source.m_bNoDamageDownState;
		m_bNoDamageStopTime = source.m_bNoDamageStopTime;
		m_Invincible = source.m_Invincible;
		m_SuperArmorLevel = source.m_SuperArmorLevel;
		m_ColorR = source.m_ColorR;
		m_ColorG = source.m_ColorG;
		m_ColorB = source.m_ColorB;
		m_fAirHigh = source.m_fAirHigh;
		m_bNoMove = source.m_bNoMove;
		m_bNoRun = source.m_bNoRun;
		m_SpeedRun = source.m_SpeedRun;
		m_SpeedJump = source.m_SpeedJump;
		m_fReloadAccel = source.m_fReloadAccel;
		m_fGAccel = source.m_fGAccel;
		m_fMaxGSpeed = source.m_fMaxGSpeed;
		m_fDamageBackFactor = source.m_fDamageBackFactor;
		m_fDamageUpFactor = source.m_fDamageUpFactor;
		m_bNoMapLimit = source.m_bNoMapLimit;
		m_SeeTarget = source.m_SeeTarget;
		m_bSeeMoreEnemy = source.m_bSeeMoreEnemy;
		NKMFindTargetData.DeepCopyFrom(source.m_TargetFindData, out m_TargetFindData);
		m_SeeRangeMax = source.m_SeeRangeMax;
		m_TargetNearRange = source.m_TargetNearRange;
		m_fPatrolRange = source.m_fPatrolRange;
		m_PenetrateDefence = source.m_PenetrateDefence;
		m_StateChangeSilence = source.m_StateChangeSilence;
		m_StateChangeStun = source.m_StateChangeStun;
		m_StateChangeSleep = source.m_StateChangeSleep;
		m_StateChangeConfuse = source.m_StateChangeConfuse;
		m_MapPosOverStatePos = source.m_MapPosOverStatePos;
		m_MapPosOverState = source.m_MapPosOverState;
		if (source.m_lstRespawnEffectData != null)
		{
			m_lstRespawnEffectData = new List<RespawnEffectData>(source.m_lstRespawnEffectData.Count);
			foreach (RespawnEffectData lstRespawnEffectDatum in source.m_lstRespawnEffectData)
			{
				if (lstRespawnEffectDatum != null)
				{
					m_lstRespawnEffectData.Add(lstRespawnEffectDatum.Clone());
				}
			}
		}
		else
		{
			m_lstRespawnEffectData = null;
		}
		m_listFixedStatusEffect.Clear();
		m_listFixedStatusEffect.UnionWith(source.m_listFixedStatusEffect);
		m_listFixedStatusImmune.Clear();
		m_listFixedStatusImmune.UnionWith(source.m_listFixedStatusImmune);
		m_listAccumStateChangePack.Clear();
		for (int i = 0; i < source.m_listAccumStateChangePack.Count; i++)
		{
			NKMAccumStateChangePack nKMAccumStateChangePack = new NKMAccumStateChangePack();
			nKMAccumStateChangePack.DeepCopyFromSource(source.m_listAccumStateChangePack[i]);
			m_listAccumStateChangePack.Add(nKMAccumStateChangePack);
		}
		m_listHitFeedBack.Clear();
		for (int j = 0; j < source.m_listHitFeedBack.Count; j++)
		{
			NKMHitFeedBack nKMHitFeedBack = new NKMHitFeedBack();
			nKMHitFeedBack.DeepCopyFromSource(source.m_listHitFeedBack[j]);
			m_listHitFeedBack.Add(nKMHitFeedBack);
		}
		m_listHitCriticalFeedBack.Clear();
		for (int k = 0; k < source.m_listHitCriticalFeedBack.Count; k++)
		{
			NKMHitFeedBack nKMHitFeedBack2 = new NKMHitFeedBack();
			nKMHitFeedBack2.DeepCopyFromSource(source.m_listHitCriticalFeedBack[k]);
			m_listHitCriticalFeedBack.Add(nKMHitFeedBack2);
		}
		m_listHitEvadeFeedBack.Clear();
		for (int l = 0; l < source.m_listHitEvadeFeedBack.Count; l++)
		{
			NKMHitFeedBack nKMHitFeedBack3 = new NKMHitFeedBack();
			nKMHitFeedBack3.DeepCopyFromSource(source.m_listHitEvadeFeedBack[l]);
			m_listHitEvadeFeedBack.Add(nKMHitFeedBack3);
		}
		m_listKillFeedBack.Clear();
		for (int m = 0; m < source.m_listKillFeedBack.Count; m++)
		{
			NKMKillFeedBack nKMKillFeedBack = new NKMKillFeedBack();
			nKMKillFeedBack.DeepCopyFromSource(source.m_listKillFeedBack[m]);
			m_listKillFeedBack.Add(nKMKillFeedBack);
		}
		m_listBuffUnitDieEvent.Clear();
		for (int n = 0; n < source.m_listBuffUnitDieEvent.Count; n++)
		{
			NKMBuffUnitDieEvent nKMBuffUnitDieEvent = new NKMBuffUnitDieEvent();
			nKMBuffUnitDieEvent.DeepCopyFromSource(source.m_listBuffUnitDieEvent[n]);
			m_listBuffUnitDieEvent.Add(nKMBuffUnitDieEvent);
		}
		m_listReflectionBuffData.Clear();
		for (int num = 0; num < source.m_listReflectionBuffData.Count; num++)
		{
			NKMStaticBuffData nKMStaticBuffData = new NKMStaticBuffData();
			nKMStaticBuffData.DeepCopyFromSource(source.m_listReflectionBuffData[num]);
			m_listReflectionBuffData.Add(nKMStaticBuffData);
		}
		m_listStaticBuffData.Clear();
		for (int num2 = 0; num2 < source.m_listStaticBuffData.Count; num2++)
		{
			NKMStaticBuffData nKMStaticBuffData2 = new NKMStaticBuffData();
			nKMStaticBuffData2.DeepCopyFromSource(source.m_listStaticBuffData[num2]);
			m_listStaticBuffData.Add(nKMStaticBuffData2);
		}
		m_listPhaseChangeData.Clear();
		for (int num3 = 0; num3 < source.m_listPhaseChangeData.Count; num3++)
		{
			NKMPhaseChangeData nKMPhaseChangeData = new NKMPhaseChangeData();
			nKMPhaseChangeData.DeepCopyFromSource(source.m_listPhaseChangeData[num3]);
			m_listPhaseChangeData.Add(nKMPhaseChangeData);
		}
		m_dicTriggerSet = null;
		m_dicTriggerSetID = null;
		if (source.m_dicTriggerSet != null)
		{
			m_dicTriggerSet = new Dictionary<int, NKMUnitTriggerSet>();
			foreach (KeyValuePair<int, NKMUnitTriggerSet> item in source.m_dicTriggerSet)
			{
				m_dicTriggerSet.Add(item.Key, item.Value);
			}
			m_dicTriggerSetID = new Dictionary<string, int>();
			foreach (KeyValuePair<string, int> item2 in source.m_dicTriggerSetID)
			{
				m_dicTriggerSetID.Add(item2.Key, item2.Value);
			}
		}
		m_dicReaction = null;
		m_dicReactionID = null;
		if (source.m_dicReaction != null)
		{
			m_dicReaction = new Dictionary<int, NKMUnitReaction>();
			foreach (KeyValuePair<int, NKMUnitReaction> item3 in source.m_dicReaction)
			{
				m_dicReaction.Add(item3.Key, item3.Value);
			}
			m_dicReactionID = new Dictionary<string, int>();
			foreach (KeyValuePair<string, int> item4 in source.m_dicReactionID)
			{
				m_dicReactionID.Add(item4.Key, item4.Value);
			}
		}
		m_dicEventConditonV2Macro = null;
		if (source.m_dicEventConditonV2Macro != null)
		{
			m_dicEventConditonV2Macro = new Dictionary<string, NKMEventConditionV2>();
			foreach (KeyValuePair<string, NKMEventConditionV2> item5 in source.m_dicEventConditonV2Macro)
			{
				m_dicEventConditonV2Macro.Add(item5.Key, item5.Value);
			}
		}
		m_listTriggerRepeatData = null;
		if (source.m_listTriggerRepeatData != null)
		{
			m_listTriggerRepeatData = new List<NKMTriggerRepeatData>(source.m_listTriggerRepeatData.Count);
			foreach (NKMTriggerRepeatData listTriggerRepeatDatum in source.m_listTriggerRepeatData)
			{
				m_listTriggerRepeatData.Add(listTriggerRepeatDatum);
			}
		}
		DeepCopyCommonState(ref m_listStartStateData, source.m_listStartStateData);
		DeepCopyCommonState(ref m_listStandStateData, source.m_listStandStateData);
		DeepCopyCommonState(ref m_listRunStateData, source.m_listRunStateData);
		DeepCopyAttackState(ref m_listAttackStateData, source.m_listAttackStateData);
		DeepCopyAttackState(ref m_listAirAttackStateData, source.m_listAirAttackStateData);
		DeepCopyAttackState(ref m_listSkillStateData, source.m_listSkillStateData);
		DeepCopyAttackState(ref m_listAirSkillStateData, source.m_listAirSkillStateData);
		DeepCopyAttackState(ref m_listHyperSkillStateData, source.m_listHyperSkillStateData);
		DeepCopyAttackState(ref m_listAirHyperSkillStateData, source.m_listAirHyperSkillStateData);
		DeepCopyAttackState(ref m_listPassiveAttackStateData, source.m_listPassiveAttackStateData);
		DeepCopyAttackState(ref m_listAirPassiveAttackStateData, source.m_listAirPassiveAttackStateData);
		m_dicAttackStateType.Clear();
		foreach (KeyValuePair<int, NKM_UNIT_ATTACK_STATE_TYPE> item6 in source.m_dicAttackStateType)
		{
			m_dicAttackStateType.Add(item6.Key, item6.Value);
		}
		m_DyingState = source.m_DyingState;
		Dictionary<string, NKMUnitState>.Enumerator enumerator8 = source.m_dicNKMUnitState.GetEnumerator();
		while (enumerator8.MoveNext())
		{
			NKMUnitState value = enumerator8.Current.Value;
			if (m_dicNKMUnitStateID.ContainsKey(value.m_StateID))
			{
				m_dicNKMUnitStateID[value.m_StateID].DeepCopyFromSource(value);
				continue;
			}
			NKMUnitState nKMUnitState = new NKMUnitState();
			nKMUnitState.DeepCopyFromSource(value);
			m_dicNKMUnitState.Add(nKMUnitState.m_StateName, nKMUnitState);
			m_dicNKMUnitStateID.Add(nKMUnitState.m_StateID, nKMUnitState);
		}
		m_fMaxRollbackTime = source.m_fMaxRollbackTime;
		NKMFindTargetData.DeepCopyFrom(source.m_SubTargetFindData, out m_SubTargetFindData);
		static void DeepCopyAttackState(ref List<NKMAttackStateData> target, List<NKMAttackStateData> list)
		{
			target.Clear();
			for (int num4 = 0; num4 < list.Count; num4++)
			{
				NKMAttackStateData nKMAttackStateData = new NKMAttackStateData();
				nKMAttackStateData.DeepCopyFromSource(list[num4]);
				target.Add(nKMAttackStateData);
			}
		}
		static void DeepCopyCommonState(ref List<NKMCommonStateData> target, List<NKMCommonStateData> list)
		{
			target.Clear();
			for (int num4 = 0; num4 < list.Count; num4++)
			{
				NKMCommonStateData nKMCommonStateData = new NKMCommonStateData();
				nKMCommonStateData.DeepCopyFromSource(list[num4]);
				target.Add(nKMCommonStateData);
			}
		}
	}

	public NKMUnitState GetUnitState(string stateName, bool bLog = true)
	{
		if (m_dicNKMUnitState.ContainsKey(stateName))
		{
			return m_dicNKMUnitState[stateName];
		}
		if (bLog)
		{
			stateErrorCallback(this, stateName);
		}
		return null;
	}

	public NKMUnitState GetUnitState(short stateID)
	{
		if (m_dicNKMUnitStateID.ContainsKey(stateID))
		{
			return m_dicNKMUnitStateID[stateID];
		}
		Log.Error("No Exist StateID: " + stateID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 1368);
		return null;
	}

	public void SetCoolTimeLink()
	{
		SetCoolTimeLink(m_listAttackStateData, m_listAttackStateData);
		SetCoolTimeLink(m_listAttackStateData, m_listAirAttackStateData, bClear: false);
		SetCoolTimeLink(m_listAirAttackStateData, m_listAirAttackStateData);
		SetCoolTimeLink(m_listAirAttackStateData, m_listAttackStateData, bClear: false);
		SetCoolTimeLink(m_listSkillStateData, m_listSkillStateData);
		SetCoolTimeLink(m_listSkillStateData, m_listAirSkillStateData, bClear: false);
		SetCoolTimeLink(m_listAirSkillStateData, m_listAirSkillStateData);
		SetCoolTimeLink(m_listAirSkillStateData, m_listSkillStateData, bClear: false);
		SetCoolTimeLink(m_listHyperSkillStateData, m_listHyperSkillStateData);
		SetCoolTimeLink(m_listHyperSkillStateData, m_listAirHyperSkillStateData, bClear: false);
		SetCoolTimeLink(m_listAirHyperSkillStateData, m_listAirHyperSkillStateData);
		SetCoolTimeLink(m_listAirHyperSkillStateData, m_listHyperSkillStateData, bClear: false);
		SetCoolTimeLink(m_listPassiveAttackStateData, m_listPassiveAttackStateData);
		SetCoolTimeLink(m_listPassiveAttackStateData, m_listAirPassiveAttackStateData, bClear: false);
		SetCoolTimeLink(m_listAirPassiveAttackStateData, m_listAirPassiveAttackStateData);
		SetCoolTimeLink(m_listAirPassiveAttackStateData, m_listPassiveAttackStateData, bClear: false);
	}

	public void SetCoolTimeLink(List<NKMAttackStateData> listAttackStateDataDest, List<NKMAttackStateData> listAttackStateDataSrc, bool bClear = true)
	{
		for (int i = 0; i < listAttackStateDataDest.Count; i++)
		{
			NKMAttackStateData nKMAttackStateData = listAttackStateDataDest[i];
			NKMUnitState unitState = GetUnitState(nKMAttackStateData.m_StateName);
			if (unitState == null)
			{
				continue;
			}
			if (bClear)
			{
				unitState.m_listCoolTimeLink.Clear();
			}
			for (int j = 0; j < listAttackStateDataSrc.Count; j++)
			{
				NKMAttackStateData nKMAttackStateData2 = listAttackStateDataSrc[j];
				NKMUnitState unitState2 = GetUnitState(nKMAttackStateData2.m_StateName);
				if (unitState2 != null && unitState.m_StateID != unitState2.m_StateID)
				{
					unitState.m_listCoolTimeLink.Add(unitState2.m_StateName);
					unitState2.m_StateCoolTime = unitState.m_StateCoolTime;
				}
			}
		}
	}

	public float GetNearTargetRandomRange()
	{
		return m_TargetNearRange;
	}

	public static void HookStateErrorHandler(Action<NKMUnitTemplet, string> handler)
	{
		stateErrorCallback = handler;
	}

	private static void DefaultStateErrorHandler(NKMUnitTemplet templet, string stateName)
	{
		Log.Error("unit state not found. unitName:" + templet.m_UnitTempletBase.m_UnitStrID + " stateName:" + stateName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitTemplet.cs", 1443);
	}

	public int GetTriggerID(string name)
	{
		if (m_dicTriggerSetID == null)
		{
			return -1;
		}
		if (m_dicTriggerSetID.TryGetValue(name, out var value))
		{
			return value;
		}
		return -1;
	}

	public NKMUnitTriggerSet GetTriggerSet(string name)
	{
		if (m_dicTriggerSet == null || m_dicTriggerSetID == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (!m_dicTriggerSetID.TryGetValue(name, out var value))
		{
			return null;
		}
		if (m_dicTriggerSet.TryGetValue(value, out var value2))
		{
			return value2;
		}
		return null;
	}

	public NKMUnitTriggerSet GetTriggerSet(int id)
	{
		if (m_dicTriggerSet == null)
		{
			return null;
		}
		if (m_dicTriggerSet.TryGetValue(id, out var value))
		{
			return value;
		}
		return null;
	}

	public int GetReactionID(string name)
	{
		if (m_dicReactionID == null)
		{
			return -1;
		}
		if (m_dicReactionID.TryGetValue(name, out var value))
		{
			return value;
		}
		return -1;
	}

	public NKMUnitReaction GetReaction(string name)
	{
		if (m_dicReaction == null || m_dicReactionID == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (!m_dicReactionID.TryGetValue(name, out var value))
		{
			return null;
		}
		if (m_dicReaction.TryGetValue(value, out var value2))
		{
			return value2;
		}
		return null;
	}

	public NKMUnitReaction GetReaction(int id)
	{
		if (m_dicReaction == null)
		{
			return null;
		}
		if (m_dicReaction.TryGetValue(id, out var value))
		{
			return value;
		}
		return null;
	}

	public NKMEventConditionV2 GetEventConditionMacro(string name)
	{
		if (m_dicEventConditonV2Macro != null && m_dicEventConditonV2Macro.TryGetValue(name, out var value))
		{
			return value;
		}
		return NKMEventConditionV2.GetTempletMacroCondition(name);
	}
}
