using System;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Logging;
using Cs.Protocol;

namespace NKM;

public sealed class NKMGameData : ISerializable
{
	public long m_GameUID;

	public short m_GameUnitUIDIndex;

	public bool m_bLocal;

	public NKM_GAME_TYPE m_NKM_GAME_TYPE;

	public int m_DungeonID;

	public bool m_bBossDungeon;

	public int m_WarfareID;

	public long m_RaidUID;

	public float m_fRespawnCostMinusPercentForTeamA;

	public int m_TeamASupply;

	public float m_fTeamAAttackPowerIncRateForWarfare;

	public List<string> m_lstTeamABuffStrIDListForRaid = new List<string>();

	public float fExtraRespawnCostAddForA;

	public float fExtraRespawnCostAddForB;

	public int m_TeamBLevelAdd;

	public int m_TeamBLevelFix;

	public List<string> m_BanUnitBuffStrIDs = new List<string>();

	public bool m_bForcedAuto;

	public float m_fDoubleCostTime = 60f;

	public int m_MapID;

	public Dictionary<int, int> m_BattleConditionIDs = new Dictionary<int, int>();

	public NKMGameTeamData m_NKMGameTeamDataA = new NKMGameTeamData();

	public NKMGameTeamData m_NKMGameTeamDataB = new NKMGameTeamData();

	private List<long> m_listUnitDeckTemp = new List<long>();

	public bool m_replay;

	public Dictionary<int, NKMBanData> m_dicNKMBanData = new Dictionary<int, NKMBanData>();

	public Dictionary<int, NKMBanShipData> m_dicNKMBanShipData = new Dictionary<int, NKMBanShipData>();

	public Dictionary<int, NKMBanOperatorData> m_dicNKMBanOperatorData = new Dictionary<int, NKMBanOperatorData>();

	public Dictionary<int, NKMUnitUpData> m_dicNKMUpData = new Dictionary<int, NKMUnitUpData>();

	public string m_NKMGameStatRateID;

	private NKMGameStatRate m_GameStatRateCache;

	private bool m_bGameStatCacheSet;

	public bool isSurrenderGame;

	public NKMGameStatRate GameStatRate
	{
		get
		{
			if (!m_bGameStatCacheSet && m_GameStatRateCache == null)
			{
				m_bGameStatCacheSet = true;
				if (!string.IsNullOrEmpty(m_NKMGameStatRateID))
				{
					NKMGameStatRateTemplet nKMGameStatRateTemplet = NKMGameStatRateTemplet.Find(m_NKMGameStatRateID);
					if (nKMGameStatRateTemplet == null)
					{
						Log.Error("GameStatRateTemplet " + m_NKMGameStatRateID + " not found!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGame.cs", 1370);
					}
					else
					{
						m_GameStatRateCache = nKMGameStatRateTemplet.m_StatRate;
					}
				}
			}
			return m_GameStatRateCache;
		}
	}

	public bool IsPVPLeague => m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_LEAGUE;

	public NKMGameData()
	{
		m_NKMGameTeamDataA.Init();
		m_NKMGameTeamDataB.Init();
		m_NKMGameTeamDataA.m_eNKM_TEAM_TYPE = NKM_TEAM_TYPE.NTT_A1;
		m_NKMGameTeamDataB.m_eNKM_TEAM_TYPE = NKM_TEAM_TYPE.NTT_B1;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_GameUID);
		stream.PutOrGet(ref m_GameUnitUIDIndex);
		stream.PutOrGet(ref m_bLocal);
		stream.PutOrGetEnum(ref m_NKM_GAME_TYPE);
		stream.PutOrGet(ref m_DungeonID);
		stream.PutOrGet(ref m_bBossDungeon);
		stream.PutOrGet(ref m_WarfareID);
		stream.PutOrGet(ref m_RaidUID);
		stream.PutOrGet(ref m_fRespawnCostMinusPercentForTeamA);
		stream.PutOrGet(ref m_TeamASupply);
		stream.PutOrGet(ref m_fTeamAAttackPowerIncRateForWarfare);
		stream.PutOrGet(ref m_lstTeamABuffStrIDListForRaid);
		stream.PutOrGet(ref fExtraRespawnCostAddForA);
		stream.PutOrGet(ref fExtraRespawnCostAddForB);
		stream.PutOrGet(ref m_TeamBLevelAdd);
		stream.PutOrGet(ref m_TeamBLevelFix);
		stream.PutOrGet(ref m_fDoubleCostTime);
		stream.PutOrGet(ref m_MapID);
		stream.PutOrGet(ref m_BattleConditionIDs);
		stream.PutOrGet(ref m_NKMGameTeamDataA);
		stream.PutOrGet(ref m_NKMGameTeamDataB);
		stream.PutOrGet(ref m_listUnitDeckTemp);
		stream.PutOrGet(ref m_replay);
		stream.PutOrGet(ref m_dicNKMBanData);
		stream.PutOrGet(ref m_dicNKMBanShipData);
		stream.PutOrGet(ref m_dicNKMBanOperatorData);
		stream.PutOrGet(ref m_dicNKMUpData);
		stream.PutOrGet(ref m_NKMGameStatRateID);
		stream.PutOrGet(ref m_bForcedAuto);
	}

	public bool IsSameData(NKMGameData gameData)
	{
		if (gameData == null)
		{
			return false;
		}
		if (m_bLocal != gameData.m_bLocal)
		{
			return false;
		}
		if (m_GameUID != gameData.m_GameUID)
		{
			return false;
		}
		if (m_NKM_GAME_TYPE != gameData.m_NKM_GAME_TYPE)
		{
			return false;
		}
		if (m_DungeonID != gameData.m_DungeonID)
		{
			return false;
		}
		return true;
	}

	public void ApplyBanUpLevelToUnits(bool isAsyncPvp = false)
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_BAN_UPDATE))
		{
			return;
		}
		List<List<NKMUnitData>> list = new List<List<NKMUnitData>>();
		if (!isAsyncPvp)
		{
			list.Add(m_NKMGameTeamDataA.m_listUnitData);
		}
		list.Add(m_NKMGameTeamDataB.m_listUnitData);
		foreach (List<NKMUnitData> item in list)
		{
			foreach (NKMUnitData item2 in item)
			{
				if (IsBanUnit(item2.m_UnitID))
				{
					int banUnitLevel = GetBanUnitLevel(item2.m_UnitID);
					if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.UNIT_REACTOR))
					{
						item2.reactorLevel = Math.Max(0, item2.reactorLevel - banUnitLevel);
					}
				}
			}
		}
	}

	public void SetGameType(NKM_GAME_TYPE gameType)
	{
		m_NKM_GAME_TYPE = gameType;
	}

	public void SetDungeonRespawnUnitTemplet()
	{
		if (m_NKMGameTeamDataB.m_MainShip != null)
		{
			m_NKMGameTeamDataB.m_MainShip.SetDungeonRespawnUnitTemplet();
		}
		for (int i = 0; i < m_NKMGameTeamDataB.m_listUnitData.Count; i++)
		{
			m_NKMGameTeamDataB.m_listUnitData[i].SetDungeonRespawnUnitTemplet();
		}
		for (int j = 0; j < m_NKMGameTeamDataA.m_listEvevtUnitData.Count; j++)
		{
			m_NKMGameTeamDataA.m_listEvevtUnitData[j].SetDungeonRespawnUnitTemplet();
		}
		for (int k = 0; k < m_NKMGameTeamDataB.m_listEvevtUnitData.Count; k++)
		{
			m_NKMGameTeamDataB.m_listEvevtUnitData[k].SetDungeonRespawnUnitTemplet();
		}
	}

	public NKM_GAME_TYPE GetGameType()
	{
		return m_NKM_GAME_TYPE;
	}

	public NKMUnitData GetUnitDataByUnitUID(long unitUID)
	{
		return m_NKMGameTeamDataA.GetUnitDataByUnitUID(unitUID) ?? m_NKMGameTeamDataB.GetUnitDataByUnitUID(unitUID);
	}

	public NKM_TEAM_TYPE GetTeamType(long uid)
	{
		NKMGameTeamData teamData = GetTeamData(uid);
		if (teamData != null)
		{
			return teamData.m_eNKM_TEAM_TYPE;
		}
		Log.Error($"Can't find Team of uid {uid}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGame.cs", 1528);
		return NKM_TEAM_TYPE.NTT_INVALID;
	}

	public NKMGameTeamData GetTeamData(long uid)
	{
		if (m_NKMGameTeamDataA != null && m_NKMGameTeamDataA.m_user_uid == uid)
		{
			return m_NKMGameTeamDataA;
		}
		if (m_NKMGameTeamDataB != null && m_NKMGameTeamDataB.m_user_uid == uid)
		{
			return m_NKMGameTeamDataB;
		}
		return null;
	}

	public NKMGameTeamData GetTeamData(NKM_TEAM_TYPE myTeamType)
	{
		return myTeamType switch
		{
			NKM_TEAM_TYPE.NTT_A1 => m_NKMGameTeamDataA, 
			NKM_TEAM_TYPE.NTT_B1 => m_NKMGameTeamDataB, 
			_ => null, 
		};
	}

	public bool IsLeaderUnit(long unitUID)
	{
		if (m_NKMGameTeamDataA != null && m_NKMGameTeamDataA.GetLeaderUnitData() != null && m_NKMGameTeamDataA.GetLeaderUnitData().m_UnitUID == unitUID)
		{
			return true;
		}
		if (m_NKMGameTeamDataB != null && m_NKMGameTeamDataB.GetLeaderUnitData() != null && m_NKMGameTeamDataB.GetLeaderUnitData().m_UnitUID == unitUID)
		{
			return true;
		}
		return false;
	}

	public NKM_TEAM_TYPE GetEnemyTeamType(NKM_TEAM_TYPE myTeamType)
	{
		switch (myTeamType)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			return m_NKMGameTeamDataB.m_eNKM_TEAM_TYPE;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			return m_NKMGameTeamDataA.m_eNKM_TEAM_TYPE;
		default:
			Log.Error("GetEnemyTeamData Error : Invalid Teamtype", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGame.cs", 1585);
			return NKM_TEAM_TYPE.NTT_INVALID;
		}
	}

	public NKMGameTeamData GetEnemyTeamData(NKM_TEAM_TYPE myTeamType)
	{
		switch (myTeamType)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			return m_NKMGameTeamDataB;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			return m_NKMGameTeamDataA;
		default:
			Log.Error("GetEnemyTeamData Error : Invalid Teamtype", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMGame.cs", 1601);
			return m_NKMGameTeamDataB;
		}
	}

	public void InitRespawnLimitCount()
	{
		m_NKMGameTeamDataA.m_DeckData.InitRespawnLimitCount(m_NKMGameTeamDataA.m_listUnitData);
		m_NKMGameTeamDataB.m_DeckData.InitRespawnLimitCount(m_NKMGameTeamDataB.m_listUnitData);
	}

	public void ShuffleDeck()
	{
		ShuffleDeck(m_NKMGameTeamDataA);
		ShuffleDeck(m_NKMGameTeamDataB);
	}

	public void ShuffleDeckForOnlyTeamA()
	{
		ShuffleDeck(m_NKMGameTeamDataA);
		DoNotShuffleDeck(m_NKMGameTeamDataB);
	}

	public void DoNotShuffleDeck()
	{
		DoNotShuffleDeck(m_NKMGameTeamDataA);
		DoNotShuffleDeck(m_NKMGameTeamDataB);
	}

	public void ShuffleDeckForOnlyTeamB()
	{
		DoNotShuffleDeck(m_NKMGameTeamDataA);
		ShuffleDeck(m_NKMGameTeamDataB);
	}

	public void DoNotShuffleDeck(NKMGameTeamData cNKMGameTeamData)
	{
		m_listUnitDeckTemp.Clear();
		for (int i = 0; i < cNKMGameTeamData.m_DeckData.GetListUnitDeckCount(); i++)
		{
			cNKMGameTeamData.m_DeckData.SetListUnitDeck(i, 0L);
		}
		for (int j = 0; j < cNKMGameTeamData.m_listUnitData.Count; j++)
		{
			NKMUnitData nKMUnitData = cNKMGameTeamData.m_listUnitData[j];
			if (nKMUnitData != null)
			{
				m_listUnitDeckTemp.Add(nKMUnitData.m_UnitUID);
			}
		}
		for (int k = 0; k < m_listUnitDeckTemp.Count; k++)
		{
			if (k < cNKMGameTeamData.m_DeckData.GetListUnitDeckCount())
			{
				cNKMGameTeamData.m_DeckData.SetListUnitDeck(k, m_listUnitDeckTemp[k]);
			}
			else
			{
				cNKMGameTeamData.m_DeckData.AddListUnitDeckUsed(m_listUnitDeckTemp[k]);
			}
		}
		m_listUnitDeckTemp.Clear();
		if (cNKMGameTeamData.m_DeckData.GetListUnitDeckUsedCount() > 0)
		{
			cNKMGameTeamData.m_DeckData.SetNextDeck(cNKMGameTeamData.m_DeckData.GetListUnitDeckUsed(0));
			cNKMGameTeamData.m_DeckData.RemoveAtListUnitDeckUsed(0);
		}
	}

	public void ShuffleDeck(NKMGameTeamData cNKMGameTeamData)
	{
		m_listUnitDeckTemp.Clear();
		cNKMGameTeamData.m_DeckData.ClearListUnitDeckUsed();
		for (int i = 0; i < cNKMGameTeamData.m_DeckData.GetListUnitDeckCount(); i++)
		{
			cNKMGameTeamData.m_DeckData.SetListUnitDeck(i, 0L);
		}
		for (int j = 0; j < cNKMGameTeamData.m_listUnitData.Count; j++)
		{
			NKMUnitData nKMUnitData = cNKMGameTeamData.m_listUnitData[j];
			if (nKMUnitData != null)
			{
				m_listUnitDeckTemp.Add(nKMUnitData.m_UnitUID);
			}
		}
		int num = 0;
		for (int k = 0; k < m_listUnitDeckTemp.Count; k++)
		{
			if (m_listUnitDeckTemp[k] == cNKMGameTeamData.m_LeaderUnitUID)
			{
				cNKMGameTeamData.m_DeckData.SetListUnitDeck(num, m_listUnitDeckTemp[k]);
				num++;
				m_listUnitDeckTemp.RemoveAt(k);
				break;
			}
		}
		while (m_listUnitDeckTemp.Count > 0)
		{
			int index = NKMRandom.Range(0, m_listUnitDeckTemp.Count);
			if (num < cNKMGameTeamData.m_DeckData.GetListUnitDeckCount())
			{
				cNKMGameTeamData.m_DeckData.SetListUnitDeck(num, m_listUnitDeckTemp[index]);
			}
			else
			{
				cNKMGameTeamData.m_DeckData.AddListUnitDeckUsed(m_listUnitDeckTemp[index]);
			}
			num++;
			m_listUnitDeckTemp.RemoveAt(index);
		}
		m_listUnitDeckTemp.Clear();
		if (cNKMGameTeamData.m_DeckData.GetListUnitDeckUsedCount() > 0)
		{
			cNKMGameTeamData.m_DeckData.SetNextDeck(cNKMGameTeamData.m_DeckData.GetListUnitDeckUsed(0));
			cNKMGameTeamData.m_DeckData.RemoveAtListUnitDeckUsed(0);
		}
	}

	public short GetGameUnitUID()
	{
		m_GameUnitUIDIndex++;
		return m_GameUnitUIDIndex;
	}

	public bool IsPVE()
	{
		return NKMGame.IsPVE(m_NKM_GAME_TYPE);
	}

	public bool IsPVP()
	{
		return NKMGame.IsPVP(m_NKM_GAME_TYPE);
	}

	public bool IsGuildDungeon()
	{
		return NKMGame.IsGuildDungeon(m_NKM_GAME_TYPE);
	}

	public bool IsBanUnit(int unitID)
	{
		if (m_dicNKMBanData.ContainsKey(unitID))
		{
			return true;
		}
		return false;
	}

	public int GetBanUnitLevel(int unitID)
	{
		if (unitID == 0)
		{
			return 0;
		}
		if (m_dicNKMBanData.ContainsKey(unitID))
		{
			return m_dicNKMBanData[unitID].m_BanLevel;
		}
		return 0;
	}

	public bool IsUpUnit(int unitID)
	{
		if (m_dicNKMUpData.ContainsKey(unitID))
		{
			return true;
		}
		return false;
	}

	public bool IsBanShip(int shipGroupId)
	{
		if (shipGroupId == 0)
		{
			return false;
		}
		if (m_dicNKMBanShipData.ContainsKey(shipGroupId))
		{
			return true;
		}
		return false;
	}

	public bool IsBanOperator(int operatorId)
	{
		if (operatorId == 0)
		{
			return false;
		}
		return m_dicNKMBanOperatorData.ContainsKey(operatorId);
	}

	public int GetBanShipLevel(int shipGroupId)
	{
		if (shipGroupId == 0)
		{
			return 0;
		}
		if (m_dicNKMBanShipData.ContainsKey(shipGroupId))
		{
			return m_dicNKMBanShipData[shipGroupId].m_BanLevel;
		}
		return 0;
	}

	public int GetUpUnitLevel(int unitID)
	{
		if (unitID == 0)
		{
			return 0;
		}
		if (m_dicNKMUpData.ContainsKey(unitID))
		{
			return m_dicNKMUpData[unitID].upLevel;
		}
		return 0;
	}

	public int GetBanOperatorLevel(int operatorId)
	{
		if (operatorId == 0)
		{
			return 0;
		}
		if (m_dicNKMBanOperatorData.TryGetValue(operatorId, out var value))
		{
			return value.m_BanLevel;
		}
		return 0;
	}

	public NKMUnitData GetAnyTeamMainShipDataByUnitUID(long unitUID)
	{
		NKMUnitData mainShipDataByUnitUID = m_NKMGameTeamDataA.GetMainShipDataByUnitUID(unitUID);
		if (mainShipDataByUnitUID == null)
		{
			mainShipDataByUnitUID = m_NKMGameTeamDataB.GetMainShipDataByUnitUID(unitUID);
		}
		return mainShipDataByUnitUID;
	}

	public bool IsBanGame()
	{
		if (m_dicNKMBanData != null && m_dicNKMBanData.Count > 0)
		{
			return true;
		}
		if (m_dicNKMBanShipData != null && m_dicNKMBanShipData.Count > 0)
		{
			return true;
		}
		if (m_dicNKMBanOperatorData != null && m_dicNKMBanOperatorData.Count > 0)
		{
			return true;
		}
		return false;
	}

	public bool IsUpUnitGame()
	{
		if (m_dicNKMUpData != null && m_dicNKMUpData.Count > 0)
		{
			return true;
		}
		return false;
	}
}
