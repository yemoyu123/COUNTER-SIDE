using NKC.Templet.Base;
using NKM;

namespace NKC;

public class NKCCollectionTagTemplet : INKCTemplet
{
	public short m_TagOrder;

	private string m_TagName;

	public int Key => m_TagOrder;

	public static NKCCollectionTagTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKCCollectionTagTemplet nKCCollectionTagTemplet = new NKCCollectionTagTemplet();
		if ((1u & (cNKMLua.GetData("m_TagOrder", ref nKCCollectionTagTemplet.m_TagOrder) ? 1u : 0u) & (cNKMLua.GetData("m_TagName", ref nKCCollectionTagTemplet.m_TagName) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKCCollectionTagTemplet;
	}

	public string GetTagName()
	{
		return NKCStringTable.GetString(m_TagName);
	}
}
