using System;
using SimpleJSON;

namespace NKM;

internal static class JSONNodeEXT
{
	public static T AsEnum<T>(this JSONNode self) where T : struct, Enum
	{
		Enum.TryParse<T>(self.Value, out var result);
		return result;
	}
}
