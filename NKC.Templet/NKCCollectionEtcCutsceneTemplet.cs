using Cs.Logging;
using NKC.Templet.Base;
using NKM;

namespace NKC.Templet;

public class NKCCollectionEtcCutsceneTemplet : INKCTemplet
{
	public int Idx;

	public string OpenTag;

	public NKCCollectionManager.COLLECTION_ETC_TAB_ID m_SubTabId;

	public string m_SubTabName;

	public int m_SubIndex;

	public string m_SubIndexName;

	public string m_CutSceneDesc;

	public UnlockInfo UnlockInfo;

	public bool m_bExclude;

	public int Key => Idx;

	public static NKCCollectionEtcCutsceneTemplet LoadFromLUA(NKMLua lua)
	{
		NKCCollectionEtcCutsceneTemplet nKCCollectionEtcCutsceneTemplet = new NKCCollectionEtcCutsceneTemplet();
		int num = (int)(1u & (lua.GetData("IDX", ref nKCCollectionEtcCutsceneTemplet.Idx) ? 1u : 0u)) & (lua.GetData("m_OpenTag", ref nKCCollectionEtcCutsceneTemplet.OpenTag) ? 1 : 0);
		lua.GetData("m_SubTabId", ref nKCCollectionEtcCutsceneTemplet.m_SubTabId);
		lua.GetData("m_SubTabName", ref nKCCollectionEtcCutsceneTemplet.m_SubTabName);
		lua.GetData("m_SubIndex", ref nKCCollectionEtcCutsceneTemplet.m_SubIndex);
		lua.GetData("m_SubIndexName", ref nKCCollectionEtcCutsceneTemplet.m_SubIndexName);
		lua.GetData("m_CutSceneDesc", ref nKCCollectionEtcCutsceneTemplet.m_CutSceneDesc);
		nKCCollectionEtcCutsceneTemplet.UnlockInfo = UnlockInfo.LoadFromLua(lua);
		lua.GetData("m_bExclude", ref nKCCollectionEtcCutsceneTemplet.m_bExclude);
		if (num == 0)
		{
			Log.Error("NKCCollectionEtcCutsceneTemplet data is not valid", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCCollectionEtcCutsceneTemplet.cs", 43);
			return null;
		}
		return nKCCollectionEtcCutsceneTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
