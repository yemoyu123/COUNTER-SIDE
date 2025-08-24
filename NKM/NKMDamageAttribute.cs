namespace NKM;

public class NKMDamageAttribute
{
	public bool m_bTrueDamage;

	public bool m_bForceCritical;

	public bool m_bNoCritical;

	public void LoadFromLua(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_bTrueDamage", ref m_bTrueDamage);
		cNKMLua.GetData("m_bForceCritical", ref m_bForceCritical);
		cNKMLua.GetData("m_bNoCritical", ref m_bNoCritical);
	}

	public static NKMDamageAttribute LoadFromLua(NKMLua cNKMLua, string tableName)
	{
		if (cNKMLua.OpenTable(tableName))
		{
			NKMDamageAttribute nKMDamageAttribute = new NKMDamageAttribute();
			nKMDamageAttribute.LoadFromLua(cNKMLua);
			cNKMLua.CloseTable();
			return nKMDamageAttribute;
		}
		return null;
	}

	public void DeepCopyFromSource(NKMDamageAttribute source)
	{
		m_bTrueDamage = source.m_bTrueDamage;
		m_bForceCritical = source.m_bForceCritical;
		m_bNoCritical = source.m_bNoCritical;
	}
}
