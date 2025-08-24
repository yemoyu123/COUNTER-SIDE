using System.Collections.Generic;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM;

public class NKMTacticalCommandTemplet : INKMTemplet
{
	public int m_TCID;

	public string m_TCStrID = "";

	public string m_TCName;

	public List<string> m_lstTCDescMyTeam = new List<string>();

	public List<string> m_lstTCDescEnemy = new List<string>();

	public string m_TCIconName = "";

	public string m_TCEffectName = "";

	public string m_TCEffectSound = "";

	public float m_fCoolTime;

	public float m_fActiveTime;

	public float m_fActiveTimePerLevel;

	public NKM_TACTICAL_COMMAND_TYPE m_NKM_TACTICAL_COMMAND_TYPE;

	public List<NKMTacticalCombo> m_listComboType = new List<NKMTacticalCombo>();

	public float m_fComboResetCoolTime;

	public int m_StartCost;

	public int m_CostAdd;

	public List<string> m_lstBuffStrID_MyTeam;

	public List<string> m_lstBuffStrID_Enemy;

	public List<string> m_lstBuffStrID_MyTeamPVP;

	public List<string> m_lstBuffStrID_EnemyPVP;

	public float m_fCostPump;

	public float m_fCostPumpPerLevel;

	public bool m_bTargetBossMyTeam;

	public bool m_bTargetBossEnemy;

	public string m_UnitStrID;

	public int Key => m_TCID;

	public bool CheckEnemyTargetBuffExist(bool bPvP)
	{
		if (bPvP && m_lstBuffStrID_EnemyPVP != null && m_lstBuffStrID_EnemyPVP.Count > 0)
		{
			return true;
		}
		if (m_lstBuffStrID_Enemy == null)
		{
			return false;
		}
		if (m_lstBuffStrID_Enemy.Count <= 0)
		{
			return false;
		}
		return true;
	}

	public bool CheckMyTeamTargetBuffExist(bool bPvP)
	{
		if (bPvP && m_lstBuffStrID_MyTeamPVP != null && m_lstBuffStrID_MyTeamPVP.Count > 0)
		{
			return true;
		}
		if (m_lstBuffStrID_MyTeam == null)
		{
			return false;
		}
		if (m_lstBuffStrID_MyTeam.Count <= 0)
		{
			return false;
		}
		return true;
	}

	public List<string> GetMyTeamBuffStrList(bool bPvP)
	{
		if (bPvP && m_lstBuffStrID_MyTeamPVP != null && m_lstBuffStrID_MyTeamPVP.Count > 0)
		{
			return m_lstBuffStrID_MyTeamPVP;
		}
		return m_lstBuffStrID_MyTeam;
	}

	public List<string> GetEnemyTeamBuffStrList(bool bPvP)
	{
		if (bPvP && m_lstBuffStrID_EnemyPVP != null && m_lstBuffStrID_EnemyPVP.Count > 0)
		{
			return m_lstBuffStrID_EnemyPVP;
		}
		return m_lstBuffStrID_Enemy;
	}

	public static NKMTacticalCommandTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTacticalCommand.cs", 305))
		{
			return null;
		}
		NKMTacticalCommandTemplet nKMTacticalCommandTemplet = new NKMTacticalCommandTemplet();
		cNKMLua.GetData("m_TCID", ref nKMTacticalCommandTemplet.m_TCID);
		cNKMLua.GetData("m_TCStrID", ref nKMTacticalCommandTemplet.m_TCStrID);
		cNKMLua.GetData("m_TCName", ref nKMTacticalCommandTemplet.m_TCName);
		if (cNKMLua.OpenTable("m_lstTCDescMyTeam"))
		{
			int i = 1;
			for (string rValue = ""; cNKMLua.GetData(i, ref rValue); i++)
			{
				nKMTacticalCommandTemplet.m_lstTCDescMyTeam.Add(rValue);
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_lstTCDescEnemy"))
		{
			int j = 1;
			for (string rValue2 = ""; cNKMLua.GetData(j, ref rValue2); j++)
			{
				nKMTacticalCommandTemplet.m_lstTCDescEnemy.Add(rValue2);
			}
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_TCIconName", ref nKMTacticalCommandTemplet.m_TCIconName);
		cNKMLua.GetData("m_TCEffectName", ref nKMTacticalCommandTemplet.m_TCEffectName);
		cNKMLua.GetData("m_TCEffectSound", ref nKMTacticalCommandTemplet.m_TCEffectSound);
		cNKMLua.GetData("m_fCoolTime", ref nKMTacticalCommandTemplet.m_fCoolTime);
		cNKMLua.GetData("m_fActiveTime", ref nKMTacticalCommandTemplet.m_fActiveTime);
		cNKMLua.GetData("m_fActiveTimePerLevel", ref nKMTacticalCommandTemplet.m_fActiveTimePerLevel);
		cNKMLua.GetData("m_NKM_TACTICAL_COMMAND_TYPE", ref nKMTacticalCommandTemplet.m_NKM_TACTICAL_COMMAND_TYPE);
		nKMTacticalCommandTemplet.m_listComboType.Clear();
		if (nKMTacticalCommandTemplet.m_NKM_TACTICAL_COMMAND_TYPE == NKM_TACTICAL_COMMAND_TYPE.NTCT_COMBO)
		{
			string rValue3 = "";
			for (int k = 1; cNKMLua.GetData($"m_ComboValue{k}", ref rValue3); k++)
			{
				NKMTacticalCombo nKMTacticalCombo = new NKMTacticalCombo();
				if (!nKMTacticalCombo.Load(rValue3))
				{
					break;
				}
				nKMTacticalCommandTemplet.m_listComboType.Add(nKMTacticalCombo);
			}
			if (nKMTacticalCommandTemplet.m_listComboType.Count <= 0)
			{
				Log.ErrorAndExit("[NKMTacticalCommandTemplet] ComboValue is not found, m_TCStrID : " + nKMTacticalCommandTemplet.m_TCStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTacticalCommand.cs", 376);
				return null;
			}
		}
		cNKMLua.GetData("m_fComboResetCoolTime", ref nKMTacticalCommandTemplet.m_fComboResetCoolTime);
		cNKMLua.GetData("m_StartCost", ref nKMTacticalCommandTemplet.m_StartCost);
		cNKMLua.GetData("m_CostAdd", ref nKMTacticalCommandTemplet.m_CostAdd);
		cNKMLua.GetDataList("m_lstBuffStrID_MyTeam", out nKMTacticalCommandTemplet.m_lstBuffStrID_MyTeam, nullIfEmpty: false);
		cNKMLua.GetDataList("m_lstBuffStrID_Enemy", out nKMTacticalCommandTemplet.m_lstBuffStrID_Enemy, nullIfEmpty: false);
		cNKMLua.GetDataList("m_lstBuffStrID_MyTeamPVP", out nKMTacticalCommandTemplet.m_lstBuffStrID_MyTeamPVP, nullIfEmpty: false);
		cNKMLua.GetDataList("m_lstBuffStrID_EnemyPVP", out nKMTacticalCommandTemplet.m_lstBuffStrID_EnemyPVP, nullIfEmpty: false);
		cNKMLua.GetData("m_fCostPump", ref nKMTacticalCommandTemplet.m_fCostPump);
		cNKMLua.GetData("m_fCostPumpPerLevel", ref nKMTacticalCommandTemplet.m_fCostPumpPerLevel);
		cNKMLua.GetData("m_bTargetBossMyTeam", ref nKMTacticalCommandTemplet.m_bTargetBossMyTeam);
		cNKMLua.GetData("m_bTargetBossEnemy", ref nKMTacticalCommandTemplet.m_bTargetBossEnemy);
		cNKMLua.GetData("m_UnitStrID", ref nKMTacticalCommandTemplet.m_UnitStrID);
		return nKMTacticalCommandTemplet;
	}

	public static NKMTacticalCommandTemplet Find(int key)
	{
		return NKMTempletContainer<NKMTacticalCommandTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_lstBuffStrID_Enemy != null)
		{
			foreach (string item in m_lstBuffStrID_Enemy)
			{
				if (NKMBuffManager.GetBuffTempletByStrID(item) == null)
				{
					Log.ErrorAndExit("[NKMTacticalCommandTemplet] m_lstBuffStrID_Enemy is invalid. m_TCStrID [" + m_TCStrID + "], m_lstBuffStrID_Enemy [" + item + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTacticalCommand.cs", 422);
				}
			}
		}
		if (m_lstBuffStrID_MyTeam != null)
		{
			foreach (string item2 in m_lstBuffStrID_MyTeam)
			{
				if (NKMBuffManager.GetBuffTempletByStrID(item2) == null)
				{
					Log.ErrorAndExit("[NKMTacticalCommandTemplet] m_lstBuffStrID_Team is invalid. m_TCStrID [" + m_TCStrID + "], m_lstBuffStrID_Team [" + item2 + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTacticalCommand.cs", 433);
				}
			}
		}
		if (m_lstTCDescMyTeam != null && m_lstBuffStrID_MyTeam != null && m_lstTCDescMyTeam.Count != m_lstBuffStrID_MyTeam.Count)
		{
			Log.ErrorAndExit("[NKMTacticalCommandTemplet] m_lstTCDescMyTeam and m_lstBuffStrID_MyTeam Count different, m_TCStrID = " + m_TCStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTacticalCommand.cs", 442);
		}
		if (m_lstTCDescEnemy != null && m_lstBuffStrID_Enemy != null && m_lstTCDescEnemy.Count != m_lstBuffStrID_Enemy.Count)
		{
			Log.ErrorAndExit("[NKMTacticalCommandTemplet] m_lstTCDescEnemy and m_lstBuffStrID_Enemy Count different : , m_TCStrID = " + m_TCStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTacticalCommand.cs", 450);
		}
		if (m_listComboType == null)
		{
			return;
		}
		for (int i = 0; i < m_listComboType.Count; i++)
		{
			NKMTacticalCombo nKMTacticalCombo = m_listComboType[i];
			if (nKMTacticalCombo == null)
			{
				Log.ErrorAndExit("[NKMTacticalCommandTemplet] Validate NKMTacticalCombo null found, m_TCStrID = " + m_TCStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTacticalCommand.cs", 461);
			}
			else if (nKMTacticalCombo.m_NKM_TACTICAL_COMBO_TYPE == NKM_TACTICAL_COMBO_TYPE.NTCBT_RESPAWN_COST)
			{
				int valueInt = nKMTacticalCombo.m_ValueInt;
				if (valueInt <= 0 || valueInt > 10)
				{
					Log.ErrorAndExit($"[NKMTacticalCommandTemplet] NTCBT_RESPAWN_COST is invalid, m_ValueInt is {nKMTacticalCombo.m_ValueInt}" + ", m_TCStrID = " + m_TCStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTacticalCommand.cs", 470);
				}
			}
			else if (nKMTacticalCombo.m_NKM_TACTICAL_COMBO_TYPE == NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_STR_ID && NKMUnitManager.GetUnitTempletBase(nKMTacticalCombo.m_Value) == null)
			{
				Log.ErrorAndExit("[NKMTacticalCommandTemplet] NTCBT_UNIT_ID is invalid, m_ValueInt is " + nKMTacticalCombo.m_Value + ", m_TCStrID = " + m_TCStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTacticalCommand.cs", 477);
			}
		}
	}

	public float GetNeedCost(NKMTacticalCommandData cNKMTacticalCommandData)
	{
		if (cNKMTacticalCommandData == null)
		{
			return 0f;
		}
		float num = m_StartCost + m_CostAdd * cNKMTacticalCommandData.m_UseCount;
		if (num > 9f)
		{
			num = 9f;
		}
		return num;
	}

	public string GetTCName()
	{
		return NKCStringTable.GetString(m_TCName);
	}

	public string GetTCDescMyTeam(int index)
	{
		if (m_lstTCDescMyTeam == null)
		{
			return "";
		}
		if (index < 0 || index >= m_lstTCDescMyTeam.Count)
		{
			return "";
		}
		return NKCStringTable.GetString(m_lstTCDescMyTeam[index]);
	}

	public string GetTCDescEnemy(int index)
	{
		if (m_lstTCDescEnemy == null)
		{
			return "";
		}
		if (index < 0 || index >= m_lstTCDescEnemy.Count)
		{
			return "";
		}
		return NKCStringTable.GetString(m_lstTCDescEnemy[index]);
	}
}
