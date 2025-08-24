using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;

namespace NKM;

public sealed class NKMGameRecord : ISerializable
{
	private Dictionary<short, NKMGameRecordUnitData> unitRecords = new Dictionary<short, NKMGameRecordUnitData>();

	private float totalDamageA;

	private float totalDamageB;

	private int totalDieCountA;

	private int totalDieCountB;

	private float totalFiercePoint;

	private float fiercePenaltyPoint;

	private float totalTrimPoint;

	public Dictionary<short, NKMGameRecordUnitData> UnitRecordList => unitRecords;

	public float ToTalDamageA => totalDamageA;

	public float TotalDamageB => totalDamageB;

	public int TotalDieCountA => totalDieCountA;

	public int TotalDieCountB => totalDieCountB;

	public float TotalFiercePoint => totalFiercePoint;

	public float FiercePenaltyPoint => fiercePenaltyPoint;

	public float TotalTrimPoint => totalTrimPoint;

	public void AddDamage(short attckGameuid, NKM_TEAM_TYPE teamType, NKMUnit target, float damage)
	{
		unitRecords[attckGameuid].recordGiveDamage += damage;
		unitRecords[target.GetUnitDataGame().m_GameUnitUID].recordTakeDamage += damage;
		switch (teamType)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			totalDamageA += damage;
			break;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			totalDamageB += damage;
			break;
		}
	}

	public void SetTotalFiercePoint(float value, float penaltyPoint)
	{
		totalFiercePoint = value;
		fiercePenaltyPoint = penaltyPoint;
	}

	public void SetTotalTrimPoint(float value)
	{
		totalTrimPoint = value;
	}

	public void AddHeal(short leaderGameUid, float value)
	{
		if (unitRecords.TryGetValue(leaderGameUid, out var value2))
		{
			value2.recordHeal += value;
		}
	}

	public void AddSummonCount(NKMUnit summonUnit)
	{
		NKMUnitDataGame unitDataGame = summonUnit.GetUnitDataGame();
		if (!unitRecords.TryGetValue(unitDataGame.m_GameUnitUID, out var value))
		{
			NKMUnitData unitData = summonUnit.GetUnitData();
			NKMGameTeamData teamData = summonUnit.GetTeamData();
			value = new NKMGameRecordUnitData
			{
				unitId = unitData.m_UnitID,
				unitLevel = unitData.m_UnitLevel,
				isSummonee = (unitDataGame.m_MasterGameUnitUID != 0),
				isAssistUnit = teamData.IsAssistUnit(unitData.m_UnitUID),
				isLeader = (teamData.m_LeaderUnitUID == unitData.m_UnitUID),
				teamType = teamData.m_eNKM_TEAM_TYPE,
				changeUnitName = ((unitData.m_DungeonRespawnUnitTemplet != null) ? unitData.m_DungeonRespawnUnitTemplet.m_ChangeUnitName : null)
			};
			unitRecords.Add(unitDataGame.m_GameUnitUID, value);
		}
		value.recordSummonCount++;
	}

	public void AddDieCount(NKMUnit dieUnit)
	{
		unitRecords[dieUnit.GetUnitDataGame().m_GameUnitUID].recordDieCount++;
		if (dieUnit.IsATeam())
		{
			totalDieCountA++;
		}
		else
		{
			totalDieCountB++;
		}
	}

	public void AddKillCount(short gameUnitUid)
	{
		unitRecords[gameUnitUid].recordKillCount++;
	}

	public void AddPlayTime(NKMUnit dieUnit, float value)
	{
		unitRecords[dieUnit.GetUnitDataGame().m_GameUnitUID].playtime += (int)value;
	}

	public float GetTotalDamage(NKM_TEAM_TYPE teamType)
	{
		switch (teamType)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			return totalDamageA;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			return totalDamageB;
		default:
			return 0f;
		}
	}

	public NKMGameRecordUnitData GetUnitHp2(short index)
	{
		return unitRecords[index];
	}

	public int GetTotalDieCount(NKM_TEAM_TYPE teamType)
	{
		switch (teamType)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			return totalDieCountA;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			return totalDieCountB;
		default:
			return 0;
		}
	}

	public int GetTotalKillCount(NKM_TEAM_TYPE teamType)
	{
		switch (teamType)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			return totalDieCountB;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			return totalDieCountA;
		default:
			return 0;
		}
	}

	public int GetSummonCount(short gameUnitUid)
	{
		if (!unitRecords.TryGetValue(gameUnitUid, out var value))
		{
			return 0;
		}
		return value.recordSummonCount;
	}

	public int GetPlayTime(short gameUnitUid)
	{
		if (!unitRecords.TryGetValue(gameUnitUid, out var value))
		{
			return 0;
		}
		return value.playtime;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitRecords);
		stream.PutOrGet(ref totalDamageA);
		stream.PutOrGet(ref totalDamageB);
		stream.PutOrGet(ref totalFiercePoint);
	}

	public float GetShipTakeDamage(short shipIndex)
	{
		return unitRecords[shipIndex].recordTakeDamage;
	}
}
