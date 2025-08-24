using NKM;
using NKM.Templet.Base;

namespace NKC;

public class NKCBackgroundTemplet : INKMTemplet
{
	public enum BgType
	{
		Background,
		Image,
		CutsceneObject
	}

	public int m_ItemMiscID;

	public string m_Background_Prefab;

	public string m_Background_Music;

	public bool m_bBackground_CamMove;

	public BgType m_BgType;

	public bool IsFlashbackBG
	{
		get
		{
			if (m_BgType != BgType.Image)
			{
				return m_BgType == BgType.CutsceneObject;
			}
			return true;
		}
	}

	public int Key => m_ItemMiscID;

	public static NKCBackgroundTemplet Find(int id)
	{
		return NKMTempletContainer<NKCBackgroundTemplet>.Find(id);
	}

	public static NKCBackgroundTemplet LoadFromLUA(NKMLua lua)
	{
		NKCBackgroundTemplet nKCBackgroundTemplet = new NKCBackgroundTemplet();
		int num = (int)(1u & (lua.GetData("m_ItemMiscID", ref nKCBackgroundTemplet.m_ItemMiscID) ? 1u : 0u) & (lua.GetData("m_Background_Prefab", ref nKCBackgroundTemplet.m_Background_Prefab) ? 1u : 0u)) & (lua.GetData("m_Background_Music", ref nKCBackgroundTemplet.m_Background_Music) ? 1 : 0);
		lua.GetData("m_bBackground_CamMove", ref nKCBackgroundTemplet.m_bBackground_CamMove);
		nKCBackgroundTemplet.m_BgType = lua.GetEnum("m_BgType", BgType.Background);
		if (num == 0)
		{
			return null;
		}
		return nKCBackgroundTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
