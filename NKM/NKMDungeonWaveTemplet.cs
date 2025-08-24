using System.Collections.Generic;

namespace NKM;

public class NKMDungeonWaveTemplet
{
	public int m_WaveID;

	public int m_NextWaveID;

	public float m_fNextWavetime;

	public List<NKMDungeonRespawnUnitTemplet> m_listDungeonUnitRespawnA = new List<NKMDungeonRespawnUnitTemplet>();

	public List<NKMDungeonRespawnUnitTemplet> m_listDungeonUnitRespawnB = new List<NKMDungeonRespawnUnitTemplet>();

	public void Validate(string dungeonStrID)
	{
		if (m_listDungeonUnitRespawnA != null)
		{
			foreach (NKMDungeonRespawnUnitTemplet item in m_listDungeonUnitRespawnA)
			{
				item.Validate(dungeonStrID);
			}
		}
		if (m_listDungeonUnitRespawnB == null)
		{
			return;
		}
		foreach (NKMDungeonRespawnUnitTemplet item2 in m_listDungeonUnitRespawnB)
		{
			item2.Validate(dungeonStrID);
		}
	}

	public bool LoadFromLUA(NKMLua cNKMLua, NKMDungeonTempletBase cNKMDungeonTempletBase)
	{
		cNKMLua.GetData("m_WaveID", ref m_WaveID);
		cNKMLua.GetData("m_NextWaveID", ref m_NextWaveID);
		cNKMLua.GetData("m_fNextWavetime", ref m_fNextWavetime);
		if (cNKMLua.OpenTable("m_listDungeonUnitRespawnA"))
		{
			for (int i = 1; cNKMLua.OpenTable(i); i++)
			{
				NKMDungeonRespawnUnitTemplet nKMDungeonRespawnUnitTemplet = null;
				if (m_listDungeonUnitRespawnA.Count >= i)
				{
					nKMDungeonRespawnUnitTemplet = m_listDungeonUnitRespawnA[i - 1];
				}
				else
				{
					nKMDungeonRespawnUnitTemplet = new NKMDungeonRespawnUnitTemplet();
					m_listDungeonUnitRespawnA.Add(nKMDungeonRespawnUnitTemplet);
				}
				nKMDungeonRespawnUnitTemplet.LoadFromLUA(cNKMLua, cNKMDungeonTempletBase, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE.WAVE_DUNGEON_UNIT_RESPAWN_A, m_WaveID, i);
				nKMDungeonRespawnUnitTemplet.m_WaveID = m_WaveID;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_listDungeonUnitRespawnB"))
		{
			for (int j = 1; cNKMLua.OpenTable(j); j++)
			{
				NKMDungeonRespawnUnitTemplet nKMDungeonRespawnUnitTemplet2 = null;
				if (m_listDungeonUnitRespawnB.Count >= j)
				{
					nKMDungeonRespawnUnitTemplet2 = m_listDungeonUnitRespawnB[j - 1];
				}
				else
				{
					nKMDungeonRespawnUnitTemplet2 = new NKMDungeonRespawnUnitTemplet();
					m_listDungeonUnitRespawnB.Add(nKMDungeonRespawnUnitTemplet2);
				}
				nKMDungeonRespawnUnitTemplet2.LoadFromLUA(cNKMLua, cNKMDungeonTempletBase, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE.WAVE_DUNGEON_UNIT_RESPAWN_B, m_WaveID, j);
				nKMDungeonRespawnUnitTemplet2.m_WaveID = m_WaveID;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		return true;
	}
}
