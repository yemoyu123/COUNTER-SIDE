using NKM.Templet;

namespace NKM.Unit;

public sealed class NKMUnitExpTemplet
{
	public int m_iLevel;

	public int m_iExpRequired;

	public int m_iExpCumulated;

	public int Key => m_iLevel;

	public static NKMUnitExpTemplet LoadFromLUA(NKMLua lua)
	{
		return new NKMUnitExpTemplet
		{
			m_iLevel = lua.GetInt32("m_iLevel"),
			m_iExpCumulated = lua.GetInt32("m_iExpCumulated")
		};
	}

	public static NKMUnitExpTemplet FindByUnitId(int unitId, int unitLevel)
	{
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitId);
		if (nKMUnitTempletBase == null || nKMUnitTempletBase.ExpTable == null)
		{
			return null;
		}
		return nKMUnitTempletBase.ExpTable.Get(unitLevel);
	}
}
