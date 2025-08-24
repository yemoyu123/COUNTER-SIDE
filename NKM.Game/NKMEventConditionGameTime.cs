using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionGameTime : NKMEventConditionDetail
{
	public enum Type
	{
		TIME_SPENT,
		TIME_LEFT
	}

	public Type m_Type = Type.TIME_LEFT;

	public NKMMinMaxFloat m_Time = new NKMMinMaxFloat(-1f, -1f);

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		return (byte)(1u & (m_Time.LoadFromLua(cNKMLua, "m_Time") ? 1u : 0u) & (cNKMLua.GetData("m_Type ", ref m_Type) ? 1u : 0u)) != 0;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		return m_Type switch
		{
			Type.TIME_SPENT => m_Time.IsBetween(cNKMGame.GetGameRuntimeData().GetGamePlayTime(), NegativeIsOpen: true), 
			Type.TIME_LEFT => m_Time.IsBetween(cNKMGame.GetGameRuntimeData().m_fRemainGameTime, NegativeIsOpen: true), 
			_ => false, 
		};
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionGameTime nKMEventConditionGameTime = new NKMEventConditionGameTime();
		nKMEventConditionGameTime.m_Time.DeepCopyFromSource(m_Time);
		nKMEventConditionGameTime.m_Type = m_Type;
		return nKMEventConditionGameTime;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return NKMEventConditionV2.ValidateNKMMinMax(m_Time, "[NKMEventConditionGameTime] m_Time\ufffd\ufffd \ufffdǹ\u033e\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd");
	}
}
