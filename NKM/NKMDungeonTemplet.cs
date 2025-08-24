using System.Collections.Generic;
using Cs.Logging;

namespace NKM;

public class NKMDungeonTemplet
{
	public bool m_bLoaded;

	public NKMDungeonTempletBase m_DungeonTempletBase;

	public bool m_bCanUseAuto = true;

	public bool m_bRespawnFreePos;

	public float m_fStartCost;

	public float m_fCostSpeedRateA = 1f;

	public float m_fCostSpeedRateB = 1f;

	public string m_BossUnitStrID = "";

	public int m_BossUnitLevel;

	public string m_BossUnitChangeName = "";

	public float m_fBossPosZ = 1f;

	public NKMDungeonRespawnUnitTemplet m_BossRespawnUnitTemplet = new NKMDungeonRespawnUnitTemplet();

	public bool m_bNoTimeStop;

	public bool m_bNoEnemyRespawnBeforeUserFirstRespawn;

	public float m_fAllyHyperCooltimeStartRatio = -1f;

	public float m_fEnemyHyperCooltimeStartRatio = -1f;

	public List<NKMDungeonRespawnUnitTemplet> m_listDungeonDeck = new List<NKMDungeonRespawnUnitTemplet>();

	public List<NKMDungeonWaveTemplet> m_listDungeonWave = new List<NKMDungeonWaveTemplet>();

	public List<NKMDungeonRespawnUnitTemplet> m_listDungeonUnitRespawnA = new List<NKMDungeonRespawnUnitTemplet>();

	public List<NKMDungeonRespawnUnitTemplet> m_listDungeonUnitRespawnB = new List<NKMDungeonRespawnUnitTemplet>();

	public List<NKMDungeonEventTemplet> m_listDungeonEventTempletTeamA = new List<NKMDungeonEventTemplet>();

	public List<NKMDungeonEventTemplet> m_listDungeonEventTempletTeamB = new List<NKMDungeonEventTemplet>();

	public NKMDungeonDefenceData m_DungeonDefenceData = new NKMDungeonDefenceData();

	public string m_DungeonTag;

	public List<float> m_listValidLand;

	public bool m_bDevForcePVP;

	public bool LoadFromLUA(NKMLua cNKMLua, NKMDungeonTempletBase cNKMDungeonTempletBase)
	{
		m_DungeonTempletBase = cNKMDungeonTempletBase;
		cNKMLua.GetData("m_bCanUseAuto", ref m_bCanUseAuto);
		cNKMLua.GetData("m_fStartCost", ref m_fStartCost);
		cNKMLua.GetData("m_bRespawnFreePos", ref m_bRespawnFreePos);
		cNKMLua.GetData("m_fCostSpeedRateA", ref m_fCostSpeedRateA);
		cNKMLua.GetData("m_fCostSpeedRateB", ref m_fCostSpeedRateB);
		cNKMLua.GetData("m_BossUnitStrID", ref m_BossUnitStrID);
		cNKMLua.GetData("m_BossUnitChangeName", ref m_BossUnitChangeName);
		cNKMLua.GetData("m_BossUnitLevel", ref m_BossUnitLevel);
		if (m_BossUnitLevel == 0)
		{
			m_BossUnitLevel = cNKMDungeonTempletBase.m_DungeonLevel;
		}
		cNKMLua.GetData("m_fBossPosZ", ref m_fBossPosZ);
		if (cNKMLua.OpenTable("m_BossRespawnUnitTemplet"))
		{
			m_BossRespawnUnitTemplet.LoadFromLUA(cNKMLua, cNKMDungeonTempletBase, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE.BOSS_RESPAWN_UNIT, 0, 0);
			cNKMLua.CloseTable();
		}
		if (!string.IsNullOrEmpty(m_BossUnitChangeName))
		{
			m_BossRespawnUnitTemplet.m_ChangeUnitName = m_BossUnitChangeName;
		}
		cNKMLua.GetData("m_bNoTimeStop", ref m_bNoTimeStop);
		cNKMLua.GetData("m_bNoEnemyRespawnBeforeUserFirstRespawn", ref m_bNoEnemyRespawnBeforeUserFirstRespawn);
		cNKMLua.GetData("m_fAllyHyperCooltimeStartRatio", ref m_fAllyHyperCooltimeStartRatio);
		cNKMLua.GetData("m_fEnemyHyperCooltimeStartRatio", ref m_fEnemyHyperCooltimeStartRatio);
		cNKMLua.GetData("m_DungeonTag", ref m_DungeonTag);
		cNKMLua.GetData("m_bDevForcePVP", ref m_bDevForcePVP);
		if (cNKMLua.GetDataList("m_listValidLand", out m_listValidLand, nullIfEmpty: false))
		{
			if (m_listValidLand.Count != 3)
			{
				Log.Error(cNKMDungeonTempletBase.DebugName + " : m_listValidLand count must 3!!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 868);
				m_listValidLand = null;
			}
		}
		else
		{
			m_listValidLand = null;
		}
		if (cNKMLua.OpenTable("m_listDungeonDeck"))
		{
			for (int i = 1; cNKMLua.OpenTable(i); i++)
			{
				NKMDungeonRespawnUnitTemplet nKMDungeonRespawnUnitTemplet = null;
				if (m_listDungeonDeck.Count >= i)
				{
					nKMDungeonRespawnUnitTemplet = m_listDungeonDeck[i - 1];
				}
				else
				{
					nKMDungeonRespawnUnitTemplet = new NKMDungeonRespawnUnitTemplet();
					m_listDungeonDeck.Add(nKMDungeonRespawnUnitTemplet);
				}
				nKMDungeonRespawnUnitTemplet.LoadFromLUA(cNKMLua, cNKMDungeonTempletBase, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE.DUNGEON_DECK, 0, i);
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_DungeonDefenceData"))
		{
			if (!m_DungeonDefenceData.LoadFromLua(cNKMLua, m_DungeonTempletBase))
			{
				Log.ErrorAndExit($"[NKMDungeonDefenceData] 저지전 던전 스크립트 웨이브데이터 읽기 실패 - DungeonID[{m_DungeonTempletBase.m_DungeonID}:{m_DungeonTempletBase.m_DungeonStrID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 904);
			}
			m_listDungeonWave.Clear();
			for (int j = 0; j < m_DungeonDefenceData.m_maxWaveCount; j++)
			{
				NKMDungeonWaveTemplet nKMDungeonWaveTemplet = new NKMDungeonWaveTemplet();
				nKMDungeonWaveTemplet.m_fNextWavetime = m_DungeonDefenceData.m_waveInterval;
				nKMDungeonWaveTemplet.m_WaveID = j + 1;
				if (j + 1 < m_DungeonDefenceData.m_maxWaveCount)
				{
					nKMDungeonWaveTemplet.m_NextWaveID = nKMDungeonWaveTemplet.m_WaveID + 1;
				}
				nKMDungeonWaveTemplet.m_listDungeonUnitRespawnA = m_DungeonDefenceData.GetDungeonRespawnUnitTempletList(nKMDungeonWaveTemplet.m_WaveID, teamA: true);
				nKMDungeonWaveTemplet.m_listDungeonUnitRespawnB = m_DungeonDefenceData.GetDungeonRespawnUnitTempletList(nKMDungeonWaveTemplet.m_WaveID, teamA: false);
				m_listDungeonWave.Add(nKMDungeonWaveTemplet);
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_listDungeonWave"))
		{
			for (int k = 1; cNKMLua.OpenTable(k); k++)
			{
				NKMDungeonWaveTemplet nKMDungeonWaveTemplet2 = null;
				if (m_listDungeonWave.Count >= k)
				{
					nKMDungeonWaveTemplet2 = m_listDungeonWave[k - 1];
				}
				else
				{
					nKMDungeonWaveTemplet2 = new NKMDungeonWaveTemplet();
					m_listDungeonWave.Add(nKMDungeonWaveTemplet2);
				}
				nKMDungeonWaveTemplet2.LoadFromLUA(cNKMLua, cNKMDungeonTempletBase);
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_listDungeonUnitRespawnA"))
		{
			for (int l = 1; cNKMLua.OpenTable(l); l++)
			{
				NKMDungeonRespawnUnitTemplet nKMDungeonRespawnUnitTemplet2 = null;
				if (m_listDungeonUnitRespawnA.Count >= l)
				{
					nKMDungeonRespawnUnitTemplet2 = m_listDungeonUnitRespawnA[l - 1];
				}
				else
				{
					nKMDungeonRespawnUnitTemplet2 = new NKMDungeonRespawnUnitTemplet();
					m_listDungeonUnitRespawnA.Add(nKMDungeonRespawnUnitTemplet2);
				}
				nKMDungeonRespawnUnitTemplet2.LoadFromLUA(cNKMLua, cNKMDungeonTempletBase, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE.DUNGEON_UNIT_RESPAWN_A, 0, l);
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_listDungeonUnitRespawnB"))
		{
			for (int m = 1; cNKMLua.OpenTable(m); m++)
			{
				NKMDungeonRespawnUnitTemplet nKMDungeonRespawnUnitTemplet3 = null;
				if (m_listDungeonUnitRespawnB.Count >= m)
				{
					nKMDungeonRespawnUnitTemplet3 = m_listDungeonUnitRespawnB[m - 1];
				}
				else
				{
					nKMDungeonRespawnUnitTemplet3 = new NKMDungeonRespawnUnitTemplet();
					m_listDungeonUnitRespawnB.Add(nKMDungeonRespawnUnitTemplet3);
				}
				nKMDungeonRespawnUnitTemplet3.LoadFromLUA(cNKMLua, cNKMDungeonTempletBase, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE.DUNGEON_UNIT_RESPAWN_B, 0, m);
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_listDungeonEventTempletTeamA"))
		{
			for (int n = 1; cNKMLua.OpenTable(n); n++)
			{
				NKMDungeonEventTemplet nKMDungeonEventTemplet = null;
				if (m_listDungeonEventTempletTeamA.Count >= n)
				{
					nKMDungeonEventTemplet = m_listDungeonEventTempletTeamA[n - 1];
				}
				else
				{
					nKMDungeonEventTemplet = new NKMDungeonEventTemplet();
					m_listDungeonEventTempletTeamA.Add(nKMDungeonEventTemplet);
				}
				nKMDungeonEventTemplet.LoadFromLUA(cNKMLua);
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_listDungeonEventTempletTeamB"))
		{
			for (int num = 1; cNKMLua.OpenTable(num); num++)
			{
				NKMDungeonEventTemplet nKMDungeonEventTemplet2 = null;
				if (m_listDungeonEventTempletTeamB.Count >= num)
				{
					nKMDungeonEventTemplet2 = m_listDungeonEventTempletTeamB[num - 1];
				}
				else
				{
					nKMDungeonEventTemplet2 = new NKMDungeonEventTemplet();
					m_listDungeonEventTempletTeamB.Add(nKMDungeonEventTemplet2);
				}
				nKMDungeonEventTemplet2.LoadFromLUA(cNKMLua);
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		m_bLoaded = true;
		return true;
	}

	public NKMDungeonWaveTemplet GetWaveTemplet(int waveID)
	{
		for (int i = 0; i < m_listDungeonWave.Count; i++)
		{
			NKMDungeonWaveTemplet nKMDungeonWaveTemplet = m_listDungeonWave[i];
			if (nKMDungeonWaveTemplet != null && nKMDungeonWaveTemplet.m_WaveID == waveID)
			{
				return nKMDungeonWaveTemplet;
			}
		}
		return null;
	}

	public int GetNextWave(int waveID)
	{
		for (int i = 0; i < m_listDungeonWave.Count; i++)
		{
			NKMDungeonWaveTemplet nKMDungeonWaveTemplet = m_listDungeonWave[i];
			if (nKMDungeonWaveTemplet != null && nKMDungeonWaveTemplet.m_WaveID == waveID)
			{
				return nKMDungeonWaveTemplet.m_NextWaveID;
			}
		}
		return 0;
	}

	public bool CheckValidWave(int waveID)
	{
		for (int i = 0; i < m_listDungeonWave.Count; i++)
		{
			NKMDungeonWaveTemplet nKMDungeonWaveTemplet = m_listDungeonWave[i];
			if (nKMDungeonWaveTemplet != null && nKMDungeonWaveTemplet.m_WaveID == waveID)
			{
				return true;
			}
		}
		return false;
	}
}
