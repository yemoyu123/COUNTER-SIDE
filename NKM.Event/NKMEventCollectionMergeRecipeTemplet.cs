using System;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Event;

public sealed class NKMEventCollectionMergeRecipeTemplet : IComparable<NKMEventCollectionMergeRecipeTemplet>
{
	private int mergeRecipeGroupId;

	private string mergeInputGrade;

	private int mergeInputGradeGroupId;

	private int mergeInputValue;

	private NKM_UNIT_GRADE mergeInputUnitGrade;

	private int mergeOutputGradeGroupId;

	private int mergeOutputValue;

	private string mergeOutputIconBundleName;

	private string mergeOutputIconAssetName;

	public int MergeRecipeGroupId => mergeRecipeGroupId;

	public int MergeInputGradeGroupId => mergeInputGradeGroupId;

	public int MergeInputValue => mergeInputValue;

	public int MergeOutputGradeGroupId => mergeOutputGradeGroupId;

	public int MergeOutputValue => mergeOutputValue;

	public NKM_UNIT_GRADE MergeInputUnitGrade => mergeInputUnitGrade;

	public string MergeOutputBundleName => mergeOutputIconBundleName;

	public string MergeOutputAssetName => mergeOutputIconAssetName;

	public int CompareTo(NKMEventCollectionMergeRecipeTemplet other)
	{
		return mergeInputGradeGroupId.CompareTo(other.mergeInputGradeGroupId);
	}

	public void Load(NKMLua lua)
	{
		mergeRecipeGroupId = lua.GetInt32("MergeRecipeGroupID");
		mergeInputGrade = lua.GetString("MergeInputGrade");
		mergeInputGradeGroupId = lua.GetInt32("MergeInputGradeGroupID");
		mergeInputValue = lua.GetInt32("MergeInputValue");
		mergeOutputGradeGroupId = lua.GetInt32("MergeOutputGradeGroupID");
		mergeOutputValue = lua.GetInt32("MergeOutputValue");
		mergeOutputIconBundleName = lua.GetString("MergeOutputIconBundleName");
		mergeOutputIconAssetName = lua.GetString("MergeOutputIconAssetName");
	}

	public void Join(int ownerMergeId)
	{
		if (Enum.TryParse<NKM_UNIT_GRADE>(mergeInputGrade, out var result))
		{
			mergeInputUnitGrade = result;
		}
		else
		{
			NKMTempletError.Add($"[NKMEventCollectionMergeTemplet:{ownerMergeId}:{mergeRecipeGroupId}] 소비 등급의 대상이 올바르지 않음. mergeInputGrade:{mergeInputGrade}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionMergeTemplet.cs", 125);
		}
	}

	public void Validate(int ownerMergeId)
	{
		if (mergeInputValue < 1)
		{
			NKMTempletError.Add($"[NKMEventCollectionMergeTemplet:{ownerMergeId}:{mergeRecipeGroupId}] MergeInputValue에 올바르지 않은 값이 입력됨. mergeInputValue:{mergeInputValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionMergeTemplet.cs", 133);
		}
		if (mergeOutputValue < 1)
		{
			NKMTempletError.Add($"[NKMEventCollectionMergeTemplet:{ownerMergeId}:{mergeRecipeGroupId}] MergeOutputValue에 올바르지 않은 값이 입력됨. mergeOutputValue:{mergeOutputValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionMergeTemplet.cs", 138);
		}
	}
}
