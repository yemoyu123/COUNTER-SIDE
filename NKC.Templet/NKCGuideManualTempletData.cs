using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCGuideManualTempletData : INKMTemplet
{
	public int CATEGORY_ID;

	public string CATEGORY_STRING;

	public string CATEGORY_TITLE;

	public string ARTICLE_ID;

	public string ARTICLE_STRING_ID;

	public string GUIDE_ID_STRING;

	public string m_ContentsVersionStart = "";

	public string m_ContentsVersionEnd = "";

	public int Key => CATEGORY_ID;

	public static NKCGuideManualTempletData LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCGuideManualTemplet.cs", 114))
		{
			return null;
		}
		NKCGuideManualTempletData nKCGuideManualTempletData = new NKCGuideManualTempletData();
		cNKMLua.GetData("m_ContentsVersionStart", ref nKCGuideManualTempletData.m_ContentsVersionStart);
		cNKMLua.GetData("m_ContentsVersionEnd", ref nKCGuideManualTempletData.m_ContentsVersionEnd);
		if ((1u & (cNKMLua.GetData("CATEGORY_ID", ref nKCGuideManualTempletData.CATEGORY_ID) ? 1u : 0u) & (cNKMLua.GetData("CATEGORY_STRING", ref nKCGuideManualTempletData.CATEGORY_STRING) ? 1u : 0u) & (cNKMLua.GetData("CATEGORY_TITLE", ref nKCGuideManualTempletData.CATEGORY_TITLE) ? 1u : 0u) & (cNKMLua.GetData("ARTICLE_ID", ref nKCGuideManualTempletData.ARTICLE_ID) ? 1u : 0u) & (cNKMLua.GetData("ARTICLE_STRING_ID", ref nKCGuideManualTempletData.ARTICLE_STRING_ID) ? 1u : 0u) & (cNKMLua.GetData("GUIDE_ID_STRING", ref nKCGuideManualTempletData.GUIDE_ID_STRING) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKCGuideManualTempletData;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
