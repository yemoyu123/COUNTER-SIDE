using System.Collections.Generic;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM;

public class NKCRandomMoldTabTemplet : INKMTemplet
{
	public int index;

	public int m_TabOrder;

	public NKM_CRAFT_TAB_TYPE m_MoldTabID;

	public string m_MoldTabName;

	public string m_MoldTabIconName;

	public List<string> m_MoldTab_Filter = new List<string>();

	public List<string> m_MoldTab_Sort = new List<string>();

	private string m_OpenTag;

	public int Key => index;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public static NKCRandomMoldTabTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKCRandomMoldTabTemplet nKCRandomMoldTabTemplet = new NKCRandomMoldTabTemplet();
		int num = (int)(1u & (cNKMLua.GetData("IDX", ref nKCRandomMoldTabTemplet.index) ? 1u : 0u) & (cNKMLua.GetData("m_TabOrder", ref nKCRandomMoldTabTemplet.m_TabOrder) ? 1u : 0u) & (cNKMLua.GetData("m_MoldTabID", ref nKCRandomMoldTabTemplet.m_MoldTabID) ? 1u : 0u) & (cNKMLua.GetData("m_MoldTabName", ref nKCRandomMoldTabTemplet.m_MoldTabName) ? 1u : 0u)) & (cNKMLua.GetData("m_MoldTabIconName", ref nKCRandomMoldTabTemplet.m_MoldTabIconName) ? 1 : 0);
		cNKMLua.GetData("m_OpenTag", ref nKCRandomMoldTabTemplet.m_OpenTag);
		if (num == 0)
		{
			Log.ErrorAndExit($"NKCRandomMoldTabTemplet 정보를 읽어오지 못하였습니다. index : {nKCRandomMoldTabTemplet.index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMItemManagerEx.cs", 279);
		}
		if (cNKMLua.OpenTable("m_MoldTab_Filter"))
		{
			bool flag = true;
			int num2 = 1;
			string rValue = "";
			while (flag)
			{
				flag = cNKMLua.GetData(num2, ref rValue);
				if (flag)
				{
					nKCRandomMoldTabTemplet.m_MoldTab_Filter.Add(rValue);
				}
				num2++;
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_MoldTab_Sort"))
		{
			bool flag2 = true;
			int num3 = 1;
			string rValue2 = "";
			while (flag2)
			{
				flag2 = cNKMLua.GetData(num3, ref rValue2);
				if (flag2)
				{
					nKCRandomMoldTabTemplet.m_MoldTab_Sort.Add(rValue2);
				}
				num3++;
			}
			cNKMLua.CloseTable();
		}
		nKCRandomMoldTabTemplet.CheckValidation();
		return nKCRandomMoldTabTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	private void CheckValidation()
	{
		for (int i = 0; i < m_MoldTab_Sort.Count; i++)
		{
			if (!NKCMoldSortSystem.MoldSortData.ContainsKey(m_MoldTab_Sort[i]))
			{
				Log.ErrorAndExit($"[NKCRandomMoldTabTemplet] Mold Sorting 정보가 없습니다. sortCnt : {i}, sort name : {m_MoldTab_Sort[i]}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMItemManagerEx.cs", 337);
			}
		}
		for (int j = 0; j < m_MoldTab_Filter.Count; j++)
		{
			if (!NKCMoldSortSystem.MoldFilterData.Contains(m_MoldTab_Filter[j]))
			{
				Log.ErrorAndExit($"[NKCRandomMoldTabTemplet] Mold Filter 정보가 없습니다. filterCnt : {j}, sort name : {m_MoldTab_Filter[j]}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMItemManagerEx.cs", 343);
			}
		}
	}
}
