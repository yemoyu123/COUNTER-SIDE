using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Event;

public class NKMEventBingoTemplet : INKMTemplet
{
	public int m_EventID;

	public int m_BingoTryItemID;

	public int m_BingoTryItemValue;

	public int m_BingoSpecialTryRequireCnt;

	public int m_BingoSpecialMissionID_1;

	public int m_BingoSpecialMissionID_2;

	public int m_BingoSpecialMissionID_3;

	public int m_BingoSpecialMissionID_4;

	public int m_BingoCompletRewardGroupID;

	public int m_BingoMissionTabId;

	public int m_BingoSize;

	public List<int> MissionTiles => CalculateBingoMissions();

	public int TileRange => (int)Math.Pow(m_BingoSize, 2.0) - MissionTiles.Count();

	public int Key => m_EventID;

	public static NKMEventBingoTemplet Find(int key)
	{
		return NKMTempletContainer<NKMEventBingoTemplet>.Find(key);
	}

	public static NKMEventBingoTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBingoTemplet.cs", 46))
		{
			return null;
		}
		NKMEventBingoTemplet nKMEventBingoTemplet = new NKMEventBingoTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("m_EventID", ref nKMEventBingoTemplet.m_EventID);
		if (NKMTempletContainer<NKMEventTabTemplet>.Find(nKMEventBingoTemplet.m_EventID) == null)
		{
			return null;
		}
		flag &= cNKMLua.GetData("m_BingoTryItemID", ref nKMEventBingoTemplet.m_BingoTryItemID);
		flag &= cNKMLua.GetData("m_BingoTryItemValue", ref nKMEventBingoTemplet.m_BingoTryItemValue);
		flag &= cNKMLua.GetData("m_BingoSpecialTryRequireCnt", ref nKMEventBingoTemplet.m_BingoSpecialTryRequireCnt);
		flag &= cNKMLua.GetData("m_BingoCompletRewardGroupID", ref nKMEventBingoTemplet.m_BingoCompletRewardGroupID);
		flag &= cNKMLua.GetData("m_BingoMissionTabID", ref nKMEventBingoTemplet.m_BingoMissionTabId);
		if (!(flag & cNKMLua.GetData("m_BingoSize", ref nKMEventBingoTemplet.m_BingoSize)))
		{
			return null;
		}
		return nKMEventBingoTemplet;
	}

	public void Join()
	{
	}

	public List<int> CalculateBingoMissions()
	{
		List<int> list = (from e in NKMMissionManager.GetMissionTempletListByType(m_BingoMissionTabId).SelectMany((NKMMissionTemplet array) => array.m_MissionReward)
			where e.reward_type == NKM_REWARD_TYPE.RT_BINGO_TILE
			select e.reward_value).Distinct().ToList();
		if (list.Count != 4)
		{
			Log.Error($"[Bingo] 미션 타일 개수 이상. count:{list.Count} eventId:{m_EventID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBingoTemplet.cs", 87);
		}
		return list;
	}

	public void Validate()
	{
	}
}
