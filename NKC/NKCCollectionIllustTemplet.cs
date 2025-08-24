using System.Collections.Generic;

namespace NKC;

public class NKCCollectionIllustTemplet
{
	public int m_CategoryID;

	private string m_CategoryTitle;

	private string m_CategorySubTitle;

	public Dictionary<int, NKCCollectionIllustData> m_dicIllustData = new Dictionary<int, NKCCollectionIllustData>();

	public NKCCollectionIllustTemplet(int CategoryID, string CategoryTitle, string CategorySubTitle, int BGGroupID, NKCCollectionIllustData IllustData)
	{
		m_CategoryID = CategoryID;
		m_CategoryTitle = CategoryTitle;
		m_CategorySubTitle = CategorySubTitle;
		m_dicIllustData.Add(BGGroupID, IllustData);
	}

	public string GetCategoryTitle()
	{
		return NKCStringTable.GetString(m_CategoryTitle);
	}

	public string GetCategorySubTitle()
	{
		return NKCStringTable.GetString(m_CategorySubTitle);
	}
}
