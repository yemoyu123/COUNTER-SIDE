using System;
using System.Collections.Generic;
using Cs.Logging;
using Cs.Math;
using NKM.Game;
using NKM.Templet;
using NKM.Unit;

namespace NKM;

public class NKMUnit : NKMObjectPoolData
{
	public delegate void StateChangeEvent(NKM_UNIT_STATE_CHANGE_TYPE stateChangeType, NKMUnit unit, NKMUnitState unitState);

	public class ReactionInstance
	{
		public short masterUnitID;

		public int ID;

		public int m_currentCount;

		public float m_fTimeLeft;

		public bool m_bFinished;

		public NKMUnitReaction Reaction;

		public NKMUnit lastInvoker;

		public NKMUnitReaction.ReactionEventType EventType => Reaction.m_EventType;

		public ReactionInstance(NKMUnit _masterUnit, int _id, float timeLeft)
		{
			SetNewReaction(_masterUnit, _id, timeLeft);
		}

		public void SetNewReaction(NKMUnit _masterUnit, int _id, float timeLeft)
		{
			masterUnitID = _masterUnit.GetUnitGameUID();
			ID = _id;
			m_fTimeLeft = timeLeft;
			m_currentCount = 0;
			Reaction = _masterUnit.GetUnitTemplet().GetReaction(_id);
			m_bFinished = false;
		}

		public bool IsInvokeCount()
		{
			return m_currentCount >= Reaction.m_RequireCount;
		}

		public bool CheckParam(float[] param)
		{
			return Reaction.CheckParam(param);
		}
	}

	public float m_RespawnTime;

	protected NKM_UNIT_CLASS_TYPE m_NKM_UNIT_CLASS_TYPE;

	protected NKMGame m_NKMGame;

	protected NKMUnitData m_UnitData;

	protected NKMUnitDataGame m_UnitDataGame = new NKMUnitDataGame();

	protected NKMUnitTemplet m_UnitTemplet;

	protected NKMUnitSyncData m_UnitSyncData = new NKMUnitSyncData();

	protected NKMUnitFrameData m_UnitFrameData = new NKMUnitFrameData();

	protected NKMUnitState m_UnitStateBefore;

	protected NKMUnitState m_UnitStateNow;

	protected NKMUnit m_TargetUnit;

	protected NKMUnit m_SubTargetUnit;

	protected string m_StateNameNow = "";

	protected string m_StateNameNext = "";

	protected string m_StateNameNextChange = "";

	protected NKM_STATE_CHANGE_PRIORITY m_StateNextPriority;

	protected List<NKMDamageInst> m_listDamageInstAtk = new List<NKMDamageInst>();

	protected Dictionary<float, NKMTimeStamp> m_EventTimeStampAnim = new Dictionary<float, NKMTimeStamp>();

	protected Dictionary<float, NKMTimeStamp> m_EventTimeStampState = new Dictionary<float, NKMTimeStamp>();

	protected Dictionary<int, NKMStateCoolTime> m_dicStateCoolTime = new Dictionary<int, NKMStateCoolTime>();

	protected Dictionary<int, float> m_dicStateMaxCoolTime = new Dictionary<int, float>();

	protected LinkedList<NKMDamageEffect> m_linklistDamageEffect = new LinkedList<NKMDamageEffect>();

	protected List<NKMShipSkillTemplet> m_listShipSkillTemplet = new List<NKMShipSkillTemplet>();

	protected List<int> m_listShipSkillStateID = new List<int>();

	protected float m_fUnitCollisonCheckTime;

	public NKMTrackingFloat m_EventMovePosX = new NKMTrackingFloat();

	public NKMTrackingFloat m_EventMovePosZ = new NKMTrackingFloat();

	public NKMTrackingFloat m_EventMovePosJumpY = new NKMTrackingFloat();

	protected float m_DeltaTime;

	protected float m_fPracticeHPReset = 3f;

	protected float m_fCheckUseShipSkillAuto;

	protected int m_TargetUIDOrg;

	protected int m_SubTargetUIDOrg;

	protected bool m_bRightOrg;

	protected bool m_PushSyncData;

	protected bool m_bPushSimpleSyncData;

	protected float m_fSyncRollbackTime;

	protected bool m_bConsumeRollback;

	protected NKMVector3 m_NKMVector3Temp = new NKMVector3(0f, 0f, 0f);

	protected float m_BuffProcessTime;

	protected float m_BuffDamageTime;

	protected List<short> m_listBuffDelete = new List<short>();

	protected List<NKMBuffCreateData> m_listBuffAdd = new List<NKMBuffCreateData>();

	protected List<NKMBuffData> m_listBuffDieEvent = new List<NKMBuffData>();

	protected bool m_bBuffChangedThisFrame;

	protected bool m_bBuffUnitLevelChangedThisFrame;

	protected bool m_bBuffHPRateConserveRequired;

	protected Dictionary<NKMEventRespawn, List<short>> m_dicDynamicRespawnPool = new Dictionary<NKMEventRespawn, List<short>>();

	protected Dictionary<string, List<short>> m_dicUnitChangeRespawnPool = new Dictionary<string, List<short>>();

	protected Dictionary<short, List<NKMKillFeedBack>> m_dicKillFeedBackGameUnitUID = new Dictionary<short, List<NKMKillFeedBack>>();

	protected List<NKMStaticBuffDataRuntime> m_listNKMStaticBuffDataRuntime = new List<NKMStaticBuffDataRuntime>();

	protected List<NKMTriggerRepeatRuntime> m_lstTriggerRepeatRuntime;

	protected List<NKMInvokedTrigger> m_lstInvokedTrigger = new List<NKMInvokedTrigger>();

	protected List<ReactionInstance> m_lstReactionEventInstance = new List<ReactionInstance>();

	public HashSet<string> m_hsVolatileEventVariables = new HashSet<string>();

	protected Dictionary<short, int> m_listDamageResistUnit = new Dictionary<short, int>();

	protected bool m_bDamageResistPositive = true;

	protected float m_fSortUnitDirtyCheckTime;

	protected bool m_bSortUnitDirty = true;

	protected bool m_bSortUnitBySizeDirty = true;

	public int m_usedRespawnCost;

	public float m_LowestHPRate = 1f;

	protected float m_TempSortDist;

	protected List<NKMUnit> m_listSortUnit = new List<NKMUnit>();

	protected List<NKMUnit> m_listSortUnitBySize = new List<NKMUnit>();

	protected List<int> m_listAttackSelectTemp = new List<int>();

	protected string beforeStateName1 = "";

	protected string beforeStateName2 = "";

	protected string beforeStateName3 = "";

	protected List<short> m_listBuffToDelete = new List<short>();

	protected List<NKM_UNIT_STATUS_EFFECT> m_lstTempStatus = new List<NKM_UNIT_STATUS_EFFECT>();

	protected bool m_bBoss;

	private StateChangeEvent dStateChangeEvent;

	public LinkedList<NKMDamageEffect> llDamageEffect => m_linklistDamageEffect;

	public NKMUnit CurrentTriggerTarget { get; protected set; }

	public NKM_UNIT_CLASS_TYPE Get_NKM_UNIT_CLASS_TYPE()
	{
		return m_NKM_UNIT_CLASS_TYPE;
	}

	public NKMUnitState GetUnitStateBefore()
	{
		return m_UnitStateBefore;
	}

	public NKMUnitState GetUnitStateNow()
	{
		return m_UnitStateNow;
	}

	public NKMUnitData GetUnitData()
	{
		return m_UnitData;
	}

	public NKMUnitDataGame GetUnitDataGame()
	{
		return m_UnitDataGame;
	}

	public NKMUnitTemplet GetUnitTemplet()
	{
		return m_UnitTemplet;
	}

	public NKMUnitTempletBase GetUnitTempletBase()
	{
		return m_UnitTemplet.m_UnitTempletBase;
	}

	public NKMUnitSyncData GetUnitSyncData()
	{
		return m_UnitSyncData;
	}

	public NKMUnitFrameData GetUnitFrameData()
	{
		return m_UnitFrameData;
	}

	public void SetPushSync()
	{
		m_PushSyncData = true;
	}

	public void SetPushSimpleSync()
	{
		m_bPushSimpleSyncData = true;
	}

	public void SetConserveHPRate()
	{
		m_bBuffHPRateConserveRequired = true;
	}

	public void SetSortUnitDirty()
	{
		m_fSortUnitDirtyCheckTime = 0.1f;
		m_bSortUnitDirty = true;
		m_bSortUnitBySizeDirty = true;
	}

	public float GetTempSortDist()
	{
		return m_TempSortDist;
	}

	public void SetBoss(bool bSet)
	{
		m_bBoss = bSet;
	}

	public bool IsBoss()
	{
		return m_bBoss;
	}

	public bool IsMonster()
	{
		if (HasMasterUnit())
		{
			NKMDungeonRespawnUnitTemplet nKMDungeonRespawnUnitTemplet = GetMasterUnit()?.m_UnitData?.m_DungeonRespawnUnitTemplet;
			if (nKMDungeonRespawnUnitTemplet != null && nKMDungeonRespawnUnitTemplet.m_bForceMonster)
			{
				return true;
			}
		}
		else if (m_UnitData.m_DungeonRespawnUnitTemplet != null && m_UnitData.m_DungeonRespawnUnitTemplet.m_bForceMonster)
		{
			return true;
		}
		return GetUnitTempletBase().m_bMonster;
	}

	public void AddStateChangeEvent(StateChangeEvent eventfunc)
	{
		dStateChangeEvent = (StateChangeEvent)Delegate.Combine(dStateChangeEvent, eventfunc);
	}

	public void RemoveStateChangeEvent(StateChangeEvent eventfunc)
	{
		dStateChangeEvent = (StateChangeEvent)Delegate.Remove(dStateChangeEvent, eventfunc);
	}

	public void ClearAllStateChangeEvent()
	{
		dStateChangeEvent = null;
	}

	public NKMUnit()
	{
		m_NKM_UNIT_CLASS_TYPE = NKM_UNIT_CLASS_TYPE.NCT_UNIT;
		m_bUnloadable = true;
	}

	public override string ToString()
	{
		return $"[{GetUnitDataGame().m_GameUnitUID}]{GetUnitTempletBase()}";
	}

	public override bool LoadComplete()
	{
		return true;
	}

	public override void Open()
	{
	}

	public override void Close()
	{
		m_TargetUnit = null;
		m_SubTargetUnit = null;
		InitLInkedDamageEffect();
		Dictionary<int, NKMStateCoolTime>.Enumerator enumerator = m_dicStateCoolTime.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMStateCoolTime value = enumerator.Current.Value;
			m_NKMGame.GetObjectPool().CloseObj(value);
		}
		m_dicStateCoolTime.Clear();
		m_dicDynamicRespawnPool.Clear();
		m_dicUnitChangeRespawnPool.Clear();
		m_hsVolatileEventVariables.Clear();
		m_dicStateMaxCoolTime.Clear();
		m_lstInvokedTrigger.Clear();
		m_lstReactionEventInstance.Clear();
	}

	public override void Unload()
	{
	}

	public virtual bool LoadUnit(NKMGame cNKMGame, NKMUnitData cNKMUnitData, short masterGameUnitUID, short gameUnitUID, float fNearTargetRange, NKM_TEAM_TYPE eNKM_TEAM_TYPE, bool bSub, bool bAsync)
	{
		m_NKMGame = cNKMGame;
		m_UnitStateNow = null;
		m_UnitStateBefore = null;
		m_StateNameNow = "";
		m_StateNameNext = "";
		m_StateNameNextChange = "";
		m_UnitDataGame.m_MasterGameUnitUID = masterGameUnitUID;
		m_UnitDataGame.m_GameUnitUID = gameUnitUID;
		m_UnitDataGame.m_fTargetNearRange = fNearTargetRange;
		m_UnitDataGame.m_NKM_TEAM_TYPE_ORG = eNKM_TEAM_TYPE;
		m_UnitDataGame.m_NKM_TEAM_TYPE = eNKM_TEAM_TYPE;
		m_UnitData = cNKMUnitData;
		m_UnitData.m_UnitUID = cNKMUnitData.m_UnitUID;
		m_UnitDataGame.m_UnitUID = cNKMUnitData.m_UnitUID;
		m_UnitSyncData.m_GameUnitUID = m_UnitDataGame.m_GameUnitUID;
		m_UnitTemplet = NKMUnitManager.GetUnitTemplet(m_UnitData.m_UnitID);
		if (m_UnitTemplet == null)
		{
			return false;
		}
		m_UnitSyncData.m_NKM_UNIT_PLAY_STATE = NKM_UNIT_PLAY_STATE.NUPS_DIE;
		m_UnitSyncData.m_dicEventVariables.Clear();
		if (m_UnitTemplet.m_listHyperSkillStateData.Count > 0 && m_UnitTemplet.m_listHyperSkillStateData[0] != null && m_UnitTemplet.m_listHyperSkillStateData[0].m_fStartCool > 0f && IsStateUnlocked(m_UnitTemplet.m_listHyperSkillStateData[0]))
		{
			float num = 1f;
			if (m_NKMGame != null)
			{
				num = m_NKMGame.GetHyperBeginRatio(eNKM_TEAM_TYPE);
				if (num.IsNearlyEqual(-1f))
				{
					num = m_UnitTemplet.m_listHyperSkillStateData[0].m_fStartCool;
				}
			}
			if (m_NKMGame != null && m_NKMGame.IsPVP(bUseDevOption: true))
			{
				num = m_UnitTemplet.m_listHyperSkillStateData[0].m_fStartCoolPVP;
			}
			for (int i = 0; i < m_UnitTemplet.m_listHyperSkillStateData.Count; i++)
			{
				SetStateCoolTime(m_UnitTemplet.m_listHyperSkillStateData[i].m_StateName, bMax: true, num);
			}
		}
		if (m_UnitTemplet.m_listSkillStateData.Count > 0 && m_UnitTemplet.m_listSkillStateData[0] != null && m_UnitTemplet.m_listSkillStateData[0].m_fStartCool > 0f && IsStateUnlocked(m_UnitTemplet.m_listSkillStateData[0]))
		{
			for (int j = 0; j < m_UnitTemplet.m_listSkillStateData.Count; j++)
			{
				SetStateCoolTime(m_UnitTemplet.m_listSkillStateData[0].m_StateName, bMax: true, m_UnitTemplet.m_listSkillStateData[0].m_fStartCool);
			}
		}
		for (int k = 0; k < GetUnitTemplet().m_listHitFeedBack.Count; k++)
		{
			GetUnitFrameData().m_listHitFeedBackCount.Add(0);
		}
		for (int l = 0; l < GetUnitTemplet().m_listHitCriticalFeedBack.Count; l++)
		{
			GetUnitFrameData().m_listHitCriticalFeedBackCount.Add(0);
		}
		for (int m = 0; m < GetUnitTemplet().m_listHitEvadeFeedBack.Count; m++)
		{
			GetUnitFrameData().m_listHitEvadeFeedBackCount.Add(0);
		}
		for (int n = 0; n < GetUnitTemplet().m_listAccumStateChangePack.Count; n++)
		{
			GetUnitFrameData().m_listUnitAccumStateData.Add(new NKMUnitAccumStateData());
		}
		m_listShipSkillTemplet.Clear();
		m_listShipSkillStateID.Clear();
		for (int num2 = 0; num2 < m_UnitTemplet.m_UnitTempletBase.GetSkillCount(); num2++)
		{
			NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(m_UnitTemplet.m_UnitTempletBase, num2);
			if (shipSkillTempletByIndex == null)
			{
				continue;
			}
			m_listShipSkillTemplet.Add(shipSkillTempletByIndex);
			if (shipSkillTempletByIndex.m_NKM_SKILL_TYPE != NKM_SKILL_TYPE.NST_SHIP_ACTIVE)
			{
				continue;
			}
			if (shipSkillTempletByIndex.m_UnitStateName.Length > 1)
			{
				NKMUnitState unitState = GetUnitState(shipSkillTempletByIndex.m_UnitStateName);
				if (unitState != null)
				{
					m_listShipSkillStateID.Add(unitState.m_StateID);
				}
			}
			if (m_NKMGame != null && m_NKMGame.GetDungeonTemplet() != null && NKMDungeonManager.IsTutorialDungeon(m_NKMGame.GetDungeonTemplet().m_DungeonTempletBase.m_DungeonID))
			{
				SetStateCoolTime(shipSkillTempletByIndex.m_UnitStateName, bMax: true, 0f);
			}
			else
			{
				SetStateCoolTime(shipSkillTempletByIndex.m_UnitStateName, bMax: true, 0.3f);
			}
		}
		if (m_NKMGame != null)
		{
			if (m_NKMGame.IsBTeam(eNKM_TEAM_TYPE))
			{
				if (m_NKMGame.GetDungeonTemplet() != null)
				{
					NKMDungeonTempletBase dungeonTempletBase = m_NKMGame.GetDungeonTemplet().m_DungeonTempletBase;
					if (dungeonTempletBase != null && dungeonTempletBase.m_StageSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
					{
						m_UnitFrameData.m_UnitSourceType = dungeonTempletBase.m_StageSourceTypeMain;
						m_UnitFrameData.m_UnitSourceTypeSub = dungeonTempletBase.m_StageSourceTypeSub;
					}
					else
					{
						m_UnitFrameData.m_UnitSourceType = m_UnitTemplet.m_UnitTempletBase.m_NKM_UNIT_SOURCE_TYPE;
						m_UnitFrameData.m_UnitSourceTypeSub = m_UnitTemplet.m_UnitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB;
					}
				}
			}
			else if (m_UnitData.GetUnitTempletBase() != null)
			{
				m_UnitFrameData.m_UnitSourceType = m_UnitData.GetUnitTempletBase().m_NKM_UNIT_SOURCE_TYPE;
				m_UnitFrameData.m_UnitSourceTypeSub = m_UnitData.GetUnitTempletBase().m_NKM_UNIT_SOURCE_TYPE_SUB;
			}
		}
		return true;
	}

	public virtual void LoadUnitComplete()
	{
	}

	public virtual void InitLInkedDamageEffect()
	{
		foreach (NKMDamageEffect item in m_linklistDamageEffect)
		{
			item.SetDie(bForce: false, bDieEvent: false);
		}
		m_linklistDamageEffect.Clear();
	}

	public virtual void RespawnUnit(float fPosX, float fPosZ, float fJumpYPos, bool bUseRight = false, bool bRight = true, float fInitHP = 0f, bool bInitHPRate = false, float rollbackTime = 0f)
	{
		m_fSyncRollbackTime = rollbackTime;
		m_bConsumeRollback = false;
		m_TargetUnit = null;
		m_SubTargetUnit = null;
		m_UnitStateNow = null;
		m_UnitStateBefore = null;
		m_UnitSyncData.RespawnInit(m_fSyncRollbackTime > 0f);
		m_UnitFrameData.RespawnInit();
		InitLInkedDamageEffect();
		SetSortUnitDirty();
		foreach (string hsVolatileEventVariable in m_hsVolatileEventVariables)
		{
			SetEventVariable(hsVolatileEventVariable, 0);
		}
		m_hsVolatileEventVariables.Clear();
		if (m_fSyncRollbackTime > 0f)
		{
			m_UnitFrameData.m_fAnimTime = m_fSyncRollbackTime;
			m_UnitFrameData.m_fStateTime = m_fSyncRollbackTime;
		}
		m_listDamageResistUnit.Clear();
		m_UnitFrameData.m_fFindTargetTime = 0f;
		m_UnitFrameData.m_fFindSubTargetTime = 0f;
		m_UnitDataGame.m_NKM_TEAM_TYPE = m_UnitDataGame.m_NKM_TEAM_TYPE_ORG;
		m_UnitDataGame.m_RespawnPosX = fPosX;
		m_UnitDataGame.m_RespawnPosZ = fPosZ;
		m_UnitDataGame.m_RespawnJumpYPos = fJumpYPos;
		m_EventMovePosX.Init();
		m_EventMovePosZ.Init();
		m_EventMovePosJumpY.Init();
		m_UnitFrameData.m_PosXCalc = m_UnitDataGame.m_RespawnPosX;
		m_UnitFrameData.m_PosZCalc = m_UnitDataGame.m_RespawnPosZ;
		m_UnitFrameData.m_JumpYPosCalc = m_UnitDataGame.m_RespawnJumpYPos;
		m_UnitSyncData.m_PosX = m_UnitDataGame.m_RespawnPosX;
		m_UnitSyncData.m_PosZ = m_UnitDataGame.m_RespawnPosZ;
		m_UnitSyncData.m_JumpYPos = m_UnitDataGame.m_RespawnJumpYPos;
		m_UnitFrameData.m_fTargetAirHigh = m_UnitTemplet.m_fAirHigh;
		m_UnitFrameData.m_fAirHigh = m_UnitTemplet.m_fAirHigh;
		if (bUseRight)
		{
			m_UnitSyncData.m_bRight = bRight;
		}
		else if (m_NKMGame.IsATeam(m_UnitDataGame.m_NKM_TEAM_TYPE))
		{
			m_UnitSyncData.m_bRight = true;
		}
		else
		{
			m_UnitSyncData.m_bRight = false;
		}
		m_UnitData.m_UserUID = m_NKMGame.GetGameData().GetTeamData(m_UnitDataGame.m_NKM_TEAM_TYPE).m_user_uid;
		m_UnitFrameData.m_StatData = NKMUnitStatManager.MakeFinalStat(m_NKMGame, m_UnitData, m_UnitDataGame.m_NKM_TEAM_TYPE, this, GetOperatorForStat());
		m_bDamageResistPositive = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_RESIST) >= 0f;
		if (!bInitHPRate)
		{
			m_UnitFrameData.m_fInitHP = fInitHP;
		}
		else
		{
			m_UnitFrameData.m_fInitHP = GetMaxHP(fInitHP);
		}
		if (!m_UnitFrameData.m_fInitHP.IsNearlyZero())
		{
			m_UnitSyncData.SetHP(m_UnitFrameData.m_fInitHP);
		}
		else
		{
			m_UnitFrameData.m_fInitHP = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP);
			m_UnitSyncData.SetHP(m_UnitFrameData.m_fInitHP);
		}
		m_LowestHPRate = GetHPRate();
		m_UnitSyncData.m_NKM_UNIT_PLAY_STATE = NKM_UNIT_PLAY_STATE.NUPS_PLAY;
		StateEvent_Phase(bRespawnTime: true);
		SaveEventMovePosition(fPosX, fJumpYPos, bSaveXOnly: false);
		InitTriggerRepeat();
		m_lstInvokedTrigger.Clear();
		m_lstReactionEventInstance.Clear();
	}

	protected float GetCoolTimeReducedByOperator(float fInputTime)
	{
		float result = fInputTime;
		if (m_UnitTemplet == null || m_UnitTemplet.m_UnitTempletBase == null)
		{
			return result;
		}
		if (m_UnitTemplet.m_UnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
		{
			return result;
		}
		NKMGameData gameData = m_NKMGame.GetGameData();
		if (gameData == null)
		{
			return result;
		}
		NKMGameTeamData teamData = gameData.GetTeamData(GetTeam());
		if (teamData == null)
		{
			return result;
		}
		NKMOperator nKMOperator = teamData.m_Operator;
		if (nKMOperator == null)
		{
			return result;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(nKMOperator.id);
		if (unitTempletBase == null || unitTempletBase.StatTemplet == null || unitTempletBase.StatTemplet.m_StatData == null)
		{
			return result;
		}
		float statBase = unitTempletBase.StatTemplet.m_StatData.GetStatBase(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE);
		if (statBase <= 0f)
		{
			return result;
		}
		statBase /= 10000f;
		float num = 1f;
		if (m_UnitTemplet.m_UnitTempletBase.IsShip() && gameData.IsPVP() && gameData.IsBanOperator(nKMOperator.id))
		{
			int banOperatorLevel = gameData.GetBanOperatorLevel(nKMOperator.id);
			float num2 = NKMUnitStatManager.m_fPercentPerBanLevel * (float)banOperatorLevel;
			if (num2 > NKMUnitStatManager.m_fMaxPercentPerUpLevel)
			{
				num2 = NKMUnitStatManager.m_fMaxPercentPerBanLevel;
			}
			num -= num2;
		}
		result = fInputTime - fInputTime * statBase * num;
		if (result <= 0f)
		{
			result = 0f;
		}
		Log.Debug("GetCoolTimeReducedByOperator success input : " + fInputTime + ", output : " + result, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 1562);
		return result;
	}

	public virtual void Update(float deltaTime)
	{
		m_DeltaTime = deltaTime;
		m_fSortUnitDirtyCheckTime -= deltaTime;
		if (m_fSortUnitDirtyCheckTime < 0f)
		{
			SetSortUnitDirty();
		}
		if (m_UnitDataGame == null || m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			return;
		}
		m_TargetUIDOrg = m_UnitSyncData.m_TargetUID;
		m_SubTargetUIDOrg = m_UnitSyncData.m_SubTargetUID;
		m_bRightOrg = m_UnitSyncData.m_bRight;
		if (GetUnitFrameData().m_PosXBefore.IsNearlyEqual(-1f))
		{
			GetUnitFrameData().m_PosXBefore = GetUnitSyncData().m_PosX;
		}
		if (GetUnitFrameData().m_PosZBefore.IsNearlyEqual(-1f))
		{
			GetUnitFrameData().m_PosZBefore = GetUnitSyncData().m_PosZ;
		}
		if (GetUnitFrameData().m_JumpYPosBefore.IsNearlyEqual(-1f))
		{
			GetUnitFrameData().m_JumpYPosBefore = GetUnitSyncData().m_JumpYPos;
		}
		if (!IsStopTime())
		{
			m_UnitFrameData.m_fStopReserveTime -= m_DeltaTime;
			if (m_UnitFrameData.m_fStopReserveTime < 0f)
			{
				m_UnitFrameData.m_fStopReserveTime = 0f;
			}
			m_UnitFrameData.m_PosXCalc = m_UnitSyncData.m_PosX;
			m_UnitFrameData.m_PosZCalc = m_UnitSyncData.m_PosZ;
			m_UnitFrameData.m_JumpYPosCalc = m_UnitSyncData.m_JumpYPos;
			StateTimeUpdate();
			AnimTimeUpdate();
			DoStateEndStart();
			StateUpdate();
			if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
			{
				StateEvent();
			}
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			m_UnitFrameData.m_StopTime[i] -= m_DeltaTime;
			if (m_UnitFrameData.m_StopTime[i] < 0f)
			{
				m_UnitFrameData.m_StopTime[i] = 0f;
			}
		}
	}

	public virtual void Update2()
	{
		if (m_UnitDataGame == null)
		{
			return;
		}
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE && !IsStopTime())
		{
			m_UnitFrameData.m_fDamageBeforeFrame = m_UnitFrameData.m_fDamageThisFrame;
			if (m_UnitFrameData.m_bInvincible || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE))
			{
				m_UnitFrameData.m_fDamageThisFrame = 0f;
			}
			if (m_UnitStateNow != null && m_UnitStateNow.m_bInvincibleState)
			{
				m_UnitFrameData.m_fDamageThisFrame = 0f;
			}
			GetUnitFrameData().m_PosXBefore = m_UnitSyncData.m_PosX;
			GetUnitFrameData().m_PosZBefore = m_UnitSyncData.m_PosZ;
			GetUnitFrameData().m_JumpYPosBefore = m_UnitSyncData.m_JumpYPos;
			m_UnitSyncData.m_PosX = m_UnitFrameData.m_PosXCalc;
			m_UnitSyncData.m_PosZ = m_UnitFrameData.m_PosZCalc;
			m_UnitSyncData.m_JumpYPos = m_UnitFrameData.m_JumpYPosCalc;
			if (m_UnitFrameData.m_bFindTargetThisFrame)
			{
				AITarget();
				m_UnitFrameData.m_bFindTargetThisFrame = false;
				m_UnitFrameData.m_fFindTargetTime = GetUnitTemplet().m_TargetFindData.m_fFindTargetTime;
			}
			if (m_UnitFrameData.m_bFindSubTargetThisFrame && GetUnitTemplet().m_SubTargetFindData != null)
			{
				AISubTarget();
				m_UnitFrameData.m_bFindSubTargetThisFrame = false;
				m_UnitFrameData.m_fFindSubTargetTime = GetUnitTemplet().m_SubTargetFindData.m_fFindTargetTime;
			}
			if (m_NKMGame.GetGameDevModeData().m_bNoHPDamageModeTeamA && m_NKMGame.IsATeam(GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				m_UnitFrameData.m_fDamageThisFrame = 0f;
			}
			if (m_NKMGame.GetGameDevModeData().m_bNoHPDamageModeTeamB && m_NKMGame.IsBTeam(GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				m_UnitFrameData.m_fDamageThisFrame = 0f;
			}
			if (m_UnitFrameData.m_fDamageThisFrame > 0f)
			{
				if (m_UnitFrameData.m_BarrierBuffData != null && m_UnitFrameData.m_BarrierBuffData.m_fBarrierHP > 0f)
				{
					m_UnitFrameData.m_BarrierBuffData.m_fBarrierHP -= m_UnitFrameData.m_fDamageThisFrame;
					if (m_UnitFrameData.m_BarrierBuffData.m_fBarrierHP < 0f)
					{
						m_UnitFrameData.m_BarrierBuffData.m_fBarrierHP = 0f;
					}
				}
				else
				{
					if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMORTAL))
					{
						if (m_UnitFrameData.m_fDamageThisFrame >= m_UnitSyncData.GetHP())
						{
							m_UnitFrameData.m_bImmortalStart = true;
							m_UnitSyncData.SetHP(1f);
							GetUnitFrameData().m_fDangerChargeDamage += m_UnitFrameData.m_fDamageThisFrame;
						}
						else
						{
							m_UnitSyncData.SetHP(m_UnitSyncData.GetHP() - m_UnitFrameData.m_fDamageThisFrame);
							GetUnitFrameData().m_fDangerChargeDamage += m_UnitFrameData.m_fDamageThisFrame;
						}
					}
					else
					{
						m_UnitSyncData.SetHP(m_UnitSyncData.GetHP() - m_UnitFrameData.m_fDamageThisFrame);
						GetUnitFrameData().m_fDangerChargeDamage += m_UnitFrameData.m_fDamageThisFrame;
					}
					bool cutDamage;
					float phaseDamageLimit = GetPhaseDamageLimit(out cutDamage);
					if (phaseDamageLimit > 0f && GetNowHP() < phaseDamageLimit)
					{
						if (cutDamage)
						{
							m_UnitSyncData.SetHP(phaseDamageLimit - 1f);
						}
						if (!IsMonster() && GetNowHP() <= 0f)
						{
							m_UnitSyncData.SetHP(1f);
						}
					}
				}
				m_UnitFrameData.m_fDamageThisFrame = 0f;
			}
			if (m_UnitSyncData.GetHP() <= 0f)
			{
				if (m_NKMGame.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
				{
					m_UnitSyncData.SetHP(1f);
				}
				else
				{
					m_UnitSyncData.SetHP(0f);
					SetDying();
				}
			}
			float hPRate = GetHPRate();
			if (hPRate < m_LowestHPRate)
			{
				OnReactionEvent(NKMUnitReaction.ReactionEventType.HP_RATE, this, 1, m_LowestHPRate, hPRate);
				BroadcastReactionEvent(NKMUnitReaction.ReactionEventType.UNIT_HP_RATE, this, 1, m_LowestHPRate, hPRate);
				m_LowestHPRate = hPRate;
			}
		}
		if (!IsStopTime())
		{
			if (m_bRightOrg != m_UnitSyncData.m_bRight)
			{
				m_bPushSimpleSyncData = true;
			}
			if (m_TargetUIDOrg != 0 && m_UnitSyncData.m_TargetUID != 0 && m_TargetUIDOrg != m_UnitSyncData.m_TargetUID)
			{
				m_bPushSimpleSyncData = true;
			}
			if (m_SubTargetUIDOrg != 0 && m_UnitSyncData.m_SubTargetUID != 0 && m_SubTargetUIDOrg != m_UnitSyncData.m_SubTargetUID)
			{
				m_bPushSimpleSyncData = true;
			}
			bool flag = false;
			if (m_StateNameNext.Length > 1)
			{
				m_StateNameNextChange = m_StateNameNext;
				m_StateNameNext = "";
				m_StateNextPriority = NKM_STATE_CHANGE_PRIORITY.NSCP_INVALID;
				flag = true;
			}
			if (!flag && m_PushSyncData)
			{
				PushSyncData();
			}
			else if (!flag && m_bPushSimpleSyncData)
			{
				PushSimpleSyncData();
			}
			m_bPushSimpleSyncData = false;
		}
	}

	protected void StateTimeUpdate()
	{
		if (m_UnitStateNow != null)
		{
			m_UnitFrameData.m_fStateTimeBack = m_UnitFrameData.m_fStateTime;
			m_UnitFrameData.m_fStateTime += m_DeltaTime;
		}
	}

	protected void AnimTimeUpdate()
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		if (m_UnitFrameData.m_fAnimTime >= m_UnitFrameData.m_fAnimTimeMax && m_UnitStateNow.m_bAnimLoop)
		{
			m_UnitFrameData.m_fAnimTimeBack = 0f;
			m_UnitFrameData.m_fAnimTime = 0f;
			Dictionary<float, NKMTimeStamp>.Enumerator enumerator = m_EventTimeStampAnim.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NKMTimeStamp value = enumerator.Current.Value;
				m_NKMGame.GetObjectPool().CloseObj(value);
			}
			m_EventTimeStampAnim.Clear();
			ChangeAnimSpeed();
		}
		m_UnitFrameData.m_fAnimTimeBack = m_UnitFrameData.m_fAnimTime;
		m_UnitFrameData.m_fAnimTime += m_DeltaTime * m_UnitFrameData.m_fAnimSpeed;
		m_UnitFrameData.m_bAnimPlayCountAddThisFrame = false;
		if (m_UnitFrameData.m_fAnimTime >= m_UnitFrameData.m_fAnimTimeMax)
		{
			if (m_UnitStateNow.m_bAnimLoop)
			{
				m_UnitFrameData.m_AnimPlayCount++;
				m_UnitFrameData.m_bAnimPlayCountAddThisFrame = true;
			}
			else
			{
				m_UnitFrameData.m_AnimPlayCount = 1;
			}
		}
	}

	public NKMUnitState GetUnitState(short StateID)
	{
		return m_UnitTemplet.GetUnitState(StateID);
	}

	public NKMUnitState GetUnitState(string StateName, bool bLog = true)
	{
		return m_UnitTemplet.GetUnitState(StateName, bLog);
	}

	protected void DoStateEndStart()
	{
		if (m_StateNameNextChange.Length <= 1)
		{
			return;
		}
		StateEnd();
		beforeStateName3 = beforeStateName2;
		beforeStateName2 = beforeStateName1;
		beforeStateName1 = m_StateNameNow;
		m_StateNameNow = m_StateNameNextChange;
		m_StateNameNextChange = "";
		m_UnitStateBefore = m_UnitStateNow;
		m_UnitStateNow = GetUnitState(m_StateNameNow);
		if (m_UnitStateNow == null)
		{
			Log.Error("UnitState is null. " + m_StateNameNow, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 1891);
			m_StateNameNow = "USN_ASTAND";
			m_UnitStateNow = GetUnitState("USN_ASTAND");
			return;
		}
		m_UnitSyncData.m_StateID = m_UnitStateNow.m_StateID;
		if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			m_UnitSyncData.m_StateChangeCount++;
			m_PushSyncData = true;
			if (m_UnitFrameData.m_ShipSkillTemplet != null && m_UnitFrameData.m_ShipSkillTemplet.m_UnitStateName == m_StateNameNow)
			{
				m_UnitFrameData.m_bSyncShipSkill = true;
			}
		}
		StateStart();
	}

	public void SetSyncRollbackTime(float rollbackTime, bool bWillConsumeRollback)
	{
		m_bConsumeRollback = bWillConsumeRollback;
		m_fSyncRollbackTime = rollbackTime;
	}

	public float GetSyncRollbackTime()
	{
		return m_fSyncRollbackTime;
	}

	public virtual void PushSyncData()
	{
		m_PushSyncData = false;
		m_bPushSimpleSyncData = false;
		if (m_bConsumeRollback)
		{
			m_fSyncRollbackTime = 0f;
		}
		m_UnitSyncData.m_listDamageData.Clear();
		m_UnitSyncData.m_listStatusTimeData.Clear();
	}

	protected virtual void PushSimpleSyncData()
	{
		m_bPushSimpleSyncData = false;
	}

	protected virtual void StateEnd()
	{
		if (m_UnitStateNow != null)
		{
			ProcessStateEvents(bStateEnd: true);
			if (m_UnitSyncData.m_TargetUID > 0 && (m_TargetUnit == null || m_TargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE || m_TargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING))
			{
				m_UnitSyncData.m_TargetUID = 0;
			}
			if (m_UnitSyncData.m_SubTargetUID > 0 && (m_SubTargetUnit == null || m_SubTargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE || m_SubTargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING))
			{
				m_UnitSyncData.m_SubTargetUID = 0;
			}
			dStateChangeEvent?.Invoke(NKM_UNIT_STATE_CHANGE_TYPE.NUSCT_END, this, m_UnitStateNow);
		}
	}

	private float FindZeroFrameAnimSpeedEvent(NKMUnitState unitState)
	{
		if (unitState == null)
		{
			return 1f;
		}
		foreach (NKMEventAnimSpeed item in unitState.m_listNKMEventAnimSpeed)
		{
			if (item.m_fEventTime == 0f && CheckEventCondition(item.m_Condition))
			{
				return item.m_fAnimSpeed;
			}
		}
		return unitState.m_fAnimSpeed;
	}

	protected virtual void StateStart()
	{
		m_bConsumeRollback = true;
		float num = FindZeroFrameAnimSpeedEvent(m_UnitStateNow);
		if (m_UnitStateNow.m_AnimName.Length > 1)
		{
			m_UnitFrameData.m_fAnimTimeBack = m_fSyncRollbackTime * num;
			if (m_UnitStateNow.m_fAnimStartTime > 0f)
			{
				m_UnitFrameData.m_fAnimTimeBack = m_UnitStateNow.m_fAnimStartTime + m_fSyncRollbackTime * num;
			}
			m_UnitFrameData.m_fAnimTime = m_UnitStateNow.m_fAnimStartTime + m_fSyncRollbackTime * num;
			m_UnitFrameData.m_fAnimTimeMax = NKMAnimDataManager.GetAnimTimeMax(m_UnitTemplet.m_UnitTempletBase.m_SpriteBundleName, m_UnitTemplet.m_UnitTempletBase.m_SpriteName, m_UnitStateNow.m_AnimName);
			if (m_UnitFrameData.m_fAnimTimeMax.IsNearlyZero())
			{
				Log.Error("NKMUnit NoExistAnim: " + GetUnitTemplet().m_UnitTempletBase.m_UnitStrID + " : " + m_UnitStateNow.m_StateName + " : " + m_UnitStateNow.m_AnimName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 2021);
				return;
			}
			m_UnitFrameData.m_AnimPlayCount = 0;
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_AIR_TO_GROUND))
		{
			m_UnitFrameData.m_fTargetAirHigh = -1f;
		}
		else if (!m_UnitStateNow.m_fAirHigh.IsNearlyEqual(-1f))
		{
			m_UnitFrameData.m_fTargetAirHigh = m_UnitStateNow.m_fAirHigh;
		}
		else
		{
			m_UnitFrameData.m_fTargetAirHigh = m_UnitTemplet.m_fAirHigh;
		}
		ChangeAnimSpeed(num);
		if (m_UnitStateNow.m_bAutoCoolTime)
		{
			m_UnitStateNow.m_StateCoolTime.m_Min = m_UnitFrameData.m_fAnimTimeMax - 0.2f;
			m_UnitStateNow.m_StateCoolTime.m_Max = m_UnitFrameData.m_fAnimTimeMax + 0.2f;
			if (m_UnitFrameData.m_fAnimSpeed > 0f)
			{
				m_UnitStateNow.m_StateCoolTime.m_Min /= m_UnitFrameData.m_fAnimSpeed;
				m_UnitStateNow.m_StateCoolTime.m_Max /= m_UnitFrameData.m_fAnimSpeed;
			}
			else
			{
				m_UnitStateNow.m_StateCoolTime.m_Min = 0f;
				m_UnitStateNow.m_StateCoolTime.m_Max = 0f;
			}
		}
		m_UnitFrameData.m_fStateTime = m_fSyncRollbackTime;
		m_UnitFrameData.m_fStateTimeBack = m_fSyncRollbackTime;
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
		SetStateCoolTime(m_UnitStateNow, bMax: false);
		if (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DAMAGE && m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_START)
		{
			SeeTarget();
		}
		if (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DAMAGE && m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_START && GetUnitTemplet().m_bSeeMoreEnemy)
		{
			SeeMoreEnemy();
		}
		if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			AccumStateProcess();
		}
		dStateChangeEvent?.Invoke(NKM_UNIT_STATE_CHANGE_TYPE.NUSCT_START, this, m_UnitStateNow);
		if (m_UnitStateNow.m_bForceRight)
		{
			if (!m_UnitStateNow.m_bForceRightLeftDependTeam)
			{
				m_UnitSyncData.m_bRight = true;
			}
			else if (IsATeam())
			{
				m_UnitSyncData.m_bRight = true;
			}
			else
			{
				m_UnitSyncData.m_bRight = false;
			}
		}
		if (m_UnitStateNow.m_bForceLeft)
		{
			if (!m_UnitStateNow.m_bForceRightLeftDependTeam)
			{
				m_UnitSyncData.m_bRight = false;
			}
			else if (IsATeam())
			{
				m_UnitSyncData.m_bRight = false;
			}
			else
			{
				m_UnitSyncData.m_bRight = true;
			}
		}
		GetUnitFrameData().m_fDangerChargeTime = m_UnitStateNow.m_DangerCharge.m_fChargeTime;
		GetUnitFrameData().m_fDangerChargeDamage = 0f;
		GetUnitFrameData().m_DangerChargeHitCount = 0;
		if (m_UnitStateNow.m_bInvincibleState || GetUnitFrameData().m_bInvincible || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE))
		{
			RemoveStatus(NKM_UNIT_STATUS_EFFECT.NUSE_STUN);
			RemoveStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP);
		}
		if (m_UnitStateNow.m_StatusEffectType == NKM_UNIT_STATUS_EFFECT.NUSE_STUN)
		{
			m_UnitFrameData.m_fSpeedX = 0f;
			m_UnitFrameData.m_fDamageSpeedX = 0f;
			m_UnitFrameData.m_fDamageSpeedKeepTimeX = 0f;
		}
		for (int i = 0; i < m_UnitTemplet.m_listHyperSkillStateData.Count; i++)
		{
			if (m_UnitStateNow.m_StateName == m_UnitTemplet.m_listHyperSkillStateData[i].m_StateName)
			{
				m_StateNameNext = "";
				m_StateNameNextChange = "";
				return;
			}
		}
		for (int j = 0; j < m_UnitTemplet.m_listAirHyperSkillStateData.Count; j++)
		{
			if (m_UnitStateNow.m_StateName == m_UnitTemplet.m_listAirHyperSkillStateData[j].m_StateName)
			{
				m_StateNameNext = "";
				m_StateNameNextChange = "";
				return;
			}
		}
		for (int k = 0; k < m_UnitTemplet.m_listPhaseChangeData.Count; k++)
		{
			if (m_UnitStateNow.m_StateName == m_UnitTemplet.m_listPhaseChangeData[k].m_ChangeStateName)
			{
				m_StateNameNext = "";
				m_StateNameNextChange = "";
				break;
			}
		}
	}

	protected void AccumStateProcessUpdate()
	{
		for (int i = 0; i < GetUnitFrameData().m_listUnitAccumStateData.Count; i++)
		{
			foreach (KeyValuePair<string, NKMUnitAccumStateDataCount> item in GetUnitFrameData().m_listUnitAccumStateData[i].m_dicAccumStateChange)
			{
				if (item.Value.m_fCountCoolTimeNow > 0f)
				{
					item.Value.m_fCountCoolTimeNow -= m_DeltaTime;
					if (item.Value.m_fCountCoolTimeNow < 0f)
					{
						item.Value.m_fCountCoolTimeNow = 0f;
					}
				}
				if (item.Value.m_fMainCoolTimeNow > 0f)
				{
					item.Value.m_fMainCoolTimeNow -= m_DeltaTime;
					if (item.Value.m_fMainCoolTimeNow < 0f)
					{
						item.Value.m_fMainCoolTimeNow = 0f;
					}
				}
			}
		}
	}

	protected void AccumStateProcess()
	{
		for (int i = 0; i < GetUnitTemplet().m_listAccumStateChangePack.Count; i++)
		{
			NKMAccumStateChangePack nKMAccumStateChangePack = GetUnitTemplet().m_listAccumStateChangePack[i];
			if (nKMAccumStateChangePack == null || !CheckEventCondition(nKMAccumStateChangePack.m_Condition))
			{
				continue;
			}
			for (int j = 0; j < nKMAccumStateChangePack.m_listAccumStateChange.Count; j++)
			{
				bool flag = false;
				NKMAccumStateChange nKMAccumStateChange = nKMAccumStateChangePack.m_listAccumStateChange[j];
				for (int k = 0; k < nKMAccumStateChange.m_listAccumStateName.Count; k++)
				{
					if (nKMAccumStateChange.m_listAccumStateName[k].CompareTo(m_UnitStateNow.m_StateName) == 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag || nKMAccumStateChange.m_listAccumStateName.Count <= 0)
				{
					continue;
				}
				string key = nKMAccumStateChange.m_listAccumStateName[0];
				NKMUnitAccumStateData nKMUnitAccumStateData = GetUnitFrameData().m_listUnitAccumStateData[i];
				if (!nKMUnitAccumStateData.m_dicAccumStateChange.ContainsKey(key))
				{
					NKMUnitAccumStateDataCount nKMUnitAccumStateDataCount = new NKMUnitAccumStateDataCount();
					nKMUnitAccumStateDataCount.m_StateCount = 1;
					nKMUnitAccumStateDataCount.m_fCountCoolTimeMax = nKMAccumStateChange.m_fAccumCountCoolTime;
					nKMUnitAccumStateDataCount.m_fCountCoolTimeNow = nKMUnitAccumStateDataCount.m_fCountCoolTimeMax;
					nKMUnitAccumStateDataCount.m_fMainCoolTimeMax = nKMAccumStateChange.m_fAccumMainCoolTime;
					nKMUnitAccumStateData.m_dicAccumStateChange.Add(key, nKMUnitAccumStateDataCount);
				}
				else
				{
					NKMUnitAccumStateDataCount nKMUnitAccumStateDataCount2 = nKMUnitAccumStateData.m_dicAccumStateChange[key];
					if (nKMUnitAccumStateDataCount2.m_fCountCoolTimeNow <= 0f)
					{
						nKMUnitAccumStateDataCount2.m_StateCount++;
						nKMUnitAccumStateDataCount2.m_fCountCoolTimeNow = nKMUnitAccumStateDataCount2.m_fCountCoolTimeMax;
					}
				}
			}
			if (GetUnitStateNow().m_NKM_SKILL_TYPE >= NKM_SKILL_TYPE.NST_SKILL)
			{
				break;
			}
			for (int l = 0; l < nKMAccumStateChangePack.m_listAccumStateChange.Count; l++)
			{
				NKMAccumStateChange nKMAccumStateChange2 = nKMAccumStateChangePack.m_listAccumStateChange[l];
				NKMUnitAccumStateDataCount value = null;
				if (nKMAccumStateChange2.m_listAccumStateName.Count <= 0)
				{
					continue;
				}
				string key2 = nKMAccumStateChange2.m_listAccumStateName[0];
				if (!GetUnitFrameData().m_listUnitAccumStateData[i].m_dicAccumStateChange.TryGetValue(key2, out value) || !(value.m_fMainCoolTimeNow <= 0f) || value.m_StateCount <= nKMAccumStateChange2.m_AccumCount)
				{
					continue;
				}
				if (nKMAccumStateChange2.m_fRange.m_Min >= 0f || nKMAccumStateChange2.m_fRange.m_Max >= 0f)
				{
					if (m_TargetUnit == null)
					{
						continue;
					}
					float num = Math.Abs(m_TargetUnit.GetUnitSyncData().m_PosX - m_UnitSyncData.m_PosX) - (m_TargetUnit.GetUnitTemplet().m_UnitSizeX * 0.5f + m_UnitTemplet.m_UnitSizeX * 0.5f);
					if (num < 0f)
					{
						num = 0f;
					}
					if ((nKMAccumStateChange2.m_fRange.m_Min >= 0f && num < nKMAccumStateChange2.m_fRange.m_Min) || (nKMAccumStateChange2.m_fRange.m_Max >= 0f && num > nKMAccumStateChange2.m_fRange.m_Max))
					{
						continue;
					}
				}
				value.m_StateCount = 0;
				value.m_fCountCoolTimeNow = value.m_fCountCoolTimeMax;
				value.m_fMainCoolTimeNow = value.m_fMainCoolTimeMax;
				if (!IsDyingOrDie())
				{
					if (m_TargetUnit != null && m_TargetUnit.IsAirUnit() && nKMAccumStateChange2.m_AirTargetStateName.Length > 1)
					{
						StateChange(nKMAccumStateChange2.m_AirTargetStateName);
					}
					else
					{
						StateChange(nKMAccumStateChange2.m_TargetStateName);
					}
				}
				return;
			}
		}
	}

	public virtual void ChangeAnimSpeed(float fAnimSpeed = 0f)
	{
		if (fAnimSpeed <= 0f)
		{
			fAnimSpeed = m_UnitFrameData.m_fAnimSpeedOrg;
		}
		else
		{
			m_UnitFrameData.m_fAnimSpeedOrg = fAnimSpeed;
		}
		if (m_UnitStateNow.m_bAnimSpeedFix)
		{
			return;
		}
		if (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_ATTACK || m_UnitStateNow.m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_SKILL)
		{
			if (!m_UnitStateNow.m_bNotUseAttackSpeedStat)
			{
				fAnimSpeed += fAnimSpeed * m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_ATTACK_SPEED_RATE);
			}
		}
		else if (m_UnitStateNow.m_bRun)
		{
			fAnimSpeed += fAnimSpeed * m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_MOVE_SPEED_RATE);
		}
		m_UnitFrameData.m_fAnimSpeed = fAnimSpeed;
	}

	public void SetStateCoolTime(Dictionary<int, float> dicStateCoolTime)
	{
		if (dicStateCoolTime == null)
		{
			return;
		}
		Dictionary<int, NKMStateCoolTime>.Enumerator enumerator = m_dicStateCoolTime.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMStateCoolTime value = enumerator.Current.Value;
			m_NKMGame.GetObjectPool().CloseObj(value);
		}
		m_dicStateCoolTime.Clear();
		foreach (KeyValuePair<int, float> item in dicStateCoolTime)
		{
			if (item.Value > 0f)
			{
				NKMStateCoolTime nKMStateCoolTime = (NKMStateCoolTime)m_NKMGame.GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMStateCoolTime);
				nKMStateCoolTime.m_CoolTime = item.Value;
				m_dicStateCoolTime.Add(item.Key, nKMStateCoolTime);
			}
		}
	}

	public void SetStateCoolTime(string stateName, bool bMax, float ratio = 1f)
	{
		NKMUnitState unitState = GetUnitState(stateName);
		SetStateCoolTime(unitState, bMax, ratio);
	}

	protected void SetStateCoolTime(NKMUnitState cNKMUnitState, bool bMax, float ratio = 1f)
	{
		NKMUnitSkillTemplet stateSkill = GetStateSkill(cNKMUnitState);
		if (cNKMUnitState != null && (stateSkill != null || cNKMUnitState.m_StateCoolTime.m_Max > 0f))
		{
			float fInputTime = ((stateSkill != null && stateSkill.m_fCooltimeSecond > 0f) ? stateSkill.m_fCooltimeSecond : ((!bMax) ? cNKMUnitState.m_StateCoolTime.GetRandom() : cNKMUnitState.m_StateCoolTime.m_Max));
			fInputTime = GetCoolTimeReducedByOperator(fInputTime);
			fInputTime *= ratio;
			SetStateCoolTime(cNKMUnitState, fInputTime);
		}
	}

	public void SetStateCoolTimeAdd(string stateName, float fAddCool)
	{
		NKMUnitState unitState = GetUnitState(stateName);
		if (unitState != null)
		{
			SetStateCoolTimeAdd(unitState, fAddCool);
		}
	}

	public void SetStateCoolTimeAdd(NKMUnitState cNKMUnitState, float fAddCool)
	{
		float stateMaxCoolTime = GetStateMaxCoolTime(cNKMUnitState.m_StateName);
		float stateCoolTime = GetStateCoolTime(cNKMUnitState.m_StateName);
		stateCoolTime += fAddCool;
		if (stateCoolTime > stateMaxCoolTime)
		{
			stateCoolTime = stateMaxCoolTime;
		}
		if (stateCoolTime < 0f)
		{
			stateCoolTime = 0f;
		}
		SetStateCoolTime(cNKMUnitState, stateCoolTime);
	}

	protected void SetStateCoolTime(NKMUnitState cNKMUnitState, float fCoolTime)
	{
		if (!m_dicStateCoolTime.ContainsKey(cNKMUnitState.m_StateID))
		{
			if (fCoolTime > 0f)
			{
				NKMStateCoolTime nKMStateCoolTime = (NKMStateCoolTime)m_NKMGame.GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMStateCoolTime);
				nKMStateCoolTime.m_CoolTime = fCoolTime;
				m_dicStateCoolTime.Add(cNKMUnitState.m_StateID, nKMStateCoolTime);
			}
		}
		else
		{
			m_dicStateCoolTime[cNKMUnitState.m_StateID].m_CoolTime = fCoolTime;
		}
		for (int i = 0; i < cNKMUnitState.m_listCoolTimeLink.Count; i++)
		{
			NKMUnitState unitState = GetUnitState(cNKMUnitState.m_listCoolTimeLink[i]);
			if (unitState == null)
			{
				continue;
			}
			if (!m_dicStateCoolTime.ContainsKey(unitState.m_StateID))
			{
				if (fCoolTime > 0f)
				{
					NKMStateCoolTime nKMStateCoolTime2 = (NKMStateCoolTime)m_NKMGame.GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMStateCoolTime);
					nKMStateCoolTime2.m_CoolTime = fCoolTime;
					m_dicStateCoolTime.Add(unitState.m_StateID, nKMStateCoolTime2);
				}
			}
			else
			{
				m_dicStateCoolTime[unitState.m_StateID].m_CoolTime = fCoolTime;
			}
		}
	}

	public float GetStateMaxCoolTime(string stateName)
	{
		NKMUnitState unitState = GetUnitState(stateName);
		if (unitState == null)
		{
			return 0f;
		}
		if (m_dicStateMaxCoolTime.ContainsKey(unitState.m_StateID))
		{
			return m_dicStateMaxCoolTime[unitState.m_StateID];
		}
		NKMUnitSkillTemplet stateSkill = GetStateSkill(unitState);
		float fInputTime;
		if (stateSkill != null)
		{
			fInputTime = stateSkill.m_fCooltimeSecond;
			if (0f < unitState.m_StateCoolTime.m_Max && unitState.m_StateCoolTime.m_Max < stateSkill.m_fCooltimeSecond)
			{
				Log.Warn("unitID: " + GetUnitTemplet().m_UnitTempletBase.m_UnitStrID + ", 스테이트 " + unitState.m_StateName + "의 쿨타임보다 해당 스테이트 스킬 템플릿 " + stateSkill.m_strID + "의 쿨타임이 깁니다. 스킬 템플릿의 쿨타임을 사용합니다. 혹시 쿨타임 설정을 빠트리셨나요?", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 2498);
			}
			else if (stateSkill.m_fCooltimeSecond.IsNearlyZero())
			{
				Log.Warn("unitID: " + GetUnitTemplet().m_UnitTempletBase.m_UnitStrID + ", 스테이트 " + unitState.m_StateName + "에 지정된 스킬 " + stateSkill.m_strID + "의 쿨타임이 0입니다.혹시 쿨타임 설정을 빠트리셨나요?", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 2502);
			}
		}
		else
		{
			fInputTime = unitState.m_StateCoolTime.m_Max;
		}
		fInputTime = GetCoolTimeReducedByOperator(fInputTime);
		m_dicStateMaxCoolTime.Add(unitState.m_StateID, fInputTime);
		for (int i = 0; i < unitState.m_listCoolTimeLink.Count; i++)
		{
			NKMUnitState unitState2 = GetUnitState(unitState.m_listCoolTimeLink[i]);
			if (unitState2 != null && !m_dicStateMaxCoolTime.ContainsKey(unitState2.m_StateID))
			{
				m_dicStateMaxCoolTime.Add(unitState2.m_StateID, fInputTime);
			}
		}
		return fInputTime;
	}

	protected void ProcessCoolTime()
	{
		if (IsATeam() && m_NKMGame.GetGameData().m_TeamASupply <= 0)
		{
			return;
		}
		Dictionary<int, NKMStateCoolTime>.Enumerator enumerator = m_dicStateCoolTime.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMStateCoolTime value = enumerator.Current.Value;
			if (value.m_CoolTime <= 0f)
			{
				continue;
			}
			float num = 0f;
			if (m_NKMGame.IsATeam(m_UnitDataGame.m_NKM_TEAM_TYPE))
			{
				num = m_NKMGame.GetCoolTimeReduceTeamA();
			}
			else if (m_NKMGame.IsBTeam(m_UnitDataGame.m_NKM_TEAM_TYPE))
			{
				num = m_NKMGame.GetCoolTimeReduceTeamB();
			}
			bool flag = false;
			float num2;
			switch (GetUnitTemplet().GetAttackStateType(enumerator.Current.Key))
			{
			case NKM_UNIT_ATTACK_STATE_TYPE.ATTACK:
			case NKM_UNIT_ATTACK_STATE_TYPE.AIR_ATTACK:
				num2 = (IsMonster() ? 0f : m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_ATTACK_SPEED_RATE));
				break;
			case NKM_UNIT_ATTACK_STATE_TYPE.SKILL:
			case NKM_UNIT_ATTACK_STATE_TYPE.AIR_SKILL:
			case NKM_UNIT_ATTACK_STATE_TYPE.HYPER:
			case NKM_UNIT_ATTACK_STATE_TYPE.AIR_HYPER:
				num2 = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE);
				if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE))
				{
					flag = true;
				}
				if (GetUnitTemplet().m_UnitTempletBase != null && GetUnitTemplet().m_UnitTempletBase.StopDefaultCoolTime)
				{
					continue;
				}
				break;
			default:
				num2 = 0f;
				break;
			}
			for (int i = 0; i < m_listShipSkillStateID.Count; i++)
			{
				if (m_listShipSkillStateID[i] == enumerator.Current.Key)
				{
					num2 = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE);
					if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE))
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				float num3 = 1f + num + num2;
				if (num3 < 0f)
				{
					num3 = 0f;
				}
				value.m_CoolTime -= m_DeltaTime * num3;
				if (value.m_CoolTime <= 0f)
				{
					value.m_CoolTime = 0f;
				}
			}
		}
	}

	protected void DecreaseSkillStateCoolTime(float decreaseValue)
	{
		if (decreaseValue <= 0f || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE))
		{
			return;
		}
		Dictionary<int, NKMStateCoolTime>.Enumerator enumerator = m_dicStateCoolTime.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMStateCoolTime value = enumerator.Current.Value;
			if (value.m_CoolTime <= 0f)
			{
				continue;
			}
			NKM_UNIT_ATTACK_STATE_TYPE attackStateType = GetUnitTemplet().GetAttackStateType(enumerator.Current.Key);
			if ((uint)(attackStateType - 5) <= 3u)
			{
				value.m_CoolTime -= decreaseValue;
			}
			for (int i = 0; i < m_listShipSkillStateID.Count; i++)
			{
				if (m_listShipSkillStateID[i] == enumerator.Current.Key)
				{
					value.m_CoolTime -= decreaseValue;
				}
			}
			if (value.m_CoolTime <= 0f)
			{
				value.m_CoolTime = 0f;
			}
		}
	}

	protected float RegenHPThisFrame(float fRegenRate, bool bIgnoreMaxHP = false)
	{
		float statFinal = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP);
		float num = statFinal * fRegenRate * m_DeltaTime;
		if (!bIgnoreMaxHP && m_UnitSyncData.GetHP() >= statFinal && num > 0f)
		{
			num = 0f;
		}
		if (m_UnitSyncData.GetHP() <= 0f && num < 0f)
		{
			num = 0f;
		}
		bool flag = false;
		if (GetUnitFrameData().m_bInvincible || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE))
		{
			flag = true;
		}
		if (m_UnitStateNow != null && m_UnitStateNow.m_bInvincibleState)
		{
			flag = true;
		}
		if (num < 0f && flag)
		{
			num = 0f;
		}
		if (GetUnitFrameData().m_fHealFeedback > 0f)
		{
			num = 0f;
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IRONWALL))
		{
			num = 0f;
		}
		if (num > 0f && HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOHEAL))
		{
			num = 0f;
		}
		return num;
	}

	protected virtual void StateUpdate()
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		bool flag = m_NKMGame.GetWorldStopTime() > 0f;
		m_TargetUnit = GetTargetUnit();
		if (m_TargetUnit != null && m_TargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DYING && m_TargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			GetUnitFrameData().m_LastTargetPosX = m_TargetUnit.GetUnitSyncData().m_PosX;
			GetUnitFrameData().m_LastTargetPosZ = m_TargetUnit.GetUnitSyncData().m_PosZ;
			GetUnitFrameData().m_LastTargetJumpYPos = m_TargetUnit.GetUnitSyncData().m_JumpYPos;
		}
		m_SubTargetUnit = GetTargetUnit(m_UnitSyncData.m_SubTargetUID, m_UnitTemplet.m_SubTargetFindData);
		m_dicKillFeedBackGameUnitUID.Clear();
		if (m_UnitStateNow.m_bRun && m_UnitFrameData.m_fSpeedX < m_UnitTemplet.m_SpeedRun * m_UnitStateNow.m_fRunSpeedRate)
		{
			float num = m_UnitTemplet.m_SpeedRun * m_UnitStateNow.m_fRunSpeedRate;
			num += num * m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_MOVE_SPEED_RATE);
			m_UnitFrameData.m_fSpeedX = num;
		}
		if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			if (!flag)
			{
				HPProcess();
			}
			if (m_TargetUnit == null || m_TargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || m_TargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
			{
				m_UnitFrameData.m_fTargetLostDurationTime += m_DeltaTime;
			}
			else
			{
				m_UnitFrameData.m_fTargetLostDurationTime = 0f;
			}
			if (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_START && m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DAMAGE && !m_UnitStateNow.m_bNoAI)
			{
				ProcessAI();
			}
		}
		else if (!flag && m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			if (m_UnitSyncData.GetHP() > 1f)
			{
				float statFinal = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP);
				float num2 = RegenHPThisFrame(m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP_REGEN_RATE), bIgnoreMaxHP: true);
				if (num2 > 0f && m_UnitFrameData.m_fHealTransfer > 0f)
				{
					float num3 = num2 * GetUnitFrameData().m_fHealTransfer;
					num2 -= num3;
					if (num2 > 0f && m_UnitSyncData.GetHP() + num2 > GetMaxHP())
					{
						float num4 = GetMaxHP() - GetUnitSyncData().GetHP();
						num3 = num3 * num4 / num2;
						num2 = num4;
					}
					NKMUnit unit = m_NKMGame.GetUnit(m_UnitFrameData.m_HealTransferMasterGameUnitUID);
					if (unit != null && unit.GetUnitFrameData().m_fHealFeedback <= 0f)
					{
						float expectedHealAmount = unit.GetExpectedHealAmount(num3, m_UnitFrameData.m_HealTransferMasterGameUnitUID);
						unit.GetUnitSyncData().SetHP(unit.GetNowHP() + expectedHealAmount);
					}
				}
				float val = num2 + m_UnitSyncData.GetHP();
				m_UnitSyncData.SetHP(Math.Min(val, statFinal));
			}
			if (m_UnitSyncData.GetHP() < 1f)
			{
				m_UnitSyncData.SetHP(1f);
			}
		}
		if (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DAMAGE && m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_START && m_UnitStateNow.m_bSeeTarget)
		{
			SeeTarget();
		}
		if (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DAMAGE && m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_START && m_UnitStateNow.m_bSeeMoreEnemy)
		{
			SeeMoreEnemy();
		}
		GetUnitFrameData().m_fLiveTime += m_DeltaTime;
		if (!flag)
		{
			ProcessCoolTime();
		}
		ProcessStateEvents(bStateEnd: false);
		if (!flag)
		{
			ProcessInvokedEventReaction();
		}
		ProcessAutoShipSkill();
		ProcessDangerCharge();
		AccumStateProcessUpdate();
		if (!flag && m_NKMGame.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY)
		{
			ProcessStatusApply();
			ProcessBuff();
			ProcessStatusAffect();
			ProcessTriggerRepeat();
		}
		ProcessStaticBuff();
		ProcessLeaguePvpRageBuff();
		ProcessLeaguePvpDeadlineBuff();
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
		m_UnitFrameData.m_fHitLightTime -= m_DeltaTime;
		if (m_UnitFrameData.m_fHitLightTime < 0f)
		{
			m_UnitFrameData.m_fHitLightTime = 0f;
		}
	}

	protected virtual void ProcessStateEvents(bool bStateEnd)
	{
		if (m_UnitStateNow != null)
		{
			ProcessStateEventServerOnly(m_UnitStateNow.m_listNKMEventFindTarget, bStateEnd);
		}
		ProcessEventText(bStateEnd);
		ProcessEventSpeed(bStateEnd);
		ProcessEventSpeedX(bStateEnd);
		ProcessEventSpeedY(bStateEnd);
		ProcessEventMove(bStateEnd);
		ProcessEventAttack(bStateEnd);
		ProcessEventStopTime(bStateEnd);
		ProcessEventInvincible(bStateEnd);
		ProcessEventInvincibleGlobal(bStateEnd);
		ProcessEventSuperArmor(bStateEnd);
		ProcessEventSound(bStateEnd);
		ProcessEventColor(bStateEnd);
		ProcessEventCameraCrash(bStateEnd);
		ProcessEventCameraMove(bStateEnd);
		ProcessEventFadeWorld(bStateEnd);
		ProcessEventDissolve(bStateEnd);
		ProcessEventMotionBlur(bStateEnd);
		ProcessEventEffect(bStateEnd);
		ProcessEventHyperSkillCutIn(bStateEnd);
		ProcessEventDamageEffect(bStateEnd);
		ProcessEventDEStateChange(bStateEnd);
		ProcessEventGameSpeed(bStateEnd);
		ProcessEventAnimSpeed(bStateEnd);
		ProcessEventBuff(bStateEnd);
		ProcessStateEventServerOnly(m_UnitStateNow.m_listNKMEventStatus, bStateEnd);
		ProcessEventRespawn(bStateEnd);
		ProcessEventUnitChange(bStateEnd);
		ProcessEventDie(bStateEnd);
		ProcessEventChangeState(bStateEnd);
		ProcessEventAgro(bStateEnd);
		ProcessEventHeal(bStateEnd);
		ProcessEventStun(bStateEnd);
		ProcessEventCost(bStateEnd);
		ProcessEventDispel(bStateEnd);
		ProcessEventCatch(bStateEnd);
		ProcessEventChangeCooltime(bStateEnd);
		ProcessStateEventServerOnly(m_UnitStateNow.m_listNKMEventTrigger, bStateEnd);
		ProcessStateEventServerOnly(m_UnitStateNow.m_listNKMEventChangeRemainTime, bStateEnd);
		ProcessStateEventServerOnly(m_UnitStateNow.m_listNKMEventVariable, bStateEnd);
		ProcessStateEventServerOnly(m_UnitStateNow.m_listNKMEventConsume, bStateEnd);
		ProcessStateEventServerOnly(m_UnitStateNow.m_listNKMEventTriggerBranch, bStateEnd);
		ProcessStateEventServerOnly(m_UnitStateNow.m_listNKMEventReaction, bStateEnd);
		ProcessInvokedTrigger(bStateEnd);
	}

	protected virtual void StateEvent()
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		if (m_NKMGame.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH && m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY && m_UnitFrameData.m_fAnimTime >= m_UnitFrameData.m_fAnimTimeMax)
		{
			StateChangeToASTAND(bForceChange: false);
		}
		else if (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_DIE && m_UnitFrameData.m_fStateTime >= GetUnitTemplet().m_fDieCompleteTime)
		{
			if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE)
			{
				m_PushSyncData = true;
				SetDie();
			}
		}
		else if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING && m_UnitStateNow.m_AnimEndDyingState.Length > 1 && m_UnitFrameData.m_fAnimTime >= m_UnitFrameData.m_fAnimTimeMax)
		{
			StateChange(m_UnitStateNow.m_AnimEndDyingState, bForceChange: true, bImmediate: false, NKM_STATE_CHANGE_PRIORITY.NSCP_DIE);
		}
		else if (m_UnitFrameData.m_bFootOnLand && m_UnitSyncData.GetHP().IsNearlyZero() && m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DIE && m_UnitFrameData.m_fAnimTime >= m_UnitFrameData.m_fAnimTimeMax)
		{
			StateChange(m_UnitTemplet.m_DyingState, bForceChange: true, bImmediate: false, NKM_STATE_CHANGE_PRIORITY.NSCP_DIE);
		}
		else
		{
			if (StateEvent_Phase())
			{
				return;
			}
			if (m_UnitStateNow.IsBadStatusState && !HasStatus(m_UnitStateNow.m_StatusEffectType))
			{
				StateChangeToASTAND(bForceChange: false);
				return;
			}
			bool flag = m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DAMAGE && m_UnitFrameData.m_bFootOnLand;
			bool flag2 = m_UnitSyncData.GetHP() > 0f && m_UnitStateNow.m_NKMEventUnitChange == null;
			if (flag2)
			{
				if (StatusStateChangeRequired(NKM_UNIT_STATUS_EFFECT.NUSE_FREEZE))
				{
					StateChange("USN_DAMAGE_FREEZE", bForceChange: false);
					return;
				}
				if (StatusStateChangeRequired(NKM_UNIT_STATUS_EFFECT.NUSE_HOLD))
				{
					StateChange("USN_DAMAGE_HOLD", bForceChange: false);
					return;
				}
			}
			if (flag2 && flag)
			{
				if (StatusStateChangeRequired(NKM_UNIT_STATUS_EFFECT.NUSE_STUN))
				{
					if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE))
					{
						if (!IsAirUnit())
						{
							if (m_UnitFrameData.m_bFootOnLand)
							{
								StateChange("USN_DAMAGE_A");
							}
							else
							{
								StateChange("USN_DAMAGE_AIR_DOWN");
							}
						}
						else
						{
							StateChange("USN_DAMAGE_A");
						}
						RemoveStatus(NKM_UNIT_STATUS_EFFECT.NUSE_STUN);
					}
					else
					{
						StateChange("USN_DAMAGE_STUN", bForceChange: false);
					}
					return;
				}
				if (StatusStateChangeRequired(NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP))
				{
					if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE))
					{
						if (!IsAirUnit())
						{
							if (m_UnitFrameData.m_bFootOnLand)
							{
								StateChange("USN_DAMAGE_A");
							}
							else
							{
								StateChange("USN_DAMAGE_AIR_DOWN");
							}
						}
						else
						{
							StateChange("USN_DAMAGE_A");
						}
						RemoveStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP);
					}
					else
					{
						StateChange("USN_DAMAGE_SLEEP", bForceChange: false);
					}
					return;
				}
				if (StatusStateChangeRequired(NKM_UNIT_STATUS_EFFECT.NUSE_FEAR))
				{
					if (!RunAllowed())
					{
						StateChange("USN_DAMAGE_FEAR_NOMOVE", bForceChange: false);
					}
					else
					{
						StateChange("USN_DAMAGE_FEAR", bForceChange: false);
					}
					return;
				}
			}
			if (m_UnitStateNow.m_StatusEffectType == NKM_UNIT_STATUS_EFFECT.NUSE_FEAR && StateEvent_NUSE_FEAR())
			{
				return;
			}
			if (m_UnitStateNow.m_AnimTimeChangeStateTime >= 0f && m_UnitFrameData.m_fAnimTime >= m_UnitStateNow.m_AnimTimeChangeStateTime)
			{
				StateChange(m_UnitStateNow.m_AnimTimeChangeState);
				return;
			}
			if (m_UnitStateNow.m_AnimTimeRateChangeStateTime >= 0f && m_UnitFrameData.m_fAnimTimeMax > 0f && m_UnitFrameData.m_fAnimTime / m_UnitFrameData.m_fAnimTimeMax >= m_UnitStateNow.m_AnimTimeRateChangeStateTime)
			{
				StateChange(m_UnitStateNow.m_AnimTimeRateChangeState);
				return;
			}
			if (m_UnitStateNow.m_StateTimeChangeStateTime >= 0f && m_UnitFrameData.m_fStateTime >= m_UnitStateNow.m_StateTimeChangeStateTime)
			{
				StateChange(m_UnitStateNow.m_StateTimeChangeState);
				return;
			}
			if (m_UnitStateNow.m_TargetDistOverChangeState.Length > 1 && m_TargetUnit != null && Math.Abs(m_TargetUnit.GetUnitSyncData().m_PosX - m_UnitSyncData.m_PosX) - (m_TargetUnit.GetUnitTemplet().m_UnitSizeX * 0.5f + m_UnitTemplet.m_UnitSizeX * 0.5f) > m_UnitStateNow.m_TargetDistOverChangeStateDist)
			{
				StateChange(m_UnitStateNow.m_TargetDistOverChangeState);
				return;
			}
			if (m_UnitStateNow.m_TargetDistLessChangeState.Length > 1 && m_TargetUnit != null && Math.Abs(m_TargetUnit.GetUnitSyncData().m_PosX - m_UnitSyncData.m_PosX) - (m_TargetUnit.GetUnitTemplet().m_UnitSizeX * 0.5f + m_UnitTemplet.m_UnitSizeX * 0.5f) < m_UnitStateNow.m_TargetDistLessChangeStateDist)
			{
				StateChange(m_UnitStateNow.m_TargetDistLessChangeState);
				return;
			}
			if (m_UnitStateNow.m_TargetLostOrDieState.Length > 1 && (m_TargetUnit == null || m_TargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || m_TargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE))
			{
				if (m_UnitFrameData.m_fTargetLostDurationTime >= m_UnitStateNow.m_TargetLostOrDieStateDurationTime)
				{
					StateChange(m_UnitStateNow.m_TargetLostOrDieState);
					return;
				}
				m_UnitFrameData.m_fTargetLostDurationTime += m_DeltaTime;
			}
			if (m_UnitStateNow.m_AnimEndChangeState.Length > 1 && !m_UnitStateNow.m_bAnimLoop && m_UnitStateNow.m_AnimEndChangeStatePlayCount <= m_UnitFrameData.m_AnimPlayCount)
			{
				StateChange(m_UnitStateNow.m_AnimEndChangeState);
				return;
			}
			if (m_UnitStateNow.m_ChangeRightTrueState.Length > 1 && m_bRightOrg != m_UnitSyncData.m_bRight && m_UnitSyncData.m_bRight)
			{
				StateChange(m_UnitStateNow.m_ChangeRightTrueState);
				return;
			}
			if (m_UnitStateNow.m_ChangeRightFalseState.Length > 1 && m_bRightOrg != m_UnitSyncData.m_bRight && !m_UnitSyncData.m_bRight)
			{
				StateChange(m_UnitStateNow.m_ChangeRightFalseState);
				return;
			}
			if (m_UnitStateNow.m_AirTargetThisFrameChangeState.Length > 1 && GetUnitFrameData().m_bTargetChangeThisFrame && m_TargetUnit != null && !m_TargetUnit.IsDyingOrDie() && m_TargetUnit.IsAirUnit())
			{
				StateChange(m_UnitStateNow.m_AirTargetThisFrameChangeState);
				return;
			}
			if (m_UnitStateNow.m_LandTargetThisFrameChangeState.Length > 1 && GetUnitFrameData().m_bTargetChangeThisFrame && m_TargetUnit != null && !m_TargetUnit.IsDyingOrDie() && !m_TargetUnit.IsAirUnit())
			{
				StateChange(m_UnitStateNow.m_LandTargetThisFrameChangeState);
				return;
			}
			if (m_UnitFrameData.m_bAnimPlayCountAddThisFrame)
			{
				if (!string.IsNullOrEmpty(m_UnitStateNow.m_AirTargetLoopChangeState) && m_TargetUnit != null && !m_TargetUnit.IsDyingOrDie() && m_TargetUnit.IsAirUnit())
				{
					StateChange(m_UnitStateNow.m_AirTargetLoopChangeState);
					return;
				}
				if (!string.IsNullOrEmpty(m_UnitStateNow.m_LandTargetLoopChangeState) && m_TargetUnit != null && !m_TargetUnit.IsDyingOrDie() && !m_TargetUnit.IsAirUnit())
				{
					StateChange(m_UnitStateNow.m_LandTargetLoopChangeState);
					return;
				}
			}
			if (m_UnitStateNow.m_ChangeRightState.Length > 1 && m_bRightOrg != m_UnitSyncData.m_bRight)
			{
				StateChange(m_UnitStateNow.m_ChangeRightState);
			}
			else if (m_UnitStateNow.m_FootOnLandChangeState.Length > 1 && m_UnitFrameData.m_bFootOnLand)
			{
				StateChange(m_UnitStateNow.m_FootOnLandChangeState);
			}
			else if (m_UnitStateNow.m_FootOffLandChangeState.Length > 1 && !m_UnitFrameData.m_bFootOnLand)
			{
				StateChange(m_UnitStateNow.m_FootOffLandChangeState);
			}
			else if (m_UnitStateNow.m_AnimEndFootOnLandChangeState.Length > 1 && m_UnitFrameData.m_fAnimTime >= m_UnitFrameData.m_fAnimTimeMax && m_UnitFrameData.m_bFootOnLand)
			{
				StateChange(m_UnitStateNow.m_AnimEndFootOnLandChangeState);
			}
			else if (m_UnitStateNow.m_AnimEndFootOffLandChangeState.Length > 1 && m_UnitFrameData.m_fAnimTime >= m_UnitFrameData.m_fAnimTimeMax && !m_UnitFrameData.m_bFootOnLand)
			{
				StateChange(m_UnitStateNow.m_AnimEndFootOffLandChangeState);
			}
			else if (m_UnitStateNow.m_SpeedYPositiveChangeState.Length > 1 && m_UnitFrameData.m_fSpeedY + m_UnitFrameData.m_fDamageSpeedJumpY > 0f)
			{
				StateChange(m_UnitStateNow.m_SpeedYPositiveChangeState);
			}
			else if (m_UnitStateNow.m_SpeedY0NegativeChangeState.Length > 1 && m_UnitFrameData.m_fSpeedY + m_UnitFrameData.m_fDamageSpeedJumpY <= 0f)
			{
				StateChange(m_UnitStateNow.m_SpeedY0NegativeChangeState);
			}
			else if (m_UnitStateNow.m_DamagedChangeState.Length > 1 && m_UnitFrameData.m_fDamageBeforeFrame > 0f && (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_START || m_UnitFrameData.m_fStateTime > 0.5f) && m_StateNameNext.Length <= 1 && m_StateNameNextChange.Length <= 1)
			{
				StateChange(m_UnitStateNow.m_DamagedChangeState);
			}
			else if (m_UnitStateNow.m_MapPosOverState.Length > 1 && m_UnitStateNow.m_MapPosOverStatePos <= m_NKMGame.GetMapTemplet().GetMapFactor(GetUnitSyncData().m_PosX, m_NKMGame.IsATeam(GetUnitDataGame().m_NKM_TEAM_TYPE)))
			{
				StateChange(m_UnitStateNow.m_MapPosOverState);
			}
			else if (GetUnitTemplet().m_MapPosOverState.Length > 1 && GetUnitTemplet().m_MapPosOverStatePos <= m_NKMGame.GetMapTemplet().GetMapFactor(GetUnitSyncData().m_PosX, m_NKMGame.IsATeam(GetUnitDataGame().m_NKM_TEAM_TYPE)))
			{
				StateChange(GetUnitTemplet().m_MapPosOverState);
			}
			else if (!m_UnitStateNow.m_bNoStateTypeEvent && (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_ASTAND || !StateEvent_NUST_ASTAND()) && (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_MOVE || !StateEvent_NUST_MOVE()) && (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_ATTACK || m_UnitStateNow.m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_SKILL))
			{
				StateEvent_NUST_ATTACK();
			}
		}
		bool StatusStateChangeRequired(NKM_UNIT_STATUS_EFFECT type)
		{
			if (HasStatus(type) && m_UnitStateNow != null)
			{
				return m_UnitStateNow.m_StatusEffectType != type;
			}
			return false;
		}
	}

	protected virtual bool StateEvent_Phase(bool bRespawnTime = false)
	{
		for (int i = 0; i < GetUnitTemplet().m_listPhaseChangeData.Count; i++)
		{
			NKMPhaseChangeData nKMPhaseChangeData = GetUnitTemplet().m_listPhaseChangeData[i];
			if (nKMPhaseChangeData == null || (!bRespawnTime && (m_UnitStateNow == null || m_UnitStateNow.m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_START || m_UnitStateNow.m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_SKILL || m_UnitStateNow.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_SKILL || m_UnitStateNow.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER)) || GetHPRate() <= 0f || IsDyingOrDie() || !CheckEventCondition(nKMPhaseChangeData.m_Condition) || nKMPhaseChangeData.m_TargetPhase <= GetUnitFrameData().m_PhaseNow)
			{
				continue;
			}
			float targetHP = nKMPhaseChangeData.GetTargetHP(GetMaxHP());
			if ((!(targetHP > 0f) || !(targetHP < GetUnitSyncData().GetHP())) && (!(nKMPhaseChangeData.m_fChangeConditionTime > 0f) || !(nKMPhaseChangeData.m_fChangeConditionTime > GetUnitFrameData().m_fLiveTime)) && (nKMPhaseChangeData.m_ChangeConditionMyKill <= 0 || nKMPhaseChangeData.m_ChangeConditionMyKill <= GetUnitFrameData().m_KillCount) && (!nKMPhaseChangeData.m_bChangeConditionImmortalStart || GetUnitFrameData().m_bImmortalStart))
			{
				for (int j = 0; j < nKMPhaseChangeData.m_listChangeCoolTime.Count; j++)
				{
					NKMPhaseChangeCoolTime nKMPhaseChangeCoolTime = nKMPhaseChangeData.m_listChangeCoolTime[j];
					SetStateCoolTime(nKMPhaseChangeCoolTime.m_StateName, bMax: false, nKMPhaseChangeCoolTime.m_fCoolTime);
				}
				if (nKMPhaseChangeData.m_ChangeStateName.Length > 1 && !bRespawnTime)
				{
					StateChange(nKMPhaseChangeData.m_ChangeStateName, bForceChange: true, bImmediate: false, NKM_STATE_CHANGE_PRIORITY.NSCP_FORCE);
				}
				GetUnitFrameData().m_PhaseNow = nKMPhaseChangeData.m_TargetPhase;
				if (!bRespawnTime)
				{
					ApplyStatusTime(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_CC, 0.1f, this, bForceOverwrite: false, bServerOnly: true, bImmediate: true);
					ApplyStatusTime(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE, 0.1f, this, bForceOverwrite: false, bServerOnly: true, bImmediate: true);
					return true;
				}
			}
		}
		return false;
	}

	public float GetPhaseDamageLimit(out bool cutDamage)
	{
		cutDamage = false;
		float num = 0f;
		for (int i = 0; i < GetUnitTemplet().m_listPhaseChangeData.Count; i++)
		{
			NKMPhaseChangeData nKMPhaseChangeData = GetUnitTemplet().m_listPhaseChangeData[i];
			if (nKMPhaseChangeData != null && m_UnitStateNow != null && !IsDyingOrDie() && CheckEventCondition(nKMPhaseChangeData.m_Condition) && nKMPhaseChangeData.m_TargetPhase > GetUnitFrameData().m_PhaseNow)
			{
				float targetHP = nKMPhaseChangeData.GetTargetHP(GetMaxHP());
				if (!(targetHP <= 0f) && num < targetHP)
				{
					num = targetHP;
					cutDamage = nKMPhaseChangeData.m_bCutHpDamage;
				}
			}
		}
		return num;
	}

	public float GetExpectedHPAfterDamage(float incomingDamage)
	{
		float num = GetHP() - incomingDamage;
		bool cutDamage;
		float phaseDamageLimit = GetPhaseDamageLimit(out cutDamage);
		if (phaseDamageLimit > 0f)
		{
			if (cutDamage && num < phaseDamageLimit)
			{
				num = phaseDamageLimit - 1f;
			}
			if (num < phaseDamageLimit && !IsMonster() && num <= 0f)
			{
				num = 1f;
			}
		}
		if (num <= 0f && HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMORTAL))
		{
			m_UnitFrameData.m_bImmortalStart = true;
			num = 1f;
		}
		return num;
	}

	protected virtual bool StateEvent_NUST_ASTAND()
	{
		if ((int)m_NKMGame.GetGameRuntimeData().m_NKM_GAME_STATE < 3)
		{
			return false;
		}
		if (m_UnitTemplet.m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && m_UnitSyncData.m_TargetUID <= 0)
		{
			float fDist = 99999f;
			bool bRight = m_UnitSyncData.m_bRight;
			NKMUnit idleDirRight = GetIdleDirRight(ref bRight, ref fDist);
			if (m_UnitStateNow != null && !m_UnitStateNow.m_bNoChangeRight)
			{
				m_UnitSyncData.m_bRight = bRight;
			}
			if (idleDirRight == null && GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE != NKM_FIND_TARGET_TYPE.NFTT_NO)
			{
				StateChangeToASTAND(bForceChange: false);
				return true;
			}
			if (fDist < GetUnitDataGame().m_fTargetNearRange)
			{
				StateChangeToASTAND(bForceChange: false);
				return true;
			}
			if (m_NKMGame.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH && m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY)
			{
				StateChangeToASTAND(bForceChange: false);
				return true;
			}
			if ((m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataA.m_bAIDisable && m_NKMGame.IsATeam(GetUnitDataGame().m_NKM_TEAM_TYPE)) || (m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataB.m_bAIDisable && m_NKMGame.IsBTeam(GetUnitDataGame().m_NKM_TEAM_TYPE)))
			{
				StateChangeToASTAND(bForceChange: false);
				return true;
			}
			if (RunAllowed())
			{
				StateChangeToRUN();
				return true;
			}
		}
		return false;
	}

	public NKMUnitSkillTemplet GetStateSkill(NKMUnitState cNKMUnitState)
	{
		if (GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL)
		{
			return null;
		}
		if (cNKMUnitState == null)
		{
			return null;
		}
		return m_UnitData.GetUnitSkillTempletByType(cNKMUnitState.m_NKM_SKILL_TYPE);
	}

	protected NKMUnit GetIdleDirRight(ref bool bRight, ref float fDist)
	{
		if (GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE == NKM_FIND_TARGET_TYPE.NFTT_NO)
		{
			return null;
		}
		NKMUnit nKMUnit = m_NKMGame.GetLiveBossUnit(!m_NKMGame.IsATeam(m_UnitDataGame.m_NKM_TEAM_TYPE));
		if (nKMUnit == null)
		{
			NKM_FIND_TARGET_TYPE nKM_FIND_TARGET_TYPE = GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE;
			if ((uint)(nKM_FIND_TARGET_TYPE - 4) <= 1u)
			{
				nKMUnit = m_NKMGame.GetLiveUnit(!m_NKMGame.IsATeam(m_UnitDataGame.m_NKM_TEAM_TYPE), IsAirUnit());
			}
			if (nKMUnit == null)
			{
				nKMUnit = m_NKMGame.GetLiveUnit(!m_NKMGame.IsATeam(m_UnitDataGame.m_NKM_TEAM_TYPE));
			}
		}
		if (nKMUnit != null)
		{
			fDist = Math.Abs(nKMUnit.GetUnitSyncData().m_PosX - GetUnitSyncData().m_PosX);
			if (GetUnitSyncData().m_PosX < nKMUnit.GetUnitSyncData().m_PosX)
			{
				bRight = true;
				return nKMUnit;
			}
			bRight = false;
			return nKMUnit;
		}
		return null;
	}

	protected virtual bool StateEvent_NUST_MOVE()
	{
		if (m_NKMGame.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH && m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			StateChangeToASTAND(bForceChange: false);
			return true;
		}
		if ((m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataA.m_bAIDisable && m_NKMGame.IsATeam(GetUnitDataGame().m_NKM_TEAM_TYPE)) || (m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataB.m_bAIDisable && m_NKMGame.IsBTeam(GetUnitDataGame().m_NKM_TEAM_TYPE)))
		{
			StateChangeToASTAND(bForceChange: false);
			return true;
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOMOVE))
		{
			StateChangeToASTAND(bForceChange: false);
			return true;
		}
		if (m_UnitTemplet.m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && GetUnitTemplet().m_fPatrolRange <= 0f)
		{
			if (m_UnitSyncData.m_TargetUID <= 0)
			{
				float fDist = 99999f;
				bool bRight = m_UnitSyncData.m_bRight;
				NKMUnit idleDirRight = GetIdleDirRight(ref bRight, ref fDist);
				if (m_UnitStateNow != null && !m_UnitStateNow.m_bNoChangeRight)
				{
					m_UnitSyncData.m_bRight = bRight;
				}
				if (idleDirRight == null && GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE != NKM_FIND_TARGET_TYPE.NFTT_NO)
				{
					StateChangeToASTAND(bForceChange: false);
					return true;
				}
				if (fDist < GetUnitDataGame().m_fTargetNearRange)
				{
					StateChangeToASTAND(bForceChange: false);
					return true;
				}
			}
			else
			{
				SeeTarget();
			}
		}
		return false;
	}

	protected virtual bool StateEvent_NUSE_FEAR()
	{
		if (m_UnitTemplet.m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			if (!RunAllowed() || !m_UnitStateNow.m_bRun)
			{
				return true;
			}
			bool bRight = m_UnitSyncData.m_bRight;
			NKMUnit liveBossUnit = m_NKMGame.GetLiveBossUnit(m_UnitDataGame.m_NKM_TEAM_TYPE);
			if (liveBossUnit != null)
			{
				bRight = GetUnitSyncData().m_PosX < liveBossUnit.GetUnitSyncData().m_PosX;
			}
			else
			{
				NKMUnit liveBossUnit2 = m_NKMGame.GetLiveBossUnit(!m_NKMGame.IsATeam(m_UnitDataGame.m_NKM_TEAM_TYPE));
				if (liveBossUnit2 != null)
				{
					bRight = GetUnitSyncData().m_PosX > liveBossUnit2.GetUnitSyncData().m_PosX;
				}
			}
			if (m_UnitStateNow != null && !m_UnitStateNow.m_bNoChangeRight)
			{
				m_UnitSyncData.m_bRight = bRight;
			}
			if (GetUnitTemplet().m_fPatrolRange > 0f && (Math.Abs(m_UnitDataGame.m_RespawnPosX - GetUnitSyncData().m_PosX) > GetUnitTemplet().m_fPatrolRange || GetUnitSyncData().m_PosX <= m_NKMGame.GetMapTemplet().GetUnitMinX() || GetUnitSyncData().m_PosX >= m_NKMGame.GetMapTemplet().GetUnitMaxX()))
			{
				StateChange("USN_DAMAGE_FEAR_NOMOVE");
				return true;
			}
		}
		return false;
	}

	protected virtual bool StateEvent_NUST_ATTACK()
	{
		if ((m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataA.m_bAIDisable && m_NKMGame.IsATeam(GetUnitDataGame().m_NKM_TEAM_TYPE)) || (m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataB.m_bAIDisable && m_NKMGame.IsBTeam(GetUnitDataGame().m_NKM_TEAM_TYPE)))
		{
			StateChangeToASTAND(bForceChange: false);
			return true;
		}
		if (m_UnitFrameData.m_fAnimTime >= m_UnitFrameData.m_fAnimTimeMax && !m_UnitStateNow.m_bAnimLoop && !AIAttack())
		{
			if (IsArriveTarget())
			{
				StateChangeToASTAND(bForceChange: false);
			}
			else if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOMOVE))
			{
				StateChangeToASTAND(bForceChange: false);
			}
			else
			{
				AIMove();
			}
		}
		return false;
	}

	private NKM_STATE_CHANGE_PRIORITY GetStateChangePriority(NKMUnitState state)
	{
		if (state.m_bInvincibleState)
		{
			return NKM_STATE_CHANGE_PRIORITY.NSCP_INVINCIBLE;
		}
		foreach (NKMEventInvincible item in state.m_listNKMEventInvincible)
		{
			if (item.m_fEventTimeMin <= 0f)
			{
				return NKM_STATE_CHANGE_PRIORITY.NSCP_INVINCIBLE;
			}
		}
		foreach (NKMEventInvincibleGlobal item2 in state.m_listNKMEventInvincibleGlobal)
		{
			if (item2.EventStartTime <= 0f)
			{
				return NKM_STATE_CHANGE_PRIORITY.NSCP_INVINCIBLE;
			}
		}
		return GetStateChangePriority(state.m_SuperArmorLevel, isAttack: false);
	}

	private NKM_STATE_CHANGE_PRIORITY GetStateChangePriority(NKM_SUPER_ARMOR_LEVEL armorLevel, bool isAttack)
	{
		switch (armorLevel)
		{
		default:
			return NKM_STATE_CHANGE_PRIORITY.NSCP_INVALID;
		case NKM_SUPER_ARMOR_LEVEL.NSAL_NO:
			if (!isAttack)
			{
				return NKM_STATE_CHANGE_PRIORITY.NSCP_NORMAL;
			}
			return NKM_STATE_CHANGE_PRIORITY.NSCP_NORMAL_CRASH;
		case NKM_SUPER_ARMOR_LEVEL.NSAL_SKILL:
			if (!isAttack)
			{
				return NKM_STATE_CHANGE_PRIORITY.NSCP_SKILL;
			}
			return NKM_STATE_CHANGE_PRIORITY.NSCP_SKILL_CRASH;
		case NKM_SUPER_ARMOR_LEVEL.NSAL_HYPER:
			if (!isAttack)
			{
				return NKM_STATE_CHANGE_PRIORITY.NSCP_HYPER;
			}
			return NKM_STATE_CHANGE_PRIORITY.NSCP_HYPER_CRASH;
		case NKM_SUPER_ARMOR_LEVEL.NSAL_SUPER:
			if (!isAttack)
			{
				return NKM_STATE_CHANGE_PRIORITY.NSCP_SUPER;
			}
			return NKM_STATE_CHANGE_PRIORITY.NSCP_SUPER_CRASH;
		}
	}

	public void StateChange(string stateName, bool bForceChange = true, bool bImmediate = false, NKM_STATE_CHANGE_PRIORITY priority = NKM_STATE_CHANGE_PRIORITY.NSCP_INVALID)
	{
		if (!bForceChange && m_UnitStateNow != null && stateName.CompareTo(m_UnitStateNow.m_StateName) == 0)
		{
			return;
		}
		NKMUnitState unitState = GetUnitState(stateName);
		if (unitState == null)
		{
			return;
		}
		if (bImmediate)
		{
			m_StateNameNext = "";
			m_StateNameNextChange = stateName;
			m_StateNextPriority = NKM_STATE_CHANGE_PRIORITY.NSCP_INVALID;
			return;
		}
		if (priority == NKM_STATE_CHANGE_PRIORITY.NSCP_INVALID)
		{
			priority = GetStateChangePriority(unitState);
		}
		if (string.IsNullOrEmpty(m_StateNameNext) || m_StateNextPriority <= priority)
		{
			m_StateNameNext = stateName;
			m_StateNextPriority = priority;
		}
	}

	public void StateChange(short stateID, bool bForceChange = true, bool bImmediate = false)
	{
		NKMUnitState unitState = GetUnitState(stateID);
		if (unitState == null)
		{
			Log.Error($"StateChange GetUnitState NULL, unitID: {GetUnitTemplet().m_UnitTempletBase.m_UnitStrID}, stateID: {stateID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 3914);
		}
		else if (bForceChange || stateID != m_UnitStateNow.m_StateID)
		{
			if (bImmediate)
			{
				m_StateNameNext = "";
				m_StateNameNextChange = unitState.m_StateName;
				m_StateNextPriority = NKM_STATE_CHANGE_PRIORITY.NSCP_INVALID;
			}
			else
			{
				m_StateNameNext = unitState.m_StateName;
			}
		}
	}

	public bool UseShipSkill(int shipSkillID, float fPosX)
	{
		NKMShipSkillTemplet shipSkillTempletByID = NKMShipSkillManager.GetShipSkillTempletByID(shipSkillID);
		if (shipSkillTempletByID == null)
		{
			return false;
		}
		m_UnitFrameData.m_ShipSkillTemplet = shipSkillTempletByID;
		m_UnitFrameData.m_fShipSkillPosX = fPosX;
		StateChange(shipSkillTempletByID.m_UnitStateName);
		m_fCheckUseShipSkillAuto = 1f;
		return true;
	}

	public virtual bool ProcessCamera()
	{
		return false;
	}

	protected void ProcessAI()
	{
		if ((m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataA.m_bAIDisable && m_NKMGame.IsATeam(GetUnitDataGame().m_NKM_TEAM_TYPE)) || (m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataB.m_bAIDisable && m_NKMGame.IsBTeam(GetUnitDataGame().m_NKM_TEAM_TYPE)) || m_UnitSyncData.m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY || (int)m_NKMGame.GetGameRuntimeData().m_NKM_GAME_STATE < 3 || m_NKMGame.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH || m_UnitStateNow == null || m_UnitStateNow.IsBadStatusState || HasCrowdControlStatus(bExculdeConfuse: true))
		{
			return;
		}
		if (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_ATTACK && m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_SKILL)
		{
			if (!AIAttack())
			{
				if (IsArriveTarget() && GetUnitTemplet().m_fPatrolRange <= 0f)
				{
					StateChangeToASTAND(bForceChange: false);
				}
				else if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOMOVE))
				{
					StateChangeToASTAND(bForceChange: false);
				}
				else if (m_UnitSyncData.m_TargetUID <= 0)
				{
					if (GetUnitTemplet().m_fPatrolRange <= 0f)
					{
						float fDist = 99999f;
						bool bRight = m_UnitSyncData.m_bRight;
						NKMUnit idleDirRight = GetIdleDirRight(ref bRight, ref fDist);
						if (!m_UnitStateNow.m_bNoChangeRight)
						{
							m_UnitSyncData.m_bRight = bRight;
						}
						if (idleDirRight != null && fDist > GetUnitDataGame().m_fTargetNearRange)
						{
							AIMove();
						}
					}
					else
					{
						AIMove();
					}
				}
				else
				{
					AIMove();
				}
			}
		}
		else if (m_UnitStateNow.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_ATTACK && m_UnitStateNow.m_bAnimLoop)
		{
			AIAttack(bSkillCheckOnly: true);
		}
		if (GetUnitTemplet().m_fPatrolRange > 0f && !m_UnitStateNow.m_bNoChangeRight && (Math.Abs(m_UnitDataGame.m_RespawnPosX - GetUnitSyncData().m_PosX) > GetUnitTemplet().m_fPatrolRange || GetUnitSyncData().m_PosX <= m_NKMGame.GetMapTemplet().GetUnitMinX() || GetUnitSyncData().m_PosX >= m_NKMGame.GetMapTemplet().GetUnitMaxX()))
		{
			bool bRight2 = GetUnitSyncData().m_bRight;
			if (m_UnitDataGame.m_RespawnPosX < GetUnitSyncData().m_PosX)
			{
				GetUnitSyncData().m_bRight = false;
			}
			else
			{
				GetUnitSyncData().m_bRight = true;
			}
			if (GetUnitSyncData().m_PosX <= m_NKMGame.GetMapTemplet().GetUnitMinX())
			{
				GetUnitSyncData().m_bRight = true;
			}
			if (GetUnitSyncData().m_PosX >= m_NKMGame.GetMapTemplet().GetUnitMaxX())
			{
				GetUnitSyncData().m_bRight = false;
			}
			if (bRight2 != m_UnitSyncData.m_bRight)
			{
				m_UnitFrameData.m_fSpeedX = 0f - m_UnitFrameData.m_fSpeedX;
			}
		}
		m_UnitFrameData.m_fFindTargetTime -= m_DeltaTime;
		if (m_UnitFrameData.m_fFindTargetTime < 0f)
		{
			m_UnitFrameData.m_fFindTargetTime = 0f;
		}
		if (m_UnitSyncData.m_TargetUID <= 0 && m_UnitFrameData.m_fFindTargetTime > GetUnitTemplet().m_TargetFindData.m_fFindTargetTime)
		{
			m_UnitFrameData.m_fFindTargetTime = GetUnitTemplet().m_TargetFindData.m_fFindTargetTime;
		}
		if ((m_UnitSyncData.m_TargetUID <= 0 || !m_UnitTemplet.m_TargetFindData.m_bTargetNoChange) && m_UnitFrameData.m_fFindTargetTime <= 0f)
		{
			m_UnitFrameData.m_bFindTargetThisFrame = true;
		}
		if (GetUnitTemplet().m_SubTargetFindData != null)
		{
			m_UnitFrameData.m_fFindSubTargetTime -= m_DeltaTime;
			if (m_UnitFrameData.m_fFindSubTargetTime < 0f)
			{
				m_UnitFrameData.m_fFindSubTargetTime = 0f;
			}
			if ((m_UnitSyncData.m_SubTargetUID <= 0 || !m_UnitTemplet.m_SubTargetFindData.m_bTargetNoChange) && m_UnitFrameData.m_fFindSubTargetTime <= 0f)
			{
				m_UnitFrameData.m_bFindSubTargetThisFrame = true;
			}
		}
	}

	protected virtual void AITarget()
	{
		NKMUnit nKMUnit = m_NKMGame.FindTarget(this, GetSortUnitListByNearDist(m_UnitTemplet.m_TargetFindData.m_bUseUnitSize), m_UnitTemplet.m_TargetFindData, GetUnitDataGame().m_NKM_TEAM_TYPE, GetUnitSyncData().m_PosX, GetUnitTemplet().m_UnitSizeX, GetUnitSyncData().m_bRight);
		if (nKMUnit != null)
		{
			m_UnitSyncData.m_TargetUID = nKMUnit.GetUnitDataGame().m_GameUnitUID;
			m_TargetUnit = nKMUnit;
			if (m_TargetUIDOrg != m_UnitSyncData.m_TargetUID)
			{
				GetUnitFrameData().m_bTargetChangeThisFrame = true;
			}
		}
	}

	protected virtual void AISubTarget()
	{
		NKMUnit nKMUnit = m_NKMGame.FindTarget(this, GetSortUnitListByNearDist(m_UnitTemplet.m_SubTargetFindData.m_bUseUnitSize), m_UnitTemplet.m_SubTargetFindData, GetUnitDataGame().m_NKM_TEAM_TYPE, GetUnitSyncData().m_PosX, GetUnitTemplet().m_UnitSizeX, GetUnitSyncData().m_bRight);
		if (nKMUnit != null)
		{
			m_UnitSyncData.m_SubTargetUID = nKMUnit.GetUnitDataGame().m_GameUnitUID;
			m_SubTargetUnit = nKMUnit;
			if (m_SubTargetUIDOrg != m_UnitSyncData.m_SubTargetUID)
			{
				GetUnitFrameData().m_bTargetChangeThisFrame = true;
			}
		}
	}

	protected bool AIMove()
	{
		if (RunAllowed())
		{
			StateChangeToRUN(bForceChange: false);
			return true;
		}
		return false;
	}

	protected bool AIAttack(bool bSkillCheckOnly = false)
	{
		if (m_NKMGame.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH && m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			return false;
		}
		if (HasCrowdControlStatus(bExculdeConfuse: true) || (m_UnitStateNow != null && m_UnitStateNow.IsBadStatusState))
		{
			return false;
		}
		if (IsATeam() && m_NKMGame.GetGameData().m_TeamASupply <= 0)
		{
			return false;
		}
		int attackIndex = 0;
		NKM_GAME_AUTO_SKILL_TYPE nKM_GAME_AUTO_SKILL_TYPE = ((!IsATeam()) ? m_NKMGame.GetGameRuntimeData().GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_B1).m_NKM_GAME_AUTO_SKILL_TYPE : m_NKMGame.GetGameRuntimeData().GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_NKM_GAME_AUTO_SKILL_TYPE);
		if (nKM_GAME_AUTO_SKILL_TYPE == NKM_GAME_AUTO_SKILL_TYPE.NGST_AUTO && CanUseAttack(m_UnitTemplet.m_listHyperSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_NORMAL, ref attackIndex) && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE))
		{
			StateChange(m_UnitTemplet.m_listHyperSkillStateData[attackIndex].m_StateName);
			return true;
		}
		if ((nKM_GAME_AUTO_SKILL_TYPE == NKM_GAME_AUTO_SKILL_TYPE.NGST_AUTO || nKM_GAME_AUTO_SKILL_TYPE == NKM_GAME_AUTO_SKILL_TYPE.NGST_OFF_HYPER) && CanUseAttack(m_UnitTemplet.m_listSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_NORMAL, ref attackIndex) && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE))
		{
			StateChange(m_UnitTemplet.m_listSkillStateData[attackIndex].m_StateName);
			return true;
		}
		if (!bSkillCheckOnly && CanUseAttack(m_UnitTemplet.m_listAttackStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_NORMAL, ref attackIndex))
		{
			StateChange(m_UnitTemplet.m_listAttackStateData[attackIndex].m_StateName);
			return true;
		}
		if (m_TargetUnit == null)
		{
			return false;
		}
		if (m_TargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			m_UnitSyncData.m_TargetUID = 0;
			return false;
		}
		float num = Math.Abs(m_TargetUnit.GetUnitSyncData().m_PosX - m_UnitSyncData.m_PosX) - (m_TargetUnit.GetUnitTemplet().m_UnitSizeX * 0.5f + m_UnitTemplet.m_UnitSizeX * 0.5f);
		if (num < 0f)
		{
			num = 0f;
		}
		if (nKM_GAME_AUTO_SKILL_TYPE == NKM_GAME_AUTO_SKILL_TYPE.NGST_AUTO)
		{
			if (CanUseAttack(m_UnitTemplet.m_listAirHyperSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_NORMAL, m_TargetUnit.IsAirUnit(), num, ref attackIndex) && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE) && m_TargetUnit.IsAirUnit())
			{
				StateChange(m_UnitTemplet.m_listAirHyperSkillStateData[attackIndex].m_StateName);
				return true;
			}
			if (CanUseAttack(m_UnitTemplet.m_listHyperSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_NORMAL, m_TargetUnit.IsAirUnit(), num, ref attackIndex) && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE))
			{
				StateChange(m_UnitTemplet.m_listHyperSkillStateData[attackIndex].m_StateName);
				return true;
			}
		}
		if (nKM_GAME_AUTO_SKILL_TYPE == NKM_GAME_AUTO_SKILL_TYPE.NGST_AUTO || nKM_GAME_AUTO_SKILL_TYPE == NKM_GAME_AUTO_SKILL_TYPE.NGST_OFF_HYPER)
		{
			if (CanUseAttack(m_UnitTemplet.m_listAirSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_NORMAL, m_TargetUnit.IsAirUnit(), num, ref attackIndex) && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE) && m_TargetUnit.IsAirUnit())
			{
				StateChange(m_UnitTemplet.m_listAirSkillStateData[attackIndex].m_StateName);
				return true;
			}
			if (CanUseAttack(m_UnitTemplet.m_listSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_NORMAL, m_TargetUnit.IsAirUnit(), num, ref attackIndex) && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE))
			{
				StateChange(m_UnitTemplet.m_listSkillStateData[attackIndex].m_StateName);
				return true;
			}
		}
		if (!bSkillCheckOnly && CanUseAttack(m_UnitTemplet.m_listAirAttackStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_NORMAL, m_TargetUnit.IsAirUnit(), num, ref attackIndex) && m_TargetUnit.IsAirUnit())
		{
			StateChange(m_UnitTemplet.m_listAirAttackStateData[attackIndex].m_StateName);
			return true;
		}
		if (!bSkillCheckOnly && CanUseAttack(m_UnitTemplet.m_listAttackStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_NORMAL, m_TargetUnit.IsAirUnit(), num, ref attackIndex))
		{
			StateChange(m_UnitTemplet.m_listAttackStateData[attackIndex].m_StateName);
			return true;
		}
		if (m_TargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			m_UnitSyncData.m_TargetUID = 0;
			return false;
		}
		return false;
	}

	public bool CanUseManualSkill(bool bUse, out bool bHyper, out byte skillStateID)
	{
		bHyper = false;
		skillStateID = 0;
		if (m_UnitStateNow == null)
		{
			return false;
		}
		if (m_NKMGame.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH && m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			return false;
		}
		if (IsATeam() && m_NKMGame.GetGameData().m_TeamASupply <= 0)
		{
			return false;
		}
		int attackIndex = 0;
		if (CanUseAttack(m_UnitTemplet.m_listHyperSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_NORMAL, ref attackIndex) && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE) && m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DAMAGE && m_UnitStateNow.m_NKM_SKILL_TYPE < NKM_SKILL_TYPE.NST_HYPER)
		{
			if (bUse)
			{
				StateChange(m_UnitTemplet.m_listHyperSkillStateData[attackIndex].m_StateName);
			}
			bHyper = true;
			NKMUnitState unitState = GetUnitState(m_UnitTemplet.m_listHyperSkillStateData[attackIndex].m_StateName);
			if (unitState != null)
			{
				skillStateID = unitState.m_StateID;
			}
			return true;
		}
		bool bAirUnit = false;
		if (m_TargetUnit != null && m_TargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			bAirUnit = m_TargetUnit.IsAirUnit();
		}
		if (CanUseAttack(m_UnitTemplet.m_listHyperSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_NORMAL, bAirUnit, 0f, ref attackIndex) && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE) && m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DAMAGE && m_UnitStateNow.m_NKM_SKILL_TYPE < NKM_SKILL_TYPE.NST_HYPER)
		{
			if (bUse)
			{
				StateChange(m_UnitTemplet.m_listHyperSkillStateData[attackIndex].m_StateName);
			}
			bHyper = true;
			NKMUnitState unitState2 = GetUnitState(m_UnitTemplet.m_listHyperSkillStateData[attackIndex].m_StateName);
			if (unitState2 != null)
			{
				skillStateID = unitState2.m_StateID;
			}
			return true;
		}
		return false;
	}

	public float GetNowHP()
	{
		return GetUnitSyncData().GetHP();
	}

	public float GetHPRate()
	{
		float statFinal = GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP);
		if (statFinal.IsNearlyZero())
		{
			return 0f;
		}
		return GetUnitSyncData().GetHP() / statFinal;
	}

	public float GetHPRateToValue(float fHPRate)
	{
		float statFinal = GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP);
		if (statFinal.IsNearlyZero())
		{
			return 0f;
		}
		return statFinal * fHPRate;
	}

	protected bool IsStateUnlocked(NKMAttackStateData attackStateData)
	{
		NKMUnitState unitState = m_UnitTemplet.GetUnitState(attackStateData.m_StateName);
		return IsStateUnlocked(unitState);
	}

	protected bool IsStateUnlocked(NKMUnitState unitState)
	{
		if (unitState == null)
		{
			return false;
		}
		return GetUnitData().IsUnitSkillUnlockedByType(unitState.m_NKM_SKILL_TYPE);
	}

	public void StateChangeToSTART(bool bForceChange = true, bool bImmediate = false)
	{
		string text = GetUsableCommonState(m_UnitTemplet.m_listStartStateData, null);
		if (string.IsNullOrEmpty(text))
		{
			text = ((!m_NKMGame.CheckBossDungeon() || GetUnitState("USN_START_BOSS", bLog: false) == null) ? "USN_START" : "USN_START_BOSS");
		}
		StateChange(text, bForceChange, bImmediate);
	}

	public void StateChangeToASTAND(bool bForceChange = true, bool bImmediate = false)
	{
		string usableCommonState = GetUsableCommonState(m_UnitTemplet.m_listStandStateData, "USN_ASTAND");
		StateChange(usableCommonState, bForceChange, bImmediate);
	}

	public void StateChangeToRUN(bool bForceChange = true, bool bImmediate = false)
	{
		string usableCommonState = GetUsableCommonState(m_UnitTemplet.m_listRunStateData, "USN_RUN");
		StateChange(usableCommonState, bForceChange, bImmediate);
	}

	protected string GetUsableCommonState(List<NKMCommonStateData> lstCommonStateData, string defaultState)
	{
		if (lstCommonStateData == null || lstCommonStateData.Count == 0)
		{
			return defaultState;
		}
		m_listAttackSelectTemp.Clear();
		for (int i = 0; i < lstCommonStateData.Count; i++)
		{
			if (CanUseState(lstCommonStateData[i]))
			{
				for (int j = 0; j < lstCommonStateData[i].m_Ratio; j++)
				{
					m_listAttackSelectTemp.Add(i);
				}
			}
		}
		if (m_listAttackSelectTemp.Count > 0)
		{
			int index = NKMRandom.Range(0, m_listAttackSelectTemp.Count);
			int index2 = m_listAttackSelectTemp[index];
			return lstCommonStateData[index2].m_StateName;
		}
		return defaultState;
	}

	protected bool CanUseState(NKMCommonStateData stateData)
	{
		if (!CheckEventCondition(stateData.m_Condition))
		{
			return false;
		}
		_ = GetUnitDataGame().m_fTargetNearRange / GetUnitTemplet().m_TargetNearRange;
		if (stateData.CanUseState(GetHPRate(), GetUnitFrameData().m_PhaseNow))
		{
			if (!CheckStateCoolTime(stateData.m_StateName))
			{
				return false;
			}
			NKMUnitState unitState = m_UnitTemplet.GetUnitState(stateData.m_StateName);
			if (IsStateUnlocked(unitState))
			{
				return true;
			}
		}
		return false;
	}

	protected bool CanUseAttack(List<NKMAttackStateData> m_listAttackStateData, NKM_ATTACK_STATE_DATA_TYPE eNKM_ATTACK_STATE_DATA_TYPE, bool bAirUnit, float fDistToTarget, ref int attackIndex)
	{
		m_listAttackSelectTemp.Clear();
		for (int i = 0; i < m_listAttackStateData.Count; i++)
		{
			if (CanUseAttack(m_listAttackStateData[i], eNKM_ATTACK_STATE_DATA_TYPE, bAirUnit, fDistToTarget))
			{
				for (int j = 0; j < m_listAttackStateData[i].m_Ratio; j++)
				{
					m_listAttackSelectTemp.Add(i);
				}
			}
		}
		if (m_listAttackSelectTemp.Count > 0)
		{
			int index = NKMRandom.Range(0, m_listAttackSelectTemp.Count);
			attackIndex = m_listAttackSelectTemp[index];
			return true;
		}
		return false;
	}

	protected bool CanUseAttack(NKMAttackStateData m_AttackStateData, NKM_ATTACK_STATE_DATA_TYPE eNKM_ATTACK_STATE_DATA_TYPE, bool bAirUnit, float fDistToTarget)
	{
		if (!CheckEventCondition(m_AttackStateData.m_Condition))
		{
			return false;
		}
		float fRangeFactor = GetUnitDataGame().m_fTargetNearRange / GetUnitTemplet().m_TargetNearRange;
		if (m_AttackStateData.CanUseAttack(eNKM_ATTACK_STATE_DATA_TYPE, GetHPRate(), bAirUnit, fDistToTarget, GetUnitFrameData().m_PhaseNow, fRangeFactor))
		{
			if (!CheckStateCoolTime(m_AttackStateData.m_StateName))
			{
				return false;
			}
			if (IsStateUnlocked(m_AttackStateData))
			{
				return true;
			}
		}
		return false;
	}

	protected bool CanUseAttack(List<NKMAttackStateData> m_listAttackStateData, NKM_ATTACK_STATE_DATA_TYPE eNKM_ATTACK_STATE_DATA_TYPE, ref int attackIndex)
	{
		m_listAttackSelectTemp.Clear();
		for (int i = 0; i < m_listAttackStateData.Count; i++)
		{
			if (CanUseAttack(m_listAttackStateData[i], eNKM_ATTACK_STATE_DATA_TYPE))
			{
				for (int j = 0; j < m_listAttackStateData[i].m_Ratio; j++)
				{
					m_listAttackSelectTemp.Add(i);
				}
			}
		}
		if (m_listAttackSelectTemp.Count > 0)
		{
			int index = NKMRandom.Range(0, m_listAttackSelectTemp.Count);
			attackIndex = m_listAttackSelectTemp[index];
			return true;
		}
		return false;
	}

	protected bool CanUseAttack(NKMAttackStateData m_AttackStateData, NKM_ATTACK_STATE_DATA_TYPE eNKM_ATTACK_STATE_DATA_TYPE)
	{
		if (!CheckEventCondition(m_AttackStateData.m_Condition))
		{
			return false;
		}
		float fRangeFactor = GetUnitDataGame().m_fTargetNearRange / GetUnitTemplet().m_TargetNearRange;
		if (m_AttackStateData.CanUseAttack(eNKM_ATTACK_STATE_DATA_TYPE, GetHPRate(), GetUnitFrameData().m_PhaseNow, fRangeFactor))
		{
			if (!CheckStateCoolTime(m_AttackStateData.m_StateName))
			{
				return false;
			}
			if (IsStateUnlocked(m_AttackStateData))
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckStateCoolTime(string stateName)
	{
		if (GetStateCoolTime(stateName) > 0f)
		{
			return false;
		}
		return true;
	}

	public float GetStateCoolTime(string stateName)
	{
		NKMUnitState unitState = GetUnitState(stateName);
		return GetStateCoolTime(unitState);
	}

	public float GetStateCoolTime(NKMUnitState cNKMUnitState)
	{
		if (cNKMUnitState != null && m_dicStateCoolTime.ContainsKey(cNKMUnitState.m_StateID))
		{
			return m_dicStateCoolTime[cNKMUnitState.m_StateID].m_CoolTime;
		}
		return 0f;
	}

	public Dictionary<int, float> GetDicStateCoolTime()
	{
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		foreach (KeyValuePair<int, NKMStateCoolTime> item in m_dicStateCoolTime)
		{
			dictionary.Add(item.Key, item.Value.m_CoolTime);
		}
		return dictionary;
	}

	public NKMAttackStateData GetFastestCoolTimeSkillData()
	{
		return GetFastestCoolTimeSkillData(m_UnitTemplet.m_listSkillStateData);
	}

	public NKMAttackStateData GetFastestCoolTimeHyperSkillData()
	{
		return GetFastestCoolTimeSkillData(m_UnitTemplet.m_listHyperSkillStateData);
	}

	private NKMAttackStateData GetFastestCoolTimeSkillData(List<NKMAttackStateData> listNKMAttackStateData)
	{
		if (listNKMAttackStateData.Count < 1)
		{
			return null;
		}
		int num = 0;
		float num2 = 999999f;
		for (int i = 0; i < listNKMAttackStateData.Count; i++)
		{
			NKMAttackStateData nKMAttackStateData = listNKMAttackStateData[i];
			if (nKMAttackStateData != null && IsStateUnlocked(nKMAttackStateData))
			{
				float stateCoolTime = GetStateCoolTime(nKMAttackStateData.m_StateName);
				if (!(num2 <= stateCoolTime))
				{
					num = i;
					num2 = stateCoolTime;
				}
			}
		}
		if (num < 0 || num >= listNKMAttackStateData.Count)
		{
			num = 0;
		}
		return listNKMAttackStateData[num];
	}

	public void SeeTarget()
	{
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY || m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER || m_UnitTemplet.m_TargetFindData.m_bNoBackTarget || m_TargetUnit == null)
		{
			return;
		}
		bool bRight = m_UnitSyncData.m_bRight;
		if (m_UnitTemplet.m_SeeTarget && m_UnitStateNow != null && !m_UnitStateNow.m_bNoChangeRight)
		{
			if (m_UnitSyncData.m_PosX < m_TargetUnit.GetUnitSyncData().m_PosX)
			{
				m_UnitSyncData.m_bRight = true;
			}
			else
			{
				m_UnitSyncData.m_bRight = false;
			}
		}
		if (bRight != m_UnitSyncData.m_bRight)
		{
			m_UnitFrameData.m_fSpeedX = 0f - m_UnitFrameData.m_fSpeedX;
		}
	}

	protected void SeeMoreEnemy()
	{
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY || m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		List<NKMUnit> sortUnitListByNearDist = GetSortUnitListByNearDist();
		for (int i = 0; i < sortUnitListByNearDist.Count; i++)
		{
			NKMUnit nKMUnit = sortUnitListByNearDist[i];
			if (nKMUnit.GetUnitSyncData().m_GameUnitUID != GetUnitSyncData().m_GameUnitUID && nKMUnit.WillBeTargetted() && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DYING && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE && !IsAlly(nKMUnit) && !(Math.Abs(nKMUnit.GetUnitSyncData().m_PosX - GetUnitSyncData().m_PosX) > GetUnitTemplet().m_TargetFindData.m_fSeeRange))
			{
				if (GetUnitSyncData().m_PosX <= nKMUnit.GetUnitSyncData().m_PosX)
				{
					num++;
				}
				else
				{
					num2++;
				}
			}
		}
		bool bRight = m_UnitSyncData.m_bRight;
		if (num == num2)
		{
			return;
		}
		if (m_UnitStateNow != null && !m_UnitStateNow.m_bNoChangeRight)
		{
			if (num >= num2)
			{
				m_UnitSyncData.m_bRight = true;
			}
			else
			{
				m_UnitSyncData.m_bRight = false;
			}
		}
		if (bRight != m_UnitSyncData.m_bRight)
		{
			m_UnitFrameData.m_fSpeedX = 0f - m_UnitFrameData.m_fSpeedX;
		}
	}

	protected void HPProcess()
	{
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			float num = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP) * m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP_REGEN_RATE) * m_DeltaTime;
			bool flag = false;
			if (GetUnitFrameData().m_bInvincible || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE))
			{
				flag = true;
			}
			if (m_UnitStateNow != null && m_UnitStateNow.m_bInvincibleState)
			{
				flag = true;
			}
			if (num < 0f && flag)
			{
				num = 0f;
			}
			if (num > 0f && HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOHEAL))
			{
				num = 0f;
			}
			if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IRONWALL))
			{
				num = 0f;
			}
			if (num > 0f)
			{
				if (GetUnitFrameData().m_fHealFeedback > 0f)
				{
					NKM_TEAM_TYPE teamType = NKM_TEAM_TYPE.NTT_INVALID;
					NKMUnit unit = m_NKMGame.GetUnit(GetUnitFrameData().m_fHealFeedbackMasterGameUnitUID, bChain: true, bPool: true);
					if (unit != null)
					{
						teamType = unit.GetUnitDataGame().m_NKM_TEAM_TYPE;
					}
					if (!flag)
					{
						AddDamage(bAttackCountOver: false, num * GetUnitFrameData().m_fHealFeedback, NKM_DAMAGE_RESULT_TYPE.NDRT_NO_MARK, GetUnitFrameData().m_fHealFeedbackMasterGameUnitUID, teamType, bPushSyncData: false, bNoRedirect: true);
					}
					num = 0f;
				}
				num = ApplyHealTransfer(num, bShowHealResult: false);
			}
			num += m_UnitSyncData.GetHP();
			bool cutDamage;
			float phaseDamageLimit = GetPhaseDamageLimit(out cutDamage);
			if (phaseDamageLimit > 0f)
			{
				if (cutDamage && m_UnitSyncData.GetHP() < phaseDamageLimit)
				{
					num = phaseDamageLimit - 1f;
				}
				if (num < phaseDamageLimit && !IsMonster() && num <= 0f)
				{
					num = 1f;
				}
			}
			if (num <= 0f && HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMORTAL))
			{
				m_UnitFrameData.m_bImmortalStart = true;
				num = 1f;
			}
			m_UnitSyncData.SetHP(num);
		}
		if (m_UnitSyncData.GetHP() > m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP))
		{
			m_UnitSyncData.SetHP(m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP));
		}
		if (m_NKMGame.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE && m_NKMGame.GetGameRuntimeData().m_bPracticeHeal)
		{
			m_fPracticeHPReset -= m_DeltaTime;
			if (m_fPracticeHPReset < 0f)
			{
				m_fPracticeHPReset = 3f;
				m_UnitSyncData.SetHP(m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP));
				m_PushSyncData = true;
			}
		}
		if (m_UnitSyncData.GetHP() <= 0f)
		{
			if (m_NKMGame.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE && m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY)
			{
				m_UnitSyncData.SetHP(1f);
				return;
			}
			m_UnitSyncData.SetHP(0f);
			SetDying();
		}
	}

	public virtual void SetDying(bool bForce = false, bool bUnitChange = false)
	{
		if (!bForce && (m_NKMGame.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_DEV || m_NKMGame.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE))
		{
			bool flag = false;
			if (m_NKMGame.GetGameData().m_NKMGameTeamDataA.m_MainShip != null)
			{
				for (int i = 0; i < m_NKMGame.GetGameData().m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID.Count; i++)
				{
					if (m_UnitSyncData.m_GameUnitUID == m_NKMGame.GetGameData().m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID[i])
					{
						flag = true;
						break;
					}
				}
			}
			if (m_NKMGame.GetGameData().m_NKMGameTeamDataB.m_MainShip != null)
			{
				for (int j = 0; j < m_NKMGame.GetGameData().m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID.Count; j++)
				{
					if (m_UnitSyncData.m_GameUnitUID == m_NKMGame.GetGameData().m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID[j])
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				m_UnitSyncData.SetHP(1f);
				return;
			}
		}
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE || m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING)
		{
			return;
		}
		NKMGameRuntimeTeamData nKMGameRuntimeTeamData = null;
		if (IsATeam())
		{
			nKMGameRuntimeTeamData = m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataA;
		}
		else if (IsBTeam())
		{
			nKMGameRuntimeTeamData = m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataB;
		}
		if (nKMGameRuntimeTeamData != null && nKMGameRuntimeTeamData.m_bGodMode)
		{
			m_UnitSyncData.SetHP(1f);
			return;
		}
		m_UnitSyncData.m_NKM_UNIT_PLAY_STATE = NKM_UNIT_PLAY_STATE.NUPS_DYING;
		StateChange(m_UnitTemplet.m_DyingState, bForceChange: false);
		ProcessUnitDyingBuff();
		if (GetUnitData().m_DungeonRespawnUnitTemplet != null && !bUnitChange && GetUnitData().m_DungeonRespawnUnitTemplet.m_EventRespawnUnitTag.Length > 0)
		{
			m_NKMGame.AddDieUnitRespawnTag(GetUnitData().m_DungeonRespawnUnitTemplet.m_EventRespawnUnitTag);
		}
		m_EventMovePosX.StopTracking();
		m_EventMovePosZ.StopTracking();
		m_EventMovePosJumpY.StopTracking();
	}

	public void ProcessUnitDyingBuff()
	{
		m_listBuffDieEvent.Clear();
		m_listBuffDelete.Clear();
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in m_UnitFrameData.m_dicBuffData)
		{
			NKMBuffData value = dicBuffDatum.Value;
			if (!value.m_NKMBuffTemplet.m_bInfinity)
			{
				m_listBuffDelete.Add(value.m_BuffSyncData.m_BuffID);
			}
			if (value.m_NKMBuffTemplet.m_bUnitDieEvent && (value.m_NKMBuffTemplet.m_AffectMe || value.m_BuffSyncData.m_MasterGameUnitUID != GetUnitDataGame().m_GameUnitUID))
			{
				m_listBuffDieEvent.Add(value);
			}
		}
		foreach (NKMBuffData item in m_listBuffDieEvent)
		{
			m_NKMGame.GetUnit(item.m_BuffSyncData.m_MasterGameUnitUID, bChain: true, bPool: true)?.ProcessEventBuffUnitDie(item, this);
		}
		m_listBuffDieEvent.Clear();
		foreach (short item2 in m_listBuffDelete)
		{
			DeleteBuff(item2, NKMBuffTemplet.BuffEndDTType.NoUse);
		}
		m_listBuffDelete.Clear();
		m_bPushSimpleSyncData = true;
	}

	public virtual bool SetDie(bool bCheckAllDie = true)
	{
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			m_UnitSyncData.m_NKM_UNIT_PLAY_STATE = NKM_UNIT_PLAY_STATE.NUPS_DIE;
			m_UnitDataGame.m_NKM_TEAM_TYPE = m_UnitDataGame.m_NKM_TEAM_TYPE_ORG;
			InitLInkedDamageEffect();
			if (m_NKMGame.IsGameUnitAllDie(GetUnitData(), m_UnitSyncData.m_GameUnitUID))
			{
				if (GetUnitData().m_DungeonRespawnUnitTemplet != null && GetUnitData().m_DungeonRespawnUnitTemplet.m_EventRespawnUnitTag.Length > 0)
				{
					m_NKMGame.AddDieDeckRespawnTag(GetUnitData().m_DungeonRespawnUnitTemplet.m_EventRespawnUnitTag);
				}
				GetUnitData().m_fLastDieTime = m_NKMGame.GetGameRuntimeData().m_GameTime;
			}
			return true;
		}
		return false;
	}

	protected virtual void PhysicProcess()
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		NKMUnit unit = m_NKMGame.GetUnit(GetUnitSyncData().m_CatcherGameUnitUID);
		if (m_UnitStateNow.m_bNoMove || m_UnitTemplet.m_bNoMove || unit != null)
		{
			m_UnitFrameData.m_fSpeedX = 0f;
			m_UnitFrameData.m_fSpeedY = 0f;
			m_UnitFrameData.m_fSpeedZ = 0f;
			m_UnitFrameData.m_fDamageSpeedX = 0f;
			m_UnitFrameData.m_fDamageSpeedZ = 0f;
			m_UnitFrameData.m_fDamageSpeedJumpY = 0f;
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_FREEZE) || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_HOLD) || (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOMOVE) && !m_UnitStateNow.IsDamageOrDieState))
		{
			m_UnitFrameData.m_fSpeedX = 0f;
			m_UnitFrameData.m_fSpeedZ = 0f;
			m_UnitFrameData.m_fDamageSpeedX = 0f;
			m_UnitFrameData.m_fDamageSpeedZ = 0f;
			m_UnitFrameData.m_fDamageSpeedJumpY = 0f;
		}
		float fGAccel = m_UnitTemplet.m_fGAccel;
		if (!m_UnitStateNow.m_fGAccel.IsNearlyEqual(-1f))
		{
			fGAccel = m_UnitStateNow.m_fGAccel;
		}
		if (m_UnitFrameData.m_fTargetAirHigh > m_UnitFrameData.m_fAirHigh)
		{
			m_UnitFrameData.m_fAirHigh += 2000f * m_DeltaTime;
			if (m_UnitFrameData.m_fTargetAirHigh < m_UnitFrameData.m_fAirHigh)
			{
				m_UnitFrameData.m_fAirHigh = m_UnitFrameData.m_fTargetAirHigh;
			}
		}
		else if (m_UnitFrameData.m_fTargetAirHigh < m_UnitFrameData.m_fAirHigh)
		{
			m_UnitFrameData.m_fAirHigh = m_UnitFrameData.m_fTargetAirHigh;
		}
		if (m_UnitSyncData.m_bRight)
		{
			m_UnitFrameData.m_PosXCalc += m_UnitFrameData.m_fSpeedX * m_DeltaTime;
		}
		else
		{
			m_UnitFrameData.m_PosXCalc -= m_UnitFrameData.m_fSpeedX * m_DeltaTime;
		}
		m_UnitFrameData.m_PosZCalc += m_UnitFrameData.m_fSpeedZ * m_DeltaTime;
		m_UnitFrameData.m_JumpYPosCalc += m_UnitFrameData.m_fSpeedY * m_DeltaTime;
		m_UnitFrameData.m_PosXCalc += m_UnitFrameData.m_fDamageSpeedX * m_DeltaTime;
		if (m_UnitSyncData.m_bAttackerZUp)
		{
			m_UnitFrameData.m_PosZCalc -= m_UnitFrameData.m_fDamageSpeedZ * m_DeltaTime;
		}
		else
		{
			m_UnitFrameData.m_PosZCalc += m_UnitFrameData.m_fDamageSpeedZ * m_DeltaTime;
		}
		m_UnitFrameData.m_JumpYPosCalc += m_UnitFrameData.m_fDamageSpeedJumpY * m_DeltaTime;
		if (m_UnitFrameData.m_fSpeedX >= 0f)
		{
			m_UnitFrameData.m_fSpeedX -= m_UnitTemplet.m_fReloadAccel * m_DeltaTime;
			if (m_UnitFrameData.m_fSpeedX <= 0f)
			{
				m_UnitFrameData.m_fSpeedX = 0f;
			}
		}
		else
		{
			m_UnitFrameData.m_fSpeedX += m_UnitTemplet.m_fReloadAccel * m_DeltaTime;
			if (m_UnitFrameData.m_fSpeedX > 0f)
			{
				m_UnitFrameData.m_fSpeedX = 0f;
			}
		}
		if (m_UnitFrameData.m_fSpeedZ >= 0f)
		{
			m_UnitFrameData.m_fSpeedZ -= m_UnitTemplet.m_fReloadAccel * m_DeltaTime;
			if (m_UnitFrameData.m_fSpeedZ <= 0f)
			{
				m_UnitFrameData.m_fSpeedZ = 0f;
			}
		}
		else
		{
			m_UnitFrameData.m_fSpeedZ += m_UnitTemplet.m_fReloadAccel * m_DeltaTime;
			if (m_UnitFrameData.m_fSpeedZ > 0f)
			{
				m_UnitFrameData.m_fSpeedZ = 0f;
			}
		}
		m_UnitFrameData.m_fSpeedY -= fGAccel * m_DeltaTime;
		if (m_UnitFrameData.m_fSpeedY <= m_UnitTemplet.m_fMaxGSpeed)
		{
			m_UnitFrameData.m_fSpeedY = m_UnitTemplet.m_fMaxGSpeed;
		}
		m_UnitFrameData.m_fDamageSpeedKeepTimeX -= m_DeltaTime;
		m_UnitFrameData.m_fDamageSpeedKeepTimeZ -= m_DeltaTime;
		m_UnitFrameData.m_fDamageSpeedKeepTimeJumpY -= m_DeltaTime;
		if (m_UnitFrameData.m_fDamageSpeedKeepTimeX <= 0f)
		{
			m_UnitFrameData.m_fDamageSpeedKeepTimeX = 0f;
			if (m_UnitFrameData.m_fDamageSpeedX >= 0f)
			{
				m_UnitFrameData.m_fDamageSpeedX -= m_UnitTemplet.m_fReloadAccel * m_DeltaTime;
				if (m_UnitFrameData.m_fDamageSpeedX <= 0f)
				{
					m_UnitFrameData.m_fDamageSpeedX = 0f;
				}
			}
			else
			{
				m_UnitFrameData.m_fDamageSpeedX += m_UnitTemplet.m_fReloadAccel * m_DeltaTime;
				if (m_UnitFrameData.m_fDamageSpeedX > 0f)
				{
					m_UnitFrameData.m_fDamageSpeedX = 0f;
				}
			}
		}
		if (m_UnitFrameData.m_fDamageSpeedKeepTimeZ <= 0f)
		{
			m_UnitFrameData.m_fDamageSpeedKeepTimeZ = 0f;
			if (m_UnitFrameData.m_fDamageSpeedZ >= 0f)
			{
				m_UnitFrameData.m_fDamageSpeedZ -= m_UnitTemplet.m_fReloadAccel * m_DeltaTime;
				if (m_UnitFrameData.m_fDamageSpeedZ <= 0f)
				{
					m_UnitFrameData.m_fDamageSpeedZ = 0f;
				}
			}
			else
			{
				m_UnitFrameData.m_fDamageSpeedZ += m_UnitTemplet.m_fReloadAccel * m_DeltaTime;
				if (m_UnitFrameData.m_fDamageSpeedZ > 0f)
				{
					m_UnitFrameData.m_fDamageSpeedZ = 0f;
				}
			}
		}
		if (m_UnitFrameData.m_fDamageSpeedKeepTimeJumpY <= 0f)
		{
			m_UnitFrameData.m_fDamageSpeedKeepTimeJumpY = 0f;
			if (m_UnitFrameData.m_fDamageSpeedJumpY >= 0f)
			{
				m_UnitFrameData.m_fDamageSpeedJumpY -= fGAccel * m_DeltaTime;
				if (m_UnitFrameData.m_fDamageSpeedJumpY <= 0f)
				{
					m_UnitFrameData.m_fDamageSpeedJumpY = 0f;
				}
			}
			else
			{
				m_UnitFrameData.m_fDamageSpeedJumpY += fGAccel * m_DeltaTime;
				if (m_UnitFrameData.m_fDamageSpeedJumpY > 0f)
				{
					m_UnitFrameData.m_fDamageSpeedJumpY = 0f;
				}
			}
		}
		else
		{
			m_UnitFrameData.m_fSpeedY = 0f;
		}
		if (m_UnitStateNow.m_bNoMove || m_UnitTemplet.m_bNoMove)
		{
			m_EventMovePosX.StopTracking();
			m_EventMovePosZ.StopTracking();
			m_EventMovePosJumpY.StopTracking();
		}
		m_EventMovePosX.Update(m_DeltaTime);
		m_EventMovePosZ.Update(m_DeltaTime);
		m_EventMovePosJumpY.Update(m_DeltaTime);
		if (m_EventMovePosX.IsTracking())
		{
			m_UnitFrameData.m_fSpeedX = 0f;
			m_UnitFrameData.m_PosXCalc = m_EventMovePosX.GetNowValue();
		}
		if (m_EventMovePosZ.IsTracking())
		{
			m_UnitFrameData.m_fSpeedZ = 0f;
			m_UnitFrameData.m_PosZCalc = m_EventMovePosZ.GetNowValue();
		}
		if (m_EventMovePosJumpY.IsTracking())
		{
			m_UnitFrameData.m_fSpeedY = 0f;
			m_UnitFrameData.m_JumpYPosCalc = m_EventMovePosJumpY.GetNowValue();
		}
		if (unit != null)
		{
			if (unit.GetUnitSyncData().m_bRight)
			{
				m_UnitFrameData.m_PosXCalc = unit.GetUnitSyncData().m_PosX + unit.GetUnitTemplet().m_UnitSizeX;
			}
			else
			{
				m_UnitFrameData.m_PosXCalc = unit.GetUnitSyncData().m_PosX - unit.GetUnitTemplet().m_UnitSizeX;
			}
			m_UnitFrameData.m_JumpYPosCalc = unit.GetUnitSyncData().m_JumpYPos;
		}
	}

	private float IsCollisionUnit(NKMUnit cNKMUnit)
	{
		float num = Math.Abs(cNKMUnit.GetUnitSyncData().m_PosX - m_UnitFrameData.m_PosXCalc);
		float num2 = cNKMUnit.GetUnitTemplet().m_UnitSizeX * 0.5f + m_UnitTemplet.m_UnitSizeX * 0.5f;
		return num - num2;
	}

	public bool IsInRange(float centerPos, float range, bool bUseUnitSize)
	{
		float num = Math.Abs(m_UnitSyncData.m_PosX - centerPos);
		if (bUseUnitSize)
		{
			num -= m_UnitTemplet.m_UnitSizeX * 0.5f;
		}
		return range >= num;
	}

	public bool IsInRange(float centerPos, float minRange, float maxRange, bool bUseUnitSize, bool bRangeRight)
	{
		float num;
		float num2;
		if (bRangeRight)
		{
			num = centerPos + minRange;
			num2 = centerPos + maxRange;
		}
		else
		{
			num = centerPos - maxRange;
			num2 = centerPos - minRange;
		}
		float posX = m_UnitSyncData.m_PosX;
		if (bUseUnitSize)
		{
			if (num <= posX && posX <= num2)
			{
				return true;
			}
			float num3 = m_UnitTemplet.m_UnitSizeX * 0.5f;
			float num4 = num - posX;
			if (num4 >= 0f && num4 < num3)
			{
				return true;
			}
			float num5 = posX - num2;
			if (num5 >= 0f && num5 < num3)
			{
				return true;
			}
			return false;
		}
		if (num <= posX)
		{
			return posX <= num2;
		}
		return false;
	}

	public bool IsInRange(NKMUnit targetUnit, float range, bool bUseUnitSize)
	{
		if (targetUnit == null)
		{
			return false;
		}
		float num = Math.Abs(m_UnitSyncData.m_PosX - targetUnit.m_UnitSyncData.m_PosX);
		if (bUseUnitSize)
		{
			num -= (m_UnitTemplet.m_UnitSizeX + targetUnit.m_UnitTemplet.m_UnitSizeX) * 0.5f;
		}
		return range >= num;
	}

	public bool IsInRange(NKMUnit targetUnit, NKMMinMaxFloat range, bool bUseUnitSize)
	{
		return IsInRange(targetUnit, range.m_Min, range.m_Max, bUseUnitSize);
	}

	public bool IsInRange(NKMUnit targetUnit, float minRange, float maxRange, bool bUseUnitSize)
	{
		if (targetUnit == null)
		{
			return false;
		}
		float num;
		float num2;
		if (m_UnitSyncData.m_bRight)
		{
			num = m_UnitSyncData.m_PosX + minRange;
			num2 = m_UnitSyncData.m_PosX + maxRange;
			if (bUseUnitSize)
			{
				float num3 = m_UnitTemplet.m_UnitSizeX * 0.5f;
				if (maxRange > 0f)
				{
					num2 += num3;
				}
				if (minRange < 0f)
				{
					num -= num3;
				}
			}
		}
		else
		{
			num = m_UnitSyncData.m_PosX - maxRange;
			num2 = m_UnitSyncData.m_PosX - minRange;
			if (bUseUnitSize)
			{
				float num4 = m_UnitTemplet.m_UnitSizeX * 0.5f;
				if (maxRange > 0f)
				{
					num -= num4;
				}
				if (minRange < 0f)
				{
					num2 += num4;
				}
			}
		}
		float posX = targetUnit.m_UnitSyncData.m_PosX;
		if (bUseUnitSize)
		{
			if (num <= posX && posX <= num2)
			{
				return true;
			}
			float num5 = targetUnit.m_UnitTemplet.m_UnitSizeX * 0.5f;
			float num6 = num - posX;
			if (num6 >= 0f && num6 < num5)
			{
				return true;
			}
			float num7 = posX - num2;
			if (num7 >= 0f && num7 < num5)
			{
				return true;
			}
			return false;
		}
		if (num <= posX)
		{
			return posX <= num2;
		}
		return false;
	}

	protected void MapEdgeProcess()
	{
		NKMMapTemplet mapTemplet = m_NKMGame.GetMapTemplet();
		if (!GetUnitTemplet().m_bNoMapLimit)
		{
			if (m_UnitFrameData.m_PosXCalc < mapTemplet.m_fMinX)
			{
				if (IsBoss())
				{
					m_UnitFrameData.m_PosXCalc = mapTemplet.m_fMinX;
				}
				else
				{
					m_UnitFrameData.m_PosXCalc = mapTemplet.GetUnitMinX();
				}
			}
			if (m_UnitFrameData.m_PosXCalc > mapTemplet.m_fMaxX)
			{
				if (IsBoss())
				{
					m_UnitFrameData.m_PosXCalc = mapTemplet.m_fMaxX;
				}
				else
				{
					m_UnitFrameData.m_PosXCalc = mapTemplet.GetUnitMaxX();
				}
			}
		}
		if (m_UnitFrameData.m_PosZCalc < mapTemplet.m_fMinZ)
		{
			m_UnitFrameData.m_PosZCalc = mapTemplet.m_fMinZ;
		}
		if (m_UnitFrameData.m_PosZCalc > mapTemplet.m_fMaxZ)
		{
			m_UnitFrameData.m_PosZCalc = mapTemplet.m_fMaxZ;
		}
		float num = 0f;
		if (IsAirUnit())
		{
			num = m_UnitFrameData.m_fAirHigh;
		}
		if (m_UnitFrameData.m_JumpYPosCalc <= num)
		{
			m_UnitFrameData.m_JumpYPosCalc = num;
			m_UnitFrameData.m_fSpeedY = 0f;
			m_UnitFrameData.m_bFootOnLand = true;
		}
		else
		{
			m_UnitFrameData.m_bFootOnLand = false;
		}
	}

	public bool CheckEventCondition(NKMEventConditionV2 cConditionV2, NKMUnit cUnitConditionOwner = null)
	{
		return cConditionV2?.CheckEventCondition(m_NKMGame, this, cUnitConditionOwner) ?? true;
	}

	public bool CheckEventCondition(NKMEventCondition cCondition, NKMUnit cUnitConditionOwner = null)
	{
		if (cCondition.m_EventConditionV2 != null)
		{
			return cCondition.m_EventConditionV2.CheckEventCondition(m_NKMGame, this, cUnitConditionOwner);
		}
		cCondition.CheckSkillID();
		if (m_NKMGame.IsPVP(bUseDevOption: true) && !cCondition.m_bUsePVP)
		{
			return false;
		}
		if (m_NKMGame.IsPVE(bUseDevOption: true) && !cCondition.m_bUsePVE)
		{
			return false;
		}
		if (!cCondition.CanUsePhase(m_UnitFrameData.m_PhaseNow))
		{
			return false;
		}
		if (!cCondition.CanUseBuff(GetUnitSyncData().m_dicBuffData))
		{
			return false;
		}
		if (!cCondition.CanUseBuffCount(this))
		{
			return false;
		}
		if (!cCondition.CanUseStatus(this))
		{
			return false;
		}
		if (cCondition.m_bLeaderUnit)
		{
			NKMGameTeamData teamData = GetTeamData();
			if (teamData == null)
			{
				return false;
			}
			if (teamData.GetLeaderUnitData() == null)
			{
				return false;
			}
			if (IsSummonUnit() || HasMasterUnit())
			{
				NKMUnit masterUnit = GetMasterUnit();
				if (masterUnit == null)
				{
					return false;
				}
				if (teamData.GetLeaderUnitData().m_UnitUID != masterUnit.GetUnitData().m_UnitUID)
				{
					return false;
				}
			}
			else if (teamData.GetLeaderUnitData().m_UnitUID != GetUnitData().m_UnitUID)
			{
				return false;
			}
		}
		if (cCondition.m_SkillID != -1)
		{
			int skillLevel = GetUnitData().GetSkillLevel(cCondition.m_SkillStrID);
			if (!cCondition.CanUseSkill(skillLevel))
			{
				return false;
			}
		}
		if (cCondition.m_MasterSkillID != -1)
		{
			int masterSkillLevel = -1;
			NKMUnit masterUnit2 = GetMasterUnit();
			if (masterUnit2 != null)
			{
				masterSkillLevel = masterUnit2.GetUnitData().GetSkillLevel(cCondition.m_MasterSkillStrID);
			}
			if (!cCondition.CanUseMasterSkill(masterSkillLevel))
			{
				return false;
			}
		}
		if (!cCondition.CheckHPRate(this))
		{
			return false;
		}
		if (!cCondition.CanUseMapPosition(m_NKMGame.GetMapTemplet().GetMapFactor(GetUnitSyncData().m_PosX, m_NKMGame.IsATeam(GetUnitDataGame().m_NKM_TEAM_TYPE))))
		{
			return false;
		}
		if (!cCondition.CanUseLevelRange(this))
		{
			return false;
		}
		if (!cCondition.CanUseUnitExist(m_NKMGame, GetTeam()))
		{
			return false;
		}
		if (!cCondition.CanUseReactorSkill(this))
		{
			return false;
		}
		if (!cCondition.CheckEventVariable(this))
		{
			return false;
		}
		return true;
	}

	public NKMEventConditionV2 GetEventConditionMacro(string name)
	{
		return GetUnitTemplet().GetEventConditionMacro(name);
	}

	public void ApplyStaticBuffToGame(NKMStaticBuffData cNKMStaticBuffData)
	{
		if (!CheckEventCondition(cNKMStaticBuffData.m_Condition))
		{
			return;
		}
		short buffID = AddBuffByStrID(cNKMStaticBuffData.m_BuffStrID, cNKMStaticBuffData.m_BuffStatLevel, cNKMStaticBuffData.m_BuffTimeLevel, GetUnitDataGame().m_GameUnitUID, bUseMasterStat: true, bRangeSon: false);
		NKMBuffData buff = GetBuff(buffID);
		if (buff != null && buff.m_NKMBuffTemplet.m_Range > 0f)
		{
			ProcessBuffRange(bMasterOK: true, this, buff);
		}
		if (!cNKMStaticBuffData.m_fRange.IsNearlyZero())
		{
			List<NKMUnit> sortUnitListByNearDist = GetSortUnitListByNearDist();
			for (int i = 0; i < sortUnitListByNearDist.Count; i++)
			{
				NKMUnit cTargetUnit = sortUnitListByNearDist[i];
				CheckAndApplyStaticBuffToUnit(cNKMStaticBuffData, cTargetUnit);
			}
		}
	}

	private bool CheckAndApplyStaticBuffToUnit(NKMStaticBuffData cNKMStaticBuffData, NKMUnit cTargetUnit)
	{
		if (cTargetUnit.GetUnitSyncData().m_GameUnitUID == GetUnitSyncData().m_GameUnitUID)
		{
			return false;
		}
		if (!cTargetUnit.WillInteractWithGameUnits())
		{
			return false;
		}
		if (cTargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || cTargetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			return false;
		}
		if (!cNKMStaticBuffData.m_bMyTeam && !m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, cTargetUnit.GetUnitDataGame().m_NKM_TEAM_TYPE))
		{
			return false;
		}
		if (!cNKMStaticBuffData.m_bEnemy && m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, cTargetUnit.GetUnitDataGame().m_NKM_TEAM_TYPE))
		{
			return false;
		}
		if (cNKMStaticBuffData.m_fRange < Math.Abs(GetUnitSyncData().m_PosX - cTargetUnit.GetUnitSyncData().m_PosX))
		{
			return false;
		}
		cTargetUnit.AddBuffByStrID(cNKMStaticBuffData.m_BuffStrID, cNKMStaticBuffData.m_BuffStatLevel, cNKMStaticBuffData.m_BuffTimeLevel, GetUnitDataGame().m_GameUnitUID, bUseMasterStat: true, bRangeSon: false);
		return true;
	}

	public void ApplyStaticBuffToSummonedUnit(NKMUnit cTargetUnit)
	{
		if (cTargetUnit.GetUnitSyncData().m_GameUnitUID == GetUnitSyncData().m_GameUnitUID)
		{
			return;
		}
		foreach (NKMStaticBuffData listStaticBuffDatum in GetUnitTemplet().m_listStaticBuffData)
		{
			if (listStaticBuffDatum.m_bApplyToNewUnits)
			{
				CheckAndApplyStaticBuffToUnit(listStaticBuffDatum, cTargetUnit);
			}
		}
	}

	public void InitAndApplyStaticBuff()
	{
		m_listNKMStaticBuffDataRuntime.Clear();
		for (int i = 0; i < GetUnitTemplet().m_listStaticBuffData.Count; i++)
		{
			NKMStaticBuffData nKMStaticBuffData = GetUnitTemplet().m_listStaticBuffData[i];
			if (nKMStaticBuffData.m_bApplyOnSummon)
			{
				ApplyStaticBuffToGame(nKMStaticBuffData);
			}
			if (nKMStaticBuffData.m_fRebuffTime > 0f)
			{
				NKMStaticBuffDataRuntime nKMStaticBuffDataRuntime = new NKMStaticBuffDataRuntime();
				nKMStaticBuffDataRuntime.m_NKMStaticBuffData = nKMStaticBuffData;
				nKMStaticBuffDataRuntime.m_fReBuffTimeNow = nKMStaticBuffData.m_fRebuffTime;
				m_listNKMStaticBuffDataRuntime.Add(nKMStaticBuffDataRuntime);
			}
		}
	}

	protected virtual void ProcessEventText(bool bStateEnd = false)
	{
	}

	public virtual void ApplyEventText(NKMEventText cNKMEventText)
	{
	}

	protected void ProcessEventSpeed(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null || m_UnitStateNow.m_bNoMove || m_UnitTemplet.m_bNoMove || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOMOVE))
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventSpeed.Count; i++)
		{
			NKMEventSpeed nKMEventSpeed = m_UnitStateNow.m_listNKMEventSpeed[i];
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
				num = ((!nKMEventSpeed.m_bAnimTime) ? nKMEventSpeed.GetSpeedX(m_UnitFrameData.m_fStateTime, m_UnitFrameData.m_fSpeedX) : nKMEventSpeed.GetSpeedX(m_UnitFrameData.m_fAnimTime, m_UnitFrameData.m_fSpeedX));
				if (nKMEventSpeed.m_bAdd)
				{
					m_UnitFrameData.m_fSpeedX += num;
				}
				else if (nKMEventSpeed.m_bMultiply)
				{
					m_UnitFrameData.m_fSpeedX *= num;
				}
				else
				{
					m_UnitFrameData.m_fSpeedX = num;
				}
			}
			if (!nKMEventSpeed.m_SpeedY.IsNearlyEqual(-1f))
			{
				float num2 = 0f;
				num2 = ((!nKMEventSpeed.m_bAnimTime) ? nKMEventSpeed.GetSpeedY(m_UnitFrameData.m_fStateTime, m_UnitFrameData.m_fSpeedY) : nKMEventSpeed.GetSpeedY(m_UnitFrameData.m_fAnimTime, m_UnitFrameData.m_fSpeedY));
				if (nKMEventSpeed.m_bAdd)
				{
					m_UnitFrameData.m_fSpeedY += num2;
				}
				else if (nKMEventSpeed.m_bMultiply)
				{
					m_UnitFrameData.m_fSpeedY *= num2;
				}
				else
				{
					m_UnitFrameData.m_fSpeedY = num2;
				}
			}
		}
	}

	protected void ProcessEventSpeedX(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null || m_UnitStateNow.m_bNoMove || m_UnitTemplet.m_bNoMove || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOMOVE))
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventSpeedX.Count; i++)
		{
			NKMEventSpeedX nKMEventSpeedX = m_UnitStateNow.m_listNKMEventSpeedX[i];
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
				num = ((!nKMEventSpeedX.m_bAnimTime) ? nKMEventSpeedX.GetSpeed(m_UnitFrameData.m_fStateTime, m_UnitFrameData.m_fSpeedX) : nKMEventSpeedX.GetSpeed(m_UnitFrameData.m_fAnimTime, m_UnitFrameData.m_fSpeedX));
				if (nKMEventSpeedX.m_bAdd)
				{
					m_UnitFrameData.m_fSpeedX += num;
				}
				else if (nKMEventSpeedX.m_bMultiply)
				{
					m_UnitFrameData.m_fSpeedX *= num;
				}
				else
				{
					m_UnitFrameData.m_fSpeedX = num;
				}
			}
		}
	}

	protected void ProcessEventSpeedY(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null || m_UnitStateNow.m_bNoMove || m_UnitTemplet.m_bNoMove || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOMOVE))
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventSpeedY.Count; i++)
		{
			NKMEventSpeedY nKMEventSpeedY = m_UnitStateNow.m_listNKMEventSpeedY[i];
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
				num = ((!nKMEventSpeedY.m_bAnimTime) ? nKMEventSpeedY.GetSpeed(m_UnitFrameData.m_fStateTime, m_UnitFrameData.m_fSpeedY) : nKMEventSpeedY.GetSpeed(m_UnitFrameData.m_fAnimTime, m_UnitFrameData.m_fSpeedY));
				if (nKMEventSpeedY.m_bAdd)
				{
					m_UnitFrameData.m_fSpeedY += num;
				}
				else if (nKMEventSpeedY.m_bMultiply)
				{
					m_UnitFrameData.m_fSpeedY *= num;
				}
				else
				{
					m_UnitFrameData.m_fSpeedY = num;
				}
			}
		}
	}

	protected void ProcessEventMove(bool bStateEnd = false)
	{
		if (m_UnitStateNow != null && !m_UnitStateNow.m_bNoMove && !m_UnitTemplet.m_bNoMove && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOMOVE))
		{
			for (int i = 0; i < m_UnitStateNow.m_listNKMEventMove.Count; i++)
			{
				NKMEventMove cNKMEventMove = m_UnitStateNow.m_listNKMEventMove[i];
				ProcessEventMove(cNKMEventMove, bUseEventTimer: true, bStateEnd);
			}
		}
	}

	public void ProcessEventMove(NKMEventMove cNKMEventMove, bool bUseEventTimer, bool bStateEnd = false, bool bFromDE = false, NKMUnit attacker = null)
	{
		if (cNKMEventMove == null || !CheckEventCondition(cNKMEventMove.m_Condition))
		{
			return;
		}
		if (bUseEventTimer)
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
			if (!flag)
			{
				return;
			}
		}
		ApplyEventMove(cNKMEventMove, bFromDE, attacker);
	}

	public void ApplyEventMove(NKMEventMove cNKMEventMove, bool bFromDE = false, NKMUnit attacker = null)
	{
		(NKMEventPosData.EventPosExtraUnitType, NKMUnit) tuple = default((NKMEventPosData.EventPosExtraUnitType, NKMUnit));
		tuple.Item1 = NKMEventPosData.EventPosExtraUnitType.ATTACKER;
		tuple.Item2 = attacker;
		float num = GetEventPosX(cNKMEventMove.m_EventPosData, IsATeam(), tuple);
		if (cNKMEventMove.m_fMaxDistance >= 0f)
		{
			float value = num - m_UnitFrameData.m_PosXCalc;
			if (Math.Abs(value) > cNKMEventMove.m_fMaxDistance)
			{
				num = m_UnitFrameData.m_PosXCalc + (float)Math.Sign(value) * cNKMEventMove.m_fMaxDistance;
			}
		}
		float num2 = num;
		float eventPosY = GetEventPosY(cNKMEventMove.m_EventPosData);
		if (cNKMEventMove.m_bSavePosition || cNKMEventMove.m_EventPosData.m_MoveBase == NKMEventPosData.MoveBase.SAVE_ONLY)
		{
			SaveEventMovePosition(num2, eventPosY, cNKMEventMove.m_bLandMove);
			return;
		}
		m_EventMovePosX.StopTracking();
		m_EventMovePosJumpY.StopTracking();
		m_EventMovePosZ.StopTracking();
		if (attacker != null && IsEnemy(attacker) && (cNKMEventMove.m_MoveTime > 0f || cNKMEventMove.m_fSpeed > 0f))
		{
			float num3 = num2 - m_UnitFrameData.m_PosXCalc;
			float num4 = 1f - GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_BACK_RESIST);
			if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NO_DAMAGE_BACK_SPEED))
			{
				num4 = 0f;
			}
			float num5 = m_UnitTemplet.m_fDamageBackFactor * m_UnitTemplet.m_fDamageBackFactor;
			num2 = num3 * num4 * num5 + m_UnitFrameData.m_PosXCalc;
		}
		if (cNKMEventMove.m_fSpeed > 0f)
		{
			float fTime = Math.Abs(m_UnitFrameData.m_PosXCalc - num) / cNKMEventMove.m_fSpeed;
			m_EventMovePosX.SetNowValue(m_UnitFrameData.m_PosXCalc);
			m_EventMovePosX.SetTracking(num2, fTime, cNKMEventMove.m_MoveTrackingType);
			if (!cNKMEventMove.m_bLandMove)
			{
				m_EventMovePosJumpY.SetNowValue(m_UnitFrameData.m_JumpYPosCalc);
				m_EventMovePosJumpY.SetTracking(eventPosY, fTime, cNKMEventMove.m_MoveTrackingType);
			}
		}
		else if (cNKMEventMove.m_MoveTime > 0f)
		{
			m_EventMovePosX.SetNowValue(m_UnitFrameData.m_PosXCalc);
			m_EventMovePosX.SetTracking(num2, cNKMEventMove.m_MoveTime, cNKMEventMove.m_MoveTrackingType);
			if (!cNKMEventMove.m_bLandMove)
			{
				m_EventMovePosJumpY.SetNowValue(m_UnitFrameData.m_JumpYPosCalc);
				m_EventMovePosJumpY.SetTracking(eventPosY, cNKMEventMove.m_MoveTime, cNKMEventMove.m_MoveTrackingType);
			}
		}
		else
		{
			m_UnitFrameData.m_PosXCalc = num2;
			if (!cNKMEventMove.m_bLandMove)
			{
				m_UnitFrameData.m_JumpYPosCalc = eventPosY;
			}
		}
		if (bFromDE && cNKMEventMove.m_fSpeed <= 0f && cNKMEventMove.m_MoveTime <= 0f)
		{
			m_UnitSyncData.m_PosX = m_UnitFrameData.m_PosXCalc;
			if (!cNKMEventMove.m_bLandMove)
			{
				m_UnitSyncData.m_JumpYPos = m_UnitFrameData.m_JumpYPosCalc;
			}
		}
	}

	public void SaveEventMovePosition(float posX, float posY, bool bSaveXOnly)
	{
		if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			m_UnitSyncData.m_fSavedPosX = posX;
			if (!bSaveXOnly)
			{
				m_UnitSyncData.m_fSavedPosY = posY;
			}
			m_PushSyncData = true;
		}
	}

	public float GetEventPosX(NKMEventPosData posData, bool isATeam, params (NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] extraParams)
	{
		if (posData.m_fDefaultOffsetX != 0f && UseDefaultPosRequired(posData.m_MoveBase))
		{
			if (GetUnitSyncData().m_bRight)
			{
				return GetUnitFrameData().m_PosXCalc + posData.m_fDefaultOffsetX;
			}
			return GetUnitFrameData().m_PosXCalc - posData.m_fDefaultOffsetX;
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

	public bool UseDefaultPosRequired(NKMEventPosData.MoveBase moveBase)
	{
		return moveBase switch
		{
			NKMEventPosData.MoveBase.TARGET_UNIT => m_TargetUnit == null, 
			NKMEventPosData.MoveBase.SUB_TARGET_UNIT => m_SubTargetUnit == null, 
			NKMEventPosData.MoveBase.MY_SHIP => GetMyBoss() == null, 
			NKMEventPosData.MoveBase.ENEMY_SHIP => m_NKMGame.GetLiveEnemyBossUnit(GetTeam()) == null, 
			_ => false, 
		};
	}

	public virtual float GetBasePosX(NKMEventPosData.MoveBase moveBase, NKMEventPosData.MoveBaseType posType, bool isATeam, float mapPosFactor, params (NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] extraUnits)
	{
		switch (moveBase)
		{
		case NKMEventPosData.MoveBase.MAP_RATE:
			return m_NKMGame.GetMapTemplet().GetMapRatePos(mapPosFactor, isATeam);
		default:
			return GetEventUnitPos(posType, GetUnitFrameData().m_PosXCalc, GetUnitFrameData().m_PosXCalc, m_UnitTemplet.m_UnitSizeX, GetUnitSyncData().m_bRight);
		case NKMEventPosData.MoveBase.TARGET_UNIT:
			return GetEventUnitPos(posType, m_TargetUnit, GetUnitFrameData().m_LastTargetPosX);
		case NKMEventPosData.MoveBase.SUB_TARGET_UNIT:
			return GetEventUnitPos(posType, m_SubTargetUnit, GetUnitFrameData().m_PosXCalc);
		case NKMEventPosData.MoveBase.TRIGGER_TARGET_UNIT:
			return GetEventUnitPos(posType, CurrentTriggerTarget, GetUnitFrameData().m_PosXCalc);
		case NKMEventPosData.MoveBase.MY_SHIP:
		{
			NKMUnit myBoss = GetMyBoss();
			return GetEventUnitPos(posType, myBoss, m_NKMGame.GetMapTemplet().GetMapRatePos(0f, isATeam));
		}
		case NKMEventPosData.MoveBase.ENEMY_SHIP:
		{
			NKMUnit liveEnemyBossUnit = m_NKMGame.GetLiveEnemyBossUnit(GetTeam());
			return GetEventUnitPos(posType, liveEnemyBossUnit, m_NKMGame.GetMapTemplet().GetMapRatePos(1f, isATeam));
		}
		case NKMEventPosData.MoveBase.SHIP_SKILL_POS:
			return m_UnitFrameData.m_fShipSkillPosX;
		case NKMEventPosData.MoveBase.SAVED_POS:
			return m_UnitSyncData.m_fSavedPosX;
		case NKMEventPosData.MoveBase.ATTACKER_POS:
		{
			NKMUnit cTargetUnit2 = GetExtraUnit(NKMEventPosData.EventPosExtraUnitType.ATTACKER);
			return GetEventUnitPos(posType, cTargetUnit2, GetUnitFrameData().m_PosXCalc);
		}
		case NKMEventPosData.MoveBase.EVENT_RESPAWN_POS:
		{
			NKMUnit cTargetUnit = GetExtraUnit(NKMEventPosData.EventPosExtraUnitType.SUMMON_INVOKER);
			return GetEventUnitPos(posType, cTargetUnit, GetUnitFrameData().m_PosXCalc);
		}
		}
		NKMUnit GetExtraUnit(NKMEventPosData.EventPosExtraUnitType type)
		{
			(NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] array = extraUnits;
			for (int i = 0; i < array.Length; i++)
			{
				(NKMEventPosData.EventPosExtraUnitType, NKMUnit) tuple = array[i];
				if (type == tuple.Item1)
				{
					return tuple.Item2;
				}
			}
			return null;
		}
	}

	private float GetEventUnitPos(NKMEventPosData.MoveBaseType type, NKMUnit cTargetUnit, float defaultPosX)
	{
		if (cTargetUnit == null)
		{
			return defaultPosX;
		}
		return GetEventUnitPos(type, GetUnitFrameData().m_PosXCalc, cTargetUnit.GetUnitSyncData().m_PosX, cTargetUnit.GetUnitTemplet().m_UnitSizeX, cTargetUnit.GetUnitSyncData().m_bRight);
	}

	public static float GetEventUnitPos(NKMEventPosData.MoveBaseType type, float myPosX, float targetPosX, float targetSizeX, bool bTargetUnitRight)
	{
		switch (type)
		{
		default:
			return targetPosX;
		case NKMEventPosData.MoveBaseType.FRONT:
			if (!bTargetUnitRight)
			{
				return targetPosX - targetSizeX * 0.5f;
			}
			return targetPosX + targetSizeX * 0.5f;
		case NKMEventPosData.MoveBaseType.BACK:
			if (!bTargetUnitRight)
			{
				return targetPosX + targetSizeX * 0.5f;
			}
			return targetPosX - targetSizeX * 0.5f;
		case NKMEventPosData.MoveBaseType.NEAR:
		{
			if (targetPosX == myPosX)
			{
				return targetPosX;
			}
			float num2 = targetSizeX * 0.5f;
			if (!(myPosX < targetPosX))
			{
				return targetPosX + num2;
			}
			return targetPosX - num2;
		}
		case NKMEventPosData.MoveBaseType.FAR:
		{
			if (targetPosX == myPosX)
			{
				return targetPosX;
			}
			float num = targetSizeX * 0.5f;
			if (!(myPosX < targetPosX))
			{
				return targetPosX - num;
			}
			return targetPosX + num;
		}
		}
	}

	public virtual bool GetOffsetDirRight(NKMEventPosData.MoveOffset offsetType, float basePos, bool isATeam, float mapPosFactor, params (NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] extraUnits)
	{
		bool flag;
		switch (offsetType)
		{
		case NKMEventPosData.MoveOffset.ME:
		case NKMEventPosData.MoveOffset.MASTER_UNIT:
			flag = GetUnitFrameData().m_PosXCalc > basePos;
			break;
		case NKMEventPosData.MoveOffset.ME_INV:
			flag = GetUnitFrameData().m_PosXCalc <= basePos;
			break;
		default:
			flag = GetUnitSyncData().m_bRight;
			break;
		case NKMEventPosData.MoveOffset.MY_SHIP:
		case NKMEventPosData.MoveOffset.MY_SHIP_INV:
		{
			NKMUnit myBoss = GetMyBoss();
			flag = ((myBoss == null) ? (!isATeam) : ((GetUnitFrameData().m_PosXCalc < myBoss.GetUnitSyncData().m_PosX) ? true : false));
			if (offsetType == NKMEventPosData.MoveOffset.MY_SHIP_INV)
			{
				flag = !flag;
			}
			break;
		}
		case NKMEventPosData.MoveOffset.ENEMY_SHIP:
		case NKMEventPosData.MoveOffset.ENEMY_SHIP_INV:
		{
			NKMUnit liveEnemyBossUnit = m_NKMGame.GetLiveEnemyBossUnit(GetTeam());
			flag = ((liveEnemyBossUnit == null) ? (!isATeam) : ((GetUnitFrameData().m_PosXCalc < liveEnemyBossUnit.GetUnitSyncData().m_PosX) ? true : false));
			if (offsetType == NKMEventPosData.MoveOffset.ENEMY_SHIP_INV)
			{
				flag = !flag;
			}
			break;
		}
		case NKMEventPosData.MoveOffset.TEAM_DIR:
			flag = isATeam;
			break;
		case NKMEventPosData.MoveOffset.TARGET_UNIT:
		{
			float num = ((m_UnitSyncData.m_TargetUID <= 0 || m_TargetUnit == null) ? GetUnitFrameData().m_LastTargetPosX : m_TargetUnit.GetUnitSyncData().m_PosX);
			flag = basePos < num;
			break;
		}
		case NKMEventPosData.MoveOffset.TARGET_UNIT_INV:
		{
			float num2 = ((m_UnitSyncData.m_TargetUID <= 0 || m_TargetUnit == null) ? GetUnitFrameData().m_LastTargetPosX : m_TargetUnit.GetUnitSyncData().m_PosX);
			flag = basePos >= num2;
			break;
		}
		case NKMEventPosData.MoveOffset.TARGET_UNIT_LOOK_DIR:
			if (m_UnitSyncData.m_TargetUID > 0 && m_TargetUnit != null)
			{
				return m_TargetUnit.GetUnitSyncData().m_bRight;
			}
			return GetUnitSyncData().m_bRight;
		case NKMEventPosData.MoveOffset.SUB_TARGET_UNIT:
		{
			float num3 = ((m_UnitSyncData.m_SubTargetUID == 0 || m_SubTargetUnit == null) ? GetUnitFrameData().m_PosXCalc : m_SubTargetUnit.GetUnitSyncData().m_PosX);
			flag = basePos < num3;
			break;
		}
		case NKMEventPosData.MoveOffset.SUB_TARGET_UNIT_LOOK_DIR:
			if (m_UnitSyncData.m_SubTargetUID != 0 && m_SubTargetUnit != null)
			{
				return m_SubTargetUnit.GetUnitSyncData().m_bRight;
			}
			return GetUnitSyncData().m_bRight;
		case NKMEventPosData.MoveOffset.TRIGGER_TARGET_UNIT:
		{
			float num5 = ((CurrentTriggerTarget == null) ? GetUnitFrameData().m_LastTargetPosX : m_TargetUnit.GetUnitSyncData().m_PosX);
			flag = basePos < num5;
			break;
		}
		case NKMEventPosData.MoveOffset.TRIGGER_TARGET_UNIT_INV:
		{
			float num4 = ((CurrentTriggerTarget == null) ? GetUnitFrameData().m_LastTargetPosX : m_TargetUnit.GetUnitSyncData().m_PosX);
			flag = basePos >= num4;
			break;
		}
		case NKMEventPosData.MoveOffset.TRIGGER_TARGET_UNIT_LOOK_DIR:
			if (CurrentTriggerTarget != null)
			{
				return CurrentTriggerTarget.GetUnitSyncData().m_bRight;
			}
			return GetUnitSyncData().m_bRight;
		case NKMEventPosData.MoveOffset.MAP_RATE:
		{
			float mapRatePos = m_NKMGame.GetMapTemplet().GetMapRatePos(mapPosFactor, isATeam);
			flag = basePos < mapRatePos;
			break;
		}
		case NKMEventPosData.MoveOffset.SAVED_POS:
			flag = basePos < m_UnitSyncData.m_fSavedPosX;
			break;
		case NKMEventPosData.MoveOffset.ATTACKER_POS:
		{
			NKMUnit unit = GetExtraUnit(NKMEventPosData.EventPosExtraUnitType.ATTACKER);
			return IsUnitRightToBase(unit);
		}
		case NKMEventPosData.MoveOffset.ATTACKER_LOOK_DIR:
			return GetExtraUnit(NKMEventPosData.EventPosExtraUnitType.ATTACKER)?.GetUnitSyncData().m_bRight ?? GetUnitSyncData().m_bRight;
		}
		return flag;
		NKMUnit GetExtraUnit(NKMEventPosData.EventPosExtraUnitType type)
		{
			(NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] array = extraUnits;
			for (int i = 0; i < array.Length; i++)
			{
				(NKMEventPosData.EventPosExtraUnitType, NKMUnit) tuple = array[i];
				if (type == tuple.Item1)
				{
					return tuple.Item2;
				}
			}
			return null;
		}
		bool IsUnitRightToBase(NKMUnit nKMUnit)
		{
			if (nKMUnit != null)
			{
				return nKMUnit.GetUnitSyncData().m_PosX > basePos;
			}
			return GetUnitSyncData().m_bRight;
		}
	}

	public float GetEventPosY(NKMEventPosData posData)
	{
		return GetEventPosY(posData.m_MoveBase, posData.m_fOffsetY);
	}

	public float GetBasePosY(NKMEventPosData.MoveBase baseType)
	{
		switch (baseType)
		{
		case NKMEventPosData.MoveBase.TARGET_UNIT:
			if (m_UnitSyncData.m_TargetUID > 0 && m_TargetUnit != null)
			{
				return m_TargetUnit.GetUnitSyncData().m_JumpYPos;
			}
			return GetUnitFrameData().m_LastTargetJumpYPos;
		case NKMEventPosData.MoveBase.SUB_TARGET_UNIT:
			if (m_UnitSyncData.m_SubTargetUID != 0 && m_SubTargetUnit != null)
			{
				return m_SubTargetUnit.GetUnitSyncData().m_JumpYPos;
			}
			return m_UnitFrameData.m_JumpYPosCalc;
		case NKMEventPosData.MoveBase.TRIGGER_TARGET_UNIT:
			if (CurrentTriggerTarget != null)
			{
				return CurrentTriggerTarget.GetUnitSyncData().m_JumpYPos;
			}
			return m_UnitFrameData.m_JumpYPosCalc;
		case NKMEventPosData.MoveBase.SHIP_SKILL_POS:
			return 0f;
		case NKMEventPosData.MoveBase.SAVED_POS:
			return m_UnitSyncData.m_fSavedPosY;
		default:
			return m_UnitFrameData.m_JumpYPosCalc;
		}
	}

	public float GetEventPosY(NKMEventPosData.MoveBase baseType, float offsetY)
	{
		return GetBasePosY(baseType) + offsetY;
	}

	public float GetBasePosZ(NKMEventPosData.MoveBase baseType)
	{
		switch (baseType)
		{
		case NKMEventPosData.MoveBase.TARGET_UNIT:
			if (m_UnitSyncData.m_TargetUID > 0 && m_TargetUnit != null)
			{
				return m_TargetUnit.GetUnitSyncData().m_PosZ;
			}
			return GetUnitFrameData().m_LastTargetPosZ;
		case NKMEventPosData.MoveBase.SUB_TARGET_UNIT:
			if (m_UnitSyncData.m_SubTargetUID != 0 && m_SubTargetUnit != null)
			{
				return m_SubTargetUnit.GetUnitSyncData().m_PosZ;
			}
			return m_UnitFrameData.m_PosZCalc;
		case NKMEventPosData.MoveBase.TRIGGER_TARGET_UNIT:
			if (CurrentTriggerTarget != null)
			{
				return CurrentTriggerTarget.GetUnitSyncData().m_PosZ;
			}
			return m_UnitFrameData.m_PosZCalc;
		case NKMEventPosData.MoveBase.SHIP_SKILL_POS:
			return m_NKMGame.GetMapTemplet().m_fMinZ + (m_NKMGame.GetMapTemplet().m_fMaxZ - m_NKMGame.GetMapTemplet().m_fMinZ) / 2f;
		default:
			return m_UnitFrameData.m_PosZCalc;
		}
	}

	public float GetEventPosZ(NKMEventPosData.MoveBase baseType, float offsetZ)
	{
		return GetBasePosZ(baseType) + offsetZ;
	}

	public NKMUnit GetMyBoss()
	{
		return m_NKMGame.GetLiveMyBossUnit(GetTeam());
	}

	protected NKMDamageInst GetDamageInstAtk(int index)
	{
		if (m_listDamageInstAtk.Count <= index)
		{
			int num = index - m_listDamageInstAtk.Count;
			for (int i = 0; i <= num; i++)
			{
				NKMDamageInst item = new NKMDamageInst();
				m_listDamageInstAtk.Add(item);
			}
		}
		return m_listDamageInstAtk[index];
	}

	protected virtual void ProcessEventAttack(bool bStateEnd)
	{
		if (bStateEnd || m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventAttack.Count; i++)
		{
			NKMEventAttack nKMEventAttack = m_UnitStateNow.m_listNKMEventAttack[i];
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
			if (!flag)
			{
				continue;
			}
			NKMDamageInst damageInstAtk = GetDamageInstAtk(i);
			if (damageInstAtk.m_Templet == null)
			{
				damageInstAtk.m_Templet = NKMDamageManager.GetTempletByStrID(nKMEventAttack.m_DamageTempletName);
				damageInstAtk.m_AttackerType = NKM_REACTOR_TYPE.NRT_GAME_UNIT;
				damageInstAtk.m_AttackerGameUnitUID = m_UnitDataGame.m_GameUnitUID;
				damageInstAtk.m_AttackerUnitSkillTemplet = GetStateSkill(m_UnitStateNow);
				damageInstAtk.m_AttackerTeamType = GetUnitDataGame().m_NKM_TEAM_TYPE;
			}
			if (m_NKMGame.DamageCheck(damageInstAtk, nKMEventAttack))
			{
				if (nKMEventAttack.m_EffectName.Length > 1)
				{
					ProcessAttackHitEffect(nKMEventAttack);
				}
				if (nKMEventAttack.m_HitStateChange.Length > 1)
				{
					StateChange(nKMEventAttack.m_HitStateChange, bForceChange: false);
				}
			}
		}
	}

	protected virtual void ProcessAttackHitEffect(NKMEventAttack cNKMEventAttack)
	{
	}

	protected void ProcessEventStopTime(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null || (m_NKMGame.GetDungeonTemplet() != null && m_NKMGame.GetDungeonTemplet().m_bNoTimeStop))
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventStopTime.Count; i++)
		{
			NKMEventStopTime nKMEventStopTime = m_UnitStateNow.m_listNKMEventStopTime[i];
			if (nKMEventStopTime == null || !CheckEventCondition(nKMEventStopTime.m_Condition))
			{
				continue;
			}
			bool flag = false;
			if (nKMEventStopTime.m_bStateEndTime && bStateEnd)
			{
				flag = true;
			}
			else if (EventTimer(nKMEventStopTime.m_bAnimTime, nKMEventStopTime.m_fEventTime, bOneTime: true) && !nKMEventStopTime.m_bStateEndTime)
			{
				flag = true;
			}
			if (flag)
			{
				if (!nKMEventStopTime.m_fStopReserveTime.IsNearlyEqual(-1f))
				{
					SetStopReserveTime(nKMEventStopTime.m_fStopReserveTime);
				}
				m_NKMGame.SetStopTime(m_UnitDataGame.m_GameUnitUID, nKMEventStopTime.m_fStopTime, nKMEventStopTime.m_bStopSelf, nKMEventStopTime.m_bStopSummonee, nKMEventStopTime.m_StopTimeIndex);
			}
		}
	}

	protected void ProcessEventInvincible(bool bStateEnd)
	{
		if (m_UnitStateNow == null || bStateEnd)
		{
			return;
		}
		m_UnitFrameData.m_bInvincible = m_UnitTemplet.m_Invincible;
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventInvincible.Count; i++)
		{
			NKMEventInvincible nKMEventInvincible = m_UnitStateNow.m_listNKMEventInvincible[i];
			if (nKMEventInvincible != null && CheckEventCondition(nKMEventInvincible.m_Condition) && EventTimer(nKMEventInvincible.m_bAnimTime, nKMEventInvincible.m_fEventTimeMin, nKMEventInvincible.m_fEventTimeMax))
			{
				m_UnitFrameData.m_bInvincible = true;
			}
		}
	}

	protected void ProcessEventInvincibleGlobal(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER && m_NKMGame.GetDungeonType() == NKM_DUNGEON_TYPE.NDT_DAMAGE_ACCRUE && m_bBoss && m_NKMGame.IsATeam(GetUnitDataGame().m_NKM_TEAM_TYPE) && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE))
		{
			ApplyStatusTime(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE, 600f, this);
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventInvincibleGlobal.Count; i++)
		{
			NKMEventInvincibleGlobal nKMEventInvincibleGlobal = m_UnitStateNow.m_listNKMEventInvincibleGlobal[i];
			if (nKMEventInvincibleGlobal != null && CheckEventCondition(nKMEventInvincibleGlobal.m_Condition))
			{
				bool flag = false;
				if (nKMEventInvincibleGlobal.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventInvincibleGlobal.m_bAnimTime, nKMEventInvincibleGlobal.m_fEventTime, bOneTime: true) && !nKMEventInvincibleGlobal.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag)
				{
					ApplyStatusTime(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE, nKMEventInvincibleGlobal.m_InvincibleTime, this, bForceOverwrite: false, bServerOnly: false, bImmediate: true);
				}
			}
		}
	}

	protected void ProcessEventSuperArmor(bool bStateEnd)
	{
		if (bStateEnd || m_UnitStateNow == null)
		{
			return;
		}
		m_UnitFrameData.m_SuperArmorLevel = NKM_SUPER_ARMOR_LEVEL.NSAL_NO;
		if (m_UnitTemplet.m_SuperArmorLevel != NKM_SUPER_ARMOR_LEVEL.NSAL_INVALID)
		{
			m_UnitFrameData.m_SuperArmorLevel = m_UnitTemplet.m_SuperArmorLevel;
		}
		if (m_UnitStateNow.m_SuperArmorLevel != NKM_SUPER_ARMOR_LEVEL.NSAL_INVALID)
		{
			m_UnitFrameData.m_SuperArmorLevel = m_UnitStateNow.m_SuperArmorLevel;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventSuperArmor.Count; i++)
		{
			NKMEventSuperArmor nKMEventSuperArmor = m_UnitStateNow.m_listNKMEventSuperArmor[i];
			if (nKMEventSuperArmor != null && EventTimer(nKMEventSuperArmor.m_bAnimTime, nKMEventSuperArmor.m_fEventTimeMin, nKMEventSuperArmor.m_fEventTimeMax))
			{
				m_UnitFrameData.m_SuperArmorLevel = nKMEventSuperArmor.m_SuperArmorLevel;
			}
		}
	}

	protected virtual void ProcessEventSound(bool bStateEnd = false)
	{
	}

	public virtual void ApplyEventSound(NKMEventSound cNKMEventSound)
	{
	}

	protected virtual void ProcessEventColor(bool bStateEnd = false)
	{
	}

	public virtual void ApplyEventColor(NKMEventColor cNKMEventColor)
	{
	}

	protected virtual void ProcessEventCameraCrash(bool bStateEnd = false)
	{
	}

	public virtual void ApplyEventCameraCrash(NKMEventCameraCrash cNKMEventCameraCrash)
	{
	}

	protected virtual void ProcessEventCameraMove(bool bStateEnd = false)
	{
	}

	public virtual void ApplyEventCameraMove(NKMEventCameraMove cNKMEventCameraMove)
	{
	}

	protected virtual void ProcessEventFadeWorld(bool bStateEnd)
	{
	}

	public virtual void ApplyEventFadeWorld(NKMEventFadeWorld cNKMEventFadeWorld)
	{
	}

	protected virtual void ProcessEventDissolve(bool bStateEnd = false)
	{
	}

	public virtual void ApplyEventDissolve(NKMEventDissolve cNKMEventDissolve)
	{
	}

	protected virtual void ProcessEventMotionBlur(bool bStateEnd)
	{
	}

	public virtual void ApplyEventMotionBlur(NKMEventMotionBlur cNKMEventMotionBlur)
	{
	}

	protected virtual void ProcessEventEffect(bool bStateEnd = false)
	{
	}

	public virtual void ApplyEventEffect(NKMEventEffect cNKMEventEffect)
	{
	}

	protected virtual void ProcessEventHyperSkillCutIn(bool bStateEnd)
	{
	}

	public virtual void ApplyEventHyperSkillCutIn(NKMEventHyperSkillCutIn cNKMEventHyperSkillCutIn)
	{
	}

	protected void ProcessEventDamageEffect(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null)
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
					value2.SetRight(m_UnitSyncData.m_bRight);
					value2.SetFollowPos(m_UnitSyncData.m_PosX, m_UnitSyncData.m_JumpYPos, m_UnitSyncData.m_PosZ);
					linkedListNode2 = linkedListNode2.Next;
				}
			}
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventDamageEffect.Count; i++)
		{
			NKMEventDamageEffect nKMEventDamageEffect = m_UnitStateNow.m_listNKMEventDamageEffect[i];
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
					NKMUnitSkillTemplet stateSkill = GetStateSkill(m_UnitStateNow);
					ApplyEventDamageEffect(nKMEventDamageEffect, stateSkill, m_UnitSyncData.m_PosX, m_UnitSyncData.m_JumpYPos, m_UnitSyncData.m_PosZ);
				}
			}
		}
	}

	public void ApplyEventDamageEffect(NKMEventDamageEffect cNKMEventDamageEffect, NKMUnitSkillTemplet cUnitStateSkillTemplet, float fPosX, float fJumpYPos, float fPosZ)
	{
		if (!cNKMEventDamageEffect.m_bIgnoreNoTarget || m_TargetUnit != null)
		{
			m_NKMVector3Temp.x = GetBasePosX(cNKMEventDamageEffect.m_EventPosData.m_MoveBase, cNKMEventDamageEffect.m_EventPosData.m_MoveBaseType, IsATeam(), cNKMEventDamageEffect.m_EventPosData.m_fMapPosFactor);
			m_NKMVector3Temp.y = GetBasePosY(cNKMEventDamageEffect.m_EventPosData.m_MoveBase);
			m_NKMVector3Temp.z = (cNKMEventDamageEffect.m_bUseMyZPos ? m_UnitSyncData.m_PosZ : GetBasePosZ(cNKMEventDamageEffect.m_EventPosData.m_MoveBase));
			float zScaleFactor = m_NKMGame.GetZScaleFactor(GetUnitSyncData().m_PosZ);
			short targetGameUID = ((!cNKMEventDamageEffect.m_bUseSubTarget || m_UnitSyncData.m_SubTargetUID == 0) ? m_UnitSyncData.m_TargetUID : m_UnitSyncData.m_SubTargetUID);
			if (cNKMEventDamageEffect.m_bIgnoreTarget)
			{
				targetGameUID = 0;
			}
			string templetID = cNKMEventDamageEffect.m_DEName;
			if (m_NKMGame.IsPVP(bUseDevOption: true) && cNKMEventDamageEffect.m_DENamePVP.Length > 1)
			{
				templetID = cNKMEventDamageEffect.m_DENamePVP;
			}
			bool bRight = (cNKMEventDamageEffect.m_bFlipRight ? (!m_UnitSyncData.m_bRight) : m_UnitSyncData.m_bRight);
			NKMUnitSkillTemplet cUnitSkillTemplet = ((cNKMEventDamageEffect.m_SkillType == NKM_SKILL_TYPE.NST_INVALID) ? cUnitStateSkillTemplet : m_UnitData.GetUnitSkillTempletByType(cNKMEventDamageEffect.m_SkillType));
			NKMDamageEffect nKMDamageEffect = m_NKMGame.GetDEManager().UseDamageEffect(templetID, m_UnitSyncData.m_GameUnitUID, targetGameUID, cUnitSkillTemplet, GetUnitFrameData().m_PhaseNow, m_NKMVector3Temp.x, m_NKMVector3Temp.y, m_NKMVector3Temp.z, cNKMEventDamageEffect.m_EventPosData.m_MoveOffset, cNKMEventDamageEffect.m_EventPosData.m_fMapPosFactor, bRight, cNKMEventDamageEffect.m_EventPosData.m_fOffsetX * zScaleFactor, cNKMEventDamageEffect.m_EventPosData.m_fOffsetY * zScaleFactor, cNKMEventDamageEffect.m_EventPosData.m_fOffsetZ, cNKMEventDamageEffect.m_fAddRotate, cNKMEventDamageEffect.m_bUseZScale, cNKMEventDamageEffect.m_fSpeedFactorX, cNKMEventDamageEffect.m_fSpeedFactorY, cNKMEventDamageEffect.m_fReserveTime);
			if (nKMDamageEffect != null && (cNKMEventDamageEffect.m_bHold || cNKMEventDamageEffect.m_bStateEndStop))
			{
				nKMDamageEffect.SetHoldFollowData(cNKMEventDamageEffect.m_FollowType, cNKMEventDamageEffect.m_FollowTime, cNKMEventDamageEffect.m_FollowUpdateTime);
				nKMDamageEffect.SetStateEndDie(cNKMEventDamageEffect.m_bStateEndStop);
				m_linklistDamageEffect.AddLast(nKMDamageEffect);
			}
		}
	}

	protected void ProcessEventDEStateChange(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null || m_linklistDamageEffect == null || m_linklistDamageEffect.First == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventDEStateChange.Count; i++)
		{
			NKMEventDEStateChange nKMEventDEStateChange = m_UnitStateNow.m_listNKMEventDEStateChange[i];
			if (nKMEventDEStateChange != null && CheckEventCondition(nKMEventDEStateChange.m_Condition))
			{
				bool flag = false;
				if (nKMEventDEStateChange.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventDEStateChange.m_bAnimTime, nKMEventDEStateChange.m_fEventTime, bOneTime: true) && !nKMEventDEStateChange.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag)
				{
					ApplyEventDEStateChange(nKMEventDEStateChange);
				}
			}
		}
	}

	public void ApplyEventDEStateChange(NKMEventDEStateChange cNKMEventDEStateChange)
	{
		foreach (NKMDamageEffect item in m_linklistDamageEffect)
		{
			if (item != null && item.GetTemplet() != null && (!cNKMEventDEStateChange.m_bStateEndTime || !item.GetStateEndDie()) && item.GetTemplet().m_DamageEffectID.Equals(cNKMEventDEStateChange.m_DamageEffectID))
			{
				item.StateChangeByUnitState(cNKMEventDEStateChange.m_ChangeState);
			}
		}
	}

	protected void ProcessEventGameSpeed(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventGameSpeed.Count; i++)
		{
			NKMEventGameSpeed nKMEventGameSpeed = m_UnitStateNow.m_listNKMEventGameSpeed[i];
			if (nKMEventGameSpeed != null && CheckEventCondition(nKMEventGameSpeed.m_Condition))
			{
				bool flag = false;
				if (nKMEventGameSpeed.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventGameSpeed.m_bAnimTime, nKMEventGameSpeed.m_fEventTime, bOneTime: true) && !nKMEventGameSpeed.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag)
				{
					nKMEventGameSpeed.ApplyEvent(m_NKMGame, this);
				}
			}
		}
	}

	protected void ProcessEventAnimSpeed(bool bStateEnd)
	{
		if (bStateEnd || m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventAnimSpeed.Count; i++)
		{
			NKMEventAnimSpeed nKMEventAnimSpeed = m_UnitStateNow.m_listNKMEventAnimSpeed[i];
			if (nKMEventAnimSpeed != null && CheckEventCondition(nKMEventAnimSpeed.m_Condition))
			{
				bool flag = false;
				if (EventTimer(nKMEventAnimSpeed.m_bAnimTime, nKMEventAnimSpeed.m_fEventTime, bOneTime: true))
				{
					flag = true;
				}
				if (flag)
				{
					nKMEventAnimSpeed.ApplyEvent(m_NKMGame, this);
				}
			}
		}
	}

	protected void ProcessEventBuff(bool bStateEnd = false)
	{
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER || m_UnitStateNow == null)
		{
			return;
		}
		if (bStateEnd)
		{
			foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in m_UnitFrameData.m_dicBuffData)
			{
				NKMBuffData value = dicBuffDatum.Value;
				if (value != null && value.m_BuffSyncData.m_MasterGameUnitUID == GetUnitSyncData().m_GameUnitUID && value.m_StateEndRemove)
				{
					value.m_StateEndCheck = true;
				}
			}
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventBuff.Count; i++)
		{
			NKMEventBuff nKMEventBuff = m_UnitStateNow.m_listNKMEventBuff[i];
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

	protected virtual void ProcessEventRespawn(bool bStateEnd = false)
	{
	}

	public virtual bool ApplyEventRespawn(NKMEventRespawn cNKMEventRespawn, NKMUnit invoker, float rollbackTime = 0f)
	{
		return true;
	}

	protected virtual void ProcessEventUnitChange(bool bStateEnd = false)
	{
		if (m_UnitStateNow != null && m_UnitStateNow.m_NKMEventUnitChange != null && CheckEventCondition(m_UnitStateNow.m_NKMEventUnitChange.m_Condition))
		{
			bool flag = false;
			if (m_UnitStateNow.m_NKMEventUnitChange.m_bStateEndTime && bStateEnd)
			{
				flag = true;
			}
			else if (EventTimer(m_UnitStateNow.m_NKMEventUnitChange.m_bAnimTime, m_UnitStateNow.m_NKMEventUnitChange.m_fEventTime, bOneTime: true) && !m_UnitStateNow.m_NKMEventUnitChange.m_bStateEndTime)
			{
				flag = true;
			}
			if (flag)
			{
				EventDie(bImmediate: true, bCheckAllDie: false, bUnitChange: true);
			}
		}
	}

	protected void ProcessEventDie(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventDie.Count; i++)
		{
			NKMEventDie nKMEventDie = m_UnitStateNow.m_listNKMEventDie[i];
			if (nKMEventDie != null && CheckEventCondition(nKMEventDie.m_Condition))
			{
				bool flag = false;
				if (nKMEventDie.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventDie.m_bAnimTime, nKMEventDie.m_fEventTime, bOneTime: true) && !nKMEventDie.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag)
				{
					EventDie(nKMEventDie.m_bImmediateDie);
				}
			}
		}
	}

	protected void ProcessEventChangeState(bool bStateEnd = false)
	{
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER || m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventChangeState.Count; i++)
		{
			NKMEventChangeState nKMEventChangeState = m_UnitStateNow.m_listNKMEventChangeState[i];
			if (nKMEventChangeState != null && CheckEventCondition(nKMEventChangeState.m_Condition))
			{
				bool flag = false;
				if (nKMEventChangeState.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventChangeState.m_bAnimTime, nKMEventChangeState.m_fEventTime, bOneTime: true) && !nKMEventChangeState.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag)
				{
					nKMEventChangeState.ApplyEvent(m_NKMGame, this);
				}
			}
		}
	}

	protected void ProcessEventBuffUnitDie(NKMBuffData cNKMBuffData, NKMUnit invoker)
	{
		for (int i = 0; i < GetUnitTemplet().m_listBuffUnitDieEvent.Count; i++)
		{
			NKMBuffUnitDieEvent nKMBuffUnitDieEvent = GetUnitTemplet().m_listBuffUnitDieEvent[i];
			if (nKMBuffUnitDieEvent == null || !CheckEventCondition(nKMBuffUnitDieEvent.m_Condition) || nKMBuffUnitDieEvent.m_BuffStrID.CompareTo(cNKMBuffData.m_NKMBuffTemplet.m_BuffStrID) != 0 || m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
			{
				continue;
			}
			for (int j = 0; j < nKMBuffUnitDieEvent.m_listNKMEventRespawn.Count; j++)
			{
				NKMEventRespawn nKMEventRespawn = nKMBuffUnitDieEvent.m_listNKMEventRespawn[j];
				if (nKMEventRespawn != null && CheckEventCondition(nKMEventRespawn.m_Condition))
				{
					ApplyEventRespawn(nKMEventRespawn, invoker);
				}
			}
			for (int k = 0; k < nKMBuffUnitDieEvent.m_listNKMEventCost.Count; k++)
			{
				NKMEventCost nKMEventCost = nKMBuffUnitDieEvent.m_listNKMEventCost[k];
				if (nKMEventCost != null && CheckEventCondition(nKMEventCost.m_Condition))
				{
					SetCost(nKMEventCost.m_AdjustCostAlly, nKMEventCost.m_AdjustCostEnemy);
				}
			}
			if (nKMBuffUnitDieEvent.m_fSkillCoolTime > 0f && GetUnitTemplet().m_listSkillStateData.Count > 0)
			{
				NKMAttackStateData nKMAttackStateData = GetUnitTemplet().m_listSkillStateData[0];
				if (nKMAttackStateData != null)
				{
					NKMUnitState unitState = GetUnitState(nKMAttackStateData.m_StateName);
					if (unitState != null && GetStateCoolTime(unitState) > nKMBuffUnitDieEvent.m_fSkillCoolTime)
					{
						SetStateCoolTime(unitState, nKMBuffUnitDieEvent.m_fSkillCoolTime);
					}
				}
			}
			if (nKMBuffUnitDieEvent.m_fHyperSkillCoolTime > 0f && GetUnitTemplet().m_listHyperSkillStateData.Count > 0)
			{
				NKMAttackStateData nKMAttackStateData2 = GetUnitTemplet().m_listHyperSkillStateData[0];
				if (nKMAttackStateData2 != null)
				{
					NKMUnitState unitState2 = GetUnitState(nKMAttackStateData2.m_StateName);
					if (unitState2 != null && GetStateCoolTime(unitState2) > nKMBuffUnitDieEvent.m_fHyperSkillCoolTime)
					{
						SetStateCoolTime(unitState2, nKMBuffUnitDieEvent.m_fHyperSkillCoolTime);
					}
				}
			}
			if (!nKMBuffUnitDieEvent.m_fSkillCoolTimeAdd.IsNearlyZero() && GetUnitTemplet().m_listSkillStateData.Count > 0)
			{
				NKMAttackStateData nKMAttackStateData3 = GetUnitTemplet().m_listSkillStateData[0];
				if (nKMAttackStateData3 != null)
				{
					NKMUnitState unitState3 = GetUnitState(nKMAttackStateData3.m_StateName);
					if (unitState3 != null)
					{
						SetStateCoolTimeAdd(unitState3, nKMBuffUnitDieEvent.m_fSkillCoolTimeAdd);
					}
				}
			}
			if (!nKMBuffUnitDieEvent.m_fHyperSkillCoolTimeAdd.IsNearlyZero() && GetUnitTemplet().m_listHyperSkillStateData.Count > 0)
			{
				NKMAttackStateData nKMAttackStateData4 = GetUnitTemplet().m_listHyperSkillStateData[0];
				if (nKMAttackStateData4 != null)
				{
					NKMUnitState unitState4 = GetUnitState(nKMAttackStateData4.m_StateName);
					if (unitState4 != null)
					{
						SetStateCoolTimeAdd(unitState4, nKMBuffUnitDieEvent.m_fHyperSkillCoolTimeAdd);
					}
				}
			}
			if (nKMBuffUnitDieEvent.m_fHPRate > 0f)
			{
				float fHeal = GetMaxHP() * nKMBuffUnitDieEvent.m_fHPRate;
				SetHeal(fHeal, GetUnitSyncData().m_GameUnitUID);
			}
			if (nKMBuffUnitDieEvent.m_OutBuffStrID.Length > 1)
			{
				AddBuffByStrID(nKMBuffUnitDieEvent.m_OutBuffStrID, nKMBuffUnitDieEvent.m_OutBuffStatLevel, nKMBuffUnitDieEvent.m_OutBuffTimeLevel, m_UnitDataGame.m_GameUnitUID, bUseMasterStat: true, bRangeSon: false, bStateEndRemove: false, (byte)nKMBuffUnitDieEvent.m_Overlap);
			}
		}
	}

	public void EventDie(bool bImmediate, bool bCheckAllDie = true, bool bUnitChange = false)
	{
		GetUnitSyncData().SetHP(0f);
		if (bImmediate)
		{
			SetDying(bForce: false, bUnitChange);
			SetDie(bCheckAllDie);
		}
		m_PushSyncData = true;
	}

	protected void ProcessEventAgro(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventAgro.Count; i++)
		{
			NKMEventAgro nKMEventAgro = m_UnitStateNow.m_listNKMEventAgro[i];
			if (nKMEventAgro != null && CheckEventCondition(nKMEventAgro.m_Condition))
			{
				bool flag = false;
				if (nKMEventAgro.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventAgro.m_bAnimTime, nKMEventAgro.m_fEventTime, bOneTime: true) && !nKMEventAgro.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag)
				{
					SetAgro(nKMEventAgro.m_bGetAgro, nKMEventAgro.m_fRange, nKMEventAgro.m_fDurationTime, nKMEventAgro.m_MaxCount, nKMEventAgro.m_bUseUnitSize);
				}
			}
		}
	}

	protected void ProcessEventHeal(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventHeal.Count; i++)
		{
			NKMEventHeal nKMEventHeal = m_UnitStateNow.m_listNKMEventHeal[i];
			if (nKMEventHeal != null && CheckEventCondition(nKMEventHeal.m_Condition) && (nKMEventHeal.m_bStateEndTime ? bStateEnd : EventTimer(nKMEventHeal.m_bAnimTime, nKMEventHeal.m_fEventTime, bOneTime: true)))
			{
				SetEventHeal(nKMEventHeal, GetUnitSyncData().m_PosX, this);
			}
		}
	}

	protected void ProcessEventCost(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventCost.Count; i++)
		{
			NKMEventCost nKMEventCost = m_UnitStateNow.m_listNKMEventCost[i];
			if (nKMEventCost != null && CheckEventCondition(nKMEventCost.m_Condition))
			{
				bool flag = false;
				if (nKMEventCost.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventCost.m_bAnimTime, nKMEventCost.m_fEventTime, bOneTime: true) && !nKMEventCost.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag)
				{
					SetCost(nKMEventCost.m_AdjustCostAlly, nKMEventCost.m_AdjustCostEnemy);
				}
			}
		}
	}

	protected void ProcessEventDispel(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventDispel.Count; i++)
		{
			NKMEventDispel nKMEventDispel = m_UnitStateNow.m_listNKMEventDispel[i];
			if (nKMEventDispel != null && CheckEventCondition(nKMEventDispel.m_Condition))
			{
				bool flag = false;
				if (nKMEventDispel.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventDispel.m_bAnimTime, nKMEventDispel.m_fEventTime, bOneTime: true) && !nKMEventDispel.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag)
				{
					SetDispel(nKMEventDispel);
				}
			}
		}
	}

	public float GetModifiedDMGAfterEventDEF(float fAttackPosX, float fOrgDmg)
	{
		if (m_UnitStateNow == null)
		{
			return fOrgDmg;
		}
		float num = fOrgDmg;
		float num2 = 0f;
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventDefence.Count; i++)
		{
			NKMEventDefence nKMEventDefence = m_UnitStateNow.m_listNKMEventDefence[i];
			if (nKMEventDefence == null || !CheckEventCondition(nKMEventDefence.m_Condition))
			{
				continue;
			}
			bool flag = false;
			if (EventTimer(nKMEventDefence.m_bAnimTime, nKMEventDefence.m_fEventTimeMin, nKMEventDefence.m_fEventTimeMax))
			{
				if (nKMEventDefence.m_bDefenceBack && nKMEventDefence.m_bDefenceFront)
				{
					flag = true;
				}
				else if (nKMEventDefence.m_bDefenceFront)
				{
					if (GetUnitSyncData().m_bRight && GetUnitSyncData().m_PosX <= fAttackPosX)
					{
						flag = true;
					}
					else if (!GetUnitSyncData().m_bRight && GetUnitSyncData().m_PosX >= fAttackPosX)
					{
						flag = true;
					}
				}
				else if (nKMEventDefence.m_bDefenceBack)
				{
					if (GetUnitSyncData().m_bRight && GetUnitSyncData().m_PosX >= fAttackPosX)
					{
						flag = true;
					}
					else if (!GetUnitSyncData().m_bRight && GetUnitSyncData().m_PosX <= fAttackPosX)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				int skillLevelIfNowSkillState = GetSkillLevelIfNowSkillState();
				if (skillLevelIfNowSkillState > 0)
				{
					num2 = (float)(skillLevelIfNowSkillState - 1) * nKMEventDefence.m_fDamageReducePerSkillLevel;
				}
				num -= num * (nKMEventDefence.m_fDamageReduceRate + nKMEventDefence.m_fDamageReduceRate * num2);
			}
		}
		return num;
	}

	protected void ProcessEventStun(bool bStateEnd = false)
	{
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER || m_UnitStateNow == null)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventStun.Count; i++)
		{
			NKMEventStun nKMEventStun = m_UnitStateNow.m_listNKMEventStun[i];
			if (nKMEventStun != null && CheckEventCondition(nKMEventStun.m_Condition))
			{
				bool flag = false;
				if (nKMEventStun.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventStun.m_bAnimTime, nKMEventStun.m_fEventTime, bOneTime: true) && !nKMEventStun.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag)
				{
					SetStun(this, nKMEventStun.m_fStunTime, nKMEventStun.m_fRange, nKMEventStun.m_bUseUnitSize, nKMEventStun.m_MaxCount, nKMEventStun.m_fStunTimePerSkillLevel, nKMEventStun.m_StunCountPerSkillLevel, nKMEventStun.m_IgnoreStyleType);
				}
			}
		}
	}

	protected void ProcessEventCatch(bool bStateEnd = false)
	{
		if (m_UnitStateNow == null)
		{
			return;
		}
		if (IsDyingOrDie() && GetUnitSyncData().m_CatcherGameUnitUID != 0)
		{
			GetUnitSyncData().m_CatcherGameUnitUID = 0;
		}
		NKMUnit unit = m_NKMGame.GetUnit(GetUnitSyncData().m_CatcherGameUnitUID);
		if ((unit == null || unit.IsDyingOrDie() || unit.GetUnitStateNow().m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_DAMAGE || unit.GetUnitStateNow().m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_DIE || unit.GetUnitStateNow().m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_ASTAND) && GetUnitSyncData().m_CatcherGameUnitUID != 0)
		{
			GetUnitSyncData().m_CatcherGameUnitUID = 0;
		}
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventCatchEnd.Count; i++)
		{
			NKMEventCatchEnd nKMEventCatchEnd = m_UnitStateNow.m_listNKMEventCatchEnd[i];
			if (nKMEventCatchEnd == null || !CheckEventCondition(nKMEventCatchEnd.m_Condition))
			{
				continue;
			}
			bool flag = false;
			if (nKMEventCatchEnd.m_bStateEndTime && bStateEnd)
			{
				flag = true;
			}
			else if (EventTimer(nKMEventCatchEnd.m_bAnimTime, nKMEventCatchEnd.m_fEventTime, bOneTime: true) && !nKMEventCatchEnd.m_bStateEndTime)
			{
				flag = true;
			}
			if (!flag)
			{
				continue;
			}
			for (int j = 0; j < m_NKMGame.GetUnitChain().Count; j++)
			{
				NKMUnit nKMUnit = m_NKMGame.GetUnitChain()[j];
				if (nKMUnit != null && nKMUnit.GetUnitSyncData().m_CatcherGameUnitUID == GetUnitSyncData().m_GameUnitUID)
				{
					nKMUnit.GetUnitSyncData().m_CatcherGameUnitUID = 0;
				}
			}
		}
	}

	protected void ProcessEventChangeCooltime(bool bStateEnd)
	{
		if (m_UnitStateNow == null || m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			return;
		}
		for (int i = 0; i < m_UnitStateNow.m_listNKMEventChangeCooltime.Count; i++)
		{
			NKMEventChangeCooltime nKMEventChangeCooltime = m_UnitStateNow.m_listNKMEventChangeCooltime[i];
			if (nKMEventChangeCooltime != null && bStateEnd == nKMEventChangeCooltime.m_bStateEndTime && (bStateEnd || EventTimer(nKMEventChangeCooltime.bAnimTime, nKMEventChangeCooltime.EventStartTime, bOneTime: true)) && CheckEventCondition(nKMEventChangeCooltime.m_Condition))
			{
				nKMEventChangeCooltime.ApplyEvent(m_NKMGame, this);
			}
		}
	}

	protected void ProcessStateEvent<T>(List<T> lstEvent, bool bStateEnd = false) where T : NKMUnitStateEventOneTime
	{
		if (m_UnitStateNow != null)
		{
			for (int i = 0; i < lstEvent.Count; i++)
			{
				lstEvent[i]?.ProcessEvent(m_NKMGame, this, bStateEnd);
			}
		}
	}

	protected void ProcessStateEventServerOnly<T>(List<T> lstEvent, bool bStateEnd = false) where T : NKMUnitStateEventOneTime
	{
		if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER && m_UnitStateNow != null)
		{
			for (int i = 0; i < lstEvent.Count; i++)
			{
				lstEvent[i]?.ProcessEvent(m_NKMGame, this, bStateEnd);
			}
		}
	}

	protected void ProcessAutoShipSkill()
	{
		if (m_UnitStateNow == null || m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER || m_NKMGame.GetGameRuntimeData().m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY || (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_ASTAND && m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_ATTACK && m_UnitStateNow.m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_DAMAGE) || m_listShipSkillTemplet.Count <= 0)
		{
			return;
		}
		NKMGameRuntimeTeamData myRuntimeTeamData = m_NKMGame.GetGameRuntimeData().GetMyRuntimeTeamData(GetUnitDataGame().m_NKM_TEAM_TYPE);
		if (myRuntimeTeamData == null || !myRuntimeTeamData.m_bAutoRespawn)
		{
			return;
		}
		m_fCheckUseShipSkillAuto -= m_DeltaTime;
		if (m_fCheckUseShipSkillAuto <= 0f)
		{
			m_fCheckUseShipSkillAuto = 0f;
		}
		if (m_fCheckUseShipSkillAuto > 0f)
		{
			return;
		}
		for (int i = 0; i < m_listShipSkillTemplet.Count; i++)
		{
			NKMShipSkillTemplet nKMShipSkillTemplet = m_listShipSkillTemplet[i];
			if (nKMShipSkillTemplet == null || CanUseShipSkill(nKMShipSkillTemplet.m_ShipSkillID) != NKM_ERROR_CODE.NEC_OK)
			{
				continue;
			}
			switch (nKMShipSkillTemplet.m_NKM_SHIP_SKILL_USE_TYPE)
			{
			case NKM_SHIP_SKILL_USE_TYPE.NSSUT_ANY:
				if (UseShipSkill(nKMShipSkillTemplet.m_ShipSkillID, GetUnitSyncData().m_PosX))
				{
					return;
				}
				break;
			case NKM_SHIP_SKILL_USE_TYPE.NSSUT_ENEMY:
			{
				NKMUnit sortUnit = GetSortUnit(bNearFirst: true, bExceptMyTeam: true, bExceptEnemyTeam: false);
				if (sortUnit != null && UseShipSkill(nKMShipSkillTemplet.m_ShipSkillID, sortUnit.GetUnitSyncData().m_PosX))
				{
					return;
				}
				break;
			}
			case NKM_SHIP_SKILL_USE_TYPE.NSSUT_MY_TEAM:
			{
				NKMUnit sortUnit2 = GetSortUnit(bNearFirst: false, bExceptMyTeam: false);
				if (sortUnit2 != null && UseShipSkill(nKMShipSkillTemplet.m_ShipSkillID, sortUnit2.GetUnitSyncData().m_PosX))
				{
					return;
				}
				break;
			}
			case NKM_SHIP_SKILL_USE_TYPE.NSSUT_SHIP_ATTACKED:
				if (!(m_UnitFrameData.m_fDamageBeforeFrame <= 0f) && UseShipSkill(nKMShipSkillTemplet.m_ShipSkillID, GetUnitSyncData().m_PosX))
				{
					return;
				}
				break;
			}
		}
	}

	protected virtual bool ProcessDangerCharge()
	{
		if (m_UnitStateNow == null)
		{
			return false;
		}
		if (GetUnitFrameData().m_fDangerChargeTime <= 0f)
		{
			return false;
		}
		if (m_UnitStateNow.m_DangerCharge.m_fChargeTime <= 0f)
		{
			return false;
		}
		if (GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			return false;
		}
		if (m_NKMGame.GetGameRuntimeData().m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY)
		{
			return false;
		}
		if (GetUnitFrameData().m_fDangerChargeTime > 0f)
		{
			if (m_UnitStateNow.m_DangerCharge.m_fCancelDamageRate > 0f && GetUnitFrameData().m_fDangerChargeDamage >= GetMaxHP(m_UnitStateNow.m_DangerCharge.m_fCancelDamageRate))
			{
				StateChange(m_UnitStateNow.m_DangerCharge.m_CancelState);
				return true;
			}
			if (m_UnitStateNow.m_DangerCharge.m_CancelHitCount > 0 && GetUnitFrameData().m_DangerChargeHitCount >= m_UnitStateNow.m_DangerCharge.m_CancelHitCount)
			{
				StateChange(m_UnitStateNow.m_DangerCharge.m_CancelState);
				return true;
			}
			if (m_NKMGame.GetWorldStopTime() <= 0f)
			{
				GetUnitFrameData().m_fDangerChargeTime -= m_DeltaTime;
			}
			if (GetUnitFrameData().m_fDangerChargeTime < 0f)
			{
				GetUnitFrameData().m_fDangerChargeTime = 0f;
				if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
				{
					StateChange(m_UnitStateNow.m_DangerCharge.m_SuccessState);
				}
				return true;
			}
		}
		return false;
	}

	public NKM_ERROR_CODE CanUseShipSkill(int m_ShipSkillID)
	{
		if (GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY || GetUnitSyncData().GetHP() <= 0f)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_SHIP_SKILL_ACK_NO_UNIT;
		}
		NKMShipSkillTemplet shipSkillTempletByID = NKMShipSkillManager.GetShipSkillTempletByID(m_ShipSkillID);
		if (shipSkillTempletByID == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_SHIP_SKILL_ACK_NO_SHIP_SKILL;
		}
		if (shipSkillTempletByID.m_NKM_SKILL_TYPE != NKM_SKILL_TYPE.NST_SHIP_ACTIVE)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_SHIP_SKILL_ACK_NO_SHIP_ACTIVE_TYPE;
		}
		if (GetStateCoolTime(shipSkillTempletByID.m_UnitStateName) > 0f)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_SHIP_SKILL_ACK_GAME_STATE_COOL_TIME;
		}
		NKMUnitState unitStateNow = GetUnitStateNow();
		if (unitStateNow == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_SHIP_SKILL_ACK_NO_UNIT;
		}
		if (unitStateNow.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER || unitStateNow.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_SKILL)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_SHIP_SKILL_ACK_NO_ALREADY_USE_SKILL;
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE))
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_SHIP_SKILL_ACK_NO_SILENCE;
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP))
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_SHIP_SKILL_ACK_NO_SLEEP;
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_FEAR) || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_FREEZE) || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_HOLD))
		{
			return NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_SHIP_SKILL_ACK_NO_SILENCE;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public float GetEventStateTime(bool bAnim, float eventTime)
	{
		if (bAnim)
		{
			if (m_UnitFrameData.m_fAnimSpeed == 0f)
			{
				return 0f;
			}
			return eventTime / m_UnitFrameData.m_fAnimSpeed;
		}
		return eventTime;
	}

	public bool EventTimer(bool bAnim, float fTime, bool bOneTime)
	{
		if (bAnim)
		{
			return EventTimer(fTime, bOneTime, m_UnitFrameData.m_fAnimTimeBack, m_UnitFrameData.m_fAnimTime, m_EventTimeStampAnim);
		}
		return EventTimer(fTime, bOneTime, m_UnitFrameData.m_fStateTimeBack, m_UnitFrameData.m_fStateTime, m_EventTimeStampState);
	}

	private bool EventTimer(float fTimeTarget, bool bOneTime, float fTimeBack, float fTimeNow, Dictionary<float, NKMTimeStamp> dicTimeStamp)
	{
		if ((fTimeTarget > fTimeBack && fTimeTarget <= fTimeNow) || (fTimeTarget.IsNearlyZero() && fTimeNow.IsNearlyZero()))
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
			if (m_UnitFrameData.m_fAnimTime >= fTimeMin && m_UnitFrameData.m_fAnimTime <= fTimeMax)
			{
				flag = true;
			}
		}
		else if (m_UnitFrameData.m_fStateTime >= fTimeMin && m_UnitFrameData.m_fStateTime <= fTimeMax)
		{
			flag = true;
		}
		if (!flag && EventTimer(bAnim, fTimeMin, bOneTime: true))
		{
			flag = true;
		}
		return flag;
	}

	public bool RollbackEventTimer(bool bAnim, float eventTime)
	{
		if (bAnim)
		{
			return eventTime < m_UnitFrameData.m_fAnimTime;
		}
		return eventTime < m_UnitFrameData.m_fStateTime;
	}

	protected void ProcessBuffRange(bool bMasterOK, NKMUnit masterUnit, NKMBuffData cNKMBuffData)
	{
		int num = 0;
		if (!bMasterOK || masterUnit == null || masterUnit.GetUnitSyncData().m_GameUnitUID != GetUnitSyncData().m_GameUnitUID || !(cNKMBuffData.m_NKMBuffTemplet.m_Range > 0f) || cNKMBuffData.m_NKMBuffTemplet.m_RangeOverlap)
		{
			return;
		}
		List<NKMUnit> unitChain = m_NKMGame.GetUnitChain();
		if (unitChain == null)
		{
			return;
		}
		for (int i = 0; i < unitChain.Count; i++)
		{
			NKMUnit nKMUnit = unitChain[i];
			if (nKMUnit == null || !IsUnitAffectedByBuffRange(nKMUnit, bMasterOK, masterUnit, cNKMBuffData))
			{
				continue;
			}
			num++;
			if (num > cNKMBuffData.m_NKMBuffTemplet.m_RangeSonCount)
			{
				break;
			}
			short num2 = cNKMBuffData.m_BuffSyncData.m_BuffID;
			bool flag = nKMUnit.IsAlly(cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID);
			if (!flag && num2 > 0)
			{
				num2 = (short)(-num2);
			}
			if (nKMUnit.IsBuffLive(num2))
			{
				NKMBuffData buff = nKMUnit.GetBuff(num2, !flag);
				if (cNKMBuffData.m_BuffSyncData.m_OverlapCount == buff.m_BuffSyncData.m_OverlapCount && cNKMBuffData.m_BuffSyncData.m_BuffStatLevel == buff.m_BuffSyncData.m_BuffStatLevel && cNKMBuffData.m_BuffSyncData.m_BuffTimeLevel == buff.m_BuffSyncData.m_BuffTimeLevel)
				{
					continue;
				}
			}
			nKMUnit.AddBuffByID(cNKMBuffData.m_BuffSyncData.m_BuffID, cNKMBuffData.m_BuffSyncData.m_BuffStatLevel, cNKMBuffData.m_BuffSyncData.m_BuffTimeLevel, cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID, cNKMBuffData.m_BuffSyncData.m_bUseMasterStat, bRangeSon: true, bStateEndRemove: false, 1);
		}
	}

	private bool IsUnitAffectedByBuffRange(NKMUnit cNKMUnit, bool bMasterOK, NKMUnit masterUnit, NKMBuffData cNKMBuffData)
	{
		if (cNKMUnit == null)
		{
			return false;
		}
		if (cNKMUnit.GetUnitSyncData().m_GameUnitUID == GetUnitSyncData().m_GameUnitUID)
		{
			return false;
		}
		if (!cNKMUnit.WillInteractWithGameUnits())
		{
			return false;
		}
		if (cNKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || cNKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			return false;
		}
		if (cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonOnlyTarget && masterUnit.GetUnitSyncData().m_TargetUID != cNKMUnit.GetUnitDataGame().m_GameUnitUID)
		{
			return false;
		}
		if (cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonOnlySubTarget && masterUnit.GetUnitSyncData().m_SubTargetUID != cNKMUnit.GetUnitDataGame().m_GameUnitUID)
		{
			return false;
		}
		bool flag = false;
		if (cNKMBuffData.m_NKMBuffTemplet.m_AffectMasterTeam && !m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE))
		{
			flag = true;
		}
		if (cNKMBuffData.m_NKMBuffTemplet.m_AffectMasterEnemyTeam && m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE))
		{
			flag = true;
		}
		if (!flag)
		{
			return false;
		}
		float num = 0f;
		num = (cNKMBuffData.m_NKMBuffTemplet.IsFixedPosBuff() ? cNKMUnit.GetDist(cNKMBuffData.m_fBuffPosX, cNKMBuffData.m_NKMBuffTemplet.m_bUseUnitSize) : cNKMUnit.GetDist(this, cNKMBuffData.m_NKMBuffTemplet.m_bUseUnitSize));
		if (cNKMBuffData.m_NKMBuffTemplet.m_Range < num)
		{
			return false;
		}
		return true;
	}

	protected void ProcessBuffRangeOverlap(bool bMasterOK, NKMUnit masterUnit, NKMBuffData cNKMBuffData)
	{
		if (!bMasterOK || masterUnit == null || masterUnit.GetUnitSyncData().m_GameUnitUID != GetUnitSyncData().m_GameUnitUID || cNKMBuffData.m_NKMBuffTemplet.m_Range <= 0f || !cNKMBuffData.m_NKMBuffTemplet.m_RangeOverlap)
		{
			return;
		}
		List<NKMUnit> unitChain = m_NKMGame.GetUnitChain();
		if (unitChain == null)
		{
			return;
		}
		int num = 0;
		foreach (NKMUnit item in unitChain)
		{
			if (IsUnitAffectedByBuffRange(item, bMasterOK, masterUnit, cNKMBuffData) && item.IsBuffAllowed(cNKMBuffData, bRangeSon: true))
			{
				num++;
				if (num >= 255)
				{
					break;
				}
				if (num >= cNKMBuffData.m_NKMBuffTemplet.m_RangeSonCount)
				{
					num = cNKMBuffData.m_NKMBuffTemplet.m_RangeSonCount;
					break;
				}
			}
		}
		if (IsBuffLive(cNKMBuffData.m_BuffSyncData.m_BuffID))
		{
			SetBuffLevel(cNKMBuffData.m_BuffSyncData.m_BuffID, Convert.ToByte(num + 1), cNKMBuffData.m_BuffSyncData.m_BuffTimeLevel);
		}
	}

	public short CompareUnitAndValueAwakenFirst(short lhsGameUID, float lhsValue, short rhsGameUID, float rhsValue, bool awakenFirst)
	{
		NKMUnit unit = m_NKMGame.GetUnit(lhsGameUID);
		NKMUnit unit2 = m_NKMGame.GetUnit(rhsGameUID);
		if (unit == null)
		{
			return rhsGameUID;
		}
		if (unit2 == null)
		{
			return lhsGameUID;
		}
		if (awakenFirst)
		{
			bool bAwaken = unit.GetUnitTempletBase().m_bAwaken;
			bool bAwaken2 = unit2.GetUnitTempletBase().m_bAwaken;
			if (bAwaken && !bAwaken2)
			{
				return lhsGameUID;
			}
			if (bAwaken2 && !bAwaken)
			{
				return rhsGameUID;
			}
		}
		if (!(lhsValue > rhsValue))
		{
			return rhsGameUID;
		}
		return lhsGameUID;
	}

	public bool WillAffectedByBuff(NKMBuffData cNKMBuffData)
	{
		NKMUnit unit = m_NKMGame.GetUnit(cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID, bChain: true, bPool: true);
		bool bMasterOK = unit != null && unit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY;
		return WillAffectedByBuff(bMasterOK, unit, cNKMBuffData);
	}

	protected bool WillAffectedByBuff(bool bMasterOK, NKMUnit masterUnit, NKMBuffData cNKMBuffData)
	{
		if (cNKMBuffData.m_NKMBuffTemplet.m_AffectMe)
		{
			return true;
		}
		if (cNKMBuffData.m_NKMBuffTemplet.m_AffectMasterTeam && bMasterOK && masterUnit != null && masterUnit.GetUnitSyncData().m_GameUnitUID != GetUnitSyncData().m_GameUnitUID && !m_NKMGame.IsEnemy(masterUnit.GetUnitDataGame().m_NKM_TEAM_TYPE, GetUnitDataGame().m_NKM_TEAM_TYPE))
		{
			return true;
		}
		if (cNKMBuffData.m_NKMBuffTemplet.m_AffectMasterEnemyTeam && bMasterOK && masterUnit != null && masterUnit.GetUnitSyncData().m_GameUnitUID != GetUnitSyncData().m_GameUnitUID && m_NKMGame.IsEnemy(masterUnit.GetUnitDataGame().m_NKM_TEAM_TYPE, GetUnitDataGame().m_NKM_TEAM_TYPE))
		{
			return true;
		}
		return false;
	}

	protected void ProcessBuffAffect(bool bMasterOK, NKMUnit masterUnit, NKMBuffData cNKMBuffData, bool bBuffDamageOK)
	{
		bool num = WillAffectedByBuff(bMasterOK, masterUnit, cNKMBuffData);
		bool bAffect = cNKMBuffData.m_BuffSyncData.m_bAffect;
		if (num)
		{
			cNKMBuffData.m_BuffSyncData.m_bAffect = true;
			if (cNKMBuffData.m_NKMBuffTemplet.m_AddAttackUnitCount > 0)
			{
				GetUnitFrameData().m_AddAttackUnitCount += cNKMBuffData.m_NKMBuffTemplet.m_AddAttackUnitCount;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_fAddAttackRange > 0f)
			{
				GetUnitFrameData().m_fAddAttackRange += cNKMBuffData.m_NKMBuffTemplet.m_fAddAttackRange;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_bNotCastSummon && !IsBoss())
			{
				GetUnitFrameData().m_bNotCastSummon = cNKMBuffData.m_NKMBuffTemplet.m_bNotCastSummon;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_SuperArmorLevel > GetUnitFrameData().m_BuffSuperArmorLevel)
			{
				GetUnitFrameData().m_BuffSuperArmorLevel = cNKMBuffData.m_NKMBuffTemplet.m_SuperArmorLevel;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_fBarrierHP > 0f)
			{
				GetUnitFrameData().m_BarrierBuffData = cNKMBuffData;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_fDamageTransfer > 0f && cNKMBuffData.m_BuffSyncData.m_bRangeSon)
			{
				GetUnitFrameData().m_fDamageTransferGameUnitUID = cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID;
				GetUnitFrameData().m_fDamageTransfer = cNKMBuffData.m_NKMBuffTemplet.m_fDamageTransfer;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_bGuard)
			{
				GetUnitFrameData().m_GuardGameUnitUID = cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_fDamageReflection > 0f && cNKMBuffData.m_NKMBuffTemplet.m_fDamageReflection > GetUnitFrameData().m_fDamageReflection)
			{
				GetUnitFrameData().m_fDamageReflection = cNKMBuffData.m_NKMBuffTemplet.m_fDamageReflection;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_fHealFeedback > GetUnitFrameData().m_fHealFeedback)
			{
				GetUnitFrameData().m_fHealFeedback = cNKMBuffData.m_NKMBuffTemplet.m_fHealFeedback + cNKMBuffData.m_NKMBuffTemplet.m_fHealFeedbackPerLevel * (float)(cNKMBuffData.m_BuffSyncData.m_BuffStatLevel - 1);
				GetUnitFrameData().m_fHealFeedbackMasterGameUnitUID = cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_fHealTransfer > 0f && cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID != GetUnitDataGame().m_GameUnitUID && CompareUnitAndValueAwakenFirst(cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID, cNKMBuffData.m_NKMBuffTemplet.m_fHealTransfer, GetUnitFrameData().m_HealTransferMasterGameUnitUID, GetUnitFrameData().m_fHealTransfer, awakenFirst: true) == cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID)
			{
				GetUnitFrameData().m_fHealTransfer = cNKMBuffData.m_NKMBuffTemplet.m_fHealTransfer;
				GetUnitFrameData().m_HealTransferMasterGameUnitUID = cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_UnitLevel != 0)
			{
				m_bBuffUnitLevelChangedThisFrame = true;
				GetUnitFrameData().m_BuffUnitLevel += cNKMBuffData.m_NKMBuffTemplet.m_UnitLevel * cNKMBuffData.m_BuffSyncData.m_OverlapCount;
			}
			if (bBuffDamageOK)
			{
				bool flag = bMasterOK && cNKMBuffData.m_NKMBuffTemplet.m_NKMDamageTemplet != null;
				if (!cNKMBuffData.m_NKMBuffTemplet.m_bSystem && HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_BUFF_DAMAGE_IMMUNE))
				{
					flag = false;
				}
				if (flag)
				{
					cNKMBuffData.m_DamageInstBuff.m_DefenderUID = GetUnitDataGame().m_GameUnitUID;
					cNKMBuffData.m_DamageInstBuff.m_ReActResult = cNKMBuffData.m_DamageInstBuff.m_Templet.m_ReActType;
					if (!cNKMBuffData.m_NKMBuffTemplet.IsFixedPosBuff())
					{
						cNKMBuffData.m_DamageInstBuff.m_AttackerPosX = GetUnitSyncData().m_PosX;
					}
					else
					{
						cNKMBuffData.m_DamageInstBuff.m_AttackerPosX = cNKMBuffData.m_fBuffPosX;
					}
					cNKMBuffData.m_DamageInstBuff.m_AttackerPosZ = GetUnitSyncData().m_PosZ;
					cNKMBuffData.m_DamageInstBuff.m_AttackerPosJumpY = GetUnitSyncData().m_JumpYPos;
					cNKMBuffData.m_DamageInstBuff.m_bAttackerRight = GetUnitSyncData().m_bRight;
					cNKMBuffData.m_DamageInstBuff.m_bAttackerAwaken = false;
					cNKMBuffData.m_DamageInstBuff.m_AttackerAddAttackUnitCount = 0;
					cNKMBuffData.m_DamageInstBuff.m_bEvade = false;
					DamageReact(cNKMBuffData.m_DamageInstBuff, bBuffDamage: true);
					if (cNKMBuffData.m_DamageInstBuff.m_ReActResult == NKM_REACT_TYPE.NRT_NO || cNKMBuffData.m_DamageInstBuff.m_ReActResult == NKM_REACT_TYPE.NRT_INVINCIBLE)
					{
						return;
					}
					if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER && masterUnit != null)
					{
						NKMStatData statAtk = masterUnit.GetUnitFrameData().m_StatData;
						NKMUnitData unitdataAtk = masterUnit.GetUnitData();
						if (!cNKMBuffData.m_BuffSyncData.m_bUseMasterStat)
						{
							statAtk = null;
							unitdataAtk = null;
						}
						NKM_DAMAGE_RESULT_TYPE eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL;
						bool bInstaKill = false;
						float fOrgDmg;
						if (m_NKMGame.GetGameData().m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PRACTICE && m_NKMGame.GetGameRuntimeData().m_bPracticeFixedDamage)
						{
							fOrgDmg = NKMUnitStatManager.GetAttackFactorDamage(cNKMBuffData.m_NKMBuffTemplet.m_NKMDamageTemplet.m_DamageTempletBase, cNKMBuffData.m_DamageInstBuff.m_AttackerUnitSkillTemplet, IsBoss());
						}
						else if (WillInstaKilled(cNKMBuffData.m_NKMBuffTemplet.m_NKMDamageTemplet))
						{
							fOrgDmg = GetNowHP() * 2f;
							AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_INSTA_KILL);
							bInstaKill = true;
						}
						else
						{
							fOrgDmg = NKMUnitStatManager.GetFinalDamage(m_NKMGame.IsPVP(bUseDevOption: true), statAtk, GetUnitFrameData().m_StatData, unitdataAtk, null, this, cNKMBuffData.m_NKMBuffTemplet.m_NKMDamageTemplet, cNKMBuffData.m_DamageInstBuff.m_AttackerUnitSkillTemplet, bAttackCountOver: false, bBuffDamage: true, bEvade: false, out eNKM_DAMAGE_RESULT_TYPE, 0f, 0f, IsBoss(), 1f, bTrueDamage: false, bSplashHit: false, bForceCritical: false, bNoCritical: false);
						}
						fOrgDmg = GetModifiedDMGAfterEventDEF(cNKMBuffData.m_DamageInstBuff.m_AttackerPosX, fOrgDmg);
						AddDamage(bAttackCountOver: false, fOrgDmg, eNKM_DAMAGE_RESULT_TYPE, masterUnit.GetUnitDataGame().m_GameUnitUID, masterUnit.GetUnitDataGame().m_NKM_TEAM_TYPE, bPushSyncData: false, bNoRedirect: false, bInstaKill);
						if (cNKMBuffData.m_NKMBuffTemplet.m_NKMDamageTemplet.m_fCoolTimeDamage > 0f)
						{
							AddDamage(bAttackCountOver: false, cNKMBuffData.m_NKMBuffTemplet.m_NKMDamageTemplet.m_fCoolTimeDamage, NKM_DAMAGE_RESULT_TYPE.NDRT_COOL_TIME, masterUnit.GetUnitDataGame().m_GameUnitUID, masterUnit.GetUnitDataGame().m_NKM_TEAM_TYPE);
						}
						m_PushSyncData = true;
					}
				}
				if (bMasterOK && cNKMBuffData.m_NKMBuffTemplet.m_NKMEventHeal != null && m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER && CheckEventCondition(cNKMBuffData.m_NKMBuffTemplet.m_NKMEventHeal.m_Condition) && (!cNKMBuffData.m_BuffSyncData.m_bRangeSon || cNKMBuffData.m_NKMBuffTemplet.m_NKMEventHeal.m_fRangeMin.IsNearlyZero() || cNKMBuffData.m_NKMBuffTemplet.m_NKMEventHeal.m_fRangeMax.IsNearlyZero()))
				{
					if (!cNKMBuffData.m_fBuffPosX.IsNearlyZero())
					{
						SetEventHeal(cNKMBuffData.m_NKMBuffTemplet.m_NKMEventHeal, cNKMBuffData.m_fBuffPosX, this);
					}
					else
					{
						SetEventHeal(cNKMBuffData.m_NKMBuffTemplet.m_NKMEventHeal, GetUnitSyncData().m_PosX, this);
					}
				}
			}
			if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
			{
				float num2 = 0f;
				if (cNKMBuffData.m_NKMBuffTemplet.m_StatType1 == NKM_STAT_TYPE.NST_HP_REGEN_RATE)
				{
					num2 += NKMStatData.GetBuffStatVal(cNKMBuffData.m_NKMBuffTemplet.m_StatType1, cNKMBuffData.m_NKMBuffTemplet.m_StatValue1, cNKMBuffData.m_NKMBuffTemplet.m_StatAddPerLevel1, cNKMBuffData.m_BuffSyncData.m_BuffStatLevel, cNKMBuffData.m_BuffSyncData.m_OverlapCount);
				}
				if (cNKMBuffData.m_NKMBuffTemplet.m_StatType2 == NKM_STAT_TYPE.NST_HP_REGEN_RATE)
				{
					num2 += NKMStatData.GetBuffStatVal(cNKMBuffData.m_NKMBuffTemplet.m_StatType2, cNKMBuffData.m_NKMBuffTemplet.m_StatValue2, cNKMBuffData.m_NKMBuffTemplet.m_StatAddPerLevel2, cNKMBuffData.m_BuffSyncData.m_BuffStatLevel, cNKMBuffData.m_BuffSyncData.m_OverlapCount);
				}
				if (cNKMBuffData.m_NKMBuffTemplet.m_StatType3 == NKM_STAT_TYPE.NST_HP_REGEN_RATE)
				{
					num2 += NKMStatData.GetBuffStatVal(cNKMBuffData.m_NKMBuffTemplet.m_StatType3, cNKMBuffData.m_NKMBuffTemplet.m_StatValue3, cNKMBuffData.m_NKMBuffTemplet.m_StatAddPerLevel3, cNKMBuffData.m_BuffSyncData.m_BuffStatLevel, cNKMBuffData.m_BuffSyncData.m_OverlapCount);
				}
				if (num2 != 0f)
				{
					float num3 = RegenHPThisFrame(num2);
					NKMUnit unit = m_NKMGame.GetUnit(cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID, bChain: true, bPool: true);
					if (num3 < 0f)
					{
						if (unit != null && !unit.IsAlly(GetTeam()))
						{
							m_NKMGame.m_GameRecord.AddDamage(cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID, unit.GetTeam(), this, 0f - num3);
						}
					}
					else if (num3 > 0f)
					{
						float num4 = 0f;
						if (unit != null)
						{
							num4 = unit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HEAL_RATE);
						}
						float num5 = 1f + num4 - m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HEAL_REDUCE_RATE);
						if (num5 < 0f)
						{
							num5 = 0f;
						}
						if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOHEAL))
						{
							num5 = 0f;
						}
						num3 *= num5;
						m_NKMGame.m_GameRecord.AddHeal(cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID, num3);
					}
				}
			}
		}
		else
		{
			cNKMBuffData.m_BuffSyncData.m_bAffect = false;
		}
		if (bAffect != cNKMBuffData.m_BuffSyncData.m_bAffect)
		{
			m_bBuffChangedThisFrame = true;
			if (cNKMBuffData.m_BuffSyncData.m_bAffect)
			{
				BuffAffectEffect(cNKMBuffData);
			}
		}
	}

	public bool WillInstaKilled(NKMDamageTemplet damageTemplet)
	{
		if (damageTemplet == null)
		{
			return false;
		}
		if (damageTemplet.m_fInstantKillHPRate.IsNearlyZero())
		{
			return false;
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_INSTANTKILL))
		{
			return false;
		}
		if (GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			return false;
		}
		if (IsBoss())
		{
			return false;
		}
		if ((damageTemplet.m_fInstantKillAwaken || !GetUnitTemplet().m_UnitTempletBase.m_bAwaken) && damageTemplet.m_fInstantKillHPRate > GetHPRate())
		{
			return true;
		}
		return false;
	}

	public bool WillBeTargetted()
	{
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_EXILE))
		{
			return false;
		}
		return GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_ENV;
	}

	public bool WillInteractWithGameUnits()
	{
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_EXILE))
		{
			return false;
		}
		return GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_ENV;
	}

	protected virtual void BuffAffectEffect(NKMBuffData cNKMBuffData)
	{
	}

	protected void ProcessBuffDelete(bool bMasterOK, NKMUnit buffOwnerUnit, NKMBuffData cNKMBuffData)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (!cNKMBuffData.m_fLifeTime.IsNearlyEqual(-1f) && !cNKMBuffData.m_BuffSyncData.m_bRangeSon)
		{
			if (cNKMBuffData.m_fLifeTime <= 0f)
			{
				flag = true;
				flag2 = true;
			}
		}
		else if (cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID != m_UnitSyncData.m_GameUnitUID)
		{
			if (bMasterOK && buffOwnerUnit != null)
			{
				float num = 0f;
				if (!cNKMBuffData.m_NKMBuffTemplet.IsFixedPosBuff())
				{
					num = GetDist(buffOwnerUnit, cNKMBuffData.m_NKMBuffTemplet.m_bUseUnitSize);
				}
				else
				{
					NKMBuffData buff = buffOwnerUnit.GetBuff(cNKMBuffData.m_NKMBuffTemplet.m_BuffID);
					if (buff != null)
					{
						num = GetDist(buff.m_fBuffPosX, buff.m_NKMBuffTemplet.m_bUseUnitSize);
					}
					else
					{
						flag = true;
					}
				}
				if (cNKMBuffData.m_NKMBuffTemplet.m_Range > 0f && cNKMBuffData.m_NKMBuffTemplet.m_Range < num && !cNKMBuffData.m_NKMBuffTemplet.m_RangeOverlap)
				{
					flag = true;
				}
				if (cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonOnlyTarget && buffOwnerUnit.GetUnitSyncData().m_TargetUID != GetUnitDataGame().m_GameUnitUID)
				{
					flag = true;
				}
				if (cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonOnlySubTarget && buffOwnerUnit.GetUnitSyncData().m_SubTargetUID != GetUnitDataGame().m_GameUnitUID)
				{
					flag = true;
				}
				bool flag4 = false;
				if (cNKMBuffData.m_NKMBuffTemplet.m_AffectMasterTeam && buffOwnerUnit.IsAlly(GetTeam()))
				{
					flag4 = true;
				}
				if (cNKMBuffData.m_NKMBuffTemplet.m_AffectMasterEnemyTeam && !buffOwnerUnit.IsAlly(GetTeam()))
				{
					flag4 = true;
				}
				if (!flag4)
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
		}
		foreach (NKM_UNIT_STATUS_EFFECT item in cNKMBuffData.m_NKMBuffTemplet.m_ApplyStatus)
		{
			if (IsImmuneStatus(item) && cNKMBuffData.m_BuffSyncData.m_bAffect)
			{
				flag = true;
				break;
			}
		}
		if (!cNKMBuffData.m_fBarrierHP.IsNearlyEqual(-1f) && cNKMBuffData.m_fBarrierHP <= 0f)
		{
			flag = true;
			flag2 = true;
			if (cNKMBuffData.m_fBarrierHP.IsNearlyEqual(-2f))
			{
				flag3 = true;
			}
		}
		if (flag)
		{
			m_listBuffDelete.Add(cNKMBuffData.m_BuffSyncData.m_BuffID);
		}
		if (flag2)
		{
			if (cNKMBuffData.m_NKMBuffTemplet.m_FinalUnitStateChange.Length > 1)
			{
				StateChange(cNKMBuffData.m_NKMBuffTemplet.m_FinalUnitStateChange);
			}
			if (!flag3 && cNKMBuffData.m_NKMBuffTemplet.m_FinalBuffStrID.Length > 1)
			{
				m_listBuffAdd.Add(new NKMBuffCreateData(cNKMBuffData.m_NKMBuffTemplet.m_FinalBuffStrID, cNKMBuffData.m_BuffSyncData.m_BuffStatLevel, cNKMBuffData.m_BuffSyncData.m_BuffTimeLevel, cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID, _bUseMasterStat: true, _bRangeSon: false, _stateEndRemove: false, 1));
			}
		}
	}

	protected virtual void ProcessBuff()
	{
		float statFinal = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_ATTACK_SPEED_RATE);
		float statFinal2 = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_MOVE_SPEED_RATE);
		GetUnitFrameData().m_bNotCastSummon = false;
		GetUnitFrameData().m_fDamageTransferGameUnitUID = 0;
		GetUnitFrameData().m_fDamageTransfer = 0f;
		GetUnitFrameData().m_GuardGameUnitUID = 0;
		GetUnitFrameData().m_fDamageReflection = 0f;
		GetUnitFrameData().m_fHealFeedback = 0f;
		GetUnitFrameData().m_fHealFeedbackMasterGameUnitUID = 0;
		GetUnitFrameData().m_BuffUnitLevel = 0;
		GetUnitFrameData().m_AddAttackUnitCount = 0;
		GetUnitFrameData().m_fAddAttackRange = 0f;
		GetUnitFrameData().m_fHealTransfer = 0f;
		GetUnitFrameData().m_HealTransferMasterGameUnitUID = 0;
		m_listBuffDelete.Clear();
		m_listBuffAdd.Clear();
		bool flag = false;
		m_BuffProcessTime -= m_DeltaTime;
		if (m_BuffProcessTime <= 0f)
		{
			flag = true;
			m_BuffProcessTime = 0.3f;
		}
		bool bBuffDamageOK = false;
		m_BuffDamageTime -= m_DeltaTime;
		if (m_BuffDamageTime <= 0f)
		{
			bBuffDamageOK = true;
			m_BuffDamageTime = 1f;
		}
		m_UnitFrameData.m_BuffSuperArmorLevel = NKM_SUPER_ARMOR_LEVEL.NSAL_NO;
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in m_UnitFrameData.m_dicBuffData)
		{
			NKMBuffData value = dicBuffDatum.Value;
			if (value == null)
			{
				continue;
			}
			NKMUnit unit = m_NKMGame.GetUnit(value.m_BuffSyncData.m_MasterGameUnitUID);
			bool bMasterOK = false;
			if (unit != null && unit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY && unit.IsBuffLive(Math.Abs(value.m_BuffSyncData.m_BuffID)))
			{
				bMasterOK = true;
			}
			if (value.m_NKMBuffTemplet == null || value.m_BuffSyncData.m_BuffID != value.m_NKMBuffTemplet.m_BuffID)
			{
				value.m_NKMBuffTemplet = NKMBuffManager.GetBuffTempletByID(value.m_BuffSyncData.m_BuffID);
			}
			if (flag && m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
			{
				ProcessBuffRange(bMasterOK, unit, value);
				ProcessBuffRangeOverlap(bMasterOK, unit, value);
			}
			ProcessBuffAffect(bMasterOK, unit, value, bBuffDamageOK);
			if (!value.m_fLifeTime.IsNearlyEqual(-1f) && !value.m_NKMBuffTemplet.m_bInfinity && !value.m_BuffSyncData.m_bRangeSon)
			{
				bool flag2 = value.m_NKMBuffTemplet.m_bDebuff;
				if (value.m_BuffSyncData.m_bRangeSon)
				{
					flag2 = value.m_NKMBuffTemplet.m_bDebuffSon;
				}
				if (flag2)
				{
					float num = 1f / (1f - m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_CC_RESIST_RATE));
					value.m_fLifeTime -= m_DeltaTime * num;
				}
				else
				{
					value.m_fLifeTime -= m_DeltaTime;
				}
				if (value.m_fLifeTime < 0f)
				{
					if (value.m_NKMBuffTemplet.m_DecreaseOverlapOnTimeover > 0 || value.m_NKMBuffTemplet.m_DecreaseStatLevelOnTimeover > 0)
					{
						bool flag3 = false;
						if (value.m_NKMBuffTemplet.m_DecreaseOverlapOnTimeover > 0 && m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
						{
							int num2 = value.m_BuffSyncData.m_OverlapCount - value.m_NKMBuffTemplet.m_DecreaseOverlapOnTimeover;
							if (num2 > 0)
							{
								value.m_BuffSyncData.m_OverlapCount = (byte)num2;
								m_bBuffChangedThisFrame = true;
								m_bPushSimpleSyncData = true;
							}
							else
							{
								flag3 = true;
							}
						}
						if (value.m_NKMBuffTemplet.m_DecreaseStatLevelOnTimeover > 0 && m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
						{
							int num3 = value.m_BuffSyncData.m_BuffStatLevel - value.m_NKMBuffTemplet.m_DecreaseStatLevelOnTimeover;
							if (num3 > 0)
							{
								value.m_BuffSyncData.m_BuffStatLevel = (byte)num3;
								m_bBuffChangedThisFrame = true;
								m_bPushSimpleSyncData = true;
							}
							else
							{
								flag3 = true;
							}
						}
						if (flag3)
						{
							value.m_fLifeTime = 0f;
						}
						else
						{
							value.m_fLifeTime = value.GetLifeTimeMax();
						}
					}
					else
					{
						value.m_fLifeTime = 0f;
					}
				}
			}
			if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
			{
				ProcessBuffDelete(bMasterOK, unit, value);
				if (value.m_BuffSyncData.m_MasterGameUnitUID == GetUnitSyncData().m_GameUnitUID && value.m_StateEndRemove && value.m_StateEndCheck)
				{
					m_listBuffDelete.Add(value.m_BuffSyncData.m_BuffID);
				}
			}
		}
		if (m_listBuffDelete.Count > 0)
		{
			m_bBuffChangedThisFrame = true;
		}
		for (int i = 0; i < m_listBuffDelete.Count; i++)
		{
			DeleteBuff(m_listBuffDelete[i], NKMBuffTemplet.BuffEndDTType.End);
		}
		m_listBuffDelete.Clear();
		if (m_listBuffAdd.Count > 0)
		{
			m_bBuffChangedThisFrame = true;
			for (int j = 0; j < m_listBuffAdd.Count; j++)
			{
				AddBuffByStrID(m_listBuffAdd[j].m_buffID, m_listBuffAdd[j].m_buffStatLevel, m_listBuffAdd[j].m_buffTimeLevel, m_listBuffAdd[j].m_masterGameUnitUID, m_listBuffAdd[j].m_bUseMasterStat, m_listBuffAdd[j].m_bRangeSon, m_listBuffAdd[j].m_stateEndRemove, m_listBuffAdd[j].m_overlapCount);
			}
			m_listBuffAdd.Clear();
		}
		if (m_bBuffChangedThisFrame)
		{
			CheckAndCalculateBuffStat();
		}
		m_bBuffHPRateConserveRequired = false;
		float statFinal3 = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_ATTACK_SPEED_RATE);
		float statFinal4 = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_MOVE_SPEED_RATE);
		if (Math.Abs(statFinal3 - statFinal) > 0.01f || Math.Abs(statFinal4 - statFinal2) > 0.01f)
		{
			ChangeAnimSpeed();
		}
	}

	protected void ProcessStatusApply()
	{
		GetUnitFrameData().m_hsStatus.Clear();
		GetUnitFrameData().m_hsImmuneStatus.Clear();
		if (GetUnitData().m_DungeonRespawnUnitTemplet != null)
		{
			if (GetUnitData().m_DungeonRespawnUnitTemplet.m_hsImmuneStatus != null)
			{
				ApplyImmuneStatus(GetUnitData().m_DungeonRespawnUnitTemplet.m_hsImmuneStatus);
			}
			if (GetUnitData().m_DungeonRespawnUnitTemplet.m_hsApplyStatus != null)
			{
				ApplyStatus(GetUnitData().m_DungeonRespawnUnitTemplet.m_hsApplyStatus);
			}
		}
		m_lstTempStatus.Clear();
		m_lstTempStatus.AddRange(m_UnitFrameData.m_dicStatusTime.Keys);
		foreach (NKM_UNIT_STATUS_EFFECT item in m_lstTempStatus)
		{
			if (IsImmuneStatus(item))
			{
				m_UnitFrameData.m_dicStatusTime.Remove(item);
				continue;
			}
			float num = m_UnitFrameData.m_dicStatusTime[item];
			if (NKMUnitStatusTemplet.IsDebuff(item))
			{
				float num2 = 1f / (1f - m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_CC_RESIST_RATE));
				num -= m_DeltaTime * num2;
			}
			else
			{
				num -= m_DeltaTime;
			}
			m_UnitFrameData.m_dicStatusTime[item] = num;
			if (num <= 0f)
			{
				m_UnitFrameData.m_dicStatusTime.Remove(item);
			}
			else
			{
				ApplyStatus(item);
			}
		}
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in m_UnitFrameData.m_dicBuffData)
		{
			NKMBuffData value = dicBuffDatum.Value;
			if (value != null && (value.m_BuffSyncData.m_bAffect || WillAffectedByBuff(value)))
			{
				ApplyImmuneStatus(value.m_NKMBuffTemplet.m_ImmuneStatus);
				ApplyStatus(value.m_NKMBuffTemplet.m_ApplyStatus);
			}
		}
	}

	protected void ProcessStatusAffect()
	{
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NULLIFY_BARRIER) && GetUnitFrameData().m_BarrierBuffData != null && !GetUnitFrameData().m_BarrierBuffData.m_NKMBuffTemplet.m_bNotDispel)
		{
			GetUnitFrameData().m_BarrierBuffData.m_fBarrierHP = -2f;
		}
		if (!HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_CONFUSE))
		{
			m_UnitDataGame.m_NKM_TEAM_TYPE = m_UnitDataGame.m_NKM_TEAM_TYPE_ORG;
		}
		if (!HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_AIR_TO_GROUND))
		{
			if (!m_UnitStateNow.m_fAirHigh.IsNearlyEqual(-1f))
			{
				m_UnitFrameData.m_fTargetAirHigh = m_UnitStateNow.m_fAirHigh;
			}
			else
			{
				m_UnitFrameData.m_fTargetAirHigh = m_UnitTemplet.m_fAirHigh;
			}
		}
	}

	private void ProcessStaticBuff()
	{
		for (int i = 0; i < m_listNKMStaticBuffDataRuntime.Count; i++)
		{
			NKMStaticBuffDataRuntime nKMStaticBuffDataRuntime = m_listNKMStaticBuffDataRuntime[i];
			if (nKMStaticBuffDataRuntime != null)
			{
				nKMStaticBuffDataRuntime.m_fReBuffTimeNow -= m_DeltaTime;
				if (nKMStaticBuffDataRuntime.m_fReBuffTimeNow <= 0f)
				{
					ApplyStaticBuffToGame(nKMStaticBuffDataRuntime.m_NKMStaticBuffData);
					nKMStaticBuffDataRuntime.m_fReBuffTimeNow = nKMStaticBuffDataRuntime.m_NKMStaticBuffData.m_fRebuffTime;
				}
			}
		}
	}

	private void ProcessLeaguePvpRageBuff()
	{
		if (m_NKMGame.GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_PVP_LEAGUE)
		{
			return;
		}
		bool isUnit = m_UnitTemplet.m_UnitTempletBase.IsUnitStyleType();
		NKMPvpCommonConst.LeaguePvpConst leaguePvp = NKMPvpCommonConst.Instance.LeaguePvp;
		if (!leaguePvp.UserRangeBuff)
		{
			return;
		}
		NKMBuffTemplet rageBuff = leaguePvp.GetRageBuff(isUnit);
		if (rageBuff == null || GetBuff(rageBuff.m_BuffID) != null)
		{
			return;
		}
		NKMUnitData mainShip = m_NKMGame.GetGameData().GetTeamData(GetTeam()).m_MainShip;
		if (mainShip != null)
		{
			NKMUnit unit = m_NKMGame.GetUnit(mainShip.m_listGameUnitUID[0]);
			float maxHP = unit.GetMaxHP();
			float num = maxHP * leaguePvp.RageBuffShipHpRate;
			float hP = unit.GetUnitSyncData().GetHP();
			if (!(hP > num))
			{
				float num2 = hP * 100f / maxHP;
				long gameUID = m_NKMGame.GetGameData().m_GameUID;
				Log.Debug($"[NKMUnit] start rage buff. gameUid:{gameUID} buffId:{rageBuff.m_BuffStrID} shipHp:{hP}({num2:0.00}%) teamType:{GetTeam()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 9315);
				AddBuffByID(rageBuff.m_BuffID, 1, 1, GetUnitDataGame().m_GameUnitUID, bUseMasterStat: true, bRangeSon: false, bStateEndRemove: false, 1);
			}
		}
	}

	private void ProcessLeaguePvpDeadlineBuff()
	{
		if (m_NKMGame.GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_PVP_LEAGUE)
		{
			return;
		}
		bool isUnit = m_UnitTemplet.m_UnitTempletBase.IsUnitStyleType();
		NKMPvpCommonConst.LeaguePvpConst leaguePvp = NKMPvpCommonConst.Instance.LeaguePvp;
		if (!leaguePvp.UseDeadlineBuff)
		{
			return;
		}
		NKMBuffTemplet deadlineBuff = leaguePvp.GetDeadlineBuff(isUnit);
		if (deadlineBuff == null)
		{
			return;
		}
		float fRemainGameTime = m_NKMGame.GetGameRuntimeData().m_fRemainGameTime;
		float deadlineBuffConditionTimeMax = leaguePvp.GetDeadlineBuffConditionTimeMax();
		if (fRemainGameTime >= deadlineBuffConditionTimeMax)
		{
			return;
		}
		if (!leaguePvp.GetDeadlineBuffCondition(fRemainGameTime, out var result))
		{
			Log.Warn($"[NKMUnit] leaguePvp.deadlineBuff condition access failed. remainTime:{fRemainGameTime}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 9358);
			return;
		}
		NKMBuffData buff = GetBuff(deadlineBuff.m_BuffID);
		if (buff == null || buff.m_BuffSyncData.m_BuffStatLevel != result.BuffLevel)
		{
			long gameUID = m_NKMGame.GetGameData().m_GameUID;
			short gameUnitUID = GetUnitDataGame().m_GameUnitUID;
			Log.Debug($"[NKMUnit] start deadline buff. gameUid:{gameUID} gameUnitUid:{gameUnitUID} buffId:{deadlineBuff.m_BuffStrID} remainTime:{fRemainGameTime} buffLevel:{result.BuffLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 9370);
			AddBuffByID(deadlineBuff.m_BuffID, (byte)result.BuffLevel, 1, gameUnitUID, bUseMasterStat: true, bRangeSon: false, bStateEndRemove: false, 1);
		}
	}

	public short AddBuffByStrID(string buffStrID, byte buffLevel, byte buffTimeLevel, short masterGameUnitUID, bool bUseMasterStat, bool bRangeSon, bool bStateEndRemove = false, int overlapCount = 1)
	{
		NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(buffStrID);
		if (buffTempletByStrID == null)
		{
			return 0;
		}
		return AddBuffByID(buffTempletByStrID.m_BuffID, buffLevel, buffTimeLevel, masterGameUnitUID, bUseMasterStat, bRangeSon, bStateEndRemove, overlapCount);
	}

	public bool ExistDispelBuff(bool bDebuff = true)
	{
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in m_UnitFrameData.m_dicBuffData)
		{
			if (dicBuffDatum.Value != null && dicBuffDatum.Value.m_NKMBuffTemplet != null)
			{
				if (!bDebuff && dicBuffDatum.Value.m_NKMBuffTemplet.m_bDispelBuff)
				{
					return true;
				}
				if (!bDebuff && dicBuffDatum.Value.m_NKMBuffTemplet.m_bRangeSonDispelBuff && dicBuffDatum.Value.m_BuffSyncData.m_bRangeSon)
				{
					return true;
				}
				if (bDebuff && dicBuffDatum.Value.m_NKMBuffTemplet.m_bDispelDebuff)
				{
					return true;
				}
				if (bDebuff && dicBuffDatum.Value.m_NKMBuffTemplet.m_bRangeSonDispelDebuff && dicBuffDatum.Value.m_BuffSyncData.m_bRangeSon)
				{
					return true;
				}
			}
		}
		return false;
	}

	public virtual short AddBuffByID(short buffID, byte buffLevel, byte buffTimeLevel, short masterGameUnitUID, bool bUseMasterStat, bool bRangeSon, bool bStateEndRemove, int overlapCount)
	{
		if (m_UnitDataGame.m_NKM_TEAM_TYPE != m_UnitDataGame.m_NKM_TEAM_TYPE_ORG)
		{
			bool flag = true;
			NKMBuffTemplet buffTempletByID = NKMBuffManager.GetBuffTempletByID(buffID);
			if (buffTempletByID != null)
			{
				if (buffTempletByID.m_bSystem)
				{
					flag = false;
				}
				if (buffTempletByID.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_CONFUSE))
				{
					flag = false;
				}
			}
			if (flag)
			{
				return 0;
			}
		}
		NKMBuffData nKMBuffData = (NKMBuffData)m_NKMGame.GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMBuffData);
		if (nKMBuffData == null)
		{
			return 0;
		}
		NKMUnit unit = m_NKMGame.GetUnit(masterGameUnitUID, bChain: true, bPool: true);
		if (unit != null && !IsAlly(unit) && buffID > 0)
		{
			buffID = (short)(-buffID);
		}
		if (GetUnitFrameData().IsNoReuseBuff(buffID))
		{
			m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
			return 0;
		}
		NKMBuffData buff = GetBuff(buffID);
		if (buff != null && bRangeSon && buff.m_BuffSyncData.m_bRangeSon && buff.m_BuffSyncData.m_BuffStatLevel == nKMBuffData.m_BuffSyncData.m_BuffStatLevel && buff.m_BuffSyncData.m_BuffTimeLevel == nKMBuffData.m_BuffSyncData.m_BuffTimeLevel)
		{
			m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
			return 0;
		}
		nKMBuffData.m_BuffSyncData.m_BuffID = buffID;
		nKMBuffData.m_BuffSyncData.m_BuffStatLevel = buffLevel;
		nKMBuffData.m_BuffSyncData.m_BuffTimeLevel = buffTimeLevel;
		nKMBuffData.m_BuffSyncData.m_bNew = true;
		nKMBuffData.m_BuffSyncData.m_bRangeSon = bRangeSon;
		nKMBuffData.m_BuffSyncData.m_MasterGameUnitUID = masterGameUnitUID;
		nKMBuffData.m_BuffSyncData.m_bUseMasterStat = bUseMasterStat;
		nKMBuffData.m_NKMBuffTemplet = NKMBuffManager.GetBuffTempletByID(buffID);
		if (nKMBuffData.m_NKMBuffTemplet == null)
		{
			m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
			return 0;
		}
		NKMBuffTemplet nKMBuffTemplet = nKMBuffData.m_NKMBuffTemplet;
		if (nKMBuffTemplet.m_RangeOverlap)
		{
			nKMBuffData.m_BuffSyncData.m_BuffStatLevel = 1;
			nKMBuffData.m_BuffSyncData.m_BuffTimeLevel = 1;
		}
		bool flag2 = nKMBuffTemplet.m_bDebuff;
		if (nKMBuffData.m_BuffSyncData.m_bRangeSon)
		{
			flag2 = nKMBuffTemplet.m_bDebuffSon;
		}
		nKMBuffData.m_StateEndRemove = bStateEndRemove;
		bool bMasterOK = unit != null && unit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY;
		bool flag3 = WillAffectedByBuff(bMasterOK, unit, nKMBuffData);
		if (flag3)
		{
			if (ExistDispelBuff(flag2) && !nKMBuffTemplet.m_bInfinity && !nKMBuffTemplet.m_bNotDispel)
			{
				m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
				return 0;
			}
			foreach (NKM_UNIT_STATUS_EFFECT item in nKMBuffTemplet.m_ApplyStatus)
			{
				if (IsImmuneStatus(item))
				{
					m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
					return 0;
				}
			}
			if (!nKMBuffTemplet.m_bIgnoreBlock && (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_BLOCK_BUFF) || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_BLOCK_DEBUFF)))
			{
				if (!flag2 && HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_BLOCK_BUFF))
				{
					m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
					return 0;
				}
				if (flag2 && HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_BLOCK_DEBUFF))
				{
					m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
					return 0;
				}
			}
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NULLIFY_BARRIER) && nKMBuffTemplet.IsBarrierBuff && !nKMBuffTemplet.m_bNotDispel)
		{
			m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
			return 0;
		}
		if (buff != null && nKMBuffTemplet.m_bNoRefresh)
		{
			m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
			return 0;
		}
		if (buff != null)
		{
			int num = buff.m_BuffSyncData.m_OverlapCount + overlapCount;
			if (num <= 0)
			{
				DeleteBuff(buffID, NKMBuffTemplet.BuffEndDTType.NoUse);
				m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
				return 0;
			}
			nKMBuffData.m_BuffSyncData.m_OverlapCount = (byte)num;
			if (nKMBuffData.m_BuffSyncData.m_OverlapCount > nKMBuffTemplet.m_MaxOverlapCount)
			{
				nKMBuffData.m_BuffSyncData.m_OverlapCount = nKMBuffTemplet.m_MaxOverlapCount;
			}
		}
		else
		{
			if (overlapCount <= 0)
			{
				m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
				return 0;
			}
			nKMBuffData.m_BuffSyncData.m_OverlapCount = (byte)overlapCount;
		}
		if (nKMBuffData.m_BuffSyncData.m_OverlapCount >= nKMBuffTemplet.m_MaxOverlapCount)
		{
			nKMBuffData.m_BuffSyncData.m_OverlapCount = nKMBuffTemplet.m_MaxOverlapCount;
			if (!string.IsNullOrEmpty(nKMBuffTemplet.m_MaxOverlapBuffStrID))
			{
				short num2 = AddBuffByStrID(nKMBuffTemplet.m_MaxOverlapBuffStrID, buffLevel, buffTimeLevel, masterGameUnitUID, bUseMasterStat, bRangeSon: false);
				if (num2 != 0)
				{
					DeleteBuff(buffID, NKMBuffTemplet.BuffEndDTType.NoUse);
					m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
					return num2;
				}
			}
		}
		if (!IsBuffAllowed(nKMBuffData, bRangeSon))
		{
			m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
			return 0;
		}
		if (!IsDyingOrDie())
		{
			string badStatusStateChange = GetBadStatusStateChange(nKMBuffTemplet.m_ApplyStatus);
			if (!string.IsNullOrEmpty(badStatusStateChange))
			{
				StateChange(badStatusStateChange);
				m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
				return 0;
			}
		}
		if (!OnApplyStatus(nKMBuffTemplet.m_ApplyStatus, unit))
		{
			m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
			return 0;
		}
		if (nKMBuffTemplet.IsFixedPosBuff() && masterGameUnitUID == GetUnitDataGame().m_GameUnitUID)
		{
			float num3 = 0f;
			num3 = ((!nKMBuffTemplet.m_bShipSkillPos) ? GetUnitSyncData().m_PosX : m_UnitFrameData.m_fShipSkillPosX);
			num3 = ((!GetUnitSyncData().m_bRight) ? (num3 - nKMBuffTemplet.m_fOffsetX) : (num3 + nKMBuffTemplet.m_fOffsetX));
			nKMBuffData.m_fBuffPosX = num3;
		}
		if (nKMBuffTemplet.m_NKMDamageTemplet != null)
		{
			nKMBuffData.m_DamageInstBuff.m_Templet = nKMBuffTemplet.m_NKMDamageTemplet;
			nKMBuffData.m_DamageInstBuff.m_AttackerType = NKM_REACTOR_TYPE.NRT_GAME_UNIT;
			nKMBuffData.m_DamageInstBuff.m_AttackerGameUnitUID = nKMBuffData.m_BuffSyncData.m_MasterGameUnitUID;
			if (unit != null)
			{
				nKMBuffData.m_DamageInstBuff.m_AttackerTeamType = unit.GetUnitDataGame().m_NKM_TEAM_TYPE;
			}
			nKMBuffData.m_DamageInstBuff.m_AttackerUnitSkillTemplet = null;
		}
		nKMBuffData.m_fLifeTime = nKMBuffData.GetLifeTimeMax();
		nKMBuffData.m_fBarrierHP = nKMBuffTemplet.GetBarrierHPMax(GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP), GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_BARRIER_REINFORCE_RATE), nKMBuffData.m_BuffSyncData.m_BuffStatLevel);
		if (nKMBuffTemplet.m_fOneTimeHPDamageRate > 0f && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_HP_RATE_DAMAGE))
		{
			float fDamage = m_UnitSyncData.GetHP() * nKMBuffTemplet.m_fOneTimeHPDamageRate;
			NKM_TEAM_TYPE teamType = unit?.GetUnitDataGame().m_NKM_TEAM_TYPE ?? NKM_TEAM_TYPE.NTT_INVALID;
			AddDamage(bAttackCountOver: false, fDamage, NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL, nKMBuffData.m_BuffSyncData.m_MasterGameUnitUID, teamType, bPushSyncData: true);
		}
		if (flag3 && nKMBuffTemplet.m_DTStart != null)
		{
			m_NKMGame.ProcessDamageTemplet(nKMBuffTemplet.m_DTStart, unit, this, bUseAttackerStat: true, buffDamage: true, nKMBuffData.m_DamageInstBuff.m_AttackerUnitSkillTemplet);
		}
		DeleteBuff(buffID, NKMBuffTemplet.BuffEndDTType.NoUse);
		m_UnitFrameData.m_dicBuffData.Add(nKMBuffData.m_BuffSyncData.m_BuffID, nKMBuffData);
		m_UnitSyncData.m_dicBuffData.Add(nKMBuffData.m_BuffSyncData.m_BuffID, nKMBuffData.m_BuffSyncData);
		if (nKMBuffTemplet != null)
		{
			bool flag4 = false;
			if (nKMBuffTemplet.m_bDispelBuff)
			{
				DispelBuff(bDebuff: false, bDeleteInfinity: false);
				flag4 = true;
			}
			if (nKMBuffTemplet.m_bRangeSonDispelBuff && nKMBuffData.m_BuffSyncData.m_bRangeSon)
			{
				DispelBuff(bDebuff: false, bDeleteInfinity: false);
				flag4 = true;
			}
			if (nKMBuffTemplet.m_bDispelDebuff)
			{
				DispelBuff(bDebuff: true, bDeleteInfinity: false);
				flag4 = true;
			}
			if (nKMBuffTemplet.m_bRangeSonDispelDebuff && nKMBuffData.m_BuffSyncData.m_bRangeSon)
			{
				DispelBuff(bDebuff: true, bDeleteInfinity: false);
				flag4 = true;
			}
			if (flag4)
			{
				AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_DISPEL);
			}
		}
		foreach (NKM_UNIT_STATUS_EFFECT item2 in nKMBuffTemplet.m_ImmuneStatus)
		{
			ApplyImmuneStatus(item2);
		}
		m_bBuffChangedThisFrame = true;
		m_bPushSimpleSyncData = true;
		if (nKMBuffTemplet != null)
		{
			if (nKMBuffTemplet.m_UnitLevel != 0)
			{
				m_bBuffUnitLevelChangedThisFrame = true;
			}
			if (nKMBuffTemplet.m_bNoReuse)
			{
				GetUnitFrameData().AddNoReuseBuff(buffID);
			}
		}
		return buffID;
	}

	private bool IsBuffAllowed(NKMBuffData cNKMBuffData, bool bRangeSon)
	{
		if (!bRangeSon)
		{
			switch (cNKMBuffData.m_NKMBuffTemplet.m_eAffectSummonType)
			{
			case NKMBuffTemplet.AffectSummonType.SummonNo:
				if (IsSummonUnit())
				{
					return false;
				}
				break;
			case NKMBuffTemplet.AffectSummonType.SummonOnly:
				if (!IsSummonUnit())
				{
					return false;
				}
				break;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_AffectCostRange.m_Min >= 0 || cNKMBuffData.m_NKMBuffTemplet.m_AffectCostRange.m_Max >= 0)
			{
				int respawnCost = m_NKMGame.GetRespawnCost(GetUnitTemplet().m_UnitTempletBase.StatTemplet, bLeader: false, GetUnitDataGame().m_NKM_TEAM_TYPE);
				if (respawnCost < cNKMBuffData.m_NKMBuffTemplet.m_AffectCostRange.m_Min)
				{
					return false;
				}
				if (respawnCost > cNKMBuffData.m_NKMBuffTemplet.m_AffectCostRange.m_Max)
				{
					return false;
				}
			}
			if (!cNKMBuffData.m_NKMBuffTemplet.m_bAllowBoss && IsBoss())
			{
				return false;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_listAllowUnitID.Count > 0)
			{
				bool flag = false;
				for (int i = 0; i < cNKMBuffData.m_NKMBuffTemplet.m_listAllowUnitID.Count; i++)
				{
					if (cNKMBuffData.m_NKMBuffTemplet.m_listAllowUnitID[i] == GetUnitTemplet().m_UnitTempletBase.m_UnitID)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_listIgnoreUnitID.Count > 0)
			{
				for (int j = 0; j < cNKMBuffData.m_NKMBuffTemplet.m_listIgnoreUnitID.Count; j++)
				{
					if (cNKMBuffData.m_NKMBuffTemplet.m_listIgnoreUnitID[j] == GetUnitTemplet().m_UnitTempletBase.m_UnitID)
					{
						return false;
					}
				}
			}
			if (!GetUnitTempletBase().IsAllowUnitStyleType(cNKMBuffData.m_NKMBuffTemplet.m_listAllowStyleType, cNKMBuffData.m_NKMBuffTemplet.m_listIgnoreStyleType))
			{
				return false;
			}
			if (!GetUnitTempletBase().IsAllowUnitRoleType(cNKMBuffData.m_NKMBuffTemplet.m_listAllowRoleType, cNKMBuffData.m_NKMBuffTemplet.m_listIgnoreRoleType))
			{
				return false;
			}
			if (!GetUnitTempletBase().IsAllowUnitTagType(cNKMBuffData.m_NKMBuffTemplet.m_listAllowTagType, cNKMBuffData.m_NKMBuffTemplet.m_listIgnoreTagType))
			{
				return false;
			}
			if (!cNKMBuffData.m_NKMBuffTemplet.m_bAllowAirUnit && IsAirUnit())
			{
				return false;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_AffectMultiRespawnMinCount > 0 && GetUnitTemplet().m_UnitTempletBase.StatTemplet.m_RespawnCount < cNKMBuffData.m_NKMBuffTemplet.m_AffectMultiRespawnMinCount)
			{
				return false;
			}
			if (!cNKMBuffData.m_NKMBuffTemplet.m_bAllowLandUnit && !IsAirUnit())
			{
				return false;
			}
			if (!cNKMBuffData.m_NKMBuffTemplet.m_bAllowAwaken && GetUnitTemplet().m_UnitTempletBase.m_bAwaken)
			{
				return false;
			}
			if (!cNKMBuffData.m_NKMBuffTemplet.m_bAllowNormal && !GetUnitTemplet().m_UnitTempletBase.m_bAwaken)
			{
				return false;
			}
		}
		else
		{
			switch (cNKMBuffData.m_NKMBuffTemplet.m_eAffectRangeSonSummonType)
			{
			case NKMBuffTemplet.AffectSummonType.SummonNo:
				if (IsSummonUnit())
				{
					return false;
				}
				break;
			case NKMBuffTemplet.AffectSummonType.SummonOnly:
				if (!IsSummonUnit())
				{
					return false;
				}
				break;
			}
			if (!cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonAllowBoss && IsBoss())
			{
				return false;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_RangeSonAffectCostRange.m_Min >= 0 || cNKMBuffData.m_NKMBuffTemplet.m_RangeSonAffectCostRange.m_Max >= 0)
			{
				int respawnCost2 = m_NKMGame.GetRespawnCost(GetUnitTemplet().m_UnitTempletBase.StatTemplet, bLeader: false, GetUnitDataGame().m_NKM_TEAM_TYPE);
				if (respawnCost2 < cNKMBuffData.m_NKMBuffTemplet.m_RangeSonAffectCostRange.m_Min)
				{
					return false;
				}
				if (respawnCost2 > cNKMBuffData.m_NKMBuffTemplet.m_RangeSonAffectCostRange.m_Max)
				{
					return false;
				}
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonOnlyTarget || cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonOnlySubTarget)
			{
				NKMUnit unit = m_NKMGame.GetUnit(cNKMBuffData.m_BuffSyncData.m_MasterGameUnitUID, bChain: true, bPool: true);
				if (unit == null)
				{
					return false;
				}
				if (cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonOnlyTarget && unit.GetUnitSyncData().m_TargetUID != GetUnitDataGame().m_GameUnitUID)
				{
					return false;
				}
				if (cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonOnlySubTarget && unit.GetUnitSyncData().m_SubTargetUID != GetUnitDataGame().m_GameUnitUID)
				{
					return false;
				}
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonAllowUnitID.Count > 0)
			{
				bool flag2 = false;
				for (int k = 0; k < cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonAllowUnitID.Count; k++)
				{
					if (cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonAllowUnitID[k] == GetUnitTemplet().m_UnitTempletBase.m_UnitID)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					return false;
				}
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonIgnoreUnitID.Count > 0)
			{
				for (int l = 0; l < cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonIgnoreUnitID.Count; l++)
				{
					if (cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonIgnoreUnitID[l] == GetUnitTemplet().m_UnitTempletBase.m_UnitID)
					{
						return false;
					}
				}
			}
			if (!GetUnitTempletBase().IsAllowUnitStyleType(cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonAllowStyleType, cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonIgnoreStyleType))
			{
				return false;
			}
			if (!GetUnitTempletBase().IsAllowUnitRoleType(cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonAllowRoleType, cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonIgnoreRoleType))
			{
				return false;
			}
			if (!GetUnitTempletBase().IsAllowUnitTagType(cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonAllowTagType, cNKMBuffData.m_NKMBuffTemplet.m_listRangeSonIgnoreTagType))
			{
				return false;
			}
			if (cNKMBuffData.m_NKMBuffTemplet.m_AffectSonMultiRespawnMinCount > 0 && GetUnitTemplet().m_UnitTempletBase.StatTemplet.m_RespawnCount < cNKMBuffData.m_NKMBuffTemplet.m_AffectSonMultiRespawnMinCount)
			{
				return false;
			}
			if (!cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonAllowAirUnit && IsAirUnit())
			{
				return false;
			}
			if (!cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonAllowLandUnit && !IsAirUnit())
			{
				return false;
			}
			if (!cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonAllowAwaken && GetUnitTemplet().m_UnitTempletBase.m_bAwaken)
			{
				return false;
			}
			if (!cNKMBuffData.m_NKMBuffTemplet.m_bRangeSonAllowNormal && GetUnitTemplet().m_UnitTempletBase.m_bAwaken)
			{
				return false;
			}
		}
		return true;
	}

	public NKMBuffData GetBuff(short buffID, bool bNegativeBuff = false)
	{
		if (m_UnitFrameData.m_dicBuffData.ContainsKey(buffID))
		{
			return m_UnitFrameData.m_dicBuffData[buffID];
		}
		if (bNegativeBuff && m_UnitFrameData.m_dicBuffData.ContainsKey((short)(-buffID)))
		{
			return m_UnitFrameData.m_dicBuffData[buffID];
		}
		return null;
	}

	public void DispelBuff(bool bDebuff, bool bDeleteInfinity)
	{
		m_listBuffToDelete.Clear();
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in m_UnitFrameData.m_dicBuffData)
		{
			NKMBuffData value = dicBuffDatum.Value;
			if (value != null && !value.m_NKMBuffTemplet.m_bSystem && (!value.m_NKMBuffTemplet.m_bInfinity || bDeleteInfinity) && !value.m_NKMBuffTemplet.m_bNotDispel && value.m_BuffSyncData.m_bAffect)
			{
				bool flag = ((!value.m_BuffSyncData.m_bRangeSon) ? value.m_NKMBuffTemplet.m_bDebuff : value.m_NKMBuffTemplet.m_bDebuffSon);
				if (flag == bDebuff && IsBuffLive(dicBuffDatum.Key))
				{
					m_listBuffToDelete.Add(dicBuffDatum.Key);
				}
			}
		}
		for (int i = 0; i < m_listBuffToDelete.Count; i++)
		{
			short buffID = m_listBuffToDelete[i];
			DeleteBuff(buffID, NKMBuffTemplet.BuffEndDTType.Dispel);
		}
	}

	public void DispelRandomBuff(int count, bool bDispelDebuff)
	{
		if (count <= 0)
		{
			return;
		}
		m_listBuffToDelete.Clear();
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in m_UnitFrameData.m_dicBuffData)
		{
			NKMBuffData value = dicBuffDatum.Value;
			bool flag = value.m_NKMBuffTemplet.m_bDebuff;
			if (value.m_BuffSyncData.m_bRangeSon)
			{
				flag = value.m_NKMBuffTemplet.m_bDebuffSon;
			}
			if (bDispelDebuff == flag && !value.m_NKMBuffTemplet.m_bInfinity && !value.m_NKMBuffTemplet.m_bNotDispel && value.m_BuffSyncData.m_bAffect)
			{
				m_listBuffToDelete.Add(value.m_NKMBuffTemplet.m_BuffID);
			}
		}
		for (int i = 0; i < count; i++)
		{
			if (m_listBuffToDelete.Count <= 0)
			{
				break;
			}
			int index = NKMRandom.Range(0, m_listBuffToDelete.Count);
			DeleteBuff(m_listBuffToDelete[index], NKMBuffTemplet.BuffEndDTType.Dispel);
			m_listBuffToDelete.RemoveAt(index);
		}
	}

	public void DeleteStatusBuff(NKM_UNIT_STATUS_EFFECT status, bool bForceRemove, bool bAffectOnly)
	{
		if (status == NKM_UNIT_STATUS_EFFECT.NUSE_NONE)
		{
			return;
		}
		m_listBuffToDelete.Clear();
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in m_UnitFrameData.m_dicBuffData)
		{
			NKMBuffData value = dicBuffDatum.Value;
			if (value.m_NKMBuffTemplet.HasStatus(status) && (bForceRemove || (!value.m_NKMBuffTemplet.m_bInfinity && !value.m_NKMBuffTemplet.m_bNotDispel)) && (!bAffectOnly || value.m_BuffSyncData.m_bAffect))
			{
				m_listBuffToDelete.Add(dicBuffDatum.Key);
			}
		}
		for (int i = 0; i < m_listBuffToDelete.Count; i++)
		{
			DeleteBuff(m_listBuffToDelete[i], NKMBuffTemplet.BuffEndDTType.Dispel);
		}
	}

	public virtual bool DeleteBuff(string buffStrID, bool bFromEnemy = false)
	{
		NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(buffStrID);
		if (buffTempletByStrID != null)
		{
			short buffID = (bFromEnemy ? ((short)(-buffTempletByStrID.m_BuffID)) : buffTempletByStrID.m_BuffID);
			return DeleteBuff(buffID, NKMBuffTemplet.BuffEndDTType.NoUse);
		}
		return false;
	}

	public virtual bool DeleteBuff(short buffID, NKMBuffTemplet.BuffEndDTType eEndDTType)
	{
		if (m_UnitFrameData.m_dicBuffData.ContainsKey(buffID))
		{
			NKMBuffData nKMBuffData = m_UnitFrameData.m_dicBuffData[buffID];
			if (nKMBuffData.m_NKMBuffTemplet.m_UnitLevel != 0)
			{
				m_bBuffUnitLevelChangedThisFrame = true;
			}
			m_UnitSyncData.m_dicBuffData.Remove(buffID);
			m_UnitFrameData.m_dicBuffData.Remove(buffID);
			if (nKMBuffData.m_BuffSyncData.m_bAffect)
			{
				switch (eEndDTType)
				{
				case NKMBuffTemplet.BuffEndDTType.End:
				{
					NKMUnit unit2 = m_NKMGame.GetUnit(nKMBuffData.m_BuffSyncData.m_MasterGameUnitUID, bChain: true, bPool: true);
					m_NKMGame.ProcessDamageTemplet(nKMBuffData.m_NKMBuffTemplet.m_DTEnd, unit2, this, bUseAttackerStat: true, buffDamage: true, nKMBuffData.m_DamageInstBuff.m_AttackerUnitSkillTemplet);
					break;
				}
				case NKMBuffTemplet.BuffEndDTType.Dispel:
				{
					NKMUnit unit = m_NKMGame.GetUnit(nKMBuffData.m_BuffSyncData.m_MasterGameUnitUID, bChain: true, bPool: true);
					m_NKMGame.ProcessDamageTemplet(nKMBuffData.m_NKMBuffTemplet.m_DTDispel, unit, this, bUseAttackerStat: true, buffDamage: true, nKMBuffData.m_DamageInstBuff.m_AttackerUnitSkillTemplet);
					break;
				}
				}
			}
			if (GetUnitFrameData().m_BarrierBuffData != null && GetUnitFrameData().m_BarrierBuffData.m_BuffSyncData.m_BuffID == buffID)
			{
				GetUnitFrameData().m_BarrierBuffData = null;
			}
			m_NKMGame.GetObjectPool().CloseObj(nKMBuffData);
			m_bBuffChangedThisFrame = true;
			m_bPushSimpleSyncData = true;
			return true;
		}
		return false;
	}

	public bool IsBuffLive(short buffID)
	{
		if (m_UnitFrameData.m_dicBuffData.ContainsKey(buffID))
		{
			return true;
		}
		return false;
	}

	protected void SetBuffLevel(short buffID, byte level, byte timeLevel)
	{
		NKMBuffData buff = GetBuff(buffID);
		if (buff != null && (buff.m_BuffSyncData.m_BuffStatLevel != level || buff.m_BuffSyncData.m_BuffTimeLevel != timeLevel))
		{
			buff.m_BuffSyncData.m_BuffStatLevel = level;
			buff.m_BuffSyncData.m_BuffTimeLevel = timeLevel;
			m_bBuffChangedThisFrame = true;
			m_bPushSimpleSyncData = true;
		}
	}

	protected NKMOperator GetOperatorForStat()
	{
		NKMOperator result = null;
		if (m_UnitTemplet != null && m_UnitTemplet.m_UnitTempletBase != null && m_UnitTemplet.m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP && m_NKMGame.GetGameData() != null && m_NKMGame.GetGameData().GetTeamData(m_UnitDataGame.m_NKM_TEAM_TYPE) != null)
		{
			result = m_NKMGame.GetGameData().GetTeamData(m_UnitDataGame.m_NKM_TEAM_TYPE).m_Operator;
		}
		return result;
	}

	public void CheckAndCalculateBuffStat()
	{
		if (m_bBuffUnitLevelChangedThisFrame)
		{
			m_UnitFrameData.m_StatData.MakeBaseStat(m_NKMGame.GetGameData(), m_NKMGame.IsPVP(bUseDevOption: true), GetUnitData(), GetUnitTemplet().m_UnitTempletBase.StatTemplet.m_StatData, bPure: false, GetUnitFrameData().m_BuffUnitLevel, GetOperatorForStat(), bInitStat: false);
			m_bBuffUnitLevelChangedThisFrame = false;
			m_bBuffChangedThisFrame = true;
		}
		if (!m_bBuffChangedThisFrame)
		{
			return;
		}
		m_UnitFrameData.m_StatData.UpdateFinalStat(m_NKMGame, m_NKMGame.GetGameData().GameStatRate, this, m_bBuffHPRateConserveRequired);
		m_bBuffChangedThisFrame = false;
		float statFinal = m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_RESIST);
		if (statFinal.IsNearlyZero())
		{
			m_listDamageResistUnit.Clear();
			return;
		}
		bool flag = statFinal > 0f;
		if (flag != m_bDamageResistPositive)
		{
			m_listDamageResistUnit.Clear();
			m_bDamageResistPositive = flag;
		}
	}

	public void SetAgro(bool bGetAgro, float fRange, float fDurationTime, int maxCount, bool bUseUnitSize)
	{
		if (bGetAgro)
		{
			int num = 0;
			List<NKMUnit> sortUnitListByNearDist = GetSortUnitListByNearDist(bUseUnitSize);
			for (int i = 0; i < sortUnitListByNearDist.Count; i++)
			{
				NKMUnit nKMUnit = sortUnitListByNearDist[i];
				if (nKMUnit == null || nKMUnit == this || !m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE) || nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
				{
					continue;
				}
				switch (nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE)
				{
				case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
					if (!IsAirUnit())
					{
						continue;
					}
					break;
				case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
					if (!IsAirUnit() || GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_TOWER)
					{
						continue;
					}
					break;
				case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
					if (IsAirUnit())
					{
						continue;
					}
					break;
				case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
					if (IsAirUnit() || GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_TOWER)
					{
						continue;
					}
					break;
				case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS:
					if (GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_TOWER)
					{
						continue;
					}
					break;
				case NKM_FIND_TARGET_TYPE.NFTT_NO:
				case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY:
					continue;
				}
				if (!(fRange > 0f) || IsInRange(nKMUnit, fRange, bUseUnitSize))
				{
					SetAgroData(nKMUnit, fDurationTime);
					num++;
					if (maxCount > 0 && maxCount <= num)
					{
						break;
					}
					continue;
				}
				break;
			}
			return;
		}
		List<NKMUnit> targetingUnitList = m_NKMGame.GetTargetingUnitList(GetUnitDataGame().m_GameUnitUID);
		if (targetingUnitList == null)
		{
			return;
		}
		for (int j = 0; j < targetingUnitList.Count; j++)
		{
			NKMUnit nKMUnit2 = targetingUnitList[j];
			if (nKMUnit2 != null)
			{
				nKMUnit2.GetUnitSyncData().m_TargetUID = 0;
			}
		}
	}

	public bool SetAgro(NKMUnit targetUnit, float fDurationTime)
	{
		if (targetUnit == null)
		{
			return false;
		}
		if (targetUnit == this)
		{
			return false;
		}
		if (!m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, targetUnit.GetUnitDataGame().m_NKM_TEAM_TYPE))
		{
			return false;
		}
		if (targetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || targetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			return false;
		}
		switch (targetUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE)
		{
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
			if (!IsAirUnit())
			{
				return false;
			}
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
			if (!IsAirUnit() || GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_TOWER)
			{
				return false;
			}
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
			if (IsAirUnit())
			{
				return false;
			}
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
			if (IsAirUnit() || GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_TOWER)
			{
				return false;
			}
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS:
			if (GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_TOWER)
			{
				return false;
			}
			break;
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY:
			return false;
		case NKM_FIND_TARGET_TYPE.NFTT_NO:
			return false;
		}
		SetAgroData(targetUnit, fDurationTime);
		return true;
	}

	public bool SetTargetUnit(NKMUnit targetUnit, float duration, bool bSubTarget)
	{
		if (targetUnit == null)
		{
			if (bSubTarget)
			{
				m_UnitSyncData.m_SubTargetUID = 0;
				m_SubTargetUnit = null;
				if (m_SubTargetUIDOrg != m_UnitSyncData.m_SubTargetUID)
				{
					GetUnitFrameData().m_bTargetChangeThisFrame = true;
				}
				m_UnitFrameData.m_bFindSubTargetThisFrame = false;
				m_UnitFrameData.m_fFindSubTargetTime = duration;
				return true;
			}
			return false;
		}
		if (targetUnit == this)
		{
			return false;
		}
		if (targetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || targetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			return false;
		}
		bool flag = m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, targetUnit.GetUnitDataGame().m_NKM_TEAM_TYPE);
		if (!bSubTarget)
		{
			if (GetTargetTypeToEnemy() != flag)
			{
				return false;
			}
			switch (GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE)
			{
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
				if (!targetUnit.IsAirUnit())
				{
					return false;
				}
				break;
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
				if (!targetUnit.IsAirUnit() || targetUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_TOWER)
				{
					return false;
				}
				break;
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
				if (targetUnit.IsAirUnit())
				{
					return false;
				}
				break;
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
				if (targetUnit.IsAirUnit() || targetUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_TOWER)
				{
					return false;
				}
				break;
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS:
				if (targetUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_TOWER)
				{
					return false;
				}
				break;
			case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_ONLY:
				return false;
			case NKM_FIND_TARGET_TYPE.NFTT_NO:
				return false;
			}
		}
		if (bSubTarget)
		{
			m_UnitSyncData.m_SubTargetUID = targetUnit.GetUnitDataGame().m_GameUnitUID;
			m_SubTargetUnit = targetUnit;
			if (m_SubTargetUIDOrg != m_UnitSyncData.m_SubTargetUID)
			{
				GetUnitFrameData().m_bTargetChangeThisFrame = true;
			}
			m_UnitFrameData.m_bFindSubTargetThisFrame = false;
			m_UnitFrameData.m_fFindSubTargetTime = duration;
		}
		else
		{
			m_UnitSyncData.m_TargetUID = targetUnit.GetUnitDataGame().m_GameUnitUID;
			m_TargetUnit = targetUnit;
			if (m_TargetUIDOrg != m_UnitSyncData.m_TargetUID)
			{
				GetUnitFrameData().m_bTargetChangeThisFrame = true;
			}
			m_UnitFrameData.m_bFindTargetThisFrame = false;
			m_UnitFrameData.m_fFindTargetTime = duration;
		}
		return true;
	}

	private void SetAgroData(NKMUnit targetUnit, float fDurationTime)
	{
		if (targetUnit != null)
		{
			if (targetUnit.GetUnitSyncData().m_TargetUID != GetUnitDataGame().m_GameUnitUID)
			{
				targetUnit.AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_GET_AGRO);
			}
			targetUnit.GetUnitSyncData().m_TargetUID = GetUnitDataGame().m_GameUnitUID;
			targetUnit.GetUnitFrameData().m_fFindTargetTime = fDurationTime;
		}
	}

	protected int GetSkillLevelIfNowSkillState()
	{
		return GetSkillTempletNowState()?.m_Level ?? 0;
	}

	protected NKMUnitSkillTemplet GetSkillTempletNowState()
	{
		NKMUnitState unitState = GetUnitState(GetUnitSyncData().m_StateID);
		NKMUnitData unitData = GetUnitData();
		if (unitState != null && unitData != null && (unitState.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER || unitState.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_SKILL))
		{
			return unitData.GetUnitSkillTempletByType(unitState.m_NKM_SKILL_TYPE);
		}
		return null;
	}

	public float GetMaxHP()
	{
		return GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP);
	}

	public float GetMaxHP(float fHPRate)
	{
		return GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP) * fHPRate;
	}

	public void SetHPRate(float rate)
	{
		GetMaxHP();
		GetUnitSyncData().SetHP(GetMaxHP() * rate);
		m_PushSyncData = true;
	}

	public float GetHP()
	{
		return GetUnitSyncData().GetHP();
	}

	public void SetHP(float value)
	{
		GetUnitSyncData().SetHP(value);
	}

	public float GetStatFinal(NKM_STAT_TYPE statType)
	{
		return m_UnitFrameData.m_StatData.GetStatFinal(statType);
	}

	public void SetDispel(NKMEventDispel cEventDispel)
	{
		int num = 0;
		int skillLevelIfNowSkillState = GetSkillLevelIfNowSkillState();
		if (skillLevelIfNowSkillState > 0 && cEventDispel.m_MaxCount > 0)
		{
			num = (skillLevelIfNowSkillState - 1) * cEventDispel.m_DispelCountPerSkillLevel;
		}
		int num2 = 0;
		NKMUnit triggerTargetUnit = GetTriggerTargetUnit(cEventDispel.m_bUseTriggerTargetRange);
		List<NKMUnit> sortUnitListByNearDist = triggerTargetUnit.GetSortUnitListByNearDist(cEventDispel.m_bUseUnitSize);
		for (int i = 0; i < sortUnitListByNearDist.Count; i++)
		{
			NKMUnit nKMUnit = sortUnitListByNearDist[i];
			if (nKMUnit != null && ((cEventDispel.m_fRangeMin.IsNearlyZero() && cEventDispel.m_fRangeMax.IsNearlyZero()) || triggerTargetUnit.IsInRange(nKMUnit, cEventDispel.m_fRangeMin, cEventDispel.m_fRangeMax, cEventDispel.m_bUseUnitSize)) && (!cEventDispel.m_bDebuff || !m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE)) && (cEventDispel.m_bDebuff || m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE)) && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DYING && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE && !(nKMUnit.GetUnitSyncData().GetHP() <= 0f) && (!cEventDispel.m_bTargetSelf || nKMUnit == triggerTargetUnit) && nKMUnit.CheckEventCondition(cEventDispel.m_ConditionTarget, this))
			{
				if (cEventDispel.m_bCanDispelStatus)
				{
					nKMUnit.DispelStatusTime(cEventDispel.m_bDebuff);
				}
				nKMUnit.DispelBuff(cEventDispel.m_bDebuff, cEventDispel.m_bDeleteInfinity);
				nKMUnit.AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_DISPEL);
				num2++;
				if (cEventDispel.m_MaxCount + num > 0 && cEventDispel.m_MaxCount + num <= num2)
				{
					break;
				}
			}
		}
	}

	public void SetEventHeal(NKMEventHeal eventHeal, float fPosX, NKMUnit centerUnit)
	{
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			return;
		}
		int num = eventHeal.m_MaxCount;
		int skillLevelIfNowSkillState = GetSkillLevelIfNowSkillState();
		if (skillLevelIfNowSkillState > 0 && eventHeal.m_MaxCount > 0)
		{
			num += (skillLevelIfNowSkillState - 1) * eventHeal.m_HealCountPerSkillLevel;
		}
		int num2 = 0;
		List<NKMUnit> list = new List<NKMUnit>();
		NKMUnit targetUnit = GetTargetUnit();
		if (targetUnit != null && IsAlly(targetUnit))
		{
			list.Add(targetUnit);
		}
		if (list.Count == 0 && eventHeal.m_bSelfTargetingOnly)
		{
			list.Add(this);
		}
		if (list.Count == 0 || num != 1)
		{
			if (targetUnit != null && eventHeal.m_bSplashNearTarget)
			{
				list.AddRange(targetUnit.GetSortUnitListByNearDist(eventHeal.m_bUseUnitSize));
			}
			else if (centerUnit != null)
			{
				list.AddRange(centerUnit.GetSortUnitListByNearDist(eventHeal.m_bUseUnitSize));
			}
			else
			{
				list.AddRange(GetSortUnitListByNearDist(eventHeal.m_bUseUnitSize));
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			targetUnit = list[i];
			if (targetUnit == null || (!eventHeal.m_bEnableSelfHeal && targetUnit == this))
			{
				continue;
			}
			if (!eventHeal.m_fRangeMin.IsNearlyZero() || !eventHeal.m_fRangeMax.IsNearlyZero())
			{
				if (centerUnit != null)
				{
					if (!centerUnit.IsInRange(targetUnit, eventHeal.m_fRangeMin, eventHeal.m_fRangeMax, eventHeal.m_bUseUnitSize))
					{
						continue;
					}
				}
				else if (!targetUnit.IsInRange(fPosX, eventHeal.m_fRangeMin, eventHeal.m_fRangeMax, eventHeal.m_bUseUnitSize, GetUnitSyncData().m_bRight))
				{
					continue;
				}
			}
			if ((!eventHeal.m_bIgnoreShip || !NKMUnitManager.IsShipType(targetUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE)) && !eventHeal.m_IgnoreStyleType.Contains(targetUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE) && !eventHeal.m_IgnoreStyleType.Contains(targetUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE_SUB) && (eventHeal.m_AllowStyleType.Count <= 0 || eventHeal.m_AllowStyleType.Contains(targetUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE) || eventHeal.m_AllowStyleType.Contains(targetUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE_SUB)) && !m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, targetUnit.GetUnitDataGame().m_NKM_TEAM_TYPE) && targetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DYING && targetUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE && !(targetUnit.GetUnitSyncData().GetHP() <= 0f) && targetUnit.CheckEventCondition(eventHeal.m_ConditionTarget, this))
			{
				num2++;
				float num3 = eventHeal.CalcHealAmount(skillLevelIfNowSkillState, this, targetUnit);
				targetUnit.SetHeal(num3, GetUnitDataGame().m_GameUnitUID);
				Log.Debug($"unitID: {GetUnitTemplet().m_UnitTempletBase.m_UnitStrID}, [EventHeal] caster:{GetUnitData().m_UnitUID} target:{targetUnit.GetUnitData().m_UnitUID} amount:{num3} count:{num2}/{num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 10803);
				if (num > 0 && num2 >= num)
				{
					break;
				}
			}
		}
	}

	protected float GetExpectedHealAmount(float fHeal, short UIDCaster)
	{
		if (fHeal > 0f)
		{
			if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOHEAL))
			{
				return 0f;
			}
			float num = 0f;
			NKMUnit unit = m_NKMGame.GetUnit(UIDCaster);
			if (unit != null)
			{
				num = unit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HEAL_RATE);
			}
			fHeal *= 1f + num - m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HEAL_REDUCE_RATE);
			if (fHeal <= 0f)
			{
				return 0f;
			}
		}
		return fHeal;
	}

	protected void SetHeal(float fHeal, short UIDCaster, bool showHealResult = true, bool allowTransfer = true)
	{
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			return;
		}
		if (fHeal > 0f)
		{
			fHeal = GetExpectedHealAmount(fHeal, UIDCaster);
			if (fHeal <= 0f)
			{
				return;
			}
		}
		if (fHeal > 0f && GetUnitFrameData().m_fHealFeedback > 0f)
		{
			NKM_TEAM_TYPE teamType = NKM_TEAM_TYPE.NTT_INVALID;
			NKMUnit unit = m_NKMGame.GetUnit(GetUnitFrameData().m_fHealFeedbackMasterGameUnitUID, bChain: true, bPool: true);
			if (unit != null)
			{
				teamType = unit.GetUnitDataGame().m_NKM_TEAM_TYPE;
			}
			bool flag = false;
			if (GetUnitFrameData().m_bInvincible || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE))
			{
				flag = true;
			}
			if (m_UnitStateNow != null && m_UnitStateNow.m_bInvincibleState)
			{
				flag = true;
			}
			if (!flag)
			{
				AddDamage(bAttackCountOver: false, fHeal * GetUnitFrameData().m_fHealFeedback, NKM_DAMAGE_RESULT_TYPE.NDRT_NO_MARK, GetUnitFrameData().m_fHealFeedbackMasterGameUnitUID, teamType, bPushSyncData: true, bNoRedirect: true);
			}
			return;
		}
		if (allowTransfer)
		{
			fHeal = ApplyHealTransfer(fHeal, bShowHealResult: true);
		}
		if (GetUnitSyncData().GetHP() + fHeal > GetMaxHP())
		{
			fHeal = GetMaxHP() - GetUnitSyncData().GetHP();
		}
		GetUnitSyncData().SetHP(GetUnitSyncData().GetHP() + fHeal);
		if (GetUnitSyncData().GetHP() > GetMaxHP())
		{
			GetUnitSyncData().SetHP(GetMaxHP());
		}
		if (fHeal > 0f)
		{
			if (showHealResult)
			{
				NKMDamageData nKMDamageData = new NKMDamageData();
				nKMDamageData.m_FinalDamage = (int)fHeal;
				nKMDamageData.m_NKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_HEAL;
				nKMDamageData.m_GameUnitUIDAttacker = UIDCaster;
				m_UnitSyncData.m_listDamageData.Add(nKMDamageData);
				m_PushSyncData = true;
			}
			m_NKMGame.m_GameRecord.AddHeal(UIDCaster, fHeal);
		}
	}

	public virtual float ApplyHealTransfer(float fHeal, bool bShowHealResult)
	{
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			return fHeal;
		}
		if (fHeal <= 0f)
		{
			return fHeal;
		}
		if (m_UnitFrameData.m_HealTransferMasterGameUnitUID == 0 || m_UnitFrameData.m_fHealTransfer <= 0f)
		{
			return fHeal;
		}
		NKMUnit unit = m_NKMGame.GetUnit(m_UnitFrameData.m_HealTransferMasterGameUnitUID);
		if (unit == null)
		{
			return fHeal;
		}
		float num = fHeal * GetUnitFrameData().m_fHealTransfer;
		fHeal -= num;
		if (fHeal > 0f && GetUnitSyncData().GetHP() + fHeal > GetMaxHP())
		{
			float num2 = GetMaxHP() - GetUnitSyncData().GetHP();
			num = num * num2 / fHeal;
			fHeal = num2;
		}
		unit.SetHeal(num, m_UnitFrameData.m_HealTransferMasterGameUnitUID, bShowHealResult, allowTransfer: false);
		return fHeal;
	}

	public void SetCost(float fAllyCostDelta, float fEnemyCostDelta)
	{
		NKMGameRuntimeTeamData nKMGameRuntimeTeamData = null;
		NKMGameRuntimeTeamData nKMGameRuntimeTeamData2 = null;
		if (m_NKMGame.IsATeam(GetUnitDataGame().m_NKM_TEAM_TYPE))
		{
			nKMGameRuntimeTeamData = m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataA;
			nKMGameRuntimeTeamData2 = m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataB;
		}
		else
		{
			if (!m_NKMGame.IsBTeam(GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				Log.Warn("unitID: " + GetUnitTemplet().m_UnitTempletBase.m_UnitStrID + ", invalid team type:" + GetUnitDataGame().m_NKM_TEAM_TYPE, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 10978);
				return;
			}
			nKMGameRuntimeTeamData = m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataB;
			nKMGameRuntimeTeamData2 = m_NKMGame.GetGameRuntimeData().m_NKMGameRuntimeTeamDataA;
		}
		nKMGameRuntimeTeamData.m_fRespawnCost += fAllyCostDelta;
		if (fAllyCostDelta != 0f)
		{
			AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_ALLY_RESPAWN_COST, fAllyCostDelta);
		}
		nKMGameRuntimeTeamData2.m_fRespawnCost += fEnemyCostDelta;
		if (fEnemyCostDelta != 0f)
		{
			AddEventMark(NKM_UNIT_EVENT_MARK.NUEM_ENEMY_RESPAWN_COST, fEnemyCostDelta);
		}
		if (nKMGameRuntimeTeamData.m_fRespawnCost > 10f)
		{
			nKMGameRuntimeTeamData.m_fRespawnCost = 10f;
		}
		if (nKMGameRuntimeTeamData.m_fRespawnCost < 0f)
		{
			nKMGameRuntimeTeamData.m_fRespawnCost = 0f;
		}
		if (nKMGameRuntimeTeamData2.m_fRespawnCost > 10f)
		{
			nKMGameRuntimeTeamData2.m_fRespawnCost = 10f;
		}
		if (nKMGameRuntimeTeamData2.m_fRespawnCost < 0f)
		{
			nKMGameRuntimeTeamData2.m_fRespawnCost = 0f;
		}
	}

	public void SetStun(NKMUnit Applier, float fStunTime, float fRange, bool bUseUnitSize, int maxCount, float fStunTimePerSkillLevel, int StunCountPerSkillLevel, HashSet<NKM_UNIT_STYLE_TYPE> m_IgnoreStyleType)
	{
		float num = 0f;
		int num2 = 0;
		int skillLevelIfNowSkillState = GetSkillLevelIfNowSkillState();
		if (skillLevelIfNowSkillState > 0)
		{
			num = fStunTimePerSkillLevel * (float)(skillLevelIfNowSkillState - 1);
			if (maxCount > 0)
			{
				num2 = (skillLevelIfNowSkillState - 1) * StunCountPerSkillLevel;
			}
		}
		int num3 = 0;
		List<NKMUnit> sortUnitListByNearDist = GetSortUnitListByNearDist(bUseUnitSize);
		for (int i = 0; i < sortUnitListByNearDist.Count; i++)
		{
			NKMUnit nKMUnit = sortUnitListByNearDist[i];
			if (nKMUnit != null && nKMUnit != this && !m_IgnoreStyleType.Contains(nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE) && !m_IgnoreStyleType.Contains(nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE_SUB) && m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE) && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DYING && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE && !(nKMUnit.GetUnitSyncData().GetHP() <= 0f))
			{
				if (!IsInRange(nKMUnit, fRange, bUseUnitSize))
				{
					break;
				}
				float time = fStunTime + num;
				nKMUnit.ApplyStatusTime(NKM_UNIT_STATUS_EFFECT.NUSE_STUN, time, Applier);
				num3++;
				if (maxCount + num2 > 0 && maxCount + num2 <= num3)
				{
					break;
				}
			}
		}
	}

	public bool GetTargetTypeToEnemy()
	{
		return GetTargetTypeToEnemy(GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE);
	}

	private bool GetTargetTypeToEnemy(NKM_FIND_TARGET_TYPE type)
	{
		if ((uint)(type - 17) <= 8u)
		{
			return false;
		}
		return true;
	}

	public bool CanAttackTargetDependingAirUnit(bool isTargetAirUnit)
	{
		switch (GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE)
		{
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_AIR:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_AIR:
			return isTargetAirUnit;
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND_RANGER_SUPPORTER_SNIPER_FIRST:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND_BOSS_LAST:
		case NKM_FIND_TARGET_TYPE.NFTT_ENEMY_BOSS_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LAND:
		case NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_LAND:
			return !isTargetAirUnit;
		case NKM_FIND_TARGET_TYPE.NFTT_NO:
			return false;
		default:
			return true;
		}
	}

	public NKMUnit GetTargetUnit(bool bDying = false)
	{
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			return null;
		}
		if (m_UnitSyncData.m_TargetUID <= 0)
		{
			return null;
		}
		NKMUnit unit = m_NKMGame.GetUnit(m_UnitSyncData.m_TargetUID);
		if (unit == null)
		{
			m_UnitSyncData.m_TargetUID = 0;
			return null;
		}
		if (!bDying && (unit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || unit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE))
		{
			m_UnitSyncData.m_TargetUID = 0;
			return null;
		}
		if (!unit.WillBeTargetted())
		{
			m_UnitSyncData.m_TargetUID = 0;
			return null;
		}
		if (unit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_CLOCKING) && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_DETECTER))
		{
			m_UnitSyncData.m_TargetUID = 0;
			return null;
		}
		if (unit.GetUnitStateNow() != null && unit.GetUnitStateNow().m_bForceNoTargeted)
		{
			m_UnitSyncData.m_TargetUID = 0;
			return null;
		}
		if (!CanAttackTargetDependingAirUnit(unit.IsAirUnit()) && !unit.IsBoss())
		{
			m_UnitSyncData.m_TargetUID = 0;
			return null;
		}
		if (GetUnitTemplet().m_TargetFindData.m_bNoBackTarget)
		{
			if (GetUnitSyncData().m_bRight && unit.GetUnitSyncData().m_PosX < m_UnitSyncData.m_PosX)
			{
				m_UnitSyncData.m_TargetUID = 0;
				return null;
			}
			if (!GetUnitSyncData().m_bRight && unit.GetUnitSyncData().m_PosX > m_UnitSyncData.m_PosX)
			{
				m_UnitSyncData.m_TargetUID = 0;
				return null;
			}
		}
		if (GetTargetTypeToEnemy() && IsAlly(unit))
		{
			m_UnitSyncData.m_TargetUID = 0;
			return null;
		}
		if (!GetTargetTypeToEnemy() && !IsAlly(unit))
		{
			m_UnitSyncData.m_TargetUID = 0;
			return null;
		}
		if ((GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE == NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP || GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE == NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_AIR || GetUnitTemplet().m_UnitTempletBase.m_NKM_FIND_TARGET_TYPE == NKM_FIND_TARGET_TYPE.NFTT_NEAR_MY_TEAM_LOW_HP_LAND) && unit.GetUnitSyncData().GetHP() >= unit.GetMaxHP())
		{
			m_UnitSyncData.m_TargetUID = 0;
			return null;
		}
		return unit;
	}

	public NKMUnit GetTargetUnit(short targetUID, NKMFindTargetData cNKMFindTargetData)
	{
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			return null;
		}
		if (targetUID <= 0)
		{
			return null;
		}
		if (cNKMFindTargetData == null)
		{
			return null;
		}
		NKMUnit unit = m_NKMGame.GetUnit(targetUID);
		if (unit == null)
		{
			return null;
		}
		if (unit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || unit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			return null;
		}
		if (!IsInRange(unit, cNKMFindTargetData.m_fSeeRange, cNKMFindTargetData.m_bUseUnitSize))
		{
			return null;
		}
		if (!IsAlly(unit) && unit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_CLOCKING) && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_DETECTER))
		{
			return null;
		}
		if (unit.GetUnitStateNow() != null && unit.GetUnitStateNow().m_bForceNoTargeted)
		{
			return null;
		}
		if (!cNKMFindTargetData.m_bCanTargetBoss && unit.IsBoss())
		{
			return null;
		}
		if (cNKMFindTargetData.m_bNoBackTarget)
		{
			if (GetUnitSyncData().m_bRight && unit.GetUnitSyncData().m_PosX < m_UnitSyncData.m_PosX)
			{
				return null;
			}
			if (!GetUnitSyncData().m_bRight && unit.GetUnitSyncData().m_PosX > m_UnitSyncData.m_PosX)
			{
				return null;
			}
			return unit;
		}
		if (cNKMFindTargetData.m_bNoFrontTarget)
		{
			if (GetUnitSyncData().m_bRight && unit.GetUnitSyncData().m_PosX > m_UnitSyncData.m_PosX)
			{
				return null;
			}
			if (!GetUnitSyncData().m_bRight && unit.GetUnitSyncData().m_PosX < m_UnitSyncData.m_PosX)
			{
				return null;
			}
			return unit;
		}
		if (GetTargetTypeToEnemy(cNKMFindTargetData.m_FindTargetType) && IsAlly(unit))
		{
			return null;
		}
		if (!GetTargetTypeToEnemy(cNKMFindTargetData.m_FindTargetType) && !IsAlly(unit))
		{
			return null;
		}
		NKM_FIND_TARGET_TYPE findTargetType = cNKMFindTargetData.m_FindTargetType;
		if ((uint)(findTargetType - 20) <= 2u)
		{
			return null;
		}
		return unit;
	}

	public bool IsArriveTarget()
	{
		if (m_TargetUnit == null)
		{
			return false;
		}
		float num = Math.Abs(m_TargetUnit.GetUnitSyncData().m_PosX - m_UnitSyncData.m_PosX);
		if (num > m_UnitTemplet.m_SeeRangeMax)
		{
			m_UnitSyncData.m_TargetUID = 0;
			return false;
		}
		if (num - (m_TargetUnit.GetUnitTemplet().m_UnitSizeX * 0.5f + m_UnitTemplet.m_UnitSizeX * 0.5f) < GetUnitDataGame().m_fTargetNearRange)
		{
			return true;
		}
		return false;
	}

	public float GetDist(NKMUnit targetUnit, bool bUseUnitSize)
	{
		if (targetUnit == null)
		{
			return 0f;
		}
		float num = Math.Abs(m_UnitSyncData.m_PosX - targetUnit.m_UnitSyncData.m_PosX);
		if (bUseUnitSize)
		{
			num -= (m_UnitTemplet.m_UnitSizeX + targetUnit.m_UnitTemplet.m_UnitSizeX) * 0.5f;
		}
		return num;
	}

	public float GetDist(float targetPosX, bool bUseUnitSize)
	{
		float num = Math.Abs(m_UnitSyncData.m_PosX - targetPosX);
		if (bUseUnitSize)
		{
			num -= m_UnitTemplet.m_UnitSizeX * 0.5f;
		}
		return num;
	}

	public float GetDist(float targetPosX, float targetSizeX, bool bUseUnitSize)
	{
		float num = Math.Abs(m_UnitSyncData.m_PosX - targetPosX);
		if (bUseUnitSize)
		{
			num -= (m_UnitTemplet.m_UnitSizeX + targetSizeX) * 0.5f;
		}
		return num;
	}

	public float GetDist(NKMUnit unit)
	{
		return Math.Abs(m_UnitSyncData.m_PosX - unit.GetUnitSyncData().m_PosX);
	}

	public bool IsStopTime()
	{
		if (m_UnitFrameData.m_fStopReserveTime > 0f)
		{
			return false;
		}
		for (int i = 0; i < 3; i++)
		{
			if (m_UnitFrameData.m_StopTime[i] > 0f)
			{
				return true;
			}
		}
		return false;
	}

	public void SetStopTime(float fStopTime, NKM_STOP_TIME_INDEX stopTimeIndex)
	{
		m_UnitFrameData.m_StopTime[(int)stopTimeIndex] = fStopTime;
		SetStopReserveTime(0.08f);
	}

	public void SetStopReserveTime(float fStopReserveTime)
	{
		m_UnitFrameData.m_fStopReserveTime = fStopReserveTime;
	}

	public int GetDamageResistCount(short gameUnitUID)
	{
		if (m_listDamageResistUnit.ContainsKey(gameUnitUID))
		{
			return m_listDamageResistUnit[gameUnitUID];
		}
		return 0;
	}

	public void AddDamageResistCount(short gameUnitUID)
	{
		if (!m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_RESIST).IsNearlyZero() && gameUnitUID > 0)
		{
			if (!m_listDamageResistUnit.ContainsKey(gameUnitUID))
			{
				m_listDamageResistUnit.Add(gameUnitUID, 1);
			}
			else
			{
				m_listDamageResistUnit[gameUnitUID] += 1;
			}
		}
	}

	public virtual void DamageReact(NKMDamageInst cNKMDamageInst, bool bBuffDamage)
	{
		if (cNKMDamageInst == null || cNKMDamageInst.m_Templet == null)
		{
			return;
		}
		if (m_UnitStateNow == null)
		{
			cNKMDamageInst.m_ReActResult = NKM_REACT_TYPE.NRT_NO;
			return;
		}
		bool flag = false;
		if (m_UnitFrameData.m_bInvincible || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE))
		{
			flag = true;
		}
		if (m_UnitStateNow.m_bInvincibleState)
		{
			flag = true;
		}
		if (flag && (cNKMDamageInst.m_EventAttack == null || !cNKMDamageInst.m_EventAttack.m_bForceHit))
		{
			cNKMDamageInst.m_ReActResult = NKM_REACT_TYPE.NRT_INVINCIBLE;
			return;
		}
		bool flag2 = cNKMDamageInst.m_Templet.IsZeroDamage();
		if (!flag2)
		{
			m_UnitFrameData.m_fHitLightTime = 0.15f;
		}
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			return;
		}
		if (!flag2 && HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP))
		{
			RemoveStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP);
		}
		bool flag3 = false;
		if (cNKMDamageInst.m_EventAttack != null && cNKMDamageInst.m_EventAttack.m_AttackUnitCount + cNKMDamageInst.m_AttackerAddAttackUnitCount <= cNKMDamageInst.m_AttackCount)
		{
			flag3 = true;
		}
		bool flag4 = ((cNKMDamageInst.m_EventAttack != null && cNKMDamageInst.m_EventAttack.m_bDamageSpeedDependRight) ? (!cNKMDamageInst.m_bAttackerRight) : ((m_UnitSyncData.m_PosX < cNKMDamageInst.m_AttackerPosX) ? true : false));
		if (m_UnitSyncData.m_PosZ < cNKMDamageInst.m_AttackerPosZ)
		{
			m_UnitSyncData.m_bAttackerZUp = true;
		}
		else
		{
			m_UnitSyncData.m_bAttackerZUp = false;
		}
		if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER && cNKMDamageInst.m_Templet.m_bCrashBarrier && GetUnitFrameData().m_BarrierBuffData != null && !GetUnitFrameData().m_BarrierBuffData.m_NKMBuffTemplet.m_bNotDispel)
		{
			if (cNKMDamageInst.m_Templet.m_fFeedbackBarrier > 0f && GetUnitFrameData().m_BarrierBuffData.m_fBarrierHP > 0f && !IsBoss())
			{
				AddDamage(bAttackCountOver: false, GetUnitFrameData().m_BarrierBuffData.m_fBarrierHP * cNKMDamageInst.m_Templet.m_fFeedbackBarrier, NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL, cNKMDamageInst.m_AttackerGameUnitUID, cNKMDamageInst.m_AttackerTeamType);
			}
			GetUnitFrameData().m_BarrierBuffData.m_fBarrierHP = -2f;
		}
		NKM_SUPER_ARMOR_LEVEL currentSuperArmorLevel = m_UnitFrameData.CurrentSuperArmorLevel;
		bool flag5 = true;
		if (currentSuperArmorLevel == NKM_SUPER_ARMOR_LEVEL.NSAL_NO || currentSuperArmorLevel < cNKMDamageInst.m_Templet.m_CrashSuperArmorLevel)
		{
			flag5 = false;
		}
		if (cNKMDamageInst.m_bEvade)
		{
			flag5 = true;
		}
		if (!flag5)
		{
			if ((!m_UnitStateNow.m_bNoMove || m_UnitTemplet.m_bNoMove) && m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER && !HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NO_DAMAGE_BACK_SPEED))
			{
				if (!cNKMDamageInst.m_Templet.m_BackSpeedX.IsNearlyEqual(-1f))
				{
					float num = (float)Math.Sqrt(Math.Max(0f, 1f - GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_BACK_RESIST)));
					float num2 = ((!flag4) ? (cNKMDamageInst.m_Templet.m_BackSpeedX * m_UnitTemplet.m_fDamageBackFactor * num) : ((0f - cNKMDamageInst.m_Templet.m_BackSpeedX) * m_UnitTemplet.m_fDamageBackFactor * num));
					if (num2 * m_UnitFrameData.m_fDamageSpeedX >= 0f)
					{
						if (Math.Abs(num2) >= Math.Abs(m_UnitFrameData.m_fDamageSpeedX))
						{
							m_UnitFrameData.m_fDamageSpeedX = num2;
							m_UnitFrameData.m_fSpeedX = 0f;
							m_UnitFrameData.m_fDamageSpeedKeepTimeX = cNKMDamageInst.m_Templet.m_BackSpeedKeepTimeX;
						}
					}
					else
					{
						m_UnitFrameData.m_fDamageSpeedX += num2;
						m_UnitFrameData.m_fSpeedX = 0f;
						if (Math.Abs(num2) >= Math.Abs(m_UnitFrameData.m_fDamageSpeedX))
						{
							m_UnitFrameData.m_fDamageSpeedKeepTimeX = cNKMDamageInst.m_Templet.m_BackSpeedKeepTimeX;
						}
					}
				}
				if (!cNKMDamageInst.m_Templet.m_BackSpeedZ.IsNearlyEqual(-1f) && !flag3)
				{
					m_UnitFrameData.m_fSpeedZ = 0f;
					m_UnitFrameData.m_fDamageSpeedZ = cNKMDamageInst.m_Templet.m_BackSpeedZ;
					m_UnitFrameData.m_fDamageSpeedKeepTimeZ = cNKMDamageInst.m_Templet.m_BackSpeedKeepTimeZ;
				}
				if (!cNKMDamageInst.m_Templet.m_BackSpeedJumpY.IsNearlyEqual(-1f))
				{
					m_UnitFrameData.m_fSpeedY = 0f;
					m_UnitFrameData.m_fDamageSpeedJumpY = cNKMDamageInst.m_Templet.m_BackSpeedJumpY * m_UnitTemplet.m_fDamageUpFactor;
					m_UnitFrameData.m_fDamageSpeedKeepTimeJumpY = cNKMDamageInst.m_Templet.m_BackSpeedKeepTimeJumpY;
				}
				if (IsAirUnit() || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_AIRBORNE))
				{
					m_UnitFrameData.m_fDamageSpeedJumpY = 0f;
					m_UnitFrameData.m_fDamageSpeedKeepTimeJumpY = 0f;
				}
			}
			if (!GetUnitTemplet().m_bNoDamageStopTime)
			{
				SetStopTime(cNKMDamageInst.m_Templet.m_fStopTimeDef, NKM_STOP_TIME_INDEX.NSTI_DAMAGE);
				SetStopReserveTime(cNKMDamageInst.m_Templet.m_fStopReserveTimeDef);
			}
		}
		else
		{
			cNKMDamageInst.m_ReActResult = NKM_REACT_TYPE.NRT_NO_ACTION;
		}
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			return;
		}
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			bool flag6 = false;
			if (!bBuffDamage && m_UnitFrameData.m_GuardGameUnitUID > 0)
			{
				NKMUnit unit = m_NKMGame.GetUnit(GetUnitFrameData().m_GuardGameUnitUID);
				if (unit != null)
				{
					flag6 = unit.CanGuard(cNKMDamageInst, bBuffDamage, this, out var guardState);
					if (!string.IsNullOrEmpty(guardState))
					{
						unit.StateChange(guardState);
					}
				}
			}
			if (!flag6)
			{
				flag6 = CanRevenge(cNKMDamageInst, bBuffDamage, out var revengeState);
				if (!string.IsNullOrEmpty(revengeState))
				{
					StateChange(revengeState);
				}
			}
			if (flag6)
			{
				cNKMDamageInst.m_ReActResult = NKM_REACT_TYPE.NRT_REVENGE;
				m_UnitFrameData.m_fSpeedX = 0f;
				m_UnitFrameData.m_fSpeedZ = 0f;
				m_UnitFrameData.m_fSpeedY = 0f;
				m_UnitFrameData.m_fDamageSpeedX = 0f;
				m_UnitFrameData.m_fDamageSpeedZ = 0f;
				m_UnitFrameData.m_fDamageSpeedJumpY = 0f;
				m_UnitFrameData.m_fDamageSpeedKeepTimeX = 0f;
				m_UnitFrameData.m_fDamageSpeedKeepTimeZ = 0f;
				m_UnitFrameData.m_fDamageSpeedKeepTimeJumpY = 0f;
			}
			if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_DAMAGE_CATCH && !m_UnitStateNow.m_bNoMove && !m_UnitTemplet.m_bNoMove && !IsBoss() && !flag5 && GetUnitSyncData().m_CatcherGameUnitUID != cNKMDamageInst.m_AttackerGameUnitUID)
			{
				GetUnitSyncData().m_CatcherGameUnitUID = cNKMDamageInst.m_AttackerGameUnitUID;
				m_PushSyncData = true;
			}
			if (!flag6 && !m_UnitTemplet.m_bNoDamageState && !flag5)
			{
				NKM_STATE_CHANGE_PRIORITY stateChangePriority = GetStateChangePriority(cNKMDamageInst.m_Templet.m_CrashSuperArmorLevel, isAttack: true);
				switch (cNKMDamageInst.m_ReActResult)
				{
				case NKM_REACT_TYPE.NRT_DAMAGE_CATCH:
				case NKM_REACT_TYPE.NRT_DAMAGE_A:
					if (!IsAirUnit())
					{
						if (m_UnitFrameData.m_bFootOnLand)
						{
							StateChange("USN_DAMAGE_A", bForceChange: true, bImmediate: false, stateChangePriority);
						}
						else
						{
							StateChange("USN_DAMAGE_AIR_DOWN", bForceChange: true, bImmediate: false, stateChangePriority);
						}
					}
					else
					{
						StateChange("USN_DAMAGE_A", bForceChange: true, bImmediate: false, stateChangePriority);
					}
					break;
				case NKM_REACT_TYPE.NRT_DAMAGE_B:
					if (!IsAirUnit())
					{
						if (m_UnitFrameData.m_bFootOnLand)
						{
							StateChange("USN_DAMAGE_B", bForceChange: true, bImmediate: false, stateChangePriority);
						}
						else
						{
							StateChange("USN_DAMAGE_AIR_DOWN", bForceChange: true, bImmediate: false, stateChangePriority);
						}
					}
					else
					{
						StateChange("USN_DAMAGE_B", bForceChange: true, bImmediate: false, stateChangePriority);
					}
					break;
				case NKM_REACT_TYPE.NRT_DAMAGE_DOWN:
					if (!IsAirUnit())
					{
						if (!m_UnitTemplet.m_bNoDamageDownState)
						{
							if (m_UnitFrameData.m_bFootOnLand)
							{
								StateChange("USN_DAMAGE_DOWN", bForceChange: true, bImmediate: false, stateChangePriority);
							}
							else
							{
								StateChange("USN_DAMAGE_AIR_DOWN", bForceChange: true, bImmediate: false, stateChangePriority);
							}
						}
						else
						{
							StateChange("USN_DAMAGE_A", bForceChange: true, bImmediate: false, stateChangePriority);
						}
					}
					else
					{
						StateChange("USN_DAMAGE_A", bForceChange: true, bImmediate: false, stateChangePriority);
					}
					break;
				case NKM_REACT_TYPE.NRT_DAMAGE_UP:
					if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_AIRBORNE))
					{
						break;
					}
					if (!IsAirUnit())
					{
						if (m_UnitTemplet.m_fDamageUpFactor > 0f)
						{
							StateChange("USN_DAMAGE_AIR_UP", bForceChange: true, bImmediate: false, stateChangePriority);
						}
						else
						{
							StateChange("USN_DAMAGE_A", bForceChange: true, bImmediate: false, stateChangePriority);
						}
					}
					else
					{
						StateChange("USN_DAMAGE_B", bForceChange: true, bImmediate: false, stateChangePriority);
					}
					break;
				}
				if (NKMDamageManager.IsHitStunReact(cNKMDamageInst.m_ReActResult))
				{
					OnReactionEvent(NKMUnitReaction.ReactionEventType.TAKE_HITSTUN, this, 1);
					BroadcastReactionEvent(NKMUnitReaction.ReactionEventType.UNIT_TAKE_HITSTUN, this, 1);
				}
			}
		}
		if (m_StateNameNext.Length <= 1)
		{
			m_PushSyncData = true;
		}
	}

	public bool HasBarrierBuff()
	{
		if (GetUnitFrameData().m_BarrierBuffData == null)
		{
			return false;
		}
		if (GetUnitFrameData().m_BarrierBuffData.m_fBarrierHP > 0f)
		{
			return true;
		}
		return false;
	}

	public void SetUnitSkillColorFade(float fTime)
	{
		m_UnitFrameData.m_fColorEventTime = fTime;
		if (fTime > 0f)
		{
			m_UnitFrameData.m_ColorR.SetTracking(0.3f, 0.2f, TRACKING_DATA_TYPE.TDT_SLOWER);
			m_UnitFrameData.m_ColorG.SetTracking(0.3f, 0.2f, TRACKING_DATA_TYPE.TDT_SLOWER);
			m_UnitFrameData.m_ColorB.SetTracking(0.3f, 0.2f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		else
		{
			m_UnitFrameData.m_ColorR.SetNowValue(m_UnitTemplet.m_ColorR);
			m_UnitFrameData.m_ColorG.SetNowValue(m_UnitTemplet.m_ColorG);
			m_UnitFrameData.m_ColorB.SetNowValue(m_UnitTemplet.m_ColorB);
		}
	}

	private bool CanRevenge(NKMDamageInst cNKMDamageInst, bool bBuffDamage, out string revengeState)
	{
		revengeState = null;
		if (cNKMDamageInst.m_bAttackerAwaken && !GetUnitTemplet().m_UnitTempletBase.m_bAwaken)
		{
			return false;
		}
		if (m_UnitStateNow.m_NKM_SKILL_TYPE >= NKM_SKILL_TYPE.NST_HYPER)
		{
			return false;
		}
		if (IsAlly(cNKMDamageInst.m_AttackerGameUnitUID))
		{
			return false;
		}
		if (!m_UnitFrameData.m_bFootOnLand)
		{
			return false;
		}
		if (m_UnitStateNow.IsBadStatusState || HasCrowdControlStatus())
		{
			return false;
		}
		if (m_UnitStateNow.m_bNormalRevengeState && cNKMDamageInst.m_Templet.m_CrashSuperArmorLevel < NKM_SUPER_ARMOR_LEVEL.NSAL_SKILL && !bBuffDamage)
		{
			if (m_UnitStateNow.m_RevengeChangeState.Length > 1)
			{
				revengeState = m_UnitStateNow.m_RevengeChangeState;
			}
			return true;
		}
		if (!IsCrowdControlAttack(cNKMDamageInst))
		{
			return false;
		}
		if (m_UnitStateNow.m_bRevengeState && cNKMDamageInst.m_Templet.m_CrashSuperArmorLevel <= NKM_SUPER_ARMOR_LEVEL.NSAL_SKILL)
		{
			if (m_UnitStateNow.m_RevengeChangeState.Length > 1)
			{
				revengeState = m_UnitStateNow.m_RevengeChangeState;
			}
			return true;
		}
		if (m_UnitStateNow.m_bSuperRevengeState)
		{
			if (m_UnitStateNow.m_RevengeChangeState.Length > 1)
			{
				revengeState = m_UnitStateNow.m_RevengeChangeState;
			}
			return true;
		}
		if (!HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE))
		{
			int attackIndex = 0;
			if (CanUseAttack(m_UnitTemplet.m_listPassiveAttackStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_REVENGE, bAirUnit: false, 0f, ref attackIndex))
			{
				revengeState = m_UnitTemplet.m_listHyperSkillStateData[attackIndex].m_StateName;
				return true;
			}
			if (CanUseAttack(m_UnitTemplet.m_listHyperSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_REVENGE, bAirUnit: false, 0f, ref attackIndex))
			{
				revengeState = m_UnitTemplet.m_listHyperSkillStateData[attackIndex].m_StateName;
				return true;
			}
			if (CanUseAttack(m_UnitTemplet.m_listSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_REVENGE, bAirUnit: false, 0f, ref attackIndex) && (cNKMDamageInst.m_Templet.m_CrashSuperArmorLevel <= NKM_SUPER_ARMOR_LEVEL.NSAL_SKILL || cNKMDamageInst.m_Templet.m_bCanRevenge))
			{
				revengeState = m_UnitTemplet.m_listSkillStateData[attackIndex].m_StateName;
				return true;
			}
		}
		return false;
	}

	private bool IsCrowdControlAttack(NKMDamageInst cNKMDamageInst)
	{
		if (NKMDamageManager.IsHitStunReact(cNKMDamageInst.m_Templet.m_ReActType))
		{
			return true;
		}
		if (cNKMDamageInst.m_Templet.m_fStunTime > 0f)
		{
			return true;
		}
		if (NKMUnitStatusTemplet.IsCrowdControlStatus(cNKMDamageInst.m_Templet.m_ApplyStatusEffect))
		{
			return true;
		}
		if (!string.IsNullOrEmpty(cNKMDamageInst.m_Templet.m_DefenderBuff) && NKMBuffManager.GetBuffTempletByStrID(cNKMDamageInst.m_Templet.m_DefenderBuff).HasCrowdControlStatus())
		{
			return true;
		}
		NKMUnit unit = m_NKMGame.GetUnit(cNKMDamageInst.m_AttackerGameUnitUID, bChain: true, bPool: true);
		if (unit == null)
		{
			return false;
		}
		for (int i = 0; i < cNKMDamageInst.m_Templet.m_listDefenderHitBuff.Count; i++)
		{
			NKMHitBuff nKMHitBuff = cNKMDamageInst.m_Templet.m_listDefenderHitBuff[i];
			if (unit.CheckEventCondition(nKMHitBuff.m_Condition) && NKMBuffManager.GetBuffTempletByStrID(nKMHitBuff.m_HitBuff).HasCrowdControlStatus())
			{
				return true;
			}
		}
		return false;
	}

	private bool CanGuard(NKMDamageInst cNKMDamageInst, bool bBuffDamage, NKMUnit defender, out string guardState)
	{
		guardState = null;
		if (bBuffDamage)
		{
			return false;
		}
		if (defender == null)
		{
			return false;
		}
		if (IsDyingOrDie())
		{
			return false;
		}
		if (cNKMDamageInst.m_bAttackerAwaken && !GetUnitTemplet().m_UnitTempletBase.m_bAwaken)
		{
			return false;
		}
		if (IsAlly(cNKMDamageInst.m_AttackerGameUnitUID))
		{
			return false;
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE))
		{
			return false;
		}
		if (HasCrowdControlStatus())
		{
			return false;
		}
		if (m_UnitStateNow != null && m_UnitStateNow.m_NKM_SKILL_TYPE >= NKM_SKILL_TYPE.NST_HYPER)
		{
			return false;
		}
		if (defender.GetUnitDataGame().m_GameUnitUID != GetUnitDataGame().m_GameUnitUID && cNKMDamageInst.m_listHitUnit.Contains(GetUnitDataGame().m_GameUnitUID))
		{
			return false;
		}
		if (m_UnitFrameData.m_bFootOnLand && (NKMDamageManager.IsHitStunReact(cNKMDamageInst.m_Templet.m_ReActType) || cNKMDamageInst.m_Templet.m_fStunTime > 0f || NKMUnitStatusTemplet.IsCrowdControlStatus(cNKMDamageInst.m_Templet.m_ApplyStatusEffect)))
		{
			int attackIndex = 0;
			if (defender.IsAirUnit())
			{
				if (CanUseAttack(m_UnitTemplet.m_listAirHyperSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_GUARD, bAirUnit: false, 0f, ref attackIndex))
				{
					guardState = m_UnitTemplet.m_listAirHyperSkillStateData[attackIndex].m_StateName;
					return true;
				}
				if (CanUseAttack(m_UnitTemplet.m_listAirSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_GUARD, bAirUnit: false, 0f, ref attackIndex) && (cNKMDamageInst.m_Templet.m_CrashSuperArmorLevel <= NKM_SUPER_ARMOR_LEVEL.NSAL_SKILL || cNKMDamageInst.m_Templet.m_bCanRevenge))
				{
					guardState = m_UnitTemplet.m_listAirSkillStateData[attackIndex].m_StateName;
					return true;
				}
				if (CanUseAttack(m_UnitTemplet.m_listAirPassiveAttackStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_GUARD, bAirUnit: false, 0f, ref attackIndex) && (cNKMDamageInst.m_Templet.m_CrashSuperArmorLevel <= NKM_SUPER_ARMOR_LEVEL.NSAL_SKILL || cNKMDamageInst.m_Templet.m_bCanRevenge))
				{
					guardState = m_UnitTemplet.m_listAirPassiveAttackStateData[attackIndex].m_StateName;
					return true;
				}
			}
			else
			{
				if (CanUseAttack(m_UnitTemplet.m_listHyperSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_GUARD, bAirUnit: false, 0f, ref attackIndex))
				{
					guardState = m_UnitTemplet.m_listHyperSkillStateData[attackIndex].m_StateName;
					return true;
				}
				if (CanUseAttack(m_UnitTemplet.m_listSkillStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_GUARD, bAirUnit: false, 0f, ref attackIndex) && (cNKMDamageInst.m_Templet.m_CrashSuperArmorLevel <= NKM_SUPER_ARMOR_LEVEL.NSAL_SKILL || cNKMDamageInst.m_Templet.m_bCanRevenge))
				{
					guardState = m_UnitTemplet.m_listSkillStateData[attackIndex].m_StateName;
					return true;
				}
				if (CanUseAttack(m_UnitTemplet.m_listPassiveAttackStateData, NKM_ATTACK_STATE_DATA_TYPE.NASDT_GUARD, bAirUnit: false, 0f, ref attackIndex) && (cNKMDamageInst.m_Templet.m_CrashSuperArmorLevel <= NKM_SUPER_ARMOR_LEVEL.NSAL_SKILL || cNKMDamageInst.m_Templet.m_bCanRevenge))
				{
					guardState = m_UnitTemplet.m_listPassiveAttackStateData[attackIndex].m_StateName;
					return true;
				}
			}
		}
		return false;
	}

	public virtual void AttackResult(NKMDamageInst cNKMDamageInst)
	{
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY || cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_NO)
		{
			return;
		}
		SetStopTime(cNKMDamageInst.m_Templet.m_fStopTimeAtk, NKM_STOP_TIME_INDEX.NSTI_DAMAGE);
		SetStopReserveTime(cNKMDamageInst.m_Templet.m_fStopReserveTimeAtk);
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			return;
		}
		if (cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_REVENGE)
		{
			if (!GetUnitTemplet().m_bNoDamageState)
			{
				if (!IsAirUnit())
				{
					if (m_UnitFrameData.m_bFootOnLand)
					{
						StateChange("USN_DAMAGE_A", bForceChange: true, bImmediate: false, NKM_STATE_CHANGE_PRIORITY.NSCP_SKILL_CRASH);
					}
					else if (!GetUnitTemplet().m_bNoDamageDownState)
					{
						StateChange("USN_DAMAGE_AIR_DOWN", bForceChange: true, bImmediate: false, NKM_STATE_CHANGE_PRIORITY.NSCP_SKILL_CRASH);
					}
					else
					{
						StateChange("USN_DAMAGE_A", bForceChange: true, bImmediate: false, NKM_STATE_CHANGE_PRIORITY.NSCP_SKILL_CRASH);
					}
				}
				else
				{
					StateChange("USN_DAMAGE_A", bForceChange: true, bImmediate: false, NKM_STATE_CHANGE_PRIORITY.NSCP_SKILL_CRASH);
				}
			}
			else
			{
				StateChangeToASTAND();
			}
			SetStopTime(0.5f, NKM_STOP_TIME_INDEX.NSTI_DAMAGE);
			SetStopReserveTime(0.1f);
		}
		else if (cNKMDamageInst.m_Templet.m_AttackerStateChange.Length > 1)
		{
			StateChange(cNKMDamageInst.m_Templet.m_AttackerStateChange);
		}
	}

	public bool IsDie()
	{
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE)
		{
			return true;
		}
		return false;
	}

	public bool IsDying()
	{
		if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING)
		{
			return true;
		}
		return false;
	}

	public bool IsDyingOrDie()
	{
		if (IsDying())
		{
			return true;
		}
		if (IsDie())
		{
			return true;
		}
		return false;
	}

	public void AddDamage(bool bAttackCountOver, float fDamage, NKM_DAMAGE_RESULT_TYPE eNKM_DAMAGE_RESULT_TYPE, short attackUnitGameUid, NKM_TEAM_TYPE teamType, bool bPushSyncData = false, bool bNoRedirect = false, bool bInstaKill = false, bool bAddDamageRecord = true)
	{
		if (eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_COOL_TIME)
		{
			if (!AddCoolTimeDamage(fDamage))
			{
				return;
			}
		}
		else
		{
			if (!bInstaKill && HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IRONWALL) && fDamage > 0f)
			{
				fDamage = ((eNKM_DAMAGE_RESULT_TYPE != NKM_DAMAGE_RESULT_TYPE.NDRT_MISS) ? 1f : 0f);
			}
			if (!bNoRedirect && !bInstaKill && fDamage > 1f)
			{
				NKMUnit nKMUnit = null;
				if (GetUnitFrameData().m_fDamageTransferGameUnitUID > 0)
				{
					nKMUnit = m_NKMGame.GetUnit(GetUnitFrameData().m_fDamageTransferGameUnitUID);
				}
				if (GetUnitFrameData().m_fDamageTransfer > 0f && nKMUnit != null && !nKMUnit.IsDyingOrDie())
				{
					nKMUnit.AddDamage(bAttackCountOver, fDamage * GetUnitFrameData().m_fDamageTransfer, NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL, attackUnitGameUid, teamType, bPushSyncData: true, bNoRedirect: true);
					if (GetUnitFrameData().m_fDamageTransfer > 0.99f)
					{
						return;
					}
					fDamage *= 1f - GetUnitFrameData().m_fDamageTransfer;
				}
				if (fDamage > 0f && !HasBarrierBuff() && GetUnitFrameData().m_fDamageReflection > 0f)
				{
					NKMUnit unit = m_NKMGame.GetUnit(attackUnitGameUid);
					if (unit != null && !unit.IsDyingOrDie())
					{
						float num = Math.Min(fDamage, GetUnitSyncData().GetHP() - m_UnitFrameData.m_fDamageThisFrame);
						if (num > 0f)
						{
							unit.AddDamage(bAttackCountOver, num * GetUnitFrameData().m_fDamageReflection, NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL, GetUnitDataGame().m_GameUnitUID, teamType, bPushSyncData: true, bNoRedirect: true);
						}
					}
				}
			}
			m_UnitFrameData.m_fDamageThisFrame += fDamage;
		}
		NKMDamageData nKMDamageData = new NKMDamageData();
		nKMDamageData.m_bAttackCountOver = bAttackCountOver;
		nKMDamageData.m_FinalDamage = (int)fDamage;
		nKMDamageData.m_NKM_DAMAGE_RESULT_TYPE = eNKM_DAMAGE_RESULT_TYPE;
		nKMDamageData.m_GameUnitUIDAttacker = attackUnitGameUid;
		m_UnitSyncData.m_listDamageData.Add(nKMDamageData);
		if (bPushSyncData)
		{
			m_PushSyncData = true;
		}
		if (eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL || eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_NO_MARK || eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_CRITICAL || eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_WEAK || eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_PROTECT || eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_MISS)
		{
			GetUnitFrameData().m_DangerChargeHitCount++;
		}
		if (eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_NO_MARK)
		{
			return;
		}
		if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER && bAddDamageRecord)
		{
			m_NKMGame.m_GameRecord.AddDamage(attackUnitGameUid, teamType, this, fDamage);
		}
		if (m_NKMGame.GetDungeonType() == NKM_DUNGEON_TYPE.NDT_FIERCE && IsBTeam() && IsBoss())
		{
			float num2 = m_NKMGame.CaculateFiercePointByDamage(this);
			if (num2 > m_NKMGame.m_GameRecord.TotalFiercePoint)
			{
				m_NKMGame.m_GameRecord.SetTotalFiercePoint(num2, 0f);
			}
		}
		if (m_NKMGame.GetDungeonType() == NKM_DUNGEON_TYPE.NDT_TRIM && IsBTeam() && IsBoss())
		{
			float num3 = m_NKMGame.CaculateTrimPointByDamage(this);
			if (num3 > m_NKMGame.m_GameRecord.TotalTrimPoint)
			{
				m_NKMGame.m_GameRecord.SetTotalTrimPoint(num3);
			}
		}
	}

	public bool AddCoolTimeDamage(float fDamage)
	{
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_SKILL_COOLTIME_DAMAGE))
		{
			return false;
		}
		if (GetUnitTemplet().m_UnitTempletBase.StopDefaultCoolTime)
		{
			return false;
		}
		if (GetUnitTemplet().m_listSkillStateData.Count > 0)
		{
			NKMAttackStateData nKMAttackStateData = GetUnitTemplet().m_listSkillStateData[0];
			NKMUnitState unitState = GetUnitState(nKMAttackStateData.m_StateName);
			if (unitState != null)
			{
				SetStateCoolTimeAdd(unitState, fDamage);
			}
		}
		if (GetUnitTemplet().m_listHyperSkillStateData.Count > 0)
		{
			NKMAttackStateData nKMAttackStateData = GetUnitTemplet().m_listHyperSkillStateData[0];
			NKMUnitState unitState2 = GetUnitState(nKMAttackStateData.m_StateName);
			if (unitState2 != null)
			{
				SetStateCoolTimeAdd(unitState2, fDamage);
			}
		}
		return true;
	}

	public bool WillKilledByDamage(float incomingDamage)
	{
		if (HasBarrierBuff())
		{
			return false;
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMORTAL))
		{
			return false;
		}
		if (!IsMonster() && GetPhaseDamageLimit(out var _) > 0f)
		{
			return false;
		}
		if (GetUnitSyncData().GetHP() <= GetUnitFrameData().m_fDamageThisFrame)
		{
			return false;
		}
		return GetUnitSyncData().GetHP() <= GetUnitFrameData().m_fDamageThisFrame + incomingDamage;
	}

	public void Kill(NKMKillFeedBack cNKMKillFeedBack, short killGameUnitUID)
	{
		for (int i = 0; i < GetUnitTemplet().m_listKillFeedBack.Count; i++)
		{
			KillFeedBack(GetUnitTemplet().m_listKillFeedBack[i], killGameUnitUID);
		}
		KillFeedBack(cNKMKillFeedBack, killGameUnitUID);
		GetUnitFrameData().m_KillCount++;
		m_NKMGame.AddKillCount(m_UnitDataGame, killGameUnitUID);
		NKMUnit unit = m_NKMGame.GetUnit(killGameUnitUID);
		OnReactionEvent(NKMUnitReaction.ReactionEventType.KILL, unit, 1);
	}

	protected void KillFeedBack(NKMKillFeedBack cNKMKillFeedBack, short killGameUnitUID)
	{
		if (cNKMKillFeedBack == null || !CheckEventCondition(cNKMKillFeedBack.m_Condition))
		{
			return;
		}
		List<NKMKillFeedBack> list = null;
		if (m_dicKillFeedBackGameUnitUID.ContainsKey(killGameUnitUID))
		{
			list = m_dicKillFeedBackGameUnitUID[killGameUnitUID];
			for (int i = 0; i < list.Count; i++)
			{
				NKMKillFeedBack nKMKillFeedBack = list[i];
				if (cNKMKillFeedBack == nKMKillFeedBack)
				{
					return;
				}
			}
			list.Add(cNKMKillFeedBack);
		}
		else
		{
			list = new List<NKMKillFeedBack>();
			list.Add(cNKMKillFeedBack);
			m_dicKillFeedBackGameUnitUID.Add(killGameUnitUID, list);
		}
		if (cNKMKillFeedBack.m_fSkillCoolTime > 0f && GetUnitTemplet().m_listSkillStateData.Count > 0)
		{
			NKMAttackStateData nKMAttackStateData = GetUnitTemplet().m_listSkillStateData[0];
			if (nKMAttackStateData != null)
			{
				NKMUnitState unitState = GetUnitState(nKMAttackStateData.m_StateName);
				if (unitState != null && GetStateCoolTime(unitState) > cNKMKillFeedBack.m_fSkillCoolTime)
				{
					SetStateCoolTime(unitState, cNKMKillFeedBack.m_fSkillCoolTime);
				}
			}
		}
		if (cNKMKillFeedBack.m_fHyperSkillCoolTime > 0f && GetUnitTemplet().m_listHyperSkillStateData.Count > 0)
		{
			NKMAttackStateData nKMAttackStateData2 = GetUnitTemplet().m_listHyperSkillStateData[0];
			if (nKMAttackStateData2 != null)
			{
				NKMUnitState unitState2 = GetUnitState(nKMAttackStateData2.m_StateName);
				if (unitState2 != null && GetStateCoolTime(unitState2) > cNKMKillFeedBack.m_fHyperSkillCoolTime)
				{
					SetStateCoolTime(unitState2, cNKMKillFeedBack.m_fHyperSkillCoolTime);
				}
			}
		}
		if (!cNKMKillFeedBack.m_fSkillCoolTimeAdd.IsNearlyZero() && GetUnitTemplet().m_listSkillStateData.Count > 0)
		{
			NKMAttackStateData nKMAttackStateData3 = GetUnitTemplet().m_listSkillStateData[0];
			if (nKMAttackStateData3 != null)
			{
				NKMUnitState unitState3 = GetUnitState(nKMAttackStateData3.m_StateName);
				if (unitState3 != null)
				{
					SetStateCoolTimeAdd(unitState3, cNKMKillFeedBack.m_fSkillCoolTimeAdd);
				}
			}
		}
		if (!cNKMKillFeedBack.m_fHyperSkillCoolTimeAdd.IsNearlyZero() && GetUnitTemplet().m_listHyperSkillStateData.Count > 0)
		{
			NKMAttackStateData nKMAttackStateData4 = GetUnitTemplet().m_listHyperSkillStateData[0];
			if (nKMAttackStateData4 != null)
			{
				NKMUnitState unitState4 = GetUnitState(nKMAttackStateData4.m_StateName);
				if (unitState4 != null)
				{
					SetStateCoolTimeAdd(unitState4, cNKMKillFeedBack.m_fHyperSkillCoolTimeAdd);
				}
			}
		}
		if (cNKMKillFeedBack.m_fHPRate > 0f)
		{
			float fHeal = GetMaxHP() * cNKMKillFeedBack.m_fHPRate;
			SetHeal(fHeal, GetUnitSyncData().m_GameUnitUID);
		}
		if (cNKMKillFeedBack.m_BuffName.Length > 1)
		{
			AddBuffByStrID(cNKMKillFeedBack.m_BuffName, cNKMKillFeedBack.m_BuffStatLevel, cNKMKillFeedBack.m_BuffTimeLevel, m_UnitDataGame.m_GameUnitUID, bUseMasterStat: true, bRangeSon: false);
		}
		if (!string.IsNullOrEmpty(cNKMKillFeedBack.m_TargetTrigger))
		{
			InvokeTrigger(cNKMKillFeedBack.m_TargetTrigger);
		}
	}

	public void StateCoolTimeReset(string stateName)
	{
		NKMUnitState unitState = GetUnitState(stateName);
		if (unitState != null)
		{
			StateCoolTimeReset(unitState.m_StateID);
		}
	}

	public void StateCoolTimeReset(int stateID)
	{
		if (m_dicStateCoolTime.ContainsKey(stateID))
		{
			m_dicStateCoolTime[stateID].m_CoolTime = 0f;
			m_PushSyncData = true;
		}
	}

	public void AddDynamicRespawnPool(NKMEventRespawn cNKMEventRespawn, short gameUnitUID)
	{
		if (!m_dicDynamicRespawnPool.ContainsKey(cNKMEventRespawn))
		{
			m_dicDynamicRespawnPool.Add(cNKMEventRespawn, new List<short>());
		}
		m_dicDynamicRespawnPool[cNKMEventRespawn].Add(gameUnitUID);
	}

	public short GetDynamicRespawnPool(NKMEventRespawn cNKMEventRespawn)
	{
		if (!m_dicDynamicRespawnPool.ContainsKey(cNKMEventRespawn))
		{
			return 0;
		}
		List<short> list = m_dicDynamicRespawnPool[cNKMEventRespawn];
		for (int i = 0; i < list.Count; i++)
		{
			short num = list[i];
			if (m_NKMGame.GetUnit(num, bChain: false, bPool: true) != null && !m_NKMGame.IsInDynamicRespawnUnitReserve(num))
			{
				return num;
			}
		}
		return 0;
	}

	public short GetDynamicRespawnPoolReRespawn(NKMEventRespawn cNKMEventRespawn)
	{
		if (!m_dicDynamicRespawnPool.ContainsKey(cNKMEventRespawn))
		{
			return 0;
		}
		List<short> list = m_dicDynamicRespawnPool[cNKMEventRespawn];
		for (int i = 0; i < list.Count; i++)
		{
			short num = list[i];
			if (m_NKMGame.GetUnit(num) != null && !m_NKMGame.IsInDynamicRespawnUnitReserve(num))
			{
				return num;
			}
		}
		return 0;
	}

	public void AddUnitChangeRespawnPool(string unitStrID, short gameUnitUID)
	{
		if (!m_dicUnitChangeRespawnPool.ContainsKey(unitStrID))
		{
			m_dicUnitChangeRespawnPool.Add(unitStrID, new List<short>());
		}
		m_dicUnitChangeRespawnPool[unitStrID].Add(gameUnitUID);
	}

	public short GetUnitChangeRespawnPool(string unitStrID)
	{
		if (!m_dicUnitChangeRespawnPool.ContainsKey(unitStrID))
		{
			return 0;
		}
		List<short> list = m_dicUnitChangeRespawnPool[unitStrID];
		for (int i = 0; i < list.Count; i++)
		{
			short num = list[i];
			if (m_NKMGame.GetUnit(num, bChain: false, bPool: true) != null)
			{
				return num;
			}
		}
		return 0;
	}

	public void SetHitFeedBack()
	{
		for (int i = 0; i < GetUnitTemplet().m_listHitFeedBack.Count; i++)
		{
			NKMHitFeedBack nKMHitFeedBack = GetUnitTemplet().m_listHitFeedBack[i];
			if (!CheckEventCondition(nKMHitFeedBack.m_Condition))
			{
				continue;
			}
			GetUnitFrameData().m_listHitFeedBackCount[i] = (byte)(GetUnitFrameData().m_listHitFeedBackCount[i] + 1);
			if (GetUnitFrameData().m_listHitFeedBackCount[i] >= nKMHitFeedBack.m_Count)
			{
				bool flag = false;
				if (nKMHitFeedBack.m_bStartAnyTime)
				{
					flag = true;
				}
				NKM_UNIT_STATE_TYPE nKM_UNIT_STATE_TYPE = GetUnitStateNow().m_NKM_UNIT_STATE_TYPE;
				if ((nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_DAMAGE || nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_ASTAND || nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_MOVE || nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_ATTACK) && GetUnitStateNow().m_NKM_SKILL_TYPE <= NKM_SKILL_TYPE.NST_ATTACK)
				{
					flag = true;
				}
				if (flag)
				{
					GetUnitFrameData().m_listHitFeedBackCount[i] = 0;
					ApplyHitFeedback(nKMHitFeedBack);
				}
			}
		}
	}

	public void SetHitCriticalFeedBack()
	{
		for (int i = 0; i < GetUnitTemplet().m_listHitCriticalFeedBack.Count; i++)
		{
			NKMHitFeedBack nKMHitFeedBack = GetUnitTemplet().m_listHitCriticalFeedBack[i];
			if (!CheckEventCondition(nKMHitFeedBack.m_Condition))
			{
				continue;
			}
			GetUnitFrameData().m_listHitCriticalFeedBackCount[i] = (byte)(GetUnitFrameData().m_listHitCriticalFeedBackCount[i] + 1);
			if (GetUnitFrameData().m_listHitCriticalFeedBackCount[i] >= nKMHitFeedBack.m_Count)
			{
				bool flag = false;
				if (nKMHitFeedBack.m_bStartAnyTime)
				{
					flag = true;
				}
				NKM_UNIT_STATE_TYPE nKM_UNIT_STATE_TYPE = GetUnitStateNow().m_NKM_UNIT_STATE_TYPE;
				if ((nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_DAMAGE || nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_ASTAND || nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_MOVE || nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_ATTACK) && GetUnitStateNow().m_NKM_SKILL_TYPE <= NKM_SKILL_TYPE.NST_ATTACK)
				{
					flag = true;
				}
				if (flag)
				{
					GetUnitFrameData().m_listHitCriticalFeedBackCount[i] = 0;
					ApplyHitFeedback(nKMHitFeedBack);
				}
			}
		}
	}

	public void SetHitEvadeFeedBack()
	{
		for (int i = 0; i < GetUnitTemplet().m_listHitEvadeFeedBack.Count; i++)
		{
			NKMHitFeedBack nKMHitFeedBack = GetUnitTemplet().m_listHitEvadeFeedBack[i];
			if (!CheckEventCondition(nKMHitFeedBack.m_Condition))
			{
				continue;
			}
			GetUnitFrameData().m_listHitEvadeFeedBackCount[i] = (byte)(GetUnitFrameData().m_listHitEvadeFeedBackCount[i] + 1);
			if (GetUnitFrameData().m_listHitEvadeFeedBackCount[i] >= nKMHitFeedBack.m_Count)
			{
				bool flag = false;
				if (nKMHitFeedBack.m_bStartAnyTime)
				{
					flag = true;
				}
				NKM_UNIT_STATE_TYPE nKM_UNIT_STATE_TYPE = GetUnitStateNow().m_NKM_UNIT_STATE_TYPE;
				if ((nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_DAMAGE || nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_ASTAND || nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_MOVE || nKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_ATTACK) && GetUnitStateNow().m_NKM_SKILL_TYPE <= NKM_SKILL_TYPE.NST_ATTACK)
				{
					flag = true;
				}
				if (flag)
				{
					GetUnitFrameData().m_listHitEvadeFeedBackCount[i] = 0;
					ApplyHitFeedback(nKMHitFeedBack);
				}
			}
		}
	}

	public void ApplyHitFeedback(NKMHitFeedBack hitFeedback)
	{
		if (hitFeedback.m_BuffStrID.Length > 1)
		{
			AddBuffByStrID(hitFeedback.m_BuffStrID, hitFeedback.m_BuffStatLevel, hitFeedback.m_BuffTimeLevel, GetUnitSyncData().m_GameUnitUID, bUseMasterStat: true, bRangeSon: false);
		}
		if (hitFeedback.m_StateName.Length > 1 && CheckStateCoolTime(hitFeedback.m_StateName))
		{
			StateChange(hitFeedback.m_StateName, bForceChange: false);
		}
		if (!string.IsNullOrEmpty(hitFeedback.m_TargetTrigger))
		{
			InvokeTrigger(hitFeedback.m_TargetTrigger);
		}
	}

	public NKMUnit GetMasterUnit(bool bPool = true)
	{
		if (GetUnitDataGame().m_MasterGameUnitUID == 0)
		{
			return null;
		}
		return m_NKMGame.GetUnit(GetUnitDataGame().m_MasterGameUnitUID, bChain: true, bPool);
	}

	public bool HasMasterUnit()
	{
		return GetUnitDataGame().m_MasterGameUnitUID != 0;
	}

	public short GetMasterUnitGameUID()
	{
		return GetUnitDataGame().m_MasterGameUnitUID;
	}

	public short GetUnitGameUID()
	{
		return GetUnitDataGame().m_GameUnitUID;
	}

	public bool IsSummonUnit()
	{
		return GetUnitData().m_bSummonUnit;
	}

	public bool IsChangeUnit()
	{
		return GetUnitDataGame().m_bChangeUnit;
	}

	public void CalcSortDist(float originPosX)
	{
		m_TempSortDist = Math.Abs(GetUnitSyncData().m_PosX - originPosX);
	}

	public void SetSortDist(float distance)
	{
		m_TempSortDist = distance;
	}

	public List<NKMUnit> GetSortUnitListByNearDist(bool bUseUnitSize)
	{
		if (bUseUnitSize)
		{
			return GetSortUnitListByNearDistAndUnitSize();
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
					nKMUnit.CalcSortDist(GetUnitSyncData().m_PosX);
					m_listSortUnit.Add(nKMUnit);
				}
			}
			m_listSortUnit.Sort((NKMUnit a, NKMUnit b) => a.GetTempSortDist().CompareTo(b.GetTempSortDist()));
			m_bSortUnitDirty = false;
		}
		return m_listSortUnit;
	}

	public List<NKMUnit> GetSortUnitListByNearDistAndUnitSize()
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
					float dist = GetDist(nKMUnit, bUseUnitSize: true);
					nKMUnit.SetSortDist(dist);
					m_listSortUnitBySize.Add(nKMUnit);
				}
			}
			m_listSortUnitBySize.Sort((NKMUnit a, NKMUnit b) => a.GetTempSortDist().CompareTo(b.GetTempSortDist()));
			m_bSortUnitBySizeDirty = false;
		}
		return m_listSortUnitBySize;
	}

	public NKMUnit GetSortUnit(bool bNearFirst, bool bExceptMyTeam = true, bool bExceptEnemyTeam = true, float fUseNearDist = 0f, float fUseFarDist = 0f, bool bExceptMe = true, bool bExceptEnv = true, bool bExceptDie = true)
	{
		List<NKMUnit> sortUnitListByNearDist = GetSortUnitListByNearDist();
		for (int i = 0; i < sortUnitListByNearDist.Count; i++)
		{
			NKMUnit nKMUnit = null;
			nKMUnit = ((!bNearFirst) ? sortUnitListByNearDist[sortUnitListByNearDist.Count - 1 - i] : sortUnitListByNearDist[i]);
			if (nKMUnit != null && (!bExceptMe || nKMUnit.GetUnitSyncData().m_GameUnitUID != GetUnitSyncData().m_GameUnitUID) && (!bExceptEnv || nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_ENV) && (!bExceptDie || (nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DYING && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE)) && (!bExceptMyTeam || m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE)) && (!bExceptEnemyTeam || !m_NKMGame.IsEnemy(GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE)) && (fUseNearDist.IsNearlyZero() || !(fUseNearDist < Math.Abs(GetUnitSyncData().m_PosX - nKMUnit.GetUnitSyncData().m_PosX))) && (fUseFarDist.IsNearlyZero() || !(fUseFarDist > Math.Abs(GetUnitSyncData().m_PosX - nKMUnit.GetUnitSyncData().m_PosX))))
			{
				return nKMUnit;
			}
		}
		return null;
	}

	public void AddEventMark(NKM_UNIT_EVENT_MARK eNKM_UNIT_EVENT_MARK, float value = 0f)
	{
		GetUnitSyncData().m_listNKM_UNIT_EVENT_MARK.Add(new NKMUnitEventSyncData(eNKM_UNIT_EVENT_MARK, value));
		m_bPushSimpleSyncData = true;
	}

	protected void AddInvokedTrigger(NKMUnit triggerMaster, int triggerID)
	{
		GetUnitSyncData().m_listInvokedTrigger.Add(new NKMUnitSyncData.InvokedTriggerInfo(triggerMaster, triggerID));
		m_bPushSimpleSyncData = true;
	}

	public virtual void OnGameEnd()
	{
	}

	public bool IsATeam()
	{
		return m_NKMGame.IsATeam(GetUnitDataGame().m_NKM_TEAM_TYPE);
	}

	public bool IsBTeam()
	{
		return m_NKMGame.IsBTeam(GetUnitDataGame().m_NKM_TEAM_TYPE);
	}

	public NKM_TEAM_TYPE GetTeam()
	{
		return GetUnitDataGame().m_NKM_TEAM_TYPE;
	}

	public NKMGameTeamData GetTeamData()
	{
		if (m_NKMGame == null)
		{
			return null;
		}
		if (m_NKMGame.GetGameData() == null)
		{
			return null;
		}
		return m_NKMGame.GetGameData().GetTeamData(GetTeam());
	}

	public bool IsAlly(short gameUnitUID)
	{
		NKMUnit unit = m_NKMGame.GetUnit(gameUnitUID, bChain: true, bPool: true);
		return IsAlly(unit);
	}

	public bool IsAlly(NKMUnit other)
	{
		if (other == null)
		{
			return false;
		}
		NKM_TEAM_TYPE nKM_TEAM_TYPE = other.m_UnitDataGame.m_NKM_TEAM_TYPE;
		return IsAlly(nKM_TEAM_TYPE);
	}

	public bool IsAlly(NKM_TEAM_TYPE otherTeam)
	{
		switch (m_UnitDataGame.m_NKM_TEAM_TYPE)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			if (otherTeam != NKM_TEAM_TYPE.NTT_A1)
			{
				return otherTeam == NKM_TEAM_TYPE.NTT_A2;
			}
			return true;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			if (otherTeam != NKM_TEAM_TYPE.NTT_B1)
			{
				return otherTeam == NKM_TEAM_TYPE.NTT_B2;
			}
			return true;
		default:
			return false;
		}
	}

	public bool IsEnemy(NKMUnit other)
	{
		if (other == null)
		{
			return false;
		}
		return m_NKMGame.IsEnemy(GetTeam(), other.GetTeam());
	}

	public bool IsAirUnit()
	{
		if (GetUnitTemplet() == null)
		{
			return false;
		}
		if (GetUnitTemplet().m_UnitTempletBase == null)
		{
			return false;
		}
		if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_AIR_TO_GROUND))
		{
			return false;
		}
		if (m_UnitStateNow != null && m_UnitStateNow.m_bChangeIsAirUnit)
		{
			return !GetUnitTemplet().m_UnitTempletBase.m_bAirUnit;
		}
		return GetUnitTemplet().m_UnitTempletBase.m_bAirUnit;
	}

	public bool CanApplyStatus(NKM_UNIT_STATUS_EFFECT status)
	{
		if (status == NKM_UNIT_STATUS_EFFECT.NUSE_NONE)
		{
			return false;
		}
		if (IsImmuneStatus(status))
		{
			return false;
		}
		return true;
	}

	public bool IsImmuneStatus(NKM_UNIT_STATUS_EFFECT status)
	{
		if (status == NKM_UNIT_STATUS_EFFECT.NUSE_NONE)
		{
			return false;
		}
		if (m_UnitTemplet.m_listFixedStatusImmune.Contains(status))
		{
			return true;
		}
		if (m_UnitStateNow != null && m_UnitStateNow.m_listFixedStatusImmune.Contains(status))
		{
			return true;
		}
		NKMUnitStatusTemplet nKMUnitStatusTemplet = NKMUnitStatusTemplet.Find(status);
		if (nKMUnitStatusTemplet == null)
		{
			if (IsBoss())
			{
				return true;
			}
			if (GetUnitTemplet().m_UnitTempletBase.IsShip())
			{
				return true;
			}
		}
		else
		{
			if (!nKMUnitStatusTemplet.m_bAllowBoss && IsBoss())
			{
				return true;
			}
			if (!nKMUnitStatusTemplet.m_bAllowShip && GetUnitTemplet().m_UnitTempletBase.IsShip())
			{
				return true;
			}
			if (nKMUnitStatusTemplet.m_bDebuff && HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_NEGATIVE_STATUS))
			{
				return true;
			}
			if (nKMUnitStatusTemplet.m_bCrowdControl)
			{
				if ((m_UnitStateNow != null && m_UnitStateNow.m_bInvincibleState) || GetUnitFrameData().m_bInvincible)
				{
					return true;
				}
				if (HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_CC))
				{
					return true;
				}
			}
		}
		if (status == NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP && GetUnitTemplet().m_UnitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_MECHANIC))
		{
			return true;
		}
		return m_UnitFrameData.m_hsImmuneStatus.Contains(status);
	}

	public bool HasStatus(NKM_UNIT_STATUS_EFFECT status)
	{
		if (status == NKM_UNIT_STATUS_EFFECT.NUSE_NONE)
		{
			return false;
		}
		if (m_UnitStateNow != null)
		{
			if (m_UnitStateNow.m_listFixedStatusImmune.Contains(status))
			{
				return false;
			}
			if (m_UnitStateNow.m_listFixedStatusEffect.Contains(status))
			{
				return true;
			}
		}
		if (m_UnitTemplet.m_listFixedStatusImmune.Contains(status))
		{
			return false;
		}
		if (m_UnitTemplet.m_listFixedStatusEffect.Contains(status))
		{
			return true;
		}
		if (GetUnitFrameData().m_hsImmuneStatus.Contains(status))
		{
			return false;
		}
		return GetUnitFrameData().m_hsStatus.Contains(status);
	}

	public void ApplyStatus(NKM_UNIT_STATUS_EFFECT status)
	{
		if (CanApplyStatus(status))
		{
			GetUnitFrameData().m_hsStatus.Add(status);
		}
	}

	public void ApplyStatus(HashSet<NKM_UNIT_STATUS_EFFECT> hsStatus)
	{
		if (hsStatus.Count == 0)
		{
			return;
		}
		foreach (NKM_UNIT_STATUS_EFFECT item in hsStatus)
		{
			ApplyStatus(item);
		}
	}

	public void RemoveStatus(NKM_UNIT_STATUS_EFFECT status)
	{
		GetUnitFrameData().m_hsStatus.Remove(status);
		bool num = GetUnitFrameData().m_dicStatusTime.Remove(status);
		DeleteStatusBuff(status, bForceRemove: true, bAffectOnly: true);
		if (num && m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			m_UnitSyncData.m_listStatusTimeData.Add(new NKMUnitStatusTimeSyncData(status, 0f));
			m_bPushSimpleSyncData = true;
		}
	}

	public void ApplyImmuneStatus(NKM_UNIT_STATUS_EFFECT status)
	{
		GetUnitFrameData().m_hsImmuneStatus.Add(status);
		RemoveStatus(status);
	}

	public void ApplyImmuneStatus(HashSet<NKM_UNIT_STATUS_EFFECT> hsStatus)
	{
		if (hsStatus.Count == 0)
		{
			return;
		}
		foreach (NKM_UNIT_STATUS_EFFECT item in hsStatus)
		{
			ApplyImmuneStatus(item);
		}
	}

	public void ApplyStatusTime(NKM_UNIT_STATUS_EFFECT type, float time, NKMUnit applierUnit, bool bForceOverwrite = false, bool bServerOnly = false, bool bImmediate = false)
	{
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			Log.Error("Client Apply status time!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnit.cs", 13163);
		}
		else
		{
			if (IsImmuneStatus(type))
			{
				return;
			}
			string badStatusStateChange = GetBadStatusStateChange(type);
			if (!string.IsNullOrEmpty(badStatusStateChange))
			{
				StateChange(badStatusStateChange);
			}
			else
			{
				if (!OnApplyStatus(type, applierUnit))
				{
					return;
				}
				if (m_UnitFrameData.m_dicStatusTime.TryGetValue(type, out var value))
				{
					if (!bForceOverwrite && time <= value)
					{
						return;
					}
					m_UnitFrameData.m_dicStatusTime[type] = time;
				}
				else
				{
					m_UnitFrameData.m_dicStatusTime.Add(type, time);
				}
				if (bImmediate)
				{
					ApplyStatus(type);
				}
				if (!bServerOnly)
				{
					m_UnitSyncData.m_listStatusTimeData.Add(new NKMUnitStatusTimeSyncData(type, time));
					m_bPushSimpleSyncData = true;
				}
			}
		}
	}

	public int DispelStatusTime(bool bDebuff, int count = 999)
	{
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			return 0;
		}
		if (count <= 0)
		{
			return 0;
		}
		m_lstTempStatus.Clear();
		int num = 0;
		foreach (NKM_UNIT_STATUS_EFFECT key in m_UnitFrameData.m_dicStatusTime.Keys)
		{
			if (count - num <= 0)
			{
				break;
			}
			NKMUnitStatusTemplet nKMUnitStatusTemplet = NKMUnitStatusTemplet.Find(key);
			if (nKMUnitStatusTemplet.m_bDispel && nKMUnitStatusTemplet.m_bDebuff == bDebuff)
			{
				m_lstTempStatus.Add(key);
				num++;
			}
		}
		if (num > 0)
		{
			foreach (NKM_UNIT_STATUS_EFFECT item in m_lstTempStatus)
			{
				m_UnitFrameData.m_dicStatusTime[item] = 0f;
				m_UnitSyncData.m_listStatusTimeData.Add(new NKMUnitStatusTimeSyncData(item, 0f));
			}
			m_bPushSimpleSyncData = true;
		}
		return num;
	}

	private bool OnApplyStatus(HashSet<NKM_UNIT_STATUS_EFFECT> hsType, NKMUnit applierUnit)
	{
		bool flag = true;
		foreach (NKM_UNIT_STATUS_EFFECT item in hsType)
		{
			flag &= OnApplyStatus(item, applierUnit);
		}
		return flag;
	}

	private bool OnApplyStatus(NKM_UNIT_STATUS_EFFECT type, NKMUnit applierUnit)
	{
		switch (type)
		{
		case NKM_UNIT_STATUS_EFFECT.NUSE_CONFUSE:
			if (applierUnit == null)
			{
				return false;
			}
			if (IsImmuneStatus(NKM_UNIT_STATUS_EFFECT.NUSE_CONFUSE))
			{
				return false;
			}
			if (IsBoss())
			{
				return false;
			}
			if (m_UnitDataGame.m_NKM_TEAM_TYPE != m_UnitDataGame.m_NKM_TEAM_TYPE_ORG && m_NKMGame.IsSameTeam(m_UnitDataGame.m_NKM_TEAM_TYPE_ORG, applierUnit.GetTeam()))
			{
				RemoveStatus(NKM_UNIT_STATUS_EFFECT.NUSE_CONFUSE);
				m_UnitDataGame.m_NKM_TEAM_TYPE = m_UnitDataGame.m_NKM_TEAM_TYPE_ORG;
				return false;
			}
			m_UnitDataGame.m_NKM_TEAM_TYPE = applierUnit.GetTeam();
			break;
		case NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_CC:
			RemoveAllCrowdControlStatus();
			break;
		case NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE:
			RemoveStatus(NKM_UNIT_STATUS_EFFECT.NUSE_STUN);
			RemoveStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP);
			break;
		}
		if (NKMUnitStatusTemplet.IsCrowdControlStatus(type))
		{
			OnReactionEvent(NKMUnitReaction.ReactionEventType.TAKE_CC, this, 1);
			BroadcastReactionEvent(NKMUnitReaction.ReactionEventType.UNIT_TAKE_CC, this, 1);
		}
		return true;
	}

	public string GetBadStatusStateChange(HashSet<NKM_UNIT_STATUS_EFFECT> hsStateType)
	{
		foreach (NKM_UNIT_STATUS_EFFECT item in hsStateType)
		{
			string badStatusStateChange = GetBadStatusStateChange(item);
			if (badStatusStateChange != null)
			{
				return badStatusStateChange;
			}
		}
		return null;
	}

	public string GetBadStatusStateChange(NKM_UNIT_STATUS_EFFECT stateType)
	{
		string text;
		switch (stateType)
		{
		case NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE:
			text = GetUnitTemplet().m_StateChangeSilence;
			break;
		case NKM_UNIT_STATUS_EFFECT.NUSE_STUN:
			text = GetUnitTemplet().m_StateChangeStun;
			break;
		case NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP:
			text = GetUnitTemplet().m_StateChangeSleep;
			break;
		case NKM_UNIT_STATUS_EFFECT.NUSE_CONFUSE:
			text = GetUnitTemplet().m_StateChangeConfuse;
			break;
		default:
			return null;
		}
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return null;
	}

	public bool HasCrowdControlStatus(bool bExculdeConfuse = false)
	{
		foreach (NKM_UNIT_STATUS_EFFECT item in GetUnitFrameData().m_hsStatus)
		{
			if (!(item == NKM_UNIT_STATUS_EFFECT.NUSE_CONFUSE && bExculdeConfuse) && NKMUnitStatusTemplet.IsCrowdControlStatus(item))
			{
				return true;
			}
		}
		return false;
	}

	public void RemoveAllCrowdControlStatus()
	{
		m_lstTempStatus.Clear();
		foreach (NKM_UNIT_STATUS_EFFECT item in GetUnitFrameData().m_hsStatus)
		{
			if (NKMUnitStatusTemplet.IsCrowdControlStatus(item))
			{
				m_lstTempStatus.Add(item);
			}
		}
		if (m_lstTempStatus.Count <= 0)
		{
			return;
		}
		foreach (NKM_UNIT_STATUS_EFFECT item2 in m_lstTempStatus)
		{
			RemoveStatus(item2);
		}
	}

	public bool RunAllowed()
	{
		if (m_UnitTemplet.m_bNoMove || m_UnitTemplet.m_bNoRun || HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NOMOVE))
		{
			return false;
		}
		return true;
	}

	public NKMUnit GetTriggerTargetUnit(bool bUseTriggerTargetUnit)
	{
		if (!bUseTriggerTargetUnit || CurrentTriggerTarget == null)
		{
			return this;
		}
		return CurrentTriggerTarget;
	}

	public virtual void InvokeTrigger(NKMUnit triggerMaster, int triggerID)
	{
	}

	public virtual void InvokeTrigger(string name)
	{
	}

	public virtual void InvokeTrigger(int id)
	{
	}

	protected void RegisterInvokedTrigger(int triggerID, NKMUnit ownerUnit)
	{
		if (ownerUnit == null)
		{
			return;
		}
		NKMUnitTriggerSet triggerSet = ownerUnit.GetUnitTemplet().GetTriggerSet(triggerID);
		if (triggerSet == null)
		{
			return;
		}
		short ownerUID = ownerUnit.GetUnitDataGame().m_GameUnitUID;
		NKMInvokedTrigger nKMInvokedTrigger = m_lstInvokedTrigger.Find((NKMInvokedTrigger x) => x.m_TriggerOwnerGameUnitUID == ownerUID && x.m_TriggerID == triggerID && !x.m_bFinished);
		if (nKMInvokedTrigger != null)
		{
			switch (triggerSet.m_DuplicateMode)
			{
			case NKMUnitTriggerSet.DuplicateInvokeMode.IGNORE:
				return;
			case NKMUnitTriggerSet.DuplicateInvokeMode.RESTART:
				nKMInvokedTrigger.Restart();
				return;
			}
		}
		NKMInvokedTrigger nKMInvokedTrigger2 = m_lstInvokedTrigger.Find((NKMInvokedTrigger x) => x.m_bFinished);
		if (nKMInvokedTrigger2 == null)
		{
			m_lstInvokedTrigger.Add(new NKMInvokedTrigger(triggerID, ownerUID));
		}
		else
		{
			nKMInvokedTrigger2.SetNewTrigger(triggerID, ownerUID);
		}
	}

	protected void ProcessInvokedTrigger(bool bStateEnd)
	{
		if (IsDie())
		{
			return;
		}
		if (bStateEnd)
		{
			foreach (NKMInvokedTrigger item in m_lstInvokedTrigger)
			{
				if (!item.m_bFinished && item.m_TriggerOwnerGameUnitUID == m_UnitDataGame.m_GameUnitUID)
				{
					NKMUnitTriggerSet triggerSet = GetUnitTemplet().GetTriggerSet(item.m_TriggerID);
					if (triggerSet.m_bProcessWhileDying || !IsDyingOrDie())
					{
						bool flag = false;
						if (item.m_LastTime < 0f)
						{
							item.m_LastTime = 0f;
							flag = InvokeTimedTrigger(triggerSet, -1f, 0f);
						}
						if (flag || triggerSet.m_bStopOnStateChange)
						{
							item.m_bFinished = true;
							InvokeEndTriggerEvents(triggerSet);
						}
					}
				}
			}
			return;
		}
		foreach (NKMInvokedTrigger item2 in m_lstInvokedTrigger)
		{
			if (item2.m_bFinished)
			{
				continue;
			}
			NKMUnit unit = m_NKMGame.GetUnit(item2.m_TriggerOwnerGameUnitUID, bChain: true, bPool: true);
			NKMUnitTriggerSet triggerSet2 = unit.GetUnitTemplet().GetTriggerSet(item2.m_TriggerID);
			float lastTime = item2.m_LastTime;
			if (triggerSet2.m_bProcessWhileDying || !IsDyingOrDie())
			{
				float timeNow = (item2.m_LastTime = ((item2.m_LastTime < 0f) ? 0f : ((!triggerSet2.m_bAnimTime) ? (lastTime + m_DeltaTime) : (lastTime + m_DeltaTime * unit.m_UnitFrameData.m_fAnimSpeed))));
				unit.CurrentTriggerTarget = this;
				if (unit.InvokeTimedTrigger(triggerSet2, lastTime, timeNow))
				{
					item2.m_bFinished = true;
					unit.InvokeEndTriggerEvents(triggerSet2);
				}
				unit.CurrentTriggerTarget = null;
			}
		}
	}

	protected virtual bool InvokeTimedTrigger(NKMUnitTriggerSet triggerSet, float timeBefore, float timeNow)
	{
		return triggerSet.InvokeTimedTrigger(timeBefore, timeNow, m_NKMGame, this);
	}

	protected virtual void InvokeEndTriggerEvents(NKMUnitTriggerSet triggerSet)
	{
		triggerSet.InvokeEndTriggerEvent(m_NKMGame, this);
	}

	private void InitTriggerRepeat()
	{
		if (m_UnitTemplet.m_listTriggerRepeatData != null)
		{
			m_lstTriggerRepeatRuntime = new List<NKMTriggerRepeatRuntime>(m_UnitTemplet.m_listTriggerRepeatData.Count);
			{
				foreach (NKMTriggerRepeatData listTriggerRepeatDatum in m_UnitTemplet.m_listTriggerRepeatData)
				{
					NKMTriggerRepeatRuntime nKMTriggerRepeatRuntime = new NKMTriggerRepeatRuntime();
					nKMTriggerRepeatRuntime.m_NKMTriggerRepeatData = listTriggerRepeatDatum;
					nKMTriggerRepeatRuntime.m_fRepeatTimeNow = listTriggerRepeatDatum.m_fRepeatTime;
					m_lstTriggerRepeatRuntime.Add(nKMTriggerRepeatRuntime);
				}
				return;
			}
		}
		m_lstTriggerRepeatRuntime = null;
	}

	private void ProcessTriggerRepeat()
	{
		if (m_lstTriggerRepeatRuntime == null)
		{
			return;
		}
		foreach (NKMTriggerRepeatRuntime item in m_lstTriggerRepeatRuntime)
		{
			if (item == null)
			{
				continue;
			}
			item.m_fRepeatTimeNow -= m_DeltaTime;
			if (item.m_fRepeatTimeNow <= 0f)
			{
				if (CheckEventCondition(item.m_NKMTriggerRepeatData.Condition))
				{
					InvokeTrigger(item.m_NKMTriggerRepeatData.m_TriggerName);
				}
				item.m_fRepeatTimeNow = item.m_NKMTriggerRepeatData.m_fRepeatTime;
			}
		}
	}

	public int GetEventVariable(string name)
	{
		if (m_UnitSyncData.m_dicEventVariables.TryGetValue(name, out var value))
		{
			return value;
		}
		return 0;
	}

	public void AddEventVariable(string name, int value, bool isVolatile = false)
	{
		if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			if (m_UnitSyncData.m_dicEventVariables.TryGetValue(name, out var value2))
			{
				m_UnitSyncData.m_dicEventVariables[name] = value2 + value;
			}
			else
			{
				m_UnitSyncData.m_dicEventVariables[name] = value;
			}
			if (isVolatile)
			{
				m_hsVolatileEventVariables.Add(name);
			}
			m_bPushSimpleSyncData = true;
		}
	}

	public void SetEventVariable(string name, int value, bool isVolatile = false)
	{
		if (m_NKM_UNIT_CLASS_TYPE == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			m_UnitSyncData.m_dicEventVariables[name] = value;
			if (isVolatile)
			{
				m_hsVolatileEventVariables.Add(name);
			}
			m_bPushSimpleSyncData = true;
		}
	}

	public void OnReactionEvent(NKMUnitReaction.ReactionEventType eventType, NKMUnit invoker, int count = 1, params float[] param)
	{
		if (m_NKM_UNIT_CLASS_TYPE != NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER || !invoker.WillInteractWithGameUnits())
		{
			return;
		}
		foreach (ReactionInstance item in m_lstReactionEventInstance)
		{
			if (item.m_bFinished || item.EventType != eventType)
			{
				continue;
			}
			if (item.Reaction.m_ReactionCondition != null)
			{
				NKMUnit unit = m_NKMGame.GetUnit(item.masterUnitID, bChain: true, bPool: true);
				if (!invoker.CheckEventCondition(item.Reaction.m_ReactionCondition, unit))
				{
					continue;
				}
			}
			if (item.CheckParam(param))
			{
				item.m_currentCount += count;
				item.lastInvoker = invoker;
				if (item.Reaction.m_bImmediate && item.IsInvokeCount())
				{
					InvokeReactionTrigger(item);
				}
				RegisterSyncReaction(item);
			}
		}
	}

	public virtual void BroadcastReactionEvent(NKMUnitReaction.ReactionEventType eventType, NKMUnit invoker, int count = 1, params float[] param)
	{
	}

	public ReactionInstance GetReactionInstance(short ownerUnitUID, int ID)
	{
		return m_lstReactionEventInstance.Find((ReactionInstance x) => !x.m_bFinished && x.masterUnitID == ownerUnitUID && x.ID == ID);
	}

	public void RegisterReaction(NKMUnit ownerUnit, string reaction, float time)
	{
		int reactionID = ownerUnit.GetUnitTemplet().GetReactionID(reaction);
		RegisterReaction(ownerUnit, reactionID, time);
	}

	public void RegisterReaction(NKMUnit ownerUnit, int reactionID, float time)
	{
		if (ownerUnit.GetUnitTemplet().GetReaction(reactionID) == null)
		{
			return;
		}
		ReactionInstance reactionInstance = GetReactionInstance(ownerUnit.GetUnitGameUID(), reactionID);
		if (reactionInstance != null)
		{
			if (time > reactionInstance.m_fTimeLeft)
			{
				reactionInstance.m_fTimeLeft = time;
			}
			return;
		}
		ReactionInstance reactionInstance2 = m_lstReactionEventInstance.Find((ReactionInstance x) => x.m_bFinished);
		if (reactionInstance2 != null)
		{
			reactionInstance2.SetNewReaction(ownerUnit, reactionID, time);
			RegisterSyncReaction(reactionInstance2);
		}
		else
		{
			ReactionInstance reactionInstance3 = new ReactionInstance(ownerUnit, reactionID, time);
			m_lstReactionEventInstance.Add(reactionInstance3);
			RegisterSyncReaction(reactionInstance3);
		}
	}

	public void RemoveReaction(NKMUnit ownerUnit, int reactionID)
	{
		ReactionInstance reactionInstance = GetReactionInstance(ownerUnit.GetUnitGameUID(), reactionID);
		if (reactionInstance != null)
		{
			reactionInstance.m_bFinished = true;
			RegisterSyncReaction(reactionInstance);
		}
	}

	private void RegisterSyncReaction(ReactionInstance instance)
	{
		int num = GetUnitSyncData().m_listUpdatedReaction.FindIndex((NKMUnitSyncData.ReactionSync x) => x.masterUnitID == instance.masterUnitID && x.ID == instance.ID);
		if (num < 0)
		{
			GetUnitSyncData().m_listUpdatedReaction.Add(new NKMUnitSyncData.ReactionSync(instance));
		}
		else
		{
			GetUnitSyncData().m_listUpdatedReaction[num].SetData(instance);
		}
		m_bPushSimpleSyncData = true;
	}

	private void ProcessInvokedEventReaction()
	{
		if (IsDyingOrDie())
		{
			return;
		}
		foreach (ReactionInstance item in m_lstReactionEventInstance)
		{
			if (item.m_bFinished)
			{
				continue;
			}
			if (item.IsInvokeCount())
			{
				InvokeReactionTrigger(item);
			}
			if (item.m_fTimeLeft >= 0f)
			{
				item.m_fTimeLeft -= m_DeltaTime;
				if (item.m_fTimeLeft < 0f)
				{
					item.m_fTimeLeft = 0f;
					item.m_bFinished = true;
				}
			}
		}
	}

	private void InvokeReactionTrigger(ReactionInstance invokedReaction)
	{
		NKMUnit unit = m_NKMGame.GetUnit(invokedReaction.masterUnitID, bChain: true, bPool: true);
		if (unit != null)
		{
			NKMUnit nKMUnit = null;
			nKMUnit = invokedReaction.Reaction.m_TriggerTarget switch
			{
				NKMUnitReaction.TargetType.MASTER_TARGET => unit.m_TargetUnit, 
				NKMUnitReaction.TargetType.MASTER_SUB_TARGET => unit.m_SubTargetUnit, 
				NKMUnitReaction.TargetType.REACTION_UNIT => this, 
				NKMUnitReaction.TargetType.REACTION_UNIT_TARGET => m_TargetUnit, 
				NKMUnitReaction.TargetType.REACTION_UNIT_SUB_TARGET => m_SubTargetUnit, 
				NKMUnitReaction.TargetType.LAST_INVOKER => invokedReaction.lastInvoker, 
				_ => unit, 
			};
			if (nKMUnit != null)
			{
				invokedReaction.m_currentCount = 0;
				int triggerID = unit.GetUnitTemplet().GetTriggerID(invokedReaction.Reaction.m_Trigger);
				nKMUnit.InvokeTrigger(unit, triggerID);
			}
		}
	}
}
