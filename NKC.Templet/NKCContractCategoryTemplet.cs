using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCContractCategoryTemplet : INKMTemplet
{
	public enum TabType
	{
		Basic,
		Awaken,
		FollowTarget,
		Hidden,
		Confirm
	}

	public int m_CategoryID;

	public int IDX;

	public string m_Name;

	public TabType m_Type;

	public int Key => m_CategoryID;

	public static NKCContractCategoryTemplet Find(int id)
	{
		return NKMTempletContainer<NKCContractCategoryTemplet>.Find(id);
	}

	public string GetName()
	{
		return NKCStringTable.GetString(m_Name);
	}

	public static NKCContractCategoryTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCContractCategoryTemplet.cs", 35))
		{
			return null;
		}
		NKCContractCategoryTemplet nKCContractCategoryTemplet = new NKCContractCategoryTemplet();
		int num = 1 & (cNKMLua.GetData("m_CategoryID", ref nKCContractCategoryTemplet.m_CategoryID) ? 1 : 0);
		cNKMLua.GetData("IDX", ref nKCContractCategoryTemplet.IDX);
		cNKMLua.GetData("m_Name", ref nKCContractCategoryTemplet.m_Name);
		cNKMLua.GetData("m_Type", ref nKCContractCategoryTemplet.m_Type);
		if (num == 0)
		{
			return null;
		}
		return nKCContractCategoryTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
