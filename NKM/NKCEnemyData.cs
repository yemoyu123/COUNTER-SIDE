using System.Collections.Generic;

namespace NKM;

public class NKCEnemyData
{
	public class CompNED : IComparer<NKCEnemyData>
	{
		public int Compare(NKCEnemyData x, NKCEnemyData y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			if (y.m_NKM_BOSS_TYPE > x.m_NKM_BOSS_TYPE)
			{
				return 1;
			}
			if (y.m_NKM_BOSS_TYPE < x.m_NKM_BOSS_TYPE)
			{
				return -1;
			}
			if (y.m_Level >= x.m_Level)
			{
				return 1;
			}
			return -1;
		}
	}

	public string m_UnitStrID = "";

	public string m_ChangeUnitName = "";

	public int m_SkinID;

	public NKM_BOSS_TYPE m_NKM_BOSS_TYPE;

	public int m_Level;

	public string Key
	{
		get
		{
			if (string.IsNullOrEmpty(m_ChangeUnitName))
			{
				return m_UnitStrID;
			}
			return $"{m_UnitStrID}@{m_ChangeUnitName}";
		}
	}
}
