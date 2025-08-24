using System.Collections.Generic;
using NKM;
using NKM.Templet;

namespace NKC;

public class CompTemplet
{
	public class CompNUTB : IComparer<NKMUnitTempletBase>
	{
		public int Compare(NKMUnitTempletBase x, NKMUnitTempletBase y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			if (y.m_NKM_UNIT_GRADE > x.m_NKM_UNIT_GRADE)
			{
				return 1;
			}
			if (y.m_NKM_UNIT_GRADE < x.m_NKM_UNIT_GRADE)
			{
				return -1;
			}
			if (x.m_NKM_UNIT_STYLE_TYPE < y.m_NKM_UNIT_STYLE_TYPE)
			{
				return -1;
			}
			if (x.m_NKM_UNIT_STYLE_TYPE > y.m_NKM_UNIT_STYLE_TYPE)
			{
				return 1;
			}
			return x.m_UnitID.CompareTo(y.m_UnitID);
		}
	}

	public class CompNET : IComparer<NKMEquipTemplet>, IComparer<int>
	{
		public int Compare(NKMEquipTemplet x, NKMEquipTemplet y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			if (x.m_EquipUnitStyleType == NKM_UNIT_STYLE_TYPE.NUST_ENCHANT && y.m_EquipUnitStyleType != NKM_UNIT_STYLE_TYPE.NUST_ENCHANT)
			{
				return -1;
			}
			if (x.m_EquipUnitStyleType != NKM_UNIT_STYLE_TYPE.NUST_ENCHANT && y.m_EquipUnitStyleType == NKM_UNIT_STYLE_TYPE.NUST_ENCHANT)
			{
				return 1;
			}
			if (x.m_NKM_ITEM_TIER > y.m_NKM_ITEM_TIER)
			{
				return -1;
			}
			if (x.m_NKM_ITEM_TIER < y.m_NKM_ITEM_TIER)
			{
				return 1;
			}
			if (y.m_NKM_ITEM_GRADE > x.m_NKM_ITEM_GRADE)
			{
				return 1;
			}
			if (x.m_NKM_ITEM_GRADE > y.m_NKM_ITEM_GRADE)
			{
				return -1;
			}
			if (x.m_EquipUnitStyleType < y.m_EquipUnitStyleType)
			{
				return -1;
			}
			if (x.m_EquipUnitStyleType > y.m_EquipUnitStyleType)
			{
				return 1;
			}
			if (x.m_ItemEquipPosition < y.m_ItemEquipPosition)
			{
				return -1;
			}
			if (x.m_ItemEquipPosition > y.m_ItemEquipPosition)
			{
				return 1;
			}
			return x.m_ItemEquipID.CompareTo(y.m_ItemEquipID);
		}

		public int Compare(int equipItem_A, int equipItem_B)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipItem_A);
			NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(equipItem_B);
			return Compare(equipTemplet, equipTemplet2);
		}
	}

	public class CompNMT : IComparer<NKMItemMoldTemplet>
	{
		public int Compare(NKMItemMoldTemplet x, NKMItemMoldTemplet y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			if (x.m_Tier > y.m_Tier)
			{
				return -1;
			}
			if (x.m_Tier < y.m_Tier)
			{
				return 1;
			}
			if (y.m_Grade > x.m_Grade)
			{
				return 1;
			}
			if (x.m_Grade > y.m_Grade)
			{
				return -1;
			}
			if (x.m_RewardEquipUnitType < y.m_RewardEquipUnitType)
			{
				return -1;
			}
			if (x.m_RewardEquipUnitType > y.m_RewardEquipUnitType)
			{
				return 1;
			}
			if (x.m_RewardEquipPosition < y.m_RewardEquipPosition)
			{
				return -1;
			}
			if (x.m_RewardEquipPosition > y.m_RewardEquipPosition)
			{
				return 1;
			}
			return x.Key.CompareTo(y.Key);
		}
	}

	public class CompNIMT : IComparer<NKMItemMiscTemplet>
	{
		public int Compare(NKMItemMiscTemplet x, NKMItemMiscTemplet y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			if (x.m_NKM_ITEM_GRADE > y.m_NKM_ITEM_GRADE)
			{
				return -1;
			}
			if (x.m_NKM_ITEM_GRADE < y.m_NKM_ITEM_GRADE)
			{
				return 1;
			}
			if (x.m_ItemMiscType < y.m_ItemMiscType)
			{
				return -1;
			}
			if (x.m_ItemMiscType > y.m_ItemMiscType)
			{
				return 1;
			}
			return x.m_ItemMiscID.CompareTo(y.m_ItemMiscID);
		}
	}
}
