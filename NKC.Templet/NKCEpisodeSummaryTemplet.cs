using System;
using Cs.Logging;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCEpisodeSummaryTemplet : INKMTempletEx, INKMTemplet
{
	public string m_OpenTag = "";

	public int INDEX;

	public int m_EpisodeID;

	public EPISODE_DIFFICULTY m_Difficulty;

	public EPISODE_CATEGORY m_EPCategory = EPISODE_CATEGORY.EC_COUNT;

	public int m_SortIndex;

	public string m_LobbyResourceID;

	public string m_BigResourceID;

	public string m_SubResourceID;

	public NKM_SHORTCUT_TYPE m_ShortcutType;

	public string m_ShortcutParam;

	private string dateStrId = "";

	public NKMEpisodeTempletV2 EpisodeTemplet;

	public int Key => INDEX;

	internal bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public NKMIntervalTemplet IntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public static NKCEpisodeSummaryTemplet LoadFromLua(NKMLua lua)
	{
		NKCEpisodeSummaryTemplet nKCEpisodeSummaryTemplet = new NKCEpisodeSummaryTemplet();
		int num = 1 & (lua.GetData("INDEX", ref nKCEpisodeSummaryTemplet.INDEX) ? 1 : 0);
		lua.GetData("EpisodeID", ref nKCEpisodeSummaryTemplet.m_EpisodeID);
		lua.GetData("Difficulty", ref nKCEpisodeSummaryTemplet.m_Difficulty);
		int num2 = (int)((uint)num & (lua.GetData("m_EPCategory", ref nKCEpisodeSummaryTemplet.m_EPCategory) ? 1u : 0u)) & (lua.GetData("m_SortIndex", ref nKCEpisodeSummaryTemplet.m_SortIndex) ? 1 : 0);
		lua.GetData("LobbyResourceID", ref nKCEpisodeSummaryTemplet.m_LobbyResourceID);
		lua.GetData("BigResourceID", ref nKCEpisodeSummaryTemplet.m_BigResourceID);
		lua.GetData("SubResourceID", ref nKCEpisodeSummaryTemplet.m_SubResourceID);
		lua.GetData("m_ShortcutType", ref nKCEpisodeSummaryTemplet.m_ShortcutType);
		lua.GetData("m_Shortcut", ref nKCEpisodeSummaryTemplet.m_ShortcutParam);
		lua.GetData("DateStrID", ref nKCEpisodeSummaryTemplet.dateStrId);
		lua.GetData("OpenTag", ref nKCEpisodeSummaryTemplet.m_OpenTag);
		if (num2 == 0)
		{
			Log.ErrorAndExit($"INDEX : {nKCEpisodeSummaryTemplet.INDEX} - data loading failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCEpisodeSummaryTemplet.cs", 78);
		}
		return nKCEpisodeSummaryTemplet;
	}

	public static NKCEpisodeSummaryTemplet Find(EPISODE_CATEGORY category, int episodeID)
	{
		return NKMTempletContainer<NKCEpisodeSummaryTemplet>.Find((NKCEpisodeSummaryTemplet x) => x.m_EPCategory == category && x.m_EpisodeID == episodeID);
	}

	public void JoinIntervalTemplet()
	{
		if (!string.IsNullOrEmpty(dateStrId))
		{
			IntervalTemplet = NKMIntervalTemplet.Find(dateStrId);
			if (IntervalTemplet == null)
			{
				IntervalTemplet = NKMIntervalTemplet.Unuseable;
				Log.ErrorAndExit("잘못된 interval id :" + dateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCEpisodeSummaryTemplet.cs", 94);
			}
			else if (IntervalTemplet.IsRepeatDate)
			{
				Log.ErrorAndExit($"[NKCEpisodeSummaryTemplet:{Key}] 반복 기간설정 사용 불가. id:{dateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCEpisodeSummaryTemplet.cs", 100);
			}
		}
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}

	public void Join()
	{
		EpisodeTemplet = NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
	}

	public void Validate()
	{
	}

	public bool CheckEnable(DateTime current)
	{
		bool result = EnableByTag && IntervalTemplet.IsValidTime(current);
		if (m_EPCategory == EPISODE_CATEGORY.EC_FIERCE)
		{
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			if (nKCFierceBattleSupportDataMgr.FierceTemplet == null || !nKCFierceBattleSupportDataMgr.IsCanAccessFierce())
			{
				return false;
			}
		}
		return result;
	}

	public bool CheckEpisodeEnable(DateTime current)
	{
		bool num = EpisodeTemplet != null && EpisodeTemplet.EnableByTag && EpisodeTemplet.IsOpen && EpisodeTemplet.IntervalTemplet.IsValidTime(current);
		bool flag = EpisodeTemplet != null && EpisodeTemplet.m_DicStage.Count > 0 && EpisodeTemplet.GetFirstStage(1) != null && EpisodeTemplet.GetFirstStage(1).IsOpenedDayOfWeek();
		return num && flag;
	}

	public bool HasDateLimit()
	{
		if (EpisodeTemplet != null)
		{
			return EpisodeTemplet.HasEventTimeLimit;
		}
		return false;
	}

	public bool ShowInPVE_01()
	{
		switch (m_EPCategory)
		{
		case EPISODE_CATEGORY.EC_MAINSTREAM:
		case EPISODE_CATEGORY.EC_SIDESTORY:
		case EPISODE_CATEGORY.EC_EVENT:
		case EPISODE_CATEGORY.EC_FIERCE:
		case EPISODE_CATEGORY.EC_SEASONAL:
			return true;
		default:
			return false;
		}
	}

	public bool ShowInPVE_02()
	{
		EPISODE_CATEGORY ePCategory = m_EPCategory;
		if ((uint)(ePCategory - 6) <= 1u)
		{
			return true;
		}
		return false;
	}
}
