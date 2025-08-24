using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMPotentialOptionGroupTemplet : INKMTemplet
{
	private const string OpenTag = "EQUIP_POTENTIAL";

	private readonly int groupId;

	private readonly List<NKMPotentialOptionTemplet> optionList;

	public static bool EnableByTag => NKMOpenTagManager.IsOpened("EQUIP_POTENTIAL");

	public static IEnumerable<NKMPotentialOptionGroupTemplet> Values => NKMTempletContainer<NKMPotentialOptionGroupTemplet>.Values;

	public int Key => groupId;

	public IReadOnlyList<NKMPotentialOptionTemplet> OptionList => optionList;

	private NKMPotentialOptionGroupTemplet(IGrouping<int, NKMPotentialOptionTemplet> group)
	{
		groupId = group.Key;
		optionList = group.ToList();
	}

	public static void LoadFromLua()
	{
		if (NKMUtil.IsServer)
		{
			(from e in NKMTempletLoader.LoadServerPath(string.Empty, "LUA_ITEM_EQUIP_POTENTIAL_OPTION", "ITEM_EQUIP_POTENTIAL_OPTION", NKMPotentialOptionTemplet.LoadFromLUA)
				group e by e.groupId into e
				select new NKMPotentialOptionGroupTemplet(e)).AddToContainer();
		}
		else
		{
			(from e in NKMTempletLoader.LoadCommonPath("AB_SCRIPT", "LUA_ITEM_EQUIP_POTENTIAL_OPTION", "ITEM_EQUIP_POTENTIAL_OPTION", NKMPotentialOptionTemplet.LoadFromLUA)
				group e by e.groupId into e
				select new NKMPotentialOptionGroupTemplet(e)).AddToContainer();
		}
	}

	public static NKMPotentialOptionGroupTemplet Find(int key)
	{
		return NKMTempletContainer<NKMPotentialOptionGroupTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (!EnableByTag)
		{
			return;
		}
		if (!optionList.Any())
		{
			NKMTempletError.Add("[NKMPotentialOptionGroup] OptionList is Empty", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMPotentialOptionGroupTemplet.cs", 60);
		}
		foreach (NKMPotentialOptionTemplet option in optionList)
		{
			option.Validate();
		}
	}
}
