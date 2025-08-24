using System;

namespace Cs.GameLog.CountryDescription;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public sealed class CountryDescriptionAttribute : Attribute
{
	public string Description { get; }

	public CountryCode CountryCode { get; }

	public CountryDescriptionAttribute(string desc, CountryCode code = CountryCode.KOR)
	{
		Description = desc;
		CountryCode = code;
	}
}
