using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCGuideTempletImage : INKMTemplet
{
	public int ID;

	public string ID_STRING;

	public string GUIDE_BUNDLE_PATH;

	public string GUIDE_IMAGE_PATH;

	public bool IsSprite = true;

	public int Key => ID;

	public static NKCGuideTempletImage LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCGuideTemplet.cs", 78))
		{
			return null;
		}
		NKCGuideTempletImage nKCGuideTempletImage = new NKCGuideTempletImage();
		int num = (int)(1u & (cNKMLua.GetData("ID", ref nKCGuideTempletImage.ID) ? 1u : 0u) & (cNKMLua.GetData("ID_STRING", ref nKCGuideTempletImage.ID_STRING) ? 1u : 0u) & (cNKMLua.GetData("GUIDE_BUNDLE_PATH", ref nKCGuideTempletImage.GUIDE_BUNDLE_PATH) ? 1u : 0u)) & (cNKMLua.GetData("GUIDE_IMAGE_PATH", ref nKCGuideTempletImage.GUIDE_IMAGE_PATH) ? 1 : 0);
		cNKMLua.GetData("IsSprite", ref nKCGuideTempletImage.IsSprite);
		if (num == 0)
		{
			return null;
		}
		return nKCGuideTempletImage;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
