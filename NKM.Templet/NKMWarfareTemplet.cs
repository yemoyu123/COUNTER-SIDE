using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using Cs.Math;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMWarfareTemplet : INKMTemplet
{
	public sealed class RewardData
	{
		internal int m_RewardGroupID;

		public int m_RewardRate;

		public NKMRewardGroupTemplet Templet { get; internal set; }
	}

	public const int MAX_REWARD_COUNT = 10;

	public int m_WarfareID;

	public string m_WarfareStrID = "";

	private string m_WarfareName = "";

	private string m_WarfareMapStrID = "";

	public int m_UserTeamCount = 1;

	public string m_WarfareBG_Stop = "";

	public string m_WarfareBG_Playing = "";

	public string m_WarfareBGM = "";

	public string m_WarfareIcon = "";

	public int m_WarfareLevel;

	public int m_fWarfareTimeMax;

	public int m_WFRecommendFightPower;

	public int m_WFLimitUserLevel;

	public string m_CutScenStrIDBefore = "";

	public string m_CutScenStrIDAfter = "";

	public WARFARE_GAME_CONDITION m_WFWinCondition = WARFARE_GAME_CONDITION.WFC_KILL_BOSS;

	public int m_WFWinValue;

	public WARFARE_GAME_CONDITION m_WFLoseCondition = WARFARE_GAME_CONDITION.WFC_KILL_ALL;

	public int m_WFLoseValue;

	public WARFARE_GAME_MISSION_TYPE m_WFMissionType_1;

	public int m_WFMissionValue_1;

	public WARFARE_GAME_MISSION_TYPE m_WFMissionType_2;

	public int m_WFMissionValue_2;

	public int m_RewardUserEXP;

	public int m_RewardCredit_Min;

	public int m_RewardCredit_Max;

	public int m_RewardEternium_Min;

	public int m_RewardEternium_Max;

	public int m_RewardMultiplyMax = 1;

	public readonly List<RewardData> RewardList = new List<RewardData>();

	private int m_ContainerGroupID;

	public bool m_bFriendSummon;

	private readonly Lazy<NKMWarfareMapTemplet> mapTemplet;

	public NKMWarfareMapTemplet MapTemplet => mapTemplet.Value;

	public NKMStageTempletV2 StageTemplet { get; internal set; }

	public NKMRewardGroupTemplet ContainerRewardTemplet { get; internal set; }

	public bool IsArrivalGame
	{
		get
		{
			if (m_WFWinCondition != WARFARE_GAME_CONDITION.WFC_TILE_ENTER)
			{
				return m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD;
			}
			return true;
		}
	}

	public bool IsTileDefenceGame
	{
		get
		{
			if (m_WFLoseCondition != WARFARE_GAME_CONDITION.WFC_TILE_ENTER)
			{
				return m_WFLoseCondition == WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD;
			}
			return true;
		}
	}

	public static IEnumerable<NKMWarfareTemplet> Values => NKMTempletContainer<NKMWarfareTemplet>.Values;

	public int Key => m_WarfareID;

	public string WarfareName => m_WarfareName;

	public NKMWarfareTemplet()
	{
		mapTemplet = new Lazy<NKMWarfareMapTemplet>(() => NKMWarfareMapContainer.GetOrLoad(m_WarfareMapStrID));
	}

	public static NKMWarfareTemplet Find(int key)
	{
		return NKMTempletContainer<NKMWarfareTemplet>.Find(key);
	}

	public static NKMWarfareTemplet Find(string key)
	{
		return NKMTempletContainer<NKMWarfareTemplet>.Find(key);
	}

	public static NKMWarfareTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 167))
		{
			return null;
		}
		NKMWarfareTemplet nKMWarfareTemplet = new NKMWarfareTemplet();
		cNKMLua.GetData("m_WarfareID", ref nKMWarfareTemplet.m_WarfareID);
		cNKMLua.GetData("m_WarfareStrID", ref nKMWarfareTemplet.m_WarfareStrID);
		cNKMLua.GetData("m_WarfareName", ref nKMWarfareTemplet.m_WarfareName);
		cNKMLua.GetData("m_WarfareMapStrID", ref nKMWarfareTemplet.m_WarfareMapStrID);
		cNKMLua.GetData("m_UserTeamCount", ref nKMWarfareTemplet.m_UserTeamCount);
		cNKMLua.GetData("m_WarfareBG_Stop", ref nKMWarfareTemplet.m_WarfareBG_Stop);
		cNKMLua.GetData("m_WarfareBG_Playing", ref nKMWarfareTemplet.m_WarfareBG_Playing);
		cNKMLua.GetData("m_WarfareBGM", ref nKMWarfareTemplet.m_WarfareBGM);
		cNKMLua.GetData("m_WarfareIcon", ref nKMWarfareTemplet.m_WarfareIcon);
		cNKMLua.GetData("m_WarfareLevel", ref nKMWarfareTemplet.m_WarfareLevel);
		cNKMLua.GetData("m_fWarfareTimeMax", ref nKMWarfareTemplet.m_fWarfareTimeMax);
		cNKMLua.GetData("m_WFRecommendFightPower", ref nKMWarfareTemplet.m_WFRecommendFightPower);
		cNKMLua.GetData("m_WFLimitUserLevel", ref nKMWarfareTemplet.m_WFLimitUserLevel);
		cNKMLua.GetData("m_CutScenStrIDBefore", ref nKMWarfareTemplet.m_CutScenStrIDBefore);
		cNKMLua.GetData("m_CutScenStrIDAfter", ref nKMWarfareTemplet.m_CutScenStrIDAfter);
		cNKMLua.GetData("m_WFWinCondition", ref nKMWarfareTemplet.m_WFWinCondition);
		cNKMLua.GetData("m_WFWinValue", ref nKMWarfareTemplet.m_WFWinValue);
		cNKMLua.GetData("m_WFLoseCondition", ref nKMWarfareTemplet.m_WFLoseCondition);
		cNKMLua.GetData("m_WFLoseValue", ref nKMWarfareTemplet.m_WFLoseValue);
		cNKMLua.GetData("m_WFMissionType_1", ref nKMWarfareTemplet.m_WFMissionType_1);
		cNKMLua.GetData("m_WFMissionValue_1", ref nKMWarfareTemplet.m_WFMissionValue_1);
		cNKMLua.GetData("m_WFMissionType_2", ref nKMWarfareTemplet.m_WFMissionType_2);
		cNKMLua.GetData("m_WFMissionValue_2", ref nKMWarfareTemplet.m_WFMissionValue_2);
		cNKMLua.GetData("m_RewardUserEXP", ref nKMWarfareTemplet.m_RewardUserEXP);
		cNKMLua.GetData("m_RewardCredit_Min", ref nKMWarfareTemplet.m_RewardCredit_Min);
		cNKMLua.GetData("m_RewardCredit_Max", ref nKMWarfareTemplet.m_RewardCredit_Max);
		cNKMLua.GetData("m_RewardEternium_Min", ref nKMWarfareTemplet.m_RewardEternium_Min);
		cNKMLua.GetData("m_RewardEternium_Max", ref nKMWarfareTemplet.m_RewardEternium_Max);
		for (int i = 0; i < 10; i++)
		{
			RewardData rewardData = new RewardData();
			if (!cNKMLua.GetData($"m_RewardGroupID_{i + 1}", ref rewardData.m_RewardGroupID) || !cNKMLua.GetData($"m_fRewardRate_{i + 1}", ref rewardData.m_RewardRate))
			{
				break;
			}
			if (NKMRewardManager.GetRewardGroup(rewardData.m_RewardGroupID) != null)
			{
				nKMWarfareTemplet.RewardList.Add(rewardData);
			}
		}
		cNKMLua.GetData("m_ContainerGroupID", ref nKMWarfareTemplet.m_ContainerGroupID);
		cNKMLua.GetData("m_bFriendSummon", ref nKMWarfareTemplet.m_bFriendSummon);
		cNKMLua.GetData("m_RewardMultiplyMax", ref nKMWarfareTemplet.m_RewardMultiplyMax);
		return nKMWarfareTemplet;
	}

	public void Join()
	{
		foreach (RewardData reward in RewardList)
		{
			reward.Templet = NKMRewardManager.GetRewardGroup(reward.m_RewardGroupID);
			if (reward.Templet == null)
			{
				Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 보상 정보가 올바르지 않음. rewardGroupId:{reward.m_RewardGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 245);
			}
		}
		if (m_ContainerGroupID != 0)
		{
			ContainerRewardTemplet = NKMRewardManager.GetRewardGroup(m_ContainerGroupID);
			if (ContainerRewardTemplet == null)
			{
				Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 컨테이너 보상 정보가 올바르지 않음. rewardGroupId:{m_ContainerGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 254);
			}
		}
	}

	public void Validate()
	{
		if (m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_KILL_COUNT)
		{
			Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 승리조건에는 KILL_COUNT를 사용할 수 없습니다", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 263);
		}
		if (m_WFLoseCondition == WARFARE_GAME_CONDITION.WFC_KILL_TARGET)
		{
			Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 패배조건에는 KILL_TARGET을 사용할 수 없습니다", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 268);
		}
		if (m_RewardCredit_Min < 0 || m_RewardCredit_Min > m_RewardCredit_Max)
		{
			Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 최소 크레딧 보상이 최대 보상 보다 큽니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 273);
		}
		if (m_RewardEternium_Min < 0 || m_RewardEternium_Min > m_RewardEternium_Max)
		{
			Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 최소 이더니움 보상이 최대 이더니움 보다 큽니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 278);
		}
		if (m_RewardUserEXP < 0)
		{
			Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 유저 경험치 보상이 0보다 작습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 283);
		}
		if (m_RewardMultiplyMax <= 0)
		{
			Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 보상 배수의 맥스치가 0 이하 입니다. value: {m_RewardMultiplyMax}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 288);
		}
	}

	public int DecideRewardCredit()
	{
		return RandomGenerator.Range(m_RewardCredit_Min, m_RewardCredit_Max + 1);
	}

	public int DecideRewardEternium()
	{
		return RandomGenerator.Range(m_RewardEternium_Min, m_RewardEternium_Max + 1);
	}

	public FirstRewardData GetFirstRewardData()
	{
		return StageTemplet?.GetFirstRewardData() ?? FirstRewardData.Empty;
	}

	public void ValidateServerOnly()
	{
		NKMWarfareMapTemplet nKMWarfareMapTemplet = MapTemplet;
		if (nKMWarfareMapTemplet == null)
		{
			Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 맵파일 정보가 올바르지 않음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 313);
			return;
		}
		switch (m_WFWinCondition)
		{
		case WARFARE_GAME_CONDITION.WFC_KILL_BOSS:
			if (!nKMWarfareMapTemplet.Tiles.Any((NKMWarfareTileTemplet tile) => tile.m_bFlagDungeon))
			{
				Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 승리 조건 WFWC_KILL_BOSS 보스(flag) 없음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 322);
			}
			break;
		case WARFARE_GAME_CONDITION.WFC_KILL_TARGET:
		{
			int num = MapTemplet.Tiles.Count((NKMWarfareTileTemplet tile) => tile.m_bTargetUnit);
			if (m_WFWinValue != num)
			{
				Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} target 개수가 올바르지 않습니다. winValue:{m_WFWinValue} targetCount:{num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 330);
			}
			break;
		}
		case WARFARE_GAME_CONDITION.WFC_TILE_ENTER:
		case WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD:
			if (nKMWarfareMapTemplet.GetWinTileIndexByWinType(m_WFWinCondition) == -1)
			{
				Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 승리 조건에 맞는 타일이 없습니다. winCondition:{m_WFWinCondition}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 338);
			}
			break;
		}
		List<short> loseTileIndexList = nKMWarfareMapTemplet.GetLoseTileIndexList(m_WFLoseCondition);
		if (m_WFLoseCondition == WARFARE_GAME_CONDITION.WFC_TILE_ENTER || m_WFLoseCondition == WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD)
		{
			if (loseTileIndexList == null || loseTileIndexList.Count == 0)
			{
				Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 패배 조건에 맞는 타일이 없습니다. loseCondition:{m_WFLoseCondition}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 349);
			}
		}
		else
		{
			if (nKMWarfareMapTemplet.Tiles.Any((NKMWarfareTileTemplet e) => e.m_NKM_WARFARE_ENEMY_ACTION_TYPE == NKM_WARFARE_ENEMY_ACTION_TYPE.NWEAT_FIND_LOSE_TILE))
			{
				Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 타일방어전이 아닌 곳에 방어타일 탐색형 actionType 설정. loseCondition:{m_WFLoseCondition}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 356);
			}
			if (loseTileIndexList != null && loseTileIndexList.Count > 0)
			{
				Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 패배 조건에 맞지 않는 방어 타일이 존재합니다. loseCondition:{m_WFLoseCondition}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 361);
			}
		}
		if (nKMWarfareMapTemplet.Tiles.Any((NKMWarfareTileTemplet tile) => tile.HasContainer()) && ContainerRewardTemplet == null)
		{
			Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 컨테이너 타일이 사용되었으나 보상 정보가 올바르지 않습니다. rewardGroupId:{m_ContainerGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 367);
		}
		foreach (RewardData reward in RewardList)
		{
			if (reward.m_RewardRate <= 0 || reward.m_RewardRate > 10000)
			{
				Log.ErrorAndExit($"[{Key}]{m_WarfareStrID} 보상 확률 정보가 잘못되었습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTemplet.cs", 374);
			}
		}
	}

	public string GetWarfareName()
	{
		return NKCStringTable.GetString(m_WarfareName);
	}

	public void ValidateClientOnly()
	{
		if (m_CutScenStrIDBefore != "" && NKCCutScenManager.GetCutScenTemple(m_CutScenStrIDBefore) == null)
		{
			Log.Error("NKMWarfareTemplet can't find m_CutScenStrIDBefore : " + m_CutScenStrIDBefore, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/Templet/NKMWarfareTempletEx.cs", 21);
		}
		if (m_CutScenStrIDAfter != "" && NKCCutScenManager.GetCutScenTemple(m_CutScenStrIDAfter) == null)
		{
			Log.Error("NKMWarfareTemplet can't find m_CutScenStrIDAfter : " + m_CutScenStrIDAfter, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/Templet/NKMWarfareTempletEx.cs", 29);
		}
	}
}
