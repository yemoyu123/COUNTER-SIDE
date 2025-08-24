using NKM;

namespace NKC;

public class NKCEffectTemplet
{
	public string m_Name = "";

	public int m_PoolCount;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData(1, ref m_Name);
		cNKMLua.GetData(2, ref m_PoolCount);
		return true;
	}
}
