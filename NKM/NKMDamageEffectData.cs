namespace NKM;

public class NKMDamageEffectData
{
	public short m_MasterGameUnitUID;

	public short m_TargetGameUnitUID;

	public NKM_TEAM_TYPE m_NKM_TEAM_TYPE;

	public NKMUnit m_MasterUnit;

	public NKMStatData m_StatData;

	public NKMUnitData m_UnitData;

	public bool m_bDie;

	public float m_fFindTargetTimeNow;

	public bool m_bStateEndStop;

	public int m_DamageCountNow;

	public float m_PosXBefore;

	public float m_PosZBefore;

	public float m_JumpYPosBefore;

	public float m_PosX;

	public float m_PosZ;

	public float m_JumpYPos;

	public float m_fLastTargetPosX;

	public float m_fLastTargetPosZ;

	public float m_fLastTargetPosJumpY;

	public float m_fLastFollowPosX;

	public float m_fLastFollowPosZ;

	public float m_fLastFollowPosJumpY;

	public NKMEventPosData.MoveOffset m_MoveOffset;

	public float m_fOffsetX;

	public float m_fOffsetY;

	public float m_fOffsetZ;

	public float m_fMapPosFactor;

	public float m_fRotate;

	public float m_fAddRotate;

	public bool m_bUseZScale;

	public float m_fSpeedFactorX;

	public float m_fSpeedFactorY;

	public NKMTrackingFloat m_TargetDirSpeed = new NKMTrackingFloat();

	public float m_fDirVectorMagniture;

	public float m_fEventDirSpeed = -1f;

	public float m_SpeedX;

	public float m_SpeedY;

	public float m_SpeedZ;

	public NKMVector3 m_DirVector;

	public NKMTrackingFloat m_DirVectorTrackX = new NKMTrackingFloat();

	public NKMTrackingFloat m_DirVectorTrackY = new NKMTrackingFloat();

	public NKMTrackingFloat m_DirVectorTrackZ = new NKMTrackingFloat();

	public bool m_bRight;

	public float m_fSeeTargetTimeNow;

	public float m_fLifeTimeMax;

	public bool m_bStateFirstFrame;

	public float m_fStateTimeBack;

	public float m_fStateTime;

	public float m_fAnimTimeBack;

	public float m_fAnimTime;

	public float m_fAnimTimeMax;

	public float m_AnimPlayCount;

	public bool m_bFootOnLand;

	public float m_fFollowTime;

	public TRACKING_DATA_TYPE m_FollowTrackingDataType = TRACKING_DATA_TYPE.TDT_NORMAL;

	public float m_fFollowUpdateTime;

	public float m_fFollowResetTime;

	public NKMDamageEffectData()
	{
		Init();
	}

	public void Init()
	{
		m_MasterGameUnitUID = 0;
		m_TargetGameUnitUID = 0;
		m_NKM_TEAM_TYPE = NKM_TEAM_TYPE.NTT_INVALID;
		m_MasterUnit = null;
		m_StatData = null;
		m_UnitData = null;
		m_bDie = false;
		m_fFindTargetTimeNow = 0f;
		m_bStateEndStop = false;
		m_DamageCountNow = 0;
		m_PosXBefore = -1f;
		m_PosZBefore = -1f;
		m_JumpYPosBefore = -1f;
		m_PosX = 0f;
		m_PosZ = 0f;
		m_JumpYPos = 0f;
		m_fLastTargetPosX = 0f;
		m_fLastTargetPosZ = 0f;
		m_fLastTargetPosJumpY = 0f;
		m_fOffsetX = 0f;
		m_fOffsetY = 0f;
		m_fOffsetZ = 0f;
		m_fAddRotate = 0f;
		m_bUseZScale = true;
		m_fSpeedFactorX = 0f;
		m_fSpeedFactorY = 0f;
		m_TargetDirSpeed.SetNowValue(m_DirVector.x);
		m_fDirVectorMagniture = 0f;
		m_fEventDirSpeed = 0f;
		m_SpeedX = 0f;
		m_SpeedY = 0f;
		m_SpeedZ = 0f;
		m_DirVector.Set();
		m_DirVectorTrackX.SetNowValue(m_DirVector.x);
		m_DirVectorTrackY.SetNowValue(m_DirVector.y);
		m_DirVectorTrackZ.SetNowValue(m_DirVector.z);
		m_bRight = true;
		m_fSeeTargetTimeNow = 0f;
		m_fLifeTimeMax = 0f;
		m_bStateFirstFrame = true;
		m_fStateTimeBack = 0f;
		m_fStateTime = 0f;
		m_fAnimTimeBack = 0f;
		m_fAnimTime = 0f;
		m_fAnimTimeMax = 0f;
		m_AnimPlayCount = 0f;
		m_bFootOnLand = false;
	}

	public int GetMasterSkinID()
	{
		if (m_MasterUnit == null)
		{
			return 0;
		}
		if (m_MasterUnit.GetUnitData() == null)
		{
			return 0;
		}
		return m_MasterUnit.GetUnitData().m_SkinID;
	}
}
