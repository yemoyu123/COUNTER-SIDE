using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Item;

public sealed class NKMCustomPackageGroupTemplet : INKMTemplet
{
	public const int MaxElementCount = 10;

	private readonly NKMCustomPackageElement[] elements;

	public int Key { get; }

	public IReadOnlyList<NKMCustomPackageElement> Elements => elements;

	public IEnumerable<NKMCustomPackageElement> OpenedElements
	{
		get
		{
			NKMCustomPackageElement[] array = elements;
			foreach (NKMCustomPackageElement nKMCustomPackageElement in array)
			{
				if (nKMCustomPackageElement.EnableByTag)
				{
					yield return nKMCustomPackageElement;
				}
			}
		}
	}

	public NKMCustomPackageGroupTemplet(int groupId, IEnumerable<NKMCustomPackageElement> elements)
	{
		this.elements = elements.OrderBy((NKMCustomPackageElement e) => e.Index).ToArray();
		Key = groupId;
	}

	public static NKMCustomPackageGroupTemplet Find(int groupId)
	{
		return NKMTempletContainer<NKMCustomPackageGroupTemplet>.Find(groupId);
	}

	public static void LoadFromLua()
	{
		NKMTempletContainer<NKMCustomPackageGroupTemplet>.Load(from e in NKMTempletLoader<NKMCustomPackageElement>.LoadGroup("AB_SCRIPT", "LUA_CUSTOM_PACKAGE_ITEM_BOX", "CUSTOM_PACKAGE_ITEM_BOX", NKMCustomPackageElement.LoadFromLUA)
			select new NKMCustomPackageGroupTemplet(e.Key, e.Value), null);
	}

	public NKMCustomPackageElement Get(int index)
	{
		if (index < 0 || index >= elements.Length)
		{
			return null;
		}
		return elements[index];
	}

	public void Join()
	{
		NKMCustomPackageElement[] array = elements;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Join();
		}
	}

	public void Validate()
	{
		for (int i = 0; i < elements.Length; i++)
		{
			NKMCustomPackageElement nKMCustomPackageElement = elements[i];
			nKMCustomPackageElement.Validate();
			if (nKMCustomPackageElement.Index != i)
			{
				NKMTempletError.Add($"[CustomPackageGroup] index 값이 올바르지 않음. groupId:{Key} index(value):{nKMCustomPackageElement.Index} index(position):{i}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMCustomPackageGroupTemplet.cs", 70);
			}
		}
		if (!OpenedElements.Any())
		{
			NKMTempletError.Add($"[CustomPackageGroup] 유효한 element가 없음. groupId:{Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMCustomPackageGroupTemplet.cs", 76);
		}
	}
}
