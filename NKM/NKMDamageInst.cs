using System.Collections.Generic;

namespace NKM;

public class NKMDamageInst : NKMObjectPoolData
{
	public NKMDamageTemplet m_Templet;

	public NKMEventAttack m_EventAttack;

	public bool m_bEvade;

	public NKM_REACT_TYPE m_ReActResult;

	public List<short> m_listHitUnit = new List<short>();

	public int m_AttackCount;

	public bool m_bReAttackCountOver;

	public int m_ReAttackCount;

	public float m_fReAttackGap;

	public NKM_TEAM_TYPE m_AttackerTeamType;

	public NKM_REACTOR_TYPE m_AttackerType;

	public short m_AttackerGameUnitUID;

	public short m_AttackerEffectUID;

	public short m_DefenderUID;

	public float m_AttackerPosX;

	public float m_AttackerPosZ;

	public float m_AttackerPosJumpY;

	public bool m_bAttackerRight;

	public bool m_bAttackerAwaken;

	public byte m_AttackerAddAttackUnitCount;

	public byte m_AtkBuffCount;

	public byte m_DefBuffCount;

	public NKMUnitSkillTemplet m_AttackerUnitSkillTemplet;

	public NKMDamageInst()
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKMDamageInst;
		Init();
	}

	public void Init()
	{
		m_Templet = null;
		m_bEvade = false;
		m_ReActResult = NKM_REACT_TYPE.NRT_INVALID;
		m_EventAttack = null;
		m_listHitUnit.Clear();
		m_AttackCount = 0;
		m_bReAttackCountOver = false;
		m_ReAttackCount = 0;
		m_fReAttackGap = 0f;
		m_AttackerTeamType = NKM_TEAM_TYPE.NTT_INVALID;
		m_AttackerType = NKM_REACTOR_TYPE.NRT_INVALID;
		m_AttackerGameUnitUID = 0;
		m_AttackerEffectUID = 0;
		m_DefenderUID = 0;
		m_AttackerPosX = 0f;
		m_AttackerPosZ = 0f;
		m_AttackerPosJumpY = 0f;
		m_bAttackerRight = false;
		m_bAttackerAwaken = false;
		m_AttackerAddAttackUnitCount = 0;
		m_AttackerUnitSkillTemplet = null;
		m_AtkBuffCount = 0;
		m_DefBuffCount = 0;
	}

	public void DeepCopyFromSource(NKMDamageInst source)
	{
		m_Templet = source.m_Templet;
		m_bEvade = source.m_bEvade;
		m_ReActResult = source.m_ReActResult;
		m_EventAttack = source.m_EventAttack;
		m_listHitUnit.Clear();
		for (int i = 0; i < source.m_listHitUnit.Count; i++)
		{
			m_listHitUnit.Add(source.m_listHitUnit[i]);
		}
		m_AttackCount = source.m_AttackCount;
		m_bReAttackCountOver = source.m_bReAttackCountOver;
		m_ReAttackCount = source.m_ReAttackCount;
		m_fReAttackGap = source.m_fReAttackGap;
		m_AttackerTeamType = source.m_AttackerTeamType;
		m_AttackerType = source.m_AttackerType;
		m_AttackerGameUnitUID = source.m_AttackerGameUnitUID;
		m_AttackerEffectUID = source.m_AttackerEffectUID;
		m_DefenderUID = source.m_DefenderUID;
		m_AttackerPosX = source.m_AttackerPosX;
		m_AttackerPosZ = source.m_AttackerPosZ;
		m_AttackerPosJumpY = source.m_AttackerPosJumpY;
		m_bAttackerRight = source.m_bAttackerRight;
		m_bAttackerAwaken = source.m_bAttackerAwaken;
		m_AttackerAddAttackUnitCount = source.m_AttackerAddAttackUnitCount;
		m_AttackerUnitSkillTemplet = source.m_AttackerUnitSkillTemplet;
	}

	public override void Close()
	{
		Init();
	}
}
