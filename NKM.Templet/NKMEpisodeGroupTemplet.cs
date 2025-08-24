using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMEpisodeGroupTemplet : INKMTemplet
{
	public EPISODE_GROUP GroupCategory;

	public int EpisodeGroupID;

	public EPISODE_CATEGORY EpCategory;

	public string m_EPGroupIcon;

	public string m_EPGroupName;

	private List<NKMEpisodeTempletV2> m_lstEpisodeTemplet = new List<NKMEpisodeTempletV2>();

	public int Key => EpisodeGroupID;

	public List<NKMEpisodeTempletV2> lstEpisodeTemplet => m_lstEpisodeTemplet;

	public static NKMEpisodeGroupTemplet LoadFromLUA(NKMLua lua)
	{
		NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = new NKMEpisodeGroupTemplet();
		lua.GetData("GroupID", ref nKMEpisodeGroupTemplet.EpisodeGroupID);
		lua.GetData("m_EPGroupCategory", ref nKMEpisodeGroupTemplet.GroupCategory);
		lua.GetData("m_EPCategory", ref nKMEpisodeGroupTemplet.EpCategory);
		lua.GetData("m_EPGroupIcon", ref nKMEpisodeGroupTemplet.m_EPGroupIcon);
		lua.GetData("m_EPGroupName", ref nKMEpisodeGroupTemplet.m_EPGroupName);
		nKMEpisodeGroupTemplet.m_lstEpisodeTemplet = new List<NKMEpisodeTempletV2>();
		if (1 == 0)
		{
			Log.ErrorAndExit($"[{nKMEpisodeGroupTemplet.GroupCategory}]{nKMEpisodeGroupTemplet.EpisodeGroupID} data loading failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMEpisodeGroupTemplet.cs", 52);
		}
		return nKMEpisodeGroupTemplet;
	}

	public static NKMEpisodeGroupTemplet Find(int key)
	{
		return NKMTempletContainer<NKMEpisodeGroupTemplet>.Find(key);
	}

	public static NKMEpisodeGroupTemplet Find(EPISODE_GROUP group)
	{
		return NKMTempletContainer<NKMEpisodeGroupTemplet>.Find((NKMEpisodeGroupTemplet x) => x.GroupCategory == group);
	}

	public static NKMEpisodeGroupTemplet Find(EPISODE_CATEGORY category)
	{
		return NKMTempletContainer<NKMEpisodeGroupTemplet>.Find((NKMEpisodeGroupTemplet x) => x.EpCategory == category);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public void AddEpisodeTemplet(NKMEpisodeTempletV2 epTemplet)
	{
		m_lstEpisodeTemplet.Add(epTemplet);
		m_lstEpisodeTemplet.Sort(SortBySortIndex);
	}

	public int SortBySortIndex(NKMEpisodeTempletV2 lItem, NKMEpisodeTempletV2 rItem)
	{
		return lItem.m_SortIndex.CompareTo(rItem.m_SortIndex);
	}
}
