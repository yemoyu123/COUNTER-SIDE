using Cs.Logging;

namespace NKM;

public class NKMEventVariable : NKMUnitStateEventOneTime
{
	public enum Type
	{
		SET,
		ADD,
		RANDOM
	}

	public Type m_Type = Type.ADD;

	public string m_Name = string.Empty;

	public int m_Value;

	public bool m_bVolatile;

	public bool m_bUseTriggerTargetRange;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		if (cNKMUnit.Get_NKM_UNIT_CLASS_TYPE() == NKM_UNIT_CLASS_TYPE.NCT_UNIT_SERVER)
		{
			NKMUnit triggerTargetUnit = cNKMUnit.GetTriggerTargetUnit(m_bUseTriggerTargetRange);
			switch (m_Type)
			{
			case Type.ADD:
				triggerTargetUnit.AddEventVariable(m_Name, m_Value, m_bVolatile);
				break;
			case Type.SET:
				triggerTargetUnit.SetEventVariable(m_Name, m_Value, m_bVolatile);
				break;
			case Type.RANDOM:
			{
				int value = NKMRandom.Range(0, m_Value + 1);
				triggerTargetUnit.SetEventVariable(m_Name, value, m_bVolatile);
				break;
			}
			}
		}
	}

	public void DeepCopyFromSource(NKMEventVariable source)
	{
		DeepCopy(source);
		m_Type = source.m_Type;
		m_Name = source.m_Name;
		m_Value = source.m_Value;
		m_bUseTriggerTargetRange = source.m_bUseTriggerTargetRange;
		m_bVolatile = source.m_bVolatile;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_Type", ref m_Type);
		cNKMLua.GetData("m_Name", ref m_Name);
		cNKMLua.GetData("m_Value", ref m_Value);
		cNKMLua.GetData("m_bVolatile", ref m_bVolatile);
		cNKMLua.GetData("m_bUseTriggerTargetRange", ref m_bUseTriggerTargetRange);
		return true;
	}

	public bool Validate()
	{
		if (m_Type == Type.RANDOM && m_Value <= 0)
		{
			Log.ErrorAndExit("[NKMEventVariable] m_Value must >0 for RANDOM option", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 4579);
			return false;
		}
		return true;
	}
}
