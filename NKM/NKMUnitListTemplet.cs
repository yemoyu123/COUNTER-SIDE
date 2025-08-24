using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMUnitListTemplet : INKMTemplet
{
	public enum UnitGroupType
	{
		BASIC,
		AWAKEN,
		REARM,
		SSR,
		SR,
		R,
		N
	}

	public int ID;

	public List<int> UnitList_A = new List<int>();

	public List<int> UnitList_B = new List<int>();

	public HashSet<UnitGroupType> m_hsUnitGroupType = new HashSet<UnitGroupType>();

	public int Key => ID;

	public int GetUnit(NKM_TEAM_TYPE eTeam, HashSet<int> excludeUnitList = null)
	{
		if (eTeam == NKM_TEAM_TYPE.NTT_INVALID)
		{
			eTeam = (NKM_TEAM_TYPE)NKMRandom.Range(1, 4);
		}
		List<int> list = GetUnitList(eTeam);
		if (list == null)
		{
			return -1;
		}
		if (excludeUnitList != null && excludeUnitList.Count > 0)
		{
			List<int> list2 = new List<int>();
			foreach (int item in list)
			{
				if (!NKMUnitManager.CheckContainsBaseUnit(excludeUnitList, item))
				{
					list2.Add(item);
				}
			}
			list = list2;
		}
		if (list.Count == 0)
		{
			return -1;
		}
		return list[NKMRandom.Range(0, list.Count)];
	}

	public List<int> GetUnitList(NKM_TEAM_TYPE eTeam)
	{
		switch (eTeam)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			return UnitList_A.FindAll(CheckUnitUsable);
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			return UnitList_B.FindAll(CheckUnitUsable);
		default:
			return null;
		}
	}

	private bool CheckUnitUsable(int unitID)
	{
		return NKMUnitManager.GetUnitTempletBase(unitID)?.PickupEnableByTag ?? false;
	}

	public static NKMUnitListTemplet Find(int key)
	{
		return NKMTempletContainer<NKMUnitListTemplet>.Find(key);
	}

	public static NKMUnitListTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMUnitListTemplet nKMUnitListTemplet = new NKMUnitListTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("RandomUnitID", ref nKMUnitListTemplet.ID);
		cNKMLua.GetDataList("UnitList_A", out nKMUnitListTemplet.UnitList_A, nullIfEmpty: false);
		cNKMLua.GetDataList("UnitList_B", out nKMUnitListTemplet.UnitList_B, nullIfEmpty: false);
		cNKMLua.GetDataListEnum("UnitGroupType", nKMUnitListTemplet.m_hsUnitGroupType);
		if (nKMUnitListTemplet.UnitList_A.Count + nKMUnitListTemplet.UnitList_B.Count + nKMUnitListTemplet.m_hsUnitGroupType.Count == 0)
		{
			flag = false;
		}
		if (!flag)
		{
			return null;
		}
		return nKMUnitListTemplet;
	}

	public static void LoadFromLua()
	{
		NKMTempletContainer<NKMUnitListTemplet>.Load("AB_SCRIPT", "LUA_RANDOM_UNIT_TEMPLET", "RANDOM_UNIT_TEMPLET", LoadFromLUA);
	}

	public void Join()
	{
		if (m_hsUnitGroupType.Count <= 0)
		{
			return;
		}
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<int> hashSet2 = new HashSet<int>();
		for (int i = 0; i < UnitList_A.Count; i++)
		{
			if (!hashSet.Contains(UnitList_A[i]))
			{
				hashSet.Add(UnitList_A[i]);
			}
		}
		for (int j = 0; j < UnitList_B.Count; j++)
		{
			if (!hashSet2.Contains(UnitList_B[j]))
			{
				hashSet2.Add(UnitList_B[j]);
			}
		}
		foreach (NKMUnitTempletBase value in NKMTempletContainer<NKMUnitTempletBase>.Values)
		{
			if (value.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && value.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER && !value.m_bMonster && value.m_bContractable && !NKMCommonConst.IgnoreCollaboUnitID.Contains(value.m_UnitID) && IsValidUnit(m_hsUnitGroupType, value))
			{
				if (!hashSet.Contains(value.m_UnitID))
				{
					hashSet.Add(value.m_UnitID);
				}
				if (!hashSet2.Contains(value.m_UnitID))
				{
					hashSet2.Add(value.m_UnitID);
				}
			}
		}
		UnitList_A = hashSet.ToList();
		UnitList_B = hashSet2.ToList();
	}

	private bool IsValidUnit(HashSet<UnitGroupType> hsGroupType, NKMUnitTempletBase templetBase)
	{
		if (templetBase == null)
		{
			return false;
		}
		using (HashSet<UnitGroupType>.Enumerator enumerator = hsGroupType.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
				case UnitGroupType.SSR:
					if (templetBase.m_NKM_UNIT_GRADE != NKM_UNIT_GRADE.NUG_SSR)
					{
						return false;
					}
					break;
				case UnitGroupType.SR:
					if (templetBase.m_NKM_UNIT_GRADE != NKM_UNIT_GRADE.NUG_SR)
					{
						return false;
					}
					break;
				case UnitGroupType.R:
					if (templetBase.m_NKM_UNIT_GRADE != NKM_UNIT_GRADE.NUG_R)
					{
						return false;
					}
					break;
				case UnitGroupType.N:
					if (templetBase.m_NKM_UNIT_GRADE != NKM_UNIT_GRADE.NUG_N)
					{
						return false;
					}
					break;
				case UnitGroupType.BASIC:
					if (templetBase.m_bAwaken)
					{
						return false;
					}
					if (templetBase.m_BaseUnitID > 0 && templetBase.m_RearmGrade > 0 && templetBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
					{
						return false;
					}
					break;
				case UnitGroupType.AWAKEN:
					if (!templetBase.m_bAwaken)
					{
						return false;
					}
					break;
				case UnitGroupType.REARM:
					if (templetBase.m_BaseUnitID <= 0 || templetBase.m_RearmGrade <= 0 || templetBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
					{
						return false;
					}
					break;
				}
			}
		}
		return true;
	}

	public void Validate()
	{
	}
}
