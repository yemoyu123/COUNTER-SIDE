using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKCBanManager
{
	public enum BAN_DATA_TYPE
	{
		CASTING,
		FINAL,
		ROTATION
	}

	public static Color UP_COLOR = new Color(0f, 1f, 1f);

	private static Dictionary<int, NKMBanData> m_dicNKMBanData = new Dictionary<int, NKMBanData>();

	private static Dictionary<int, NKMBanShipData> m_dicNKMBanShipData = new Dictionary<int, NKMBanShipData>();

	private static Dictionary<int, NKMBanOperatorData> m_dicNKMBanOperData = new Dictionary<int, NKMBanOperatorData>();

	public static Dictionary<int, NKMUnitUpData> m_dicNKMUpData = new Dictionary<int, NKMUnitUpData>();

	private static Dictionary<int, NKMBanData> m_dicNKMFinalBanDataUnit = new Dictionary<int, NKMBanData>();

	private static Dictionary<int, NKMBanShipData> m_dicNKMFinalBanDataShip = new Dictionary<int, NKMBanShipData>();

	private static Dictionary<int, NKMBanOperatorData> m_dicNKMFinalBanDataOper = new Dictionary<int, NKMBanOperatorData>();

	private static Dictionary<int, NKMBanData> m_dicNKMCastingBanDataUnit = new Dictionary<int, NKMBanData>();

	private static Dictionary<int, NKMBanShipData> m_dicNKMCastingBanDataShip = new Dictionary<int, NKMBanShipData>();

	private static Dictionary<int, NKMBanOperatorData> m_dicNKMCastingBanDataOper = new Dictionary<int, NKMBanOperatorData>();

	public static PvpCastingVoteData m_CastingVoteData;

	public static PvpCastingVoteData m_GlobalVoteData;

	public static Dictionary<int, NKMBanData> GetBanData(BAN_DATA_TYPE banType = BAN_DATA_TYPE.FINAL)
	{
		return banType switch
		{
			BAN_DATA_TYPE.CASTING => m_dicNKMCastingBanDataUnit, 
			BAN_DATA_TYPE.ROTATION => m_dicNKMBanData, 
			_ => m_dicNKMFinalBanDataUnit, 
		};
	}

	public static Dictionary<int, NKMBanShipData> GetBanDataShip(BAN_DATA_TYPE banType = BAN_DATA_TYPE.FINAL)
	{
		return banType switch
		{
			BAN_DATA_TYPE.CASTING => m_dicNKMCastingBanDataShip, 
			BAN_DATA_TYPE.ROTATION => m_dicNKMBanShipData, 
			_ => m_dicNKMFinalBanDataShip, 
		};
	}

	public static Dictionary<int, NKMBanOperatorData> GetBanDataOperator(BAN_DATA_TYPE banType = BAN_DATA_TYPE.FINAL)
	{
		return banType switch
		{
			BAN_DATA_TYPE.CASTING => m_dicNKMCastingBanDataOper, 
			BAN_DATA_TYPE.ROTATION => m_dicNKMBanOperData, 
			_ => m_dicNKMFinalBanDataOper, 
		};
	}

	public static bool IsBanUnit(int unitID, BAN_DATA_TYPE banType = BAN_DATA_TYPE.FINAL)
	{
		if (banType == BAN_DATA_TYPE.ROTATION && m_dicNKMBanData.ContainsKey(unitID))
		{
			return true;
		}
		if (banType == BAN_DATA_TYPE.FINAL && m_dicNKMFinalBanDataUnit.ContainsKey(unitID))
		{
			return true;
		}
		if (banType == BAN_DATA_TYPE.CASTING && m_dicNKMCastingBanDataUnit.ContainsKey(unitID))
		{
			return true;
		}
		return false;
	}

	public static bool IsUpUnit(int unitID)
	{
		if (m_dicNKMUpData.ContainsKey(unitID))
		{
			return true;
		}
		return false;
	}

	public static bool IsBanShip(int shipGroupId, BAN_DATA_TYPE banType = BAN_DATA_TYPE.FINAL)
	{
		if (shipGroupId == 0)
		{
			return false;
		}
		if (banType == BAN_DATA_TYPE.ROTATION && m_dicNKMBanShipData.ContainsKey(shipGroupId))
		{
			return true;
		}
		if (banType == BAN_DATA_TYPE.FINAL && m_dicNKMFinalBanDataShip.ContainsKey(shipGroupId))
		{
			return true;
		}
		if (banType == BAN_DATA_TYPE.CASTING && m_dicNKMCastingBanDataShip.ContainsKey(shipGroupId))
		{
			return true;
		}
		return false;
	}

	public static bool IsBanOperator(int operID, BAN_DATA_TYPE banType = BAN_DATA_TYPE.FINAL)
	{
		if (m_dicNKMBanOperData.ContainsKey(operID))
		{
			return true;
		}
		return false;
	}

	public static int GetShipBanLevel(int shipGroupId, BAN_DATA_TYPE banType = BAN_DATA_TYPE.FINAL)
	{
		if (shipGroupId == 0)
		{
			return 0;
		}
		if (banType == BAN_DATA_TYPE.ROTATION && m_dicNKMBanShipData.TryGetValue(shipGroupId, out var value))
		{
			return value.m_BanLevel;
		}
		if (banType == BAN_DATA_TYPE.FINAL && m_dicNKMFinalBanDataShip.TryGetValue(shipGroupId, out var value2))
		{
			return value2.m_BanLevel;
		}
		if (banType == BAN_DATA_TYPE.CASTING && m_dicNKMCastingBanDataShip.TryGetValue(shipGroupId, out var value3))
		{
			return value3.m_BanLevel;
		}
		return 0;
	}

	public static int GetUnitBanLevel(int unitID, BAN_DATA_TYPE banType = BAN_DATA_TYPE.FINAL)
	{
		if (unitID == 0)
		{
			return 0;
		}
		if (banType == BAN_DATA_TYPE.ROTATION && m_dicNKMBanData.TryGetValue(unitID, out var value))
		{
			return value.m_BanLevel;
		}
		if (banType == BAN_DATA_TYPE.FINAL && m_dicNKMFinalBanDataUnit.TryGetValue(unitID, out var value2))
		{
			return value2.m_BanLevel;
		}
		if (banType == BAN_DATA_TYPE.CASTING && m_dicNKMCastingBanDataUnit.TryGetValue(unitID, out var value3))
		{
			return value3.m_BanLevel;
		}
		return 0;
	}

	public static int GetOperBanLevel(int operID, BAN_DATA_TYPE banType = BAN_DATA_TYPE.FINAL)
	{
		if (operID == 0)
		{
			return 0;
		}
		if (banType == BAN_DATA_TYPE.ROTATION && m_dicNKMBanOperData.TryGetValue(operID, out var value))
		{
			return value.m_BanLevel;
		}
		if (banType == BAN_DATA_TYPE.FINAL && m_dicNKMFinalBanDataOper.TryGetValue(operID, out var value2))
		{
			return value2.m_BanLevel;
		}
		if (banType == BAN_DATA_TYPE.CASTING && m_dicNKMCastingBanDataOper.TryGetValue(operID, out var value3))
		{
			return value3.m_BanLevel;
		}
		return 0;
	}

	public static int GetUnitUpLevel(int unitID)
	{
		if (unitID == 0)
		{
			return 0;
		}
		if (m_dicNKMUpData.TryGetValue(unitID, out var value))
		{
			return value.upLevel;
		}
		return 0;
	}

	public static bool IsBanUnitByUTB(NKMUnitTempletBase cNKMUnitTempletBase)
	{
		bool flag = false;
		if (cNKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			return IsBanShip(cNKMUnitTempletBase.m_ShipGroupID);
		}
		return IsBanUnit(cNKMUnitTempletBase.m_UnitID);
	}

	public static bool IsUpUnitByUTB(NKMUnitTempletBase cNKMUnitTempletBase)
	{
		bool result = false;
		if (cNKMUnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
		{
			result = IsUpUnit(cNKMUnitTempletBase.m_UnitID);
		}
		return result;
	}

	public static int GetUnitBanLevelByUTB(NKMUnitTempletBase cNKMUnitTempletBase)
	{
		int num = 0;
		if (cNKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			return GetShipBanLevel(cNKMUnitTempletBase.m_ShipGroupID);
		}
		return GetUnitBanLevel(cNKMUnitTempletBase.m_UnitID);
	}

	public static int GetUnitUpLevelByUTB(NKMUnitTempletBase cNKMUnitTempletBase)
	{
		int result = 0;
		if (cNKMUnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
		{
			result = GetUnitUpLevel(cNKMUnitTempletBase.m_UnitID);
		}
		return result;
	}

	public static void UpdatePVPBanData(NKMPvpBanResult pvpBanData)
	{
		if (pvpBanData != null)
		{
			if (pvpBanData.unitBanList != null)
			{
				m_dicNKMBanData = pvpBanData.unitBanList;
			}
			if (pvpBanData.shipBanList != null)
			{
				m_dicNKMBanShipData = pvpBanData.shipBanList;
			}
			if (pvpBanData.unitUpList != null)
			{
				m_dicNKMUpData = pvpBanData.unitUpList;
			}
			if (pvpBanData.unitFinalBanList != null)
			{
				m_dicNKMFinalBanDataUnit = pvpBanData.unitFinalBanList;
			}
			if (pvpBanData.shipFinalBanList != null)
			{
				m_dicNKMFinalBanDataShip = pvpBanData.shipFinalBanList;
			}
			if (pvpBanData.shipCastingBanList != null)
			{
				m_dicNKMCastingBanDataUnit = pvpBanData.unitCastingBanList;
			}
			if (pvpBanData.shipFinalBanList != null)
			{
				m_dicNKMCastingBanDataShip = pvpBanData.shipCastingBanList;
			}
			if (pvpBanData.operatorBanList != null)
			{
				m_dicNKMBanOperData = pvpBanData.operatorBanList;
			}
			if (pvpBanData.operatorFinalBanList != null)
			{
				m_dicNKMFinalBanDataOper = pvpBanData.operatorFinalBanList;
			}
			if (pvpBanData.operatorCastingBanList != null)
			{
				m_dicNKMCastingBanDataOper = pvpBanData.operatorCastingBanList;
			}
		}
	}

	public static void UpdatePVPCastingVoteData(PvpCastingVoteData voteData)
	{
		m_CastingVoteData = voteData;
	}

	public static void UpdatePVPGlobalVoteData(PvpCastingVoteData voteData)
	{
		m_GlobalVoteData = voteData;
	}

	public static bool IsCastingBanVoted()
	{
		if (m_CastingVoteData != null)
		{
			if (m_CastingVoteData.unitIdList.Count <= 0 && m_CastingVoteData.shipGroupIdList.Count <= 0)
			{
				return m_CastingVoteData.operatorIdList.Count > 0;
			}
			return true;
		}
		return false;
	}

	public static bool IsTryDraftBan()
	{
		if (m_GlobalVoteData != null && m_GlobalVoteData.unitIdList != null && m_GlobalVoteData.unitIdList.Count >= 2 && m_GlobalVoteData.shipGroupIdList != null && m_GlobalVoteData.shipGroupIdList.Count >= 1)
		{
			foreach (int unitId in m_GlobalVoteData.unitIdList)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitId);
				if (unitTempletBase == null)
				{
					return false;
				}
			}
			foreach (int shipGroupId in m_GlobalVoteData.shipGroupIdList)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(ConvertShipGroupIdToShipId(shipGroupId));
				if (unitTempletBase2 == null)
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public static int ConvertShipGroupIdToShipId(int shipGroupID)
	{
		foreach (int unit in NKCCollectionManager.GetUnitList(NKM_UNIT_TYPE.NUT_SHIP))
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unit);
			if (unitTempletBase != null && unitTempletBase.m_ShipGroupID == shipGroupID)
			{
				return unit;
			}
		}
		return 0;
	}

	public static bool IsSameGroupShip(int shipId, int shipGroupID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipId);
		if (unitTempletBase != null)
		{
			return unitTempletBase.m_ShipGroupID == shipGroupID;
		}
		return false;
	}

	public static List<int> GetGlobalBanUnitList(NKM_UNIT_TYPE unitType)
	{
		switch (unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
		{
			List<int> list2 = NKCLeaguePVPMgr.GetLeftDraftTeamData().globalBanUnitIdList.ToList();
			list2.AddRange(NKCLeaguePVPMgr.GetRightDraftTeamData().globalBanUnitIdList);
			return list2;
		}
		case NKM_UNIT_TYPE.NUT_SHIP:
		{
			List<int> list = NKCLeaguePVPMgr.GetLeftDraftTeamData().globalBanShipGroupIdList.ToList();
			list.AddRange(NKCLeaguePVPMgr.GetRightDraftTeamData().globalBanShipGroupIdList);
			return list;
		}
		default:
			return new List<int>();
		}
	}
}
