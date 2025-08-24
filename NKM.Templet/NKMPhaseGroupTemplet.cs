using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMPhaseGroupTemplet : INKMTemplet
{
	private const int MinDungeonCount = 2;

	private readonly List<NKMPhaseOrderTemplet> list = new List<NKMPhaseOrderTemplet>();

	public int Key => GroupId;

	public int GroupId { get; }

	public IReadOnlyList<NKMPhaseOrderTemplet> List => list;

	private NKMPhaseGroupTemplet(int groupId)
	{
		GroupId = groupId;
	}

	public static void LoadFromLua()
	{
		string bundleName = "AB_SCRIPT";
		string text = "LUA_PHASE_ORDER_TEMPLET";
		string text2 = "PHASE_ORDER_TEMPLET";
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath(bundleName, text) || !nKMLua.OpenTable(text2))
		{
			Log.ErrorAndExit("[RandomUnitPool] loading file failed. fileName:" + text + " tablName:" + text2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMPhaseOrderTemplet.cs", 84);
		}
		int num = 1;
		while (nKMLua.OpenTable(num))
		{
			NKMPhaseOrderTemplet nKMPhaseOrderTemplet = NKMPhaseOrderTemplet.LoadFromLua(nKMLua);
			if (nKMPhaseOrderTemplet == null)
			{
				num++;
				nKMLua.CloseTable();
				continue;
			}
			if (!NKMTempletContainer<NKMPhaseGroupTemplet>.TryGetValue(nKMPhaseOrderTemplet.PhaseGroupId, out var result))
			{
				result = new NKMPhaseGroupTemplet(nKMPhaseOrderTemplet.PhaseGroupId);
				NKMTempletContainer<NKMPhaseGroupTemplet>.Add(result, null);
			}
			result.list.Add(nKMPhaseOrderTemplet);
			num++;
			nKMLua.CloseTable();
		}
	}

	public static NKMPhaseGroupTemplet Find(int key)
	{
		return NKMTempletContainer<NKMPhaseGroupTemplet>.Find(key);
	}

	public void Join()
	{
		foreach (NKMPhaseOrderTemplet item in list)
		{
			item.Join(this);
		}
	}

	public void Validate()
	{
		foreach (NKMPhaseOrderTemplet item in list)
		{
			item.Validate();
		}
		if (list.Count < 2)
		{
			NKMTempletError.Add($"[PhaseOrder] 던전 개수가 모자람. groupId:{GroupId} #dungeon:{list.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Stage/NKMPhaseOrderTemplet.cs", 130);
		}
	}
}
