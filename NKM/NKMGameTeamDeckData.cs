using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMGameTeamDeckData : ISerializable
{
	private byte m_DataEncryptSeed;

	private List<long> m_listUnitDeck = new List<long>();

	private long m_NextDeck;

	private List<long> m_listUnitDeckUsed = new List<long>();

	private List<long> m_listUnitDeckTomb = new List<long>();

	private int m_AutoRespawnIndex;

	private int m_AutoRespawnIndexAssist;

	private Dictionary<long, int> m_dicRespawnLimitCount = new Dictionary<long, int>();

	public void AddListUnitDeck(long unitUID)
	{
		long target = 0L;
		NKMUtil.SimpleEncrypt(m_DataEncryptSeed, ref target, unitUID);
		m_listUnitDeck.Add(target);
	}

	public void SetListUnitDeck(int index, long unitUID)
	{
		long target = 0L;
		NKMUtil.SimpleEncrypt(m_DataEncryptSeed, ref target, unitUID);
		m_listUnitDeck[index] = target;
	}

	public int GetListUnitDeckCount()
	{
		return m_listUnitDeck.Count;
	}

	public long GetListUnitDeck(int index)
	{
		return NKMUtil.SimpleDecrypt(m_DataEncryptSeed, m_listUnitDeck[index]);
	}

	public void AddListUnitDeckUsed(long unitUID)
	{
		long target = 0L;
		NKMUtil.SimpleEncrypt(m_DataEncryptSeed, ref target, unitUID);
		m_listUnitDeckUsed.Add(target);
	}

	public void RemoveAtListUnitDeckUsed(int index)
	{
		m_listUnitDeckUsed.RemoveAt(index);
	}

	public void ClearListUnitDeckUsed()
	{
		m_listUnitDeckUsed.Clear();
	}

	public void SetListUnitDeckUsed(int index, long unitUID)
	{
		long target = 0L;
		NKMUtil.SimpleEncrypt(m_DataEncryptSeed, ref target, unitUID);
		m_listUnitDeckUsed[index] = target;
	}

	public int GetListUnitDeckUsedCount()
	{
		return m_listUnitDeckUsed.Count;
	}

	public long GetListUnitDeckUsed(int index)
	{
		return NKMUtil.SimpleDecrypt(m_DataEncryptSeed, m_listUnitDeckUsed[index]);
	}

	public void AddListUnitDeckTomb(long unitUID)
	{
		long target = 0L;
		NKMUtil.SimpleEncrypt(m_DataEncryptSeed, ref target, unitUID);
		m_listUnitDeckTomb.Add(target);
	}

	public int GetListUnitDeckTombCount()
	{
		return m_listUnitDeckTomb.Count;
	}

	public void SetNextDeck(long nextDeck)
	{
		NKMUtil.SimpleEncrypt(m_DataEncryptSeed, ref m_NextDeck, nextDeck);
	}

	public void SetAutoRespawnIndex(int autoRespawnIndex)
	{
		NKMUtil.SimpleEncrypt(m_DataEncryptSeed, ref m_AutoRespawnIndex, autoRespawnIndex);
	}

	public void SetAutoRespawnIndexAssist(int autoRespawnIndexAssist)
	{
		NKMUtil.SimpleEncrypt(m_DataEncryptSeed, ref m_AutoRespawnIndexAssist, autoRespawnIndexAssist);
	}

	public long GetNextDeck()
	{
		return NKMUtil.SimpleDecrypt(m_DataEncryptSeed, m_NextDeck);
	}

	public int GetAutoRespawnIndex()
	{
		return NKMUtil.SimpleDecrypt(m_DataEncryptSeed, m_AutoRespawnIndex);
	}

	public int GetAutoRespawnIndexAssist()
	{
		return NKMUtil.SimpleDecrypt(m_DataEncryptSeed, m_AutoRespawnIndexAssist);
	}

	public NKMGameTeamDeckData()
	{
		m_DataEncryptSeed = (byte)NKMRandom.Range(10, 100);
		for (int i = 0; i < 4; i++)
		{
			AddListUnitDeck(0L);
		}
	}

	public void Init()
	{
		m_DataEncryptSeed = (byte)NKMRandom.Range(10, 100);
		SetNextDeck(0L);
		for (int i = 0; i < GetListUnitDeckCount(); i++)
		{
			SetListUnitDeck(i, 0L);
		}
		m_listUnitDeckUsed.Clear();
		m_listUnitDeckTomb.Clear();
		SetAutoRespawnIndex(0);
		SetAutoRespawnIndexAssist(0);
	}

	public void SetRespawnLimitCount(long unitUID, int count)
	{
		m_dicRespawnLimitCount[unitUID] = count;
	}

	public int GetRespawnLimitCount(long unitUID)
	{
		if (m_dicRespawnLimitCount.TryGetValue(unitUID, out var value))
		{
			return value;
		}
		return -1;
	}

	public void InitRespawnLimitCount(List<NKMUnitData> lstUnitData)
	{
		foreach (NKMUnitData lstUnitDatum in lstUnitData)
		{
			int respawnLimitCount = lstUnitDatum.GetUnitTempletBase().GetRespawnLimitCount();
			if (respawnLimitCount > 0)
			{
				SetRespawnLimitCount(lstUnitDatum.m_UnitUID, respawnLimitCount);
			}
		}
	}

	public void DecreaseRespawnLimitCount(long unitUID)
	{
		if (m_dicRespawnLimitCount.TryGetValue(unitUID, out var value))
		{
			m_dicRespawnLimitCount[unitUID] = value - 1;
		}
	}

	public bool IsRespawnLimitCountLeft(long unitUID)
	{
		if (m_dicRespawnLimitCount.TryGetValue(unitUID, out var value))
		{
			return value > 0;
		}
		return true;
	}

	public virtual void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_DataEncryptSeed);
		stream.PutOrGet(ref m_listUnitDeck);
		stream.PutOrGet(ref m_NextDeck);
		stream.PutOrGet(ref m_listUnitDeckUsed);
		stream.PutOrGet(ref m_listUnitDeckTomb);
		stream.PutOrGet(ref m_AutoRespawnIndex);
		stream.PutOrGet(ref m_AutoRespawnIndexAssist);
		stream.PutOrGet(ref m_dicRespawnLimitCount);
	}
}
