using System;
using NKC.StringSearch;
using NKM;
using NKM.Templet;

namespace NKC.Sort;

public static class NKCFilterTokensUtil
{
	public static readonly char[] Seperator = new char[6] { ',', '/', ' ', '\n', '\t', '\r' };

	public static NKM_UNIT_GRADE ParseGrade(string value)
	{
		if (value.Equals("SSR", StringComparison.InvariantCultureIgnoreCase))
		{
			return NKM_UNIT_GRADE.NUG_SSR;
		}
		if (value.Equals("SR", StringComparison.InvariantCultureIgnoreCase))
		{
			return NKM_UNIT_GRADE.NUG_SR;
		}
		if (value.Equals("R", StringComparison.InvariantCultureIgnoreCase))
		{
			return NKM_UNIT_GRADE.NUG_R;
		}
		if (value.Equals("N", StringComparison.InvariantCultureIgnoreCase))
		{
			return NKM_UNIT_GRADE.NUG_N;
		}
		if (NKCStringTable.NationalCode == NKM_NATIONAL_CODE.NNC_SIMPLIFIED_CHINESE)
		{
			if (NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_SSR").Equals(value))
			{
				return NKM_UNIT_GRADE.NUG_SSR;
			}
			if (NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_SR").Equals(value))
			{
				return NKM_UNIT_GRADE.NUG_SR;
			}
			if (NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_R").Equals(value))
			{
				return NKM_UNIT_GRADE.NUG_R;
			}
			if (NKCStringTable.GetString("SI_DP_UNIT_GRADE_STRING_NUG_N").Equals(value))
			{
				return NKM_UNIT_GRADE.NUG_N;
			}
		}
		return NKM_UNIT_GRADE.NUG_COUNT;
	}

	public static NKM_UNIT_STYLE_TYPE ParseStyle(string value)
	{
		if (value.Equals("C", StringComparison.InvariantCultureIgnoreCase) || value.Equals("CO", StringComparison.InvariantCultureIgnoreCase))
		{
			return NKM_UNIT_STYLE_TYPE.NUST_COUNTER;
		}
		if (value.Equals("S", StringComparison.InvariantCultureIgnoreCase) || value.Equals("SO", StringComparison.InvariantCultureIgnoreCase))
		{
			return NKM_UNIT_STYLE_TYPE.NUST_SOLDIER;
		}
		if (value.Equals("M", StringComparison.InvariantCultureIgnoreCase) || value.Equals("ME", StringComparison.InvariantCultureIgnoreCase))
		{
			return NKM_UNIT_STYLE_TYPE.NUST_MECHANIC;
		}
		foreach (NKM_UNIT_STYLE_TYPE value2 in Enum.GetValues(typeof(NKM_UNIT_STYLE_TYPE)))
		{
			string unitStyleName = NKCUtilString.GetUnitStyleName(value2);
			if (!string.IsNullOrEmpty(unitStyleName) && NKCStringSearchTool.StartsWith(unitStyleName, value))
			{
				return value2;
			}
		}
		return NKM_UNIT_STYLE_TYPE.NUST_INVALID;
	}

	public static NKM_UNIT_ROLE_TYPE ParseRole(string value)
	{
		foreach (NKM_UNIT_ROLE_TYPE value2 in Enum.GetValues(typeof(NKM_UNIT_ROLE_TYPE)))
		{
			if (value2 != NKM_UNIT_ROLE_TYPE.NURT_INVALID)
			{
				string roleText = NKCUtilString.GetRoleText(value2, bAwaken: false);
				if (!string.IsNullOrEmpty(roleText) && NKCStringSearchTool.StartsWith(roleText, value))
				{
					return value2;
				}
			}
		}
		return NKM_UNIT_ROLE_TYPE.NURT_INVALID;
	}

	public static NKM_ITEM_GRADE ParseGradeItem(string value)
	{
		if (value.Equals("SSR", StringComparison.InvariantCultureIgnoreCase))
		{
			return NKM_ITEM_GRADE.NIG_SSR;
		}
		if (value.Equals("SR", StringComparison.InvariantCultureIgnoreCase))
		{
			return NKM_ITEM_GRADE.NIG_SR;
		}
		if (value.Equals("R", StringComparison.InvariantCultureIgnoreCase))
		{
			return NKM_ITEM_GRADE.NIG_R;
		}
		if (value.Equals("N", StringComparison.InvariantCultureIgnoreCase))
		{
			return NKM_ITEM_GRADE.NIG_R;
		}
		return NKM_ITEM_GRADE.NIG_COUNT;
	}
}
