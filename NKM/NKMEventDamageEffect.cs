using NKM.Unit;

namespace NKM;

public class NKMEventDamageEffect : NKMUnitEffectStateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public bool m_bIgnoreTarget;

	public bool m_bIgnoreNoTarget;

	public string m_DEName = "";

	public string m_DENamePVP = "";

	public NKMEventPosData m_EventPosData;

	public bool m_bUseMyZPos = true;

	public float m_fAddRotate;

	public bool m_bUseZScale = true;

	public float m_fSpeedFactorX;

	public float m_fSpeedFactorY;

	public float m_fReserveTime;

	public bool m_bStateEndStop;

	public bool m_bHold;

	public TRACKING_DATA_TYPE m_FollowType = TRACKING_DATA_TYPE.TDT_NORMAL;

	public float m_FollowTime;

	public float m_FollowUpdateTime;

	public bool m_bFlipRight;

	public bool m_bUseSubTarget;

	public NKM_SKILL_TYPE m_SkillType;

	public override EventRollbackType RollbackType => EventRollbackType.Warning;

	public override EventHostType HostType => EventHostType.Both;

	public void DeepCopyFromSource(NKMEventDamageEffect source)
	{
		DeepCopy(source);
		m_bIgnoreTarget = source.m_bIgnoreTarget;
		m_bIgnoreNoTarget = source.m_bIgnoreNoTarget;
		m_DEName = source.m_DEName;
		m_DENamePVP = source.m_DENamePVP;
		m_EventPosData.DeepCopy(source.m_EventPosData);
		m_bUseMyZPos = source.m_bUseMyZPos;
		m_fAddRotate = source.m_fAddRotate;
		m_bUseZScale = source.m_bUseZScale;
		m_fSpeedFactorX = source.m_fSpeedFactorX;
		m_fSpeedFactorY = source.m_fSpeedFactorY;
		m_fReserveTime = source.m_fReserveTime;
		m_bStateEndStop = source.m_bStateEndStop;
		m_bHold = source.m_bHold;
		m_FollowType = source.m_FollowType;
		m_FollowTime = source.m_FollowTime;
		m_FollowUpdateTime = source.m_FollowUpdateTime;
		m_bFlipRight = source.m_bFlipRight;
		m_SkillType = source.m_SkillType;
		m_bUseSubTarget = source.m_bUseSubTarget;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bIgnoreTarget", ref m_bIgnoreTarget);
		cNKMLua.GetData("m_bIgnoreNoTarget", ref m_bIgnoreNoTarget);
		cNKMLua.GetData("m_DEName", ref m_DEName);
		cNKMLua.GetData("m_DENamePVP", ref m_DENamePVP);
		cNKMLua.GetData("m_bUseSubTarget", ref m_bUseSubTarget);
		bool rbValue = false;
		bool rbValue2 = false;
		bool rbValue3 = false;
		if (cNKMLua.GetData("m_bTargetPos", ref rbValue) && rbValue)
		{
			m_EventPosData.m_MoveBase = NKMEventPosData.MoveBase.TARGET_UNIT;
		}
		if (cNKMLua.GetData("m_bShipSkillPos", ref rbValue2) && rbValue2)
		{
			m_EventPosData.m_MoveBase = NKMEventPosData.MoveBase.SHIP_SKILL_POS;
			m_bUseMyZPos = false;
		}
		if (cNKMLua.GetData("m_bUseMapPos", ref rbValue3) && rbValue3)
		{
			m_EventPosData.m_MoveBase = NKMEventPosData.MoveBase.MAP_RATE;
		}
		if (m_bUseSubTarget)
		{
			m_EventPosData.m_MoveBase = NKMEventPosData.MoveBase.SUB_TARGET_UNIT;
		}
		cNKMLua.GetData("m_OffsetX", ref m_EventPosData.m_fOffsetX);
		cNKMLua.GetData("m_OffsetY", ref m_EventPosData.m_fOffsetY);
		cNKMLua.GetData("m_OffsetZ", ref m_EventPosData.m_fOffsetZ);
		cNKMLua.GetData("m_fMapPosRate", ref m_EventPosData.m_fMapPosFactor);
		m_EventPosData.LoadFromLua(cNKMLua);
		cNKMLua.GetData("m_bUseMyZPos", ref m_bUseMyZPos);
		cNKMLua.GetData("m_fAddRotate", ref m_fAddRotate);
		cNKMLua.GetData("m_bUseZScale", ref m_bUseZScale);
		cNKMLua.GetData("m_fSpeedFactorX", ref m_fSpeedFactorX);
		cNKMLua.GetData("m_fSpeedFactorY", ref m_fSpeedFactorY);
		cNKMLua.GetData("m_fReserveTime", ref m_fReserveTime);
		cNKMLua.GetData("m_bStateEndStop", ref m_bStateEndStop);
		cNKMLua.GetData("m_bHold", ref m_bHold);
		cNKMLua.GetData("m_FollowType", ref m_FollowType);
		cNKMLua.GetData("m_FollowTime", ref m_FollowTime);
		cNKMLua.GetData("m_FollowUpdateTime", ref m_FollowUpdateTime);
		cNKMLua.GetData("m_SkillType", ref m_SkillType);
		cNKMLua.GetData("m_bFlipRight", ref m_bFlipRight);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		NKMUnitSkillTemplet stateSkill = cNKMUnit.GetStateSkill(cNKMUnit.GetUnitStateNow());
		cNKMUnit.ApplyEventDamageEffect(this, stateSkill, cNKMUnit.GetUnitSyncData().m_PosX, cNKMUnit.GetUnitSyncData().m_JumpYPos, cNKMUnit.GetUnitSyncData().m_PosZ);
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMDamageEffect cNKMDamageEffect)
	{
		cNKMDamageEffect.ApplyEventDamageEffect(this);
	}
}
