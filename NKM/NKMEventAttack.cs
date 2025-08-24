using System.Collections.Generic;
using NKM.Templet;
using NKM.Unit;

namespace NKM;

public class NKMEventAttack : IEventConditionOwner, INKMUnitStateEvent
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public NKMEventCondition m_ConditionTarget = new NKMEventCondition();

	public bool m_bAnimTime = true;

	public float m_fEventTimeMin;

	public float m_fEventTimeMax;

	public float m_fRangeMin = 100f;

	public float m_fRangeMax = 100f;

	public bool m_bHitLand = true;

	public bool m_bHitAir = true;

	public bool m_bHitSummonOnly;

	public bool m_bHitAwakenUnit = true;

	public bool m_bHitNormalUnit = true;

	public bool m_bHitBossUnit = true;

	public bool m_bForceHit;

	public bool m_bTrueDamage;

	public bool m_bCleanHit;

	public NKM_DAMAGE_TARGET_TYPE m_NKM_DAMAGE_TARGET_TYPE = NKM_DAMAGE_TARGET_TYPE.NDTT_ENEMY;

	public HashSet<NKM_UNIT_STYLE_TYPE> m_listAllowStyle = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public HashSet<NKM_UNIT_STYLE_TYPE> m_listIgnoreStyle = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public HashSet<NKM_UNIT_ROLE_TYPE> m_listAllowRole = new HashSet<NKM_UNIT_ROLE_TYPE>();

	public HashSet<NKM_UNIT_ROLE_TYPE> m_listIgnoreRole = new HashSet<NKM_UNIT_ROLE_TYPE>();

	public int m_TargetCostLess = -1;

	public int m_TargetCostOver = -1;

	public bool m_AttackTargetUnit = true;

	public int m_AttackUnitCount = 1;

	public bool m_AttackUnitCountOnly;

	public bool m_bDamageSpeedDependRight;

	public string m_DamageTempletName = "";

	public bool m_bForceCritical;

	public bool m_bNoCritical;

	public float m_fGetAgroTime;

	public string m_SoundName = "";

	public float m_fLocalVol = 1f;

	public string m_EffectName = "";

	public string m_HitStateChange = "";

	public NKMEventCondition Condition => m_Condition;

	public float EventStartTime => m_fEventTimeMin;

	public EventRollbackType RollbackType => EventRollbackType.Prohibited;

	public bool bAnimTime => m_bAnimTime;

	public bool bStateEnd => false;

	public void DeepCopyFromSource(NKMEventAttack source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_ConditionTarget.DeepCopyFromSource(source.m_ConditionTarget);
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTimeMin = source.m_fEventTimeMin;
		m_fEventTimeMax = source.m_fEventTimeMax;
		m_fRangeMin = source.m_fRangeMin;
		m_fRangeMax = source.m_fRangeMax;
		m_bHitLand = source.m_bHitLand;
		m_bHitAir = source.m_bHitAir;
		m_bHitSummonOnly = source.m_bHitSummonOnly;
		m_bHitAwakenUnit = source.m_bHitAwakenUnit;
		m_bHitNormalUnit = source.m_bHitNormalUnit;
		m_bHitBossUnit = source.m_bHitBossUnit;
		m_bForceHit = source.m_bForceHit;
		m_bTrueDamage = source.m_bTrueDamage;
		m_bCleanHit = source.m_bCleanHit;
		m_NKM_DAMAGE_TARGET_TYPE = source.m_NKM_DAMAGE_TARGET_TYPE;
		m_listAllowStyle.Clear();
		m_listAllowStyle.UnionWith(source.m_listAllowStyle);
		m_listIgnoreStyle.Clear();
		m_listIgnoreStyle.UnionWith(source.m_listIgnoreStyle);
		m_listAllowRole.Clear();
		m_listAllowRole.UnionWith(source.m_listAllowRole);
		m_listIgnoreRole.Clear();
		m_listIgnoreRole.UnionWith(source.m_listIgnoreRole);
		m_TargetCostLess = source.m_TargetCostLess;
		m_TargetCostOver = source.m_TargetCostOver;
		m_AttackTargetUnit = source.m_AttackTargetUnit;
		m_AttackUnitCount = source.m_AttackUnitCount;
		m_AttackUnitCountOnly = source.m_AttackUnitCountOnly;
		m_bDamageSpeedDependRight = source.m_bDamageSpeedDependRight;
		m_DamageTempletName = (string)source.m_DamageTempletName.Clone();
		m_bForceCritical = source.m_bForceCritical;
		m_bNoCritical = source.m_bNoCritical;
		m_fGetAgroTime = source.m_fGetAgroTime;
		m_SoundName = source.m_SoundName;
		m_fLocalVol = source.m_fLocalVol;
		m_EffectName = (string)source.m_EffectName.Clone();
		m_HitStateChange = (string)source.m_HitStateChange.Clone();
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		m_ConditionTarget.LoadFromLUA(cNKMLua, "m_ConditionTarget");
		cNKMLua.GetData("m_bAnimTime", ref m_bAnimTime);
		cNKMLua.GetData("m_fEventTimeMin", ref m_fEventTimeMin);
		cNKMLua.GetData("m_fEventTimeMax", ref m_fEventTimeMax);
		cNKMLua.GetData("m_fRangeMin", ref m_fRangeMin);
		cNKMLua.GetData("m_fRangeMax", ref m_fRangeMax);
		cNKMLua.GetData("m_bHitLand", ref m_bHitLand);
		cNKMLua.GetData("m_bHitAir", ref m_bHitAir);
		cNKMLua.GetData("m_bHitSummonOnly", ref m_bHitSummonOnly);
		cNKMLua.GetData("m_bHitAwakenUnit", ref m_bHitAwakenUnit);
		cNKMLua.GetData("m_bHitNormalUnit", ref m_bHitNormalUnit);
		cNKMLua.GetData("m_bHitBossUnit", ref m_bHitBossUnit);
		cNKMLua.GetData("m_bForceHit", ref m_bForceHit);
		cNKMLua.GetData("m_bTrueDamage", ref m_bTrueDamage);
		cNKMLua.GetData("m_bCleanHit", ref m_bCleanHit);
		cNKMLua.GetData("m_NKM_DAMAGE_TARGET_TYPE", ref m_NKM_DAMAGE_TARGET_TYPE);
		m_listAllowStyle.Clear();
		cNKMLua.GetDataListEnum("m_listAllowStyle", m_listAllowStyle);
		m_listIgnoreStyle.Clear();
		cNKMLua.GetDataListEnum("m_listIgnoreStyle", m_listIgnoreStyle);
		m_listAllowRole.Clear();
		cNKMLua.GetDataListEnum("m_listAllowRole", m_listAllowRole);
		m_listIgnoreRole.Clear();
		cNKMLua.GetDataListEnum("m_listIgnoreRole", m_listIgnoreRole);
		HashSet<NKM_UNIT_STYLE_TYPE> hashSet = new HashSet<NKM_UNIT_STYLE_TYPE>();
		if (cNKMLua.GetDataListEnum("m_listNKM_UNIT_STYLE_TYPE", hashSet))
		{
			m_listAllowStyle.UnionWith(hashSet);
		}
		HashSet<NKM_UNIT_ROLE_TYPE> hashSet2 = new HashSet<NKM_UNIT_ROLE_TYPE>();
		if (cNKMLua.GetDataListEnum("m_listNKM_UNIT_ROLE_TYPE", hashSet2))
		{
			m_listAllowRole.UnionWith(hashSet2);
		}
		cNKMLua.GetData("m_TargetCostLess", ref m_TargetCostLess);
		cNKMLua.GetData("m_TargetCostOver", ref m_TargetCostOver);
		cNKMLua.GetData("m_AttackTargetUnit", ref m_AttackTargetUnit);
		cNKMLua.GetData("m_AttackUnitCount", ref m_AttackUnitCount);
		cNKMLua.GetData("m_AttackUnitCountOnly", ref m_AttackUnitCountOnly);
		cNKMLua.GetData("m_bDamageSpeedDependRight", ref m_bDamageSpeedDependRight);
		cNKMLua.GetData("m_DamageTempletName", ref m_DamageTempletName);
		cNKMLua.GetData("m_bForceCritical", ref m_bForceCritical);
		cNKMLua.GetData("m_bNoCritical", ref m_bNoCritical);
		cNKMLua.GetData("m_fGetAgroTime", ref m_fGetAgroTime);
		cNKMLua.GetData("m_SoundName", ref m_SoundName);
		cNKMLua.GetData("m_fLocalVol", ref m_fLocalVol);
		cNKMLua.GetData("m_EffectName", ref m_EffectName);
		cNKMLua.GetData("m_HitStateChange", ref m_HitStateChange);
		return true;
	}
}
