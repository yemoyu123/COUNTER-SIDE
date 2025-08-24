using NKM.Unit;

namespace NKM;

public class NKMEventMove : NKMUnitEffectStateEventOneTime, INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public bool m_bSavePosition;

	public NKMEventPosData m_EventPosData;

	public float m_MoveTime;

	public float m_fSpeed;

	public TRACKING_DATA_TYPE m_MoveTrackingType = TRACKING_DATA_TYPE.TDT_SLOWER;

	public bool m_bLandMove;

	public float m_fMaxDistance = -1f;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Both;

	public void DeepCopyFromSource(NKMEventMove source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTime = source.m_fEventTime;
		m_bStateEndTime = source.m_bStateEndTime;
		m_EventPosData.DeepCopy(source.m_EventPosData);
		m_MoveTime = source.m_MoveTime;
		m_fSpeed = source.m_fSpeed;
		m_MoveTrackingType = source.m_MoveTrackingType;
		m_bLandMove = source.m_bLandMove;
		m_fMaxDistance = source.m_fMaxDistance;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		bool rbValue = false;
		bool rbValue2 = false;
		bool rbValue3 = false;
		cNKMLua.GetData("m_bTargetUnit", ref rbValue);
		cNKMLua.GetData("m_bDependMyShip", ref rbValue2);
		cNKMLua.GetData("m_bOffsetDependMe", ref rbValue3);
		if (rbValue)
		{
			m_EventPosData.m_MoveBase = NKMEventPosData.MoveBase.TARGET_UNIT;
			if (rbValue3)
			{
				m_EventPosData.m_MoveOffset = NKMEventPosData.MoveOffset.ME_INV;
			}
		}
		else
		{
			m_EventPosData.m_MoveBase = NKMEventPosData.MoveBase.ME;
		}
		if (rbValue2)
		{
			m_EventPosData.m_MoveOffset = NKMEventPosData.MoveOffset.MY_SHIP_INV;
		}
		if (cNKMLua.GetData("m_fMapPosFactor", ref m_EventPosData.m_fMapPosFactor) && m_EventPosData.m_fMapPosFactor >= 0f)
		{
			m_EventPosData.m_MoveBase = NKMEventPosData.MoveBase.MAP_RATE;
		}
		cNKMLua.GetData("m_OffsetX", ref m_EventPosData.m_fOffsetX);
		cNKMLua.GetData("m_OffsetJumpYPos", ref m_EventPosData.m_fOffsetY);
		m_EventPosData.LoadFromLua(cNKMLua);
		cNKMLua.GetData("m_MoveTime", ref m_MoveTime);
		cNKMLua.GetData("m_fSpeed", ref m_fSpeed);
		cNKMLua.GetData("m_MoveTrackingType", ref m_MoveTrackingType);
		cNKMLua.GetData("m_bLandMove", ref m_bLandMove);
		cNKMLua.GetData("m_bSavePosition", ref m_bSavePosition);
		cNKMLua.GetData("m_fMaxDistance", ref m_fMaxDistance);
		return true;
	}

	public override void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime)
	{
		if (m_bAnimTime && cNKMUnit.GetUnitFrameData().m_fAnimSpeed == 0f)
		{
			return;
		}
		cNKMUnit.m_EventMovePosX.StopTracking();
		cNKMUnit.m_EventMovePosJumpY.StopTracking();
		cNKMUnit.m_EventMovePosZ.StopTracking();
		float eventPosX = cNKMUnit.GetEventPosX(m_EventPosData, cNKMUnit.IsATeam());
		float eventPosY = cNKMUnit.GetEventPosY(m_EventPosData);
		float num = ((!m_bAnimTime) ? m_fEventTime : (m_fEventTime / cNKMUnit.GetUnitFrameData().m_fAnimSpeed));
		float num2 = m_fEventTime + m_MoveTime;
		if (m_MoveTime == 0f || rollbackTime >= num2)
		{
			cNKMUnit.GetUnitFrameData().m_PosXCalc = eventPosX;
			if (!m_bLandMove)
			{
				cNKMUnit.GetUnitFrameData().m_JumpYPosCalc = eventPosY;
			}
			return;
		}
		float deltaTime = rollbackTime - num;
		cNKMUnit.m_EventMovePosX.SetNowValue(cNKMUnit.GetUnitFrameData().m_PosXCalc);
		cNKMUnit.m_EventMovePosX.SetTracking(eventPosX, m_MoveTime, m_MoveTrackingType);
		cNKMUnit.m_EventMovePosX.Update(deltaTime);
		if (!m_bLandMove)
		{
			cNKMUnit.m_EventMovePosJumpY.SetNowValue(cNKMUnit.GetUnitFrameData().m_JumpYPosCalc);
			cNKMUnit.m_EventMovePosJumpY.SetTracking(eventPosY, m_MoveTime, m_MoveTrackingType);
			cNKMUnit.m_EventMovePosJumpY.Update(deltaTime);
		}
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.ApplyEventMove(this);
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMDamageEffect cNKMDamageEffect)
	{
		cNKMDamageEffect.ApplyEventMove(this);
	}
}
