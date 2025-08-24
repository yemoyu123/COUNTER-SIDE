using System;
using System.Collections.Generic;
using Cs.Core.Util;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMEpisodeTempletV2 : INKMTemplet, INKMTempletEx
{
	public enum EPISODE_COMPELTE_TYPE
	{
		NONE,
		MEDAL,
		CLEAR
	}

	public Dictionary<int, List<NKMStageTempletV2>> m_DicStage = new Dictionary<int, List<NKMStageTempletV2>>();

	public int m_Idx;

	public int m_EpisodeID;

	public string m_EpisodeStrID = "";

	public int m_GroupID;

	public EPISODE_CATEGORY m_EPCategory;

	public string m_EpisodeTitle = "";

	public string m_EpisodeName = "";

	public string m_EPThumbnail = "";

	public string m_EPThumbnail_SUB_Node = "";

	public bool m_HasCompleteReward;

	public bool m_bNoCollectionCutscene;

	public bool m_bIsSupplement;

	public string m_OpenTag = "";

	public string m_CollectionOpenTag = "";

	public NKM_SHORTCUT_TYPE m_ButtonShortCutType;

	public string m_ButtonShortCutParam = "";

	public string m_Stage_Viewer_Prefab = "";

	public string m_EpisodeDesc = "";

	public string m_EpisodeDescSub = "";

	public string m_DefaultSubTabIcon = "";

	public string m_SelectSubTabIcon = "";

	public string m_BG_Music = "";

	public string intervalId;

	public NKMIntervalTemplet intervalTemplet = NKMIntervalTemplet.Invalid;

	public List<int> m_resourceTypeID;

	public EPISODE_SCROLL_TYPE m_ScrollType;

	public int m_SortIndex;

	public bool m_bHideActTab;

	public EPISODE_DIFFICULTY m_Difficulty;

	public int m_ActCount;

	public CompletionReward[] m_CompletionReward = new CompletionReward[3];

	public List<int> m_lstConnect_EpisodeID;

	public DateTime EpisodeDateStartUtc => ServiceTime.ToUtcTime(intervalTemplet.StartDate);

	public DateTime EpisodeDateEndUtc => ServiceTime.ToUtcTime(intervalTemplet.EndDate);

	public bool HasEventTimeLimit => intervalTemplet.IsValid;

	public IIntervalTemplet IntervalTemplet => intervalTemplet;

	internal bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public bool CollectionEnableByTag => NKMOpenTagManager.IsOpened(m_CollectionOpenTag);

	public int Key => GetEpisodeTempletKey(m_EpisodeID, m_Difficulty);

	public static IEnumerable<NKMEpisodeTempletV2> Values => NKMTempletContainer<NKMEpisodeTempletV2>.Values;

	public bool IsOpen
	{
		get
		{
			if (!EnableByTag)
			{
				return false;
			}
			if (m_DicStage.Count == 0)
			{
				return false;
			}
			if (!HasEventTimeLimit)
			{
				return true;
			}
			bool num = NKCSynchronizedTime.GetTimeLeft(EpisodeDateStartUtc).TotalSeconds == 0.0;
			bool flag = NKCSynchronizedTime.GetTimeLeft(EpisodeDateEndUtc).TotalSeconds == 0.0;
			if (num && !flag)
			{
				return true;
			}
			return false;
		}
	}

	public bool HaveEventDrop
	{
		get
		{
			foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in m_DicStage)
			{
				if (item.Value == null || item.Value.Count == 0)
				{
					continue;
				}
				for (int i = 0; i < item.Value.Count; i++)
				{
					if (NKMEpisodeMgr.CheckStageHasEventDrop(item.Value[i]))
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public bool HaveBuffDrop
	{
		get
		{
			foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in m_DicStage)
			{
				if (item.Value == null || item.Value.Count == 0)
				{
					continue;
				}
				for (int i = 0; i < item.Value.Count; i++)
				{
					if (NKMEpisodeMgr.CheckStageHasBuffDrop(item.Value[i]))
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public bool HasCompletionReward
	{
		get
		{
			for (int i = 0; i < m_CompletionReward.Length; i++)
			{
				if (m_CompletionReward[i] != null)
				{
					return true;
				}
			}
			return false;
		}
	}

	public List<int> ResourceIdList => m_resourceTypeID;

	public static int GetEpisodeTempletKey(int episodeID, EPISODE_DIFFICULTY difficulty)
	{
		return (int)(episodeID * 10 + difficulty);
	}

	public static NKMEpisodeTempletV2 LoadFromLUA(NKMLua lua)
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = new NKMEpisodeTempletV2();
		bool flag = true;
		flag &= lua.GetData("m_EpisodeID", ref nKMEpisodeTempletV.m_EpisodeID);
		flag &= lua.GetData("m_EpisodeStrID", ref nKMEpisodeTempletV.m_EpisodeStrID);
		flag &= lua.GetData("GroupID", ref nKMEpisodeTempletV.m_GroupID);
		lua.GetData("m_EPCategory", ref nKMEpisodeTempletV.m_EPCategory);
		lua.GetData("m_EpisodeTitle", ref nKMEpisodeTempletV.m_EpisodeTitle);
		lua.GetData("m_EpisodeName", ref nKMEpisodeTempletV.m_EpisodeName);
		lua.GetData("m_EPThumbnail", ref nKMEpisodeTempletV.m_EPThumbnail);
		lua.GetData("m_EPThumbnail_SUB_Node", ref nKMEpisodeTempletV.m_EPThumbnail_SUB_Node);
		lua.GetData("m_bNoCollectionCutscene", ref nKMEpisodeTempletV.m_bNoCollectionCutscene);
		lua.GetData("m_bSupplement", ref nKMEpisodeTempletV.m_bIsSupplement);
		lua.GetData("m_OpenTag", ref nKMEpisodeTempletV.m_OpenTag);
		lua.GetData("m_CollectionOpenTag", ref nKMEpisodeTempletV.m_CollectionOpenTag);
		lua.GetData("m_ButtonShortCutType", ref nKMEpisodeTempletV.m_ButtonShortCutType);
		lua.GetData("m_ButtonShortCut", ref nKMEpisodeTempletV.m_ButtonShortCutParam);
		lua.GetData("m_Stage_Viewer_Prefab", ref nKMEpisodeTempletV.m_Stage_Viewer_Prefab);
		lua.GetData("m_EpisodeDesc_1", ref nKMEpisodeTempletV.m_EpisodeDesc);
		lua.GetData("m_EpisodeDesc_2", ref nKMEpisodeTempletV.m_EpisodeDescSub);
		lua.GetData("m_DefaultSubTabIcon", ref nKMEpisodeTempletV.m_DefaultSubTabIcon);
		lua.GetData("m_SelectSubTabIcon", ref nKMEpisodeTempletV.m_SelectSubTabIcon);
		lua.GetData("m_DateStrID", ref nKMEpisodeTempletV.intervalId);
		lua.GetData("m_BG_Music", ref nKMEpisodeTempletV.m_BG_Music);
		lua.GetData("m_Scroll_Type", ref nKMEpisodeTempletV.m_ScrollType);
		lua.GetData("m_SortIndex", ref nKMEpisodeTempletV.m_SortIndex);
		lua.GetData("m_bHideActTab", ref nKMEpisodeTempletV.m_bHideActTab);
		nKMEpisodeTempletV.m_resourceTypeID = new List<int>();
		lua.GetData("m_ResourceTypeID", nKMEpisodeTempletV.m_resourceTypeID);
		lua.GetData("m_ActCount", ref nKMEpisodeTempletV.m_ActCount);
		lua.GetData("m_Difficulty", ref nKMEpisodeTempletV.m_Difficulty);
		nKMEpisodeTempletV.m_lstConnect_EpisodeID = new List<int>();
		lua.GetData("Connect_EpisodeID", nKMEpisodeTempletV.m_lstConnect_EpisodeID);
		for (int i = 0; i < 3; i++)
		{
			int rValue = 0;
			lua.GetData($"m_CompleteRate_{i + 1}", ref rValue);
			if (rValue != 0)
			{
				CompletionReward completionReward = new CompletionReward
				{
					m_CompleteRate = rValue
				};
				NKMRewardInfo rewardInfo = completionReward.m_RewardInfo;
				lua.GetData($"m_RewardType_{i + 1}", ref rewardInfo.rewardType);
				lua.GetData($"m_RewardID_{i + 1}", ref rewardInfo.ID);
				lua.GetData($"m_RewardValue_{i + 1}", ref rewardInfo.Count);
				nKMEpisodeTempletV.m_CompletionReward[i] = completionReward;
				nKMEpisodeTempletV.m_HasCompleteReward = true;
			}
		}
		if (!flag)
		{
			Log.ErrorAndExit($"[{nKMEpisodeTempletV.m_EpisodeID}]{nKMEpisodeTempletV.m_EpisodeStrID} data loading failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMEpisodeTempletV2.cs", 168);
		}
		return nKMEpisodeTempletV;
	}

	public static NKMEpisodeTempletV2 Find(int episodeID, EPISODE_DIFFICULTY difficulty)
	{
		return NKMTempletContainer<NKMEpisodeTempletV2>.Find(GetEpisodeTempletKey(episodeID, difficulty));
	}

	public void Join()
	{
		Sort();
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
		NKMEpisodeGroupTemplet.Find(m_GroupID)?.AddEpisodeTemplet(this);
	}

	public void JoinIntervalTemplet()
	{
		if (!string.IsNullOrEmpty(intervalId))
		{
			intervalTemplet = NKMIntervalTemplet.Find(intervalId);
			if (intervalTemplet == null)
			{
				intervalTemplet = NKMIntervalTemplet.Invalid;
				NKMTempletError.Add("[NKMEpisodeSubTemplet:" + m_EpisodeStrID + "] 잘못된 interval id:" + intervalId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMEpisodeTempletV2.cs", 198);
			}
			else if (intervalTemplet.IsRepeatDate)
			{
				NKMTempletError.Add("[NKMEpisodeSubTemplet:" + m_EpisodeStrID + "] 반복 기간설정 사용 불가. id:" + intervalId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMEpisodeTempletV2.cs", 204);
			}
		}
	}

	public void Validate()
	{
		if (m_Difficulty < EPISODE_DIFFICULTY.NORMAL || m_Difficulty > EPISODE_DIFFICULTY.HARD)
		{
			NKMTempletError.Add($"[NKMEpisodeTemplet] 난이도가 유효하지 않음 m_EpisodeID:{m_EpisodeID} m_Difficulty:{m_Difficulty}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMEpisodeTempletV2.cs", 214);
		}
	}

	public void Sort()
	{
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in m_DicStage)
		{
			item.Value.Sort();
		}
	}

	public void AddStageTemplet(NKMStageTempletV2 stageTemplet)
	{
		if (!m_DicStage.ContainsKey(stageTemplet.ActId))
		{
			m_DicStage.Add(stageTemplet.ActId, new List<NKMStageTempletV2> { stageTemplet });
		}
		else if (!m_DicStage[stageTemplet.ActId].Contains(stageTemplet))
		{
			m_DicStage[stageTemplet.ActId].Add(stageTemplet);
		}
		m_DicStage[stageTemplet.ActId].Sort(SortBySortIndex);
	}

	private int SortBySortIndex(NKMStageTempletV2 lItem, NKMStageTempletV2 rItem)
	{
		return lItem.m_StageIndex.CompareTo(rItem.m_StageIndex);
	}

	public bool CheckEnable(DateTime current)
	{
		if (EnableByTag)
		{
			return IntervalTemplet.IsValidTime(current);
		}
		return false;
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in m_DicStage)
		{
			for (int num = item.Value.Count - 1; num >= 0; num--)
			{
				if (!item.Value[num].EnableByTag)
				{
					item.Value.RemoveAt(num);
				}
			}
		}
	}

	public string GetEpisodeTitle()
	{
		return NKCStringTable.GetString(m_EpisodeTitle);
	}

	public string GetEpisodeName()
	{
		return NKCStringTable.GetString(m_EpisodeName);
	}

	public string GetEpisodeDesc()
	{
		return NKCStringTable.GetString(m_EpisodeDesc);
	}

	public string GetEpisodeDescSub()
	{
		return NKCStringTable.GetString(m_EpisodeDescSub);
	}

	public bool IsOpenedDayOfWeek()
	{
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in m_DicStage)
		{
			if (item.Value == null || item.Value.Count == 0)
			{
				continue;
			}
			for (int i = 0; i < item.Value.Count; i++)
			{
				if (item.Value[i].IsOpenedDayOfWeek())
				{
					return true;
				}
			}
		}
		return false;
	}

	public UnlockInfo GetUnlockInfo()
	{
		if (m_DicStage.Count == 0 || GetFirstStage(1) == null)
		{
			return new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_LOCKED, 0);
		}
		return GetFirstStage(1)?.m_UnlockInfo ?? new UnlockInfo(STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_LOCKED, 0);
	}

	public NKMStageTempletV2 GetFirstStage(int actID)
	{
		if (!m_DicStage.ContainsKey(actID) || m_DicStage[actID].Count == 0)
		{
			return null;
		}
		return m_DicStage[actID][0];
	}

	public int GetFirstBattleStageLevel(int actID)
	{
		if (!m_DicStage.ContainsKey(actID) || m_DicStage[actID].Count == 0)
		{
			return 0;
		}
		for (int i = 0; i < m_DicStage[actID].Count; i++)
		{
			if (m_DicStage[actID][i].m_STAGE_TYPE == STAGE_TYPE.ST_DUNGEON)
			{
				NKMDungeonTempletBase dungeonTempletBase = m_DicStage[actID][i].DungeonTempletBase;
				if (dungeonTempletBase.m_DungeonType != NKM_DUNGEON_TYPE.NDT_CUTSCENE)
				{
					return dungeonTempletBase.m_DungeonLevel;
				}
			}
			else if (m_DicStage[actID][i].m_STAGE_TYPE == STAGE_TYPE.ST_PHASE)
			{
				NKMPhaseTemplet phaseTemplet = m_DicStage[actID][i].PhaseTemplet;
				if (phaseTemplet != null)
				{
					return phaseTemplet.PhaseLevel;
				}
			}
		}
		return 0;
	}

	public bool CheckExistReward(int completePercent)
	{
		if (completePercent <= 0)
		{
			return false;
		}
		for (int i = 0; i < 3 && m_CompletionReward[i] != null; i++)
		{
			if (m_CompletionReward[i].m_CompleteRate == completePercent)
			{
				return true;
			}
		}
		return false;
	}

	public bool UseEpSlot()
	{
		if (m_EPCategory != EPISODE_CATEGORY.EC_SUPPLY)
		{
			return m_EPCategory == EPISODE_CATEGORY.EC_DAILY;
		}
		return true;
	}
}
