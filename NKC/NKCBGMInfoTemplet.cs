using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;
using NKM.Templet.Office;

namespace NKC;

public class NKCBGMInfoTemplet : INKMTemplet
{
	public int m_Idx;

	public int m_OrderIdx;

	public string m_OpenTag;

	public string m_BgmID;

	public string m_BgmNameStringID;

	public string m_BgmAssetID;

	private int m_BgmVolume;

	public string m_BgmCoverIMGID;

	public string m_BgmUnlcokStringID;

	public NKC_BGM_COND m_UnlockCond;

	public List<int> m_UnlockCondValue1;

	public int m_UnlockCondValue2;

	public bool bAllCollecte;

	public int Key => m_Idx;

	public float BGMVolume => (float)m_BgmVolume * 0.01f;

	internal bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public static NKCBGMInfoTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCBGMInfoTemplet.cs", 39))
		{
			return null;
		}
		NKCBGMInfoTemplet nKCBGMInfoTemplet = new NKCBGMInfoTemplet();
		int num = (int)(1u & (cNKMLua.GetData("IDX", ref nKCBGMInfoTemplet.m_Idx) ? 1u : 0u)) & (cNKMLua.GetData("OrderIdx", ref nKCBGMInfoTemplet.m_OrderIdx) ? 1 : 0);
		cNKMLua.GetData("m_OpenTag", ref nKCBGMInfoTemplet.m_OpenTag);
		int num2 = (int)((uint)num & (cNKMLua.GetData("BgmID", ref nKCBGMInfoTemplet.m_BgmID) ? 1u : 0u) & (cNKMLua.GetData("BgmNameStringID", ref nKCBGMInfoTemplet.m_BgmNameStringID) ? 1u : 0u) & (cNKMLua.GetData("BgmAssetID", ref nKCBGMInfoTemplet.m_BgmAssetID) ? 1u : 0u) & (cNKMLua.GetData("BgmVolume", ref nKCBGMInfoTemplet.m_BgmVolume) ? 1u : 0u)) & (cNKMLua.GetData("BgmCoverIMGID", ref nKCBGMInfoTemplet.m_BgmCoverIMGID) ? 1 : 0);
		cNKMLua.GetData("BgmUnlockStringID", ref nKCBGMInfoTemplet.m_BgmUnlcokStringID);
		cNKMLua.GetData("UnlockCond", ref nKCBGMInfoTemplet.m_UnlockCond);
		nKCBGMInfoTemplet.m_UnlockCondValue1 = new List<int>();
		cNKMLua.GetData("UnlockCondValue1", nKCBGMInfoTemplet.m_UnlockCondValue1);
		cNKMLua.GetData("UnlockCondValue2", out nKCBGMInfoTemplet.m_UnlockCondValue2, 0);
		cNKMLua.GetData("AndTrue", out nKCBGMInfoTemplet.bAllCollecte, defValue: false);
		if (num2 == 0)
		{
			return null;
		}
		return nKCBGMInfoTemplet;
	}

	public static NKCBGMInfoTemplet Find(int bgmID)
	{
		return NKMTempletContainer<NKCBGMInfoTemplet>.Find(bgmID);
	}

	public static NKCBGMInfoTemplet Find(string bgmID)
	{
		return NKMTempletContainer<NKCBGMInfoTemplet>.Find((NKCBGMInfoTemplet x) => x.m_BgmID == bgmID);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_UnlockCond != NKC_BGM_COND.STAGE_CLEAR_TOTAL_CNT && m_UnlockCondValue2 != 0)
		{
			NKMTempletError.Add($"[NKCBGMInfoTemplet:{Key}] {m_UnlockCond}는 m_UnlockCondValue1 : {m_UnlockCondValue2}를 사용하지 않습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCBGMInfoTemplet.cs", 74);
		}
		if (m_UnlockCond != NKC_BGM_COND.COLLECT_ITEM_MISC)
		{
			return;
		}
		foreach (int item in m_UnlockCondValue1)
		{
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMTempletContainer<NKMOfficeInteriorTemplet>.Find(item);
			if (nKMOfficeInteriorTemplet == null)
			{
				NKMTempletError.Add($"[NKCBGMInfoTemplet:{Key}] {m_UnlockCond}는 NKMOfficeInteriorTemplet : {item}이 존재 하지 않습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCBGMInfoTemplet.cs", 83);
			}
		}
	}
}
