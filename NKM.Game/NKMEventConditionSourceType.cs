using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionSourceType : NKMEventConditionDetail
{
	private NKM_UNIT_SOURCE_TYPE m_SourceType;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return cNKMLua.GetData("m_SourceType", ref m_SourceType);
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		NKMUnitTempletBase unitTempletBase = cNKMUnit.GetUnitTempletBase();
		if (unitTempletBase == null)
		{
			return false;
		}
		if (m_SourceType == NKM_UNIT_SOURCE_TYPE.NUST_NONE)
		{
			if (unitTempletBase.m_NKM_UNIT_SOURCE_TYPE == NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				return unitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB == NKM_UNIT_SOURCE_TYPE.NUST_NONE;
			}
			return false;
		}
		return unitTempletBase.HasSourceType(m_SourceType);
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionSourceType
		{
			m_SourceType = m_SourceType
		};
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
