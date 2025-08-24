using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionNoCC : NKMEventConditionDetail
{
	private bool m_bExcludeConfuse;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_bExcludeConfuse", ref m_bExcludeConfuse);
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		foreach (NKM_UNIT_STATUS_EFFECT item in cNKMUnit.GetUnitFrameData().m_hsStatus)
		{
			if ((!m_bExcludeConfuse || item != NKM_UNIT_STATUS_EFFECT.NUSE_CONFUSE) && NKMUnitStatusTemplet.IsCrowdControlStatus(item))
			{
				return false;
			}
		}
		return true;
	}

	public override NKMEventConditionDetail Clone()
	{
		return new NKMEventConditionNoCC
		{
			m_bExcludeConfuse = m_bExcludeConfuse
		};
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
