using System.Collections.Generic;

namespace NLua.Extensions;

internal static class StringExtensions
{
	public static IEnumerable<string> SplitWithEscape(this string input, char separator, char escapeCharacter)
	{
		int num = 0;
		int index = 0;
		while (index < input.Length)
		{
			index = input.IndexOf(separator, index);
			if (index == -1)
			{
				break;
			}
			if (input[index - 1] == escapeCharacter)
			{
				input = input.Remove(index - 1, 1);
				continue;
			}
			yield return input.Substring(num, index - num);
			index++;
			num = index;
		}
		yield return input.Substring(num);
	}
}
