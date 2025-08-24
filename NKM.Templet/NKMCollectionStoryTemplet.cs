using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMCollectionStoryTemplet : INKMTemplet
{
	public int m_StageID;

	public List<UnlockInfo> m_UnlockReqList = new List<UnlockInfo>();

	public EPISODE_CATEGORY m_EPCategory;

	public int m_EpisodeID;

	public int m_ActID;

	public int Key => m_StageID;

	public static NKMCollectionStoryTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionStoryTemplet.cs", 21))
		{
			return null;
		}
		NKMCollectionStoryTemplet nKMCollectionStoryTemplet = new NKMCollectionStoryTemplet();
		cNKMLua.GetData("m_StageID", ref nKMCollectionStoryTemplet.m_StageID);
		nKMCollectionStoryTemplet.m_UnlockReqList.Clear();
		STAGE_UNLOCK_REQ_TYPE result = STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE;
		int rValue = 0;
		string rValue2 = "";
		int num = (int)(1u & (cNKMLua.GetData("m_UnlockReqType", ref result) ? 1u : 0u)) & (cNKMLua.GetData("m_UnlockReqValue", ref rValue) ? 1 : 0);
		cNKMLua.GetData("m_UnlockReqStrValue", ref rValue2);
		nKMCollectionStoryTemplet.m_UnlockReqList.Add(new UnlockInfo(result, rValue, rValue2));
		int num2 = (int)(1u & (cNKMLua.GetData("m_UnlockReqType_2", ref result) ? 1u : 0u)) & (cNKMLua.GetData("m_UnlockReqValue_2", ref rValue) ? 1 : 0);
		cNKMLua.GetData("m_UnlockReqStrValue_2", ref rValue2);
		if (num2 != 0)
		{
			nKMCollectionStoryTemplet.m_UnlockReqList.Add(new UnlockInfo(result, rValue, rValue2));
		}
		cNKMLua.GetData("m_EPCategory", ref nKMCollectionStoryTemplet.m_EPCategory);
		cNKMLua.GetData("m_EpisodeID", ref nKMCollectionStoryTemplet.m_EpisodeID);
		cNKMLua.GetData("m_actID", ref nKMCollectionStoryTemplet.m_ActID);
		if (num == 0)
		{
			return null;
		}
		return nKMCollectionStoryTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
