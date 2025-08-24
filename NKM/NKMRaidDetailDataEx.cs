using System.Collections.Generic;
using ClientPacket.Raid;
using NKC;

namespace NKM;

public static class NKMRaidDetailDataEx
{
	public class Comp : IComparer<NKMRaidJoinData>
	{
		public int Compare(NKMRaidJoinData x, NKMRaidJoinData y)
		{
			if (y.damage > x.damage)
			{
				return 1;
			}
			if (y.damage < x.damage)
			{
				return -1;
			}
			if (y.userUID <= x.userUID)
			{
				return 1;
			}
			return -1;
		}
	}

	public static void SortJoinDataByDamage(this NKMRaidDetailData data)
	{
		data.raidJoinDataList.Sort(new Comp());
	}

	public static void DeepCopyFromSource(this NKMRaidDetailData data, NKMMyRaidData source)
	{
		data.raidUID = source.raidUID;
		data.stageID = source.stageID;
		data.cityID = source.cityID;
		data.curHP = source.curHP;
		data.maxHP = source.maxHP;
		data.isCoop = source.isCoop;
		data.isNew = source.isNew;
		data.expireDate = source.expireDate;
		data.seasonID = source.seasonID;
		if (NKCScenManager.CurrentUserData() != null)
		{
			data.userUID = NKCScenManager.CurrentUserData().m_UserUID;
		}
	}

	public static void DeepCopyFromSource(this NKMRaidDetailData data, NKMRaidResultData source)
	{
		data.raidUID = source.raidUID;
		data.stageID = source.stageID;
		data.cityID = source.cityID;
		data.curHP = source.curHP;
		data.maxHP = source.maxHP;
		data.isCoop = source.isCoop;
		data.isNew = false;
		data.expireDate = source.expireDate;
		data.userUID = source.userUID;
		data.seasonID = source.seasonID;
		data.friendCode = source.friendCode;
		data.raidJoinDataList = source.raidJoinDataList;
	}

	public static NKMRaidJoinData FindJoinData(this NKMRaidDetailData data, long userUID)
	{
		if (data.raidJoinDataList == null)
		{
			return null;
		}
		for (int i = 0; i < data.raidJoinDataList.Count; i++)
		{
			NKMRaidJoinData nKMRaidJoinData = data.raidJoinDataList[i];
			if (nKMRaidJoinData != null && nKMRaidJoinData.userUID == userUID)
			{
				return nKMRaidJoinData;
			}
		}
		return null;
	}
}
