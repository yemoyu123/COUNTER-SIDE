using System;
using NKM;
using NKM.Templet.Base;

namespace NKC;

public class NKCNewsTemplet : INKMTemplet
{
	public int Idx;

	public int m_Order;

	public DateTime m_DateStart;

	public DateTime m_DateEnd;

	public bool m_bDateAlert;

	public eNewsFilterType m_FilterType;

	public string m_TabImage;

	public string m_Title;

	public string m_Contents;

	public string m_BannerImage;

	public NKM_SHORTCUT_TYPE m_ShortCutType;

	public string m_ShortCut;

	public int Key => Idx;

	public DateTime m_DateStartUtc => NKMTime.LocalToUTC(m_DateStart);

	public DateTime m_DateEndUtc => NKMTime.LocalToUTC(m_DateEnd);

	public static NKCNewsTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCNewsManager.cs", 39))
		{
			return null;
		}
		NKCNewsTemplet nKCNewsTemplet = new NKCNewsTemplet();
		int num = (int)(1u & (cNKMLua.GetData("Idx", ref nKCNewsTemplet.Idx) ? 1u : 0u)) & (cNKMLua.GetData("m_Order", ref nKCNewsTemplet.m_Order) ? 1 : 0);
		cNKMLua.GetData("m_DateStart", ref nKCNewsTemplet.m_DateStart);
		cNKMLua.GetData("m_DateEnd", ref nKCNewsTemplet.m_DateEnd);
		int num2 = (int)((uint)num & (cNKMLua.GetData("m_bDateAlert", ref nKCNewsTemplet.m_bDateAlert) ? 1u : 0u) & (cNKMLua.GetData("m_FliterType", ref nKCNewsTemplet.m_FilterType) ? 1u : 0u)) & (cNKMLua.GetData("m_TabImage", ref nKCNewsTemplet.m_TabImage) ? 1 : 0);
		cNKMLua.GetData("m_Title", ref nKCNewsTemplet.m_Title);
		cNKMLua.GetData("m_Contents", ref nKCNewsTemplet.m_Contents);
		cNKMLua.GetData("m_BannerImage", ref nKCNewsTemplet.m_BannerImage);
		cNKMLua.GetData("m_ShortCutType", ref nKCNewsTemplet.m_ShortCutType);
		cNKMLua.GetData("m_ShortCut", ref nKCNewsTemplet.m_ShortCut);
		if (num2 == 0)
		{
			return null;
		}
		return nKCNewsTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
