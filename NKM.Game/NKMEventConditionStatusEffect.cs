using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Game;

public class NKMEventConditionStatusEffect : NKMEventConditionDetail
{
	public NKM_UNIT_STATUS_EFFECT m_StatusEffect;

	public bool m_bIgnore;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_bIgnore ", ref m_bIgnore);
		return cNKMLua.GetData("m_StatusEffect", ref m_StatusEffect);
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return cNKMUnit.HasStatus(m_StatusEffect) != m_bIgnore;
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionStatusEffect
		{
			m_StatusEffect = m_StatusEffect,
			m_bIgnore = m_bIgnore
		};
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		if (m_StatusEffect == NKM_UNIT_STATUS_EFFECT.NUSE_NONE)
		{
			NKMTempletError.Add("[NKMEventConditionStatusEffect] m_StatusEffect\ufffd\ufffd NONE\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 1058);
			return false;
		}
		return true;
	}
}
