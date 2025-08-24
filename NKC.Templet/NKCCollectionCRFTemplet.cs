using Cs.Logging;
using NKC.Templet.Base;
using NKM;

namespace NKC.Templet;

public class NKCCollectionCRFTemplet : INKCTemplet
{
	public int ProfileAmountID;

	public string CRFType;

	public int ProfileAmountMin;

	public int ProfileAmountMax;

	public string ProfileAmountDescStrID;

	public int Key => ProfileAmountID;

	public static NKCCollectionCRFTemplet LoadFromLUA(NKMLua lua)
	{
		NKCCollectionCRFTemplet nKCCollectionCRFTemplet = new NKCCollectionCRFTemplet();
		if ((1u & (lua.GetData("ProfileAmountID", ref nKCCollectionCRFTemplet.ProfileAmountID) ? 1u : 0u) & (lua.GetData("CRFType", ref nKCCollectionCRFTemplet.CRFType) ? 1u : 0u) & (lua.GetData("ProfileAmountMin", ref nKCCollectionCRFTemplet.ProfileAmountMin) ? 1u : 0u) & (lua.GetData("ProfileAmountMax", ref nKCCollectionCRFTemplet.ProfileAmountMax) ? 1u : 0u) & (lua.GetData("ProfileAmountDescStrID", ref nKCCollectionCRFTemplet.ProfileAmountDescStrID) ? 1u : 0u)) == 0)
		{
			Log.Error("NKCCollectionCRFTemplet data is not valid", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCCollectionCRFTemplet.cs", 33);
			return null;
		}
		return nKCCollectionCRFTemplet;
	}
}
