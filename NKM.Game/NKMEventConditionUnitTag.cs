using System.Collections.Generic;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Game;

public class NKMEventConditionUnitTag : NKMEventConditionDetail
{
	public HashSet<NKM_UNIT_TAG> m_Type = new HashSet<NKM_UNIT_TAG>();

	public bool m_bIgnore;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_bIgnore", ref m_bIgnore);
		return cNKMLua.GetDataListEnum("m_Type", m_Type);
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return cNKMUnit.GetUnitTempletBase().HasUnitTagType(m_Type) != m_bIgnore;
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionUnitTag nKMEventConditionUnitTag = new NKMEventConditionUnitTag();
		nKMEventConditionUnitTag.m_bIgnore = m_bIgnore;
		nKMEventConditionUnitTag.m_Type.Clear();
		nKMEventConditionUnitTag.m_Type.UnionWith(m_Type);
		return nKMEventConditionUnitTag;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		if (m_Type.Count == 0)
		{
			NKMTempletError.Add("[NKMEventConditionUnitTag] m_Type\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 1555);
			return false;
		}
		return true;
	}
}
