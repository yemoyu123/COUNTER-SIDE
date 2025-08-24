using Cs.Logging;
using NKC.Templet.Base;
using NKM;

namespace NKC.Templet;

public class NKCCollectionProfileToolTipTemplet : INKCTemplet
{
	public int ProfileTypeID;

	public string ProfileType;

	public string ProfileTypeDesc;

	public int Key => ProfileTypeID;

	public static NKCCollectionProfileToolTipTemplet LoadFromLUA(NKMLua lua)
	{
		NKCCollectionProfileToolTipTemplet nKCCollectionProfileToolTipTemplet = new NKCCollectionProfileToolTipTemplet();
		int num = (int)(1u & (lua.GetData("ProfileTypeID", ref nKCCollectionProfileToolTipTemplet.ProfileTypeID) ? 1u : 0u)) & (lua.GetData("ProfileType", ref nKCCollectionProfileToolTipTemplet.ProfileType) ? 1 : 0);
		lua.GetData("ProfileTypeDesc", ref nKCCollectionProfileToolTipTemplet.ProfileTypeDesc);
		if (num == 0)
		{
			Log.Error("NKCCollectionProfileDescTemplet data is not valid", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCCollectionEmployeeTemplet.cs", 189);
			return null;
		}
		return nKCCollectionProfileToolTipTemplet;
	}
}
