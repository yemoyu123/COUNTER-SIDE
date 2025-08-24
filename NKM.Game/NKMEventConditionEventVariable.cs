using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Game;

public class NKMEventConditionEventVariable : NKMEventConditionDetail
{
	public string m_Name = string.Empty;

	public NKMMinMaxInt m_Value = new NKMMinMaxInt();

	public bool m_bCheckMaster;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		int result = (int)(1u & (cNKMLua.GetData("m_Name", ref m_Name) ? 1u : 0u)) & (m_Value.LoadFromLua(cNKMLua, "m_Value") ? 1 : 0);
		cNKMLua.GetData("m_bCheckMaster", ref m_bCheckMaster);
		return (byte)result != 0;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		if (m_bCheckMaster && cNKMUnit.HasMasterUnit())
		{
			NKMUnit masterUnit = cNKMUnit.GetMasterUnit();
			if (masterUnit == null)
			{
				return false;
			}
			return m_Value.IsBetween(masterUnit.GetEventVariable(m_Name), negativeIsTrue: false);
		}
		int eventVariable = cNKMUnit.GetEventVariable(m_Name);
		return m_Value.IsBetween(eventVariable, negativeIsTrue: false);
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionEventVariable nKMEventConditionEventVariable = new NKMEventConditionEventVariable();
		nKMEventConditionEventVariable.m_Name = m_Name;
		nKMEventConditionEventVariable.m_Value.DeepCopyFromSource(m_Value);
		nKMEventConditionEventVariable.m_bCheckMaster = m_bCheckMaster;
		return nKMEventConditionEventVariable;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		if (string.IsNullOrEmpty(m_Name))
		{
			NKMTempletError.Add("[NKMEventConditionEventVariable] m_Name\ufffd\ufffd \ufffdԷµ\ufffd\ufffd\ufffd \ufffdʾ\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 1248);
			return false;
		}
		return true;
	}
}
