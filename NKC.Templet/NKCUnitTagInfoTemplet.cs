using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCUnitTagInfoTemplet : INKMTemplet
{
	public enum Category
	{
		CORE,
		BUFF,
		DEBUFF,
		STATUS
	}

	public enum SuffixType
	{
		NONE,
		POSITIVE,
		NEGATIVE
	}

	public int index;

	public string strID;

	public Category category = Category.STATUS;

	public string tagStringKey;

	public SuffixType m_Suffix;

	private static int s_index;

	public int Key => index;

	public static NKCUnitTagInfoTemplet Find(string tagID)
	{
		return NKMTempletContainer<NKCUnitTagInfoTemplet>.Find(tagID);
	}

	public static NKCUnitTagInfoTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCUnitTagInfoTemplet.cs", 41))
		{
			return null;
		}
		NKCUnitTagInfoTemplet nKCUnitTagInfoTemplet = new NKCUnitTagInfoTemplet();
		nKCUnitTagInfoTemplet.index = s_index;
		s_index++;
		int num = (int)(1u & (cNKMLua.GetData("NKM_UNIT_TAG", ref nKCUnitTagInfoTemplet.strID) ? 1u : 0u)) & (cNKMLua.GetData("TAG_STRING_ID", ref nKCUnitTagInfoTemplet.tagStringKey) ? 1 : 0);
		cNKMLua.GetData("m_Suffix", ref nKCUnitTagInfoTemplet.m_Suffix);
		cNKMLua.GetData("m_Category", ref nKCUnitTagInfoTemplet.category);
		if (num == 0)
		{
			return null;
		}
		return nKCUnitTagInfoTemplet;
	}

	public string GetTagString(bool AddHashMark = true)
	{
		if (AddHashMark)
		{
			return m_Suffix switch
			{
				SuffixType.POSITIVE => $"#{NKCStringTable.GetString(tagStringKey)}+", 
				SuffixType.NEGATIVE => $"#{NKCStringTable.GetString(tagStringKey)}-", 
				_ => $"#{NKCStringTable.GetString(tagStringKey)}", 
			};
		}
		return m_Suffix switch
		{
			SuffixType.POSITIVE => $"{NKCStringTable.GetString(tagStringKey)}+", 
			SuffixType.NEGATIVE => $"{NKCStringTable.GetString(tagStringKey)}-", 
			_ => NKCStringTable.GetString(tagStringKey), 
		};
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
