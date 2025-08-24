using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionBoss : NKMEventConditionDetail
{
	private bool m_bBoss = true;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_bBoss", ref m_bBoss);
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return cNKMUnit.IsBoss() == m_bBoss;
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionBoss
		{
			m_bBoss = m_bBoss
		};
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
