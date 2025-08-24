using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMShipLevelUpTemplet : INKMTemplet
{
	private int m_ShipStarGrade;

	private int m_ShipLimitBreakGrade;

	private NKM_UNIT_GRADE m_ShipRareGrade;

	private int m_ShipMaxLevel;

	private int m_Credit;

	private int m_Eternium;

	private List<LevelupMaterial> m_LevelupMaterialList = new List<LevelupMaterial>(3);

	private static Dictionary<NKM_UNIT_GRADE, List<NKMShipLevelUpTemplet>> m_dicShipLevelUpTemplet;

	public NKM_UNIT_GRADE ShipUnitGrade => m_ShipRareGrade;

	public int ShipStarGrade => m_ShipStarGrade;

	public int ShipLimitBreakGrade => m_ShipLimitBreakGrade;

	public int ShipMaxLevel => m_ShipMaxLevel;

	public int ShipUpgradeCredit => m_Credit;

	public int ShipUpgradeEternium => m_Eternium;

	public List<LevelupMaterial> ShipLevelupMaterialList => m_LevelupMaterialList;

	public int Key => ShipStarGrade;

	public static NKMShipLevelUpTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMShipLevelUpTemplet nKMShipLevelUpTemplet = new NKMShipLevelUpTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("m_ShipStarGrade", ref nKMShipLevelUpTemplet.m_ShipStarGrade);
		cNKMLua.GetData("m_ShipLimitBreakGrade", ref nKMShipLevelUpTemplet.m_ShipLimitBreakGrade);
		flag &= cNKMLua.GetData("m_ShipRareGrade", ref nKMShipLevelUpTemplet.m_ShipRareGrade);
		flag &= cNKMLua.GetData("m_ShipMaxLevel", ref nKMShipLevelUpTemplet.m_ShipMaxLevel);
		flag &= cNKMLua.GetData("m_Credit", ref nKMShipLevelUpTemplet.m_Credit);
		flag &= cNKMLua.GetData("m_Eternium", ref nKMShipLevelUpTemplet.m_Eternium);
		LevelupMaterial item = default(LevelupMaterial);
		for (int i = 0; i < 3; i++)
		{
			int rValue = 0;
			int rValue2 = 0;
			string text = (i + 1).ToString("D");
			cNKMLua.GetData("m_LevelupMaterialItemID" + text, ref rValue);
			cNKMLua.GetData("m_LevelupMaterialCount" + text, ref rValue2);
			if (rValue != 0 && rValue2 > 0)
			{
				item.m_LevelupMaterialItemID = rValue;
				item.m_LevelupMaterialCount = rValue2;
				nKMShipLevelUpTemplet.m_LevelupMaterialList.Add(item);
			}
		}
		if (!flag)
		{
			return null;
		}
		return nKMShipLevelUpTemplet;
	}

	public static bool LoadFromLua(string fileName, string tableName)
	{
		IEnumerable<NKMShipLevelUpTemplet> enumerable = NKMTempletLoader.LoadCommonPath("AB_SCRIPT", fileName, tableName, LoadFromLUA);
		if (enumerable != null)
		{
			m_dicShipLevelUpTemplet = (from e in enumerable
				group e by e.m_ShipRareGrade).ToDictionary((IGrouping<NKM_UNIT_GRADE, NKMShipLevelUpTemplet> e) => e.Key, (IGrouping<NKM_UNIT_GRADE, NKMShipLevelUpTemplet> e) => e.ToList());
		}
		return m_dicShipLevelUpTemplet != null;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public static NKMShipLevelUpTemplet Find(int starGrade, NKM_UNIT_GRADE key = NKM_UNIT_GRADE.NUG_N, int ShipLimitBreakGrade = 0)
	{
		if (m_dicShipLevelUpTemplet.ContainsKey(key))
		{
			foreach (NKMShipLevelUpTemplet item in m_dicShipLevelUpTemplet[key])
			{
				if (item.Key == starGrade && item.m_ShipLimitBreakGrade == ShipLimitBreakGrade)
				{
					return item;
				}
			}
		}
		return null;
	}

	public static int GetMaxLevel(int starGrade, NKM_UNIT_GRADE grade = NKM_UNIT_GRADE.NUG_N, int ShipLimitBreakGrade = 0)
	{
		if (m_dicShipLevelUpTemplet.ContainsKey(grade))
		{
			foreach (NKMShipLevelUpTemplet item in m_dicShipLevelUpTemplet[grade])
			{
				if (item.m_ShipStarGrade == starGrade && item.m_ShipLimitBreakGrade == ShipLimitBreakGrade)
				{
					return item.m_ShipMaxLevel;
				}
			}
		}
		return 0;
	}

	public static int GetLevelUpCredit(int starGrade, NKM_UNIT_GRADE grade = NKM_UNIT_GRADE.NUG_N, int ShipLimitBreakGrade = 0)
	{
		if (m_dicShipLevelUpTemplet.ContainsKey(grade))
		{
			foreach (NKMShipLevelUpTemplet item in m_dicShipLevelUpTemplet[grade])
			{
				if (item.m_ShipStarGrade == starGrade && item.m_ShipLimitBreakGrade == ShipLimitBreakGrade)
				{
					return item.m_Credit;
				}
			}
		}
		return 0;
	}

	public static NKMShipLevelUpTemplet GetShipLevelupTempletByLevel(int targetLevel, NKM_UNIT_GRADE grade = NKM_UNIT_GRADE.NUG_N, int ShipLimitBreakGrade = 0)
	{
		NKMShipLevelUpTemplet result = new NKMShipLevelUpTemplet();
		if (m_dicShipLevelUpTemplet.ContainsKey(grade))
		{
			foreach (NKMShipLevelUpTemplet item in m_dicShipLevelUpTemplet[grade])
			{
				if (item.m_ShipMaxLevel > targetLevel && item.m_ShipLimitBreakGrade == ShipLimitBreakGrade)
				{
					return item;
				}
				result = item;
			}
		}
		return result;
	}

	public static int GetShipMinLimitBreakLevel(int level, NKM_UNIT_GRADE unitGrade)
	{
		NKMShipLevelUpTemplet nKMShipLevelUpTemplet = null;
		if (m_dicShipLevelUpTemplet.ContainsKey(unitGrade))
		{
			foreach (NKMShipLevelUpTemplet item in m_dicShipLevelUpTemplet[unitGrade])
			{
				if ((nKMShipLevelUpTemplet == null || nKMShipLevelUpTemplet.m_ShipLimitBreakGrade >= item.m_ShipLimitBreakGrade) && item.m_ShipMaxLevel >= level)
				{
					nKMShipLevelUpTemplet = item;
				}
			}
		}
		return nKMShipLevelUpTemplet?.m_ShipLimitBreakGrade ?? 0;
	}
}
