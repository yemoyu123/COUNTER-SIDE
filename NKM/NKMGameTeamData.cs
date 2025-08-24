using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Community;
using Cs.Protocol;

namespace NKM;

public class NKMGameTeamData : ISerializable
{
	public NKM_TEAM_TYPE m_eNKM_TEAM_TYPE;

	public long m_LeaderUnitUID;

	public int m_UserLevel;

	public string m_UserNickname;

	public int m_Tier;

	public int m_Score;

	public int m_WinStreak;

	public long m_FriendCode;

	public NKMCommonProfile m_userCommonProfile = new NKMCommonProfile();

	public NKMGuildSimpleData guildSimpleData = new NKMGuildSimpleData();

	public NKMUnitData m_MainShip;

	public NKMOperator m_Operator;

	public long m_user_uid;

	public List<NKMUnitData> m_listUnitData = new List<NKMUnitData>();

	public List<NKMUnitData> m_listAssistUnitData = new List<NKMUnitData>();

	public List<NKMUnitData> m_listEvevtUnitData = new List<NKMUnitData>();

	public List<NKMUnitData> m_listEnvUnitData = new List<NKMUnitData>();

	public List<NKMUnitData> m_listOperatorUnitData = new List<NKMUnitData>();

	public List<NKMDynamicRespawnUnitData> m_listDynamicRespawnUnitData = new List<NKMDynamicRespawnUnitData>();

	public List<NKMTacticalCommandData> m_listTacticalCommandData = new List<NKMTacticalCommandData>();

	public NKMGameTeamDeckData m_DeckData = new NKMGameTeamDeckData();

	public float m_fInitHP;

	public Dictionary<long, NKMEquipItemData> m_ItemEquipData = new Dictionary<long, NKMEquipItemData>();

	public EmoticonPresetData m_emoticonPreset = new EmoticonPresetData();

	public void Init()
	{
		m_eNKM_TEAM_TYPE = NKM_TEAM_TYPE.NTT_INVALID;
		m_LeaderUnitUID = 0L;
		m_UserLevel = 0;
		m_UserNickname = "";
		m_Tier = 0;
		m_Score = 0;
		m_MainShip = null;
		m_user_uid = 0L;
		m_listUnitData.Clear();
		m_listAssistUnitData.Clear();
		m_listEvevtUnitData.Clear();
		m_listDynamicRespawnUnitData.Clear();
		m_listOperatorUnitData.Clear();
		m_DeckData.Init();
		m_fInitHP = 0f;
	}

	public virtual void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref m_eNKM_TEAM_TYPE);
		stream.PutOrGet(ref m_LeaderUnitUID);
		stream.PutOrGet(ref m_UserLevel);
		stream.PutOrGet(ref m_UserNickname);
		stream.PutOrGet(ref m_Tier);
		stream.PutOrGet(ref m_Score);
		stream.PutOrGet(ref m_WinStreak);
		stream.PutOrGet(ref m_MainShip);
		stream.PutOrGet(ref m_Operator);
		stream.PutOrGet(ref m_user_uid);
		stream.PutOrGet(ref m_listUnitData);
		stream.PutOrGet(ref m_listAssistUnitData);
		stream.PutOrGet(ref m_listEvevtUnitData);
		stream.PutOrGet(ref m_listEnvUnitData);
		stream.PutOrGet(ref m_listDynamicRespawnUnitData);
		stream.PutOrGet(ref m_listOperatorUnitData);
		stream.PutOrGet(ref m_listTacticalCommandData);
		stream.PutOrGet(ref m_DeckData);
		stream.PutOrGet(ref m_fInitHP);
		stream.PutOrGet(ref m_ItemEquipData);
		stream.PutOrGet(ref m_FriendCode);
		stream.PutOrGet(ref m_emoticonPreset);
		stream.PutOrGet(ref guildSimpleData);
		stream.PutOrGet(ref m_userCommonProfile);
	}

	public int GetLeaderLV()
	{
		return GetUnitDataByUnitUID(m_LeaderUnitUID)?.m_UnitLevel ?? 0;
	}

	public NKMUnitData GetLeaderUnitData()
	{
		if (m_LeaderUnitUID > 0)
		{
			return GetUnitDataByUnitUID(m_LeaderUnitUID);
		}
		return null;
	}

	public NKMUnitData GetFirstUnitData()
	{
		NKMUnitData nKMUnitData = null;
		for (int i = 0; i < m_listUnitData.Count; i++)
		{
			nKMUnitData = m_listUnitData[i];
			if (nKMUnitData != null)
			{
				return nKMUnitData;
			}
		}
		for (int j = 0; j < m_listEvevtUnitData.Count; j++)
		{
			nKMUnitData = m_listEvevtUnitData[j];
			if (nKMUnitData != null)
			{
				return nKMUnitData;
			}
		}
		for (int k = 0; k < m_listDynamicRespawnUnitData.Count; k++)
		{
			NKMDynamicRespawnUnitData nKMDynamicRespawnUnitData = m_listDynamicRespawnUnitData[k];
			if (nKMDynamicRespawnUnitData != null)
			{
				return nKMDynamicRespawnUnitData.m_NKMUnitData;
			}
		}
		for (int l = 0; l < m_listAssistUnitData.Count; l++)
		{
			nKMUnitData = m_listAssistUnitData[l];
			if (nKMUnitData != null)
			{
				return nKMUnitData;
			}
		}
		for (int m = 0; m < m_listOperatorUnitData.Count; m++)
		{
			nKMUnitData = m_listOperatorUnitData[m];
			if (nKMUnitData != null)
			{
				return nKMUnitData;
			}
		}
		return null;
	}

	public NKMUnitData GetMainShipDataByUnitUID(long unitUID)
	{
		if (m_MainShip == null || m_MainShip.m_UnitUID != unitUID)
		{
			return null;
		}
		return m_MainShip;
	}

	public NKMUnitData GetUnitDataByUnitUID(long unitUID)
	{
		NKMUnitData mainShipDataByUnitUID = GetMainShipDataByUnitUID(unitUID);
		if (mainShipDataByUnitUID != null)
		{
			return mainShipDataByUnitUID;
		}
		if (unitUID <= 0)
		{
			return null;
		}
		for (int i = 0; i < m_listUnitData.Count; i++)
		{
			mainShipDataByUnitUID = m_listUnitData[i];
			if (mainShipDataByUnitUID != null && mainShipDataByUnitUID.m_UnitUID == unitUID)
			{
				return mainShipDataByUnitUID;
			}
		}
		for (int j = 0; j < m_listAssistUnitData.Count; j++)
		{
			mainShipDataByUnitUID = m_listAssistUnitData[j];
			if (mainShipDataByUnitUID != null && mainShipDataByUnitUID.m_UnitUID == unitUID)
			{
				return mainShipDataByUnitUID;
			}
		}
		for (int k = 0; k < m_listEvevtUnitData.Count; k++)
		{
			mainShipDataByUnitUID = m_listEvevtUnitData[k];
			if (mainShipDataByUnitUID != null && mainShipDataByUnitUID.m_UnitUID == unitUID)
			{
				return mainShipDataByUnitUID;
			}
		}
		for (int l = 0; l < m_listDynamicRespawnUnitData.Count; l++)
		{
			NKMDynamicRespawnUnitData nKMDynamicRespawnUnitData = m_listDynamicRespawnUnitData[l];
			if (nKMDynamicRespawnUnitData != null && nKMDynamicRespawnUnitData.m_NKMUnitData.m_UnitUID == unitUID)
			{
				return nKMDynamicRespawnUnitData.m_NKMUnitData;
			}
		}
		for (int m = 0; m < m_listOperatorUnitData.Count; m++)
		{
			mainShipDataByUnitUID = m_listOperatorUnitData[m];
			if (mainShipDataByUnitUID != null && mainShipDataByUnitUID.m_UnitUID == unitUID)
			{
				return mainShipDataByUnitUID;
			}
		}
		return null;
	}

	public NKMUnitData GetAssistUnitDataByIndex(int index)
	{
		if (m_listAssistUnitData.Count <= 0)
		{
			return null;
		}
		if (index < 0)
		{
			return null;
		}
		if (index >= m_listAssistUnitData.Count)
		{
			return null;
		}
		return m_listAssistUnitData[index];
	}

	public bool IsFullDeck()
	{
		if (m_listUnitData.Count < 8)
		{
			return false;
		}
		for (int i = 0; i < m_listUnitData.Count; i++)
		{
			if (m_listUnitData[i] == null)
			{
				return false;
			}
			if (m_listUnitData[i].m_UnitID <= 0)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsAssistUnit(long unitUID)
	{
		for (int i = 0; i < m_listAssistUnitData.Count; i++)
		{
			NKMUnitData nKMUnitData = m_listAssistUnitData[i];
			if (nKMUnitData != null && nKMUnitData.m_UnitUID == unitUID)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsOperatorUnit(long unitUID)
	{
		for (int i = 0; i < m_listOperatorUnitData.Count; i++)
		{
			NKMUnitData nKMUnitData = m_listOperatorUnitData[i];
			if (nKMUnitData != null && nKMUnitData.m_UnitUID == unitUID)
			{
				return true;
			}
		}
		return false;
	}

	public NKMTacticalCommandData GetTacticalCommandDataByID(int TCID)
	{
		for (int i = 0; i < m_listTacticalCommandData.Count; i++)
		{
			if (m_listTacticalCommandData[i].m_TCID == TCID)
			{
				return m_listTacticalCommandData[i];
			}
		}
		return null;
	}

	public void SetInfo(PvpState pvpData, NKMGuildSimpleData guildSimpleData)
	{
		m_Score = pvpData.Score;
		m_Tier = pvpData.LeagueTierID;
		m_WinStreak = pvpData.WinStreak;
		this.guildSimpleData = guildSimpleData;
	}

	public NKMTacticalCommandData GetTC_Combo()
	{
		for (int i = 0; i < m_listTacticalCommandData.Count; i++)
		{
			NKMTacticalCommandData nKMTacticalCommandData = m_listTacticalCommandData[i];
			if (nKMTacticalCommandData != null)
			{
				NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(nKMTacticalCommandData.m_TCID);
				if (tacticalCommandTempletByID != null && tacticalCommandTempletByID.m_NKM_TACTICAL_COMMAND_TYPE == NKM_TACTICAL_COMMAND_TYPE.NTCT_COMBO)
				{
					return nKMTacticalCommandData;
				}
			}
		}
		return null;
	}
}
