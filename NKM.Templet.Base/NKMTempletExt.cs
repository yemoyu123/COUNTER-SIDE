using System.Collections.Generic;

namespace NKM.Templet.Base;

public static class NKMTempletExt
{
	public static void AddToContainer<T>(this IEnumerable<T> templets) where T : class, INKMTemplet
	{
		NKMTempletContainer<T>.AddRange(templets, null);
	}
}
