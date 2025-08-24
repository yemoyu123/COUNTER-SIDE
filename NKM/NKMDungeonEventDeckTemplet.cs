using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMDungeonEventDeckTemplet : INKMTemplet
{
	public enum SLOT_TYPE
	{
		ST_CLOSED,
		ST_FREE,
		ST_FIXED,
		ST_GUEST,
		ST_NPC,
		ST_FREE_COUNTER,
		ST_FREE_SOLDIER,
		ST_FREE_MECHANIC,
		ST_RANDOM
	}

	public struct EventDeckSlot
	{
		public SLOT_TYPE m_eType;

		public int m_ID;

		public int m_Level;

		public int m_SkinID;

		public int m_TacticLevel;

		public List<int> GetConnectedUnitList(NKM_TEAM_TYPE teamType)
		{
			return NKMUnitListTemplet.Find(m_ID)?.GetUnitList(teamType);
		}
	}

	public int ID;

	public string NAME;

	public EventDeckSlot ShipSlot;

	public EventDeckSlot OperatorSlot;

	public int OperatorSubSkillID;

	public List<EventDeckSlot> m_lstUnitSlot = new List<EventDeckSlot>(8);

	public int Key => ID;

	public EventDeckSlot GetUnitSlot(int index)
	{
		if (0 <= index && index < m_lstUnitSlot.Count)
		{
			return m_lstUnitSlot[index];
		}
		return new EventDeckSlot
		{
			m_eType = SLOT_TYPE.ST_CLOSED,
			m_ID = 0,
			m_Level = 0,
			m_SkinID = 0
		};
	}

	public bool IsUnitFitInSlot(EventDeckSlot slot, NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return false;
		}
		switch (slot.m_eType)
		{
		default:
			return false;
		case SLOT_TYPE.ST_FIXED:
		case SLOT_TYPE.ST_GUEST:
			return unitData.IsSameBaseUnit(slot.m_ID);
		case SLOT_TYPE.ST_FREE:
			return true;
		case SLOT_TYPE.ST_FREE_COUNTER:
		case SLOT_TYPE.ST_FREE_SOLDIER:
		case SLOT_TYPE.ST_FREE_MECHANIC:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
			if (unitTempletBase == null)
			{
				return false;
			}
			return NKMDungeonManager.GetUnitStyleTypeFromEventDeckType(slot.m_eType) == unitTempletBase.m_NKM_UNIT_STYLE_TYPE;
		}
		}
	}

	public bool IsOperatorFitInSlot(NKMOperator operatorData)
	{
		if (operatorData == null)
		{
			return false;
		}
		switch (OperatorSlot.m_eType)
		{
		default:
			return false;
		case SLOT_TYPE.ST_FIXED:
		case SLOT_TYPE.ST_GUEST:
			return operatorData.id == OperatorSlot.m_ID;
		case SLOT_TYPE.ST_FREE:
			return true;
		case SLOT_TYPE.ST_FREE_COUNTER:
		case SLOT_TYPE.ST_FREE_SOLDIER:
		case SLOT_TYPE.ST_FREE_MECHANIC:
			return false;
		}
	}

	public static NKMDungeonEventDeckTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonEventDeck.cs", 125))
		{
			return null;
		}
		NKMDungeonEventDeckTemplet nKMDungeonEventDeckTemplet = new NKMDungeonEventDeckTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("ID", ref nKMDungeonEventDeckTemplet.ID);
		flag &= cNKMLua.GetData("NAME", ref nKMDungeonEventDeckTemplet.NAME);
		cNKMLua.GetData("SLOT_TYPE_SHIP", ref nKMDungeonEventDeckTemplet.ShipSlot.m_eType);
		cNKMLua.GetData("SLOT_UNIT_ID_SHIP", out nKMDungeonEventDeckTemplet.ShipSlot.m_ID, 0);
		cNKMLua.GetData("SLOT_UNIT_LEVEL_SHIP", out nKMDungeonEventDeckTemplet.ShipSlot.m_Level, 0);
		nKMDungeonEventDeckTemplet.ShipSlot.m_SkinID = 0;
		if (nKMDungeonEventDeckTemplet.ShipSlot.m_eType == SLOT_TYPE.ST_CLOSED)
		{
			Log.Error("EventDeck : Ship slot is closed!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonEventDeck.cs", 141);
			flag = false;
		}
		cNKMLua.GetData("SLOT_TYPE_OPERATOR", ref nKMDungeonEventDeckTemplet.OperatorSlot.m_eType);
		cNKMLua.GetData("SLOT_UNIT_ID_OPERATOR", out nKMDungeonEventDeckTemplet.OperatorSlot.m_ID, 0);
		cNKMLua.GetData("SLOT_UNIT_LEVEL_OPERATOR", out nKMDungeonEventDeckTemplet.OperatorSlot.m_Level, 0);
		cNKMLua.GetData("SLOT_UNIT_SKILL_ID_OPERATOR", out nKMDungeonEventDeckTemplet.OperatorSubSkillID, 0);
		EventDeckSlot item = default(EventDeckSlot);
		for (int i = 0; i < 8; i++)
		{
			string text = (i + 1).ToString();
			if (cNKMLua.GetDataEnum<SLOT_TYPE>("SLOT_TYPE_UNIT_" + text, out item.m_eType))
			{
				cNKMLua.GetData("SLOT_UNIT_ID_" + text, out item.m_ID, 0);
				cNKMLua.GetData("SLOT_UNIT_LEVEL_" + text, out item.m_Level, 1);
				cNKMLua.GetData("SLOT_UNIT_SKIN_ID_" + text, out item.m_SkinID, 0);
				cNKMLua.GetData("SLOT_UNIT_TACTICS_" + text, out item.m_TacticLevel, 0);
				nKMDungeonEventDeckTemplet.m_lstUnitSlot.Add(item);
			}
		}
		if (!flag)
		{
			Log.Error($"NKMDungeonEventDeckTemplet Load Fail - {nKMDungeonEventDeckTemplet.ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonEventDeck.cs", 169);
			return null;
		}
		nKMDungeonEventDeckTemplet.CheckValidation();
		return nKMDungeonEventDeckTemplet;
	}

	public HashSet<int> GetAllFixedUnitID()
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (EventDeckSlot item in m_lstUnitSlot)
		{
			SLOT_TYPE eType = item.m_eType;
			if ((uint)(eType - 2) <= 2u)
			{
				hashSet.Add(item.m_ID);
			}
		}
		return hashSet;
	}

	public HashSet<int> GetRandomSlotUnitList(EventDeckSlot slotData)
	{
		HashSet<int> hashSet = new HashSet<int>();
		if (slotData.m_eType != SLOT_TYPE.ST_RANDOM)
		{
			return hashSet;
		}
		List<int> connectedUnitList = slotData.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_A1);
		List<int> connectedUnitList2 = slotData.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_B1);
		if (connectedUnitList != null)
		{
			hashSet.UnionWith(connectedUnitList);
		}
		if (connectedUnitList2 != null)
		{
			hashSet.UnionWith(connectedUnitList2);
		}
		HashSet<int> allFixedUnitID = GetAllFixedUnitID();
		hashSet.ExceptWith(allFixedUnitID);
		return hashSet;
	}

	public HashSet<int> GetRandomSlotUnitList(int slotIndex = -1)
	{
		if (slotIndex < 0)
		{
			return GetAllRandomUnits();
		}
		EventDeckSlot unitSlot = GetUnitSlot(slotIndex);
		return GetRandomSlotUnitList(unitSlot);
	}

	public HashSet<int> GetAllRandomUnits()
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (EventDeckSlot item in m_lstUnitSlot)
		{
			if (item.m_eType == SLOT_TYPE.ST_RANDOM)
			{
				List<int> connectedUnitList = item.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_A1);
				List<int> connectedUnitList2 = item.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_B1);
				if (connectedUnitList != null)
				{
					hashSet.UnionWith(connectedUnitList);
				}
				if (connectedUnitList2 != null)
				{
					hashSet.UnionWith(connectedUnitList2);
				}
			}
		}
		HashSet<int> allFixedUnitID = GetAllFixedUnitID();
		hashSet.ExceptWith(allFixedUnitID);
		return hashSet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public bool HasRandomUnitSlot()
	{
		return m_lstUnitSlot.Exists((EventDeckSlot x) => x.m_eType == SLOT_TYPE.ST_RANDOM);
	}

	private void CheckValidation()
	{
	}
}
