using System;
using System.ComponentModel;

namespace NKC;

public static class EnumsHelperExtension
{
	public static string ToDescription(this Enum value)
	{
		DescriptionAttribute[] array = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
		if (array.Length == 0)
		{
			return value.ToString();
		}
		return array[0].Description;
	}
}
