using System.Collections.Generic;

namespace NKM;

public class NKMDungeonDefenceData
{
	public int m_maxWaveCount;

	public float m_waveInterval;

	public Dictionary<int, List<NKMDungeonRespawnUnitTemplet>> m_dungeonRespawnUnitTempletListA = new Dictionary<int, List<NKMDungeonRespawnUnitTemplet>>();

	public Dictionary<int, List<NKMDungeonRespawnUnitTemplet>> m_dungeonRespawnUnitTempletListB = new Dictionary<int, List<NKMDungeonRespawnUnitTemplet>>();

	public void ResetData()
	{
		m_maxWaveCount = 0;
		m_waveInterval = 0f;
		m_dungeonRespawnUnitTempletListA.Clear();
		m_dungeonRespawnUnitTempletListB.Clear();
	}

	public List<NKMDungeonRespawnUnitTemplet> GetDungeonRespawnUnitTempletList(int waveId, bool teamA)
	{
		List<NKMDungeonRespawnUnitTemplet> value2;
		if (teamA)
		{
			if (m_dungeonRespawnUnitTempletListA.TryGetValue(waveId, out var value))
			{
				return value;
			}
		}
		else if (m_dungeonRespawnUnitTempletListB.TryGetValue(waveId, out value2))
		{
			return value2;
		}
		return new List<NKMDungeonRespawnUnitTemplet>();
	}

	public void AddDungeonRespawnUnitTemplet(DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE respawnUnitTempletType, NKMDungeonTempletBase dungeonTempletBase, NKMDungeonRespawnUnitTemplet dungeonRespawnUnitTemplet, List<int> includeWaveList)
	{
		foreach (int includeWave in includeWaveList)
		{
			List<NKMDungeonRespawnUnitTemplet> value = null;
			switch (respawnUnitTempletType)
			{
			case DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE.WAVE_DUNGEON_UNIT_RESPAWN_A:
				if (!m_dungeonRespawnUnitTempletListA.TryGetValue(includeWave, out value))
				{
					value = new List<NKMDungeonRespawnUnitTemplet>();
					m_dungeonRespawnUnitTempletListA.Add(includeWave, value);
				}
				break;
			case DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE.WAVE_DUNGEON_UNIT_RESPAWN_B:
				if (!m_dungeonRespawnUnitTempletListB.TryGetValue(includeWave, out value))
				{
					value = new List<NKMDungeonRespawnUnitTemplet>();
					m_dungeonRespawnUnitTempletListB.Add(includeWave, value);
				}
				break;
			}
			if (value != null)
			{
				NKMDungeonRespawnUnitTemplet nKMDungeonRespawnUnitTemplet = new NKMDungeonRespawnUnitTemplet();
				nKMDungeonRespawnUnitTemplet.DeecCopyFromSource(dungeonRespawnUnitTemplet);
				nKMDungeonRespawnUnitTemplet.RegisterDungeonrEspawnUnitTemplet(dungeonTempletBase, respawnUnitTempletType, includeWave, value.Count + 1);
				nKMDungeonRespawnUnitTemplet.m_WaveID = includeWave;
				value.Add(nKMDungeonRespawnUnitTemplet);
			}
		}
	}

	public void LoadRespawnDataFromLua(NKMLua cNKMLua, NKMDungeonTempletBase cNKMDungeonTempletBase, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE respawnUnitTempletType)
	{
		for (int i = 1; cNKMLua.OpenTable(i); i++)
		{
			NKMDungeonRespawnUnitTemplet nKMDungeonRespawnUnitTemplet = new NKMDungeonRespawnUnitTemplet();
			nKMDungeonRespawnUnitTemplet.LoadFromLUA(cNKMLua, cNKMDungeonTempletBase);
			List<int> list = new List<int>();
			int rValue = 0;
			cNKMLua.GetData("waveId", ref rValue);
			if (rValue != 0)
			{
				list.Add(rValue);
			}
			else
			{
				int rValue2 = 0;
				int rValue3 = 0;
				int rValue4 = 0;
				cNKMLua.GetData("startWaveId", ref rValue2);
				cNKMLua.GetData("endWaveId", ref rValue3);
				cNKMLua.GetData("respawnInterval", ref rValue4);
				rValue3 = ((rValue3 != 0) ? (rValue3 + 1) : (m_maxWaveCount + 1));
				for (int j = rValue2; j < rValue3; j++)
				{
					if (rValue4 == 0 || j % rValue4 == 0)
					{
						list.Add(j);
					}
				}
			}
			AddDungeonRespawnUnitTemplet(respawnUnitTempletType, cNKMDungeonTempletBase, nKMDungeonRespawnUnitTemplet, list);
			cNKMLua.CloseTable();
		}
	}

	public bool LoadFromLua(NKMLua cNKMLua, NKMDungeonTempletBase cNKMDungeonTempletBase)
	{
		ResetData();
		cNKMLua.GetData("m_maxWaveCount", ref m_maxWaveCount);
		cNKMLua.GetData("m_waveInterval", ref m_waveInterval);
		if (cNKMLua.OpenTable("m_listDungeonUnitRespawnA"))
		{
			LoadRespawnDataFromLua(cNKMLua, cNKMDungeonTempletBase, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE.WAVE_DUNGEON_UNIT_RESPAWN_A);
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_listDungeonUnitRespawnB"))
		{
			LoadRespawnDataFromLua(cNKMLua, cNKMDungeonTempletBase, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE.WAVE_DUNGEON_UNIT_RESPAWN_B);
			cNKMLua.CloseTable();
		}
		return true;
	}
}
