using System.Collections.Generic;
using NKC.Util;
using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCOfficePartyTemplet : INKMTemplet
{
	public int IDX;

	public string IllustID;

	public int IllustRatio;

	public List<string> PartyEndAni;

	public NKMAssetName IllustName => NKMAssetName.ParseBundleName(IllustID, IllustID);

	public int Key => IDX;

	public static NKCOfficePartyTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCOfficePartyTemplet.cs", 24))
		{
			return null;
		}
		NKCOfficePartyTemplet nKCOfficePartyTemplet = new NKCOfficePartyTemplet();
		if ((1u & (cNKMLua.GetData("IDX", ref nKCOfficePartyTemplet.IDX) ? 1u : 0u) & (cNKMLua.GetData("IllustID", ref nKCOfficePartyTemplet.IllustID) ? 1u : 0u) & (cNKMLua.GetData("IllustRatio", ref nKCOfficePartyTemplet.IllustRatio) ? 1u : 0u) & (cNKMLua.GetDataList("PartyEndAni", out nKCOfficePartyTemplet.PartyEndAni, nullIfEmpty: false) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKCOfficePartyTemplet;
	}

	public static NKCOfficePartyTemplet Find(int ID)
	{
		return NKMTempletContainer<NKCOfficePartyTemplet>.Find(ID);
	}

	public static NKCOfficePartyTemplet GetRandomTemplet()
	{
		return NKCTempletUtility.PickRatio(NKMTempletContainer<NKCOfficePartyTemplet>.Values, (NKCOfficePartyTemplet x) => x.IllustRatio);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
