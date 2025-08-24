namespace NKM;

public struct NKMEventPosData
{
	public enum MoveBase
	{
		ME,
		TARGET_UNIT,
		SUB_TARGET_UNIT,
		MASTER_UNIT,
		TRIGGER_TARGET_UNIT,
		MAP_RATE,
		MY_SHIP,
		ENEMY_SHIP,
		SAVE_ONLY,
		SAVED_POS,
		SHIP_SKILL_POS,
		ATTACKER_POS,
		EVENT_RESPAWN_POS
	}

	public enum MoveOffset
	{
		MY_LOOK_DIR,
		ME,
		ME_INV,
		TARGET_UNIT,
		TARGET_UNIT_INV,
		TARGET_UNIT_LOOK_DIR,
		SUB_TARGET_UNIT,
		SUB_TARGET_UNIT_LOOK_DIR,
		MASTER_UNIT,
		MASTER_UNIT_LOOK_DIR,
		TRIGGER_TARGET_UNIT,
		TRIGGER_TARGET_UNIT_INV,
		TRIGGER_TARGET_UNIT_LOOK_DIR,
		MY_SHIP,
		MY_SHIP_INV,
		ENEMY_SHIP,
		ENEMY_SHIP_INV,
		TEAM_DIR,
		MAP_RATE,
		SAVED_POS,
		ATTACKER_POS,
		ATTACKER_LOOK_DIR
	}

	public enum EventPosExtraUnitType
	{
		ATTACKER,
		SUMMON_INVOKER
	}

	public enum MoveBaseType : short
	{
		CENTER,
		FRONT,
		BACK,
		NEAR,
		FAR
	}

	public MoveBase m_MoveBase;

	public MoveOffset m_MoveOffset;

	public MoveBaseType m_MoveBaseType;

	public float m_fMapPosFactor;

	public float m_fOffsetX;

	public float m_fOffsetY;

	public float m_fOffsetZ;

	public float m_fDefaultOffsetX;

	public void Init()
	{
		m_MoveBase = MoveBase.ME;
		m_MoveOffset = MoveOffset.MY_LOOK_DIR;
		m_MoveBaseType = MoveBaseType.CENTER;
		m_fMapPosFactor = 0f;
		m_fOffsetX = 0f;
		m_fOffsetY = 0f;
		m_fOffsetZ = 0f;
		m_fDefaultOffsetX = 0f;
	}

	public void DeepCopy(NKMEventPosData source)
	{
		m_MoveBase = source.m_MoveBase;
		m_MoveOffset = source.m_MoveOffset;
		m_MoveBaseType = source.m_MoveBaseType;
		m_fMapPosFactor = source.m_fMapPosFactor;
		m_fOffsetX = source.m_fOffsetX;
		m_fOffsetY = source.m_fOffsetY;
		m_fOffsetZ = source.m_fOffsetZ;
	}

	public void LoadFromLua(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_MoveBase", ref m_MoveBase);
		cNKMLua.GetData("m_MoveOffset", ref m_MoveOffset);
		cNKMLua.GetData("m_MoveBaseType", ref m_MoveBaseType);
		cNKMLua.GetData("m_fMapPosFactor", ref m_fMapPosFactor);
		float rValue = 0f;
		if (cNKMLua.GetData("m_OffsetJumpYPos", ref rValue))
		{
			m_fOffsetY = rValue;
		}
		cNKMLua.GetData("m_fOffsetX", ref m_fOffsetX);
		cNKMLua.GetData("m_fOffsetY", ref m_fOffsetY);
		cNKMLua.GetData("m_fOffsetZ", ref m_fOffsetZ);
		cNKMLua.GetData("m_fDefaultOffsetX", ref m_fDefaultOffsetX);
	}
}
