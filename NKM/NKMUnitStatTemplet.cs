using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public class NKMUnitStatTemplet : INKMTemplet
{
	public int m_UnitID;

	public string m_UnitStrID = "";

	private int m_RespawnCost = 1;

	public int m_RespawnCount = 1;

	public NKMStatData m_StatData = new NKMStatData();

	public int Key => m_UnitID;

	public static NKMUnitStatTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 141))
		{
			return null;
		}
		NKMUnitStatTemplet nKMUnitStatTemplet = new NKMUnitStatTemplet();
		cNKMLua.GetData("m_UnitStrID", ref nKMUnitStatTemplet.m_UnitStrID);
		nKMUnitStatTemplet.m_UnitID = NKMUnitManager.GetUnitID(nKMUnitStatTemplet.m_UnitStrID);
		if (nKMUnitStatTemplet.m_UnitID == 0)
		{
			Log.ErrorAndExit("GetUnitID failed. unitStrId:" + nKMUnitStatTemplet.m_UnitStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitManager.cs", 150);
			return null;
		}
		cNKMLua.GetData("m_RespawnCost", ref nKMUnitStatTemplet.m_RespawnCost);
		cNKMLua.GetData("m_RespawnCount", ref nKMUnitStatTemplet.m_RespawnCount);
		if (cNKMLua.OpenTable("m_StatData"))
		{
			nKMUnitStatTemplet.m_StatData.LoadFromLUA(cNKMLua);
			cNKMLua.CloseTable();
		}
		return nKMUnitStatTemplet;
	}

	public void DeepCopyFromSource(NKMUnitStatTemplet source)
	{
		if (source != null)
		{
			m_UnitID = source.m_UnitID;
			m_UnitStrID = source.m_UnitStrID;
			m_RespawnCost = source.m_RespawnCost;
			m_RespawnCount = source.m_RespawnCount;
			m_StatData.DeepCopyFromSource(source.m_StatData);
		}
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public int GetRespawnCost(bool bLeader, Dictionary<int, NKMBanData> dicNKMBanData)
	{
		int num = m_RespawnCost;
		if (dicNKMBanData != null && dicNKMBanData.ContainsKey(m_UnitID))
		{
			NKMBanData nKMBanData = dicNKMBanData[m_UnitID];
			num = m_RespawnCost + nKMBanData.m_BanLevel;
		}
		else if (bLeader)
		{
			num--;
		}
		if (num < 1)
		{
			num = 1;
		}
		if (num > 10)
		{
			num = 10;
		}
		return num;
	}

	public int GetRespawnCost(bool bLeader, Dictionary<int, NKMBanData> dicNKMBanData, Dictionary<int, NKMUnitUpData> dicNKMUpData)
	{
		int num = m_RespawnCost;
		if (dicNKMBanData != null && dicNKMBanData.ContainsKey(m_UnitID))
		{
			NKMBanData nKMBanData = dicNKMBanData[m_UnitID];
			num = m_RespawnCost + nKMBanData.m_BanLevel;
		}
		else if (bLeader)
		{
			num--;
		}
		if (NKMCommonConst.PVP_USE_UP_COST_DOWN && dicNKMUpData != null && dicNKMUpData.ContainsKey(m_UnitID))
		{
			NKMUnitUpData nKMUnitUpData = dicNKMUpData[m_UnitID];
			num -= nKMUnitUpData.upLevel;
		}
		if (num < 1)
		{
			num = 1;
		}
		if (num > 10)
		{
			num = 10;
		}
		return num;
	}

	public int GetRespawnCost(bool bPVP, bool bLeader, Dictionary<int, NKMBanData> dicNKMBanData, Dictionary<int, NKMUnitUpData> dicNKMUpData)
	{
		if (bPVP)
		{
			return GetRespawnCost(bLeader, dicNKMBanData, dicNKMUpData);
		}
		int num = m_RespawnCost;
		if (bLeader)
		{
			num--;
		}
		if (num < 1)
		{
			num = 1;
		}
		if (num > 10)
		{
			num = 10;
		}
		return num;
	}
}
