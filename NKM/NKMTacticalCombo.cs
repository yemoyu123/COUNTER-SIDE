using System;
using Cs.Logging;
using NKM.Templet;

namespace NKM;

public class NKMTacticalCombo
{
	public NKM_TACTICAL_COMBO_TYPE m_NKM_TACTICAL_COMBO_TYPE = NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_ROLE_TYPE;

	public string m_Value = "";

	public NKM_UNIT_ROLE_TYPE m_NKM_UNIT_ROLE_TYPE;

	public NKM_UNIT_STYLE_TYPE m_NKM_UNIT_STYLE_TYPE;

	public int m_ValueInt;

	public bool CheckCond(NKMUnitTempletBase cUnitTempletBase, int respawnCost)
	{
		switch (m_NKM_TACTICAL_COMBO_TYPE)
		{
		case NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_ROLE_TYPE:
			if (cUnitTempletBase == null)
			{
				return false;
			}
			return m_NKM_UNIT_ROLE_TYPE == cUnitTempletBase.m_NKM_UNIT_ROLE_TYPE;
		case NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_STYLE_TYPE:
			return cUnitTempletBase?.HasUnitStyleType(m_NKM_UNIT_STYLE_TYPE) ?? false;
		case NKM_TACTICAL_COMBO_TYPE.NTCBT_RESPAWN_COST:
			return m_ValueInt == respawnCost;
		case NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_STR_ID:
			if (cUnitTempletBase == null)
			{
				return false;
			}
			return m_Value == cUnitTempletBase.m_UnitStrID;
		default:
			return false;
		}
	}

	public bool Load(string comboValue)
	{
		if (string.IsNullOrWhiteSpace(comboValue))
		{
			return false;
		}
		if (int.TryParse(comboValue, out m_ValueInt))
		{
			m_NKM_TACTICAL_COMBO_TYPE = NKM_TACTICAL_COMBO_TYPE.NTCBT_RESPAWN_COST;
			return true;
		}
		if (Enum.TryParse<NKM_UNIT_ROLE_TYPE>(comboValue, ignoreCase: true, out m_NKM_UNIT_ROLE_TYPE))
		{
			m_NKM_TACTICAL_COMBO_TYPE = NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_ROLE_TYPE;
			return true;
		}
		if (Enum.TryParse<NKM_UNIT_STYLE_TYPE>(comboValue, ignoreCase: true, out m_NKM_UNIT_STYLE_TYPE))
		{
			m_NKM_TACTICAL_COMBO_TYPE = NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_STYLE_TYPE;
			return true;
		}
		if (NKMUnitManager.GetUnitTempletBase(comboValue) != null)
		{
			m_NKM_TACTICAL_COMBO_TYPE = NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_STR_ID;
			m_Value = comboValue;
			return true;
		}
		Log.ErrorAndExit("[NKMTacticalCommandTemplet] comboValue is bad, comboValue is " + comboValue, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTacticalCommand.cs", 187);
		return false;
	}
}
