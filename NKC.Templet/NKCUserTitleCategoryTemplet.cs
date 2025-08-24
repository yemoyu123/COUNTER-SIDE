using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCUserTitleCategoryTemplet : INKMTemplet
{
	private int titleCategoryID;

	private string categoryString;

	public int Key => titleCategoryID;

	public int TitleCategoryID => titleCategoryID;

	public string CategoryString => categoryString;

	public static NKCUserTitleCategoryTemplet Find(int key)
	{
		return NKMTempletContainer<NKCUserTitleCategoryTemplet>.Find(key);
	}

	public static NKCUserTitleCategoryTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCUserTitleCategoryTemplet.cs", 19))
		{
			return null;
		}
		NKCUserTitleCategoryTemplet nKCUserTitleCategoryTemplet = new NKCUserTitleCategoryTemplet();
		cNKMLua.GetData("TitleCategoryID", ref nKCUserTitleCategoryTemplet.titleCategoryID);
		cNKMLua.GetData("CategoryString", ref nKCUserTitleCategoryTemplet.categoryString);
		return nKCUserTitleCategoryTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (titleCategoryID == 0)
		{
			NKMTempletError.Add("NKCUserTitleCategoryTemplet Error - titleCategoryID == 0", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCUserTitleCategoryTemplet.cs", 38);
		}
		else if (string.IsNullOrEmpty(categoryString))
		{
			NKMTempletError.Add($"NKCUserTitleCategoryTemplet Error - categoryString null : {TitleCategoryID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCUserTitleCategoryTemplet.cs", 44);
		}
	}
}
