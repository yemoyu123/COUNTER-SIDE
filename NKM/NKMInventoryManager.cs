namespace NKM;

public class NKMInventoryManager
{
	public const int INVENTORY_EXPAND_ITEM_ID = 101;

	public const int UNIT_EXPAND_ITEM_COUNT = 100;

	public const int SHIP_EXPAND_ITEM_COUNT = 100;

	public const int EQUIP_EXPAND_ITEM_COUNT = 50;

	public const int OPERATOR_EXPAND_ITEM_COUNT = 100;

	public const int TROPHY_EXPAND_ITEM_COUNT = 50;

	public const int UNIT_EXPAND_COUNT = 5;

	public const int SHIP_EXPAND_COUNT = 1;

	public const int EQUIP_EXPAND_COUNT = 5;

	public const int OPERATOR_EXPAND_COUNT = 5;

	public const int TROPHY_EXPAND_COUNT = 10;

	public const int MIN_UNIT_EXPAND_COUNT = 200;

	public const int MIN_SHIP_EXPAND_COUNT = 10;

	public const int MIN_EQUIP_EXPAND_COUNT = 300;

	public const int MIN_OPERATOR_EXPAND_COUNT = 300;

	public const int MIN_TROPHY_EXPAND_COUNT = 2000;

	public const int MAX_UNIT_EXPAND_COUNT = 1100;

	public const int MAX_SHIP_EXPAND_COUNT = 60;

	public const int MAX_EQUIP_EXPAND_COUNT = 2000;

	public const int MAX_OPERATOR_EXPAND_COUNT = 500;

	public const int MAX_TROPHY_EXPAND_COUNT = 2000;

	public static bool IsValidExpandType(NKM_INVENTORY_EXPAND_TYPE type)
	{
		if (NKM_INVENTORY_EXPAND_TYPE.NIET_NONE > type || type >= NKM_INVENTORY_EXPAND_TYPE.NEIT_MAX)
		{
			return false;
		}
		return true;
	}

	public static bool ExpandInventory(NKM_INVENTORY_EXPAND_TYPE type, NKMUserData userData, int count, out int maxEquipCount, out int maxUnitCount, out int maxShipCount, out int maxTrophyCount, out int expandedCount)
	{
		maxEquipCount = GetCurrentInventoryCount(NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP, userData);
		maxUnitCount = GetCurrentInventoryCount(NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT, userData);
		maxShipCount = GetCurrentInventoryCount(NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP, userData);
		maxTrophyCount = GetCurrentInventoryCount(NKM_INVENTORY_EXPAND_TYPE.NIET_TROPHY, userData);
		expandedCount = 0;
		GetInventoryExpandData(type, out var expandCount, out var maxCount);
		switch (type)
		{
		case NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP:
			maxEquipCount += expandCount * count;
			expandedCount = maxEquipCount;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT:
			maxUnitCount += expandCount * count;
			expandedCount = maxUnitCount;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP:
			maxShipCount += expandCount * count;
			expandedCount = maxShipCount;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_TROPHY:
			maxTrophyCount += expandCount * count;
			expandedCount = maxTrophyCount;
			break;
		}
		return expandedCount <= maxCount;
	}

	public static bool CanExpandInventory(NKM_INVENTORY_EXPAND_TYPE type, NKMUserData userData, int count, out int resultCount)
	{
		if (!IsValidExpandType(type))
		{
			resultCount = 0;
			return false;
		}
		resultCount = GetCurrentInventoryCount(type, userData);
		GetInventoryExpandData(type, out var expandCount, out var maxCount);
		resultCount += expandCount * count;
		return resultCount <= maxCount;
	}

	public static bool CanExpandInventoryByAd(NKM_INVENTORY_EXPAND_TYPE type, NKMUserData userData, int count, out int resultCount)
	{
		if (!IsValidExpandType(type))
		{
			resultCount = 0;
			return false;
		}
		resultCount = GetCurrentInventoryCount(type, userData);
		GetInventoryExpandData(type, out var _, out var maxCount);
		resultCount += count;
		return resultCount <= maxCount;
	}

	public static int GetExpandInventoryCount(NKM_INVENTORY_EXPAND_TYPE type, NKMUserData userData, int MaxCount)
	{
		int currentInventoryCount = GetCurrentInventoryCount(type, userData);
		GetInventoryExpandData(type, out var expandCount, out var _);
		return currentInventoryCount + expandCount;
	}

	private static int GetCurrentInventoryCount(NKM_INVENTORY_EXPAND_TYPE type, NKMUserData userData)
	{
		int result = 0;
		switch (type)
		{
		case NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP:
			result = userData.m_InventoryData.m_MaxItemEqipCount;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT:
			result = userData.m_ArmyData.m_MaxUnitCount;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP:
			result = userData.m_ArmyData.m_MaxShipCount;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_OPERATOR:
			result = userData.m_ArmyData.m_MaxOperatorCount;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_TROPHY:
			result = userData.m_ArmyData.m_MaxTrophyCount;
			break;
		}
		return result;
	}

	private static void GetInventoryExpandData(NKM_INVENTORY_EXPAND_TYPE type, out int expandCount, out int maxCount)
	{
		expandCount = 0;
		maxCount = 0;
		switch (type)
		{
		case NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP:
			expandCount = 5;
			maxCount = 2000;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT:
			expandCount = 5;
			maxCount = 1100;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP:
			expandCount = 1;
			maxCount = 60;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_OPERATOR:
			expandCount = 5;
			maxCount = 500;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_TROPHY:
			expandCount = 10;
			maxCount = 2000;
			break;
		}
	}

	public static void UpdateInventoryCount(NKM_INVENTORY_EXPAND_TYPE type, int count, NKMUserData userData)
	{
		switch (type)
		{
		case NKM_INVENTORY_EXPAND_TYPE.NIET_EQUIP:
			userData.m_InventoryData.m_MaxItemEqipCount = count;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_UNIT:
			userData.m_ArmyData.m_MaxUnitCount = count;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP:
			userData.m_ArmyData.m_MaxShipCount = count;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_OPERATOR:
			userData.m_ArmyData.m_MaxOperatorCount = count;
			break;
		case NKM_INVENTORY_EXPAND_TYPE.NIET_TROPHY:
			userData.m_ArmyData.m_MaxTrophyCount = count;
			break;
		}
	}
}
