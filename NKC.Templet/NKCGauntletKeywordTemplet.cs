using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCGauntletKeywordTemplet : INKMTemplet
{
	private int keywordID;

	private string keywordNameStrID;

	private string keywordDescStrID;

	public int Key => keywordID;

	public int KeywordId => keywordID;

	public string KeywordNameStrId => keywordNameStrID;

	public string KeywordDescStrId => keywordDescStrID;

	public static NKCGauntletKeywordTemplet Find(int keywordId)
	{
		return NKMTempletContainer<NKCGauntletKeywordTemplet>.Find(keywordId);
	}

	public static NKCGauntletKeywordTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCGauntletKeywordTemplet.cs", 25))
		{
			return null;
		}
		NKCGauntletKeywordTemplet nKCGauntletKeywordTemplet = new NKCGauntletKeywordTemplet();
		if ((1u & (cNKMLua.GetData("KeywordID", ref nKCGauntletKeywordTemplet.keywordID) ? 1u : 0u) & (cNKMLua.GetData("KeywordNameStrID", ref nKCGauntletKeywordTemplet.keywordNameStrID) ? 1u : 0u) & (cNKMLua.GetData("KeywordDescStrID", ref nKCGauntletKeywordTemplet.keywordDescStrID) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKCGauntletKeywordTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
