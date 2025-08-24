using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Game;

public class NKMEventConditionV2
{
	public enum ConditionType
	{
		ALL,
		ANY,
		NOT_ANY,
		NOT_ALL
	}

	private List<string> m_lstMacro;

	private ConditionType m_conditionType;

	private List<NKMEventConditionDetail> m_lstCondition = new List<NKMEventConditionDetail>();

	private static Dictionary<string, NKMEventConditionV2> s_dicTempletMacro = new Dictionary<string, NKMEventConditionV2>();

	public static void LoadTempletMacro()
	{
		s_dicTempletMacro.Clear();
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("ab_script", "LUA_EVENT_CONDITION_TEMPLET"))
		{
			Log.ErrorAndExit("Templet macro load failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 48);
		}
		else
		{
			LoadMacroFromLua(nKMLua, "LUA_EVENT_CONDITION", ref s_dicTempletMacro);
		}
	}

	public static NKMEventConditionV2 GetTempletMacroCondition(string name)
	{
		if (!s_dicTempletMacro.TryGetValue(name, out var value))
		{
			return null;
		}
		return value;
	}

	public static void ValidateTempletMacro()
	{
		foreach (KeyValuePair<string, NKMEventConditionV2> item in s_dicTempletMacro)
		{
			item.Value.Validate(null, null);
		}
	}

	public static bool CheckTempletMacro(List<string> lstMacro, NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		if (lstMacro == null)
		{
			return true;
		}
		foreach (string item in lstMacro)
		{
			if (!string.IsNullOrEmpty(item))
			{
				if (!s_dicTempletMacro.TryGetValue(item, out var value))
				{
					Log.Error("Templet Eventcondition macro [" + item + "] not found", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 85);
					return false;
				}
				if (!value.CheckEventCondition(cNKMGame, cNKMUnit, null))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static NKMEventConditionV2 LoadFromLUA(NKMLua cNKMLua, string tableName = "m_ConditionV2")
	{
		if (!cNKMLua.OpenTable(tableName))
		{
			string rValue = null;
			if (cNKMLua.GetData(tableName, ref rValue))
			{
				NKMEventConditionV2 nKMEventConditionV = new NKMEventConditionV2();
				nKMEventConditionV.m_lstMacro = new List<string>();
				nKMEventConditionV.m_lstMacro.Add(rValue);
				return nKMEventConditionV;
			}
			return null;
		}
		NKMEventConditionV2 result = LoadEventCondition(cNKMLua);
		cNKMLua.CloseTable();
		return result;
	}

	private static NKMEventConditionV2 LoadEventCondition(NKMLua cNKMLua)
	{
		NKMEventConditionV2 nKMEventConditionV = new NKMEventConditionV2();
		if (cNKMLua.GetData(1, out var rValue, string.Empty))
		{
			if (nKMEventConditionV.m_lstMacro == null)
			{
				nKMEventConditionV.m_lstMacro = new List<string>();
			}
			nKMEventConditionV.m_lstMacro.Add(rValue);
			for (int i = 2; cNKMLua.GetData(i, out rValue, string.Empty); i++)
			{
				nKMEventConditionV.m_lstMacro.Add(rValue);
			}
			return nKMEventConditionV;
		}
		cNKMLua.GetData("ConditionType", ref nKMEventConditionV.m_conditionType);
		int num = 1;
		while (cNKMLua.OpenTable(num))
		{
			NKMEventConditionDetail nKMEventConditionDetail = LoadDetail(cNKMLua);
			if (nKMEventConditionDetail != null)
			{
				nKMEventConditionV.m_lstCondition.Add(nKMEventConditionDetail);
			}
			num++;
			cNKMLua.CloseTable();
		}
		return nKMEventConditionV;
	}

	public static bool LoadMacroFromLua(NKMLua cNKMLua, string tableName, ref Dictionary<string, NKMEventConditionV2> dicEventMacro)
	{
		if (!cNKMLua.OpenTable(tableName))
		{
			return false;
		}
		if (dicEventMacro == null)
		{
			dicEventMacro = new Dictionary<string, NKMEventConditionV2>();
		}
		int num = 1;
		while (cNKMLua.OpenTable(num))
		{
			num++;
			string rValue = null;
			if (!cNKMLua.GetData("m_Name", ref rValue))
			{
				NKMTempletError.Add("[m_listConditionMacro] m_Name Not Found!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 184);
				cNKMLua.CloseTable();
			}
			else
			{
				NKMEventConditionV2 value = LoadEventCondition(cNKMLua);
				dicEventMacro.Add(rValue, value);
				cNKMLua.CloseTable();
			}
		}
		cNKMLua.CloseTable();
		return true;
	}

	private static NKMEventConditionDetail LoadDetail(NKMLua cNKMLua)
	{
		string rValue = null;
		if (!cNKMLua.GetData("Type", ref rValue))
		{
			return null;
		}
		NKMEventConditionDetail nKMEventConditionDetail = TypeFactory(rValue);
		if (nKMEventConditionDetail == null)
		{
			return null;
		}
		if (!nKMEventConditionDetail.LoadFromLUA(cNKMLua))
		{
			NKMTempletError.Add("[NKMEventCondtionV2] " + rValue + " condition parse failed!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 214);
			return null;
		}
		nKMEventConditionDetail.Inverse = cNKMLua.GetBoolean("Inverse", defaultValue: false);
		return nKMEventConditionDetail;
	}

	private static NKMEventConditionDetail TypeFactory(string type)
	{
		switch (type)
		{
		case "Phase":
			return new NKMEventConditionPhase();
		case "SkillLevel":
			return new NKMEventConditionSkillLevel();
		case "Leader":
			return new NKMEventConditionLeader();
		case "Buff":
			return new NKMEventConditionBuff();
		case "MapPosition":
			return new NKMEventConditionMapPosition();
		case "HPRate":
			return new NKMEventConditionHPRate();
		case "Level":
			return new NKMEventConditionLevel();
		case "BuffCount":
			return new NKMEventConditionBuffCount();
		case "PVP":
			return new NKMEventConditionPVP();
		case "PVE":
			return new NKMEventConditionPVE();
		case "StatusEffect":
			return new NKMEventConditionStatusEffect();
		case "UnitExist":
			return new NKMEventConditionUnitExist();
		case "ReactorLevel":
			return new NKMEventConditionReactorLevel();
		case "EventVariable":
			return new NKMEventConditionEventVariable();
		case "Fail":
			return new NKMEventConditionFail();
		case "Cooltime":
			return new NKMEventConditionCooltime();
		case "StateCooltime":
			return new NKMEventConditionStateCooltime();
		case "NoCC":
			return new NKMEventConditionNoCC();
		case "GameTime":
			return new NKMEventConditionGameTime();
		case "UnitStyle":
			return new NKMEventConditionUnitStyle();
		case "UnitRole":
			return new NKMEventConditionUnitRole();
		case "UnitTag":
			return new NKMEventConditionUnitTag();
		case "Enemy":
			return new NKMEventConditionEnemy();
		case "Ally":
			return new NKMEventConditionAlly();
		case "Air":
			return new NKMEventConditionAir();
		case "Land":
			return new NKMEventConditionLand();
		case "Cost":
			return new NKMEventConditionCost();
		case "Boss":
			return new NKMEventConditionBoss();
		case "BattleCondition":
			return new NKMEventConditionBattleCondition();
		case "NoDamageState":
			return new NKMEventConditionNoDamageState();
		case "UnitCount":
			return new NKMEventConditionUnitCount();
		case "TriggerTarget":
			return new NKMEventConditionTriggerTarget();
		case "HasTarget":
			return new NKMEventConditionHasTarget();
		case "UnitID":
			return new NKMEventConditionUnitID();
		case "Distance":
			return new NKMEventConditionDistance();
		case "Alive":
			return new NKMEventConditionAlive();
		case "SourceType":
			return new NKMEventConditionSourceType();
		case "Front":
			return new NKMEventConditionFront();
		case "Awaken":
			return new NKMEventConditionAwaken();
		case "Rearm":
			return new NKMEventConditionRearm();
		default:
			NKMTempletError.Add("[NKMEventCondtionV2] Bad condition type : " + type, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 350);
			return null;
		}
	}

	public NKMEventConditionV2 Clone()
	{
		NKMEventConditionV2 nKMEventConditionV = new NKMEventConditionV2();
		nKMEventConditionV.m_conditionType = m_conditionType;
		if (m_lstMacro != null)
		{
			nKMEventConditionV.m_lstMacro = new List<string>(m_lstMacro);
		}
		else
		{
			nKMEventConditionV.m_lstMacro = null;
		}
		foreach (NKMEventConditionDetail item in m_lstCondition)
		{
			NKMEventConditionDetail nKMEventConditionDetail = item.Clone();
			nKMEventConditionDetail.Inverse = item.Inverse;
			nKMEventConditionV.m_lstCondition.Add(nKMEventConditionDetail);
		}
		return nKMEventConditionV;
	}

	public static NKMEventConditionV2 Clone(NKMEventConditionV2 source)
	{
		return source?.Clone();
	}

	public void DeepCopyFromSource(NKMEventConditionV2 source)
	{
		m_conditionType = source.m_conditionType;
		if (source.m_lstMacro != null)
		{
			m_lstMacro = new List<string>(source.m_lstMacro);
		}
		else
		{
			m_lstMacro = null;
		}
		m_lstCondition.Clear();
		foreach (NKMEventConditionDetail item in source.m_lstCondition)
		{
			NKMEventConditionDetail nKMEventConditionDetail = item.Clone();
			nKMEventConditionDetail.Inverse = item.Inverse;
			m_lstCondition.Add(nKMEventConditionDetail);
		}
	}

	public bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTempletBase)
	{
		if (m_lstMacro != null)
		{
			foreach (string item in m_lstMacro)
			{
				if (!string.IsNullOrEmpty(item) && unitTemplet.GetEventConditionMacro(item) == null)
				{
					Log.Error("[NKMEventConditionV2] Macro " + item + " Not found from unit " + unitTemplet.m_UnitTempletBase.DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 408);
					return false;
				}
			}
		}
		bool flag = true;
		foreach (NKMEventConditionDetail item2 in m_lstCondition)
		{
			flag &= item2.Validate(unitTemplet, masterTempletBase);
		}
		return flag;
	}

	public bool CheckEventCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		if (m_lstMacro != null)
		{
			foreach (string item in m_lstMacro)
			{
				if (string.IsNullOrEmpty(item))
				{
					continue;
				}
				NKMEventConditionV2 eventConditionMacro;
				if (cUnitConditionOwner != null)
				{
					eventConditionMacro = cUnitConditionOwner.GetEventConditionMacro(item);
					if (eventConditionMacro == null)
					{
						Log.Error("Eventcondition Macro '" + item + "' Not found from unit " + cUnitConditionOwner.GetUnitTempletBase().DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 440);
						return false;
					}
				}
				else
				{
					eventConditionMacro = cNKMUnit.GetEventConditionMacro(item);
					if (eventConditionMacro == null)
					{
						Log.Error("Eventcondition Macro '" + item + "' Not found from unit " + cNKMUnit.GetUnitTempletBase().DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 449);
						return false;
					}
				}
				if (!eventConditionMacro.CheckEventCondition(cNKMGame, cNKMUnit, cUnitConditionOwner))
				{
					return false;
				}
			}
			return true;
		}
		switch (m_conditionType)
		{
		default:
			foreach (NKMEventConditionDetail item2 in m_lstCondition)
			{
				if (item2.CheckCondition(cNKMGame, cNKMUnit, cUnitConditionOwner) == item2.Inverse)
				{
					return false;
				}
			}
			return true;
		case ConditionType.ANY:
			foreach (NKMEventConditionDetail item3 in m_lstCondition)
			{
				if (item3.CheckCondition(cNKMGame, cNKMUnit, cUnitConditionOwner) != item3.Inverse)
				{
					return true;
				}
			}
			return false;
		case ConditionType.NOT_ANY:
			foreach (NKMEventConditionDetail item4 in m_lstCondition)
			{
				if (item4.CheckCondition(cNKMGame, cNKMUnit, cUnitConditionOwner) == item4.Inverse)
				{
					return true;
				}
			}
			return false;
		case ConditionType.NOT_ALL:
			foreach (NKMEventConditionDetail item5 in m_lstCondition)
			{
				if (item5.CheckCondition(cNKMGame, cNKMUnit, cUnitConditionOwner) != item5.Inverse)
				{
					return false;
				}
			}
			return true;
		}
	}

	public static bool ValidateNKMMinMax(NKMMinMaxInt m_MinMax, string ErrorMsg)
	{
		if (m_MinMax.m_Min < 0 && m_MinMax.m_Max < 0)
		{
			NKMTempletError.Add(ErrorMsg, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 519);
			return false;
		}
		return true;
	}

	public static bool ValidateNKMMinMax(NKMMinMaxFloat m_MinMax, string ErrorMsg)
	{
		if (m_MinMax.m_Min < 0f && m_MinMax.m_Max < 0f)
		{
			NKMTempletError.Add(ErrorMsg, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 530);
			return false;
		}
		return true;
	}
}
