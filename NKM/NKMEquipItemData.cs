using System.Collections.Generic;
using Cs.Logging;
using Cs.Protocol;

namespace NKM;

public class NKMEquipItemData : ISerializable
{
	public enum NKM_EQUIP_STAT_TYPE
	{
		NESI_DEFAULT,
		NESI_RANDOM,
		NESI_RANDOM2,
		NESI_MAX
	}

	public long m_ItemUid;

	public int m_ItemEquipID;

	public int m_EnchantLevel;

	public int m_EnchantExp;

	public List<EQUIP_ITEM_STAT> m_Stat = new List<EQUIP_ITEM_STAT>();

	public long m_OwnerUnitUID = -1L;

	public bool m_bLock;

	public int m_Precision;

	public int m_Precision2;

	public int m_SetOptionId;

	public int m_ImprintUnitId;

	public List<NKMPotentialOption> potentialOptions = new List<NKMPotentialOption>();

	public NKMEquipItemData()
	{
		m_Stat.Add(new EQUIP_ITEM_STAT());
	}

	public NKMEquipItemData(long itemUid, NKMEquipTemplet templet)
	{
		m_ItemUid = itemUid;
		m_ItemEquipID = templet.m_ItemEquipID;
		m_Stat.Add(new EQUIP_ITEM_STAT
		{
			type = templet.m_StatType,
			stat_value = templet.m_StatData.m_fBaseValue,
			stat_level_value = templet.m_StatData.m_fLevelUpValue
		});
	}

	public void AddSubStat(EQUIP_ITEM_STAT subStat)
	{
		if (m_Stat.Count >= 3)
		{
			Log.Error($"[Equip] add substat failed. m_Stat.Count:{m_Stat.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEquipItemData.cs", 54);
		}
		else
		{
			m_Stat.Add(subStat);
		}
	}

	public EQUIP_ITEM_STAT GetStat(int index)
	{
		if (index < 0 || index >= m_Stat.Count)
		{
			return null;
		}
		return m_Stat[index];
	}

	public EQUIP_ITEM_STAT GetSubStatOrDefault(int index)
	{
		EQUIP_ITEM_STAT result = new EQUIP_ITEM_STAT();
		if (index <= 0 || index >= m_Stat.Count)
		{
			return result;
		}
		return m_Stat[index];
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_ItemUid);
		stream.PutOrGet(ref m_ItemEquipID);
		stream.PutOrGet(ref m_EnchantLevel);
		stream.PutOrGet(ref m_EnchantExp);
		stream.PutOrGet(ref m_Stat);
		stream.PutOrGet(ref m_OwnerUnitUID);
		stream.PutOrGet(ref m_bLock);
		stream.PutOrGet(ref m_Precision);
		stream.PutOrGet(ref m_Precision2);
		stream.PutOrGet(ref m_SetOptionId);
		stream.PutOrGet(ref m_ImprintUnitId);
		stream.PutOrGet(ref potentialOptions);
	}

	public void UpgradeEquip(NKMEquipTemplet templet, int upgradeEquipLevel, int upgradeEquipExp)
	{
		m_ItemEquipID = templet.m_ItemEquipID;
		m_Stat[0].stat_value = templet.m_StatData.m_fBaseValue;
		m_Stat[0].stat_level_value = templet.m_StatData.m_fLevelUpValue;
		m_Stat[1] = templet.StatGroups[0].GenerateSubStat(m_Stat[1].type, m_Precision);
		m_Stat[2] = templet.StatGroups[1].GenerateSubStat(m_Stat[2].type, m_Precision2);
		m_EnchantLevel = upgradeEquipLevel;
		m_EnchantExp = upgradeEquipExp;
	}

	public float CalculatePotentialPercent()
	{
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(m_ItemEquipID);
		if (equipTemplet == null)
		{
			return 0f;
		}
		if (!equipTemplet.m_bRelic)
		{
			return 0f;
		}
		if (potentialOptions.Count <= 0)
		{
			return 0f;
		}
		NKMPotentialOptionTemplet nKMPotentialOptionTemplet = NKMPotentialOptionTemplet.Find(potentialOptions[0].optionKey);
		if (nKMPotentialOptionTemplet == null)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		int num4 = potentialOptions[0].sockets.Length;
		for (int i = 0; i < num4; i++)
		{
			if (potentialOptions[0].sockets[i] != null)
			{
				NKMPotentialSocketTemplet nKMPotentialSocketTemplet = nKMPotentialOptionTemplet.sockets[i];
				if (nKMPotentialSocketTemplet != null)
				{
					num += potentialOptions[0].sockets[i].statValue;
					num2 += nKMPotentialSocketTemplet.MaxStat;
					num3 += nKMPotentialSocketTemplet.MinStat;
				}
			}
		}
		if (num2 == num3)
		{
			return 1f;
		}
		return (num - num3) / (num2 - num3);
	}
}
