using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMCollectionTagTemplet : INKMTemplet
{
	private int m_TagOrder;

	private string m_TagName = "";

	public int Key => m_TagOrder;

	public int TagOrder => m_TagOrder;

	public string TagName => m_TagName;

	public static NKMCollectionTagTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMCollectionTagTemplet nKMCollectionTagTemplet = new NKMCollectionTagTemplet();
		if ((1u & (cNKMLua.GetData("m_TagOrder", ref nKMCollectionTagTemplet.m_TagOrder) ? 1u : 0u) & (cNKMLua.GetData("m_TagName", ref nKMCollectionTagTemplet.m_TagName) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKMCollectionTagTemplet;
	}

	public static NKMCollectionTagTemplet Find(int key)
	{
		return NKMTempletContainer<NKMCollectionTagTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
