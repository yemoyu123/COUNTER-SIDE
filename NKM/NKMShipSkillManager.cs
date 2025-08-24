using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMShipSkillManager
{
	public static Dictionary<int, NKMShipSkillTemplet> m_dicNKMShipSkillTempletByID;

	public static Dictionary<string, NKMShipSkillTemplet> m_dicNKMShipSkillTempletByStrID;

	public static bool LoadFromLUA(string fileName)
	{
		m_dicNKMShipSkillTempletByID?.Clear();
		m_dicNKMShipSkillTempletByStrID?.Clear();
		m_dicNKMShipSkillTempletByID = NKMTempletLoader.LoadDictionary("AB_SCRIPT", fileName, "m_dicNKMShipSkillTempletByID", NKMShipSkillTemplet.LoadFromLUA);
		int result = 1 & ((m_dicNKMShipSkillTempletByID != null) ? 1 : 0);
		if (m_dicNKMShipSkillTempletByID != null)
		{
			m_dicNKMShipSkillTempletByStrID = m_dicNKMShipSkillTempletByID.ToDictionary((KeyValuePair<int, NKMShipSkillTemplet> e) => e.Value.m_ShipSkillStrID, (KeyValuePair<int, NKMShipSkillTemplet> e) => e.Value);
		}
		return (byte)result != 0;
	}

	public static int GetSkillID(string strID)
	{
		return GetShipSkillTempletByStrID(strID)?.m_ShipSkillID ?? (-1);
	}

	public static string GetSkillStrID(int id)
	{
		NKMShipSkillTemplet shipSkillTempletByID = GetShipSkillTempletByID(id);
		if (shipSkillTempletByID != null)
		{
			return shipSkillTempletByID.m_ShipSkillStrID;
		}
		return string.Empty;
	}

	public static NKMShipSkillTemplet GetShipSkillTempletByID(int shipSkillID)
	{
		if (m_dicNKMShipSkillTempletByID.ContainsKey(shipSkillID))
		{
			return m_dicNKMShipSkillTempletByID[shipSkillID];
		}
		return null;
	}

	public static NKMShipSkillTemplet GetShipSkillTempletByStrID(string shipSkillStrID)
	{
		if (m_dicNKMShipSkillTempletByStrID.ContainsKey(shipSkillStrID))
		{
			return m_dicNKMShipSkillTempletByStrID[shipSkillStrID];
		}
		return null;
	}

	public static NKMShipSkillTemplet GetShipSkillTempletByIndex(NKMUnitTempletBase shipTemplet, int index)
	{
		return GetShipSkillTempletByStrID(shipTemplet.GetSkillStrID(index));
	}

	public static NKMShipSkillTemplet GetShipSkillFromUnitState(NKMUnitTempletBase unitTemplet, string stateName)
	{
		if (unitTemplet == null)
		{
			return null;
		}
		for (int i = 0; i < 5; i++)
		{
			NKMShipSkillTemplet shipSkillTempletByIndex = GetShipSkillTempletByIndex(unitTemplet, i);
			if (shipSkillTempletByIndex != null && string.Equals(shipSkillTempletByIndex.m_UnitStateName, stateName))
			{
				return shipSkillTempletByIndex;
			}
		}
		return null;
	}
}
