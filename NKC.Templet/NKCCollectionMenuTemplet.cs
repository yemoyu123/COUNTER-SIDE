using Cs.Logging;
using NKC.Templet.Base;
using NKC.UI.Collection;
using NKM;

namespace NKC.Templet;

public class NKCCollectionMenuTemplet : INKCTemplet
{
	public int Id;

	public string OpenTag;

	public string TabGroupID;

	public NKCUICollectionGeneral.CollectionType TabType = NKCUICollectionGeneral.CollectionType.CT_NONE;

	public string TabIconStrID;

	public string TabDescStrID;

	public string TabPrefabID;

	public bool UpperTab;

	public int Key => Id;

	public static NKCCollectionMenuTemplet LoadFromLUA(NKMLua lua)
	{
		NKCCollectionMenuTemplet nKCCollectionMenuTemplet = new NKCCollectionMenuTemplet();
		int num = (int)(1u & (lua.GetData("ID", ref nKCCollectionMenuTemplet.Id) ? 1u : 0u)) & (lua.GetData("OpenTag", ref nKCCollectionMenuTemplet.OpenTag) ? 1 : 0);
		lua.GetData("TabGroupID", ref nKCCollectionMenuTemplet.TabGroupID);
		lua.GetData("TabType", ref nKCCollectionMenuTemplet.TabType);
		lua.GetData("TabIconStrID", ref nKCCollectionMenuTemplet.TabIconStrID);
		lua.GetData("TabDescStrID", ref nKCCollectionMenuTemplet.TabDescStrID);
		lua.GetData("TabPrefabID", ref nKCCollectionMenuTemplet.TabPrefabID);
		lua.GetData("bUpperTab", ref nKCCollectionMenuTemplet.UpperTab);
		if (num == 0)
		{
			Log.Error("NKCCollectionMenuTemplet data is not valid", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCCollectionMenuTemplet.cs", 43);
			return null;
		}
		return nKCCollectionMenuTemplet;
	}
}
