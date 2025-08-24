using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Event;

public sealed class NKMEventCollectionTemplet : INKMTemplet
{
	private int eventCollectionGroupId;

	public string openTag;

	public int collectionMergeId;

	private List<NKMEventCollectionDetailTemplet> detailTemplets = new List<NKMEventCollectionDetailTemplet>();

	public int Key => eventCollectionGroupId;

	public bool IsOpen => NKMOpenTagManager.IsOpened(openTag);

	public int CollectionMergeId => collectionMergeId;

	public static IEnumerable<NKMEventCollectionTemplet> Values => NKMTempletContainer<NKMEventCollectionTemplet>.Values;

	public List<NKMEventCollectionDetailTemplet> Details => detailTemplets;

	public static NKMEventCollectionTemplet Find(int groupId)
	{
		return NKMTempletContainer<NKMEventCollectionTemplet>.Find(groupId);
	}

	public static void LoadFromLua()
	{
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_EVENT_COLLECTION_TEMPLET") || !nKMLua.OpenTable("EVENT_COLLECTION_TEMPLET"))
		{
			Log.ErrorAndExit("loading lua file failed. fileName:LUA_EVENT_COLLECTION_TEMPLET", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionTemplet.cs", 32);
		}
		int num = 1;
		while (nKMLua.OpenTable(num++))
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionTemplet.cs", 38))
			{
				nKMLua.CloseTable();
				continue;
			}
			int @int = nKMLua.GetInt32("EventCollectionGroupID");
			if (!NKMTempletContainer<NKMEventCollectionTemplet>.TryGetValue(@int, out var result))
			{
				result = new NKMEventCollectionTemplet();
				result.eventCollectionGroupId = @int;
				nKMLua.GetData("OpenTag", ref result.openTag);
				nKMLua.GetData("CollectionMergeID", ref result.collectionMergeId);
				NKMTempletContainer<NKMEventCollectionTemplet>.Add(result, null);
			}
			NKMEventCollectionDetailTemplet nKMEventCollectionDetailTemplet = new NKMEventCollectionDetailTemplet();
			nKMEventCollectionDetailTemplet.Load(nKMLua);
			result.detailTemplets.Add(nKMEventCollectionDetailTemplet);
			nKMLua.CloseTable();
		}
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (collectionMergeId > 0 && NKMTempletContainer<NKMEventCollectionMergeTemplet>.Find(collectionMergeId) == null)
		{
			NKMTempletError.Add($"[NKMEventCollectionTemplet:{Key}] collectionMergeId를 키로 가진 NKMEventCollectionMergeTemplet가 없음 CollectionMergeID:{collectionMergeId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionTemplet.cs", 73);
		}
		foreach (NKMEventCollectionDetailTemplet detailTemplet in detailTemplets)
		{
			detailTemplet.Validate();
		}
	}
}
