using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Game;

public class NKMEventConditionUnitExist : NKMEventConditionDetail
{
	public enum UnitCountCond
	{
		INVALID,
		ALL,
		ANY,
		NOT_ANY,
		NOT_ALL
	}

	public List<int> m_listReqUnit;

	public UnitCountCond m_ReqUnitCond;

	public bool m_bReqUnitEnemy;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		int result = (int)(1u & (cNKMLua.GetDataList("m_listReqUnit", out m_listReqUnit, nullIfEmpty: false) ? 1u : 0u)) & (cNKMLua.GetData("m_ReqUnitCond", ref m_ReqUnitCond) ? 1 : 0);
		cNKMLua.GetData("m_bReqUnitEnemy", ref m_bReqUnitEnemy);
		return (byte)result != 0;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		NKM_TEAM_TYPE team = cNKMUnit.GetTeam();
		return CanUseUnitExist(cNKMGame, team);
	}

	private bool CanUseUnitExist(NKMGame cNKMGame, NKM_TEAM_TYPE myTeam)
	{
		return m_ReqUnitCond switch
		{
			UnitCountCond.ALL => m_listReqUnit.All(UnitExist), 
			UnitCountCond.ANY => m_listReqUnit.Exists(UnitExist), 
			UnitCountCond.NOT_ALL => m_listReqUnit.All(UnitNotExist), 
			UnitCountCond.NOT_ANY => m_listReqUnit.Exists(UnitNotExist), 
			_ => true, 
		};
		bool UnitExist(int unitID)
		{
			return (m_bReqUnitEnemy ? cNKMGame.GetEnemyUnitByUnitID(unitID, myTeam) : cNKMGame.GetUnitByUnitID(unitID, myTeam)) != null;
		}
		bool UnitNotExist(int unitID)
		{
			return (m_bReqUnitEnemy ? cNKMGame.GetEnemyUnitByUnitID(unitID, myTeam) : cNKMGame.GetUnitByUnitID(unitID, myTeam)) == null;
		}
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionUnitExist
		{
			m_ReqUnitCond = m_ReqUnitCond,
			m_bReqUnitEnemy = m_bReqUnitEnemy,
			m_listReqUnit = new List<int>(m_listReqUnit)
		};
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		if (m_listReqUnit == null || m_listReqUnit.Count == 0)
		{
			NKMTempletError.Add("[NKMEventConditionUnit] m_listReqUnit\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 1156);
			return false;
		}
		return true;
	}
}
