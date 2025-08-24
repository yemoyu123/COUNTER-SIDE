using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public sealed class NKMDummyDeckData : ISerializable
{
	public sbyte LeaderIndex;

	public NKMDummyUnitData Ship = new NKMDummyUnitData();

	public NKMDummyUnitData operatorUnit = new NKMDummyUnitData();

	public NKMDummyUnitData[] List = new NKMDummyUnitData[8];

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref LeaderIndex);
		stream.PutOrGet(ref Ship);
		stream.PutOrGet(ref operatorUnit);
		stream.PutOrGet(ref List);
	}

	public int GetShipUnitId()
	{
		return Ship.UnitId;
	}

	public NKMDummyUnitData GetLeader()
	{
		if (LeaderIndex < 0 || LeaderIndex >= List.Length)
		{
			return null;
		}
		return List[LeaderIndex];
	}

	public NKMUnitData CreateShip(long shipUid)
	{
		if (Ship == null)
		{
			return null;
		}
		if (NKMUnitManager.GetUnitTempletBase(Ship.UnitId) == null)
		{
			return null;
		}
		return Ship.ToUnitData(shipUid);
	}

	public NKMUnitTempletBase GetShipTemplet()
	{
		return NKMUnitManager.GetUnitTempletBase(GetShipUnitId());
	}

	public bool Equal(NKMDummyDeckData dummyDeckData)
	{
		if (LeaderIndex != dummyDeckData.LeaderIndex)
		{
			return false;
		}
		if (Ship.UnitId != dummyDeckData.Ship.UnitId || Ship.UnitLevel != dummyDeckData.Ship.UnitLevel)
		{
			return false;
		}
		for (int i = 0; i < 8; i++)
		{
			NKMDummyUnitData nKMDummyUnitData = List[i];
			NKMDummyUnitData nKMDummyUnitData2 = dummyDeckData.List[i];
			if (nKMDummyUnitData.UnitId != nKMDummyUnitData2.UnitId || nKMDummyUnitData.UnitLevel != nKMDummyUnitData2.UnitLevel)
			{
				return false;
			}
		}
		return true;
	}

	public int CalculateOperationPower()
	{
		int num = 0;
		int num2 = 0;
		NKMUnitData nKMUnitData = CreateShip(-1L);
		if (nKMUnitData != null)
		{
			num2++;
			num += nKMUnitData.CalculateOperationPower(null);
		}
		for (int i = 0; i < List.Length; i++)
		{
			NKMDummyUnitData nKMDummyUnitData = List[i];
			if (nKMDummyUnitData != null)
			{
				NKMUnitData nKMUnitData2 = nKMDummyUnitData.ToUnitData(-1L);
				num2++;
				num += nKMUnitData2.CalculateOperationPower(null);
			}
		}
		if (num2 == 0)
		{
			return 0;
		}
		return (int)((float)(num / num2) + (3f - CalculateSummonCost()) * (float)num / (float)num2 / 10f);
	}

	public int CalculateOperationPowerForPrivatePvp()
	{
		int num = 0;
		int num2 = 0;
		NKMUnitData nKMUnitData = CreateShip(-1L);
		if (nKMUnitData != null)
		{
			num += nKMUnitData.CalculateOperationPower(null);
		}
		for (int i = 0; i < List.Length; i++)
		{
			NKMDummyUnitData nKMDummyUnitData = List[i];
			if (nKMDummyUnitData != null)
			{
				NKMUnitData nKMUnitData2 = nKMDummyUnitData.ToUnitData(-1L);
				num2++;
				num += nKMUnitData2.CalculateOperationPower(null);
			}
		}
		if (num2 == 0)
		{
			return 0;
		}
		float num3 = CalculateSummonCost();
		return (int)(((float)(num / (num2 + 1)) + (3f - num3) * (float)num / (float)(num2 + 1) / 10f) * ((float)num2 / 8f));
	}

	public float CalculateSummonCost()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < List.Length; i++)
		{
			NKMDummyUnitData nKMDummyUnitData = List[i];
			if (nKMDummyUnitData != null)
			{
				NKMUnitData nKMUnitData = new NKMUnitData();
				nKMUnitData.FillDataFromDummy(nKMDummyUnitData);
				num2++;
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(nKMUnitData.m_UnitID);
				if (unitStatTemplet != null)
				{
					num = ((LeaderIndex != i) ? (num + unitStatTemplet.GetRespawnCost(bLeader: false, null, null)) : (num + unitStatTemplet.GetRespawnCost(bLeader: true, null, null)));
				}
			}
		}
		if (num2 == 0)
		{
			return 0f;
		}
		return (float)num / (float)num2;
	}

	public float CalculateAvgSummonCost(Dictionary<int, NKMBanData> dicNKMBanData = null, Dictionary<int, NKMUnitUpData> dicNKMUpData = null)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < List.Length; i++)
		{
			NKMDummyUnitData nKMDummyUnitData = List[i];
			if (nKMDummyUnitData != null)
			{
				NKMUnitData nKMUnitData = new NKMUnitData();
				nKMUnitData.FillDataFromDummy(nKMDummyUnitData);
				num2++;
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(nKMUnitData.m_UnitID);
				if (unitStatTemplet != null)
				{
					num = ((LeaderIndex != i) ? (num + unitStatTemplet.GetRespawnCost(bLeader: false, dicNKMBanData, dicNKMUpData)) : (num + unitStatTemplet.GetRespawnCost(bLeader: true, dicNKMBanData, dicNKMUpData)));
				}
			}
		}
		if (num2 == 0)
		{
			return 0f;
		}
		return (float)num / (float)num2;
	}
}
