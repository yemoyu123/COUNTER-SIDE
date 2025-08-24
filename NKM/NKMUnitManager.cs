using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.User;
using ClientPacket.WorldMap;
using Cs.Logging;
using Cs.Math;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Unit;

namespace NKM;

public static class NKMUnitManager
{
	public const int SHIP_REAL_MAX_STAR_COUNT = 6;

	public static Dictionary<string, NKMUnitStatTemplet> m_dicNKMUnitStatTempletByStrID = null;

	public static Dictionary<int, NKMUnitStatTemplet> m_dicNKMUnitStatTempletByID = null;

	public static Dictionary<string, NKMUnitTemplet> m_dicNKMUnitTempletStrID = new Dictionary<string, NKMUnitTemplet>();

	public static Dictionary<int, NKMUnitTemplet> m_dicNKMUnitTempletID = new Dictionary<int, NKMUnitTemplet>();

	public static string m_UnitTempletFolderName = "";

	public static string m_ModuleUnitTempletFolderName = "";

	public static bool LoadFromLUA(string[] fileNames, string unitTempletFolderName, string moduleUnitTempletFolderName, bool bFullLoad = false)
	{
		m_dicNKMUnitStatTempletByStrID?.Clear();
		m_dicNKMUnitStatTempletByID?.Clear();
		m_dicNKMUnitTempletStrID.Clear();
		m_dicNKMUnitTempletID.Clear();
		m_UnitTempletFolderName = unitTempletFolderName;
		m_ModuleUnitTempletFolderName = moduleUnitTempletFolderName;
		return (byte)(1u & (LoadFromLUA_LUA_UNIT_STAT_TEMPLET(fileNames) ? 1u : 0u) & (LoadFromLUA_LUA_UNIT_TEMPLET(bFullLoad) ? 1u : 0u)) != 0;
	}

	private static bool LoadFromLUA_LUA_UNIT_STAT_TEMPLET(string[] fileNames, bool bReload = false)
	{
		if (bReload)
		{
			m_dicNKMUnitStatTempletByID?.Clear();
			m_dicNKMUnitStatTempletByStrID?.Clear();
		}
		m_dicNKMUnitStatTempletByID = NKMTempletLoader.LoadDictionary("AB_SCRIPT_UNIT_DATA", fileNames, "m_dicNKMUnitStatByID", NKMUnitStatTemplet.LoadFromLUA);
		m_dicNKMUnitStatTempletByStrID = m_dicNKMUnitStatTempletByID.ToDictionary((KeyValuePair<int, NKMUnitStatTemplet> e) => e.Value.m_UnitStrID, (KeyValuePair<int, NKMUnitStatTemplet> e) => e.Value);
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath("AB_SCRIPT_UNIT_DATA", fileNames[0]))
			{
				return false;
			}
			if (!nKMLua.OpenTable("m_dicNKMUnitStatByID"))
			{
				return false;
			}
		}
		return true;
	}

	private static bool LoadFromLUA_LUA_UNIT_TEMPLET(bool bFullLoad = false, bool bReload = false)
	{
		if (bFullLoad)
		{
			foreach (NKMUnitTempletBase value2 in NKMUnitTempletBase.Values)
			{
				if (value2.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SYSTEM && !LoadFromLUA_LUA_UNIT_TEMPLET(value2, bReload))
				{
					Log.ErrorAndExit($"[UnitTemplet] 유닛 상세 정보가 존재하지 않음 m_UnitID : {value2.m_UnitID}, m_UnitTempletFileName : {value2.m_UnitTempletFileName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 1015);
				}
			}
		}
		else
		{
			Dictionary<string, NKMUnitTemplet>.Enumerator enumerator2 = m_dicNKMUnitTempletStrID.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				NKMUnitTemplet value = enumerator2.Current.Value;
				if (value.m_UnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SYSTEM && !LoadFromLUA_LUA_UNIT_TEMPLET(value.m_UnitTempletBase, bReload))
				{
					Log.ErrorAndExit($"[UnitTemplet] 유닛 상세 정보가 존재하지 않음 m_UnitID : {value.m_UnitTempletBase.m_UnitID}, m_UnitTempletFileName : {value.m_UnitTempletBase.m_UnitTempletFileName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 1030);
				}
			}
		}
		return true;
	}

	private static bool LoadFromLUA_LUA_UNIT_TEMPLET(NKMUnitTempletBase cNKMUnitTempletBase, bool bReload = false)
	{
		if (cNKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			return true;
		}
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT_UNIT_DATA_UNIT_TEMPLET", m_UnitTempletFolderName + cNKMUnitTempletBase.m_UnitTempletFileName))
		{
			if (!nKMLua.OpenTable("NKMUnitTemplet"))
			{
				return false;
			}
			NKMUnitTemplet nKMUnitTemplet = null;
			NKMUnitTemplet nKMUnitTemplet2 = new NKMUnitTemplet();
			string rValue = "";
			nKMLua.GetData("BASE_UNIT_STR_ID", ref rValue);
			if (rValue.Length <= 1)
			{
				switch (cNKMUnitTempletBase.m_NKM_UNIT_TYPE)
				{
				case NKM_UNIT_TYPE.NUT_NORMAL:
					rValue = "NKM_UNIT_BASE_NORMAL";
					break;
				case NKM_UNIT_TYPE.NUT_SHIP:
					rValue = "NKM_UNIT_BASE_SHIP";
					break;
				}
			}
			if (rValue.Length > 1)
			{
				nKMUnitTemplet = GetUnitTemplet(rValue);
				if (nKMUnitTemplet != null)
				{
					nKMUnitTemplet2.DeepCopyFromSource(nKMUnitTemplet);
				}
				else
				{
					Log.Error(cNKMUnitTempletBase.m_UnitStrID + ": BASE_UNIT_STR_ID(" + rValue + ") baseTemplet null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 1076);
				}
			}
			if (nKMUnitTemplet2.LoadFromLUA(nKMLua, cNKMUnitTempletBase))
			{
				nKMUnitTemplet2.SetCoolTimeLink();
				if (!m_dicNKMUnitTempletID.ContainsKey(nKMUnitTemplet2.m_UnitTempletBase.m_UnitID))
				{
					m_dicNKMUnitTempletID.Add(nKMUnitTemplet2.m_UnitTempletBase.m_UnitID, nKMUnitTemplet2);
					if (!m_dicNKMUnitTempletStrID.ContainsKey(nKMUnitTemplet2.m_UnitTempletBase.m_UnitStrID))
					{
						m_dicNKMUnitTempletStrID.Add(nKMUnitTemplet2.m_UnitTempletBase.m_UnitStrID, nKMUnitTemplet2);
					}
					else if (bReload)
					{
						m_dicNKMUnitTempletStrID[nKMUnitTemplet2.m_UnitTempletBase.m_UnitStrID].DeepCopyFromSource(nKMUnitTemplet2);
					}
					else
					{
						Log.Error("m_dicNKMUnitTempletID Duplicate Key: " + nKMUnitTemplet2.m_UnitTempletBase.m_UnitStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 1099);
					}
				}
				else if (bReload)
				{
					m_dicNKMUnitTempletID[nKMUnitTemplet2.m_UnitTempletBase.m_UnitID].DeepCopyFromSource(nKMUnitTemplet2);
				}
				else if (nKMUnitTemplet != null && nKMUnitTemplet.m_UnitTempletBase.m_UnitID != nKMUnitTemplet2.m_UnitTempletBase.m_UnitID)
				{
					Log.Error("m_dicNKMUnitTempletID Duplicate Key: " + nKMUnitTemplet2.m_UnitTempletBase.m_UnitStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 1114);
				}
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		return true;
	}

	public static int GetUnitID(string unitStrID)
	{
		return NKMTempletContainer<NKMUnitTempletBase>.Find(unitStrID)?.m_UnitID ?? 0;
	}

	public static string GetUnitStrID(int unitID)
	{
		return NKMTempletContainer<NKMUnitTempletBase>.Find(unitID)?.m_UnitStrID;
	}

	public static NKMUnitTemplet GetUnitTemplet(int unitID)
	{
		if (!m_dicNKMUnitTempletID.ContainsKey(unitID))
		{
			NKMUnitTempletBase unitTempletBase = GetUnitTempletBase(unitID);
			if (unitTempletBase != null)
			{
				LoadFromLUA_LUA_UNIT_TEMPLET(unitTempletBase);
			}
		}
		if (m_dicNKMUnitTempletID.ContainsKey(unitID))
		{
			return m_dicNKMUnitTempletID[unitID];
		}
		return null;
	}

	public static NKMUnitTemplet GetUnitTemplet(string unitStrID)
	{
		if (!m_dicNKMUnitTempletStrID.ContainsKey(unitStrID))
		{
			NKMUnitTempletBase unitTempletBase = GetUnitTempletBase(unitStrID);
			if (unitTempletBase != null)
			{
				LoadFromLUA_LUA_UNIT_TEMPLET(unitTempletBase);
			}
		}
		if (m_dicNKMUnitTempletStrID.ContainsKey(unitStrID))
		{
			return m_dicNKMUnitTempletStrID[unitStrID];
		}
		return null;
	}

	public static List<NKMUnitTempletBase> GetListTeamUPUnitTempletBase(string TeamUp)
	{
		List<NKMUnitTempletBase> list = new List<NKMUnitTempletBase>();
		foreach (NKMUnitTempletBase value in NKMTempletContainer<NKMUnitTempletBase>.Values)
		{
			if (string.Equals(value.TeamUp, TeamUp))
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static NKMUnitTempletBase GetUnitTempletBase(NKMUnitData UnitData)
	{
		if (UnitData == null)
		{
			return null;
		}
		return GetUnitTempletBase(UnitData.m_UnitID);
	}

	public static NKMUnitTempletBase GetUnitTempletBase(NKMOperator operatorData)
	{
		if (operatorData == null)
		{
			return null;
		}
		return GetUnitTempletBase(operatorData.id);
	}

	public static NKMUnitTempletBase GetUnitTempletBase(int unitID)
	{
		return NKMTempletContainer<NKMUnitTempletBase>.Find(unitID);
	}

	public static NKMUnitTempletBase GetUnitTempletBase(string unitStrID)
	{
		return NKMTempletContainer<NKMUnitTempletBase>.Find(unitStrID);
	}

	public static NKMUnitStatTemplet GetUnitStatTemplet(int unitID)
	{
		m_dicNKMUnitStatTempletByID.TryGetValue(unitID, out var value);
		return value;
	}

	public static NKMUnitStatTemplet GetUnitStatTemplet(string unitStrID)
	{
		if (m_dicNKMUnitStatTempletByStrID.ContainsKey(unitStrID))
		{
			return m_dicNKMUnitStatTempletByStrID[unitStrID];
		}
		return null;
	}

	public static int GetUnitTempletCount()
	{
		return m_dicNKMUnitTempletStrID.Count;
	}

	public static NKM_ERROR_CODE IsUnitBusy(NKMUserData userdata, NKMUnitData unitData, bool ignoreWorldmapState = false)
	{
		switch (userdata.m_ArmyData.GetUnitDeckState(unitData.m_UnitUID))
		{
		case NKM_DECK_STATE.DECK_STATE_WORLDMAP_MISSION:
			if (ignoreWorldmapState)
			{
				return NKM_ERROR_CODE.NEC_OK;
			}
			return NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_DOING;
		case NKM_DECK_STATE.DECK_STATE_WARFARE:
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING;
		case NKM_DECK_STATE.DECK_STATE_DIVE:
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING;
		default:
			return NKM_ERROR_CODE.NEC_OK;
		}
	}

	public static NKM_ERROR_CODE GetCanDeleteUnit(NKMUnitData cUnitData, NKMUserData cUserData, NKMSupportUnitData supportUnitData = null)
	{
		NKMArmyData armyData = cUserData.m_ArmyData;
		if (NKMMain.excludeUnitID.Contains(cUnitData.m_UnitID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DELETE_EXCLUDE_UNIT;
		}
		if (!NKMOpenTagManager.IsOpened("TAG_DELETE_YOO_MI_NA") && cUnitData.m_UnitID == 1001)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DELETE_EXCLUDE_UNIT;
		}
		if (!NKMOpenTagManager.IsOpened("TAG_DELETE_TEAM_FENRIR") && cUnitData.m_UnitID == 1002)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DELETE_EXCLUDE_UNIT;
		}
		if (!NKMOpenTagManager.IsOpened("TAG_DELETE_JOO_SHI_YOON") && cUnitData.m_UnitID == 1003)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DELETE_EXCLUDE_UNIT;
		}
		if (cUnitData.m_bLock)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_LOCKED;
		}
		if (cUserData.backGroundInfo.unitInfoList.Find((NKMBackgroundUnitInfo e) => e.unitUid == cUnitData.m_UnitUID) != null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_LOBBYUNIT;
		}
		if (armyData.IsUnitInAnyDeck(cUnitData.m_UnitUID))
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IN_DECK;
		}
		if (cUnitData.GetEquipItemAccessoryUid() != 0L || cUnitData.GetEquipItemDefenceUid() != 0L || cUnitData.GetEquipItemWeaponUid() != 0L || cUnitData.GetEquipItemAccessory2Uid() != 0L)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_EQUIP_ITEM;
		}
		foreach (NKMWorldMapCityData value in cUserData.m_WorldmapData.worldMapCityDataMap.Values)
		{
			if (value.leaderUnitUID == cUnitData.m_UnitUID)
			{
				return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_WORLDMAP_LEADER;
			}
		}
		if (cUnitData.OfficeRoomId > 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_UNIT_DELETE_IN_ROOM;
		}
		if (supportUnitData != null && supportUnitData.asyncUnitEquip != null && (supportUnitData.asyncUnitEquip.asyncUnit?.unitUid ?? 0) == cUnitData.m_UnitUID)
		{
			return NKM_ERROR_CODE.NEC_FAIL_CONTAIN_SUPPORT_UNIT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static bool CanUnitUsedInDeck(NKMUnitData unitData)
	{
		return CanUnitUsedInDeck(GetUnitTempletBase(unitData));
	}

	public static bool CanUnitUsedInDeck(int unitID)
	{
		return CanUnitUsedInDeck(GetUnitTempletBase(unitID));
	}

	public static bool CanUnitUsedInDeck(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase == null)
		{
			return false;
		}
		return unitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER;
	}

	public static bool IsShipType(NKM_UNIT_STYLE_TYPE type)
	{
		if ((uint)(type - 7) <= 5u)
		{
			return true;
		}
		return false;
	}

	public static bool CheckContainsBaseUnit(IEnumerable<int> lstUnitID, int unitID)
	{
		if (lstUnitID == null)
		{
			return false;
		}
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitID);
		if (nKMUnitTempletBase == null)
		{
			return lstUnitID.Contains(unitID);
		}
		foreach (int item in lstUnitID)
		{
			if (nKMUnitTempletBase.IsSameBaseUnit(item))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckContainsBaseUnit(IEnumerable<int> lstUnitID, NKMUnitTempletBase unitTempletBase)
	{
		if (lstUnitID == null)
		{
			return false;
		}
		if (unitTempletBase == null)
		{
			return false;
		}
		foreach (int item in lstUnitID)
		{
			if (unitTempletBase.IsSameBaseUnit(item))
			{
				return true;
			}
		}
		return false;
	}

	public static void CheckValidation()
	{
		int num = 0;
		foreach (NKMUnitTemplet value in m_dicNKMUnitTempletStrID.Values)
		{
			if (value.m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SYSTEM || value.m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
			{
				continue;
			}
			foreach (NKMUnitState value2 in value.m_dicNKMUnitState.Values)
			{
				if (!string.IsNullOrEmpty(value2.m_AnimName) && NKMAnimDataManager.GetAnimTimeMax(value.m_UnitTempletBase.m_SpriteBundleName, value.m_UnitTempletBase.m_SpriteName, value2.m_AnimName).IsNearlyZero())
				{
					num++;
					Log.Error("[UnitTemplet] 유닛 상태에 따른 애니메이션이 존재하지 않음 UnitStrID : " + value.m_UnitTempletBase.m_UnitStrID + ", StateName : " + value2.m_StateName + ", AniName : " + value2.m_AnimName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 1439);
				}
				foreach (NKMEventDamageEffect item in value2.m_listNKMEventDamageEffect)
				{
					if (NKMDETempletManager.GetDETemplet(item.m_DEName) == null)
					{
						num++;
						Log.Error("[UnitTemplet] 유닛 상태에 따른 데미지 이펙트가 존재하지 않음 UnitStrID : " + value.m_UnitTempletBase.m_UnitStrID + ", StateName : " + value2.m_StateName + ", DEName : " + item.m_DEName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 1447);
					}
					if (item.m_DENamePVP.Length > 1 && NKMDETempletManager.GetDETemplet(item.m_DENamePVP) == null)
					{
						num++;
						Log.Error("[UnitTemplet] 유닛 상태에 따른 데미지 이펙트(m_DENamePVP)가 존재하지 않음 UnitStrID : " + value.m_UnitTempletBase.m_UnitStrID + ", StateName : " + value2.m_StateName + ", DEName : " + item.m_DENamePVP, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 1455);
					}
				}
			}
		}
		if (num > 0)
		{
			Log.ErrorAndExit($"[UnitTemplet] 유닛 스크립트 정합성 체크에 실패 했습니다. invalidCount : {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 1464);
		}
	}

	public static (NKM_ERROR_CODE errorCode, NKMUnitData unitData) CreateUnitData(int unitId, long unitUid, int unitLevel, int limitBreakLevel, int skillLevel, int reactor, int tacticLevel, int loyalty, bool fromContract, int skinId = 0)
	{
		NKMUnitTempletBase unitTempletBase = GetUnitTempletBase(unitId);
		if (unitTempletBase == null)
		{
			return (errorCode: NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_BASE_TEMPLET_NULL, unitData: null);
		}
		if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP || unitTempletBase.m_bMonster)
		{
			return (errorCode: NKM_ERROR_CODE.NEC_FAIL_UNIT_INVALID_UNIT_ID, unitData: null);
		}
		NKMUnitData nKMUnitData = new NKMUnitData(unitId, unitUid, islock: false, isPermanentContract: false, isSeized: false, fromContract);
		loyalty = Math.Min(10000, Math.Max(0, loyalty));
		nKMUnitData.loyalty = loyalty;
		if (tacticLevel > 6)
		{
			return (errorCode: NKM_ERROR_CODE.NEC_FAIL_UNIT_TACTIC_ALREADY_MAX_LEVEL, unitData: null);
		}
		nKMUnitData.tacticLevel = tacticLevel;
		nKMUnitData.m_LimitBreakLevel = 0;
		bool flag = false;
		if (limitBreakLevel > 0)
		{
			nKMUnitData.m_LimitBreakLevel = (short)limitBreakLevel;
		}
		if (skillLevel > 0)
		{
			foreach (string item in unitTempletBase.m_lstSkillStrID)
			{
				int skillIndex = unitTempletBase.GetSkillIndex(item);
				int skillID = NKMUnitSkillManager.GetSkillID(item);
				int maxSkillLevelFromLimitBreakLevel = NKMUnitSkillManager.GetMaxSkillLevelFromLimitBreakLevel(skillID, limitBreakLevel);
				int num = skillLevel;
				num = ((num > maxSkillLevelFromLimitBreakLevel) ? maxSkillLevelFromLimitBreakLevel : skillLevel);
				NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(skillID, num);
				if (skillTemplet == null)
				{
					return (errorCode: NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_TEMPLET_NOT_EXIST, unitData: null);
				}
				if (NKMUnitSkillManager.GetSkillTemplet(skillID, num + 1) != null)
				{
					flag = true;
				}
				if (NKMUnitSkillManager.IsLockedSkill(skillTemplet.m_ID, limitBreakLevel))
				{
					nKMUnitData.m_aUnitSkillLevel[skillIndex] = ((skillTemplet.m_Level - 1 <= 0) ? 1 : (skillTemplet.m_Level - 1));
				}
				else
				{
					nKMUnitData.m_aUnitSkillLevel[skillIndex] = skillTemplet.m_Level;
				}
			}
		}
		if (unitLevel > 1)
		{
			int num2 = 0;
			num2 = NKCExpManager.GetUnitMaxLevel(unitTempletBase, nKMUnitData.m_LimitBreakLevel);
			if (unitLevel > num2)
			{
				unitLevel = num2;
			}
			NKMUnitExpTemplet nKMUnitExpTemplet = unitTempletBase.ExpTable?.Get(unitLevel);
			if (nKMUnitExpTemplet == null)
			{
				return (errorCode: NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_EXP_TEMPLET_NULL, unitData: null);
			}
			int iExpCumulated = nKMUnitExpTemplet.m_iExpCumulated;
			(nKMUnitData.m_UnitLevel, nKMUnitData.m_iUnitLevelEXP) = NKCExpManager.CalcUnitTotalExp(unitTempletBase, nKMUnitData.m_LimitBreakLevel, iExpCumulated);
		}
		if (reactor > 0 && unitTempletBase.IsReactorUnit)
		{
			NKMUnitReactorTemplet nKMUnitReactorTemplet = NKMUnitReactorTemplet.Find(unitTempletBase.m_ReactorId);
			if (nKMUnitReactorTemplet == null)
			{
				reactor = 0;
			}
			else
			{
				if (!nKMUnitReactorTemplet.EnableByTag || flag || reactor > NKMCommonConst.ReactorMaxLevel)
				{
					reactor = 0;
				}
				if (reactor > 0)
				{
					int maxReactorLevel = nKMUnitReactorTemplet.GetMaxReactorLevel();
					reactor = ((reactor > maxReactorLevel) ? maxReactorLevel : reactor);
					NKMReactorSkillTemplet skillTemplets = nKMUnitReactorTemplet.GetSkillTemplets(reactor);
					if (skillTemplets == null)
					{
						return (errorCode: NKM_ERROR_CODE.NEC_FAIL_UNIT_REACTOR_INVALID_SKILL_TEMPLET, unitData: null);
					}
					if (!skillTemplets.EnableByTag)
					{
						reactor = 0;
					}
				}
			}
		}
		nKMUnitData.reactorLevel = reactor;
		nKMUnitData.m_SkinID = skinId;
		return (errorCode: NKM_ERROR_CODE.NEC_OK, unitData: nKMUnitData);
	}

	public static (NKM_ERROR_CODE errorCode, NKMUnitData unitData) CreateShipData(int unitId, long unitUid, int unitLevel, int limitBreakLevel)
	{
		if (!TryGetShipTempletByUnitLevel(unitId, unitLevel, limitBreakLevel, out var result))
		{
			return (errorCode: NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_BASE_TEMPLET_NULL, unitData: null);
		}
		if (result.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP || result.m_bMonster)
		{
			return (errorCode: NKM_ERROR_CODE.NEC_FAIL_UNIT_INVALID_UNIT_ID, unitData: null);
		}
		if (result.m_ShipGroupID == 0)
		{
			return (errorCode: NKM_ERROR_CODE.NEC_FAIL_SHIP_INVALID_SHIP_ID, unitData: null);
		}
		unitId = result.m_UnitID;
		NKMUnitData nKMUnitData = new NKMUnitData(unitId, unitUid, islock: false, isPermanentContract: false, isSeized: false, fromContract: false);
		int shipMinLimitBreakLevel = NKMShipLevelUpTemplet.GetShipMinLimitBreakLevel(unitLevel, result.m_NKM_UNIT_GRADE);
		limitBreakLevel = ((limitBreakLevel != shipMinLimitBreakLevel) ? shipMinLimitBreakLevel : limitBreakLevel);
		NKMShipLevelUpTemplet shipLevelupTempletByLevel = NKMShipManager.GetShipLevelupTempletByLevel(unitLevel, result.m_NKM_UNIT_GRADE, limitBreakLevel);
		if (shipLevelupTempletByLevel == null)
		{
			return (errorCode: NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_BASE_TEMPLET_NULL, unitData: null);
		}
		unitLevel = ((unitLevel > shipLevelupTempletByLevel.ShipMaxLevel) ? shipLevelupTempletByLevel.ShipMaxLevel : unitLevel);
		nKMUnitData.m_UnitLevel = unitLevel;
		nKMUnitData.m_LimitBreakLevel = (short)limitBreakLevel;
		for (int i = 0; i < limitBreakLevel; i++)
		{
			nKMUnitData.ShipCommandModule.Add(new NKMShipCmdModule());
		}
		return (errorCode: NKM_ERROR_CODE.NEC_OK, unitData: nKMUnitData);
		static bool TryGetShipTempletByUnitLevel(int num, int level, int limitBreak, out NKMUnitTempletBase reference)
		{
			Dictionary<int, List<NKMUnitTempletBase>> dictionary = new Dictionary<int, List<NKMUnitTempletBase>>();
			foreach (NKMUnitTempletBase item in NKMUnitTempletBase.Values.Where((NKMUnitTempletBase e) => e.IsShip()))
			{
				int shipGroupID = item.m_ShipGroupID;
				if (!dictionary.TryGetValue(shipGroupID, out var value))
				{
					value = new List<NKMUnitTempletBase>();
					dictionary.Add(shipGroupID, value);
				}
				value.Add(item);
			}
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(num);
			if (nKMUnitTempletBase == null || !nKMUnitTempletBase.IsShip())
			{
				Log.Error($"[ShipGroup] invalid unitId:{num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 1705);
				reference = null;
				return false;
			}
			int shipMinLimitBreakLevel2 = NKMShipLevelUpTemplet.GetShipMinLimitBreakLevel(level, nKMUnitTempletBase.m_NKM_UNIT_GRADE);
			limitBreak = ((limitBreak != shipMinLimitBreakLevel2) ? shipMinLimitBreakLevel2 : limitBreak);
			if (!dictionary.TryGetValue(nKMUnitTempletBase.m_ShipGroupID, out var _))
			{
				Log.Error($"[ShipGroup] invalid shipGroupId:{nKMUnitTempletBase.m_ShipGroupID} ship:{nKMUnitTempletBase.DebugName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 1715);
				reference = null;
				return false;
			}
			NKMShipLevelUpTemplet shipLevelUpTemplet = NKMShipManager.GetShipLevelupTempletByLevel(level, nKMUnitTempletBase.m_NKM_UNIT_GRADE, limitBreak);
			reference = dictionary[nKMUnitTempletBase.m_ShipGroupID].Where((NKMUnitTempletBase e) => e.m_StarGradeMax == shipLevelUpTemplet.ShipStarGrade).FirstOrDefault();
			if (reference == null)
			{
				reference = null;
				return false;
			}
			return true;
		}
	}

	public static bool ReloadFromLUA()
	{
		string[] fileNames = new string[4] { "LUA_UNIT_TEMPLET_BASE", "LUA_UNIT_TEMPLET_BASE2", "LUA_UNIT_TEMPLET_BASE_SD", "LUA_UNIT_TEMPLET_BASE_OPR" };
		NKMTempletContainer<NKMUnitTempletBase>.Load("AB_SCRIPT_UNIT_DATA", fileNames, "m_dicNKMUnitTempletBaseByStrID", NKMUnitTempletBase.LoadFromLUA, (NKMUnitTempletBase e) => e.m_UnitStrID);
		fileNames = new string[4] { "LUA_UNIT_STAT_TEMPLET", "LUA_UNIT_STAT_TEMPLET2", "LUA_UNIT_STAT_TEMPLET_SD", "LUA_UNIT_STAT_TEMPLET_OPR" };
		LoadFromLUA_LUA_UNIT_STAT_TEMPLET(fileNames, bReload: true);
		LoadFromLUA_LUA_UNIT_TEMPLET(bFullLoad: false, bReload: true);
		NKMTempletContainer<NKMUnitTempletBase>.Join();
		ValidateLoadedUnitTemplet();
		return true;
	}

	public static NKMUnitTempletBase GetUnitTempletBaseByShipGroupID(int groupID)
	{
		if (groupID == 0)
		{
			return null;
		}
		return NKMTempletContainer<NKMUnitTempletBase>.Find((NKMUnitTempletBase e) => e.m_ShipGroupID == groupID);
	}

	public static NKM_ERROR_CODE GetCanDeleteOperator(NKMOperator operatorData, NKMUserData cUserData)
	{
		NKMArmyData armyData = cUserData.m_ArmyData;
		if (operatorData.bLock)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_LOCKED;
		}
		if (cUserData.GetBackgroundUnitIndex(operatorData.uid) >= 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_LOBBYUNIT;
		}
		if (armyData.IsOperatorAnyDeck(operatorData.uid))
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IN_DECK;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static void ValidateLoadedUnitTemplet()
	{
		foreach (NKMUnitTemplet value in m_dicNKMUnitTempletID.Values)
		{
			value.Validate();
		}
	}
}
