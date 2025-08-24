using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Event;

public sealed class NKMEventCollectionMergeTemplet : INKMTemplet
{
	private int collectionMergeId;

	private readonly List<NKMEventCollectionMergeRecipeTemplet> recipeTemplets = new List<NKMEventCollectionMergeRecipeTemplet>();

	public int Key => collectionMergeId;

	public List<NKMEventCollectionMergeRecipeTemplet> RecipeTemplets => recipeTemplets;

	public NKMEventCollectionMergeRecipeTemplet GetRecipeTemplet(int recipeGroupId)
	{
		return recipeTemplets.FirstOrDefault((NKMEventCollectionMergeRecipeTemplet e) => recipeGroupId == e.MergeRecipeGroupId);
	}

	public static void LoadFromLua()
	{
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_EVENT_COLLECTION_MERGE_TEMPLET") || !nKMLua.OpenTable("EVENT_COLLECTION_MERGE_TEMPLET"))
		{
			Log.ErrorAndExit("loading lua file failed. fileName:LUA_EVENT_COLLECTION_MERGE_TEMPLET", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionMergeTemplet.cs", 29);
		}
		int num = 1;
		while (nKMLua.OpenTable(num++))
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionMergeTemplet.cs", 35))
			{
				nKMLua.CloseTable();
				continue;
			}
			int @int = nKMLua.GetInt32("CollectionMergeID");
			if (!NKMTempletContainer<NKMEventCollectionMergeTemplet>.TryGetValue(@int, out var result))
			{
				result = new NKMEventCollectionMergeTemplet
				{
					collectionMergeId = @int
				};
				NKMTempletContainer<NKMEventCollectionMergeTemplet>.Add(result, null);
			}
			NKMEventCollectionMergeRecipeTemplet nKMEventCollectionMergeRecipeTemplet = new NKMEventCollectionMergeRecipeTemplet();
			nKMEventCollectionMergeRecipeTemplet.Load(nKMLua);
			result.recipeTemplets.Add(nKMEventCollectionMergeRecipeTemplet);
			nKMLua.CloseTable();
		}
	}

	public void Join()
	{
		foreach (NKMEventCollectionMergeRecipeTemplet recipeTemplet in recipeTemplets)
		{
			recipeTemplet.Join(Key);
		}
	}

	public void Validate()
	{
		foreach (NKMEventCollectionMergeRecipeTemplet recipeTemplet in recipeTemplets)
		{
			recipeTemplet.Validate(Key);
		}
	}
}
