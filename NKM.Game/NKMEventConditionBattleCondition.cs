using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Game;

public class NKMEventConditionBattleCondition : NKMEventConditionDetail
{
	public string m_BCID;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return cNKMLua.GetData("m_BCID", ref m_BCID);
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(m_BCID);
		if (cNKMGame.HasBattleCondition(templetByStrID))
		{
			return cNKMGame.IsBattleConditionActivated(templetByStrID);
		}
		return false;
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionBattleCondition
		{
			m_BCID = m_BCID
		};
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		if (string.IsNullOrEmpty(m_BCID))
		{
			NKMTempletError.Add("[NKMEventConditionBattleCondition] " + m_BCID + " Empty!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 1726);
			return false;
		}
		if (NKMBattleConditionManager.GetTempletByStrID(m_BCID) == null)
		{
			NKMTempletError.Add("[NKMEventConditionBattleCondition] " + m_BCID + " Templet not exist!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 1733);
			return false;
		}
		return true;
	}
}
