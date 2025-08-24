using System;
using System.Collections.Generic;
using Cs.Core.Util;
using Cs.Logging;
using NKM.Contract2;
using NKM.Templet.Base;

namespace NKM;

public static class NKMOpenTagManager
{
	private static readonly HashSet<string> openTagSet;

	private static readonly string[] SystemOpenTag;

	private static Dictionary<string, Action> recalculateActions;

	public static IEnumerable<string> Tags => openTagSet;

	public static int TagCount => openTagSet.Count;

	static NKMOpenTagManager()
	{
		openTagSet = new HashSet<string>();
		SystemOpenTag = new string[EnumUtil<SystemOpenTagType>.Count];
		recalculateActions = new Dictionary<string, Action>();
		for (int i = 0; i < SystemOpenTag.Length; i++)
		{
			string[] systemOpenTag = SystemOpenTag;
			int num = i;
			SystemOpenTagType systemOpenTagType = (SystemOpenTagType)i;
			systemOpenTag[num] = systemOpenTagType.ToString();
		}
	}

	public static bool TryAddTag(string tag)
	{
		if (string.IsNullOrEmpty(tag))
		{
			throw new ArgumentException("[OpenTag] tag:" + tag);
		}
		return openTagSet.Add(tag);
	}

	public static void ClearOpenTag()
	{
		openTagSet.Clear();
	}

	public static void SetTagList(IReadOnlyList<string> openTagList)
	{
		Log.Debug("[NKMOpenTagManager] SetTagList", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMOpenTagManager.cs", 139);
		ClearOpenTag();
		foreach (string openTag in openTagList)
		{
			TryAddTag(openTag);
		}
		RecalculateTemplets();
	}

	public static void AddRecalculateAction(string descKey, Action action)
	{
		if (!recalculateActions.ContainsKey(descKey))
		{
			recalculateActions.Add(descKey, action);
		}
	}

	public static void RecalculateTemplets()
	{
		foreach (Action value in recalculateActions.Values)
		{
			value?.Invoke();
		}
		foreach (RandomUnitPoolTempletV2 value2 in NKMTempletContainer<RandomUnitPoolTempletV2>.Values)
		{
			value2.RecalculateUnitTemplets();
		}
		foreach (ContractTempletV2 value3 in ContractTempletV2.Values)
		{
			value3.RecalculateIntrusivePool();
		}
	}

	public static bool IsOpened(string openTagID)
	{
		if (string.IsNullOrWhiteSpace(openTagID))
		{
			return true;
		}
		return openTagSet.Contains(openTagID);
	}

	public static bool IsSystemOpened(SystemOpenTagType tagType)
	{
		return IsOpened(SystemOpenTag[(int)tagType]);
	}

	public static void Drop()
	{
		ClearOpenTag();
		recalculateActions.Clear();
	}
}
