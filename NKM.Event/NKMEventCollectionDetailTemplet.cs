using System;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Event;

public sealed class NKMEventCollectionDetailTemplet : IComparable<NKMEventCollectionDetailTemplet>
{
	private int collectionGradeGroupId;

	private NKM_UNIT_STYLE_TYPE collectionGoodsType;

	private int collectionGoodsId;

	private string collectionGoodsStrId;

	private bool collectionGoodsEffect;

	private int collectionGoodsRatio;

	public int Key => collectionGoodsId;

	public int CollectionGradeGroupId => collectionGradeGroupId;

	public int CollectionGoodsRatio => collectionGoodsRatio;

	public int CompareTo(NKMEventCollectionDetailTemplet other)
	{
		return collectionGoodsId.CompareTo(other.collectionGoodsId);
	}

	public void Load(NKMLua lua)
	{
		collectionGradeGroupId = lua.GetInt32("CollectionGradeGroupID");
		collectionGoodsId = lua.GetInt32("CollectionGoodsID");
		collectionGoodsStrId = lua.GetString("CollectionGoodsStrID");
		collectionGoodsEffect = lua.GetBoolean("CollectionGoodsEffect");
		collectionGoodsRatio = lua.GetInt32("CollectionGoodsRatio");
		string text = lua.GetString("CollectionGoodsType");
		if (Enum.TryParse<NKM_UNIT_STYLE_TYPE>(text, out var result))
		{
			collectionGoodsType = result;
		}
		else
		{
			NKMTempletError.Add($"[NKMEventCollectionDetailTemplet:{collectionGradeGroupId}:{collectionGoodsId}] CollectionGoodsType 값이 올바르지 않음. collectionGoodsType:{text}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionTemplet.cs", 119);
		}
	}

	public void Validate()
	{
		if (NKMUnitManager.GetUnitTempletBase(collectionGoodsId) == null)
		{
			NKMTempletError.Add($"[NKMEventCollectionDetailTemplet:{collectionGradeGroupId}:{collectionGoodsId}] 대상 유닛 정보를 찾을 수 없음. collectionGoodsId:{collectionGoodsId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionTemplet.cs", 128);
		}
	}
}
