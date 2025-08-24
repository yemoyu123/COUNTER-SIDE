using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Mode;
using Cs.Logging;
using NKC.UI;
using NKM;
using NKM.Templet;

namespace NKC;

public static class NKCPhaseManager
{
	private static int LastStageID = 0;

	private static Dictionary<int, NKMPhaseClearData> m_dicPhaseStageClearData = new Dictionary<int, NKMPhaseClearData>();

	private static HashSet<int> m_sHsFirstClearStage = new HashSet<int>();

	private static Dictionary<(NKM_TEAM_TYPE, int, bool, bool), NKMUnitData> s_dicTempUnitData = new Dictionary<(NKM_TEAM_TYPE, int, bool, bool), NKMUnitData>();

	public static PhaseModeState PhaseModeState { get; private set; }

	public static void SetPhaseModeState(PhaseModeState state)
	{
		if (state == null || state.stageId == 0)
		{
			LastStageID = ((PhaseModeState != null) ? PhaseModeState.stageId : 0);
		}
		PhaseModeState = state;
	}

	public static void Reset()
	{
		PhaseModeState = null;
		LastStageID = 0;
		m_dicPhaseStageClearData.Clear();
	}

	public static int GetLastStageID()
	{
		if (PhaseModeState == null || PhaseModeState.stageId == 0)
		{
			return LastStageID;
		}
		return PhaseModeState.stageId;
	}

	public static NKMStageTempletV2 GetStageTemplet()
	{
		if (PhaseModeState == null)
		{
			return null;
		}
		return NKMStageTempletV2.Find(PhaseModeState.stageId);
	}

	public static NKMPhaseTemplet GetPhaseTemplet()
	{
		return GetStageTemplet()?.PhaseTemplet;
	}

	public static NKMPhaseOrderTemplet GetPhaseOrderTemplet()
	{
		if (PhaseModeState == null)
		{
			return null;
		}
		return GetPhaseTemplet()?.GetPhase(PhaseModeState.phaseIndex);
	}

	public static void SetPhaseClearDataList(List<NKMPhaseClearData> list)
	{
		m_dicPhaseStageClearData.Clear();
		foreach (NKMPhaseClearData item in list)
		{
			m_dicPhaseStageClearData.Add(item.stageId, item);
		}
	}

	public static void UpdateClearData(NKMPhaseClearData _NKMPhaseClearData)
	{
		if (_NKMPhaseClearData != null)
		{
			if (!m_dicPhaseStageClearData.ContainsKey(_NKMPhaseClearData.stageId))
			{
				m_sHsFirstClearStage.Add(_NKMPhaseClearData.stageId);
			}
			m_dicPhaseStageClearData[_NKMPhaseClearData.stageId] = _NKMPhaseClearData;
		}
	}

	public static bool WasPhaseStageFirstClear(int stageID)
	{
		if (m_sHsFirstClearStage.Contains(stageID))
		{
			m_sHsFirstClearStage.Remove(stageID);
			return true;
		}
		return false;
	}

	public static NKMPhaseClearData GetPhaseClearData(NKMStageTempletV2 templet)
	{
		if (templet == null)
		{
			return null;
		}
		if (m_dicPhaseStageClearData.TryGetValue(templet.Key, out var value))
		{
			return value;
		}
		return null;
	}

	public static NKMPhaseClearData GetPhaseClearData(NKMPhaseTemplet templet)
	{
		if (templet == null)
		{
			return null;
		}
		if (templet.StageTemplet == null)
		{
			return null;
		}
		if (m_dicPhaseStageClearData.TryGetValue(templet.StageTemplet.Key, out var value))
		{
			return value;
		}
		return null;
	}

	public static bool CheckPhaseClear(int phaseID)
	{
		NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(phaseID);
		if (nKMPhaseTemplet == null)
		{
			return false;
		}
		return CheckPhaseStageClear(nKMPhaseTemplet);
	}

	public static bool CheckPhaseClear(NKMPhaseTemplet phaseTemplet)
	{
		if (phaseTemplet == null)
		{
			return false;
		}
		return CheckPhaseStageClear(phaseTemplet);
	}

	public static bool CheckPhaseStageClear(int stageID)
	{
		return m_dicPhaseStageClearData.ContainsKey(stageID);
	}

	public static bool CheckPhaseStageClear(NKMStageTempletV2 templet)
	{
		if (templet == null)
		{
			return false;
		}
		return CheckPhaseStageClear(templet.Key);
	}

	public static bool CheckPhaseStageClear(NKMPhaseTemplet templet)
	{
		if (templet == null)
		{
			return false;
		}
		if (templet.StageTemplet == null)
		{
			return false;
		}
		return CheckPhaseStageClear(templet.StageTemplet.Key);
	}

	public static bool IsPhaseOnGoing()
	{
		if (PhaseModeState != null)
		{
			return PhaseModeState.stageId != 0;
		}
		return false;
	}

	public static bool IsCurrentPhaseDungeon(int dungeonID)
	{
		NKMPhaseTemplet phaseTemplet = GetPhaseTemplet();
		if (phaseTemplet == null)
		{
			return false;
		}
		foreach (NKMPhaseOrderTemplet item in phaseTemplet.PhaseList.List)
		{
			if (item.Dungeon.m_DungeonID == dungeonID)
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckOneTimeReward(NKMStageTempletV2 stageTemplet, int index)
	{
		if (index < 0)
		{
			return false;
		}
		NKMPhaseClearData phaseClearData = GetPhaseClearData(stageTemplet);
		if (phaseClearData != null)
		{
			if (phaseClearData.onetimeRewardResults == null || phaseClearData.onetimeRewardResults.Count < index)
			{
				return false;
			}
			return phaseClearData.onetimeRewardResults[index];
		}
		return false;
	}

	public static bool ShouldPlayNextPhase()
	{
		if (PhaseModeState == null)
		{
			return false;
		}
		if (PhaseModeState.stageId == 0)
		{
			return false;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(PhaseModeState.stageId);
		if (nKMStageTempletV == null)
		{
			Log.Error($"NKMStageTemplet not found! Stage ID : {PhaseModeState.stageId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCPhaseManager.cs", 238);
			return false;
		}
		if (nKMStageTempletV.PhaseTemplet == null)
		{
			Log.Error($"NKMPhaseTemplet not found! Stage ID : {PhaseModeState.stageId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCPhaseManager.cs", 244);
			return false;
		}
		if (nKMStageTempletV.PhaseTemplet.GetPhase(PhaseModeState.phaseIndex) == null)
		{
			Log.Error($"NKMPhaseOrderTemplet not found! Phase ID : {nKMStageTempletV.PhaseTemplet.Id}, index : {PhaseModeState.phaseIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCPhaseManager.cs", 251);
			return false;
		}
		if (NKMDungeonManager.GetDungeonTempletBase(PhaseModeState.dungeonId) == null)
		{
			Log.Error($"Dungeon templet not found! ID : {PhaseModeState.dungeonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCPhaseManager.cs", 258);
			return false;
		}
		return true;
	}

	public static bool PlayNextPhase()
	{
		if (!ShouldPlayNextPhase())
		{
			return false;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(PhaseModeState.dungeonId);
		NKCUICutScenPlayer.CutScenCallBack cutScenCallBack = null;
		cutScenCallBack = delegate
		{
			NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(0, PhaseModeState.stageId, 0, PhaseModeState.dungeonId, 0, bLocal: false, 1, 0, PhaseModeState.supportingUserUid);
		};
		NKMStageTempletV2 stageTemplet = GetStageTemplet();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool flag = false;
		bool flag2 = true;
		if (NKCScenManager.CurrentUserData() != null)
		{
			flag2 = NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene;
		}
		bool isOnGoing = NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing();
		if (!nKMUserData.CheckStageCleared(stageTemplet) || (flag2 && !isOnGoing))
		{
			flag = true;
		}
		NKCCutScenTemplet cutScenTemple = NKCCutScenManager.GetCutScenTemple(dungeonTempletBase.m_CutScenStrIDBefore);
		if (flag && cutScenTemple != null)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedOneCutscenType(cutScenTemple.m_CutScenStrID, cutScenCallBack);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
		}
		else
		{
			cutScenCallBack();
		}
		return true;
	}

	public static bool IsFirstStage(int dungeonID)
	{
		NKMPhaseTemplet phaseTemplet = GetPhaseTemplet();
		if (phaseTemplet == null)
		{
			return false;
		}
		NKMPhaseOrderTemplet phase = phaseTemplet.GetPhase(0);
		if (phase == null)
		{
			return false;
		}
		return dungeonID == phase.Dungeon.m_DungeonID;
	}

	public static bool IsLastStage(int dungeonID)
	{
		NKMPhaseTemplet phaseTemplet = GetPhaseTemplet();
		if (phaseTemplet == null)
		{
			return true;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
		return phaseTemplet.IsLastPhase(dungeonTempletBase);
	}

	public static void SaveTempUnitData(NKMGame nkmGame, NKMGameRecord gameRecord)
	{
		if (nkmGame == null || gameRecord == null)
		{
			return;
		}
		foreach (KeyValuePair<short, NKMGameRecordUnitData> unitRecord in gameRecord.UnitRecordList)
		{
			_ = unitRecord.Key;
			NKMGameRecordUnitData value = unitRecord.Value;
			NKMUnit unit = nkmGame.GetUnit(unitRecord.Key, bChain: true, bPool: true);
			if (unit != null)
			{
				NKMUnitData unitData = unit.GetUnitData();
				if (unitData != null && !s_dicTempUnitData.ContainsKey((value.teamType, value.unitId, value.isSummonee, value.isAssistUnit)))
				{
					s_dicTempUnitData.Add((value.teamType, value.unitId, value.isSummonee, value.isAssistUnit), unitData);
				}
			}
		}
	}

	public static NKMUnitData GetTempUnitData(NKMGameRecordUnitData gameRecordUnitData)
	{
		if (s_dicTempUnitData.TryGetValue((gameRecordUnitData.teamType, gameRecordUnitData.unitId, gameRecordUnitData.isSummonee, gameRecordUnitData.isAssistUnit), out var value))
		{
			return value;
		}
		return null;
	}

	public static void ClearTempUnitData()
	{
		s_dicTempUnitData.Clear();
	}
}
