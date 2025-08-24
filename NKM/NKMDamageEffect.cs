using System;
using System.Collections.Generic;
using Cs.Logging;
using Cs.Math;
using NKM.Templet;

namespace NKM;

public class NKMDamageEffect : NKMObjectPoolData
{
	protected NKM_DAMAGE_EFFECT_CLASS_TYPE m_NKM_DAMAGE_EFFECT_CLASS_TYPE;

	protected NKMGame m_NKMGame;

	protected NKMDamageEffectManager m_DEManager;

	protected short m_DamageEffectUID;

	protected NKMDamageEffectTemplet m_DETemplet;

	protected NKMDamageEffectData m_DEData = new NKMDamageEffectData();

	protected float m_fDeltaTime;

	protected NKMDamageEffectState m_EffectStateNow;

	protected string m_StateNameNow;

	protected string m_StateNameNext;

	protected NKMTrackingFloat m_EventMovePosX = new NKMTrackingFloat();

	protected NKMTrackingFloat m_EventMovePosZ = new NKMTrackingFloat();

	protected NKMTrackingFloat m_EventMovePosJumpY = new NKMTrackingFloat();

	protected float m_fEventMoveSavedPositionX;

	protected float m_fEventMoveSavedPositionY;

	protected Dictionary<int, NKMDamageInst> m_dictDamageInstAtk = new Dictionary<int, NKMDamageInst>();

	protected NKMUnitSkillTemplet m_UnitSkillTemplet;

	protected int m_MasterUnitPhase;

	protected LinkedList<NKMDamageEffect> m_linklistDamageEffect = new LinkedList<NKMDamageEffect>();

	protected Dictionary<float, NKMTimeStamp> m_EventTimeStampAnim = new Dictionary<float, NKMTimeStamp>();

	protected Dictionary<float, NKMTimeStamp> m_EventTimeStampState = new Dictionary<float, NKMTimeStamp>();

	protected NKMVector3 m_NKMVector3Temp1;

	protected NKMVector3 m_NKMVector3Temp2;

	protected NKMVector3 m_NKMVector3Temp3;

	protected bool m_bSortUnitDirty = true;

	protected bool m_bSortUnitBySizeDirty = true;

	protected float m_TempSortDist;

	protected List<NKMUnit> m_listSortUnit = new List<NKMUnit>();

	protected List<NKMUnit> m_listSortUnitBySize = new List<NKMUnit>();

	protected NKMUnit m_TargetUnit;

	protected NKMDamageInst GetDamageInstAtk(int index)
	{
		if (m_dictDamageInstAtk.TryGetValue(index, out var value))
		{
			return value;
		}
		NKMDamageInst nKMDamageInst = new NKMDamageInst();
		m_dictDamageInstAtk.Add(index, nKMDamageInst);
		return nKMDamageInst;
	}

	public float GetTempSortDist()
	{
		return m_TempSortDist;
	}

	public NKMDamageEffect()
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKMDamageEffect;
		m_NKM_DAMAGE_EFFECT_CLASS_TYPE = NKM_DAMAGE_EFFECT_CLASS_TYPE.NDECT_NKM;
		Init();
	}

	public void Init()
	{
		m_DamageEffectUID = 0;
		m_DETemplet = null;
		m_DEData.Init();
		m_fDeltaTime = 0f;
		m_EffectStateNow = null;
		m_StateNameNow = "";
		m_StateNameNext = "";
		m_dictDamageInstAtk.Clear();
		m_UnitSkillTemplet = null;
		m_TargetUnit = null;
		m_EventMovePosX.StopTracking();
		m_EventMovePosZ.StopTracking();
		m_EventMovePosJumpY.StopTracking();
		Dictionary<float, NKMTimeStamp>.Enumerator enumerator = m_EventTimeStampAnim.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMTimeStamp value = enumerator.Current.Value;
			m_NKMGame.GetObjectPool().CloseObj(value);
		}
		m_EventTimeStampAnim.Clear();
		Dictionary<float, NKMTimeStamp>.Enumerator enumerator2 = m_EventTimeStampState.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			NKMTimeStamp value2 = enumerator2.Current.Value;
			m_NKMGame.GetObjectPool().CloseObj(value2);
		}
		m_EventTimeStampState.Clear();
		foreach (NKMDamageEffect item in m_linklistDamageEffect)
		{
			item.SetDie(bForce: false, bDieEvent: false);
		}
		m_linklistDamageEffect.Clear();
	}

	public override void Close()
	{
		Init();
	}

	public virtual bool SetDamageEffect(NKMGame cNKMGame, NKMDamageEffectManager cDEManager, NKMUnitSkillTemplet cSkillTemplet, int masterUnitPhase, short deUID, string deTempletID, short masterGameUnitUID, short targetGameUnitUID, float fX, float fY, float fZ, bool bRight, NKMEventPosData.MoveOffset moveOffset, float fPosMapRate, float offsetX = 0f, float offsetY = 0f, float offsetZ = 0f, float fAddRotate = 0f, bool bUseZScale = true, float fSpeedFactorX = 0f, float fSpeedFactorY = 0f)
	{
		m_NKMGame = cNKMGame;
		m_DEManager = cDEManager;
		m_DamageEffectUID = deUID;
		m_DETemplet = NKMDETempletManager.GetDETemplet(deTempletID);
		m_DEData.m_MasterGameUnitUID = masterGameUnitUID;
		m_DEData.m_TargetGameUnitUID = targetGameUnitUID;
		m_DEData.m_MasterUnit = null;
		m_DEData.m_MasterUnit = GetMasterUnit();
		if (m_DEData.m_MasterUnit == null)
		{
			return false;
		}
		m_DEData.m_NKM_TEAM_TYPE = m_DEData.m_MasterUnit.GetUnitDataGame().m_NKM_TEAM_TYPE;
		m_DEData.m_StatData = m_DEData.m_MasterUnit.GetUnitFrameData().m_StatData;
		m_DEData.m_UnitData = m_DEData.m_MasterUnit.GetUnitData();
		m_DEData.m_MoveOffset = moveOffset;
		m_DEData.m_fMapPosFactor = fPosMapRate;
		m_DEData.m_fOffsetX = offsetX;
		m_DEData.m_fOffsetY = offsetY;
		m_DEData.m_fOffsetZ = offsetZ;
		m_DEData.m_fAddRotate = fAddRotate;
		m_DEData.m_bUseZScale = bUseZScale;
		m_DEData.m_fSpeedFactorX = fSpeedFactorX;
		m_DEData.m_fSpeedFactorY = fSpeedFactorY;
		m_DEData.m_fEventDirSpeed = 0f;
		m_DEData.m_bRight = bRight;
		if (m_DEData.m_bRight)
		{
			m_DEData.m_DirVector.x = 1f;
		}
		else
		{
			m_DEData.m_DirVector.x = -1f;
		}
		m_DEData.m_DirVectorTrackX.SetNowValue(m_DEData.m_DirVector.x);
		m_UnitSkillTemplet = cSkillTemplet;
		m_MasterUnitPhase = masterUnitPhase;
		m_TargetUnit = GetTargetUnit();
		SetPos(fX, fY, fZ, bUseOffset: true);
		m_DEData.m_TargetDirSpeed.SetNowValue(m_DETemplet?.m_fTargetDirSpeed ?? 0f);
		MakeDirVec();
		m_EventMovePosX.StopTracking();
		m_EventMovePosZ.StopTracking();
		m_EventMovePosJumpY.StopTracking();
		SaveEventMovePosition(fX, fY, bSaveXOnly: false);
		StateChange("DES_BASE");
		return true;
	}

	public void SetHoldFollowData(TRACKING_DATA_TYPE followTrackingType, float followTime, float resetTime)
	{
		m_DEData.m_FollowTrackingDataType = followTrackingType;
		m_DEData.m_fFollowTime = followTime;
		m_DEData.m_fFollowResetTime = resetTime;
	}

	public void SetStateEndDie(bool bStateEndStop)
	{
		m_DEData.m_bStateEndStop = bStateEndStop;
	}

	public bool GetStateEndDie()
	{
		return m_DEData.m_bStateEndStop;
	}

	public void SetFollowPos(float fX, float fY, float fZ)
	{
		if (m_DEData.m_FollowTrackingDataType == TRACKING_DATA_TYPE.TDT_INVALID)
		{
			return;
		}
		if (m_DEData.m_fFollowTime == 0f || m_DEData.m_FollowTrackingDataType == TRACKING_DATA_TYPE.TDT_NORMAL)
		{
			SetPos(fX, fY, fZ, bUseOffset: true);
			return;
		}
		float eventPosX = GetEventPosX(fX, m_DEData.m_MoveOffset, m_DEData.m_fOffsetX, IsATeam(), m_DEData.m_fMapPosFactor);
		float num = fY + m_DEData.m_fOffsetY;
		float num2 = fZ + m_DEData.m_fOffsetZ;
		m_DEData.m_fFollowUpdateTime -= m_fDeltaTime;
		if (!(m_DEData.m_fFollowUpdateTime > 0f))
		{
			if (!m_EventMovePosX.IsTracking() || m_EventMovePosX.GetTargetValue() != eventPosX)
			{
				m_EventMovePosX.SetTracking(eventPosX, m_DEData.m_fFollowTime, m_DEData.m_FollowTrackingDataType);
			}
			if (!m_EventMovePosJumpY.IsTracking() || m_EventMovePosJumpY.GetTargetValue() != num)
			{
				m_EventMovePosJumpY.SetTracking(num, m_DEData.m_fFollowTime, m_DEData.m_FollowTrackingDataType);
			}
			if (!m_EventMovePosZ.IsTracking() || m_EventMovePosZ.GetTargetValue() != num2)
			{
				m_EventMovePosZ.SetTracking(num2, m_DEData.m_fFollowTime, m_DEData.m_FollowTrackingDataType);
			}
			m_DEData.m_fFollowUpdateTime = m_DEData.m_fFollowResetTime;
		}
	}

	public void SetPos(float fX, float fY, float fZ, bool bUseOffset)
	{
		m_DEData.m_PosX = fX;
		m_DEData.m_PosZ = fZ;
		m_DEData.m_JumpYPos = fY;
		if (bUseOffset)
		{
			if (m_DEData.m_bRight)
			{
				m_DEData.m_PosX += m_DEData.m_fOffsetX;
			}
			else
			{
				m_DEData.m_PosX -= m_DEData.m_fOffsetX;
			}
			m_DEData.m_PosZ += m_DEData.m_fOffsetZ;
			m_DEData.m_JumpYPos += m_DEData.m_fOffsetY;
		}
		if (GetTemplet().m_bLandConnect)
		{
			m_DEData.m_JumpYPos = 0f;
		}
		m_DEData.m_PosXBefore = m_DEData.m_PosX;
		m_DEData.m_PosZBefore = m_DEData.m_PosZ;
		m_DEData.m_JumpYPosBefore = m_DEData.m_JumpYPos;
	}

	public virtual void SetRight(bool bRight)
	{
		if (m_DEData.m_FollowTrackingDataType != TRACKING_DATA_TYPE.TDT_INVALID)
		{
			m_DEData.m_bRight = bRight;
		}
	}

	public virtual void Update(float deltaTime)
	{
		m_fDeltaTime = deltaTime;
		m_bSortUnitDirty = true;
		m_bSortUnitBySizeDirty = true;
		if (GetDEData().m_PosXBefore.IsNearlyEqual(-1f))
		{
			GetDEData().m_PosXBefore = GetDEData().m_PosX;
		}
		if (GetDEData().m_PosZBefore.IsNearlyEqual(-1f))
		{
			GetDEData().m_PosZBefore = GetDEData().m_PosZ;
		}
		if (GetDEData().m_JumpYPosBefore.IsNearlyEqual(-1f))
		{
			GetDEData().m_JumpYPosBefore = GetDEData().m_JumpYPos;
		}
		m_TargetUnit = GetTargetUnit();
		if (!m_DEData.m_bStateFirstFrame)
		{
			StateTimeUpdate();
			AnimTimeUpdate();
		}
		DoStateEndStart();
		StateUpdate();
		StateEvent();
		DieCheck();
		m_DEData.m_bStateFirstFrame = false;
	}

	public List<NKMUnit> GetSortUnitListByNearDist(bool bUseUnitSize)
	{
		if (bUseUnitSize)
		{
			return GetSortUnitListByNearDistBySize();
		}
		return GetSortUnitListByNearDist();
	}

	public List<NKMUnit> GetSortUnitListByNearDist()
	{
		if (m_bSortUnitDirty)
		{
			m_listSortUnit.Clear();
			List<NKMUnit> unitChain = m_NKMGame.GetUnitChain();
			for (int i = 0; i < unitChain.Count; i++)
			{
				NKMUnit nKMUnit = unitChain[i];
				if (nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_ENV)
				{
					nKMUnit.CalcSortDist(GetDEData().m_PosX);
					m_listSortUnit.Add(nKMUnit);
				}
			}
			m_listSortUnit.Sort((NKMUnit a, NKMUnit b) => a.GetTempSortDist().CompareTo(b.GetTempSortDist()));
			m_bSortUnitDirty = false;
		}
		return m_listSortUnit;
	}

	public List<NKMUnit> GetSortUnitListByNearDistBySize()
	{
		if (m_bSortUnitBySizeDirty)
		{
			m_listSortUnitBySize.Clear();
			List<NKMUnit> unitChain = m_NKMGame.GetUnitChain();
			for (int i = 0; i < unitChain.Count; i++)
			{
				NKMUnit nKMUnit = unitChain[i];
				if (nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_ENV)
				{
					float dist = nKMUnit.GetDist(GetDEData().m_PosX, bUseUnitSize: true);
					nKMUnit.SetSortDist(dist);
					m_listSortUnitBySize.Add(nKMUnit);
				}
			}
			m_listSortUnitBySize.Sort((NKMUnit a, NKMUnit b) => a.GetTempSortDist().CompareTo(b.GetTempSortDist()));
			m_bSortUnitBySizeDirty = false;
		}
		return m_listSortUnitBySize;
	}

	protected void DieCheck()
	{
		if (m_EffectStateNow == null)
		{
			return;
		}
		if (m_EffectStateNow.m_NKM_LIFE_TIME_TYPE == NKM_LIFE_TIME_TYPE.NLTT_TIME && m_DEData.m_fLifeTimeMax <= m_DEData.m_fStateTime)
		{
			SetDie();
		}
		if (m_EffectStateNow.m_NKM_LIFE_TIME_TYPE == NKM_LIFE_TIME_TYPE.NLTT_ANIM_COUNT && m_DEData.m_AnimPlayCount >= m_EffectStateNow.m_LifeTimeAnimCount)
		{
			SetDie();
		}
		if (m_DETemplet.m_bDamageCountMaxDie && m_DEData.m_DamageCountNow >= m_DETemplet.m_DamageCountMax)
		{
			SetDie();
		}
		if (!(m_DETemplet.m_fTargetDistDie > 0f))
		{
			return;
		}
		bool flag = false;
		if (m_TargetUnit == null || m_TargetUnit.IsDyingOrDie())
		{
			flag = true;
		}
		if (m_DETemplet.m_bTargetDistDieOnlyTargetDie && !flag)
		{
			return;
		}
		bool flag2 = false;
		if (Math.Abs(m_DEData.m_fLastTargetPosX - m_DEData.m_PosX) <= m_DETemplet.m_fTargetDistDie)
		{
			flag2 = true;
		}
		if (!flag2 && !m_DEData.m_PosXBefore.IsNearlyEqual(-1f))
		{
			if (m_DEData.m_PosXBefore >= m_DEData.m_fLastTargetPosX && m_DEData.m_PosX < m_DEData.m_fLastTargetPosX)
			{
				flag2 = true;
			}
			if (m_DEData.m_PosX >= m_DEData.m_fLastTargetPosX && m_DEData.m_PosXBefore < m_DEData.m_fLastTargetPosX)
			{
				flag2 = true;
			}
		}
		if (flag2)
		{
			SetDie();
		}
	}

	protected void StateTimeUpdate()
	{
		m_DEData.m_fStateTimeBack = m_DEData.m_fStateTime;
		m_DEData.m_fStateTime += m_fDeltaTime;
	}

	protected void AnimTimeUpdate()
	{
		if (m_EffectStateNow == null)
		{
			return;
		}
		if (m_DEData.m_fAnimTime >= m_DEData.m_fAnimTimeMax)
		{
			if (m_EffectStateNow.m_bAnimLoop)
			{
				m_DEData.m_fAnimTimeBack = 0f;
				m_DEData.m_fAnimTime = 0f;
				Dictionary<float, NKMTimeStamp>.Enumerator enumerator = m_EventTimeStampAnim.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NKMTimeStamp value = enumerator.Current.Value;
					m_NKMGame.GetObjectPool().CloseObj(value);
				}
				m_EventTimeStampAnim.Clear();
			}
			m_DEData.m_AnimPlayCount += 1f;
		}
		m_DEData.m_fAnimTimeBack = m_DEData.m_fAnimTime;
		if (m_DETemplet.m_UseMasterAnimSpeed && GetMasterUnit() != null && GetMasterUnit().GetUnitFrameData() != null)
		{
			m_DEData.m_fAnimTime += m_fDeltaTime * GetMasterUnit().GetUnitFrameData().m_fAnimSpeed;
		}
		else
		{
			m_DEData.m_fAnimTime += m_fDeltaTime * m_EffectStateNow.m_fAnimSpeed;
		}
	}

	public void DoStateEndStart()
	{
		if (m_StateNameNext.Length <= 1)
		{
			return;
		}
		StateEnd();
		m_StateNameNow = m_StateNameNext;
		m_StateNameNext = "";
		if (m_DETemplet == null)
		{
			Log.Error("m_DETemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffect.cs", 643);
			return;
		}
		m_EffectStateNow = m_DETemplet.GetState(m_StateNameNow);
		if (m_EffectStateNow == null)
		{
			Log.Error(m_DETemplet.m_MainEffectName + " m_EffectStateNow is null stateName: " + m_StateNameNow, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffect.cs", 650);
		}
		else
		{
			StateStart();
		}
	}

	protected virtual void StateEnd()
	{
		if (m_EffectStateNow != null)
		{
			ProcessEventDirSpeed(bStateEnd: true);
			ProcessEventSpeedX(bStateEnd: true);
			ProcessEventSpeedY(bStateEnd: true);
			ProcessEventMove(bStateEnd: true);
			ProcessEventSound(bStateEnd: true);
			ProcessEventCameraCrash(bStateEnd: true);
			ProcessEventDissolve(bStateEnd: true);
			ProcessEventEffect(bStateEnd: true);
			ProcessEventDamageEffect(bStateEnd: true);
			ProcessEventBuff(bStateEnd: true);
			ProcessEventStatus(bStateEnd: true);
			ProcessEventHeal(bStateEnd: true);
		}
	}

	protected virtual void StateStart()
	{
		m_DEData.m_bStateFirstFrame = true;
		if (m_DETemplet == null)
		{
			Log.Error("m_DETemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffect.cs", 681);
			return;
		}
		if (m_EffectStateNow == null)
		{
			Log.Error(m_DETemplet.m_MainEffectName + " m_EffectStateNow is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffect.cs", 686);
			return;
		}
		m_DEData.m_fAnimTimeBack = 0f;
		m_DEData.m_fAnimTime = 0f;
		m_DEData.m_fAnimTimeMax = NKMAnimDataManager.GetAnimTimeMax(m_DETemplet.m_MainEffectName, m_DETemplet.m_MainEffectName, m_EffectStateNow.m_AnimName);
		if (m_DEData.m_fAnimTimeMax.IsNearlyZero())
		{
			Log.Error("NKMDamageEffect NoExistAnim: " + m_DETemplet.m_MainEffectName + " : " + m_EffectStateNow.m_AnimName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffect.cs", 695);
			return;
		}
		m_DEData.m_AnimPlayCount = 0f;
		m_DEData.m_fStateTime = 0f;
		m_DEData.m_fStateTimeBack = 0f;
		Dictionary<float, NKMTimeStamp>.Enumerator enumerator = m_EventTimeStampAnim.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMTimeStamp value = enumerator.Current.Value;
			m_NKMGame.GetObjectPool().CloseObj(value);
		}
		m_EventTimeStampAnim.Clear();
		Dictionary<float, NKMTimeStamp>.Enumerator enumerator2 = m_EventTimeStampState.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			NKMTimeStamp value2 = enumerator2.Current.Value;
			m_NKMGame.GetObjectPool().CloseObj(value2);
		}
		m_EventTimeStampState.Clear();
		SetLifeTimeMax();
		SeeTarget();
	}

	protected virtual void StateUpdate()
	{
		if (m_EffectStateNow != null)
		{
			m_DEData.m_fSeeTargetTimeNow -= m_fDeltaTime;
			if (m_DEData.m_fSeeTargetTimeNow <= 0f)
			{
				m_DEData.m_fSeeTargetTimeNow = m_DETemplet.m_fSeeTargetTime;
				SeeTarget();
			}
			ProcessTarget();
			ProcessEventTargetDirSpeed();
			ProcessEventDirSpeed();
			ProcessEventSpeedX();
			ProcessEventSpeedY();
			ProcessEventMove();
			ProcessEventAttack();
			ProcessEventSound();
			ProcessEventCameraCrash();
			ProcessEventDissolve();
			ProcessEventEffect();
			ProcessEventDamageEffect();
			ProcessEventBuff();
			ProcessEventStatus();
			ProcessEventHeal();
			PhysicProcess();
			MapEdgeProcess();
			Dictionary<float, NKMTimeStamp>.Enumerator enumerator = m_EventTimeStampAnim.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Value.m_FramePass = true;
			}
			Dictionary<float, NKMTimeStamp>.Enumerator enumerator2 = m_EventTimeStampState.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				enumerator2.Current.Value.m_FramePass = true;
			}
		}
	}

	protected virtual void StateEvent()
	{
		if (m_EffectStateNow == null)
		{
			return;
		}
		if (m_EffectStateNow.m_StateTimeChangeStateTime >= 0f && m_DEData.m_fStateTime >= m_EffectStateNow.m_StateTimeChangeStateTime)
		{
			StateChange(m_EffectStateNow.m_StateTimeChangeState);
		}
		else if (m_EffectStateNow.m_AnimEndChangeState.Length > 1 && m_DEData.m_fAnimTime >= m_DEData.m_fAnimTimeMax)
		{
			StateChange(m_EffectStateNow.m_AnimEndChangeState);
		}
		else if (m_EffectStateNow.m_FootOnLandChangeState.Length > 1 && m_DEData.m_bFootOnLand)
		{
			StateChange(m_EffectStateNow.m_FootOnLandChangeState);
		}
		else if (m_EffectStateNow.m_DamageCountChangeStateCount > 0 && m_DEData.m_DamageCountNow >= m_EffectStateNow.m_DamageCountChangeStateCount)
		{
			StateChange(m_EffectStateNow.m_DamageCountChangeState);
		}
		else if (m_EffectStateNow.m_TargetDistFarChangeStateDist > 0f && !m_DEData.m_fLastTargetPosX.IsNearlyZero() && Math.Abs(m_DEData.m_fLastTargetPosX - m_DEData.m_PosX) >= m_EffectStateNow.m_TargetDistFarChangeStateDist)
		{
			StateChange(m_EffectStateNow.m_TargetDistFarChangeState);
		}
		else
		{
			if (!(m_EffectStateNow.m_TargetDistNearChangeStateDist > 0f) || m_DEData.m_fLastTargetPosX.IsNearlyZero())
			{
				return;
			}
			bool flag = false;
			if (Math.Abs(m_DEData.m_fLastTargetPosX - m_DEData.m_PosX) <= m_EffectStateNow.m_TargetDistNearChangeStateDist)
			{
				flag = true;
			}
			if (!flag && !m_DEData.m_PosXBefore.IsNearlyEqual(-1f))
			{
				if (m_DEData.m_PosXBefore >= m_DEData.m_fLastTargetPosX && m_DEData.m_PosX < m_DEData.m_fLastTargetPosX)
				{
					flag = true;
				}
				if (m_DEData.m_PosX >= m_DEData.m_fLastTargetPosX && m_DEData.m_PosXBefore < m_DEData.m_fLastTargetPosX)
				{
					flag = true;
				}
			}
			if (flag)
			{
				StateChange(m_EffectStateNow.m_TargetDistNearChangeState);
			}
		}
	}

	public void StateChangeByUnitState(string stateName, bool bForceChange = true)
	{
		StateChange(stateName, bForceChange);
	}

	protected virtual void StateChange(string stateName, bool bForceChange = true)
	{
		if (bForceChange || stateName.CompareTo(m_EffectStateNow.m_StateName) != 0)
		{
			if (m_DETemplet == null)
			{
				Log.Error("StateChange m_DETemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffect.cs", 860);
			}
			else if (m_DETemplet.GetState(stateName) == null)
			{
				Log.Error("StateChange " + m_DETemplet.m_MainEffectName + " GetState is null stateName: " + stateName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffect.cs", 867);
			}
			else
			{
				m_StateNameNext = stateName;
			}
		}
	}

	protected void SetLifeTimeMax()
	{
		if (m_EffectStateNow != null)
		{
			switch (m_EffectStateNow.m_NKM_LIFE_TIME_TYPE)
			{
			case NKM_LIFE_TIME_TYPE.NLTT_INFINITY:
				m_DEData.m_fLifeTimeMax = 0f;
				break;
			case NKM_LIFE_TIME_TYPE.NLTT_TIME:
			case NKM_LIFE_TIME_TYPE.NLTT_ANIM_COUNT:
				m_DEData.m_fLifeTimeMax = m_EffectStateNow.m_LifeTime;
				break;
			}
		}
	}

	public bool IsEnd()
	{
		return m_DEData.m_bDie;
	}

	public virtual void SetDie(bool bForce = false, bool bDieEvent = true)
	{
		if (!m_DEData.m_bDie)
		{
			if (bDieEvent)
			{
				ProcessDieEventAttack();
				ProcessDieEventEffect();
				ProcessDieEventDamageEffect();
				ProcessDieEventSound();
			}
			m_DEData.m_bDie = true;
			if (bForce)
			{
				m_DEManager.DeleteDE(m_DamageEffectUID);
			}
		}
	}

	protected void SeeTarget()
	{
		if (m_TargetUnit == null)
		{
			return;
		}
		bool bRight = m_DEData.m_bRight;
		if (m_DETemplet.m_bSeeTarget)
		{
			if (m_DEData.m_PosX < m_TargetUnit.GetUnitSyncData().m_PosX)
			{
				m_DEData.m_bRight = true;
			}
			else
			{
				m_DEData.m_bRight = false;
			}
		}
		if (m_DETemplet.m_bSeeTargetSpeed && bRight != m_DEData.m_bRight)
		{
			m_DEData.m_SpeedX = 0f - m_DEData.m_SpeedX;
		}
	}

	protected void MakeDirVec()
	{
		NKMDamageEffectTemplet dETemplet = m_DETemplet;
		if (dETemplet != null && dETemplet.m_bUseTargetDir)
		{
			if (m_TargetUnit != null)
			{
				float num = m_TargetUnit.GetUnitSyncData().m_PosX - m_DEData.m_PosX;
				float num2 = m_TargetUnit.GetUnitSyncData().m_JumpYPos + m_TargetUnit.GetUnitTemplet().m_UnitSizeY * 0.5f - m_DEData.m_JumpYPos;
				if (m_DEData.m_TargetDirSpeed.GetNowValue() > 0f)
				{
					m_DEData.m_DirVectorTrackX.SetTracking(num, m_DEData.m_TargetDirSpeed.GetNowValue(), TRACKING_DATA_TYPE.TDT_NORMAL);
					m_DEData.m_DirVectorTrackY.SetTracking(num2, m_DEData.m_TargetDirSpeed.GetNowValue(), TRACKING_DATA_TYPE.TDT_NORMAL);
				}
				else
				{
					m_DEData.m_DirVectorTrackX.SetNowValue(num);
					m_DEData.m_DirVectorTrackY.SetNowValue(num2);
				}
				m_DEData.m_DirVectorTrackX.Update(m_fDeltaTime);
				m_DEData.m_DirVectorTrackY.Update(m_fDeltaTime);
				m_DEData.m_DirVector.x = m_DEData.m_DirVectorTrackX.GetNowValue();
				m_DEData.m_DirVector.y = m_DEData.m_DirVectorTrackY.GetNowValue();
				m_DEData.m_DirVector.Normalize();
			}
		}
		else
		{
			m_DEData.m_DirVector.x = m_DEData.m_SpeedX;
			m_DEData.m_DirVector.y = m_DEData.m_SpeedY;
			if (!m_DEData.m_fSpeedFactorX.IsNearlyZero())
			{
				m_DEData.m_DirVector.x *= m_DEData.m_fSpeedFactorX;
			}
			if (!m_DEData.m_fSpeedFactorY.IsNearlyZero())
			{
				m_DEData.m_DirVector.y *= m_DEData.m_fSpeedFactorY;
			}
			if (!m_DEData.m_fAddRotate.IsNearlyZero())
			{
				NKMMathf.RotateVector2(m_DEData.m_DirVector.x, m_DEData.m_DirVector.y, m_DEData.m_fAddRotate, out m_DEData.m_DirVector.x, out m_DEData.m_DirVector.y);
			}
			if (!m_DEData.m_bRight)
			{
				m_DEData.m_DirVector.x = 0f - m_DEData.m_DirVector.x;
			}
			m_DEData.m_fDirVectorMagniture = m_DEData.m_DirVector.magnitude;
			m_DEData.m_DirVector.Normalize();
		}
		if (m_DEData.m_DirVector.x.IsNearlyZero() && m_DEData.m_DirVector.y.IsNearlyZero() && m_DEData.m_DirVector.z.IsNearlyZero())
		{
			if (m_DEData.m_bRight)
			{
				m_DEData.m_DirVector.x = 1f;
			}
			else
			{
				m_DEData.m_DirVector.x = -1f;
			}
			m_DEData.m_DirVectorTrackX.SetNowValue(m_DEData.m_DirVector.x);
		}
	}

	protected virtual void PhysicProcess()
	{
		if (m_EffectStateNow == null || m_DETemplet == null)
		{
			return;
		}
		m_DEData.m_PosXBefore = m_DEData.m_PosX;
		m_DEData.m_PosZBefore = m_DEData.m_PosZ;
		m_DEData.m_JumpYPosBefore = m_DEData.m_JumpYPos;
		if (m_EffectStateNow.m_bNoMove || m_DETemplet.m_bNoMove)
		{
			m_DEData.m_SpeedX = 0f;
			m_DEData.m_SpeedY = 0f;
			m_DEData.m_SpeedZ = 0f;
		}
		if (m_DEData.m_bFootOnLand && m_DETemplet.m_bLandStruck)
		{
			m_DEData.m_SpeedX = 0f;
			m_DEData.m_SpeedY = 0f;
			m_DEData.m_SpeedZ = 0f;
		}
		if (GetTemplet().m_bLandConnect)
		{
			m_DEData.m_JumpYPos = 0f;
		}
		MakeDirVec();
		bool flag = false;
		if (m_TargetUnit != null && !m_TargetUnit.IsDyingOrDie() && m_DETemplet.m_bUseTargetDir && Math.Abs(m_DEData.m_fLastTargetPosX - m_DEData.m_PosX) < 40f)
		{
			flag = true;
		}
		float num = ((m_DEData.m_fEventDirSpeed != 0f) ? m_DEData.m_fEventDirSpeed : m_DEData.m_fDirVectorMagniture);
		if (!num.IsNearlyZero() && !flag)
		{
			if (m_DEData.m_fSpeedFactorX.IsNearlyZero())
			{
				float num2 = num * m_fDeltaTime;
				m_DEData.m_PosX += m_DEData.m_DirVector.x * num2;
				m_DEData.m_PosZ += m_DEData.m_DirVector.z * num2;
				m_DEData.m_JumpYPos += m_DEData.m_DirVector.y * num2;
			}
			else
			{
				float num3 = num * m_fDeltaTime * m_DEData.m_fSpeedFactorX;
				m_DEData.m_PosX += m_DEData.m_DirVector.x * num3;
				m_DEData.m_PosZ += m_DEData.m_DirVector.z * num3;
				m_DEData.m_JumpYPos += m_DEData.m_DirVector.y * num3;
			}
		}
		if (m_DETemplet.m_bUseTargetDir)
		{
			if (m_DEData.m_fSpeedFactorY.IsNearlyZero())
			{
				m_DEData.m_JumpYPos += m_DEData.m_SpeedY * m_fDeltaTime;
			}
			else
			{
				m_DEData.m_JumpYPos += m_DEData.m_SpeedY * m_DEData.m_fSpeedFactorY * m_fDeltaTime;
			}
			float num4 = m_DEData.m_fLastTargetPosX - m_DEData.m_PosXBefore;
			float num5 = m_DEData.m_fLastTargetPosX - m_DEData.m_PosX;
			if (num4 * num5 <= 0f)
			{
				m_DEData.m_PosX = m_DEData.m_fLastTargetPosX;
			}
		}
		if (m_DEData.m_SpeedX >= 0f)
		{
			m_DEData.m_SpeedX -= m_DETemplet.m_fReloadAccel * m_fDeltaTime;
			if (m_DEData.m_SpeedX <= 0f)
			{
				m_DEData.m_SpeedX = 0f;
			}
		}
		else
		{
			m_DEData.m_SpeedX += m_DETemplet.m_fReloadAccel * m_fDeltaTime;
			if (m_DEData.m_SpeedX > 0f)
			{
				m_DEData.m_SpeedX = 0f;
			}
		}
		if (m_DEData.m_SpeedZ >= 0f)
		{
			m_DEData.m_SpeedZ -= m_DETemplet.m_fReloadAccel * m_fDeltaTime;
			if (m_DEData.m_SpeedZ <= 0f)
			{
				m_DEData.m_SpeedZ = 0f;
			}
		}
		else
		{
			m_DEData.m_SpeedZ += m_DETemplet.m_fReloadAccel * m_fDeltaTime;
			if (m_DEData.m_SpeedZ > 0f)
			{
				m_DEData.m_SpeedZ = 0f;
			}
		}
		m_DEData.m_SpeedY -= m_DETemplet.m_fGAccel * m_fDeltaTime;
		if (m_DEData.m_SpeedY <= m_DETemplet.m_fMaxGSpeed)
		{
			m_DEData.m_SpeedY = m_DETemplet.m_fMaxGSpeed;
		}
		if (m_EffectStateNow.m_bNoMove || m_DETemplet.m_bNoMove)
		{
			m_EventMovePosX.StopTracking();
			m_EventMovePosZ.StopTracking();
			m_EventMovePosJumpY.StopTracking();
		}
		m_EventMovePosX.Update(m_fDeltaTime);
		m_EventMovePosZ.Update(m_fDeltaTime);
		m_EventMovePosJumpY.Update(m_fDeltaTime);
		if (m_EventMovePosX.IsTracking())
		{
			m_DEData.m_SpeedX = 0f;
			m_DEData.m_PosX = m_EventMovePosX.GetNowValue();
		}
		if (m_EventMovePosZ.IsTracking())
		{
			m_DEData.m_SpeedZ = 0f;
			m_DEData.m_PosZ = m_EventMovePosZ.GetNowValue();
		}
		if (m_EventMovePosJumpY.IsTracking())
		{
			m_DEData.m_SpeedY = 0f;
			m_DEData.m_JumpYPos = m_EventMovePosJumpY.GetNowValue();
		}
		if (GetTemplet().m_bLandConnect)
		{
			m_DEData.m_JumpYPos = 0f;
		}
	}

	protected void MapEdgeProcess()
	{
		if (m_DEData.m_JumpYPos <= 0f)
		{
			m_DEData.m_JumpYPos = 0f;
			if (m_DETemplet.m_bLandBind && m_DEData.m_SpeedY <= -10f)
			{
				m_DEData.m_SpeedY = (0f - m_DEData.m_SpeedY) * 0.5f;
			}
			else
			{
				m_DEData.m_SpeedY = 0f;
				m_DEData.m_bFootOnLand = true;
			}
		}
		else
		{
			m_DEData.m_bFootOnLand = false;
		}
		if (m_DETemplet.m_bLandEdge)
		{
			if (m_DEData.m_PosX > m_NKMGame.GetMapTemplet().m_fMaxX)
			{
				m_DEData.m_PosX = m_NKMGame.GetMapTemplet().m_fMaxX;
			}
			if (m_DEData.m_PosX < m_NKMGame.GetMapTemplet().m_fMinX)
			{
				m_DEData.m_PosX = m_NKMGame.GetMapTemplet().m_fMinX;
			}
		}
	}

	private void ProcessTarget()
	{
		if (m_DETemplet.m_FindTargetData.m_fFindTargetTime <= 0f)
		{
			return;
		}
		m_DEData.m_fFindTargetTimeNow -= m_fDeltaTime;
		if (m_DEData.m_fFindTargetTimeNow > 0f)
		{
			return;
		}
		m_DEData.m_fFindTargetTimeNow = m_DETemplet.m_FindTargetData.m_fFindTargetTime;
		if (m_DEData.m_TargetGameUnitUID == 0 || !m_DETemplet.m_FindTargetData.m_bTargetNoChange)
		{
			m_TargetUnit = m_NKMGame.FindTarget(m_DEData.m_MasterUnit, GetSortUnitListByNearDist(m_DETemplet.m_FindTargetData.m_bUseUnitSize), m_DETemplet.m_FindTargetData, m_DEData.m_NKM_TEAM_TYPE, m_DEData.m_PosX, 0f, m_DEData.m_bRight);
			if (m_TargetUnit != null)
			{
				m_DEData.m_TargetGameUnitUID = m_TargetUnit.GetUnitDataGame().m_GameUnitUID;
			}
		}
	}

	public bool CheckEventCondition(NKMEventCondition cCondition)
	{
		if (cCondition.m_EventConditionV2 != null)
		{
			return cCondition.m_EventConditionV2.CheckEventCondition(m_NKMGame, m_DEData.m_MasterUnit, null);
		}
		if (m_DEData.m_MasterUnit == null)
		{
			return false;
		}
		if (m_NKMGame.IsPVP(bUseDevOption: true) && !cCondition.m_bUsePVP)
		{
			return false;
		}
		if (m_NKMGame.IsPVE(bUseDevOption: true) && !cCondition.m_bUsePVE)
		{
			return false;
		}
		if (m_DEData.m_MasterUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
		{
			cCondition.CheckSkillID();
		}
		if (!cCondition.CanUsePhase(m_MasterUnitPhase))
		{
			return false;
		}
		if (!cCondition.CanUseBuff(m_DEData.m_MasterUnit.GetUnitSyncData().m_dicBuffData))
		{
			return false;
		}
		if (!cCondition.CanUseBuffCount(m_DEData.m_MasterUnit))
		{
			return false;
		}
		if (!cCondition.CanUseStatus(m_DEData.m_MasterUnit))
		{
			return false;
		}
		if (cCondition.m_bLeaderUnit)
		{
			NKMGameTeamData teamData = m_DEData.m_MasterUnit.GetTeamData();
			if (teamData == null)
			{
				return false;
			}
			if (teamData.GetLeaderUnitData() == null)
			{
				return false;
			}
			if (m_DEData.m_MasterUnit.IsSummonUnit() || m_DEData.m_MasterUnit.HasMasterUnit())
			{
				NKMUnit masterUnit = m_DEData.m_MasterUnit.GetMasterUnit();
				if (masterUnit == null)
				{
					return false;
				}
				if (teamData.GetLeaderUnitData().m_UnitUID != masterUnit.GetUnitData().m_UnitUID)
				{
					return false;
				}
			}
			else if (teamData.GetLeaderUnitData().m_UnitUID != m_DEData.m_MasterUnit.GetUnitData().m_UnitUID)
			{
				return false;
			}
		}
		if (m_DEData.m_MasterUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
		{
			if (cCondition.m_SkillID != -1)
			{
				int unitSkillLevel = m_DEData.m_MasterUnit.GetUnitData().GetUnitSkillLevel(cCondition.m_SkillID);
				if (!cCondition.CanUseSkill(unitSkillLevel))
				{
					return false;
				}
			}
			if (cCondition.m_MasterSkillID != -1)
			{
				int masterSkillLevel = -1;
				NKMUnit masterUnit2 = m_DEData.m_MasterUnit.GetMasterUnit();
				if (masterUnit2 != null)
				{
					masterSkillLevel = masterUnit2.GetUnitData().GetUnitSkillLevel(cCondition.m_MasterSkillID);
				}
				if (!cCondition.CanUseMasterSkill(masterSkillLevel))
				{
					return false;
				}
			}
		}
		if (!cCondition.CheckHPRate(m_DEData.m_MasterUnit))
		{
			return false;
		}
		if (!cCondition.CanUseMapPosition(m_NKMGame.GetMapTemplet().GetMapFactor(m_DEData.m_MasterUnit.GetUnitSyncData().m_PosX, m_NKMGame.IsATeam(m_DEData.m_MasterUnit.GetUnitDataGame().m_NKM_TEAM_TYPE))))
		{
			return false;
		}
		if (!cCondition.CanUseLevelRange(m_DEData.m_MasterUnit))
		{
			return false;
		}
		if (!cCondition.CanUseUnitExist(m_NKMGame, m_DEData.m_NKM_TEAM_TYPE))
		{
			return false;
		}
		if (!cCondition.CanUseReactorSkill(m_DEData.m_MasterUnit))
		{
			return false;
		}
		if (!cCondition.CheckEventVariable(m_DEData.m_MasterUnit))
		{
			return false;
		}
		return true;
	}

	protected void ProcessEventTargetDirSpeed()
	{
		if (m_EffectStateNow == null || m_EffectStateNow.m_bNoMove || m_DETemplet.m_bNoMove)
		{
			return;
		}
		m_DEData.m_TargetDirSpeed.Update(m_fDeltaTime);
		for (int i = 0; i < m_EffectStateNow.m_listNKMEventTargetDirSpeed.Count; i++)
		{
			NKMEventTargetDirSpeed nKMEventTargetDirSpeed = m_EffectStateNow.m_listNKMEventTargetDirSpeed[i];
			if (nKMEventTargetDirSpeed != null)
			{
				bool flag = false;
				if (EventTimer(nKMEventTargetDirSpeed.m_bAnimTime, nKMEventTargetDirSpeed.m_fEventTime, bOneTime: true))
				{
					flag = true;
				}
				if (flag)
				{
					m_DEData.m_TargetDirSpeed.SetTracking(nKMEventTargetDirSpeed.m_fTargetDirSpeed, nKMEventTargetDirSpeed.m_fChangeTime, TRACKING_DATA_TYPE.TDT_NORMAL);
				}
			}
		}
	}

	protected void ProcessEventDirSpeed(bool bStateEnd = false)
	{
		if (m_EffectStateNow == null || m_EffectStateNow.m_bNoMove || m_DETemplet.m_bNoMove)
		{
			return;
		}
		for (int i = 0; i < m_EffectStateNow.m_listNKMEventDirSpeed.Count; i++)
		{
			NKMEventDirSpeed nKMEventDirSpeed = m_EffectStateNow.m_listNKMEventDirSpeed[i];
			if (nKMEventDirSpeed != null && CheckEventCondition(nKMEventDirSpeed.m_Condition))
			{
				bool flag = false;
				if (nKMEventDirSpeed.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventDirSpeed.m_bAnimTime, nKMEventDirSpeed.m_fEventTimeMin, nKMEventDirSpeed.m_fEventTimeMax) && !nKMEventDirSpeed.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag)
				{
					float num = 0f;
					num = ((!nKMEventDirSpeed.m_bAnimTime) ? nKMEventDirSpeed.GetSpeed(m_DEData.m_fStateTime, m_DEData.m_fEventDirSpeed) : nKMEventDirSpeed.GetSpeed(m_DEData.m_fAnimTime, m_DEData.m_fEventDirSpeed));
					m_DEData.m_fEventDirSpeed = num;
				}
			}
		}
	}

	protected void ProcessEventSpeed(bool bStateEnd = false)
	{
		if (m_EffectStateNow == null || m_EffectStateNow.m_bNoMove || m_DETemplet.m_bNoMove)
		{
			return;
		}
		for (int i = 0; i < m_EffectStateNow.m_listNKMEventSpeed.Count; i++)
		{
			NKMEventSpeed nKMEventSpeed = m_EffectStateNow.m_listNKMEventSpeed[i];
			if (nKMEventSpeed == null || !CheckEventCondition(nKMEventSpeed.m_Condition))
			{
				continue;
			}
			bool flag = false;
			if (nKMEventSpeed.m_bStateEndTime && bStateEnd)
			{
				flag = true;
			}
			else if (EventTimer(nKMEventSpeed.m_bAnimTime, nKMEventSpeed.m_fEventTimeMin, nKMEventSpeed.m_fEventTimeMax) && !nKMEventSpeed.m_bStateEndTime)
			{
				flag = true;
			}
			if (!flag)
			{
				continue;
			}
			if (!nKMEventSpeed.m_SpeedX.IsNearlyEqual(-1f))
			{
				float num = 0f;
				num = ((!nKMEventSpeed.m_bAnimTime) ? nKMEventSpeed.GetSpeedX(m_DEData.m_fStateTime, m_DEData.m_SpeedX) : nKMEventSpeed.GetSpeedX(m_DEData.m_fAnimTime, m_DEData.m_SpeedX));
				if (nKMEventSpeed.m_bAdd)
				{
					m_DEData.m_SpeedX += num;
				}
				else if (nKMEventSpeed.m_bMultiply)
				{
					m_DEData.m_SpeedX *= num;
				}
				else
				{
					m_DEData.m_SpeedX = num;
				}
			}
			if (!nKMEventSpeed.m_SpeedY.IsNearlyEqual(-1f))
			{
				float num2 = 0f;
				num2 = ((!nKMEventSpeed.m_bAnimTime) ? nKMEventSpeed.GetSpeedY(m_DEData.m_fStateTime, m_DEData.m_SpeedY) : nKMEventSpeed.GetSpeedY(m_DEData.m_fAnimTime, m_DEData.m_SpeedY));
				if (nKMEventSpeed.m_bAdd)
				{
					m_DEData.m_SpeedY += num2;
				}
				else if (nKMEventSpeed.m_bMultiply)
				{
					m_DEData.m_SpeedY *= num2;
				}
				else
				{
					m_DEData.m_SpeedY = num2;
				}
			}
		}
	}

	protected void ProcessEventSpeedX(bool bStateEnd = false)
	{
		if (m_EffectStateNow == null || m_EffectStateNow.m_bNoMove || m_DETemplet.m_bNoMove)
		{
			return;
		}
		for (int i = 0; i < m_EffectStateNow.m_listNKMEventSpeedX.Count; i++)
		{
			NKMEventSpeedX nKMEventSpeedX = m_EffectStateNow.m_listNKMEventSpeedX[i];
			if (nKMEventSpeedX == null || !CheckEventCondition(nKMEventSpeedX.m_Condition))
			{
				continue;
			}
			bool flag = false;
			if (nKMEventSpeedX.m_bStateEndTime && bStateEnd)
			{
				flag = true;
			}
			else if (EventTimer(nKMEventSpeedX.m_bAnimTime, nKMEventSpeedX.m_fEventTimeMin, nKMEventSpeedX.m_fEventTimeMax) && !nKMEventSpeedX.m_bStateEndTime)
			{
				flag = true;
			}
			if (flag)
			{
				float num = 0f;
				num = ((!nKMEventSpeedX.m_bAnimTime) ? nKMEventSpeedX.GetSpeed(m_DEData.m_fStateTime, m_DEData.m_SpeedX) : nKMEventSpeedX.GetSpeed(m_DEData.m_fAnimTime, m_DEData.m_SpeedX));
				if (nKMEventSpeedX.m_bAdd)
				{
					m_DEData.m_SpeedX += num;
				}
				else if (nKMEventSpeedX.m_bMultiply)
				{
					m_DEData.m_SpeedX *= num;
				}
				else
				{
					m_DEData.m_SpeedX = num;
				}
			}
		}
	}

	protected void ProcessEventSpeedY(bool bStateEnd = false)
	{
		if (m_EffectStateNow == null || m_EffectStateNow.m_bNoMove || m_DETemplet.m_bNoMove)
		{
			return;
		}
		for (int i = 0; i < m_EffectStateNow.m_listNKMEventSpeedY.Count; i++)
		{
			NKMEventSpeedY nKMEventSpeedY = m_EffectStateNow.m_listNKMEventSpeedY[i];
			if (nKMEventSpeedY == null || !CheckEventCondition(nKMEventSpeedY.m_Condition))
			{
				continue;
			}
			bool flag = false;
			if (nKMEventSpeedY.m_bStateEndTime && bStateEnd)
			{
				flag = true;
			}
			else if (EventTimer(nKMEventSpeedY.m_bAnimTime, nKMEventSpeedY.m_fEventTimeMin, nKMEventSpeedY.m_fEventTimeMax) && !nKMEventSpeedY.m_bStateEndTime)
			{
				flag = true;
			}
			if (flag)
			{
				float num = 0f;
				num = ((!nKMEventSpeedY.m_bAnimTime) ? nKMEventSpeedY.GetSpeed(m_DEData.m_fStateTime, m_DEData.m_SpeedY) : nKMEventSpeedY.GetSpeed(m_DEData.m_fAnimTime, m_DEData.m_SpeedY));
				if (nKMEventSpeedY.m_bAdd)
				{
					m_DEData.m_SpeedY += num;
				}
				else if (nKMEventSpeedY.m_bMultiply)
				{
					m_DEData.m_SpeedY *= num;
				}
				else
				{
					m_DEData.m_SpeedY = num;
				}
			}
		}
	}

	protected void ProcessEventMove(bool bStateEnd = false)
	{
		if (m_EffectStateNow != null && !m_EffectStateNow.m_bNoMove && !m_DETemplet.m_bNoMove)
		{
			for (int i = 0; i < m_EffectStateNow.m_listNKMEventMove.Count; i++)
			{
				NKMEventMove cNKMEventMove = m_EffectStateNow.m_listNKMEventMove[i];
				ProcessEventMove(cNKMEventMove, bUseEventTimer: true, bStateEnd);
			}
		}
	}

	public void ProcessEventMove(NKMEventMove cNKMEventMove, bool bUseEventTimer, bool bStateEnd = false)
	{
		if (cNKMEventMove != null && CheckEventCondition(cNKMEventMove.m_Condition))
		{
			bool flag = false;
			if (cNKMEventMove.m_bStateEndTime && bStateEnd)
			{
				flag = true;
			}
			else if (EventTimer(cNKMEventMove.m_bAnimTime, cNKMEventMove.m_fEventTime, bOneTime: true) && !cNKMEventMove.m_bStateEndTime)
			{
				flag = true;
			}
			if (flag)
			{
				ApplyEventMove(cNKMEventMove);
			}
		}
	}

	public void ApplyEventMove(NKMEventMove cNKMEventMove)
	{
		float num = GetEventPosX(cNKMEventMove.m_EventPosData, IsATeam());
		float eventMovePosY = GetEventMovePosY(cNKMEventMove.m_EventPosData);
		if (cNKMEventMove.m_bSavePosition || cNKMEventMove.m_EventPosData.m_MoveBase == NKMEventPosData.MoveBase.SAVE_ONLY)
		{
			SaveEventMovePosition(num, eventMovePosY, cNKMEventMove.m_bLandMove);
			return;
		}
		if (cNKMEventMove.m_fMaxDistance >= 0f)
		{
			float value = num - GetMasterUnit().GetUnitSyncData().m_PosX;
			if (Math.Abs(value) > cNKMEventMove.m_fMaxDistance)
			{
				num = GetMasterUnit().GetUnitSyncData().m_PosX + (float)Math.Sign(value) * cNKMEventMove.m_fMaxDistance;
			}
		}
		if (cNKMEventMove.m_fSpeed > 0f)
		{
			float fTime = Math.Abs(m_DEData.m_PosX - num) / cNKMEventMove.m_fSpeed;
			m_EventMovePosX.SetNowValue(m_DEData.m_PosX);
			m_EventMovePosX.SetTracking(num, fTime, cNKMEventMove.m_MoveTrackingType);
			if (!cNKMEventMove.m_bLandMove)
			{
				m_EventMovePosJumpY.SetNowValue(m_DEData.m_JumpYPos);
				m_EventMovePosJumpY.SetTracking(eventMovePosY, fTime, cNKMEventMove.m_MoveTrackingType);
			}
		}
		else if (cNKMEventMove.m_MoveTime > 0f)
		{
			m_EventMovePosX.SetNowValue(m_DEData.m_PosX);
			m_EventMovePosX.SetTracking(num, cNKMEventMove.m_MoveTime, cNKMEventMove.m_MoveTrackingType);
			if (!cNKMEventMove.m_bLandMove)
			{
				m_EventMovePosJumpY.SetNowValue(m_DEData.m_JumpYPos);
				m_EventMovePosJumpY.SetTracking(eventMovePosY, cNKMEventMove.m_MoveTime, cNKMEventMove.m_MoveTrackingType);
			}
		}
		else
		{
			m_DEData.m_PosX = num;
			if (!cNKMEventMove.m_bLandMove)
			{
				m_DEData.m_JumpYPos = eventMovePosY;
			}
		}
	}

	public float GetEventPosX(NKMEventPosData posData, bool isATeam, params (NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] extraParams)
	{
		if (posData.m_fDefaultOffsetX != 0f && UseDefaultPosRequired(posData.m_MoveBase))
		{
			if (m_DEData.m_bRight)
			{
				return GetMasterUnit().GetUnitSyncData().m_PosX + posData.m_fDefaultOffsetX;
			}
			return GetMasterUnit().GetUnitSyncData().m_PosX - posData.m_fDefaultOffsetX;
		}
		float basePosX = GetBasePosX(posData.m_MoveBase, posData.m_MoveBaseType, isATeam, posData.m_fMapPosFactor, extraParams);
		if (GetOffsetDirRight(posData.m_MoveOffset, basePosX, isATeam, posData.m_fMapPosFactor, extraParams))
		{
			return basePosX + posData.m_fOffsetX;
		}
		return basePosX - posData.m_fOffsetX;
	}

	public float GetEventPosX(float basePos, NKMEventPosData.MoveOffset offsetType, float offsetX, bool isATeam, float mapPosFactor)
	{
		if (GetOffsetDirRight(offsetType, basePos, isATeam, mapPosFactor))
		{
			return basePos + offsetX;
		}
		return basePos - offsetX;
	}

	public virtual float GetBasePosX(NKMEventPosData.MoveBase moveBase, NKMEventPosData.MoveBaseType moveBaseType, bool isATeam, float mapPosFactor, params (NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] extraParams)
	{
		switch (moveBase)
		{
		case NKMEventPosData.MoveBase.ME:
		case NKMEventPosData.MoveBase.SAVE_ONLY:
			return m_DEData.m_PosX;
		case NKMEventPosData.MoveBase.TARGET_UNIT:
			return GetEventUnitPos(moveBaseType, m_TargetUnit, m_DEData.m_fLastTargetPosX);
		case NKMEventPosData.MoveBase.MASTER_UNIT:
			return GetEventUnitPos(moveBaseType, GetMasterUnit(), GetMasterUnit().GetUnitSyncData().m_PosX);
		case NKMEventPosData.MoveBase.SAVED_POS:
			return m_fEventMoveSavedPositionX;
		case NKMEventPosData.MoveBase.TRIGGER_TARGET_UNIT:
			Log.Error("DE에서 TRIGGER_TARGET_UNIT 사용 불가", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffect.cs", 1781);
			return m_DEData.m_PosX;
		case NKMEventPosData.MoveBase.MAP_RATE:
			if (m_DEData.m_bRight)
			{
				return m_NKMGame.GetMapTemplet().GetMapRatePos(mapPosFactor, bTeamA: true);
			}
			return m_NKMGame.GetMapTemplet().GetMapRatePos(mapPosFactor, bTeamA: false);
		default:
			return GetMasterUnit().GetBasePosX(moveBase, moveBaseType, isATeam, mapPosFactor, extraParams);
		}
	}

	public float GetEventUnitPos(NKMEventPosData.MoveBaseType type, NKMUnit cTargetUnit, float defaultPosX)
	{
		if (cTargetUnit == null)
		{
			return defaultPosX;
		}
		return NKMUnit.GetEventUnitPos(type, m_DEData.m_PosX, cTargetUnit.GetUnitSyncData().m_PosX, cTargetUnit.GetUnitTemplet().m_UnitSizeX, cTargetUnit.GetUnitSyncData().m_bRight);
	}

	public virtual bool GetOffsetDirRight(NKMEventPosData.MoveOffset offsetType, float basePos, bool isATeam, float mapPosFactor, params (NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] extraParams)
	{
		switch (offsetType)
		{
		case NKMEventPosData.MoveOffset.ME:
			return m_DEData.m_PosX > basePos;
		case NKMEventPosData.MoveOffset.ME_INV:
			return m_DEData.m_PosX <= basePos;
		case NKMEventPosData.MoveOffset.MY_LOOK_DIR:
			return m_DEData.m_bRight;
		case NKMEventPosData.MoveOffset.TARGET_UNIT:
			return basePos < m_DEData.m_fLastTargetPosX;
		case NKMEventPosData.MoveOffset.TARGET_UNIT_INV:
			return basePos >= m_DEData.m_fLastTargetPosX;
		case NKMEventPosData.MoveOffset.MASTER_UNIT:
			return GetMasterUnit().GetUnitSyncData().m_PosX > basePos;
		case NKMEventPosData.MoveOffset.SAVED_POS:
			return m_fEventMoveSavedPositionX > basePos;
		case NKMEventPosData.MoveOffset.TRIGGER_TARGET_UNIT:
		case NKMEventPosData.MoveOffset.TRIGGER_TARGET_UNIT_INV:
			Log.Error("DE에서 TRIGGER_TARGET 사용 불가", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffect.cs", 1837);
			return m_DEData.m_PosX > basePos;
		default:
			return GetMasterUnit().GetOffsetDirRight(offsetType, basePos, isATeam, mapPosFactor, extraParams);
		}
	}

	public float GetBasePosY(NKMEventPosData.MoveBase baseType)
	{
		return baseType switch
		{
			NKMEventPosData.MoveBase.TARGET_UNIT => m_DEData.m_fLastTargetPosJumpY, 
			NKMEventPosData.MoveBase.MASTER_UNIT => GetMasterUnit().GetUnitSyncData().m_JumpYPos, 
			NKMEventPosData.MoveBase.ME => m_DEData.m_JumpYPos, 
			NKMEventPosData.MoveBase.SAVED_POS => m_fEventMoveSavedPositionY, 
			_ => GetMasterUnit().GetBasePosY(baseType), 
		};
	}

	public float GetEventMovePosY(NKMEventPosData.MoveBase baseType, float offset)
	{
		return GetBasePosY(baseType) + offset;
	}

	public float GetEventMovePosY(NKMEventPosData eventPosData)
	{
		return GetBasePosY(eventPosData.m_MoveBase) + eventPosData.m_fOffsetY;
	}

	public float GetBasePosZ(NKMEventPosData.MoveBase baseType)
	{
		return baseType switch
		{
			NKMEventPosData.MoveBase.TARGET_UNIT => m_DEData.m_fLastTargetPosZ, 
			NKMEventPosData.MoveBase.MASTER_UNIT => GetMasterUnit().GetUnitSyncData().m_PosZ, 
			NKMEventPosData.MoveBase.ME => m_DEData.m_PosZ, 
			_ => GetMasterUnit().GetBasePosZ(baseType), 
		};
	}

	public bool UseDefaultPosRequired(NKMEventPosData.MoveBase moveBase)
	{
		if (moveBase == NKMEventPosData.MoveBase.TARGET_UNIT)
		{
			return m_TargetUnit == null;
		}
		return GetMasterUnit().UseDefaultPosRequired(moveBase);
	}

	private void SaveEventMovePosition(float posX, float posY, bool bSaveXOnly)
	{
		m_fEventMoveSavedPositionX = posX;
		if (!bSaveXOnly)
		{
			m_fEventMoveSavedPositionY = posY;
		}
	}

	public bool IsATeam()
	{
		return m_NKMGame.IsATeam(GetDEData().m_NKM_TEAM_TYPE);
	}

	protected virtual void ProcessEventAttack()
	{
		if (m_EffectStateNow == null || m_DEData.m_DamageCountNow >= m_DETemplet.m_DamageCountMax)
		{
			return;
		}
		for (int i = 0; i < m_EffectStateNow.m_listNKMEventAttack.Count; i++)
		{
			NKMEventAttack nKMEventAttack = m_EffectStateNow.m_listNKMEventAttack[i];
			if (nKMEventAttack == null || !CheckEventCondition(nKMEventAttack.m_Condition))
			{
				continue;
			}
			bool flag = false;
			if (EventTimer(nKMEventAttack.m_bAnimTime, nKMEventAttack.m_fEventTimeMin, bOneTime: true))
			{
				flag = true;
			}
			if (flag)
			{
				GetDamageInstAtk(i)?.Init();
			}
			flag = false;
			if (EventTimer(nKMEventAttack.m_bAnimTime, nKMEventAttack.m_fEventTimeMin, nKMEventAttack.m_fEventTimeMax))
			{
				flag = true;
			}
			if (nKMEventAttack.m_fEventTimeMin.IsNearlyEqual(nKMEventAttack.m_fEventTimeMax) && EventTimer(nKMEventAttack.m_bAnimTime, nKMEventAttack.m_fEventTimeMin, bOneTime: true))
			{
				flag = true;
			}
			if (flag)
			{
				NKMDamageInst damageInstAtk = GetDamageInstAtk(i);
				if (damageInstAtk.m_Templet == null)
				{
					damageInstAtk.m_Templet = NKMDamageManager.GetTempletByStrID(nKMEventAttack.m_DamageTempletName);
					damageInstAtk.m_AttackerType = NKM_REACTOR_TYPE.NRT_DAMAGE_EFFECT;
					damageInstAtk.m_AttackerEffectUID = m_DamageEffectUID;
					damageInstAtk.m_AttackerGameUnitUID = GetDEData().m_MasterGameUnitUID;
					damageInstAtk.m_AttackerUnitSkillTemplet = m_UnitSkillTemplet;
					damageInstAtk.m_AttackerTeamType = GetDEData().m_NKM_TEAM_TYPE;
				}
				if (m_NKMGame.DamageCheck(damageInstAtk, nKMEventAttack) && nKMEventAttack.m_EffectName.Length > 1)
				{
					ProcessAttackHitEffect(nKMEventAttack);
				}
			}
		}
	}

	protected virtual void ProcessEventSound(bool bStateEnd = false)
	{
	}

	public virtual void ApplyEventSound(NKMEventSound cNKMEventSound)
	{
	}

	protected virtual void ProcessEventCameraCrash(bool bStateEnd = false)
	{
	}

	protected virtual void ProcessEventEffect(bool bStateEnd = false)
	{
	}

	public virtual void ApplyEventEffect(NKMEventEffect cNKMEventEffect)
	{
	}

	protected void ProcessEventDamageEffect(bool bStateEnd = false)
	{
		if (m_EffectStateNow == null)
		{
			return;
		}
		if (bStateEnd)
		{
			for (LinkedListNode<NKMDamageEffect> linkedListNode = m_linklistDamageEffect.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				NKMDamageEffect value = linkedListNode.Value;
				if (value.GetStateEndDie())
				{
					value.SetDie();
				}
			}
		}
		else
		{
			LinkedListNode<NKMDamageEffect> linkedListNode2 = m_linklistDamageEffect.First;
			while (linkedListNode2 != null)
			{
				NKMDamageEffect value2 = linkedListNode2.Value;
				if (!m_NKMGame.GetDEManager().IsLiveEffect(value2.GetDEUID()))
				{
					LinkedListNode<NKMDamageEffect> next = linkedListNode2.Next;
					m_linklistDamageEffect.Remove(linkedListNode2);
					linkedListNode2 = next;
				}
				else
				{
					value2.SetRight(m_DEData.m_bRight);
					value2.SetFollowPos(m_DEData.m_PosX, m_DEData.m_JumpYPos, m_DEData.m_PosZ);
					linkedListNode2 = linkedListNode2.Next;
				}
			}
		}
		for (int i = 0; i < m_EffectStateNow.m_listNKMEventDamageEffect.Count; i++)
		{
			NKMEventDamageEffect nKMEventDamageEffect = m_EffectStateNow.m_listNKMEventDamageEffect[i];
			if (nKMEventDamageEffect != null && CheckEventCondition(nKMEventDamageEffect.m_Condition))
			{
				bool flag = false;
				if (nKMEventDamageEffect.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventDamageEffect.m_bAnimTime, nKMEventDamageEffect.m_fEventTime, bOneTime: true) && !nKMEventDamageEffect.m_bStateEndTime)
				{
					flag = true;
				}
				if (nKMEventDamageEffect.m_bIgnoreNoTarget && m_TargetUnit == null)
				{
					flag = false;
				}
				if (flag)
				{
					ApplyEventDamageEffect(nKMEventDamageEffect);
				}
			}
		}
	}

	public void ApplyEventDamageEffect(NKMEventDamageEffect cNKMEventDamageEffect)
	{
		m_NKMVector3Temp1.x = GetBasePosX(cNKMEventDamageEffect.m_EventPosData.m_MoveBase, cNKMEventDamageEffect.m_EventPosData.m_MoveBaseType, IsATeam(), cNKMEventDamageEffect.m_EventPosData.m_fMapPosFactor);
		m_NKMVector3Temp1.y = GetBasePosY(cNKMEventDamageEffect.m_EventPosData.m_MoveBase);
		m_NKMVector3Temp1.z = (cNKMEventDamageEffect.m_bUseMyZPos ? m_DEData.m_PosZ : GetBasePosZ(cNKMEventDamageEffect.m_EventPosData.m_MoveBase));
		float zScaleFactor = m_NKMGame.GetZScaleFactor(m_DEData.m_PosZ);
		short targetGameUID = m_DEData.m_TargetGameUnitUID;
		if (cNKMEventDamageEffect.m_bIgnoreTarget)
		{
			targetGameUID = 0;
		}
		string templetID = cNKMEventDamageEffect.m_DEName;
		if (m_NKMGame.IsPVP(bUseDevOption: true) && cNKMEventDamageEffect.m_DENamePVP.Length > 1)
		{
			templetID = cNKMEventDamageEffect.m_DENamePVP;
		}
		bool bRight = (cNKMEventDamageEffect.m_bFlipRight ? (!m_DEData.m_bRight) : m_DEData.m_bRight);
		NKMUnitSkillTemplet cUnitSkillTemplet = ((cNKMEventDamageEffect.m_SkillType == NKM_SKILL_TYPE.NST_INVALID) ? m_UnitSkillTemplet : GetMasterUnit().GetUnitData().GetUnitSkillTempletByType(cNKMEventDamageEffect.m_SkillType));
		NKMDamageEffect nKMDamageEffect = m_NKMGame.GetDEManager().UseDamageEffect(templetID, m_DEData.m_MasterGameUnitUID, targetGameUID, cUnitSkillTemplet, m_MasterUnitPhase, m_NKMVector3Temp1.x, m_NKMVector3Temp1.y, m_NKMVector3Temp1.z, cNKMEventDamageEffect.m_EventPosData.m_MoveOffset, cNKMEventDamageEffect.m_EventPosData.m_fMapPosFactor, bRight, cNKMEventDamageEffect.m_EventPosData.m_fOffsetX * zScaleFactor, cNKMEventDamageEffect.m_EventPosData.m_fOffsetY * zScaleFactor, cNKMEventDamageEffect.m_EventPosData.m_fOffsetZ, cNKMEventDamageEffect.m_fAddRotate, cNKMEventDamageEffect.m_bUseZScale, cNKMEventDamageEffect.m_fSpeedFactorX, cNKMEventDamageEffect.m_fSpeedFactorY, cNKMEventDamageEffect.m_fReserveTime, bNextFrame: true);
		if (nKMDamageEffect != null && (cNKMEventDamageEffect.m_bHold || cNKMEventDamageEffect.m_bStateEndStop))
		{
			nKMDamageEffect.SetHoldFollowData(cNKMEventDamageEffect.m_FollowType, cNKMEventDamageEffect.m_FollowTime, cNKMEventDamageEffect.m_FollowUpdateTime);
			nKMDamageEffect.SetStateEndDie(cNKMEventDamageEffect.m_bStateEndStop);
			m_linklistDamageEffect.AddLast(nKMDamageEffect);
		}
	}

	protected virtual void ProcessEventDissolve(bool bStateEnd = false)
	{
	}

	public virtual void ApplyEventDissolve(NKMEventDissolve cNKMEventDissolve)
	{
	}

	protected virtual void ProcessEventBuff(bool bStateEnd = false)
	{
		if (m_EffectStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_EffectStateNow.m_listNKMEventBuff.Count; i++)
		{
			NKMEventBuff nKMEventBuff = m_EffectStateNow.m_listNKMEventBuff[i];
			if (nKMEventBuff != null && !nKMEventBuff.m_bReflection && CheckEventCondition(nKMEventBuff.m_Condition))
			{
				bool flag = false;
				if (nKMEventBuff.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventBuff.m_bAnimTime, nKMEventBuff.m_fEventTime, bOneTime: true) && !nKMEventBuff.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag)
				{
					nKMEventBuff.ApplyEvent(m_NKMGame, this);
				}
			}
		}
	}

	protected virtual void ProcessEventStatus(bool bStateEnd = false)
	{
		if (m_EffectStateNow != null)
		{
			for (int i = 0; i < m_EffectStateNow.m_listNKMEventStatus.Count; i++)
			{
				m_EffectStateNow.m_listNKMEventStatus[i]?.ProcessEvent(m_NKMGame, this, bStateEnd);
			}
		}
	}

	protected virtual void ProcessEventHeal(bool bStateEnd = false)
	{
		if (GetMasterUnit().Get_NKM_UNIT_CLASS_TYPE() != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER || m_EffectStateNow == null || GetMasterUnit() == null)
		{
			return;
		}
		for (int i = 0; i < m_EffectStateNow.m_listNKMEventHeal.Count; i++)
		{
			NKMEventHeal nKMEventHeal = m_EffectStateNow.m_listNKMEventHeal[i];
			if (nKMEventHeal != null && CheckEventCondition(nKMEventHeal.m_Condition) && (nKMEventHeal.m_bStateEndTime ? bStateEnd : EventTimer(nKMEventHeal.m_bAnimTime, nKMEventHeal.m_fEventTime, bOneTime: true)))
			{
				GetMasterUnit().SetEventHeal(nKMEventHeal, GetDEData().m_PosX, null);
			}
		}
	}

	public bool EventTimer(bool bAnim, float fTime, bool bOneTime)
	{
		if (bAnim)
		{
			return EventTimer(fTime, bOneTime, m_DEData.m_fAnimTimeBack, m_DEData.m_fAnimTime, m_EventTimeStampAnim);
		}
		return EventTimer(fTime, bOneTime, m_DEData.m_fStateTimeBack, m_DEData.m_fStateTime, m_EventTimeStampState);
	}

	private bool EventTimer(float fTimeTarget, bool bOneTime, float fTimeBack, float fTimeNow, Dictionary<float, NKMTimeStamp> dicTimeStamp)
	{
		if ((fTimeTarget > fTimeBack && fTimeTarget <= fTimeNow) || (fTimeTarget.IsNearlyZero() && m_DEData.m_bStateFirstFrame))
		{
			if (!bOneTime)
			{
				return true;
			}
			if (!dicTimeStamp.ContainsKey(fTimeTarget))
			{
				NKMTimeStamp nKMTimeStamp = (NKMTimeStamp)m_NKMGame.GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMTimeStamp);
				nKMTimeStamp.m_FramePass = false;
				dicTimeStamp.Add(fTimeTarget, nKMTimeStamp);
				return true;
			}
			if (!dicTimeStamp[fTimeTarget].m_FramePass)
			{
				return true;
			}
		}
		return false;
	}

	public bool EventTimer(bool bAnim, float fTimeMin, float fTimeMax)
	{
		bool flag = false;
		if (bAnim)
		{
			if (m_DEData.m_fAnimTime >= fTimeMin && m_DEData.m_fAnimTime <= fTimeMax)
			{
				flag = true;
			}
		}
		else if (m_DEData.m_fStateTime >= fTimeMin && m_DEData.m_fStateTime <= fTimeMax)
		{
			flag = true;
		}
		if (!flag && EventTimer(bAnim, fTimeMin, bOneTime: true))
		{
			flag = true;
		}
		return flag;
	}

	public NKMUnit GetTargetUnit()
	{
		if (m_DEData.m_TargetGameUnitUID > 0)
		{
			NKMUnit unit = m_NKMGame.GetUnit(m_DEData.m_TargetGameUnitUID);
			if (unit != null)
			{
				m_DEData.m_fLastTargetPosX = unit.GetUnitSyncData().m_PosX;
				m_DEData.m_fLastTargetPosZ = unit.GetUnitSyncData().m_PosZ;
				m_DEData.m_fLastTargetPosJumpY = unit.GetUnitSyncData().m_JumpYPos;
				return unit;
			}
		}
		return null;
	}

	public float GetDist(NKMUnit unit)
	{
		return Math.Abs(m_DEData.m_PosX - unit.GetUnitSyncData().m_PosX);
	}

	public short GetDEUID()
	{
		return m_DamageEffectUID;
	}

	public NKMDamageEffectData GetDEData()
	{
		return m_DEData;
	}

	public NKMUnit GetMasterUnit()
	{
		if (m_DEData.m_MasterUnit == null)
		{
			return m_NKMGame.GetUnit(m_DEData.m_MasterGameUnitUID, bChain: true, bPool: true);
		}
		return m_DEData.m_MasterUnit;
	}

	public NKMDamageEffectTemplet GetTemplet()
	{
		return m_DETemplet;
	}

	public virtual void AttackResult(NKMDamageInst cNKMDamageInst, NKMUnit pDefender)
	{
		if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_NO)
		{
			return;
		}
		if (m_NKM_DAMAGE_EFFECT_CLASS_TYPE == NKM_DAMAGE_EFFECT_CLASS_TYPE.NDECT_NKM && m_DEData.m_MasterUnit != null)
		{
			if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE)
			{
				if (!m_DEData.m_MasterUnit.GetUnitTemplet().m_bNoDamageState)
				{
					if (!m_DEData.m_MasterUnit.IsAirUnit())
					{
						if (m_DEData.m_MasterUnit.GetUnitFrameData().m_bFootOnLand)
						{
							m_DEData.m_MasterUnit.StateChange("USN_DAMAGE_A", bForceChange: true, bImmediate: false, NKM_STATE_CHANGE_PRIORITY.NSCP_SKILL_CRASH);
						}
						else if (!m_DEData.m_MasterUnit.GetUnitTemplet().m_bNoDamageDownState)
						{
							m_DEData.m_MasterUnit.StateChange("USN_DAMAGE_AIR_DOWN", bForceChange: true, bImmediate: false, NKM_STATE_CHANGE_PRIORITY.NSCP_SKILL_CRASH);
						}
						else
						{
							m_DEData.m_MasterUnit.StateChange("USN_DAMAGE_A", bForceChange: true, bImmediate: false, NKM_STATE_CHANGE_PRIORITY.NSCP_SKILL_CRASH);
						}
					}
					else
					{
						m_DEData.m_MasterUnit.StateChange("USN_DAMAGE_A", bForceChange: true, bImmediate: false, NKM_STATE_CHANGE_PRIORITY.NSCP_SKILL_CRASH);
					}
				}
				else
				{
					m_DEData.m_MasterUnit.StateChangeToASTAND();
				}
				m_DEData.m_MasterUnit.SetStopTime(0.5f, NKM_STOP_TIME_INDEX.NSTI_DAMAGE);
				m_DEData.m_MasterUnit.SetStopReserveTime(0.1f);
				SetDie(bForce: false, bDieEvent: false);
			}
			else if (cNKMDamageInst.m_Templet.m_AttackerStateChange.Length > 1)
			{
				m_DEData.m_MasterUnit.StateChange(cNKMDamageInst.m_Templet.m_AttackerStateChange);
			}
		}
		m_DEData.m_DamageCountNow++;
		if (pDefender != null && pDefender.GetUnitTemplet().m_PenetrateDefence > 0)
		{
			m_DEData.m_DamageCountNow += pDefender.GetUnitTemplet().m_PenetrateDefence;
		}
	}

	protected virtual void ProcessAttackHitEffect(NKMEventAttack cNKMEventAttack)
	{
	}

	protected virtual void ProcessDieEventAttack()
	{
		for (int i = 0; i < m_DETemplet.m_listNKMDieEventAttack.Count; i++)
		{
			NKMEventAttack nKMEventAttack = m_DETemplet.m_listNKMDieEventAttack[i];
			if (nKMEventAttack != null && CheckEventCondition(nKMEventAttack.m_Condition))
			{
				NKMDamageInst damageInstAtk = GetDamageInstAtk(-1);
				damageInstAtk.Init();
				if (damageInstAtk.m_Templet == null)
				{
					damageInstAtk.m_Templet = NKMDamageManager.GetTempletByStrID(nKMEventAttack.m_DamageTempletName);
					damageInstAtk.m_AttackerType = NKM_REACTOR_TYPE.NRT_DAMAGE_EFFECT;
					damageInstAtk.m_AttackerEffectUID = m_DamageEffectUID;
					damageInstAtk.m_AttackerGameUnitUID = GetDEData().m_MasterGameUnitUID;
					damageInstAtk.m_AttackerUnitSkillTemplet = m_UnitSkillTemplet;
					damageInstAtk.m_AttackerTeamType = GetDEData().m_NKM_TEAM_TYPE;
				}
				m_NKMGame.DamageCheck(damageInstAtk, nKMEventAttack, bDieAttack: true);
			}
		}
	}

	protected virtual void ProcessDieEventEffect()
	{
	}

	protected virtual void ProcessDieEventDamageEffect()
	{
		for (int i = 0; i < GetTemplet().m_listNKMDieEventDamageEffect.Count; i++)
		{
			NKMEventDamageEffect nKMEventDamageEffect = GetTemplet().m_listNKMDieEventDamageEffect[i];
			if (nKMEventDamageEffect == null || !CheckEventCondition(nKMEventDamageEffect.m_Condition))
			{
				continue;
			}
			bool flag = true;
			if (nKMEventDamageEffect.m_bIgnoreNoTarget && m_TargetUnit == null)
			{
				flag = false;
			}
			if (flag)
			{
				m_NKMVector3Temp1.x = GetBasePosX(nKMEventDamageEffect.m_EventPosData.m_MoveBase, nKMEventDamageEffect.m_EventPosData.m_MoveBaseType, IsATeam(), nKMEventDamageEffect.m_EventPosData.m_fMapPosFactor);
				m_NKMVector3Temp1.y = GetBasePosY(nKMEventDamageEffect.m_EventPosData.m_MoveBase);
				m_NKMVector3Temp1.z = (nKMEventDamageEffect.m_bUseMyZPos ? m_DEData.m_PosZ : GetBasePosZ(nKMEventDamageEffect.m_EventPosData.m_MoveBase));
				float zScaleFactor = m_NKMGame.GetZScaleFactor(m_DEData.m_PosZ);
				short targetGameUID = m_DEData.m_TargetGameUnitUID;
				if (nKMEventDamageEffect.m_bIgnoreTarget)
				{
					targetGameUID = 0;
				}
				string templetID = nKMEventDamageEffect.m_DEName;
				if (m_NKMGame.IsPVP(bUseDevOption: true) && nKMEventDamageEffect.m_DENamePVP.Length > 1)
				{
					templetID = nKMEventDamageEffect.m_DENamePVP;
				}
				bool bRight = (nKMEventDamageEffect.m_bFlipRight ? (!m_DEData.m_bRight) : m_DEData.m_bRight);
				m_NKMGame.GetDEManager().UseDamageEffect(templetID, m_DEData.m_MasterGameUnitUID, targetGameUID, m_UnitSkillTemplet, m_MasterUnitPhase, m_NKMVector3Temp1.x, m_NKMVector3Temp1.y, m_NKMVector3Temp1.z, nKMEventDamageEffect.m_EventPosData.m_MoveOffset, nKMEventDamageEffect.m_EventPosData.m_fMapPosFactor, bRight, nKMEventDamageEffect.m_EventPosData.m_fOffsetX * zScaleFactor, nKMEventDamageEffect.m_EventPosData.m_fOffsetY * zScaleFactor, nKMEventDamageEffect.m_EventPosData.m_fOffsetZ, nKMEventDamageEffect.m_fAddRotate, nKMEventDamageEffect.m_bUseZScale, nKMEventDamageEffect.m_fSpeedFactorX, nKMEventDamageEffect.m_fSpeedFactorY, nKMEventDamageEffect.m_fReserveTime, bNextFrame: true);
			}
		}
	}

	protected virtual void ProcessDieEventSound()
	{
	}
}
