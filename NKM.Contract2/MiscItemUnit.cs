using System.Runtime.CompilerServices;
using NKM.Templet.Base;

namespace NKM.Contract2;

public sealed class MiscItemUnit
{
	public int ItemId { get; }

	public long Count { get; }

	public int Count32 => (int)Count;

	public NKMItemMiscTemplet Templet { get; internal set; }

	public MiscItemUnit(int itemId, long count)
	{
		ItemId = itemId;
		Count = count;
	}

	public void Join([CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		Templet = NKMItemManager.GetItemMiscTempletByID(ItemId);
		if (Templet == null)
		{
			NKMTempletError.Add($"[MiscItemUnit] invalid miscItemId:{ItemId}", file, line);
		}
		if (Count <= 0)
		{
			NKMTempletError.Add($"[MiscItemUnit] invalid itemCount:{Count}", file, line);
		}
	}

	public override string ToString()
	{
		return $"itemId:{ItemId} itemCount:{Count}";
	}
}
