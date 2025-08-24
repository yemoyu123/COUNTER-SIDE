using ClientPacket.Common;
using ClientPacket.Mode;
using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.Trim;

public static class NKCTrimManager
{
	public static TrimModeState TrimModeState { get; private set; }

	public static bool GiveUpState { get; private set; }

	public static void Reset()
	{
		ClearTrimModeState();
	}

	public static void SetGiveUpState(bool value)
	{
		GiveUpState = value;
	}

	public static void SetTrimModeState(TrimModeState state)
	{
		TrimModeState = state;
	}

	public static void ClearTrimModeState()
	{
		TrimModeState = null;
		GiveUpState = false;
	}

	public static NKMTrimTemplet GetTrimTemplet()
	{
		if (TrimModeState == null)
		{
			return null;
		}
		return NKMTrimTemplet.Find(TrimModeState.trimId);
	}

	public static NKMTrimStageData GetFinishedTrimStageData(int index)
	{
		if (TrimModeState == null)
		{
			return null;
		}
		if (TrimModeState.lastClearStage.index == index)
		{
			return TrimModeState.lastClearStage;
		}
		if (TrimModeState.stageList == null)
		{
			return null;
		}
		return TrimModeState.stageList.Find((NKMTrimStageData x) => x.index == index);
	}

	public static int GetLastClearedTrimIndex(this TrimModeState trimState)
	{
		if (trimState.lastClearStage != null)
		{
			return trimState.lastClearStage.index;
		}
		if (trimState.stageList != null)
		{
			int num = -1;
			{
				foreach (NKMTrimStageData stage in trimState.stageList)
				{
					num = Mathf.Max(num, stage.index);
				}
				return num;
			}
		}
		return -1;
	}

	public static bool ProcessTrim()
	{
		if (TrimModeState == null)
		{
			return false;
		}
		if (ShouldPlayNextTrim())
		{
			PlayNextTrim();
			return true;
		}
		NKCPacketSender.Send_NKMPacket_TRIM_END_REQ(TrimModeState.trimId);
		return true;
	}

	private static bool ShouldPlayNextTrim()
	{
		if (TrimModeState.trimId == 0)
		{
			Log.Error("TrimModeState trimid 0?", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCTrimManager.cs", 102);
			return false;
		}
		if (TrimModeState.lastClearStage == null)
		{
			return true;
		}
		int num = TrimModeState.lastClearStage.index + 1;
		if (num > 3)
		{
			return false;
		}
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(TrimModeState.trimId);
		if (nKMTrimTemplet != null && num >= nKMTrimTemplet.TrimDungeonIds.Length)
		{
			return false;
		}
		if (TrimModeState.nextDungeonId <= 0)
		{
			return false;
		}
		return true;
	}

	public static bool WillPlayTrimDungeonCutscene(int trimID, int dungeonID, int level)
	{
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(trimID);
		if (nKMTrimTemplet == null)
		{
			return false;
		}
		NKMTrimDungeonTemplet trimDungeonTempletByDungeonID = nKMTrimTemplet.GetTrimDungeonTempletByDungeonID(TrimModeState.nextDungeonId, TrimModeState.trimLevel);
		if (trimDungeonTempletByDungeonID == null)
		{
			return false;
		}
		if (!trimDungeonTempletByDungeonID.m_bShowCutScene)
		{
			return false;
		}
		if (trimDungeonTempletByDungeonID.TrimLevelLow != level)
		{
			return false;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (NKCScenManager.CurrentUserData() != null && NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene)
		{
			return true;
		}
		return nKMUserData.TrimData.GetTrimClearData(TrimModeState.trimId, TrimModeState.trimLevel) == null;
	}

	private static bool PlayNextTrim()
	{
		NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(0, 0, 0, TrimModeState.nextDungeonId, 0, bLocal: false, 1, 0, 0L);
		return true;
	}
}
