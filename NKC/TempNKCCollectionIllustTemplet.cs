using NKC.Templet.Base;
using NKM;

namespace NKC;

public class TempNKCCollectionIllustTemplet : INKCTemplet
{
	public int m_CategoryID;

	public string m_CategoryTitle = "";

	public string m_CategorySubTitle = "";

	public int m_BGGroupID;

	private string m_BGGroupTitle = "";

	private string m_BGGroupText = "";

	public string m_BGThumbnailFileName = "";

	public string m_BGFileName = "";

	public string m_GameObjectBGAniName = "";

	public STAGE_UNLOCK_REQ_TYPE m_UnlockReqType;

	public int m_UnlockReqValue;

	public int Key => m_CategoryID;

	public static TempNKCCollectionIllustTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		TempNKCCollectionIllustTemplet tempNKCCollectionIllustTemplet = new TempNKCCollectionIllustTemplet();
		if ((1u & (cNKMLua.GetData("m_CategoryID", ref tempNKCCollectionIllustTemplet.m_CategoryID) ? 1u : 0u) & (cNKMLua.GetData("m_CategoryTitle", ref tempNKCCollectionIllustTemplet.m_CategoryTitle) ? 1u : 0u) & (cNKMLua.GetData("m_CategorySubtitle", ref tempNKCCollectionIllustTemplet.m_CategorySubTitle) ? 1u : 0u) & (cNKMLua.GetData("m_BGGroupID", ref tempNKCCollectionIllustTemplet.m_BGGroupID) ? 1u : 0u) & (cNKMLua.GetData("m_BGGroupTitle", ref tempNKCCollectionIllustTemplet.m_BGGroupTitle) ? 1u : 0u) & (cNKMLua.GetData("m_BGGroupText", ref tempNKCCollectionIllustTemplet.m_BGGroupText) ? 1u : 0u) & (cNKMLua.GetData("m_BGThumbnailFileName", ref tempNKCCollectionIllustTemplet.m_BGThumbnailFileName) ? 1u : 0u) & (cNKMLua.GetData("m_BGFileName", ref tempNKCCollectionIllustTemplet.m_BGFileName) ? 1u : 0u) & (cNKMLua.GetData("m_GameObjectBGAniName", ref tempNKCCollectionIllustTemplet.m_GameObjectBGAniName) ? 1u : 0u) & (cNKMLua.GetData("m_UnlockReqType", ref tempNKCCollectionIllustTemplet.m_UnlockReqType) ? 1u : 0u) & (cNKMLua.GetData("m_UnlockReqValue", ref tempNKCCollectionIllustTemplet.m_UnlockReqValue) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return tempNKCCollectionIllustTemplet;
	}

	public string GetBGGroupTitle()
	{
		return NKCStringTable.GetString(m_BGGroupTitle);
	}

	public string GetBGGroupText()
	{
		return NKCStringTable.GetString(m_BGGroupText);
	}
}
