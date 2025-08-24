using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMLobbyFaceTemplet : INKMTemplet
{
	public const int TOUCH_ANI_LOBBY_FACE_ID = -10;

	public int Key;

	public int reqLoyalty;

	public string AnimationName;

	public string strFaceName;

	int INKMTemplet.Key => Key;

	public static NKMLobbyFaceTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMLobbyFaceTemplet nKMLobbyFaceTemplet = new NKMLobbyFaceTemplet();
		if ((1u & (cNKMLua.GetData("Key", ref nKMLobbyFaceTemplet.Key) ? 1u : 0u) & (cNKMLua.GetData("reqLoyalty", ref nKMLobbyFaceTemplet.reqLoyalty) ? 1u : 0u) & (cNKMLua.GetData("AnimationName", ref nKMLobbyFaceTemplet.AnimationName) ? 1u : 0u) & (cNKMLua.GetData("strFaceName", ref nKMLobbyFaceTemplet.strFaceName) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKMLobbyFaceTemplet;
	}

	public bool CanUseFace(NKMUnitData unitData)
	{
		if (reqLoyalty == 0)
		{
			return true;
		}
		if (unitData == null)
		{
			return false;
		}
		if (unitData.IsPermanentContract)
		{
			return true;
		}
		return unitData.loyalty >= reqLoyalty;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
