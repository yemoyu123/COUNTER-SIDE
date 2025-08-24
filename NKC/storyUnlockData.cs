using System.Collections.Generic;
using NKM;
using NKM.Templet;

namespace NKC;

public struct storyUnlockData
{
	public List<UnlockInfo> m_UnlockReqList;

	public EPISODE_CATEGORY m_EpisodeCategory { get; private set; }

	public int m_EpisodeID { get; private set; }

	public int m_ActID { get; private set; }

	public int m_StageID { get; private set; }

	public storyUnlockData(List<UnlockInfo> unlockReqList, EPISODE_CATEGORY episodeCategory, int episodeID, int actID, int stageID)
	{
		m_UnlockReqList = unlockReqList;
		m_EpisodeCategory = episodeCategory;
		m_EpisodeID = episodeID;
		m_ActID = actID;
		m_StageID = stageID;
	}
}
