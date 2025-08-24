using System.Collections.Generic;
using ClientPacket.Raid;
using Cs.Protocol;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKCRaidDataMgr
{
	private List<NKMRaidDetailData> m_RaidDataList = new List<NKMRaidDetailData>();

	public void SetData(NKMRaidDetailData newRaidData)
	{
		if (m_RaidDataList == null)
		{
			m_RaidDataList = new List<NKMRaidDetailData>();
		}
		bool flag = false;
		for (int i = 0; i < m_RaidDataList.Count; i++)
		{
			NKMRaidDetailData nKMRaidDetailData = m_RaidDataList[i];
			if (nKMRaidDetailData.raidUID == newRaidData.raidUID)
			{
				nKMRaidDetailData.DeepCopyFrom(newRaidData);
				flag = true;
			}
		}
		if (!flag)
		{
			m_RaidDataList.Add(newRaidData);
		}
	}

	public void SetDataList(List<NKMMyRaidData> newRaidDataList)
	{
		if (newRaidDataList == null)
		{
			if (m_RaidDataList != null)
			{
				m_RaidDataList.Clear();
			}
			else
			{
				m_RaidDataList = new List<NKMRaidDetailData>();
			}
			return;
		}
		if (m_RaidDataList == null)
		{
			m_RaidDataList = new List<NKMRaidDetailData>();
		}
		int i;
		for (i = 0; i < newRaidDataList.Count; i++)
		{
			NKMRaidDetailData nKMRaidDetailData = new NKMRaidDetailData();
			nKMRaidDetailData.DeepCopyFromSource(newRaidDataList[i]);
			NKMRaidDetailData nKMRaidDetailData2 = m_RaidDataList.Find((NKMRaidDetailData x) => x.raidUID == newRaidDataList[i].raidUID);
			if (nKMRaidDetailData2 != null)
			{
				nKMRaidDetailData.raidJoinDataList = nKMRaidDetailData2.raidJoinDataList;
				m_RaidDataList.Remove(nKMRaidDetailData2);
			}
			m_RaidDataList.Add(nKMRaidDetailData);
		}
	}

	public void SetDataList(List<NKMRaidResultData> resultDataList)
	{
		if (resultDataList == null)
		{
			return;
		}
		if (m_RaidDataList == null)
		{
			m_RaidDataList = new List<NKMRaidDetailData>();
		}
		int i;
		for (i = 0; i < resultDataList.Count; i++)
		{
			NKMRaidDetailData nKMRaidDetailData = m_RaidDataList.Find((NKMRaidDetailData x) => x.raidUID == resultDataList[i].raidUID);
			if (nKMRaidDetailData == null)
			{
				nKMRaidDetailData = new NKMRaidDetailData();
				nKMRaidDetailData.DeepCopyFromSource(resultDataList[i]);
				m_RaidDataList.Add(nKMRaidDetailData);
			}
			else
			{
				nKMRaidDetailData.DeepCopyFromSource(resultDataList[i]);
			}
		}
	}

	public List<NKMRaidDetailData> GetDataList()
	{
		return m_RaidDataList;
	}

	public NKMRaidDetailData Find(int cityID, int stageID)
	{
		if (m_RaidDataList == null)
		{
			return null;
		}
		for (int i = 0; i < m_RaidDataList.Count; i++)
		{
			NKMRaidDetailData nKMRaidDetailData = m_RaidDataList[i];
			if (nKMRaidDetailData != null && nKMRaidDetailData.cityID == cityID && nKMRaidDetailData.stageID == stageID)
			{
				return nKMRaidDetailData;
			}
		}
		return null;
	}

	public NKMRaidDetailData Find(long raidUID)
	{
		if (m_RaidDataList == null)
		{
			return null;
		}
		for (int i = 0; i < m_RaidDataList.Count; i++)
		{
			NKMRaidDetailData nKMRaidDetailData = m_RaidDataList[i];
			if (nKMRaidDetailData != null && nKMRaidDetailData.raidUID == raidUID)
			{
				return nKMRaidDetailData;
			}
		}
		return null;
	}

	public void Remove(long raidUID)
	{
		if (m_RaidDataList == null)
		{
			return;
		}
		for (int i = 0; i < m_RaidDataList.Count; i++)
		{
			NKMRaidDetailData nKMRaidDetailData = m_RaidDataList[i];
			if (nKMRaidDetailData != null && nKMRaidDetailData.raidUID == raidUID)
			{
				m_RaidDataList.RemoveAt(i);
				break;
			}
		}
	}

	public bool CheckCompletableRaid(long raidUID)
	{
		NKMRaidDetailData nKMRaidDetailData = Find(raidUID);
		if (nKMRaidDetailData == null)
		{
			return false;
		}
		if (NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate))
		{
			return true;
		}
		if (nKMRaidDetailData.curHP <= 0f)
		{
			return true;
		}
		NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
		if (nKMRaidTemplet != null && nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID && nKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID)?.tryCount == nKMRaidTemplet.RaidTryCount)
		{
			return true;
		}
		return false;
	}

	public bool CheckRaidCoopOn(long raidUID)
	{
		return Find(raidUID)?.isCoop ?? false;
	}

	public void SetRaidCoopOn(long raidUID, List<NKMRaidJoinData> lstNKMRaidJoinData)
	{
		NKMRaidDetailData nKMRaidDetailData = Find(raidUID);
		if (nKMRaidDetailData != null)
		{
			nKMRaidDetailData.raidJoinDataList = lstNKMRaidJoinData;
			nKMRaidDetailData.isCoop = true;
		}
	}
}
