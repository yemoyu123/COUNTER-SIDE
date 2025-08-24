using System.Collections.Generic;
using Cs.Math;
using NKM.Game;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Unit;

namespace NKM;

public class NKMEventHeal : NKMUnitEffectStateEventOneTime, INKMTemplet, INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public string m_EventStrID = "";

	public float m_fHeal;

	public float m_fHealRate;

	public float m_fHealRatePerAtk;

	public float m_fHealRatePerLoseHpRate;

	public bool m_bEnableSelfHeal = true;

	public float m_fRangeMin;

	public float m_fRangeMax;

	public bool m_bUseUnitSize;

	public bool m_bUseTriggerTargetRange;

	public int m_MaxCount;

	public float m_fHealPowerPerSkillLevel;

	public int m_HealCountPerSkillLevel;

	public HashSet<NKM_UNIT_STYLE_TYPE> m_AllowStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public HashSet<NKM_UNIT_STYLE_TYPE> m_IgnoreStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public bool m_bIgnoreShip = true;

	public bool m_bSplashNearTarget;

	public bool m_bSelfTargetingOnly;

	public NKMEventConditionV2 m_ConditionTarget;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public int Key => 0;

	public void DeepCopyFromSource(NKMEventHeal source)
	{
		DeepCopy(source);
		m_EventStrID = source.m_EventStrID;
		m_fHeal = source.m_fHeal;
		m_fHealRate = source.m_fHealRate;
		m_fHealRatePerAtk = source.m_fHealRatePerAtk;
		m_fHealRatePerLoseHpRate = source.m_fHealRatePerLoseHpRate;
		m_bEnableSelfHeal = source.m_bEnableSelfHeal;
		m_fRangeMin = source.m_fRangeMin;
		m_fRangeMax = source.m_fRangeMax;
		m_bUseUnitSize = source.m_bUseUnitSize;
		m_bUseTriggerTargetRange = source.m_bUseTriggerTargetRange;
		m_MaxCount = source.m_MaxCount;
		m_fHealPowerPerSkillLevel = source.m_fHealPowerPerSkillLevel;
		m_HealCountPerSkillLevel = source.m_HealCountPerSkillLevel;
		m_bIgnoreShip = source.m_bIgnoreShip;
		m_IgnoreStyleType.Clear();
		foreach (NKM_UNIT_STYLE_TYPE item in source.m_IgnoreStyleType)
		{
			m_IgnoreStyleType.Add(item);
		}
		m_AllowStyleType.Clear();
		foreach (NKM_UNIT_STYLE_TYPE item2 in source.m_AllowStyleType)
		{
			m_AllowStyleType.Add(item2);
		}
		m_bSplashNearTarget = source.m_bSplashNearTarget;
		m_bSelfTargetingOnly = source.m_bSelfTargetingOnly;
		m_ConditionTarget = NKMEventConditionV2.Clone(source.m_ConditionTarget);
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_EventStrID", ref m_EventStrID);
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_fHeal", ref m_fHeal);
		cNKMLua.GetData("m_fHealRate", ref m_fHealRate);
		cNKMLua.GetData("m_fHealRatePerAtk", ref m_fHealRatePerAtk);
		cNKMLua.GetData("m_fHealRatePerLoseHpRate", ref m_fHealRatePerLoseHpRate);
		cNKMLua.GetData("m_bEnableSelfHeal", ref m_bEnableSelfHeal);
		cNKMLua.GetData("m_fRangeMin", ref m_fRangeMin);
		cNKMLua.GetData("m_fRangeMax", ref m_fRangeMax);
		cNKMLua.GetData("m_bUseUnitSize", ref m_bUseUnitSize);
		cNKMLua.GetData("m_bUseTriggerTargetRange", ref m_bUseTriggerTargetRange);
		cNKMLua.GetData("m_MaxCount", ref m_MaxCount);
		cNKMLua.GetData("m_fHealPowerPerSkillLevel", ref m_fHealPowerPerSkillLevel);
		cNKMLua.GetData("m_HealCountPerSkillLevel", ref m_HealCountPerSkillLevel);
		cNKMLua.GetData("m_bIgnoreShip", ref m_bIgnoreShip);
		cNKMLua.GetData("m_bSplashNearTarget", ref m_bSplashNearTarget);
		cNKMLua.GetData("m_bSelfTargetingOnly", ref m_bSelfTargetingOnly);
		m_IgnoreStyleType.Clear();
		if (cNKMLua.OpenTable("m_IgnoreStyleType"))
		{
			bool flag = true;
			int num = 1;
			NKM_UNIT_STYLE_TYPE result = NKM_UNIT_STYLE_TYPE.NUST_INVALID;
			while (flag)
			{
				flag = cNKMLua.GetData(num, ref result);
				if (flag)
				{
					m_IgnoreStyleType.Add(result);
				}
				num++;
			}
			cNKMLua.CloseTable();
		}
		m_AllowStyleType.Clear();
		if (cNKMLua.OpenTable("m_AllowStyleType"))
		{
			bool flag2 = true;
			int num2 = 1;
			NKM_UNIT_STYLE_TYPE result2 = NKM_UNIT_STYLE_TYPE.NUST_INVALID;
			while (flag2)
			{
				flag2 = cNKMLua.GetData(num2, ref result2);
				if (flag2)
				{
					m_AllowStyleType.Add(result2);
				}
				num2++;
			}
			cNKMLua.CloseTable();
		}
		m_ConditionTarget = NKMEventConditionV2.LoadFromLUA(cNKMLua, "m_ConditionTarget");
		return true;
	}

	public static NKMEventHeal LoadFromLUAStatic(NKMLua cNKMLua)
	{
		NKMEventHeal nKMEventHeal = new NKMEventHeal();
		nKMEventHeal.LoadFromLUA(cNKMLua);
		nKMEventHeal.Validate();
		return nKMEventHeal;
	}

	public float CalcHealAmount(int skillLevel, NKMUnit caster, NKMUnit targetUnit)
	{
		float num = 0f;
		float num2 = 1f;
		if (skillLevel > 1)
		{
			num2 += (float)(m_HealCountPerSkillLevel * (skillLevel - 1));
		}
		if (m_fHeal > 0f)
		{
			num += m_fHeal;
		}
		if (m_fHealRate != 0f && targetUnit != null)
		{
			num += targetUnit.GetMaxHP() * m_fHealRate;
		}
		if (m_fHealRatePerAtk > 0f && caster != null)
		{
			num += caster.GetStatFinal(NKM_STAT_TYPE.NST_ATK) * m_fHealRatePerAtk;
		}
		if (m_fHealRatePerLoseHpRate > 0f && targetUnit != null)
		{
			num += targetUnit.GetMaxHP() * (1f - targetUnit.GetHPRate()) * m_fHealRatePerLoseHpRate;
		}
		return num * num2;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_fHeal.IsNearlyZero() && m_fHealRate.IsNearlyZero() && m_fHealRatePerAtk.IsNearlyZero() && m_fHealRatePerLoseHpRate.IsNearlyZero())
		{
			NKMTempletError.Add($"[EventHeal:{m_EventStrID}] 수치가 올바르지 않음. heal:{m_fHeal} healRate:{m_fHealRate} m_fHealRatePerAtk:{m_fHealRatePerAtk} m_fHealRatePerLoseHpRate:{m_fHealRatePerLoseHpRate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 2263);
		}
		if (m_bSelfTargetingOnly)
		{
			if (m_bSplashNearTarget)
			{
				NKMTempletError.Add("[EventHeal:" + m_EventStrID + "] 개인 이벤트힐은 m_bSplashNearTarget 설정 불가.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 2270);
			}
			if (m_IgnoreStyleType.Count > 0)
			{
				NKMTempletError.Add("[EventHeal:" + m_EventStrID + "] 개인 이벤트힐은 m_IgnoreStyleType 설정 불가.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 2275);
			}
			if (!m_bEnableSelfHeal)
			{
				NKMTempletError.Add("[EventHeal:" + m_EventStrID + "] 개인 이벤트힐은 m_bEnableSelfHeal 설정 필수.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 2280);
			}
			if (m_MaxCount > 1)
			{
				NKMTempletError.Add($"[EventHeal:{m_EventStrID}] 개인 이벤트힐은 m_MaxCount = 0 or 1 제한. maxCount:{m_MaxCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 2285);
			}
			if (m_HealCountPerSkillLevel != 0)
			{
				NKMTempletError.Add($"[EventHeal:{m_EventStrID}] 개인 이벤트힐은 m_HealCountPerSkillLevel = 0 설정 필수. m_HealCountPerSkillLevel:{m_HealCountPerSkillLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 2290);
			}
			if (!m_fRangeMin.IsNearlyZero() || !m_fRangeMax.IsNearlyZero())
			{
				NKMTempletError.Add($"[EventHeal:{m_EventStrID}] 개인 이벤트힐은 범위 지정하지 마세요. range min:{m_fRangeMin} max:{m_fRangeMax}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 2295);
			}
		}
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		NKMUnit triggerTargetUnit = cNKMUnit.GetTriggerTargetUnit(m_bUseTriggerTargetRange);
		cNKMUnit.SetEventHeal(this, triggerTargetUnit.GetUnitSyncData().m_PosX, triggerTargetUnit);
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMDamageEffect cNKMDamageEffect)
	{
		cNKMDamageEffect.GetMasterUnit().SetEventHeal(this, cNKMDamageEffect.GetDEData().m_PosX, null);
	}

	public override void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime)
	{
		NKMUnit triggerTargetUnit = cNKMUnit.GetTriggerTargetUnit(m_bUseTriggerTargetRange);
		cNKMUnit.SetEventHeal(this, triggerTargetUnit.GetUnitSyncData().m_PosX, triggerTargetUnit);
	}
}
