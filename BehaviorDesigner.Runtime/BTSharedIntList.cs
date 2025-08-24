using System;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime;

[Serializable]
public class BTSharedIntList : BTSharedNKCValue<List<int>>
{
	private readonly char[] defaultSeperator = new char[4] { '\n', ' ', '\t', ',' };

	public static implicit operator BTSharedIntList(List<int> value)
	{
		return new BTSharedIntList
		{
			Value = value
		};
	}

	public override bool TryParse(string parameters)
	{
		return TryParse(parameters, defaultSeperator);
	}

	public override string ToString()
	{
		return BTSharedNKCValue.ToDebugString(base.Value);
	}

	public bool TryParse(string parameters, char[] seperator)
	{
		string[] array = parameters.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
		List<int> list = new List<int>();
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			if (int.TryParse(array2[i], out var result))
			{
				list.Add(result);
				continue;
			}
			return false;
		}
		base.Value = list;
		return true;
	}
}
