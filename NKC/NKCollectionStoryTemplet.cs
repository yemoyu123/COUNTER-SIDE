using System.Collections.Generic;

namespace NKC;

public class NKCollectionStoryTemplet
{
	public int m_EpisodeID;

	private string m_EpisodeTitle;

	private string m_EpisodeName;

	public int m_ActID;

	public List<CollectionStoryData> m_lstData;

	public NKCollectionStoryTemplet(int EpID, string EpTitle, string EpName, CollectionStoryData newData)
	{
		m_EpisodeID = EpID;
		m_EpisodeTitle = EpTitle;
		m_EpisodeName = EpName;
		m_lstData = new List<CollectionStoryData>();
		m_lstData.Add(newData);
	}

	public string GetEpisodeTitle()
	{
		return NKCStringTable.GetString(m_EpisodeTitle);
	}

	public string GetEpisodeName()
	{
		return NKCStringTable.GetString(m_EpisodeName);
	}
}
