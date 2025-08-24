using System;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMPostTemplet : INKMTemplet
{
	private int postId;

	private NKM_POST_TYPE postType;

	private bool allowReceiveAll = true;

	private int postPeriod;

	public int Key => postId;

	public NKM_POST_TYPE? PostType => postType;

	public bool AllowReceiveAll => allowReceiveAll;

	public int PostPeriod => postPeriod;

	public static NKMPostTemplet Find(int key)
	{
		return NKMTempletContainer<NKMPostTemplet>.Find((NKMPostTemplet x) => x.Key == key);
	}

	public static NKMPostTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPostTemplet.cs", 22))
		{
			return null;
		}
		NKMPostTemplet nKMPostTemplet = new NKMPostTemplet();
		nKMPostTemplet.postId = lua.GetInt32("m_PostID");
		if (Enum.TryParse<NKM_POST_TYPE>(lua.GetString("m_PostType"), out var result))
		{
			nKMPostTemplet.postType = result;
		}
		nKMPostTemplet.allowReceiveAll = lua.GetBoolean("m_AllowReceiveAll", defaultValue: true);
		nKMPostTemplet.postPeriod = lua.GetInt32("m_PostPeriod", 0);
		return nKMPostTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
