using System.Collections.Generic;
using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionUnitID : NKMEventConditionDetail
{
	private HashSet<int> m_hsUnitID;

	private bool m_bIncludeBaseUnit = true;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		HashSet<string> hashSet = new HashSet<string>();
		cNKMLua.GetData("m_bIncludeBaseUnit", ref m_bIncludeBaseUnit);
		bool data = cNKMLua.GetData("m_UnitID", hashSet);
		if (!data)
		{
			return false;
		}
		m_hsUnitID = new HashSet<int>();
		foreach (string item in hashSet)
		{
			m_hsUnitID.Add(NKMUnitManager.GetUnitID(item));
		}
		return data;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		if (m_bIncludeBaseUnit)
		{
			return NKMUnitManager.CheckContainsBaseUnit(m_hsUnitID, cNKMUnit.GetUnitData().m_UnitID);
		}
		return m_hsUnitID.Contains(cNKMUnit.GetUnitData().m_UnitID);
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionUnitID
		{
			m_bIncludeBaseUnit = m_bIncludeBaseUnit,
			m_hsUnitID = new HashSet<int>(m_hsUnitID)
		};
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		if (m_hsUnitID != null)
		{
			return m_hsUnitID.Count > 0;
		}
		return false;
	}
}
