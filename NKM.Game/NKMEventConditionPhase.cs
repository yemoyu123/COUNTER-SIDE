using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionPhase : NKMEventConditionDetail
{
	public NKMMinMaxInt m_Phase;

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		int phaseNow = cNKMUnit.GetUnitFrameData().m_PhaseNow;
		if (m_Phase.m_Min != -1 && m_Phase.m_Min > phaseNow)
		{
			return false;
		}
		if (m_Phase.m_Max != -1 && m_Phase.m_Max < phaseNow)
		{
			return false;
		}
		return true;
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionPhase nKMEventConditionPhase = new NKMEventConditionPhase();
		nKMEventConditionPhase.m_Phase.DeepCopyFromSource(m_Phase);
		return nKMEventConditionPhase;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return m_Phase.LoadFromLua(cNKMLua, "m_Phase");
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return NKMEventConditionV2.ValidateNKMMinMax(m_Phase, "[NKMEventConditionPhase] m_Phase\ufffd\ufffd \ufffdǹ\u033e\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd");
	}
}
