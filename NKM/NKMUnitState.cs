using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public class NKMUnitState
{
	public delegate bool ValidateEvent(INKMUnitStateEvent stateEvent);

	public delegate void DeepCopyFactory<T>(T target, T source);

	public NKM_UNIT_STATE_TYPE m_NKM_UNIT_STATE_TYPE;

	public NKM_UNIT_STATUS_EFFECT m_StatusEffectType;

	public string m_StateName = "";

	public byte m_StateID;

	public NKM_SKILL_TYPE m_NKM_SKILL_TYPE;

	public List<string> m_listCoolTimeLink = new List<string>();

	public string m_AnimName = "";

	public float m_fAnimStartTime;

	public float m_fAnimSpeed = 1f;

	public bool m_bAnimSpeedFix;

	public bool m_bAnimLoop;

	public bool m_bSeeTarget;

	public bool m_bSeeMoreEnemy;

	public float m_fAirHigh = -1f;

	public bool m_bChangeIsAirUnit;

	public bool m_bNoAI;

	public bool m_bNoChangeRight;

	public bool m_bNoMove;

	public bool m_bRun;

	public float m_fRunSpeedRate = 1f;

	public bool m_bJump;

	public bool m_bForceNoTargeted;

	public bool m_bNoStateTypeEvent;

	public float m_fGAccel = -1f;

	public bool m_bForceRightLeftDependTeam;

	public bool m_bForceRight;

	public bool m_bForceLeft;

	public bool m_bShowGage = true;

	public NKM_SUPER_ARMOR_LEVEL m_SuperArmorLevel;

	public bool m_bNormalRevengeState;

	public bool m_bRevengeState;

	public bool m_bSuperRevengeState;

	public string m_RevengeChangeState = "";

	public bool m_bInvincibleState;

	public bool m_bNotUseAttackSpeedStat;

	public bool m_bSkillCutIn;

	public bool m_bHyperSkillCutIn;

	public string m_SkillCutInName = "";

	public bool m_bAutoCoolTime;

	public NKMMinMaxFloat m_StateCoolTime = new NKMMinMaxFloat();

	public NKMDangerCharge m_DangerCharge = new NKMDangerCharge();

	public float m_AnimTimeChangeStateTime = -1f;

	public string m_AnimTimeChangeState = "";

	public float m_AnimTimeRateChangeStateTime = -1f;

	public string m_AnimTimeRateChangeState = "";

	public float m_StateTimeChangeStateTime = -1f;

	public string m_StateTimeChangeState = "";

	public float m_TargetLostOrDieStateDurationTime = -1f;

	public string m_TargetLostOrDieState = "";

	public int m_AnimEndChangeStatePlayCount = 1;

	public string m_AnimEndChangeState = "";

	public float m_TargetDistOverChangeStateDist;

	public string m_TargetDistOverChangeState = "";

	public float m_TargetDistLessChangeStateDist;

	public string m_TargetDistLessChangeState = "";

	public float m_MapPosOverStatePos;

	public string m_MapPosOverState = "";

	public string m_FootOnLandChangeState = "";

	public string m_FootOffLandChangeState = "";

	public string m_AnimEndFootOnLandChangeState = "";

	public string m_AnimEndFootOffLandChangeState = "";

	public string m_SpeedYPositiveChangeState = "";

	public string m_SpeedY0NegativeChangeState = "";

	public string m_DamagedChangeState = "";

	public string m_AnimEndDyingState = "";

	public string m_ChangeRightState = "";

	public string m_ChangeRightTrueState = "";

	public string m_ChangeRightFalseState = "";

	public string m_AirTargetThisFrameChangeState = "";

	public string m_LandTargetThisFrameChangeState = "";

	public string m_AirTargetLoopChangeState = string.Empty;

	public string m_LandTargetLoopChangeState = string.Empty;

	public float m_fGageOffsetX;

	public float m_fGageOffsetY;

	public List<NKMEventText> m_listNKMEventText = new List<NKMEventText>();

	public List<NKMEventSpeed> m_listNKMEventSpeed = new List<NKMEventSpeed>();

	public List<NKMEventSpeedX> m_listNKMEventSpeedX = new List<NKMEventSpeedX>();

	public List<NKMEventSpeedY> m_listNKMEventSpeedY = new List<NKMEventSpeedY>();

	public List<NKMEventMove> m_listNKMEventMove = new List<NKMEventMove>();

	public List<NKMEventAttack> m_listNKMEventAttack = new List<NKMEventAttack>();

	public List<NKMEventStopTime> m_listNKMEventStopTime = new List<NKMEventStopTime>();

	public List<NKMEventInvincible> m_listNKMEventInvincible = new List<NKMEventInvincible>();

	public List<NKMEventInvincibleGlobal> m_listNKMEventInvincibleGlobal = new List<NKMEventInvincibleGlobal>();

	public List<NKMEventSuperArmor> m_listNKMEventSuperArmor = new List<NKMEventSuperArmor>();

	public List<NKMEventSound> m_listNKMEventSound = new List<NKMEventSound>();

	public List<NKMEventColor> m_listNKMEventColor = new List<NKMEventColor>();

	public List<NKMEventCameraCrash> m_listNKMEventCameraCrash = new List<NKMEventCameraCrash>();

	public List<NKMEventCameraMove> m_listNKMEventCameraMove = new List<NKMEventCameraMove>();

	public List<NKMEventFadeWorld> m_listNKMEventFadeWorld = new List<NKMEventFadeWorld>();

	public List<NKMEventDissolve> m_listNKMEventDissolve = new List<NKMEventDissolve>();

	public List<NKMEventMotionBlur> m_listNKMEventMotionBlur = new List<NKMEventMotionBlur>();

	public List<NKMEventEffect> m_listNKMEventEffect = new List<NKMEventEffect>();

	public List<NKMEventHyperSkillCutIn> m_listNKMEventHyperSkillCutIn = new List<NKMEventHyperSkillCutIn>();

	public List<NKMEventDamageEffect> m_listNKMEventDamageEffect = new List<NKMEventDamageEffect>();

	public List<NKMEventDEStateChange> m_listNKMEventDEStateChange = new List<NKMEventDEStateChange>();

	public List<NKMEventGameSpeed> m_listNKMEventGameSpeed = new List<NKMEventGameSpeed>();

	public List<NKMEventAnimSpeed> m_listNKMEventAnimSpeed = new List<NKMEventAnimSpeed>();

	public List<NKMEventBuff> m_listNKMEventBuff = new List<NKMEventBuff>();

	public List<NKMEventStatus> m_listNKMEventStatus = new List<NKMEventStatus>();

	public List<NKMEventRespawn> m_listNKMEventRespawn = new List<NKMEventRespawn>();

	public List<NKMEventDie> m_listNKMEventDie = new List<NKMEventDie>();

	public List<NKMEventChangeState> m_listNKMEventChangeState = new List<NKMEventChangeState>();

	public List<NKMEventAgro> m_listNKMEventAgro = new List<NKMEventAgro>();

	public List<NKMEventHeal> m_listNKMEventHeal = new List<NKMEventHeal>();

	public List<NKMEventStun> m_listNKMEventStun = new List<NKMEventStun>();

	public List<NKMEventCost> m_listNKMEventCost = new List<NKMEventCost>();

	public List<NKMEventDefence> m_listNKMEventDefence = new List<NKMEventDefence>();

	public List<NKMEventDispel> m_listNKMEventDispel = new List<NKMEventDispel>();

	public List<NKMEventChangeCooltime> m_listNKMEventChangeCooltime = new List<NKMEventChangeCooltime>();

	public List<NKMEventCatchEnd> m_listNKMEventCatchEnd = new List<NKMEventCatchEnd>();

	public List<NKMEventFindTarget> m_listNKMEventFindTarget = new List<NKMEventFindTarget>();

	public List<NKMEventTrigger> m_listNKMEventTrigger = new List<NKMEventTrigger>();

	public List<NKMEventChangeRemainTime> m_listNKMEventChangeRemainTime = new List<NKMEventChangeRemainTime>();

	public List<NKMEventVariable> m_listNKMEventVariable = new List<NKMEventVariable>();

	public List<NKMEventConsume> m_listNKMEventConsume = new List<NKMEventConsume>();

	public List<NKMEventSkillCutIn> m_listNKMEventSkillCutIn = new List<NKMEventSkillCutIn>();

	public List<NKMEventTriggerBranch> m_listNKMEventTriggerBranch = new List<NKMEventTriggerBranch>();

	public List<NKMEventReaction> m_listNKMEventReaction = new List<NKMEventReaction>();

	public NKMEventUnitChange m_NKMEventUnitChange;

	public HashSet<NKM_UNIT_STATUS_EFFECT> m_listFixedStatusEffect = new HashSet<NKM_UNIT_STATUS_EFFECT>();

	public HashSet<NKM_UNIT_STATUS_EFFECT> m_listFixedStatusImmune = new HashSet<NKM_UNIT_STATUS_EFFECT>();

	public bool IsBadStatusState => m_StatusEffectType != NKM_UNIT_STATUS_EFFECT.NUSE_NONE;

	public bool IsDamageState => m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_DAMAGE;

	public bool IsDamageOrDieState
	{
		get
		{
			if (m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DAMAGE)
			{
				return m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_DIE;
			}
			return true;
		}
	}

	public void Validate(NKMUnitTemplet templet)
	{
		string unitStrID = templet.m_UnitTempletBase.m_UnitStrID;
		if (m_listNKMEventBuff != null)
		{
			foreach (NKMEventBuff item in m_listNKMEventBuff)
			{
				if (!item.Validate())
				{
					Log.ErrorAndExit("[NKMUnitState] m_listNKMEventBuff is invalid. UnitStrID [" + unitStrID + "], m_StateName [" + m_StateName + "], m_BuffStrID [" + item.m_BuffStrID + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitState.cs", 204);
				}
			}
		}
		if (m_listNKMEventStatus != null)
		{
			foreach (NKMEventStatus item2 in m_listNKMEventStatus)
			{
				if (!item2.Validate())
				{
					Log.ErrorAndExit($"[NKMUnitState] m_listNKMEventStatus is invalid. UnitStrID [{unitStrID}], m_StateName [{m_StateName}], m_StatusType [{item2.m_StatusType}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitState.cs", 215);
				}
			}
		}
		if (m_listNKMEventStopTime != null)
		{
			foreach (NKMEventStopTime item3 in m_listNKMEventStopTime)
			{
				if (item3.m_StopTimeIndex >= NKM_STOP_TIME_INDEX.NSTI_MAX || item3.m_StopTimeIndex < NKM_STOP_TIME_INDEX.NSTI_DAMAGE)
				{
					Log.ErrorAndExit($"[NKMUnitState] Invalid NKMEventStopTime/m_StopTimeIndex Data. unit:{unitStrID} InvalidData:{item3.m_StopTimeIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitState.cs", 227);
				}
			}
		}
		if (m_listFixedStatusEffect.Overlaps(m_listFixedStatusImmune))
		{
			Log.ErrorAndExit("[NKMUnitState] m_listFixedStatusEffect / m_listFixedStatusImmune has same NUSE element!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitState.cs", 234);
		}
		if (m_listNKMEventTrigger != null)
		{
			foreach (NKMEventTrigger item4 in m_listNKMEventTrigger)
			{
				item4.Validate(templet);
			}
		}
		if (m_listNKMEventTriggerBranch != null)
		{
			foreach (NKMEventTriggerBranch item5 in m_listNKMEventTriggerBranch)
			{
				item5.Validate(templet);
			}
		}
		if (m_listNKMEventReaction == null)
		{
			return;
		}
		foreach (NKMEventReaction item6 in m_listNKMEventReaction)
		{
			item6.Validate(templet);
		}
	}

	public static void LoadAndMergeEventList<T>(NKMLua cNKMLua, string tableName, ref List<T> lstEvent, ValidateEvent validateEvent = null) where T : INKMUnitStateEvent, new()
	{
		if (!cNKMLua.OpenTable(tableName))
		{
			return;
		}
		int num = 1;
		while (cNKMLua.OpenTable(num))
		{
			T val;
			if (lstEvent.Count < num)
			{
				val = new T();
				lstEvent.Add(val);
			}
			else
			{
				val = lstEvent[num - 1];
			}
			val.LoadFromLUA(cNKMLua);
			validateEvent?.Invoke(val);
			num++;
			cNKMLua.CloseTable();
		}
		cNKMLua.CloseTable();
	}

	public static void LoadEventList<T>(NKMLua cNKMLua, string tableName, ref List<T> lstEvent, ValidateEvent validateEvent = null) where T : INKMUnitStateEvent, new()
	{
		if (cNKMLua.OpenTable(tableName))
		{
			lstEvent.Clear();
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				T val = new T();
				lstEvent.Add(val);
				val.LoadFromLUA(cNKMLua);
				validateEvent?.Invoke(val);
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
	}

	public static void DeepCopy<T>(List<T> sourceList, ref List<T> targetList, DeepCopyFactory<T> factory) where T : INKMUnitStateEvent, new()
	{
		int num = 0;
		if (sourceList == null || sourceList.Count == 0)
		{
			targetList.Clear();
			return;
		}
		num = targetList.Count;
		for (int i = 0; i < sourceList.Count; i++)
		{
			T val;
			if (num - 1 >= i)
			{
				val = targetList[i];
			}
			else
			{
				val = new T();
				targetList.Add(val);
			}
			factory(val, sourceList[i]);
		}
		if (targetList.Count > sourceList.Count)
		{
			targetList.RemoveRange(sourceList.Count, targetList.Count - sourceList.Count);
		}
	}

	public bool LoadFromLUA(NKMLua cNKMLua, string unitStrId)
	{
		bool data = cNKMLua.GetData("m_NKM_UNIT_STATE_TYPE", ref m_NKM_UNIT_STATE_TYPE);
		cNKMLua.GetData("m_StateName", ref m_StateName);
		data &= cNKMLua.GetData("m_NKM_SKILL_TYPE", ref m_NKM_SKILL_TYPE);
		if (m_NKM_SKILL_TYPE.IsStatHoldType())
		{
			NKMTempletError.Add($"스테이트 [{m_StateName}]에 스킬 타입 [{m_NKM_SKILL_TYPE}]이 지정되어 있습니다. 오작동의 가능성이 있습니다", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitState.cs", 352);
			return false;
		}
		cNKMLua.GetData("m_StatusEffectType", ref m_StatusEffectType);
		cNKMLua.GetData("m_AnimName", ref m_AnimName);
		cNKMLua.GetData("m_fAnimStartTime", ref m_fAnimStartTime);
		cNKMLua.GetData("m_fAnimSpeed", ref m_fAnimSpeed);
		cNKMLua.GetData("m_bAnimSpeedFix", ref m_bAnimSpeedFix);
		cNKMLua.GetData("m_bAnimLoop", ref m_bAnimLoop);
		cNKMLua.GetData("m_bSeeTarget", ref m_bSeeTarget);
		cNKMLua.GetData("m_bSeeMoreEnemy", ref m_bSeeMoreEnemy);
		cNKMLua.GetData("m_fAirHigh", ref m_fAirHigh);
		cNKMLua.GetData("m_bChangeIsAirUnit", ref m_bChangeIsAirUnit);
		cNKMLua.GetData("m_bNoAI", ref m_bNoAI);
		cNKMLua.GetData("m_bNoChangeRight", ref m_bNoChangeRight);
		cNKMLua.GetData("m_bNoMove", ref m_bNoMove);
		cNKMLua.GetData("m_bRun", ref m_bRun);
		cNKMLua.GetData("m_fRunSpeedRate", ref m_fRunSpeedRate);
		cNKMLua.GetData("m_bJump", ref m_bJump);
		cNKMLua.GetData("m_bForceNoTargeted", ref m_bForceNoTargeted);
		cNKMLua.GetData("m_bNoStateTypeEvent", ref m_bNoStateTypeEvent);
		cNKMLua.GetData("m_fGAccel", ref m_fGAccel);
		cNKMLua.GetData("m_bForceRightLeftDependTeam", ref m_bForceRightLeftDependTeam);
		cNKMLua.GetData("m_bForceRight", ref m_bForceRight);
		cNKMLua.GetData("m_bForceLeft", ref m_bForceLeft);
		cNKMLua.GetData("m_bShowGage", ref m_bShowGage);
		data &= cNKMLua.GetData("m_SuperArmorLevel", ref m_SuperArmorLevel);
		cNKMLua.GetData("m_bNormalRevengeState", ref m_bNormalRevengeState);
		cNKMLua.GetData("m_bRevengeState", ref m_bRevengeState);
		cNKMLua.GetData("m_bSuperRevengeState", ref m_bSuperRevengeState);
		bool rbValue = false;
		cNKMLua.GetData("m_bSleepState", ref rbValue);
		if (rbValue)
		{
			m_StatusEffectType = NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP;
		}
		bool rbValue2 = false;
		cNKMLua.GetData("m_bStunState", ref rbValue2);
		if (rbValue2)
		{
			m_StatusEffectType = NKM_UNIT_STATUS_EFFECT.NUSE_STUN;
		}
		bool rbValue3 = false;
		cNKMLua.GetData("m_bNoDamageBackSpeed", ref rbValue3);
		if (rbValue3)
		{
			m_listFixedStatusEffect.Add(NKM_UNIT_STATUS_EFFECT.NUSE_NO_DAMAGE_BACK_SPEED);
		}
		cNKMLua.GetData("m_RevengeChangeState", ref m_RevengeChangeState);
		cNKMLua.GetData("m_bInvincibleState", ref m_bInvincibleState);
		cNKMLua.GetData("m_bNotUseAttackSpeedStat", ref m_bNotUseAttackSpeedStat);
		cNKMLua.GetData("m_bSkillCutIn", ref m_bSkillCutIn);
		cNKMLua.GetData("m_bHyperSkillCutIn", ref m_bHyperSkillCutIn);
		cNKMLua.GetData("m_SkillCutInName", ref m_SkillCutInName);
		cNKMLua.GetData("m_bAutoCoolTime", ref m_bAutoCoolTime);
		m_StateCoolTime.LoadFromLua(cNKMLua, "m_StateCoolTime");
		if (cNKMLua.OpenTable("m_DangerCharge"))
		{
			m_DangerCharge.LoadFromLUA(cNKMLua);
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_AnimTimeChangeStateTime", ref m_AnimTimeChangeStateTime);
		cNKMLua.GetData("m_AnimTimeChangeState", ref m_AnimTimeChangeState);
		cNKMLua.GetData("m_AnimTimeRateChangeStateTime", ref m_AnimTimeRateChangeStateTime);
		cNKMLua.GetData("m_AnimTimeRateChangeState", ref m_AnimTimeRateChangeState);
		cNKMLua.GetData("m_StateTimeChangeStateTime", ref m_StateTimeChangeStateTime);
		cNKMLua.GetData("m_StateTimeChangeState", ref m_StateTimeChangeState);
		cNKMLua.GetData("m_TargetDistOverChangeStateDist", ref m_TargetDistOverChangeStateDist);
		cNKMLua.GetData("m_TargetDistOverChangeState", ref m_TargetDistOverChangeState);
		cNKMLua.GetData("m_TargetDistLessChangeStateDist", ref m_TargetDistLessChangeStateDist);
		cNKMLua.GetData("m_TargetDistLessChangeState", ref m_TargetDistLessChangeState);
		cNKMLua.GetData("m_TargetLostOrDieStateDurationTime", ref m_TargetLostOrDieStateDurationTime);
		cNKMLua.GetData("m_TargetLostOrDieState", ref m_TargetLostOrDieState);
		cNKMLua.GetData("m_AnimEndChangeStatePlayCount", ref m_AnimEndChangeStatePlayCount);
		cNKMLua.GetData("m_AnimEndChangeState", ref m_AnimEndChangeState);
		cNKMLua.GetData("m_MapPosOverStatePos", ref m_MapPosOverStatePos);
		cNKMLua.GetData("m_MapPosOverState", ref m_MapPosOverState);
		cNKMLua.GetData("m_FootOnLandChangeState", ref m_FootOnLandChangeState);
		cNKMLua.GetData("m_FootOffLandChangeState", ref m_FootOffLandChangeState);
		cNKMLua.GetData("m_AnimEndFootOnLandChangeState", ref m_AnimEndFootOnLandChangeState);
		cNKMLua.GetData("m_AnimEndFootOffLandChangeState", ref m_AnimEndFootOffLandChangeState);
		cNKMLua.GetData("m_SpeedYPositiveChangeState", ref m_SpeedYPositiveChangeState);
		cNKMLua.GetData("m_SpeedY0NegativeChangeState", ref m_SpeedY0NegativeChangeState);
		cNKMLua.GetData("m_DamagedChangeState", ref m_DamagedChangeState);
		cNKMLua.GetData("m_AnimEndDyingState", ref m_AnimEndDyingState);
		cNKMLua.GetData("m_ChangeRightState", ref m_ChangeRightState);
		cNKMLua.GetData("m_ChangeRightTrueState", ref m_ChangeRightTrueState);
		cNKMLua.GetData("m_ChangeRightFalseState", ref m_ChangeRightFalseState);
		cNKMLua.GetData("m_AirTargetThisFrameChangeState", ref m_AirTargetThisFrameChangeState);
		cNKMLua.GetData("m_LandTargetThisFrameChangeState", ref m_LandTargetThisFrameChangeState);
		cNKMLua.GetData("m_AirTargetLoopChangeState", ref m_AirTargetLoopChangeState);
		cNKMLua.GetData("m_LandTargetLoopChangeState", ref m_LandTargetLoopChangeState);
		cNKMLua.GetData("m_fGageOffsetX", ref m_fGageOffsetX);
		cNKMLua.GetData("m_fGageOffsetY", ref m_fGageOffsetY);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventText", ref m_listNKMEventText);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventSpeed", ref m_listNKMEventSpeed);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventSpeedX", ref m_listNKMEventSpeedX);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventSpeedY", ref m_listNKMEventSpeedY);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventMove", ref m_listNKMEventMove);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventAttack", ref m_listNKMEventAttack);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventStopTime", ref m_listNKMEventStopTime);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventInvincible", ref m_listNKMEventInvincible);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventInvincibleGlobal", ref m_listNKMEventInvincibleGlobal);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventSuperArmor", ref m_listNKMEventSuperArmor);
		if (cNKMLua.OpenTable("m_listNKMEventSuperArmorGlobal"))
		{
			Log.Error("m_listNKMEventSuperArmorGlobal Obsolete!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitState.cs", 486);
			cNKMLua.CloseTable();
		}
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventSound", ref m_listNKMEventSound);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventColor", ref m_listNKMEventColor);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventCameraCrash", ref m_listNKMEventCameraCrash);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventCameraMove", ref m_listNKMEventCameraMove);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventFadeWorld", ref m_listNKMEventFadeWorld);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventDissolve", ref m_listNKMEventDissolve);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventMotionBlur", ref m_listNKMEventMotionBlur);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventEffect", ref m_listNKMEventEffect);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventHyperSkillCutIn", ref m_listNKMEventHyperSkillCutIn);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventDamageEffect", ref m_listNKMEventDamageEffect);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventDEStateChange", ref m_listNKMEventDEStateChange);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventGameSpeed", ref m_listNKMEventGameSpeed, delegate(INKMUnitStateEvent cEvent)
		{
			if (cEvent is NKMEventGameSpeed)
			{
				NKMEventGameSpeed nKMEventGameSpeed = (NKMEventGameSpeed)cEvent;
				if (nKMEventGameSpeed.m_fGameSpeed < 1f && m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DAMAGE)
				{
					Log.Error($"EventSpeed is too low! : unit[{unitStrId}] state[{m_StateName}] speed[{nKMEventGameSpeed.m_fGameSpeed}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitState.cs", 509);
					return false;
				}
				return true;
			}
			return false;
		});
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventAnimSpeed", ref m_listNKMEventAnimSpeed);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventBuff", ref m_listNKMEventBuff);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventStatus", ref m_listNKMEventStatus);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventRespawn", ref m_listNKMEventRespawn);
		if (cNKMLua.OpenTable("m_NKMEventUnitChange"))
		{
			if (m_NKMEventUnitChange == null)
			{
				m_NKMEventUnitChange = new NKMEventUnitChange();
			}
			m_NKMEventUnitChange.LoadFromLUA(cNKMLua);
			cNKMLua.CloseTable();
		}
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventDie", ref m_listNKMEventDie);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventChangeState", ref m_listNKMEventChangeState);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventAgro", ref m_listNKMEventAgro);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventHeal", ref m_listNKMEventHeal);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventStun", ref m_listNKMEventStun);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventCost", ref m_listNKMEventCost);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventDefence", ref m_listNKMEventDefence);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventDispel", ref m_listNKMEventDispel);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventChangeCooltime", ref m_listNKMEventChangeCooltime);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventCatchEnd", ref m_listNKMEventCatchEnd);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventFindTarget", ref m_listNKMEventFindTarget);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventTrigger", ref m_listNKMEventTrigger);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventChangeRemainTime", ref m_listNKMEventChangeRemainTime);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventVariable", ref m_listNKMEventVariable);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventConsume", ref m_listNKMEventConsume);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventSkillCutIn", ref m_listNKMEventSkillCutIn);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventTriggerBranch", ref m_listNKMEventTriggerBranch);
		LoadAndMergeEventList(cNKMLua, "m_listNKMEventReaction", ref m_listNKMEventReaction);
		cNKMLua.GetDataListEnum("m_listFixedStatusEffect", m_listFixedStatusEffect, bClearList: false);
		cNKMLua.GetDataListEnum("m_listFixedStatusImmune", m_listFixedStatusImmune, bClearList: false);
		return data;
	}

	public void DeepCopyFromSource(NKMUnitState source)
	{
		m_NKM_UNIT_STATE_TYPE = source.m_NKM_UNIT_STATE_TYPE;
		m_StateName = (string)source.m_StateName.Clone();
		m_StateID = source.m_StateID;
		m_StatusEffectType = source.m_StatusEffectType;
		m_NKM_SKILL_TYPE = source.m_NKM_SKILL_TYPE;
		m_listCoolTimeLink.Clear();
		for (int i = 0; i < source.m_listCoolTimeLink.Count; i++)
		{
			m_listCoolTimeLink.Add(source.m_listCoolTimeLink[i]);
		}
		m_AnimName = (string)source.m_AnimName.Clone();
		m_fAnimStartTime = source.m_fAnimStartTime;
		m_fAnimSpeed = source.m_fAnimSpeed;
		m_bAnimSpeedFix = source.m_bAnimSpeedFix;
		m_bAnimLoop = source.m_bAnimLoop;
		m_bSeeTarget = source.m_bSeeTarget;
		m_bSeeMoreEnemy = source.m_bSeeMoreEnemy;
		m_fAirHigh = source.m_fAirHigh;
		m_bChangeIsAirUnit = source.m_bChangeIsAirUnit;
		m_bNoAI = source.m_bNoAI;
		m_bNoMove = source.m_bNoMove;
		m_bNoChangeRight = source.m_bNoChangeRight;
		m_bRun = source.m_bRun;
		m_fRunSpeedRate = source.m_fRunSpeedRate;
		m_bJump = source.m_bJump;
		m_bForceNoTargeted = source.m_bForceNoTargeted;
		m_bNoStateTypeEvent = source.m_bNoStateTypeEvent;
		m_fGAccel = source.m_fGAccel;
		m_bForceRightLeftDependTeam = source.m_bForceRightLeftDependTeam;
		m_bForceRight = source.m_bForceRight;
		m_bForceLeft = source.m_bForceLeft;
		m_bShowGage = source.m_bShowGage;
		m_SuperArmorLevel = source.m_SuperArmorLevel;
		m_bNormalRevengeState = source.m_bNormalRevengeState;
		m_bRevengeState = source.m_bRevengeState;
		m_bSuperRevengeState = source.m_bSuperRevengeState;
		m_RevengeChangeState = source.m_RevengeChangeState;
		m_bInvincibleState = source.m_bInvincibleState;
		m_bNotUseAttackSpeedStat = source.m_bNotUseAttackSpeedStat;
		m_bSkillCutIn = source.m_bSkillCutIn;
		m_bHyperSkillCutIn = source.m_bHyperSkillCutIn;
		m_SkillCutInName = source.m_SkillCutInName;
		m_bAutoCoolTime = source.m_bAutoCoolTime;
		m_StateCoolTime.DeepCopyFromSource(source.m_StateCoolTime);
		m_DangerCharge.DeepCopyFromSource(source.m_DangerCharge);
		m_AnimTimeChangeStateTime = source.m_AnimTimeChangeStateTime;
		m_AnimTimeChangeState = (string)source.m_AnimTimeChangeState.Clone();
		m_AnimTimeRateChangeStateTime = source.m_AnimTimeRateChangeStateTime;
		m_AnimTimeRateChangeState = (string)source.m_AnimTimeRateChangeState.Clone();
		m_StateTimeChangeStateTime = source.m_StateTimeChangeStateTime;
		m_StateTimeChangeState = (string)source.m_StateTimeChangeState.Clone();
		m_TargetDistOverChangeStateDist = source.m_TargetDistOverChangeStateDist;
		m_TargetDistOverChangeState = (string)source.m_TargetDistOverChangeState.Clone();
		m_TargetDistLessChangeStateDist = source.m_TargetDistLessChangeStateDist;
		m_TargetDistLessChangeState = (string)source.m_TargetDistLessChangeState.Clone();
		m_TargetLostOrDieStateDurationTime = source.m_TargetLostOrDieStateDurationTime;
		m_TargetLostOrDieState = (string)source.m_TargetLostOrDieState.Clone();
		m_AnimEndChangeStatePlayCount = source.m_AnimEndChangeStatePlayCount;
		m_AnimEndChangeState = (string)source.m_AnimEndChangeState.Clone();
		m_MapPosOverStatePos = source.m_MapPosOverStatePos;
		m_MapPosOverState = (string)source.m_MapPosOverState.Clone();
		m_FootOffLandChangeState = source.m_FootOffLandChangeState;
		m_FootOnLandChangeState = (string)source.m_FootOnLandChangeState.Clone();
		m_AnimEndFootOnLandChangeState = (string)source.m_AnimEndFootOnLandChangeState.Clone();
		m_AnimEndFootOffLandChangeState = (string)source.m_AnimEndFootOffLandChangeState.Clone();
		m_SpeedYPositiveChangeState = source.m_SpeedYPositiveChangeState;
		m_SpeedY0NegativeChangeState = source.m_SpeedY0NegativeChangeState;
		m_DamagedChangeState = source.m_DamagedChangeState;
		m_AnimEndDyingState = source.m_AnimEndDyingState;
		m_ChangeRightState = source.m_ChangeRightState;
		m_ChangeRightTrueState = source.m_ChangeRightTrueState;
		m_ChangeRightFalseState = source.m_ChangeRightFalseState;
		m_AirTargetThisFrameChangeState = source.m_AirTargetThisFrameChangeState;
		m_LandTargetThisFrameChangeState = source.m_LandTargetThisFrameChangeState;
		m_AirTargetLoopChangeState = source.m_AirTargetLoopChangeState;
		m_LandTargetLoopChangeState = source.m_LandTargetLoopChangeState;
		m_fGageOffsetX = source.m_fGageOffsetX;
		m_fGageOffsetY = source.m_fGageOffsetY;
		if (source.m_NKMEventUnitChange == null)
		{
			m_NKMEventUnitChange = null;
		}
		else
		{
			m_NKMEventUnitChange = source.m_NKMEventUnitChange;
		}
		DeepCopy(source.m_listNKMEventText, ref m_listNKMEventText, delegate(NKMEventText t, NKMEventText s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventSpeed, ref m_listNKMEventSpeed, delegate(NKMEventSpeed t, NKMEventSpeed s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventSpeedX, ref m_listNKMEventSpeedX, delegate(NKMEventSpeedX t, NKMEventSpeedX s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventSpeedY, ref m_listNKMEventSpeedY, delegate(NKMEventSpeedY t, NKMEventSpeedY s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventMove, ref m_listNKMEventMove, delegate(NKMEventMove t, NKMEventMove s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventAttack, ref m_listNKMEventAttack, delegate(NKMEventAttack t, NKMEventAttack s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventStopTime, ref m_listNKMEventStopTime, delegate(NKMEventStopTime t, NKMEventStopTime s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventInvincible, ref m_listNKMEventInvincible, delegate(NKMEventInvincible t, NKMEventInvincible s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventInvincibleGlobal, ref m_listNKMEventInvincibleGlobal, delegate(NKMEventInvincibleGlobal t, NKMEventInvincibleGlobal s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventSuperArmor, ref m_listNKMEventSuperArmor, delegate(NKMEventSuperArmor t, NKMEventSuperArmor s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventSound, ref m_listNKMEventSound, delegate(NKMEventSound t, NKMEventSound s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventColor, ref m_listNKMEventColor, delegate(NKMEventColor t, NKMEventColor s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventCameraCrash, ref m_listNKMEventCameraCrash, delegate(NKMEventCameraCrash t, NKMEventCameraCrash s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventCameraMove, ref m_listNKMEventCameraMove, delegate(NKMEventCameraMove t, NKMEventCameraMove s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventFadeWorld, ref m_listNKMEventFadeWorld, delegate(NKMEventFadeWorld t, NKMEventFadeWorld s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventDissolve, ref m_listNKMEventDissolve, delegate(NKMEventDissolve t, NKMEventDissolve s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventMotionBlur, ref m_listNKMEventMotionBlur, delegate(NKMEventMotionBlur t, NKMEventMotionBlur s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventEffect, ref m_listNKMEventEffect, delegate(NKMEventEffect t, NKMEventEffect s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventHyperSkillCutIn, ref m_listNKMEventHyperSkillCutIn, delegate(NKMEventHyperSkillCutIn t, NKMEventHyperSkillCutIn s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventDamageEffect, ref m_listNKMEventDamageEffect, delegate(NKMEventDamageEffect t, NKMEventDamageEffect s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventDEStateChange, ref m_listNKMEventDEStateChange, delegate(NKMEventDEStateChange t, NKMEventDEStateChange s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventGameSpeed, ref m_listNKMEventGameSpeed, delegate(NKMEventGameSpeed t, NKMEventGameSpeed s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventAnimSpeed, ref m_listNKMEventAnimSpeed, delegate(NKMEventAnimSpeed t, NKMEventAnimSpeed s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventBuff, ref m_listNKMEventBuff, delegate(NKMEventBuff t, NKMEventBuff s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventStatus, ref m_listNKMEventStatus, delegate(NKMEventStatus t, NKMEventStatus s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventRespawn, ref m_listNKMEventRespawn, delegate(NKMEventRespawn t, NKMEventRespawn s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventDie, ref m_listNKMEventDie, delegate(NKMEventDie t, NKMEventDie s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventChangeState, ref m_listNKMEventChangeState, delegate(NKMEventChangeState t, NKMEventChangeState s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventAgro, ref m_listNKMEventAgro, delegate(NKMEventAgro t, NKMEventAgro s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventHeal, ref m_listNKMEventHeal, delegate(NKMEventHeal t, NKMEventHeal s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventStun, ref m_listNKMEventStun, delegate(NKMEventStun t, NKMEventStun s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventCost, ref m_listNKMEventCost, delegate(NKMEventCost t, NKMEventCost s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventDefence, ref m_listNKMEventDefence, delegate(NKMEventDefence t, NKMEventDefence s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventDispel, ref m_listNKMEventDispel, delegate(NKMEventDispel t, NKMEventDispel s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventChangeCooltime, ref m_listNKMEventChangeCooltime, delegate(NKMEventChangeCooltime t, NKMEventChangeCooltime s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventCatchEnd, ref m_listNKMEventCatchEnd, delegate(NKMEventCatchEnd t, NKMEventCatchEnd s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventFindTarget, ref m_listNKMEventFindTarget, delegate(NKMEventFindTarget t, NKMEventFindTarget s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventTrigger, ref m_listNKMEventTrigger, delegate(NKMEventTrigger t, NKMEventTrigger s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventChangeRemainTime, ref m_listNKMEventChangeRemainTime, delegate(NKMEventChangeRemainTime t, NKMEventChangeRemainTime s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventVariable, ref m_listNKMEventVariable, delegate(NKMEventVariable t, NKMEventVariable s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventConsume, ref m_listNKMEventConsume, delegate(NKMEventConsume t, NKMEventConsume s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventSkillCutIn, ref m_listNKMEventSkillCutIn, delegate(NKMEventSkillCutIn t, NKMEventSkillCutIn s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventTriggerBranch, ref m_listNKMEventTriggerBranch, delegate(NKMEventTriggerBranch t, NKMEventTriggerBranch s)
		{
			t.DeepCopyFromSource(s);
		});
		DeepCopy(source.m_listNKMEventReaction, ref m_listNKMEventReaction, delegate(NKMEventReaction t, NKMEventReaction s)
		{
			t.DeepCopyFromSource(s);
		});
		m_listFixedStatusEffect.Clear();
		m_listFixedStatusEffect.UnionWith(source.m_listFixedStatusEffect);
		m_listFixedStatusImmune.Clear();
		m_listFixedStatusImmune.UnionWith(source.m_listFixedStatusImmune);
	}

	public IEnumerable<IReadOnlyList<INKMUnitStateEvent>> StateEventLists()
	{
		yield return m_listNKMEventText;
		yield return m_listNKMEventSpeed;
		yield return m_listNKMEventSpeedX;
		yield return m_listNKMEventSpeedY;
		yield return m_listNKMEventMove;
		yield return m_listNKMEventAttack;
		yield return m_listNKMEventStopTime;
		yield return m_listNKMEventInvincible;
		yield return m_listNKMEventInvincibleGlobal;
		yield return m_listNKMEventSuperArmor;
		yield return m_listNKMEventSound;
		yield return m_listNKMEventColor;
		yield return m_listNKMEventCameraCrash;
		yield return m_listNKMEventCameraMove;
		yield return m_listNKMEventFadeWorld;
		yield return m_listNKMEventDissolve;
		yield return m_listNKMEventMotionBlur;
		yield return m_listNKMEventEffect;
		yield return m_listNKMEventHyperSkillCutIn;
		yield return m_listNKMEventDamageEffect;
		yield return m_listNKMEventDEStateChange;
		yield return m_listNKMEventGameSpeed;
		yield return m_listNKMEventAnimSpeed;
		yield return m_listNKMEventBuff;
		yield return m_listNKMEventStatus;
		yield return m_listNKMEventRespawn;
		yield return m_listNKMEventDie;
		yield return m_listNKMEventChangeState;
		yield return m_listNKMEventAgro;
		yield return m_listNKMEventHeal;
		yield return m_listNKMEventStun;
		yield return m_listNKMEventCost;
		yield return m_listNKMEventDefence;
		yield return m_listNKMEventDispel;
		yield return m_listNKMEventChangeCooltime;
		yield return m_listNKMEventCatchEnd;
		yield return m_listNKMEventFindTarget;
		yield return m_listNKMEventTrigger;
		yield return m_listNKMEventVariable;
		yield return m_listNKMEventChangeRemainTime;
		yield return m_listNKMEventConsume;
		yield return m_listNKMEventSkillCutIn;
		yield return m_listNKMEventTriggerBranch;
		yield return m_listNKMEventReaction;
	}

	public float CalcaluateMaxRollbackTime(string unitStrID)
	{
		float num = NKMCommonConst.SUMMON_UNIT_NOEVENT_TIME;
		foreach (IReadOnlyList<INKMUnitStateEvent> item in StateEventLists())
		{
			num = NKMMathf.Min(num, GetMaxRollbackTime(item, unitStrID));
		}
		return num;
	}

	private float GetMaxRollbackTime<T>(IReadOnlyList<T> lstEvent, string unitStrID) where T : INKMUnitStateEvent
	{
		float num = NKMCommonConst.SUMMON_UNIT_NOEVENT_TIME;
		foreach (T item in lstEvent)
		{
			switch (item.RollbackType)
			{
			case EventRollbackType.Warning:
			{
				float num3 = ((!item.bAnimTime) ? num : (m_fAnimStartTime + m_fAnimSpeed * num));
				if (item.EventStartTime < num3)
				{
					if (item.bAnimTime)
					{
						Log.Warn($"{unitStrID}.{m_StateName} : 롤백 위험 이벤트 {item.ToString()} (animTime {item.EventStartTime}s) 감지", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitState.cs", 851);
					}
					else
					{
						Log.Warn($"{unitStrID}.{m_StateName} : 롤백 위험 이벤트 {item.ToString()} (stateTime {item.EventStartTime}s) 감지", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitState.cs", 855);
					}
				}
				break;
			}
			case EventRollbackType.Prohibited:
			{
				float num2 = ((!item.bAnimTime) ? num : (m_fAnimStartTime + m_fAnimSpeed * num));
				if (item.EventStartTime < num)
				{
					num = item.EventStartTime;
				}
				if (item.EventStartTime < num2)
				{
					if (item.bAnimTime)
					{
						Log.Error($"{unitStrID}.{m_StateName} : 롤백 금지 이벤트 {item.ToString()} (animTime {item.EventStartTime}s) 감지", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitState.cs", 880);
					}
					else
					{
						Log.Error($"{unitStrID}.{m_StateName} : 롤백 금지 이벤트 {item.ToString()} (stateTime {item.EventStartTime}s) 감지", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitState.cs", 884);
					}
				}
				break;
			}
			}
		}
		return num;
	}
}
